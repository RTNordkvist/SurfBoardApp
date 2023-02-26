namespace SurfBoardApp.Models
{
    public class Image
    {
        public int Id { get; set; }

        public string Picture { get; set; }

        public int BoardId { get; set; }

        public Board Board { get; set; }
    }
}
