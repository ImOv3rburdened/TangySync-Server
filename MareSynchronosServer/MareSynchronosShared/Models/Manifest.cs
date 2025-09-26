using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MareSynchronosShared.Models
{
    public class Manifest
    {
        [Key]
        [MaxLength(40)]
        public string FileHash { get; set; } = default!;

        [Required]
        public string Magnet { get; set; } = default!;

        [Required]
        public string PieceHashesJson { get; set; } = default!;

        public long Size { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(128)]
        public string Token { get; set; } = string.Empty;

        [ForeignKey(nameof(FileHash))]
        public FileCache FileCache { get; set; } = default!;
    }
}