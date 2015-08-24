using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Xml;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;

namespace Common
{
    public class MailRecipient
    {
        string name;
        string address;
        bool active = true;
        public MailRecipient(string name, string address)
        {
            this.name = name;
            this.address = address;
        }
        public void setActive(bool active)
        {
            this.active = active;
        }
        public bool isActive()
        {
            return active;
        }
        public string getAddress()
        {
            return address;
        }
        public override string ToString()
        {
            return name;
        }

    }
    public class MailSender
    {
        private static string server = "smtp.gmail.com";
        private static string user = "";
        private static string pw = "";
        private List<MailRecipient> list = new List<MailRecipient>();
        private string signature = "n/a";

        public MailSender()
        {
        }
        public void addRecipient(MailRecipient recipient)
        {
            list.Add(recipient);
        }
        public void clearRecipients()
        {
            list.Clear();
        }

        public void setSignature(string signature)
        {
            this.signature = signature;
        }

    /**
     *             //sendMail("test", "test message");
     * 
     */
        public void sendMail(string subject, string body)
        {
            // create mail message object
            //MailMessage message = new MailMessage(
            //user,
            //"{cell-number}@txt.att.net",
            //subject, body);

            if (list.Count == 0)
            {
                Console.WriteLine("NO Recipients for mail message");
                return;
            }
            MailMessage message = new MailMessage();
            foreach (MailRecipient mr in list)
            {
                if (mr.isActive())
                    message.To.Add(mr.getAddress());

            }
            message.From = new MailAddress(user);
            message.Subject = subject;
            message.Body = string.Format("{0}\n{1}", body, signature);

            SmtpClient client = new SmtpClient(server);

            NetworkCredential basicCredential =
                new NetworkCredential(user, pw);

            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential;
            // set the port to 587. This is the SSL port of Gmail SMTP server
            client.Port = 587;
            // set the security mode to explicit
            //client..SecurityMode = SmtpSslSecurityMode.Explicit;
            client.EnableSsl = true;

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                Exception exin = ex.InnerException;
                if (exin != null)
                {
                    Console.Error.WriteLine("Error: " + exin.Message);
                }
            }
        }
    }

    public class Pop3Exception : System.ApplicationException
    {
        public Pop3Exception(string str)
            : base(str)
        {
        }
    }
    public class Pop3Message
    {
        public long number;
        public long bytes;
        public bool retrieved;
        public string message;
    }
    public class Pop3 : System.Net.Sockets.TcpClient
    {
        public void Connect(string server, string username, string password)
        {
            string message;
            string response;

            Connect(server, 995);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }

            message = "USER " + username + "\r\n";
            Write(message);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }

            message = "PASS " + password + "\r\n";
            Write(message);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }
        }

        public void Disconnect()
        {
            string message;
            string response;
            message = "QUIT\r\n";
            Write(message);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }
        }

        public ArrayList List()
        {
            string message;
            string response;

            ArrayList retval = new ArrayList();
            message = "LIST\r\n";
            Write(message);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }

            while (true)
            {
                response = Response();
                if (response == ".\r\n")
                {
                    return retval;
                }
                else
                {
                    Pop3Message msg = new Pop3Message();
                    char[] seps = { ' ' };
                    string[] values = response.Split(seps);
                    msg.number = Int32.Parse(values[0]);
                    msg.bytes = Int32.Parse(values[1]);
                    msg.retrieved = false;
                    retval.Add(msg);
                    continue;
                }
            }
        }

        public Pop3Message Retrieve(Pop3Message rhs)
        {
            string message;
            string response;

            Pop3Message msg = new Pop3Message();
            msg.bytes = rhs.bytes;
            msg.number = rhs.number;

            message = "RETR " + rhs.number + "\r\n";
            Write(message);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }

            msg.retrieved = true;
            while (true)
            {
                response = Response();
                if (response == ".\r\n")
                {
                    break;
                }
                else
                {
                    msg.message += response;
                }
            }

            return msg;
        }

        public void Delete(Pop3Message rhs)
        {
            string message;
            string response;

            message = "DELE " + rhs.number + "\r\n";
            Write(message);
            response = Response();
            if (response.Substring(0, 3) != "+OK")
            {
                throw new Pop3Exception(response);
            }
        }

        private void Write(string message)
        {
            System.Text.ASCIIEncoding en = new System.Text.ASCIIEncoding();

            byte[] WriteBuffer = new byte[1024];
            WriteBuffer = en.GetBytes(message);

            NetworkStream stream = GetStream();
            stream.Write(WriteBuffer, 0, WriteBuffer.Length);

            Debug.WriteLine("WRITE:" + message);
        }

        private string Response()
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] serverbuff = new Byte[1024];
            NetworkStream stream = GetStream();
            int count = 0;
            while (true)
            {
                byte[] buff = new Byte[2];
                int bytes = stream.Read(buff, 0, 1);
                if (bytes == 1)
                {
                    serverbuff[count] = buff[0];
                    count++;

                    if (buff[0] == '\n')
                    {
                        break;
                    }
                }
                else
                {
                    break;
                };
            };

            string retval = enc.GetString(serverbuff, 0, count);
            Debug.WriteLine("READ:" + retval);
            return retval;
        }

        private static string server = "pop.gmail.com";
        private static string user = "";
        private static string pw = "";
        public static void fMain(string[] args)
        {
            try
            {
                Pop3 obj = new Pop3();
                obj.Connect(server, user, pw);
                ArrayList list = obj.List();
                foreach (Pop3Message msg in list)
                {
                    Pop3Message msg2 = obj.Retrieve(msg);
                    System.Console.WriteLine("Message {0}: {1}",
                        msg2.number, msg2.message);
                }
                obj.Disconnect();
            }
            catch (Pop3Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }
    }
}
