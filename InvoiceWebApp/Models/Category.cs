using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceWebApp.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public List<Product> Products { get; set; }

        public virtual List<Product> ProductList { get; set; }
    }
}
