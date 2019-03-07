using System;
using System.Net.Mail;
using ISB_BIA_IMPORT1.Services.Interfaces;

namespace ISB_BIA_IMPORT1.Services
{

    class MyMailNotificationService : IMyMailNotificationService
    {
        IMyDialogService myDia;
        IMySharedResourceService myShared;
        private static readonly string from = "BIA-Tool@isb.rlp.de";

        public MyMailNotificationService(IMyDialogService myDialogService, IMySharedResourceService mySharedResourceService)
        {
            myDia = myDialogService;
            myShared = mySharedResourceService;
        }

        public void Send_NotificationMail(string subject, string body, Current_Environment ce)
        {
            string to = myShared.TargetMail;
            try
            {
                if (ce == Current_Environment.Local_Test)
                {
                    myDia.ShowMessage("Test: 'Mail gesendet an "+ to + "'");
                }
                else
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        mail.IsBodyHtml = false;
                        mail.From = new MailAddress(from, "BIA Tool");
                        mail.Subject = (ce == Current_Environment.Prod)? subject: subject+ " [Testumgebung]";
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
