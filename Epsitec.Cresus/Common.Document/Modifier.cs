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
			this.CreateObjectMemory();

			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.RealUnitDimension = RealUnitType.None;
				this.duplicateMove = new Point(1.0, 1.0);
			}
			else
			{
				this.RealUnitDimension = RealUnitType.DimensionMillimeter;
				this.duplicateMove = new Point(50.0, 50.0);  // 5mm
			}

			int total = 0;
			foreach ( int value in System.Enum.GetValues(typeof(Properties.Type)) )
			{
				Properties.Type type = (Properties.Type)value;
				total ++;
			}
			this.isPropertiesExtended = new bool[total];
			for ( int i=0 ; i<total ; i++ )
			{
				this.isPropertiesExtended[i] = false;
			}
		}

		// Outil sélectionné dans la palette.
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
					if ( this.ActiveViewer.IsCreating )
					{
						this.ActiveViewer.CreateEnding(false);
					}

					bool isCreate = this.opletCreate;
					this.OpletQueueBeginAction();
					this.InsertOpletTool();

					Objects.Abstract editObject = this.RetEditObject();

					if ( this.tool == "HotSpot" || value == "HotSpot" )
					{
						this.document.Notifier.NotifyArea(this.ActiveViewer);
					}

					this.tool = value;

					if ( this.tool == "Select" && isCreate )  // on vient de créer un objet ?
					{
						DrawingContext context = this.ActiveViewer.DrawingContext;
						Objects.Abstract layer = context.RootObject();
						Objects.Abstract obj = layer.Objects[layer.Objects.Count-1] as Objects.Abstract;
						this.ActiveViewer.Select(obj, false, false);
					}

					else if ( this.tool == "Edit" && isCreate )  // on vient de créer un objet ?
					{
						DrawingContext context = this.ActiveViewer.DrawingContext;
						Objects.Abstract layer = context.RootObject();
						Objects.Abstract obj = layer.Objects[layer.Objects.Count-1] as Objects.Abstract;
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
							Objects.Abstract sel = this.RetOnlySelectedObject();
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

					else if ( !IsTool )  // choix d'un objet à créer ?
					{
						this.DeselectAll();
					}

					this.OpletQueueValidateAction();
					this.document.Notifier.NotifyToolChanged();
					this.document.Notifier.NotifySelectionChanged();
				}
			}
		}

		// Indique si l'outil sélectionné n'est pas un objet.
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

		// Indique si l'outil sélectionné est un objet de type "texte".
		protected bool IsToolText
		{
			get
			{
				if ( this.tool == "ObjectTextLine"  )  return true;
				if ( this.tool == "ObjectTextBox"   )  return true;
				return false;
			}
		}


		public OpletQueue OpletQueue
		{
			get { return this.opletQueue; }
		}

		public Objects.Memory ObjectMemory
		{
			get { return this.objectMemory; }
		}

		public Objects.Memory ObjectMemoryText
		{
			get { return this.objectMemoryText; }
		}

		public Objects.Memory ObjectMemoryTool
		{
			get { return this.IsToolText ? this.objectMemoryText : this.objectMemory; }
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


		#region RealUnit
		// Choix de l'unité de dimension par défaut.
		public RealUnitType RealUnitDimension
		{
			get
			{
				return this.realUnitDimension;
			}
			
			set
			{
				this.realUnitDimension = value;

				switch ( this.realUnitDimension )
				{
					case RealUnitType.DimensionMillimeter:
						this.realScale = 10.0;
						break;

					case RealUnitType.DimensionCentimeter:
						this.realScale = 100.0;
						break;

					case RealUnitType.DimensionInch:
						this.realScale = 254.0;
						break;

					default:
						this.realScale = 1.0;
						break;
				}

				this.AdaptAllTextFieldReal();
			}
		}

		// Facteur d'échelle.
		public double RealScale
		{
			get
			{
				return this.realScale;
			}
		}

		// Adapte un TextFieldReal pour éditer un scalaire.
		public void AdaptTextFieldRealScalar(TextFieldReal field)
		{
			field.UnitType = RealUnitType.Scalar;
			field.Step = 1.0M;
			field.Resolution = 1.0M;
		}

		// Adapte un TextFieldReal pour éditer un pourcent.
		public void AdaptTextFieldRealPercent(TextFieldReal field)
		{
			field.UnitType = RealUnitType.Percent;
			field.Step = 1.0M;
			field.Resolution = 0.1M;
		}

		// Adapte un TextFieldReal pour éditer une dimension.
		public void AdaptTextFieldRealDimension(TextFieldReal field)
		{
			field.UnitType = this.realUnitDimension;
			field.Scale = (decimal) this.realScale;

			if ( this.document.Type == DocumentType.Pictogram )
			{
				field.InternalMinValue = 100.0M * field.FactorMinRange;
				field.InternalMaxValue = 100.0M * field.FactorMaxRange;
				field.Step = 1.0M * field.FactorStep;
				field.Resolution = 0.1M;
			}
			else
			{
				field.InternalMinValue = 10000.0M * field.FactorMinRange;
				field.InternalMaxValue = 10000.0M * field.FactorMaxRange;

				switch ( this.realUnitDimension )
				{
					case RealUnitType.DimensionMillimeter:
						field.Step = 1.0M * field.FactorStep;
						field.Resolution = 0.01M;
						break;

					case RealUnitType.DimensionCentimeter:
						field.Step = 0.1M * field.FactorStep;
						field.Resolution = 0.001M;
						break;

					case RealUnitType.DimensionInch:
						field.Step = 0.1M * field.FactorStep;
						field.Resolution = 0.0001M;
						break;

					default:
						field.Step = 1.0M * field.FactorStep;
						field.Resolution = 1.0M;
						break;
				}
			}
		}

		// Adapte un TextFieldReal pour éditer une taille de fonte.
		public void AdaptTextFieldRealFontSize(TextFieldReal field)
		{
			if ( this.document.Type == DocumentType.Pictogram )
			{
				field.UnitType = RealUnitType.Scalar;
				field.Scale = 1.0M;
				field.InternalMinValue = 0.1M;
				field.InternalMaxValue = 50.0M;
				field.Step = 1.0M;
				field.Resolution = 0.1M;
			}
			else
			{
				field.UnitType = RealUnitType.DimensionMillimeter;
				field.Scale = (decimal) Modifier.fontSizeScale;
				field.InternalMinValue = 1.0M;
				field.InternalMaxValue = (decimal) (200.0*Modifier.fontSizeScale);
				field.Step = 1.0M;
				field.Resolution = 0.1M;
			}
		}

		// Adapte un TextFieldReal pour éditer un angle.
		public void AdaptTextFieldRealAngle(TextFieldReal field)
		{
			field.UnitType = RealUnitType.AngleDeg;
			field.InternalMinValue = 0.0M;
			field.InternalMaxValue = 360.0M;
			field.Step = 2.5M;
			field.Resolution = 0.1M;
		}

		// Modifie tous les widgets de l'application reflétant des dimensions
		// pour utiliser une autre unité.
		protected void AdaptAllTextFieldReal()
		{
			foreach ( Window window in Window.DebugAliveWindows )
			{
				if ( window.Root == null )  continue;
				Widget[] widgets = window.Root.FindAllChildren();
				foreach ( Widget widget in widgets )
				{
					if ( widget is TextFieldReal )
					{
						TextFieldReal field = widget as TextFieldReal;
						if ( field.IsDimension && field.Text != "" )
						{
							decimal val = field.InternalValue;
							this.AdaptTextFieldRealDimension(field);
							field.InternalValue = val;
						}
					}
				}
			}
		}
		#endregion


		#region Viewers
		// Un seul visualisateur privilégié peut être actif.
		public Viewer ActiveViewer
		{
			get { return this.activeViewer; }
			set { this.activeViewer = value; }
		}

		// Attache un nouveau visualisateur à ce document.
		public void AttachViewer(Viewer viewer)
		{
			this.attachViewers.Add(viewer);
		}

		// Détache un visualisateur de ce document.
		public void DetachViewer(Viewer viewer)
		{
			this.attachViewers.Remove(viewer);
		}

		// Liste des visualisateurs attachés au document.
		public System.Collections.ArrayList	AttachViewers
		{
			get { return this.attachViewers; }
		}
		#endregion


		#region Containers
		// Attache un nouveau conteneur à ce document.
		public void AttachContainer(Containers.Abstract container)
		{
			this.attachContainers.Add(container);
		}

		// Détache un conteneur de ce document.
		public void DetachContainer(Containers.Abstract container)
		{
			this.attachContainers.Remove(container);
		}

		// Met en évidence l'objet survolé par la souris.
		public void ContainerHilite(Objects.Abstract obj)
		{
			foreach ( Containers.Abstract container in this.attachContainers )
			{
				container.Hilite(obj);
			}
		}

		// Indique quel est le container actif (visible).
		public Containers.Abstract ActiveContainer
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
			this.document.GetObjects.Clear();

			this.document.PropertiesAuto.Clear();
			this.document.PropertiesSel.Clear();
			this.document.PropertiesStyle.Clear();
			this.CreateObjectMemory();

			Objects.Page page = new Objects.Page(this.document, null);  // crée la page initiale
			this.document.GetObjects.Add(page);

			Objects.Layer layer = new Objects.Layer(this.document, null);  // crée le calque initial
			page.Objects.Add(layer);

			foreach ( Viewer viewer in this.attachViewers )
			{
				DrawingContext context = viewer.DrawingContext;
				context.InternalPageLayer(0, 0);
				context.ZoomPageAndCenter();
			}

			this.document.Settings.Reset();
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

		protected void CreateObjectMemory()
		{
			this.objectMemory = new Objects.Memory(this.document, null);

			this.objectMemoryText = new Objects.Memory(this.document, null);
			this.objectMemoryText.PropertyLineMode.Width = 0.0;
			this.objectMemoryText.PropertyFillGradient.Color1 = Color.FromARGB(0, 1,1,1);
		}


		#region Counters
		// Indique qu'il faudra mettre à jour tous les compteurs.
		public void DirtyCounters()
		{
			this.dirtyCounters = true;
		}

		// Met à jour tous les compteurs.
		protected void UpdateCounters()
		{
			if ( this.dirtyCounters == false )  return;

			DrawingContext context = this.ActiveViewer.DrawingContext;

			this.totalPageHide = 0;
			Objects.Abstract page = context.RootObject(1);
			foreach ( Objects.Abstract obj in this.document.Deep(page) )
			{
				if ( obj.IsHide )  this.totalPageHide ++;
			}

			this.totalSelected = 0;
			this.totalHide = 0;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer) )
			{
				if ( obj.IsSelected )  this.totalSelected ++;
				if ( obj.IsHide )  this.totalHide ++;
			}

			this.dirtyCounters = false;
		}

		// Retourne le nombre total d'objets, y compris les objets cachés.
		public int TotalObjects
		{
			get
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				return layer.Objects.Count;
			}
		}

		// Retourne le nombre d'objets sélectionnés.
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

		// Retourne le nombre d'objets cachés dans le calque courant.
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

		// Retourne le nombre d'objets cachés dans toute la page.
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
		public string Statistic(bool fonts, bool images)
		{
			string t1 = "<font size=\"120%\"><b>";
			string t2 = " :</b></font><br/><br/>";
			string chip = "<list type=\"fix\" width=\"1.5\"/>";
			string info;

			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			if ( fonts || images )
			{
				builder.Append(string.Format("{0}Résumé{1}", t1, t2));
			}

			info = string.Format("{0}Nom complet: {1}<br/>", chip, Misc.FullName(this.document.Filename, this.document.IsDirtySerialize));
			builder.Append(info);
			
			info = string.Format("{0}Dimensions: {1}x{2}<br/>", chip, this.document.Size.Width/this.document.Modifier.RealScale, this.document.Size.Height/this.document.Modifier.RealScale);
			builder.Append(info);
			
			info = string.Format("{0}Nombre de pages: {1}<br/>", chip, this.StatisticTotalPages());
			builder.Append(info);
			
			info = string.Format("{0}Nombre de calques: {1}<br/>", chip, this.StatisticTotalLayers());
			builder.Append(info);
			
			info = string.Format("{0}Nombre d'objets: {1}<br/>", chip, this.StatisticTotalObjects());
			builder.Append(info);

			info = string.Format("{0}Objets dégradés ou transparents: {1}<br/>", chip, this.StatisticTotalComplex());
			builder.Append(info);

			if ( fonts )
			{
				builder.Append(string.Format("<br/>{0}Polices utilisées{1}", t1, t2));
				System.Collections.ArrayList list = this.StatisticFonts();
				if ( list.Count == 0 )
				{
					info = string.Format("{0}{1}<br/>", chip, "<i>Aucune</i>");
					builder.Append(info);
				}
				else
				{
					foreach ( string s in list )
					{
						info = string.Format("{0}{1}<br/>", chip, s);
						builder.Append(info);
					}
				}
			}

			if ( images )
			{
				builder.Append(string.Format("<br/>{0}Images utilisées{1}", t1, t2));
				System.Collections.ArrayList list = this.StatisticImages();
				if ( list.Count == 0 )
				{
					info = string.Format("{0}{1}<br/>", chip, "<i>Aucune</i>");
					builder.Append(info);
				}
				else
				{
					foreach ( string s in list )
					{
						info = string.Format("{0}{1}<br/>", chip, s);
						builder.Append(info);
					}
				}
			}

			return builder.ToString();
		}

		// Construit la liste de toutes les fontes utilisées.
		protected System.Collections.ArrayList StatisticFonts()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			foreach ( Objects.Abstract obj in this.document.Deep(null) )
			{
				Properties.Font font = obj.PropertyTextFont;
				if ( font != null )
				{
					if ( !list.Contains(font.FontName) )
					{
						list.Add(font.FontName);
					}
				}

				obj.FillFontFaceList(list);
			}
			list.Sort();
			return list;
		}

		// Construit la liste de toutes les images utilisées.
		protected System.Collections.ArrayList StatisticImages()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			foreach ( Objects.Abstract obj in this.document.Deep(null) )
			{
				Properties.Image image = obj.PropertyImage;
				if ( image != null )
				{
					if ( !list.Contains(image.Filename) )
					{
						list.Add(image.Filename);
					}
				}
			}
			list.Sort();
			return list;
		}

		// Retourne le nombre total de pages.
		public int StatisticTotalPages()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			return context.TotalPages();
		}

		// Retourne le nombre total de calques.
		public int StatisticTotalLayers()
		{
			int total = 0;
			foreach ( Objects.Abstract obj in this.document.Deep(null) )
			{
				if ( obj is Objects.Layer )  total ++;
			}
			return total;
		}

		// Retourne le nombre total d'objets.
		public int StatisticTotalObjects()
		{
			int total = 0;
			foreach ( Objects.Abstract obj in this.document.Deep(null) )
			{
				if ( obj is Objects.Page  )  continue;
				if ( obj is Objects.Layer )  continue;
				if ( obj is Objects.Group )  continue;
				total ++;
			}
			return total;
		}

		// Retourne le nombre total d'objets complexes (dégradés ou transparents).
		public int StatisticTotalComplex()
		{
			int total = 0;
			foreach ( Objects.Abstract obj in this.document.Deep(null) )
			{
				if ( obj.IsComplexPrinting )  total ++;
			}
			return total;
		}
		#endregion


		#region Selection
		// Retourne la bbox des objets sélectionnés.
		public Rectangle SelectedBbox
		{
			get
			{
				Rectangle bbox = Rectangle.Empty;
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
				{
					bbox.MergeWith(obj.BoundingBoxDetect);
				}
				return bbox;
			}
		}

		// Retourne le seul objet sélectionné.
		public Objects.Abstract RetOnlySelectedObject()
		{
			if ( this.TotalSelected != 1 )  return null;

			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
#if true
			int total = layer.Objects.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Abstract obj = layer.Objects[i] as Objects.Abstract;
				if ( obj.IsSelected )  return obj;
			}
#else
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				return obj;
			}
#endif
			return null;
		}

		// Retourne le seul objet en édition.
		public Objects.Abstract RetEditObject()
		{
			if ( this.tool != "Edit" )  return null;
			if ( this.TotalSelected != 1 )  return null;

			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Abstract obj = layer.Objects[i] as Objects.Abstract;
				if ( obj.IsSelected && obj.IsEdited )  return obj;
			}
			return null;
		}

		// Désélectionne tous les objets.
		public void DeselectAll()
		{
			using ( this.OpletQueueBeginAction() )
			{
				this.ActiveViewer.CreateEnding(false);
				if ( this.TotalSelected > 0 )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					Objects.Abstract layer = context.RootObject();
					foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
					{
						obj.Deselect();
					}
					this.TotalSelected = 0;
					this.ActiveViewer.GlobalSelect = false;
				}
				this.OpletQueueValidateAction();
			}
		}

		// Sélectionne tous les objets.
		public void SelectAll()
		{
			using ( this.OpletQueueBeginAction() )
			{
				this.ActiveViewer.CreateEnding(false);

				this.opletCreate = false;
				this.Tool = "Select";

				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				foreach ( Objects.Abstract obj in this.document.Flat(layer) )
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

		// Inverse la sélection.
		public void InvertSelection()
		{
			using ( this.OpletQueueBeginAction() )
			{
				this.ActiveViewer.CreateEnding(false);

				this.opletCreate = false;
				this.Tool = "Select";

				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				foreach ( Objects.Abstract obj in this.document.Flat(layer) )
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
		// Supprime tous les objets sélectionnés.
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
						Objects.Abstract layer = context.RootObject();
						foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
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

		// Choix du déplacement des objets dupliqués.
		public Point DuplicateMove
		{
			get
			{
				return this.duplicateMove;
			}
			
			set
			{
				if ( this.duplicateMove != value )
				{
					this.duplicateMove = value;
					this.document.Notifier.NotifySettingsChanged();
					this.document.IsDirtySerialize = true;
				}
			}
		}

		// Duplique tous les objets sélectionnés.
		public void DuplicateSelection(Point move)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.Tool = "Select";
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				Modifier.Duplicate(this.document, this.document, layer.Objects, layer.Objects, true, move, true);
				this.ActiveViewer.GlobalSelect = ( this.document.Modifier.TotalSelected > 1 );

				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		// Coupe tous les objets sélectionnés dans le bloc-notes.
		public void CutSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				this.document.Clipboard.Modifier.New();
				this.document.Clipboard.Modifier.OpletQueueEnable = false;
				this.Duplicate(this.document, this.document.Clipboard, new Point(0,0), true);
				this.DeleteSelection();

				this.OpletQueueValidateAction();
			}
		}

		// Copie tous les objets sélectionnés dans le bloc-notes.
		public void CopySelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction() )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
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
				Objects.Abstract layer = context.RootObject();
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
				Objects.Abstract obj = srcList[index] as Objects.Abstract;
				if ( onlySelected && !obj.IsSelected )  continue;

				Objects.Abstract newObject = null;
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
		// Annule la dernière action.
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

		// Refait la dernière action.
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
		// Met au premier plan tous les objets sélectionnés.
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

		// Met à l'arrière plan tous les objets sélectionnés.
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
			Objects.Abstract layer = context.RootObject();

			int total = layer.Objects.Count;
			int iSrc = 0;
			int iDst = (dir < 0) ? 0 : total-1;
			do
			{
				Objects.Abstract obj = layer.Objects[iSrc] as Objects.Abstract;
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
		// Déplace tous les objets sélectionnés.
		public void MoveSelection(Point move)
		{
			this.PrepareOper();
			this.ActiveViewer.Selector.OperMove(move);
			this.TerminateOper();
		}

		// Tourne tous les objets sélectionnés.
		public void RotateSelection(double angle)
		{
			this.PrepareOper();
			this.ActiveViewer.Selector.OperRotate(angle);
			this.TerminateOper();
		}

		// Miroir de tous les objets sélectionnés. 
		public void MirrorSelection(bool horizontal)
		{
			this.PrepareOper();
			this.ActiveViewer.Selector.OperMirror(horizontal);
			this.TerminateOper();
		}

		// Zoom de tous les objets sélectionnés.
		public void ZoomSelection(double scale)
		{
			this.PrepareOper();
			this.ActiveViewer.Selector.OperZoom(scale);
			this.TerminateOper();
		}

		// Prépare pour l'opération.
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

		// Termine l'opération.
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
		// Fusionne tous les objets sélectionnés.
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

		// Groupe tous les objets sélectionnés.
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

		// Sépare tous les objets sélectionnés.
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
			Objects.Abstract layer = context.RootObject();
			System.Collections.ArrayList extract = new System.Collections.ArrayList();

			// Extrait tous les objets sélectionnés dans la liste extract.
			Rectangle bbox = Rectangle.Empty;
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				extract.Add(obj);
				bbox.MergeWith(obj.BoundingBoxGroup);
			}

			// Supprime les objets sélectionnés de la liste principale, sans
			// supprimer les propriétés.
			bool bDo = false;
			do
			{
				bDo = false;
				foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
				{
					this.document.Modifier.TotalSelected --;
					layer.Objects.Remove(obj);
					bDo = true;
					break;
				}
			}
			while ( bDo );

			// Crée l'objet groupe.
			Objects.Group group = new Objects.Group(this.document, null);
			layer.Objects.Add(group);
			group.UpdateDim(bbox);
			group.Select();
			this.TotalSelected ++;

			// Remet les objets extraits dans le groupe.
			foreach ( Objects.Abstract obj in extract )
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
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			int index = 0;
			do
			{
				Objects.Abstract obj = layer.Objects[index] as Objects.Abstract;
				if ( obj.IsSelected && obj is Objects.Group )
				{
					int rank = index+1;
					foreach ( Objects.Abstract inside in this.document.Flat(obj) )
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

		// Entre dans tous les objets sélectionnés.
		public void InsideSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction() )
			{
				Objects.Abstract group = this.RetOnlySelectedObject();
				if ( group != null && group is Objects.Group )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					Objects.Abstract layer = context.RootObject();
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

		// Sort de tous les objets sélectionnés.
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
					Objects.Abstract layer = context.RootObject();
					Objects.Abstract group = layer.Objects[index] as Objects.Abstract;
					group.Select();
					this.Tool = "Select";
					this.DirtyCounters();
				}

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		protected Rectangle RetBbox(Objects.Abstract group)
		{
			Rectangle bbox = Rectangle.Empty;
			foreach ( Objects.Abstract obj in this.document.Flat(group) )
			{
				bbox.MergeWith(obj.BoundingBoxGroup);
			}
			return bbox;
		}

		// Adapte les dimensions de tous les groupes fils.
		public void GroupUpdateChildrens()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer) )
			{
				Objects.Group group = obj as Objects.Group;
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
				Objects.Group group = context.RootObject(i) as Objects.Group;
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
		// Cache tous les objets sélectionnés du calque courant.
		public void HideSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction() )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				foreach ( Objects.Abstract obj in this.document.Flat(layer) )
				{
					if ( !(obj is Objects.Page) && !(obj is Objects.Layer) )
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

		// Cache tous les objets non sélectionnés du calque courant.
		public void HideRest()
		{
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction() )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				foreach ( Objects.Abstract obj in this.document.Flat(layer) )
				{
					if ( !(obj is Objects.Page) && !(obj is Objects.Layer) )
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
		protected void HideObjectAndSoons(Objects.Abstract obj)
		{
			obj.IsHide = true;

			if ( obj.Objects != null && obj.Objects.Count > 0 )
			{
				int total = obj.Objects.Count;
				for ( int i=0 ; i<total ; i++ )
				{
					Objects.Abstract sub = obj.Objects[i] as Objects.Abstract;
					this.HideObjectAndSoons(sub);
				}
			}
		}

		// Vend visible tous les objets cachés de la page (donc dans tous les calques).
		public void HideCancel()
		{
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction() )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract page = context.RootObject(1);
				foreach ( Objects.Abstract obj in this.document.Deep(page) )
				{
					if ( !(obj is Objects.Page) && !(obj is Objects.Layer) )
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
			if ( rank < this.document.GetObjects.Count )
			{
				Objects.Page page = this.document.GetObjects[rank] as Objects.Page;
				page.CurrentLayer = this.ActiveViewer.DrawingContext.CurrentLayer;
			}

			this.DeselectAll();
		}

		// Termine un changement de page.
		public void TerminateChangingPage(int rank)
		{
			Objects.Page page = this.document.GetObjects[rank] as Objects.Page;
			int layer = page.CurrentLayer;
			this.ActiveViewer.DrawingContext.PageLayer(rank, layer);
			this.DirtyCounters();
		}

		// Crée une nouvelle page.
		public void PageCreate(int rank, string name)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				this.InitiateChangingPage();

				UndoableList list = this.document.GetObjects;  // liste des pages
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count);

				Objects.Page page = new Objects.Page(this.document, null);
				page.Name = name;
				list.Insert(rank, page);

				Objects.Layer layer = new Objects.Layer(this.document, null);
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

				UndoableList list = this.document.GetObjects;  // liste des pages
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);

				Objects.Page srcPage = list[rank] as Objects.Page;

				Objects.Page page = new Objects.Page(this.document, srcPage);
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

				UndoableList list = this.document.GetObjects;  // liste des pages
				if ( list.Count <= 1 )  return;  // il doit rester une page
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);

				UndoableList pages = this.document.GetObjects;
				Objects.Page page = pages[rank] as Objects.Page;
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
				UndoableList list = this.document.GetObjects;  // liste des pages
				rank1 = System.Math.Max(rank1, 0);
				rank1 = System.Math.Min(rank1, list.Count-1);
				rank2 = System.Math.Max(rank2, 0);
				rank2 = System.Math.Min(rank2, list.Count-1);

				UndoableList pages = this.document.GetObjects;
				Objects.Page temp = pages[rank1] as Objects.Page;
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
			UndoableList pages = this.document.GetObjects;
			Objects.Page page = pages[rank] as Objects.Page;
			return page.Name;
		}

		// Change le nom d'une page.
		public void PageName(int rank, string name)
		{
			this.document.IsDirtySerialize = true;
			using ( this.OpletQueueBeginAction("ChangePageName") )
			{
				UndoableList pages = this.document.GetObjects;
				Objects.Page page = pages[rank] as Objects.Page;
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

		// Crée un nouveau calque.
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

				Objects.Layer layer = new Objects.Layer(this.document, null);
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

				Objects.Layer srcLayer = list[rank] as Objects.Layer;

				Objects.Layer layer = new Objects.Layer(this.document, srcLayer);
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
				Objects.Layer layer = layers[rank] as Objects.Layer;
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
				Objects.Layer temp = layers[rank1] as Objects.Layer;
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
			Objects.Layer layer = list[rank] as Objects.Layer;
			return layer.Name;
		}

		// Change le nom d'un calque.
		public void LayerName(int rank, string name)
		{
			this.document.IsDirtySerialize = true;
			using ( this.OpletQueueBeginAction("ChangeLayerName") )
			{
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				Objects.Layer layer = list[rank] as Objects.Layer;
				layer.Name = name;

				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyLayerChanged(layer);
				this.OpletQueueValidateAction();
			}
		}
		#endregion


		#region Properties
		// Mode de représentation pour le panneau des propriétés.
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

		// Indique si on est en mode "détail" avec plus d'un objet sélectionné.
		public bool PropertiesDetailMany
		{
			get
			{
				return (this.propertiesDetail && this.TotalSelected > 1);
			}
		}

		// Ajoute une nouvelle propriété.
		public void PropertyAdd(Properties.Abstract property)
		{
			this.PropertyList(property).Add(property);

			if ( property.IsStyle )
			{
				this.document.Notifier.NotifyStyleChanged();
			}
		}

		// Supprime une propriété.
		public void PropertyRemove(Properties.Abstract property)
		{
			this.PropertyList(property).Remove(property);

			if ( property.IsStyle )
			{
				this.document.Notifier.NotifyStyleChanged();
			}
		}

		// Donne la liste à utiliser pour une propriété.
		public UndoableList PropertyList(Properties.Abstract property)
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

		// Ajoute toutes les propriétés des objets sélectionnés dans une liste.
		// Tient compte du mode propertiesDetail.
		public void PropertiesList(System.Collections.ArrayList list)
		{
			this.document.Modifier.OpletQueueEnable = false;
			list.Clear();

			if ( this.IsTool )  // outil select, edit, loupe, etc. ?
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
				{
					obj.PropertiesList(list, this.propertiesDetail);
				}
			}
			else	// choix d'un objet à créer ?
			{
				// Crée un objet factice (document = null), c'est-à-dire sans
				// propriétés, qui servira juste de filtre pour objectMemory.
				Objects.Abstract dummy = Objects.Abstract.CreateObject(null, this.tool, null);
				this.ObjectMemoryTool.PropertiesList(list, dummy);
				dummy.Dispose();
			}

			list.Sort();
			this.document.Modifier.OpletQueueEnable = true;
		}
		#endregion


		#region IsPropertiesExtended
		// Indique si le panneau d'une propriété doit être étendu.
		public bool IsPropertiesExtended(Properties.Type type)
		{
			int i=0;
			foreach ( int value in System.Enum.GetValues(typeof(Properties.Type)) )
			{
				if ( type == (Properties.Type)value )
				{
					return this.isPropertiesExtended[i];
				}
				i++;
			}
			return false;
		}

		// Indique si le panneau d'une propriété doit être étendu.
		public void IsPropertiesExtended(Properties.Type type, bool extended)
		{
			int i=0;
			foreach ( int value in System.Enum.GetValues(typeof(Properties.Type)) )
			{
				if ( type == (Properties.Type)value )
				{
					this.isPropertiesExtended[i] = extended;
					break;
				}
				i++;
			}

			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.HandlePropertiesUpdateVisible();
				obj.HandlePropertiesUpdatePosition();
				this.document.Notifier.NotifyArea(this.ActiveViewer, obj.BoundingBox);
			}
		}
		#endregion


		#region Zoom
		// Zoom sur les objets sélectionnés.
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

		// Change le zoom d'un certain facteur, avec centrage au milieu du dessin.
		public void ZoomChange(double factor)
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			this.ZoomChange(factor, context.Center);
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
			context.ZoomAndCenter(newZoom, center);
		}

		// Retourne le nombre de zoom mémorisés.
		public int ZoomMemorizeCount
		{
			get { return this.zoomHistory.Count; }
		}

		// Mémorise le zoom actuel.
		public void ZoomMemorize()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;

			ZoomHistory.ZoomElement item = new ZoomHistory.ZoomElement();
			item.Zoom = context.Zoom;
			item.Center = context.Center;
			this.zoomHistory.Add(item);
		}

		// Revient au niveau de zoom précédent.
		public void ZoomPrev()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;

			ZoomHistory.ZoomElement item = this.zoomHistory.Remove();
			if ( item == null )  return;
			context.ZoomAndCenter(item.Zoom, item.Center);
		}

		// Retourne le zoom minimal.
		public double ZoomMin
		{
			get
			{
				if ( this.document.Type == DocumentType.Pictogram )
				{
					double x = this.SizeArea.Width/this.document.Size.Width;
					double y = this.SizeArea.Height/this.document.Size.Height;
					return 1.0/System.Math.Min(x,y);
				}
				else
				{
					return 0.1;  // 10%
				}
			}
		}

		// Retourne le zoom maximal.
		public double ZoomMax
		{
			get
			{
				if ( this.document.Type == DocumentType.Pictogram )
				{
					return 8.0;  // 800%
				}
				else
				{
					return 16.0;  // 1600%
				}
			}
		}
		#endregion


		#region StyleMenu
		// Construit le menu des styles.
		public VMenu CreateStyleMenu(Properties.Abstract original)
		{
			this.menuProperty = original;

			VMenu menu = new VMenu();
			MenuItem item;

			bool isStyle = false;
			bool isAuto  = false;
			if ( original.IsMulti )
			{
				foreach ( Properties.Abstract property in original.Owners )
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
				item = new MenuItem("StyleMake", "manifest:Epsitec.App.DocumentEditor.Images.StyleMake.icon", "Créer un nouveau style", "");
				item.Pressed += new MessageEventHandler(this.HandleMenuPressed);
				menu.Items.Add(item);
			}

			if ( isStyle )
			{
				item = new MenuItem("StyleFree", "manifest:Epsitec.App.DocumentEditor.Images.StyleFree.icon", "Rendre indépendant du style", "");
				item.Pressed += new MessageEventHandler(this.HandleMenuPressed);
				menu.Items.Add(item);
			}

			bool first = true;
			int total = this.document.PropertiesStyle.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Properties.Abstract property = this.document.PropertiesStyle[i] as Properties.Abstract;
				if ( property.Type != original.Type )  continue;

				if ( first )
				{
					menu.Items.Add(new MenuSeparator());
					first = false;
				}

				string icon = "manifest:Epsitec.App.DocumentEditor.Images.ActiveNo.icon";
				if ( property == original )
				{
					icon = "manifest:Epsitec.App.DocumentEditor.Images.ActiveYes.icon";
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
				Properties.Abstract style = this.document.PropertiesStyle[rank] as Properties.Abstract;
				this.StyleUse(this.menuProperty, style);
			}

			this.menuProperty = null;
		}

		// Fabrique un nouveau style.
		public void StyleMake(Properties.Abstract property)
		{
			if ( this.IsTool )  // propriétés des objets sélectionnés ?
			{
				using ( this.OpletQueueBeginAction() )
				{
					if ( property.IsMulti )
					{
						Properties.Abstract style = Properties.Abstract.NewProperty(this.document, property.Type);
						property.CopyTo(style);
						style.IsStyle = true;
						style.StyleName = Modifier.GetNextStyleName();
						this.PropertyList(style).Add(style);

						DrawingContext context = this.ActiveViewer.DrawingContext;
						Objects.Abstract layer = context.RootObject();
						foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
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
			else	// propriété de objectMemory ?
			{
				using ( this.OpletQueueBeginAction() )
				{
					Properties.Abstract style = Properties.Abstract.NewProperty(this.document, property.Type);
					property.CopyTo(style);
					style.IsStyle = true;
					style.StyleName = Modifier.GetNextStyleName();
					this.PropertyList(true, style.IsSelected).Add(style);
					this.ObjectMemoryTool.ChangeProperty(style);

					this.document.Notifier.NotifyStyleChanged();

					this.OpletQueueValidateAction();
				}
			}
		}

		// Libère un style.
		public void StyleFree(Properties.Abstract property)
		{
			if ( this.IsTool )  // propriétés des objets sélectionnés ?
			{
				using ( this.OpletQueueBeginAction() )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					Objects.Abstract layer = context.RootObject();
					foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
					{
						obj.FreeProperty(property);
					}

					this.document.Notifier.NotifyStyleChanged();
					this.document.Notifier.NotifySelectionChanged();

					this.OpletQueueValidateAction();
				}
			}
			else	// propriété de objectMemory ?
			{
				using ( this.OpletQueueBeginAction() )
				{
					Properties.Abstract free = Properties.Abstract.NewProperty(this.document, property.Type);
					property.CopyTo(free);
					free.IsOnlyForCreation = true;
					free.IsStyle = false;
					free.StyleName = "";
					this.ObjectMemoryTool.ChangeProperty(free);

					this.document.Notifier.NotifyStyleChanged();

					this.OpletQueueValidateAction();
				}
			}
		}

		// Utilise un style.
		public void StyleUse(Properties.Abstract property, Properties.Abstract style)
		{
			if ( this.IsTool )  // propriétés des objets sélectionnés ?
			{
				using ( this.OpletQueueBeginAction() )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					Objects.Abstract layer = context.RootObject();
					foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
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
			else	// propriété de objectMemory ?
			{
				this.OpletQueueEnable = false;
				this.ObjectMemoryTool.ChangeProperty(style);
				this.OpletQueueEnable = true;

				this.document.Notifier.NotifySelectionChanged();
			}
		}

		// Crée un nouveau style d'un type quelconque.
		public void StyleCreate(int rank, Properties.Type type)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction() )
			{
				UndoableList list = this.document.PropertiesStyle;
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);

				Properties.Abstract style = Properties.Abstract.NewProperty(this.document, type);
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
				Properties.Abstract property = list[rank] as Properties.Abstract;

				Properties.Abstract style = Properties.Abstract.NewProperty(this.document, property.Type);
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
				Properties.Abstract property = list[rank] as Properties.Abstract;

				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract doc = context.RootObject(0);
				foreach ( Objects.Abstract obj in this.document.Deep(doc) )
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

				Properties.Abstract temp = list[rank1] as Properties.Abstract;
				list.RemoveAt(rank1);
				list.Insert(rank2, temp);
				list.Selected = rank2;

				this.document.Notifier.NotifyStyleChanged();
				this.OpletQueueValidateAction();
			}
		}

		// Le style sélectionné reprend la bonne propriété de l'objet modèle
		// pointé par la souris avec la pipette.
		public void PickerProperty(Objects.Abstract model)
		{
			int sel = this.document.PropertiesStyle.Selected;
			if ( sel < 0 )  return;
			Properties.Abstract dst = this.document.PropertiesStyle[sel] as Properties.Abstract;
			Properties.Abstract src = model.Property(dst.Type);
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
		// Ajoute un oplet pour mémoriser l'outil.
		protected void InsertOpletTool()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletTool oplet = new OpletTool(this.document);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// Mémorise l'outil.
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
		// Détermine si les actions seront annulables ou non.
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

		// Début d'une zone annulable.
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
		protected Objects.Memory				objectMemory;
		protected Objects.Memory				objectMemoryText;
		protected Properties.Abstract			menuProperty;
		protected bool							operInitialSelector;
		protected Containers.Abstract			activeContainer;
		protected bool[]						isPropertiesExtended;
		protected RealUnitType					realUnitDimension;
		protected double						realScale;
		protected Point							duplicateMove;

		public static readonly double			fontSizeScale = 3.5;  // empyrique !
	}
}
