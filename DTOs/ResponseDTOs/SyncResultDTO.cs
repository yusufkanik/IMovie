namespace MovieAPI.DTOs.ResponseDTOs;

// a record to send sycronization result after updating the database
public record SyncResultDTO(
    int ProcessedPages,
    int AddedCount,
    int UpdatedCount,
    string Message
);
