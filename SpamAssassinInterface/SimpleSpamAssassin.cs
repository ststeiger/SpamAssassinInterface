
namespace SpamAssassinInterface
{
    
    public class SimpleSpamAssassin
    {


        public class RuleResult
        {
            public double Score = 0;
            public string Rule = "";
            public string Description = "";

            public RuleResult() { }
            public RuleResult(string line)
            {
                Score = double.Parse(line.Substring(0, line.IndexOf(" ")).Trim());
                line = line.Substring(line.IndexOf(" ") + 1);
                Rule = line.Substring(0, 23).Trim();
                Description = line.Substring(23).Trim();
            }
        }


        public static System.Collections.Generic.List<RuleResult> GetReport(string serverIP, string message)
        {
            string command = "REPORT";

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("{0} SPAMC/1.2\r\n", command);
            sb.AppendFormat("Content-Length: {0}\r\n\r\n", message.Length);
            sb.AppendFormat(message);

            byte[] messageBuffer = System.Text.Encoding.ASCII.GetBytes(sb.ToString());

            using (System.Net.Sockets.Socket spamAssassinSocket = new System.Net.Sockets.Socket(
                System.Net.Sockets.AddressFamily.InterNetwork
                , System.Net.Sockets.SocketType.Stream
                , System.Net.Sockets.ProtocolType.Tcp))
            {
                spamAssassinSocket.Connect(serverIP, 783);
                spamAssassinSocket.Send(messageBuffer);
                spamAssassinSocket.Shutdown(System.Net.Sockets.SocketShutdown.Send);

                int received;
                string receivedMessage = string.Empty;
                do
                {
                    byte[] receiveBuffer = new byte[1024];
                    received = spamAssassinSocket.Receive(receiveBuffer);
                    receivedMessage += System.Text.Encoding.ASCII.GetString(receiveBuffer, 0, received);
                }
                while (received > 0);

                spamAssassinSocket.Shutdown(System.Net.Sockets.SocketShutdown.Both);

                return ParseResponse(receivedMessage);
            }

        }


        private static System.Collections.Generic.List<RuleResult> ParseResponse(string receivedMessage)
        {
            //merge line endings
            receivedMessage = receivedMessage.Replace("\r\n", "\n");
            receivedMessage = receivedMessage.Replace("\r", "\n");
            string[] lines = receivedMessage.Split('\n');

            System.Collections.Generic.List<RuleResult> results = new System.Collections.Generic.List<RuleResult>();
            bool inReport = false;
            foreach (string line in lines)
            {
                if (inReport)
                {
                    try
                    {
                        results.Add(new RuleResult(line.Trim()));
                    }
                    catch
                    {
                        //past the end of the report
                    }
                }

                if (line.StartsWith("---"))
                    inReport = true;
            }

            return results;
        }


    }


}
