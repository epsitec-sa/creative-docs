//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe AbstractController sert de base pour tous les contrôleurs.
	/// </summary>
	public abstract class AbstractController : Support.ICommandDispatcherHost, System.IDisposable
	{
		public AbstractController(Application application)
		{
			this.application = application;
			this.dispatcher  = application.CommandDispatcher;
			
			//	Enregistre toutes les commandes exportées par cette classe (en particulier
			//	celles de la classe dérivée).
			
			this.dispatcher.RegisterController (this);
		}
		
		
		public Application						Application
		{
			get
			{
				return this.application;
			}
		}
		
		public Support.CommandDispatcher		CommandDispatcher
		{
			get
			{
				return this.dispatcher;
			}
		}
		
		
		public virtual void Initialise()
		{
		}
		
		public virtual void FillToolBar(AbstractToolBar tool_bar)
		{
		}
		
		
		#region ICommandDispatcherHost Members
		Support.CommandDispatcher				Support.ICommandDispatcherHost.CommandDispatcher
		{
			get
			{
				return this.CommandDispatcher;
			}
			set
			{
				if (this.dispatcher != value)
				{
					throw new System.InvalidOperationException ("CommandDispatcher may not be changed.");
				}
			}
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
		}
		
		protected virtual void ThrowInvalidOperationException(CommandEventArgs e, int num_expected)
		{
			string command   = e.CommandName;
			int    num_found = e.CommandArgs.Length;
			
			throw new System.InvalidOperationException (string.Format ("Command {0} requires {1} argument(s), got {2}.", command, num_expected, num_found));
		}
		
		
		protected Support.CommandDispatcher		dispatcher;
		protected Application					application;
	}
}
