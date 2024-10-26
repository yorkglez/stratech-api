namespace StratechAPI.Model
{
    public class AudienceRecord
    {
        public int Id { get; set; } 
        public DateTime Timestamp { get; set; }
        public int Age { get; set; }
        public string Genre { get; set; }
        public string Mood { get; set; }
        public int TotalAudience { get; set; }
        public int DwellTimeAverage { get; set; }
    }
}
