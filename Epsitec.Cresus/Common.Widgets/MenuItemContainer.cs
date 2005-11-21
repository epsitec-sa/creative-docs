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
			this.InternalState |= InternalState.Focusable;
		}
		
		public MenuItemContainer(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		

		public void FocusFromMenu(Message message)
		{
			System.Diagnostics.Debug.WriteLine ("FocusFromMenu called");
			this.Window.MakeFocused ();
			
			if (this.Children.Count > 0)
			{
				Widget[] widgets = this.Children.Widgets;
				
				foreach (Widget widget in widgets)
				{
					if (widget.CanFocus)
					{
						if (widget.IsFocused)
						{
							System.Diagnostics.Debug.WriteLine ("Removing focus from " + widget.ToString ());
							
							this.EnableFilter ();
							this.Focus ();
							
							if (message != null)
							{
								message.Swallowed = true;
							}
						}
						else
						{
							System.Diagnostics.Debug.WriteLine ("Setting focus on " + widget.ToString ());
							this.DisableFilter ();
							widget.Focus ();
						}
						break;
					}
				}
			}
			
			this.Invalidate ();
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

		protected override MenuItemType GetPaintItemType()
		{
			if (this.filter_disabled)
			{
				return MenuItemType.Default;
			}
			else
			{
				return base.GetPaintItemType ();
			}
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
		
		
		protected override void OnFocused()
		{
			System.Diagnostics.Debug.WriteLine ("MenuItemContainer focused");
			base.OnFocused ();
		}

		protected override void OnDefocused()
		{
			System.Diagnostics.Debug.WriteLine ("MenuItemContainer de-focused");
			base.OnDefocused ();
		}
		
		protected override void OnPressed(MessageEventArgs e)
		{
			this.FocusFromMenu (e == null ? null : e.Message);
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
				(message.Type == MessageType.KeyDown))
			{
				IFeel feel    = Feel.Factory.Active;
				bool  forward = false;
				
				if (feel.TestAcceptKey (message))
				{
					this.EnableFilter ();
					this.Focus ();
					
					message.Consumer = this;
					return;
				}
				else if (feel.TestCancelKey (message))
				{
					this.EnableFilter ();
					this.Focus ();
					
					message.Consumer = this;
					return;
				}
				
				if (forward)
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
			}
			
			base.ProcessMessage (message, pos);
		}

		
		private void DisableFilter()
		{
			if (! this.filter_disabled)
			{
				Behaviors.MenuBehavior behavior = MenuItem.GetMenuBehavior (this);
				
				behavior.IsFrozen = true;
				
				System.Diagnostics.Debug.WriteLine ("Keyboard Filter disabled temporarily");
				
				this.filter_disabled   = true;
				this.IsVisibleChanged += new Types.PropertyChangedEventHandler (this.HandleIsVisibleChanged);
			}
		}
		
		private void EnableFilter()
		{
			if (this.filter_disabled)
			{
				Behaviors.MenuBehavior behavior = MenuItem.GetMenuBehavior (this);
				
				behavior.IsFrozen = false;
				
				System.Diagnostics.Debug.WriteLine ("Keyboard Filter re-enabled");
				
				this.filter_disabled   = false;
				this.IsVisibleChanged -= new Types.PropertyChangedEventHandler (this.HandleIsVisibleChanged);
			}
		}
		
		
		private bool							filter_disabled;
	}
}
