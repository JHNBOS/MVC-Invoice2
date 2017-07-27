using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace InvoiceWebApp.Models {

    public class User {

        [Key]
        public int ID { get; set; }

        public int DebtorID { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        public virtual Debtor Debtor { get; set; }
    }
}