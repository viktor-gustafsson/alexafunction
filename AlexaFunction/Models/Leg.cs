namespace AlexaFunction.Models
{
    public class Leg
    {
        public Origin Origin { get; set; }
        public Destination Destination { get; set; }
        public JourneyDetailRef JourneyDetailRef { get; set; }
        public Messages Messages { get; set; }
        public string JourneyStatus { get; set; }
        public Product Product { get; set; }
        public object Stops { get; set; }
        public string idx { get; set; }
        public string name { get; set; }
        public string number { get; set; }
        public string category { get; set; }
        public string type { get; set; }
        public bool reachable { get; set; }
        public bool redirected { get; set; }
        public string direction { get; set; }
    }
}