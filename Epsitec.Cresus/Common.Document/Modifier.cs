using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Summary description for Modifier.
	/// </summary>
	public class Modifier
	{
		public Modifier(Document document)
		{
			this.document = document;
			this.attachViewers = new System.Collections.ArrayList();
			this.attachContainers = new System.Collections.ArrayList();
			this.tool = "Select";
			this.zoomHistory = new ZoomHistory();
			this.opletQueue = new OpletQueue();
			this.objectMemory = new ObjectMemory(this.document, null);
		}

		// Outil s�lectionn� dans la palette.
		public string Tool
		{
			get
			{
				return this.tool;
			}

			set
			{
				if ( this.tool != value )
				{
					bool isCreate = this.opletCreate;
					this.OpletQueueBeginAction();
					this.InsertOpletTool();

					AbstractObject editObject = this.RetEditObject();

					if ( this.tool == "HotSpot" || value == "HotSpot" )
					{
						this.document.Notifier.NotifyArea(this.ActiveViewer);
					}

					this.tool = value;

					if ( this.tool == "Select" && isCreate )  // on vient de cr�er un objet ?
					{
						DrawingContext context = this.ActiveViewer.DrawingContext;
						AbstractObject layer = context.RootObject();
						AbstractObject obj = layer.Objects[layer.Objects.Count-1] as AbstractObject;
						this.ActiveViewer.Select(obj, false, false);
					}

					else if ( this.tool == "Edit" && isCreate )  // on vient de cr�er un objet ?
					{
						DrawingContext context = this.ActiveViewer.DrawingContext;
						AbstractObject layer = context.RootObject();
						AbstractObject obj = layer.Objects[layer.Objects.Count-1] as AbstractObject;
						this.ActiveViewer.Select(obj, true, false);
					}

					else if ( this.IsTool && this.tool != "Edit" )
					{
						if ( editObject != null )
						{
							editObject.Select(true);
						}
					}

					else if ( this.tool == "Edit" )
					{
						if ( this.TotalSelected == 1 )
						{
							AbstractObject sel = this.RetOnlySelectedObject();
							if ( sel != null )
							{
								if ( sel.IsEditable )
								{
									sel.Select(true, true);
								}
								else
								{
									this.DeselectAll();
								}
							}
						}
						else if ( this.TotalSelected > 1 )
						{
							this.DeselectAll();
						}
					}

					else if ( !IsTool )  // choix d'un objet � cr�er ?
					{
						this.DeselectAll();
					}

					this.OpletQueueValidateAction();
					this.document.Notifier.NotifyToolChanged();
					this.document.Notifier.NotifySelectionChanged();
				}
			}
		}

		// Indique si l'outil s�lectionn� n'est pas un objet.
		protected bool IsTool
		{
			get
			{
				if ( this.tool == "Select"  )  return true;
				if ( this.tool == "Global"  )  return true;
				if ( this.tool == "Edit"    )  return true;
				if ( this.tool == "Zoom"    )  return true;
				if ( this.tool == "Hand"    )  return true;
				if ( this.tool == "Picker"  )  return true;
				if ( this.tool == "HotSpot" )  return true;
				return false;
			}
		}


		public OpletQueue OpletQueue
		{
			get { return this.opletQueue; }
		}

		public ObjectMemory ObjectMemory
		{
			get { return this.objectMemory; }
		}

		// Taille de la zone de travail.
		public Size SizeArea
		{
			get
			{
				return new Size(this.document.Size.Width*3, this.document.Size.Height*3);
			}
		}

		// Origine de la zone de travail.
		public Point OriginArea
		{
			get
			{
				Size area = this.SizeArea;
				Point origin = new Point();
				origin.X = (this.document.Size.Width-area.Width)/2;
				origin.Y = (this.document.Size.Height-area.Height)/2;
				return origin;
			}
		}


		#region Viewers
		// Un seul visualisateur privil�gi� peut �tre actif.
		public Viewer ActiveViewer
		{
			get { return this.activeViewer; }
			set { this.activeViewer = value; }
		}

		// Attache un nouveau visualisateur � ce document.
		public void AttachViewer(Viewer viewer)
		{
			this.attachViewers.Add(viewer);
		}

		// D�tache un visualisateur de ce document.
		public void DetachViewer(Viewer viewer)
		{
			this.attachViewers.Remove(viewer);
		}

		// Liste des visualisateurs attach�s au document.
		public System.Collections.ArrayList	AttachViewers
		{
			get { return this.attachViewers; }
		}
		#endregion


		#region Containers
		// Attache un nouveau conteneur � ce document.
		public void AttachContainer(AbstractContainer container)
		{
			this.attachContainers.Add(container);
		}

		// D�tache un conteneur de ce document.
		public void DetachContainer(AbstractContainer container)
		{
			this.attachContainers.Remove(container);
		}

		// Met en �vidence l'objet survol� par la souris.
		public void ContainerHilite(AbstractObject obj)
		{
			foreach ( AbstractContainer container in this.attachContainers )
			{
				container.Hilite(obj);
			}
		}

		// Indique quel est le container actif (visible).
		public AbstractContainer ActiveContainer
		{
			get { return this.activeContainer; }
			set { this.activeContainer = value; }
		}
		#endregion


		// Vide le document de tous ses objets.
		public void New()
		{
			this.ActiveViewer.CreateEnding(false);
			this.OpletQueueEnable = false;

			Modifier.UniqueObjectId = 0;
			Modifier.UniqueStyleId = 0;
			this.TotalSelected = 0;
			this.totalHide = 0;
			this.totalPageHide = 0;
			this.document.Objects.Clear();

			this.document.PropertiesAuto.Clear();
			this.document.PropertiesSel.Clear();
			this.document.PropertiesStyle.Clear();
			this.objectMemory = new ObjectMemory(this.document, null);

			ObjectPage page = new ObjectPage(this.document, null);  // cr�e la page initiale
			this.document.Objects.Add(page);

			ObjectLayer layer = new ObjectLayer(this.document, null);  // cr�e le calque initial
			page.Objects.Add(layer);

			foreach ( Viewer viewer in this.attachViewers )
			{
				DrawingContext context = viewer.DrawingContext;
				context.InternalPageLayer(0, 0);
				context.ZoomAndOrigin(1, new Point(0,0));
			}

			this.zoomHistory.Clear();
			this.document.HotSpot = new Point(0, 0);
			this.document.Filename = "";
			this.document.IsDirtySerialize = false;
			this.ActiveViewer.GlobalSelect = false;
			this.opletCreate = false;

			this.OpletQueueEnable = true;
			this.opletQueue.PurgeUndo();
			this.opletQueue.PurgeRedo();

			this.document.Notifier.NotifyArea();
			this.document.Notifier.NotifySelectionChanged();
			this.document.Notifier.NotifyStyleChanged();
			this.document.Notifier.NotifyZoomChanged();
			this.document.Notifier.NotifyUndoRedoChanged();
			this.document.Notifier.NotifyPagesChanged();
			this.document.Notifier.NotifyLayersChanged();
		}


		#region Counters
		// Indique qu'il faudra mettre � jour tous les compteurs.
		public void DirtyCounters()
		{
			this.dirtyCounters = true;
		}

		// Met � jour tous les compteurs.
		protected void UpdateCounters()
		{
			if ( this.dirtyCounters == false )  return;

			DrawingContext context = this.ActiveViewer.DrawingContext;

			this.totalPageHide = 0;
			AbstractObject page = context.RootObject(1);
			foreach ( AbstractObject obj in this.document.Deep(page) )
			{
				if ( obj.IsHide )  this.totalPageHide ++;
			}

			this.totalSelected = 0;
			this.totalHide = 0;
			AbstractObject layer = context.RootObject();
			foreach ( AbstractObject obj in this.document.Deep(layer) )
			{
				if ( obj.IsSelected )  this.totalSelected ++;
				if ( obj.IsHide )  this.totalHide ++;
			}

			this.dirtyCounters = false;
		}

		// Retourne le nombre total d'objets, y compris les objets cach�s.
		public int TotalObjects
		{
			get
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject layer = context.RootObject();
				return layer.Objects.Count;
			}
		}

		// Retourne le nombre d'objets s�lectionn�s.
		public int TotalSelected
		{
			get
			{
				if ( this.dirtyCounters )  this.UpdateCounters();
				return this.totalSelected;
			}

			set
			{
				this.totalSelected = value;
			}
		}

		// Retourne le nombre d'objets cach�s dans le calque courant.
		public int TotalHide
		{
			get
			{
				if ( this.dirtyCounters )  this.UpdateCounters();
				return this.totalHide;
			}

			set
			{
				this.totalPageHide += value-this.totalHide;
				this.totalHide = value;
			}
		}

		// Retourne le nombre d'objets cach�s dans toute la page.
		public int TotalPageHide
		{
			get
			{
				if ( this.dirtyCounters )  this.UpdateCounters();
				return this.totalPageHide;
			}
		}
		#endregion


		#region Statistic
		// Retourne un texte multi-lignes de statistiques sur le document.
		public string Statistic()
		{
			string chip = "<list type=\"fix\" width=\"1.5\"/>";
			string info1 = string.Format("{0}Nom complet: {1}<br/>", chip, this.document.Filename);
			string info2 = string.Format("{0}Dimensions: {1}x{2}<br/>", chip, this.document.Size.Width, this.document.Size.Height);
			string info3 = string.Format("{0}Nombre de pages: {1}<br/>", chip, this.StatisticTotalPages());
			string info4 = string.Format("{0}Nombre de calques: {1}<br/>", chip, this.StatisticTotalLayers());
			string info5 = string.Format("{0}Nombre d'objets: {1}<br/>", chip, this.StatisticTotalObjects());
			
			if ( this.document.Filename == "" )
			{
				info1 = "";
			}

			return string.Format("{0}{1}{2}{3}{4}", info1, info2, info3, info4, info5);
		}

		// Retourne le nombre total de pages.
		protected int StatisticTotalPages()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			return context.TotalPages();
		}

		// Retourne le nombre total de calques.
		protected int StatisticTotalLayers()
		{
			int total = 0;
			foreach ( AbstractObject obj in this.document.Deep(null) )
			{
				if ( obj is ObjectLayer )  total ++;
			}
			return total;
		}

		// Retourne le nombre total d'objets.
		public int StatisticTotalObjects()
		{
			int total = 0;
			foreach ( AbstractObject obj in this.document.Deep(null) )
			{
				if ( obj is ObjectPage  )  continue;
				if ( obj is ObjectLayer )  continue;
				if ( obj is ObjectGroup )  continue;
				total ++;
			}
			return total;
		}
		#endregion


		#region Selection
		// Retourne la bbox des objets s�lectionn�s.
		public Rectangle SelectedBbox
		{
			get
			{
				Rectangle bbox = Rectangle.Empty;
				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject layer = context.RootObject();
				foreach ( AbstractObject obj in this.document.Flat(layer, true) )
				{
					bbox.MergeWith(obj.BoundingBoxDetect);
				}
				return bbox;
			}
		}

		// Retourne le seul objet s�lectionn�.
		public AbstractObject RetOnlySelectedObject()
		{
			if ( this.TotalSelected != 1 )  return null;

			DrawingContext context = this.ActiveViewer.DrawingContext;
			AbstractObject layer = context.RootObject();
#if true
			int total = layer.Objects.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				AbstractObject obj = layer.Objects[i] as AbstractObject;
				if ( obj.IsSelected )  return obj;
			}
#else
			foreach ( AbstractObject obj in this.document.Flat(layer, true) )
			{
				return obj;
			}
#endif
			return null;
		}

		// Retourne le seul objet en �dition.
		public AbstractObject RetEditObject()
		{
			if ( this.tool != "Edit" )  return null;
			if ( this.TotalSelected != 1 )  return null;

			DrawingContext context = this.ActiveViewer.DrawingContext;
			AbstractObject layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				AbstractObject obj = layer.Objects[i] as AbstractObject;
				if ( obj.IsSelected && obj.IsEdited )  return obj;
			}
			return null;
		}

		// D�s�lectionne tous les objets.
		public void DeselectAll()
		{
			using ( this.OpletQueueBeginAction() )
			{
				this.ActiveViewer.CreateEnding(false);
				if ( this.TotalSelected > 0 )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					AbstractObject layer = context.RootObject();
					foreach ( AbstractObject obj in this.document.Flat(layer, true) )
					{
						obj.Deselect();
					}
					this.TotalSelected = 0;
					this.ActiveViewer.GlobalSelect = false;
				}
				this.OpletQueueValidateAction();
			}
		}

		// S�lectionne tous les objets.
		public void SelectAll()
		{
			using ( this.OpletQueueBeginAction() )
			{
				this.ActiveViewer.CreateEnding(false);

				this.opletCreate = false;
				this.Tool = "Select";

				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject layer = context.RootObject();
				foreach ( AbstractObject obj in this.document.Flat(layer) )
				{
					if ( !obj.IsSelected && !obj.IsHide )
					{
						obj.Select();
						this.TotalSelected ++;
					}
				}

				this.ActiveViewer.GlobalSelect = ( this.TotalSelected > 1 );
				this.OpletQueueValidateAction();
			}
		}

		// Inverse la s�lection.
		public void InvertSelection()
		{
			using ( this.OpletQueueBeginAction() )
			{
				this.ActiveViewer.CreateEnding(false);

				this.opletCreate = false;
				this.Tool = "Select";

				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject layer = context.RootObject();
				foreach ( AbstractObject obj in this.document.Flat(layer) )
				{
					if ( obj.IsHide )  continue;

					if ( obj.IsSelected )
					{
						obj.Deselect();
						this.TotalSelected --;
					}
					else
					{
						obj.Select();
						this.TotalSelected ++;
					}
				}

				this.ActiveViewer.GlobalSelect = ( this.TotalSelected > 1 );
				this.OpletQueueValidateAction();
			}
		}
		#endregion


		#region Delete, Duplicate and Clipboard
		// Supprime tous les objets s�lectionn�s.
		public void DeleteSelection()
		{
			this.document.IsDirtySerialize = true;

			if ( this.ActiveViewer.IsCreating )
			{
				this.ActiveViewer.CreateEnding(true);
			}
			else
			{
				using ( this.OpletQueueBeginAction() )
				{
					bool bDo = false;
					do
					{
						bDo = false;
						DrawingContext context = this.ActiveViewer.DrawingContext;
						AbstractObject layer = context.RootObject();
						foreach ( AbstractObject obj in this.document.Flat(layer, true) )
						{
							this.document.Modifier.TotalSelected --;
							this.document.Notifier.NotifyArea(obj.BoundingBox);
							obj.Dispose();
							layer.Objects.Remove(obj);
							bDo = true;
							break;
						}
					}
					while ( bDo );

					this.ActiveViewer.GlobalSelect = false;
					this.document.Notifier.NotifySelectionChanged();

					this.OpletQueueValidateAction();
				}
			}
		}

		// Duplique tous les objets s�lectionn�s.
		public void DuplicateSelection(Point move)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.Tool = "Select";
				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject layer = context.RootObject();
				Modifier.Duplicate(this.document, this.document, layer.Objects, layer.Objects, true, move, true);
				this.ActiveViewer.GlobalSelect = ( this.document.Modifier.TotalSelected > 1 );

				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		// Coupe tous les objets s�lectionn�s dans le bloc-notes.
		public void CutSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject layer = context.RootObject();
				this.document.Clipboard.Modifier.New();
				this.document.Clipboard.Modifier.OpletQueueEnable = false;
				this.Duplicate(this.document, this.document.Clipboard, new Point(0,0), true);
				this.DeleteSelection();

				this.OpletQueueValidateAction();
			}
		}

		// Copie tous les objets s�lectionn�s dans le bloc-notes.
		public void CopySelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction() )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject layer = context.RootObject();
				this.document.Clipboard.Modifier.New();
				this.document.Clipboard.Modifier.OpletQueueEnable = false;
				this.Duplicate(this.document, this.document.Clipboard, new Point(0,0), true);

				this.OpletQueueValidateAction();
			}
		}

		// Colle le contenu du bloc-notes.
		public void Paste()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject layer = context.RootObject();
				this.DeselectAll();
				this.document.Clipboard.Modifier.OpletQueueEnable = false;
				this.Duplicate(this.document.Clipboard, this.document, new Point(0,0), true);
				this.ActiveViewer.GlobalSelect = ( this.document.Modifier.TotalSelected > 1 );

				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		// Duplique d'un document dans un autre.
		protected void Duplicate(Document srcDoc, Document dstDoc,
								 Point move, bool onlySelected)
		{
			DrawingContext srcContext = srcDoc.Modifier.ActiveViewer.DrawingContext;
			UndoableList srcList = srcContext.RootObject().Objects;

			DrawingContext dstContext = dstDoc.Modifier.ActiveViewer.DrawingContext;
			UndoableList dstList = dstContext.RootObject().Objects;

			Modifier.Duplicate(srcDoc, dstDoc, srcList, dstList, false, move, onlySelected);
		}

		// Copie tous les objets d'une liste source dans une liste destination.
		protected static void Duplicate(Document srcDoc, Document dstDoc,
										UndoableList srcList, UndoableList dstList,
										bool deselect, Point move, bool onlySelected)
		{
			int total = srcList.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = srcList[index] as AbstractObject;
				if ( onlySelected && !obj.IsSelected )  continue;

				AbstractObject newObject = null;
				if ( !obj.DuplicateObject(dstDoc, ref newObject) )  continue;

				if ( deselect && obj.IsSelected )
				{
					obj.Deselect();
					srcDoc.Modifier.TotalSelected --;
				}

				if ( dstDoc.Mode == DocumentMode.Modify )
				{
					newObject.DuplicateAdapt();
				}
				newObject.MoveAllStarting();
				newObject.MoveAllProcess(move);

				dstList.Add(newObject);

				if ( dstDoc.Mode == DocumentMode.Modify && newObject.IsSelected )
				{
					dstDoc.Modifier.TotalSelected ++;
					dstDoc.Notifier.NotifyArea(newObject.BoundingBox);
				}

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					Modifier.Duplicate(srcDoc, dstDoc, obj.Objects, newObject.Objects, deselect, move, false);
				}
			}
		}

		// Indique si le bloc-notes est vide.
		public bool IsClipboardEmpty()
		{
			if ( this.document.Clipboard == null )  return true;
			return ( this.document.Clipboard.Modifier.TotalObjects == 0 );
		}
		#endregion


		#region UndoRedo
		// Annule la derni�re action.
		public void Undo()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;
			this.ActiveViewer.CreateEnding(false);
			this.opletQueue.UndoAction();
			this.opletLastCmd = "";
			this.opletLastId = 0;
			this.document.Notifier.NotifySelectionChanged();
			this.document.Notifier.NotifyStyleChanged();
			this.document.Notifier.NotifyUndoRedoChanged();
			this.document.Notifier.NotifyArea();
		}

		// Refait la derni�re action.
		public void Redo()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;
			this.ActiveViewer.CreateEnding(false);
			this.opletQueue.RedoAction();
			this.opletLastCmd = "";
			this.opletLastId = 0;
			this.document.Notifier.NotifySelectionChanged();
			this.document.Notifier.NotifyStyleChanged();
			this.document.Notifier.NotifyUndoRedoChanged();
			this.document.Notifier.NotifyArea();
		}
		#endregion


		#region Order
		// Met au premier plan tous les objets s�lectionn�s.
		public void OrderUpSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.OrderSelection(1);
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		// Met � l'arri�re plan tous les objets s�lectionn�s.
		public void OrderDownSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.OrderSelection(-1);
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		protected void OrderSelection(int dir)
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			AbstractObject layer = context.RootObject();

			int total = layer.Objects.Count;
			int iSrc = 0;
			int iDst = (dir < 0) ? 0 : total-1;
			do
			{
				AbstractObject obj = layer.Objects[iSrc] as AbstractObject;
				if ( obj.IsSelected )
				{
					layer.Objects.RemoveAt(iSrc);
					layer.Objects.Insert(iDst, obj);
					this.document.Notifier.NotifyArea(obj.BoundingBox);
					if ( dir < 0 )
					{
						iSrc ++;
						iDst ++;
					}
					else
					{
						total --;
					}
				}
				else
				{
					iSrc ++;
				}
			}
			while ( iSrc < total );
		}
		#endregion


		#region Operations
		// D�place tous les objets s�lectionn�s.
		public void MoveSelection(Point move)
		{
			this.PrepareOper();
			this.ActiveViewer.Selector.OperMove(move);
			this.TerminateOper();
		}

		// Tourne tous les objets s�lectionn�s.
		public void RotateSelection(double angle)
		{
			this.PrepareOper();
			this.ActiveViewer.Selector.OperRotate(angle);
			this.TerminateOper();
		}

		// Miroir de tous les objets s�lectionn�s. 
		public void MirrorSelection(bool horizontal)
		{
			this.PrepareOper();
			this.ActiveViewer.Selector.OperMirror(horizontal);
			this.TerminateOper();
		}

		// Zoom de tous les objets s�lectionn�s.
		public void ZoomSelection(double scale)
		{
			this.PrepareOper();
			this.ActiveViewer.Selector.OperZoom(scale);
			this.TerminateOper();
		}

		// Pr�pare pour l'op�ration.
		protected void PrepareOper()
		{
			this.document.IsDirtySerialize = true;
			this.OpletQueueBeginAction();

			this.operInitialSelector = this.ActiveViewer.GlobalSelect;
			if ( !this.operInitialSelector )
			{
				this.ActiveViewer.GlobalSelect = true;
			}
		}

		// Termine l'op�ration.
		protected void TerminateOper()
		{
			if ( !this.operInitialSelector )
			{
				this.ActiveViewer.GlobalSelect = false;
			}

			this.OpletQueueValidateAction();
		}
		#endregion


		#region Group
		// Fusionne tous les objets s�lectionn�s.
		public void MergeSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.Ungroup();
				this.Group();
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		// Groupe tous les objets s�lectionn�s.
		public void GroupSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.Group();
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		// S�pare tous les objets s�lectionn�s.
		public void UngroupSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.Ungroup();
				this.ActiveViewer.GlobalSelect = ( this.TotalSelected > 1 );
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		protected void Group()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			AbstractObject layer = context.RootObject();
			System.Collections.ArrayList extract = new System.Collections.ArrayList();

			// Extrait tous les objets s�lectionn�s dans la liste extract.
			Rectangle bbox = Rectangle.Empty;
			foreach ( AbstractObject obj in this.document.Flat(layer, true) )
			{
				extract.Add(obj);
				bbox.MergeWith(obj.BoundingBoxGroup);
			}

			// Supprime les objets s�lectionn�s de la liste principale, sans
			// supprimer les propri�t�s.
			bool bDo = false;
			do
			{
				bDo = false;
				foreach ( AbstractObject obj in this.document.Flat(layer, true) )
				{
					this.document.Modifier.TotalSelected --;
					layer.Objects.Remove(obj);
					bDo = true;
					break;
				}
			}
			while ( bDo );

			// Cr�e l'objet groupe.
			ObjectGroup group = new ObjectGroup(this.document, null);
			layer.Objects.Add(group);
			group.UpdateDim(bbox);
			group.Select();
			this.TotalSelected ++;

			// Remet les objets extraits dans le groupe.
			foreach ( AbstractObject obj in extract )
			{
				obj.Deselect();
				group.Objects.Add(obj);
			}

			this.document.Notifier.NotifyArea(group.BoundingBox);
			this.ActiveViewer.Selector.Visible = false;
		}

		protected void Ungroup()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			AbstractObject layer = context.RootObject();
			int total = layer.Objects.Count;
			int index = 0;
			do
			{
				AbstractObject obj = layer.Objects[index] as AbstractObject;
				if ( obj.IsSelected && obj is ObjectGroup )
				{
					int rank = index+1;
					foreach ( AbstractObject inside in this.document.Flat(obj) )
					{
						inside.Select();
						layer.Objects.Insert(rank++, inside);
						this.TotalSelected ++;
					}
					this.document.Notifier.NotifyArea(obj.BoundingBox);
					obj.Dispose();
					layer.Objects.RemoveAt(index);
					this.TotalSelected --;
					index += obj.Objects.Count-1;
					total = layer.Objects.Count;
				}
				index ++;
			}
			while ( index < total );
		}

		// Entre dans tous les objets s�lectionn�s.
		public void InsideSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction() )
			{
				AbstractObject group = this.RetOnlySelectedObject();
				if ( group != null && group is ObjectGroup )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					AbstractObject layer = context.RootObject();
					group.Deselect();
					int index = layer.Objects.IndexOf(group);
					context.RootStackPush(index);
					this.DirtyCounters();

					this.document.Notifier.NotifyArea(this.ActiveViewer);
					this.document.Notifier.NotifySelectionChanged();
				}

				this.OpletQueueValidateAction();
			}
		}

		// Sort de tous les objets s�lectionn�s.
		public void OutsideSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction() )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				if ( !context.RootStackIsBase )
				{
					this.DeselectAll();
					this.GroupUpdateParents();
					int index = context.RootStackPop();
					AbstractObject layer = context.RootObject();
					AbstractObject group = layer.Objects[index] as AbstractObject;
					group.Select();
					this.Tool = "Select";
					this.DirtyCounters();
				}

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		protected Rectangle RetBbox(AbstractObject group)
		{
			Rectangle bbox = Rectangle.Empty;
			foreach ( AbstractObject obj in this.document.Flat(group) )
			{
				bbox.MergeWith(obj.BoundingBoxGroup);
			}
			return bbox;
		}

		// Adapte les dimensions de tous les groupes fils.
		public void GroupUpdateChildrens()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			AbstractObject layer = context.RootObject();
			foreach ( AbstractObject obj in this.document.Deep(layer) )
			{
				ObjectGroup group = obj as ObjectGroup;
				if ( group != null )
				{
					this.document.Notifier.NotifyArea(group.BoundingBox);
					Rectangle bbox = this.RetBbox(group);
					group.UpdateDim(bbox);
					this.document.Notifier.NotifyArea(group.BoundingBox);
				}
			}
		}

		// Adapte les dimensions de tous les groupes parents.
		public void GroupUpdateParents()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			if ( context.RootStackIsBase )  return;

			int deep = context.RootStackDeep;
			for ( int i=deep ; i>=3 ; i-- )
			{
				ObjectGroup group = context.RootObject(i) as ObjectGroup;
				if ( group != null )
				{
					this.document.Notifier.NotifyArea(group.BoundingBox);
					Rectangle bbox = this.RetBbox(group);
					group.UpdateDim(bbox);
					this.document.Notifier.NotifyArea(group.BoundingBox);
				}
			}
		}
		#endregion


		#region Hide
		// Cache tous les objets s�lectionn�s du calque courant.
		public void HideSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction() )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject layer = context.RootObject();
				foreach ( AbstractObject obj in this.document.Flat(layer) )
				{
					if ( !(obj is ObjectPage) && !(obj is ObjectLayer) )
					{
						if ( obj.IsSelected && !obj.IsHide )
						{
							obj.Deselect();
							this.TotalSelected --;

							this.HideObjectAndSoons(obj);
							this.TotalHide ++;
						}
					}
				}
				this.ActiveViewer.GlobalSelect = false;
				this.OpletQueueValidateAction();
			}
		}

		// Cache tous les objets non s�lectionn�s du calque courant.
		public void HideRest()
		{
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction() )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject layer = context.RootObject();
				foreach ( AbstractObject obj in this.document.Flat(layer) )
				{
					if ( !(obj is ObjectPage) && !(obj is ObjectLayer) )
					{
						if ( !obj.IsSelected && !obj.IsHide )
						{
							this.HideObjectAndSoons(obj);
							this.TotalHide ++;
						}
					}
				}
				this.OpletQueueValidateAction();
			}
		}

		// Cache un objet et tous ses fils.
		protected void HideObjectAndSoons(AbstractObject obj)
		{
			obj.IsHide = true;

			if ( obj.Objects != null && obj.Objects.Count > 0 )
			{
				int total = obj.Objects.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					AbstractObject sub = obj.Objects[i] as AbstractObject;
					this.HideObjectAndSoons(sub);
				}
			}
		}

		// Vend visible tous les objets cach�s de la page (donc dans tous les calques).
		public void HideCancel()
		{
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction() )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject page = context.RootObject(1);
				foreach ( AbstractObject obj in this.document.Deep(page) )
				{
					if ( !(obj is ObjectPage) && !(obj is ObjectLayer) )
					{
						obj.IsHide = false;
					}
				}
				this.totalHide = 0;
				this.totalPageHide = 0;
				this.OpletQueueValidateAction();
			}
		}
		#endregion


		#region Page
		// Commence un changement de page.
		public void InitiateChangingPage()
		{
			int rank = this.ActiveViewer.DrawingContext.CurrentPage;
			ObjectPage page = this.document.Objects[rank] as ObjectPage;
			page.CurrentLayer = this.ActiveViewer.DrawingContext.CurrentLayer;

			this.DeselectAll();
		}

		// Termine un changement de page.
		public void TerminateChangingPage(int rank)
		{
			ObjectPage page = this.document.Objects[rank] as ObjectPage;
			int layer = page.CurrentLayer;
			this.ActiveViewer.DrawingContext.PageLayer(rank, layer);
			this.DirtyCounters();
		}

		// Cr�e une nouvelle page.
		public void PageCreate(int rank, string name)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.InitiateChangingPage();

				UndoableList list = this.document.Objects;  // liste des pages
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count);

				ObjectPage page = new ObjectPage(this.document, null);
				page.Name = name;
				list.Insert(rank, page);

				ObjectLayer layer = new ObjectLayer(this.document, null);
				page.Objects.Add(layer);

				this.TerminateChangingPage(rank);

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyPagesChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Duplique une nouvelle page.
		public void PageDuplicate(int rank, string name)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.InitiateChangingPage();

				UndoableList list = this.document.Objects;  // liste des pages
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);

				ObjectPage srcPage = list[rank] as ObjectPage;

				ObjectPage page = new ObjectPage(this.document, srcPage);
				if ( name == "" )
				{
					page.Name = Misc.CopyName(srcPage.Name);
				}
				else
				{
					page.Name = name;
				}
				list.Insert(rank+1, page);

				UndoableList src = srcPage.Objects;
				UndoableList dst = page.Objects;
				Modifier.Duplicate(this.document, this.document, src, dst, false, new Point(0,0), false);

				this.TerminateChangingPage(rank+1);

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyPagesChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Supprime une page.
		public void PageDelete(int rank)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.DeselectAll();

				UndoableList list = this.document.Objects;  // liste des pages
				if ( list.Count <= 1 )  return;  // il doit rester une page
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);

				UndoableList pages = this.document.Objects;
				ObjectPage page = pages[rank] as ObjectPage;
				page.Dispose();
				list.RemoveAt(rank);

				rank = System.Math.Min(rank, list.Count-1);
				this.ActiveViewer.DrawingContext.CurrentPage = rank;

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyPagesChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Permute deux pages.
		public void PageSwap(int rank1, int rank2)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				UndoableList list = this.document.Objects;  // liste des pages
				rank1 = System.Math.Max(rank1, 0);
				rank1 = System.Math.Min(rank1, list.Count-1);
				rank2 = System.Math.Max(rank2, 0);
				rank2 = System.Math.Min(rank2, list.Count-1);

				UndoableList pages = this.document.Objects;
				ObjectPage temp = pages[rank1] as ObjectPage;
				pages.RemoveAt(rank1);
				pages.Insert(rank2, temp);

				this.ActiveViewer.DrawingContext.CurrentPage = rank2;

				this.document.Notifier.NotifyArea();
				this.document.Notifier.NotifyPagesChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Retourne le nom d'une page.
		public string PageName(int rank)
		{
			UndoableList pages = this.document.Objects;
			ObjectPage page = pages[rank] as ObjectPage;
			return page.Name;
		}

		// Change le nom d'une page.
		public void PageName(int rank, string name)
		{
			this.document.IsDirtySerialize = true;
			using ( this.OpletQueueBeginAction("ChangePageName") )
			{
				UndoableList pages = this.document.Objects;
				ObjectPage page = pages[rank] as ObjectPage;
				page.Name = name;

				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyPageChanged(page);
				this.OpletQueueValidateAction();
			}
		}
		#endregion


		#region Layer
		// Commence un changement de calque.
		public void InitiateChangingLayer()
		{
			this.DeselectAll();
		}

		// Termine un changement de calque.
		public void TerminateChangingLayer(int rank)
		{
			int page = this.ActiveViewer.DrawingContext.CurrentPage;
			this.ActiveViewer.DrawingContext.PageLayer(page, rank);
			this.DirtyCounters();
		}

		// Cr�e un nouveau calque.
		public void LayerCreate(int rank, string name)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.DeselectAll();

				// Liste des calques:
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count);

				ObjectLayer layer = new ObjectLayer(this.document, null);
				layer.Name = name;
				list.Insert(rank, layer);

				this.ActiveViewer.DrawingContext.CurrentLayer = rank;

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Duplique un calque.
		public void LayerDuplicate(int rank, string name)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.DeselectAll();

				// Liste des calques:
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);

				ObjectLayer srcLayer = list[rank] as ObjectLayer;

				ObjectLayer layer = new ObjectLayer(this.document, srcLayer);
				if ( name == "" )
				{
					layer.Name = Misc.CopyName(srcLayer.Name);
				}
				else
				{
					layer.Name = name;
				}
				list.Insert(rank+1, layer);

				UndoableList src = srcLayer.Objects;
				UndoableList dst = layer.Objects;
				Modifier.Duplicate(this.document, this.document, src, dst, false, new Point(0,0), false);

				this.ActiveViewer.DrawingContext.CurrentLayer = rank+1;

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Supprime un calque.
		public void LayerDelete(int rank)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.DeselectAll();

				// Liste des calques:
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				if ( list.Count <= 1 )  return;  // il doit rester un calque
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);

				UndoableList layers = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				ObjectLayer layer = layers[rank] as ObjectLayer;
				layer.Dispose();
				list.RemoveAt(rank);

				rank = System.Math.Min(rank, list.Count-1);
				this.ActiveViewer.DrawingContext.CurrentLayer = rank;

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Permute deux calques.
		public void LayerSwap(int rank1, int rank2)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				// Liste des calques:
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				rank1 = System.Math.Max(rank1, 0);
				rank1 = System.Math.Min(rank1, list.Count-1);
				rank2 = System.Math.Max(rank2, 0);
				rank2 = System.Math.Min(rank2, list.Count-1);

				UndoableList layers = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				ObjectLayer temp = layers[rank1] as ObjectLayer;
				layers.RemoveAt(rank1);
				layers.Insert(rank2, temp);

				this.ActiveViewer.DrawingContext.CurrentLayer = rank2;

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Retourne le nom d'un calque.
		public string LayerName(int rank)
		{
			UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
			ObjectLayer layer = list[rank] as ObjectLayer;
			return layer.Name;
		}

		// Change le nom d'un calque.
		public void LayerName(int rank, string name)
		{
			this.document.IsDirtySerialize = true;
			using ( this.OpletQueueBeginAction("ChangeLayerName") )
			{
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				ObjectLayer layer = list[rank] as ObjectLayer;
				layer.Name = name;

				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyLayerChanged(layer);
				this.OpletQueueValidateAction();
			}
		}
		#endregion


		#region Properties
		// Mode de repr�sentation pour le panneau des propri�t�s.
		public bool PropertiesDetail
		{
			get
			{
				return this.propertiesDetail;
			}

			set
			{
				if ( this.propertiesDetail != value )
				{
					this.propertiesDetail = value;
					this.document.Notifier.NotifySelectionChanged();
				}
			}
		}

		// Indique si on est en mode "d�tail" avec plus d'un objet s�lectionn�.
		public bool PropertiesDetailMany
		{
			get
			{
				return (this.propertiesDetail && this.TotalSelected > 1);
			}
		}

		// Ajoute une nouvelle propri�t�.
		public void PropertyAdd(AbstractProperty property)
		{
			this.PropertyList(property).Add(property);

			if ( property.IsStyle )
			{
				this.document.Notifier.NotifyStyleChanged();
			}
		}

		// Supprime une propri�t�.
		public void PropertyRemove(AbstractProperty property)
		{
			this.PropertyList(property).Remove(property);

			if ( property.IsStyle )
			{
				this.document.Notifier.NotifyStyleChanged();
			}
		}

		// Donne la liste � utiliser pour une propri�t�.
		public UndoableList PropertyList(AbstractProperty property)
		{
			return this.PropertyList(property.IsStyle,  property.IsSelected);
		}

		public UndoableList PropertyList(bool style, bool selected)
		{
			if ( style )
			{
				return this.document.PropertiesStyle;
			}
			else
			{
				if ( selected )
				{
					return this.document.PropertiesSel;
				}
				else
				{
					return this.document.PropertiesAuto;
				}
			}
		}

		// Ajoute toutes les propri�t�s des objets s�lectionn�s dans une liste.
		// Tient compte du mode propertiesDetail.
		public void PropertiesList(System.Collections.ArrayList list)
		{
			this.document.Modifier.OpletQueueEnable = false;
			list.Clear();

			if ( this.IsTool )  // outil select, edit, loupe, etc. ?
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject layer = context.RootObject();
				foreach ( AbstractObject obj in this.document.Flat(layer, true) )
				{
					obj.PropertiesList(list, this.propertiesDetail);
				}
			}
			else	// choix d'un objet � cr�er ?
			{
				// Cr�e un objet factice (document = null), c'est-�-dire sans
				// propri�t�s, qui servira juste de filtre pour objectMemory.
				AbstractObject dummy = AbstractObject.CreateObject(null, this.tool, null);
				this.objectMemory.PropertiesList(list, dummy);
				dummy.Dispose();
			}

			list.Sort();
			this.document.Modifier.OpletQueueEnable = true;
		}
		#endregion


		#region Zoom
		// Zoom sur les objets s�lectionn�s.
		public void ZoomSel()
		{
			Rectangle bbox = this.SelectedBbox;
			if ( bbox.IsEmpty )  return;
			bbox.Inflate(2);
			this.ZoomChange(bbox.BottomLeft, bbox.TopRight);
		}

		// Change le zoom d'un certain facteur pour agrandir une zone rectangulaire.
		public void ZoomChange(Point p1, Point p2)
		{
			Viewer viewer = this.ActiveViewer;
			DrawingContext context = this.ActiveViewer.DrawingContext;

			if ( p1.X == p2.X || p1.Y == p2.Y )  return;
			double fx = viewer.Width/(System.Math.Abs(p1.X-p2.X)*context.ScaleX);
			double fy = viewer.Height/(System.Math.Abs(p1.Y-p2.Y)*context.ScaleY);
			double factor = System.Math.Min(fx, fy);
			this.ZoomChange(factor, (p1+p2)/2);
		}

		// Change le zoom d'un certain facteur, avec centrage au centre du dessin.
		public void ZoomChange(double factor)
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;

			Point center = new Point();
			center.X = -context.OriginX+(this.document.Size.Width/context.Zoom)/2;
			center.Y = -context.OriginY+(this.document.Size.Height/context.Zoom)/2;
			this.ZoomChange(factor, center);
		}

		// Change le zoom d'un certain facteur, avec centrage quelconque.
		public void ZoomChange(double factor, Point center)
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;

			double newZoom = context.Zoom*factor;
			newZoom = System.Math.Max(newZoom, this.ZoomMin);
			newZoom = System.Math.Min(newZoom, this.ZoomMax);
			if ( newZoom == context.Zoom )  return;

			this.ZoomMemorize();
			Point origin = new Point();
			origin.X = center.X-(this.document.Size.Width/newZoom)/2;
			origin.Y = center.Y-(this.document.Size.Height/newZoom)/2;
			context.ZoomAndOrigin(newZoom, -origin);
		}

		// Retourne le nombre de zoom m�moris�s.
		public int ZoomMemorizeCount
		{
			get { return this.zoomHistory.Count; }
		}

		// M�morise le zoom actuel.
		public void ZoomMemorize()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;

			ZoomHistory.ZoomElement item = new ZoomHistory.ZoomElement();
			item.Zoom = context.Zoom;
			item.Ox   = context.OriginX;
			item.Oy   = context.OriginY;
			this.zoomHistory.Add(item);
		}

		// Revient au niveau de zoom pr�c�dent.
		public void ZoomPrev()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;

			ZoomHistory.ZoomElement item = this.zoomHistory.Remove();
			if ( item == null )  return;
			context.ZoomAndOrigin(item.Zoom, item.Ox, item.Oy);
		}

		// Retourne le zoom minimal.
		public double ZoomMin
		{
			get
			{
				double x = this.SizeArea.Width/this.document.Size.Width;
				double y = this.SizeArea.Height/this.document.Size.Height;
				return 1.0/System.Math.Min(x,y);
			}
		}

		// Retourne le zoom maximal.
		public double ZoomMax
		{
			get
			{
				return 8.0;
			}
		}
		#endregion


		#region StyleMenu
		// Construit le menu des styles.
		public VMenu CreateStyleMenu(AbstractProperty original)
		{
			this.menuProperty = original;

			VMenu menu = new VMenu();
			MenuItem item;

			bool isStyle = false;
			bool isAuto  = false;
			if ( original.IsMulti )
			{
				foreach ( AbstractProperty property in original.Owners )
				{
					if ( property.IsStyle )  isStyle = true;
					else                     isAuto  = true;
				}
			}
			else
			{
				isStyle = original.IsStyle;
				isAuto = !original.IsStyle;
			}

			if ( isAuto )
			{
				item = new MenuItem("StyleMake", @"file:images/stylemake.icon", "Cr�er un nouveau style", "");
				item.Pressed += new MessageEventHandler(this.HandleMenuPressed);
				menu.Items.Add(item);
			}

			if ( isStyle )
			{
				item = new MenuItem("StyleFree", @"file:images/stylefree.icon", "Rendre ind�pendant du style", "");
				item.Pressed += new MessageEventHandler(this.HandleMenuPressed);
				menu.Items.Add(item);
			}

			bool first = true;
			int total = this.document.PropertiesStyle.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				AbstractProperty property = this.document.PropertiesStyle[i] as AbstractProperty;
				if ( property.Type != original.Type )  continue;

				if ( first )
				{
					menu.Items.Add(new MenuSeparator());
					first = false;
				}

				string icon = @"file:images/activeno.icon";
				if ( property == original )
				{
					icon = @"file:images/activeyes.icon";
				}

				item = new MenuItem("StyleUse", icon, property.StyleName, "", i.ToString());
				item.Pressed += new MessageEventHandler(this.HandleMenuPressed);
				menu.Items.Add(item);
			}

			menu.AdjustSize();
			return menu;
		}

		private void HandleMenuPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;

			if ( item.Command == "StyleMake" )
			{
				this.StyleMake(this.menuProperty);
			}

			if ( item.Command == "StyleFree" )
			{
				this.StyleFree(this.menuProperty);
			}

			if ( item.Command == "StyleUse" )
			{
				int rank = System.Convert.ToInt32(item.Name);
				AbstractProperty style = this.document.PropertiesStyle[rank] as AbstractProperty;
				this.StyleUse(this.menuProperty, style);
			}

			this.menuProperty = null;
		}

		// Fabrique un nouveau style.
		public void StyleMake(AbstractProperty property)
		{
			if ( this.IsTool )  // // propri�t�s des objets s�lectionn�s ?
			{
				using ( this.OpletQueueBeginAction() )
				{
					if ( property.IsMulti )
					{
						AbstractProperty style = AbstractProperty.NewProperty(this.document, property.Type);
						property.CopyTo(style);
						style.IsStyle = true;
						style.StyleName = Modifier.GetNextStyleName();
						this.PropertyList(style).Add(style);

						DrawingContext context = this.ActiveViewer.DrawingContext;
						AbstractObject layer = context.RootObject();
						foreach ( AbstractObject obj in this.document.Flat(layer, true) )
						{
							obj.UseProperty(style);
						}
					}
					else
					{
						this.PropertyList(property).Remove(property);
						property.IsStyle = true;
						property.StyleName = Modifier.GetNextStyleName();
						this.PropertyList(property).Add(property);
					}

					this.document.Notifier.NotifyStyleChanged();
					this.document.Notifier.NotifySelectionChanged();

					this.OpletQueueValidateAction();
				}
			}
			else	// propri�t� de objectMemory ?
			{
				using ( this.OpletQueueBeginAction() )
				{
					AbstractProperty style = AbstractProperty.NewProperty(this.document, property.Type);
					property.CopyTo(style);
					style.IsStyle = true;
					style.StyleName = Modifier.GetNextStyleName();
					this.PropertyList(true, style.IsSelected).Add(style);
					this.objectMemory.ChangeProperty(style);

					this.document.Notifier.NotifyStyleChanged();

					this.OpletQueueValidateAction();
				}
			}
		}

		// Lib�re un style.
		public void StyleFree(AbstractProperty property)
		{
			if ( this.IsTool )  // propri�t�s des objets s�lectionn�s ?
			{
				using ( this.OpletQueueBeginAction() )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					AbstractObject layer = context.RootObject();
					foreach ( AbstractObject obj in this.document.Flat(layer, true) )
					{
						obj.FreeProperty(property);
					}

					this.document.Notifier.NotifyStyleChanged();
					this.document.Notifier.NotifySelectionChanged();

					this.OpletQueueValidateAction();
				}
			}
			else	// propri�t� de objectMemory ?
			{
				using ( this.OpletQueueBeginAction() )
				{
					AbstractProperty free = AbstractProperty.NewProperty(this.document, property.Type);
					property.CopyTo(free);
					free.IsOnlyForCreation = true;
					free.IsStyle = false;
					free.StyleName = "";
					this.objectMemory.ChangeProperty(free);

					this.document.Notifier.NotifyStyleChanged();

					this.OpletQueueValidateAction();
				}
			}
		}

		// Utilise un style.
		public void StyleUse(AbstractProperty property, AbstractProperty style)
		{
			if ( this.IsTool )  // propri�t�s des objets s�lectionn�s ?
			{
				using ( this.OpletQueueBeginAction() )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					AbstractObject layer = context.RootObject();
					foreach ( AbstractObject obj in this.document.Flat(layer, true) )
					{
						if ( property.IsMulti )
						{
							obj.UseProperty(style);
						}
						else
						{
							if ( obj.Property(style.Type) == property )
							{
								obj.UseProperty(style);
							}
						}
					}

					this.document.Notifier.NotifyStyleChanged();
					this.document.Notifier.NotifySelectionChanged();

					this.OpletQueueValidateAction();
				}
			}
			else	// propri�t� de objectMemory ?
			{
				this.OpletQueueEnable = false;
				this.objectMemory.ChangeProperty(style);
				this.OpletQueueEnable = true;

				this.document.Notifier.NotifySelectionChanged();
			}
		}

		// Cr�e un nouveau style d'un type quelconque.
		public void StyleCreate(int rank, PropertyType type)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				UndoableList list = this.document.PropertiesStyle;
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);

				AbstractProperty style = AbstractProperty.NewProperty(this.document, type);
				style.IsStyle = true;
				style.IsOnlyForCreation = false;
				style.StyleName = Modifier.GetNextStyleName();
				list.Insert(rank+1, style);
				list.Selected = rank+1;

				this.document.Notifier.NotifyStyleChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Duplique un style.
		public void StyleDuplicate(int rank)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				UndoableList list = this.document.PropertiesStyle;
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);
				AbstractProperty property = list[rank] as AbstractProperty;

				AbstractProperty style = AbstractProperty.NewProperty(this.document, property.Type);
				property.CopyTo(style);
				style.IsStyle = true;
				style.IsOnlyForCreation = false;
				style.StyleName = Modifier.GetNextStyleName();
				list.Insert(rank+1, style);
				list.Selected = rank+1;

				this.document.Notifier.NotifyStyleChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Supprime un style.
		public void StyleDelete(int rank)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				UndoableList list = this.document.PropertiesStyle;
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);
				AbstractProperty property = list[rank] as AbstractProperty;

				DrawingContext context = this.ActiveViewer.DrawingContext;
				AbstractObject doc = context.RootObject(0);
				foreach ( AbstractObject obj in this.document.Deep(doc) )
				{
					obj.FreeProperty(property);
				}

				list.RemoveAt(rank);

				rank = System.Math.Min(rank, list.Count-1);
				list.Selected = rank;

				this.document.Notifier.NotifyStyleChanged();
				this.document.Notifier.NotifySelectionChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Permute deux styles.
		public void StyleSwap(int rank1, int rank2)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				UndoableList list = this.document.PropertiesStyle;
				rank1 = System.Math.Max(rank1, 0);
				rank1 = System.Math.Min(rank1, list.Count-1);
				rank2 = System.Math.Max(rank2, 0);
				rank2 = System.Math.Min(rank2, list.Count-1);

				AbstractProperty temp = list[rank1] as AbstractProperty;
				list.RemoveAt(rank1);
				list.Insert(rank2, temp);
				list.Selected = rank2;

				this.document.Notifier.NotifyStyleChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Le style s�lectionn� reprend la bonne propri�t� de l'objet mod�le
		// point� par la souris avec la pipette.
		public void PickerProperty(AbstractObject model)
		{
			int sel = this.document.PropertiesStyle.Selected;
			if ( sel < 0 )  return;
			AbstractProperty dst = this.document.PropertiesStyle[sel] as AbstractProperty;
			AbstractProperty src = model.Property(dst.Type);
			if ( src == null )  return;

			using ( this.OpletQueueBeginAction() )
			{
				dst.PickerProperty(src);
				this.OpletQueueValidateAction();
			}
			this.document.IsDirtySerialize = true;
		}

		// Donne le prochain nom unique de style.
		protected static string GetNextStyleName()
		{
			return string.Format("Style {0}", Modifier.GetNextUniqueStyleId());
		}
		#endregion


		#region UniqueId
		// Retourne le prochain identificateur unique pour les objets.
		public static int GetNextUniqueObjectId()
		{
			return ++Modifier.uniqueObjectId;
		}

		// Modification de l'identificateur unique.
		public static int UniqueObjectId
		{
			get { return Modifier.uniqueObjectId; }
			set { Modifier.uniqueObjectId = value; }
		}

		protected static int uniqueObjectId = 0;

		// Retourne le prochain identificateur unique pour les noms de style.
		public static int GetNextUniqueStyleId()
		{
			return ++Modifier.uniqueStyleId;
		}

		// Modification de l'identificateur unique.
		public static int UniqueStyleId
		{
			get { return Modifier.uniqueStyleId; }
			set { Modifier.uniqueStyleId = value; }
		}

		protected static int uniqueStyleId = 0;
		#endregion


		#region OpletTool
		// Ajoute un oplet pour m�moriser l'outil.
		protected void InsertOpletTool()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletTool oplet = new OpletTool(this.document);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// M�morise l'outil.
		protected class OpletTool : AbstractOplet
		{
			public OpletTool(Document host)
			{
				this.host = host;
				this.tool = host.Modifier.Tool;
			}

			protected void Swap()
			{
				string temp = host.Modifier.tool;
				host.Modifier.tool = this.tool;  // host.Modifier.tool <-> this.tool
				this.tool = temp;

				this.host.Notifier.NotifyToolChanged();
			}

			public override IOplet Undo()
			{
				this.Swap();
				return this;
			}

			public override IOplet Redo()
			{
				this.Swap();
				return this;
			}

			protected Document				host;
			protected string				tool;
		}
		#endregion


		#region OpletQueue
		// D�termine si les actions seront annulables ou non.
		public bool OpletQueueEnable
		{
			get
			{
				return this.opletQueue.IsEnabled && !this.opletSkip;
			}

			set
			{
				if ( value )  this.opletQueue.Enable();
				else          this.opletQueue.Disable();
			}
		}

		// D�but d'une zone annulable.
		public System.IDisposable OpletQueueBeginAction()
		{
			return this.OpletQueueBeginAction("", 0);
		}

		public System.IDisposable OpletQueueBeginAction(string cmd)
		{
			return this.OpletQueueBeginAction(cmd, 0);
		}

		public System.IDisposable OpletQueueBeginAction(string cmd, int id)
		{
			this.opletLevel ++;
			if ( this.opletLevel > 1 )  return null;

			if ( cmd == this.opletLastCmd && id == this.opletLastId )
			{
				if ( cmd == "ChangeProperty"  ||
					 cmd == "ChangePageName"  ||
					 cmd == "ChangeLayerName" )
				{
					this.opletSkip = true;
					return null;
				}
			}

			this.opletCreate = (cmd == "Create");
			this.opletLastCmd = cmd;
			this.opletLastId = id;

			return this.opletQueue.BeginAction();
		}

		// Fin d'une zone annulable.
		public void OpletQueueValidateAction()
		{
			this.opletLevel --;
			if ( this.opletLevel > 0 )  return;

			if ( this.opletSkip )
			{
				this.opletSkip = false;
			}
			else
			{
				this.opletQueue.ValidateAction();
				this.document.Notifier.NotifyUndoRedoChanged();
			}
		}

		// Fin d'une zone annulable.
		public void OpletQueueCancelAction()
		{
			this.opletLevel --;
			if ( this.opletLevel > 0 )  return;

			if ( this.opletSkip )
			{
				this.opletSkip = false;
			}
			else
			{
				this.opletQueue.CancelAction();
				this.document.Notifier.NotifyUndoRedoChanged();
			}
		}
		#endregion


		protected Document						document;
		protected Viewer						activeViewer;
		protected System.Collections.ArrayList	attachViewers;
		protected System.Collections.ArrayList	attachContainers;
		protected string						tool;
		protected bool							propertiesDetail;
		protected bool							dirtyCounters;
		protected int							totalSelected;
		protected int							totalHide;
		protected int							totalPageHide;
		protected ZoomHistory					zoomHistory;
		protected OpletQueue					opletQueue;
		protected int							opletLevel;
		protected bool							opletSkip;
		protected bool							opletCreate;
		protected string						opletLastCmd;
		protected int							opletLastId;
		protected ObjectMemory					objectMemory;
		protected AbstractProperty				menuProperty;
		protected bool							operInitialSelector;
		protected AbstractContainer				activeContainer;
	}
}
