//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Panels
{
	/// <summary>
	/// La classe DataSourcePalette permet de remplir un panel proposant une liste
	/// des données utilisables avec DataWidget, pour la construction de la GUI.
	/// </summary>
	public class DataSourcePalette : AbstractPalette, IDropSource
	{
		public DataSourcePalette()
		{
			Common.UI.Data.Record record = new Common.UI.Data.Record ();
			
			Common.UI.Data.Field field_a = new Common.UI.Data.Field ("a", "");
			Common.UI.Data.Field field_b = new Common.UI.Data.Field ("b", 0);
			Common.UI.Data.Field field_c = new Common.UI.Data.Field ("c", Common.UI.Data.Representation.None);
			
			field_a.DefineCaption ("Texte A");
			field_b.DefineCaption ("Valeur B");
			field_c.DefineCaption ("Enumération C");
			
			record.Add (field_a);
			record.Add (field_b);
			record.Add (field_c);
			
			this.size = new Drawing.Size (172+2*10, 145+2*10);
			this.data_graph = record;
		}
		
		
		#region IDropSource Members
		public Widget							DroppedWidget
		{
			get
			{
				if (this.active_drag_source != null)
				{
					return this.active_drag_source.DroppedWidget;
				}
				
				return null;
			}
		}
		#endregion
		
		protected override void CreateWidgets(Widget parent)
		{
			System.Diagnostics.Debug.Assert (this.widget == parent);
			
			this.data_list = new ScrollList (this.widget);
			
			this.data_list.Anchor        = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			this.data_list.AnchorMargins = new Drawing.Margins (4, 4, 4, 40);
			this.data_list.AdjustHeight (ScrollAdjustMode.MoveBottom);
			
			Common.UI.Engine.BindWidget (this.data_graph.Root, this.data_list);
			
			this.CreateDragSource (typeof (Button), "[ 1 ]", 10+34*0, 133, 32, 32);
			this.CreateDragSource (typeof (Button), "[ 2 ]", 10+34*1, 133, 32, 32);
			this.CreateDragSource (typeof (Button), "[ 3 ]", 10+34*2, 133, 32, 32);
			
			Button test = new Button (this.widget);
			
			test.Text          = "+";
			test.Size          = new Drawing.Size (test.DefaultHeight, test.DefaultHeight);
			test.Anchor        = AnchorStyles.BottomRight;
			test.AnchorMargins = new Drawing.Margins (0, 4, 0, 4);
			test.Clicked      += new MessageEventHandler(this.HandleTestClicked);
		}
		
		protected void CreateDragSource(System.Type type, string text, double x, double y, double dx, double dy)
		{
			Widget             widget = System.Activator.CreateInstance (type) as Widget;
			Widgets.DragSource source = new Widgets.DragSource (this.widget);
			
			widget.Text = text;
			
			source.Widget   = widget;
			source.Parent   = this.widget;
			source.Location = new Drawing.Point (x, this.size.Height - y - dy);
			source.Size     = new Drawing.Size (dx, dy);
			
			source.DragBegin += new Support.EventHandler (this.HandleSourceDragBegin);
			source.DragEnd   += new Support.EventHandler (this.HandleSourceDragEnd);
		}
		
		
		private void HandleSourceDragBegin(object sender)
		{
			System.Diagnostics.Debug.Assert (this.active_drag_source == null);
			
			this.active_drag_source = sender as Widgets.DragSource;
			
			if (this.DragBegin != null)
			{
				this.DragBegin (this);
			}
		}
		
		private void HandleSourceDragEnd(object sender)
		{
			System.Diagnostics.Debug.Assert (this.active_drag_source == sender);
			
			if (this.DragEnd != null)
			{
				this.DragEnd (this);
			}
			
			this.active_drag_source = null;
		}
		
		
		public event Support.EventHandler		DragBegin;
		public event Support.EventHandler		DragEnd;
		
		
		protected Widgets.DragSource			active_drag_source;
		protected ScrollList					data_list;
		protected Types.IDataGraph				data_graph;

		private void HandleTestClicked(object sender, MessageEventArgs e)
		{
			Common.UI.Data.Record record = this.data_graph as Common.UI.Data.Record;
			Common.UI.Data.Field  field  = new Common.UI.Data.Field ("Field"+record.Count.ToString (), "");
			
			record.Add (field);
		}
	}
}
