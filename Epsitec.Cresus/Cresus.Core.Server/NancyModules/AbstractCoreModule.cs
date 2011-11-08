//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -


using Epsitec.Cresus.Core.Server.CoreServer;

using Nancy;
using Epsitec.Cresus.Core.Server.NancyHosting;


namespace Epsitec.Cresus.Core.Server.NancyModules
{


	/// <summary>
	/// Base module thats allows to easly get the CoreSession for a defined user,
	/// and that requires the user to be logged in.
	/// </summary>
	public abstract class AbstractCoreModule : NancyModule
	{
		
		
		protected AbstractCoreModule(ServerContext serverContext): base()
		{
			this.serverContext = serverContext;			
		}


		protected AbstractCoreModule(ServerContext serverContext, string modulePath) : base (modulePath)
		{
			this.serverContext = serverContext;
		}


		internal ServerContext ServerContext
		{
			get
			{
				return this.serverContext;
			}
		}


		private readonly ServerContext serverContext;


	}


}
