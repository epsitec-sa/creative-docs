//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// Summary description for RoamingClientTool.
	/// </summary>
	public sealed class RoamingClientTool
	{
		public static void CreateDatabase(Remoting.IOperatorService service, Remoting.IOperation operation, string database_name)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			Database.DbAccess         access = Database.DbInfrastructure.CreateDbAccess (database_name);
			
			access.Provider        = "FirebirdEmbedded";
			access.CheckConnection = false;
			
			buffer.Append (Common.Support.Globals.Directories.UserAppData);
			buffer.Append (System.IO.Path.DirectorySeparatorChar);
			buffer.Append (database_name);
			
			string backup_path   = buffer.ToString () + ".gbk";
			string database_path = buffer.ToString () + ".firebird";
			
			try
			{
				System.IO.File.Delete (database_path);
			}
			catch (System.IO.FileNotFoundException ex)
			{
				System.Diagnostics.Debug.WriteLine (ex.Message);
			}
			
			Database.IDbServiceTools  tools  = Database.DbFactory.FindDbAbstraction (access).ServiceTools;
			
			Remoting.ClientIdentity client;
			byte[] compressed_data;
			
			service.GetRoamingClientData (operation, out client, out compressed_data);
			
			System.IO.MemoryStream source       = new System.IO.MemoryStream (compressed_data);
			System.IO.FileStream   target       = System.IO.File.OpenWrite (backup_path);
			System.IO.Stream       decompressed = Common.IO.Decompression.CreateStream (source);
			
			int total_read = 0;
			
			for (;;)
			{
				byte[] read_buffer = new byte[1000];
				int    read_length = decompressed.Read (read_buffer, 0, read_buffer.Length);
				
				if (read_length == 0)
				{
					break;
				}
				
				target.Write (read_buffer, 0, read_length);
				total_read += read_length;
			}
			
			target.Close ();
			decompressed.Close ();
			source.Close ();
			
			System.Diagnostics.Debug.WriteLine ("Retrieved backup as " + backup_path);
			
			tools.Restore (backup_path);
			
			System.Diagnostics.Debug.WriteLine ("Restored backup.");
			
			RoamingClientTool.WaitForFileReadable (database_path, 20000);
			
			//	Le fichier a �t� recr�� sur le disque, en local, d�comprim�.
			
			access.CheckConnection = true;
			
			using (Database.DbInfrastructure infrastructure = new Database.DbInfrastructure ())
			{
				infrastructure.AttachDatabase (access);
				infrastructure.SetupRoamingDatabase (client.ClientId);
			}
		}
		
		private static bool WaitForFileReadable(string name, int max_wait)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			bool ok = false;
			int wait = 0;
			
			for (int i = 5; wait < max_wait; i += 5)
			{
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
