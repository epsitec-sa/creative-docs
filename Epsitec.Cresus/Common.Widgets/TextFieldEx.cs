using System;

namespace Epsitec.Common.Widgets
{
	using BundleAttribute  = Support.BundleAttribute;
	
	public enum DefocusAction
	{
		None,
		
		AcceptEdition,
		RejectEdition,
		
		Modal,
		AutoAcceptOrRejectEdition
	}
	
	public enum ShowCondition
	{
		Always,
		
		WhenFocused,
		WhenFocusedFlagSet,
		
		WhenModified,
		
		Never
	}
	
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
			
			this.DefocusAction       = DefocusAction.None;
			this.ButtonShowCondition = ShowCondition.WhenModified;
		}
		
		public TextFieldEx(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		
		[Bundle] public DefocusAction			DefocusAction
		{
			get
			{
				return this.defocus_action;
			}
			set
			{
				this.defocus_action = value;
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
		
		
		public bool AcceptEdition()
		{
			if (this.IsValid)
			{
				this.accept_reject_behavior.InitialText = this.Text;
				this.has_edited_text = false;
				this.SelectAll ();
				this.UpdateButtonVisibility ();
				this.OnEditionAccepted ();
				return true;
			}
			
			return false;
		}
		
		public bool RejectEdition()
		{
			this.Text = this.accept_reject_behavior.InitialText;
			this.has_edited_text = false;
			this.SelectAll ();
			this.UpdateButtonVisibility ();
			this.OnEditionRejected ();
			return true;
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

		protected override bool AboutToLoseFocus(Widget.TabNavigationDir dir, Widget.TabNavigationMode mode)
		{
			if (this.accept_reject_behavior.IsVisible)
			{
				switch (this.DefocusAction)
				{
					case DefocusAction.Modal:
						return this.IsValid;
				}
			}
			
			return base.AboutToLoseFocus (dir, mode);
		}
		
		protected override bool AboutToGetFocus(Widget.TabNavigationDir dir, Widget.TabNavigationMode mode, out Widget focus)
		{
			return base.AboutToGetFocus (dir, mode, out focus);
		}
		
		protected override void OnDefocused()
		{
			if (this.IsFocusedFlagSet == false)
			{
				switch (this.DefocusAction)
				{
					case DefocusAction.AcceptEdition:
						this.AcceptEdition ();
						break;
					
					case DefocusAction.RejectEdition:
						this.RejectEdition ();
						break;
					
					case DefocusAction.Modal:
					case DefocusAction.AutoAcceptOrRejectEdition:
						if (this.IsValid)
						{
							this.AcceptEdition ();
						}
						else
						{
							this.RejectEdition ();
						}
						break;
					
					case DefocusAction.None:
						break;
					
					default:
						throw new System.NotImplementedException (string.Format ("DefocusAction.{0} not implemented.", this.DefocusAction));
				}
			}
			
			base.OnDefocused ();
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

		
		protected virtual void SetButtonVisibility(bool show)
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
		
		protected virtual void UpdateButtonVisibility()
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
				
				case ShowCondition.WhenFocusedFlagSet:
					show = this.IsFocusedFlagSet;
					break;
				
				case ShowCondition.WhenModified:
					show = this.has_edited_text;
					break;
				
				default:
					throw new System.NotImplementedException (string.Format ("ButtonShowCondition.{0} not implemented.", this.ButtonShowCondition));
			}
			
			this.SetButtonVisibility (show);
		}
		
		protected virtual void UpdateButtonEnable()
		{
			if (this.accept_reject_behavior != null)
			{
				this.accept_reject_behavior.SetEnabledOk (this.IsValid);
			}
		}
		
		
		protected override void OnTextDefined()
		{
			base.OnTextDefined ();
			
			this.accept_reject_behavior.InitialText = this.Text;
			this.has_edited_text = false;
		}

		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			
			if (this.Text != this.accept_reject_behavior.InitialText)
			{
				this.has_edited_text = true;
			}
			
			this.UpdateButtonEnable ();
			this.UpdateButtonVisibility ();
		}

		protected override void OnFocusChanged()
		{
			base.OnFocusChanged ();
			this.UpdateButtonVisibility ();
		}
		
		protected virtual void  OnEditionAccepted()
		{
			if (this.EditionAccepted != null)
			{
				this.EditionAccepted (this);
			}
		}
		
		protected virtual void  OnEditionRejected()
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
		protected bool							has_edited_text;
		protected ShowCondition					button_show_condition;
		protected DefocusAction					defocus_action;
		protected Helpers.AcceptRejectBehavior	accept_reject_behavior;
	}
}
