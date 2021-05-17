namespace DWAsync
{
    using System.IO;
    using System.Threading.Tasks;

    public class Program
    {
        private const string PATH = "Files";
        public static async Task Main(string[] args)
        {
            var getFiles = Directory.GetFiles(PATH);

            foreach (var file in getFiles)
            {
                var fileName = file.Split('\\')[1];
                var textRead = await ReadAsync(file);
                await WriteAsync(fileName, textRead);
            }
        }

        private static async Task<string> ReadAsync(string file)
        {
            return await File.ReadAllTextAsync(file);
        }

        private static async Task WriteAsync(string file, string text)
        {
            var newFile = $"{PATH}\\new_{file}";
            await File.WriteAllTextAsync(newFile, text);
        }
    }
}
