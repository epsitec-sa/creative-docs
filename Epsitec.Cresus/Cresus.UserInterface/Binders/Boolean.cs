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
		public void CreateBinding(object ui_object, DataLayer.DataStore root, string path, string args, Database.DbColumn db_column)
		{
			System.Diagnostics.Debug.WriteLine ("Boolean Binder, args="+args);
			
			Epsitec.Common.Widgets.Widget widget = ui_object as Epsitec.Common.Widgets.Widget;
			
			System.Diagnostics.Debug.Assert (widget != null);
			System.Diagnostics.Debug.Assert (db_column != null);
			
			new Controller (widget, root, path);
		}
		#endregion
		
		protected class Controller
		{
			public Controller(Epsitec.Common.Widgets.Widget widget, DataLayer.DataStore data_store, string path)
			{
				this.widget     = widget;
				this.data_store = data_store;
				this.binding    = path;
				
				this.widget.ActiveStateChanged += new Epsitec.Common.Support.EventHandler(this.SetDataFromWidget);
				data_store.AttachObserver (binding, new DataLayer.DataChangeEventHandler (this.SetWidgetFromData));
				
				this.SetWidgetFromData (null, new DataLayer.DataChangeEventArgs (path, System.Data.DataRowAction.Nothing));
			}
			
			public void SetWidgetValue(bool value)
			{
				widget.ActiveState = value ? Epsitec.Common.Widgets.WidgetState.ActiveYes : Epsitec.Common.Widgets.WidgetState.ActiveNo;
				
				System.Diagnostics.Debug.WriteLine ("Setting widget to " + value.ToString ());
			}
			
			public void SetDataFromWidget(object sender)
			{
				bool value1 = (this.widget.ActiveState == Epsitec.Common.Widgets.WidgetState.ActiveYes);
				bool value2 = (bool) this.data_store[this.binding];
				
				if (value1 != value2)
				{
					System.Diagnostics.Debug.WriteLine ("Widget changed to " + value1.ToString () + ", updating data");
					this.data_store[this.binding] = value1;
				}
			}
			
			public void SetWidgetFromData(object sender, DataLayer.DataChangeEventArgs e)
			{
				bool value1 = (bool) this.data_store[this.binding];
				bool value2 = (this.widget.ActiveState == Epsitec.Common.Widgets.WidgetState.ActiveYes);
				
				if (value1 != value2)
				{
					System.Diagnostics.Debug.WriteLine ("Data changed to " + value1.ToString () + ", updating widget");
					widget.ActiveState = value1 ? Epsitec.Common.Widgets.WidgetState.ActiveYes : Epsitec.Common.Widgets.WidgetState.ActiveNo;
				}
			}
			
			private Epsitec.Common.Widgets.Widget	widget;
			private DataLayer.DataStore				data_store;
			private string							binding;
		}
	}
}
