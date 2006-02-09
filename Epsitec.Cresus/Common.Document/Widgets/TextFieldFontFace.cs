using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe TextFieldFontFace implémente la ligne éditable avec bouton "v" pour choisir une police.
	/// </summary>
	public class TextFieldFontFace : TextFieldCombo
	{
		public TextFieldFontFace()
		{
		}
		
		public TextFieldFontFace(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public System.Collections.ArrayList		FontList
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

		public double							SampleHeight
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

		public bool								SampleAbc
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

		public int								QuickCount
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
			if (disposing)
			{
				//	TODO: ...
			}
			
			base.Dispose(disposing);
		}

		protected override void Navigate(int dir)
		{
		}
		
		
		protected override AbstractMenu CreateMenu()
		{
			this.fontSelector = new FontSelector(null);
			this.fontSelector.FontList = this.fontList;
			this.fontSelector.QuickCount = this.quickCount;
			this.fontSelector.SampleHeight = this.sampleHeight;
			this.fontSelector.SampleAbc = this.sampleAbc;
			
			TextFieldComboMenu menu = new TextFieldComboMenu ();
			
			menu.Contents = this.fontSelector;
			menu.AdjustSize();
			
			//	On n'a pas le droit de définir le "SelectedFontFace"
			//	avant d'avoir fait cette mise à jour du contenu avec
			//	la nouvelle taille ajustée, sinon on risque d'avoir
			//	un offset incorrect pour le début...
			
			this.fontSelector.UpdateContents();
			this.fontSelector.SelectedFontFace = this.Text;
			
			this.fontSelector.SelectionChanged += new Support.EventHandler(this.HandleSelectorSelectionChanged);
			
			return menu;
		}
		
//		protected override void OpenCombo()
//		{
//			if ( this.IsComboOpen )  return;
//			
//			Support.CancelEventArgs cancelEvent = new Support.CancelEventArgs();
//			this.OnComboOpening(cancelEvent);
//			
//			if ( cancelEvent.Cancel )  return;
//			
//			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
//			Drawing.Margins shadow = adorner.GeometryMenuShadow;
//
//			Drawing.Point pos  = this.MapClientToScreen(new Drawing.Point(0, 0));
//			ScreenInfo info = ScreenInfo.Find(pos);
//			Drawing.Rectangle area = info.WorkingArea;
//
//			double bestHeight = FontSelector.BestHeight(500, this.fontList.Count, this.sampleHeight);
//
//			bool bottom = true;  // déroule contre le bas
//			if ( pos.Y-area.Bottom < bestHeight )
//			{
//				bottom = pos.Y > (area.Bottom+area.Top)/2;  // choix du sens de déroulement
//			}
//
//			double w = FontSelector.BestWidth(this.sampleHeight, this.sampleAbc);
//			double h = 0;
//
//			if ( pos.X+w > area.Right )  // dépasse à droite ?
//			{
//				pos.X = area.Right-w;
//			}
//
//			if ( bottom )  // déroule contre le bas ?
//			{
//				h = System.Math.Min(pos.Y-area.Bottom, 500);
//				h = FontSelector.BestHeight(h, this.fontList.Count, this.sampleHeight);
//				pos.Y -= h+shadow.Bottom;
//			}
//			else	// déroule contre le haut ?
//			{
//				h = System.Math.Min(area.Top-pos.Y, 500);
//				h = FontSelector.BestHeight(h, this.fontList.Count, this.sampleHeight);
//				pos.Y += this.Height;
//			}
//
//			this.fontSelector = new FontSelector(null);
//			this.fontSelector.Bounds = new Drawing.Rectangle(0, 0, w, h);
//			this.fontSelector.FontList = this.fontList;
//			this.fontSelector.QuickCount = this.quickCount;
//			this.fontSelector.SampleHeight = this.sampleHeight;
//			this.fontSelector.SampleAbc = this.sampleAbc;
//			this.fontSelector.SelectedFontFace = this.Text;
//			
//			this.menu = new TextFieldComboMenu ();
//			this.menu.Window.WindowBounds = new Drawing.Rectangle(pos.X, pos.Y, this.fontSelector.Width+shadow.Width, this.fontSelector.Height+shadow.Height);
//			
//			MenuItem.SetMenuHost (this, new MenuHost (this.menu));
//			
//			this.fontSelector.Location = new Drawing.Point(shadow.Left, shadow.Bottom);
//			this.fontSelector.SelectionChanged += new Support.EventHandler(this.HandleSelectorSelectionChanged);
////-			this.RegisterFilter();
//			this.menu.Window.Root.Children.Add(this.fontSelector);
//			this.menu.Window.WindowAnimationEnded += new Epsitec.Common.Support.EventHandler(this.HandleComboWindowWindowAnimationEnded);
//			this.menu.ShowAsComboList (this, this.MapClientToScreen (new Drawing.Point (0, 0)));
//			
//			this.openText = this.Text;
//			this.OnComboOpened();
//		}
		
//		protected override void CloseCombo(CloseMode mode)
//		{
//			this.fontSelector.SelectionChanged -= new Support.EventHandler(this.HandleSelectorSelectionChanged);
////-			this.UnregisterFilter();
//			this.fontSelector.Dispose();
//			this.fontSelector = null;
//			
//			if ( Window.IsApplicationActive )
//			{
//				this.Window.MakeActive();
//			}
//			
//			this.SelectAll();
//			
//			if ( this.AutoFocus )
//			{
//				this.Focus();
//			}
//			
//			if ( mode == CloseMode.Reject )
//			{
//				this.Text = this.openText;
//			}
//			
//			this.OnComboClosed();
//		}

		
		protected override void OnComboClosed()
		{
			base.OnComboClosed ();
			
			if ( this.Window != null )
			{
				this.Window.RestoreLogicalFocus();
			}
		}
		
		
//		private void MessageFilter(object sender, Message message)
//		{
//			Window window = sender as Window;
//			
//			System.Diagnostics.Debug.Assert(this.IsComboOpen);
//			System.Diagnostics.Debug.Assert(window != null);
//			
//			if ( this.fontSelector == null )  return;
//			
//			IFeel feel = Epsitec.Common.Widgets.Feel.Factory.Active;
//			
//			switch ( message.Type )
//			{
//				case MessageType.KeyPress:
//					if ( feel.TestCancelKey(message) )
//					{
//						this.CloseCombo(false);
//						message.Swallowed = true;
//					}
//					if ( feel.TestAcceptKey(message) )
//					{
//						this.CloseCombo(true);
//						message.Swallowed = true;
//					}
//					if ( feel.TestNavigationKey(message) )
//					{
//						this.CloseCombo(true);
//						//?Message.DefineLastWindow(this.Window);
//					}
//					break;
//				
//				case MessageType.MouseDown:
//					Drawing.Point mouse = window.Root.MapClientToScreen(message.Cursor);
//					Drawing.Point pos = this.fontSelector.MapScreenToClient(mouse);
//					if ( !this.fontSelector.HitTest(pos) )
//					{
//						this.CloseCombo(false);
//						message.Swallowed = ! message.NonClient;
//					}
//					break;
//			}
//		}
		
		
//		private void HandleApplicationDeactivated(object sender)
//		{
//			this.CloseCombo(false);
//		}
//
//		private void HandleButtonPressed(object sender, MessageEventArgs e)
//		{
//			this.OpenCombo();
//		}
		
		private void HandleSelectorSelectionChanged(object sender)
		{
			//	L'utilisateur a cliqué dans la liste pour terminer son choix.
			string text = this.fontSelector.SelectedFontFace;
			if ( this.Text != text )
			{
				this.Text = text;
				this.SelectAll();
			}

			this.CloseCombo(CloseMode.Accept);
		}
		
		private void HandleComboWindowWindowAnimationEnded(object sender)
		{
			this.fontSelector.Focus();
			System.Diagnostics.Debug.WriteLine("Focus set to window...");
		}
		
		
//		private void RegisterFilter()
//		{
//			Window.MessageFilter          += new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
//			Window.ApplicationDeactivated += new Support.EventHandler(this.HandleApplicationDeactivated);
//			
//			if ( this.Window != null && this.AutoFocus == false )
//			{
//				this.initiallyFocusedWidget = this.Window.FocusedWidget;
//			}
//		}
//		
//		private void UnregisterFilter()
//		{
//			Window.MessageFilter          -= new Epsitec.Common.Widgets.MessageHandler(this.MessageFilter);
//			Window.ApplicationDeactivated -= new Support.EventHandler(this.HandleApplicationDeactivated);
//			
//			if ( this.initiallyFocusedWidget != null )
//			{
//				if ( this.initiallyFocusedWidget.Window != null )
//				{
//					this.initiallyFocusedWidget.Focus();
//				}
//				
//				this.initiallyFocusedWidget = null;
//			}
//		}
		
		
		
		protected System.Collections.ArrayList	fontList;
		protected double						sampleHeight;
		protected bool							sampleAbc;
		protected int							quickCount;
		protected FontSelector					fontSelector;
		protected string						openText;
		protected ShowCondition					buttonShowCondition;
		protected double						defaultButtonWidth;
	}
}
