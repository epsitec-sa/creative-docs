using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Image repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Image : Abstract
	{
		public Image(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			this.filename = "";
			this.mirrorH  = false;
			this.mirrorV  = false;
			this.homo     = true;
			this.filter   = true;
			this.reload   = false;
		}

		public string Filename
		{
			get
			{
				return this.filename;
			}
			
			set
			{
				if ( this.filename != value )
				{
					this.NotifyBefore();
					this.filename = value;
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

		public bool Reload
		{
			get
			{
				return this.reload;
			}
			
			set
			{
				if ( this.reload != value )
				{
					if ( value )
					{
						this.NotifyBefore(false);
						this.reload = value;
						this.NotifyAfter(false);
					}
					else
					{
						this.reload = value;
					}
				}
			}
		}

		// Donne le petit texte pour les �chantillons.
		public override string SampleText
		{
			get
			{
				return Misc.ExtractName(this.filename);
			}
		}


		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}

		// Effectue une copie de la propri�t�.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Image p = property as Image;
			p.filename = this.filename;
			p.mirrorH  = this.mirrorH;
			p.mirrorV  = this.mirrorV;
			p.homo     = this.homo;
			p.filter   = this.filter;
			p.reload   = this.reload;
		}

		// Compare deux propri�t�s.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Image p = property as Image;
			if ( p.filename != this.filename )  return false;
			if ( p.mirrorH  != this.mirrorH  )  return false;
			if ( p.mirrorV  != this.mirrorV  )  return false;
			if ( p.homo     != this.homo     )  return false;
			if ( p.filter   != this.filter   )  return false;
			if ( p.reload   != this.reload   )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override Panels.Abstract CreatePanel(Document document)
		{
			Panels.Abstract.StaticDocument = document;
			return new Panels.Image(document);
		}

		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			// Si le dossier d'acc�s � l'image est le m�me que le dossier dans
			// lequel est s�rialis� le fichier, s�rialise juste le nom (relatif).
			string filename = this.filename;
			if ( filename != "" &&
				 this.document.IoDirectory != "" &&
				 this.document.IoDirectory == System.IO.Path.GetDirectoryName(filename) )
			{
				filename = System.IO.Path.GetFileName(filename);
			}

			info.AddValue("Filename", filename);
			info.AddValue("MirrorH", this.mirrorH);
			info.AddValue("MirrorV", this.mirrorV);
			info.AddValue("Homo", this.homo);
			info.AddValue("Filter", this.filter);
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected Image(SerializationInfo info, StreamingContext context) : base(info, context)
		{
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

			// Si le nom de l'image ne contient pas de nom de dossier (nom relatif),
			// ajoute le nom du dossier dans lequel est d�s�rialis� le fichier
			// (pour le rendre absolu).
			if ( this.filename != "" &&
				 this.document.IoDirectory != "" &&
				 System.IO.Path.GetDirectoryName(this.filename) == "" )
			{
				this.filename = this.document.IoDirectory + "\\" + this.filename;
			}
		}
		#endregion

	
		protected string			filename;
		protected bool				mirrorH;
		protected bool				mirrorV;
		protected bool				homo;
		protected bool				filter;
		protected bool				reload;
	}
}
