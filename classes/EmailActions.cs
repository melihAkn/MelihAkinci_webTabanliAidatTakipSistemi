using DotNetEnv;
using System.Net;
using System.Net.Mail;

namespace MelihAkıncı_webTabanliAidatTakipSistemi.classes {
    public class EmailActions {
        // burada dto kullanılabilir
        public Boolean SendEmail(string apartmentManagerName, string targetMailAddress, string address, int floor, int unitNumber, string username, string password) {
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

                        message.Subject = "web tabanlı aidat takip sistemi apartman ataması";
                        message.Body =
                            $"""
                            Merhaba, {apartmentManagerName} sizi web tabanlı aidat takip sistemin de şu adres {address} de ki şu kat {floor} şu daire nolu {unitNumber} daireye kat maliki olarak ekledi.
                            Sisteme giriş yapablimeniz için giriş bilgileriniz:
                            kullanıcı adınız : {username}
                            şifreniz : {password}

                            eğer bu işlem bilginiz dahilinde değilse bize bu mail adresinden bildirin
                            webtabanliaidaat@gmail.com
                        """;

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
