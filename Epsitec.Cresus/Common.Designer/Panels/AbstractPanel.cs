//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

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
		
		public Support.CommandDispatcher		CommandDispatcher
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
		protected Support.CommandDispatcher		dispatcher;
	}
}
