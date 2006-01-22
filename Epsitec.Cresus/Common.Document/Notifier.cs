using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	public delegate void SimpleEventHandler();
	public delegate void ObjectEventHandler(Objects.Abstract obj);
	public delegate void PropertyEventHandler(System.Collections.ArrayList propertyList);
	public delegate void AggregateEventHandler(System.Collections.ArrayList aggregateList);
	public delegate void TextStyleEventHandler(System.Collections.ArrayList textStyleList);
	public delegate void RedrawEventHandler(Viewer viewer, Rectangle rect);
	public delegate void RibbonEventHandler(string name);
	public delegate void BookPanelEventHandler(string page, string sub);
	public delegate void SettingsEventHandler(string book, string tab);

	/// <summary>
	/// Summary description for Notifier.
	/// </summary>
	public class Notifier
	{
		public Notifier(Document document)
		{
			this.document = document;
		}


		public bool Enable
		{
			//	Etat du notificateur.
			get { return this.enable; }
			set { this.enable = value; }
		}

		public bool EnableSelectionChanged
		{
			//	Etat du notificateur.
			get { return this.enableSelectionChanged; }
			set { this.enableSelectionChanged = value; }
		}


		public void NotifyAllChanged()
		{
			//	Indique que tout a chang�.
			if ( !this.enable )  return;

			this.documentChanged = true;
			this.mouseChanged = true;
			this.modifChanged = true;
			this.originChanged = true;
			this.zoomChanged = true;
			this.toolChanged = true;
			this.saveChanged = true;
			this.selectionChanged = true;
			this.shaperChanged = true;
			this.textChanged = true;
			this.textCursorChanged = true;
			this.styleChanged = true;
			this.pagesChanged = true;
			this.layersChanged = true;
			this.undoRedoChanged = true;
			this.gridChanged = true;
			this.labelPropertiesChanged = true;
			this.magnetChanged = true;
			this.previewChanged = true;
			this.settingsChanged = true;
			this.fontsSettingsChanged = true;
			this.guidesChanged = true;
			this.hideHalfChanged = true;
			this.debugChanged = true;
			this.selNamesChanged = true;

			this.NotifyArea();
		}

		public void NotifyCreateChanged()
		{
			//	Indique que la cr�ation d'un objet � d�but� ou s'est termin�e.
			if ( !this.enable )  return;

			this.toolChanged = true;
			this.selectionChanged = true;
			this.pagesChanged = true;
			this.layersChanged = true;
			this.undoRedoChanged = true;

			this.NotifyAsync();
		}


		public void NotifyDocumentChanged()
		{
			//	Indique que les informations sur le document ont chang�.
			//	Nom du document, taille, etc.
			if ( !this.enable )  return;
			this.documentChanged = true;
			this.NotifyAsync();
		}

		public void NotifyMouseChanged()
		{
			//	Indique que la position de la souris a chang�.
			if ( !this.enable )  return;
			this.mouseChanged = true;
			this.NotifyAsync();
		}

		public void NotifyModifChanged()
		{
			//	Indique que le texte des modifications a chang�.
			if ( !this.enable )  return;
			this.modifChanged = true;
			this.NotifyAsync();
		}

		public void NotifyOriginChanged()
		{
			//	Indique que l'origine a chang�.
			if ( !this.enable )  return;
			this.originChanged = true;
			this.NotifyAsync();
		}

		public void NotifyZoomChanged()
		{
			//	Indique que le zoom a chang�.
			if ( !this.enable )  return;
			this.zoomChanged = true;
			this.NotifyAsync();
		}

		public void NotifyToolChanged()
		{
			//	Indique que l'outil s�lectionn� a chang�.
			if ( !this.enable )  return;
			this.toolChanged = true;
			this.NotifyAsync();
		}

		public void NotifySaveChanged()
		{
			//	Indique que le bouton "enregistrer" a chang�.
			if ( !this.enable )  return;
			this.saveChanged = true;
			this.NotifyAsync();
		}

		public void NotifySelectionChanged()
		{
			//	Indique que les objets s�lectionn�s ont chang�.
			if ( !this.enable || !this.enableSelectionChanged )  return;
			this.selectionChanged = true;
			this.NotifyAsync();
		}

		public void NotifyShaperChanged()
		{
			//	Indique que les objets pour le modeleur ont chang�.
			if ( !this.enable )  return;
			this.shaperChanged = true;
			this.NotifyAsync();
		}

		public void NotifyTextChanged()
		{
			//	Indique que le texte en �dition a chang�.
			if ( !this.enable )  return;
			this.textChanged = true;
			this.NotifyAsync();
		}

		public void NotifyTextCursorChanged()
		{
			//	Indique que le curseur du texte en �dition a chang�.
			if ( !this.enable )  return;
			this.textCursorChanged = true;
			this.NotifyAsync();
		}

		public void NotifyStyleChanged()
		{
			//	Indique que les styles ont chang�.
			if ( !this.enable )  return;
			this.styleChanged = true;
			this.NotifyAsync();
		}

		public void NotifyPagesChanged()
		{
			//	Indique que les pages ont chang�.
			if ( !this.enable )  return;
			this.pagesChanged = true;

			if ( !this.document.Settings.GlobalGuides )
			{
				this.NotifyGuidesChanged();
			}

			this.NotifyAsync();
		}

		public void NotifyLayersChanged()
		{
			//	Indique que les calques ont chang�.
			if ( !this.enable )  return;
			this.layersChanged = true;
			this.NotifyAsync();
		}

		public void NotifyPageChanged(Objects.Abstract page)
		{
			//	Indique qu'une page a chang�.
			if ( !this.enable )  return;
			this.pageObject = page;
			this.NotifyAsync();
		}

		public void NotifyLayerChanged(Objects.Abstract layer)
		{
			//	Indique qu'un calque a chang�.
			if ( !this.enable )  return;
			this.layerObject = layer;
			this.NotifyAsync();
		}

		public void NotifyUndoRedoChanged()
		{
			//	Indique que les commandes undo/redo ont chang�.
			if ( !this.enable )  return;
			this.undoRedoChanged = true;
			this.NotifyAsync();
		}

		public void NotifyGridChanged()
		{
			//	Indique que les commandes pour la grille ont chang�.
			if ( !this.enable )  return;
			this.gridChanged = true;
			this.NotifyAsync();
		}

		public void NotifyLabelPropertiesChanged()
		{
			//	Indique que les commandes pour les noms d'attributs ont chang�.
			if ( !this.enable )  return;
			this.labelPropertiesChanged = true;
			this.NotifyAsync();
		}

		public void NotifyMagnetChanged()
		{
			//	Indique que les commandes pour les lignes magn�tiques ont chang�.
			if ( !this.enable )  return;
			this.magnetChanged = true;
			this.NotifyAsync();
		}

		public void NotifyPreviewChanged()
		{
			//	Indique que la commande aper�u a chang�.
			if ( !this.enable )  return;
			this.previewChanged = true;
			this.NotifyAsync();
		}

		public void NotifySettingsChanged()
		{
			//	Indique que les r�glages ont chang�.
			if ( !this.enable )  return;
			this.settingsChanged = true;
			this.NotifyAsync();
		}

		public void NotifyFontsSettingsChanged()
		{
			//	Indique que les r�glages de police ont chang�.
			if ( !this.enable )  return;
			this.fontsSettingsChanged = true;
			this.NotifyAsync();
		}

		public void NotifyGuidesChanged()
		{
			//	Indique que les rep�res ont chang�.
			if ( !this.enable )  return;
			this.guidesChanged = true;
			this.NotifyAsync();
		}

		public void NotifyHideHalfChanged()
		{
			//	Indique que la commande estomp� a chang�.
			if ( !this.enable )  return;
			this.hideHalfChanged = true;
			this.NotifyAsync();
		}

		public void NotifyDebugChanged()
		{
			//	Indique que les commandes pour le debug ont chang�.
			if ( !this.enable )  return;
			this.debugChanged = true;
			this.NotifyAsync();
		}


		public void NotifyPropertyChanged(Properties.Abstract property)
		{
			//	Indique qu'une propri�t� a chang�.
			if ( !this.enable )  return;
			if ( !this.propertyList.Contains(property) )
			{
				this.propertyList.Add(property);
			}
			this.NotifyAsync();
		}

		public void NotifyAggregateChanged(Properties.Aggregate agg)
		{
			//	Indique qu'un aggr�gat a chang�.
			if ( !this.enable )  return;
			if ( !this.aggregateList.Contains(agg) )
			{
				this.aggregateList.Add(agg);
			}
			this.NotifyAsync();
		}

		public void NotifyTextStyleChanged(Text.TextStyle textStyle)
		{
			//	Indique qu'un style de texte a chang�.
			if ( !this.enable )  return;
			if ( !this.textStyleList.Contains(textStyle) )
			{
				this.textStyleList.Add(textStyle);
			}
			this.NotifyAsync();
		}

		public void NotifySelNamesChanged()
		{
			//	Indique que la s�lection par noms a chang�.
			if ( !this.enable )  return;
			this.selNamesChanged = true;
			this.NotifyAsync();
		}


		public void NotifyArea()
		{
			//	Agrandit au maximum la zone de redessin de tous les visualisateurs.
			if ( !this.enable )  return;
			this.NotifyArea(Rectangle.Infinite);
		}

		public void NotifyArea(Rectangle rect)
		{
			//	Agrandit la zone de redessin de tous les visualisateurs.
			//	Les unit�s pour le rectangle sont internes.
			if ( !this.enable )  return;
			if ( rect.IsEmpty )  return;
			foreach ( Viewer viewer in this.document.Modifier.AttachViewers )
			{
				this.NotifyArea(viewer, rect);
			}
		}

		public void NotifyArea(Viewer viewer)
		{
			//	Agrandit au maximum la zone de redessin d'un visualisateur.
			if ( !this.enable )  return;
			this.NotifyArea(viewer, Rectangle.Infinite);
		}

		public void NotifyArea(Viewer viewer, Rectangle rect)
		{
			//	Agrandit la zone de redessin d'un visualisateur.
			//	Les unit�s pour le rectangle sont internes.
			if ( !this.enable )  return;
			if ( viewer == null || rect.IsEmpty )  return;
			viewer.RedrawAreaMerge(rect);
			this.NotifyAsync();
		}

		protected void NotifyAsync()
		{
			//	Notifie qu'il faudra faire le GenerateEvents lorsque Windows
			//	aura le temps.
			if ( this.document.Modifier.ActiveViewer == null )  return;
			Window window = this.document.Modifier.ActiveViewer.Window;
			if ( window == null )  return;
			window.AsyncNotify();
		}

		
		public void NotifyRibbonCommand(string name)
		{
			//	Indique qu'on ruban doit changer.
			this.OnRibbonCommand(name);
		}

		public void NotifyBookPanelShowPage(string page, string sub)
		{
			//	Indique qu'il faut afficher un onglet donn�.
			this.OnBookPanelShowPage(page, sub);
		}

		public void NotifySettingsShowPage(string book, string tab)
		{
			//	Indique qu'il faut afficher une page du dialogue des commandes.
			this.OnSettingsShowPage(book, tab);
		}

		
		public void GenerateEvents()
		{
			//	G�n�re tous les �v�nements pour informer des changements, en fonction
			//	des NotifyXYZ fait pr�c�demment.
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

			if ( this.shaperChanged )
			{
				this.OnShaperChanged();
				this.shaperChanged = false;
			}

			if ( this.textChanged )
			{
				this.OnTextChanged();
				this.textChanged = false;
			}

			if ( this.textCursorChanged )
			{
				this.OnTextCursorChanged();
				this.textCursorChanged = false;
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

			if ( this.labelPropertiesChanged )
			{
				this.OnLabelPropertiesChanged();
				this.labelPropertiesChanged = false;
			}

			if ( this.magnetChanged )
			{
				this.OnMagnetChanged();
				this.magnetChanged = false;
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

			if ( this.fontsSettingsChanged )
			{
				this.OnFontsSettingsChanged();
				this.fontsSettingsChanged = false;
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

			if ( this.aggregateList.Count > 0 )
			{
				this.OnAggregateChanged(this.aggregateList);
				this.aggregateList.Clear();
			}

			if ( this.textStyleList.Count > 0 )
			{
				this.OnTextStyleChanged(this.textStyleList);
				this.textStyleList.Clear();
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

		protected void OnShaperChanged()
		{
			if ( this.ShaperChanged != null )  // qq'un �coute ?
			{
				this.ShaperChanged();
			}
		}

		protected void OnTextChanged()
		{
			if ( this.TextChanged != null )  // qq'un �coute ?
			{
				this.TextChanged();
			}
		}

		protected void OnTextCursorChanged()
		{
			if ( this.TextCursorChanged != null )  // qq'un �coute ?
			{
				this.TextCursorChanged();
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

		protected void OnLabelPropertiesChanged()
		{
			if ( this.LabelPropertiesChanged != null )  // qq'un �coute ?
			{
				this.LabelPropertiesChanged();
			}
		}

		protected void OnMagnetChanged()
		{
			if ( this.MagnetChanged != null )  // qq'un �coute ?
			{
				this.MagnetChanged();
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

		protected void OnFontsSettingsChanged()
		{
			if ( this.FontsSettingsChanged != null )  // qq'un �coute ?
			{
				this.FontsSettingsChanged();
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

		protected void OnAggregateChanged(System.Collections.ArrayList aggregateList)
		{
			if ( this.AggregateChanged != null )  // qq'un �coute ?
			{
				this.AggregateChanged(aggregateList);
			}
		}

		protected void OnTextStyleChanged(System.Collections.ArrayList textStyleList)
		{
			if ( this.TextStyleChanged != null )  // qq'un �coute ?
			{
				this.TextStyleChanged(textStyleList);
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

		protected void OnRibbonCommand(string name)
		{
			if ( this.RibbonCommand != null )  // qq'un �coute ?
			{
				this.RibbonCommand(name);
			}
		}

		protected void OnBookPanelShowPage(string page, string sub)
		{
			if ( this.BookPanelShowPage != null )  // qq'un �coute ?
			{
				this.BookPanelShowPage(page, sub);
			}
		}

		protected void OnSettingsShowPage(string book, string tab)
		{
			if ( this.SettingsShowPage != null )  // qq'un �coute ?
			{
				this.SettingsShowPage(book, tab);
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
		public event SimpleEventHandler			ShaperChanged;
		public event SimpleEventHandler			TextChanged;
		public event SimpleEventHandler			TextCursorChanged;
		public event SimpleEventHandler			StyleChanged;
		public event SimpleEventHandler			PagesChanged;
		public event SimpleEventHandler			LayersChanged;
		public event ObjectEventHandler			PageChanged;
		public event ObjectEventHandler			LayerChanged;
		public event SimpleEventHandler			UndoRedoChanged;
		public event SimpleEventHandler			GridChanged;
		public event SimpleEventHandler			LabelPropertiesChanged;
		public event SimpleEventHandler			MagnetChanged;
		public event SimpleEventHandler			PreviewChanged;
		public event SimpleEventHandler			SettingsChanged;
		public event SimpleEventHandler			FontsSettingsChanged;
		public event SimpleEventHandler			GuidesChanged;
		public event SimpleEventHandler			HideHalfChanged;
		public event SimpleEventHandler			DebugChanged;
		public event PropertyEventHandler		PropertyChanged;
		public event AggregateEventHandler		AggregateChanged;
		public event TextStyleEventHandler		TextStyleChanged;
		public event SimpleEventHandler			SelNamesChanged;
		public event RedrawEventHandler			DrawChanged;
		public event RibbonEventHandler			RibbonCommand;
		public event BookPanelEventHandler		BookPanelShowPage;
		public event SettingsEventHandler		SettingsShowPage;

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
		protected bool							shaperChanged;
		protected bool							textChanged;
		protected bool							textCursorChanged;
		protected bool							styleChanged;
		protected bool							pagesChanged;
		protected bool							layersChanged;
		protected Objects.Abstract				pageObject;
		protected Objects.Abstract				layerObject;
		protected bool							undoRedoChanged;
		protected bool							gridChanged;
		protected bool							labelPropertiesChanged;
		protected bool							magnetChanged;
		protected bool							previewChanged;
		protected bool							settingsChanged;
		protected bool							fontsSettingsChanged;
		protected bool							guidesChanged;
		protected bool							hideHalfChanged;
		protected bool							debugChanged;
		protected System.Collections.ArrayList	propertyList = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	aggregateList = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	textStyleList = new System.Collections.ArrayList();
		protected bool							selNamesChanged;
	}
}
