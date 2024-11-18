namespace DACN_N3.Areas.Admin.Models.editMovie
{
    public class Tap
    {
        public string VideoUrl { get; set; }
    }

    public class Pham
    {
        public Dictionary<int ,Tap> Tap { get; set; }
    }

    public class MovieViewModel
    {
        public Dictionary<int, Pham> Phan { get; set; }
    }
}
