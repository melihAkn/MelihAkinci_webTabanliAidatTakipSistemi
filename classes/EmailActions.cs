using DotNetEnv;
using System.Net;
using System.Net.Mail;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.classes {
    public class EmailActions {
        // burada dto kullanılabilir
        public Boolean SendEmail(string targetMailAddress, string mailSubject, string mailBody) {
            try {
                using(var client = new SmtpClient()) {
                    client.Host = Env.GetString("EMAIL_HOST");
                    client.Port = Env.GetInt("EMAIL_PORT");
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(Env.GetString("EMAIL_CREDENTIAL_MAIL_ADDRESS"), Env.GetString("EMAIL_CREDENTIAL_PASSWORD"));
                    // mail gönderilecek kişinin display name olarak ilk 5 karakteri
                    using(var message = new MailMessage(
                        from: new MailAddress(Env.GetString("EMAIL_CREDENTIAL_MAIL_ADDRESS"), Env.GetString("EMAIL_SENDER_NAME")),
                        to: new MailAddress(targetMailAddress, targetMailAddress[..5])
                        )) {

                        message.Subject = mailSubject;
                        message.Body = mailBody;

                        client.Send(message);
                        return true;
                    }
                }
            }
            catch(Exception ex) {
                Console.WriteLine($"Email gönderilirken hata oluştu: {ex.Message}");
                return false;
            }
        }
    }
}
