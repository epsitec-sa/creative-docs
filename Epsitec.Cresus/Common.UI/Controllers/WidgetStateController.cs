//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA, 28/04/2004

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// La classe WidgetStateController r�alise un contr�leur tr�s simple qui
	/// s'appuie sur un widget existant et interagit avec sa propri�t� ActiveState.
	/// Lorsque la widget observ� est un RadioButton (avec Group/Index), alors la
	/// gestion est un peu plus subtile, en ce sens qu'elle tient compte du groupe
	/// complet et d�termine ainsi une valeur num�rique, pas un bool�en.
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
		
		public WidgetStateController(Adapters.IAdapter adapter, Widget widget) : this ()
		{
			this.Adapter = adapter;
			this.CreateUI (widget);
		}
		
		
		public override void CreateUI(Widget widget)
		{
			if (widget is RadioButton)
			{
				//	Cas particulier: le widget est un bouton radio, et on ne doit s'attacher non
				//	pas au bouton radio lui-m�me, mais au contr�leur du groupe de boutons :
				
				RadioButton radio = widget as RadioButton;
				
				this.widget = radio;
				this.group  = radio.Controller;
				
				if (this.group == null)
				{
					throw new System.ArgumentException ("Specified widget (RadioButton) is not properly set up.", "widget");
				}
				
				this.group.Changed += new EventHandler (this.HandleGroupChanged);
			}
			else
			{
				this.widget = widget;
				this.widget.ActiveStateChanged += new EventHandler (this.HandleStateChanged);
			}
			
			this.SyncFromAdapter ();
		}
		
		public override void SyncFromAdapter()
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
			this.SyncFromUI ();
		}
		
		private void HandleGroupChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (sender == this.group);
			this.SyncFromUI ();
		}
		
		
		private Widget							widget;
		private RadioButton.GroupController		group;
	}
}
