using System;

namespace Epsitec.Common.Widgets
{
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
			
			this.button_ok.Name        = "OK";
			this.button_ok.GlyphShape  = GlyphShape.Validate;
			this.button_ok.ButtonStyle = ButtonStyle.Combo;
			this.button_ok.Clicked    += new MessageEventHandler(this.HandleButtonOkClicked);
			
			this.button_cancel.Name        = "Cancel";
			this.button_cancel.GlyphShape  = GlyphShape.Cancel;
			this.button_cancel.ButtonStyle = ButtonStyle.Combo;
			this.button_cancel.Clicked    += new MessageEventHandler(this.HandleButtonCancelClicked);
			
			this.IsReadOnly = true;
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
				this.IsReadOnly = false;
				this.SelectedItem = "";
				this.SetFocused (true);
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

		
		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			
			if (this.button_ok != null)
			{
				this.button_ok.SetEnabled (this.IsValid);
			}
		}

		
		protected override void OnReadOnlyChanged()
		{
			base.OnReadOnlyChanged ();
			
			if ((this.button == null) ||
				(this.button_ok == null) ||
				(this.button_cancel == null))
			{
				return;
			}
			
			if (this.IsReadOnly)
			{
				this.margins.Right = this.button.DefaultWidth;
				this.button.SetVisible (true);
				this.button_ok.SetVisible (false);
				this.button_cancel.SetVisible (false);
			}
			else
			{
				this.margins.Right = this.button_ok.DefaultWidth + this.button_cancel.DefaultWidth;
				this.button.SetVisible (false);
				this.button_ok.SetVisible (true);
				this.button_cancel.SetVisible (true);
			}
			
			this.UpdateButtonGeometry ();
		}

		
		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.button_ok);
		}		
		
		private void HandleButtonCancelClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.button_cancel);
		}		
		
		
		
		protected string						place_holder;
		
		protected GlyphButton					button_ok;
		protected GlyphButton					button_cancel;
	}
}
