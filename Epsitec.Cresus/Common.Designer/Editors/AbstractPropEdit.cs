//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Editors
{
	using IComparer = System.Collections.IComparer;
	
	/// <summary>
	/// La classe AbstractPropEdit définit les méthodes communes à toutes les
	/// classes permettant d'éditer des propriétés.
	/// </summary>
	public class AbstractPropEdit
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
			this.page.DockMargins = new Drawing.Margins (5, 5, 5, 5);
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
		
		
		protected Widget CreatePropPane(string title, double height)
		{
			Widget widget = new Ui.Widgets.PropPane ();
			
			widget.Dock   = DockStyle.Top;
			widget.Size   = new Drawing.Size (this.page.Width, height);
			widget.Parent = this.page;
			
			return widget;
		}
		
		protected virtual void FillTabPage()
		{
		}
		
		protected virtual void LoadContents()
		{
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
	}
}
