using FileHelpers;
using FileHelpers.Events;

namespace BlazorFileParser.Data
{
    [DelimitedRecord("; ")]
    public class EventRecord : INotifyRead
    {
        private const int maxNameLength = 32;
        private const int maxDescriptionLength = 255;
        public string? Name { get; set; }
        public string? Description { get; set; }
        [FieldConverter(ConverterKind.Date, "yyyy-MM-ddTHH:mmzzz")]
        public DateTime Start { get; set; }
        [FieldConverter(ConverterKind.Date, "yyyy-MM-ddTHH:mmzzz")]
        public DateTime End { get; set; }

        public void AfterRead(AfterReadEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new Exception($"Error at line {e.LineNumber}. Event name cannot be empty");
            
            if (Name.Length > maxNameLength)
                throw new Exception($"Error at line {e.LineNumber}. Event name cannot be longer than {maxNameLength} characters");
            
            if (string.IsNullOrWhiteSpace(Description))
                throw new Exception($"Error at line {e.LineNumber}. Event description cannot be empty");
            
            if (Description.Length > maxDescriptionLength)
                throw new Exception($"Error at line {e.LineNumber}. Event description cannot be longer than {maxDescriptionLength} characters");
        }

        public void BeforeRead(BeforeReadEventArgs e)
        { 
            if (e.RecordLine.EndsWith(';'))
            {
                e.RecordLine = e.RecordLine.TrimEnd(';');
            }
            else
            {
                throw new Exception("Line must end with ';' character");
            }
        }
    }
}
