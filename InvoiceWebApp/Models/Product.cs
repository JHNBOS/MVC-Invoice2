using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace InvoiceWebApp.Models {

    public class Product {

        [Key]
        public int ProductID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? Price { get; set; }

        [Display(Name = "VAT")]
        [Required]
        public int VAT { get; set; }

        public int? CategoryID { get; set; }

        public virtual Category Category { get; set; }
        public virtual List<InvoiceItem> InvoiceItems { get; set; }
    }
}