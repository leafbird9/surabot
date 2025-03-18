namespace surabot.Models
{
    public class LiveStreamInfo
    {
        public int LiveId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string ThumbnailUrl { get; set; }
        public int ViewerCount { get; set; }
        public int AccumulateCount { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public bool IsAdult { get; set; }
        public bool IsChatActive { get; set; }
        public string ChatChannelId { get; set; }
        public string CategoryType { get; set; }
        public string Category { get; set; }
        public string Tags { get; set; }
        public bool PaidPromotion { get; set; }
        public string StreamUrl { get; set; }  // ✅ 직접 URL 생성
        public LiveChannelInfo Channel { get; set; }
        public LiveMediaInfo Media { get; set; }
    }
}
