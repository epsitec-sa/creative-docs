//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// La classe WidgetFlagController réalise un contrôleur très simple qui
	/// s'appuie sur un widget existant et interagit avec sa propriété ActiveState.
	/// </summary>
	public class WidgetFlagController : AbstractConstrainedController
	{
		public WidgetFlagController()
		{
		}
		
		public WidgetFlagController(Adapters.IAdapter adapter) : this ()
		{
			this.Adapter = adapter;
		}
		
		public WidgetFlagController(Adapters.IAdapter adapter, Widget widget, string caption, Types.INamedType type) : this ()
		{
			this.caption  = caption;
			this.Adapter  = adapter;
			this.DataType = type;
			
			this.CreateUI (widget);
		}
		
		
		public Types.INamedType					DataType
		{
			get
			{
				return this.data_type;
			}
			set
			{
				this.data_type = value;
			}
		}
		
		
		public override void CreateUI(Widget widget)
		{
			this.widget = widget;
			
			if ((this.widget.Text == "") &&
				(this.caption != null))
			{
				this.widget.Text = this.caption;
			}
			
			this.widget.ActiveStateChanged += new EventHandler (this.HandleStateChanged);
			
			this.SyncFromAdapter (SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
			Adapters.StringAdapter adapter = this.Adapter as Adapters.StringAdapter;
			
			if ((adapter != null) &&
				(this.widget != null))
			{
				string   value  = adapter.Value;
				string[] values = value.Split (',', '|', ';');
				
				bool active = false;
				
				for (int i = 0; i < values.Length; i++)
				{
					string flag = values[i].Trim ();
					
					if (this.widget.Name == flag)
					{
						active = true;
						break;
					}
				}
				
				this.widget.ActiveState = active ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.StringAdapter adapter = this.Adapter as Adapters.StringAdapter;
			
			if ((adapter != null) &&
				(this.widget != null))
			{
				string   value  = adapter.Value;
				string[] values = value.Split (',', '|', ';');
				
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				for (int i = 0; i < values.Length; i++)
				{
					string flag = values[i].Trim ();
					
					if ((flag != "") &&
						(this.widget.Name != flag))
					{
						list.Add (flag);
					}
				}
				
				if (this.widget.IsActive)
				{
					list.Add (this.widget.Name);
				}
				
				values = new string[list.Count];
				list.CopyTo (values);
				
				//	TODO: ...
				
				adapter.Value = (values.Length == 0) ? "0" : string.Join (", ", values);
			}
		}
		
		
		private void HandleStateChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.widget);
			this.SyncFromUI ();
		}
		
		private void HandleGroupChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.group);
			this.SyncFromUI ();
		}
		
		
		private Widget							widget;
		private RadioButton.GroupController		group;
		private Types.INamedType				data_type;
	}
}
