//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Editors
{
	using IComparer = System.Collections.IComparer;
	
	/// <summary>
	/// La classe AbstractPropEdit définit les méthodes communes à toutes les
	/// classes permettant d'éditer des propriétés.
	/// </summary>
	public abstract class AbstractPropEdit
	{
		public AbstractPropEdit()
		{
		}
		
		
		public System.Object					ActiveObject
		{
			get
			{
				return this.active;
			}
			set
			{
				if (this.active != value)
				{
					this.active = value;
					this.type   = value == null ? null : this.active.GetType ();
					
					this.UpdateContents ();
				}
			}
		}
		
		public TabPage							TabPage
		{
			get
			{
				if (this.page == null)
				{
					this.CreateTabPage ();
				}
				
				return this.page;
			}
		}
		
		public static IComparer					RankComparer
		{
			get
			{
				return new RankComparerClass ();
			}
		}
		
		
		public static System.Type[] FindMatching(System.Type type)
		{
			//	Analyse le type passé en entrée et retourne une liste avec tous les
			//	types d'éditeurs de propriétés adéquats.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if (type.IsSubclassOf (typeof (Widget)))
			{
				list.Add (typeof (WidgetPropEdit1));
			}
			
			System.Type[] types = new System.Type[list.Count];
			list.CopyTo (types);
			
			return types;
		}
		
		
		protected void CreateTabPage()
		{
			this.page = new TabPage ();
			this.page.DockPadding = new Drawing.Margins (5, 5, 5, 5);
			this.page.PreferHorizontalDockLayout = false;
			
			this.FillTabPage ();
			this.UpdateContents ();
		}
		
		protected void UpdateContents()
		{
			if ((this.page == null) ||
				(this.active == null))
			{
				return;
			}
			
			this.LoadContents ();
		}
		
		
		protected Widget CreatePropPane(string property_name, Common.UI.Adapters.IAdapter adapter)
		{
			Common.UI.Widgets.PropPane       pane   = new Common.UI.Widgets.PropPane ();
			Common.UI.Binders.PropertyBinder binder = this.GetBinder (property_name, adapter);
			
			pane.Dock   = DockStyle.Top;
			pane.Width  = this.page.Width;
			pane.Parent = this.page;
			pane.Name   = property_name;
			
			pane.Attach (adapter);
			
			return pane;
		}
		
		protected void AddPropPane(Widget pane)
		{
			if (this.prop_panes == null)
			{
				this.prop_panes = new System.Collections.ArrayList ();
			}
			
			this.prop_panes.Add (pane);
			
			pane.TabIndex      = this.prop_panes.Count;
			pane.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
		}
		
		protected abstract void FillTabPage();
		
		protected virtual void LoadContents()
		{
			foreach (Common.UI.Binders.PropertyBinder binder in this.binders.Values)
			{
				binder.Source = this.active;
			}
		}
		
		protected Common.UI.Binders.PropertyBinder GetBinder(string name, Common.UI.Adapters.IAdapter adapter)
		{
			if (this.binders == null)
			{
				this.binders = new System.Collections.Hashtable ();
			}
			
			Common.UI.Binders.PropertyBinder binder = this.binders[name] as Common.UI.Binders.PropertyBinder;
			
			if (binder == null)
			{
				binder = new Common.UI.Binders.PropertyBinder (name);
				this.binders[name] = binder;
			}
			
			binder.Adapter = adapter;
			adapter.Binder = binder;
			
			return binder;
		}
		
		
		private class RankComparerClass : IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				AbstractPropEdit prop_x = x as AbstractPropEdit;
				AbstractPropEdit prop_y = y as AbstractPropEdit;

				if (prop_x == prop_y)
				{
					return 0;
				}
				
				if (prop_x == null)
				{
					return -1;
				}
				if (prop_y == null)
				{
					return 1;
				}
				
				int rx = prop_x.rank;
				int ry = prop_y.rank;
				
				return rx - ry;
			}
			#endregion
		}
		
		
		
		protected System.Object					active;
		protected System.Type					type;
		protected int							rank;
		protected TabPage						page;
		
		protected System.Collections.ArrayList	prop_panes;
		protected System.Collections.Hashtable	binders;
	}
}
