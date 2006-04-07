//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>;
	
	/// <summary>
	/// La classe MenuItemContainer permet d'incorporer dans une case de menu
	/// des widgets divers et vari�s.
	/// </summary>
	public class MenuItemContainer : MenuItem
	{
		public MenuItemContainer()
		{
			this.InternalState |= InternalState.Focusable;
			this.IsFocusedChanged += this.HandleIsFocusedChanged;
		}
		
		public MenuItemContainer(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		

		public bool DefocusFromMenu()
		{
			bool changed = false;
			
			if (this.Children.Count > 0)
			{
				Widget[] widgets = this.Children.Widgets;
				
				foreach (Widget widget in widgets)
				{
					if (widget.IsFocused)
					{
						System.Diagnostics.Debug.WriteLine ("Removing focus from " + widget.ToString ());
						
						this.EnableFilter ();
						this.Focus ();
						
						changed = true;
					}
				}
			}
			
			this.Invalidate ();
			
			return changed;
		}
		
		public bool FocusFromMenu()
		{
			bool swallow = false;
			
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
							
							swallow = true;
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
			return swallow;
		}
		
		
		public override Drawing.Size GetBestFitSize()
		{
			return this.RealMinSize;
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.IsFocusedChanged -= this.HandleIsFocusedChanged;
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

		
		private void HandleIsVisibleChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			bool is_visible = (bool) e.NewValue;
			
			if (is_visible == false)
			{
				//	Si le widget devient invisible, il faut r�activer le filtre
				//	clavier du gestionnaire de menus !
				
				this.EnableFilter ();
			}
		}


		private void HandleIsFocusedChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			bool focused = (bool) e.NewValue;
			
			if (focused)
			{
				this.HandleFocused ();
			}
			else
			{
				this.HandleDefocused ();
			}
		}

		protected void HandleFocused()
		{
			System.Diagnostics.Debug.WriteLine ("MenuItemContainer focused");
		}

		protected void HandleDefocused()
		{
			System.Diagnostics.Debug.WriteLine ("MenuItemContainer de-focused");
		}
		
//		protected override void OnPressed(MessageEventArgs e)
//		{
//			if (this.FocusFromMenu ())
//			{
//				if (e != null)
//				{
//					e.Message.Swallowed = true;
//				}
//			}
//		}

		protected override void OnIconSizeChanged()
		{
			//	Met � jour le positionnement des �l�ments internes en fonction
			//	de la place disponible :
			
			double width = this.IconWidth;
			
			Drawing.Margins padding = this.Padding;
			
			if (padding.Left != width)
			{
				padding.Left = width;
				
				this.Padding = padding;
			}
			
			base.OnIconSizeChanged ();
		}

		protected override void OnUserAction(MessageEventArgs e)
		{
			//	Mange l'�v�nement. Un item "riche" ne peut pas g�n�rer d'actions
			//	lui-m�me !
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
				
				switch (message.KeyCode)
				{
					case KeyCode.ArrowUp:
					case KeyCode.ArrowDown:
						forward = true;
						break;
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
				
				behavior.AttachMenuItemContainer (this);
				
				System.Diagnostics.Debug.WriteLine ("Keyboard Filter disabled temporarily");
				
				this.filter_disabled   = true;
				this.IsVisibleChanged += new PropertyChangedEventHandler (this.HandleIsVisibleChanged);
			}
		}
		
		private void EnableFilter()
		{
			if (this.filter_disabled)
			{
				Behaviors.MenuBehavior behavior = MenuItem.GetMenuBehavior (this);
				
				behavior.DetachMenuItemContainer (this);
				
				System.Diagnostics.Debug.WriteLine ("Keyboard Filter re-enabled");
				
				this.filter_disabled   = false;
				this.IsVisibleChanged -= new PropertyChangedEventHandler (this.HandleIsVisibleChanged);
			}
		}
		
		
		private bool							filter_disabled;
	}
}
