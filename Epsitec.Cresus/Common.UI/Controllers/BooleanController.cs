//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for BooleanController.
	/// </summary>
	public class BooleanController : AbstractController
	{
		public BooleanController()
		{
		}
		
		
		public override void CreateUI(Widget panel)
		{
			this.caption_label = new StaticText (panel);
			this.check_button  = new CheckButton (panel);
			
			double h_line = this.check_button.Height;
			double h_pane = System.Math.Max (panel.Height, h_line + 6);
			
			panel.Height = h_pane;
			
			double y = System.Math.Floor ((h_pane - h_line) / 2);
			
			this.caption_label.Size          = new Drawing.Size (80, h_line);
			this.caption_label.Anchor        = AnchorStyles.TopLeft;
			this.caption_label.Margins = new Drawing.Margins (0, 0, h_pane - y - h_line, 0);
			
			this.check_button.Text           = "True";
			this.check_button.Height         = h_line;
			this.check_button.Anchor         = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.check_button.Margins  = new Drawing.Margins (this.caption_label.Right, 0, h_pane - y - h_line, 0);
			this.check_button.TabIndex       = 10;
			this.check_button.Name           = "Value";
			
			this.check_button.ActiveStateChanged += new EventHandler (this.HandleCheckButtonActiveStateChanged);
			
			this.OnCaptionChanged ();
			
			this.SyncFromAdapter (SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
			Adapters.BooleanAdapter adapter = this.Adapter as Adapters.BooleanAdapter;
			
			if ((adapter != null) &&
				(this.check_button != null))
			{
				this.check_button.ActiveState = adapter.Value ? ActiveState.Yes : ActiveState.No;
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.BooleanAdapter adapter = this.Adapter as Adapters.BooleanAdapter;
			
			if ((adapter != null) &&
				(this.check_button != null))
			{
				adapter.Value = this.check_button.IsActive;
			}
		}
		
		
		private void HandleCheckButtonActiveStateChanged(object sender)
		{
			this.OnUIDataChanged ();
		}
		
		
		private CheckButton						check_button;
	}
}
