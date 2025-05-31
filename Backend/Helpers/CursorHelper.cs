using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Helpers
{
    public static class CursorHelper
    {
        public enum CursorType
        {
            Post,
            User
        }

        public static string EncodeCursor(CursorType type, int id)
        {
            var raw = $"{type.ToString().ToUpper()}|{id}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
        }

        public static (CursorType? Type, int? Id) DecodeCursor(string? cursor)
        {
            if (string.IsNullOrEmpty(cursor)) return (null, null);

            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
                var parts = decoded.Split('|');

                if (parts.Length == 2 &&
                    Enum.TryParse(parts[0], true, out CursorType type) &&
                    int.TryParse(parts[1], out var id))
                {
                    return (type, id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid cursor: {ex.Message}");
            }
            
            return (null, null);
        }
    }

}