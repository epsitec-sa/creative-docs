//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	using GroupController = Epsitec.Common.Widgets.Helpers.GroupController;
	
	/// <summary>
	/// La classe WidgetStateController réalise un contrôleur très simple qui
	/// s'appuie sur un widget existant et interagit avec sa propriété ActiveState.
	/// Lorsque la widget observé est un widget 'radio' (avec Group/Index), alors la
	/// gestion est un peu plus subtile, en ce sens qu'elle tient compte du groupe
	/// complet et détermine ainsi une valeur numérique, pas un booléen.
	/// </summary>
	public class WidgetStateController : AbstractConstrainedController
	{
		public WidgetStateController()
		{
		}
		
		public WidgetStateController(Adapters.IAdapter adapter) : this ()
		{
			this.Adapter = adapter;
		}
		
		public WidgetStateController(Adapters.IAdapter adapter, Widget widget, string caption, Types.INamedType type) : this ()
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
			if (widget.AutoRadio)
			{
				//	Cas particulier: le widget est un bouton radio, et on ne doit s'attacher non
				//	pas au bouton radio lui-même, mais au contrôleur du groupe de boutons :
				
				this.widget = widget;
				this.group  = GroupController.GetGroupController (widget);
				
				if (this.group == null)
				{
					throw new System.ArgumentException ("Specified widget (RadioButton) is not properly set up.", "widget");
				}
				
				if (this.data_type is Types.IEnum)
				{
					Types.IEnum        enum_type   = this.data_type as Types.IEnum;
					Types.IEnumValue[] enum_values = enum_type.Values;
					
					//	C'est une énumération pour laquelle nous connaissons peut-être les légendes des
					//	divers éléments. Dans ce cas, on va renommer les boutons radio qui correspondent :
					
					foreach (RadioButton item in this.group.FindWidgets ())
					{
						int index = item.Index;
						
						for (int i = 0; i < enum_values.Length; i++)
						{
							if (enum_values[i].Rank == index)
							{
								if (enum_values[i].Caption != null)
								{
									item.Text = enum_values[i].Caption;
								}
								
								break;
							}
						}
					}
				}
				
				this.group.Changed += new EventHandler (this.HandleGroupChanged);
			}
			else
			{
				this.widget = widget;
				
				if ((this.widget.Text == "") &&
					(this.caption != null))
				{
					this.widget.Text = this.caption;
				}
				
				this.widget.ActiveStateChanged += new EventHandler (this.HandleStateChanged);
			}
			
			this.SyncFromAdapter (SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
			Adapters.DecimalAdapter dec_adapter  = this.Adapter as Adapters.DecimalAdapter;
			Adapters.BooleanAdapter bool_adapter = this.Adapter as Adapters.BooleanAdapter;
			
			if ((dec_adapter != null) &&
				(this.widget != null))
			{
				if (this.group != null)
				{
					this.group.ActiveIndex = (int) dec_adapter.Value;
				}
				else
				{
					this.widget.ActiveState = (dec_adapter.Value == 0) ? WidgetState.ActiveNo : WidgetState.ActiveYes;
				}
			}
			if ((bool_adapter != null) &&
				(this.widget != null))
			{
				this.widget.ActiveState = (bool_adapter.Value) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.DecimalAdapter dec_adapter  = this.Adapter as Adapters.DecimalAdapter;
			Adapters.BooleanAdapter bool_adapter = this.Adapter as Adapters.BooleanAdapter;
			
			if ((dec_adapter != null) &&
				(this.widget != null))
			{
				if (this.group != null)
				{
					dec_adapter.Value = this.group.ActiveIndex;
				}
				else
				{
					dec_adapter.Value = this.widget.IsActive ? 1 : 0;
				}
			}
			if ((bool_adapter != null) &&
				(this.widget != null))
			{
				bool_adapter.Value = this.widget.IsActive;
			}
		}
		
		
		private void HandleStateChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.widget);
			this.OnUIDataChanged ();
		}
		
		private void HandleGroupChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.group);
			this.OnUIDataChanged ();
		}
		
		
		private Widget							widget;
		private GroupController					group;
		private Types.INamedType				data_type;
	}
}
