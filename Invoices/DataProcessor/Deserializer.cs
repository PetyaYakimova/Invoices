namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using AutoMapper;
    using System.Text;
    using Invoices.Data;
    using Invoices.Utilities;
    using Invoices.DataProcessor.ImportDto;
    using Invoices.Data.Models;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            XmlHelper xmlHelper = new XmlHelper();
            Mapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InvoicesProfile>();
            }));
            StringBuilder sb = new StringBuilder();

            ImportClientDto[] clients = xmlHelper.Deserialize<ImportClientDto[]>(xmlString, "Clients");

            List<Client> validClients = new List<Client>();

            foreach (ImportClientDto client in clients)
            {
                if (!IsValid(client))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Client validClient = mapper.Map<Client>(client);

                List<Address> validAddresses = new List<Address>();

                foreach (ImportAddressDto address in client.Addresses)
                {
                    if (!IsValid(address))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Address validAddress = mapper.Map<Address>(address);
                    validAddresses.Add(validAddress);
                }

                validClient.Addresses = validAddresses;
                validClients.Add(validClient);
                sb.AppendLine(string.Format(SuccessfullyImportedClients, validClient.Name));
            }

            context.Clients.AddRange(validClients);
            context.SaveChanges();
            return sb.ToString().Trim();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InvoicesProfile>();
            }));
            StringBuilder sb = new StringBuilder();

            List<int> validClientIds = context.Clients.Select(c => c.Id).ToList();

            ImportInvoiceDto[] invoices = JsonConvert.DeserializeObject<ImportInvoiceDto[]>(jsonString);

            List<Invoice> validInvoices = new List<Invoice>();

            foreach (ImportInvoiceDto invoice in invoices)
            {
                if (!IsValid(invoice))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!validClientIds.Contains(invoice.ClientId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Invoice validInvoice = new Invoice();

                try
                {
                    validInvoice = mapper.Map<Invoice>(invoice);
                }
                catch (Exception ex) 
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (validInvoice.DueDate < validInvoice.IssueDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                validInvoices.Add(validInvoice);
                sb.AppendLine(string.Format(SuccessfullyImportedInvoices, validInvoice.Number));
            }

            context.Invoices.AddRange(validInvoices);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            Mapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<InvoicesProfile>();
            }));
            StringBuilder sb = new StringBuilder();

            List<int> validClientIds = context.Clients.Select(c => c.Id).ToList();

            ImportProductDto[] products = JsonConvert.DeserializeObject<ImportProductDto[]>(jsonString);

            List<Product> validProducts = new List<Product>();

            foreach (ImportProductDto product in products)
            {
                if (!IsValid(product))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Product validProduct = mapper.Map<Product>(product);

                product.Clients = product.Clients.Distinct().ToList();

                foreach (int clientId in product.Clients)
                {
                    if (!validClientIds.Contains(clientId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    ProductClient validProductClient = new ProductClient();
                    validProductClient.ClientId = clientId;
                    validProductClient.Product = validProduct;
                    validProduct.ProductsClients.Add(validProductClient);
                }

                validProducts.Add(validProduct);
                sb.AppendLine(string.Format(SuccessfullyImportedProducts, validProduct.Name, validProduct.ProductsClients.Count));
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();

            return sb.ToString().Trim();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
