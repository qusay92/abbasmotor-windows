namespace WebAPI.DTOs
{
    public class SubmitComplaint
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
    }
}
