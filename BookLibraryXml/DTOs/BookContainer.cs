using BookLibraryXml.Models;
using System.Xml.Serialization;

namespace BookLibraryXml.DTOs
{
    [XmlRoot("Books")]
    public class BookContainer
    {
        [XmlElement("Book")]
        public List<Book> Books { get; set; } = new();
    }
}
