﻿using Invoices.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ExportDto
{
    [XmlType("Invoice")]
    public class ExportInvoiceDto
    {
        [XmlElement("InvoiceNumber")]
        public int Number { get; set; }

        [XmlElement("InvoiceAmount")]
        public decimal Amount { get; set; }

        [XmlElement("DueDate")]
        public string DueDate { get; set; } = null!;

        [XmlElement("Currency")]
        public string Currency { get; set; } = null!;
    }
}
