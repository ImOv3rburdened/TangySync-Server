namespace MareSynchronosShared.Entities
{
    public class ManifestEntity
    {
        public string Id { get; set; }
        public string Magnet { get; set; }
        public string PieceHashes { get; set; } // JSON serialized array
        public long Size { get; set; }
        public string Token { get; set; }
        public string OwnerUserId { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
