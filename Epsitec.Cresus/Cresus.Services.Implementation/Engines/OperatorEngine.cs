//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe OperatorEngine gère la création de bases de données "client"
	/// et leur durée de vie.
	/// </summary>
	internal sealed class OperatorEngine : AbstractServiceEngine, Remoting.IOperatorService
	{
		public OperatorEngine(Engine engine)
			: base (engine)
		{
		}


		public override System.Guid GetServiceId()
		{
			return RemotingServices.OperatorServiceId;
		}
		
		#region IOperatorService Members

		public void CreateRoamingClient(string name, out Remoting.ProgressInformation operation)
		{
			//	Démarre, de manière asynchrone, la création d'une copie comprimée de la base
			//	de données du serveur.

			operation = new CreateRoamingClientOperation (this, name).GetProgressInformation ();
		}
		
		public void GetRoamingClientData(long operationId, out Remoting.ClientIdentity client, out byte[] compressed_data)
		{
			//	Récupère la base de données sous sa forme comprimée.
			
			CreateRoamingClientOperation op = OperationManager.Resolve<CreateRoamingClientOperation> (operationId);
			
			if (op == null)
			{
				throw new System.ArgumentException ("Operation mismatch.");
			}
			
			op.WaitForProgress (100);
			
			Engine.ThrowExceptionBasedOnStatus (op.ProgressState);
			
			
			//	Récupère les données qui attendent le client :
			
			client          = new Remoting.ClientIdentity (op.ClientName, op.ClientId);
			compressed_data = op.CompressedData;
		}
		#endregion
		
		#region CreateRoamingClientOperation Class
		/// <summary>
		/// La classe CreateRoamingClientOperation crée une copie de la base de données
		/// active, la comprime et remplit un buffer avec ces données pour pouvoir les
		/// retourner à l'appelant.
		/// </summary>
		private class CreateRoamingClientOperation : Remoting.AbstractStepThreadedOperation
		{
			public CreateRoamingClientOperation(OperatorEngine oper, string name)
			{
//-				System.Diagnostics.Debugger.Break ();
				this.oper = oper;
				this.client_name = name;
				this.Start ();
			}
			
			
			public byte[]						CompressedData
			{
				get
				{
					return this.data;
				}
			}
			
			public int							ClientId
			{
				get
				{
					return this.client_id;
				}
			}
			
			public string						ClientName
			{
				get
				{
					return this.client_name;
				}
			}
			
			
			protected override void ProcessOperation()
			{
				try
				{
					string path = Epsitec.Common.Support.Globals.Directories.CommonAppDataRevision;

					this.temp = new Epsitec.Common.IO.TemporaryFile (path);
					
					this.Add (this.Step_CreateClient);
					this.Add (this.Step_CopyDatabase);
					this.Add (this.Step_CompressDatabase);
					this.Add (this.Step_Finished);
					
					base.ProcessOperation ();
				}
				finally
				{
					if (this.temp != null)
					{
						this.temp.Delete ();
						this.temp = null;
					}
					
					System.Diagnostics.Debug.WriteLine ("Operator: operation thread exited.");
				}
			}
			
			
			private void Step_CreateClient()
			{
				Database.DbInfrastructure infrastructure = this.oper.engine.Orchestrator.Infrastructure;
				Database.DbClientManager  client_manager = infrastructure.ClientManager;
				
				using (Database.DbTransaction transaction = infrastructure.BeginTransaction (Database.DbTransactionMode.ReadWrite))
				{
					infrastructure.Logger.CreatePermanentEntry (transaction);
					
					this.client_id = client_manager.CreateAndInsertNewClient (this.client_name).ClientId;
					
					client_manager.PersistToBase (transaction);
					
					transaction.Commit ();
				}
			}
			
			private void Step_CopyDatabase()
			{
				Database.DbInfrastructure infrastructure = this.oper.engine.Orchestrator.Infrastructure;
				Database.IDbServiceTools  tools          = infrastructure.DefaultDbAbstraction.ServiceTools;
				
				tools.Backup (this.temp.Path);
				
				if (! Common.IO.Tools.WaitForFileReadable (this.temp.Path, 10*1000, () => this.IsCancelRequested))
				{
					throw new System.IO.IOException ("File cannot be opened in read mode within specified delay.");
				}
			}
			
			private void Step_CompressDatabase()
			{
				System.Diagnostics.Debug.WriteLine ("Compressing...");
				
				System.IO.FileStream   source     = System.IO.File.OpenRead (this.temp.Path);
				System.IO.MemoryStream memory     = new System.IO.MemoryStream ();
				System.IO.Stream       compressed = Common.IO.Compression.CreateDeflateStream (memory, 9);
				
				int total_read = 0;
				
				for (;;)
				{
					byte[] buffer = new byte[1000];
					int read = source.Read (buffer, 0, buffer.Length);
					
					if (read == 0)
					{
						break;
					}
					
					compressed.Write (buffer, 0, read);
					total_read += read;
				}
				
				compressed.Close ();
				source.Close ();
				memory.Close ();
				
				this.data = memory.ToArray ();
			}
			
			private void Step_Finished()
			{
				System.Diagnostics.Debug.WriteLine ("Ready for data transfer.");
			}
			
			
			
			Epsitec.Common.IO.TemporaryFile		temp;
			OperatorEngine						oper;
			byte[]								data;
			int									client_id;
			string								client_name;
		}
		#endregion
	}
}
