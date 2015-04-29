
namespace ZetaSpamAssassinTest
{
	
	using System;
	using System.Configuration;
	using System.Diagnostics;

	using ZetaSpamAssassin;


	/// <summary>
	/// Console application to test the SpamAssassinXxx classes.
	/// </summary>
	/// <remarks>
	/// Developed 2006-02-16.
	/// For questions and comments, please contact Uwe Keim at
	/// mailto:uwe.keim@zeta-software.de.
	/// Also, see my private webcam and weblog at http://www.magerquark.de
	/// </remarks>
	internal static class Program
	{
		

		/// <summary>
		/// Main entry point.
		/// </summary>
		private static void Main(
			string[] commandLineArgs )
		{
			SpamAssassinProtocol sap = new SpamAssassinProtocol(
				ConfigurationManager.AppSettings["spamAssassinServerName"] );

			if ( true )
			{
				SpamAssassinCheckArgs e = new SpamAssassinCheckArgs();
				e.TextToCheck = @"This is certainly no Spam.";

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