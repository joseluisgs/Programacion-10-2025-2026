// ============================================================
// ProductoMapper.cs - Mapper entre Entity y Model
// ============================================================
// Convierte entre ProductoEntity (BD) y Producto (dominio).
//
// CONCEPTOS IMPORTANTES:
//
// 1. MAPPER:
//    - Clase estática para conversión bidireccional
//    - ToEntity: Model -> Entity
//    - ToModel: Entity -> Model
//    - ToModel(IEnumerable): Convierte colecciones
//
// 2. USO CON DAPPER:
//    - Dapper mapea resultados SQL a Entity
//    - Repository convierte Entity a Model
//    - Separa la lógica de acceso a datos

using System.Collections.Generic;
using System.Linq;
using ListaCompra.Dto;
using ListaCompra.Entity;
using ListaCompra.Models;

namespace ListaCompra.Mappers;

public static class ProductoMapper
{
    public static ProductoEntity ToEntity(Producto model)
    {
        return new ProductoEntity
        {
            Id = model.Id,
            Nombre = model.Nombre,
            Cantidad = model.Cantidad,
            Precio = model.Precio,
            EstaComprado = model.EstaComprado
        };
    }

    public static Producto ToModel(ProductoEntity? entity)
    {
        if (entity == null) return null!;
        
        return new Producto(
            entity.Id,
            entity.Nombre,
            entity.Cantidad,
            entity.Precio,
            entity.EstaComprado
        );
    }

    public static IEnumerable<Producto> ToModel(IEnumerable<ProductoEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public static ProductoDto ToDto(Producto model)
    {
        return new ProductoDto(
            model.Id,
            model.Nombre,
            model.Cantidad,
            model.Precio,
            model.EstaComprado
        );
    }

    public static Producto ToModel(ProductoDto? dto)
    {
        if (dto == null) return null!;
        
        return new Producto(
            dto.Id,
            dto.Nombre,
            dto.Cantidad,
            dto.Precio,
            dto.EstaComprado
        );
    }

    public static IEnumerable<Producto> ToModel(IEnumerable<ProductoDto> dtos)
    {
        return dtos.Select(ToModel);
    }
}
