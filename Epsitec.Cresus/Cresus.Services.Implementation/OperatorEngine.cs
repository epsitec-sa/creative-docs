//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.IO.IsolatedStorage;

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
					
					this.temp = new TemporaryFile ();
					
					this.Step1_CopyDatabase ();				this.InterruptIfCancelRequested ();
					this.Step2_CompressDatabase ();			this.InterruptIfCancelRequested ();
					this.Step3_Finished ();
				}
				catch (Remoting.Exceptions.InterruptedException)
				{
					this.SetCancelled ();
				}
				catch (System.Exception exception)
				{
					this.SetFailed (exception.ToString ());
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
			
			
			private void Step1_CopyDatabase()
			{
				this.SetCurrentStep (1);
				
				Database.DbInfrastructure infrastructure = this.oper.engine.Orchestrator.Infrastructure;
				Database.IDbServiceTools  tools          = infrastructure.DefaultDbAbstraction.ServiceTools;
				
				tools.Backup (this.temp.Path);
				
				if (! this.WaitForFileReadable (this.temp.Path, 10*1000))
				{
					throw new System.IO.IOException ("File cannot be opened in read mode within specified delay.");
				}
			}
			
			private void Step2_CompressDatabase()
			{
				System.Diagnostics.Debug.WriteLine ("Compressing...");
				
				this.SetCurrentStep (2);
				
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
			
			private void Step3_Finished()
			{
				this.SetCurrentStep (3);
				this.SetProgress (100);
			}
			
			
			private bool WaitForFileReadable(string name, int max_wait)
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				bool ok = false;
				int wait = 0;
				
				for (int i = 5; wait < max_wait; i += 5)
				{
					this.InterruptIfCancelRequested ();
					
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
			
			
			private TemporaryFile				temp;
			private OperatorEngine				oper;
			private byte[]						data;
		}
		
		private class TemporaryFile : System.IDisposable
		{
			public TemporaryFile()
			{
				this.name = System.IO.Path.GetTempFileName ();
			}
			
			~ TemporaryFile()
			{
				this.Dispose (false);
			}
			
			
			public string						Path
			{
				get
				{
					return this.name;
				}
			}
			
			
			public void Delete()
			{
				//	Tente de supprimer le fichier tout de suite. Si on n'y réussit pas,
				//	ce sera le 'finalizer' qui s'en chargera...
				
				this.RemoveFile ();
				
				if (this.name == null)
				{
					//	Fichier détruit, plus besoin d'exécuter le 'finalizer'.
					
					System.GC.SuppressFinalize (this);
				}
			}
			
			
			#region IDisposable Members
			public void Dispose()
			{
				this.Dispose (true);
				System.GC.SuppressFinalize (this);
			}
			#endregion
			
			protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
					//	rien à faire de plus...
				}
				
				this.RemoveFile ();
			}
			
			protected virtual void RemoveFile()
			{
				if (this.name != null)
				{
					try
					{
						if (System.IO.File.Exists (this.name))
						{
							System.IO.File.Delete (this.name);
							this.name = null;
						}
					}
					catch (System.Exception ex)
					{
						System.Diagnostics.Debug.WriteLine ("Could not remove file " + this.name + ";\n" + ex.ToString ());
					}
				}
			}
			
			
			private string						name;
		}
	}
}
