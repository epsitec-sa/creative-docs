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
	public abstract class AbstractPropEdit : System.IDisposable
	{
		public AbstractPropEdit(Application application)
		{
			this.application = application;
		}
		
		
		public Application						Application
		{
			get
			{
				return this.application;
			}
		}
		
		public System.Object					ActiveObject
		{
			get
			{
				return this.active_object;
			}
			set
			{
				if (this.active_object != value)
				{
					this.active_object = value;
					
					this.UpdateContents ();
				}
			}
		}
		
		public WidgetEditor						ActiveEditor
		{
			get
			{
				return this.active_editor;
			}
			set
			{
				this.active_editor = value;
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
				return new InternalRankComparer ();
			}
		}
		
		public Common.UI.Widgets.PropPane[]		PropPanes
		{
			get
			{
				if (this.prop_panes != null)
				{
					Common.UI.Widgets.PropPane[] value = new Common.UI.Widgets.PropPane[this.prop_panes.Count];
					this.prop_panes.CopyTo (value);
					return value;
				}
				
				return new Common.UI.Widgets.PropPane[0];
			}
		}
		
		
		public static System.Type[] FindMatchingPropertyEditors(System.Type type)
		{
			//	Analyse le type passé en entrée et retourne une liste avec tous les
			//	types d'éditeurs de propriétés adéquats.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if (type == typeof (WindowRoot))
			{
				list.Add (typeof (WindowPropEdit1));
			}
			else if (type == typeof (Common.UI.Widgets.DataWidget))
			{
				list.Add (typeof (DataWidgetPropEdit1));
			}
			else if (type.IsSubclassOf (typeof (Widget)))
			{
				list.Add (typeof (WidgetPropEdit1));
			}
			
			System.Type[] types = new System.Type[list.Count];
			list.CopyTo (types);
			
			return types;
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.prop_panes != null)
				{
					Widget[] panes = new Widget[this.prop_panes.Count];
					this.prop_panes.CopyTo (panes, 0);
					
					for (int i = 0; i < panes.Length; i++)
					{
						panes[i].Dispose ();
					}
					
					this.prop_panes = null;
				}
				
				if (this.binders != null)
				{
					Common.UI.Binders.PropertyBinder[] binders = new Common.UI.Binders.PropertyBinder[this.binders.Count];
					this.binders.Values.CopyTo (binders, 0);
					
					for (int i = 0; i < binders.Length; i++)
					{
						binders[i].Source = null;
					}
					
					this.binders = null;
				}
			}
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
				(this.active_object == null))
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
				binder.Source = this.active_object;
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
				binder.PropertyValueChanged += new EventHandler(this.HandleBinderPropertyValueChanged);
				this.binders[name] = binder;
			}
			
			binder.Adapter = adapter;
			adapter.Binder = binder;
			
			return binder;
		}
		
		
		private void HandleBinderPropertyValueChanged(object sender)
		{
			Common.UI.Binders.PropertyBinder binder = sender as Common.UI.Binders.PropertyBinder;
			
			System.Diagnostics.Debug.Assert (binder != null);
			
			if (this.active_editor != null)
			{
				this.active_editor.NotifyWidgetModified (binder.Source as Widget);
			}
			
#if DEBUG
			object data;
			binder.ReadData (out data);
			System.Diagnostics.Debug.WriteLine (string.Format ("New value of property {0} is {1}", binder.Caption, data));
#endif
		}
		
		
		#region InternalRankComparer Class
		private class InternalRankComparer : IComparer
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
				
				int rx = prop_x.class_rank;
				int ry = prop_y.class_rank;
				
				return rx - ry;
			}
			#endregion
		}
		#endregion
		
		protected int							class_rank;			//	rang servant au tri des variantes de XyzPropEdit
		
		private System.Object					active_object;		//	object actuellement en cours d'édition
		private WidgetEditor					active_editor;		//	éditeur associé à l'objet actif
		
		private TabPage							page;				//	page à onglet contenant les panneaux des propriétés
		
		private System.Collections.ArrayList	prop_panes;
		private System.Collections.Hashtable	binders;
		private Application						application;
	}
}
