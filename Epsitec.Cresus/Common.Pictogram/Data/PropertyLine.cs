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
			this.limit = 5.0;
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

		[XmlAttribute]
		public double Limit
		{
			get { return this.limit; }
			set { this.limit = value; }
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
			p.Cap   = this.cap;
			p.Join  = this.join;
			p.Limit = this.limit;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyLine p = property as PropertyLine;
			if ( p.Width != this.width )  return false;
			if ( p.Cap   != this.cap   )  return false;
			if ( p.Join  != this.join  )  return false;
			if ( p.Limit != this.limit )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelLine();
		}


		// Engraisse la bbox selon le trait.
		public void InflateBoundingBox(ref Drawing.Rectangle bbox)
		{
			if ( this.join == Drawing.JoinStyle.Miter )
			{
				bbox.Inflate(this.width*0.5*this.limit);
			}
			else if ( this.cap == Drawing.CapStyle.Square )
			{
				bbox.Inflate(this.width*0.5*1.415);  // augmente de racine de 2
			}
			else
			{
				bbox.Inflate(this.width*0.5);
			}
		}


		protected double				width;
		protected Drawing.CapStyle		cap;
		protected Drawing.JoinStyle		join;
		protected double				limit;  // longueur (et non angle) !
	}
}
