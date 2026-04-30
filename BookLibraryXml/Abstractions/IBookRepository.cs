using BookLibraryXml.Models;

namespace BookLibraryXml.Abstractions
{
    public interface IBookRepository
    {
        void Add(Book book);
        void AddRange(List<Book> books);
        IReadOnlyList<Book> GetAll();
        void ReplaceAll(List<Book> books);
    }
}
