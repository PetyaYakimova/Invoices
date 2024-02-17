using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Client")]
    public class ImportClientDto
    {

        [Required]
        [MaxLength(25)]
        [MinLength(10)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(15)]
        [MinLength(10)]
        [XmlElement("NumberVat")]
        public string NumberVat { get; set; } = null!;

        [NotMapped]
        [XmlArray("Addresses")]
        public List<ImportAddressDto> Addresses { get; set; } = new List<ImportAddressDto>();
    }
}
