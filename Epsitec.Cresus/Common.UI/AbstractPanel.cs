//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
				this.dispatcher = dispatcher;
				this.OnCommandDispatcherChanged ();
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
					this.widget.SetCommandDispatcher (this.CommandDispatcher);
				}
				
				return this.widget;
			}
		}
		
		public CommandDispatcher				CommandDispatcher
		{
			get
			{
				return this.dispatcher;
			}
			set
			{
				if (this.dispatcher != value)
				{
					this.SetCommandDispatcher (value);
				}
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
			
			this.widget.Size    = this.Size;
			this.widget.MinSize = this.Size;
			this.widget.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			
			this.CreateWidgets (this.widget);
		}

		protected abstract void CreateWidgets(Widget parent);
		
		protected virtual void OnCommandDispatcherChanged()
		{
			if (this.widget != null)
			{
				this.widget.SetCommandDispatcher (this.dispatcher);
			}
		}
		
		
		protected Drawing.Size					size;
		protected Widget						widget;
		protected CommandDispatcher				dispatcher;
	}
}
