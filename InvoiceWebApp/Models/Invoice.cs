using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceWebApp.Models {

    public class Invoice {

        [Key]
        [Display(Name = "Invoice Number")]
        public int InvoiceNumber { get; set; }

        [Display(Name = "Debtor")]
        [ForeignKey("Debtor"), Column(Order = 0)]
        public int? DebtorID { get; set; }

        [Display(Name = "Company")]
        [ForeignKey("Company"), Column(Order = 1)]
        public int? CompanyID { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Invoice Date")]
        [Required]
        public DateTime CreatedOn { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Expiration Date")]
        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public int Discount { get; set; }

        [DataType(DataType.Currency)]
        public decimal Total { get; set; }

        [Display(Name = "Save As")]
        [Required]
        public string Type { get; set; }

        public bool Paid { get; set; }

		public string Comments { get; set; }

		public virtual Debtor Debtor { get; set; }
        public virtual Company Company { get; set; }
        public virtual List<InvoiceItem> InvoiceItems { get; set; }
    }
}