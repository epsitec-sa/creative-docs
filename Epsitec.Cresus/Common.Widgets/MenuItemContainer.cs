//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe MenuItemContainer permet d'incorporer dans une case de menu
	/// des widgets divers et variés.
	/// </summary>
	public class MenuItemContainer : MenuItem
	{
		public MenuItemContainer()
		{
		}
		
		public MenuItemContainer(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		

		public void FocusFromMenu()
		{
			this.Window.MakeFocused ();
			
			this.DisableFilter ();
		}
		
		
		public override Drawing.Size GetBestFitSize()
		{
			return this.RealMinSize;
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.EnableFilter ();
			}
			
			base.Dispose (disposing);
		}

		
		private void HandleIsVisibleChanged(object sender, Epsitec.Common.Types.PropertyChangedEventArgs e)
		{
			bool is_visible = (bool) e.NewValue;
			
			if (is_visible == false)
			{
				//	Si le widget devient invisible, il faut réactiver le filtre
				//	clavier du gestionnaire de menus !
				
				this.EnableFilter ();
			}
		}
		
		
		protected override void OnIconSizeChanged()
		{
			//	Met à jour le positionnement des éléments internes en fonction
			//	de la place disponible :
			
			double width = this.IconWidth;
			
			Drawing.Margins padding = this.DockPadding;
			
			if (padding.Left != width)
			{
				padding.Left = width;
				
				this.DockPadding = padding;
			}
			
			base.OnIconSizeChanged ();
		}

		protected override void ProcessMessage(Message message, Epsitec.Common.Drawing.Point pos)
		{
			if ((this.filter_disabled) &&
				(message.IsKeyType))
			{
				Behaviors.MenuBehavior behavior = MenuItem.GetMenuBehavior (this);
				Window                 window   = this.Window;
				
				if ((behavior != null) &&
					(window != null))
				{
					System.Diagnostics.Debug.WriteLine ("Bubbling up message " + message.ToString ());
					
					behavior.HandleKeyboardEvent (window, message);
				}
			}
			
			base.ProcessMessage (message, pos);
		}

		
		private void DisableFilter()
		{
			if (! this.filter_disabled)
			{
				Behaviors.MenuBehavior.DisableKeyboardFilter ();
				
				System.Diagnostics.Debug.WriteLine ("Keyboard Filter disabled temporarily");
				
				this.filter_disabled   = true;
				this.IsVisibleChanged += new Types.PropertyChangedEventHandler (this.HandleIsVisibleChanged);
			}
		}
		
		private void EnableFilter()
		{
			if (this.filter_disabled)
			{
				Behaviors.MenuBehavior.EnableKeyboardFilter ();
				
				System.Diagnostics.Debug.WriteLine ("Keyboard Filter re-enabled");
				
				this.filter_disabled   = false;
				this.IsVisibleChanged -= new Types.PropertyChangedEventHandler (this.HandleIsVisibleChanged);
			}
		}
		
		
		private bool							filter_disabled;
	}
}
