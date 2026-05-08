using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace GymManagementSystem.Storage
{
    public static class StorageExtensions
    {
        public static string GetNewName()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string GetBaseUrl(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            var configuredBaseUrl = configuration["Storage:BaseUrl"];
            if (!string.IsNullOrWhiteSpace(configuredBaseUrl))
            {
                return configuredBaseUrl.TrimEnd('/');
            }

            var request = httpContextAccessor.HttpContext?.Request;
            if (request is null)
            {
                return string.Empty;
            }

            return $"{request.Scheme}://{request.Host}{request.PathBase}".TrimEnd('/');
        }

        public static string NormalizeRelativePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            var normalized = path.Replace("\\", "/");
            return normalized.StartsWith('/') ? normalized : "/" + normalized;
        }
    }
}
