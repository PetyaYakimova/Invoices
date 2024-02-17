using AutoMapper;
using Invoices.Data.Models;
using Invoices.Data.Models.Enums;
using Invoices.DataProcessor.ExportDto;
using Invoices.DataProcessor.ImportDto;
using System.Globalization;

namespace Invoices
{
    public class InvoicesProfile : Profile
    {
        public InvoicesProfile()
        {
            this.CreateMap<ImportAddressDto, Address>();

            this.CreateMap<ImportClientDto, Client>();
            this.CreateMap<Client, ExportClientDto>()
                .ForMember(d => d.InvoicesCount, opt => opt.MapFrom(s => s.Invoices.Count))
                .ForMember(d => d.Invoices, opt => opt.MapFrom(s => s.Invoices));

            this.CreateMap<ImportInvoiceDto, Invoice>()
                .ForMember(d => d.CurrencyType, opt => opt.MapFrom(s => (CurrencyType)s.CurrencyType))
                .ForMember(d => d.IssueDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.IssueDate, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture)))
                .ForMember(d => d.DueDate, opt => opt.MapFrom(s => DateTime.ParseExact(s.DueDate, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture)));
            this.CreateMap<Invoice, ExportInvoiceDto>()
                .ForMember(d => d.DueDate, opt => opt.MapFrom(s => s.DueDate.ToString("d", CultureInfo.InvariantCulture)))
                .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.CurrencyType.ToString()));

            this.CreateMap<ImportProductDto, Product>()
                .ForMember(d => d.CategoryType, opt => opt.MapFrom(s => (CategoryType)s.CategoryType));
        }
    }
}
