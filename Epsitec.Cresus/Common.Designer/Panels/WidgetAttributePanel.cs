//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Panels
{
	/// <summary>
	/// La classe WidgetAttributePalette offre l'accès aux propriétés d'un
	/// widget, lesquelles sont gérées par les classes XyzPropEdit..
	/// </summary>
	public class WidgetAttributePanel : AbstractPanel
	{
		public WidgetAttributePanel(Application application)
		{
			this.size  = new Drawing.Size (250, 600);
			this.props = new System.Collections.ArrayList ();
			
			this.saved_views = new System.Collections.Hashtable ();
			this.saved_focus = null;
			
			this.application = application;
		}
		
		
		public object							ActiveObject
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
					this.active_type   = value == null ? null : this.active_object.GetType ();
					
					System.Diagnostics.Debug.Assert ((this.active_object == null) || (this.active_editor != null));
					
					this.RebindContents ();
				}
			}
		}
		
		public Editors.WidgetEditor				ActiveEditor
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
		
		
		internal void NotifyActiveEditionWidgetChanged(Widget widget, bool restart_edition)
		{
			if (this.active_object == null)
			{
				//	Aucun widget actif => probablement plusieurs widgets sélectionnés. On ne
				//	fait donc rien de plus ici.

				return;
			}
			
			System.Diagnostics.Debug.Assert (this.active_object == widget);
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
		
		internal void NotifyActiveEditorChanged(Editors.WidgetEditor editor)
		{
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.ActiveObject = null;
			}
			
			base.Dispose (disposing);
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
		
		
		private void RebindContents()
		{
			if (this.active_object == null)
			{
				//	TODO: L10n
				
				this.title.Text = @"<font size=""120%""><b>Object Attributes</b><br/>(no selected object)</font>";
				
				this.DetachAllProperties ();
				this.DisposeUnusedProperties ();
				
				System.Diagnostics.Debug.Assert (this.props.Count == 0);
				
				this.book.Items.Clear ();
			}
			else
			{
				string         name           = this.active_type.Name;
				IBundleSupport bundle_support = this.active_object as IBundleSupport;
				
				if (bundle_support != null)
				{
					name = bundle_support.PublicClassName;
				}
				
				this.title.Text = string.Format (@"<font size=""120%""><b>Object Attributes</b><br/><i>{0}</i></font>", name);
				
				//	Il faut recréer les pages permettant d'éditer les diverses propriétés.
				
				this.UpdateProperties ();
			}
		}
		
		private void DetachAllProperties()
		{
			foreach (Editors.AbstractPropEdit prop in this.props)
			{
				prop.ActiveObject = null;
				prop.ActiveEditor = null;
			}
		}
		
		private void DisposeUnusedProperties()
		{
			int i = 0;
			
			while (i < this.props.Count)
			{
				Editors.AbstractPropEdit prop = this.props[i] as Editors.AbstractPropEdit;
				
				if (prop.ActiveObject == null)
				{
					this.props.RemoveAt (i);
					prop.Dispose ();
				}
				else
				{
					i++;
				}
			}
		}
		
		
		private void UpdateProperties()
		{
			//	Re-crée les propriétés liées à l'objet actif, si possible en conservant les réglages
			//	actifs précédemment si on ne change pas de type d'objet (le focus est aussi conservé
			//	dans la mesure du possible).
			
			System.Diagnostics.Debug.Assert (this.active_type != null);
			
			int         active_index    = this.book.ActivePageIndex;
			System.Type active_tab_type = (active_index < 0) ? null : this.props[active_index].GetType ();
			
			this.SaveVisiblePropViews ();
			this.SaveFocus ();
			
			this.DetachAllProperties ();
			this.SelectMatchingProperties ();
			this.DisposeUnusedProperties ();
			
			//	Met à jour la liste des propriétés à utiliser pour l'édition de l'objet actif :
			
			Editors.AbstractPropEdit[] props = new Editors.AbstractPropEdit[this.props.Count];
			this.props.CopyTo (props);
			System.Array.Sort (props, Editors.AbstractPropEdit.RankComparer);
			
			this.props.Clear ();
			this.props.AddRange (props);
			
			//	Rafraîchit le contenu des divers onglets en s'appuyant sur les éditeurs de
			//	propriétés sélectionnés :
			
			this.UpdateBookPages (active_tab_type);
			
			this.RestoreVisiblePropViews ();
			this.RestoreFocus ();
		}

		private void UpdateBookPages(System.Type active_tab_type)
		{
			this.book.Items.Clear ();
			
			int active_index = -1;
			int i = 0;
			
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
		}
		
		
		private void SaveFocus()
		{
			if (this.book.ActivePage != null)
			{
				Widget focused = this.book.ActivePage.FindFocusedChild ();
				
				if (focused != null)
				{
					this.saved_focus = focused.FullPathName;
				}
			}
		}
		
		private void SaveVisiblePropViews()
		{
			foreach (Editors.AbstractPropEdit prop in this.props)
			{
				int[] views = new int[prop.PropPanes.Length];
				
				for (int i = 0; i < prop.PropPanes.Length; i++)
				{
					views[i] = prop.PropPanes[i].VisibleViewIndex;
				}
				
				this.saved_views[prop.GetType ()] = views;
			}
		}
		
		
		private void RestoreFocus()
		{
			//	Lors du changement du contenu des onglets, on aimerait conserver le focus clavier
			//	sur le widget précédent, si cela est possible. Pour ce faire, on a pris note plus
			//	haut du chemin d'accès au widget qui avait le focus, et on tente de trouver quelque
			//	chose de correspondant ici :
			
			if ((this.saved_focus != null) &&
				(this.book.ActivePage != null))
			{
				Widget to_be_focused = this.book.RootParent.FindChildByPath (this.saved_focus);
				
				if (to_be_focused != null)
				{
					to_be_focused.Focus ();
				}
			}
		}
		
		private void RestoreVisiblePropViews()
		{
			foreach (Editors.AbstractPropEdit prop in this.props)
			{
				System.Array array = this.saved_views[prop.GetType ()] as System.Array;
				
				if (array != null)
				{
					for (int i = 0; i < prop.PropPanes.Length; i++)
					{
						prop.PropPanes[i].VisibleViewIndex = ((int[]) array)[i];
					}
				}
			}
		}
		
		
		private void SelectMatchingProperties()
		{
			System.Type[] types = Editors.AbstractPropEdit.FindMatchingPropertyEditors (this.active_type);
			
			//	On a obtenu la table des types d'éditeurs applicables pour l'objet actuellement
			//	actif. C'est en principe l'un des suivants :
			//
			//	- WindowPropEdit.. pour l'édition d'une fenêtre
			//	- DataWidgetPropEdit.. pour l'édition d'un widget standard
			//	- WidgetPropEdit.. pour l'édition d'un "data widget" (avec binding)
			
			for (int i = 0; i < types.Length; i++)
			{
				this.CreateOrReuseProperty (types[i]);
			}
		}
		
		private void CreateOrReuseProperty(System.Type prop_type)
		{
			foreach (Editors.AbstractPropEdit prop in this.props)
			{
				if (prop.GetType () == prop_type)
				{
					prop.ActiveEditor = this.active_editor;
					prop.ActiveObject = this.active_object;
					return;
				}
			}
			
			//	Pas trouvé de propriété correspondante. Il faut donc en créer une nouvelle !
			
			object[] args = new object[] { this.application };
			
			Editors.AbstractPropEdit new_prop = System.Activator.CreateInstance (prop_type, args) as Editors.AbstractPropEdit;
			
			new_prop.ActiveEditor = this.active_editor;
			new_prop.ActiveObject = this.active_object;
			
			this.props.Add (new_prop);
		}
		
		
		
		private StaticText						title;				//	titre dans le panneau
		private Scrollable						surface;			//	surface qui contient les onglets
		private TabBook							book;				//	onglets avec les divers attributs
		
		private object							active_object;		//	objet actif (dont les propriétés sont visibles)
		private System.Type						active_type;		//	type de l'objet actif
		private Editors.WidgetEditor			active_editor;		//	éditeur associé à l'objet actif
		
		private System.Collections.ArrayList	props;				//	propriétés
		
		private System.Collections.Hashtable	saved_views;
		private string							saved_focus;
		
		private Application						application;
	}
}
