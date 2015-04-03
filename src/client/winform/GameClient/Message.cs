using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient
{
    public class Message
    {
        private string m_prefix;
        private string m_command;
        private string[] m_parameters;
        private Message()
        {
            m_prefix = null;
            m_command = null;
            m_parameters = null;
        }
        public Message(string prefix, string command, params string[] parameters)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentNullException("command");
            }
            m_prefix = prefix;
            m_command = command;
            m_parameters = parameters;
        }
        public string Prefix
        {
            get
            {
                return m_prefix;
            }
        }
        public string Command
        {
            get
            {
                return m_command;
            }
        }
        public string[] Parameters
        {
            get
            {
                return m_parameters;
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if(!string.IsNullOrEmpty(m_prefix))
            {
                sb.Append(":");
                sb.Append(m_prefix);
                sb.Append(" ");
            }
            sb.Append(m_command);
            if(Parameters.Length > 0)
            {
                for(int i = 0; i < Parameters.Length; i++)
                {
                    if(i == Parameters.Length - 1) //last element
                    {
                        sb.Append(" :");
                        sb.Append(Parameters[i]);
                    }
                    else
                    {
                        sb.Append(" ");
                        sb.Append(Parameters[i]);
                    }
                }
            }
            sb.Append("\r\n");
            return sb.ToString();
        }
        public static Message Parse(string rawMessage)
        {
            Message msg = new Message();
            if (rawMessage.StartsWith(":")) // has prefix
            {
                int indexOfSpace = rawMessage.IndexOf(" ");
                msg.m_prefix = rawMessage.Substring(1, indexOfSpace - 1);
                rawMessage = rawMessage.Substring(indexOfSpace + 1);
            }
            int indexOfCommandSpace = rawMessage.IndexOf(" ");
            if (indexOfCommandSpace != -1)
            {
                msg.m_command = rawMessage.Substring(0, indexOfCommandSpace);
                rawMessage = rawMessage.Substring(indexOfCommandSpace); // reserve this space
                int indexOfTrailing = rawMessage.IndexOf(" :"); // Space+Colon
                if (indexOfTrailing != -1)
                {
                    if (indexOfTrailing == 0)
                    {
                        msg.m_parameters = new string[1];
                        msg.m_parameters[0] = rawMessage.Substring(2);
                    }
                    else
                    {
                        string beforeTrailing = rawMessage.Substring(0, indexOfTrailing);
                        string trailing = rawMessage.Substring(indexOfTrailing + 2);
                        string[] splits = beforeTrailing.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        msg.m_parameters = new string[splits.Length + 1];
                        Array.Copy(splits, msg.m_parameters, splits.Length);
                        msg.m_parameters[msg.m_parameters.Length - 1] = trailing;
                    }
                }
                else
                {
                    string[] splits = rawMessage.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    msg.m_parameters = splits;
                }
            }
            else
            {
                msg.m_command = rawMessage;
                msg.m_parameters = new string[0];
            }
            return msg;
        }
    }
}
