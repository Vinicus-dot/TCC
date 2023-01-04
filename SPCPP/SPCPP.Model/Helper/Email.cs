using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace SPCPP.Model.Helper
{
    public class Email : IEmail
    {
        private readonly IConfiguration _configuration;

        public Email(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Enviar(string email, string assunto, string mensagem)
        {

            try
            {
                string host = _configuration.GetSection("SMTP:Host").Value;
                string nome = _configuration.GetSection("SMTP:Nome").Value;
                string username = _configuration.GetSection("SMTP:UserName").Value;
                string senha = _configuration.GetSection("SMTP:Senha").Value;
                string porta = _configuration.GetSection("SMTP:Porta").Value;

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(username, nome)
                };

                mail.To.Add(email);
                mail.Subject = assunto;
                mail.Body = mensagem;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

               
                using (SmtpClient smtp = new SmtpClient(host, Convert.ToInt32(porta)))
                {
                    smtp.Credentials = new NetworkCredential(username, senha);
                    smtp.EnableSsl = true;

                    smtp.Send(mail);
                    return true;
                }

            }
            catch(Exception ex)
            {
                //Gravar Log de erro ao enviar e-mail
                return false;
            }
            
        }
    }
}
