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
			Adapters.DecimalAdapter adapter = this.Adapter as Adapters.DecimalAdapter;
			
			if ((adapter != null) &&
				(this.widget != null))
			{
				long flags = (long) adapter.Value;
				long bit   = (long) this.widget.Index;
				
				this.widget.ActiveState = ((flags & bit) != 0) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.DecimalAdapter adapter = this.Adapter as Adapters.DecimalAdapter;
			
			if ((adapter != null) &&
				(this.widget != null))
			{
				long flags = (long) adapter.Value;
				long bit   = (long) this.widget.Index;
				
				if (this.widget.IsActive)
				{
					flags |= bit;
				}
				else
				{
					flags &= ~ bit;
				}
				
				adapter.Value = flags;
			}
		}
		
		
		private void HandleStateChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.widget);
			this.OnUIDataChanged ();
		}
		
		
		private Widget							widget;
		private Types.INamedType				data_type;
	}
}
