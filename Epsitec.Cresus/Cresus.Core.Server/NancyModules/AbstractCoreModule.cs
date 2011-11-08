//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -


using Epsitec.Cresus.Core.Server.CoreServer;

using Nancy;


namespace Epsitec.Cresus.Core.Server.NancyModules
{


	/// <summary>
	/// Base module thats allows to easly get the CoreSession for a defined user,
	/// and that requires the user to be logged in.
	/// </summary>
	public abstract class AbstractCoreModule : NancyModule
	{
		
		
		protected AbstractCoreModule(): base()
		{
			this.serverContext = this.GetServerContext ();
		}


		protected AbstractCoreModule(string modulePath) : base (modulePath)
		{
			this.serverContext = this.GetServerContext ();
		}


		private ServerContext GetServerContext()
		{
			return CoreServerProgram.serverContext;
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
