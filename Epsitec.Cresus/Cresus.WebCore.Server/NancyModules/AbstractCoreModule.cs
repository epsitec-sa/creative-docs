//	Copyright © 2011-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.WebCore.Server.Core;

using Nancy;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{
	/// <summary>
	/// This is the base class of all modules used in this project. It provides them with the
	/// instance of CoreServer that will be used to access to the application services like the
	/// CoreWorkerPool, the Caches, etc.
	/// </summary>
	public abstract class AbstractCoreModule : NancyModule
	{
		protected AbstractCoreModule(CoreServer coreServer)
			: base ()
		{
			this.coreServer = coreServer;
		}

		protected AbstractCoreModule(CoreServer coreServer, string modulePath)
			: base (modulePath)
		{
			this.coreServer = coreServer;
		}


		protected CoreServer					CoreServer
		{
			get
			{
				return this.coreServer;
			}
		}


		private readonly CoreServer				coreServer;
	}
}
