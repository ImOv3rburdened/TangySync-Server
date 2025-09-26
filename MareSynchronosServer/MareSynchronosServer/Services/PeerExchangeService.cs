using MareSynchronosShared.Data;
using MareSynchronosShared.Models;
using Microsoft.EntityFrameworkCore;

namespace MareSynchronosServer.Services
{
    public class PeerExchangeService
    {
        private readonly MareDbContext _db;
        public PeerExchangeService(MareDbContext db) { _db = db; }

        public async Task RegisterAsync(string fileHash, string userUid, string ip, int port, string area)
        {
            var now = DateTime.UtcNow;
            var row = await _db.P2PPeers.FirstOrDefaultAsync(p => p.FileHash == fileHash && p.UserUid == userUid);
            if (row == null)
            {
                _db.P2PPeers.Add(new P2PPeer { FileHash = fileHash, UserUid = userUid, Ip = ip, Port = port, Area = area ?? "", LastSeenUtc = now });
            }
            else
            {
                row.Ip = ip; row.Port = port; row.Area = area ?? ""; row.LastSeenUtc = now;
            }
            await _db.SaveChangesAsync();
        }

        public async Task UnregisterAsync(string fileHash, string userUid)
        {
            var row = await _db.P2PPeers.FirstOrDefaultAsync(p => p.FileHash == fileHash && p.UserUid == userUid);
            if (row != null) { _db.P2PPeers.Remove(row); await _db.SaveChangesAsync(); }
        }

        public async Task<List<P2PPeer>> GetPeersAsync(string fileHash, string requesterUid, string? area, int take)
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-5);
            var q = _db.P2PPeers.AsNoTracking()
                    .Where(p => p.FileHash == fileHash && p.LastSeenUtc >= cutoff && p.UserUid != requesterUid);

            if (!string.IsNullOrEmpty(area))
                q = q.OrderByDescending(p => p.Area == area).ThenByDescending(p => p.LastSeenUtc);
            else
                q = q.OrderByDescending(p => p.LastSeenUtc);

            return await q.Take(Math.Clamp(take, 1, 200)).ToListAsync();
        }
    }
}
