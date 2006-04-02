//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for FlagsController.
	/// </summary>
	public class FlagsController : AbstractController
	{
		public FlagsController()
		{
		}
		
		
		public override void CreateUI(Widget panel)
		{
			Adapters.FlagsAdapter adapter = this.Adapter as Adapters.FlagsAdapter;
			
			Types.IEnumType enum_type = adapter.EnumType;
			
			this.data_widget = new Widgets.DataWidget (panel);
			this.data_source = new Data.Field (enum_type.Name, null, enum_type);
			
			this.data_widget.Representation = Data.Representation.CheckList;
			this.data_widget.HasCaption     = false;
			this.data_widget.DataSource     = this.data_source;
			
			this.caption_label = new StaticText (panel);
			
			double h_data    = this.data_widget.GetBestFitSize ().Height;
			double h_pane    = System.Math.Max (panel.Height, h_data + 6);
			
			panel.Height = h_pane;
			
			double y = System.Math.Floor ((h_pane - h_data) / 2);
			
			this.caption_label.Width         = 80;
			this.caption_label.Anchor        = AnchorStyles.TopLeft;
			this.caption_label.Margins = new Drawing.Margins (0, 0, h_pane - y - h_data + 1, 0);
			
			this.data_widget.Height         = h_data;
			this.data_widget.Anchor         = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.data_widget.Margins  = new Drawing.Margins (this.caption_label.Right, 0, h_pane - y - h_data, 0);
			
			this.data_source.Changed += new EventHandler (this.HandleDataSourceChanged);
			
			this.OnCaptionChanged ();
			
			this.SyncFromAdapter (SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
			Adapters.FlagsAdapter adapter = this.Adapter as Adapters.FlagsAdapter;
			
			if ((adapter != null) &&
				(this.data_widget != null))
			{
				this.data_source.Value = adapter.Value;
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.FlagsAdapter adapter = this.Adapter as Adapters.FlagsAdapter;
			
			if ((adapter != null) &&
				(this.data_widget != null))
			{
				string      name = this.data_source.Value.ToString ();
				System.Type type = adapter.EnumType.SystemType;
				System.Enum value;
				
				Types.Converter.Convert (name, type, out value);
				
				adapter.Value = value;
			}
		}
		
		
		private void HandleDataSourceChanged(object sender)
		{
			this.OnUIDataChanged ();
		}
		
		
		private Data.Field						data_source;
		private Widgets.DataWidget				data_widget;
	}
}
