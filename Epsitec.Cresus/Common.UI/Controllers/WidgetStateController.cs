//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// La classe WidgetStateController réalise un contrôleur très simple qui
	/// s'appuie sur un widget existant et interagit avec sa propriété ActiveState.
	/// Lorsque la widget observé est un RadioButton (avec Group/Index), alors la
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
			if (widget is RadioButton)
			{
				//	Cas particulier: le widget est un bouton radio, et on ne doit s'attacher non
				//	pas au bouton radio lui-même, mais au contrôleur du groupe de boutons :
				
				RadioButton radio = widget as RadioButton;
				
				this.widget = radio;
				this.group  = radio.Controller;
				
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
					
					foreach (RadioButton item in RadioButton.FindRadioChildren (radio.Parent, radio.Group))
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
			Adapters.DecimalAdapter adapter = this.Adapter as Adapters.DecimalAdapter;
			
			if ((adapter != null) &&
				(this.widget != null))
			{
				if (this.group != null)
				{
					this.group.ActiveIndex = (int) adapter.Value;
				}
				else
				{
					this.widget.ActiveState = (adapter.Value == 0) ? WidgetState.ActiveNo : WidgetState.ActiveYes;
				}
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.DecimalAdapter adapter = this.Adapter as Adapters.DecimalAdapter;
			
			if ((adapter != null) &&
				(this.widget != null))
			{
				if (this.group != null)
				{
					adapter.Value = this.group.ActiveIndex;
				}
				else
				{
					adapter.Value = this.widget.IsActive ? 1 : 0;
				}
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
		private RadioButton.GroupController		group;
		private Types.INamedType				data_type;
	}
}
