//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

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
			this.text_field    = new TextField (panel);
			this.label_field   = new StaticText (panel);
			this.combo_field   = new TextFieldCombo (panel);
			this.label_bundle  = new StaticText (panel);
			this.combo_bundle  = new TextFieldCombo (panel);
			
			panel.Height = System.Math.Max (panel.Height, 82);
			
			this.caption_label.Width         = 80;
			this.caption_label.Anchor        = AnchorStyles.TopLeft;
			this.caption_label.AnchorMargins = new Drawing.Margins (0, 0, 8, 0);
			
			this.text_field.Anchor           = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.text_field.AnchorMargins    = new Drawing.Margins (this.caption_label.Right, 0, 4, 0);
			this.text_field.TextChanged     += new EventHandler (this.HandleTextFieldTextChanged);
			
			this.label_field.Width           = this.caption_label.Width;
			this.label_field.Anchor          = this.caption_label.Anchor;
			this.label_field.AnchorMargins   = new Drawing.Margins (0, 0, 8+20+8, 0);
			this.label_field.Text            = "Field Name";
			
			this.combo_field.Anchor          = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.combo_field.AnchorMargins   = new Drawing.Margins (this.caption_label.Right, 0, 4+20+8, 0);
			this.combo_field.IsReadOnly      = true;
			
			this.label_bundle.Width          = this.caption_label.Width;
			this.label_bundle.Anchor         = this.caption_label.Anchor;
			this.label_bundle.AnchorMargins  = new Drawing.Margins (0, 0, 8+20+8+20+4, 0);
			this.label_bundle.Text           = "Bundle Name";
			
			this.combo_bundle.Anchor         = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.combo_bundle.AnchorMargins  = new Drawing.Margins (this.caption_label.Right, 0, 4+20+8+20+4, 0);
			this.combo_bundle.IsReadOnly     = true;
			
			this.combo_field.SelectedIndexChanged  += new EventHandler(this.HandleComboFieldSelectedIndexChanged);
			this.combo_bundle.SelectedIndexChanged += new EventHandler(this.HandleComboBundleSelectedIndexChanged);
			
			this.OnCaptionChanged ();
			
			this.SyncFromAdapter ();
		}
		
		public override void SyncFromAdapter()
		{
			TextRefAdapter adapter = this.Adapter as TextRefAdapter;
			
			if ((adapter != null) &&
				(this.text_field != null))
			{
				this.text_field.Text = adapter.Value;
				
				string field  = adapter.FieldName;
				string bundle = adapter.BundleName;
				
				this.UpdateBundleList (adapter);
				this.UpdateFieldList (adapter, bundle);
				
				this.combo_bundle.Text = bundle;
				this.combo_field.Text  = field;
			}
		}
		
		public override void SyncFromUI()
		{
			TextRefAdapter adapter = this.Adapter as TextRefAdapter;
			
			if ((adapter != null) &&
				(this.text_field != null))
			{
				adapter.Value = this.text_field.Text;
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
				this.combo_field.Items.Clear ();
				this.combo_field.Items.AddRange (adapter.StringController.GetStringFieldNames (name));
			}
		}
		
		
		private void HandleTextFieldTextChanged(object sender)
		{
			this.SyncFromUI ();
		}
		
		private void HandleComboBundleSelectedIndexChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.combo_bundle);
			TextRefAdapter adapter = this.Adapter as TextRefAdapter;
			this.UpdateFieldList (adapter, this.combo_bundle.SelectedItem);
		}
		
		private void HandleComboFieldSelectedIndexChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.combo_field);
			TextRefAdapter adapter = this.Adapter as TextRefAdapter;
			
			string bundle = this.combo_bundle.SelectedItem;
			string field  = this.combo_field.SelectedItem;
			
			if ((bundle != "") &&
				(field != ""))
			{
				adapter.XmlRefTarget = ResourceBundle.MakeTarget (bundle, field);
				
				this.SyncFromAdapter ();
			}
		}
		
		
		private TextField						text_field;
		
		private StaticText						label_field;
		private StaticText						label_bundle;
		
		private TextFieldCombo					combo_field;
		private TextFieldCombo					combo_bundle;
	}
}
