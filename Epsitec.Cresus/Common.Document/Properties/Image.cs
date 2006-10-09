using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

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

		protected override void Initialize()
		{
			this.filename       = "";
			this.shortName      = "";
			this.insideDoc      = true;
			this.rotation       = Rotation.Angle0;
			this.mirrorH        = false;
			this.mirrorV        = false;
			this.homo           = true;
			this.filterCategory = 0;  // catégorie A
			this.cropMargins    = Margins.Zero;
		}

		public string Filename
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

		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			Image p = property as Image;
			p.filename       = this.filename;
			p.shortName      = this.shortName;
			p.insideDoc      = this.insideDoc;
			p.rotation       = this.rotation;
			p.mirrorH        = this.mirrorH;
			p.mirrorV        = this.mirrorV;
			p.homo           = this.homo;
			p.filterCategory = this.filterCategory;
			p.cropMargins    = this.cropMargins;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Image p = property as Image;
			if ( p.filename       != this.filename       )  return false;
			if ( p.shortName      != this.shortName      )  return false;
			if ( p.insideDoc      != this.insideDoc      )  return false;
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
		public static Magick.FilterType FilterToMagick(ImageFilter filter)
		{
			//	Conversion du type de filtre pour la librairie Magick.
			//	TODO: à vérifier !!!
			switch (filter.Mode)
			{
				case ImageFilteringMode.None:      return Magick.FilterType.Point;
				case ImageFilteringMode.Bilinear:  return Magick.FilterType.Box;
				case ImageFilteringMode.Bicubic:   return Magick.FilterType.Cubic;
				case ImageFilteringMode.Quadric:   return Magick.FilterType.Quadratic;
				case ImageFilteringMode.Blackman:  return Magick.FilterType.Blackman;
				case ImageFilteringMode.Gaussian:  return Magick.FilterType.Gaussian;
				case ImageFilteringMode.Catrom:    return Magick.FilterType.Catrom;
				case ImageFilteringMode.Mitchell:  return Magick.FilterType.Mitchell;
				case ImageFilteringMode.Lanczos:   return Magick.FilterType.Lanczos;
				case ImageFilteringMode.Bessel:    return Magick.FilterType.Bessel;
				case ImageFilteringMode.Sinc:      return Magick.FilterType.Sinc;
				default:                           return Magick.FilterType.Cubic;
			}
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
				case 1:   return "Bilinear";  // filtrage simple et rapide (mauvais si réduction)
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
				case "Bilinear":  return new ImageFilter(ImageFilteringMode.Bilinear);
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
			info.AddValue("InsideDoc", this.insideDoc);
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
				this.filename = this.document.IoDirectory + "\\" + this.filename;
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
		}
		#endregion

	
		protected string			filename;
		protected string			shortName;
		protected bool				insideDoc;
		protected Rotation			rotation;
		protected bool				mirrorH;
		protected bool				mirrorV;
		protected bool				homo;
		protected int				filterCategory;
		protected Margins			cropMargins;
	}
}
