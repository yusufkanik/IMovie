namespace MovieAPI.DTOs;

public record SyncResultDTO(
    int ProcessedPages,
    int AddedCount,
    int UpdatedCount,
    string Message
);
