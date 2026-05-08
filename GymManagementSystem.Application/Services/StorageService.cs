using GymManagementSystem.Interfaces;
using GymManagementSystem.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace GymManagementSystem.Services
{
    public class StorageService(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration) : IStorageService
    {
        public async Task<string> UploadAsync(IFormFile file, string location, bool withBaseUrl = false)
        {
            if (file is null || file.Length == 0)
            {
                throw new InvalidOperationException("File is required.");
            }

            var extension = Path.GetExtension(file.FileName);
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (string.IsNullOrWhiteSpace(extension) || !allowed.Contains(extension.ToLowerInvariant()))
            {
                throw new InvalidOperationException("Only jpg, jpeg, png and webp files are allowed.");
            }

            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", location);
            if (!Directory.Exists(uploadsRoot))
            {
                Directory.CreateDirectory(uploadsRoot);
            }

            var fileName = $"{StorageExtensions.GetNewName()}{extension.ToLowerInvariant()}";
            var fullPath = Path.Combine(uploadsRoot, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/{location}/{fileName}";
            return withBaseUrl ? BuildFileUrl(relativePath) : relativePath;
        }

        public bool Delete(string fileUrlOrPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileUrlOrPath))
                {
                    return false;
                }

                var relativePath = ExtractRelativePath(fileUrlOrPath);
                if (string.IsNullOrWhiteSpace(relativePath))
                {
                    return false;
                }

                var localPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (!File.Exists(localPath))
                {
                    return false;
                }

                File.Delete(localPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string BuildFileUrl(string relativePath)
        {
            var normalized = StorageExtensions.NormalizeRelativePath(relativePath);
            var baseUrl = StorageExtensions.GetBaseUrl(httpContextAccessor, configuration);
            return string.IsNullOrWhiteSpace(baseUrl) ? normalized : $"{baseUrl}{normalized}";
        }

        private static string ExtractRelativePath(string value)
        {
            if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
                {
                    return string.Empty;
                }

                return uri.AbsolutePath;
            }

            return StorageExtensions.NormalizeRelativePath(value);
        }
    }
}
