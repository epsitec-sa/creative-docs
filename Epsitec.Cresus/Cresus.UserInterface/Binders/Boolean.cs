//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.UserInterface.Binders
{
	/// <summary>
	/// Summary description for Boolean.
	/// </summary>
	public class Boolean : IBinder
	{
		public Boolean()
		{
			BinderFactory.RegisterBinder ("boolean", this);
		}
		
		
		#region IBinder Members
		public void CreateBinding(object ui_object, DataLayer.DataSet root, string binding_path, DataLayer.DataRecord data_record)
		{
			Epsitec.Common.Widgets.Widget widget = ui_object as Epsitec.Common.Widgets.Widget;
			DataLayer.DataField           field  = data_record as DataLayer.DataField;
			
			System.Diagnostics.Debug.Assert (widget != null);
			System.Diagnostics.Debug.Assert (field != null);
			
			new Controller (widget, root, binding_path);
		}
		#endregion
		
		protected class Controller
		{
			public Controller(Epsitec.Common.Widgets.Widget widget, DataLayer.DataSet data_set, string binding_path)
			{
				this.widget   = widget;
				this.data_set = data_set;
				this.binding  = binding_path;
				
				this.widget.ActiveStateChanged += new Epsitec.Common.Widgets.EventHandler(this.SetDataFromWidget);
				data_set.AttachObserver (binding, new DataLayer.DataChangedHandler (this.SetWidgetFromData));
				
				this.SetWidgetFromData (null, null);
			}
			
			public void SetWidgetValue(bool value)
			{
				widget.ActiveState = value ? Epsitec.Common.Widgets.WidgetState.ActiveYes : Epsitec.Common.Widgets.WidgetState.ActiveNo;
				
				System.Diagnostics.Debug.WriteLine ("Setting widget to " + value.ToString ());
			}
			
			public void SetDataFromWidget(object sender)
			{
				bool value1 = (this.widget.ActiveState == Epsitec.Common.Widgets.WidgetState.ActiveYes);
				bool value2 = (bool) this.data_set.GetData (this.binding);
				
				if (value1 != value2)
				{
					System.Diagnostics.Debug.WriteLine ("Widget changed to " + value1.ToString () + ", updating data");
					this.data_set.UpdateData (this.binding, value1);
				}
			}
			
			public void SetWidgetFromData(DataLayer.AbstractRecord data, string path)
			{
				bool value1 = (bool) this.data_set.GetData (this.binding);
				bool value2 = (this.widget.ActiveState == Epsitec.Common.Widgets.WidgetState.ActiveYes);
				
				if (value1 != value2)
				{
					System.Diagnostics.Debug.WriteLine ("Data changed to " + value1.ToString () + ", updating widget");
					widget.ActiveState = value1 ? Epsitec.Common.Widgets.WidgetState.ActiveYes : Epsitec.Common.Widgets.WidgetState.ActiveNo;
				}
			}
			
			private Epsitec.Common.Widgets.Widget	widget;
			private DataLayer.DataSet				data_set;
			private string							binding;
		}
	}
}
