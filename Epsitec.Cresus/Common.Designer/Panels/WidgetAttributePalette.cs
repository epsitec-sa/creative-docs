//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Panels
{
	/// <summary>
	/// La classe WidgetAttributePalette offre l'accès aux propriétés d'un
	/// widget.
	/// </summary>
	public class WidgetAttributePalette : AbstractPalette
	{
		public WidgetAttributePalette(Application application)
		{
			this.size  = new Drawing.Size (250, 600);
			this.props = new System.Collections.ArrayList ();
			
			this.application = application;
		}
		
		
		public object					ActiveObject
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
					
					this.RebindContents ();
				}
			}
		}
		
		
		public void ActivateEditor(Widget widget, bool restart_edition)
		{
			if (this.active == null)
			{
				//	Aucun widget actif => probablement plusieurs widgets sélectionnés. On ne
				//	fait donc rien de plus ici.

				return;
			}
			
			System.Diagnostics.Debug.Assert (this.active == widget);
			System.Diagnostics.Debug.WriteLine ("Activated widget " + widget.ToString ());
			
			if (this.widget != null)
			{
				Window window = this.widget.Window;
				
				if (window != null)
				{
					this.widget.Window.MakeActive ();
					
					if (restart_edition)
					{
						if (this.book != null)
						{
							if (this.book.ActivePage != null)
							{
								this.book.ActivePage.SetFocusOnTabWidget ();
							}
						}
					}
				}
			}
		}
		
		
		protected override void CreateWidgets(Widget parent)
		{
			System.Diagnostics.Debug.Assert (this.widget == parent);
			
			parent.SuspendLayout ();
			
			this.title   = new StaticText (parent);
			this.surface = new Scrollable (parent);
			this.book    = new TabBook (this.surface);
			
			this.title.Height    = 50;
			this.title.Dock      = DockStyle.Top;
			this.title.Text      = "<br/>";
			this.title.Alignment = Drawing.ContentAlignment.MiddleCenter;
			
			this.surface.Dock = DockStyle.Fill;
			this.book.Dock    = DockStyle.Fill;
			
			this.RebindContents ();
			
			parent.ResumeLayout ();
		}
		
		
		protected void RebindContents()
		{
			if (this.active == null)
			{
				this.title.Text = @"<font size=""120%""><b>Object Attributes</b><br/>(no selected object)</font>";
				this.props.Clear ();
				this.book.Items.Clear ();
			}
			else
			{
				string         name           = this.type.Name;
				IBundleSupport bundle_support = this.active as IBundleSupport;
				
				if (bundle_support != null)
				{
					name = bundle_support.PublicClassName;
				}
				
				this.title.Text = string.Format (@"<font size=""120%""><b>Object Attributes</b><br/><i>{0}</i></font>", name);
				
				//	Il faut recréer les pages permettant d'éditer les diverses propriétés.
				
				this.CreateProps ();
			}
		}
		
		protected void CreateProps()
		{
			System.Diagnostics.Debug.Assert (this.type != null);
			
			int         active_index    = this.book.ActivePageIndex;
			System.Type active_tab_type = (active_index < 0) ? null : this.props[active_index].GetType ();
			
			foreach (Editors.AbstractPropEdit prop in this.props)
			{
				prop.ActiveObject = null;
			}
			
			this.SelectMatchingProps ();
			
			int i = 0;
			
			while (i < this.props.Count)
			{
				Editors.AbstractPropEdit prop = this.props[i] as Editors.AbstractPropEdit;
				
				if (prop.ActiveObject == null)
				{
					this.props.RemoveAt (i);
				}
				else
				{
					i++;
				}
			}
			
			Editors.AbstractPropEdit[] props = new Editors.AbstractPropEdit[this.props.Count];
			this.props.CopyTo (props);
			System.Array.Sort (props, Editors.AbstractPropEdit.RankComparer);
			
			this.props.Clear ();
			this.props.AddRange (props);
			
			string focused_property_name = null;
			
			if (this.book.ActivePage != null)
			{
				Widget focused = this.book.ActivePage.FindFocusedChild ();
				
				if (focused != null)
				{
					focused_property_name = focused.FullPathName;
				}
			}
			
			
			this.book.Items.Clear ();
			
			active_index = -1;
			i = 0;
			
			foreach (Editors.AbstractPropEdit prop in this.props)
			{
				this.book.Items.Add (prop.TabPage);
				
				if (prop.GetType () == active_tab_type)
				{
					active_index = i;
				}
				
				i++;
			}
			
			if ((active_index < 0) &&
				(this.book.Items.Count > 0))
			{
				active_index = 0;
			}
			
			this.book.ActivePageIndex = active_index;
			
			//	Lors du changement du contenu des onglets, on aimerait conserver le focus clavier
			//	sur le widget précédent, si cela est possible. Pour ce faire, on a pris note plus
			//	haut du chemin d'accès au widget qui avait le focus, et on tente de trouver quelque
			//	chose de correspondant ici :
			
			if ((focused_property_name != null) &&
				(this.book.ActivePage != null))
			{
				Widget to_be_focused = this.book.RootParent.FindChildByPath (focused_property_name);
				
				if (to_be_focused != null)
				{
					to_be_focused.Focus ();
				}
			}
		}
		
		protected void SelectMatchingProps()
		{
			System.Type[] types = Editors.AbstractPropEdit.FindMatching (this.type);
			
			for (int i = 0; i < types.Length; i++)
			{
				this.CreateOrReuseProp (types[i]);
			}
		}
		
		protected void CreateOrReuseProp(System.Type prop_type)
		{
			foreach (Editors.AbstractPropEdit prop in this.props)
			{
				if (prop.GetType () == prop_type)
				{
					prop.ActiveObject = this.active;
					return;
				}
			}
			
			//	Pas trouvé de propriété correspondante. Il faut donc en créer une nouvelle !
			
			object[] args = new object[1];
			
			args[0] = this.application;
			
			Editors.AbstractPropEdit new_prop = System.Activator.CreateInstance (prop_type, args) as Editors.AbstractPropEdit;
			
			new_prop.ActiveObject = this.active;
			
			this.props.Add (new_prop);
		}
		
		
		
		
		protected StaticText					title;
		protected Scrollable					surface;
		protected object						active;
		protected System.Type					type;
		protected TabBook						book;
		protected System.Collections.ArrayList	props;
		protected Application					application;
	}
}
