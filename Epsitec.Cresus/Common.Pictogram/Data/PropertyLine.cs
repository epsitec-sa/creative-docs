using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyLine représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyLine : AbstractProperty
	{
		public PropertyLine()
		{
			this.width = 1.0;
			this.cap   = Drawing.CapStyle.Round;
			this.join  = Drawing.JoinStyle.Round;
		}

		[XmlAttribute]
		public double Width
		{
			get { return this.width; }
			set { this.width = value; }
		}

		[XmlAttribute]
		public Drawing.CapStyle Cap
		{
			get { return this.cap; }
			set { this.cap = value; }
		}

		[XmlAttribute]
		public Drawing.JoinStyle Join
		{
			get { return this.join; }
			set { this.join = value; }
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
			PropertyLine p = property as PropertyLine;
			p.Width = this.width;
			p.Cap = this.cap;
			p.Join = this.join;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyLine p = property as PropertyLine;
			if ( p.Width != this.width )  return false;
			if ( p.Cap   != this.cap   )  return false;
			if ( p.Join  != this.join  )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelLine();
		}

		protected double				width;
		protected Drawing.CapStyle		cap;
		protected Drawing.JoinStyle		join;
	}
}
