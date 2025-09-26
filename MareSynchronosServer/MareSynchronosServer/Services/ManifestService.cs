using System.Text.Json;
using MareSynchronosShared.Data;
using MareSynchronosShared.Models;
using Microsoft.EntityFrameworkCore;
using MareSynchronos.API.Dto.Files;

namespace MareSynchronosServer.Services
{
    public class ManifestService
    {
        private readonly MareDbContext _db;
        public ManifestService(MareDbContext db) { _db = db; }

        public async Task<ManifestDto> UpsertAsync(ManifestDto dto, string ownerUid)
        {
            var file = await _db.Files.AsNoTracking().FirstOrDefaultAsync(f => f.Hash == dto.Hash);
            if (file == null)
            {
                file = new FileCache
                {
                    Hash = dto.Hash,
                    UploaderUID = ownerUid,
                    Uploaded = true,
                    UploadDate = DateTime.UtcNow,
                    Size = dto.Size,
                    RawSize = dto.Size
                };
                _db.Files.Add(file);
            }

            var existing = await _db.Manifests.FirstOrDefaultAsync(m => m.FileHash == dto.Hash);
            var pieceJson = JsonSerializer.Serialize(dto.PieceHashes ?? Array.Empty<string>());

            if (existing == null)
            {
                _db.Manifests.Add(new Manifest
                {
                    FileHash = dto.Hash,
                    Magnet = dto.Magnet,
                    PieceHashesJson = pieceJson,
                    Size = dto.Size,
                    Token = dto.Token ?? string.Empty,
                    CreatedUtc = DateTime.UtcNow
                });
            }
            else
            {
                existing.Magnet = dto.Magnet;
                existing.PieceHashesJson = pieceJson;
                existing.Size = dto.Size;
                if (!string.IsNullOrEmpty(dto.Token))
                    existing.Token = dto.Token;
            }

            await _db.SaveChangesAsync();

            return new ManifestDto
            {
                Hash = dto.Hash,
                Magnet = dto.Magnet,
                Size = dto.Size,
                Token = dto.Token ?? string.Empty,
                CreatedUtc = DateTime.UtcNow,
                PieceHashes = dto.PieceHashes ?? Array.Empty<string>()
            };
        }

        public async Task<ManifestDto?> GetAsync(string hash)
        {
            var e = await _db.Manifests.AsNoTracking().FirstOrDefaultAsync(m => m.FileHash == hash);
            if (e == null) return null;

            return new ManifestDto
            {
                Hash = e.FileHash,
                Magnet = e.Magnet,
                Size = e.Size,
                Token = e.Token,
                CreatedUtc = e.CreatedUtc,
                PieceHashes = JsonSerializer.Deserialize<string[]>(e.PieceHashesJson) ?? Array.Empty<string>()
            };
        }
    }
}
