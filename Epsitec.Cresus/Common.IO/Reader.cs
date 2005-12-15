namespace Epsitec.Common.IO
{
	/// <summary>
	/// La classe Reader permet de lire la quantité de données demandée, en plusieurs
	/// fois si nécessaire.
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
				//	Il se peut que le Read n'ait pas retourné tout ce
				//	qui lui a été demandé, sans pour autant que la fin
				//	ait été atteinte (par ex. lors de décompression).
				
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
