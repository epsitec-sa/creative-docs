using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyImage représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyImage : AbstractProperty
	{
		public PropertyImage()
		{
			this.filename = "";
			this.mirrorH  = false;
			this.mirrorV  = false;
			this.homo     = true;
		}

		[XmlAttribute]
		public string Filename
		{
			get { return this.filename; }
			set { this.filename = value; }
		}

		[XmlAttribute]
		public bool MirrorH
		{
			get { return this.mirrorH; }
			set { this.mirrorH = value; }
		}

		[XmlAttribute]
		public bool MirrorV
		{
			get { return this.mirrorV; }
			set { this.mirrorV = value; }
		}

		[XmlAttribute]
		public bool Homo
		{
			get { return this.homo; }
			set { this.homo = value; }
		}

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		[XmlIgnore]
		public override bool AlterBoundingBox
		{
			get { return true; }
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyImage p = property as PropertyImage;
			p.Filename = this.filename;
			p.MirrorH  = this.mirrorH;
			p.MirrorV  = this.mirrorV;
			p.Homo     = this.homo;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyImage p = property as PropertyImage;
			if ( p.Filename != this.filename )  return false;
			if ( p.MirrorH  != this.mirrorH  )  return false;
			if ( p.MirrorV  != this.mirrorV  )  return false;
			if ( p.Homo     != this.homo     )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelImage();
		}

		protected string			filename;
		protected bool				mirrorH;
		protected bool				mirrorV;
		protected bool				homo;
	}
}
