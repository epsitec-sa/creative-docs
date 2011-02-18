//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			this.acceptRejectBehavior = new Behaviors.AcceptRejectBehavior (this);
			this.acceptRejectBehavior.CreateButtons ();
			
			this.acceptRejectBehavior.RejectClicked += this.HandleAcceptRejectRejectClicked;
			this.acceptRejectBehavior.AcceptClicked += this.HandleAcceptRejectAcceptClicked;
			
			this.DefocusAction       = DefocusAction.None;
			this.ButtonShowCondition = ButtonShowCondition.WhenModified;
		}
		
		public TextFieldEx(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}


		protected override bool CanStartEdition
		{
			get
			{
				return true;
			}
		}
		
		
		public override bool AcceptEdition()
		{
			if ((this.IsValid) &&
				(this.IsEditing) &&
				(this.CheckAcceptEdition ()))
			{
				this.acceptRejectBehavior.InitialText = this.Text;
				this.OnTextDefined ();
				this.UpdateButtonVisibility ();

				return base.AcceptEdition ();
			}
			
			return false;
		}
		
		public override bool RejectEdition()
		{
			if ((this.IsEditing) &&
				(this.CheckRejectEdition ()))
			{
				bool ok = base.RejectEdition ();

				if (ok)
				{
					this.UpdateButtonVisibility ();
				}

				return ok;
			}
			else
			{
				return base.RejectEdition ();
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.acceptRejectBehavior.RejectClicked -= this.HandleAcceptRejectRejectClicked;
				this.acceptRejectBehavior.AcceptClicked -= this.HandleAcceptRejectAcceptClicked;
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
						if (this.IsValid)
						{
							return this.CheckAcceptEdition ();
						}

						return false;
				}
			}

			if (this.CheckBeforeDefocus ())
			{
				return base.AboutToLoseFocus (dir, mode);
			}
			else
			{
				return false;
			}
		}
		
		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			return base.AboutToGetFocus (dir, mode, out focus);
		}

		
		protected override void UpdateButtonGeometry()
		{
			if (this.acceptRejectBehavior != null)
			{
				this.margins.Right = this.acceptRejectBehavior.DefaultWidth;
				this.acceptRejectBehavior.UpdateButtonGeometry ();
			}
			
			base.UpdateButtonGeometry ();
		}

		protected override void UpdateButtonVisibility()
		{
			bool show = false;
			
			switch (this.ButtonShowCondition)
			{
				case ButtonShowCondition.Always:
					show = true;
					break;
				
				case ButtonShowCondition.Never:
					break;
				
				case ButtonShowCondition.WhenFocused:
					show = this.IsFocused;
					break;
				
				case ButtonShowCondition.WhenKeyboardFocused:
					show = this.KeyboardFocus;
					break;
				
				case ButtonShowCondition.WhenModified:
					show = this.HasEditedText;
					break;
				
				default:
					throw new System.NotImplementedException (string.Format ("ButtonShowCondition.{0} not implemented.", this.ButtonShowCondition));
			}
			
			this.SetButtonVisibility (show);
		}
		
		
		protected void SetButtonVisibility(bool show)
		{
			if (this.acceptRejectBehavior == null)
			{
				return;
			}
			
			if (this.acceptRejectBehavior.IsVisible != show)
			{
				this.acceptRejectBehavior.SetVisible (show);

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
			if (this.acceptRejectBehavior != null)
			{
				this.acceptRejectBehavior.SetAcceptEnabled (this.IsValid);
			}
		}
		
		
		protected override void OnTextDefined()
		{
			base.OnTextDefined ();
			
			this.acceptRejectBehavior.InitialText = this.Text;
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
			System.Diagnostics.Debug.Assert (sender == this.acceptRejectBehavior);
			this.AcceptEdition ();
		}		
		
		private void HandleAcceptRejectRejectClicked(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.acceptRejectBehavior);
			this.RejectEdition ();
		}		
		
		
		private readonly Behaviors.AcceptRejectBehavior	acceptRejectBehavior;
	}
}
