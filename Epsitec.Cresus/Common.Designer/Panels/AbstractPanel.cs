using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Panels
{
	/// <summary>
	/// La classe AbstractPanel est la base de toutes les classes XyzPanel.
	/// </summary>
	public abstract class AbstractPanel : Support.ICommandDispatcherHost
	{
		public AbstractPanel()
		{
		}
		
		
		public void SetCommandDispatcher(Support.CommandDispatcher dispatcher)
		{
			if (this.dispatcher != dispatcher)
			{
				this.dispatcher = dispatcher;
				this.OnCommandDispatcherChanged ();
			}
		}
		
		
		public Drawing.Size					Size
		{
			get
			{
				return this.size;
			}
		}
		
		public Widget						Widget
		{
			get
			{
				if (this.widget == null)
				{
					this.widget = this.CreateWidget ();
					this.widget.SetCommandDispatcher (this.CommandDispatcher);
				}
				
				return this.widget;
			}
		}
		
		public Support.CommandDispatcher	CommandDispatcher
		{
			get
			{
				return this.dispatcher;
			}
		}
		
		
		protected virtual Widget CreateWidget()
		{
			Widget host = new Widget ();
			
			host.Size    = this.Size;
			host.MinSize = this.Size;
			
			this.CreateWidgets (host);
			
			return host;
		}

		protected abstract void CreateWidgets(Widget parent);
		
		protected virtual void OnCommandDispatcherChanged()
		{
			if (this.widget != null)
			{
				this.widget.SetCommandDispatcher (this.dispatcher);
			}
		}
		
		
		protected Drawing.Size				size;
		protected Widget					widget;
		protected Support.CommandDispatcher	dispatcher;
	}
}
