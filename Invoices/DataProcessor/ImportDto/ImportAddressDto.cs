using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Address")]
    public class ImportAddressDto
    {
        [Required]
        [MaxLength(20)]
        [MinLength(10)]
        [XmlElement("StreetName")]
        public string StreetName { get; set; } = null!;

        [Required]
        [XmlElement("StreetNumber")]
        public int StreetNumber { get; set; }

        [Required]
        [XmlElement("PostCode")]
        public string PostCode { get; set; } = null!;

        [Required]
        [MaxLength(15)]
        [MinLength(5)]
        [XmlElement("City")]
        public string City { get; set; } = null!;

        [Required]
        [MaxLength(15)]
        [MinLength(5)]
        [XmlElement("Country")]
        public string Country { get; set; } = null!;
    }
}
