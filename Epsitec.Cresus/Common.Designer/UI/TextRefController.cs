//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.UI
{
	/// <summary>
	/// Summary description for TextRefController.
	/// </summary>
	public class TextRefController : Epsitec.Common.UI.Controllers.AbstractController
	{
		public TextRefController()
		{
		}
		
		
		public override void CreateUI(Widget panel)
		{
			this.caption_label = new StaticText (panel);
			this.combo_text    = new TextFieldCombo (panel);
			this.label_bundle  = new StaticText (panel);
			this.combo_bundle  = new TextFieldCombo (panel);
			this.label_field   = new StaticText (panel);
			this.combo_field   = new TextFieldExList (panel);
			
			panel.Height = System.Math.Max (panel.Height, 82);
			
			this.caption_label.Width         = 80;
			this.caption_label.Anchor        = AnchorStyles.TopLeft;
			this.caption_label.AnchorMargins = new Drawing.Margins (0, 0, 8, 0);
			
			this.combo_text.Anchor           = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.combo_text.AnchorMargins    = new Drawing.Margins (this.caption_label.Right, 0, 4, 0);
			this.combo_text.TextChanged     += new EventHandler (this.HandleComboTextTextChanged);
			this.combo_text.TextEdited      += new EventHandler (this.HandleComboTextTextEdited);
			this.combo_text.OpeningCombo    += new CancelEventHandler (this.HandleComboTextOpeningCombo);
			this.combo_text.Button.Clicked  += new MessageEventHandler (this.HandleComboTextButtonClicked);
			this.combo_text.ButtonGlyphShape = GlyphShape.Dots;
			this.combo_text.TabIndex         = 10;
			this.combo_text.Name             = "Value_1";
			
			this.label_bundle.Width          = this.caption_label.Width;
			this.label_bundle.Anchor         = this.caption_label.Anchor;
			this.label_bundle.AnchorMargins  = new Drawing.Margins (0, 0, 8+20+8, 0);
			this.label_bundle.Text           = "Bundle Name";
			
			this.combo_bundle.Anchor         = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.combo_bundle.AnchorMargins  = new Drawing.Margins (this.caption_label.Right, 0, 4+20+8, 0);
			this.combo_bundle.IsReadOnly     = true;
			this.combo_bundle.TabIndex       = 11;
			this.combo_bundle.Name           = "Value_2";
			
			this.label_field.Width           = this.caption_label.Width;
			this.label_field.Anchor          = this.caption_label.Anchor;
			this.label_field.AnchorMargins   = new Drawing.Margins (0, 0, 8+20+8+20+4, 0);
			this.label_field.Text            = "Field Name";
			
			this.combo_field.Anchor          = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.combo_field.AnchorMargins   = new Drawing.Margins (this.caption_label.Right, 0, 4+20+8+20+4, 0);
			this.combo_field.IsReadOnly      = true;
			this.combo_field.PlaceHolder     = "<b>&lt;create new field&gt;</b>";
			this.combo_field.TabIndex        = 12;
			this.combo_field.Name            = "Value_3";
			this.combo_field.DefocusAction   = DefocusAction.AutoAcceptOrRejectEdition;
			
			IValidator field_validator_1 = new Common.Widgets.Validators.RegexValidator (this.combo_field, RegexFactory.ResourceName, false);
			IValidator field_validator_2 = new CustomFieldValidator (this.combo_field, this);
			IValidator bundle_validator  = new Common.Widgets.Validators.RegexValidator (this.combo_bundle, RegexFactory.ResourceName, false);
			
			this.combo_bundle.SelectedIndexChanged += new EventHandler (this.HandleComboBundleSelectedIndexChanged);
			this.combo_field.SelectedIndexChanged  += new EventHandler (this.HandleComboFieldSelectedIndexChanged);
			this.combo_field.EditionAccepted       += new EventHandler (this.HandleComboFieldEditionAccepted);
			this.combo_field.EditionRejected       += new EventHandler (this.HandleComboFieldEditionRejected);
			
			this.OnCaptionChanged ();
			
			this.SyncFromAdapter (Common.UI.SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(Common.UI.SyncReason reason)
		{
			System.Diagnostics.Debug.Assert (this.sync_level == 0);
			
			this.sync_level++;
			
			if ((reason == Common.UI.SyncReason.SourceChanged) ||
				(reason == Common.UI.SyncReason.Initialisation))
			{
				this.State = InternalState.Passive;
			}
			
			TextRefAdapter adapter = this.Adapter as TextRefAdapter;
			
			if ((adapter != null) &&
				(this.combo_text != null))
			{
				string current_text = adapter.Value;
				string stored_text  = adapter.GetFieldValue ();
				
				this.combo_text.Text = current_text;
				
				if (current_text == stored_text)
				{
					string field  = adapter.FieldName;
					string bundle = adapter.BundleName;
					
					this.UpdateBundleList (adapter);
					this.UpdateFieldList (adapter, bundle);
					
					this.combo_bundle.Text   = bundle;
					this.combo_bundle.Cursor = 0;
					this.combo_field.Text    = field;
					this.combo_field.Cursor  = 0;
				}
				else if (stored_text == null)
				{
					if (this.State != InternalState.UsingCustomText)
					{
						this.State = InternalState.UsingUndefinedText;
					}
				}
				
				if (reason != Common.UI.SyncReason.ValueChanged)
				{
					this.combo_text.SelectAll ();
				}
				
				if ((reason == Common.UI.SyncReason.SourceChanged) ||
					(reason == Common.UI.SyncReason.Initialisation))
				{
					this.combo_text.SelectAll ();
				}
			}
			
			this.sync_level--;
		}
		
		public override void SyncFromUI()
		{
			TextRefAdapter adapter = this.Adapter as TextRefAdapter;
			
			if ((adapter != null) &&
				(this.combo_text != null))
			{
				adapter.Value = this.combo_text.Text;
			}
		}
		
		
		protected void UpdateBundleList(TextRefAdapter adapter)
		{
			if (adapter != null)
			{
				this.combo_bundle.Items.Clear ();
				this.combo_bundle.Items.AddRange (adapter.StringController.GetStringBundleNames ());
			}
		}
		
		protected void UpdateFieldList(TextRefAdapter adapter, string name)
		{
			if (adapter != null)
			{
				if (name == "")
				{
					this.combo_field.SetEnabled (false);
				}
				else
				{
					this.combo_field.SetEnabled (true);
					this.combo_field.Items.Clear ();
					this.combo_field.Items.AddRange (adapter.StringController.GetStringFieldNames (name));
				}
			}
		}
		
		
		private void HandleComboTextTextChanged(object sender)
		{
			if (this.sync_level > 0)
			{
				return;
			}
			
			this.SyncFromUI ();
		}
		
		private void HandleComboTextTextEdited(object sender)
		{
			if (this.sync_level > 0)
			{
				return;
			}
			
			this.SyncFromUI ();
			
			this.State = InternalState.UsingCustomText;
		}
		
		private void HandleComboTextOpeningCombo(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
		}
		
		private void HandleComboTextButtonClicked(object sender, MessageEventArgs e)
		{
			this.combo_text.SelectAll ();
			this.combo_text.Focus ();
			
			//	Demande à l'application de commuter dans l'éditeur de ressources textuelles
			//	avec des boutons pour accepter/rejeter le choix. A la fin de l'édition, on
			//	demande d'appeler HandleStringPickerValidated en cas d'acceptation :
			
			TextRefAdapter adapter = this.Adapter as TextRefAdapter;
			
			adapter.Application.OpenStringPicker (new Support.EventHandler (this.HandleStringPickerValidated));
		}
		
		private void HandleStringPickerValidated(object sender)
		{
			//	L'utilisateur a validé le choix d'une string dans l'éditeur de ressources
			//	textuelles. Il faut reprendre le nom du bundle et celui du champ :
			
			Application            app     = sender as Application;
			Panels.StringEditPanel panel   = app.StringEditController.ActivePanel;
			EditArray              edit    = panel.EditArray;
			TextRefAdapter         adapter = this.Adapter as TextRefAdapter;
			
			int index = edit.SelectedIndex;
			
			if (index >= 0)
			{
				this.UpdateBundleList (adapter);
				
				string bundle = panel.Store.Name;
				string field  = edit[index, 0];
				
				this.combo_bundle.SelectedItem = bundle;
				this.combo_field.SelectedItem  = field;
				
				System.Diagnostics.Debug.Assert (this.combo_bundle.SelectedItem == bundle);
				System.Diagnostics.Debug.Assert (this.combo_field.SelectedItem == field);
				
				this.combo_text.SelectAll ();
				this.combo_text.Focus ();
			}
		}
		
		private void HandleComboBundleSelectedIndexChanged(object sender)
		{
			if (this.sync_level > 0)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (sender == this.combo_bundle);
			TextRefAdapter adapter = this.Adapter as TextRefAdapter;
			this.UpdateFieldList (adapter, this.combo_bundle.SelectedItem);
		}
		
		private void HandleComboFieldSelectedIndexChanged(object sender)
		{
			if (this.sync_level > 0)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (sender == this.combo_field);
			TextRefAdapter adapter = this.Adapter as TextRefAdapter;
			
			string bundle = this.combo_bundle.SelectedItem;
			string field  = this.combo_field.SelectedItem;
			
			if ((bundle != "") &&
				(field != ""))
			{
#if false
				adapter.XmlRefTarget = ResourceBundle.MakeTarget (bundle, field);
#else
				adapter.TextRef = ResourceBundle.MakeTarget (bundle, field);
#endif
				
				this.SyncFromAdapter (Common.UI.SyncReason.AdapterChanged);
				this.State = InternalState.UsingExistingText;
			}
			else
			{
#if false
				adapter.XmlRefTarget = "";
#else
				adapter.TextRef = "";
#endif
			}
		}
		
		private void HandleComboFieldEditionAccepted(object sender)
		{
			if (this.sync_level > 0)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (sender == this.combo_field);
			
			//	Il faut créer un nouveau champ dans le bundle.
			
			TextRefAdapter adapter = this.Adapter as TextRefAdapter;
			
			string bundle = this.combo_bundle.SelectedItem;
			string field  = this.combo_field.Text;
			string value  = this.combo_text.Text;
			
			if ((bundle != "") &&
				(field != ""))
			{
				adapter.DefineFieldValue (bundle, field, value);
#if false
				adapter.XmlRefTarget = ResourceBundle.MakeTarget (bundle, field);
#else
				adapter.TextRef = ResourceBundle.MakeTarget (bundle, field);
#endif
				
				this.SyncFromAdapter (Common.UI.SyncReason.AdapterChanged);
				this.State = InternalState.UsingExistingText;
			}
		}
		
		private void HandleComboFieldEditionRejected(object sender)
		{
			if (this.sync_level > 0)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (sender == this.combo_field);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Rejected field, restored to {0}#{1}.", this.combo_bundle.Text, this.combo_field.Text));
		}
		
		
		private InternalState					State
		{
			get
			{
				return this.state;
			}
			set
			{
				if (this.state != value)
				{
					TextRefAdapter adapter = this.Adapter as TextRefAdapter;
					
					switch (value)
					{
						case InternalState.Passive:
							this.combo_field.RejectEdition ();
							break;
						case InternalState.UsingCustomText:
							if (this.combo_field.Mode == TextFieldExListMode.EditActive)
							{
								//	L'utilisateur a déjà commencé à éditer le nom du champ, donc il n'y a rien à faire
								//	de particulier ici.
							}
							else
							{
								this.combo_field.StartPassiveEdition (this.combo_field.PlaceHolder);
							}
#if false
							adapter.XmlRefTarget = "";
#else
							adapter.TextRef = "";
#endif
							break;
						case InternalState.UsingExistingText:
							this.combo_field.RejectEdition ();
							break;
						case InternalState.UsingUndefinedText:
							this.combo_field.StartPassiveEdition (this.combo_field.PlaceHolder);
							break;
					}
					
					this.state = value;
				}
			}
		}
		
		
		[SuppressBundleSupport]
		
		private class CustomFieldValidator : Epsitec.Common.Widgets.Validators.AbstractTextValidator
		{
			public CustomFieldValidator(Widget widget, TextRefController controller) : base (widget)
			{
				this.controller = controller;
			}
			
			protected override void ValidateText(string text)
			{
				string bundle = this.controller.combo_bundle.SelectedItem;
				
				TextRefAdapter adapter = this.controller.Adapter as TextRefAdapter;
				
				if ((adapter != null) &&
					(adapter.StringController != null) &&
					(adapter.StringController.IsStringBundleLoaded (bundle)) &&
					(System.Array.IndexOf (adapter.StringController.GetStringFieldNames (bundle), text) == -1))
				{
					this.state = Support.ValidationState.Ok;
				}
				else
				{
					this.state = Support.ValidationState.Error;
				}
			}
			
			
			private TextRefController			controller;
		}
		
		
		private enum InternalState
		{
			None,
			Passive,
			UsingCustomText,
			UsingUndefinedText,
			UsingExistingText
		}
		
		
		private int								sync_level;
		private InternalState					state;
		private TextFieldCombo					combo_text;
		
		private StaticText						label_field;
		private StaticText						label_bundle;
		
		private TextFieldCombo					combo_bundle;
		private TextFieldExList					combo_field;
	}
}
