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

			this.rotateAngle = 10.0;  // 10 degrés
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

		// Outil sélectionné dans la palette.
		public string Tool
		{
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

					bool isCreate = this.opletCreate;
					string name = string.Format(Res.Strings.Action.ChangeTool, this.ToolName(value));
					this.OpletQueueBeginAction(name);
					this.InsertOpletTool();

					Objects.Abstract editObject = this.RetEditObject();

					if ( this.tool == "HotSpot" || value == "HotSpot" )
					{
						this.document.Notifier.NotifyArea(this.ActiveViewer);
					}

					string initialTool = this.tool;
					this.tool = value;

					this.ToolAdaptRibbon();

					if ( this.tool == "Select" && isCreate )  // on vient de créer un objet ?
					{
						DrawingContext context = this.ActiveViewer.DrawingContext;
						Objects.Abstract layer = context.RootObject();
						Objects.Abstract obj = layer.Objects[layer.Objects.Count-1] as Objects.Abstract;
						this.ActiveViewer.Select(obj, false, false);
					}

					if ( this.tool == "Shaper" && isCreate )  // on vient de créer un objet ?
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

					else if ( this.tool == "Shaper" )
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

					else if ( this.IsTool && this.tool != "Edit" )
					{
						this.SelectedSegmentClear();

						if ( editObject != null )
						{
							editObject.Select(true);
						}
						else if ( initialTool == "Shaper" )
						{
							this.ActiveViewer.UpdateSelector();
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

					else if ( !this.IsTool )  // choix d'un objet à créer ?
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

		// Adapte les rubans à l'outil sélectionné.
		protected void ToolAdaptRibbon()
		{
			if ( this.tool == "Edit" )
			{
				this.document.Notifier.NotifyRibbonCommand("Text");
			}
			else
			{
				this.document.Notifier.NotifyRibbonCommand("!Text");
			}
		}

		// Indique si l'outil sélectionné n'est pas un objet.
		public bool IsTool
		{
			get
			{
				if ( this.tool == "Select"  )  return true;
				if ( this.tool == "Global"  )  return true;
				if ( this.tool == "Shaper"  )  return true;
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
				if ( this.tool == "ObjectTextBox2"  )  return true;
				return false;
			}
		}

		// Indique si l'outil est le modeleur.
		public bool IsToolShaper
		{
			get
			{
				return this.tool == "Shaper";
			}
		}

		// Retourne le nom d'un outil.
		public string ToolName(string tool)
		{
			switch ( tool )
			{
				case "Select":           return Res.Strings.Tool.Select;
				case "Global":           return Res.Strings.Tool.Global;
				case "Shaper":           return Res.Strings.Tool.Shaper;
				case "Edit":             return Res.Strings.Tool.Edit;
				case "Zoom":             return Res.Strings.Tool.Zoom;
				case "Hand":             return Res.Strings.Tool.Hand;
				case "Picker":           return Res.Strings.Tool.Picker;
				case "HotSpot":          return Res.Strings.Tool.HotSpot;

				case "ObjectLine":       return Res.Strings.Tool.Line;
				case "ObjectRectangle":  return Res.Strings.Tool.Rectangle;
				case "ObjectCircle":     return Res.Strings.Tool.Circle;
				case "ObjectEllipse":    return Res.Strings.Tool.Ellipse;
				case "ObjectPoly":       return Res.Strings.Tool.Poly;
				case "ObjectBezier":     return Res.Strings.Tool.Bezier;
				case "ObjectRegular":    return Res.Strings.Tool.Regular;
				case "ObjectSurface":    return Res.Strings.Tool.Surface;
				case "ObjectVolume":     return Res.Strings.Tool.Volume;
				case "ObjectTextLine":   return Res.Strings.Tool.TextLine;
				case "ObjectTextBox":    return Res.Strings.Tool.TextBox;
				case "ObjectTextBox2":   return Res.Strings.Tool.TextBox;
				case "ObjectArray":      return Res.Strings.Tool.Array;
				case "ObjectImage":      return Res.Strings.Tool.Image;
				case "ObjectDimension":  return Res.Strings.Tool.Dimension;
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


		// Taille de la zone de travail.
		public Size SizeArea
		{
			get
			{
				return new Size(this.document.Size.Width+this.outsideArea*2.0, this.document.Size.Height+this.outsideArea*2.0);
			}
		}

		// Origine de la zone de travail.
		public Point OriginArea
		{
			get
			{
				return new Point(-this.outsideArea, -this.outsideArea);
			}
		}

		// Rectangle de la zone de travail.
		public Rectangle RectangleArea
		{
			get
			{
				return new Rectangle(this.OriginArea, this.SizeArea);
			}
		}

		// Rectangle de la page de travail.
		public Rectangle PageArea
		{
			get
			{
				return new Rectangle(new Point(0,0), this.document.Size);
			}
		}

		// Marges autour de la page physique.
		public double OutsideArea
		{
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

		// Echelle pour les cotes.
		public double DimensionScale
		{
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

		// Nombre de décimales des cotes.
		public double DimensionDecimal
		{
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


		// Texte des informations de modification.
		public string TextInfoModif
		{
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

		// Appelé lorsque l'onglet a changé.
		public void TabBookChanged(string name)
		{
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
		// Conversion d'une distance en chaîne.
		public string RealToString(double value)
		{
			if ( this.document.Type == DocumentType.Pictogram )
			{
				return value.ToString("F1");
			}
			else
			{
				value /= this.realScale;
				value /= this.realPrecision*10.0;  // *10 -> un digit de moins
				value = System.Math.Floor(value+0.5);
				value *= this.realPrecision*10.0;
				return value.ToString();
			}
		}

		// Conversion d'un angle en chaîne.
		public string AngleToString(double value)
		{
			return string.Format("{0}\u00B0", value.ToString("F1"));
		}

		// Choix de l'unité de dimension par défaut.
		public RealUnitType RealUnitDimension
		{
			get
			{
				return this.realUnitDimension;
			}
			
			set
			{
				this.SetRealUnitDimension(value, true);
			}
		}

		// Choix de l'unité de dimension par défaut.
		public void SetRealUnitDimension(RealUnitType unit, bool adapt)
		{
			this.realUnitDimension = unit;

			switch ( this.realUnitDimension )
			{
				case RealUnitType.DimensionMillimeter:
					this.realScale = 10.0;
					this.realPrecision = 0.01;
					break;

				case RealUnitType.DimensionCentimeter:
					this.realScale = 100.0;
					this.realPrecision = 0.001;
					break;

				case RealUnitType.DimensionInch:
					this.realScale = 254.0;
					this.realPrecision = 0.0001;
					break;

				default:
					this.realScale = 1.0;
					this.realPrecision = 1.0;
					break;
			}

			if ( adapt )
			{
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
			field.TextSuffix = "%";
		}

		// Adapte un TextFieldReal pour éditer une dimension.
		public void AdaptTextFieldRealDimension(TextFieldReal field)
		{
			field.UnitType = this.realUnitDimension;
			field.Scale = (decimal) this.realScale;

			if ( this.document.Type == DocumentType.Pictogram )
			{
				field.InternalMinValue = 200.0M * field.FactorMinRange;
				field.InternalMaxValue = 200.0M * field.FactorMaxRange;
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

				field.Resolution = (decimal) this.realPrecision;
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
				field.UnitType = RealUnitType.Scalar;
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
			field.TextSuffix = "\u00B0";  // symbole unicode "degré" (#176)
		}

		// Modifie tous les widgets de l'application reflétant des dimensions
		// pour utiliser une autre unité.
		public void AdaptAllTextFieldReal()
		{
			foreach ( Window window in Window.DebugAliveWindows )
			{
				if ( window.Root == null )  continue;
				this.AdaptAllTextFieldReal(window.Root);
			}
		}

		// Modifie tous les widgets d'un panneau qui sera utilisé.
		protected void AdaptAllTextFieldReal(Widget parent)
		{
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
			this.ActiveViewer.CreateEnding(false, false);
			this.OpletQueueEnable = false;

			this.TotalSelected = 0;
			this.totalHide = 0;
			this.totalPageHide = 0;
			this.document.GetObjects.Clear();

			this.document.PropertiesAuto.Clear();
			this.document.PropertiesSel.Clear();
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
			this.objectMemoryText.PropertyFillGradient.Color1 = RichColor.FromARGB(0, 1,1,1);
		}


		#region Counters
		// Indique qu'il faudra mettre à jour tous les compteurs.
		public void DirtyCounters()
		{
			this.dirtyCounters = true;
		}

		// Met à jour tous les compteurs.
		public void UpdateCounters()
		{
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

		// Est-ce que le calque courant contient des noms d'objets ?
		public bool NamesExist
		{
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
		// Retourne un texte multi-lignes de statistiques sur le document.
		public string Statistic(bool fonts, bool images)
		{
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
			
			info = string.Format(Res.Strings.Statistic.Size, chip, this.RealToString(this.document.Size.Width), this.RealToString(this.document.Size.Height));
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
				System.Collections.ArrayList list = this.StatisticFonts();
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
					bbox.MergeWith(obj.BoundingBoxPartial);
				}
				return bbox;
			}
		}

		// Retourne la bbox mince des objets sélectionnés.
		public Rectangle SelectedBboxThin
		{
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

		// Retourne le seul objet sélectionné.
		public Objects.Abstract RetOnlySelectedObject()
		{
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

		// Retourne le premier objet sélectionné.
		public Objects.Abstract RetFirstSelectedObject()
		{
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

		// Insère un texte dans le pavé en édition.
		public bool EditInsertText(string text, string fontFace, string fontStyle)
		{
			Objects.Abstract editObject = this.RetEditObject();
			if ( editObject == null )  return false;

			return editObject.EditInsertText(text, fontFace, fontStyle);
		}

		// Insère un glyphe dans le pavé en édition.
		public bool EditInsertGlyph(int code, int glyph, string fontFace, string fontStyle)
		{
			Objects.Abstract editObject = this.RetEditObject();
			if ( editObject == null )  return false;

			return editObject.EditInsertGlyph(code, glyph, fontFace, fontStyle);
		}

		// Donne la fonte actullement utilisée.
		public void EditGetFont(out string fontFace, out string fontStyle)
		{
			if ( this.document.IsWrappersAttached )
			{
				fontFace = this.document.TextWrapper.Defined.FontFace;
				if ( fontFace == null )
				{
					fontFace = this.document.TextWrapper.Active.FontFace;
					if ( fontFace == null )
					{
						fontFace = "";
					}
				}

				fontStyle = this.document.TextWrapper.Defined.FontStyle;
				if ( fontStyle == null )
				{
					fontStyle = this.document.TextWrapper.Active.FontStyle;
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

		// Supprime tous les segments sélectionnés des objets sélectionnés.
		protected void SelectedSegmentClear()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.SelectedSegmentClear();
			}
		}

		// Désélectionne tous les objets.
		public void DeselectAllCmd()
		{
			if ( this.ActiveViewer.EditFlowTerminate() )  return;

			this.DeselectAll();
		}

		// Désélectionne tous les objets.
		public void DeselectAll()
		{

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

		// Sélectionne tous les objets.
		public void SelectAll()
		{
			using ( this.OpletQueueBeginAction(Res.Strings.Action.SelectAll) )
			{
				this.ActiveViewer.CreateEnding(false, false);

				this.opletCreate = false;
				this.Tool = "Select";

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

		// Inverse la sélection.
		public void InvertSelection()
		{
			using ( this.OpletQueueBeginAction(Res.Strings.Action.SelectInvert) )
			{
				this.ActiveViewer.CreateEnding(false, false);

				this.opletCreate = false;
				this.Tool = "Select";

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

		// Retourne la liste de tous les objets ayant un nom dans la page
		// et la calque courant.
		public System.Collections.ArrayList SelectNames()
		{
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

		// Sélectionne d'après un nom d'objet.
		public void SelectName(string name)
		{
			if ( name == "" )  return;
			
			System.Text.RegularExpressions.Regex regex = Support.RegexFactory.FromSimpleJoker(name);

			string nm = string.Format(Res.Strings.Action.SelectName, name);
			using ( this.OpletQueueBeginAction(nm) )
			{
				this.ActiveViewer.CreateEnding(false, false);

				this.opletCreate = false;
				this.Tool = "Select";

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
		// Supprime tous les objets sélectionnés.
		public void DeleteSelection()
		{
			this.DeleteSelection(false);
		}

		// Supprime tous les objets sélectionnés.
		// Si onlyMark=true, on ne détruit que les objets marqués.
		public void DeleteSelection(bool onlyMark)
		{
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

		// Détruit les objets fils éventuels.
		protected void DeleteGroup(Objects.Abstract group)
		{
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

		// Supprime toutes les marques des objets sélectionnés.
		protected void ClearMarks()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.Mark = false;
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
					this.FlushMoveAfterDuplicate();
					this.document.Notifier.NotifySettingsChanged();
					this.document.IsDirtySerialize = true;
				}
			}
		}

		// Choix du déplacement des objets avec les touches flèches.
		public Point ArrowMove
		{
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

		// Choix du multiplicateur avec Shift.
		public double ArrowMoveMul
		{
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

		// Choix du diviseur avec Ctrl.
		public double ArrowMoveDiv
		{
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

		// Annule le déplacement après un duplique.
		public void FlushMoveAfterDuplicate()
		{
			this.lastOperIsDuplicate = false;
			this.moveAfterDuplicate = new Point(0,0);
		}

		// Ajoute un déplacement effectué après un duplique.
		public void AddMoveAfterDuplicate(Point move)
		{
			if ( this.lastOperIsDuplicate )
			{
				this.moveAfterDuplicate += move;
			}
		}

		// Réglage "répète le dernier déplacement".
		public bool RepeatDuplicateMove
		{
			get
			{
				return this.repeatDuplicateMove;
			}

			set
			{
				this.repeatDuplicateMove = value;
			}
		}

		// Donne le déplacement effectif à utiliser pour la commande "dupliquer".
		public Point EffectiveDuplicateMove
		{
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

		// Duplique tous les objets sélectionnés.
		public void DuplicateSelection(Point move)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Duplicate) )
			{
				this.Tool = "Select";
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

		// Coupe tous les objets sélectionnés dans le bloc-notes.
		public void CutSelection()
		{
			Objects.Abstract editObject = this.RetEditObject();
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

		// Copie tous les objets sélectionnés dans le bloc-notes.
		public void CopySelection()
		{
			Objects.Abstract editObject = this.RetEditObject();
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

		// Colle le contenu du bloc-notes.
		public void Paste()
		{
			Objects.Abstract editObject = this.RetEditObject();
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

		// Indique si le bloc-notes est vide.
		public bool IsClipboardEmpty()
		{
			if ( this.document.Clipboard == null )  return true;
			return ( this.document.Clipboard.Modifier.TotalObjects == 0 );
		}
		#endregion


		#region TextFormat
		// Modifie le flux.
		public void TextFlowChange(Objects.TextBox2 obj, Objects.TextBox2 parent, bool after)
		{
			using ( this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.Text.FlowChanged) )
			{
				if ( parent == null )
				{
					obj.TextFlow.Remove(obj);
				}
				else
				{
					parent.TextFlow.Add(obj, parent, after);
					this.EditObject(obj);
				}

				this.document.Modifier.OpletQueueValidateAction();
			}
		}

		// Edite n'importe quel objet, en changeant de page si nécessaire.
		public void EditObject(Objects.Abstract edit)
		{
			this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.Edit);

			int page = edit.PageNumber;
			if ( this.ActiveViewer.DrawingContext.CurrentPage != page )
			{
				this.ActiveViewer.DrawingContext.CurrentPage = page;
			}
			
			this.document.Modifier.ActiveViewer.Select(edit, true, false);
			
			this.document.Modifier.OpletQueueValidateAction();
		}
		#endregion


		#region UndoRedo
		// Annule les dernières actions.
		public void Undo(int number)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;
			this.ActiveViewer.CreateEnding(false, false);

			for ( int i=0 ; i<number ; i++ )
			{
				this.opletQueue.UndoAction();
			}

			this.opletLastCmd = "";
			this.opletLastId = 0;
			this.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
			this.document.Notifier.NotifySelectionChanged();
			this.document.Notifier.NotifyTextChanged();
			this.document.Notifier.NotifyStyleChanged();
			this.document.Notifier.NotifyTextStyleChanged();
			this.document.Notifier.NotifyUndoRedoChanged();
			this.document.Notifier.NotifyArea();
		}

		// Refait les dernières actions.
		public void Redo(int number)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;
			this.ActiveViewer.CreateEnding(false, false);

			for ( int i=0 ; i<number ; i++ )
			{
				this.opletQueue.RedoAction();
			}

			this.opletLastCmd = "";
			this.opletLastId = 0;
			this.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
			this.document.Notifier.NotifySelectionChanged();
			this.document.Notifier.NotifyTextChanged();
			this.document.Notifier.NotifyStyleChanged();
			this.document.Notifier.NotifyTextStyleChanged();
			this.document.Notifier.NotifyUndoRedoChanged();
			this.document.Notifier.NotifyArea();
		}

		// Construit le menu des actions à refaire/annuler.
		public VMenu CreateUndoRedoMenu(MessageEventHandler message)
		{
			string[] undoNames = this.opletQueue.UndoActionNames;
			string[] redoNames = this.opletQueue.RedoActionNames;

			// Pour des raisons historiques, le menu est construit à l'envers,
			// c'est-à-dire la dernière action au début du menu (en haut).
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

			// Met éventuellement la dernière action à refaire.
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

			// Met les actions à refaire puis à celles à annuler.
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

			// Met éventuellement la dernière action à annuler.
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

			// Génère le menu à l'envers, c'est-à-dire la première action au
			// début du menu (en haut).
			VMenu menu = new VMenu();
			for ( int i=list.Count-1 ; i>=0 ; i-- )
			{
				menu.Items.Add(list[i]);
			}
			menu.AdjustSize();
			return menu;
		}

		// Crée une case du menu des actions à refaire/annuler.
		protected void CreateUndoRedoMenu(System.Collections.ArrayList list, MessageEventHandler message,
										  int active, int rank, string action, int todo)
		{
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
		#endregion


		#region Order
		// Met dessus tous les objets sélectionnés.
		public void OrderUpOneSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.OrderUpOne) )
			{
				this.OrderOneSelection(1);
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		// Met dessous tous les objets sélectionnés.
		public void OrderDownOneSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.OrderDownOne) )
			{
				this.OrderOneSelection(-1);
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		// Met au premier plan tous les objets sélectionnés.
		public void OrderUpAllSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.OrderUpAll) )
			{
				this.OrderAllSelection(1);
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		// Met à l'arrière plan tous les objets sélectionnés.
		public void OrderDownAllSelection()
		{
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

		// Retourne l'index du premier objet sélectionné.
		protected int OrderFirstSelected()
		{
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

		// Retourne l'index du dernier objet sélectionné.
		protected int OrderLastSelected()
		{
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
		// Distance du déplacement horizontal.
		public double MoveDistanceH
		{
			get { return this.moveDistance.X; }
			set { this.moveDistance.X = value; }
		}

		// Distance du déplacement vertical.
		public double MoveDistanceV
		{
			get { return this.moveDistance.Y; }
			set { this.moveDistance.Y = value; }
		}

		// Angle de rotation.
		public double RotateAngle
		{
			get { return this.rotateAngle; }
			set { this.rotateAngle = value; }
		}

		// Facteur d'échelle pour les agrandissement/réductions.
		public double ScaleFactor
		{
			get { return this.scaleFactor; }
			set { this.scaleFactor = value; }
		}

		// Facteur d'ajustement de couleur.
		public double ColorAdjust
		{
			get { return this.colorAdjust; }
			set { this.colorAdjust = value; }
		}

		// Déplace tous les objets sélectionnés.
		public void MoveSelection(Point dir, int alter)
		{
			if ( this.tool == "Edit" )  return;

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

		// Déplace tous les objets sélectionnés.
		public void MoveSelection(Point move)
		{
			if ( this.tool == "Edit" )  return;
			string name = string.Format(Res.Strings.Action.Move, this.RealToString(move.X), this.RealToString(move.Y));
			this.PrepareOper(name);
			this.ActiveViewer.Selector.OperMove(move);
			this.TerminateOper();
		}

		// Tourne tous les objets sélectionnés.
		public void RotateSelection(double angle)
		{
			if ( this.tool == "Edit" )  return;
			string name = string.Format(Res.Strings.Action.Rotate, this.AngleToString(angle));
			this.PrepareOper(name);
			this.ActiveViewer.Selector.OperRotate(angle);
			this.TerminateOper();
		}

		// Miroir de tous les objets sélectionnés. 
		public void MirrorSelection(bool horizontal)
		{
			if ( this.tool == "Edit" )  return;
			string name = horizontal ? Res.Strings.Action.MirrorH : Res.Strings.Action.MirrorV;
			this.PrepareOper(name);
			this.ActiveViewer.Selector.OperMirror(horizontal);
			this.TerminateOper();
		}

		// Mise à l'échelle de tous les objets sélectionnés.
		public void ScaleSelection(double scale)
		{
			if ( this.tool == "Edit" )  return;
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

		// Prépare pour l'opération.
		protected void PrepareOper(string name)
		{
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

		// Termine l'opération.
		protected void TerminateOper()
		{
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
		// Aligne sur la grille tous les objets sélectionnés.
		public void AlignGridSelection()
		{
			this.OpletQueueBeginAction(Res.Strings.Action.AlignGrid);
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				obj.AlignGrid(context);
			}
			this.OpletQueueValidateAction();
		}

		// Aligne tous les objets sélectionnés.
		public void AlignSelection(int dir, bool horizontal)
		{
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

		// Distribue tous les objets sélectionnés.
		public void ShareSelection(int dir, bool horizontal)
		{
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

		// Distribue les espaces entre tous les objets sélectionnés.
		public void SpaceSelection(bool horizontal)
		{
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
				pos += obj.BoundingBoxDetect.Size/2;  // position souhaitée du centre de l'objet

				Point move = new Point(0,0);
				if ( horizontal )  move.X = pos.X-so.Position;
				else               move.Y = pos.Y-so.Position;

				this.MoveAllStarting(obj);
				this.MoveAllProcess(obj, move);

				pos += obj.BoundingBoxDetect.Size/2;
			}
			this.OpletQueueValidateAction();
		}

		// Début du déplacement d'un objet isolé ou d'un groupe.
		protected void MoveAllStarting(Objects.Abstract group)
		{
			group.MoveAllStarting();

			if ( group is Objects.Group )
			{
				foreach ( Objects.Abstract obj in this.document.Deep(group) )
				{
					obj.MoveAllStarting();
				}
			}
		}

		// Effectue le déplacement d'un objet isolé ou d'un groupe.
		protected void MoveAllProcess(Objects.Abstract group, Point move)
		{
			group.MoveAllProcess(move);

			if ( group is Objects.Group )
			{
				foreach ( Objects.Abstract obj in this.document.Deep(group) )
				{
					obj.MoveAllProcess(move);
				}
			}
		}

		// Construit la liste triée de tous les objets à distribuer,
		// de gauche à droite ou de bas en haut.
		protected System.Collections.ArrayList ShareSortedList(int dir, bool horizontal)
		{
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

		// Cette classe représente un objet à distribuer, essentiellement
		// représentée par la position de référence de l'objet. Selon dir et
		// horizontal, il peut s'agir de la position x ou y d'une extrémité
		// ou du centre de l'objet.
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

		// Ajuste tous les objets sélectionnés.
		public void AdjustSelection(bool horizontal)
		{
			this.OpletQueueBeginAction(Res.Strings.Action.Adjust);
			Rectangle globalBox = this.SelectedBbox;
			Selector selector = new Selector(this.document);
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				// A chaque itération, l'objet tend vers la bonne largeur.
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
		// Ajuste la couleur de tous les objets sélectionnés, y compris à l'intérieur
		// des groupes.
		public void ColorSelection(ColorSpace cs)
		{
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

		// Ajuste la couleur de tous les objets sélectionnés, y compris à l'intérieur
		// des groupes.
		public void ColorSelection(double adjust, bool stroke)
		{
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

		// Sélectionne en profondeur tous les objets déjà sélectionnés.
		protected void DeepSelect(bool select)
		{
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
		// Fusionne tous les objets sélectionnés.
		public void MergeSelection()
		{
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

		// Extrait tous les objets sélectionnés du groupe.
		public void ExtractSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Extract) )
			{
				this.Extract();
				this.document.Notifier.NotifySelectionChanged();

				this.OpletQueueValidateAction();
			}
		}

		// Groupe tous les objets sélectionnés.
		public void GroupSelection()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.Group) )
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

			// Extrait tous les objets sélectionnés dans la liste extract.
			Rectangle bbox = Rectangle.Empty;
			foreach ( Objects.Abstract obj in this.document.Flat(layer, true) )
			{
				extract.Add(obj);
				bbox.MergeWith(obj.BoundingBoxGroup);
			}

			// Supprime les objets sélectionnés de la liste principale, sans
			// supprimer les propriétés.
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

		// Entre dans tous les objets sélectionnés.
		public void InsideSelection()
		{
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

		// Sort de tous les objets sélectionnés.
		public void OutsideSelection()
		{
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


		#region Combine and Fragment
		// Choix du diviseur avec Ctrl.
		public double ToLinePrecision
		{
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

		// Combine tous les objets sélectionnés.
		public void CombineSelection()
		{
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

				for ( int rank=0 ; rank<100 ; rank++ )
				{
					Path path = obj.GetPath(rank);
					if ( path == null )
					{
						if ( rank == 0 )  error = true;
						break;
					}

					if ( bezier == null )
					{
						bezier = new Objects.Bezier(this.document, obj);
						layer.Objects.Add(bezier);
						bezier.Select(true);
						this.XferProperties(bezier, obj);
						this.TotalSelected ++;
					}
			
					if ( bezier.CreateFromPath(path, -1) )
					{
						obj.Mark = true;  // il faudra le détruire
					}
					else
					{
						obj.Mark = false;  // il ne faudra pas le détruire
						error = true;
					}
				}
			}
			bezier.CreateFinalise();
			this.Simplify(bezier);
			this.document.Notifier.NotifyArea(bezier.BoundingBox);

			this.DeleteSelection(true);  // détruit les objets sélectionnés et marqués
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.Combine;
				this.ActiveViewer.DialogError(message);
			}
		}

		// Sépare tous les objets sélectionnés.
		public void UncombineSelection()
		{
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

				for ( int rank=0 ; rank<100 ; rank++ )
				{
					Path path = obj.GetPath(rank);
					if ( path == null )
					{
						if ( rank == 0 )  error = true;
						break;
					}

					Objects.Bezier first = null;
					for ( int i=0 ; i<100 ; i++ )
					{
						Objects.Bezier bezier = new Objects.Bezier(this.document, obj);
						layer.Objects.Add(bezier);
						bezier.Select(true);
						this.XferProperties(bezier, obj);
						this.TotalSelected ++;
						if ( first == null )  first = bezier;

						if ( bezier.CreateFromPath(path, i) )
						{
							bezier.CreateFinalise();
							this.Simplify(bezier);
							this.document.Notifier.NotifyArea(bezier.BoundingBox);
							obj.Mark = true;  // il faudra le détruire
						}
						else
						{
							layer.Objects.Remove(bezier);
							this.TotalSelected --;

							if ( i == 1 )  // un seul objet créé ?
							{
								layer.Objects.Remove(first);
								this.TotalSelected --;
								obj.Mark = false;  // il ne faudra pas le détruire
								error = true;
							}

							break;
						}
					}
				}
			}
			this.DeleteSelection(true);  // détruit les objets sélectionnés et marqués
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.Uncombine;
				this.ActiveViewer.DialogError(message);
			}
		}

		// Converti en Bézier tous les objets sélectionnés.
		public void ToBezierSelection()
		{
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

				for ( int rank=0 ; rank<100 ; rank++ )
				{
					Path path = obj.GetPath(rank);
					if ( path == null )
					{
						if ( rank == 0 )  error = true;
						break;
					}

					Objects.Bezier bezier = new Objects.Bezier(this.document, obj);
					layer.Objects.Add(bezier);
					bezier.Select(true);
					this.XferProperties(bezier, obj);
					this.TotalSelected ++;

					if ( bezier.CreateFromPath(path, -1) )
					{
						bezier.CreateFinalise();
						this.Simplify(bezier);
						this.document.Notifier.NotifyArea(bezier.BoundingBox);
						obj.Mark = true;  // il faudra le détruire
					}
					else
					{
						obj.Mark = false;  // il ne faudra pas le détruire
						layer.Objects.Remove(bezier);
						this.TotalSelected --;
						error = true;
					}
				}
			}
			this.DeleteSelection(true);  // détruit les objets sélectionnés et marqués
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.ToBezier;
				this.ActiveViewer.DialogError(message);
			}
		}

		// Converti en polygone tous les objets sélectionnés.
		public void ToPolySelection()
		{
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

				for ( int rank=0 ; rank<100 ; rank++ )
				{
					Path path = obj.GetPath(rank);
					if ( path == null )
					{
						if ( rank == 0 )  error = true;
						break;
					}

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

					if ( poly.CreateFromPath(p, -1) )
					{
						poly.CreateFinalise();
						this.Simplify(poly);
						this.document.Notifier.NotifyArea(poly.BoundingBox);
						obj.Mark = true;  // il faudra le détruire
					}
					else
					{
						obj.Mark = false;  // il ne faudra pas le détruire
						layer.Objects.Remove(poly);
						this.TotalSelected --;
						error = true;
					}
				}
			}
			this.DeleteSelection(true);  // détruit les objets sélectionnés et marqués
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.ToPoly;
				this.ActiveViewer.DialogError(message);
			}
		}

		// Converti en Bézier tous les objets sélectionnés.
		public void ToSimplestSelection()
		{
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

				for ( int rank=0 ; rank<100 ; rank++ )
				{
					Path path = obj.GetPath(rank);
					if ( path == null )
					{
						if ( rank == 0 )  error = true;
						break;
					}

					Objects.Bezier bezier = new Objects.Bezier(this.document, obj);
					layer.Objects.Add(bezier);
					bezier.Select(true);
					this.XferProperties(bezier, obj);
					this.TotalSelected ++;

					Path simplyPath = Geometry.PathToCurve(path);

					if ( bezier.CreateFromPath(simplyPath, -1) )
					{
						bezier.CreateFinalise();
						this.Simplify(bezier);
						this.document.Notifier.NotifyArea(bezier.BoundingBox);
						obj.Mark = true;  // il faudra le détruire
					}
					else
					{
						obj.Mark = false;  // il ne faudra pas le détruire
						layer.Objects.Remove(bezier);
						this.TotalSelected --;
						error = true;
					}
				}
			}
			this.DeleteSelection(true);  // détruit les objets sélectionnés et marqués
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.ToBezier;
				this.ActiveViewer.DialogError(message);
			}
		}

		// Fragmente tous les objets sélectionnés.
		public void FragmentSelection()
		{
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
					obj.Mark = true;  // il faudra le détruire
				}
				else
				{
					error = true;
				}
			}
			this.DeleteSelection(true);  // détruit les objets sélectionnés et marqués
			this.document.Notifier.NotifySelectionChanged();
			this.OpletQueueValidateAction();

			if ( error )
			{
				string message = Res.Strings.Error.Fragment;
				this.ActiveViewer.DialogError(message);
			}
		}

		// Fragmente un objet.
		protected bool FragmentObject(Objects.Abstract obj)
		{
			for ( int rank=0 ; rank<100 ; rank++ )
			{
				Path path = obj.GetPath(rank);
				if ( path == null )
				{
					return (rank != 0);
				}

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
							p1 = points[i++];
							p2 = points[i++];
							this.FragmentCreateBezier(current, p1, p1, p2, obj);
							current = p2;
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
			return false;
		}

		// Crée un fragment de ligne droite.
		protected bool FragmentCreateLine(Point p1, Point p2, Objects.Abstract model)
		{
			if ( p1 == p2 )  return false;
			Objects.Line line = new Objects.Line(this.document, model);
			line.CreateFromPoints(p1, p2);
			line.Select(true);
			this.TotalSelected ++;

			Objects.Abstract layer = this.ActiveViewer.DrawingContext.RootObject();
			layer.Objects.Add(line);

			this.document.Notifier.NotifyArea(line.BoundingBox);
			return true;
		}

		// Crée un fragment de ligne courbe.
		protected bool FragmentCreateBezier(Point p1, Point s1, Point s2, Point p2, Objects.Abstract model)
		{
			if ( p1 == p2 )  return false;
			Objects.Bezier bezier = new Objects.Bezier(this.document, model);
			bezier.CreateFromPoints(p1, s1, s2, p2);
			bezier.CreateFinalise();
			this.Simplify(bezier);
			this.TotalSelected ++;
			bezier.PropertyFillGradient.FillType = Properties.GradientFillType.None;
			bezier.PropertyFillGradient.Color1 = RichColor.FromARGB(0, 1,1,1);
			bezier.PropertyPolyClose.BoolValue = false;

			Objects.Abstract layer = this.ActiveViewer.DrawingContext.RootObject();
			layer.Objects.Add(bezier);

			this.document.Notifier.NotifyArea(bezier.BoundingBox);
			return true;
		}

		// Reprend éventuellement quelques propriétés à l'objet model pour
		// un objet polygone ou bézier.
		protected void XferProperties(Objects.Abstract obj, Objects.Abstract model)
		{
			if ( model is Objects.TextLine )
			{
				obj.PropertyFillGradient.FillType = Properties.GradientFillType.None;
				obj.PropertyFillGradient.Color1 = model.PropertyTextFont.FontColor;
				obj.PropertyLineMode.Width = 0.0;
			}
		}

		// Simplifie un objet.
		protected void Simplify(Objects.Abstract obj)
		{
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
		#endregion


		#region ShaperHandle
		// Met à jour toutes les commandes pour le modeleur.
		public void ShaperHandleUpdate(CommandDispatcher cd)
		{
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

		// Effectue une commande du modeleur sur une poignée.
		public void ShaperHandleCommand(string cmd)
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer, true) )
			{
				obj.ShaperHandleCommand(cmd);
			}
		}
		#endregion


		#region Booolean
		// Opérations booléenne sur tous les objets sélectionnés.
		public void BooleanSelection(Drawing.PathOperation op)
		{
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

				for ( int rank=0 ; rank<100 ; rank++ )
				{
					Path path = obj.GetPath(rank);
					if ( path == null )
					{
						if ( rank == 0 )  error = true;
						break;
					}

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

					obj.Mark = true;  // il faudra le détruire
				}
			}

			Objects.Bezier bezier = new Objects.Bezier(this.document, model);
			layer.Objects.Add(bezier);
			bezier.Select(true);
			this.XferProperties(bezier, model);
			this.TotalSelected ++;

			if ( bezier.CreateFromPath(pathResult, -1) )
			{
				bezier.CreateFinalise();
				this.Simplify(bezier);
				this.document.Notifier.NotifyArea(bezier.BoundingBox);
				this.DeleteSelection(true);  // détruit les objets sélectionnés et marqués
			}
			else
			{
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
		#endregion


		#region Hide
		// Cache tous les objets sélectionnés du calque courant.
		public void HideSelection()
		{
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

		// Cache tous les objets non sélectionnés du calque courant.
		public void HideRest()
		{
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
			this.ActiveViewer.ClearHilite();
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
		public void PageNew(int rank, string name)
		{
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

		// Duplique une nouvelle page.
		public void PageDuplicate(int rank, string name)
		{
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

		// Supprime une page.
		public void PageDelete(int rank)
		{
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

		// Permute deux pages.
		public void PageSwap(int rank1, int rank2)
		{
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

		// Retourne le nom court d'une page ("n" ou "Mn").
		public string PageShortName(int rank)
		{
			UndoableList pages = this.document.GetObjects;
			Objects.Page page = pages[rank] as Objects.Page;
			return page.ShortName;
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

		// Retourne le nombre total de pages imprimables.
		public int PrintableTotalPages()
		{
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

		// Retourne le rang d'une page imprimable.
		public int PrintablePageRank(int index)
		{
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

		// Met à jour après une suppression de page.
		protected void UpdatePageDelete(Objects.Page deletedPage)
		{
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

		// Met à jour tout ce qu'il faut après un changement de page (création d'une
		// nouvelle page, suppression d'une page, etc.).
		public void UpdatePageAfterChanging()
		{
			this.UpdatePageShortNames();
			this.UpdatePageNumbers();
		}

		// Met à jour tous les noms courts des pages ("n" ou "Mn").
		public void UpdatePageShortNames()
		{
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

		// Met à jour les numéros de page de tous les objets du document.
		public void UpdatePageNumbers()
		{
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

		// Génère la liste des pages maîtres à utiliser pour une page donnée.
		public void ComputeMasterPageList(System.Collections.ArrayList masterPageList, int pageNumber)
		{
			masterPageList.Clear();
			Objects.Page currentPage = this.document.GetObjects[pageNumber] as Objects.Page;

			if ( currentPage.MasterType == Objects.MasterType.Slave )
			{
				if ( currentPage.MasterUse == Objects.MasterUse.Specific )
				{
					if ( currentPage.MasterPageToUse != null &&
						 currentPage.MasterPageToUse.MasterType != Objects.MasterType.Slave )
					{
						this.ComputeMasterPageList(masterPageList, currentPage.MasterPageToUse);
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
							this.ComputeMasterPageList(masterPageList, page);
						}

						if ( page.MasterType == Objects.MasterType.Even )
						{
							if ( currentPage.Rank%2 != 0 )
							{
								this.ComputeMasterPageList(masterPageList, page);
							}
						}

						if ( page.MasterType == Objects.MasterType.Odd )
						{
							if ( currentPage.Rank%2 == 0 )
							{
								this.ComputeMasterPageList(masterPageList, page);
							}
						}
					}
				}
			}
			else	// page modèle ?
			{
				if ( currentPage.MasterSpecific )
				{
					if ( currentPage.MasterPageToUse != null &&
						 currentPage.MasterPageToUse.MasterType != Objects.MasterType.Slave )
					{
						this.ComputeMasterPageList(masterPageList, currentPage.MasterPageToUse);
					}
				}
			}

			masterPageList.Reverse();
		}

		protected void ComputeMasterPageList(System.Collections.ArrayList masterPageList, Objects.Page master)
		{
			if ( masterPageList.Contains(master) )  return;

			masterPageList.Add(master);

			if ( master.MasterSpecific )
			{
				if ( master.MasterPageToUse != null &&
					 master.MasterPageToUse.MasterType != Objects.MasterType.Slave )
				{
					this.ComputeMasterPageList(masterPageList, master.MasterPageToUse);
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

		// Retourne les informations qui résument la structure d'une page.
		public System.Collections.ArrayList GetPageStackInfos(int pageNumber)
		{
			System.Collections.ArrayList infos = new System.Collections.ArrayList();

			System.Collections.ArrayList masterList = new System.Collections.ArrayList();
			this.ComputeMasterPageList(masterList, pageNumber);

			// Mets d'abord les premiers calques de toutes les pages maîtres.
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

			// Mets ensuite tous les calques de la page.
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

			// Mets finalement les derniers calques de toutes les pages maîtres.
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
		// Commence un changement de calque.
		public void InitiateChangingLayer()
		{
			this.DeselectAll();
			this.ActiveViewer.ClearHilite();
		}

		// Termine un changement de calque.
		public void TerminateChangingLayer(int rank)
		{
			int page = this.ActiveViewer.DrawingContext.CurrentPage;
			this.ActiveViewer.DrawingContext.PageLayer(page, rank);
			this.DirtyCounters();
		}

		// Crée un nouveau calque.
		public void LayerNew(int rank, string name)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerNew) )
			{
				this.InitiateChangingLayer();

				// Liste des calques:
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

		// Crée un nouveau calque contenant les objets sélectionnés.
		public void LayerNewSel(int rank, string name)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerNewSel) )
			{
				this.InsertOpletDirtyCounters();

				// Liste des calques:
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

		// Duplique un calque.
		public void LayerDuplicate(int rank, string name)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerDuplicate) )
			{
				this.InitiateChangingLayer();

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

				this.TerminateChangingLayer(rank+1);

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

			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerDelete) )
			{
				this.InitiateChangingLayer();

				// Liste des calques:
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

		// Fusionne deux calques.
		public void LayerMerge(int rankSrc, int rankDst)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerMerge) )
			{
				this.InitiateChangingLayer();

				// Liste des calques:
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

		// Permute deux calques.
		public void LayerSwap(int rank1, int rank2)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.LayerSwap) )
			{
				this.InitiateChangingLayer();

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

				this.TerminateChangingLayer(rank2);

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
		#endregion

		#region MagnetLayer
		// Change l'état "objets magnétiques" d'un calque.
		public void MagnetLayerInvert(int rank)
		{
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

		// Donne l'état "objets magnétiques" d'un calque.
		public bool MagnetLayerState(int rank)
		{
			UndoableList list = this.ActiveViewer.DrawingContext.RootObject(1).Objects;
			Objects.Layer layer = list[rank] as Objects.Layer;
			return layer.Magnet;
		}

		// Détecte sur quel segment de droite est la souris.
		public bool MagnetLayerDetect(Point pos, Point filterP1, Point filterP2,
									  out Point p1, out Point p2)
		{
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

		// Génère la liste des calques magnétiques à utiliser pour une page donnée.
		public void ComputeMagnetLayerList(System.Collections.ArrayList magnetLayerList, int pageNumber)
		{
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

		// Ajoute toutes les propriétés des objets sélectionnés dans une liste.
		// Tient compte du mode propertiesDetail.
		public void PropertiesList(System.Collections.ArrayList list)
		{
			this.OpletQueueEnable = false;
			list.Clear();

			if ( this.tool == "Picker" && this.TotalSelected == 0 )  // pipette ?
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
			else	// choix d'un objet à créer ?
			{
				// Crée un objet factice (document = null), c'est-à-dire sans
				// propriétés, qui servira juste de filtre pour objectMemory.
				Objects.Abstract dummy = Objects.Abstract.CreateObject(null, this.tool, null);
				this.ObjectMemoryTool.PropertiesList(list, dummy);
				dummy.Dispose();
			}

			list.Sort();
			this.OpletQueueEnable = true;
		}

		// Ajoute le détail de toutes les propriétés des objets sélectionnés
		// dans une liste, en vue d'une modifications des couleurs.
		protected void PropertiesListDeep(System.Collections.ArrayList list)
		{
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
				obj.HandlePropertiesUpdate();
				this.document.Notifier.NotifyArea(this.ActiveViewer, obj.BoundingBox);
			}
		}
		#endregion


		#region IsTextPanelExtended
		// Indique si un panneau pour le texte est étendu ou pas.
		public bool IsTextPanelExtended(TextPanels.Abstract panel)
		{
			if ( this.tableTextPanelExtended == null )
			{
				this.tableTextPanelExtended = new System.Collections.Hashtable();
			}

			System.Type type = panel.GetType();  // clé pour HashTable

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

		// Modifie l'état étendu ou pas d'un panneau pour le texte.
		public void IsTextPanelExtended(TextPanels.Abstract panel, bool extended)
		{
			if ( this.tableTextPanelExtended == null )
			{
				this.tableTextPanelExtended = new System.Collections.Hashtable();
			}

			System.Type type = panel.GetType();  // clé pour HashTable

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
		// Montre les objets sélectionnés si nécessaire.
		protected void ShowSelection()
		{
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

		// Zoom sur les objets sélectionnés.
		public void ZoomSel()
		{
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

		// Zoom sur la largeur des objets sélectionnés.
		public void ZoomSelWidth()
		{
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

		// Change le zoom d'un certain facteur pour agrandir une zone rectangulaire.
		public void ZoomChangeWidth(Point p1, Point p2)
		{
			Viewer viewer = this.ActiveViewer;
			DrawingContext context = this.ActiveViewer.DrawingContext;

			if ( p1.X == p2.X )  return;
			double factor = viewer.Width/(System.Math.Abs(p1.X-p2.X)*context.ScaleX);
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

		// Force une valeur de zoom.
		public void ZoomValue(double value)
		{
			this.ZoomValue(value, true);
		}

		// Force une valeur de zoom.
		public void ZoomValue(double value, bool memorize)
		{
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


		#region Aggregates
		// Retourne le nom de l'agrégat sélectionné.
		public string AggregateGetSelectedName()
		{
			string name = "";
			this.aggregateUsed = -1;

			if ( this.IsTool )  // objets sélectionnés ?
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
							if ( name != aggName )  // plusieurs agrégats différents ?
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

#if false
		// Cherche un agrégat d'après son nom. Rien ne garantit que plusieurs
		// agrégats n'aient pas des noms identiques, mais c'est le seul moyen
		// de retrouver un agrégat à partir du nom choisi par l'utilisateur dans
		// le TextFieldCombo du panneau principal !
		public Properties.Aggregate AggregateSearch22(string name)
		{
			UndoableList aggregates = this.document.Aggregates;
			foreach ( Properties.Aggregate agg in aggregates )
			{
				if ( agg.AggregateName == name )
				{
					return agg;
				}
			}

			if ( this.objectMemory.AggregateName == name )
			{
				return this.objectMemory.Aggregate;
			}

			if ( this.objectMemoryText.AggregateName == name )
			{
				return this.objectMemoryText.Aggregate;
			}

			return null;
		}
#endif

		// Crée un nouvel agrégat vide.
		public void AggregateNewEmpty(int rank, string name, bool putToList)
		{
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

		// Crée un nouvel agrégat avec seulement 3 propriétés.
		public void AggregateNew3(int rank, string name, bool putToList)
		{
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

		// Crée un nouvel agrégat avec toutes les propriétés.
		public void AggregateNewAll(int rank, string name, bool putToList)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			Objects.Abstract model = null;
			if ( this.IsTool )  // objets sélectionnés ?
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

			this.AggregateUse(agg);
		}

		// Crée un nouvel agrégat avec seulement 3 propriétés, pas encore référencé.
		protected Properties.Aggregate AggregateCreate(string name, bool three)
		{
			this.OpletQueueEnable = false;

			Properties.Aggregate agg = new Properties.Aggregate(this.document);

			if ( name == "" )
			{
				name = this.GetNextAggregateName();  // nom unique
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

		// Crée un nouvel agrégat avec toutes les propriétés d'un objet modèle,
		// pas encore référencé.
		protected Properties.Aggregate AggregateCreate(string name, Objects.Abstract model)
		{
			this.OpletQueueEnable = false;

			Properties.Aggregate agg = new Properties.Aggregate(this.document);

			if ( name == "" )
			{
				name = this.GetNextAggregateName();  // nom unique
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

		// Crée une nouvelle propriété pour un agrégat. Dans Aggregate.Styles,
		// les propriétés sont toujours selon l'ordre Properties.Abstract.SortOrder !
		protected Properties.Abstract AggregateCreateProperty(Properties.Aggregate agg, Objects.Abstract model, Properties.Type type)
		{
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

		// Utilise un agrégat.
		public void AggregateUse(Properties.Aggregate agg)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			if ( this.IsTool )  // objets sélectionnés ?
			{
				if ( this.TotalSelected == 0 )  return;

				using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateUse) )
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

		// Ajoute l'agrégat dans la liste du document, si nécessaire.
		public void AggregateToDocument(Properties.Aggregate agg)
		{
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

		// Libère les objets sélectionnés des agrégats.
		public void AggregateFree()
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			if ( this.IsTool )  // objets sélectionnés ?
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

		// Libère tous les objets coupés ou copiés des agrégats.
		protected void AggregateFreeAll()
		{
			DrawingContext context = this.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Flat(layer, false) )
			{
				obj.AggregateFree();
				obj.Aggregates.Clear();
			}
		}

		// Duplique un agrégat.
		public void AggregateDuplicate(int rank)
		{
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

		// Supprime un agrégat.
		public void AggregateDelete(int rank)
		{
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

		// Permute deux agrégats.
		public void AggregateSwap(int rank1, int rank2)
		{
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

		// Change le nom d'un agrégat.
		public void AggregateChangeName(string name)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			if ( name == "..." )  return;
			this.document.IsDirtySerialize = true;

			if ( this.IsTool )  // objets sélectionnés ?
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

		// Crée un nouveau style dans un agrégat.
		public void AggregateStyleNew(Properties.Aggregate agg, Properties.Type type)
		{
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

		// Supprime le style sélectionné d'un agrégat.
		public void AggregateStyleDelete(Properties.Aggregate agg)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			if ( agg.Styles.Selected == -1 )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateStyleDelete) )
			{
				int rank = agg.Styles.Selected;
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

		// Ajoute un enfant à un agrégat.
		public void AggregateChildrensNew(Properties.Aggregate agg, Properties.Aggregate newAgg)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			using ( this.OpletQueueBeginAction(Res.Strings.Action.AggregateChildrensNew) )
			{
				agg.Childrens.Insert(0, newAgg);
				agg.Childrens.Selected = 0;

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

		// Permute deux enfants.
		public void AggregateChildrensSwap(Properties.Aggregate agg, int rank1, int rank2)
		{
			if ( this.ActiveViewer.IsCreating )  return;
			this.document.IsDirtySerialize = true;

			string op = (rank1 > rank2) ? Res.Strings.Action.AggregateChildrensUp : Res.Strings.Action.AggregateChildrensDown;
			using ( this.OpletQueueBeginAction(op) )
			{
				Properties.Aggregate temp = agg.Childrens[rank1] as Properties.Aggregate;
				agg.Childrens.RemoveAt(rank1);
				agg.Childrens.Insert(rank2, temp);
				agg.Childrens.Selected = rank2;

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

		// Supprime un enfant à un agrégat.
		public void AggregateChildrensDelete(Properties.Aggregate agg, Properties.Aggregate delAgg)
		{
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

		// Reprend les propriétés d'un objet modèle.
		public void AggregatePicker(Objects.Abstract model)
		{
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

		// Donne le prochain nom unique d'agrégat.
		protected string GetNextAggregateName()
		{
			return string.Format(Res.Strings.Aggregate.Name, this.document.GetNextUniqueAggregateId());
		}
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
		// Ajoute un oplet pour mémoriser les dimensions du document.
		public void InsertOpletSize()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletSize oplet = new OpletSize(this.document);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// Mémorise les dimensions.
		protected class OpletSize : AbstractOplet
		{
			public OpletSize(Document host)
			{
				this.host = host;
				this.documentSize = this.host.Size;
				this.outsideArea = this.host.Modifier.outsideArea;
			}

			protected void Swap()
			{
				Size temp = this.documentSize;
				this.documentSize = this.host.Size;
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
		// Ajoute un oplet pour indiquer que les compteurs devront être mis à jour.
		public void InsertOpletDirtyCounters()
		{
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
		// Vide toutes les queues.
		public void OpletQueuePurge()
		{
			this.opletQueue.PurgeUndo();
			this.opletQueue.PurgeRedo();
			this.opletLastCmd = "";
		}

		// Détermine si les actions seront annulables ou non.
		public bool OpletQueueEnable
		{
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

		// Début d'une zone annulable.
		public System.IDisposable OpletQueueBeginAction(string name)
		{
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
					 cmd == "ChangeDocSize"       )
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

		// Nomme une action annulable.
		public void OpletQueueNameAction(string name)
		{
			this.opletQueue.DefineActionName(name);
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
		protected bool							namesExist;
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
		protected SelectorType					operInitialSelector;
		protected SelectorType					bookInitialSelector = SelectorType.None;
		protected Containers.Abstract			activeContainer;
		protected bool[]						isPropertiesExtended;
		protected System.Collections.Hashtable	tableTextPanelExtended;
		protected RealUnitType					realUnitDimension;
		protected double						realScale;
		protected double						realPrecision;
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

		public static readonly double			fontSizeScale = 3.5;  // empyrique !
	}
}
