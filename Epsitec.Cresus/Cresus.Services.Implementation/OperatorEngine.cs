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
		#endregion
		
		
		private class Operation : Remoting.AbstractThreadedOperation
		{
			public Operation(OperatorEngine oper)
			{
				this.oper = oper;
				this.Start ();
			}
			
			
			protected override void ProcessOperation()
			{
				//	Cette méthode fait le véritable travail.
				
				try
				{
					this.SetLastStep (3);
					
					this.Step1_CopyDatabase ();				if (this.IsCancelRequested) return;
					this.Step2_CompressDatabase ();			if (this.IsCancelRequested) return;
					this.Step2_CompressDatabaseDeflate ();	if (this.IsCancelRequested) return;
					this.Step3_Finished ();
					
//					while (! this.IsCancelRequested)
//					{
//						//	TODO: faire du travail réel ici
//						
//						System.Threading.Thread.Sleep (50);
//					}
				}
				catch (System.Exception exception)
				{
					System.Diagnostics.Debug.WriteLine (exception.Message);
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
			}
			
			private void Step3_Finished()
			{
				this.SetCurrentStep (3);
				this.SetProgress (100);
			}
			
			
			private OperatorEngine				oper;
		}
	}
}
