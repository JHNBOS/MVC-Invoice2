using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceWebApp.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        public int DebtorID { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string AccountType { get; set; }

        public virtual Debtor Debtor { get; set; }
    }
}
