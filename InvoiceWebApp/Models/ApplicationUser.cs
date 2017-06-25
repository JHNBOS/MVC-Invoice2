﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace InvoiceWebApp.Models {

    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser {
        public int DebtorID { get; set; }
        public string AccountType { get; set; }

        public virtual Debtor Debtor { get; set; }
    }
}