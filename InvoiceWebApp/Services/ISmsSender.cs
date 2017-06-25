using System.Threading.Tasks;

namespace InvoiceWebApp.Services {

    public interface ISmsSender {

        Task SendSmsAsync(string number, string message);
    }
}