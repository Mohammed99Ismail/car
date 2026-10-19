namespace ClinicAPI.Shared.Model
{
    public class DocumentSettings
    {
        public string StoragePath { get; set; } = "documents";
        public bool IsAbsolutePath { get; set; } = false;
        public long MaxFileSize { get; set; } = 15728640; // 15MB
    }
}
