using System;

namespace Epsitec.Common.Widgets
{
	public enum TextFieldExListMode
	{
		Undefined		= -1,
		
		Combo,
		EditPassive,
		EditActive
	}
	
	/// <summary>
	/// La classe TextFieldExList implémente une variante de TextFieldCombo, permettant
	/// d'éditer les éléments contenus dans la liste.
	/// </summary>
	public class TextFieldExList : TextFieldCombo
	{
		public TextFieldExList()
		{
			this.accept_reject_behavior = new Helpers.AcceptRejectBehavior (this);
			
			this.accept_reject_behavior.RejectClicked += new Support.EventHandler(this.HandleAcceptRejectRejectClicked);
			this.accept_reject_behavior.AcceptClicked += new Support.EventHandler(this.HandleAcceptRejectAcceptClicked);
			
			this.IsReadOnly = true;
			this.SwitchToState (TextFieldExListMode.Combo);
		}
		
		public TextFieldExList(Widget embedder) : this ()
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

		
		
		public string							PlaceHolder
		{
			get
			{
				return this.place_holder;
			}
			set
			{
				this.place_holder = value;
			}
		}
		
		public bool								HasPlaceHolder
		{
			get
			{
				return this.place_holder != null;
			}
		}
		
		public TextFieldExListMode				Mode
		{
			get
			{
				return this.mode;
			}
		}
		
		
		public void StartPassiveEdition(string text)
		{
//-			this.RejectEdition ();
			this.SelectedItem = text;
			this.SwitchToState (TextFieldExListMode.EditPassive);
			this.OnEditionStarted ();
		}
		
		public void StartActiveEdition(string text)
		{
//-			this.RejectEdition ();
			this.SelectedItem = text;
			this.SwitchToState (TextFieldExListMode.EditActive);
			this.OnEditionStarted ();
		}
		
		public bool RejectEdition()
		{
			if ((this.mode == TextFieldExListMode.EditActive) ||
				(this.mode == TextFieldExListMode.EditPassive))
			{
				this.SwitchToState (TextFieldExListMode.Combo);
				this.SelectedItem = this.saved_item;
				this.OnEditionRejected ();
				return true;
			}
			
			return false;
		}
		
		public bool AcceptEdition()
		{
			if ((this.mode == TextFieldExListMode.EditActive) ||
				(this.mode == TextFieldExListMode.EditPassive))
			{
				if (this.IsValid)
				{
					this.SwitchToState (TextFieldExListMode.Combo);
					this.SelectedItem = this.Text;
					this.OnEditionAccepted ();
					return true;
				}
			}
			
			return false;
		}
		
		private void SwitchToActiveEdition()
		{
			switch (this.mode)
			{
				case TextFieldExListMode.EditActive:
					if (this.Text == this.PlaceHolder)
					{
						this.Text = "";
					}
					break;
				
				case TextFieldExListMode.EditPassive:
					this.Text = "";
					this.SwitchToState (TextFieldExListMode.EditActive);
					break;
			}
		}
		
		protected override bool ProcessMouseDown(Message message, Epsitec.Common.Drawing.Point pos)
		{
			this.SwitchToActiveEdition ();
			return base.ProcessMouseDown (message, pos);
		}
		
		protected override bool ProcessKeyDown(Message message, Epsitec.Common.Drawing.Point pos)
		{
			this.SwitchToActiveEdition ();
			return base.ProcessKeyDown (message, pos);
		}

		protected override bool AboutToGetFocus(Widget.TabNavigationDir dir, Widget.TabNavigationMode mode, out Widget focus)
		{
			if ((this.mode == TextFieldExListMode.EditPassive) &&
				(mode != Widget.TabNavigationMode.Passive))
			{
				//	Si on entre par un TAB dans ce widget, il faut passer en mode édition active,
				//	si l'état précédent était passif :
				
				this.SwitchToActiveEdition ();
			}
			
			return base.AboutToGetFocus (dir, mode, out focus);
		}

		protected override void FillComboList(Epsitec.Common.Widgets.Helpers.StringCollection list)
		{
			if (this.HasPlaceHolder)
			{
				list.Add ("$PlaceHolder$", this.PlaceHolder);
			}
			
			base.FillComboList (list);
		}
		
		protected override int MapComboListToIndex(int value)
		{
			if (this.HasPlaceHolder)
			{
				value--;
			}
			
			return base.MapComboListToIndex (value);
		}
		
		protected override int MapIndexToComboList(int value)
		{
			if ((this.HasPlaceHolder) &&
				(value >= 0))
			{
				value++;
			}
			
			return base.MapIndexToComboList (value);
		}
		
		protected override void ComboActivatedIndex(int sel)
		{
			if ((this.HasPlaceHolder) &&
				(sel == 0))
			{
				this.StartPassiveEdition (this.PlaceHolder);
				this.Focus ();
				this.CloseCombo ();
			}
			else
			{
				base.ComboActivatedIndex (sel);
			}
		}

		protected override void UpdateButtonGeometry()
		{
			base.UpdateButtonGeometry ();
			
			if (this.accept_reject_behavior != null)
			{
				this.accept_reject_behavior.UpdateButtonGeometry ();
			}
		}

		
		protected virtual void SwitchToState(TextFieldExListMode mode)
		{
			if (this.mode != mode)
			{
				this.mode = mode;
				
				if ((this.button == null) ||
					(this.accept_reject_behavior == null))
				{
					return;
				}
				
				if (this.mode == TextFieldExListMode.EditActive)
				{
					//	Montre les boutons de validation et d'annulation :
					
					this.margins.Right = this.accept_reject_behavior.DefaultWidth;
					this.button.SetVisible (false);
					this.accept_reject_behavior.SetVisible (true);
				}
				else
				{
					this.margins.Right = this.button.DefaultWidth;
					this.button.SetVisible (true);
					this.accept_reject_behavior.SetVisible (false);
				}
				
				this.UpdateButtonGeometry ();
				this.UpdateButtonEnable ();
				this.UpdateTextLayout ();
			}
			
			this.IsReadOnly = (this.mode == TextFieldExListMode.Combo);
		}
		
		protected virtual void UpdateButtonEnable()
		{
			if ((this.accept_reject_behavior != null) &&
				(this.mode == TextFieldExListMode.EditActive))
			{
				this.accept_reject_behavior.SetEnabledOk (this.IsValid);
			}
		}
		
		
		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			this.UpdateButtonEnable ();
		}

		
		protected virtual void OnEditionStarted()
		{
			if (this.EditionStarted != null)
			{
				this.EditionStarted (this);
			}
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
		
		
		protected override void OpenCombo()
		{
			if (this.mode != TextFieldExListMode.Combo)
			{
				this.SwitchToState (TextFieldExListMode.Combo);
			}
			
			if (this.SelectedItem != "")
			{
				//	Prend note de l'élément actuellement actif, afin de pouvoir le restaurer
				//	en cas d'annulation par la suite :
				
				this.saved_item = this.SelectedItem;
			}
			
			base.OpenCombo ();
		}
		
		protected override void CloseCombo()
		{
			base.CloseCombo ();
			
			if ((this.HasPlaceHolder) &&
				(this.Text == this.PlaceHolder))
			{
				this.SwitchToState (TextFieldExListMode.EditPassive);
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
			
			if (this.Items.FindExactMatch (this.saved_item) == -1)
			{
				this.Text = this.PlaceHolder;
				this.SwitchToState (TextFieldExListMode.EditPassive);
				this.Focus ();
				this.SelectAll ();
			}
			else
			{
				this.SelectedItem = this.saved_item;
				this.SwitchToState (TextFieldExListMode.Combo);
			}
			
			this.OnEditionRejected ();
		}		
		
		
		public event Support.EventHandler		EditionStarted;
		public event Support.EventHandler		EditionAccepted;
		public event Support.EventHandler		EditionRejected;
		
		protected string						place_holder;
		protected string						saved_item;
		
		protected TextFieldExListMode			mode = TextFieldExListMode.Undefined;
		protected Helpers.AcceptRejectBehavior	accept_reject_behavior;
	}
}
