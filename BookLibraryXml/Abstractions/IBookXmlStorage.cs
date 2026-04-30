using BookLibraryXml.Models;

namespace BookLibraryXml.Abstractions
{
    public interface IBookXmlStorage
    {
        Task<List<Book>> LoadAsync(string filePath);
        Task SaveAsync(string filePath, List<Book> books);
    }
}
