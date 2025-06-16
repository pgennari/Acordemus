namespace acordemus.Models
{
    public class NotificationRecord
    {
        public int Id { get; set; } 
        public string? MessageId { get; set; }
        public string? TopicArn { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
