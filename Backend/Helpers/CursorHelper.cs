using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Helpers
{
   public static class CursorHelper
{
    public static string EncodeCursor(int lastPostId)
    {
        var raw = $"POST|{lastPostId}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
    }

    public static int? DecodeCursor(string? cursor)
    {
        if (string.IsNullOrEmpty(cursor)) return null;

        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
            var parts = decoded.Split('|');
            if (parts.Length == 2 && parts[0] == "POST" && int.TryParse(parts[1], out var id))
            {
                return id;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Invalid cursor: {ex.Message}");
        }
        return null;
    }
}

}