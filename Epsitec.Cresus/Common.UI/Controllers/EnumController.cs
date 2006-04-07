//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for EnumController.
	/// </summary>
	public class EnumController : AbstractController
	{
		public EnumController()
		{
		}
		
		
		public override void CreateUI(Widget panel)
		{
			this.caption_label = new StaticText (panel);
			this.combo_value   = new TextFieldCombo (panel);
			
			double h_line = this.combo_value.Height;
			double h_pane = System.Math.Max (panel.Height, h_line + 6);
			
			panel.Height = h_pane;
			
			double y = System.Math.Floor ((h_pane - h_line) / 2);
			
			this.caption_label.Size          = new Drawing.Size (80, h_line);
			this.caption_label.Anchor        = AnchorStyles.TopLeft;
			this.caption_label.Margins = new Drawing.Margins (0, 0, h_pane - y - h_line, 0);
			
			this.combo_value.IsReadOnly     = true;
			this.combo_value.Height         = h_line;
			this.combo_value.Anchor         = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.combo_value.Margins  = new Drawing.Margins (this.caption_label.Right, 0, h_pane - y - h_line, 0);
			this.combo_value.TabIndex       = 10;
			this.combo_value.Name           = "Value";
			
			this.combo_value.SelectedIndexChanged += new EventHandler (this.HandleComboValueSelectedIndexChanged);
			
			this.OnCaptionChanged ();
			
			this.SyncFromAdapter (SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
			Adapters.EnumAdapter adapter = this.Adapter as Adapters.EnumAdapter;
			
			if ((adapter != null) &&
				(this.combo_value != null))
			{
				if (reason == SyncReason.Initialisation)
				{
					this.combo_value.Items.Clear ();
					
					Types.IEnumType    enum_type   = adapter.EnumType;
					Types.IEnumValue[] enum_values = enum_type.Values;
					
					foreach (Types.IEnumValue value in enum_values)
					{
						if (value.IsHidden)
						{
							continue;
						}
						
						string name    = value.Name;
						string caption = value.Caption;
						
						if (caption == null)
						{
							caption = name;
						}
						
						this.combo_value.Items.Add (name, caption);
					}
				}
				
				if (adapter.Value != null)
				{
					this.combo_value.SelectedName = adapter.Value.ToString ();
				}
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.EnumAdapter adapter = this.Adapter as Adapters.EnumAdapter;
			
			if ((adapter != null) &&
				(this.combo_value != null))
			{
				string      name = this.combo_value.SelectedName;
				System.Type type = adapter.EnumType.SystemType;
				System.Enum value;
				
				Types.Converter.Convert (name, type, out value);
				
				adapter.Value = value;
			}
		}
		
		
		private void HandleComboValueSelectedIndexChanged(object sender)
		{
			this.OnUIDataChanged ();
		}
		
		
		private TextFieldCombo					combo_value;
	}
}
