//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for Num2Controller.
	/// </summary>
	public class Num2Controller : AbstractController
	{
		public Num2Controller()
		{
		}
		
		
		public override void CreateUI(Widget panel)
		{
			this.caption_label = new StaticText (panel);
			this.container     = new Widget (panel);
			this.text_field_1  = new TextFieldUpDown (this.container);
			this.text_field_2  = new TextFieldUpDown (this.container);
			
			double h_line = this.text_field_1.Height;
			double h_pane = System.Math.Max (panel.Height, h_line + 6);
			
			panel.Height = h_pane;
			
			double y = System.Math.Floor ((h_pane - h_line) / 2);
			
			this.caption_label.Bounds = new Drawing.Rectangle (0, y, 80, h_line);
			this.caption_label.Anchor = AnchorStyles.TopLeft;
			
			this.container.Bounds = new Drawing.Rectangle (this.caption_label.Right, y, panel.Width - this.caption_label.Width, h_line);
			this.container.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.container.PreferHorizontalDockLayout = true;
			
			this.text_field_1.Dock        = DockStyle.Fill;
			this.text_field_1.DockMargins = new Drawing.Margins (0, 2, 0, 0);
			this.text_field_1.MinValue    = 0;
			this.text_field_1.MaxValue    = 9999;
			this.text_field_1.Resolution  = 1;
			this.text_field_2.Dock        = DockStyle.Fill;
			this.text_field_2.DockMargins = new Drawing.Margins (2, 0, 0, 0);
			this.text_field_2.MinValue    = 0;
			this.text_field_2.MaxValue    = 9999;
			this.text_field_2.Resolution  = 1;
			
			this.text_field_1.TextChanged += new EventHandler (this.HandleTextFieldTextChanged);
			this.text_field_2.TextChanged += new EventHandler (this.HandleTextFieldTextChanged);
			
			this.OnCaptionChanged ();
			
			this.SyncFromAdapter ();
		}
		
		public override void SyncFromAdapter()
		{
			Adapters.Num2Adapter adapter = this.Adapter as Adapters.Num2Adapter;
			
			if (this.inhibit_events > 0)
			{
				return;
			}
			
			this.inhibit_events++;
			
			if ((adapter != null) &&
				(this.text_field_1 != null) &&
				(this.text_field_2 != null))
			{
				string   value = adapter.Value;
				string[] args  = value == null ? null : value.Split (';');
				
				if ((args != null) &&
					(args.Length == 2))
				{
					this.text_field_1.Text = args[0];
					this.text_field_2.Text = args[1];
				}
				else
				{
					this.text_field_1.Text = "";
					this.text_field_2.Text = "";
				}
			}
			
			this.inhibit_events--;
		}
		
		public override void SyncFromUI()
		{
			Adapters.Num2Adapter adapter = this.Adapter as Adapters.Num2Adapter;
			
			if (this.inhibit_events > 0)
			{
				return;
			}
			
			this.inhibit_events++;
			
			if ((adapter != null) &&
				(this.text_field_1 != null) &&
				(this.text_field_2 != null))
			{
				try
				{
					adapter.Value = this.text_field_1.Text + ";" + this.text_field_2.Text;
				}
				catch
				{
				}
			}
			
			this.inhibit_events--;
		}
		
		
		private void HandleTextFieldTextChanged(object sender)
		{
			this.SyncFromUI ();
		}
		
		
		private int								inhibit_events;
		private Widget							container;
		private TextFieldUpDown					text_field_1;
		private TextFieldUpDown					text_field_2;
	}
}
