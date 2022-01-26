using FileHelpers;
using Microsoft.AspNetCore.Components.Forms;
using System.IO.Abstractions;

namespace BlazorFileParser.Data
{
    public class FileProcessingService
    {
        private const long maxFileSize = 1_048_576 * 3;
        private readonly IFileSystem fileSystem;

        public FileProcessingService(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public async Task<ProcessingResult<TRecord>> ProcessFile<TRecord>(IBrowserFile file) where TRecord : class
        {
            if (fileSystem.Path.GetExtension(file.Name) != ".txt")
            {
                return new ProcessingResult<TRecord>("The file must have .txt extension");
            }
            if (file.Size > maxFileSize)
            {
                return new ProcessingResult<TRecord>("The provided file is too big. Maximum supported size is 3MiB.");
            }

            string tempFile = await SaveToTempFileAsync(file.OpenReadStream(maxFileSize));

            var result = ParseFile<TRecord>(tempFile);

            fileSystem.File.Delete(tempFile);

            return result;
        }

        private static ProcessingResult<TRecord> ParseFile<TRecord>(string filePath) where TRecord : class
        {
            var engine = new FileHelperEngine<TRecord>()
            {
                ErrorMode = ErrorMode.SaveAndContinue
            };

            var records = engine.ReadFileAsList(filePath);

            if (engine.ErrorManager.HasErrors)
            {
                return new ProcessingResult<TRecord>(
                    $"{engine.ErrorManager.ErrorCount} records are invalid.",
                    engine.ErrorManager.Errors.Select(e => e.RecordString).ToList());
            }

            return new ProcessingResult<TRecord>(records);
        }

        private async Task<string> SaveToTempFileAsync(Stream stream)
        {
            var tempFile = fileSystem.Path.GetTempFileName();
            await using Stream fileStream = fileSystem.File.OpenWrite(tempFile);
            await stream.CopyToAsync(fileStream);
            return tempFile;
        }
    }
}
