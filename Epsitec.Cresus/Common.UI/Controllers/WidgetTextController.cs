//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// La classe WidgetTextController réalise un contrôleur très simple qui
	/// s'appuie sur un widget existant et interagit avec sa propriété Text.
	/// </summary>
	public class WidgetTextController : AbstractController
	{
		public WidgetTextController()
		{
		}
		
		public WidgetTextController(Adapters.IAdapter adapter) : this ()
		{
			this.Adapter = adapter;
		}
		
		public WidgetTextController(Adapters.IAdapter adapter, Widget widget) : this ()
		{
			this.Adapter = adapter;
			this.CreateUI (widget);
		}
		
		
		public override void CreateUI(Widget widget)
		{
			this.widget = widget;
			this.widget.TextChanged += new EventHandler (this.HandleWidgetTextChanged);
			this.SyncFromAdapter ();
		}
		
		public override void SyncFromAdapter()
		{
			Adapters.StringAdapter adapter = this.Adapter as Adapters.StringAdapter;
			
			if ((adapter != null) &&
				(this.widget != null))
			{
				this.widget.Text = adapter.Value;
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.StringAdapter adapter = this.Adapter as Adapters.StringAdapter;
			
			if ((adapter != null) &&
				(this.widget != null))
			{
				adapter.Value = this.widget.Text;
			}
		}
		
		
		private void HandleWidgetTextChanged(object sender)
		{
			this.SyncFromUI ();
		}
		
		
		private Widget							widget;
	}
}
