namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldCombo implémente la ligne éditable avec bouton "v".
	/// </summary>
	public class TextFieldCombo : AbstractTextField, Helpers.IStringCollectionHost, Support.IBundleSupport
	{
		public TextFieldCombo()
		{
			this.textStyle = TextFieldStyle.Combo;

			this.items = new Helpers.StringCollection(this);
			this.isCombo = true;
			
			this.button = new GlyphButton(this);
			this.button.Name = "Open";
			this.button.GlyphType = GlyphType.ArrowDown;
			this.button.ButtonStyle = ButtonStyle.Combo;
			this.button.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			//this.button.Dock = DockStyle.Right;

			this.margins.Right = this.button.Width;
		}
		
		public TextFieldCombo(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			base.RestoreFromBundle(bundler, bundle);
			
			Support.ResourceBundle items = bundle["items"].AsBundle;
			
			if ( items != null )
			{
				string[] names = items.FieldNames;
				System.Array.Sort(names);
				
				for ( int i=0 ; i<items.CountFields ; i++ )
				{
					string name = names[i];
					string item = items[name].AsString;
					
					if ( item == null )
					{
						throw new Support.ResourceException(string.Format("Item '{0}' is invalid", name));
					}
					
					this.Items.Add(Support.ResourceBundle.ExtractName(name), item);
				}
			}
		}
		
		
		public Helpers.StringCollection Items
		{
			get { return this.items; }
		}
		
		
		public int SelectedIndex
		{
			get
			{
				int		sel;
				bool	exact;
				if ( this.FindMatch(this.Text, out sel, out exact) )  return sel;
				return -1;
			}

			set
			{
				string text = "";
				if ( value >= 0 && value < this.items.Count )
				{
					text = this.items[value];
				}

				if ( this.Text != text )
				{
					this.Text = text;
					this.OnTextChanged();
					this.OnTextInserted();
					this.OnSelectedIndexChanged();
					this.SelectAll();
				}
			}
		}
		
		public string SelectedItem
		{
			get
			{
				int index = this.SelectedIndex;
				if ( index < 0 )  return null;
				return this.Items[index];
			}
			
			set
			{
				this.SelectedIndex = this.Items.IndexOf (value);
			}
		}

		public string SelectedName
		{
			// Nom de la ligne sélectionnée, null si aucune.
			get
			{
				int index = this.SelectedIndex;
				if ( index < 0 )  return null;
				return this.items.GetName(index);
			}
			
			set
			{
				if ( this.SelectedName != value )
				{
					int index = -1;
					
					if ( value != null )
					{
						index = this.items.FindNameIndex(value);
						
						if ( index < 0 )
						{
							throw new System.ArgumentException(string.Format("No element named '{0}' in list", value));
						}
					}
					
					this.SelectedIndex = index;
				}
			}
		}

		public bool FindMatch(string find, out int index, out bool exactMatch)
		{
			index = this.items.FindExactMatch(find);
			
			if ( index < 0 )
			{
				exactMatch = false;
				index = this.items.FindStartMatch(find);
				
				if ( index < 0 )
				{
					return false;
				}
			}
			else
			{
				exactMatch = true;
			}
			
			return true;
		}

		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			
			if ( disposing )
			{
				this.button.Pressed -= new MessageEventHandler(this.HandleButtonPressed);
				this.button.Dispose();
				this.button = null;
			}
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			if ( this.button != null )
			{
				IAdorner adorner = Widgets.Adorner.Factory.Active;
				Drawing.Rectangle rect = new Drawing.Rectangle();
				rect.Left   = this.Bounds.Width-this.margins.Right-adorner.GeometryComboRightMargin;
				rect.Right  = this.Bounds.Width-adorner.GeometryComboRightMargin;
				rect.Bottom = adorner.GeometryComboBottomMargin;
				rect.Top    = this.Bounds.Height-adorner.GeometryComboTopMargin;
				this.button.Bounds = rect;
			}
		}

		protected override void OnAdornerChanged()
		{
			this.UpdateClientGeometry();
			base.OnAdornerChanged();
		}
		
		protected virtual void OnSelectedIndexChanged()
		{
			if (this.SelectedIndexChanged != null)
			{
				this.SelectedIndexChanged (this);
			}
		}

		
		protected override bool ProcessKeyDown(KeyCode key, bool isShiftPressed, bool isCtrlPressed)
		{
			if ( this.IsReadOnly )
			{
				if ( key == KeyCode.ArrowUp || key == KeyCode.ArrowDown )
				{
					this.OpenCombo();
					return true;
				}
			}
			else
			{
				switch ( key )
				{
					case KeyCode.ArrowUp:
						this.Navigate(-1);
						return true;

					case KeyCode.ArrowDown:
						this.Navigate(1);
						return true;

					default:
						return base.ProcessKeyDown(key, isShiftPressed, isCtrlPressed);
				}
			}
			
			return false;
		}
		
		
		protected void Navigate(int dir)
		{
			// Cherche le nom suivant ou précédent dans la comboList, même si elle
			// n'est pas "déroulée".
			
			int		sel;
			bool	exact;

			if ( this.FindMatch(this.Text, out sel, out exact) )
			{
				if ( exact )  sel += dir;
			}
			
			sel = System.Math.Max(sel, 0);
			sel = System.Math.Min(sel, this.items.Count-1);
			
			this.SelectedIndex = sel;
			this.SetFocused(true);
		}
		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if ( message.Type == MessageType.MouseWheel )
			{
				if ( message.Wheel > 0 )  this.Navigate(-1);
				if ( message.Wheel < 0 )  this.Navigate(1);
				message.Consumer = this;
				return;
			}

			if ( this.IsReadOnly )
			{
				if ( message.Type == MessageType.MouseDown )
				{
					this.OpenCombo();
					return;
				}
			}
			
			base.ProcessMessage(message, pos);
		}

		
		private void MessageFilter(object sender, Message message)
		{
			if ( this.scrollList == null )  return;
			Window window = sender as Window;

			switch ( message.Type )
			{
				case MessageType.KeyPress:
					if ( message.KeyCode == KeyCode.Escape )
					{
						this.CloseCombo();
						message.Swallowed = true;
					}
					break;
				case MessageType.MouseDown:
					Drawing.Point mouse = window.Root.MapClientToScreen(message.Cursor);
					Drawing.Point pos = this.scrollList.MapScreenToClient(mouse);
					if ( !this.scrollList.HitTest(pos) )
					{
						this.CloseCombo();
						message.Swallowed = ! message.NonClient;
					}
					break;
			}
		}
		
		private void OpenCombo()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Margins shadow = adorner.GeometryMenuShadow;

			this.scrollList = new ScrollList(null);
			this.scrollList.ScrollListStyle = ScrollListStyle.Menu;
			this.scrollList.Bounds = new Drawing.Rectangle(0, 0, this.Width, 200);
			
			for ( int i=0 ; i<this.items.Count ; i++ )
			{
				string name = this.items.GetName(i);
				string text = this.items[i];
				this.scrollList.Items.Add(name, text);
			}
			
			Drawing.Point     pos  = this.MapClientToScreen(new Drawing.Point(0, 0));
			ScreenInfo        info = ScreenInfo.Find(pos);
			Drawing.Rectangle area = info.WorkingArea;
			double            hMax = pos.Y-area.Bottom;
			
			this.scrollList.AdjustHeightToContent(ScrollListAdjust.MoveUp, 40, hMax);
			this.scrollList.SelectedIndex = this.SelectedIndex;
			this.scrollList.ShowSelectedLine(ScrollListShow.Middle);
			
			this.comboWindow = new Window();
			this.comboWindow.MakeFramelessWindow();
			this.comboWindow.MakeToolWindow();
			this.comboWindow.Owner = this.Window;
			if ( adorner.AlphaVMenu < 1.0 )
			{
				this.comboWindow.MakeLayeredWindow();
				this.comboWindow.Alpha = adorner.AlphaVMenu;
				this.comboWindow.Root.BackColor = Drawing.Color.Transparent;
			}
			pos = this.MapClientToScreen(new Drawing.Point(-shadow.Left, -this.scrollList.Height-shadow.Bottom));
			this.comboWindow.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.scrollList.Width+shadow.Width, this.scrollList.Height+shadow.Height);
			this.scrollList.Location = new Drawing.Point(shadow.Left, shadow.Bottom);
			this.scrollList.SelectedIndexChanged += new Support.EventHandler(this.HandleScrollerSelectedIndexChanged);
			this.scrollList.Validation += new Support.EventHandler(this.HandleScrollListValidation);
			Window.MessageFilter += new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
			Window.ApplicationDeactivated += new Support.EventHandler(this.HandleApplicationDeactivated);
			this.comboWindow.Root.Children.Add(this.scrollList);
			this.comboWindow.AnimateShow(Animation.RollDown);
			
			this.SetFocused(true);
			this.SetFocused(false);
			this.scrollList.SetFocused(true);
		}
		
		private void CloseCombo()
		{
			this.scrollList.Validation -= new Support.EventHandler(this.HandleScrollListValidation);
			this.scrollList.SelectedIndexChanged -= new Support.EventHandler(this.HandleScrollerSelectedIndexChanged);
			Window.MessageFilter -= new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
			Window.ApplicationDeactivated -= new Support.EventHandler(this.HandleApplicationDeactivated);
			
			this.scrollList.Dispose();
			this.scrollList = null;
			this.comboWindow.Dispose();
			this.comboWindow = null;
			
			this.Window.MakeActive();
			this.SelectAll();
			this.SetFocused(true);
		}

		private void HandleApplicationDeactivated(object sender)
		{
			this.CloseCombo();
		}

		
		private void HandleButtonPressed(object sender, MessageEventArgs e)
		{
			this.OpenCombo();
		}
		
		private void HandleScrollListValidation(object sender)
		{
			// Gestion d'un événement lorsque la scroll-liste est sélectionnée.
			
			int sel = this.scrollList.SelectedIndex;
			if ( sel == -1 )  return;
			this.SelectedIndex = sel;
			this.SetFocused(true);
			this.CloseCombo();
		}
		
		private void HandleScrollerSelectedIndexChanged(object sender)
		{
			this.SelectedIndex = this.scrollList.SelectedIndex;
		}

		#region IStringCollectionHost Members
		public void StringCollectionChanged()
		{
		}
		#endregion
		
		
		public event Support.EventHandler			SelectedIndexChanged;
		
		protected GlyphButton						button;
		protected Helpers.StringCollection			items;
		protected Window							comboWindow;
		protected ScrollList						scrollList;
	}
}
