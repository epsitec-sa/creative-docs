//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// La classe WidgetValueController réalise un contrôleur très simple qui
	/// s'appuie sur un widget existant et interagit avec sa propriété Value.
	/// </summary>
	public class WidgetValueController : AbstractConstrainedController
	{
		public WidgetValueController()
		{
		}
		
		public WidgetValueController(Adapters.IAdapter adapter) : this ()
		{
			this.Adapter = adapter;
		}
		
		public WidgetValueController(Adapters.IAdapter adapter, Widget widget) : this ()
		{
			this.Adapter = adapter;
			this.CreateUI (widget);
		}
		
		public WidgetValueController(Adapters.IAdapter adapter, Widget widget, Types.IDataConstraint constraint, Types.INum num_type) : this ()
		{
			this.Adapter    = adapter;
			this.Constraint = constraint;
			this.NumType    = num_type;
			this.CreateUI (widget);
		}
		
		
		public Types.INum						NumType
		{
			get
			{
				return this.num_type;
			}
			set
			{
				this.num_type = value;
			}
		}
		
		
		public override void CreateUI(Widget widget)
		{
			Support.Data.INumValue num_value = widget as Support.Data.INumValue;
			
			if (num_value == null)
			{
				throw new System.ArgumentException ("The specified widget does not conform to INumValue interface.", "widget");
			}
			
			this.widget    = widget;
			this.num_value = num_value;
			
			if (this.num_type != null)
			{
				this.num_value.MinValue   = this.NumType.Range.Minimum;
				this.num_value.MaxValue   = this.NumType.Range.Maximum;
				this.num_value.Resolution = this.NumType.Range.Resolution;
			}
			
			this.num_value.ValueChanged += new EventHandler (this.HandleWidgetValueChanged);
			
			this.SyncFromAdapter (SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
			Adapters.DecimalAdapter adapter = this.Adapter as Adapters.DecimalAdapter;
			
			if ((adapter != null) &&
				(this.num_value != null))
			{
				this.num_value.Value = adapter.Value;
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.DecimalAdapter adapter = this.Adapter as Adapters.DecimalAdapter;
			
			if ((adapter != null) &&
				(this.num_value != null))
			{
				decimal value = this.num_value.Value;
				
				if ((this.num_value.IsValid) &&
					(this.CheckConstraint (value)))
				{
					adapter.Value    = value;
					adapter.Validity = true;
				}
				else
				{
					adapter.Validity = false;
				}
			}
		}
		
		
		private void HandleWidgetValueChanged(object sender)
		{
			this.OnUIDataChanged ();
		}
		
		
		private Widget							widget;
		private Support.Data.INumValue			num_value;
		private Types.INum						num_type;
	}
}
