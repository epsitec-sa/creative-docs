using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// StyleCombo est un widget "combo" pour les styles graphique, de paragraphe ou de caractère.
	/// </summary>
	public class StyleCombo : AbstractTextField
	{
		public StyleCombo()
		{
			this.textFieldStyle = TextFieldStyle.Combo;

			this.isCombo = true;
			
			this.button = new GlyphButton(this);
			this.button.Name = "Open";
			this.button.GlyphShape = GlyphShape.ArrowDown;
			this.button.ButtonStyle = ButtonStyle.Combo;
			this.button.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			
			this.defaultButtonWidth = this.button.Width;
			this.margins.Right = this.button.Width;
			
			this.ButtonShowCondition = ShowCondition.Always;
		}
		
		public StyleCombo(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public Document Document
		{
			get
			{
				return this.document;
			}

			set
			{
				this.document = value;
			}
		}

		public StyleCategory StyleCategory
		{
			get
			{
				return this.styleCategory;
			}

			set
			{
				this.styleCategory = value;
			}
		}

		public int ExcludeRank
		{
			//	Ligne éventuelle à exclure.
			get
			{
				return this.excludeRank;
			}

			set
			{
				this.excludeRank = value;
			}
		}

		public bool IsDeep
		{
			//	Attributs cherchés en profondeur, dans les parents.
			get
			{
				return this.isDeep;
			}

			set
			{
				this.isDeep = value;
			}
		}

		public bool IsNoneLine
		{
			//	Première ligne avec <aucun>.
			get
			{
				return this.isNoneLine;
			}

			set
			{
				this.isNoneLine = value;
			}
		}

		public int SelectedIndex
		{
			get
			{
				return this.selectedIndex;
			}

			set
			{
				this.selectedIndex = value;
			}
		}

		public GlyphShape ButtonGlyphShape
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
		
		public Button Button
		{
			get
			{
				return this.button;
			}
		}
		
		[Bundle] public ShowCondition ButtonShowCondition
		{
			get
			{
				return this.buttonShowCondition;
			}

			set
			{
				if ( this.buttonShowCondition != value )
				{
					this.buttonShowCondition = value;
					this.UpdateButtonVisibility();
				}
			}
		}
		
		public bool IsComboOpen
		{
			get
			{
				return (this.list != null);
			}
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
				this.margins.Right = this.button.Visibility ? this.defaultButtonWidth : 0;
				this.button.Bounds = this.GetButtonBounds();
			}
		}
		
		protected virtual void UpdateButtonVisibility()
		{
			this.SetButtonVisibility(this.ComputeButtonVisibility());
		}
		
		protected virtual bool ComputeButtonVisibility()
		{
			bool show = false;
			
			switch ( this.ButtonShowCondition )
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
					show = this.hasEditedText;
					break;
				
				default:
					throw new System.NotImplementedException(string.Format("ButtonShowCondition.{0} not implemented.", this.ButtonShowCondition));
			}
			
			return show;
		}
		
		protected virtual void SetButtonVisibility(bool show)
		{
			if ( this.button != null )
			{
				if ( this.button.Visibility != show )
				{
					this.button.Visibility = (show);
					
					this.UpdateButtonGeometry();
					this.UpdateTextLayout();
					this.UpdateMouseCursor(this.MapRootToClient(Message.State.LastPosition));
				}
			}
		}
		
		
		protected override void OnTextDefined()
		{
			base.OnTextDefined();
			
			this.hasEditedText = false;
		}
		
		protected override void OnTextChanged()
		{
			base.OnTextChanged();
			
			this.UpdateButtonVisibility();
		}

		protected override void HandleFocused()
		{
			base.HandleFocused();
			this.UpdateButtonVisibility();
		}

		protected override void HandleDefocused()
		{
			base.HandleDefocused();
			this.UpdateButtonVisibility();
		}
		
		protected override void OnTextEdited()
		{
			base.OnTextEdited();
			
			if ( this.hasEditedText == false )
			{
				this.hasEditedText = true;
				this.UpdateButtonVisibility();
			}
		}



		
		protected virtual bool OpenComboAfterKeyDown(Message message)
		{
			if ( this.IsReadOnly )
			{
				IFeel feel = Common.Widgets.Feel.Factory.Active;
				
				if ( feel.TestComboOpenKey(message) )
				{
					this.OpenCombo();
					return true;
				}
			}
			
			return false;
		}
		

		
		protected override void ProcessMessage(Message message, Point pos)
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

		protected override bool ProcessKeyDown(Message message, Point pos)
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
		
		protected virtual void ProcessComboActivatedIndex(int sel)
		{
			//	Cette méthode n'est appelée que lorsque le contenu de la liste déroulée
			//	est validée par un clic de souris.
			sel = this.MapComboListToIndex(sel);
			if ( sel == -1 )  return;
			
			this.list.SelectRow(sel, true);
			this.CloseCombo(true);
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
		}
		
		protected virtual void OpenCombo()
		{
			if ( this.IsComboOpen )  return;

			int count = 0;
			int sel = -1;
			Common.Text.TextStyle[] styles = null;

			if ( this.styleCategory == StyleCategory.Graphic )
			{
				count = this.document.Aggregates.Count;
				sel = this.GetSelectedStyle(this.document.Aggregates, this.Text);
			}
		
			if ( this.styleCategory == StyleCategory.Paragraph || this.styleCategory == StyleCategory.Character )
			{
				styles = this.document.TextStyles(this.styleCategory);
				count = styles.Length;
				sel = this.GetSelectedTextStyle(styles, this.Text);
			}

			if ( count == 0 )  return;

			Support.CancelEventArgs cancelEvent = new Support.CancelEventArgs();
			this.OnOpeningCombo(cancelEvent);
			if ( cancelEvent.Cancel )  return;
			
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Margins margins = adorner.GeometryArrayMargins;

			double width = 116+128+margins.Left+margins.Right;
			double h = count*32+17+margins.Bottom+margins.Top;

			if ( this.styleCategory == StyleCategory.Graphic )
			{
				AggregateList list = new AggregateList();
				list = new AggregateList();
				list.Document = this.document;
				list.List = this.document.Aggregates;
				list.ExcludeRank = this.excludeRank;
				list.IsNoneLine = this.isNoneLine;
				list.IsDeep = this.isDeep;
				list.HScroller = false;
				list.VScroller = false;
				list.IsHiliteColumn = false;
				list.IsChildrensColumn = false;
				list.IsInitialSelection = false;
				list.FixSelection = sel;
				list.FixWidth = width;
				list.UpdateContent();
				this.list = list;
			}

			if ( this.styleCategory == StyleCategory.Paragraph || this.styleCategory == StyleCategory.Character )
			{
				TextStylesList list = new TextStylesList();
				list.Document = this.document;
				list.Category = this.styleCategory;
				list.List = styles;
				list.ExcludeRank = this.excludeRank;
				list.IsNoneLine = this.isNoneLine;
				list.IsDeep = this.isDeep;
				list.HScroller = false;
				list.VScroller = false;
				list.IsHiliteColumn = false;
				list.IsChildrensColumn = false;
				list.IsInitialSelection = false;
				list.FixSelection = sel;
				list.FixWidth = width;
				list.UpdateContent();
				this.list = list;
			}

			Point pos = this.MapClientToScreen(new Point(0, 1));
			ScreenInfo info = ScreenInfo.Find(pos);
			Rectangle area = info.WorkingArea;
			double hMax = pos.Y-area.Bottom;
			if ( h > hMax )  // dépasse en bas ?
			{
				if ( hMax > 100 )  // place minimale ?
				{
					h = hMax;
					this.list.VScroller = true;
					width += 16;
				}
				else	// déroule contre le haut ?
				{
					pos = this.MapClientToScreen(new Point(0, this.Height-1));
					hMax = area.Top-pos.Y;
					if ( h > hMax )  // dépasse en haut ?
					{
						h = hMax;
						this.list.VScroller = true;
						width += 16;
					}
					pos.Y += h;
				}
			}
			pos.Y -= h;

			if ( pos.X+width > area.Right )  // dépasse à droite ?
			{
				pos.X = area.Right-width;
			}

			this.list.Bounds = new Rectangle(0, 0, width, h);
			this.list.Location = new Point(0, 0);
			this.list.FinalSelectionChanged += new EventHandler(this.HandleListSelectionActivated);
			
			this.comboWindow = new Window();
			this.comboWindow.MakeFramelessWindow();
			this.comboWindow.MakeFloatingWindow();
			this.comboWindow.Owner = this.Window;
			this.comboWindow.WindowBounds = new Rectangle(pos.X, pos.Y, this.list.Width, this.list.Height);
			this.RegisterFilter();
			this.comboWindow.Root.Children.Add(this.list);
			this.comboWindow.AnimateShow(Animation.RollDown);
			
			this.list.Focus();
			
			this.OnOpenedCombo();
		}
		
		protected virtual void CloseCombo(bool accept)
		{
			if ( accept )
			{
				int row = this.list.SelectedRow;
				int rank = this.list.RowToRank(row);
				if ( rank == -1 )  rank = -2;  // ligne <aucun> ?
				this.selectedIndex = rank;
			}
			else
			{
				this.selectedIndex = -1;
			}

			this.list.FinalSelectionChanged -= new EventHandler(this.HandleListSelectionActivated);
			this.UnregisterFilter();
			this.list.Dispose();
			this.list = null;
			
			if ( Window.IsApplicationActive )
			{
				this.Window.MakeActive();
			}
			
			this.comboWindow.Dispose();
			this.comboWindow = null;
			
			this.SelectAll();
			
			if ( this.AutoFocus )
			{
				this.Focus();
			}
			
			this.OnClosedCombo();
		}

		
		protected virtual void OnOpeningCombo(Support.CancelEventArgs e)
		{
			if ( this.OpeningCombo != null )
			{
				this.OpeningCombo(this, e);
			}
		}
		
		protected virtual void OnOpenedCombo()
		{
			System.Diagnostics.Debug.Assert(this.IsComboOpen == true);
			this.UpdateButtonVisibility();
			
			if ( this.OpenedCombo != null )
			{
				this.OpenedCombo(this);
			}
		}
		
		protected virtual void OnClosedCombo()
		{
			System.Diagnostics.Debug.Assert(this.IsComboOpen == false);
			this.UpdateButtonVisibility();
			
			if ( this.ClosedCombo != null )
			{
				this.ClosedCombo(this);
			}
		}
		
		
		private void MessageFilter(object sender, Message message)
		{
			Window window = sender as Window;
			
			System.Diagnostics.Debug.Assert(this.IsComboOpen);
			System.Diagnostics.Debug.Assert(window != null);
			if ( this.list == null )  return;
			
			IFeel feel = Common.Widgets.Feel.Factory.Active;
			
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
						//?Widgets.Message.DefineLastWindow(this.Window);
					}
					break;
				
				case MessageType.MouseDown:
					Point mouse = window.Root.MapClientToScreen(message.Cursor);
					Point pos = this.list.MapScreenToClient(mouse);
					if ( !this.list.HitTest(pos) )
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
		
		private void HandleListSelectionActivated(object sender)
		{
			//	L'utilisateur a cliqué dans la liste pour terminer son choix.
			this.ProcessComboActivatedIndex(this.list.SelectedRow);
		}

		
		private void RegisterFilter()
		{
			Window.MessageFilter          += new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
			Window.ApplicationDeactivated += new Support.EventHandler(this.HandleApplicationDeactivated);
			
			if ( this.Window != null && this.AutoFocus == false )
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
					this.initiallyFocusedWidget.Focus();
				}
				
				this.initiallyFocusedWidget = null;
			}
		}


		protected int GetSelectedStyle(UndoableList aggregates, string currentStyle)
		{
			//	Cherche le rang du style graphique actuellement en édition.
			for ( int i=0 ; i<aggregates.Count ; i++ )
			{
				Properties.Aggregate aggregate = aggregates[i] as Properties.Aggregate;

				if ( aggregate.AggregateName == currentStyle )
				{
					return i;
				}
			}

			return -1;
		}

		protected int GetSelectedTextStyle(Text.TextStyle[] styles, string currentStyle)
		{
			//	Cherche le rang du style de texte actuellement en édition.
			for ( int i=0 ; i<styles.Length ; i++ )
			{
				Text.TextStyle style = styles[i];

				if ( this.document.TextContext.StyleList.StyleMap.GetCaption(style) == currentStyle )
				{
					return i;
				}
			}

			return -1;
		}
		
		
		
		
		public event Support.CancelEventHandler	OpeningCombo;
		public event Support.EventHandler		OpenedCombo;
		public event Support.EventHandler		ClosedCombo;
		
		private Widget							initiallyFocusedWidget;

		protected Document						document;
		protected StyleCategory					styleCategory = StyleCategory.Graphic;
		protected int							excludeRank = -1;
		protected bool							isDeep = false;
		protected bool							isNoneLine = false;
		protected GlyphButton					button;
		protected Window						comboWindow;
		protected AbstractStyleList				list;
		protected int							selectedIndex = -1;
		protected ShowCondition					buttonShowCondition;
		protected bool							hasEditedText;
		protected double						defaultButtonWidth;
	}
}
