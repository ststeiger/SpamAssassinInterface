namespace ZetaSpamAssassin
{
	
	
	public class SpamAssassinException :
		System.Exception
	{
		
		public SpamAssassinException()
			:
			base()
		{
		}

		public SpamAssassinException(
			string message )
			:
			base( message )
		{
		}

		public SpamAssassinException(
			string message,
            System.Exception innerException)
			:
		base( message, innerException )
		{
		}

		
	}

	
}