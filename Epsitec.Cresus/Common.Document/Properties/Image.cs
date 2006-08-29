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

		protected override void Initialise()
		{
			this.filename  = "";
			this.shortName = "";
			this.insideDoc = true;
			this.rotation  = Rotation.Angle0;
			this.mirrorH   = false;
			this.mirrorV   = false;
			this.homo      = true;
			this.filter    = true;
			this.cropMargins = Margins.Zero;
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

		public bool Filter
		{
			get
			{
				return this.filter;
			}
			
			set
			{
				if ( this.filter != value )
				{
					this.NotifyBefore();
					this.filter = value;
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
			p.filename    = this.filename;
			p.shortName   = this.shortName;
			p.insideDoc   = this.insideDoc;
			p.rotation    = this.rotation;
			p.mirrorH     = this.mirrorH;
			p.mirrorV     = this.mirrorV;
			p.homo        = this.homo;
			p.filter      = this.filter;
			p.cropMargins = this.cropMargins;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Image p = property as Image;
			if ( p.filename    != this.filename    )  return false;
			if ( p.shortName   != this.shortName   )  return false;
			if ( p.insideDoc   != this.insideDoc   )  return false;
			if ( p.rotation    != this.rotation    )  return false;
			if ( p.mirrorH     != this.mirrorH     )  return false;
			if ( p.mirrorV     != this.mirrorV     )  return false;
			if ( p.homo        != this.homo        )  return false;
			if ( p.filter      != this.filter      )  return false;
			if ( p.cropMargins != this.cropMargins )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Image(document);
		}

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
			info.AddValue("Filter", this.filter);

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

			if ( this.document.IsRevisionGreaterOrEqual(1,2,4) )
			{
				this.filter = info.GetBoolean("Filter");
			}
			else
			{
				this.filter = true;
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
		protected bool				filter;
		protected Margins			cropMargins;
	}
}
