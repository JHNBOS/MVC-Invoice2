using InvoiceWebApp.Data;
using InvoiceWebApp.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceWebApp.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private AppSettings _settings;
        private ApplicationDbContext _context;

        public AuthMessageSender(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task SendLoginEmailAsync(string email, string pass)
        {
            string smtp = _settings.SMTP;
            int port = _settings.Port;
            string company = _settings.CompanyName;
            string company_email = _settings.Email;
            string password = _settings.Password;
            string website = _settings.Website;

            string subject = "Inloggevens " + company;
            string message = "Geachte heer, mevrouw,"
                            + "\n\n"
                            + "Hierbij ontvangt u van ons uw inloggevens. Hiermee kunt u inloggen om uw facturen te bekijken en/of te betalen."
                            + "\n\n"
                            + "Gebruikersnaam:\t\t" + email
                            + "\n"
                            + "Wachtwoord:\t\t" + pass
                            + "\n\n"
                            + "U kunt inloggen op " + "http://" + website
                            + "\n\n\n"
                            + "Met vriendelijke groet,"
                            + "\n\n"
                            + company;

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(company, company_email));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(smtp, port, false).ConfigureAwait(false);

                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(company_email, password)
                    .ConfigureAwait(false);

                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }

            // Plug in your email service here to send an email.
            //return Task.FromResult(0);
        }

        public async Task SendInvoiceEmailAsync(string email)
        {
            string smtp = _settings.SMTP;
            int port = _settings.Port;
            string company = _settings.CompanyName;
            string company_email = _settings.Email;
            string password = _settings.Password;
            string website = _settings.Website;

            string subject = "Factuur beschikbaar op " + company;
            string message = "Er staat een factuur voor u klaar op " + company + "."
                            + "\n"
                            + "http://" + website
                            + "\n\n\n"
                            + "Met vriendelijke groet,"
                            + "\n\n"
                            + company;

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(company, company_email));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(smtp, port, false).ConfigureAwait(false);

                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(company_email, password)
                    .ConfigureAwait(false);

                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }

        
    }
}
