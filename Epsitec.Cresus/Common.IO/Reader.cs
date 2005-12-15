namespace Epsitec.Common.IO
{
	/// <summary>
	/// La classe Reader permet de lire la quantit� de donn�es demand�e, en plusieurs
	/// fois si n�cessaire.
	/// </summary>
	public sealed class Reader
	{
		private Reader()
		{
		}
		
		
		public static int Read(System.IO.Stream stream, byte[] buffer, int offset, int length)
		{
			if (length == 0)
			{
				return 0;
			}
			
			int read  = stream.Read (buffer, offset, length);
			int total = read;
			
			while (total < length)
			{
				//	Il se peut que le Read n'ait pas retourn� tout ce
				//	qui lui a �t� demand�, sans pour autant que la fin
				//	ait �t� atteinte (par ex. lors de d�compression).
				
				//	Dans ce cas, il faut tenter de lire la suite par
				//	petits morceaux :
				
				read = stream.Read (buffer, offset + total, length - total);
				
				if (read == 0)
				{
					break;
				}
				
				total += read;
			}
			
			return total;
		}
	}
}
