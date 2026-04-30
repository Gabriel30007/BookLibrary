using BookLibraryXml.Abstractions;
using BookLibraryXml.DTOs;
using BookLibraryXml.Models;
using System.Xml.Serialization;

namespace BookLibraryXml.FileHandlers
{
    public class BookXmlHandler : IBookXmlStorage
    {
        public async Task<List<Book>> LoadAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("XML file not found.", filePath);

            var xml = await File.ReadAllTextAsync(filePath);
            var serializer = new XmlSerializer(typeof(BookContainer));

            using (var reader = new StringReader(xml))
            {
                var result = (BookContainer?)serializer.Deserialize(reader);
                return result?.Books ?? new List<Book>();
            }
        }

        public async Task SaveAsync(string filePath, List<Book> books)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path is required.", filePath);

            if (books == null)
                throw new ArgumentNullException(nameof(books));

            var container = new BookContainer
            {
                Books = books
            };

            var serializer = new XmlSerializer(typeof(BookContainer));

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, container);

                stream.Position = 0;
                // await using — ensures asynchronous disposal via DisposeAsync
                await using (var fileStream = new FileStream( 
                    filePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 4096,
                    useAsync: true)) // useAsync: true — enables asynchronous I/O operations on the stream 
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
        }
    }
}
