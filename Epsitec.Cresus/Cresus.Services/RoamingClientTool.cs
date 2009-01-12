//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// Summary description for RoamingClientTool.
	/// </summary>
	public sealed class RoamingClientTool
	{
		public static void CreateDatabase(Remoting.IOperatorService service, long operationId, string databaseName)
		{
			//	Cr�e une base de donn�es locale � partir de la version comprim�e personnalis�e
			//	g�n�r�e par le serveur. Si une base existait d�j� avec ce nom-l�, elle sera tout
			//	simplement supprim�e.
			
			Database.DbAccess         access = Database.DbInfrastructure.CreateDatabaseAccess ("Firebird" /*"FirebirdEmbedded"*/, databaseName);
			
			access.CheckConnection = false;
			
			Database.IDbServiceTools  tools  = Database.DbFactory.CreateDatabaseAbstraction (access).ServiceTools;
			Common.IO.TemporaryFile   temp   = new Common.IO.TemporaryFile ();
			
			string databasePath = tools.GetDatabasePath ();
			string backupPath   = temp.Path;
			
			try
			{
				System.IO.File.Delete (databasePath);
			}
			catch (System.IO.FileNotFoundException ex)
			{
				System.Diagnostics.Debug.WriteLine (ex.Message);
			}
			
			Remoting.ClientIdentity client;
			byte[] compressedData;
			
			service.GetRoamingClientData (operationId, out client, out compressedData);
			
			System.IO.MemoryStream source       = new System.IO.MemoryStream (compressedData);
			System.IO.FileStream   target       = System.IO.File.OpenWrite (backupPath);
			System.IO.Stream       decompressed = Common.IO.Decompression.CreateStream (source);
			
			int totalRead = 0;
			
			for (;;)
			{
				byte[] readBuffer = new byte[1000];
				int    readLength = decompressed.Read (readBuffer, 0, readBuffer.Length);
				
				if (readLength == 0)
				{
					break;
				}
				
				target.Write (readBuffer, 0, readLength);
				totalRead += readLength;
			}
			
			target.Close ();
			decompressed.Close ();
			source.Close ();
			
			System.Diagnostics.Debug.WriteLine ("Retrieved backup as " + backupPath);
			
			tools.Restore (backupPath);
			
			System.Diagnostics.Debug.WriteLine ("Restored backup.");
			
			Common.IO.Tools.WaitForFileReadable (databasePath, 20000);
			
			temp.Dispose ();
			temp = null;
			
			//	Le fichier a �t� recr�� sur le disque, en local, d�comprim�.
			
			access.CheckConnection = true;
			
			using (Database.DbInfrastructure infrastructure = new Database.DbInfrastructure ())
			{
				infrastructure.AttachToDatabase (access);
				infrastructure.SetupRoamingDatabase (client.Id);
			}
		}
	}
}
