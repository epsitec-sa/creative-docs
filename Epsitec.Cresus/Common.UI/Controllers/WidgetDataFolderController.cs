//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	using IStringCollectionHost = Common.Widgets.Helpers.IStringCollectionHost;
	using IStringSelection      = Common.Support.Data.IStringSelection;
	
	/// <summary>
	/// La classe WidgetDataFolderController réalise un contrôleur très simple qui
	/// s'appuie sur un widget existant et interagit avec son contenu (liste).
	/// </summary>
	public class WidgetDataFolderController : AbstractConstrainedController
	{
		public WidgetDataFolderController()
		{
		}
		
		public WidgetDataFolderController(Adapters.IAdapter adapter) : this ()
		{
			this.Adapter = adapter;
		}
		
		public WidgetDataFolderController(Adapters.IAdapter adapter, Widget widget) : this ()
		{
			this.Adapter = adapter;
			this.CreateUI (widget);
		}
		
		public WidgetDataFolderController(Adapters.IAdapter adapter, Widget widget, Types.IDataConstraint constraint) : this ()
		{
			this.Adapter    = adapter;
			this.Constraint = constraint;
			this.CreateUI (widget);
		}
		
		
		public override void CreateUI(Widget widget)
		{
			IStringCollectionHost string_list = widget as IStringCollectionHost;
			IStringSelection      string_sel  = widget as IStringSelection;
			
			if (string_list == null)
			{
				throw new System.ArgumentException ("Widget does not support IStringCollectionHost interface.", "widget");
			}
			
			if (string_sel == null)
			{
				throw new System.ArgumentException ("Widget does not support IStringSelection interface.", "widget");
			}
			
			this.widget      = widget;
			this.string_list = string_list;
			this.string_sel  = string_sel;
			
			this.SyncFromAdapter (SyncReason.Initialisation);
		}
		
		public override void SyncFromAdapter(SyncReason reason)
		{
			Adapters.DataFolderAdapter adapter = this.Adapter as Adapters.DataFolderAdapter;
			
			if ((adapter != null) &&
				(this.widget != null))
			{
				Types.IDataItem[] items = adapter.Value;
				
				int index = this.string_sel.SelectedIndex;
				
				this.string_list.Items.Clear ();
				
				foreach (Types.IDataItem item in items)
				{
					string name    = item.Name;
					string caption = item.Caption;
					
					if (caption == null)
					{
						caption = name;
					}
					
					this.string_list.Items.Add (name, caption);
				}
				
				this.string_sel.SelectedIndex = index;
			}
		}
		
		public override void SyncFromUI()
		{
			Adapters.DataFolderAdapter adapter = this.Adapter as Adapters.DataFolderAdapter;
			
			if ((adapter != null) &&
				(this.widget != null))
			{
				//	TODO: ...
			}
		}
		
		
		private Widget							widget;
		private IStringCollectionHost			string_list;
		private IStringSelection				string_sel;
	}
}
