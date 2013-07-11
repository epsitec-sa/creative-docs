//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;

using Epsitec.Cresus.WebCore.Server.Core.Databases;

using System.Globalization;


namespace Epsitec.Cresus.WebCore.Server.Core
{
	public sealed class CoreServer : System.IDisposable
	{
		public CoreServer(int nbCoreWorkers, CultureInfo uiCulture)
		{
			var coreWorkerPool = new CoreWorkerPool (nbCoreWorkers, uiCulture);

			this.coreWorkerPool = coreWorkerPool;
			this.authenticationManager = new AuthenticationManager (coreWorkerPool);
			this.databaseManager = new DatabaseManager ();
			this.caches = new Caches ();

			Logger.LogToConsole ("Core server started");
		}


		public CoreWorkerPool					CoreWorkerPool
		{
			get
			{
				return this.coreWorkerPool;
			}
		}

		public AuthenticationManager			AuthenticationManager
		{
			get
			{
				return this.authenticationManager;
			}
		}

		internal DatabaseManager				DatabaseManager
		{
			get
			{
				return this.databaseManager;
			}
		}

		internal Caches							Caches
		{
			get
			{
				return this.caches;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.coreWorkerPool.Dispose ();
			this.caches.Dispose ();
		}

		#endregion

		private readonly CoreWorkerPool			coreWorkerPool;
		private readonly AuthenticationManager	authenticationManager;
		private readonly DatabaseManager		databaseManager;
		private readonly Caches					caches;
	}
}
