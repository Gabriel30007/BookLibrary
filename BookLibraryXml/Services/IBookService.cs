using BookLibraryXml.Models;

namespace BookLibraryXml.Services
{
    public interface IBookService
    {
        void AddBook(Book book);
        void AddBooks(List<Book> books);
        List<Book> GetAllBooks();
        List<Book> GetSortedBooks();
        List<Book> SearchByTitle(string titlePart);
        Task LoadFromXmlAsync(string filePath);
        Task SaveToXmlAsync(string filePath);
    }
}
