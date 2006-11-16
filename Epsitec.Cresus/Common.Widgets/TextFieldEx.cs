//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextFieldEx implémente une variante de TextField, avec une fonction
	/// pour accepter/annuler une édition.
	/// </summary>
	public class TextFieldEx : TextField
	{
		public TextFieldEx()
		{
			this.accept_reject_behavior = new Behaviors.AcceptRejectBehavior (this);
			this.accept_reject_behavior.CreateButtons ();
			
			this.accept_reject_behavior.RejectClicked += new Support.EventHandler(this.HandleAcceptRejectRejectClicked);
			this.accept_reject_behavior.AcceptClicked += new Support.EventHandler(this.HandleAcceptRejectAcceptClicked);
			
			this.DefocusAction       = DefocusAction.None;
			this.ButtonShowCondition = ShowCondition.WhenModified;
		}
		
		public TextFieldEx(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		
		
		public override bool AcceptEdition()
		{
			if ((this.IsValid) &&
				(this.IsEditing))
			{
				this.accept_reject_behavior.InitialText = this.Text;
				this.OnTextDefined ();
				this.UpdateButtonVisibility ();

				return base.AcceptEdition ();
			}
			
			return false;
		}
		
		public override bool RejectEdition()
		{
			if (this.IsEditing)
			{
				this.UpdateButtonVisibility ();
			}

			return base.RejectEdition ();
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.accept_reject_behavior.RejectClicked -= new Support.EventHandler(this.HandleAcceptRejectRejectClicked);
				this.accept_reject_behavior.AcceptClicked -= new Support.EventHandler(this.HandleAcceptRejectAcceptClicked);
				
				this.accept_reject_behavior = null;
			}
			
			base.Dispose (disposing);
		}

		
		protected override bool ProcessMouseDown(Message message, Drawing.Point pos)
		{
			return base.ProcessMouseDown (message, pos);
		}
		
		protected override bool ProcessKeyDown(Message message, Drawing.Point pos)
		{
			return base.ProcessKeyDown (message, pos);
		}

		protected override bool AboutToLoseFocus(TabNavigationDir dir, TabNavigationMode mode)
		{
//			if (this.accept_reject_behavior.IsVisible)
			{
				switch (this.DefocusAction)
				{
					case DefocusAction.Modal:
						return this.IsValid;
				}
			}
			
			return base.AboutToLoseFocus (dir, mode);
		}
		
		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			return base.AboutToGetFocus (dir, mode, out focus);
		}

		
		protected override void UpdateButtonGeometry()
		{
			if (this.accept_reject_behavior != null)
			{
				this.margins.Right = this.accept_reject_behavior.DefaultWidth;
				this.accept_reject_behavior.UpdateButtonGeometry ();
			}
			
			base.UpdateButtonGeometry ();
		}

		protected override void UpdateButtonVisibility()
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
					show = this.IsFocused;
					break;
				
				case ShowCondition.WhenKeyboardFocused:
					show = this.KeyboardFocus;
					break;
				
				case ShowCondition.WhenModified:
					show = this.HasEditedText;
					break;
				
				default:
					throw new System.NotImplementedException (string.Format ("ButtonShowCondition.{0} not implemented.", this.ButtonShowCondition));
			}
			
			this.SetButtonVisibility (show);
		}
		
		
		protected void SetButtonVisibility(bool show)
		{
			if (this.accept_reject_behavior == null)
			{
				return;
			}
			
			if (this.accept_reject_behavior.IsVisible != show)
			{
				this.accept_reject_behavior.SetVisible (show);

				Window window = this.Window;

				if (window != null)
				{
					window.ForceLayout ();
				}

				this.UpdateButtonGeometry ();
				this.UpdateButtonEnable ();
				this.UpdateTextLayout ();
				this.UpdateMouseCursor (this.MapRootToClient (Message.CurrentState.LastPosition));
			}
		}
		
		protected void UpdateButtonEnable()
		{
			if (this.accept_reject_behavior != null)
			{
				this.accept_reject_behavior.SetAcceptEnabled (this.IsValid);
			}
		}
		
		
		protected override void OnTextDefined()
		{
			base.OnTextDefined ();
			
			this.accept_reject_behavior.InitialText = this.Text;
		}

		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			
			//? System.Diagnostics.Debug.Assert (this.HasEditedText || this.Text == this.accept_reject_behavior.InitialText);
			
			this.UpdateButtonEnable ();
			this.UpdateButtonVisibility ();
		}

		protected override void OnKeyboardFocusChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			base.OnKeyboardFocusChanged (e);
			
			this.UpdateButtonVisibility ();
		}
		
		
		private void HandleAcceptRejectAcceptClicked(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.accept_reject_behavior);
			this.AcceptEdition ();
		}		
		
		private void HandleAcceptRejectRejectClicked(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.accept_reject_behavior);
			this.RejectEdition ();
		}		
		
		
		private Behaviors.AcceptRejectBehavior	accept_reject_behavior;
	}
}
