//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for TextRefController.
	/// </summary>
	public class TextRefController : AbstractController
	{
		public TextRefController()
		{
		}
		
		
		public override void CreateUI(Widget panel)
		{
			this.caption_label = new StaticText (panel);
			this.text_field    = new TextField (panel);
			this.combo_name    = new TextFieldCombo (panel);
			this.combo_bundle  = new TextFieldCombo (panel);
			
			panel.Height = System.Math.Max (panel.Height, 82);
			
			this.caption_label.Width         = 80;
			this.caption_label.Anchor        = AnchorStyles.TopLeft;
			this.caption_label.AnchorMargins = new Drawing.Margins (0, 0, 8, 0);
			
			this.text_field.Anchor           = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.text_field.AnchorMargins    = new Drawing.Margins (this.caption_label.Right, 0, 4, 0);
			this.text_field.TextChanged     += new EventHandler (this.HandleTextFieldTextChanged);
			
			this.combo_name.Anchor           = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.combo_name.AnchorMargins    = new Drawing.Margins (this.caption_label.Right, 0, 4+20+8, 0);
			
			this.combo_bundle.Anchor         = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.combo_bundle.AnchorMargins  = new Drawing.Margins (this.caption_label.Right, 0, 4+20+8+20+4, 0);
			
			this.OnCaptionChanged ();
			
			this.SyncFromAdapter ();
		}
		
		public override void SyncFromAdapter()
		{
			Adapters.TextRefAdapter adapter = this.Adapter as Adapters.TextRefAdapter;
			
			if ((adapter != null) &&
				(this.text_field != null))
			{
				this.text_field.Text = adapter.Value;
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.TextRefAdapter adapter = this.Adapter as Adapters.TextRefAdapter;
			
			if ((adapter != null) &&
				(this.text_field != null))
			{
				adapter.Value = this.text_field.Text;
			}
		}
		
		
		private void HandleTextFieldTextChanged(object sender)
		{
			this.SyncFromUI ();
		}
		
		
		private TextField						text_field;
		private TextFieldCombo					combo_name;
		private TextFieldCombo					combo_bundle;
	}
}
