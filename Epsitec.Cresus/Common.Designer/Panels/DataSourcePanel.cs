//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Panels
{
	using Representation = Epsitec.Common.UI.Data.Representation;
	
	/// <summary>
	/// La classe DataSourcePalette permet de remplir un panel proposant une liste
	/// des données utilisables avec DataWidget, pour la construction de la GUI.
	/// </summary>
	public class DataSourcePanel : AbstractPanel, IDropSource
	{
		public DataSourcePanel(Application application) : base (application)
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
			
			this.UpdateDragSources ();
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
			this.data_list.SelectedIndexChanged += new EventHandler (this.HandleDataListSelectedIndexChanged);
			
			Common.UI.Engine.BindWidget (this.data_graph.Root, this.data_list);
			
			this.CreateDragSource (Representation.TextField,     typeof (Button), @"<img src=""manifest:Epsitec.Common.Designer.Images.WidgetField.icon""/>", "1", 4+34*0, 133, 32, 32);
			this.CreateDragSource (Representation.NumericUpDown, typeof (Button), @"<img src=""manifest:Epsitec.Common.Designer.Images.WidgetFieldUpDown.icon""/>", "2", 4+34*1, 133, 32, 32);
			this.CreateDragSource (Representation.RadioList,     typeof (Button), @"<img src=""manifest:Epsitec.Common.Designer.Images.WidgetRadios.icon""/>", "3", 4+34*2, 133, 32, 32);
			this.CreateDragSource (Representation.CheckColumns,  typeof (Button), @"<img src=""manifest:Epsitec.Common.Designer.Images.WidgetChecks.icon""/>", "4", 4+34*3, 133, 32, 32);
			this.CreateDragSource (Representation.StatusLed,     typeof (Button), @"<img src=""manifest:Epsitec.Common.Designer.Images.WidgetChecks.icon""/>", "5", 4+34*4, 133, 32, 32);
			
			this.UpdateDragSources ();
		}
		
		
		protected void CreateDragSource(Representation representation, System.Type type, string text, string name, double x, double y, double dx, double dy)
		{
			Widget             widget = System.Activator.CreateInstance (type) as Widget;
			Widgets.DragSource source = new Widgets.DragSource (this.widget);
			
			widget.Text = text;
			widget.Name = name;
			
			source.Widget   = widget;
			source.SetParent (this.widget);
			source.Location = new Drawing.Point (x, this.size.Height - y - dy);
			source.Size     = new Drawing.Size (dx, dy);
			
			source.DragBeginning += new Widgets.DragBeginningEventHandler (this.HandleSourceDragBeginning);
			source.DragBegin     += new Support.EventHandler (this.HandleSourceDragBegin);
			source.DragEnd       += new Support.EventHandler (this.HandleSourceDragEnd);
			
			this.drag_sources[representation] = source;
		}
		
		protected void UpdateDragSources()
		{
			Types.IDataValue data = null;
			
			if (this.data_list.SelectedIndex >= 0)
			{
				data = this.data_graph.Navigate (this.data_list.SelectedName) as Types.IDataValue;
			}
			
			foreach (Representation mode in this.drag_sources.Keys)
			{
				Widget widget = this.drag_sources[mode] as Widget;
				
				if ((data != null) &&
					(Common.UI.Widgets.DataWidget.CheckCompatibility (data, mode)))
				{
					widget.SetEnabled (true);
				}
				else
				{
					widget.SetEnabled (false);
				}
			}
		}
		
		protected Common.UI.Widgets.DataWidget CreateDataWidget (Representation mode)
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
			
			widget.ResourceManager = this.application.UserResourceManager;
			widget.Representation  = mode;
			widget.DataSource      = data;
			widget.Size            = widget.GetBestFitSize ();
			widget.BindingInfo     = Common.UI.Engine.MakeBindingDefinition (this.data_list.SelectedName);
			
			return widget;
		}
		
		
		private void HandleSourceDragBeginning(object sender, Widgets.DragBeginningEventArgs e)
		{
			string name = e.Model.Name;
			
			Common.UI.Widgets.DataWidget widget = null;
			
			switch (name)
			{
				case "1":
					widget = this.CreateDataWidget (Representation.TextField);
					break;
				
				case "2":
					widget = this.CreateDataWidget (Representation.NumericUpDown);
					break;
				
				case "3":
					widget = this.CreateDataWidget (Representation.RadioList);
					break;
				
				case "4":
					widget = this.CreateDataWidget (Representation.CheckColumns);
					break;
				
				case "5":
					widget = this.CreateDataWidget (Representation.StatusLed);
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
		
		
		private void HandleDataListSelectedIndexChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.data_list == sender);
			
			this.UpdateDragSources ();
		}
		
		
		protected override void UpdateUserResourceManager()
		{
			Support.ResourceManager resource_manager = this.application.UserResourceManager;
			
			this.data_list.ResourceManager = resource_manager;
			this.data_list.Invalidate ();
		}

		
		
		
		public event Support.EventHandler		DragBegin;
		public event Support.EventHandler		DragEnd;
		
		
		protected Widgets.DragSource			active_drag_source;
		protected ScrollList					data_list;
		protected Common.UI.Data.Record			data_graph;
		protected System.Collections.Hashtable	drag_sources = new System.Collections.Hashtable ();
	}
}
