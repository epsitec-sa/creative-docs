//	Copyright © 2003-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe WindowRoot implémente le fond de chaque fenêtre. L'utilisateur obtient
	/// en général une instance de WindowRoot en appelant Window.Root.
	/// </summary>
	public sealed class WindowRoot : AbstractGroup
	{
		private WindowRoot()
		{
			this.focusChain = new List<Visual> ();

			this.WindowType   = WindowType.Document;
			this.WindowStyles = WindowStyles.CanResize | WindowStyles.HasCloseButton;
			
			this.InternalState |= WidgetInternalState.PossibleContainer;
			this.AutoDoubleClick = true;
		}


		public WindowRoot(Window window)
			: this ()
		{
			this.window  = window;
			this.isReady = true;
		}
		
		
		public override Window					Window
		{
			get
			{
				return this.window;
			}
		}
		
		public WindowStyles						WindowStyles
		{
			get
			{
				return this.windowStyles;
			}
			set
			{
				if (this.windowStyles != value)
				{
					this.windowStyles = value;
					
					if (this.window != null)
					{
						this.window.WindowStyles = value;
					}
					
					this.OnWindowStylesChanged ();
				}
			}
		}
		
		public WindowType						WindowType
		{
			get
			{
				return this.windowType;
			}
			set
			{
				if (this.windowType != value)
				{
					this.windowType = value;
					
					if (this.window != null)
					{
						this.window.WindowType = value;
					}
					
					this.OnWindowTypeChanged ();
				}
			}
		}

		public int								TreeChangeCounter
		{
			get
			{
				return this.treeChangeCounter;
			}
		}
		
		
		public bool DoesVisualContainKeyboardFocus(Visual visual)
		{
			//	Retourne true si le visual passé en entrée contient le focus,
			//	ou qu'un de ses enfants contient le focus.

			this.RefreshFocusChain ();
			
			return this.focusChain.Contains (visual);
		}
		
		public override void MessageHandler(Message message, Drawing.Point pos)
		{
			message.WindowRoot = this;
			
			base.MessageHandler (message, pos);
		}

		
		public override void InvalidateRectangle(Drawing.Rectangle rect, bool sync)
		{
			System.Diagnostics.Debug.Assert (this.Parent == null);
			
			if (this.window != null)
			{
				if (sync)
				{
					this.window.SynchronousRepaint ();
					this.window.MarkForRepaint (rect);
					this.window.SynchronousRepaint ();
				}
				else
				{
					this.window.MarkForRepaint (rect);
				}
			}
		}
		
		public override Epsitec.Common.Drawing.Size GetBestFitSize()
		{
			Widget[] children = this.Children.Widgets;
			
			if (children.Length == 1)
			{
				return children[0].GetBestFitSize ();
			}
			else
			{
				return this.PreferredSize;
			}
		}

		internal void RefreshFocusChain()
		{
			//	Si la chaîne des widgets décrivant les widgets contenant le
			//	focus n'existe pas, on la construit

			if ((this.focusChain.Count == 0) &&
				(this.window != null))
			{
				Widget widget = this.window.FocusedWidget;

				while (widget != null)
				{
					this.focusChain.Add (widget);
					widget = widget.Parent;
				}
			}
		}
		
		internal void ClearFocusChain()
		{
			this.focusChain.Clear ();
		}
		
		internal override void SetBounds(Drawing.Rectangle value)
		{
			double dx = value.Width;
			double dy = value.Height;
			
			if (this.window != null)
			{
				this.window.ClientSize = new Drawing.Size (dx, dy);
			}
			
			base.SetBounds (value);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.window = null;
			}
			
			base.Dispose (disposing);
		}

		protected override bool ShortcutHandler(Shortcut shortcut, bool executeFocused)
		{
			if (base.ShortcutHandler (shortcut, executeFocused))
			{
				return true;
			}
			
			if (this.InternalShortcutHandler (shortcut, executeFocused))
			{
				return true;
			}
			
			return false;
		}
		
		private bool InternalShortcutHandler(Shortcut shortcut, bool executeFocused)
		{
			if (this.window != null)
			{
				//	Le raccourci clavier n'a pas été consommé. Il faut voir si le raccourci clavier
				//	est attaché à une commande globale.
				
				System.Diagnostics.Debug.WriteLine (shortcut.ToString ());
				
				Widget focused = this.window.FocusedWidget;
				bool   execute = true;
				
				if ((focused != null) &&
					(focused.AutoRadio) &&
					(shortcut.IsAltDefined == false) &&
					(shortcut.IsControlDefined == false) &&
					(shortcut.IsShiftDefined == false))
				{
					switch (shortcut.KeyCodeOnly)
					{
						case KeyCode.ArrowLeft:
						case KeyCode.ArrowRight:
						case KeyCode.ArrowUp:
						case KeyCode.ArrowDown:
							execute = false;
							break;
					}
				}
				
				if (execute)
				{
					Widget widget = focused == null ? this : focused;
					Window window = this.window;

					List<Command> commands = Types.Collection.ToList (Widgets.Command.FindAll (shortcut));
					
					if (commands.Count > 0)
					{
						CommandDispatcherChain dispatcherChain = CommandDispatcherChain.BuildChain (focused ?? this);
						
						if (dispatcherChain != null)
						{
							Command bestCommand = dispatcherChain.GetBestCommand (commands);
							window.QueueCommand (widget, bestCommand);
							return true;
						}
					}
					
#if false
					if ((shortcut.KeyCodeOnly == KeyCode.FuncF4) &&
						(shortcut.IsAltDefined) &&
						(shortcut.IsControlDefined == false) &&
						(shortcut.IsShiftDefined == false))
					{
						if (string.IsNullOrEmpty (window.Name) == false)
						{
							window.QueueCommand (this, "Quit" + this.Window.Name);

							if (window.Name == "Application")
							{
								window.QueueCommand (this, ApplicationCommands.Quit);
							}
						}
						return true;
					}
#endif
				}
				
				if (focused == null)
				{
					return false;
				}
				
				TabNavigationMode mode = TabNavigationMode.None;
				TabNavigationDir  dir  = TabNavigationDir.None;
				
				switch (shortcut.KeyCodeOnly)
				{
					case KeyCode.Tab:
						mode = TabNavigationMode.ActivateOnTab;
						dir  = Message.CurrentState.IsShiftPressed ? TabNavigationDir.Backwards : TabNavigationDir.Forwards;
						break;
					
					case KeyCode.ArrowLeft:
						mode = TabNavigationMode.ActivateOnCursorX;
						dir  = TabNavigationDir.Backwards;
						break;
					
					case KeyCode.ArrowRight:
						mode = TabNavigationMode.ActivateOnCursorX;
						dir  = TabNavigationDir.Forwards;
						break;
					
					case KeyCode.ArrowUp:
						mode = TabNavigationMode.ActivateOnCursorY;
						dir  = TabNavigationDir.Backwards;
						break;
					
					case KeyCode.ArrowDown:
						mode = TabNavigationMode.ActivateOnCursorY;
						dir  = TabNavigationDir.Forwards;
						break;
					
					case KeyCode.NumericEnter:
					case KeyCode.Return:
						if (window != null)
						{
							if (window.RestoreLogicalFocus ())
							{
								return true;
							}
						}
						break;
					
					case KeyCode.Escape:
						if (window != null)
						{
							if (window.RestoreLogicalFocus ())
							{
								return true;
							}
						}
						break;
				}
				
				if ((mode != TabNavigationMode.None) &&
					(dir != TabNavigationDir.None))
				{
					//	Navigue dans la hiérarchie...
					
					Widget find = focused.FindTabWidget (dir, mode);
					this.Window.FocusWidget (find, dir, mode);
				}
			}
				
			return false;
		}
		
		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			
			if (this.window != null)
			{
				this.window.Text = this.Text;
			}
		}
		
		protected override void OnNameChanged()
		{
			base.OnNameChanged ();
			
			if (this.window != null)
			{
				this.window.Name = this.Name;
			}
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if (this.isReady == false)
			{
				return;
			}

			double dx = this.Client.Size.Width;
			double dy = this.Client.Size.Height;
			
			double x1 = System.Math.Max (clipRect.Left, 0);
			double y1 = System.Math.Max (clipRect.Bottom, 0);
			double x2 = System.Math.Min (clipRect.Right, dx);
			double y2 = System.Math.Min (clipRect.Top, dy);
			
			if (this.BackColor.IsValid)
			{
				if (this.BackColor.A != 1.0)
				{
					graphics.Pixmap.Erase (new System.Drawing.Rectangle ((int) x1, (int) y1, (int) x2 - (int) x1, (int) y2 - (int) y1));
				}
				if (this.BackColor.A > 0.0)
				{
					graphics.SolidRenderer.Color = this.BackColor;
					graphics.AddFilledRectangle (x1, y1, x2-x1, y2-y1);
					graphics.RenderSolid ();
				}
			}
			else
			{
				IAdorner adorner = Widgets.Adorners.Factory.Active;
				Drawing.Rectangle rect = new Drawing.Rectangle(x1, y1, x2-x1, y2-y1);
				adorner.PaintWindowBackground(graphics, this.Client.Bounds, rect, WidgetPaintState.None);
			}
		}
		
		
		private void OnWindowStylesChanged()
		{
			var handler = this.GetUserEventHandler("WindowStylesChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		private void OnWindowTypeChanged()
		{
			var handler = this.GetUserEventHandler("WindowTypeChanged");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		
		internal void NotifyAdornerChanged()
		{
			this.HandleAdornerChanged ();
		}
		
		internal void NotifyCultureChanged()
		{
			this.HandleCultureChanged ();
		}

		internal void NotifyWindowIsVisibleChanged()
		{
			//	Copie l'état de visibilité de la fenêtre de manière à ce que
			//	notre propriété IsVisible soit toujours synchronisée avec la
			//	fenêtre :
			
			this.SetValue (Visual.IsVisibleProperty, this.window.IsVisible);
		}

		internal void NotifyWindowSizeChanged(double width, double height)
		{
			this.SetManualBounds (new Drawing.Rectangle (0, 0, width, height));
			
			Layouts.LayoutContext.AddToArrangeQueue (this);
		}
		
		internal override void SetDirtyLayoutFlag()
		{
		}

		internal void IncrementTreeChangeCounter()
		{
			this.treeChangeCounter++;
		}
		
		public event EventHandler					WindowStylesChanged
		{
			add
			{
				this.AddUserEventHandler("WindowStylesChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("WindowStylesChanged", value);
			}
		}

		public event EventHandler					WindowTypeChanged
		{
			add
			{
				this.AddUserEventHandler("WindowTypeChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("WindowTypeChanged", value);
			}
		}


		private WindowStyles						windowStyles;
		private WindowType							windowType;
		private Window								window;
		private bool								isReady;
		private readonly List<Visual>				focusChain;
		private int									treeChangeCounter;
	}
}
