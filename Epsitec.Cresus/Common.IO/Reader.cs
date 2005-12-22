//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.IO
{
	/// <summary>
	/// The Reader class retries a Read until the required amount of data has
	/// been read or the end of the stream has been reached. This hack is needed
	/// since the decompressing streams Read method may return less than what
	/// was requested, even if the end of the stream has not yet been reached.
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
