//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Statut : OK/PA, 12/03/2004

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe SmartTag représente un Tag avec un menu associé.
	/// </summary>
	public class SmartTag : Tag
	{
		public SmartTag() : this (null, null)
		{
		}
		
		public SmartTag(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		public SmartTag(string command) : this (command, null)
		{
		}
		
		public SmartTag(string command, string name) : base (command, name)
		{
		}
		
		
		public VMenu							Menu
		{
			get
			{
				return this.menu;
			}
			set
			{
				this.menu = value;
			}
		}
		
		public object							Context
		{
			get
			{
				return this.context;
			}
			set
			{
				this.context = value;
			}
		}
		
		
		protected virtual VMenu GetMenu()
		{
			this.OnPrepareMenu ();
			return this.menu;
		}
		
		protected override void OnClicked(MessageEventArgs e)
		{
			base.OnClicked (e);
			
			Drawing.Point pos  = this.MapClientToScreen (new Drawing.Point (0, this.ActualHeight));
			VMenu         menu = this.GetMenu ();
			
			menu.Host = this;
			pos.X    -= menu.ActualWidth;
			
			menu.ShowAsContextMenu (this.Window, pos);
		}
		
		protected virtual  void OnPrepareMenu()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("PrepareMenu");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		
		public event EventHandler				PrepareMenu
		{
			add
			{
				this.AddUserEventHandler("PrepareMenu", value);
			}
			remove
			{
				this.RemoveUserEventHandler("PrepareMenu", value);
			}
		}

		
		protected VMenu							menu;
		protected object						context;
	}
}
