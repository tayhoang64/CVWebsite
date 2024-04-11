using System.ComponentModel.DataAnnotations;

namespace WebCV.Models
{
    public class Slider
    {
        [Key]
        public int SlideID { get; set; }
        public string? Title {  get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public int? Order { get; set; }
        public string? Link { get; set; }
        public int? Hide { get; set; }


    }
}
