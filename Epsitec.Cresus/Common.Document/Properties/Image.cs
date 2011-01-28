using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;
using Epsitec.Common.Drawing.Platform;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Image représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Image : Abstract
	{
		//	Rotation CCW de l'image.
		public enum Rotation
		{
			Angle0,		// droit
			Angle90,	// quart de tour à gauche
			Angle180,	// demi-tour
			Angle270,	// quart de tour à droite
		}


		public Image(Document document, Type type) : base(document, type)
		{
		}

		public override Objects.HandleType GetHandleType(int rank)
		{
			switch (rank)
            {
				case 0:
					return Objects.HandleType.PropertyMove;

				default:
					return base.GetHandleType (rank);
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			this.filename       = "";
			this.date           = System.DateTime.MinValue;
			this.shortName      = "";
			this.insideDoc      = true;
			this.fromClipboard  = false;
			this.rotation       = Rotation.Angle0;
			this.mirrorH        = false;
			this.mirrorV        = false;
			this.homo           = true;
			this.filterCategory = 0;  // catégorie A
			this.cropMargins    = Margins.Zero;
		}

		public string FileName
		{
			//	Nom du fichier original avec le chemin d'accès complet.
			get
			{
				return this.filename;
			}

			set
			{
				if (this.filename != value)
				{
					this.NotifyBefore();
					this.filename = value;
					this.NotifyAfter();
				}
			}
		}

		public System.DateTime FileDate
		{
			//	Date de dernière modification de l'image.
			get
			{
				return this.date;
			}

			set
			{
				if (this.date != value)
				{
					this.date = value;
				}
			}
		}

		public string ShortName
		{
			//	Nom court de l'image, à l'intérieur du fichier Zip.
			get
			{
				return this.shortName;
			}

			set
			{
				this.shortName = value;
			}
		}

		public bool InsideDoc
		{
			//	Détermine si les données de l'images sont incorporées au fichier crdoc.
			get
			{
				return this.insideDoc;
			}

			set
			{
				this.insideDoc = value;
			}
		}

		public bool FromClipboard
		{
			//	Indique si les données de l'images proviennent du clipboard.
			get
			{
				return this.fromClipboard;
			}

			set
			{
				this.fromClipboard = value;
			}
		}

		public Rotation RotationMode
		{
			get
			{
				return this.rotation;
			}
			
			set
			{
				if (this.rotation != value)
				{
					this.NotifyBefore();
					this.rotation = value;
					this.NotifyAfter();
				}
			}
		}

		public bool MirrorH
		{
			get
			{
				return this.mirrorH;
			}
			
			set
			{
				if ( this.mirrorH != value )
				{
					this.NotifyBefore();
					this.mirrorH = value;
					this.NotifyAfter();
				}
			}
		}

		public bool MirrorV
		{
			get
			{
				return this.mirrorV;
			}
			
			set
			{
				if ( this.mirrorV != value )
				{
					this.NotifyBefore();
					this.mirrorV = value;
					this.NotifyAfter();
				}
			}
		}

		public bool Homo
		{
			get
			{
				return this.homo;
			}
			
			set
			{
				if ( this.homo != value )
				{
					this.NotifyBefore();
					this.homo = value;
					this.NotifyAfter();
				}
			}
		}

		public int FilterCategory
		{
			//	Catégorie du filtre à utiliser.
			//	-1 = aucun, 0 = groupe A, 1 = groupe B
			get
			{
				return this.filterCategory;
			}

			set
			{
				if (this.filterCategory != value)
				{
					this.NotifyBefore();
					this.filterCategory = value;
					this.NotifyAfter();
				}
			}
		}

		public Margins CropMargins
		{
			//	Marge de recadrage.
			get
			{
				return this.cropMargins;
			}

			set
			{
				if (this.cropMargins != value)
				{
					this.NotifyBefore();
					this.cropMargins = value;
					this.NotifyAfter();
				}
			}
		}


		public override string SampleText
		{
			//	Donne le petit texte pour les échantillons.
			get
			{
				return Misc.ExtractName(this.filename);
			}
		}

		public override void PutStyleBrief(System.Text.StringBuilder builder)
		{
			//	Construit le texte résumé d'un style pour une propriété.
			this.PutStyleBriefPrefix(builder);

			builder.Append(this.filename);
			
			this.PutStyleBriefPostfix(builder);
		}


		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propriété modifie la bbox de l'objet.
			get { return true; }
		}


		public override void MoveHandleStarting(Objects.Abstract obj, int rank, Point pos, DrawingContext drawingContext)
		{
			//	Début du déplacement d'une poignée.
		}

		public override int TotalHandle(Objects.Abstract obj)
		{
			//	Nombre de poignées.
			return 1;
		}

		public override bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			//	Indique si une poignée est visible.
			return obj.IsSelected && !string.IsNullOrEmpty (this.filename);
		}

		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			//	Retourne la position d'une poignée.
			this.UpdateCropLogic (obj);
			Point pos = new Point ();

			if (rank == 0)  // position ?
			{
				Point center;
				double radius;
				this.GetPositionCenter (obj, out center, out radius);

				Point rel = this.cropLogic.RelativePosition;
				pos = new Point (center.X+rel.X*radius, center.Y+rel.Y*radius);
			}

			return pos;
		}

		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			//	Modifie la position d'une poignée.
			this.UpdateCropLogic (obj);

			if (rank == 0)  // position ?
			{
				Point center;
				double radius;
				this.GetPositionCenter (obj, out center, out radius);

				double rx = (pos.X-center.X)/radius;
				double ry = (pos.Y-center.Y)/radius;

				this.cropLogic.RelativePosition = new Point (rx, ry);
			}

			base.SetHandlePosition (obj, rank, pos);
		}

		private void GetPositionCenter(Objects.Abstract obj, out Point center, out double radius)
		{
			var p0 = obj.Handle (0).Position;
			var p1 = obj.Handle (1).Position;
			var p2 = obj.Handle (2).Position;
			var p3 = obj.Handle (3).Position;

			center = (p0+p1+p2+p3)/4;

			double d1 = Point.Distance (p0, p3);
			double d2 = Point.Distance (p3, p1);
			double d3 = Point.Distance (p1, p2);
			double d4 = Point.Distance (p2, p0);

			radius = System.Math.Min (System.Math.Min (d1, d2), System.Math.Min (d3, d4)) / 2;
		}

		private void UpdateCropLogic(Objects.Abstract obj)
		{
			if (this.cropLogic == null)
			{
				this.cropLogic = new CropLogic (this.document, obj as Objects.Image);
			}

			this.cropLogic.UpdateImge ();
		}


		#region Popup interface
		public override void OpenOrClosePopupInterface(Objects.Abstract obj)
		{
			var viewer = this.document.Modifier.ActiveViewer;

			if (obj.IsSelected && !obj.IsGlobalSelected)  // objet sélectionné ?
			{
				if (viewer.PopupInterfaceFrame == null)
				{
					//	Crée l'interface popup.
					this.UpdateCropLogic (obj);

					var i = this.CreatePopupInterface (obj);
					viewer.OpenPopupInterface (i, obj, this.GetPopupInterfacePosition);
					viewer.MovePopupInterface ();
				}

				this.UpdatePopupInterface (obj);
			}
			else
			{
				this.ClosePopupInterface (obj);
			}
		}

		public override void ClosePopupInterface(Objects.Abstract obj)
		{
			var viewer = this.document.Modifier.ActiveViewer;

			if (viewer.PopupInterfaceFrame != null)
			{
				viewer.ClosePopupInterface ();
			}
		}

		private Point GetPopupInterfacePosition(Viewer viewer, Objects.Abstract obj)
		{
			var pos = viewer.InternalToScreen (obj.BoundingBoxThin.BottomLeft);
			return new Point (pos.X+5, pos.Y+5);
		}

		private void UpdatePopupInterface(Objects.Abstract obj)
		{
			this.UpdateCropLogic (obj);

			bool hasImage = !string.IsNullOrEmpty (this.FileName);

			this.popupInterfaceFillModeButton.Enable = hasImage;
			this.popupInterfaceRotateButton.Enable = hasImage;
			this.popupInterfaceSlider.Enable = hasImage;

			//	Met à jour le bouton 'fill'.
			if (this.cropLogic.IsFillMode)
			{
				this.popupInterfaceFillModeButton.IconUri = Misc.Icon ("ImageShowAll");
				ToolTip.Default.SetToolTip (this.popupInterfaceFillModeButton, Res.Strings.Action.ImageShowAll);
			}
			else
			{
				this.popupInterfaceFillModeButton.IconUri = Misc.Icon ("ImageFillFrame");
				ToolTip.Default.SetToolTip (this.popupInterfaceFillModeButton, Res.Strings.Action.ImageFillMode);
			}

			//	Met à jour le slider.
			var value = (decimal) this.cropLogic.RelativeZoom;

			if (this.popupInterfaceSlider.Value != value)
			{
				this.popupInterfaceSlider.Value = value;
				this.popupInterfaceSlider.Invalidate ();  // TODO: devrait être inutile
			}
		}

		private Widget CreatePopupInterface(Objects.Abstract obj)
		{
			double buttonSize = 23;

			var frame = new FrameBox ()
			{
				PreferredSize = new Size (220, 5+buttonSize+5),
				DrawFullFrame = true,
				BackColor = Drawing.Color.FromAlphaRgb (0.9, 0.8, 0.8, 0.8),
				Padding = new Margins (5),
			};

			var importButton = new IconButton
			{
				Parent = frame,
				PreferredSize = new Size (buttonSize, buttonSize),
				ButtonStyle = ButtonStyle.ActivableIcon,
				IconUri = Misc.Icon ("Import"),
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.popupInterfaceFillModeButton = new IconButton
			{
				Parent = frame,
				PreferredSize = new Size (buttonSize, buttonSize),
				ButtonStyle = ButtonStyle.ActivableIcon,
				Dock = DockStyle.Left,
				Margins = new Margins (5, 0, 0, 0),
			};

			this.popupInterfaceRotateButton = new IconButton
			{
				Parent = frame,
				PreferredSize = new Size (buttonSize, buttonSize),
				ButtonStyle = ButtonStyle.ActivableIcon,
				IconUri = Misc.Icon ("ImageRotation90"),
				Dock = DockStyle.Left,
				Margins = new Margins (-1, 0, 0, 0),
			};

			this.popupInterfaceSlider = new HSlider
			{
				Parent = frame,
				MinValue = 0.0M,
				MaxValue = 1.0M,
				Resolution = 0.0000001M,
				SmallChange = 0.1M,
				LargeChange = 0.5M,
				Value = (decimal) this.cropLogic.RelativeZoom,
				PreferredHeight = buttonSize-3-3,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 0, 3, 3),
			};

			ToolTip.Default.SetToolTip (importButton, Res.Strings.Action.ImageImport);
			ToolTip.Default.SetToolTip (this.popupInterfaceRotateButton, Res.Strings.Action.ImageRotate);
			ToolTip.Default.SetToolTip (this.popupInterfaceSlider, Res.Strings.Action.ImageZoom);

			importButton.Clicked += delegate
			{
				this.PopupInterfaceImport (importButton);
			};

			this.popupInterfaceFillModeButton.Clicked += delegate
			{
				this.PopupInterfaceSwapFill (obj);
			};

			this.popupInterfaceRotateButton.Clicked += delegate
			{
				this.PopupInterfaceRotate(obj);
			};

			this.popupInterfaceSlider.ValueChanged += delegate
			{
				this.PopupInterfaceChangeZoom (obj, (double) this.popupInterfaceSlider.Value);
			};

			return frame;
		}

		private void PopupInterfaceImport(Widget button)
		{
			var filename = Panels.Image.OpenImageDialog (this.document, button, this.FileName);

			if (filename != null)
			{
				using (this.document.Modifier.OpletQueueBeginAction (Res.Strings.Action.ImageImport, "ChangeImageFilename"))
				{
					this.FileName = filename;
					this.FileDate = this.document.ImageCache.LoadFromFile (this.FileName);
					this.document.Modifier.OpletQueueValidateAction ();
				}
			}
		}

		private void PopupInterfaceSwapFill(Objects.Abstract obj)
		{
			this.UpdateCropLogic (obj);
			bool isFill = this.cropLogic.IsFillMode;

			using (this.document.Modifier.OpletQueueBeginAction (isFill ? Res.Strings.Action.ImageShowAll : Res.Strings.Action.ImageFillMode, "ChangeImageZoom"))
			{
				if (isFill)
				{
					this.CropMargins = Margins.Zero;
				}
				else
				{
					this.cropLogic.FillAll ();
				}

				this.document.Modifier.OpletQueueValidateAction ();
			}
		}

		private void PopupInterfaceRotate(Objects.Abstract obj)
		{
			this.UpdateCropLogic (obj);

			using (this.document.Modifier.OpletQueueBeginAction (Res.Strings.Action.ImageZoom, "ChangeImageRotation"))
			{
				switch (this.RotationMode)
				{
					case Rotation.Angle0:
						this.RotationMode = Rotation.Angle90;
						break;

					case Rotation.Angle90:
						this.RotationMode = Rotation.Angle180;
						break;

					case Rotation.Angle180:
						this.RotationMode = Rotation.Angle270;
						break;

					case Rotation.Angle270:
						this.RotationMode = Rotation.Angle0;
						break;
				}

				this.document.Modifier.OpletQueueValidateAction ();
			}
		}

		private void PopupInterfaceChangeZoom(Objects.Abstract obj, double zoom)
		{
			using (this.document.Modifier.OpletQueueBeginAction (Res.Strings.Action.ImageZoom, "ChangeImageZoom"))
			{
				this.cropLogic.RelativeZoom = zoom;
				this.document.Modifier.OpletQueueValidateAction ();
			}
		}
		#endregion


		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			Image p = property as Image;
			p.filename        = this.filename;
			p.date            = this.date;
			p.shortName       = this.shortName;
			p.insideDoc       = this.insideDoc;
			p.fromClipboard   = this.fromClipboard;
			p.rotation        = this.rotation;
			p.mirrorH         = this.mirrorH;
			p.mirrorV         = this.mirrorV;
			p.homo            = this.homo;
			p.filterCategory  = this.filterCategory;
			p.cropMargins     = this.cropMargins;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Image p = property as Image;
			if ( p.filename       != this.filename       )  return false;
			if ( p.date           != this.date           )  return false;
			if ( p.shortName      != this.shortName      )  return false;
			if ( p.insideDoc      != this.insideDoc      )  return false;
			if ( p.fromClipboard  != this.fromClipboard  )  return false;
			if ( p.rotation       != this.rotation       )  return false;
			if ( p.mirrorH        != this.mirrorH        )  return false;
			if ( p.mirrorV        != this.mirrorV        )  return false;
			if ( p.homo           != this.homo           )  return false;
			if ( p.filterCategory != this.filterCategory )  return false;
			if ( p.cropMargins    != this.cropMargins    )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Image(document);
		}


		#region ImageFilter

		public static BitmapFileFormat FilterQualityToMode(double quality)
		{
			int n = (int) (quality*100);
			n = System.Math.Min(100, n);
			n = System.Math.Max(10, n);
			
			return new BitmapFileFormat ()
			{
				Type = BitmapFileType.Jpeg,
				Quality = n
			};
		}

		public static int FilterNameToIndex(string name)
		{
			//	Conversion du nom du filtre en index pour le combo.
			for (int i=0; i<100; i++)
			{
				string n = Image.FilterIndexToName(i);
				if (n == null)  break;
				if (n == name)  return i;
			}
			return -1;
		}

		public static string FilterIndexToName(int index)
		{
			//	Conversion de l'index dans le combo en nom du filtre.
			switch (index)
			{
				case 0:   return "None";      // pas de filtrage
				case 1:   return "Bilinear";  // filtrage simple et rapide
				case 2:   return "Blackman";  // normal
				case 3:   return "Bicubic";   // flou
				case 4:   return "Sinc";      // accentué
				default:  return null;
			}
		}

		public static string FilterNameToText(string name)
		{
			//	Conversion du nom du filtre en texte clair pour l'utilisateur.
			//	Par exemple: "Bilinear" -> "Rapide (bilinéaire)"
			switch (name)
			{
				case "None":      return Res.Strings.Panel.Image.Filter.None;
				case "Bilinear":  return Res.Strings.Panel.Image.Filter.Bilinear;
				case "Blackman":  return Res.Strings.Panel.Image.Filter.Blackman;
				case "Bicubic":   return Res.Strings.Panel.Image.Filter.Bicubic;
				case "Sinc":      return Res.Strings.Panel.Image.Filter.Sinc;
				default:          return string.Format(Res.Strings.Panel.Image.Filter.Other, name);
			}
		}

		public static ImageFilter CategoryToFilter(DrawingContext context, int filterCategory, bool resampling)
		{
			//	Retourne le filtre défini dans un DrawingContext en fonction de la catégorie
			//	(-1=aucun, 0=groupe A, 1=groupe B) et de mode de redimensionnement.
			//	resampling = true lorsqu'on effectue une réduction, pour éviter les moirés.
			if (filterCategory < 0)
			{
				return NameToFilter("None", resampling);
			}
			else
			{
				return NameToFilter(context.GetImageNameFilter(filterCategory), resampling);
			}
		}

		protected static ImageFilter NameToFilter(string name, bool resampling)
		{
			//	Retourne le filtre en fonction du nom et du mode de redimensionnement.
			//	resampling = true lorsqu'on effectue une réduction, pour éviter les moirés.
			switch (name)
			{
				case "None":      return new ImageFilter(ImageFilteringMode.None);
				case "Bilinear":  return new ImageFilter(resampling ? ImageFilteringMode.ResamplingBilinear : ImageFilteringMode.Bilinear);
				case "Bicubic":   return new ImageFilter(resampling ? ImageFilteringMode.ResamplingBicubic : ImageFilteringMode.Bicubic);
				case "Spline16":  return new ImageFilter(resampling ? ImageFilteringMode.ResamplingSpline16 : ImageFilteringMode.Spline16);
				case "Spline36":  return new ImageFilter(resampling ? ImageFilteringMode.ResamplingSpline36 : ImageFilteringMode.Spline36);
				case "Kaiser":    return new ImageFilter(resampling ? ImageFilteringMode.ResamplingKaiser : ImageFilteringMode.Kaiser);
				case "Quadric":   return new ImageFilter(resampling ? ImageFilteringMode.ResamplingQuadric : ImageFilteringMode.Quadric);
				case "Catrom":    return new ImageFilter(resampling ? ImageFilteringMode.ResamplingCatrom : ImageFilteringMode.Catrom);
				case "Gaussian":  return new ImageFilter(resampling ? ImageFilteringMode.ResamplingGaussian : ImageFilteringMode.Gaussian);
				case "Bessel":    return new ImageFilter(resampling ? ImageFilteringMode.ResamplingBessel : ImageFilteringMode.Bessel);
				case "Mitchell":  return new ImageFilter(resampling ? ImageFilteringMode.ResamplingMitchell : ImageFilteringMode.Mitchell);
				case "Sinc":      return new ImageFilter(resampling ? ImageFilteringMode.ResamplingSinc : ImageFilteringMode.Sinc, 1.0);
				case "Lanczos":   return new ImageFilter(resampling ? ImageFilteringMode.ResamplingLanczos : ImageFilteringMode.Lanczos, 1.0);
				case "Blackman":  return new ImageFilter(resampling ? ImageFilteringMode.ResamplingBlackman : ImageFilteringMode.Blackman, 1.0);
				default:          return new ImageFilter(ImageFilteringMode.None);
			}
		}
		#endregion


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			base.GetObjectData(info, context);

			//	Si le dossier d'accès à l'image est le même que le dossier dans
			//	lequel est sérialisé le fichier, sérialise juste le nom (relatif).
			string filename = this.filename;
			if ( filename != "" &&
				 this.document.IoDirectory != "" &&
				 this.document.IoDirectory == System.IO.Path.GetDirectoryName(filename) )
			{
				filename = System.IO.Path.GetFileName(filename);
			}

			info.AddValue("Filename", filename);
			info.AddValue("ShortName", shortName);
			info.AddValue("Date", this.date);
			info.AddValue("InsideDoc", this.insideDoc);
			info.AddValue("FromClipboard", this.fromClipboard);
			info.AddValue("Rotation", this.rotation);
			info.AddValue("MirrorH", this.mirrorH);
			info.AddValue("MirrorV", this.mirrorV);
			info.AddValue("Homo", this.homo);
			info.AddValue("FilterCategory", this.filterCategory);

			info.AddValue("CropLeft", this.cropMargins.Left);
			info.AddValue("CropRight", this.cropMargins.Right);
			info.AddValue("CropBottom", this.cropMargins.Bottom);
			info.AddValue("CropTop", this.cropMargins.Top);
		}

		protected Image(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise la propriété.
			this.filename = info.GetString("Filename");
			this.mirrorH = info.GetBoolean("MirrorH");
			this.mirrorV = info.GetBoolean("MirrorV");
			this.homo = info.GetBoolean("Homo");

			if ( this.document.IsRevisionGreaterOrEqual(2, 0, 4) )
			{
				this.filterCategory = info.GetInt32("FilterCategory");
			}
			else
			{
				this.filterCategory = 1;
			}

			//	Si le nom de l'image ne contient pas de nom de dossier (nom relatif),
			//	ajoute le nom du dossier dans lequel est désérialisé le fichier
			//	(pour le rendre absolu).
			if ( this.filename != "" &&
				 this.document.IoDirectory != "" &&
				 System.IO.Path.GetDirectoryName(this.filename) == "" )
			{
				this.filename = string.Concat(this.document.IoDirectory, "\\", this.filename);
			}

			if ( this.document.IsRevisionGreaterOrEqual(2, 0, 1) )
			{
				this.shortName = info.GetString("ShortName");
				this.insideDoc = info.GetBoolean("InsideDoc");

				this.cropMargins.Left = info.GetDouble("CropLeft");
				this.cropMargins.Right = info.GetDouble("CropRight");
				this.cropMargins.Bottom = info.GetDouble("CropBottom");
				this.cropMargins.Top = info.GetDouble("CropTop");

				this.rotation = (Rotation) info.GetValue("Rotation", typeof(Rotation));
			}
			else
			{
				this.shortName = null;
				this.insideDoc = false;
				this.cropMargins = Margins.Zero;
				this.rotation = Rotation.Angle0;
			}

			if (this.document.IsRevisionGreaterOrEqual(2, 0, 5))
			{
				this.date = (System.DateTime) info.GetValue("Date", typeof(System.DateTime));
			}

			if (this.document.IsRevisionGreaterOrEqual(2, 1, 10))
			{
				this.fromClipboard = info.GetBoolean("FromClipboard");
			}
			else
			{
				this.fromClipboard = false;
			}

			if (this.date.Ticks == 0)
			{
				//	Met une date spéciale différente de System.DateTime.MinValue pour les anciens fichiers.
				this.date = System.DateTime.MinValue.AddSeconds(1);
			}
		}
		#endregion


		private string					filename;
		private string					shortName;
		private System.DateTime			date;
		private bool					insideDoc;
		private bool					fromClipboard;
		private Rotation				rotation;
		private bool					mirrorH;
		private bool					mirrorV;
		private bool					homo;
		private int						filterCategory;
		private Margins					cropMargins;
		private CropLogic				cropLogic;
		private IconButton				popupInterfaceFillModeButton;
		private IconButton				popupInterfaceRotateButton;
		private HSlider					popupInterfaceSlider;
	}
}
