//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for StringController.
	/// </summary>
	public class StringController : AbstractController
	{
		public StringController()
		{
		}
		
		
		public override void CreateUI(Widget panel)
		{
			this.caption_label = new StaticText (panel);
			this.text_field    = new TextField (panel);
			
			double h_line = this.text_field.Height;
			double h_pane = System.Math.Max (panel.Height, h_line + 6);
			
			panel.Height = h_pane;
			
			double y = System.Math.Floor ((h_pane - h_line) / 2);
			
			this.caption_label.Size          = new Drawing.Size (80, h_line);
			this.caption_label.Anchor        = AnchorStyles.TopLeft;
			this.caption_label.Margins = new Drawing.Margins (0, 0, h_pane - y - h_line, 0);
			
			this.text_field.Height           = h_line;
			this.text_field.Anchor           = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.text_field.Margins    = new Drawing.Margins (this.caption_label.Right, 0, h_pane - y - h_line, 0);
			this.text_field.TextChanged     += new EventHandler (this.HandleTextFieldTextChanged);
			this.text_field.TabIndex         = 10;
			this.text_field.Name             = "Value";
			
			this.OnCaptionChanged ();
			
			this.SyncFromAdapter (SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
			Adapters.StringAdapter adapter = this.Adapter as Adapters.StringAdapter;
			
			if ((adapter != null) &&
				(this.text_field != null))
			{
				this.text_field.Text = adapter.Value;
				
				if ((reason == Common.UI.SyncReason.SourceChanged) ||
					(reason == Common.UI.SyncReason.Initialisation))
				{
					this.text_field.SelectAll ();
				}
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.StringAdapter adapter = this.Adapter as Adapters.StringAdapter;
			
			if ((adapter != null) &&
				(this.text_field != null))
			{
				adapter.Value = this.text_field.Text;
			}
		}
		
		
		private void HandleTextFieldTextChanged(object sender)
		{
			this.OnUIDataChanged ();
		}
		
		
		private TextField						text_field;
	}
}
