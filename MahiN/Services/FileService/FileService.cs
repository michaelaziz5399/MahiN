namespace MahiN.Services.FileService
{
    public class FileService(IWebHostEnvironment environment) : IFileService
    {
        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            // 🎯 1. حماية الـ WebRootPath: لو كان null هيستخدم مسار الـ ContentRootPath + wwwroot
            string webRootPath = environment.WebRootPath
                ?? Path.Combine(environment.ContentRootPath, "wwwroot");

            // 2. تحديد المسار الكامل للمجلد
            string uploadsFolder = Path.Combine(webRootPath, "uploads", folderName);

            // 3. إنشاء المجلد أوتوماتيكياً لو مش موجود على السيرفر
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 4. إنشاء اسم فريد للصورة لمنع تكرار الأسماء
            string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 5. حفظ الملف على الهارد
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 6. إرجاع المسار النسبي
            return $"/uploads/{folderName}/{uniqueFileName}";
        }

        public void DeleteFile(string imagePath, string folderName)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            string webRootPath = environment.WebRootPath
                ?? Path.Combine(environment.ContentRootPath, "wwwroot");

            string fileName = Path.GetFileName(imagePath);
            string filePath = Path.Combine(webRootPath, "uploads", folderName, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
