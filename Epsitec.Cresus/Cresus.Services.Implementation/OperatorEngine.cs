//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// Summary description for OperatorEngine.
	/// </summary>
	internal sealed class OperatorEngine : AbstractServiceEngine, Remoting.IOperatorService
	{
		public OperatorEngine(Engine engine) : base (engine, "Operator")
		{
		}
		
		
		#region IOperatorService Members
		public void CreateRoamingClient(out Remoting.IOperation operation)
		{
			operation = new Operation (this);
		}
		
		public void GetRoamingClientData(Remoting.IOperation operation, out Remoting.ClientIdentity client, out byte[] compressed_data)
		{
			if (operation == null)
			{
				throw new System.ArgumentNullException ("operation");
			}
			
			Operation op = operation as Operation;
			
			if (op == null)
			{
				throw new System.ArgumentException ("Operation mismatch.");
			}
			
			op.WaitForProgress (100);
			
			this.ThrowExceptionBasedOnStatus (op.ProgressStatus);
			
			
			//	Récupère les données qui attendent le client :
			
			client          = new Remoting.ClientIdentity ("", 101);	//	TODO: générer l'identité correctement
			compressed_data = op.GetCompressedData ();
		}
		#endregion
		
		
		private class Operation : Remoting.AbstractThreadedOperation
		{
			public Operation(OperatorEngine oper)
			{
				this.oper = oper;
				this.Start ();
			}
			
			
			public byte[] GetCompressedData()
			{
				return this.data;
			}
			
			
			protected override void ProcessOperation()
			{
				//	Cette méthode fait le véritable travail.
				
				try
				{
					this.SetLastStep (3);
					
					this.Step1_CopyDatabase ();				this.InterruptIfCancelRequested ();
					this.Step2_CompressDatabase ();			this.InterruptIfCancelRequested ();
					this.Step2_CompressDatabaseDeflate ();	this.InterruptIfCancelRequested ();
					this.Step3_Finished ();
				}
				catch (Remoting.Exceptions.InterruptedException)
				{
					this.SetCancelled ();
				}
				catch (System.Exception exception)
				{
					this.SetFailed (exception.Message);
				}
				finally
				{
					System.Diagnostics.Debug.WriteLine ("Operator: operation thread exited.");
				}
			}
			
			
			private void Step1_CopyDatabase()
			{
				this.SetCurrentStep (1);
				
				Database.DbInfrastructure infrastructure = this.oper.engine.Orchestrator.Infrastructure;
				Database.IDbServiceTools  tools          = infrastructure.DefaultDbAbstraction.ServiceTools;
				
				tools.Backup (@"c:\test.backup.firebird");
				
				for (int i = 5; i < 50; i += 5)
				{
					System.IO.FileStream stream;
					
					try
					{
						stream = System.IO.File.OpenRead (@"c:\test.backup.firebird");
					}
					catch
					{
						System.Threading.Thread.Sleep (i);
						continue;
					}
					
					stream.Close ();
					break;
				}
			}
			
			private void Step2_CompressDatabase()
			{
				System.Diagnostics.Debug.WriteLine ("Compressing...");
				
				this.SetCurrentStep (2);
				
				System.IO.FileStream   source = System.IO.File.OpenRead (@"c:\test.backup.firebird");
				System.IO.MemoryStream memory = new System.IO.MemoryStream ();
				System.IO.Stream compressed = Common.IO.Compression.CreateBZip2Stream (memory);
				
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
				
				System.Diagnostics.Debug.WriteLine ("Compressed database from " + total_read + " to " + memory.ToArray ().Length + " bytes (BZIP2).");
			}
			
			private void Step2_CompressDatabaseDeflate()
			{
				System.Diagnostics.Debug.WriteLine ("Compressing...");
				
				this.SetCurrentStep (2);
				
				System.IO.FileStream   source = System.IO.File.OpenRead (@"c:\test.backup.firebird");
				System.IO.MemoryStream memory = new System.IO.MemoryStream ();
				System.IO.Stream compressed = Common.IO.Compression.CreateDeflateStream (memory, 9);
				
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
				
				System.Diagnostics.Debug.WriteLine ("Compressed database from " + total_read + " to " + memory.ToArray ().Length + " bytes (deflate).");
				
				this.data = memory.ToArray ();
			}
			
			private void Step3_Finished()
			{
				this.SetCurrentStep (3);
				this.SetProgress (100);
			}
			
			
			private OperatorEngine				oper;
			private byte[]						data;
		}
	}
}
