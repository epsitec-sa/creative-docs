using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe PropertyImage représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class PropertyImage : AbstractProperty
	{
		public PropertyImage(Document document) : base(document)
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

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyImage p = property as PropertyImage;
			p.filename = this.filename;
			p.mirrorH  = this.mirrorH;
			p.mirrorV  = this.mirrorV;
			p.homo     = this.homo;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyImage p = property as PropertyImage;
			if ( p.filename != this.filename )  return false;
			if ( p.mirrorH  != this.mirrorH  )  return false;
			if ( p.mirrorV  != this.mirrorV  )  return false;
			if ( p.homo     != this.homo     )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel(Document document)
		{
			return new PanelImage(document);
		}

		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Filename", this.filename);
			info.AddValue("MirrorH", this.mirrorH);
			info.AddValue("MirrorV", this.mirrorV);
			info.AddValue("Homo", this.homo);
		}

		// Constructeur qui désérialise la propriété.
		protected PropertyImage(SerializationInfo info, StreamingContext context) : base(info, context)
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
