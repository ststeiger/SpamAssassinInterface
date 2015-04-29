
namespace ZetaSpamAssassin
{
	
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Net.Sockets;
	// using ZetaLib.Core.Common;
	using System.Diagnostics;



	/// <summary>
	/// Implements an easy-to-use way to communicate with a SpamAssassin
	/// server.
	/// </summary>
	/// <remarks>
	/// See http://spamassassin.apache.org/full/3.1.x/dist/spamd/PROTOCOL
	/// for the full protocol definition.
	/// 
	/// Developed 2006-02-16.
	/// For questions and comments, please contact Uwe Keim at
	/// mailto:uwe.keim@zeta-software.de.
	/// Also, see my private webcam and weblog at http://www.magerquark.de
	/// </remarks>
	public class SpamAssassinProtocolBase
	{
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="serverName">The server to connect to.</param>
		/// <param name="serverPort">The port of the server to connect to. 
		/// The default value is 783.</param>
		public SpamAssassinProtocolBase(
			string serverName,
			int serverPort )
		{
			this.serverName = serverName;
			this.serverPort = serverPort;
		}

		/// <summary>
		/// Constructor. Uses the default port 783.
		/// </summary>
		/// <param name="serverName">The server to connect to.</param>
		public SpamAssassinProtocolBase(
			string serverName )
		{
			this.serverName = serverName;
			this.serverPort = 783;
		}

		

		/// <summary>
		/// The version of the SPAMC client that this class implements.
		/// </summary>
		public readonly string SpamCVersion = "1.2";

		public string ServerName
		{
			get
			{
				return serverName;
			}
			set
			{
				serverName = value;
			}
		}

		public int ServerPort
		{
			get
			{
				return serverPort;
			}
			set
			{
				serverPort = value;
			}
		}

		public string UserName
		{
			get
			{
				return userName;
			}
			set
			{
				userName = value;
			}
		}

		

		
		/// <summary>
		/// High-level function for sending a message to the server
		/// and receiving the response message.
		/// </summary>
		/// <param name="command">The command to send.</param>
		/// <param name="message">The request message to send.</param>
		/// <returns>Returns the received response message.</returns>
		protected string SendMessage(
			string command,
			string message )
		{
			return SendMessageEx(
				command,
				message ).Message;
		}

		/// <summary>
		/// High-level function for sending a message to the server
		/// and receiving the response message.
		/// </summary>
		/// <param name="command">The command to send.</param>
		/// <param name="message">The request message to send.</param>
		/// <returns>Returns the received response message.</returns>
		protected ResponsePacket SendMessageEx(
			string command,
			string message )
		{
			RequestPacket request = new RequestPacket(
				this,
				command,
				message );

			ResponsePacket response = SendRequest(
				request );

			return response;
		}

		
		
		private string serverName;
		private int serverPort;
		private string userName;

		
		
		/// <summary>
		/// Central function for sending a request and reading the response.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private ResponsePacket SendRequest(
			RequestPacket request )
		{

			LogCentral.Current.LogDebug(string.Format("Sending the following message to SpamAssassin:" ) );
			LogCentral.Current.LogDebug( request.RawPacket );

            
			using ( Socket spamAssassinSocket = new Socket(
                AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp ) 
                )
			{
				spamAssassinSocket.Connect(
					serverName,
					serverPort );

				byte[] messageBuffer =
					Encoding.ASCII.GetBytes(
					request.RawPacket );

				spamAssassinSocket.Send( messageBuffer );

				spamAssassinSocket.Shutdown( SocketShutdown.Send );

				int received;
				string receivedMessage = string.Empty;
				do
				{
					byte[] receiveBuffer = new byte[1024];
					received = spamAssassinSocket.Receive( receiveBuffer );
					LogCentral.Current.LogDebug(
						string.Format(
						"Received {0} bytes from SpamAssassin.", received ) );

					receivedMessage += Encoding.ASCII.GetString(
						receiveBuffer,
						0,
						received );
				}
				while ( received > 0 );

				spamAssassinSocket.Shutdown( SocketShutdown.Both );

				if ( request.Command!=SpamAssassinCommands.Skip &&
					!receivedMessage.StartsWith(
					"SPAMD",
					StringComparison.InvariantCultureIgnoreCase ) )
				{
					LogCentral.Current.LogWarn(
						"The received message does not start with 'SPAMD'." );
				}

				ResponsePacket response = new ResponsePacket(
					this,
					request,
					receivedMessage.Trim() );

				return response;
			}
		}

		
		/// <summary>
		/// Base class for request and response packets.
		/// </summary>
		protected class PacketBase
		{
			
			public PacketBase(
				SpamAssassinProtocolBase owner )
			{
				this.owner = owner;
			}

			
			protected SpamAssassinProtocolBase Owner
			{
				get
				{
					return owner;
				}
			}

			/// <summary>
			/// The raw, unparsed string.
			/// </summary>
			public string RawPacket
			{
				get
				{
					return rawPacket;
				}
				set
				{
					rawPacket = value;
				}
			}

			
			private SpamAssassinProtocolBase owner;
			private string rawPacket;

		}

		
		/// <summary>
		/// A request, initiated from SPAMC to SPAMD.
		/// </summary>
		protected class RequestPacket : PacketBase
		{
			
			public RequestPacket(
				SpamAssassinProtocolBase owner,
				string command,
				string message )
				: base( owner )
			{
				this.command = command;
				this.message = message;

				RawPacket = BuildRawPacket(
					command,
					message );
			}

			
			
			/// <summary>
			/// The command.
			/// </summary>
			public string Command
			{
				get
				{
					return command;
				}
			}

			/// <summary>
			/// The message.
			/// </summary>
			public string Message
			{
				get
				{
					return message;
				}
			}

			
			
			private string BuildRawPacket(
				string command,
				string message )
			{
				StringBuilder sb = new StringBuilder();

				sb.AppendFormat( "{0} SPAMC/{1}\r\n", command, Owner.SpamCVersion );

				if ( !string.IsNullOrEmpty( Owner.UserName ) )
				{
					sb.AppendFormat( "User: {0}\r\n", Owner.UserName );
				}

				sb.AppendFormat( "Content-Length: {0}\r\n\r\n", message.Length );
				sb.AppendFormat( message );

				return sb.ToString();
			}

			
			private string command;
			private string message;

		}

		
		
		/// <summary>
		/// A response, returned from SPAMD to SPAMC.
		/// </summary>
		protected class ResponsePacket : PacketBase
		{
			
			public ResponsePacket(
				SpamAssassinProtocolBase owner,
				RequestPacket associatedRequestPackage,
				string rawPacket )
				: base( owner )
			{
				RawPacket = rawPacket;
				this.associatedRequestPackage = associatedRequestPackage;

				ParseRawPacket( rawPacket );
			}

			
			/// <summary>
			/// The message.
			/// </summary>
			public string Message
			{
				get
				{
					return message;
				}
			}

			public string[] RawLines
			{
				get
				{
					return rawLines;
				}
			}

			public string ProtocolVersion
			{
				get
				{
					return protocolVersion;
				}
			}

			public int ResponseCode
			{
				get
				{
					return responseCode;
				}
			}

			public string ResponseMessage
			{
				get
				{
					return responseMessage;
				}
			}

			
			private void ParseRawPacket(
				string rawPacket )
			{
				if ( !string.IsNullOrEmpty( rawPacket ) )
				{
					rawPacket = rawPacket.Replace( "\r\n", "\n" );
					rawPacket = rawPacket.Replace( "\r", "\n" );

					rawLines = rawPacket.Split( '\n' );

					string[] line1Elements = rawLines[0].Split( ' ' );

					protocolVersion = line1Elements[0].Replace( "SPAMD/", string.Empty );
					responseCode = Convert.ToInt32( line1Elements[1] );

					responseMessage = string.Empty;
					for ( int i = 2; i < line1Elements.Length; ++i )
					{
						responseMessage += " " + line1Elements[i];
					}

					responseMessage = responseMessage.Trim();

					string line1Plus =
						string.Join(
						"\r\n",
						rawLines,
						1,
						rawLines.Length - 1 );
					message = line1Plus.Trim();
				}
			}

			
			
			private string message;
			private string[] rawLines;
			private RequestPacket associatedRequestPackage;

			private string protocolVersion;
			private int responseCode;
			private string responseMessage;
		}
		

	}


}
