using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	public delegate void SimpleEventHandler();
	public delegate void ObjectEventHandler(Objects.Abstract obj);
	public delegate void PropertyEventHandler(System.Collections.ArrayList propertyList);
	public delegate void RedrawEventHandler(Viewer viewer, Rectangle rect);

	/// <summary>
	/// Summary description for Notifier.
	/// </summary>
	public class Notifier
	{
		public Notifier(Document document)
		{
			this.document = document;
		}


		// Etat du notificateur.
		public bool Enable
		{
			get { return this.enable; }
			set { this.enable = value; }
		}

		// Etat du notificateur.
		public bool EnableSelectionChanged
		{
			get { return this.enableSelectionChanged; }
			set { this.enableSelectionChanged = value; }
		}


		// Indique que tout a chang�.
		public void NotifyAllChanged()
		{
			if ( !this.enable )  return;

			this.documentChanged = true;
			this.mouseChanged = true;
			this.modifChanged = true;
			this.originChanged = true;
			this.zoomChanged = true;
			this.toolChanged = true;
			this.saveChanged = true;
			this.selectionChanged = true;
			this.createChanged = true;
			this.styleChanged = true;
			this.pagesChanged = true;
			this.layersChanged = true;
			this.undoRedoChanged = true;
			this.gridChanged = true;
			this.previewChanged = true;
			this.settingsChanged = true;
			this.guidesChanged = true;
			this.hideHalfChanged = true;
			this.debugChanged = true;
			this.selNamesChanged = true;
			this.NotifyArea();
		}


		// Indique que les informations sur le document ont chang�.
		// Nom du document, taille, etc.
		public void NotifyDocumentChanged()
		{
			if ( !this.enable )  return;
			this.documentChanged = true;
			this.NotifyAsync();
		}

		// Indique que la position de la souris a chang�.
		public void NotifyMouseChanged()
		{
			if ( !this.enable )  return;
			this.mouseChanged = true;
			this.NotifyAsync();
		}

		// Indique que le texte des modifications a chang�.
		public void NotifyModifChanged()
		{
			if ( !this.enable )  return;
			this.modifChanged = true;
			this.NotifyAsync();
		}

		// Indique que l'origine a chang�.
		public void NotifyOriginChanged()
		{
			if ( !this.enable )  return;
			this.originChanged = true;
			this.NotifyAsync();
		}

		// Indique que le zoom a chang�.
		public void NotifyZoomChanged()
		{
			if ( !this.enable )  return;
			this.zoomChanged = true;
			this.NotifyAsync();
		}

		// Indique que l'outil s�lectionn� a chang�.
		public void NotifyToolChanged()
		{
			if ( !this.enable )  return;
			this.toolChanged = true;
			this.NotifyAsync();
		}

		// Indique que le bouton "enregistrer" a chang�.
		public void NotifySaveChanged()
		{
			if ( !this.enable )  return;
			this.saveChanged = true;
			this.NotifyAsync();
		}

		// Indique que les objets s�lectionn�s ont chang�.
		public void NotifySelectionChanged()
		{
			if ( !this.enable || !this.enableSelectionChanged )  return;
			this.selectionChanged = true;
			this.NotifyAsync();
		}

		// Indique que la cr�ation d'un objet � d�but� ou s'est termin�e.
		public void NotifyCreateChanged()
		{
			if ( !this.enable )  return;
			this.createChanged = true;
			this.NotifyAsync();
		}

		// Indique que les styles ont chang�.
		public void NotifyStyleChanged()
		{
			if ( !this.enable )  return;
			this.styleChanged = true;
			this.NotifyAsync();
		}

		// Indique que les pages ont chang�.
		public void NotifyPagesChanged()
		{
			if ( !this.enable )  return;
			this.pagesChanged = true;

			if ( !this.document.Settings.GlobalGuides )
			{
				this.NotifyGuidesChanged();
			}

			this.NotifyAsync();
		}

		// Indique que les calques ont chang�.
		public void NotifyLayersChanged()
		{
			if ( !this.enable )  return;
			this.layersChanged = true;
			this.NotifyAsync();
		}

		// Indique qu'une page a chang�.
		public void NotifyPageChanged(Objects.Abstract page)
		{
			if ( !this.enable )  return;
			this.pageObject = page;
			this.NotifyAsync();
		}

		// Indique qu'un calque a chang�.
		public void NotifyLayerChanged(Objects.Abstract layer)
		{
			if ( !this.enable )  return;
			this.layerObject = layer;
			this.NotifyAsync();
		}

		// Indique que les commandes undo/redo ont chang�.
		public void NotifyUndoRedoChanged()
		{
			if ( !this.enable )  return;
			this.undoRedoChanged = true;
			this.NotifyAsync();
		}

		// Indique que les commandes pour la grille ont chang�.
		public void NotifyGridChanged()
		{
			if ( !this.enable )  return;
			this.gridChanged = true;
			this.NotifyAsync();
		}

		// Indique que la commande aper�u a chang�.
		public void NotifyPreviewChanged()
		{
			if ( !this.enable )  return;
			this.previewChanged = true;
			this.NotifyAsync();
		}

		// Indique que les r�glages ont chang�.
		public void NotifySettingsChanged()
		{
			if ( !this.enable )  return;
			this.settingsChanged = true;
			this.NotifyAsync();
		}

		// Indique que les rep�res ont chang�.
		public void NotifyGuidesChanged()
		{
			if ( !this.enable )  return;
			this.guidesChanged = true;
			this.NotifyAsync();
		}

		// Indique que la commande estomp� a chang�.
		public void NotifyHideHalfChanged()
		{
			if ( !this.enable )  return;
			this.hideHalfChanged = true;
			this.NotifyAsync();
		}

		// Indique que les commandes pour le debug ont chang�.
		public void NotifyDebugChanged()
		{
			if ( !this.enable )  return;
			this.debugChanged = true;
			this.NotifyAsync();
		}


		// Indique qu'une propri�t� a chang�.
		public void NotifyPropertyChanged(Properties.Abstract property)
		{
			if ( !this.enable )  return;
			if ( !this.propertyList.Contains(property) )
			{
				this.propertyList.Add(property);
			}
			this.NotifyAsync();
		}

		// Indique que la s�lection par noms a chang�.
		public void NotifySelNamesChanged()
		{
			if ( !this.enable )  return;
			this.selNamesChanged = true;
			this.NotifyAsync();
		}


		// Agrandit au maximum la zone de redessin de tous les visualisateurs.
		public void NotifyArea()
		{
			if ( !this.enable )  return;
			this.NotifyArea(Rectangle.Infinite);
		}

		// Agrandit la zone de redessin de tous les visualisateurs.
		// Les unit�s pour le rectangle sont internes.
		public void NotifyArea(Rectangle rect)
		{
			if ( !this.enable )  return;
			if ( rect.IsEmpty )  return;
			foreach ( Viewer viewer in this.document.Modifier.AttachViewers )
			{
				this.NotifyArea(viewer, rect);
			}
		}

		// Agrandit au maximum la zone de redessin d'un visualisateur.
		public void NotifyArea(Viewer viewer)
		{
			if ( !this.enable )  return;
			this.NotifyArea(viewer, Rectangle.Infinite);
		}

		// Agrandit la zone de redessin d'un visualisateur.
		// Les unit�s pour le rectangle sont internes.
		public void NotifyArea(Viewer viewer, Rectangle rect)
		{
			if ( !this.enable )  return;
			if ( viewer == null || rect.IsEmpty )  return;
			viewer.RedrawAreaMerge(rect);
			this.NotifyAsync();
		}

		// Notifie qu'il faudra faire le GenerateEvents lorsque Windows
		// aura le temps.
		protected void NotifyAsync()
		{
			if ( this.document.Modifier.ActiveViewer == null )  return;
			Window window = this.document.Modifier.ActiveViewer.Window;
			if ( window == null )  return;
			window.AsyncNotify();
		}

		
		// G�n�re tous les �v�nements pour informer des changements, en fonction
		// des NotifyXYZ fait pr�c�demment.
		public void GenerateEvents()
		{
			if ( this.documentChanged )
			{
				this.OnDocumentChanged();
				this.documentChanged = false;
			}

			if ( this.mouseChanged )
			{
				this.OnMouseChanged();
				this.mouseChanged = false;
			}

			if ( this.modifChanged )
			{
				this.OnModifChanged();
				this.modifChanged = false;
			}

			if ( this.originChanged )
			{
				this.OnOriginChanged();
				this.originChanged = false;
			}

			if ( this.zoomChanged )
			{
				this.OnZoomChanged();
				this.zoomChanged = false;
			}

			if ( this.toolChanged )
			{
				this.OnToolChanged();
				this.toolChanged = false;
			}

			if ( this.saveChanged )
			{
				this.OnSaveChanged();
				this.saveChanged = false;
			}

			if ( this.selectionChanged )
			{
				this.OnSelectionChanged();
				this.selectionChanged = false;
			}

			if ( this.createChanged )
			{
				this.OnCreateChanged();
				this.createChanged = false;
			}

			if ( this.styleChanged )
			{
				this.OnStyleChanged();
				this.styleChanged = false;
			}

			if ( this.pagesChanged )
			{
				this.OnPagesChanged();
				this.pagesChanged = false;
			}

			if ( this.layersChanged )
			{
				this.OnLayersChanged();
				this.layersChanged = false;
			}

			if ( this.pageObject != null )
			{
				this.OnPageChanged(this.pageObject);
				this.pageObject = null;
			}

			if ( this.layerObject != null )
			{
				this.OnLayerChanged(this.layerObject);
				this.layerObject = null;
			}

			if ( this.undoRedoChanged )
			{
				this.OnUndoRedoChanged();
				this.undoRedoChanged = false;
			}

			if ( this.gridChanged )
			{
				this.OnGridChanged();
				this.gridChanged = false;
			}

			if ( this.previewChanged )
			{
				this.OnPreviewChanged();
				this.previewChanged = false;
			}

			if ( this.settingsChanged )
			{
				this.OnSettingsChanged();
				this.settingsChanged = false;
			}

			if ( this.guidesChanged )
			{
				this.OnGuidesChanged();
				this.guidesChanged = false;
			}

			if ( this.hideHalfChanged )
			{
				this.OnHideHalfChanged();
				this.hideHalfChanged = false;
			}

			if ( this.debugChanged )
			{
				this.OnDebugChanged();
				this.debugChanged = false;
			}

			if ( this.propertyList.Count > 0 )
			{
				this.OnPropertyChanged(this.propertyList);
				this.propertyList.Clear();
			}

			if ( this.selNamesChanged )
			{
				this.OnSelNamesChanged();
				this.selNamesChanged = false;
			}

			foreach ( Viewer viewer in this.document.Modifier.AttachViewers )
			{
				if ( !viewer.RedrawArea.IsEmpty )
				{
					if ( viewer == this.document.Modifier.ActiveViewer )
					{
						viewer.UpdateRulerGeometry();
					}
					this.OnDrawChanged(viewer, viewer.RedrawArea);
					viewer.RedrawAreaFlush();
				}
			}
		}


		protected void OnDocumentChanged()
		{
			if ( this.DocumentChanged != null )  // qq'un �coute ?
			{
				this.DocumentChanged();
			}
		}

		protected void OnMouseChanged()
		{
			if ( this.MouseChanged != null )  // qq'un �coute ?
			{
				this.MouseChanged();
			}
		}

		protected void OnModifChanged()
		{
			if ( this.ModifChanged != null )  // qq'un �coute ?
			{
				this.ModifChanged();
			}
		}

		protected void OnOriginChanged()
		{
			if ( this.OriginChanged != null )  // qq'un �coute ?
			{
				this.OriginChanged();
			}
		}

		protected void OnZoomChanged()
		{
			if ( this.ZoomChanged != null )  // qq'un �coute ?
			{
				this.ZoomChanged();
			}
		}

		protected void OnToolChanged()
		{
			if ( this.ToolChanged != null )  // qq'un �coute ?
			{
				this.ToolChanged();
			}
		}

		protected void OnSaveChanged()
		{
			if ( this.SaveChanged != null )  // qq'un �coute ?
			{
				this.SaveChanged();
			}
		}

		protected void OnSelectionChanged()
		{
			if ( this.SelectionChanged != null )  // qq'un �coute ?
			{
				this.SelectionChanged();
			}
		}

		protected void OnCreateChanged()
		{
			if ( this.CreateChanged != null )  // qq'un �coute ?
			{
				this.CreateChanged();
			}
		}

		protected void OnStyleChanged()
		{
			if ( this.StyleChanged != null )  // qq'un �coute ?
			{
				this.StyleChanged();
			}
		}

		protected void OnPagesChanged()
		{
			if ( this.PagesChanged != null )  // qq'un �coute ?
			{
				this.PagesChanged();
			}
		}

		protected void OnLayersChanged()
		{
			if ( this.LayersChanged != null )  // qq'un �coute ?
			{
				this.LayersChanged();
			}
		}

		protected void OnPageChanged(Objects.Abstract page)
		{
			if ( this.PageChanged != null )  // qq'un �coute ?
			{
				this.PageChanged(page);
			}
		}

		protected void OnLayerChanged(Objects.Abstract layer)
		{
			if ( this.LayerChanged != null )  // qq'un �coute ?
			{
				this.LayerChanged(layer);
			}
		}

		protected void OnUndoRedoChanged()
		{
			if ( this.UndoRedoChanged != null )  // qq'un �coute ?
			{
				this.UndoRedoChanged();
			}
		}

		protected void OnGridChanged()
		{
			if ( this.GridChanged != null )  // qq'un �coute ?
			{
				this.GridChanged();
			}
		}

		protected void OnPreviewChanged()
		{
			if ( this.PreviewChanged != null )  // qq'un �coute ?
			{
				this.PreviewChanged();
			}
		}

		protected void OnSettingsChanged()
		{
			if ( this.SettingsChanged != null )  // qq'un �coute ?
			{
				this.SettingsChanged();
			}
		}

		protected void OnGuidesChanged()
		{
			if ( this.GuidesChanged != null )  // qq'un �coute ?
			{
				this.GuidesChanged();
			}
		}

		protected void OnHideHalfChanged()
		{
			if ( this.HideHalfChanged != null )  // qq'un �coute ?
			{
				this.HideHalfChanged();
			}
		}

		protected void OnDebugChanged()
		{
			if ( this.DebugChanged != null )  // qq'un �coute ?
			{
				this.DebugChanged();
			}
		}

		protected void OnPropertyChanged(System.Collections.ArrayList propertyList)
		{
			if ( this.PropertyChanged != null )  // qq'un �coute ?
			{
				this.PropertyChanged(propertyList);
			}
		}

		protected void OnSelNamesChanged()
		{
			if ( this.SelNamesChanged != null )  // qq'un �coute ?
			{
				this.SelNamesChanged();
			}
		}

		protected void OnDrawChanged(Viewer viewer, Rectangle rect)
		{
			if ( this.DrawChanged != null )  // qq'un �coute ?
			{
				this.DrawChanged(viewer, rect);
			}
		}


		public event SimpleEventHandler			DocumentChanged;
		public event SimpleEventHandler			MouseChanged;
		public event SimpleEventHandler			ModifChanged;
		public event SimpleEventHandler			OriginChanged;
		public event SimpleEventHandler			ZoomChanged;
		public event SimpleEventHandler			ToolChanged;
		public event SimpleEventHandler			SaveChanged;
		public event SimpleEventHandler			SelectionChanged;
		public event SimpleEventHandler			CreateChanged;
		public event SimpleEventHandler			StyleChanged;
		public event SimpleEventHandler			PagesChanged;
		public event SimpleEventHandler			LayersChanged;
		public event ObjectEventHandler			PageChanged;
		public event ObjectEventHandler			LayerChanged;
		public event SimpleEventHandler			UndoRedoChanged;
		public event SimpleEventHandler			GridChanged;
		public event SimpleEventHandler			PreviewChanged;
		public event SimpleEventHandler			SettingsChanged;
		public event SimpleEventHandler			GuidesChanged;
		public event SimpleEventHandler			HideHalfChanged;
		public event SimpleEventHandler			DebugChanged;
		public event PropertyEventHandler		PropertyChanged;
		public event SimpleEventHandler			SelNamesChanged;
		public event RedrawEventHandler			DrawChanged;

		protected Document						document;
		protected bool							enable = true;
		protected bool							enableSelectionChanged = true;
		protected bool							documentChanged;
		protected bool							mouseChanged;
		protected bool							modifChanged;
		protected bool							originChanged;
		protected bool							zoomChanged;
		protected bool							toolChanged;
		protected bool							saveChanged;
		protected bool							selectionChanged;
		protected bool							createChanged;
		protected bool							styleChanged;
		protected bool							pagesChanged;
		protected bool							layersChanged;
		protected Objects.Abstract				pageObject;
		protected Objects.Abstract				layerObject;
		protected bool							undoRedoChanged;
		protected bool							gridChanged;
		protected bool							previewChanged;
		protected bool							settingsChanged;
		protected bool							guidesChanged;
		protected bool							hideHalfChanged;
		protected bool							debugChanged;
		protected System.Collections.ArrayList	propertyList = new System.Collections.ArrayList();
		protected bool							selNamesChanged;
	}
}
