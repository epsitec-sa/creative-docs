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
			this.accept_cancel_behavior = new Helpers.AcceptCancelBehavior (this);
			
			this.accept_cancel_behavior.CancelClicked += new Support.EventHandler(this.HandleAcceptCancelCancelClicked);
			this.accept_cancel_behavior.AcceptClicked += new Support.EventHandler(this.HandleAcceptCancelAcceptClicked);
			
			this.ShowAcceptCancel (true);
		}
		
		public TextFieldEx(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.accept_cancel_behavior.CancelClicked -= new Support.EventHandler(this.HandleAcceptCancelCancelClicked);
				this.accept_cancel_behavior.AcceptClicked -= new Support.EventHandler(this.HandleAcceptCancelAcceptClicked);
				
				this.accept_cancel_behavior = null;
			}
			
			base.Dispose (disposing);
		}

		
		
		public bool CancelEdition()
		{
			this.OnEditionCancelled ();
			return false;
		}
		
		public bool ValidateEdition()
		{
			this.OnEditionValidated ();
			return true;
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
			
			if (this.accept_cancel_behavior != null)
			{
				this.accept_cancel_behavior.UpdateButtonGeometry ();
			}
		}

		
		protected virtual void ShowAcceptCancel(bool show)
		{
			if (this.show_buttons != show)
			{
				this.show_buttons = show;
				
				if (this.accept_cancel_behavior == null)
				{
					return;
				}
				
				if (show)
				{
					this.margins.Right = this.accept_cancel_behavior.DefaultWidth;
					this.accept_cancel_behavior.SetVisible (true);
				}
				else
				{
					this.margins.Right = 0;
					this.accept_cancel_behavior.SetVisible (false);
				}
				
				this.UpdateButtonGeometry ();
				this.UpdateButtonEnable ();
				this.UpdateTextLayout ();
			}
		}
		
		protected virtual void UpdateButtonEnable()
		{
			if (this.accept_cancel_behavior != null)
			{
				this.accept_cancel_behavior.SetEnabledOk (this.IsValid);
			}
		}
		
		
		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			this.UpdateButtonEnable ();
		}

		
		protected virtual void OnEditionValidated()
		{
			if (this.EditionValidated != null)
			{
				this.EditionValidated (this);
			}
		}
		
		protected virtual void OnEditionCancelled()
		{
			if (this.EditionCancelled != null)
			{
				this.EditionCancelled (this);
			}
		}
		
		
		private void HandleAcceptCancelAcceptClicked(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.accept_cancel_behavior);
			this.ValidateEdition ();
		}		
		
		private void HandleAcceptCancelCancelClicked(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.accept_cancel_behavior);
			this.OnEditionCancelled ();
		}		
		
		
		public event Support.EventHandler		EditionValidated;
		public event Support.EventHandler		EditionCancelled;
		
		protected bool							show_buttons;
		protected Helpers.AcceptCancelBehavior	accept_cancel_behavior;
	}
}
