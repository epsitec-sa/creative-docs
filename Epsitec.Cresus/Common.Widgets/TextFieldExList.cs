using System;

namespace Epsitec.Common.Widgets
{
	public enum TextFieldExListMode
	{
		Undefined,
		
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
			this.acceptRejectBehavior = new Behaviors.AcceptRejectBehavior (this);
			this.acceptRejectBehavior.CreateButtons ();
			
			this.acceptRejectBehavior.RejectClicked += this.HandleRejectClicked;
			this.acceptRejectBehavior.AcceptClicked += this.HandleAcceptClicked;
			
			this.DefocusAction = DefocusAction.None;
			this.IsReadOnly    = true;
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
				if (this.acceptRejectBehavior != null)
				{
					this.acceptRejectBehavior.RejectClicked -= this.HandleRejectClicked;
					this.acceptRejectBehavior.AcceptClicked -= this.HandleAcceptClicked;
				}
				
				this.acceptRejectBehavior = null;
			}
			
			base.Dispose (disposing);
		}

		
		
		public string							PlaceHolder
		{
			get
			{
				return this.placeHolder;
			}
			set
			{
				this.placeHolder = value;
			}
		}
		
		public bool								HasPlaceHolder
		{
			get
			{
				return this.placeHolder != null;
			}
		}
		
		public TextFieldExListMode				Mode
		{
			get
			{
				return this.mode;
			}
		}

		protected override bool CanStartEdition
		{
			get
			{
				return true;
			}
		}
		
		
		public void StartPassiveEdition(string text)
		{
			this.SelectedItem = text;
			this.SwitchToState (TextFieldExListMode.EditPassive);
			this.OnEditionStarted ();
		}
		
		public void StartActiveEdition(string text)
		{
			this.SelectedItem = text;
			this.SwitchToState (TextFieldExListMode.EditActive);
			this.OnEditionStarted ();
		}
		
		public override bool RejectEdition()
		{
			if ((this.mode == TextFieldExListMode.EditActive) ||
				(this.mode == TextFieldExListMode.EditPassive))
			{
				this.SwitchToState (TextFieldExListMode.Combo);
				this.SelectedItem = this.acceptRejectBehavior.InitialText;
				this.OnEditionRejected ();
				return true;
			}
			else
			{
				return base.RejectEdition ();
			}
		}
		
		public override bool AcceptEdition()
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
				else
				{
					return false;
				}
			}
			else
			{
				return base.AcceptEdition ();
			}
		}
		
		public bool AutoRejectEdition(bool changeFocus)
		{
			if ((this.mode == TextFieldExListMode.EditActive) ||
				(this.mode == TextFieldExListMode.EditPassive))
			{
				if (this.Items.FindIndexByValueExactMatch (this.acceptRejectBehavior.InitialText) == -1)
				{
					this.Text = this.PlaceHolder;
					this.SwitchToState (TextFieldExListMode.EditPassive);
					
					if (changeFocus)
					{
						this.Focus ();
					}
					
					this.SelectAll ();
				}
				else
				{
					this.SelectedItem = this.acceptRejectBehavior.InitialText;
					this.SwitchToState (TextFieldExListMode.Combo);
				}
				
				this.OnEditionRejected ();
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
			if ((this.mode == TextFieldExListMode.EditPassive) &&
				(Feel.Factory.Active.TestComboOpenKey (message)))
			{
				//	L'utilisateur aimerait plutôt ouvrir la list déroulante que de commencer
				//	l'édition du champ en cours :
				
				return base.ProcessKeyDown (message, pos);
			}
			
			this.SwitchToActiveEdition ();
			
			return base.ProcessKeyDown (message, pos);
		}

		protected override void ProcessComboActivatedIndex(int sel)
		{
			if ((this.HasPlaceHolder) &&
				(sel == 0))
			{
				this.CloseCombo (CloseMode.Accept);
			}
			else
			{
				base.ProcessComboActivatedIndex (sel);
			}
		}

		protected override bool AboutToLoseFocus(TabNavigationDir dir, TabNavigationMode mode)
		{
			if (this.Mode == TextFieldExListMode.EditActive)
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
			if ((this.mode == TextFieldExListMode.EditPassive) &&
				(mode != TabNavigationMode.None))
			{
				//	Si on entre par un TAB dans ce widget, il faut passer en mode édition active,
				//	si l'état précédent était passif :
				
				this.SwitchToActiveEdition ();
			}
			
			return base.AboutToGetFocus (dir, mode, out focus);
		}

		public override void DefocusAndAcceptOrReject()
		{
			if (this.Mode == TextFieldExListMode.EditActive)
			{
				base.DefocusAndAcceptOrReject ();
			}
		}
		
		protected override void CopyItemsToComboList(Epsitec.Common.Widgets.Collections.StringCollection list)
		{
			if (this.HasPlaceHolder)
			{
				list.Add (TextFieldExList.PlaceHolderTag, this.PlaceHolder);
			}
			
			base.CopyItemsToComboList (list);
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
		
		protected override void UpdateButtonGeometry()
		{
			if (this.acceptRejectBehavior != null)
			{
				this.margins.Right = this.mode == TextFieldExListMode.EditActive ? this.acceptRejectBehavior.DefaultWidth : this.button.PreferredWidth;
				this.acceptRejectBehavior.UpdateButtonGeometry ();
			}
			
			base.UpdateButtonGeometry ();
		}

		protected virtual  void UpdateButtonEnable()
		{
			if ((this.acceptRejectBehavior != null) &&
				(this.mode == TextFieldExListMode.EditActive))
			{
				this.acceptRejectBehavior.SetAcceptEnabled (this.IsValid);
			}
		}
		
		protected override  void UpdateButtonVisibility()
		{
			if ((this.button != null) &&
				(this.acceptRejectBehavior != null))
			{
				if (this.mode == TextFieldExListMode.EditActive)
				{
					this.button.Visibility = false;
					this.acceptRejectBehavior.SetVisible (this.ComputeButtonVisibility ());
				}
				else
				{
					this.button.Visibility = (this.ComputeButtonVisibility ());
					this.acceptRejectBehavior.SetVisible (false);
				}
			}
		}
		
		
		protected virtual  void SwitchToState(TextFieldExListMode mode)
		{
			if (this.mode != mode)
			{
				this.mode = mode;
				
				if ((this.button == null) ||
					(this.acceptRejectBehavior == null))
				{
					return;
				}
				
				this.UpdateButtonVisibility ();
				this.UpdateButtonGeometry ();
				this.UpdateButtonEnable ();
				this.UpdateTextLayout ();
			}
			
			this.IsReadOnly = (this.mode == TextFieldExListMode.Combo);
		}
		
		
		protected override void OnTextDefined()
		{
			base.OnTextDefined ();
			this.acceptRejectBehavior.InitialText = this.Text;
		}
		
		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			this.UpdateButtonEnable ();
			this.UpdateButtonVisibility ();
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
				
				this.acceptRejectBehavior.InitialText = this.SelectedItem;
			}
			
			base.OpenCombo ();
		}
		
		protected override void CloseCombo(CloseMode mode)
		{
			if ((this.HasPlaceHolder) &&
				(mode == CloseMode.Accept) &&
				(this.ScrollList.SelectedItemIndex == 0))
			{
				this.StartPassiveEdition (this.PlaceHolder);
				this.Focus ();
			}
			
			base.CloseCombo (mode);
			
			if ((this.HasPlaceHolder) &&
				(this.Text == this.PlaceHolder))
			{
				this.SwitchToState (TextFieldExListMode.EditPassive);
			}
		}

		protected override bool CheckIfOpenComboRequested(Message message)
		{
			switch (this.mode)
			{
				case TextFieldExListMode.Combo:
				case TextFieldExListMode.EditPassive:
					return base.CheckIfOpenComboRequested (message);
				
				default:
					return false;
			}
		}

		protected override void Navigate(int dir)
		{
			if (this.mode != TextFieldExListMode.EditActive)
			{
				base.Navigate (dir);
			}
		}

		protected override bool ProcessKeyPressInSelectItemBehavior(Message message)
		{
			//	N'utilise pas le mécanisme de sélection automatique dans la liste
			//	si on a commencé l'édition en mode interactif.
			
			if ((this.mode == TextFieldExListMode.EditActive) ||
				(this.mode == TextFieldExListMode.EditPassive))
			{
				return false;
			}
			else
			{
				return base.ProcessKeyPressInSelectItemBehavior (message);
			}
		}

		
		private void HandleAcceptClicked(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.acceptRejectBehavior);
			this.AcceptEdition ();
		}		
		
		private void HandleRejectClicked(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.acceptRejectBehavior);
			this.AutoRejectEdition (true);
		}
		
		
		public const string						PlaceHolderTag = "$PlaceHolder$";
		
		private string							placeHolder;
		
		private TextFieldExListMode				mode = TextFieldExListMode.Undefined;
		private Behaviors.AcceptRejectBehavior	acceptRejectBehavior;
	}
}
