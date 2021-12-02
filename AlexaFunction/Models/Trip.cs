namespace AlexaFunction.Models;

public class Trip
{
    public List<ServiceDay> ServiceDays { get; set; }
    public LegList LegList { get; set; }
    public TariffResult TariffResult { get; set; }
    public bool alternative { get; set; }
    public bool valid { get; set; }
    public int idx { get; set; }
    public string tripId { get; set; }
    public string ctxRecon { get; set; }
    public string duration { get; set; }
    public bool @return { get; set; }
    public string checksum { get; set; }
    public int transferCount { get; set; }
}