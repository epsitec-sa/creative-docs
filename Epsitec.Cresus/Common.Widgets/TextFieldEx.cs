using System;

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
			this.accept_reject_behavior = new Helpers.AcceptRejectBehavior (this);
			
			this.accept_reject_behavior.RejectClicked += new Support.EventHandler(this.HandleAcceptRejectRejectClicked);
			this.accept_reject_behavior.AcceptClicked += new Support.EventHandler(this.HandleAcceptRejectAcceptClicked);
			
			this.ShowAcceptReject (true);
		}
		
		public TextFieldEx(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
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

		
		
		public bool AcceptEdition()
		{
			this.OnEditionAccepted ();
			return true;
		}
		
		public bool RejectEdition()
		{
			this.OnEditionRejected ();
			return false;
		}
		
		
		protected override bool ProcessMouseDown(Message message, Epsitec.Common.Drawing.Point pos)
		{
			return base.ProcessMouseDown (message, pos);
		}
		
		protected override bool ProcessKeyDown(Message message, Epsitec.Common.Drawing.Point pos)
		{
			return base.ProcessKeyDown (message, pos);
		}

		protected override bool AboutToGetFocus(Widget.TabNavigationDir dir, Widget.TabNavigationMode mode, out Widget focus)
		{
			return base.AboutToGetFocus (dir, mode, out focus);
		}

		protected override void UpdateButtonGeometry()
		{
			base.UpdateButtonGeometry ();
			
			if (this.accept_reject_behavior != null)
			{
				this.margins.Right = this.accept_reject_behavior.DefaultWidth;
				this.accept_reject_behavior.UpdateButtonGeometry ();
			}
		}

		
		public void ShowAcceptReject(bool show)
		{
			if (this.show_buttons != show)
			{
				this.show_buttons = show;
				this.SetAcceptRejectVisible (show);
			}
		}
		
		protected virtual void SetAcceptRejectVisible(bool show)
		{
			if (this.accept_reject_behavior == null)
			{
				return;
			}
			
			if (this.accept_reject_behavior.IsVisible != show)
			{
				this.accept_reject_behavior.SetVisible (show);
				
				this.UpdateButtonGeometry ();
				this.UpdateButtonEnable ();
				this.UpdateTextLayout ();
				this.UpdateMouseCursor (this.MapRootToClient (Message.State.LastPosition));
			}
		}
		
		protected virtual void UpdateButtonEnable()
		{
			if (this.accept_reject_behavior != null)
			{
				this.accept_reject_behavior.SetEnabledOk (this.IsValid);
			}
		}
		
		
		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			this.UpdateButtonEnable ();
		}

		protected override void OnFocusChanged()
		{
			base.OnFocusChanged ();
			
			this.SetAcceptRejectVisible (this.IsFocusedFlagSet && this.show_buttons);
		}
		
		protected virtual void OnEditionAccepted()
		{
			if (this.EditionAccepted != null)
			{
				this.EditionAccepted (this);
			}
		}
		
		protected virtual void OnEditionRejected()
		{
			if (this.EditionRejected != null)
			{
				this.EditionRejected (this);
			}
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
		
		
		public event Support.EventHandler		EditionAccepted;
		public event Support.EventHandler		EditionRejected;
		
		protected bool							show_buttons;
		protected Helpers.AcceptRejectBehavior	accept_reject_behavior;
	}
}
