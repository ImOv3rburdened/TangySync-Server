using System.ComponentModel.DataAnnotations;

namespace MareSynchronosShared.Models
{
    public class P2PPeer
    {
        [Key] public int Id { get; set; }
        [MaxLength(40)] public string FileHash { get; set; } = default!;
        [MaxLength(10)] public string UserUid { get; set; } = default!;
        [MaxLength(64)] public string Area { get; set; } = "";
        [MaxLength(64)] public string Ip { get; set; } = default!;
        public int Port { get; set; }
        public DateTime LastSeenUtc { get; set; } = DateTime.UtcNow;
    }
}
