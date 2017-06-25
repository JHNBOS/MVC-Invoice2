using System.Threading.Tasks;

namespace InvoiceWebApp.Services {

    public interface IEmailSender {

        //Task SendEmailAsync(string email, string subject, string message);
        Task SendLoginEmailAsync(string email, string password);

        Task SendInvoiceEmailAsync(string email);
    }
}