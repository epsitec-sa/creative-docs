//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

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
			
			this.caption_label.Bounds = new Drawing.Rectangle (0, y, 80, h_line);
			this.caption_label.Anchor = AnchorStyles.TopLeft;
			
			this.text_field.Bounds = new Drawing.Rectangle (this.caption_label.Right, y, panel.Width - this.caption_label.Width, h_line);
			this.text_field.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.text_field.TextChanged += new EventHandler (this.HandleTextFieldTextChanged);
			
			this.OnCaptionChanged ();
			
			this.SyncFromAdapter ();
		}
		
		public override void SyncFromAdapter()
		{
			Adapters.StringAdapter adapter = this.Adapter as Adapters.StringAdapter;
			
			if ((adapter != null) &&
				(this.text_field != null))
			{
				this.text_field.Text = adapter.Value;
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
			this.SyncFromUI ();
		}
		
		
		private TextField						text_field;
	}
}
