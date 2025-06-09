using System.Text;
using System.Text.Json;
using Backend.Services.Interfaces;

namespace Backend.Services
{
    public class CursorService : ICursorService
    {
        public enum CursorType
        {
            Post,
            User,
            Group
        }
        public class CursorPayload
        {
            public CursorType Type { get; set; }
            public int Id { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public string EncodeCursor(CursorPayload payload)
        {
            var json = JsonSerializer.Serialize(payload);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        public CursorPayload? DecodeCursor(string? cursor)
        {
            if (string.IsNullOrEmpty(cursor)) return null;

            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
                return JsonSerializer.Deserialize<CursorPayload>(decoded);
            }
            catch
            {
                return null;
            }
        }
    }
}