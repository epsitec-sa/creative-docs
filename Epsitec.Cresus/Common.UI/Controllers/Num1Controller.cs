//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for Num1Controller.
	/// </summary>
	public class Num1Controller : AbstractController
	{
		public Num1Controller()
		{
		}
		
		
		public override void CreateUI(Widget panel)
		{
			this.caption_label = new StaticText (panel);
			this.container     = new Widget (panel);
			this.text_field_1  = new TextFieldUpDown (this.container);
			
			double h_line = this.text_field_1.Height;
			double h_pane = System.Math.Max (panel.Height, h_line + 6);
			
			panel.Height = h_pane;
			
			double y = System.Math.Floor ((h_pane - h_line) / 2);
			
			this.caption_label.Size          = new Drawing.Size (80, h_line);
			this.caption_label.Anchor        = AnchorStyles.TopLeft;
			this.caption_label.AnchorMargins = new Drawing.Margins (0, 0, h_pane - y - h_line, 0);
			
			this.container.Height              = h_line;
			this.container.Anchor              = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.container.AnchorMargins       = new Drawing.Margins (this.caption_label.Right, 0, h_pane - y - h_line, 0);
			this.container.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.container.TabIndex            = 10;
			this.container.TabNavigation       = Widget.TabNavigationMode.ForwardTabPassive;
			
			this.text_field_1.Dock        = DockStyle.Fill;
			this.text_field_1.DockMargins = new Drawing.Margins (0, 2, 0, 0);
			this.text_field_1.MinValue    = 0;
			this.text_field_1.MaxValue    = 9999;
			this.text_field_1.Resolution  = 1;
			this.text_field_1.TabIndex    = 1;
			this.text_field_1.Name        = "Value_1";
			
			this.text_field_1.TextChanged += new EventHandler (this.HandleTextFieldTextChanged);
			
			this.OnCaptionChanged ();
			
			this.SyncFromAdapter (SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
			Adapters.Num1Adapter adapter = this.Adapter as Adapters.Num1Adapter;
			
			if (this.inhibit_events > 0)
			{
				return;
			}
			
			this.inhibit_events++;
			
			if ((adapter != null) &&
				(this.text_field_1 != null))
			{
				string value = adapter.Value;
				
				if ((value != null) &&
					(value.Length > 0))
				{
					this.text_field_1.Text = value;
				}
				else
				{
					this.text_field_1.Text = "";
				}
				
				if ((reason == Common.UI.SyncReason.SourceChanged) ||
					(reason == Common.UI.SyncReason.Initialisation))
				{
					this.text_field_1.SelectAll ();
				}
			}
			
			this.inhibit_events--;
		}
		
		public override void SyncFromUI()
		{
			Adapters.Num1Adapter adapter = this.Adapter as Adapters.Num1Adapter;
			
			if (this.inhibit_events > 0)
			{
				return;
			}
			
			this.inhibit_events++;
			
			if ((adapter != null) &&
				(this.text_field_1 != null))
			{
				try
				{
					adapter.Value = this.text_field_1.Text;
				}
				catch
				{
				}
			}
			
			this.inhibit_events--;
		}
		
		
		private void HandleTextFieldTextChanged(object sender)
		{
			this.OnUIDataChanged ();
		}
		
		
		private int								inhibit_events;
		private Widget							container;
		private TextFieldUpDown					text_field_1;
	}
}
