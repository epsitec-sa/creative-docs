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
			this.filename = "";
			this.mirrorH  = false;
			this.mirrorV  = false;
			this.homo     = true;
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

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override Panels.Abstract CreatePanel(Document document)
		{
			return new Panels.Image(document);
		}

		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Filename", this.filename);
			info.AddValue("MirrorH", this.mirrorH);
			info.AddValue("MirrorV", this.mirrorV);
			info.AddValue("Homo", this.homo);
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected Image(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filename = info.GetString("Filename");
			this.mirrorH = info.GetBoolean("MirrorH");
			this.mirrorV = info.GetBoolean("MirrorV");
			this.homo = info.GetBoolean("Homo");
		}
		#endregion

	
		protected string			filename;
		protected bool				mirrorH;
		protected bool				mirrorV;
		protected bool				homo;
	}
}
