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
					this.Step1_CopyDatabase ();
					
					while (! this.IsCancelRequested)
					{
						//	TODO: faire du travail réel ici
						
						System.Threading.Thread.Sleep (50);
					}
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
				Database.DbInfrastructure infrastructure = this.oper.engine.Orchestrator.Infrastructure;
				Database.IDbServiceTools  tools          = infrastructure.DefaultDbAbstraction.ServiceTools;
				
				tools.Backup (@"c:\test.backup.firebird");
			}
			
			
			private OperatorEngine				oper;
		}
	}
}
