namespace BlazorFileParser.Data
{
    public class ProcessingResult<TRecord>
    {
        public ProcessingResult(string errorMessage, List<string>? invalidLines = null)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
            InvalidLines = invalidLines;
        }

        public ProcessingResult(List<TRecord> records)
        {
            Records = records;
            IsSuccess = true;
        }

        public bool IsSuccess { get; init; }
        public string? ErrorMessage { get; init; }
        public List<TRecord>? Records { get; init; }
        public List<string>? InvalidLines { get; init; }
    }
}
