using System;
using System.Net.Mail;

namespace ISB_BIA_IMPORT1.Services
{

    class MyMailNotificationService : IMyMailNotificationService
    {
        IMyDialogService myDia;
        private static readonly string from = "BIA-Tool@isb.rlp.de";

        public MyMailNotificationService(IMyDialogService myDialogService)
        {
            this.myDia = myDialogService;
        }

        public void Send_NotificationMail(string subject, string body, Current_Environment ce)
        {
            string to = "inethelpdesk@isb.rlp.de";
            to = "Tim.Wolf@isb.rlp.de";
            try
            {
                if (ce == Current_Environment.Local_Test)
                {
                    myDia.ShowMessage("Mail gesendet");
                }
                else
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        mail.IsBodyHtml = false;
                        mail.From = new MailAddress(from, "BIA Tool");
                        mail.Subject = subject;
                        mail.Body = body;

                        mail.To.Add(new MailAddress(to));

                        SmtpClient client = new SmtpClient
                        {
                            Port = 25,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            Host = "mail",
                            EnableSsl = false,
                            UseDefaultCredentials = false
                        };
                        client.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                myDia.ShowError("Mail Notification konnte nicht gesendet werden.\n\n", ex);
            }
        }
    }
}
