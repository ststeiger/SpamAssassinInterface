
namespace ZetaSpamAssassin
{
	

	using System.Net;
	using System.Globalization;
	using System.Collections;
	


	/// <summary>
	/// Implements an easy-to-use way to communicate with a SpamAssassin
	/// server.
	/// </summary>
	/// <remarks>
	/// See http://spamassassin.apache.org/full/3.1.x/dist/spamd/PROTOCOL
	/// for the full protocol definition.
	public sealed class SpamAssassinProtocol :
		SpamAssassinProtocolBase
	{
		

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="serverName">The server to connect to.</param>
		/// <param name="serverPort">The port of the server to connect to. 
		/// The default value is 783.</param>
		public SpamAssassinProtocol(
			string serverName,
			int serverPort )
			:
			base( serverName, serverPort )
		{
		}

		/// <summary>
		/// Constructor. Uses the default port 783.
		/// </summary>
		/// <param name="serverName">The server to connect to.</param>
		public SpamAssassinProtocol(
			string serverName )
			:
			base( serverName )
		{
		}


		/// <summary>
		/// Execute the Check command.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public SpamAssassinCheckResult ExecuteCheck(
			SpamAssassinCheckArgs e )
		{
			string requestMessage = PrepareCheckRequestMessage( e );

			// --

			ResponsePacket responsePacket =
				SendMessageEx( SpamAssassinCommands.Check, requestMessage );
			CheckThrowResponsePacket( responsePacket );

			// --

			string[] additionalLines;
			SpamAssassinCheckResult result = new SpamAssassinCheckResult();

			InterpretCheckResponseMessage(
				responsePacket.Message,
				out additionalLines,
				result );

			return result;
		}

		/// <summary>
		/// Execute the Symbols command.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public SpamAssassinSymbolsResult ExecuteSymbols(
			SpamAssassinSymbolsArgs e )
		{
			string requestMessage = PrepareCheckRequestMessage( e );

			// --

			ResponsePacket responsePacket =
				SendMessageEx( SpamAssassinCommands.Symbols, requestMessage );
			CheckThrowResponsePacket( responsePacket );

			// --

			string[] additionalLines;
			SpamAssassinSymbolsResult result =
				new SpamAssassinSymbolsResult();

			InterpretCheckResponseMessage(
				responsePacket.Message,
				out additionalLines,
				result );

			// Remove empty line at the beginning.
			additionalLines = SplitLines( JoinLines( additionalLines ) );

			result.SymbolLines = additionalLines[0].Split(
				new char[] { ',' },
				System.StringSplitOptions.RemoveEmptyEntries );

			return result;
		}

		/// <summary>
		/// Execute the Report command.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public SpamAssassinReportResult ExecuteReport(
			SpamAssassinReportArgs e )
		{
			string requestMessage = PrepareCheckRequestMessage( e );

			// --

			ResponsePacket responsePacket =
				SendMessageEx( SpamAssassinCommands.Report, requestMessage );
			CheckThrowResponsePacket( responsePacket );

			// --

			string[] additionalLines;
			SpamAssassinReportResult result =
				new SpamAssassinReportResult();

			InterpretCheckResponseMessage(
				responsePacket.Message,
				out additionalLines,
				result );

			result.ReportText = JoinLines( additionalLines );

			return result;
		}

		/// <summary>
		/// Execute the ReportIfSpam command.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public SpamAssassinReportIfSpamResult ExecuteReportIfSpam(
			SpamAssassinReportIfSpamArgs e )
		{
			string requestMessage = PrepareCheckRequestMessage( e );

			// --

			ResponsePacket responsePacket =
				SendMessageEx( SpamAssassinCommands.ReportIfSpam, requestMessage );
			CheckThrowResponsePacket( responsePacket );

			// --

			string[] additionalLines;
			SpamAssassinReportIfSpamResult result =
				new SpamAssassinReportIfSpamResult();

			InterpretCheckResponseMessage(
				responsePacket.Message,
				out additionalLines,
				result );

			if ( additionalLines != null && additionalLines.Length > 0 )
			{
				result.ReportText = JoinLines( additionalLines );
			}

			return result;
		}

		/// <summary>
		/// Execute the Skip command.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public SpamAssassinSkipResult ExecuteSkip(
			SpamAssassinSkipArgs e )
		{
			ResponsePacket responsePacket =
				SendMessageEx( SpamAssassinCommands.Skip, string.Empty );
			CheckThrowResponsePacket( responsePacket );

			SpamAssassinSkipResult result =
				new SpamAssassinSkipResult();

			return result;
		}

		/// <summary>
		/// Execute the Ping command.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public SpamAssassinPingResult ExecutePing(
			SpamAssassinPingArgs e )
		{
			ResponsePacket responsePacket =
				SendMessageEx( SpamAssassinCommands.Ping, string.Empty );
			CheckThrowResponsePacket( responsePacket );

			if ( string.Compare( responsePacket.ResponseMessage, "PONG", true ) != 0 )
			{
				throw new SpamAssassinException(
					string.Format(
					"The PING response from SPAMD was '{0}' but is expected to be 'PONG'.",
					responsePacket.ResponseMessage ) );
			}
			else
			{
				SpamAssassinPingResult result = new SpamAssassinPingResult();

				return result;
			}
		}


		/// <summary>
		/// Execute the Process command.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public SpamAssassinProcessResult ExecuteProcess(
			SpamAssassinProcessArgs e )
		{
			string requestMessage = PrepareCheckRequestMessage( e );

			// --

			ResponsePacket responsePacket = SendMessageEx( SpamAssassinCommands.Process, requestMessage );
			CheckThrowResponsePacket( responsePacket );

			// --

			string[] lines = SplitLines( responsePacket.Message );
			lines = RemoveLine( lines, 0 );

			SpamAssassinProcessResult result = new SpamAssassinProcessResult();

			result.ProcessedMessage = JoinLines( lines );

			return result;
		}

		/// <summary>
		/// Execute the Tell command.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public SpamAssassinTellResult ExecuteTell(
			SpamAssassinTellArgs e )
		{
			string messageClassString = "spam";
			string setString = string.Empty;
			string removeString = string.Empty;

			if ( (e.SetLocation & SpamAssassinTellArgs.Location.Local) != 0 )
			{
				setString += "local";
			}
			if ( (e.SetLocation & SpamAssassinTellArgs.Location.Remote) != 0 )
			{
				if ( !string.IsNullOrEmpty( setString ) )
				{
					setString += ", ";
				}
				setString += "remote";
			}

			if ( (e.RemoveLocation & SpamAssassinTellArgs.Location.Local) != 0 )
			{
				removeString += "local";
			}
			if ( (e.RemoveLocation & SpamAssassinTellArgs.Location.Remote) != 0 )
			{
				if ( !string.IsNullOrEmpty( removeString ) )
				{
					removeString += ", ";
				}
				removeString += "remote";
			}



            System.Text.StringBuilder sb = new System.Text.StringBuilder();

			sb.AppendLine( string.Format( "Message-class: {0}", messageClassString ) );
			if ( !string.IsNullOrEmpty( setString ) )
			{
				sb.AppendLine( string.Format( "Set: {0}", setString ) );
			}
			if ( !string.IsNullOrEmpty( removeString ) )
			{
				sb.AppendLine( string.Format( "Remove: {0}", removeString ) );
			}

			string requestMessage = PrepareCheckRequestMessage( e );
			sb.AppendLine();
			sb.AppendLine( requestMessage );

			// --

			ResponsePacket responsePacket =
				SendMessageEx( SpamAssassinCommands.Tell, sb.ToString() );
			CheckThrowResponsePacket( responsePacket );

			// --

			SpamAssassinTellResult result = new SpamAssassinTellResult();

			result.DidSet = responsePacket.Message.IndexOf(
				"DidSet",
                System.StringComparison.InvariantCultureIgnoreCase) >= 0;
			result.DidRemove = responsePacket.Message.IndexOf(
				"DidRemove",
                System.StringComparison.InvariantCultureIgnoreCase) >= 0;

			return result;
		}

		
		/// <summary>
		/// Internal helper.
		/// </summary>
		private string PrepareCheckRequestMessage(
			SpamAssassinCheckArgs e )
		{
			// Create mini-RFC822 message and translate it to DOS format for spamd.
			string requestMessage =
@"From {SenderEMailAddress} {EMailDateUTC}
Received: from {SenderHostName} ({SenderHostAddress}) by {ServerHostName} with HTTP via ZetaSoftware;
	{EMailDateRFC2822}
From: {SenderEMailName} <{ReceiverEMailAddress}>
Date: {EMailDateRFC2822}
Subject: ZetaSoftware comment
To: {ReceiverEMailAddress}

{TextToCheck}";

			requestMessage = requestMessage.Replace( "{SenderEMailName}", e.SenderEMailName );
			requestMessage = requestMessage.Replace( "{SenderEMailAddress}", e.SenderEMailAddress );
			requestMessage = requestMessage.Replace( "{EMailDateUTC}", e.EMailDate.ToUniversalTime().ToString( "ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture ) );
			requestMessage = requestMessage.Replace( "{SenderHostName}", e.SenderHostName );
			requestMessage = requestMessage.Replace( "{SenderHostAddress}", e.SenderHostAddress );
			requestMessage = requestMessage.Replace( "{ServerHostName}", e.ServerHostName );
			requestMessage = requestMessage.Replace( "{EMailDateRFC2822}", e.EMailDate.ToString( "r" ) );
			requestMessage = requestMessage.Replace( "{ReceiverEMailAddress}", e.ReceiverEMailAddress );
			requestMessage = requestMessage.Replace( "{TextToCheck}", e.TextToCheck );

			return requestMessage;
		}


		/// <summary>
		/// Internal helper.
		/// </summary>
		private void InterpretCheckResponseMessage(
			string responseMessage,
			out string[] additionalLines,
			SpamAssassinCheckResult result )
		{
			string[] lines = SplitLines( responseMessage );

			string[] firstLineColumns = lines[0].Split( ' ' );

			string spamdFlag = firstLineColumns[1];
			string spamdScore = firstLineColumns[3];
			string spamdThreshold = firstLineColumns[5];

			// --

			result.IsSpam = string.Compare( spamdFlag, "True", true ) == 0;
			result.Score =
                System.Convert.ToDouble(
				spamdScore, CultureInfo.InvariantCulture );
			result.Threshold =
                System.Convert.ToDouble(
				spamdThreshold, CultureInfo.InvariantCulture );

			// --

			ArrayList rawAdditionalLines = new ArrayList();

			for ( int index = 1; index < lines.Length; index++ )
			{
				rawAdditionalLines.Add( lines[index] );
			}

			additionalLines =
				(string[])rawAdditionalLines.ToArray(
				typeof( string ) );
		}

		/// <summary>
		/// Line-operation helper.
		/// </summary>
		private string JoinLines( string[] lines )
		{
			if ( lines == null || lines.Length <= 0 )
			{
				return string.Empty;
			}
			else
			{
				return string.Join( "\r\n", lines ).Trim();
			}
		}

		/// <summary>
		/// Line-operation helper.
		/// </summary>
		private string[] SplitLines(
			string text )
		{
			if ( string.IsNullOrEmpty( text ) )
			{
				return new string[] { };
			}
			else
			{
				text = text.Replace( "\r\n", "\n" );
				text = text.Replace( "\r", "\n" );

				string[] lines = text.Split( '\n' );
				return lines;
			}
		}

		/// <summary>
		/// Line-operation helper.
		/// </summary>
		private string[] RemoveLine(
			string[] lines,
			int indexToRemove )
		{
			if ( lines == null || lines.Length <= indexToRemove )
			{
				return lines;
			}
			else
			{
				ArrayList list = new ArrayList( lines );
				list.RemoveAt( indexToRemove );

				if ( list.Count <= 0 )
				{
					return new string[] { };
				}
				else
				{
					return (string[])list.ToArray( typeof( string ) );
				}
			}
		}

		/// <summary>
		/// Throws an exception if a code!=0 is returned.
		/// </summary>
		private void CheckThrowResponsePacket(
			ResponsePacket responsePacket )
		{
			if ( responsePacket.ResponseCode != 0 )
			{
				throw new SpamAssassinException(
					string.Format(
					"{0} ({1})",
					responsePacket.ResponseMessage,
					responsePacket.ResponseCode ) );
			}
		}

	}

	
	/// <summary>
	/// Parameters to the execute function.
	/// </summary>
	public class SpamAssassinCheckArgs
	{
		public string SenderEMailName = "zeta software GmbH";
		public string SenderEMailAddress = "info@zeta-software.de";
        public System.DateTime EMailDate = System.DateTime.Now;
		public string SenderHostName = Dns.GetHostName();
		public string SenderHostAddress = Dns.GetHostEntry( Dns.GetHostName() ).AddressList[0].ToString();
		public string ServerHostName = Dns.GetHostName();
		public string ReceiverEMailAddress = "info@zeta-software.de";
		public string TextToCheck;
	}

	/// <summary>
	/// Result from the execute function.
	/// </summary>
	public class SpamAssassinCheckResult
	{
		public bool IsSpam;
		public double Score;
		public double Threshold;
	}

	
	/// <summary>
	/// Parameters to the execute function.
	/// </summary>
	public class SpamAssassinSymbolsArgs :
		SpamAssassinCheckArgs
	{ }

	/// <summary>
	/// Result from the execute function.
	/// </summary>
	public class SpamAssassinSymbolsResult :
		SpamAssassinCheckResult
	{
		public string[] SymbolLines;
	}


	/// <summary>
	/// Parameters to the execute function.
	/// </summary>
	public class SpamAssassinReportArgs :
		SpamAssassinCheckArgs
	{ }

	/// <summary>
	/// Result from the execute function.
	/// </summary>
	public class SpamAssassinReportResult :
		SpamAssassinCheckResult
	{
		public string ReportText;
	}

	
	/// <summary>
	/// Parameters to the execute function.
	/// </summary>
	public class SpamAssassinReportIfSpamArgs :
		SpamAssassinCheckArgs
	{ }

	/// <summary>
	/// Result from the execute function.
	/// </summary>
	public class SpamAssassinReportIfSpamResult :
		SpamAssassinCheckResult
	{
		public string ReportText;
	}

	/// <summary>
	/// Parameters to the execute function.
	/// </summary>
	public class SpamAssassinSkipArgs
	{ }

	/// <summary>
	/// Result from the execute function.
	/// </summary>
	public class SpamAssassinSkipResult
	{ }



	/// <summary>
	/// Parameters to the execute function.
	/// </summary>
	public class SpamAssassinPingArgs
	{ }

	/// <summary>
	/// Result from the execute function.
	/// </summary>
	public class SpamAssassinPingResult
	{ }

	

	/// <summary>
	/// Parameters to the execute function.
	/// </summary>
	public class SpamAssassinProcessArgs :
		SpamAssassinCheckArgs
	{ }



	/// <summary>
	/// Result from the execute function.
	/// </summary>
	public class SpamAssassinProcessResult
	{
		public string ProcessedMessage;
	}

	

	/// <summary>
	/// Parameters to the execute function.
	/// </summary>
	public class SpamAssassinTellArgs :
		SpamAssassinCheckArgs
	{
		/// <summary>
		/// High-level interface to the Location enumeration.
		/// </summary>
		public enum TellAction
		{
			LearnMessageAsSpam,
			ForgetLearnedMessage,
			ReportSpamMessage,
			RevokeHamMessage
		}


        [System.Flags]
		public enum Location
		{
			Local = 0x01,
			Remote = 0x02
		}

		/// <summary>
		/// High-level interface to the Location enumeration.
		/// </summary>
		public TellAction Action
		{
			set
			{
				switch ( value )
				{
					case TellAction.LearnMessageAsSpam:
						SetLocation = Location.Local;
						RemoveLocation = 0;
						break;
					case TellAction.ForgetLearnedMessage:
						SetLocation = 0;
						RemoveLocation = Location.Local;
						break;
					case TellAction.ReportSpamMessage:
						SetLocation = Location.Local | Location.Remote;
						RemoveLocation = 0;
						break;
					case TellAction.RevokeHamMessage:
						SetLocation = Location.Local;
						RemoveLocation = Location.Remote;
						break;

					default:
                        throw new System.Exception(string.Format("Unknown TellAction '{0}'.", value));
						// Debug.Assert( false, string.Format( "Unknown TellAction '{0}'.", value )    );
				}
			}
		}

		public Location SetLocation;
		public Location RemoveLocation;

	}

	/// <summary>
	/// Result from the execute function.
	/// </summary>
	public class SpamAssassinTellResult
	{
		public bool DidSet;
		public bool DidRemove;
	}


}
