namespace Invoices.DataProcessor
{
    using AutoMapper;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.DataProcessor.ExportDto;
    using Invoices.Utilities;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Text;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            XmlHelper xmlHelper = new XmlHelper();
            Mapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InvoicesProfile>();
            }));

            List<Client> clients = context.Clients
                .Include(x => x.Invoices)
                .AsNoTracking()
                .ToList()
                .Where(c => c.Invoices.Any(i => i.IssueDate > date))
                .OrderByDescending(c => c.Invoices.Count)
                .ThenBy(c => c.Name)
                .ToList();

            foreach (Client client in clients)
            {
                client.Invoices = client.Invoices
                    .OrderBy(i => i.IssueDate)
                    .ThenByDescending(i => i.DueDate)
                    .ToList();
            }

            List<ExportClientDto> exportClients = mapper.Map<List<ExportClientDto>>(clients);

            return xmlHelper.Serialize<List<ExportClientDto>>(exportClients, "Clients");
        }

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {
            var products = context.Products
                .Include(p => p.ProductsClients)
                .ThenInclude(pc => pc.Client)
                .ToList()
                .Where(p => p.ProductsClients.Any(pc => pc.Client.Name.Length >= nameLength))
                .Select(p => new
                {
                    p.Name,
                    p.Price,
                    Category = p.CategoryType.ToString(),
                    Clients = p.ProductsClients
                            .Select(pc => new
                            {
                                Name = pc.Client.Name,
                                NumberVat = pc.Client.NumberVat
                            })
                            .Where(c => c.Name.Length >= nameLength)
                            .OrderBy(c => c.Name)
                            .ToList()
                })
                .OrderByDescending(p => p.Clients.Count)
                .ThenBy(p => p.Name)
                .Take(5)
                .ToList();

            return JsonConvert.SerializeObject(products, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
        }
    }
}