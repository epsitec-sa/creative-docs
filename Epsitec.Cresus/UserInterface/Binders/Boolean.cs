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
		public void CreateBinding(object ui_object, DataLayer.DataSet root, string binding, DataLayer.DataRecord data_record)
		{
			Epsitec.Common.Widgets.Widget widget = ui_object as Epsitec.Common.Widgets.Widget;
			DataLayer.DataField           field  = data_record as DataLayer.DataField;
			
			System.Diagnostics.Debug.Assert (widget != null);
			System.Diagnostics.Debug.Assert (field != null);
			
			Controller controller = new Controller (widget, root, binding);
			
			controller.SetWidgetValue ((bool) field.GetData ());
		}
		#endregion
		
		protected class Controller
		{
			public Controller(Epsitec.Common.Widgets.Widget widget, DataLayer.DataSet data_set, string binding)
			{
				this.widget   = widget;
				this.data_set = data_set;
				this.binding  = binding;
				
				this.widget.ActiveStateChanged += new Epsitec.Common.Widgets.EventHandler(this.HandleActiveStateChanged);
			}
			
			public void SetWidgetValue(bool value)
			{
				widget.ActiveState = value ? Epsitec.Common.Widgets.WidgetState.ActiveYes : Epsitec.Common.Widgets.WidgetState.ActiveNo;
				
				System.Diagnostics.Debug.WriteLine ("Widget changed to " + value.ToString ());
			}
			
			public void HandleActiveStateChanged(object sender)
			{
				bool value = (this.widget.ActiveState == Epsitec.Common.Widgets.WidgetState.ActiveYes);
				this.data_set.UpdateData (this.binding, value);
				
				System.Diagnostics.Debug.WriteLine ("Widget changed to " + value.ToString ());
			}
			
			private Epsitec.Common.Widgets.Widget	widget;
			private DataLayer.DataSet				data_set;
			private string							binding;
		}
	}
}
