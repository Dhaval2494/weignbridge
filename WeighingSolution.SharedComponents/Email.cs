using System;
using System.Collections.Generic;
using System.Net.Mail;
using WeighingSolution.DataLayer;

namespace WeighingSolution.SharedComponents
{
    public class Email
    {
        private string m_emailAddress;
        private string m_subject;
        private string m_body;
        private Dictionary<int, String> m_attachments;
        private string m_EmailFrom;
        private string m_SMTPServer;
        private string m_SMTPUserName;
        private string m_SMTPPassword;

        private SmtpClient m_smtpClient;
        public Email()
        {
            m_emailAddress = string.Empty;
            m_subject = string.Empty;
            m_body = string.Empty;
            m_smtpClient = null;
            m_SMTPServer = string.Empty;
            m_SMTPUserName = string.Empty;
            m_SMTPPassword = string.Empty;
        }

        #region "Properties"

        public SmtpClient smtp
        {
            get { return m_smtpClient; }
            set { m_smtpClient = value; }
        }

        public string EmailAddress
        {
            get { return m_emailAddress; }
            set { m_emailAddress = value; }
        }
        public string Subject
        {
            get { return m_subject; }
            set { m_subject = value; }
        }

        public string Body
        {
            get { return m_body; }
            set { m_body = value; }
        }
        public string EmailFrom
        {
            get { return m_EmailFrom; }
            set { m_EmailFrom = value; }
        }
        public Dictionary<int, string> Attachments
        {
            get { return m_attachments; }
            set { m_attachments = value; }
        }

        public string SMTPUserName
        {
            get { return m_SMTPUserName; }
            set { m_SMTPUserName = value; }
        }
        public string SMTPPassword
        {
            get { return m_SMTPPassword; }
            set { m_SMTPPassword = value; }
        }
        public string SMTPServer
        {
            get { return m_SMTPServer; }
            set { m_SMTPServer = value; }
        }
        #endregion

        #region "Helper For Send Mail"

        private void SendMail(MailMessage ObjMail, int Port, bool Ssl)
        {
            if (smtp == null)
            {
                smtp = new SmtpClient(m_SMTPServer, Port);
            }

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = Ssl;
            System.Net.NetworkCredential myCredential = new System.Net.NetworkCredential();//("email", "password");
            myCredential.UserName = m_SMTPUserName;
            myCredential.Password = m_SMTPPassword;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = myCredential;
            smtp.Send(ObjMail);
            ObjMail.Dispose();
            ObjMail = null;
            smtp.Dispose();
            smtp = null;
        }

        public bool SendHtmlEmail(int Port, bool Ssl, string Bcc = "")
        {
            try
            {
                MailMessage ObjMail = new MailMessage();
                m_emailAddress = m_emailAddress.Replace(";", ",").TrimEnd(",".ToCharArray()).TrimEnd(";".ToCharArray()).ToString();
                ObjMail.To.Add(m_emailAddress);
                if ((!string.IsNullOrEmpty(Bcc)))
                {
                    ObjMail.Bcc.Add(Bcc);
                }
                ObjMail.Subject = this.Subject;
                ObjMail.Body = this.Body;
                ObjMail.From = new MailAddress(m_EmailFrom);
                ObjMail.IsBodyHtml = true;
                if (m_attachments != null && m_attachments.Values.Count > 0)
                {
                    foreach (String attachpath in m_attachments.Values)
                    {
                        try
                        {
                            Attachment mailAttachment = new Attachment(attachpath.ToString());
                            ObjMail.Attachments.Add(mailAttachment);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                System.Net.ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
                SendMail(ObjMail, Port, Ssl);
            }
            catch (Exception ex)
            {
                Common.logErrorInDB(ex.Message, ex);
                return false;
            }
            return true;
        }

        #endregion

        private static bool CertificateValidationCallBack(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
