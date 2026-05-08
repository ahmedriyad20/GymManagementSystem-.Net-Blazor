using Microsoft.AspNetCore.Http;

namespace GymManagementSystem.Interfaces
{
    public interface IStorageService
    {
        Task<string> UploadAsync(IFormFile file, string location, bool withBaseUrl = false);
        bool Delete(string fileUrlOrPath);
        string BuildFileUrl(string relativePath);
    }
}
