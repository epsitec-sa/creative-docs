//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Server
{
	public static class Program
	{
		public static void Main()
		{
#if true
			Database.DbInfrastructure infrastructure = DatabaseTools.GetDatabase (null);

			System.Diagnostics.Debug.Assert (infrastructure.LocalSettings.IsServer);
			System.Diagnostics.Debug.Assert (infrastructure.LocalSettings.ClientId == 1);

			Epsitec.Cresus.Services.EngineHost host = new Epsitec.Cresus.Services.EngineHost (1234);

//			Epsitec.Cresus.Services.Engine engine = new Epsitec.Cresus.Services.Engine (infrastructure, host);

			System.AppDomain domain = System.AppDomain.CreateDomain ("TestDomain/A");

			TestEngine te = (TestEngine) domain.CreateInstanceAndUnwrap ("Cresus.Server", "Epsitec.Cresus.Server.TestEngine");

			te.SetEngineHost (host);
			te.CreateEngine ();

			System.Threading.Thread.Sleep (-1);

#else
			System.ServiceProcess.ServiceBase[] services_to_run;

			services_to_run = new System.ServiceProcess.ServiceBase[] { new WindowsService () };

			System.ServiceProcess.ServiceBase.Run (services_to_run);
#endif
		}
	}

	public class TestEngine : System.MarshalByRefObject
	{
		public TestEngine()
		{
		}

		public void SetEngineHost(Epsitec.Cresus.Services.EngineHost host)
		{
			this.host = host;
		}

		public void CreateEngine()
		{
			this.infrastructure = DatabaseTools.GetDatabase (null);
			this.engine = new Epsitec.Cresus.Services.Engine (this.infrastructure, System.Guid.Empty);
			this.host.AddEngine (this.engine);
		}

		private Database.DbInfrastructure infrastructure;
		private Epsitec.Cresus.Services.EngineHost host;
		
		private Epsitec.Cresus.Services.Engine engine;
	}
}
