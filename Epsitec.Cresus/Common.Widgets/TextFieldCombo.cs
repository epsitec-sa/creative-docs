namespace Epsitec.Common.Widgets
{
	using Keys = System.Windows.Forms.Keys;

	/// <summary>
	/// La classe TextFieldCombo implémente la ligne éditable avec bouton "v".
	/// </summary>
	public class TextFieldCombo : AbstractTextField
	{
		public TextFieldCombo()
		{
			this.items = new Helpers.StringCollection ();
			
			this.button = new ArrowButton();
			this.button.Direction = Direction.Down;
			this.button.ButtonStyle = ButtonStyle.Scroller;
			this.button.Parent = this;
			this.button.Pressed += new MessageEventHandler (this.HandleButtonPressed);
			this.button.Dock = DockStyle.Right;
			
			this.rightMargin = this.button.Width;
		}
		
		
		public Helpers.StringCollection Items
		{
			get { return this.items; }
		}
		
		
		public bool FindMatch(string find, out int index, out bool exact_match)
		{
			index = this.items.FindExactMatch (find);
			
			if (index < 0)
			{
				exact_match = false;
				index = this.items.FindStartMatch (find);
				
				if (index < 0)
				{
					return false;
				}
			}
			else
			{
				exact_match = true;
			}
			
			return true;
		}

		
		protected override void Dispose(bool disposing)
		{
			base.Dispose (disposing);
			
			if (disposing)
			{
				this.button.Pressed -= new MessageEventHandler (this.HandleButtonPressed);
				this.button.Dispose ();
				this.button = null;
			}
		}

		protected override void ProcessKeyDown(Keys key, bool isShiftPressed, bool isCtrlPressed)
		{
			switch (key)
			{
				case Keys.Up:
					this.Navigate(-1);
					break;
				case Keys.Down:
					this.Navigate(1);
					break;
				default:
					base.ProcessKeyDown (key, isShiftPressed, isCtrlPressed);
					break;
			}
		}
		
		
		protected void Navigate(int dir)
		{
			// Cherche le nom suivant ou précédent dans la comboList, même si elle
			// n'est pas "déroulée".
			
			int		sel;
			bool	exact;

			if ( this.FindMatch(this.Text, out sel, out exact) )
			{
				if ( exact)  sel += dir;
			}
			
			sel = System.Math.Max(sel, 0);
			sel = System.Math.Min(sel, this.items.Count-1);
			
			this.Text = this.items[sel];
			this.SelectAll();
			this.SetFocused(true);
		}
		
		
		private void HandlerMessageFilter(object sender, Message message)
		{
			if ( this.scrollList == null )  return;
			WindowFrame window = sender as WindowFrame;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					Drawing.Point mouse = window.MapWindowToScreen(message.Cursor);
					Drawing.Point pos = this.scrollList.MapScreenToClient (mouse);
					if ( !this.scrollList.HitTest(pos) )
					{
						this.scrollList.SelectedIndexChanged -= new EventHandler(this.HandleScrollListSelectedIndexChanged);
						WindowFrame.MessageFilter -= new Epsitec.Common.Widgets.MessageHandler(this.HandlerMessageFilter);
						WindowFrame.ApplicationDeactivated -= new EventHandler(this.HandleApplicationDeactivated);
						this.scrollList.Dispose();
						this.scrollList = null;
						this.comboWindow.Dispose();
						this.comboWindow = null;

						if ( !message.NonClient )
						{
							message.Handled = true;
							message.Swallowed = true;
						}
					}
					break;
			}
		}

		private void HandleApplicationDeactivated(object sender)
		{
			this.scrollList.SelectedIndexChanged -= new EventHandler(this.HandleScrollListSelectedIndexChanged);
			WindowFrame.MessageFilter -= new Epsitec.Common.Widgets.MessageHandler(this.HandlerMessageFilter);
			WindowFrame.ApplicationDeactivated -= new EventHandler(this.HandleApplicationDeactivated);
			this.scrollList.Dispose();
			this.scrollList = null;
			this.comboWindow.Dispose();
			this.comboWindow = null;
		}

		
		private void HandleButtonPressed(object sender, MessageEventArgs e)
		{
			this.scrollList = new ScrollList();
			this.scrollList.ScrollListStyle = ScrollListStyle.Simple;
			this.scrollList.ComboMode = true;
			this.scrollList.Bounds = new Drawing.Rectangle (0, 0, this.Width, 200);
			
			foreach (string text in this.items)
			{
				this.scrollList.AddText(text);
			}
			
			Drawing.Point     pos  = this.MapClientToScreen (new Drawing.Point(0, 0));
			ScreenInfo        info = ScreenInfo.Find (pos);
			Drawing.Rectangle area = info.WorkingArea;
			double            hMax = pos.Y-area.Bottom;
			
			this.scrollList.AdjustToContent(ScrollListAdjust.MoveUp, 40, hMax);
			
			this.scrollList.SelectedIndex = this.items.FindExactMatch (this.Text);
			this.scrollList.ShowSelect(ScrollListShow.Middle);
			this.scrollList.SelectedIndexChanged += new EventHandler(this.HandleScrollListSelectedIndexChanged);
			
			this.comboWindow = new WindowFrame();
			this.comboWindow.MakeFramelessWindow();
			pos = this.MapClientToScreen (new Drawing.Point(0, -this.scrollList.Height));
			this.comboWindow.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.scrollList.Width, this.scrollList.Height);
			WindowFrame.MessageFilter += new Epsitec.Common.Widgets.MessageHandler(this.HandlerMessageFilter);
			WindowFrame.ApplicationDeactivated += new EventHandler(this.HandleApplicationDeactivated);
			this.comboWindow.Root.Children.Add(this.scrollList);
			this.comboWindow.AnimateShow(Animation.RollDown);
			
			this.SetFocused(false);
			this.scrollList.SetFocused(true);
		}
		
		// Gestion d'un événement lorsque la scroll-liste est sélectionnée.
		private void HandleScrollListSelectedIndexChanged(object sender)
		{
			int sel = this.scrollList.SelectedIndex;
			if ( sel == -1 )  return;
			this.Text = this.scrollList.GetText(sel);
			this.OnTextChanged();
			this.OnTextInserted();
			this.SelectAll();
			this.SetFocused(true);

			this.scrollList.SelectedIndexChanged -= new EventHandler(this.HandleScrollListSelectedIndexChanged);
			WindowFrame.MessageFilter -= new Epsitec.Common.Widgets.MessageHandler(this.HandlerMessageFilter);
			WindowFrame.ApplicationDeactivated -= new EventHandler(this.HandleApplicationDeactivated);
			this.scrollList.Dispose();
			this.scrollList = null;
			this.comboWindow.Dispose();
			this.comboWindow = null;
		}

		
		protected ArrowButton						button;
		protected Helpers.StringCollection			items;
		protected WindowFrame						comboWindow;
		protected ScrollList						scrollList;
	}
}
