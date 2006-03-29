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
			this.tool = "ToolSelect";
			this.zoomHistory = new ZoomHistory();
			this.opletQueue = new OpletQueue();
			this.isUndoRedoInProgress = false;
			this.CreateObjectMemory();

			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.RealUnitDimension = RealUnitType.None;
				this.duplicateMove = new Point(1.0, 1.0);
				this.arrowMove = new Point(1.0, 1.0);
				this.arrowMoveMul = 10.0;
				this.arrowMoveDiv = 10.0;
				this.outsideArea = 10.0;
				this.moveDistance = new Point(1.0, 1.0);
			}
			else
			{
				if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
				{
					this.SetRealUnitDimension(RealUnitType.DimensionMillimeter, false);
					this.duplicateMove = new Point(50.0, 50.0);  // 5mm
					this.arrowMove = new Point(10.0, 10.0);  // 1mm
					this.moveDistance = new Point(100.0, 100.0);  // 10mm
				}
				else
				{
					this.SetRealUnitDimension(RealUnitType.DimensionInch, false);
					this.duplicateMove = new Point(50.8, 50.8);  // 0.2in
					this.arrowMove = new Point(25.4, 25.4);  // 0.1in
					this.moveDistance = new Point(254.0, 254.0);  // 1in
				}
				this.arrowMoveMul = 10.0;
				this.arrowMoveDiv = 10.0;
				this.outsideArea = 0.0;
			}

			this.rotateAngle = 10.0;  // 10 degr�s
			this.scaleFactor = 1.2;  // x1.2
			this.colorAdjust = 0.1;  // 10%

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

			this.repeatDuplicateMove = true;
			this.FlushMoveAfterDuplicate();

			this.toLinePrecision = 0.25;
			this.aggregateUsed = -1;

			if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
			{
				this.dimensionScale = 1.0;
				this.dimensionDecimal = 1.0;
			}
			else
			{
				this.dimensionScale = 1.0;
				this.dimensionDecimal = 2.0;
			}
		}

		public void Dispose()
		{
		}

		public string Tool
		{
			//	Outil s�lectionn� dans la palette.
			get
			{
				return this.tool;
			}

			set
			{
				this.ActiveViewer.EditFlowReset();

				if ( this.tool != value )
				{
					if ( this.ActiveViewer.IsCreating )
					{
						this.ActiveViewer.CreateEnding(false, false);
					}

					this.ActiveViewer.ClearHilite();
					this.ActiveViewer.ClearEditCreateRect();

					bool isCreate = this.opletCreate;
					string name = string.Format(Res.Strings.Action.ChangeTool, this.ToolName(value));
					this.OpletQueueBeginAction(name);
					this.InsertOpletTool();

					Objects.AbstractText editObject = this.RetEditObject();

					if ( this.tool == "ToolHotSpot" || value == "ToolHotSpot" )
					{
						this.document.Notifier.NotifyArea(this.ActiveViewer);
					}

					string initialTool = this.tool;
					this.tool = value;

					this.ToolAdaptRibbon();

					if ( this.tool == "ToolSelect" && isCreate )  // on vient de cr�er un objet ?
					{
						DrawingContext context = this.ActiveViewer.DrawingContext;
						Objects.Abstract layer = context.RootObject();
						Objects.Abstract obj = layer.Objects[layer.Objects.Count-1] as Objects.Abstract;
						this.ActiveViewer.Select(obj, false, false);
					}

					if ( this.tool == "ToolShaper" && isCreate )  // on vient de cr�er un objet ?
					{
						DrawingContext context = this.ActiveViewer.DrawingContext;
						Objects.Abstract layer = context.RootObject();
						Objects.Abstract obj = layer.Objects[layer.Objects.Count-1] as Objects.Abstract;
						this.ActiveViewer.Select(obj, false, false);
					}

					else if ( this.tool == "ToolEdit" && isCreate )  // on vient de cr�er un objet ?
					{
						DrawingContext context = this.ActiveViewer.DrawingContext;
						Objects.Abstract layer = context.RootObject();
						Objects.Abstract obj = layer.Objects[layer.Objects.Count-1] as Objects.Abstract;
						this.ActiveViewer.Select(obj, true, false);
					}

					else if ( this.tool == "ToolShaper" )
					{
						if ( editObject != null )
						{
							editObject.Select(true);
						}
						else
						{
							this.ActiveViewer.SelectToShaper();
						}
					}

					else if ( this.IsTool && this.tool != "ToolEdit" )
					{
						this.SelectedSegmentClear();

						if ( editObject != null )
						{
							editObject.Select(true);
						}
						else if ( initialTool == "ToolShaper" )
						{
							this.ActiveViewer.UpdateSelector();
						}
					}

					else if ( this.tool == "ToolEdit" )
					{
						if ( this.TotalSelected == 1 )
						{
							Objects.Abstract sel = this.RetOnlySelectedObject();
							if ( sel != null )
							{
								if ( sel.IsEditable )
								{
									sel.Select(true, true);
									this.ActiveViewer.UpdateSelector();
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

					else if ( !this.IsTool )  // choix d'un objet � cr�er ?
					{
						this.DeselectAll();
					}

					this.ActiveViewer.DrawingContext.MagnetDelStarting();

					this.OpletQueueValidateAction();
					this.document.Notifier.NotifyToolChanged();
					this.document.Notifier.NotifySelectionChanged();
				}
			}
		}

		protected void ToolAdaptRibbon()
		{
			//	Adapte les rubans � l'outil s�lectionn�.
			if ( this.IsUndoRedoInProgress )  return;

			if ( this.IsToolEdit )
			{
				this.document.Notifier.NotifyRibbonCommand("Text");
			}
			else
			{
				this.document.Notifier.NotifyRibbonCommand("!Text");
			}
		}

		public bool IsTool
		{
			//	Indique si l'outil s�lectionn� n'est pas un objet.
			get
			{
				if ( this.tool == "ToolSelect"  )  return true;
				if ( this.tool == "ToolGlobal"  )  return true;
				if ( this.tool == "ToolShaper"  )  return true;
				if ( this.tool == "ToolEdit"    )  return true;
				if ( this.tool == "ToolZoom"    )  return true;
				if ( this.tool == "ToolHand"    )  return true;
				if ( this.tool == "ToolPicker"  )  return true;
				if ( this.tool == "ToolHotSpot" )  return true;
				return false;
			}
		}

		public bool IsToolText
		{
			//	Indique si l'outil s�lectionn� est un objet de type "texte".
			get
			{
				if ( this.tool == "ObjectTextLine"  )  return true;
				if ( this.tool == "ObjectTextLine2" )  return true;
				if ( this.tool == "ObjectTextBox"   )  return true;
				if ( this.tool == "ObjectTextBox2"  )  return true;
				return false;
			}
		}

		public bool IsToolEdit
		{
			//	Indique si l'outil est l'�diteur.
			get
			{
				return this.tool == "ToolEdit";
			}
		}

		public bool IsToolShaper
		{
			//	Indique si l'outil est le modeleur.
			get
			{
				return this.tool == "ToolShaper";
			}
		}

		public string ToolName(string tool)
		{
			//	Retourne le nom d'un outil.
			switch ( tool )
			{
				case "ToolSelect":       return Res.Strings.Action.ToolSelect;
				case "ToolGlobal":       return Res.Strings.Action.ToolGlobal;
				case "ToolShaper":       return Res.Strings.Action.ToolShaper;
				case "ToolEdit":         return Res.Strings.Action.ToolEdit;
				case "ToolZoom":         return Res.Strings.Action.ToolZoom;
				case "ToolHand":         return Res.Strings.Action.ToolHand;
				case "ToolPicker":       return Res.Strings.Action.ToolPicker;
				case "ToolHotSpot":      return Res.Strings.Action.ToolHotSpot;

				case "ObjectLine":       return Res.Strings.Action.ToolLine;
				case "ObjectRectangle":  return Res.Strings.Action.ToolRectangle;
				case "ObjectCircle":     return Res.Strings.Action.ToolCircle;
				case "ObjectEllipse":    return Res.Strings.Action.ToolEllipse;
				case "ObjectPoly":       return Res.Strings.Action.ToolPoly;
				case "ObjectBezier":     return Res.Strings.Action.ToolBezier;
				case "ObjectRegular":    return Res.Strings.Action.ToolRegular;
				case "ObjectSurface":    return Res.Strings.Action.ToolSurface;
				case "ObjectVolume":     return Res.Strings.Action.ToolVolume;
				case "ObjectTextLine":   return Res.Strings.Action.ToolTextLine;
				case "ObjectTextLine2":  return Res.Strings.Action.ToolTextLine;
				case "ObjectTextBox":    return Res.Strings.Action.ToolTextBox;
				case "ObjectTextBox2":   return Res.Strings.Action.ToolTextBox;
				case "ObjectArray":      return Res.Strings.Action.ToolArray;
				case "ObjectImage":      return Res.Strings.Action.ToolImage;
				case "ObjectDimension":  return Res.Strings.Action.ToolDimension;
			}

			return "?";
		}


		public OpletQueue OpletQueue
		{
			get { return this.opletQueue; }
		}

		public Objects.Memory ObjectMemory
		{
			get { return this.objectMemory; }
			set { this.objectMemory = value; }
		}

		public Objects.Memory ObjectMemoryText
		{
			get { return this.objectMemoryText; }
			set { this.objectMemoryText = value; }
		}

		public Objects.Memory ObjectMemoryTool
		{
			get { return this.IsToolText ? this.objectMemoryText : this.objectMemory; }
		}


		public Size SizeArea
		{
			//	Taille de la zone de travail.
			get
			{
				return new Size(this.document.PageSize.Width+this.outsideArea*2.0, this.document.PageSize.Height+this.outsideArea*2.0);
			}
		}

		public Point OriginArea
		{
			//	Origine de la zone de travail.
			get
			{
				return new Point(-this.outsideArea, -this.outsideArea);
			}
		}

		public Rectangle RectangleArea
		{
			//	Rectangle de la zone de travail.
			get
			{
				return new Rectangle(this.OriginArea, this.SizeArea);
			}
		}

		public Rectangle PageArea
		{
			//	Rectangle de la page de travail.
			get
			{
				return new Rectangle(new Point(0,0), this.document.PageSize);
			}
		}

		public double OutsideArea
		{
			//	Marges autour de la page physique.
			get
			{
				return this.outsideArea;
			}

			set
			{
				if ( this.outsideArea != value )
				{
					this.OpletQueueBeginAction(Res.Strings.Action.OutsideArea, "ChangeDocSize");
					this.InsertOpletSize();
					this.outsideArea = value;
					this.document.Notifier.NotifyOriginChanged();
					this.document.Notifier.NotifyArea(this.ActiveViewer);
					this.OpletQueueValidateAction();
				}
			}
		}

		public double DimensionScale
		{
			//	Echelle pour les cotes.
			get
			{
				return this.dimensionScale;
			}
			
			set
			{
				if ( this.dimensionScale != value )
				{
					this.dimensionScale = value;
					this.document.Notifier.NotifySettingsChanged();
					this.document.Notifier.NotifyArea(this.ActiveViewer);
					this.document.IsDirtySerialize = true;
				}
			}
		}

		public double DimensionDecimal
		{
			//	Nombre de d�cimales des cotes.
			get
			{
				return this.dimensionDecimal;
			}
			
			set
			{
				if ( this.dimensionDecimal != value )
				{
					this.dimensionDecimal = value;
					this.document.Notifier.NotifySettingsChanged();
					this.document.Notifier.NotifyArea(this.ActiveViewer);
					this.document.IsDirtySerialize = true;
				}
			}
		}


		public string TextInfoModif
		{
			//	Texte des informations de modification.
			get
			{
				return this.textInfoModif;
			}

			set
			{
				if ( this.textInfoModif != value )
				{
					this.textInfoModif = value;
					this.document.Notifier.NotifyModifChanged();
				}
			}
		}

		public void TabBookChanged(string name)
		{
			//	Appel� lorsque l'onglet a chang�.
			if ( name == "Operations" )
			{
				this.bookInitialSelector = SelectorType.None;
				if ( !this.ActiveViewer.Selector.Visible ||
					 this.ActiveViewer.Selector.TypeInUse != SelectorType.Scaler )
				{
					this.bookInitialSelector = this.ActiveViewer.SelectorType;
					this.ActiveViewer.SelectorType = SelectorType.Scaler;
				}
			}
			else
			{
				if ( this.bookInitialSelector != SelectorType.None )
				{
					this.ActiveViewer.SelectorType = this.bookInitialSelector;
					this.bookInitialSelector = SelectorType.None;
				}
			}
		}


		#region RealUnit
		public string RealToString(double value)
		{
			//	Conversion d'une distance en cha�ne.
			if ( this.document.Type == DocumentType.Pictogram )
			{
				return value.ToString("F1");
			}
			else
			{
				value /= this.realScale;
				value /= this.realResolution*10.0;  // *10 -> un digit de moins
				value = System.Math.Floor(value+0.5);
				value *= this.realResolution*10.0;
				return value.ToString();
			}
		}

		public string AngleToString(double value)
		{
			//	Conversion d'un angle en cha�ne.
			return string.Format("{0}\u00B0", value.ToString("F1"));
		}

		public RealUnitType RealUnitDimension
		{
			//	Choix de l'unit� de dimension par d�faut.
			get
			{
				return this.realUnitDimension;
			}
			
			set
			{
				this.SetRealUnitDimension(value, true);
			}
		}

		public void SetRealUnitDimension(RealUnitType unit, bool adapt)
		{
			//	Choix de l'unit� de dimension par d�faut.
			this.realUnitDimension = unit;

			switch ( this.realUnitDimension )
			{
				case RealUnitType.DimensionMillimeter:
					this.realScale = 10.0;
					this.realResolution = 0.01;
					this.realShortNameUnitDimension = Res.Strings.Units.Short.Millimeter;
					this.realLongNameUnitDimension  = Res.Strings.Units.Long.Millimeter;
					break;

				case RealUnitType.DimensionCentimeter:
					this.realScale = 100.0;
					this.realResolution = 0.001;
					this.realShortNameUnitDimension = Res.Strings.Units.Short.Centimeter;
					this.realLongNameUnitDimension  = Res.Strings.Units.Long.Centimeter;
					break;

				case RealUnitType.DimensionInch:
					this.realScale = 254.0;
					this.realResolution = 0.0001;
					this.realShortNameUnitDimension = Res.Strings.Units.Short.Inch;
					this.realLongNameUnitDimension  = Res.Strings.Units.Long.Inch;
					break;

				default:
					this.realScale = 1.0;
					this.realResolution = 1.0;
					this.realShortNameUnitDimension = "";
					this.realLongNameUnitDimension  = "";
					break;
			}

			if ( adapt )
			{
				this.AdaptAllTextFieldReal();
			}
		}

		public double RealScale
		{
			//	Facteur d'�chelle pour les distances.
			get
			{
				return this.realScale;
			}
		}

		public string ShortNameUnitDimension
		{
			//	Nom compact de l'unit� de dimension.
			get
			{
				return this.realShortNameUnitDimension;
			}
		}

		public string LongNameUnitDimension
		{
			//	Nom complet de l'unit� de dimension.
			get
			{
				return this.realLongNameUnitDimension;
			}
		}

		public void AdaptTextFieldRealScalar(TextFieldReal field)
		{
			//	Adapte un TextFieldReal pour �diter un scalaire.
			field.UnitType = RealUnitType.Scalar;
			field.Step = 1.0M;
			field.Resolution = 1.0M;
		}

		public void AdaptTextFieldRealPercent(TextFieldReal field)
		{
			//	Adapte un TextFieldReal pour �diter un pourcent.
			field.UnitType = RealUnitType.Percent;
			field.Step = 1.0M;
			field.Resolution = 0.1M;
			field.TextSuffix = "%";
		}

		public void AdaptTextFieldRealDimension(TextFieldReal field)
		{
			//	Adapte un TextFieldReal pour �diter une dimension.
			field.UnitType = this.realUnitDimension;
			field.Scale = (decimal) this.realScale;

			if ( this.document.Type == DocumentType.Pictogram )
			{
				field.InternalMinValue     = 200.0M * field.FactorMinRange;
				field.InternalMaxValue     = 200.0M * field.FactorMaxRange;
				field.InternalDefaultValue = 200.0M * field.FactorDefaultRange;
				field.Step                 =   1.0M * field.FactorStep;
				field.Resolution           =   0.1M;
			}
			else
			{
				field.InternalMinValue     = 10000.0M * field.FactorMinRange;
				field.InternalMaxValue     = 10000.0M * field.FactorMaxRange;
				field.InternalDefaultValue = 10000.0M * field.FactorDefaultRange;

				switch ( this.realUnitDimension )
				{
					case RealUnitType.DimensionMillimeter:
						field.Step = 1.0M * field.FactorStep;
						break;

					case RealUnitType.DimensionCentimeter:
						field.Step = 0.1M * field.FactorStep;
						break;

					case RealUnitType.DimensionInch:
						field.Step = 0.1M * field.FactorStep;
						break;

					default:
						field.Step = 1.0M * field.FactorStep;
						break;
				}

				field.Resolution = (decimal) this.realResolution * field.FactorResolution;
			}
		}

		public void AdaptTextFieldRealFontSize(TextFieldReal field)
		{
			//	Adapte un TextFieldReal pour �diter une taille de fonte.
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
				field.UnitType = RealUnitType.Scalar;
				field.Scale = (decimal) Modifier.FontSizeScale;
				field.InternalMinValue = 1.0M;
				field.InternalMaxValue = (decimal) (200.0*Modifier.FontSizeScale);
				field.Step = 1.0M;
				field.Resolution = 0.1M;
			}
		}

		public void AdaptTextFieldRealAngle(TextFieldReal field)
		{
			//	Adapte un TextFieldReal pour �diter un angle.
			field.UnitType = RealUnitType.AngleDeg;
			field.InternalMinValue = 0.0M;
			field.InternalMaxValue = 360.0M;
			field.Step = 2.5M;
			field.Resolution = 0.1M;
			field.TextSuffix = "\u00B0";  // symbole unicode "degr�" (#176)
		}

		public void AdaptAllTextFieldReal()
		{
			//	Modifie tous les widgets de l'application refl�tant des dimensions
			//	pour utiliser une autre unit�.
			foreach ( Window window in Window.DebugAliveWindows )
			{
				if ( window.Root == null )  continue;
				this.AdaptAllTextFieldReal(window.Root);
			}
		}

		protected void AdaptAllTextFieldReal(Widget parent)
		{
			//	Modifie tous les widgets d'un panneau qui sera utilis�.
			Widget[] widgets = parent.FindAllChildren();
			foreach ( Widget widget in widgets )
			{
				if ( widget is TextFieldReal )
				{
					TextFieldReal field = widget as TextFieldReal;
					if ( field.IsDimension )
					{
						if ( field.Text == "" )
						{
							this.AdaptTextFieldRealDimension(field);
						}
						else
						{
							decimal value = field.InternalValue;
							this.AdaptTextFieldRealDimension(field);
							field.InternalValue = value;
						}
					}
				}
			}
		}
		#endregion


		#region Viewers
		public Viewer ActiveViewer
		{
			//	Un seul visualisateur privil�gi� peut �tre actif.
			get { return this.activeViewer; }
			set { this.activeViewer = value; }
		}

		public void AttachViewer(Viewer viewer)
		{
			//	Attache un nouveau visualisateur � ce document.
			this.attachViewers.Add(viewer);
		}

		public void DetachViewer(Viewer viewer)
		{
			//	D�tache un visualisateur de ce document.
			this.attachViewers.Remove(viewer);
		}

		public System.Collections.ArrayList	AttachViewers
		{
			//	Liste des visualisateurs attach�s au document.
			get { return this.attachViewers; }
		}
		#endregion


		#region Containers
		public void AttachContainer(Containers.Abstract container)
		{
			//	Attache un nouveau conteneur � ce document.
			this.attachContainers.Add(container);
		}

		public void DetachContainer(Containers.Abstract container)
		{
			//	D�tache un conteneur de ce document.
			this.attachContainers.Remove(container);
		}

		public void ContainerHilite(Objects.Abstract obj)
		{
			//	Met en �vidence l'objet survol� par la souris.
			foreach ( Containers.Abstract container in this.attachContainers )
			{
				container.Hilite(obj);
			}
		}

		public Containers.Abstract ActiveContainer
		{
			//	Indique quel est le container actif (visible).
			get { return this.activeContainer; }
			set { this.activeContainer = value; }
		}
		#endregion


		#region RibbonTextStyle
		public int RibbonTextStyleSelected
		{
			get { return this.ribbonTextStyleSelected; }
			set { this.ribbonTextStyleSelected = value; }
		}

		public int RibbonTextStyleFirst
		{
			get { return this.ribbonTextStyleFirst; }
			set { this.ribbonTextStyleFirst = value; }
		}
		#endregion


		public void New()
		{
			//	Vide le document de tous ses objets.
			this.ActiveViewer.CreateEnding(false, false);
			this.OpletQueueEnable = false;

			this.TotalSelected = 0;
			this.totalHide = 0;
			this.totalPageHide = 0;
			this.document.GetObjects.Clear();

			this.document.PropertiesAuto.Clear();
			this.document.PropertiesSel.Clear();
			this.CreateObjectMemory();

			Objects.Page page = new Objects.Page(this.document, null);  // cr�e la page initiale
			this.document.GetObjects.Add(page);

			Objects.Layer layer = new Objects.Layer(this.document, null);  // cr�e le calque initial
			page.Objects.Add(layer);

			foreach ( Viewer viewer in this.attachViewers )
			{
				DrawingContext context = viewer.DrawingContext;
				context.InternalPageLayer(0, 0);
				context.ZoomPageAndCenter();
			}

			this.UpdatePageAfterChanging();
			this.document.Settings.Reset();
			this.zoomHistory.Clear();
			this.document.HotSpot = new Point(0, 0);
			this.document.Filename = "";
			this.document.IsDirtySerialize = false;
			this.ActiveViewer.SelectorType = SelectorType.Auto;
			this.opletCreate = false;

			this.OpletQueueEnable = true;
			this.OpletQueuePurge();

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
			this.objectMemoryText.PropertyFillGradient.Color1 = RichColor.FromAlphaRgb(0, 1,1,1);
		}


		#region Counters
		public void DirtyCounters()
		{
			//	Indique qu'il faudra mettre � jour tous les compteurs.
			this.dirtyCounters = true;
		}

		public bool IsDirtyCounters
		{
			//	Indique s'il faudra mettre � jour tous les compteurs.
			get
			{
				return this.dirtyCounters;
			}
		}

		public void UpdateCounters()
		{
			//	Met � jour tous les compteurs.
			if ( !this.dirtyCounters )  return;

			DrawingContext context = this.ActiveViewer.DrawingContext;

			this.totalPageHide = 0;
			Objects.Abstract page = context.RootObject(1);
			foreach ( Objects.Abstract obj in this.document.Deep(page) )
			{
				if ( obj.IsHide )  this.totalPageHide ++;
			}

			this.totalSelected = 0;
			this.totalHide = 0;
			this.NamesExist = false;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				if ( obj.IsSelected )  this.totalSelected ++;
				if ( obj.IsHide )  this.totalHide ++;

				Properties.Name pn = obj.PropertyName;
				if ( pn != null )
				{
					if ( pn.String != "" )  this.NamesExist = true;
				}
			}

			this.dirtyCounters = false;
		}

		public int TotalObjects
		{
			//	Retourne le nombre total d'objets, y compris les objets cach�s.
			get
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				return layer.Objects.Count;
			}
		}

		public int TotalSelected
		{
			//	Retourne le nombre d'objets s�lectionn�s.
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

		public int TotalHide
		{
			//	Retourne le nombre d'objets cach�s dans le calque courant.
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

		public int TotalPageHide
		{
			//	Retourne le nombre d'objets cach�s dans toute la page.
			get
			{
				if ( this.dirtyCounters )  this.UpdateCounters();
				return this.totalPageHide;
			}
		}

		public bool NamesExist
		{
			//	Est-ce que le calque courant contient des noms d'objets ?
			get
			{
				return this.namesExist;
			}

			set
			{
				if ( this.namesExist != value )
				{
					this.namesExist = value;
					this.document.Notifier.NotifySelNamesChanged();
				}
			}
		}
		#endregion


		#region Statistic
		public string Statistic(bool fonts, bool images)
		{
			//	Retourne un texte multi-lignes de statistiques sur le document.
			string t1 = "<font size=\"120%\"><b>";
			string t2 = " :</b></font><br/><br/>";
			string chip = "<list type=\"fix\" width=\"1.5\"/>";
			string br = "<br/>";
			string info;

			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			if ( fonts || images )
			{
				builder.Append(string.Format(Res.Strings.Statistic.Summary, t1, t2));
			}

			if ( fonts || images )
			{
				info = string.Format(Res.Strings.Statistic.Filename, chip, Misc.FullName(this.document.Filename, this.document.IsDirtySerialize));
				builder.Append(info);
				builder.Append(br);
			}
			else
			{
				if ( this.document.Filename != "" )
				{
					info = string.Format(Res.Strings.Statistic.Filename, chip, Misc.FullName(this.document.Filename, this.document.IsDirtySerialize));
					builder.Append(info);
					builder.Append(br);
				}
			}
			
			info = string.Format(Res.Strings.Statistic.Size, chip, this.RealToString(this.document.PageSize.Width), this.RealToString(this.document.PageSize.Height));
			builder.Append(info);
			builder.Append(br);
			
			info = string.Format(Res.Strings.Statistic.Pages, chip, this.StatisticTotalPages());
			builder.Append(info);
			builder.Append(br);
			
			info = string.Format(Res.Strings.Statistic.Layers, chip, this.StatisticTotalLayers());
			builder.Append(info);
			builder.Append(br);
			
			info = string.Format(Res.Strings.Statistic.Objects, chip, this.StatisticTotalObjects());
			builder.Append(info);
			builder.Append(br);

			info = string.Format(Res.Strings.Statistic.Gradients, chip, this.StatisticTotalComplex());
			builder.Append(info);
			builder.Append(br);

			if ( fonts )
			{
				builder.Append(br);
				builder.Append(string.Format(Res.Strings.Statistic.Fonts, t1, t2));
				System.Collections.ArrayList list = new System.Collections.ArrayList();
				TextFlow.StatisticFonts(list, this.document.TextFlows);
				this.StatisticFonts(list);
				list.Sort();
				if ( list.Count == 0 )
				{
					info = string.Format("{0}{1}<br/>", chip, Res.Strings.Statistic.None);
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
				builder.Append(br);
				builder.Append(string.Format(Res.Strings.Statistic.Images, t1, t2));
				System.Collections.ArrayList list = this.StatisticImages();
				if ( list.Count == 0 )
				{
					info = string.Format("{0}{1}<br/>", chip, Res.Strings.Statistic.None);
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

		// Construit la liste de toutes les fontes utilis�es.
		protected void StatisticFonts(System.Collections.ArrayList list)
		{
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
		}

		protected System.Collections.ArrayList StatisticImages()
		{
			//	Construit la liste de toutes les images utilis�es.
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

		public int StatisticTotalPages()
		{
			//	Retourne le nombre total de pages.
			DrawingContext context = this.ActiveViewer.DrawingContext;
			return context.TotalPages();
		}

		public int StatisticTotalLayers()
		{
			//	Retourne le nombre total de calques.
			int total = 0;
			foreach ( Objects.Abstract obj in this.document.Deep(null) )
			{
				if ( obj is Objects.Layer )  total ++;
			}
			return total;
		}

		public int StatisticTotalObjects()
		{
			//	Retourne le nombre total d'objets.
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

		public int StatisticTotalComplex()
		{
			//	Retourne le nombre total d'objets complexes (d�grad�s ou transparents).
			int total = 0;
			foreach ( Objects.Abstract obj in this.document.Deep(null) )
			{
				if ( obj.IsComplexPrinting )  total ++;
			}
			return total;
		}
		#endregion


		#region Selection
		public Rectangle SelectedBbox
		{
			//	Retourne la bbox des objets s�lectionn�s.
			get
			{
				Rectangle bbox = Rectangle.Empty;
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
				{
					bbox.MergeWith(obj.BoundingBoxPartial);
				}
				return bbox;
			}
		}

		public Rectangle SelectedBboxThin
		{
			//	Retourne la bbox mince des objets s�lectionn�s.
			get
			{
				Rectangle bbox = Rectangle.Empty;
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
				{
					bbox.MergeWith(obj.BoundingBoxThin);
				}
				return bbox;
			}
		}

		public bool IsSelectedOldText()
		{
			//	Retourne true si un ancien objet TextBox ou TextLine est s�lectionn�.
			if ( this.TotalSelected == 0 )  return false;

			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Abstract obj = layer.Objects[i] as Objects.Abstract;
				if ( obj.IsSelected )
				{
					if ( obj is Objects.TextBox  )  return true;
					if ( obj is Objects.TextLine )  return true;
				}
			}
			return false;
		}

		public Objects.Abstract RetOnlySelectedObject()
		{
			//	Retourne le seul objet s�lectionn�.
			if ( this.TotalSelected != 1 )  return null;

			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Abstract obj = layer.Objects[i] as Objects.Abstract;
				if ( obj.IsSelected )  return obj;
			}
			return null;
		}

		public Objects.Abstract RetFirstSelectedObject()
		{
			//	Retourne le premier objet s�lectionn�.
			if ( this.TotalSelected == 0 )  return null;

			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Abstract obj = layer.Objects[i] as Objects.Abstract;
				if ( obj.IsSelected )  return obj;
			}
			return null;
		}

		public bool EditInsertText(string text, string fontFace, string fontStyle)
		{
			//	Ins�re un texte dans le pav� en �dition.
			Objects.AbstractText editObject = this.RetEditObject();
			if ( editObject == null )  return false;

			return editObject.EditInsertText(text, fontFace, fontStyle);
		}

		public bool EditInsertText(Text.Unicode.Code code)
		{
			//	Ins�re un texte dans le pav� en �dition.
			Objects.AbstractText editObject = this.RetEditObject();
			if ( editObject == null )  return false;

			return editObject.EditInsertText(code);
		}

		public bool EditInsertText(Text.Properties.BreakProperty brk)
		{
			//	Ins�re un texte dans le pav� en �dition.
			Objects.AbstractText editObject = this.RetEditObject();
			if ( editObject == null )  return false;

			return editObject.EditInsertText(brk);
		}

		public bool EditInsertGlyph(int code, int glyph, string fontFace, string fontStyle)
		{
			//	Ins�re un glyphe dans le pav� en �dition.
			Objects.AbstractText editObject = this.RetEditObject();
			if ( editObject == null )  return false;

			return editObject.EditInsertGlyph(code, glyph, fontFace, fontStyle);
		}

		public void EditGetFont(out string fontFace, out string fontStyle)
		{
			//	Donne la fonte actullement utilis�e.
			if ( this.document.Wrappers.IsWrappersAttached )
			{
				fontFace = this.document.Wrappers.TextWrapper.Defined.FontFace;
				if ( fontFace == null )
				{
					fontFace = this.document.Wrappers.TextWrapper.Active.FontFace;
					if ( fontFace == null )
					{
						fontFace = "";
					}
				}

				fontStyle = this.document.Wrappers.TextWrapper.Defined.FontStyle;
				if ( fontStyle == null )
				{
					fontStyle = this.document.Wrappers.TextWrapper.Active.FontStyle;
					if ( fontStyle == null )
					{
						fontStyle = "";
					}
				}
			}
			else
			{
				fontFace  = "";
				fontStyle = "";
			}
		}

		public Objects.AbstractText RetEditObject()
		{
			//	Retourne le seul objet en �dition.
			if ( !this.IsToolEdit )  return null;
			if ( this.TotalSelected != 1 )  return null;

			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Abstract obj = layer.Objects[i] as Objects.Abstract;
				if ( obj.IsSelected && obj.IsEdited )  return obj as Objects.AbstractText;
			}
			return null;
		}

		public void SetEditObject(Objects.AbstractText edit, bool changeTool)
		{
			//	Edite l'objet demand�, en changeant de page et de calque si n�cessaire.
			if ( edit == this.RetEditObject() )  // d�j� en cours d'�dition ?
			{
				edit.SetAutoScroll();  // montre le cureur
				return;
			}

			this.OpletQueueBeginAction(Res.Strings.Action.Edit);

			if ( this.ActiveViewer.DrawingContext.CurrentPage != edit.PageNumber )
			{
				this.ActiveViewer.DrawingContext.CurrentPage = edit.PageNumber;
			}

			int layer = this.GetLayerRank(edit);
			if ( this.ActiveViewer.DrawingContext.CurrentLayer != layer )
			{
				this.ActiveViewer.DrawingContext.CurrentLayer = layer;
			}

			if ( changeTool )
			{
				this.Tool = "ToolEdit";
			}
			this.ActiveViewer.Select(edit, true, false);
			edit.SetAutoScroll();  // montre le cureur

			this.OpletQueueValidateAction();
		}

		protected void SelectedSegmentClear()
		{
			//	Supprime tous les segments s�lectionn�s des objets s�lectionn�s.
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.SelectedSegmentClear();
			}
		}

		public void DeselectAllCmd()
		{
			//	D�s�lectionne tous les objets.
			if ( this.ActiveViewer.CloseMiniBar() )  return;
			if ( this.ActiveViewer.EditFlowTerminate() )  return;

			this.DeselectAll();
		}

		public void DeselectAll()
		{
			//	D�s�lectionne tous les objets.

			using ( this.OpletQueueBeginAction(Res.Strings.Action.DeselectAll) )
			{
				this.ActiveViewer.CreateEnding(false, false);
				if ( this.TotalSelected > 0 )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					Objects.Abstract layer = context.RootObject();
					foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
					{
						obj.Deselect();
					}
					this.TotalSelected = 0;
					this.ActiveViewer.UpdateSelector();
				}
				this.OpletQueueValidateAction();
			}

			this.FlushMoveAfterDuplicate();
		}

		public void SelectAll()
		{
			//	S�lectionne tous les objets.
			using ( this.OpletQueueBeginAction(Res.Strings.Action.SelectAll) )
			{
				this.ActiveViewer.CreateEnding(false, false);

				this.opletCreate = false;
				this.Tool = "ToolSelect";

				this.UpdateCounters();
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

				this.ActiveViewer.UpdateSelector();
				this.OpletQueueValidateAction();
			}

			this.FlushMoveAfterDuplicate();
		}

		public void InvertSelection()
		{
			//	Inverse la s�lection.
			using ( this.OpletQueueBeginAction(Res.Strings.Action.SelectInvert) )
			{
				this.ActiveViewer.CreateEnding(false, false);

				this.opletCreate = false;
				this.Tool = "ToolSelect";

				this.UpdateCounters();
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

				this.ActiveViewer.UpdateSelector();
				this.OpletQueueValidateAction();
			}

			this.FlushMoveAfterDuplicate();
		}

		public System.Collections.ArrayList SelectNames()
		{
			//	Retourne la liste de tous les objets ayant un nom dans la page
			//	et la calque courant.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer) )
			{
				if ( obj.IsHide )  continue;

				Properties.Name pn = obj.PropertyName;
				if ( pn == null || pn.String == "" )  continue;

				if ( list.Contains(pn.String) )  continue;
				list.Add(pn.String);
			}

			list.Sort();
			return list;
		}

		public void SelectName(string name)
		{
			//	S�lectionne d'apr�s un nom d'objet.
			if ( name == "" )  return;
			
			System.Text.RegularExpressions.Regex regex = Support.RegexFactory.FromSimpleJoker(name);

			string nm = string.Format(Res.Strings.Action.SelectName, name);
			using ( this.OpletQueueBeginAction(nm) )
			{
				this.ActiveViewer.CreateEnding(false, false);

				this.opletCreate = false;
				this.Tool = "ToolSelect";

				this.UpdateCounters();
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				foreach ( Objects.Abstract obj in this.document.Flat(layer) )
				{
					if ( obj.IsHide )  continue;

					string objName = "";
					Properties.Name pn = obj.PropertyName;
					if ( pn != null )
					{
						objName = pn.String;
					}

					if ( obj.IsSelected )
					{
						if ( !regex.IsMatch(objName) )
						{
							obj.Deselect();
							this.TotalSelected --;
						}
					}
					else
					{
						if ( regex.IsMatch(objName) )
						{
							obj.Select();
							this.TotalSelected ++;
						}
					}
				}

				this.ActiveViewer.UpdateSelector();
				this.ShowSelection();
				this.OpletQueueValidateAction();
			}

			this.FlushMoveAfterDuplicate();
		}
		#endregion


		#region Delete, Duplicate and Clipboard
		public void DeleteSelection()
		{
			//	Supprime tous les objets s�lectionn�s.
			this.DeleteSelection(false);
		}

		public void DeleteSelection(bool onlyMark)
		{
			//	Supprime tous les objets s�lectionn�s.
			//	Si onlyMark=true, on ne d�truit que les objets marqu�s.
			this.document.IsDirtySerialize = true;

			if ( this.ActiveViewer.IsCreating )
			{
				this.ActiveViewer.CreateEnding(true, false);
			}
			else
			{
				using ( this.OpletQueueBeginAction(Res.Strings.Action.Delete) )
				{
					this.UpdateCounters();
					bool bDo = false;
					do
					{
						bDo = false;
						DrawingContext context = this.ActiveViewer.DrawingContext;
						Objects.Abstract layer = context.RootObject();
						foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
						{
							if ( onlyMark && !obj.Mark )  continue;
							this.TotalSelected --;
							this.document.Notifier.NotifyArea(obj.BoundingBox);
							this.DeleteGroup(obj);
							obj.Dispose();
							layer.Objects.Remove(obj);
							bDo = true;
							break;
						}
					}
					while ( bDo );

					this.ActiveViewer.UpdateSelector();
					this.document.Notifier.NotifySelectionChanged();
					this.OpletQueueValidateAction();
				}
			}
		}

		protected void DeleteGroup(Objects.Abstract group)
		{
			//	D�truit les objets fils �ventuels.
			if ( group.Objects != null )
			{
				bool bDo = false;
				do
				{
					bDo = false;
					foreach ( Objects.Abstract obj in this.document.Flat(group) )
					{
						this.DeleteGroup(obj);
						obj.Dispose();
						group.Objects.Remove(obj);
						bDo = true;
						break;
					}
				}
				while ( bDo );
			}
		}

		protected void ClearMarks()
		{
			//	Supprime toutes les marques des objets s�lectionn�s.
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.Mark = false;
			}
		}

		public Point DuplicateMove
		{
			//	Choix du d�placement des objets dupliqu�s.
			get
			{
				return this.duplicateMove;
			}
			
			set
			{
				if ( this.duplicateMove != value )
				{
					this.duplicateMove = value;
					this.FlushMoveAfterDuplicate();
					this.document.Notifier.NotifySettingsChanged();
					this.document.IsDirtySerialize = true;
				}
			}
		}

		public Point ArrowMove
		{
			//	Choix du d�placement des objets avec les touches fl�ches.
			get
			{
				return this.arrowMove;
			}
			
			set
			{
				if ( this.arrowMove != value )
				{
					this.arrowMove = value;
					this.document.Notifier.NotifySettingsChanged();
					this.document.IsDirtySerialize = true;
				}
			}
		}

		public double ArrowMoveMul
		{
			//	Choix du multiplicateur avec Shift.
			get
			{
				return this.arrowMoveMul;
			}
			
			set
			{
				if ( this.arrowMoveMul != value )
				{
					this.arrowMoveMul = value;
					this.document.Notifier.NotifySettingsChanged();
					this.document.IsDirtySerialize = true;
				}
			}
		}

		public double ArrowMoveDiv
		{
			//	Choix du diviseur avec Ctrl.
			get
			{
				return this.arrowMoveDiv;
			}
			
			set
			{
				if ( this.arrowMoveDiv != value )
				{
					this.arrowMoveDiv = value;
					this.document.Notifier.NotifySettingsChanged();
					this.document.IsDirtySerialize = true;
				}
			}
		}

		public void FlushMoveAfterDuplicate()
		{
			//	Annule le d�placement apr�s un duplique.
			this.lastOperIsDuplicate = false;
			this.moveAfterDuplicate = new Point(0,0);
		}

		public void AddMoveAfterDuplicate(Point move)
		{
			//	Ajoute un d�placement effectu� apr�s un duplique.
			if ( this.lastOperIsDuplicate )
			{
				this.moveAfterDuplicate += move;
			}
		}

		public bool RepeatDuplicateMove
		{
			//	R�glage "r�p�te le dernier d�placement".
			get
			{
				return this.repeatDuplicateMove;
			}

			set
			{
				this.repeatDuplicateMove = value;
			}
		}

		public Point EffectiveDuplicateMove
		{
			//	Donne le d�placement effectif � utiliser pour la commande "dupliquer".
			get
			{
				if ( !this.repeatDuplicateMove || this.moveAfterDuplicate.IsEmpty )
				{
					return this.duplicateMove;
				}
				else
				{
					return this.moveAfterDuplicate;
				}
			}
		}

		public void DuplicateSelection(Point move)
		{
			//	Duplique tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Duplicate) )
			{
				this.Tool = "ToolSelect";
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				Modifier.Duplicate(this.document, this.document, layer.Objects, layer.Objects, true, move, true);
				this.ActiveViewer.UpdateSelector();

				this.ShowSelection();
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}

			this.lastOperIsDuplicate = true;
			this.moveAfterDuplicate = move;
		}

		public void CutSelection()
		{
			//	Coupe tous les objets s�lectionn�s dans le bloc-notes.
			Objects.AbstractText editObject = this.RetEditObject();
			if ( editObject != null )
			{
				if ( editObject.EditCut() )  return;
			}

			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Cut) )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				this.document.Clipboard.Modifier.New();
				this.document.Clipboard.Modifier.OpletQueueEnable = false;
				this.Duplicate(this.document, this.document.Clipboard, new Point(0,0), true);
				this.document.Clipboard.Modifier.AggregateFreeAll();
				this.DeleteSelection();
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		public void CopySelection()
		{
			//	Copie tous les objets s�lectionn�s dans le bloc-notes.
			Objects.AbstractText editObject = this.RetEditObject();
			if ( editObject != null )
			{
				if ( editObject.EditCopy() )  return;
			}

			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Copy) )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				this.document.Clipboard.Modifier.New();
				this.document.Clipboard.Modifier.OpletQueueEnable = false;
				this.Duplicate(this.document, this.document.Clipboard, new Point(0,0), true);
				this.document.Clipboard.Modifier.AggregateFreeAll();
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		public void Paste()
		{
			//	Colle le contenu du bloc-notes.
			Objects.AbstractText editObject = this.RetEditObject();
			if ( editObject != null )
			{
				if ( editObject.EditPaste() )  return;
			}

			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Paste) )
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				this.DeselectAll();
				this.document.Clipboard.Modifier.OpletQueueEnable = false;
				this.Duplicate(this.document.Clipboard, this.document, new Point(0,0), true);
				this.ActiveViewer.UpdateSelector();
				this.ShowSelection();
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		protected void Duplicate(Document srcDoc, Document dstDoc,
								 Point move, bool onlySelected)
		{
			//	Duplique d'un document dans un autre.
			DrawingContext srcContext = srcDoc.Modifier.ActiveViewer.DrawingContext;
			UndoableList srcList = srcContext.RootObject().Objects;

			DrawingContext dstContext = dstDoc.Modifier.ActiveViewer.DrawingContext;
			UndoableList dstList = dstContext.RootObject().Objects;

			Modifier.Duplicate(srcDoc, dstDoc, srcList, dstList, false, move, onlySelected);
		}

		protected static void Duplicate(Document srcDoc, Document dstDoc,
										UndoableList srcList, UndoableList dstList,
										bool deselect, Point move, bool onlySelected)
		{
			//	Copie tous les objets d'une liste source dans une liste destination.
			srcDoc.Modifier.UpdateCounters();
			dstDoc.Modifier.UpdateCounters();
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

		public bool IsClipboardEmpty()
		{
			//	Indique si le bloc-notes est vide.
			if ( this.document.Clipboard == null )  return true;
			return ( this.document.Clipboard.Modifier.TotalObjects == 0 );
		}
		#endregion


		#region TextFormat
		public void SetTextStyle(Text.TextStyle style)
		{
			//	Modifie le style du texte.
			Objects.AbstractText edit = this.RetEditObject();
			if ( edit == null )  return;

			string name = this.document.TextContext.StyleList.StyleMap.GetCaption(style);
			string text = string.Format(Res.Strings.Action.AggregateUse, name);

			if ( style.TextStyleClass == Text.TextStyleClass.Paragraph )
			{
				this.document.Modifier.OpletQueueBeginAction(text);
				edit.TextFlow.MetaNavigator.EndSelection();
				edit.TextFlow.TextNavigator.SetParagraphStyles(style);
				this.document.Modifier.OpletQueueValidateAction();
			}

			if ( style.TextStyleClass == Text.TextStyleClass.Text )
			{
				this.document.Modifier.OpletQueueBeginAction(text);
				edit.TextFlow.MetaNavigator.EndSelection();
				edit.TextFlow.TextNavigator.SetTextStyles(style);
				this.document.Modifier.OpletQueueValidateAction();
			}
		}

		public void TextFlowChange(Objects.AbstractText obj, Objects.AbstractText parent, bool after)
		{
			//	Modifie le flux.
			using ( this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.Text.FlowChanged) )
			{
				if ( parent == null )
				{
					TextFlow flow = obj.TextFlow;
					
					this.document.Modifier.OpletQueue.Insert(new TextFlowChangeOplet(flow));
					obj.TextFlow.Remove(obj);
					this.document.Modifier.OpletQueue.Insert(new TextFlowChangeOplet(flow));
				}
				else
				{
					TextFlow flow1 = obj.TextFlow;
					TextFlow flow2 = parent.TextFlow;
					
					this.document.Modifier.OpletQueue.Insert(new TextFlowChangeOplet(flow1));
					this.document.Modifier.OpletQueue.Insert(new TextFlowChangeOplet(flow2));
					
					parent.TextFlow.Add(obj, parent, after);
					
					this.SetEditObject(obj, false);
					
					this.document.Modifier.OpletQueue.Insert(new TextFlowChangeOplet(flow1));
					this.document.Modifier.OpletQueue.Insert(new TextFlowChangeOplet(flow2));
				}
				
				this.document.Modifier.OpletQueueValidateAction();
			}
		}
		
		public class TextFlowChangeOplet : AbstractOplet
		{
			public TextFlowChangeOplet(TextFlow flow)
			{
				this.flow = flow;
			}
			
			public override IOplet Redo()
			{
				if ( this.flow != null )
				{
					this.flow.RebuildFrameList();
				}
				return this;
			}
			
			public override IOplet Undo()
			{
				if ( this.flow != null )
				{
					this.flow.RebuildFrameList();
				}
				return this;
			}
			
			private TextFlow					flow;
		}
		#endregion


		#region UndoRedo
		public bool IsUndoRedoInProgress
		{
			//	Indique si une op�ration undo/redo est en cours.
			get
			{
				return this.isUndoRedoInProgress;
			}
		}

		public void Undo(int number)
		{
			//	Annule les derni�res actions.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;
			this.ActiveViewer.CreateEnding(false, false);

			this.isUndoRedoInProgress = true;
			this.AccumulateStarting();
			for ( int i=0 ; i<number ; i++ )
			{
				this.opletQueue.UndoAction();
			}
			this.AccumulateEnding();
			this.isUndoRedoInProgress = false;

			this.opletLastCmd = "";
			this.opletLastId = 0;
			this.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
			this.document.Notifier.NotifySelectionChanged();
			this.document.Notifier.NotifyTextChanged();
			this.document.Notifier.NotifyStyleChanged();
			this.document.Notifier.NotifyUndoRedoChanged();
			this.document.Notifier.NotifyArea();
		}

		public void Redo(int number)
		{
			//	Refait les derni�res actions.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;
			this.ActiveViewer.CreateEnding(false, false);

			this.isUndoRedoInProgress = true;
			this.AccumulateStarting();
			for ( int i=0 ; i<number ; i++ )
			{
				this.opletQueue.RedoAction();
			}
			this.AccumulateEnding();
			this.isUndoRedoInProgress = false;

			this.opletLastCmd = "";
			this.opletLastId = 0;
			this.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
			this.document.Notifier.NotifySelectionChanged();
			this.document.Notifier.NotifyTextChanged();
			this.document.Notifier.NotifyStyleChanged();
			this.document.Notifier.NotifyUndoRedoChanged();
			this.document.Notifier.NotifyArea();
		}

		public VMenu CreateUndoRedoMenu(MessageEventHandler message)
		{
			//	Construit le menu des actions � refaire/annuler.
			string[] undoNames = this.opletQueue.UndoActionNames;
			string[] redoNames = this.opletQueue.RedoActionNames;

			//	Pour des raisons historiques, le menu est construit � l'envers,
			//	c'est-�-dire la derni�re action au d�but du menu (en haut).
			//	-
			//	| redo
			//	-
			//	|
			//	| undo
			//	|
			//	- 0
			int all = undoNames.Length+redoNames.Length;
			int total = System.Math.Min(all, 20);
			int start = undoNames.Length;
			start -= total/2;  if ( start < 0     )  start = 0;
			start += total-1;  if ( start > all-1 )  start = all-1;

			System.Collections.ArrayList list = new System.Collections.ArrayList();

			//	Met �ventuellement la derni�re action � refaire.
			if ( start < all-1 )
			{
				int todo = -redoNames.Length;
				string action = redoNames[redoNames.Length-1];
				action = Misc.Italic(action);
				this.CreateUndoRedoMenu(list, message, 0, all, action, todo);

				if ( start < all-2 )
				{
					list.Add(new MenuSeparator());
				}
			}

			//	Met les actions � refaire puis � celles � annuler.
			for ( int i=start ; i>start-total ; i-- )
			{
				if ( i >= undoNames.Length )  // redo ?
				{
					int todo = -(i-undoNames.Length+1);
					string action = redoNames[i-undoNames.Length];
					action = Misc.Italic(action);
					this.CreateUndoRedoMenu(list, message, 0, i+1, action, todo);

					if ( i == undoNames.Length && undoNames.Length != 0 )
					{
						list.Add(new MenuSeparator());
					}
				}
				else	// undo ?
				{
					int todo = undoNames.Length-i;
					string action = undoNames[undoNames.Length-i-1];
					int active = 1;
					if ( i == undoNames.Length-1 )
					{
						active = 2;
						action = Misc.Bold(action);
					}
					this.CreateUndoRedoMenu(list, message, active, i+1, action, todo);
				}
			}

			//	Met �ventuellement la derni�re action � annuler.
			if ( start-total >= 0 )
			{
				if ( start-total > 0 )
				{
					list.Add(new MenuSeparator());
				}

				int todo = undoNames.Length;
				string action = undoNames[undoNames.Length-1];
				this.CreateUndoRedoMenu(list, message, 1, 1, action, todo);
			}

			//	G�n�re le menu � l'envers, c'est-�-dire la premi�re action au
			//	d�but du menu (en haut).
			VMenu menu = new VMenu();
			for ( int i=list.Count-1 ; i>=0 ; i-- )
			{
				menu.Items.Add(list[i]);
			}
			menu.AdjustSize();
			return menu;
		}

		protected void CreateUndoRedoMenu(System.Collections.ArrayList list, MessageEventHandler message,
										  int active, int rank, string action, int todo)
		{
			//	Cr�e une case du menu des actions � refaire/annuler.
			string icon = "";
			if ( active == 1 )  icon = Misc.Icon("ActiveNo");
			if ( active == 2 )  icon = Misc.Icon("ActiveCurrent");

			string name = string.Format("{0}: {1}", rank.ToString(), action);

			MenuItem item = new MenuItem("UndoRedoListDo(this.Name)", icon, name, "", todo.ToString());

			if ( message != null )
			{
				item.Pressed += message;
			}

			list.Add(item);
		}

		protected void AccumulateStarting()
		{
			//	D�but de l'accumulation des objets dont on a modifi� les propri�t�s.
			//	Voir la remarque dans Properties.Abstract.NotifyAfter !
			System.Diagnostics.Debug.Assert(this.accumulateObjects == null);
			this.accumulateObjects = new System.Collections.Hashtable();
		}

		public void AccumulateObject(Objects.Abstract obj)
		{
			//	Accumulation d'un objet dont on a modifi� les propri�t�s.
			System.Diagnostics.Debug.Assert(this.accumulateObjects != null);
			if ( this.accumulateObjects.ContainsKey(obj) )  return;
			this.accumulateObjects.Add(obj, null);
		}

		protected void AccumulateEnding()
		{
			//	Fin de l'accumulation des objets dont on a modifi� les propri�t�s.
			//	Indique qu'il faudra recalculer la bbox de tous les objets accumul�s.
			System.Diagnostics.Debug.Assert(this.accumulateObjects != null);
			foreach ( Objects.Abstract obj in this.accumulateObjects.Keys )
			{
				obj.HandlePropertiesUpdate();
				obj.SetDirtyBbox();
			}
			this.accumulateObjects = null;
		}
		#endregion


		#region Order
		public void OrderUpOneSelection()
		{
			//	Met dessus tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.OrderUpOne) )
			{
				this.OrderOneSelection(1);
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		public void OrderDownOneSelection()
		{
			//	Met dessous tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.OrderDownOne) )
			{
				this.OrderOneSelection(-1);
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		public void OrderUpAllSelection()
		{
			//	Met au premier plan tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.OrderUpAll) )
			{
				this.OrderAllSelection(1);
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		public void OrderDownAllSelection()
		{
			//	Met � l'arri�re plan tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.OrderDownAll) )
			{
				this.OrderAllSelection(-1);
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		protected void OrderOneSelection(int dir)
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			if ( total == 0 )  return;

			int iDst = 0;
			if ( dir < 0 )
			{
				iDst = this.OrderFirstSelected()-1;
				if ( iDst < 0 )  iDst = 0;
			}
			else
			{
				iDst = this.OrderLastSelected()+1;
				if ( iDst > total )  iDst = total;
				total = iDst;
			}
			this.OrderSelection(dir, iDst, total);
		}

		protected int OrderFirstSelected()
		{
			//	Retourne l'index du premier objet s�lectionn�.
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();

			int total = layer.Objects.Count;
			int i = 0;
			do
			{
				Objects.Abstract obj = layer.Objects[i] as Objects.Abstract;
				if ( obj.IsSelected )  return i;
				i ++;
			}
			while ( i < total );
			return -1;
		}

		protected int OrderLastSelected()
		{
			//	Retourne l'index du dernier objet s�lectionn�.
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();

			int total = layer.Objects.Count;
			int i = total-1;
			do
			{
				Objects.Abstract obj = layer.Objects[i] as Objects.Abstract;
				if ( obj.IsSelected )  return i;
				i --;
			}
			while ( i >= 0 );
			return -1;
		}

		protected void OrderAllSelection(int dir)
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();

			int total = layer.Objects.Count;
			int iDst = (dir < 0) ? 0 : total-1;
			this.OrderSelection(dir, iDst, total);
		}

		protected void OrderSelection(int dir, int iDst, int total)
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();

			int iSrc = 0;
			do
			{
				Objects.Abstract obj = layer.Objects[iSrc] as Objects.Abstract;
				if ( obj.IsSelected )
				{
					layer.Objects.RemoveAt(iSrc);
					layer.Objects.Insert(System.Math.Min(iDst, layer.Objects.Count), obj);
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
		public double MoveDistanceH
		{
			//	Distance du d�placement horizontal.
			get { return this.moveDistance.X; }
			set { this.moveDistance.X = value; }
		}

		public double MoveDistanceV
		{
			//	Distance du d�placement vertical.
			get { return this.moveDistance.Y; }
			set { this.moveDistance.Y = value; }
		}

		public double RotateAngle
		{
			//	Angle de rotation.
			get { return this.rotateAngle; }
			set { this.rotateAngle = value; }
		}

		public double ScaleFactor
		{
			//	Facteur d'�chelle pour les agrandissement/r�ductions.
			get { return this.scaleFactor; }
			set { this.scaleFactor = value; }
		}

		public double ColorAdjust
		{
			//	Facteur d'ajustement de couleur.
			get { return this.colorAdjust; }
			set { this.colorAdjust = value; }
		}

		public void MoveSelection(Point dir, int alter)
		{
			//	D�place tous les objets s�lectionn�s.
			if ( this.IsToolEdit )  return;

			Point move = Point.ScaleMul(this.arrowMove, dir);

			if ( alter < 0 )  // touche Ctrl ?
			{
				move /= this.arrowMoveDiv;
			}

			if ( alter > 0 )  // touche Shift ?
			{
				move *= this.arrowMoveMul;
			}

			if ( this.ActiveViewer.DrawingContext.GridActive )
			{
				Point initial = move;
				this.ActiveViewer.DrawingContext.SnapGridForce(ref move, new Point(0,0));
				if ( initial.X != 0.0 && move.X == 0.0 )
				{
					move.X = this.ActiveViewer.DrawingContext.GridStep.X;
					if ( initial.X < 0.0 )  move.X = -move.X;
				}
				if ( initial.Y != 0.0 && move.Y == 0.0 )
				{
					move.Y = this.ActiveViewer.DrawingContext.GridStep.Y;
					if ( initial.Y < 0.0 )  move.Y = -move.Y;
				}
			}

			string name = string.Format(Res.Strings.Action.Move, this.RealToString(move.X), this.RealToString(move.Y));
			this.PrepareOper(name);
			this.ActiveViewer.Selector.OperMove(move);
			this.TerminateOper();
		}

		public void MoveSelection(Point move)
		{
			//	D�place tous les objets s�lectionn�s.
			if ( this.IsToolEdit )  return;
			string name = string.Format(Res.Strings.Action.Move, this.RealToString(move.X), this.RealToString(move.Y));
			this.PrepareOper(name);
			this.ActiveViewer.Selector.OperMove(move);
			this.TerminateOper();
		}

		public void RotateSelection(double angle)
		{
			//	Tourne tous les objets s�lectionn�s.
			if ( this.IsToolEdit )  return;
			string name = string.Format(Res.Strings.Action.Rotate, this.AngleToString(angle));
			this.PrepareOper(name);
			this.ActiveViewer.Selector.OperRotate(angle);
			this.TerminateOper();
		}

		public void MirrorSelection(bool horizontal)
		{
			//	Miroir de tous les objets s�lectionn�s. 
			if ( this.IsToolEdit )  return;
			string name = horizontal ? Res.Strings.Action.MirrorH : Res.Strings.Action.MirrorV;
			this.PrepareOper(name);
			this.ActiveViewer.Selector.OperMirror(horizontal);
			this.TerminateOper();
		}

		public void ScaleSelection(double scale)
		{
			//	Mise � l'�chelle de tous les objets s�lectionn�s.
			if ( this.IsToolEdit )  return;
			string name = "";
			if ( scale > 1.0 )
			{
				name = string.Format(Res.Strings.Action.ScaleMul, (scale*100.0).ToString("F1"));
			}
			else
			{
				name = string.Format(Res.Strings.Action.ScaleDiv, (scale*100.0).ToString("F1"));
			}
			this.PrepareOper(name);
			this.ActiveViewer.Selector.OperScale(scale);
			this.TerminateOper();
		}

		protected void PrepareOper(string name)
		{
			//	Pr�pare pour l'op�ration.
			this.document.IsDirtySerialize = true;
			this.OpletQueueBeginAction(name);

			this.document.Notifier.EnableSelectionChanged = false;

			if ( this.ActiveViewer.Selector.Visible &&
				 this.ActiveViewer.Selector.TypeInUse == SelectorType.Scaler )
			{
				this.operInitialSelector = SelectorType.None;
			}
			else
			{
				this.operInitialSelector = this.ActiveViewer.SelectorType;
				this.ActiveViewer.SelectorType = SelectorType.Scaler;
			}

			this.document.Notifier.EnableSelectionChanged = true;
		}

		protected void TerminateOper()
		{
			//	Termine l'op�ration.
			this.document.Notifier.EnableSelectionChanged = false;

			if ( this.operInitialSelector != SelectorType.None )
			{
				this.ActiveViewer.SelectorType = this.operInitialSelector;
			}

			this.document.Notifier.EnableSelectionChanged = true;

			this.ShowSelection();
			this.OpletQueueValidateAction();
		}
		#endregion


		#region Align and Share
		public void AlignGridSelection()
		{
			//	Aligne sur la grille tous les objets s�lectionn�s.
			this.OpletQueueBeginAction(Res.Strings.Action.AlignGrid);
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.AlignGrid(context);
			}
			this.OpletQueueValidateAction();
		}

		public void AlignSelection(int dir, bool horizontal)
		{
			//	Aligne tous les objets s�lectionn�s.
			this.OpletQueueBeginAction(Res.Strings.Action.Align);
			Rectangle globalBox = this.SelectedBbox;
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				Rectangle objBox = obj.BoundingBoxDetect;
				Point move = new Point(0,0);
				if ( horizontal )
				{
					if ( dir <  0 )  move.X = globalBox.Left-objBox.Left;
					if ( dir == 0 )  move.X = globalBox.Center.X-objBox.Center.X;
					if ( dir >  0 )  move.X = globalBox.Right-objBox.Right;
				}
				else
				{
					if ( dir <  0 )  move.Y = globalBox.Bottom-objBox.Bottom;
					if ( dir == 0 )  move.Y = globalBox.Center.Y-objBox.Center.Y;
					if ( dir >  0 )  move.Y = globalBox.Top-objBox.Top;
				}
				this.MoveAllStarting(obj);
				this.MoveAllProcess(obj, move);
			}
			this.OpletQueueValidateAction();
		}

		public void ShareSelection(int dir, bool horizontal)
		{
			//	Distribue tous les objets s�lectionn�s.
			System.Collections.ArrayList list = this.ShareSortedList(dir, horizontal);
			ShareObject first = list[0] as ShareObject;
			ShareObject last = list[list.Count-1] as ShareObject;

			Rectangle globalBox = this.SelectedBbox;
			if ( dir < 0 )
			{
				globalBox.Right -= last.Object.BoundingBoxDetect.Width;
				globalBox.Top   -= last.Object.BoundingBoxDetect.Height;
			}
			if ( dir == 0 )
			{
				globalBox.Left   += first.Object.BoundingBoxDetect.Width/2;
				globalBox.Bottom += first.Object.BoundingBoxDetect.Height/2;
				globalBox.Right  -= last.Object.BoundingBoxDetect.Width/2;
				globalBox.Top    -= last.Object.BoundingBoxDetect.Height/2;
			}
			if ( dir > 0 )
			{
				globalBox.Left   += first.Object.BoundingBoxDetect.Width;
				globalBox.Bottom += first.Object.BoundingBoxDetect.Height;
			}

			if ( ( horizontal && globalBox.Width  <= 0.0) ||
				 (!horizontal && globalBox.Height <= 0.0) )
			{
				string message = Res.Strings.Error.Share;
				this.ActiveViewer.DialogError(message);
				return;
			}

			this.OpletQueueBeginAction(Res.Strings.Action.Share);
			int total = list.Count;
			for ( int i=1 ; i<total-1 ; i++ )
			{
				ShareObject so = list[i] as ShareObject;
				Objects.Abstract obj = so.Object;

				Rectangle objBox = obj.BoundingBoxDetect;

				Point pos = new Point();
				pos.X = globalBox.Left + globalBox.Width*i/(total-1);
				pos.Y = globalBox.Bottom + globalBox.Height*i/(total-1);

				Point move = new Point(0,0);
				if ( horizontal )  move.X = pos.X-so.Position;
				else               move.Y = pos.Y-so.Position;

				this.MoveAllStarting(obj);
				this.MoveAllProcess(obj, move);
			}
			this.OpletQueueValidateAction();
		}

		public void SpaceSelection(bool horizontal)
		{
			//	Distribue les espaces entre tous les objets s�lectionn�s.
			System.Collections.ArrayList list = this.ShareSortedList(0, horizontal);
			ShareObject first = list[0] as ShareObject;
			int total = list.Count;

			Size space = this.SelectedBbox.Size;
			foreach ( ShareObject so in list )
			{
				space -= so.Object.BoundingBoxDetect.Size;
			}
			space /= total-1;
			Point pos = first.Object.BoundingBoxDetect.TopRight;

			if ( ( horizontal && space.Width  <= 0.0) ||
				 (!horizontal && space.Height <= 0.0) )
			{
				string message = Res.Strings.Error.Share;
				this.ActiveViewer.DialogError(message);
				return;
			}

			this.OpletQueueBeginAction(Res.Strings.Action.ShareSpace);
			for ( int i=1 ; i<total-1 ; i++ )
			{
				ShareObject so = list[i] as ShareObject;
				Objects.Abstract obj = so.Object;

				pos += space;
				pos += obj.BoundingBoxDetect.Size/2;  // position souhait�e du centre de l'objet

				Point move = new Point(0,0);
				if ( horizontal )  move.X = pos.X-so.Position;
				else               move.Y = pos.Y-so.Position;

				this.MoveAllStarting(obj);
				this.MoveAllProcess(obj, move);

				pos += obj.BoundingBoxDetect.Size/2;
			}
			this.OpletQueueValidateAction();
		}

		protected void MoveAllStarting(Objects.Abstract group)
		{
			//	D�but du d�placement d'un objet isol� ou d'un groupe.
			group.MoveAllStarting();

			if ( group is Objects.Group )
			{
				foreach ( Objects.Abstract obj in this.document.Deep(group) )
				{
					obj.MoveAllStarting();
				}
			}
		}

		protected void MoveAllProcess(Objects.Abstract group, Point move)
		{
			//	Effectue le d�placement d'un objet isol� ou d'un groupe.
			group.MoveAllProcess(move);

			if ( group is Objects.Group )
			{
				foreach ( Objects.Abstract obj in this.document.Deep(group) )
				{
					obj.MoveAllProcess(move);
				}
			}
		}

		protected System.Collections.ArrayList ShareSortedList(int dir, bool horizontal)
		{
			//	Construit la liste tri�e de tous les objets � distribuer,
			//	de gauche � droite ou de bas en haut.
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				ShareObject so = new ShareObject(obj, dir, horizontal);
				list.Add(so);
			}
			list.Sort();
			return list;
		}

		//	Cette classe repr�sente un objet � distribuer, essentiellement
		//	repr�sent�e par la position de r�f�rence de l'objet. Selon dir et
		//	horizontal, il peut s'agir de la position x ou y d'une extr�mit�
		//	ou du centre de l'objet.
		protected class ShareObject : System.IComparable
		{
			public ShareObject(Objects.Abstract obj, int dir, bool horizontal)
			{
				this.obj = obj;
				this.dir = dir;
				this.horizontal = horizontal;

				Rectangle box = this.obj.BoundingBoxDetect;
				if ( this.horizontal )
				{
					if ( this.dir <  0 )  this.position = box.Left;
					if ( this.dir == 0 )  this.position = box.Center.X;
					if ( this.dir >  0 )  this.position = box.Right;
				}
				else
				{
					if ( this.dir <  0 )  this.position = box.Bottom;
					if ( this.dir == 0 )  this.position = box.Center.Y;
					if ( this.dir >  0 )  this.position = box.Top;
				}
			}

			public Objects.Abstract Object
			{
				get { return this.obj; }
			}

			public double Position
			{
				get { return this.position; }
			}

			public int CompareTo(object obj)
			{
				ShareObject ao = obj as ShareObject;
				return this.position.CompareTo(ao.position);
			}

			protected int					dir;
			protected bool					horizontal;
			protected Objects.Abstract		obj;
			protected double				position;
		}

		public void AdjustSelection(bool horizontal)
		{
			//	Ajuste tous les objets s�lectionn�s.
			this.OpletQueueBeginAction(Res.Strings.Action.Adjust);
			Rectangle globalBox = this.SelectedBbox;
			Selector selector = new Selector(this.document);
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				//	A chaque it�ration, l'objet tend vers la bonne largeur.
				for ( int i=0 ; i<10 ; i++ )
				{
					Rectangle objBox = obj.BoundingBoxGeom;
					Rectangle finalBox = objBox;

					if ( horizontal )
					{
						finalBox.Left  = globalBox.Left;
						finalBox.Right = globalBox.Right;
					}
					else
					{
						finalBox.Bottom = globalBox.Bottom;
						finalBox.Top    = globalBox.Top;
					}
					selector.QuickScale(objBox, finalBox);

					obj.MoveGlobalStarting();
					obj.MoveGlobalProcess(selector);
				}
			}
			this.OpletQueueValidateAction();
		}
		#endregion


		#region Color
		public void ColorSelection(ColorSpace cs)
		{
			//	Ajuste la couleur de tous les objets s�lectionn�s, y compris � l'int�rieur
			//	des groupes.
			this.OpletQueueBeginAction(Res.Strings.Action.Color);
			this.DeepSelect(true);

			System.Collections.ArrayList list = new System.Collections.ArrayList();
			this.PropertiesListDeep(list);

			foreach ( Properties.Abstract property in list )
			{
				if ( property.IsStyle )  continue;
				property.ChangeColorSpace(cs);
			}

			this.DeepSelect(false);
			this.OpletQueueValidateAction();
		}

		public void ColorSelection(double adjust, bool stroke)
		{
			//	Ajuste la couleur de tous les objets s�lectionn�s, y compris � l'int�rieur
			//	des groupes.
			if ( adjust == 0.0 )  return;

			this.OpletQueueBeginAction(Res.Strings.Action.Color);
			this.DeepSelect(true);

			System.Collections.ArrayList list = new System.Collections.ArrayList();
			this.PropertiesListDeep(list);

			foreach ( Properties.Abstract property in list )
			{
				if ( property.IsStyle )  continue;
				property.ChangeColor(adjust, stroke);
			}

			this.DeepSelect(false);
			this.OpletQueueValidateAction();
		}

		protected void DeepSelect(bool select)
		{
			//	S�lectionne en profondeur tous les objets d�j� s�lectionn�s.
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				if ( obj is Objects.Group )
				{
					foreach ( Objects.Abstract subObj in this.document.Deep(obj, false) )
					{
						System.Diagnostics.Debug.Assert(subObj.IsSelected != select);
						subObj.Select(select);
					}
				}
			}
		}
		#endregion


		#region Group
		public void MergeSelection()
		{
			//	Fusionne tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Merge) )
			{
				this.Ungroup();
				this.Group();
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		public void ExtractSelection()
		{
			//	Extrait tous les objets s�lectionn�s du groupe.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Extract) )
			{
				this.Extract();
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		public void GroupSelection()
		{
			//	Groupe tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Group) )
			{
				this.Group();
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		public void UngroupSelection()
		{
			//	S�pare tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Ungroup) )
			{
				this.Ungroup();
				this.ActiveViewer.UpdateSelector();
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		protected void Group()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			System.Collections.ArrayList extract = new System.Collections.ArrayList();

			//	Extrait tous les objets s�lectionn�s dans la liste extract.
			Rectangle bbox = Rectangle.Empty;
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				extract.Add(obj);
				bbox.MergeWith(obj.BoundingBoxGroup);
			}

			//	Supprime les objets s�lectionn�s de la liste principale, sans
			//	supprimer les propri�t�s.
			this.UpdateCounters();
			bool bDo = false;
			do
			{
				bDo = false;
				foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
				{
					this.TotalSelected --;
					layer.Objects.Remove(obj);
					bDo = true;
					break;
				}
			}
			while ( bDo );

			//	Cr�e l'objet groupe.
			Objects.Group group = new Objects.Group(this.document, null);
			layer.Objects.Add(group);
			group.UpdateDim(bbox);
			group.Select();
			this.TotalSelected ++;

			//	Remet les objets extraits dans le groupe.
			foreach ( Objects.Abstract obj in extract )
			{
				obj.Deselect();
				group.Objects.Add(obj);
			}

			this.document.Notifier.NotifyArea(group.BoundingBox);
			this.ActiveViewer.UpdateSelector();
		}

		protected void Ungroup()
		{
			this.UpdateCounters();
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

		protected void Extract()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			if ( context.RootStackIsBase )  return;
			Objects.Abstract layer = context.RootObject();
			Objects.Abstract parent = context.RootObject(context.RootStackDeep-1);
			UndoableList src = layer.Objects;
			UndoableList dst = parent.Objects;

			Modifier.Duplicate(this.document, this.document, src, dst, false, new Point(0,0), true);
			this.DeleteSelection();

			this.GroupUpdateParents();
			context.RootStackPop();
			this.DirtyCounters();
			this.ActiveViewer.UpdateSelector();
			this.document.Notifier.NotifyArea(this.ActiveViewer);
		}

		public void InsideSelection()
		{
			//	Entre dans tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Inside) )
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

		public void OutsideSelection()
		{
			//	Sort de tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Outside) )
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
					this.Tool = "ToolSelect";
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

		public void GroupUpdateChildrens()
		{
			//	Adapte les dimensions de tous les groupes fils.
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

		public void GroupUpdateParents()
		{
			//	Adapte les dimensions de tous les groupes parents.
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


		#region Combine and Fragment
		public double ToLinePrecision
		{
			//	Choix du diviseur avec Ctrl.
			get
			{
				return this.toLinePrecision;
			}
			
			set
			{
				if ( this.toLinePrecision != value )
				{
					this.toLinePrecision = value;
					this.document.Notifier.NotifySettingsChanged();
					this.document.IsDirtySerialize = true;
				}
			}
		}

		public void CombineSelection()
		{
			//	Combine tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.UpdateCounters();
			this.OpletQueueBeginAction(Res.Strings.Action.Combine);
			bool error = false;
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			Objects.Bezier bezier = null;
			int total = layer.Objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				Objects.Abstract obj = layer.Objects[index] as Objects.Abstract;
				if ( !obj.IsSelected )  continue;

				Path[] paths = obj.GetPaths();
				if ( paths == null )
				{
					error = true;
				}
				else
				{
					if ( this.TooMany(paths.Length) )  continue;

					foreach ( Path path in paths )
					{
						if ( path.HasZeroElements )  continue;

						if ( bezier == null )
						{
							bezier = new Objects.Bezier(this.document, obj);
							layer.Objects.Add(bezier);
							bezier.Select(true);
							this.XferProperties(bezier, obj);
							this.TotalSelected ++;
						}
			
						if ( bezier.CreateBezierFromPath(path, -1) )
						{
							obj.Mark = true;  // il faudra le d�truire
						}
						else
						{
							obj.Mark = false;  // il ne faudra pas le d�truire
							error = true;
						}
					}
				}
			}
			bezier.CreateFinalise();
			this.Simplify(bezier);
			this.document.Notifier.NotifyArea(bezier.BoundingBox);

			this.DeleteSelection(true);  // d�truit les objets s�lectionn�s et marqu�s
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.Combine;
				this.ActiveViewer.DialogError(message);
			}
		}

		public void UncombineSelection()
		{
			//	S�pare tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.UpdateCounters();
			this.OpletQueueBeginAction(Res.Strings.Action.Uncombine);
			bool error = false;
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				Objects.Abstract obj = layer.Objects[index] as Objects.Abstract;
				if ( !obj.IsSelected )  continue;

				Path[] paths = obj.GetPaths();
				if ( paths == null )
				{
					error = true;
				}
				else
				{
					if ( this.TooMany(paths.Length) )  continue;

					foreach ( Path path in paths )
					{
						if ( path.HasZeroElements )  continue;

						for ( int i=0 ; i<100 ; i++ )
						{
							Objects.Bezier bezier = new Objects.Bezier(this.document, obj);
							layer.Objects.Add(bezier);
							bezier.Select(true);
							this.XferProperties(bezier, obj);
							this.TotalSelected ++;

							if ( bezier.CreateBezierFromPath(path, i) )
							{
								bezier.CreateFinalise();
								this.Simplify(bezier);
								this.document.Notifier.NotifyArea(bezier.BoundingBox);
								obj.Mark = true;  // il faudra le d�truire
							}
							else
							{
								bezier.Dispose();
								layer.Objects.Remove(bezier);
								this.TotalSelected --;

								if ( i == 0 )  // aucun objet cr�� ?
								{
									obj.Mark = false;  // il ne faudra pas le d�truire
									error = true;
								}

								break;
							}
						}
					}
				}
			}
			this.DeleteSelection(true);  // d�truit les objets s�lectionn�s et marqu�s
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.Uncombine;
				this.ActiveViewer.DialogError(message);
			}
		}

		public void ToBezierSelection()
		{
			//	Converti en B�zier tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.UpdateCounters();
			this.OpletQueueBeginAction(Res.Strings.Action.ToBezier);
			bool error = false;
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				Objects.Abstract obj = layer.Objects[index] as Objects.Abstract;
				if ( !obj.IsSelected )  continue;

				Path[] paths = obj.GetPaths();
				if ( paths == null )
				{
					error = true;
				}
				else
				{
					if ( this.TooMany(paths.Length) )  continue;

					foreach ( Path path in paths )
					{
						if ( path.HasZeroElements )  continue;

						Objects.Bezier bezier = new Objects.Bezier(this.document, obj);
						layer.Objects.Add(bezier);
						bezier.Select(true);
						this.XferProperties(bezier, obj);
						this.TotalSelected ++;

						if ( bezier.CreateBezierFromPath(path, -1) )
						{
							bezier.CreateFinalise();
							this.Simplify(bezier);
							this.document.Notifier.NotifyArea(bezier.BoundingBox);
							obj.Mark = true;  // il faudra le d�truire
						}
						else
						{
							obj.Mark = false;  // il ne faudra pas le d�truire
							bezier.Dispose();
							layer.Objects.Remove(bezier);
							this.TotalSelected --;
							error = true;
						}
					}
				}
			}
			this.DeleteSelection(true);  // d�truit les objets s�lectionn�s et marqu�s
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.ToBezier;
				this.ActiveViewer.DialogError(message);
			}
		}

		public void ToPolySelection()
		{
			//	Converti en polygone tous les objets s�lectionn�s.
			double precision = this.ToLinePrecision*0.19+0.01;  // 0.01 .. 0.2

			if ( this.ActiveViewer.IsCreating )  return;
			this.UpdateCounters();
			this.OpletQueueBeginAction(Res.Strings.Action.ToPoly);
			bool error = false;
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				Objects.Abstract obj = layer.Objects[index] as Objects.Abstract;
				if ( !obj.IsSelected )  continue;

				Path[] paths = obj.GetPaths();
				if ( paths == null )
				{
					error = true;
				}
				else
				{
					if ( this.TooMany(paths.Length) )  continue;

					foreach ( Path path in paths )
					{
						if ( path.HasZeroElements )  continue;

						Path p = path;
						if ( precision != 0 )
						{
							p = new Path();
							p.Append(path, precision, 0.0);
						}

						Objects.Poly poly = new Objects.Poly(this.document, obj);
						layer.Objects.Add(poly);
						poly.Select(true);
						this.XferProperties(poly, obj);
						this.TotalSelected ++;

						if ( poly.CreatePolyFromPath(p, -1) )
						{
							poly.CreateFinalise();
							this.Simplify(poly);
							this.document.Notifier.NotifyArea(poly.BoundingBox);
							obj.Mark = true;  // il faudra le d�truire
						}
						else
						{
							obj.Mark = false;  // il ne faudra pas le d�truire
							poly.Dispose();
							layer.Objects.Remove(poly);
							this.TotalSelected --;
							error = true;
						}
					}
				}
			}
			this.DeleteSelection(true);  // d�truit les objets s�lectionn�s et marqu�s
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.ToPoly;
				this.ActiveViewer.DialogError(message);
			}
		}

		public void ToTextBox2Selection()
		{
			//	Converti en TextBox2 tous les TextBox et TextLine s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.UpdateCounters();
			this.OpletQueueBeginAction(Res.Strings.Action.ToTextBox2);
			DrawingContext context = this.ActiveViewer.DrawingContext;
			bool iga = context.GridActive;
			context.GridActive = false;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				Objects.Abstract obj = layer.Objects[index] as Objects.Abstract;
				if ( !obj.IsSelected )  continue;

				if ( obj is Objects.TextBox )
				{
					Objects.TextBox  t1 = obj as Objects.TextBox;
					Objects.TextBox2 t2 = new Objects.TextBox2(this.document, obj);
					layer.Objects.Add(t2);

					t2.CreateMouseDown(obj.BoundingBoxThin.BottomLeft, context);
					t2.CreateMouseUp  (obj.BoundingBoxThin.TopRight,   context);

					this.XferProperties(t2, obj);

					string text = t1.SimpleText;
					string face = t1.PropertyTextFont.FontName;
					double size = t1.PropertyTextFont.FontSize;
					t2.EditInsertText(text, face, size);
					t2.UpdateGeometry();
					t2.Select(true);
					this.TotalSelected ++;
					
					obj.Mark = true;  // il faudra le d�truire
				}

				if ( obj is Objects.TextLine )
				{
					Objects.TextLine  t1 = obj as Objects.TextLine;
					Objects.TextLine2 t2 = new Objects.TextLine2(this.document, obj);
					layer.Objects.Add(t2);

					Path path = t1.GetMagnetPath();
					t2.CreateBezierFromPath(path, -1);
					t2.CreateFinalise();
					this.Simplify(t2);

					t2.PropertyLineMode.Width = 0;

					string text = TextLayout.ConvertToSimpleText(t1.Content);
					string face = t1.PropertyTextFont.FontName;
					double size = t1.PropertyTextFont.FontSize;
					t2.EditInsertText(text, face, size);
					t2.UpdateGeometry();
					t2.Select(true);
					this.TotalSelected ++;
					
					obj.Mark = true;  // il faudra le d�truire
				}
			}
			this.DeleteSelection(true);  // d�truit les objets s�lectionn�s et marqu�s
			context.GridActive = iga;
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();
		}

		public void ToSimplestSelection()
		{
			//	Converti en B�zier tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.UpdateCounters();
			this.OpletQueueBeginAction(Res.Strings.Action.ToSimplest);
			bool error = false;
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				Objects.Abstract obj = layer.Objects[index] as Objects.Abstract;
				if ( !obj.IsSelected )  continue;

				Path[] paths = obj.GetPaths();
				if ( paths == null )
				{
					error = true;
				}
				else
				{
					if ( this.TooMany(paths.Length) )  continue;

					foreach ( Path path in paths )
					{
						if ( path.HasZeroElements )  continue;

						Objects.Bezier bezier = new Objects.Bezier(this.document, obj);
						layer.Objects.Add(bezier);
						bezier.Select(true);
						this.XferProperties(bezier, obj);
						this.TotalSelected ++;

						Path simplyPath = Geometry.PathToCurve(path);

						if ( bezier.CreateBezierFromPath(simplyPath, -1) )
						{
							bezier.CreateFinalise();
							this.Simplify(bezier);
							this.document.Notifier.NotifyArea(bezier.BoundingBox);
							obj.Mark = true;  // il faudra le d�truire
						}
						else
						{
							obj.Mark = false;  // il ne faudra pas le d�truire
							bezier.Dispose();
							layer.Objects.Remove(bezier);
							this.TotalSelected --;
							error = true;
						}
					}
				}
			}
			this.DeleteSelection(true);  // d�truit les objets s�lectionn�s et marqu�s
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.ToBezier;
				this.ActiveViewer.DialogError(message);
			}
		}

		public void FragmentSelection()
		{
			//	Fragmente tous les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.UpdateCounters();
			this.OpletQueueBeginAction(Res.Strings.Action.Fragment);
			bool error = false;
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				Objects.Abstract obj = layer.Objects[index] as Objects.Abstract;
				if ( !obj.IsSelected )  continue;

				if ( this.FragmentObject(obj) )
				{
					obj.Mark = true;  // il faudra le d�truire
				}
				else
				{
					error = true;
				}
			}
			this.DeleteSelection(true);  // d�truit les objets s�lectionn�s et marqu�s
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.Fragment;
				this.ActiveViewer.DialogError(message);
			}
		}

		protected bool FragmentObject(Objects.Abstract obj)
		{
			//	Fragmente un objet.
			Path[] paths = obj.GetPaths();
			if ( paths == null )
			{
				return false;
			}
			else
			{
				if ( this.TooMany(paths.Length) )  return false;

				foreach ( Path path in paths )
				{
					if ( path.HasZeroElements )  continue;

					PathElement[] elements;
					Point[] points;
					path.GetElements(out elements, out points);
					if ( elements.Length > 100 )  return false;

					Point start = new Point(0, 0);
					Point current = new Point(0, 0);
					Point p1 = new Point(0, 0);
					Point p2 = new Point(0, 0);
					Point p3 = new Point(0, 0);
					int i = 0;
					while ( i < elements.Length )
					{
						switch ( elements[i] & PathElement.MaskCommand )
						{
							case PathElement.MoveTo:
								current = points[i++];
								start = current;
								break;

							case PathElement.LineTo:
								p1 = points[i++];
								this.FragmentCreateLine(current, p1, obj);
								current = p1;
								break;

							case PathElement.Curve3:
								p1 = points[i];
								p2 = points[i++];
								p3 = points[i++];
								Geometry.BezierS1ToS2(current, ref p1, ref p2, p3);
								this.FragmentCreateBezier(current, p1, p2, p3, obj);
								current = p3;
								break;

							case PathElement.Curve4:
								p1 = points[i++];
								p2 = points[i++];
								p3 = points[i++];
								this.FragmentCreateBezier(current, p1, p2, p3, obj);
								current = p3;
								break;

							default:
								if ( (elements[i] & PathElement.FlagClose) != 0 )
								{
									this.FragmentCreateLine(current, start, obj);
								}
								i ++;
								break;
						}
					}
				}
			}
			return true;
		}

		protected bool FragmentCreateLine(Point p1, Point p2, Objects.Abstract model)
		{
			//	Cr�e un fragment de ligne droite.
			if ( p1 == p2 )  return false;
			Objects.Line line = new Objects.Line(this.document, model);
			line.CreateFromPoints(p1, p2);
			line.Select(true);
			this.TotalSelected ++;
			this.VisibilityForce(line);

			Objects.Abstract layer = this.ActiveViewer.DrawingContext.RootObject();
			layer.Objects.Add(line);

			this.document.Notifier.NotifyArea(line.BoundingBox);
			return true;
		}

		protected bool FragmentCreateBezier(Point p1, Point s1, Point s2, Point p2, Objects.Abstract model)
		{
			//	Cr�e un fragment de ligne courbe.
			if ( p1 == p2 )  return false;
			Objects.Bezier bezier = new Objects.Bezier(this.document, model);
			bezier.CreateFromPoints(p1, s1, s2, p2);
			bezier.CreateFinalise();
			this.Simplify(bezier);
			this.TotalSelected ++;
			bezier.PropertyFillGradient.FillType = Properties.GradientFillType.None;
			bezier.PropertyFillGradient.Color1 = RichColor.FromAlphaRgb(0, 1,1,1);
			bezier.PropertyPolyClose.BoolValue = false;
			this.VisibilityForce(bezier);

			Objects.Abstract layer = this.ActiveViewer.DrawingContext.RootObject();
			layer.Objects.Add(bezier);

			this.document.Notifier.NotifyArea(bezier.BoundingBox);
			return true;
		}

		protected void XferProperties(Objects.Abstract obj, Objects.Abstract model)
		{
			//	Reprend �ventuellement quelques propri�t�s � l'objet model pour
			//	un objet polygone ou b�zier.
			if ( model is Objects.TextLine )
			{
				obj.PropertyFillGradient.FillType = Properties.GradientFillType.None;
				obj.PropertyFillGradient.Color1 = model.PropertyTextFont.FontColor;
				obj.PropertyLineMode.Width = 0.0;
			}

			if ( model is Objects.TextLine2 ||
				 model is Objects.TextBox2  )
			{
				obj.PropertyFillGradient.FillType = Properties.GradientFillType.None;
				obj.PropertyFillGradient.Color1 = RichColor.FromBrightness(0);  // noir
				obj.PropertyLineMode.Width = 0.0;
			}
		}

		protected void Simplify(Objects.Abstract obj)
		{
			//	Simplifie un objet.
			Properties.Arrow arrow = obj.PropertyArrow;
			if ( arrow != null )
			{
				arrow.SetArrowType(0, Properties.ArrowType.Right);
				arrow.SetArrowType(1, Properties.ArrowType.Right);
			}

			Properties.Corner corner = obj.PropertyCorner;
			if ( corner != null )
			{
				corner.CornerType = Properties.CornerType.Right;
			}
		}

		protected void VisibilityForce(Objects.Abstract obj)
		{
			//	Force un objet � �tre visible, c'est-�-dire � avoir un trait d'�paisseur non nulle.
			Properties.Line line = obj.PropertyLineMode;
			if ( line == null )  return;
			if ( line.Width != 0.0 )  return;

			if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
			{
				line.Width = 1.0;  // 0.1mm
			}
			else
			{
				line.Width = 1.27;  // 0.05in
			}
		}
		#endregion


		#region ShaperHandle
		public void ShaperHandleUpdate(CommandDispatcher cd)
		{
			//	Met � jour toutes les commandes pour le modeleur.
			if ( this.IsToolShaper && this.TotalSelected != 0 )
			{
				bool enable;
				System.Collections.ArrayList actives = new System.Collections.ArrayList();

				this.ShaperHandleState("Add", out enable, actives);
				this.ShaperHandleState(cd, "ShaperHandleAdd", enable, actives, "");

				this.ShaperHandleState("Continue", out enable, actives);
				this.ShaperHandleState(cd, "ShaperHandleContinue",  enable, actives, "");

				this.ShaperHandleState("Sub", out enable, actives);
				this.ShaperHandleState(cd, "ShaperHandleSub", enable, actives, "");

				this.ShaperHandleState("Segment", out enable, actives);
				this.ShaperHandleState(cd, "ShaperHandleToLine",  enable, actives, "ToLine");
				this.ShaperHandleState(cd, "ShaperHandleToCurve", enable, actives, "ToCurve");

				this.ShaperHandleState("Curve", out enable, actives);
				this.ShaperHandleState(cd, "ShaperHandleSym",    enable, actives, "Sym");
				this.ShaperHandleState(cd, "ShaperHandleSmooth", enable, actives, "Smooth");
				this.ShaperHandleState(cd, "ShaperHandleDis",    enable, actives, "Dis");

				this.ShaperHandleState("CurveLine", out enable, actives);
				this.ShaperHandleState(cd, "ShaperHandleInline", enable, actives, "Inline");
				this.ShaperHandleState(cd, "ShaperHandleFree",   enable, actives, "Free");

				this.ShaperHandleState("Poly", out enable, actives);
				this.ShaperHandleState(cd, "ShaperHandleSimply", enable, actives, "Simply");
				this.ShaperHandleState(cd, "ShaperHandleCorner", enable, actives, "Corner");
			}
			else
			{
				cd.GetCommandState("ShaperHandleAdd").Enable = false;
				cd.GetCommandState("ShaperHandleContinue").Enable = false;
				cd.GetCommandState("ShaperHandleSub").Enable = false;
				cd.GetCommandState("ShaperHandleToLine").Enable = false;
				cd.GetCommandState("ShaperHandleToCurve").Enable = false;
				cd.GetCommandState("ShaperHandleSym").Enable = false;
				cd.GetCommandState("ShaperHandleSmooth").Enable = false;
				cd.GetCommandState("ShaperHandleDis").Enable = false;
				cd.GetCommandState("ShaperHandleInline").Enable = false;
				cd.GetCommandState("ShaperHandleFree").Enable = false;
				cd.GetCommandState("ShaperHandleSimply").Enable = false;
				cd.GetCommandState("ShaperHandleCorner").Enable = false;
			}
		}

		protected void ShaperHandleState(string family, out bool enable, System.Collections.ArrayList actives)
		{
			enable = false;
			actives.Clear();

			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.ShaperHandleState(family, ref enable, actives);
			}
		}

		protected void ShaperHandleState(CommandDispatcher cd, string cmd, bool enable, System.Collections.ArrayList actives, string active)
		{
			cd.GetCommandState(cmd).Enable = enable;
			cd.GetCommandState(cmd).ActiveState = actives.Contains(active) ? Common.Widgets.ActiveState.Yes : Common.Widgets.ActiveState.No;
		}

		public void ShaperHandleCommand(string cmd)
		{
			//	Effectue une commande du modeleur sur une poign�e.
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.ShaperHandleCommand(cmd);
			}
		}
		#endregion


		#region Booolean
		public void BooleanSelection(Drawing.PathOperation op)
		{
			//	Op�rations bool�enne sur tous les objets s�lectionn�s.
			double precision = this.ToLinePrecision*0.19+0.01;  // 0.01 .. 0.2

			if ( this.ActiveViewer.IsCreating )  return;
			this.UpdateCounters();
			this.OpletQueueBeginAction(Res.Strings.Action.BooleanMain);
			bool error = false;
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Path pathResult = new Path();
			Objects.Abstract model = null;
			Objects.Abstract layer = context.RootObject();
			int total = layer.Objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				int ii = index;
				Drawing.PathOperation oper = op;
				if ( op == Drawing.PathOperation.AMinusB )
				{
					ii = total-index-1;
				}
				else if ( op == Drawing.PathOperation.BMinusA )
				{
					oper = Drawing.PathOperation.AMinusB;
				}

				Objects.Abstract obj = layer.Objects[ii] as Objects.Abstract;
				if ( !obj.IsSelected )  continue;

				Path[] paths = obj.GetPaths();
				if ( paths == null )
				{
					error = true;
				}
				else
				{
					if ( this.TooMany(paths.Length) )  continue;

					foreach ( Path path in paths )
					{
						if ( path.HasZeroElements )  continue;

						if ( model == null )
						{
							model = obj;
							pathResult.Append(path, precision, 0.0);
						}
						else
						{
							Path pathLight = new Path();
							pathLight.Append(path, precision, 0.0);
							pathResult = Drawing.Path.Combine(pathResult, pathLight, oper);
						}

						obj.Mark = true;  // il faudra le d�truire
					}
				}
			}

			Objects.Bezier bezier = new Objects.Bezier(this.document, model);
			layer.Objects.Add(bezier);
			bezier.Select(true);
			this.XferProperties(bezier, model);
			this.TotalSelected ++;

			if ( bezier.CreateBezierFromPath(pathResult, -1) )
			{
				bezier.CreateFinalise();
				this.Simplify(bezier);
				this.document.Notifier.NotifyArea(bezier.BoundingBox);
				this.DeleteSelection(true);  // d�truit les objets s�lectionn�s et marqu�s
			}
			else
			{
				bezier.Dispose();
				layer.Objects.Remove(bezier);
				this.TotalSelected --;
				this.ClearMarks();
				error = true;
			}

			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.Boolean;
				this.ActiveViewer.DialogError(message);
			}
		}

		protected bool TooMany(int total)
		{
			if ( total < 100 )  return false;

			string question = string.Format(Res.Strings.Question.ManyObjects, total);
			return !this.ActiveViewer.DialogYesNo(question);
		}
		#endregion


		#region Hide
		public void HideSelection()
		{
			//	Cache tous les objets s�lectionn�s du calque courant.
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.HideSel) )
			{
				this.UpdateCounters();
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
				this.ActiveViewer.UpdateSelector();
				this.OpletQueueValidateAction();
			}
		}

		public void HideRest()
		{
			//	Cache tous les objets non s�lectionn�s du calque courant.
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.HideRest) )
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

		protected void HideObjectAndSoons(Objects.Abstract obj)
		{
			//	Cache un objet et tous ses fils.
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

		public void HideCancel()
		{
			//	Vend visible tous les objets cach�s de la page (donc dans tous les calques).
			if ( this.ActiveViewer.IsCreating )  return;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.HideCancel) )
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


		#region Text Find and Replace
		public string GetSelectedWord()
		{
			//	Retourne le mot actuellement s�lectionn�.
			Objects.AbstractText edit = this.RetEditObject();
			if ( edit == null )  return null;

			if ( edit.TextFlow.TextNavigator.HasRealSelection )
			{
				string[] texts = edit.TextFlow.TextNavigator.GetSelectedTexts();
				return texts[0];
			}

			//	Si aucun mot n'est s�lectionn�, cherche le mot o� est le curseur.
			int pos = edit.TextFlow.TextNavigator.CursorPosition;
			string text = edit.TextFlow.TextStory.GetDebugText();
			int length = text.Length-1;  // ignore le EOT � la fin
			if ( length <= 0 )  return null;

			int start = pos;
			if ( start >= length )  start --;
			while ( start > 0 )
			{
				char c1 = text[start-1];
				char c2 = text[start];
				if ( Text.Unicode.IsWordStart(c2, c1) )  break;
				start --;
			}

			int end = pos;
			if ( end > 0 )  end --;
			while ( end < length-1 )
			{
				char c1 = text[end];
				char c2 = text[end+1];
				if ( Text.Unicode.IsWordEnd(c2, c1) )  break;
				end ++;
			}

			if ( start > end )  return null;

			return text.Substring(start, end-start+1);
		}

		public bool TextReplace(string find, string replace, Misc.StringSearch mode)
		{
			//	Effectue une recherche ou un remplacement dans un objet texte du document.
			//	Pour une simple recherche, 'replace' doit �tre null.
			if ( find == "" )
			{
				this.ActiveViewer.DialogError(Res.Strings.Error.NotFound);
				return false;
			}
			
			if ( this.document.TextFlows.Count == 0 )
			{
				this.ActiveViewer.DialogError(Res.Strings.Error.NoText);
				return false;
			}
			
			if ( replace == null )
			{
				this.OpletQueueBeginAction(string.Format(Res.Strings.Action.UndoFind, Misc.Resume(find)));
			}
			else
			{
				this.OpletQueueBeginAction(string.Format(Res.Strings.Action.UndoReplace, Misc.Resume(find, 10), Misc.Resume(replace, 10)));
			}

			TextFlow textFlow = null;
			bool skipFirst = false;
			Objects.AbstractText edit = this.RetEditObject();
			if ( edit == null )  // aucun objet en �dition ?
			{
				if ( (mode&Misc.StringSearch.EndToStart) != 0 )  // en arri�re ?
				{
					textFlow = this.document.TextFlows[this.document.TextFlows.Count-1] as TextFlow;
					textFlow.MetaNavigator.ClearSelection();
					textFlow.TextNavigator.MoveTo(Text.TextNavigator.Target.WordEnd, 1);  // d�marre la recherche � la fin du dernier TextFlow
				}
				else
				{
					textFlow = this.document.TextFlows[0] as TextFlow;
					textFlow.MetaNavigator.ClearSelection();
					textFlow.TextNavigator.MoveTo(Text.TextNavigator.Target.TextStart, 1);  // d�marre la recherche au d�but du premier TextFlow
				}
			}
			else	// il existe un objet en �dition ?
			{
				textFlow = edit.TextFlow;  // d�marre la recherche dans l'objet �dit�
				if ( replace == null && textFlow.TextNavigator.HasRealSelection )  skipFirst = true;
			}

			if ( !Modifier.TextFind(this.document, ref textFlow, find, skipFirst, mode) )
			{
				this.OpletQueueValidateAction();
				this.ActiveViewer.DialogError(Res.Strings.Error.NotFound);
				return false;
			}

			edit = textFlow.FindObject();  // cherche l'objet contenant le texte trouv�
			if ( edit != null )
			{
				this.SetEditObject(edit, true);  // �dite l'objet trouv�
			}

			if ( replace != null )
			{
				textFlow.TextNavigator.Delete();  // supprime la cha�ne cherch�e
				textFlow.TextNavigator.Insert(replace);  // ins�re la cha�ne de remplacement

				int pos = textFlow.TextNavigator.CursorPosition;
				textFlow.TextNavigator.MoveTo(pos-replace.Length, 1);
				textFlow.TextNavigator.StartSelection();  // s�lectionne la cha�ne de remplacement
				textFlow.TextNavigator.MoveTo(pos, 1);
				textFlow.TextNavigator.EndSelection();

				this.document.IsDirtySerialize = true;
			}

			this.OpletQueueValidateAction();
			return true;
		}

		protected static bool TextFind(Document document, ref TextFlow textFlow, string find, bool skipFirst, Misc.StringSearch mode)
		{
			//	Cherche la prochaine occurence d'un texte dans un TextFlow ou dans le prochain TextFlow.
			TextFlow initialFlow = textFlow;
			bool last = false;

			if ( (mode&Misc.StringSearch.EndToStart) != 0 )  // en arri�re ?
			{
				int startPosition = 0;
				int endPosition   = textFlow.TextNavigator.CursorPosition;  // premi�re partie du premier flux
				if ( skipFirst )  endPosition --;

				while ( true )
				{
					string text = textFlow.TextStory.GetDebugText();
					int i = Misc.IndexOf(text, find, endPosition-1, endPosition-startPosition+1, mode);
					if ( i == -1 && last )  // pas trouv� dans le dernier flux ?
					{
						return false;
					}
					if ( i != -1 )  // trouv� ?
					{
						textFlow.MetaNavigator.ClearSelection();
						textFlow.TextNavigator.MoveTo(i+find.Length, 1);
						textFlow.TextNavigator.StartSelection();
						textFlow.TextNavigator.MoveTo(i, 1);  // le curseur est au d�but du mot
						textFlow.TextNavigator.EndSelection();
						return true;
					}

					i = document.TextFlows.IndexOf(textFlow);
					i --;  if ( i < 0 )  i = document.TextFlows.Count-1;
					textFlow = document.TextFlows[i] as TextFlow;  // flux pr�c�dent

					if ( textFlow == initialFlow )  // revenu dans le flux initial ?
					{
						last = true;
						startPosition = textFlow.TextNavigator.CursorPosition;
						endPosition   = textFlow.TextNavigator.TextLength;  // deuxi�me partie du premier flux
						startPosition -= find.Length;
					}
					else
					{
						startPosition = 0;
						endPosition   = textFlow.TextNavigator.TextLength;  // tout le flux
					}
				}
			}
			else	// en avant ?
			{
				int startPosition = textFlow.TextNavigator.CursorPosition;
				int endPosition   = textFlow.TextNavigator.TextLength;  // deuxi�me partie du premier flux
				if ( skipFirst )  startPosition ++;

				while ( true )
				{
					string text = textFlow.TextStory.GetDebugText();
					int i = Misc.IndexOf(text, find, startPosition, endPosition-startPosition, mode);
					if ( i == -1 && last )  // pas trouv� dans le dernier flux ?
					{
						return false;
					}
					if ( i != -1 )  // trouv� ?
					{
						textFlow.MetaNavigator.ClearSelection();
						textFlow.TextNavigator.MoveTo(i, 1);
						textFlow.TextNavigator.StartSelection();
						textFlow.TextNavigator.MoveTo(i+find.Length, 1);  // le curseur est � la fin du mot
						textFlow.TextNavigator.EndSelection();
						return true;
					}

					i = document.TextFlows.IndexOf(textFlow);
					i ++;  if ( i >= document.TextFlows.Count )  i = 0;
					textFlow = document.TextFlows[i] as TextFlow;  // flux suivant

					if ( textFlow == initialFlow )  // revenu dans le flux initial ?
					{
						last = true;
						startPosition = 0;
						endPosition   = textFlow.TextNavigator.CursorPosition;  // premi�re partie du premier flux
						endPosition += find.Length;
					}
					else
					{
						startPosition = 0;
						endPosition   = textFlow.TextNavigator.TextLength;  // tout le flux
					}
				}
			}
		}
		#endregion


		#region Page
		public void InitiateChangingPage()
		{
			//	Commence un changement de page.
			int rank = this.ActiveViewer.DrawingContext.CurrentPage;
			if ( rank < this.document.GetObjects.Count )
			{
				Objects.Page page = this.document.GetObjects[rank] as Objects.Page;
				page.CurrentLayer = this.ActiveViewer.DrawingContext.CurrentLayer;
			}

			this.DeselectAll();
			this.ActiveViewer.ClearHilite();
			this.ActiveViewer.ClearEditCreateRect();
		}

		public void TerminateChangingPage(int rank)
		{
			//	Termine un changement de page.
			Objects.Page page = this.document.GetObjects[rank] as Objects.Page;
			int layer = page.CurrentLayer;
			this.ActiveViewer.DrawingContext.PageLayer(rank, layer);
			this.DirtyCounters();
		}

		public void PageNew(int rank, string name)
		{
			//	Cr�e une nouvelle page.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.PageNew) )
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

				this.UpdatePageAfterChanging();
				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyPagesChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void PageDuplicate(int rank, string name)
		{
			//	Duplique une nouvelle page.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.PageDuplicate) )
			{
				this.InitiateChangingPage();

				UndoableList list = this.document.GetObjects;  // liste des pages
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);

				Objects.Page srcPage = list[rank] as Objects.Page;

				Objects.Page page = new Objects.Page(this.document, srcPage);
				page.CloneObject(srcPage);
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

				this.UpdatePageAfterChanging();
				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyPagesChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void PageDelete(int rank)
		{
			//	Supprime une page.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.PageDelete) )
			{
				this.InitiateChangingPage();

				UndoableList list = this.document.GetObjects;  // liste des pages
				if ( list.Count <= 1 )  return;  // il doit rester une page
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);

				UndoableList pages = this.document.GetObjects;
				Objects.Page page = pages[rank] as Objects.Page;
				this.UpdatePageDelete(page);
				this.DeleteGroup(page);
				page.Dispose();
				list.RemoveAt(rank);

				rank = System.Math.Min(rank, list.Count-1);
				this.TerminateChangingPage(rank);

				this.UpdatePageAfterChanging();
				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyPagesChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void PageSwap(int rank1, int rank2)
		{
			//	Permute deux pages.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.PageSwap) )
			{
				this.InitiateChangingPage();

				UndoableList list = this.document.GetObjects;  // liste des pages
				rank1 = System.Math.Max(rank1, 0);
				rank1 = System.Math.Min(rank1, list.Count-1);
				rank2 = System.Math.Max(rank2, 0);
				rank2 = System.Math.Min(rank2, list.Count-1);

				UndoableList pages = this.document.GetObjects;
				Objects.Page temp = pages[rank1] as Objects.Page;
				pages.RemoveAt(rank1);
				pages.Insert(rank2, temp);

				this.TerminateChangingPage(rank2);

				this.UpdatePageAfterChanging();
				this.document.Notifier.NotifyArea();
				this.document.Notifier.NotifyPagesChanged();
				this.OpletQueueValidateAction();
			}
		}

		public string PageShortName(int rank)
		{
			//	Retourne le nom court d'une page ("n" ou "Mn").
			UndoableList pages = this.document.GetObjects;
			Objects.Page page = pages[rank] as Objects.Page;
			return page.ShortName;
		}

		public string PageName(int rank)
		{
			//	Retourne le nom d'une page.
			UndoableList pages = this.document.GetObjects;
			Objects.Page page = pages[rank] as Objects.Page;
			return page.Name;
		}

		public void PageName(int rank, string name)
		{
			//	Change le nom d'une page.
			this.document.IsDirtySerialize = true;
			using ( this.OpletQueueBeginAction(Res.Strings.Action.PageName, "ChangePageName") )
			{
				UndoableList pages = this.document.GetObjects;
				Objects.Page page = pages[rank] as Objects.Page;
				page.Name = name;

				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyPageChanged(page);
				this.OpletQueueValidateAction();
			}
		}

		public int PrintableTotalPages()
		{
			//	Retourne le nombre total de pages imprimables.
			UndoableList pages = this.document.GetObjects;
			int total = pages.Count;
			int printable = 0;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Page page = pages[i] as Objects.Page;

				if ( page.MasterType == Objects.MasterType.Slave )
				{
					printable ++;
				}
			}
			return printable;
		}

		public int PrintablePageRank(int index)
		{
			//	Retourne le rang d'une page imprimable.
			UndoableList pages = this.document.GetObjects;
			int total = pages.Count;
			int rank = 0;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Page page = pages[i] as Objects.Page;

				if ( page.MasterType == Objects.MasterType.Slave )
				{
					if ( rank == index )  return i;
					rank ++;
				}
			}
			return -1;
		}

		protected void UpdatePageDelete(Objects.Page deletedPage)
		{
			//	Met � jour apr�s une suppression de page.
			UndoableList pages = this.document.GetObjects;
			int total = pages.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Page page = pages[i] as Objects.Page;

				if ( page.MasterPageToUse == deletedPage )
				{
					page.MasterPageToUse = null;
					page.MasterUse = Objects.MasterUse.Never;
				}
			}
		}

		public void UpdatePageAfterChanging()
		{
			//	Met � jour tout ce qu'il faut apr�s un changement de page (cr�ation d'une
			//	nouvelle page, suppression d'une page, etc.).
			this.UpdatePageShortNames();
			this.UpdatePageNumbers();
		}

		public void UpdatePageShortNames()
		{
			//	Met � jour tous les noms courts des pages ("n" ou "Mn").
			UndoableList pages = this.document.GetObjects;
			int total = pages.Count;
			int slaveNumber = 0;
			int masterNumber = 0;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Page page = pages[i] as Objects.Page;

				if ( page.MasterType == Objects.MasterType.Slave )
				{
					page.Rank = slaveNumber;
					slaveNumber ++;
					page.ShortName = string.Format(Res.Strings.Page.ShortName.Default, slaveNumber.ToString());
				}
				else
				{
					page.Rank = masterNumber;
					masterNumber ++;
					page.ShortName = string.Format(Res.Strings.Page.ShortName.Model, masterNumber.ToString());
				}
			}
		}

		public void UpdatePageNumbers()
		{
			//	Met � jour les num�ros de page de tous les objets du document.
			int total = this.document.GetObjects.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Page page = this.document.GetObjects[i] as Objects.Page;
				foreach ( Objects.Abstract obj in this.document.Deep(page) )
				{
					obj.PageNumber = i;
				}
			}
		}

		public void ComputeMasterPageList(System.Collections.ArrayList masterPageList, int pageNumber)
		{
			//	G�n�re la liste des pages ma�tres � utiliser pour une page donn�e.
			masterPageList.Clear();
			Objects.Page currentPage = this.document.GetObjects[pageNumber] as Objects.Page;
			Size currentSize = this.document.GetPageSize(pageNumber);

			if ( currentPage.MasterType == Objects.MasterType.Slave )
			{
				if ( currentPage.MasterUse == Objects.MasterUse.Specific )
				{
					if ( currentPage.MasterPageToUse != null &&
						 currentPage.MasterPageToUse.MasterType != Objects.MasterType.Slave )
					{
						this.ComputeMasterPageList(masterPageList, currentPage.MasterPageToUse, currentSize);
					}
				}

				if ( currentPage.MasterUse == Objects.MasterUse.Default )
				{
					int total = this.document.GetObjects.Count;
					for ( int i=0 ; i<total ; i++ )
					{
						Objects.Page page = this.document.GetObjects[i] as Objects.Page;

						if ( page.MasterAutoStop )
						{
							if ( this.IsRejectMaster(i, pageNumber) )  continue;
						}

						if ( page.MasterType == Objects.MasterType.All )
						{
							this.ComputeMasterPageList(masterPageList, page, currentSize);
						}

						if ( page.MasterType == Objects.MasterType.Even )
						{
							if ( currentPage.Rank%2 != 0 )
							{
								this.ComputeMasterPageList(masterPageList, page, currentSize);
							}
						}

						if ( page.MasterType == Objects.MasterType.Odd )
						{
							if ( currentPage.Rank%2 == 0 )
							{
								this.ComputeMasterPageList(masterPageList, page, currentSize);
							}
						}
					}
				}
			}
			else	// page mod�le ?
			{
				if ( currentPage.MasterSpecific )
				{
					if ( currentPage.MasterPageToUse != null &&
						 currentPage.MasterPageToUse.MasterType != Objects.MasterType.Slave )
					{
						this.ComputeMasterPageList(masterPageList, currentPage.MasterPageToUse, currentSize);
					}
				}
			}

			masterPageList.Reverse();
		}

		protected void ComputeMasterPageList(System.Collections.ArrayList masterPageList, Objects.Page master, Size currentSize)
		{
			if ( masterPageList.Contains(master) )  return;
			if ( this.document.GetPageSize(master) != currentSize )  return;  // pas la m�me taille ?

			masterPageList.Add(master);

			if ( master.MasterSpecific )
			{
				if ( master.MasterPageToUse != null &&
					 master.MasterPageToUse.MasterType != Objects.MasterType.Slave )
				{
					this.ComputeMasterPageList(masterPageList, master.MasterPageToUse, currentSize);
				}
			}
		}

		protected bool IsRejectMaster(int rankMaster, int pageNumber)
		{
			if ( rankMaster >= pageNumber )  return true;

			while ( rankMaster < pageNumber )
			{
				Objects.Page page = this.document.GetObjects[rankMaster] as Objects.Page;
				if ( page.MasterType == Objects.MasterType.Slave )  break;
				rankMaster ++;
			}

			for ( int i=rankMaster ; i<pageNumber ; i++ )
			{
				Objects.Page page = this.document.GetObjects[i] as Objects.Page;
				if ( page.MasterType != Objects.MasterType.Slave )  return true;
			}

			return false;
		}
		#endregion

		#region PageStack
		public class PageStackInfos
		{
			public Objects.Page		Page;
			public Objects.Layer	Layer;
			public string			LayerShortName;
			public string			LayerAutoName;
			public bool				Master;
		}

		public System.Collections.ArrayList GetPageStackInfos(int pageNumber)
		{
			//	Retourne les informations qui r�sument la structure d'une page.
			System.Collections.ArrayList infos = new System.Collections.ArrayList();

			System.Collections.ArrayList masterList = new System.Collections.ArrayList();
			this.ComputeMasterPageList(masterList, pageNumber);

			//	Mets d'abord les premiers calques de toutes les pages ma�tres.
			foreach ( Objects.Page master in masterList )
			{
				int frontier = master.MasterFirstFrontLayer;
				for ( int i=0 ; i<frontier ; i++ )
				{
					Objects.Layer layer = master.Objects[i] as Objects.Layer;
					if ( layer.Print == Objects.LayerPrint.Hide )  continue;

					PageStackInfos info = new PageStackInfos();
					info.Page = master;
					info.Layer = layer;
					info.LayerShortName = Objects.Layer.ShortName(i);
					info.LayerAutoName = Objects.Layer.LayerPositionName(i, master.Objects.Count);
					info.Master = true;
					infos.Add(info);
				}
			}

			//	Mets ensuite tous les calques de la page.
			Objects.Page page = this.document.GetObjects[pageNumber] as Objects.Page;
			int rl = -1;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				rl ++;
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;

				PageStackInfos info = new PageStackInfos();
				info.Page = page;
				info.Layer = layer;
				info.LayerShortName = Objects.Layer.ShortName(rl);
				info.LayerAutoName = Objects.Layer.LayerPositionName(rl, page.Objects.Count);
				info.Master = false;
				infos.Add(info);
			}

			//	Mets finalement les derniers calques de toutes les pages ma�tres.
			foreach ( Objects.Page master in masterList )
			{
				int frontier = master.MasterFirstFrontLayer;
				int total = master.Objects.Count;
				for ( int i=frontier ; i<total ; i++ )
				{
					Objects.Layer layer = master.Objects[i] as Objects.Layer;
					if ( layer.Print == Objects.LayerPrint.Hide )  continue;

					PageStackInfos info = new PageStackInfos();
					info.Page = master;
					info.Layer = layer;
					info.LayerShortName = Objects.Layer.ShortName(i);
					info.LayerAutoName = Objects.Layer.LayerPositionName(i, master.Objects.Count);
					info.Master = true;
					infos.Add(info);
				}
			}

			return infos;
		}
		#endregion


		#region Layer
		public void InitiateChangingLayer()
		{
			//	Commence un changement de calque.
			this.DeselectAll();
			this.ActiveViewer.ClearHilite();
		}

		public void TerminateChangingLayer(int rank)
		{
			//	Termine un changement de calque.
			int page = this.ActiveViewer.DrawingContext.CurrentPage;
			this.ActiveViewer.DrawingContext.PageLayer(page, rank);
			this.DirtyCounters();
		}

		public void LayerNew(int rank, string name)
		{
			//	Cr�e un nouveau calque.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerNew) )
			{
				this.InitiateChangingLayer();

				//	Liste des calques:
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count);

				Objects.Layer layer = new Objects.Layer(this.document, null);
				layer.Name = name;
				list.Insert(rank, layer);

				this.TerminateChangingLayer(rank);

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void LayerNewSel(int rank, string name)
		{
			//	Cr�e un nouveau calque contenant les objets s�lectionn�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerNewSel) )
			{
				this.InsertOpletDirtyCounters();

				//	Liste des calques:
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);

				Objects.Layer srcLayer = list[rank] as Objects.Layer;

				Objects.Layer layer = new Objects.Layer(this.document, null);
				if ( name == "" )
				{
					layer.Name = Misc.CopyName(srcLayer.Name, Res.Strings.Misc.Extract, Res.Strings.Misc.ExtractOf);
				}
				else
				{
					layer.Name = name;
				}

				UndoableList src = srcLayer.Objects;
				UndoableList dst = layer.Objects;
				Modifier.Duplicate(this.document, this.document, src, dst, false, new Point(0,0), true);
				this.DeleteSelection();

				this.InitiateChangingLayer();
				list.Insert(rank+1, layer);
				this.TerminateChangingLayer(rank+1);
				this.ActiveViewer.UpdateSelector();

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void LayerDuplicate(int rank, string name)
		{
			//	Duplique un calque.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerDuplicate) )
			{
				this.InitiateChangingLayer();

				//	Liste des calques:
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

				this.TerminateChangingLayer(rank+1);

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void LayerDelete(int rank)
		{
			//	Supprime un calque.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerDelete) )
			{
				this.InitiateChangingLayer();

				//	Liste des calques:
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				if ( list.Count <= 1 )  return;  // il doit rester un calque
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);

				UndoableList layers = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				Objects.Layer layer = layers[rank] as Objects.Layer;
				this.DeleteGroup(layer);
				layer.Dispose();
				list.RemoveAt(rank);

				rank = System.Math.Min(rank, list.Count-1);
				this.TerminateChangingLayer(rank);

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void LayerMerge(int rankSrc, int rankDst)
		{
			//	Fusionne deux calques.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerMerge) )
			{
				this.InitiateChangingLayer();

				//	Liste des calques:
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				rankSrc = System.Math.Max(rankSrc, 0);
				rankSrc = System.Math.Min(rankSrc, list.Count-1);
				rankDst = System.Math.Max(rankDst, 0);
				rankDst = System.Math.Min(rankDst, list.Count-1);

				if ( rankSrc < rankDst )
				{
					Misc.Swap(ref rankSrc, ref rankDst);
				}

				Objects.Layer srcLayer = list[rankSrc] as Objects.Layer;
				Objects.Layer dstLayer = list[rankDst] as Objects.Layer;

				if ( srcLayer.Name != "" && dstLayer.Name != "" )
				{
					dstLayer.Name = dstLayer.Name + " + " + srcLayer.Name;
				}
				else if ( srcLayer.Name != "" && dstLayer.Name == "" )
				{
					dstLayer.Name = srcLayer.Name;
				}

				UndoableList src = srcLayer.Objects;
				UndoableList dst = dstLayer.Objects;
				Modifier.Duplicate(this.document, this.document, src, dst, false, new Point(0,0), false);

				srcLayer.Dispose();
				list.RemoveAt(rankSrc);

				this.TerminateChangingLayer(rankDst);

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void LayerSwap(int rank1, int rank2)
		{
			//	Permute deux calques.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerSwap) )
			{
				this.InitiateChangingLayer();

				//	Liste des calques:
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				rank1 = System.Math.Max(rank1, 0);
				rank1 = System.Math.Min(rank1, list.Count-1);
				rank2 = System.Math.Max(rank2, 0);
				rank2 = System.Math.Min(rank2, list.Count-1);

				UndoableList layers = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				Objects.Layer temp = layers[rank1] as Objects.Layer;
				layers.RemoveAt(rank1);
				layers.Insert(rank2, temp);

				this.TerminateChangingLayer(rank2);

				this.document.Notifier.NotifyArea(this.ActiveViewer);
				this.document.Notifier.NotifyLayersChanged();
				this.OpletQueueValidateAction();
			}
		}

		public string LayerName(int rank)
		{
			//	Retourne le nom d'un calque.
			UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
			Objects.Layer layer = list[rank] as Objects.Layer;
			return layer.Name;
		}

		public void LayerName(int rank, string name)
		{
			//	Change le nom d'un calque.
			this.document.IsDirtySerialize = true;
			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerName, "ChangeLayerName") )
			{
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				Objects.Layer layer = list[rank] as Objects.Layer;
				layer.Name = name;

				this.document.Notifier.NotifySelectionChanged();
				this.document.Notifier.NotifyLayerChanged(layer);
				this.OpletQueueValidateAction();
			}
		}

		protected int GetLayerRank(Objects.Abstract search)
		{
			//	Indique le num�ro du calque auquel appartient un objet.
			UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
			for ( int rank=0 ; rank<list.Count ; rank++ )
			{
				Objects.Layer layer = list[rank] as Objects.Layer;
				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj == search )  return rank;
				}
			}
			return -1;
		}
		#endregion

		#region MagnetLayer
		public void MagnetLayerInvert(int rank)
		{
			//	Change l'�tat "objets magn�tiques" d'un calque.
			this.document.IsDirtySerialize = true;
			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerChangeMagnet) )
			{
				UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
				Objects.Layer layer = list[rank] as Objects.Layer;
				layer.Magnet = !layer.Magnet;

				this.document.Notifier.NotifyLayersChanged();
				this.document.Notifier.NotifyMagnetChanged();
				this.OpletQueueValidateAction();
			}
		}

		public bool MagnetLayerState(int rank)
		{
			//	Donne l'�tat "objets magn�tiques" d'un calque.
			UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
			Objects.Layer layer = list[rank] as Objects.Layer;
			return layer.Magnet;
		}

		public bool MagnetLayerDetect(Point pos, Point filterP1, Point filterP2,
									  out Point p1, out Point p2)
		{
			//	D�tecte sur quel segment de droite est la souris.
			double width = this.ActiveViewer.DrawingContext.MagnetMargin;
			double min = 1000000.0;
			p1 = new Point(0,0);
			p2 = new Point(0,0);

			System.Collections.ArrayList layers = this.ActiveViewer.DrawingContext.MagnetLayerList;
			int total = layers.Count;
			for ( int i=total-1 ; i>=0 ; i-- )
			{
				Objects.Layer layer = layers[i] as Objects.Layer;
				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsSelected )  continue;
					if ( obj.IsCreating )  continue;

					Rectangle bbox = obj.BoundingBoxThin;
					bbox.Inflate(width);
					if ( !bbox.Contains(pos) )  continue;

					Path path = obj.GetMagnetPath();
					if ( path == null )  continue;

					Point pp1, s1, s2, pp2;
					if ( Geometry.DetectOutlineRank(path, width, pos, out pp1, out s1, out s2, out pp2) == -1 )  continue;
					if ( pp1 == s1 && pp2 == s2 )  // segment de droite ?
					{
						if ( pp1 == filterP1 && pp2 == filterP2 )  continue;

						Point p = Point.Projection(pp1, pp2, pos);
						double dist = Point.Distance(p, pos);
						if ( dist < min )
						{
							min = dist;
							p1 = pp1;
							p2 = pp2;
						}
					}
				}
			}

			return ( min < 1000000.0 );
		}

		public void ComputeMagnetLayerList(System.Collections.ArrayList magnetLayerList, int pageNumber)
		{
			//	G�n�re la liste des calques magn�tiques � utiliser pour une page donn�e.
			magnetLayerList.Clear();
			Objects.Page page = this.document.GetObjects[pageNumber] as Objects.Page;
			int rank = 0;
			int cl = this.ActiveViewer.DrawingContext.CurrentLayer;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Magnet && (layer.Type != Objects.LayerType.Hide || cl == rank) )
				{
					magnetLayerList.Add(layer);
				}
				rank ++;
			}
		}
		#endregion


		#region Properties
		public bool PropertiesDetail
		{
			//	Mode de repr�sentation pour le panneau des propri�t�s.
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

		public bool PropertiesDetailMany
		{
			//	Indique si on est en mode "d�tail" avec plus d'un objet s�lectionn�.
			get
			{
				return (this.propertiesDetail && this.TotalSelected > 1);
			}
		}

		public void PropertyAdd(Properties.Abstract property)
		{
			//	Ajoute une nouvelle propri�t�.
			this.PropertyList(property).Add(property);

			if ( property.IsStyle )
			{
				this.document.Notifier.NotifyStyleChanged();
			}
		}

		public void PropertyRemove(Properties.Abstract property)
		{
			//	Supprime une propri�t�.
			this.PropertyList(property).Remove(property);

			if ( property.IsStyle )
			{
				this.document.Notifier.NotifyStyleChanged();
			}
		}

		public UndoableList PropertyList(Properties.Abstract property)
		{
			//	Donne la liste � utiliser pour une propri�t�.
			return this.PropertyList(property.IsSelected);
		}

		public UndoableList PropertyList(bool selected)
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

		public void PropertiesList(System.Collections.ArrayList list)
		{
			//	Ajoute toutes les propri�t�s des objets s�lectionn�s dans une liste.
			//	Tient compte du mode propertiesDetail.
			this.OpletQueueEnable = false;
			list.Clear();

			if ( this.tool == "ToolPicker" && this.TotalSelected == 0 )  // pipette ?
			{
				this.ObjectMemoryTool.PropertiesList(list, null);
			}
			else if ( this.IsTool )  // outil select, edit, loupe, etc. ?
			{
				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract layer = context.RootObject();
				foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
				{
					obj.PropertiesList(list, this.propertiesDetail);
				}
			}
			else	// choix d'un objet � cr�er ?
			{
				//	Cr�e un objet factice (document = null), c'est-�-dire sans
				//	propri�t�s, qui servira juste de filtre pour objectMemory.
				Objects.Abstract dummy = Objects.Abstract.CreateObject(null, this.tool, null);
				this.ObjectMemoryTool.PropertiesList(list, dummy);
				dummy.Dispose();
			}

			list.Sort();
			this.OpletQueueEnable = true;
		}

		protected void PropertiesListDeep(System.Collections.ArrayList list)
		{
			//	Ajoute le d�tail de toutes les propri�t�s des objets s�lectionn�s
			//	dans une liste, en vue d'une modifications des couleurs.
			this.OpletQueueEnable = false;
			list.Clear();

			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.PropertiesList(list, true);
			}

			this.OpletQueueEnable = true;
		}
		#endregion


		#region IsPropertiesExtended
		public bool IsPropertiesExtended(Properties.Type type)
		{
			//	Indique si le panneau d'une propri�t� doit �tre �tendu.
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

		public void IsPropertiesExtended(Properties.Type type, bool extended)
		{
			//	Indique si le panneau d'une propri�t� doit �tre �tendu.
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
				obj.HandlePropertiesUpdate();
				this.document.Notifier.NotifyArea(this.ActiveViewer, obj.BoundingBox);
			}
		}
		#endregion


		#region IsTextPanelExtended
		public bool IsTextPanelExtended(TextPanels.Abstract panel)
		{
			//	Indique si un panneau pour le texte est �tendu ou pas.
			if ( this.tableTextPanelExtended == null )
			{
				this.tableTextPanelExtended = new System.Collections.Hashtable();
			}

			System.Type type = panel.GetType();  // cl� pour HashTable

			if ( this.tableTextPanelExtended.ContainsKey(type) )
			{
				return (bool) this.tableTextPanelExtended[type];
			}
			else
			{
				this.tableTextPanelExtended.Add(type, false);
				return false;
			}
		}

		public void IsTextPanelExtended(TextPanels.Abstract panel, bool extended)
		{
			//	Modifie l'�tat �tendu ou pas d'un panneau pour le texte.
			if ( this.tableTextPanelExtended == null )
			{
				this.tableTextPanelExtended = new System.Collections.Hashtable();
			}

			System.Type type = panel.GetType();  // cl� pour HashTable

			if ( this.tableTextPanelExtended.ContainsKey(type) )
			{
				this.tableTextPanelExtended[type] = extended;
			}
			else
			{
				this.tableTextPanelExtended.Add(type, extended);
			}
		}
		#endregion


		#region Zoom
		protected void ShowSelection()
		{
			//	Montre les objets s�lectionn�s si n�cessaire.
			if ( this.TotalSelected == 0 )  return;

			DrawingContext context = this.ActiveViewer.DrawingContext;
			Rectangle box = this.SelectedBbox;
			if ( !this.ActiveViewer.ScrollRectangle.Contains(box) )
			{
				this.ZoomMemorize();
				context.ZoomAndCenter(context.Zoom, box.Center);

				box = this.SelectedBbox;
				if ( !this.ActiveViewer.ScrollRectangle.Contains(box) )
				{
					this.ZoomSel();
				}
			}
		}

		public void ZoomSel()
		{
			//	Zoom sur les objets s�lectionn�s.
			Rectangle bbox = this.SelectedBbox;
			if ( bbox.IsEmpty )  return;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				bbox.Inflate(2);
			}
			else
			{
				bbox.Inflate(20);
			}
			this.ZoomChange(bbox.BottomLeft, bbox.TopRight);
		}

		public void ZoomSelWidth()
		{
			//	Zoom sur la largeur des objets s�lectionn�s.
			Rectangle bbox = this.SelectedBbox;
			if ( bbox.IsEmpty )  return;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				bbox.Inflate(2);
			}
			else
			{
				bbox.Inflate(20);
			}
			this.ZoomChangeWidth(bbox.BottomLeft, bbox.TopRight);
		}

		public void ZoomChange(Point p1, Point p2)
		{
			//	Change le zoom d'un certain facteur pour agrandir une zone rectangulaire.
			Viewer viewer = this.ActiveViewer;
			DrawingContext context = this.ActiveViewer.DrawingContext;

			if ( p1.X == p2.X || p1.Y == p2.Y )  return;
			double fx = viewer.Width/(System.Math.Abs(p1.X-p2.X)*context.ScaleX);
			double fy = viewer.Height/(System.Math.Abs(p1.Y-p2.Y)*context.ScaleY);
			double factor = System.Math.Min(fx, fy);
			this.ZoomChange(factor, (p1+p2)/2);
		}

		public void ZoomChangeWidth(Point p1, Point p2)
		{
			//	Change le zoom d'un certain facteur pour agrandir une zone rectangulaire.
			Viewer viewer = this.ActiveViewer;
			DrawingContext context = this.ActiveViewer.DrawingContext;

			if ( p1.X == p2.X )  return;
			double factor = viewer.Width/(System.Math.Abs(p1.X-p2.X)*context.ScaleX);
			this.ZoomChange(factor, (p1+p2)/2);
		}

		public void ZoomChange(double factor)
		{
			//	Change le zoom d'un certain facteur, avec centrage au milieu du dessin.
			DrawingContext context = this.ActiveViewer.DrawingContext;
			this.ZoomChange(factor, context.Center);
		}

		public void ZoomChange(double factor, Point center)
		{
			//	Change le zoom d'un certain facteur, avec centrage quelconque.
			DrawingContext context = this.ActiveViewer.DrawingContext;

			double newZoom = context.Zoom*factor;
			newZoom = System.Math.Max(newZoom, this.ZoomMin);
			newZoom = System.Math.Min(newZoom, this.ZoomMax);
			if ( newZoom == context.Zoom )  return;

			this.ZoomMemorize();
			context.ZoomAndCenter(newZoom, center);
		}

		public void ZoomValue(double value)
		{
			//	Force une valeur de zoom.
			this.ZoomValue(value, true);
		}

		public void ZoomValue(double value, bool memorize)
		{
			//	Force une valeur de zoom.
			DrawingContext context = this.ActiveViewer.DrawingContext;

			value = System.Math.Max(value, this.ZoomMin);
			value = System.Math.Min(value, this.ZoomMax);
			if ( value == context.Zoom )  return;

			if ( memorize )
			{
				this.ZoomMemorize();
			}

			context.ZoomAndCenter(value, context.Center);
		}

		public int ZoomMemorizeCount
		{
			//	Retourne le nombre de zoom m�moris�s.
			get { return this.zoomHistory.Count; }
		}

		public void ZoomMemorize()
		{
			//	M�morise le zoom actuel.
			DrawingContext context = this.ActiveViewer.DrawingContext;

			ZoomHistory.ZoomElement item = new ZoomHistory.ZoomElement();
			item.Zoom = context.Zoom;
			item.Center = context.Center;
			this.zoomHistory.Add(item);
		}

		public void ZoomPrev()
		{
			//	Revient au niveau de zoom pr�c�dent.
			DrawingContext context = this.ActiveViewer.DrawingContext;

			ZoomHistory.ZoomElement item = this.zoomHistory.Remove();
			if ( item == null )  return;
			context.ZoomAndCenter(item.Zoom, item.Center);
		}

		public double ZoomMin
		{
			//	Retourne le zoom minimal.
			get
			{
				if ( this.document.Type == DocumentType.Pictogram )
				{
					double x = this.SizeArea.Width/this.document.PageSize.Width;
					double y = this.SizeArea.Height/this.document.PageSize.Height;
					return 1.0/System.Math.Min(x,y);
				}
				else
				{
					return 0.1;  // 10%
				}
			}
		}

		public double ZoomMax
		{
			//	Retourne le zoom maximal.
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


		#region Aggregates
		public string AggregateGetSelectedName()
		{
			//	Retourne le nom de l'agr�gat s�lectionn�.
			string name = "";
			this.aggregateUsed = -1;

			if ( this.IsTool )  // objets s�lectionn�s ?
			{
				if ( this.TotalSelected != 0 )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					Objects.Abstract layer = context.RootObject();
					foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
					{
						if ( obj.Aggregates.Count == 0 )  continue;

						string aggName = obj.AggregateName;

						if ( name == "" )
						{
							name = aggName;
							this.aggregateUsed = this.document.Aggregates.IndexOf(obj.Aggregates[0]);
						}
						else
						{
							if ( name != aggName )  // plusieurs agr�gats diff�rents ?
							{
								name = "...";
								this.aggregateUsed = -1;
								break;
							}
						}
					}
				}
			}
			else	// objectMemory ?
			{
				name = this.ObjectMemoryTool.AggregateName;
				this.aggregateUsed = -1;
			}

			return name;
		}

		public bool AggregateIsFreeName(Properties.Aggregate agg, string name)
		{
			//	V�rifie si un nom est possible pour un agr�gat donn�.
			UndoableList aggregates = this.document.Aggregates;
			foreach ( Properties.Aggregate existing in aggregates )
			{
				if ( existing == agg )  continue;
				if ( existing.AggregateName == name )  return false;
			}
			return true;
		}

		public void AggregateNewEmpty(int rank, string name, bool putToList)
		{
			//	Cr�e un nouvel agr�gat vide.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			Properties.Aggregate agg = this.AggregateCreate(name, false);

			UndoableList list = this.document.Aggregates;
			if ( putToList && list.IndexOf(agg) == -1 )
			{
				using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateNew3) )
				{
					rank = System.Math.Max(rank, 0);
					rank = System.Math.Min(rank, list.Count-1);
					list.Insert(rank+1, agg);
					list.Selected = rank+1;

					this.document.Notifier.NotifyStyleChanged();
					this.OpletQueueValidateAction();
				}
			}

			this.AggregateUse(agg);
		}

		public void AggregateNew3(int rank, string name, bool putToList)
		{
			//	Cr�e un nouvel agr�gat avec seulement 3 propri�t�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			Properties.Aggregate agg = this.AggregateCreate(name, true);

			UndoableList list = this.document.Aggregates;
			if ( putToList && list.IndexOf(agg) == -1 )
			{
				using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateNew3) )
				{
					rank = System.Math.Max(rank, 0);
					rank = System.Math.Min(rank, list.Count-1);
					list.Insert(rank+1, agg);
					list.Selected = rank+1;

					this.document.Notifier.NotifyStyleChanged();
					this.OpletQueueValidateAction();
				}
			}

			this.AggregateUse(agg);
		}

		public void AggregateNewAll(int rank, string name, bool putToList)
		{
			//	Cr�e un nouvel agr�gat avec toutes les propri�t�s.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			Objects.Abstract model = null;
			if ( this.IsTool )  // objets s�lectionn�s ?
			{
				model = this.RetFirstSelectedObject();
			}
			else	// objectMemory ?
			{
				this.OpletQueueEnable = false;
				model = Objects.Abstract.CreateObject(this.document, this.tool, this.ObjectMemoryTool);
				this.OpletQueueEnable = true;
			}
			if ( model == null )  return;

			Properties.Aggregate agg = this.AggregateCreate(name, model);

			UndoableList list = this.document.Aggregates;
			if ( putToList && list.IndexOf(agg) == -1 )
			{
				using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateNew3) )
				{
					rank = System.Math.Max(rank, 0);
					rank = System.Math.Min(rank, list.Count-1);
					list.Insert(rank+1, agg);
					list.Selected = rank+1;

					this.document.Notifier.NotifyStyleChanged();
					this.OpletQueueValidateAction();
				}
			}

			if ( !this.IsTool )  // objectMemory ?
			{
				this.OpletQueueEnable = false;
				model.Dispose();
				this.OpletQueueEnable = true;
			}

			this.AggregateUse(agg);
		}

		protected Properties.Aggregate AggregateCreate(string name, bool three)
		{
			//	Cr�e un nouvel agr�gat avec seulement 3 propri�t�s, pas encore r�f�renc�.
			this.OpletQueueEnable = false;

			Properties.Aggregate agg = new Properties.Aggregate(this.document);

			if ( name == "" )
			{
				name = this.GetNextTextStyleName(StyleCategory.Graphic);  // nom unique
			}
			agg.AggregateName = name;

			Objects.Abstract model = this.ObjectMemoryTool;
			if ( this.TotalSelected > 0 )
			{
				model = this.RetFirstSelectedObject();
			}

			if ( three )
			{
				this.AggregateCreateProperty(agg, model, Properties.Type.LineMode);
				this.AggregateCreateProperty(agg, model, Properties.Type.LineColor);
				this.AggregateCreateProperty(agg, model, Properties.Type.FillGradient);
			}
			agg.Styles.Selected = -1;

			this.OpletQueueEnable = true;
			return agg;
		}

		protected Properties.Aggregate AggregateCreate(string name, Objects.Abstract model)
		{
			//	Cr�e un nouvel agr�gat avec toutes les propri�t�s d'un objet mod�le,
			//	pas encore r�f�renc�.
			this.OpletQueueEnable = false;

			Properties.Aggregate agg = new Properties.Aggregate(this.document);

			if ( name == "" )
			{
				name = this.GetNextTextStyleName(StyleCategory.Graphic);  // nom unique
			}
			agg.AggregateName = name;

			for ( int i=0 ; i<100 ; i++ )
			{
				Properties.Type type = Properties.Abstract.SortOrder(i);
				if ( !Properties.Abstract.StyleAbility(type) )  continue;
				if ( model.Property(type) == null )  continue;

				this.AggregateCreateProperty(agg, model, type);
			}
			agg.Styles.Selected = -1;

			this.OpletQueueEnable = true;
			return agg;
		}

		protected Properties.Abstract AggregateCreateProperty(Properties.Aggregate agg, Objects.Abstract model, Properties.Type type)
		{
			//	Cr�e une nouvelle propri�t� pour un agr�gat. Dans Aggregate.Styles,
			//	les propri�t�s sont toujours selon l'ordre Properties.Abstract.SortOrder !
			Properties.Abstract style = Properties.Abstract.NewProperty(this.document, type);

			if ( model != null )
			{
				Properties.Abstract original = model.Property(type);
				if ( original != null )
				{
					original.CopyTo(style);
				}
			}

			style.IsStyle = true;
			style.IsOnlyForCreation = false;

			int order = Properties.Abstract.SortOrder(type);
			int rank = 0;
			while ( rank < agg.Styles.Count )
			{
				Properties.Abstract property = agg.Styles[rank] as Properties.Abstract;
				if ( order < Properties.Abstract.SortOrder(property.Type) )  break;
				rank ++;
			}
			agg.Styles.Insert(rank, style);
			agg.Styles.Selected = rank;

			return style;
		}

		public void AggregateUse(Properties.Aggregate agg)
		{
			//	Utilise un agr�gat.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			if ( this.IsTool )  // objets s�lectionn�s ?
			{
				if ( this.TotalSelected == 0 )  return;

				string text = string.Format(Res.Strings.Action.AggregateUse, agg.AggregateName);
				using ( this.OpletQueueBeginAction(text) )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					Objects.Abstract layer = context.RootObject();
					foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
					{
						obj.AggregateFree();
						obj.Aggregates.Clear();
						obj.Aggregates.Add(agg);
						obj.AggregateUse();
					}

					this.AggregateToDocument(agg);

					this.document.Notifier.NotifyStyleChanged();
					this.document.Notifier.NotifySelectionChanged();

					this.OpletQueueValidateAction();
				}
			}
			else	// objectMemory ?
			{
				this.OpletQueueEnable = false;
				this.ObjectMemoryTool.AggregateFree();
				this.ObjectMemoryTool.Aggregates.Clear();
				this.ObjectMemoryTool.Aggregates.Add(agg);
				this.ObjectMemoryTool.AggregateUse();
				this.OpletQueueEnable = true;

				this.document.Notifier.NotifySelectionChanged();
			}

			this.aggregateUsed = this.document.Aggregates.IndexOf(agg);
		}

		public void AggregateToDocument(Properties.Aggregate agg)
		{
			//	Ajoute l'agr�gat dans la liste du document, si n�cessaire.
			UndoableList list = this.document.Aggregates;
			int index = list.IndexOf(agg);
			if ( index == -1 )
			{
				list.Add(agg);
				list.Selected = list.Count-1;

				this.aggregateUsed = list.Selected;
			}
			else
			{
				this.aggregateUsed = index;
			}
		}

		public void AggregateFree()
		{
			//	Lib�re les objets s�lectionn�s des agr�gats.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			if ( this.IsTool )  // objets s�lectionn�s ?
			{
				if ( this.TotalSelected == 0 )  return;

				using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateFree) )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					Objects.Abstract layer = context.RootObject();
					foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
					{
						obj.AggregateFree();
						obj.Aggregates.Clear();
					}

					this.document.Notifier.NotifyStyleChanged();
					this.document.Notifier.NotifySelectionChanged();

					this.OpletQueueValidateAction();
				}
			}
			else	// objectMemory ?
			{
				this.OpletQueueEnable = false;
				this.ObjectMemoryTool.AggregateFree();
				this.ObjectMemoryTool.Aggregates.Clear();
				this.OpletQueueEnable = true;

				this.document.Notifier.NotifyStyleChanged();
				this.document.Notifier.NotifySelectionChanged();
			}
		}

		protected void AggregateFreeAll()
		{
			//	Lib�re tous les objets coup�s ou copi�s des agr�gats.
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, false) )
			{
				obj.AggregateFree();
				obj.Aggregates.Clear();
			}
		}

		public void AggregateDuplicate(int rank)
		{
			//	Duplique un agr�gat.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateDuplicate) )
			{
				UndoableList list = this.document.Aggregates;
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);
				Properties.Aggregate srcAgg = list[rank] as Properties.Aggregate;

				Properties.Aggregate newAgg = new Properties.Aggregate(this.document);
				srcAgg.DuplicateTo(newAgg);
				newAgg.AggregateName = Misc.CopyName(newAgg.AggregateName);
				list.Insert(rank+1, newAgg);
				list.Selected = rank+1;

				this.document.Notifier.NotifyStyleChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void AggregateDelete(int rank)
		{
			//	Supprime un agr�gat.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateDelete) )
			{
				UndoableList list = this.document.Aggregates;
				rank = System.Math.Max(rank, 0);
				rank = System.Math.Min(rank, list.Count-1);
				Properties.Aggregate agg = list[rank] as Properties.Aggregate;

				for ( int i=0 ; i<this.document.Aggregates.Count ; i++ )
				{
					Properties.Aggregate a = this.document.Aggregates[i] as Properties.Aggregate;
					if ( a.Childrens.Contains(agg) )
					{
						a.Childrens.Remove(agg);
					}
				}

				list.RemoveAt(rank);

				rank = System.Math.Min(rank, list.Count-1);
				list.Selected = rank;

				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract doc = context.RootObject(0);
				foreach ( Objects.Abstract obj in this.document.Deep(doc) )
				{
					obj.AggregateDelete(agg);
				}

				this.objectMemory.AggregateDelete(agg);
				this.objectMemoryText.AggregateDelete(agg);

				this.document.Notifier.NotifyStyleChanged();
				this.document.Notifier.NotifySelectionChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void AggregateSwap(int rank1, int rank2)
		{
			//	Permute deux agr�gats.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateSwap) )
			{
				UndoableList list = this.document.Aggregates;
				rank1 = System.Math.Max(rank1, 0);
				rank1 = System.Math.Min(rank1, list.Count-1);
				rank2 = System.Math.Max(rank2, 0);
				rank2 = System.Math.Min(rank2, list.Count-1);

				Properties.Aggregate temp = list[rank1] as Properties.Aggregate;
				list.RemoveAt(rank1);
				list.Insert(rank2, temp);
				list.Selected = rank2;

				this.document.Notifier.NotifyStyleChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void AggregateChangeName(string name)
		{
			//	Change le nom d'un agr�gat.
			if ( this.ActiveViewer.IsCreating )  return;
			if ( name == "..." )  return;
			this.document.IsDirtySerialize = true;

			if ( this.IsTool )  // objets s�lectionn�s ?
			{
				int sel = this.aggregateUsed;
				if ( sel == -1 )  return;

				Properties.Aggregate agg = this.document.Aggregates[sel] as Properties.Aggregate;

				this.OpletQueueBeginAction(Res.Strings.Action.AggregateChange, "ChangeAggregateName", sel);
				agg.AggregateName = name;
				this.OpletQueueValidateAction();

				this.document.Notifier.NotifyAggregateChanged(agg);
			}
			else	// objectMemory ?
			{
				if ( this.ObjectMemoryTool.Aggregates.Count == 0 )  return;

				this.OpletQueueEnable = false;
				Properties.Aggregate agg = this.ObjectMemoryTool.Aggregates[0] as Properties.Aggregate;
				agg.AggregateName = name;
				this.OpletQueueEnable = true;
			}
		}

		public void AggregateStyleNew(Properties.Aggregate agg, Properties.Type type)
		{
			//	Cr�e un nouveau style dans un agr�gat.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateStyleNew) )
			{
				Objects.Abstract model = this.ObjectMemoryTool;
				if ( this.TotalSelected > 0 )
				{
					model = this.RetFirstSelectedObject();
				}
				this.AggregateCreateProperty(agg, model, type);

				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract doc = context.RootObject(0);
				foreach ( Objects.Abstract obj in this.document.Deep(doc) )
				{
					obj.AggregateAdapt(agg);
				}

				this.objectMemory.AggregateAdapt(agg);
				this.objectMemoryText.AggregateAdapt(agg);

				this.document.Notifier.NotifyStyleChanged();
				this.document.Notifier.NotifySelectionChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void AggregateStyleDelete(Properties.Aggregate agg, Properties.Type type)
		{
			//	Supprime le style s�lectionn� d'un agr�gat.
			if ( this.ActiveViewer.IsCreating )  return;
			if ( type == Properties.Type.None )  return;

			Properties.Abstract property = agg.Property(type);
			if ( property == null )  return;
			int rank = agg.Styles.IndexOf(property);
			if ( rank == -1 )  return;
			
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateStyleDelete) )
			{
				agg.Styles.RemoveAt(rank);

				rank = System.Math.Min(rank, agg.Styles.Count-1);
				agg.Styles.Selected = rank;

				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract doc = context.RootObject(0);
				foreach ( Objects.Abstract obj in this.document.Deep(doc) )
				{
					obj.AggregateAdapt(agg);
				}

				this.objectMemory.AggregateAdapt(agg);
				this.objectMemoryText.AggregateAdapt(agg);

				this.document.Notifier.NotifyStyleChanged();
				this.document.Notifier.NotifySelectionChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void AggregateChildrensNew(Properties.Aggregate agg, Properties.Aggregate newAgg)
		{
			//	Ajoute un enfant � un agr�gat.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateChildrensNew) )
			{
				UndoableList aggregates = this.document.Aggregates;
				int ins = aggregates.IndexOf(newAgg);
				int rank = 0;
				while ( rank < agg.Childrens.Count )
				{
					Properties.Aggregate children = agg.Childrens[rank] as Properties.Aggregate;
					if ( ins < aggregates.IndexOf(children) )  break;
					rank ++;
				}
				
				agg.Childrens.Insert(rank, newAgg);
				agg.Childrens.Selected = rank;

				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract doc = context.RootObject(0);
				foreach ( Objects.Abstract obj in this.document.Deep(doc) )
				{
					obj.AggregateAdapt(agg);
				}

				this.objectMemory.AggregateAdapt(agg);
				this.objectMemoryText.AggregateAdapt(agg);

				this.document.Notifier.NotifyStyleChanged();
				this.document.Notifier.NotifySelectionChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void AggregateChildrensDelete(Properties.Aggregate agg, Properties.Aggregate delAgg)
		{
			//	Supprime un enfant � un agr�gat.
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateChildrensDelete) )
			{
				agg.Childrens.Remove(delAgg);

				int sel = System.Math.Min(agg.Childrens.Selected, agg.Childrens.Count-1);
				agg.Childrens.Selected = sel;

				DrawingContext context = this.ActiveViewer.DrawingContext;
				Objects.Abstract doc = context.RootObject(0);
				foreach ( Objects.Abstract obj in this.document.Deep(doc) )
				{
					obj.AggregateAdapt(agg);
				}

				this.objectMemory.AggregateAdapt(agg);
				this.objectMemoryText.AggregateAdapt(agg);

				this.document.Notifier.NotifyStyleChanged();
				this.document.Notifier.NotifySelectionChanged();
				this.OpletQueueValidateAction();
			}
		}

		public void AggregatePicker(Objects.Abstract model)
		{
			//	Reprend les propri�t�s d'un objet mod�le.
			if ( this.TotalSelected == 0 )
			{
				this.OpletQueueEnable = false;
				this.ObjectMemory.PickerProperties(model);
				this.ObjectMemoryText.PickerProperties(model);
				this.OpletQueueEnable = true;

				this.document.Notifier.NotifyStyleChanged();
				this.document.Notifier.NotifySelectionChanged();
			}
			else
			{
				using ( this.OpletQueueBeginAction(Res.Strings.Action.Picker) )
				{
					DrawingContext context = this.ActiveViewer.DrawingContext;
					Objects.Abstract layer = context.RootObject();
					foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
					{
						if ( obj == model )  continue;
						this.document.Notifier.NotifyArea(obj.BoundingBox);
						obj.PickerProperties(model);
						this.document.Notifier.NotifyArea(obj.BoundingBox);
					}
					this.document.Notifier.NotifySelectionChanged();
					this.OpletQueueValidateAction();
				}
			}
		}

		public string GetNextTextStyleName(StyleCategory category)
		{
			//	Donne le prochain nom unique d'un style.
			switch ( category )
			{
				case StyleCategory.Graphic:
					return string.Format(Res.Strings.Style.Aggregate.Name, this.document.GetNextUniqueAggregateId());
				
				case StyleCategory.Paragraph:
					return string.Format(Res.Strings.Style.Paragraph.Name, this.document.GetNextUniqueParagraphStyleId());
				
				case StyleCategory.Character:
					return string.Format(Res.Strings.Style.Character.Name, this.document.GetNextUniqueCharacterStyleId());
			}
			
			throw new System.ArgumentException("GetNextTextStyleName(" + category.ToString() + ")");
		}
		#endregion


		#region OpletTool
		protected void InsertOpletTool()
		{
			//	Ajoute un oplet pour m�moriser l'outil.
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletTool oplet = new OpletTool(this.document);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	M�morise l'outil.
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

				this.host.Modifier.ToolAdaptRibbon();
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


		#region OpletSize
		public void InsertOpletSize()
		{
			//	Ajoute un oplet pour m�moriser les dimensions du document.
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletSize oplet = new OpletSize(this.document);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	M�morise les dimensions.
		protected class OpletSize : AbstractOplet
		{
			public OpletSize(Document host)
			{
				this.host = host;
				this.documentSize = this.host.DocumentSize;
				this.outsideArea = this.host.Modifier.outsideArea;
			}

			protected void Swap()
			{
				Size temp = this.documentSize;
				this.documentSize = this.host.DocumentSize;
				this.host.InternalSize = temp;

				Misc.Swap(ref this.outsideArea, ref this.host.Modifier.outsideArea);

				this.host.Modifier.ActiveViewer.DrawingContext.ZoomPageAndCenter();
				this.host.Notifier.NotifyAllChanged();
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
			protected Size					documentSize;
			protected double				outsideArea;
		}
		#endregion


		#region OpletDirtyCounters
		public void InsertOpletDirtyCounters()
		{
			//	Ajoute un oplet pour indiquer que les compteurs devront �tre mis � jour.
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletDirtyCounters oplet = new OpletDirtyCounters(this.document);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		protected class OpletDirtyCounters : AbstractOplet
		{
			public OpletDirtyCounters(Document host)
			{
				this.host = host;
			}

			protected void Swap()
			{
				this.host.Modifier.DirtyCounters();
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
		}
		#endregion


		#region OpletQueue
		public void OpletQueuePurge()
		{
			//	Vide toutes les queues.
			this.opletQueue.PurgeUndo();
			this.opletQueue.PurgeRedo();
			this.opletLastCmd = "";
		}

		public bool OpletQueueEnable
		{
			//	D�termine si les actions seront annulables ou non.
			get
			{
				return this.opletQueue.IsEnabled && !this.opletSkip;
			}

			set
			{
				if ( value != this.opletQueue.IsEnabled )
				{
					if ( value )  this.opletQueue.Enable();
					else          this.opletQueue.Disable();
				}
			}
		}

		public System.IDisposable OpletQueueBeginAction(string name)
		{
			//	D�but d'une zone annulable.
			return this.OpletQueueBeginAction(name, "", 0);
		}

		public System.IDisposable OpletQueueBeginAction(string name, string cmd)
		{
			return this.OpletQueueBeginAction(name, cmd, 0);
		}

		public System.IDisposable OpletQueueBeginAction(string name, string cmd, int id)
		{
			this.opletLevel ++;
			if ( this.opletLevel > 1 )  return null;

			if ( cmd == this.opletLastCmd && id == this.opletLastId )
			{
				if ( cmd == "ChangeProperty"      ||
					 cmd == "ChangePageName"      ||
					 cmd == "ChangeLayerName"     ||
					 cmd == "ChangeAggregateName" ||
					 cmd == "ChangeGuide"         ||
					 cmd == "ChangeDocSize"       ||
					 cmd == "SpecialPageSize"     ||
					 cmd == "SpecialPageLanguage" ||
					 cmd == "SpecialPageStyle"    )
				{
					this.opletSkip = true;
					return null;
				}
			}

			this.opletCreate = (cmd == "Create");
			this.opletLastCmd = cmd;
			this.opletLastId = id;

			return this.opletQueue.BeginAction(name);
		}

		public System.IDisposable OpletQueueBeginActionNoMerge(string name)
		{
			//	D�but d'une zone annulable.
			return this.OpletQueueBeginActionNoMerge(name, "", 0);
		}

		public System.IDisposable OpletQueueBeginActionNoMerge(string name, string cmd)
		{
			return this.OpletQueueBeginActionNoMerge(name, cmd, 0);
		}

		public System.IDisposable OpletQueueBeginActionNoMerge(string name, string cmd, int id)
		{
			this.opletLevel ++;
			if ( this.opletLevel > 1 )  return null;

			if ( cmd == this.opletLastCmd && id == this.opletLastId )
			{
				if ( cmd == "ChangeProperty"      ||
					 cmd == "ChangePageName"      ||
					 cmd == "ChangeLayerName"     ||
					 cmd == "ChangeAggregateName" ||
					 cmd == "ChangeGuide"         ||
					 cmd == "ChangeDocSize"       )
				{
					this.opletSkip = true;
					return null;
				}
			}

			this.opletCreate = (cmd == "Create");
			this.opletLastCmd = cmd;
			this.opletLastId = id;

			return this.opletQueue.BeginAction(name, Common.Support.OpletQueue.MergeMode.Disabled);
		}

		public void OpletQueueNameAction(string name)
		{
			//	Nomme une action annulable.
			this.opletQueue.DefineActionName(name);
		}

		public void OpletQueueChangeLastNameAction(string name)
		{
			//	Nomme une action annulable.
			this.opletQueue.ChangeLastActionName(name);
		}

		public void OpletQueueValidateAction()
		{
			//	Fin d'une zone annulable.
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

		public void OpletQueueCancelAction()
		{
			//	Fin d'une zone annulable.
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
		protected bool							namesExist;
		protected ZoomHistory					zoomHistory;
		protected OpletQueue					opletQueue;
		protected int							opletLevel;
		protected bool							opletSkip;
		protected bool							opletCreate;
		protected string						opletLastCmd;
		protected int							opletLastId;
		protected bool							isUndoRedoInProgress;
		protected Objects.Memory				objectMemory;
		protected Objects.Memory				objectMemoryText;
		protected Properties.Abstract			menuProperty;
		protected SelectorType					operInitialSelector;
		protected SelectorType					bookInitialSelector = SelectorType.None;
		protected Containers.Abstract			activeContainer;
		protected bool[]						isPropertiesExtended;
		protected System.Collections.Hashtable	tableTextPanelExtended;
		protected RealUnitType					realUnitDimension;
		protected double						realScale;
		protected double						realResolution;
		protected string						realShortNameUnitDimension;
		protected string						realLongNameUnitDimension;
		protected Point							duplicateMove;
		protected Point							arrowMove;
		protected double						arrowMoveMul;
		protected double						arrowMoveDiv;
		protected bool							repeatDuplicateMove;
		protected bool							lastOperIsDuplicate;
		protected Point							moveAfterDuplicate;
		protected Point							moveDistance;
		protected double						rotateAngle;
		protected double						scaleFactor;
		protected double						colorAdjust;
		protected double						toLinePrecision;
		protected string						textInfoModif = "";
		protected double						outsideArea;
		protected double						dimensionScale;
		protected double						dimensionDecimal;
		protected int							aggregateUsed;
		protected System.Collections.Hashtable	accumulateObjects;
		protected int							ribbonTextStyleSelected;
		protected int							ribbonTextStyleFirst;

		public static readonly double			FontSizeScale = 254.0 / 72.0;	//	1pt = 1/72 de pouce (unit�s internes, 0.1mm)
	}
}
