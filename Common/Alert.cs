using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public enum Command { Stop = 0, Start = 1, CancelOffset =2, CancelAll = 3, CancelLimit = 4, Status = 5 };
    public enum AlertType { PriceSignal = 0, VolumeSignal = 1, Command = 2 };

    public interface IController
    {
        void handleAlert(List<Alert> alerts);
    }

    # region alert
    public class Alert
    {
        DateTime timestamp;

        public DateTime Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }
        string contract;

        public string Contract
        {
            get { return contract; }
            set { contract = value; }
        }
        double last;

        public double Last
        {
            get { return last; }
            set { last = value; }
        }
        string order;

        public string Order
        {
            get { return order; }
            set { order = value; }
        }
        double bid;

        public double Bid
        {
            get { return bid; }
            set { bid = value; }
        }
        double ask;

        public double Ask
        {
            get { return ask; }
            set { ask = value; }
        }
        long volume;

        public long Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        string subject;
        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        string sender;
        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        AlertType type;
        public AlertType Type
        {
            get { return type; }
            set { type = value; }
        }

        List<object> commands;
        public List<object> Commands
        {
            get { return commands; }
            set { commands = value; }
        }

        public static Alert parseAlert(string record, string subject, string senderAddress, DateTime deliveryDate)
        {
            Alert alert = new Alert();

            alert.Subject = subject;
            alert.Sender = senderAddress;

            if (subject.ToLower() == "cmd")
            {
                alert.Type = AlertType.Command;
                alert.timestamp = deliveryDate;
                string[] fields = record.Split(' ','\r','\n','\t');
                alert.Commands = new List<object>();
                foreach (string fld in fields)
                {
                    if (fld.ToLower() == "stop")
                    {
                        alert.Commands.Add(Command.Stop);
                    }
                    else if (fld.ToLower() == "start")
                    {
                        alert.Commands.Add(Command.Start);
                    }
                    else if (fld.ToLower() == "canceloffset")
                    {
                        alert.Commands.Add(Command.CancelOffset);
                    }
                    else if (fld.ToLower() == "cancellimit")
                    {
                        alert.Commands.Add(Command.CancelLimit);
                    }
                    else if (fld.ToLower() == "cancelall")
                    {
                        alert.Commands.Add(Command.CancelAll);
                    }
                    else if (fld.ToLower() == "status")
                    {
                        alert.Commands.Add(Command.Status);
                    }
                    else
                    {
                        alert.Commands.Add(fld);
                    }
                }
            }
            else if (subject.ToLower() == "volume")
            {
                alert.Type = AlertType.VolumeSignal;
                string[] fields = record.Split(';');
                for (int i = 0; i < fields.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            alert.timestamp = DateTime.Parse(fields[i]);
                            break;

                        case 1:
                            alert.contract = fields[i];
                            break;

                        case 2:
                            alert.volume = long.Parse(fields[i]);
                            break;

                        case 3:
                            alert.order = fields[i];    // this is yet to be defined, but is some qualifier on volume
                            break;

                        default:
                            throw new Exception("too many fields in alert");

                    }
                }
            }
            else
            {
                alert.Type = AlertType.PriceSignal;
                string[] fields = record.Split(';');
                for (int i = 0; i < fields.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            alert.timestamp = DateTime.Parse(fields[i]);
                            break;

                        case 1:
                            alert.contract = fields[i];
                            break;

                        case 2:
                            alert.last = double.Parse(fields[i]);
                            break;

                        case 3:
                            alert.order = fields[i].ToLower().Trim();
                            if (alert.order != "buy" && alert.order != "sell")
                                throw new Exception("buy or sell not found in alert");
                            break;

                        case 4:
                            alert.bid = double.Parse(fields[i]);
                            break;

                        case 5:
                            alert.ask = double.Parse(fields[i]);
                            break;

                        case 6:
                            alert.volume = long.Parse(fields[i]);
                            break;

                        default:
                            throw new Exception("too many fields in alert");

                    }
                }
            }
            return alert;
        }
        public override string ToString()
        {
            if (Type == AlertType.PriceSignal)
                return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}",
                Type,
                Sender,
                Subject,
                timestamp,
                contract,
                last,
                order,
                Bid,
                Ask,
                Volume
                );
            else if (Type == AlertType.VolumeSignal)
                return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}",
                Type,
                Sender,
                Subject,
                timestamp,
                contract,
                order,
                Volume
                );
            else if (Type == AlertType.Command)
            {
                StringBuilder sb = new StringBuilder();
                foreach (object cmd in Commands)
                {
                    sb.Append(cmd);
                    sb.Append(" ");
                }
                return string.Format("{0}, {1}, {2}, {3}, {4}",
                Type,
                Sender,
                Subject,
                timestamp,
                sb
                );

            }
            else return "n/a";
        }
    }
    #endregion
}
