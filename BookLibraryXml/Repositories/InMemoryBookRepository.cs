using BookLibraryXml.Abstractions;
using BookLibraryXml.Models;

namespace BookLibraryXml.Repositories
{
    public class InMemoryBookRepository : IBookRepository
    {
        private readonly List<Book> _books = new();

        public void Add(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            _books.Add(book);
        }

        public void AddRange(List<Book> books)
        {
            if (books == null)
                throw new ArgumentNullException(nameof(books));

            _books.AddRange(books);
        }

        public IReadOnlyList<Book> GetAll()
        {
            return _books.AsReadOnly();
        }

        public void ReplaceAll(List<Book> books)
        {
            if (books == null)
                throw new ArgumentNullException(nameof(books));

            _books.Clear();
            _books.AddRange(books);
        }
    }
}
