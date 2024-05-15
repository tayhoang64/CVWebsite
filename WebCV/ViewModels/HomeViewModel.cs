using WebCV.Models;

namespace WebCV.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<Evaluate> Evaluates { get; set; }
        public List<Company> Companies { get; set; }
        public List<Job> Jobs { get; set; }
    }
}
