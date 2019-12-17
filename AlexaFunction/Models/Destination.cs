namespace AlexaFunction.Models
{
    public class Destination
    {
        public string name { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public string extId { get; set; }
        public double lon { get; set; }
        public double lat { get; set; }
        public string prognosisType { get; set; }
        public string time { get; set; }
        public string date { get; set; }
        public string track { get; set; }
        public string rtTime { get; set; }
        public string rtDate { get; set; }
        public bool hasMainMast { get; set; }
        public string mainMastId { get; set; }
        public string mainMastExtId { get; set; }
        public bool additional { get; set; }
    }
}