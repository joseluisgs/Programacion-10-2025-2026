using CSharpFunctionalExtensions;
using GestionAcademica.Errors.Common;

namespace GestionAcademica.Services.Images;

public interface IImageService {
    Task<Result<string, DomainError>> SaveImageAsync(string sourcePath);
    Task<Result<bool, DomainError>> DeleteImageAsync(string fileName);
    Task<Result<bool, DomainError>> UpdateImageAsync(string sourcePath, string existingFileName);
    Task<bool> IsValidImageAsync(string sourcePath);
    Task<Result<bool, DomainError>> ValidateImageSizeAsync(string sourcePath, long maxSizeInBytes = 5_242_880);
    Task<Result<bool, DomainError>> ValidateImageDimensionsAsync(string sourcePath, int maxWidth = 4096, int maxHeight = 4096);
    Task<Result<string, DomainError>> CreatePreviewAsync(string sourcePath, int maxWidth = 300, int maxHeight = 300);
}
