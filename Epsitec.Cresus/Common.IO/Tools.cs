//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.IO
{
	/// <summary>
	/// La classe Tools contient des routines utilitaires liées aux fichiers.
	/// </summary>
	public sealed class Tools
	{
		public delegate void Callback();
		
		public static bool WaitForFileReadable(string name, int max_wait)
		{
			return Tools.WaitForFileReadable (name, max_wait, null);
		}
		
		public static bool WaitForFileReadable(string name, int max_wait, Callback interrupt)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			bool ok   = false;
			int  wait = 0;
			
			for (int i = 5; wait < max_wait; i += 5)
			{
				if (interrupt != null)
				{
					interrupt ();
				}
				
				System.IO.FileStream stream;
				
				try
				{
					stream = System.IO.File.OpenRead (name);
				}
				catch
				{
					System.Threading.Thread.Sleep (i);
					wait += i;
					buffer.Append ('.');
					continue;
				}
				
				stream.Close ();
				ok = true;
				break;
			}
			
			if (buffer.Length > 0)
			{
				if (wait > max_wait)
				{
					System.Diagnostics.Debug.WriteLine ("Timed out waiting for file.");
				}
				else
				{
					System.Diagnostics.Debug.WriteLine ("Waited for file for " + wait + " ms " + buffer.ToString ());
				}
			}
			
			return ok;
		}
	}
}
