using System.ComponentModel.DataAnnotations;

namespace WebCV.Models
{
    public class Evaluate
    {
        [Key]
        public int EvaluateId {  get; set; }
        public string ClientName { get; set; }
        public string? Content { get; set; }
        public string? Profession { get; set; }
        public string? Image {  get; set; }
        public int? Order { get; set; }
        public int? Hide { get; set; }
    }
}
