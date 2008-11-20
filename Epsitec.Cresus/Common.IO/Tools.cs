//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.IO
{
	/// <summary>
	/// The <c>Tools</c> class contains several file related utility functions.
	/// </summary>
	public static class Tools
	{
		/// <summary>
		/// The <c>ShouldInterruptCallback</c> delegate should return <c>true</c>
		/// to interrupt whatever the caller was doing and return immediately.
		/// </summary>
		public delegate bool ShouldInterruptCallback();


		/// <summary>
		/// Waits for a file to become readable.
		/// </summary>
		/// <param name="path">The file path.</param>
		/// <param name="timeout">The timeout.</param>
		/// <returns>Returns <c>true</c> if the file is readable; otherwise, return <c>false</c>.</returns>
		public static bool WaitForFileReadable(string path, int timeout)
		{
			return Tools.WaitForFileReadable (path, timeout, null);
		}

		/// <summary>
		/// Waits for a file to become readable.
		/// </summary>
		/// <param name="path">The file path.</param>
		/// <param name="timeout">The timeout.</param>
		/// <param name="interrupt">The interrupt callback.</param>
		/// <returns>
		/// Returns <c>true</c> if the file is readable; otherwise, return <c>false</c>.
		/// </returns>
		public static bool WaitForFileReadable(string path, int timeout, ShouldInterruptCallback shouldInterruptCallback)
		{
			bool ok   = false;
			int  wait = 0;
			int  increment = 5;

			for (int i = increment; wait < timeout; i = System.Math.Min (i+increment, 100*increment))
			{
				if (shouldInterruptCallback != null)
				{
					if (shouldInterruptCallback ())
					{
						break;
					}
				}
				
				System.IO.FileStream stream;
				
				try
				{
					stream = System.IO.File.OpenRead (path);
				}
				catch
				{
					System.Threading.Thread.Sleep (i);
					wait += i;
					continue;
				}
				
				stream.Close ();
				ok = true;
				break;
			}
			
			if (wait > 0)
			{
				if (wait > timeout)
				{
					System.Diagnostics.Debug.WriteLine ("Timed out waiting for file.");
				}
				else
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Waited for file for {0} ms.", wait));
				}
			}
			
			return ok;
		}
	}
}
