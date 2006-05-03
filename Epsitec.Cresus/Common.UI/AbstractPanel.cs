//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// La classe AbstractPanel est la base de toutes les classes XyzPanel.
	/// </summary>
	public abstract class AbstractPanel : ICommandDispatcherHost, System.IDisposable
	{
		public AbstractPanel()
		{
		}
		
		
		public void SetCommandDispatcher(CommandDispatcher dispatcher)
		{
			if (this.dispatcher != dispatcher)
			{
				CommandDispatcher old_dispatcher = this.dispatcher;
				this.dispatcher = dispatcher;
				CommandDispatcher new_dispatcher = this.dispatcher;
				
				this.OnCommandDispatcherChanged (old_dispatcher, new_dispatcher);
			}
		}
		
		
		public Drawing.Size						Size
		{
			get
			{
				return this.size;
			}
		}
		
		public Widget							Widget
		{
			get
			{
				if (this.widget == null)
				{
					this.CreateWidget ();
					
					if (this.dispatcher != null)
					{
						this.widget.AttachCommandDispatcher (this.dispatcher);
					}
				}
				
				return this.widget;
			}
		}
		
		public CommandDispatcher[]				CommandDispatchers
		{
			get
			{
				return CommandDispatcher.ToArray (this.dispatcher);
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
				this.widget.Dispose ();
				
				this.widget     = null;
				this.dispatcher = null;
			}
		}
		
		protected virtual void CreateWidget()
		{
			this.widget = new Widget ();
			
			this.widget.PreferredSize = this.Size;
			this.widget.MinSize = this.Size;
			this.widget.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			
			this.CreateWidgets (this.widget);
		}

		protected abstract void CreateWidgets(Widget parent);
		
		protected virtual void OnCommandDispatcherChanged(CommandDispatcher old_dispatcher, CommandDispatcher new_dispatcher)
		{
			if (this.widget != null)
			{
				this.widget.DetachCommandDispatcher (old_dispatcher);
				this.widget.AttachCommandDispatcher (new_dispatcher);
			}
		}
		
		
		protected Drawing.Size					size;
		protected Widget						widget;
		protected CommandDispatcher				dispatcher;
	}
}
