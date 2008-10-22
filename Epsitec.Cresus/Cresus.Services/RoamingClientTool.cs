//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			//	Cr�e une base de donn�es locale � partir de la version comprim�e personnalis�e
			//	g�n�r�e par le serveur. Si une base existait d�j� avec ce nom-l�, elle sera tout
			//	simplement supprim�e.
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			Database.DbAccess         access = Database.DbInfrastructure.CreateDatabaseAccess ("Firebird" /*"FirebirdEmbedded"*/, database_name);
			
			access.CheckConnection = false;
			
			Database.IDbServiceTools  tools  = Database.DbFactory.CreateDatabaseAbstraction (access).ServiceTools;
			Common.IO.TemporaryFile   temp   = new Common.IO.TemporaryFile ();
			
			string database_path = tools.GetDatabasePath ();
			string backup_path   = temp.Path;
			
			try
			{
				System.IO.File.Delete (database_path);
			}
			catch (System.IO.FileNotFoundException ex)
			{
				System.Diagnostics.Debug.WriteLine (ex.Message);
			}
			
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
			
			Common.IO.Tools.WaitForFileReadable (database_path, 20000);
			
			temp.Dispose ();
			temp = null;
			
			//	Le fichier a �t� recr�� sur le disque, en local, d�comprim�.
			
			access.CheckConnection = true;
			
			using (Database.DbInfrastructure infrastructure = new Database.DbInfrastructure ())
			{
				infrastructure.AttachToDatabase (access);
				infrastructure.SetupRoamingDatabase (client.ClientId);
			}
		}
	}
}
