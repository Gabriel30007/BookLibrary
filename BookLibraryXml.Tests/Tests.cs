using BookLibraryXml.Services;
using BookLibraryXml.Models;

namespace BookLibraryXml.Tests
{
    public class Tests
    {
        [Fact]
        public void AddBook_Should_Add_Book()
        {
            var service = new BookService();

            var book = new Book
            {
                Title = "The Shining",
                Author = "Stephen King",
                Pages = 447
            };

            service.AddBook(book);

            var result = service.GetAllBooks();

            Assert.Single(result);
            Assert.Equal("The Shining", result[0].Title);
        }

        [Fact]
        public void AddBook_Should_Throw_When_Book_Is_Null()
        {
            var service = new BookService();

            Assert.Throws<ArgumentNullException>(() => service.AddBook(null));
        }

        [Fact]
        public void AddBooks_Should_Add_Multiple_Books()
        {
            var service = new BookService();

            var books = new List<Book>
            {
                new Book { Title = "The Shining", Author = "Stephen King", Pages = 447 },
                new Book { Title = "It", Author = "Stephen King", Pages = 1138 }
            };

            service.AddBooks(books);

            var result = service.GetAllBooks();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetAllBooks_Should_Return_All_Books()
        {
            var service = new BookService();

            service.AddBooks(new List<Book>
            {
                new Book { Title = "Book 1", Author = "Author 1", Pages = 100 },
                new Book { Title = "Book 2", Author = "Author 2", Pages = 200 }
            });

            var result = service.GetAllBooks();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetSortedBooks_Should_Sort_By_Author_Then_Title()
        {
            var service = new BookService();

            service.AddBooks(new List<Book>
            {
                new Book { Title = "Zoo", Author = "Andersen", Pages = 100 },
                new Book { Title = "Apple", Author = "Andersen", Pages = 100 },
                new Book { Title = "It", Author = "King", Pages = 100 }
            });

            var result = service.GetSortedBooks();

            Assert.Equal("Andersen", result[0].Author);
            Assert.Equal("Apple", result[0].Title);

            Assert.Equal("Andersen", result[1].Author);
            Assert.Equal("Zoo", result[1].Title);

            Assert.Equal("King", result[2].Author);
            Assert.Equal("It", result[2].Title);
        }

        [Fact]
        public void SearchByTitle_Should_Find_Book_By_Part_Of_Title()
        {
            var service = new BookService();

            service.AddBooks(new List<Book>
            {
                new Book { Title = "Harry Potter", Author = "J.K. Rowling", Pages = 300 },
                new Book { Title = "The Shining", Author = "Stephen King", Pages = 447 }
            });

            var result = service.SearchByTitle("potter");

            Assert.Single(result);
            Assert.Equal("Harry Potter", result[0].Title);
        }

        [Fact]
        public void SearchByTitle_Should_Return_Empty_List_When_Nothing_Found()
        {
            var service = new BookService();

            service.AddBook(new Book
            {
                Title = "Harry Potter",
                Author = "J.K. Rowling",
                Pages = 300
            });

            var result = service.SearchByTitle("King");

            Assert.Empty(result);
        }

        [Fact]
        public async Task SaveToXmlAsync_Should_Create_Xml_File()
        {
            var service = new BookService();

            service.AddBook(new Book
            {
                Title = "The Shining",
                Author = "Stephen King",
                Pages = 447
            });

            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xml");

            try
            {
                await service.SaveToXmlAsync(filePath);

                Assert.True(File.Exists(filePath));
            }
            finally
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        [Fact]
        public async Task LoadFromXmlAsync_Should_Load_Books_From_Xml_File()
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xml");

            try
            {
                var service = new BookService();

                service.AddBooks(new List<Book>
                {
                    new Book { Title = "The Shining", Author = "Stephen King", Pages = 447 },
                    new Book { Title = "It", Author = "Stephen King", Pages = 1138 }
                });

                await service.SaveToXmlAsync(filePath);

                var loadedservice = new BookService();

                await loadedservice.LoadFromXmlAsync(filePath);

                var result = loadedservice.GetAllBooks();

                Assert.Equal(2, result.Count);
                Assert.Contains(result, b => b.Title == "The Shining");
                Assert.Contains(result, b => b.Title == "It");
            }
            finally
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        [Fact]
        public async Task LoadFromXmlAsync_Should_Replace_Existing_Books()
        {
            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xml");

            try
            {
                var sourceservice = new BookService();

                sourceservice.AddBook(new Book
                {
                    Title = "New Book",
                    Author = "New Author",
                    Pages = 150
                });

                await sourceservice.SaveToXmlAsync(filePath);

                var targetservice = new BookService();

                targetservice.AddBook(new Book
                {
                    Title = "Old Book",
                    Author = "Old Author",
                    Pages = 100
                });

                await targetservice.LoadFromXmlAsync(filePath);

                var result = targetservice.GetAllBooks();

                Assert.Single(result);
                Assert.Equal("New Book", result[0].Title);
            }
            finally
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }
    }
}