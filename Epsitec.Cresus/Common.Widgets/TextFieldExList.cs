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
			this.button_ok     = new GlyphButton(this);
			this.button_cancel = new GlyphButton(this);
			
			IFeel feel = Feel.Factory.Active;
			
			this.button_ok.Name        = "OK";
			this.button_ok.GlyphShape  = GlyphShape.Validate;
			this.button_ok.ButtonStyle = ButtonStyle.ExListMiddle;
			this.button_ok.Clicked    += new MessageEventHandler(this.HandleButtonOkClicked);
			this.button_ok.Shortcut    = feel.AcceptShortcut;
			
			this.button_cancel.Name        = "Cancel";
			this.button_cancel.GlyphShape  = GlyphShape.Cancel;
			this.button_cancel.ButtonStyle = ButtonStyle.ExListRight;
			this.button_cancel.Clicked    += new MessageEventHandler(this.HandleButtonCancelClicked);
			this.button_cancel.Shortcut    = feel.CancelShortcut;
			
			this.IsReadOnly = true;
			this.SwitchToState (TextFieldExListMode.Combo);
		}
		
		public TextFieldExList(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
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
//-			this.CancelEdition ();
			this.SelectedItem = text;
			this.SwitchToState (TextFieldExListMode.EditPassive);
			this.OnEditionStarted ();
		}
		
		public void StartActiveEdition(string text)
		{
//-			this.CancelEdition ();
			this.SelectedItem = text;
			this.SwitchToState (TextFieldExListMode.EditActive);
			this.OnEditionStarted ();
		}
		
		public bool CancelEdition()
		{
			if ((this.mode == TextFieldExListMode.EditActive) ||
				(this.mode == TextFieldExListMode.EditPassive))
			{
				this.SwitchToState (TextFieldExListMode.Combo);
				this.SelectedItem = this.saved_item;
				this.OnEditionCancelled ();
				return true;
			}
			
			return false;
		}
		
		public bool ValidateEdition()
		{
			if (((this.mode == TextFieldExListMode.EditActive) || (this.mode == TextFieldExListMode.EditPassive)) &&
				(this.IsValid))
			{
				this.SwitchToState (TextFieldExListMode.Combo);
				this.SelectedItem = this.Text;
				this.OnEditionValidated ();
				return true;
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
			
			if ((this.button_ok != null) &&
				(this.button_cancel != null))
			{
				Drawing.Rectangle bounds = this.GetButtonBounds ();
				Drawing.Rectangle rect_1 = bounds;
				Drawing.Rectangle rect_2 = bounds;
				
				rect_1.Right = rect_1.Left + rect_1.Width / 2;
				rect_2.Left  = rect_1.Right;
				
				this.button_ok.Bounds     = rect_1;
				this.button_cancel.Bounds = rect_2;
			}
		}

		
		protected virtual void SwitchToState(TextFieldExListMode mode)
		{
			if (this.mode != mode)
			{
				this.mode = mode;
				
				if ((this.button == null) ||
					(this.button_ok == null) ||
					(this.button_cancel == null))
				{
					return;
				}
				
				if (this.mode == TextFieldExListMode.EditActive)
				{
					//	Montre les boutons de validation et d'annulation :
					
					this.margins.Right = this.button_ok.DefaultWidth + this.button_cancel.DefaultWidth;
					this.button.SetVisible (false);
					this.button_ok.SetVisible (true);
					this.button_cancel.SetVisible (true);
				}
				else
				{
					this.margins.Right = this.button.DefaultWidth;
					this.button.SetVisible (true);
					this.button_ok.SetVisible (false);
					this.button_cancel.SetVisible (false);
				}
				
				this.UpdateButtonGeometry ();
				this.UpdateButtonEnable ();
				this.UpdateTextLayout ();
			}
			
			this.IsReadOnly = (this.mode == TextFieldExListMode.Combo);
		}
		
		protected virtual void UpdateButtonEnable()
		{
			if ((this.button_ok != null) &&
				(this.mode == TextFieldExListMode.EditActive))
			{
				bool enable_ok = this.IsValid;
				
				this.button_ok.SetEnabled (enable_ok);
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

		
		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.button_ok);
			this.ValidateEdition ();
		}		
		
		private void HandleButtonCancelClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.button_cancel);
			
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
			
			this.OnEditionCancelled ();
		}		
		
		
		public event Support.EventHandler		EditionStarted;
		public event Support.EventHandler		EditionValidated;
		public event Support.EventHandler		EditionCancelled;
		
		protected string						place_holder;
		protected string						saved_item;
		
		protected TextFieldExListMode			mode = TextFieldExListMode.Undefined;
		
		protected GlyphButton					button_ok;
		protected GlyphButton					button_cancel;
	}
}
