namespace Entities
{
    public class ContainerImages : BaseModel
    {
        public string PreviewImageSrc { get; set; }
        public string ThumbnailImageSrc { get; set; }
        public string Alt { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Extintion { get; set; }
        public long ContainerId { get; set; }
        public virtual Container Container { get; set; }
    }
}
