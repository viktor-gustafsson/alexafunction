namespace AlexaFunction.Models
{
    public class Message
    {
        public string id { get; set; }
        public bool act { get; set; }
        public string head { get; set; }
        public string text { get; set; }
        public string category { get; set; }
        public int priority { get; set; }
        public int products { get; set; }
        public string sTime { get; set; }
        public string sDate { get; set; }
        public string eTime { get; set; }
        public string eDate { get; set; }
    }
}