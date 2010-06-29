//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		public ProgressInformation CreateRoamingClient(string name)
		{
			//	Démarre, de manière asynchrone, la création d'une copie comprimée de la base
			//	de données du serveur.

			return new CreateRoamingClientOperation (this, name).GetProgressInformation ();
		}
		
		public bool GetRoamingClientData(long operationId, out Remoting.ClientIdentity client, out byte[] compressed_data)
		{
			//	Récupère la base de données sous sa forme comprimée.
			
			CreateRoamingClientOperation op = OperationManager.Resolve<CreateRoamingClientOperation> (operationId);
			
			if (op == null)
			{
				client = Remoting.ClientIdentity.Empty;
				compressed_data = null;
				return false;
			}
			
			op.WaitForProgress (100);
			
			Engine.ThrowExceptionBasedOnStatus (op.ProgressState);
			
			
			//	Récupère les données qui attendent le client :
			
			client          = new Remoting.ClientIdentity (op.ClientName, op.ClientId);
			compressed_data = op.CompressedData;

			return true;
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
				this.oper = oper;
				this.clientName = name;
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
					return this.clientId;
				}
			}
			
			public string						ClientName
			{
				get
				{
					return this.clientName;
				}
			}
			
			
			protected override void ProcessOperation()
			{
				try
				{
					string path = Epsitec.Common.Support.Globals.Directories.CommonAppDataRevision;

					this.temp = new Epsitec.Common.IO.TemporaryFile (path);
					
					this.Add (this.StepCreateClient);
					this.Add (this.StepCopyDatabase);
					this.Add (this.StepCompressDatabase);
					this.Add (this.StepFinished);
					
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
			
			
			private void StepCreateClient()
			{
				Database.DbInfrastructure infrastructure = this.oper.Engine.Orchestrator.Infrastructure;
				Database.DbClientManager  clientManager = infrastructure.ClientManager;
				
				using (Database.DbTransaction transaction = infrastructure.BeginTransaction (Database.DbTransactionMode.ReadWrite))
				{
					infrastructure.Logger.CreatePermanentEntry (transaction);
					
					this.clientId = clientManager.CreateAndInsertNewClient (this.clientName).ClientId;
					
					clientManager.PersistToBase (transaction);
					
					transaction.Commit ();
				}
			}
			
			private void StepCopyDatabase()
			{
				Database.DbInfrastructure infrastructure = this.oper.Engine.Orchestrator.Infrastructure;
				Database.IDbServiceTools  tools          = infrastructure.DefaultDbAbstraction.ServiceTools;
				
				tools.Backup (this.temp.Path);
				
				if (! Common.IO.Tools.WaitForFileReadable (this.temp.Path, 10*1000, () => this.IsCancelRequested))
				{
					throw new System.IO.IOException ("File cannot be opened in read mode within specified delay.");
				}
			}
			
			private void StepCompressDatabase()
			{
				System.Diagnostics.Debug.WriteLine ("Compressing...");
				
				System.IO.FileStream   source     = System.IO.File.OpenRead (this.temp.Path);
				System.IO.MemoryStream memory     = new System.IO.MemoryStream ();
				System.IO.Stream       compressed = Common.IO.Compression.CreateDeflateStream (memory, 9);
				
				int totalRead = 0;
				
				for (;;)
				{
					byte[] buffer = new byte[1000];
					int read = source.Read (buffer, 0, buffer.Length);
					
					if (read == 0)
					{
						break;
					}
					
					compressed.Write (buffer, 0, read);
					totalRead += read;
				}

				this.data = memory.ToArray ();

				compressed.Close ();
				source.Close ();
				memory.Close ();
			}
			
			private void StepFinished()
			{
				System.Diagnostics.Debug.WriteLine ("Ready for data transfer.");
			}
			
			
			
			private Epsitec.Common.IO.TemporaryFile		temp;
			private OperatorEngine						oper;
			private byte[]								data;
			private int									clientId;
			private string								clientName;
		}
		#endregion
	}
}
