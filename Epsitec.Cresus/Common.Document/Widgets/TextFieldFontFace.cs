using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Widgets
{
	using BundleAttribute  = Support.BundleAttribute;
	
	/// <summary>
	/// La classe TextFieldFontFace implémente la ligne éditable avec bouton "v" pour choisir une police.
	/// </summary>
	public class TextFieldFontFace : AbstractTextField
	{
		public TextFieldFontFace()
		{
			this.textFieldStyle = TextFieldStyle.Combo;

			this.button = new GlyphButton(this);
			this.button.Name = "Open";
			this.button.GlyphShape = GlyphShape.ArrowDown;
			this.button.ButtonStyle = ButtonStyle.Combo;
			this.button.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			
			this.defaultButtonWidth = this.button.Width;
			this.margins.Right = this.button.Width;
			
			this.ButtonShowCondition = ShowCondition.Always;
		}
		
		public TextFieldFontFace(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override bool					IsCombo
		{
			get
			{
				return true;
			}
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
		
		public bool								IsComboOpen
		{
			get
			{
				return this.fontSelector != null;
			}
		}


		public System.Collections.ArrayList FontList
		{
			get
			{
				return this.fontList;
			}
			set
			{
				this.fontList = value;
			}
		}

		public double SampleHeight
		{
			get
			{
				return this.sampleHeight;
			}
			set
			{
				this.sampleHeight = value;
			}
		}

		public bool SampleAbc
		{
			get
			{
				return this.sampleAbc;
			}

			set
			{
				this.sampleAbc = value;
			}
		}

		public int QuickCount
		{
			get
			{
				return this.quickCount;
			}
			set
			{
				this.quickCount = value;
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
		
		protected virtual  void UpdateButtonVisibility()
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
					this.button.Visibility = show;
					
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

		protected override void OnIsKeyboardFocusedChanged(Types.PropertyChangedEventArgs e)
		{
			base.OnIsKeyboardFocusedChanged(e);
			
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
				IFeel feel = Epsitec.Common.Widgets.Feel.Factory.Active;
				
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
		
		
		protected virtual void Navigate(int dir)
		{
		}
		
		protected virtual void OpenCombo()
		{
			if ( this.IsComboOpen )  return;
			
			Support.CancelEventArgs cancelEvent = new Support.CancelEventArgs();
			this.OnComboOpening(cancelEvent);
			
			if ( cancelEvent.Cancel )  return;
			
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			Drawing.Margins shadow = adorner.GeometryMenuShadow;

			Drawing.Point pos  = this.MapClientToScreen(new Drawing.Point(0, 0));
			ScreenInfo info = ScreenInfo.Find(pos);
			Drawing.Rectangle area = info.WorkingArea;

			double bestHeight = FontSelector.BestHeight(500, this.fontList.Count, this.sampleHeight);

			bool bottom = true;  // déroule contre le bas
			if ( pos.Y-area.Bottom < bestHeight )
			{
				bottom = pos.Y > (area.Bottom+area.Top)/2;  // choix du sens de déroulement
			}

			double w = FontSelector.BestWidth(this.sampleHeight, this.sampleAbc);
			double h = 0;

			if ( pos.X+w > area.Right )  // dépasse à droite ?
			{
				pos.X = area.Right-w;
			}

			if ( bottom )  // déroule contre le bas ?
			{
				h = System.Math.Min(pos.Y-area.Bottom, 500);
				h = FontSelector.BestHeight(h, this.fontList.Count, this.sampleHeight);
				pos.Y -= h+shadow.Bottom;
			}
			else	// déroule contre le haut ?
			{
				h = System.Math.Min(area.Top-pos.Y, 500);
				h = FontSelector.BestHeight(h, this.fontList.Count, this.sampleHeight);
				pos.Y += this.Height;
			}

			this.fontSelector = new FontSelector(null);
			this.fontSelector.Bounds = new Drawing.Rectangle(0, 0, w, h);
			this.fontSelector.FontList = this.fontList;
			this.fontSelector.QuickCount = this.quickCount;
			this.fontSelector.SampleHeight = this.sampleHeight;
			this.fontSelector.SampleAbc = this.sampleAbc;
			this.fontSelector.SelectedFontFace = this.Text;
			
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
			this.comboWindow.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.fontSelector.Width+shadow.Width, this.fontSelector.Height+shadow.Height);
			this.fontSelector.Location = new Drawing.Point(shadow.Left, shadow.Bottom);
			this.fontSelector.SelectionChanged += new Support.EventHandler(this.HandleSelectorSelectionChanged);
			this.RegisterFilter();
			this.comboWindow.Root.Children.Add(this.fontSelector);
			this.comboWindow.WindowAnimationEnded += new Epsitec.Common.Support.EventHandler(this.HandleComboWindowWindowAnimationEnded);
			this.comboWindow.AnimateShow(Animation.RollDown);
			
			this.openText = this.Text;
			this.OnComboOpened();
		}
		
		protected virtual void CloseCombo(bool accept)
		{
			this.fontSelector.SelectionChanged -= new Support.EventHandler(this.HandleSelectorSelectionChanged);
			this.UnregisterFilter();
			this.fontSelector.Dispose();
			this.fontSelector = null;
			
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
			
			if ( !accept )
			{
				this.Text = this.openText;
			}
			
			this.OnComboClosed();
		}

		
		protected virtual void OnComboOpening(Support.CancelEventArgs e)
		{
			if ( this.ComboOpening != null )
			{
				this.ComboOpening(this, e);
			}
		}
		
		protected virtual void OnComboOpened()
		{
			System.Diagnostics.Debug.Assert(this.IsComboOpen == true);
			this.UpdateButtonVisibility();
			
			if ( this.ComboOpened != null )
			{
				this.ComboOpened(this);
			}
		}
		
		protected virtual void OnComboClosed()
		{
			System.Diagnostics.Debug.Assert(this.IsComboOpen == false);
			this.UpdateButtonVisibility();
			
			if ( this.ComboClosed != null )
			{
				this.ComboClosed(this);
			}
			
			if ( this.Window != null )
			{
				this.Window.RestoreLogicalFocus();
			}
		}
		
		
		private void MessageFilter(object sender, Message message)
		{
			Window window = sender as Window;
			
			System.Diagnostics.Debug.Assert(this.IsComboOpen);
			System.Diagnostics.Debug.Assert(window != null);
			
			if ( this.fontSelector == null )  return;
			
			IFeel feel = Epsitec.Common.Widgets.Feel.Factory.Active;
			
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
						//?Message.DefineLastWindow(this.Window);
					}
					break;
				
				case MessageType.MouseDown:
					Drawing.Point mouse = window.Root.MapClientToScreen(message.Cursor);
					Drawing.Point pos = this.fontSelector.MapScreenToClient(mouse);
					if ( !this.fontSelector.HitTest(pos) )
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
		
		private void HandleSelectorSelectionChanged(object sender)
		{
			//	L'utilisateur a cliqué dans la liste pour terminer son choix.
			string text = this.fontSelector.SelectedFontFace;
			if ( this.Text != text )
			{
				this.Text = text;
				this.SelectAll();
			}

			this.CloseCombo(true);
		}
		
		private void HandleComboWindowWindowAnimationEnded(object sender)
		{
			this.comboWindow.MakeFocused();
			this.fontSelector.Focus();
			System.Diagnostics.Debug.WriteLine("Focus set to window...");
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
		
		
		
		public event Support.CancelEventHandler	ComboOpening;
		public event Support.EventHandler		ComboOpened;
		public event Support.EventHandler		ComboClosed;
		
		private Widget							initiallyFocusedWidget;
		
		protected GlyphButton					button;
		protected System.Collections.ArrayList	fontList;
		protected double						sampleHeight;
		protected bool							sampleAbc;
		protected int							quickCount;
		protected Window						comboWindow;
		protected FontSelector					fontSelector;
		protected string						openText;
		protected ShowCondition					buttonShowCondition;
		protected bool							hasEditedText;
		protected double						defaultButtonWidth;
	}
}
