using System.Buffers.Binary;
using System.IO;
using System.Text;
using CSharpFunctionalExtensions;
using GestionAcademica.Config;
using GestionAcademica.Dto.Personas;
using GestionAcademica.Errors.Common;
using GestionAcademica.Errors.Storage;
using GestionAcademica.Mappers.Personas;
using GestionAcademica.Models.Personas;
using Serilog;

namespace GestionAcademica.Storage.Binary;

/// <summary>
///     Almacén binario que usa Stream.WriteAsync/ReadAsync para I/O verdaderamente asíncrono.
///     BinaryWriter/BinaryReader no tienen API async en .NET, así que escribimos
///     los primitivos directamente en el Stream usando BitConverter + async.
/// </summary>
public class AcademiaBinStorage : IAcademiaBinStorage {
    private readonly ILogger _logger = Log.ForContext<AcademiaBinStorage>();

    public AcademiaBinStorage() {
        _logger.Debug("Inicializando la clase AcademiaBinStorage");
        InitStorage();
    }

    public async Task<Result<bool, DomainError>> SalvarAsync(IEnumerable<Persona> items, string path) {
        try {
            _logger.Debug("Guardando los items en el archivo binario '{path}'", path);
            await using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true);

            var dtos = items.Select(p => p.ToDto()).ToList();
            await WriteIntAsync(stream, dtos.Count);

            foreach (var dto in dtos) {
                await WriteIntAsync(stream, dto.Id);
                await WriteStringAsync(stream, dto.Dni);
                await WriteStringAsync(stream, dto.Nombre);
                await WriteStringAsync(stream, dto.Apellidos);
                await WriteStringAsync(stream, dto.FechaNacimiento);
                await WriteStringAsync(stream, dto.Email);
                await WriteStringAsync(stream, dto.Imagen ?? "");
                await WriteStringAsync(stream, dto.Tipo);
                await WriteStringAsync(stream, dto.Experiencia ?? "");
                await WriteStringAsync(stream, dto.Especialidad ?? "");
                await WriteStringAsync(stream, dto.Ciclo);
                await WriteStringAsync(stream, dto.Curso ?? "");
                await WriteStringAsync(stream, dto.Calificacion ?? "");
                await WriteStringAsync(stream, dto.CreatedAt);
                await WriteStringAsync(stream, dto.UpdatedAt);
                await WriteBoolAsync(stream, dto.IsDeleted);
                await WriteStringAsync(stream, dto.DeletedAt ?? "");
            }

            return Result.Success<bool, DomainError>(true);
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al guardar los items en el archivo binario '{path}'", path);
            return Result.Failure<bool, DomainError>(StorageErrors.WriteError(ex.Message));
        }
    }

    public async Task<Result<IEnumerable<Persona>, DomainError>> CargarAsync(string path) {
        _logger.Debug("Cargando los items del archivo binario '{path}'", path);

        if (!File.Exists(path)) {
            _logger.Warning("El archivo '{path}' no existe.", path);
            return Result.Failure<IEnumerable<Persona>, DomainError>(StorageErrors.FileNotFound(path));
        }

        try {
            await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 4096, useAsync: true);

            var count = await ReadIntAsync(stream);
            var personas = new List<Persona>();

            for (var i = 0; i < count; i++) {
                var dto = new PersonaDto(
                    await ReadIntAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadNullableStringAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadStringAsync(stream),
                    await ReadBoolAsync(stream),
                    await ReadNullableStringAsync(stream)
                );
                personas.Add(dto.ToModel());
            }

            return Result.Success<IEnumerable<Persona>, DomainError>(personas);
        }
        catch (Exception ex) {
            _logger.Error(ex, "Error al cargar los items del archivo binario '{path}'", path);
            return Result.Failure<IEnumerable<Persona>, DomainError>(StorageErrors.InvalidFormat(ex.Message));
        }
    }

    // ── Helpers de escritura asíncrona ─────────────────────────────────────

    private static async Task WriteIntAsync(Stream stream, int value) {
        var buffer = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
        await stream.WriteAsync(buffer);
    }

    private static async Task WriteBoolAsync(Stream stream, bool value) {
        var buffer = new byte[] { value ? (byte)1 : (byte)0 };
        await stream.WriteAsync(buffer);
    }

    private static async Task WriteStringAsync(Stream stream, string value) {
        var bytes = Encoding.UTF8.GetBytes(value);
        await Write7BitEncodedIntAsync(stream, bytes.Length);
        if (bytes.Length > 0)
            await stream.WriteAsync(bytes);
    }

    private static async Task Write7BitEncodedIntAsync(Stream stream, int value) {
        var buffer = new byte[5];
        var idx = 0;
        var v = (uint)value;
        while (v > 0x7F) {
            buffer[idx++] = (byte)(v | 0x80);
            v >>= 7;
        }
        buffer[idx++] = (byte)v;
        await stream.WriteAsync(buffer.AsMemory(0, idx));
    }

    // ── Helpers de lectura asíncrona ───────────────────────────────────────

    private static async Task<int> ReadIntAsync(Stream stream) {
        var buffer = new byte[4];
        await ReadExactAsync(stream, buffer);
        return BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    private static async Task<bool> ReadBoolAsync(Stream stream) {
        var buffer = new byte[1];
        await ReadExactAsync(stream, buffer);
        return buffer[0] != 0;
    }

    private static async Task<string> ReadStringAsync(Stream stream) {
        var length = await Read7BitEncodedIntAsync(stream);
        if (length == 0) return string.Empty;
        var buffer = new byte[length];
        await ReadExactAsync(stream, buffer);
        return Encoding.UTF8.GetString(buffer);
    }

    private static async Task<string?> ReadNullableStringAsync(Stream stream) {
        var length = await Read7BitEncodedIntAsync(stream);
        if (length == 0) return null;
        var buffer = new byte[length];
        await ReadExactAsync(stream, buffer);
        return Encoding.UTF8.GetString(buffer);
    }

    private static async Task<int> Read7BitEncodedIntAsync(Stream stream) {
        var result = 0;
        var shift = 0;
        byte b;
        do {
            var buffer = new byte[1];
            await ReadExactAsync(stream, buffer);
            b = buffer[0];
            result |= (b & 0x7F) << shift;
            shift += 7;
        } while ((b & 0x80) != 0);
        return result;
    }

    private static async Task ReadExactAsync(Stream stream, byte[] buffer) {
        var totalRead = 0;
        while (totalRead < buffer.Length) {
            var read = await stream.ReadAsync(buffer.AsMemory(totalRead));
            if (read == 0)
                throw new EndOfStreamException();
            totalRead += read;
        }
    }

    private void InitStorage() {
        if (Directory.Exists(AppConfig.DataFolder))
            return;
        _logger.Debug("El directorio 'data' no existe. Creándolo...");
        Directory.CreateDirectory(AppConfig.DataFolder);
    }
}
