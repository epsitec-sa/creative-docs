using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Image représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Image : Abstract
	{
		public Image(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			this.filename  = "";
			this.shortName = "";
			this.mirrorH   = false;
			this.mirrorV   = false;
			this.homo      = true;
			this.filter    = true;
			this.reload    = false;
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

		public string ShortName
		{
			get
			{
				return this.shortName;
			}

			set
			{
				if (this.shortName != value)
				{
					this.NotifyBefore();
					this.shortName = value;
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

		public void ReloadDo()
		{
			this.document.Modifier.OpletQueueEnable = false;
			this.NotifyBefore();
			this.reload = true;
			this.NotifyAfter();
			this.document.Modifier.OpletQueueEnable = true;
		}

		public void ReloadReset()
		{
			this.reload = false;
		}

		public bool Reload
		{
			get
			{
				return this.reload;
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
			p.filename  = this.filename;
			p.shortName = this.shortName;
			p.mirrorH   = this.mirrorH;
			p.mirrorV   = this.mirrorV;
			p.homo      = this.homo;
			p.filter    = this.filter;
			p.reload    = this.reload;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Image p = property as Image;
			if ( p.filename  != this.filename  )  return false;
			if ( p.shortName != this.shortName )  return false;
			if ( p.mirrorH   != this.mirrorH   )  return false;
			if ( p.mirrorV   != this.mirrorV   )  return false;
			if ( p.homo      != this.homo      )  return false;
			if ( p.filter    != this.filter    )  return false;
			if ( p.reload    != this.reload    )  return false;

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
			info.AddValue("ShortName", this.shortName);
			info.AddValue("MirrorH", this.mirrorH);
			info.AddValue("MirrorV", this.mirrorV);
			info.AddValue("Homo", this.homo);
			info.AddValue("Filter", this.filter);
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
			}
			else
			{
				this.shortName = "";
			}
		}
		#endregion

	
		protected string			filename;
		protected string			shortName;
		protected bool				mirrorH;
		protected bool				mirrorV;
		protected bool				homo;
		protected bool				filter;
		protected bool				reload;
	}
}
