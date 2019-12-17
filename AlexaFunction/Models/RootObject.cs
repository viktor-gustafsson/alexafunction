using System.Collections.Generic;

namespace AlexaFunction.Models
{
    public class RootObject
{
    public List<Trip> Trip { get; set; }
    public string scrB { get; set; }
    public string scrF { get; set; }
    public string serverVersion { get; set; }
    public string dialectVersion { get; set; }
    public string requestId { get; set; }
}
}