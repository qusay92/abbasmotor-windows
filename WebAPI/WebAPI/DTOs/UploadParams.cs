namespace WebAPI.DTOs
{
    public class UploadParams
    {
        public ICollection<IFormFile> Files { get; set; }
        public long AutoId { get; set; }
        public string Path { get; set; }
    }
}
