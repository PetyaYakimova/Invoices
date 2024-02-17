using Invoices.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportProductDto
    {
        [Required]
        [MaxLength(30)]
        [MinLength(9)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(5.0, 1000.0)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 4)]
        public int CategoryType { get; set; }

        [NotMapped]
        public List<int> Clients { get; set; } = new List<int>();
    }
}
