namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldCombo implémente la ligne éditable avec bouton "v".
	/// </summary>
	public class TextFieldCombo : AbstractTextField, Helpers.IStringCollectionHost, Support.IBundleSupport
	{
		public TextFieldCombo()
		{
			this.items = new Helpers.StringCollection (this);
			this.isCombo = true;
			
			this.button = new ArrowButton();
			this.button.Direction = Direction.Down;
			this.button.ButtonStyle = ButtonStyle.Scroller;
			this.button.Parent = this;
			this.button.Pressed += new MessageEventHandler (this.HandleButtonPressed);
			this.button.Dock = DockStyle.Right;
			
			this.rightMargin = this.button.Width;
		}
		
		
		public override void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			base.RestoreFromBundle (bundler, bundle);
			
			Support.ResourceBundle items = bundle.GetFieldBundle ("items");
			
			if (items != null)
			{
				string[] names = items.FieldNames;
				System.Array.Sort (names);
				
				for (int i = 0; i < items.CountFields; i++)
				{
					string name = names[i];
					string item = items[name] as string;
					
					if (item == null)
					{
						throw new Support.ResourceException (string.Format ("Item '{0}' is invalid", name));
					}
					
					this.Items.Add (Support.ResourceBundle.ExtractName (name), item);
				}
			}
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

		protected override void ProcessKeyDown(KeyCode key, bool isShiftPressed, bool isCtrlPressed)
		{
			if (this.IsReadOnly)
			{
				if (key == KeyCode.ArrowDown)
				{
					this.OpenCombo ();
				}
			}
			else
			{
				switch (key)
				{
					case KeyCode.ArrowUp:
						this.Navigate(-1);
						break;
					case KeyCode.ArrowDown:
						this.Navigate(1);
						break;
					default:
						base.ProcessKeyDown (key, isShiftPressed, isCtrlPressed);
						break;
				}
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
		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if (this.IsReadOnly)
			{
				if (message.Type == MessageType.MouseDown)
				{
					this.OpenCombo ();
					return;
				}
			}
			
			base.ProcessMessage (message, pos);
		}

		
		private void MessageFilter(object sender, Message message)
		{
			if ( this.scrollList == null )  return;
			Window window = sender as Window;

			switch ( message.Type )
			{
				case MessageType.KeyPress:
					if (message.KeyCode == KeyCode.Escape)
					{
						this.CloseCombo ();
						message.Swallowed = true;
					}
					break;
				case MessageType.MouseDown:
					Drawing.Point mouse = window.Root.MapClientToScreen(message.Cursor);
					Drawing.Point pos = this.scrollList.MapScreenToClient(mouse);
					if ( !this.scrollList.HitTest(pos) )
					{
						this.CloseCombo ();
						message.Swallowed = ! message.NonClient;
					}
					break;
			}
		}
		
		private void OpenCombo()
		{
			this.scrollList = new ScrollList();
			this.scrollList.ScrollListStyle = ScrollListStyle.Simple;
			this.scrollList.ComboMode = true;
			this.scrollList.Bounds = new Drawing.Rectangle (0, 0, this.Width, 200);
			
			for (int i = 0; i < this.items.Count; i++)
			{
				string name = this.items.GetName (i);
				string text = this.items[i];
				this.scrollList.Items.Add(name, text);
			}
			
			Drawing.Point     pos  = this.MapClientToScreen (new Drawing.Point(0, 0));
			ScreenInfo        info = ScreenInfo.Find (pos);
			Drawing.Rectangle area = info.WorkingArea;
			double            hMax = pos.Y-area.Bottom;
			
			this.scrollList.AdjustHeightToContent(ScrollListAdjust.MoveUp, 40, hMax);
			this.scrollList.SelectedIndex = this.items.FindExactMatch (this.Text);
			this.scrollList.ShowSelectedLine(ScrollListShow.Middle);
			
			this.comboWindow = new Window();
			this.comboWindow.MakeFramelessWindow();
			pos = this.MapClientToScreen (new Drawing.Point(0, -this.scrollList.Height));
			this.comboWindow.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.scrollList.Width, this.scrollList.Height);
			this.scrollList.SelectedIndexChanged += new EventHandler(this.HandleScrollerSelectedIndexChanged);
			this.scrollList.Validation += new EventHandler(this.HandleScrollListValidation);
			Window.MessageFilter += new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
			Window.ApplicationDeactivated += new EventHandler(this.HandleApplicationDeactivated);
			this.comboWindow.Root.Children.Add(this.scrollList);
			this.comboWindow.AnimateShow(Animation.RollDown);
			
			this.SetFocused(true);
			this.SetFocused(false);
			this.scrollList.SetFocused(true);
		}
		
		private void CloseCombo()
		{
			this.scrollList.Validation -= new EventHandler(this.HandleScrollListValidation);
			this.scrollList.SelectedIndexChanged -= new EventHandler(this.HandleScrollerSelectedIndexChanged);
			Window.MessageFilter -= new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
			Window.ApplicationDeactivated -= new EventHandler(this.HandleApplicationDeactivated);
			
			this.scrollList.Dispose();
			this.scrollList = null;
			this.comboWindow.Dispose();
			this.comboWindow = null;
			
			this.SelectAll ();
			this.SetFocused (true);
		}

		private void HandleApplicationDeactivated(object sender)
		{
			this.CloseCombo ();
		}

		
		private void HandleButtonPressed(object sender, MessageEventArgs e)
		{
			this.OpenCombo ();
		}
		
		private void HandleScrollListValidation(object sender)
		{
			// Gestion d'un événement lorsque la scroll-liste est sélectionnée.
			
			int sel = this.scrollList.SelectedIndex;
			if ( sel == -1 )  return;
			this.Text = this.scrollList.Items[sel];
			this.OnTextChanged();
			this.OnTextInserted();
			this.SelectAll();
			this.SetFocused(true);
			this.CloseCombo();
		}
		
		private void HandleScrollerSelectedIndexChanged(object sender)
		{
			int sel = this.scrollList.SelectedIndex;
			this.Text = this.scrollList.Items[sel];
		}

		#region IStringCollectionHost Members
		public void StringCollectionChanged()
		{
		}
		#endregion
		
		
		
		protected ArrowButton						button;
		protected Helpers.StringCollection			items;
		protected Window							comboWindow;
		protected ScrollList						scrollList;
	}
}
