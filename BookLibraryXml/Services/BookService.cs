using BookLibraryXml.Abstractions;
using BookLibraryXml.Models;
using BookLibraryXml.Repositories;
using BookLibraryXml.FileHandlers;

namespace BookLibraryXml.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;
        private readonly IBookXmlStorage _xmlStorage;

        public BookService()
            : this(new InMemoryBookRepository(), new BookXmlHandler())
        {
        }

        public BookService(IBookRepository repository, IBookXmlStorage xmlStorage) // using dependency injection (constructor injection) to decouple dependencies and improve testability
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _xmlStorage = xmlStorage ?? throw new ArgumentNullException(nameof(xmlStorage));
        }

        public void AddBook(Book book)
        {
            ValidateBook(book);
            _repository.Add(book);
        }

        public void AddBooks(List<Book> books)
        {
            ValidateBooks(books);
            _repository.AddRange(books);
        }

        public List<Book> GetAllBooks()
        {
            return _repository.GetAll().ToList();
        }

        public List<Book> GetSortedBooks()
        {
            return _repository.GetAll()
                .OrderBy(book => book.Author)
                .ThenBy(book => book.Title)
                .ToList();
        }

        public List<Book> SearchByTitle(string titlePart)
        {
            if (string.IsNullOrWhiteSpace(titlePart))
                return new List<Book>();

            return _repository.GetAll()
                .Where(book => book.Title.Contains(titlePart, StringComparison.OrdinalIgnoreCase)) // use OrdinalIgnoreCase for case -insensitive comparison
                .ToList();
        }

        public async Task LoadFromXmlAsync(string filePath)
        {
            var books = await _xmlStorage.LoadAsync(filePath);

            ValidateBooks(books);

            _repository.ReplaceAll(books); // replace current data to avoid duplicates when loading from XML
        }

        public async Task SaveToXmlAsync(string filePath)
        {
            var books = _repository.GetAll().ToList();

            await _xmlStorage.SaveAsync(filePath, books);
        }

        private static void ValidateBooks(IEnumerable<Book> books)
        {
            if (books == null)
                throw new ArgumentNullException(nameof(books));

            foreach (var book in books)
            {
                ValidateBook(book);
            }
        }

        private static void ValidateBook(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Book title is required.");

            if (string.IsNullOrWhiteSpace(book.Author))
                throw new ArgumentException("Book author is required.");

            if (book.Pages <= 0)
                throw new ArgumentException("Book pages count must be greater than zero.");
        }
    }
}
