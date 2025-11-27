namespace WebAPI.DTOs
{
    public class ContainerImageDto
    {
        public long Id { get; set; }
        public string previewImageSrc { get; set; }
        public string thumbnailImageSrc { get; set; }
        public string alt { get; set; }
        public string title { get; set; }
    }
}
