using TheBoringChat.Enums;

namespace TheBoringChat.Helpers;

public static class StaticDataHelper
{
    public static bool IsImage(IFormFile file)
    {
        if (file.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".gif"];
        string fileExtension = Path.GetExtension(file.FileName);

        return Array.Exists(allowedExtensions, ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));
    }

    public static async Task<string> WriteFile(IFormFile file, EnumImageType type)
    {
        var fileStorageLocation = Path.Combine("StaticFiles/Images/", type.GetDescription());
        string pathExists = Path.Combine(Directory.GetCurrentDirectory(), fileStorageLocation);
        if (!Directory.Exists(pathExists))
            Directory.CreateDirectory(pathExists);

        var extension = Path.GetExtension(file.FileName);
        var fileName = string.Concat(Guid.NewGuid(), extension);
        var path = Path.Combine(fileStorageLocation, fileName);

        using var bits = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(bits);
        return path;
    }
}
