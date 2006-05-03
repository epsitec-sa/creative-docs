//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 12/03/2004

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
			if (this.PrepareMenu != null)
			{
				this.PrepareMenu (this);
			}
		}
		
		
		public event Support.EventHandler		PrepareMenu;
		
		protected VMenu							menu;
		protected object						context;
	}
}
