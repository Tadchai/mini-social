using static Backend.Services.CursorService;

namespace Backend.Services.Interfaces
{
    public interface ICursorService
    {
        string EncodeCursor(CursorPayload payload);
        CursorPayload? DecodeCursor(string? cursor);
    }
}