//	Copyright © 2011-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;

using Epsitec.Cresus.WebCore.Server.Core.Databases;

using System.Globalization;
using Epsitec.Cresus.Core.Library;


namespace Epsitec.Cresus.WebCore.Server.Core
{
	/// <summary>
	/// This class is used to initialize all objects that will be used globally across the server
	/// to access data, and to hold references on them, so that they can be used by the Nancy
	/// modules.
	/// </summary>
	public sealed class CoreServer : System.IDisposable
	{
		public CoreServer(int nbCoreWorkers, CultureInfo uiCulture)
		{
			var coreWorkerPool = new CoreWorkerPool (nbCoreWorkers, uiCulture);

			this.coreWorkerPool = coreWorkerPool;
			this.authenticationManager = new AuthenticationManager (coreWorkerPool);
			this.databaseManager = new DatabaseManager ();
			this.caches = new Caches ();
			this.coreWorkerQueue = new Core.CoreWorkerQueue (uiCulture);

			Logger.LogToConsole ("Core server started");
		}


		public CoreWorkerQueue					CoreWorkerQueue
		{
			get
			{
				return this.coreWorkerQueue;
			}
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

		public DatabaseManager					DatabaseManager
		{
			get
			{
				return this.databaseManager;
			}
		}

		public Caches							Caches
		{
			get
			{
				return this.caches;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.coreWorkerQueue.Dispose ();
			this.coreWorkerPool.Dispose ();
			this.caches.Dispose ();
		}

		#endregion

		private readonly CoreWorkerPool			coreWorkerPool;
		private readonly CoreWorkerQueue		coreWorkerQueue;
		private readonly AuthenticationManager	authenticationManager;
		private readonly DatabaseManager		databaseManager;
		private readonly Caches					caches;
	}
}
