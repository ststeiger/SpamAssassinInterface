
namespace ZetaSpamAssassinTest
{
	
	using System.Diagnostics;
	using ZetaSpamAssassin;


    // http://www.codeproject.com/Articles/13106/A-C-Wrapper-for-the-SpamAssassin-Protocol
	internal static class Program
	{


        // http://serverfault.com/questions/183461/how-do-i-allow-outgoing-connections-via-iptables
        // iptables -A INPUT -j ACCEPT
        // iptables -A OUTPUT -j ACCEPT
        
        // Is the service you are running listening only on localhost? Run
        // netstat -ltn
        // If you see a line like 0.0.0.0:2194 then you are ok. 
        // If you see 127.0.0.1:2194 then you are listening only on local connections 
        // (or :::2194 and ::1:2194 respectively for IPv6 addresses, shown as tcp6 lines).
        
        // What are the current iptables rules?
        // iptables -L

        // Options in /etc/default/spamassassin
        // spamd -D --listen 192.168.1.11 --allowed-ips=192.168.1.0/24 --allow-tell


		/// <summary>
		/// Main entry point.
		/// </summary>
		private static void Main(
			string[] commandLineArgs )
		{
			SpamAssassinProtocol sap = new SpamAssassinProtocol(
                System.Configuration.ConfigurationManager.AppSettings["spamAssassinServerName"]);
            
			if ( true )
			{
				SpamAssassinCheckArgs e = new SpamAssassinCheckArgs();
				e.TextToCheck = @"This is certainly no Spam.";
                e.TextToCheck = @"You need penis enlargement !";

				SpamAssassinCheckResult r = sap.ExecuteCheck( e );

				Debug.WriteLine( "IsSpam: " + r.IsSpam );
				Debug.WriteLine( "Score: " + r.Score );
				Debug.WriteLine( "Threshold: " + r.Threshold );
			}

			if ( true )
			{
				SpamAssassinPingArgs e = new SpamAssassinPingArgs();

				SpamAssassinPingResult r = sap.ExecutePing( e );
			}

			if ( true )
			{
				SpamAssassinProcessArgs e = new SpamAssassinProcessArgs();
				e.TextToCheck = @"This is certainly no Spam.";

				SpamAssassinProcessResult r = sap.ExecuteProcess( e );

				Debug.WriteLine( "ProcessedMessage: " + r.ProcessedMessage );
			}

			if ( true )
			{
				SpamAssassinReportArgs e = new SpamAssassinReportArgs();
				e.TextToCheck = @"This is certainly no Spam.";

				SpamAssassinReportResult r = sap.ExecuteReport( e );

				Debug.WriteLine( "IsSpam: " + r.IsSpam );
				Debug.WriteLine( "Score: " + r.Score );
				Debug.WriteLine( "Threshold: " + r.Threshold );
				Debug.WriteLine( "ReportedText: " + r.ReportText );
			}

			if ( true )
			{
				SpamAssassinReportIfSpamArgs e = new SpamAssassinReportIfSpamArgs();
				e.TextToCheck = @"This is certainly no Spam.";

				SpamAssassinReportIfSpamResult r = sap.ExecuteReportIfSpam( e );

				Debug.WriteLine( "IsSpam: " + r.IsSpam );
				Debug.WriteLine( "Score: " + r.Score );
				Debug.WriteLine( "Threshold: " + r.Threshold );
				Debug.WriteLine( "ReportText: " + r.ReportText );
			}

			if ( true )
			{
				SpamAssassinSkipArgs e = new SpamAssassinSkipArgs();

				SpamAssassinSkipResult r = sap.ExecuteSkip( e );
			}

			if ( true )
			{
				SpamAssassinSymbolsArgs e = new SpamAssassinSymbolsArgs();
				e.TextToCheck = @"This is certainly no Spam.";

				SpamAssassinSymbolsResult r = sap.ExecuteSymbols( e );

				Debug.WriteLine( "IsSpam: " + r.IsSpam );
				Debug.WriteLine( "Score: " + r.Score );
				Debug.WriteLine( "Threshold: " + r.Threshold );

				if ( r.SymbolLines != null )
				{
					foreach ( string symbolLine in r.SymbolLines )
					{
						Debug.WriteLine( "Symbol line: " + symbolLine );
					}
				}
			}

			if ( true )
			{
				SpamAssassinTellArgs e = new SpamAssassinTellArgs();
				e.TextToCheck = @"This is certainly no Spam.";

				e.Action = SpamAssassinTellArgs.TellAction.ForgetLearnedMessage;

				SpamAssassinTellResult r = sap.ExecuteTell( e );

				Debug.WriteLine( "DidSet: " + r.DidSet );
				Debug.WriteLine( "DidRemove: " + r.DidRemove );
			}
		}


	}

	
}