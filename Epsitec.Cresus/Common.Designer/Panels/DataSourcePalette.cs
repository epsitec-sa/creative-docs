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
			this.size = new Drawing.Size (172+2*10, 145+2*10);
			this.data_graph = new Common.UI.Data.Record ();
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
		
		public void DefineData(Types.IDataGraph data)
		{
			this.data_graph.Clear ();
			this.data_graph.AddGraph (data);
		}
		
		
		internal void NotifyActiveEditorChanged(Editors.WidgetEditor editor)
		{
			this.DefineData (editor.DialogDesigner.DialogData);
		}
		
		internal void NotifyDialogDataSourceChanged(Types.IDataGraph data)
		{
			this.DefineData (data);
		}
		
		
		protected override void CreateWidgets(Widget parent)
		{
			System.Diagnostics.Debug.Assert (this.widget == parent);
			
			this.data_list = new ScrollList (this.widget);
			
			this.data_list.Anchor        = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			this.data_list.AnchorMargins = new Drawing.Margins (4, 4, 4, 40);
			this.data_list.AdjustHeight (ScrollAdjustMode.MoveBottom);
			
			Common.UI.Engine.BindWidget (this.data_graph.Root, this.data_list);
			
			this.CreateDragSource (typeof (Button), "[ 1 ]", "1", 4+34*0, 133, 32, 32);
			this.CreateDragSource (typeof (Button), "[ 2 ]", "2", 4+34*1, 133, 32, 32);
			this.CreateDragSource (typeof (Button), "[ 3 ]", "3", 4+34*2, 133, 32, 32);
			this.CreateDragSource (typeof (Button), "[ 4 ]", "4", 4+34*3, 133, 32, 32);
			
			Button test = new Button (this.widget);
			
			test.Text          = "+";
			test.Size          = new Drawing.Size (test.DefaultHeight, test.DefaultHeight);
			test.Anchor        = AnchorStyles.BottomRight;
			test.AnchorMargins = new Drawing.Margins (0, 4, 0, 4);
			test.Clicked      += new MessageEventHandler(this.HandleTestClicked);
		}
		
		
		protected void CreateDragSource(System.Type type, string text, string name, double x, double y, double dx, double dy)
		{
			Widget             widget = System.Activator.CreateInstance (type) as Widget;
			Widgets.DragSource source = new Widgets.DragSource (this.widget);
			
			widget.Text = text;
			widget.Name = name;
			
			source.Widget   = widget;
			source.Parent   = this.widget;
			source.Location = new Drawing.Point (x, this.size.Height - y - dy);
			source.Size     = new Drawing.Size (dx, dy);
			
			source.DragBeginning += new Widgets.DragBeginningEventHandler (this.HandleSourceDragBeginning);
			source.DragBegin     += new Support.EventHandler (this.HandleSourceDragBegin);
			source.DragEnd       += new Support.EventHandler (this.HandleSourceDragEnd);
		}
		
		protected Common.UI.Widgets.DataWidget CreateDataWidget (Common.UI.Data.Representation mode)
		{
			Common.UI.Widgets.DataWidget widget = new Common.UI.Widgets.DataWidget ();
			
			if (this.data_list.SelectedIndex < 0)
			{
				return null;
			}
			
			Types.IDataValue data = this.data_graph.Navigate (this.data_list.SelectedName) as Types.IDataValue;
			
			if (data == null)
			{
				return null;
			}
			
			if (Common.UI.Widgets.DataWidget.CheckCompatibility (data, mode) == false)
			{
				return null;
			}
			
			widget.Representation = mode;
			widget.DataSource     = data;
			widget.Size           = widget.GetBestFitSize ();
			
			return widget;
		}
		
		
		private void HandleSourceDragBeginning(object sender, Widgets.DragBeginningEventArgs e)
		{
			string name   = e.Model.Name;
			Widget widget = null;
			
			switch (name)
			{
				case "1":
					widget = this.CreateDataWidget (Common.UI.Data.Representation.TextField);
					break;
				
				case "2":
					widget = this.CreateDataWidget (Common.UI.Data.Representation.NumericUpDown);
					break;
				
				case "3":
					widget = this.CreateDataWidget (Common.UI.Data.Representation.RadioList);
					break;
				
				case "4":
					widget = this.CreateDataWidget (Common.UI.Data.Representation.RadioColumns);
					break;
			}
			
			if (widget == null)
			{
				e.Cancel = true;
			}
			else
			{
				e.Replacement = widget;
			}
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
		
		
		private void HandleTestClicked(object sender, MessageEventArgs e)
		{
			Common.UI.Data.Field field = new Common.UI.Data.Field ("Field"+this.data_graph.Count.ToString (), "Abc");
			
			field.DefineCaption ("Field " + this.data_graph.Count.ToString ());
			
			this.data_graph.Add (field);
		}
		
		
		public event Support.EventHandler		DragBegin;
		public event Support.EventHandler		DragEnd;
		
		
		protected Widgets.DragSource			active_drag_source;
		protected ScrollList					data_list;
		protected Common.UI.Data.Record			data_graph;
	}
}
