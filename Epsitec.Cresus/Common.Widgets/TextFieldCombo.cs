//	Désolé -- j'ai fait ces modifications dans l'urgence, avec mon propre
//	style. C'est possible que je prenne une fois le contrôle de ce source... / PA

namespace Epsitec.Common.Widgets
{
	using BundleAttribute  = Support.BundleAttribute;
	
	/// <summary>
	/// La classe TextFieldCombo implémente la ligne éditable avec bouton "v".
	/// </summary>
	public class TextFieldCombo : AbstractTextField, Collections.IStringCollectionHost, Support.Data.INamedStringSelection
	{
		public TextFieldCombo()
		{
			this.textFieldStyle = TextFieldStyle.Combo;

			this.items = new Collections.StringCollection(this);
			this.isCombo = true;
			
			this.button = new GlyphButton(this);
			this.button.Name = "Open";
			this.button.GlyphShape = GlyphShape.ArrowDown;
			this.button.ButtonStyle = ButtonStyle.Combo;
			this.button.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			
			this.default_button_width = this.button.Width;
			this.margins.Right = this.button.Width;
			
			this.ButtonShowCondition = ShowCondition.Always;
		}
		
		public TextFieldCombo(Widget embedder) : this ()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public GlyphShape						ButtonGlyphShape
		{
			get
			{
				return this.button.GlyphShape;
			}
			set
			{
				this.button.GlyphShape = value;
			}
		}
		
		public Button							Button
		{
			get
			{
				return this.button;
			}
		}
		
		[Bundle] public ShowCondition			ButtonShowCondition
		{
			get
			{
				return this.button_show_condition;
			}
			set
			{
				if (this.button_show_condition != value)
				{
					this.button_show_condition = value;
					this.UpdateButtonVisibility ();
				}
			}
		}
		
		public bool								IsComboOpen
		{
			get
			{
				return this.scrollList != null;
			}
		}
		
		
		#region Interface IBundleSupport
		public override void RestoreFromBundle(Epsitec.Common.Support.ObjectBundler bundler, Epsitec.Common.Support.ResourceBundle bundle)
		{
			base.RestoreFromBundle (bundler, bundle);
			this.items.RestoreFromBundle ("items", bundler, bundle);
		}
		
		public override void SerializeToBundle(Support.ObjectBundler bundler, Support.ResourceBundle bundle)
		{
			base.SerializeToBundle (bundler, bundle);
			this.items.SerializeToBundle ("items", bundler, bundle);
		}
		#endregion
		
		public bool FindMatch(string find, out int index, out bool exactMatch)
		{
			index = this.items.FindExactMatch(find);
			
			if ( index < 0 )
			{
				exactMatch = false;
				
				if ( find == "" )
				{
					return false;
				}
				
				
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
			if ( disposing )
			{
				this.button.Pressed -= new MessageEventHandler(this.HandleButtonPressed);
				this.button.Dispose();
				this.button = null;
			}
			
			base.Dispose(disposing);
		}

		
		protected override void UpdateButtonGeometry()
		{
			base.UpdateButtonGeometry();
			
			if ( this.button != null )
			{
				this.margins.Right = this.button.Visibility ? this.default_button_width : 0;
				this.button.Bounds = this.GetButtonBounds();
			}
		}
		
		protected virtual  void UpdateButtonVisibility()
		{
			this.SetButtonVisibility (this.ComputeButtonVisibility ());
		}
		
		protected virtual  bool ComputeButtonVisibility()
		{
			bool show = false;
			
			switch (this.ButtonShowCondition)
			{
				case ShowCondition.Always:
					show = true;
					break;
				
				case ShowCondition.Never:
					break;
				
				case ShowCondition.WhenFocused:
					show = this.IsFocused || this.IsComboOpen;
					break;
				
				case ShowCondition.WhenKeyboardFocused:
					show = this.IsKeyboardFocused || this.IsComboOpen;
					break;
				
				case ShowCondition.WhenModified:
					show = this.has_edited_text;
					break;
				
				default:
					throw new System.NotImplementedException (string.Format ("ButtonShowCondition.{0} not implemented.", this.ButtonShowCondition));
			}
			
			return show;
		}
		
		protected virtual  void SetButtonVisibility(bool show)
		{
			if (this.button != null)
			{
				if (this.button.Visibility != show)
				{
					this.button.Visibility = (show);
					
					this.UpdateButtonGeometry ();
					this.UpdateTextLayout ();
					this.UpdateMouseCursor (this.MapRootToClient (Message.State.LastPosition));
				}
			}
		}
		
		
		protected virtual  void OnSelectedIndexChanged()
		{
			if ( this.scrollList == null )
			{
				if ( this.SelectedIndexChanged != null )
				{
					this.SelectedIndexChanged(this);
				}
			}
		}

		protected override void OnTextDefined()
		{
			base.OnTextDefined ();
			
			this.has_edited_text = false;
		}
		
		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			
			this.UpdateButtonVisibility ();
		}

		protected override void OnIsKeyboardFocusedChanged(Types.PropertyChangedEventArgs e)
		{
			base.OnIsKeyboardFocusedChanged (e);
			
			this.UpdateButtonVisibility ();
		}
		
		protected override void OnTextEdited()
		{
			base.OnTextEdited ();
			
			if (this.has_edited_text == false)
			{
				this.has_edited_text = true;
				
				this.UpdateButtonVisibility ();
			}
		}



		
		protected virtual bool OpenComboAfterKeyDown(Message message)
		{
			if ( this.IsReadOnly )
			{
				IFeel feel = Feel.Factory.Active;
				
				if ( feel.TestComboOpenKey(message) )
				{
					this.OpenCombo();
					return true;
				}
			}
			
			return false;
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

		protected override bool ProcessKeyDown(Message message, Drawing.Point pos)
		{
			if ( this.OpenComboAfterKeyDown(message) == false )
			{
				switch ( message.KeyCode )
				{
					case KeyCode.ArrowUp:	this.Navigate(-1);	break;
					case KeyCode.ArrowDown:	this.Navigate(1);	break;
					
					default:
						return base.ProcessKeyDown(message, pos);
				}
			}
			
			return true;
		}
		
		protected virtual  void ProcessComboActivatedIndex(int sel)
		{
			sel = this.MapComboListToIndex(sel);
			
			// Cette méthode n'est appelée que lorsque le contenu de la liste déroulée
			// est validée par un clic de souris, au contraire de ProcessComboSelectedIndex
			// qui est appelée à chaque changement "visuel".
			
			if ( sel == -1 )  return;
			
			this.SelectedIndex = sel;
//			this.SetFocused(true);
			this.CloseCombo(true);
		}
		
		protected virtual  void ProcessComboSelectedIndex(int sel)
		{
			this.SelectedIndex = this.MapComboListToIndex(sel);
		}
		
		
		protected virtual void FillComboList(Collections.StringCollection list)
		{
			for ( int i=0 ; i<this.items.Count ; i++ )
			{
				string name = this.items.GetName(i);
				string text = this.items[i];
				list.Add(name, text);
			}
		}
		
		protected virtual int MapComboListToIndex(int value)
		{
			return (value < 0) ? -1 : value;
		}
		
		protected virtual int MapIndexToComboList(int value)
		{
			return (value < 0) ? -1 : value;
		}
		
		
		protected virtual void Navigate(int dir)
		{
			if ( this.items.Count == 0 )
			{
				return;
			}

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
		
		protected virtual void OpenCombo()
		{
			if ( this.IsComboOpen )  return;
			
			Support.CancelEventArgs cancel_event = new Support.CancelEventArgs ();
			this.OnOpeningCombo (cancel_event);
			
			if ( cancel_event.Cancel )  return;
			
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Margins shadow = adorner.GeometryMenuShadow;

			this.scrollList = new ScrollList(null);
			this.scrollList.ScrollListStyle = ScrollListStyle.Menu;
			this.scrollList.Bounds = new Drawing.Rectangle(0, 0, this.Width, 200);
			
			this.FillComboList(this.scrollList.Items);
			
			Drawing.Point     pos  = this.MapClientToScreen(new Drawing.Point(0, 0));
			ScreenInfo        info = ScreenInfo.Find(pos);
			Drawing.Rectangle area = info.WorkingArea;
			double            hMax = pos.Y-area.Bottom;

			if ( hMax > this.scrollList.Height || hMax > 100 )  // assez de place pour dérouler contre le bas ?
			{
				this.scrollList.AdjustHeightToContent(ScrollAdjustMode.MoveTop, 40, hMax);
				pos.X -= shadow.Left;
				pos.Y -= this.scrollList.Height+shadow.Bottom;
			}
			else	// déroule contre le haut ?
			{
				pos = this.MapClientToScreen(new Drawing.Point(0, this.Height));
				hMax = area.Top-pos.Y;
				this.scrollList.AdjustHeightToContent(ScrollAdjustMode.MoveTop, 40, hMax);
			}
			
			this.scrollList.SelectedIndex = this.MapIndexToComboList(this.SelectedIndex);
			this.scrollList.ShowSelected(ScrollShowMode.Center);
			
			this.comboWindow = new Window();
			this.comboWindow.MakeFramelessWindow();
			this.comboWindow.MakeFloatingWindow();
			this.comboWindow.Owner = this.Window;
			if ( adorner.AlphaMenu < 1.0 )
			{
				this.comboWindow.MakeLayeredWindow();
				this.comboWindow.Alpha = adorner.AlphaMenu;
				this.comboWindow.Root.BackColor = Drawing.Color.Transparent;
			}
			//?pos = this.MapClientToScreen(new Drawing.Point(0, 0));
			//?pos.X -= shadow.Left;
			//?pos.Y -= this.scrollList.Height+shadow.Bottom;
			this.comboWindow.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.scrollList.Width+shadow.Width, this.scrollList.Height+shadow.Height);
			this.scrollList.Location = new Drawing.Point(shadow.Left, shadow.Bottom);
			this.scrollList.SelectedIndexChanged += new Support.EventHandler(this.HandleScrollerSelectedIndexChanged);
			this.scrollList.SelectionActivated += new Support.EventHandler(this.HandleScrollListSelectionActivated);
			this.RegisterFilter();
			this.comboWindow.Root.Children.Add(this.scrollList);
			this.comboWindow.AnimateShow(Animation.RollDown);
			
//			this.SetFocused(true);
//			this.SetFocused(false);
			this.scrollList.SetFocused(true);
			
			this.openText = this.Text;
			this.OnOpenedCombo ();
		}
		
		protected virtual void CloseCombo(bool accept)
		{
			this.scrollList.SelectionActivated -= new Support.EventHandler(this.HandleScrollListSelectionActivated);
			this.scrollList.SelectedIndexChanged -= new Support.EventHandler(this.HandleScrollerSelectedIndexChanged);
			this.UnregisterFilter();
			this.scrollList.Dispose();
			this.scrollList = null;
			
			if ( Window.IsApplicationActive )
			{
				this.Window.MakeActive();
			}
			
			this.comboWindow.Dispose();
			this.comboWindow = null;
			
			this.SelectAll();
			
			if ( this.AutoFocus )
			{
				this.SetFocused(true);
			}
			
			if ( !accept )
			{
				this.Text = this.openText;
			}
			
			this.OnClosedCombo ();
			
			if ( this.openText != this.Text )
			{
				this.OnSelectedIndexChanged();
			}
		}

		
		protected virtual void OnOpeningCombo(Support.CancelEventArgs e)
		{
			if (this.OpeningCombo != null)
			{
				this.OpeningCombo (this, e);
			}
		}
		
		protected virtual void OnOpenedCombo()
		{
			System.Diagnostics.Debug.Assert (this.IsComboOpen == true);
			this.UpdateButtonVisibility ();
			
			if (this.OpenedCombo != null)
			{
				this.OpenedCombo (this);
			}
		}
		
		protected virtual void OnClosedCombo()
		{
			System.Diagnostics.Debug.Assert (this.IsComboOpen == false);
			this.UpdateButtonVisibility ();
			
			if (this.ClosedCombo != null)
			{
				this.ClosedCombo (this);
			}
		}
		
		
		private void MessageFilter(object sender, Message message)
		{
			Window window = sender as Window;
			
			System.Diagnostics.Debug.Assert(this.IsComboOpen);
			System.Diagnostics.Debug.Assert(window != null);
			
			if ( this.scrollList == null )  return;
			
			IFeel feel = Feel.Factory.Active;
			
			switch ( message.Type )
			{
				case MessageType.KeyPress:
					if ( feel.TestCancelKey(message) )
					{
						this.CloseCombo(false);
						message.Swallowed = true;
					}
					if ( feel.TestAcceptKey(message) )
					{
						this.CloseCombo(true);
						message.Swallowed = true;
					}
					if ( feel.TestNavigationKey(message) )
					{
						this.CloseCombo(true);
						Message.DefineLastWindow (this.Window);
					}
					break;
				
				case MessageType.MouseDown:
					Drawing.Point mouse = window.Root.MapClientToScreen(message.Cursor);
					Drawing.Point pos = this.scrollList.MapScreenToClient(mouse);
					if ( !this.scrollList.HitTest(pos) )
					{
						this.CloseCombo(false);
						message.Swallowed = ! message.NonClient;
					}
					break;
			}
		}
		
		
		private void HandleApplicationDeactivated(object sender)
		{
			this.CloseCombo(false);
		}

		private void HandleButtonPressed(object sender, MessageEventArgs e)
		{
			this.OpenCombo();
		}
		
		private void HandleScrollListSelectionActivated(object sender)
		{
			// L'utilisateur a cliqué dans la liste pour terminer son choix.
			
			this.ProcessComboActivatedIndex(this.scrollList.SelectedIndex);
		}
		
		private void HandleScrollerSelectedIndexChanged(object sender)
		{
			// L'utilisateur a simplement déplacé la souris dans la liste.
			
			this.ProcessComboSelectedIndex(this.scrollList.SelectedIndex);
		}
		
		
		private void RegisterFilter()
		{
			Window.MessageFilter          += new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
			Window.ApplicationDeactivated += new Support.EventHandler(this.HandleApplicationDeactivated);
			
			if ( this.Window != null &&
				 this.AutoFocus == false )
			{
				this.initiallyFocusedWidget = this.Window.FocusedWidget;
			}
		}
		
		private void UnregisterFilter()
		{
			Window.MessageFilter          -= new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
			Window.ApplicationDeactivated -= new Support.EventHandler(this.HandleApplicationDeactivated);
			
			if ( this.initiallyFocusedWidget != null )
			{
				if ( this.initiallyFocusedWidget.Window != null )
				{
					this.initiallyFocusedWidget.SetFocused(true);
				}
				
				this.initiallyFocusedWidget = null;
			}
		}
		
		
		
		#region IStringCollectionHost Members
		public void StringCollectionChanged()
		{
		}
		
		
		public Collections.StringCollection				Items
		{
			get { return this.items; }
		}
		#endregion
		
		#region INamedStringSelection Members
		public int									SelectedIndex
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
					this.OnSelectedIndexChanged();
//-					this.Cursor = 0;
					this.SelectAll();
				}
			}
		}
		
		public string								SelectedItem
		{
			get
			{
				int index = this.SelectedIndex;
				if ( index < 0 )  return "";
				return this.Items[index];
			}
			
			set
			{
				if ( value == null )
				{
					value = "";
				}
				
				int index = this.Items.IndexOf(value);
				if ( index < 0 )
				{
					this.Text = value;
					this.Cursor = 0;
					this.SelectAll();
				}
				else
				{
					this.SelectedIndex = index;
				}
			}
		}

		public string								SelectedName
		{
			// Nom de la ligne sélectionnée, null si aucune.
			get
			{
				int index = this.SelectedIndex;
				if ( index < 0 )  return "";
				return this.items.GetName(index);
			}
			
			set
			{
				if ( value == null )
				{
					value = "";
				}
				
				if ( this.SelectedName != value )
				{
					int index = -1;
					
					if ( value.Length > 0 )
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
		
		
		public event Support.EventHandler		SelectedIndexChanged;
		#endregion
		
		public event Support.CancelEventHandler	OpeningCombo;
		public event Support.EventHandler		OpenedCombo;
		public event Support.EventHandler		ClosedCombo;
		
		private Widget							initiallyFocusedWidget;
		
		protected GlyphButton					button;
		protected Collections.StringCollection		items;
		protected Window						comboWindow;
		protected ScrollList					scrollList;
		protected string						openText;
		protected ShowCondition					button_show_condition;
		protected bool							has_edited_text;
		protected double						default_button_width;
	}
}
