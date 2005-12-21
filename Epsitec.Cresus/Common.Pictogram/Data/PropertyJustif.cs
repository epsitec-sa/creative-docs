using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	public enum JustifHorizontal
	{
		None,
		Left,				// |abc  |
		Center,				// | abc |
		Right,				// |  abc|
		Justif,				// |a b c| sauf la dernière ligne
		All,				// |a b c| avec la dernière ligne
		Stretch,			// |abc| étendu pour ObjectTextLine
	}

	public enum JustifVertical
	{
		None,
		Top,				// en haut
		Center,				// au milieu
		Bottom,				// en bas
	}

	public enum JustifOrientation
	{
		None,
		LeftToRight,		// -> (normal)
		BottomToTop,		// ^
		RightToLeft,		// <-
		TopToBottom,		// v
	}

	/// <summary>
	/// La classe PropertyJustif représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyJustif : AbstractProperty
	{
		public PropertyJustif()
		{
			this.horizontal  = JustifHorizontal.Left;
			this.vertical    = JustifVertical.Top;
			this.orientation = JustifOrientation.LeftToRight;
			this.marginH     = 0.2;
			this.marginV     = 0.1;
			this.offsetV     = 0.0;
		}

		[XmlAttribute]
		public JustifHorizontal Horizontal
		{
			get { return this.horizontal; }
			set { this.horizontal = value; }
		}

		[XmlAttribute]
		public JustifVertical Vertical
		{
			get { return this.vertical; }
			set { this.vertical = value; }
		}

		[XmlAttribute]
		public JustifOrientation Orientation
		{
			get { return this.orientation; }
			set { this.orientation = value; }
		}

		[XmlAttribute]
		public double MarginH
		{
			get { return this.marginH; }
			set { this.marginH = value; }
		}

		[XmlAttribute]
		public double MarginV
		{
			get { return this.marginV; }
			set { this.marginV = value; }
		}

		[XmlAttribute]
		public double OffsetV
		{
			get { return this.offsetV; }
			set { this.offsetV = value; }
		}

		[XmlIgnore]
		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propriété modifie la bbox de l'objet.
			get { return true; }
		}

		public override void CopyTo(AbstractProperty property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			PropertyJustif p = property as PropertyJustif;
			p.Horizontal  = this.horizontal;
			p.Vertical    = this.vertical;
			p.Orientation = this.orientation;
			p.MarginH     = this.marginH;
			p.MarginV     = this.marginV;
			p.OffsetV     = this.offsetV;
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			PropertyJustif p = property as PropertyJustif;
			if ( p.Horizontal  != this.horizontal  )  return false;
			if ( p.Vertical    != this.vertical    )  return false;
			if ( p.Orientation != this.orientation )  return false;
			if ( p.MarginH     != this.marginH     )  return false;
			if ( p.MarginV     != this.marginV     )  return false;
			if ( p.OffsetV     != this.offsetV     )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			return new PanelJustif(drawer);
		}


		public bool DeflateBox(ref Drawing.Point pbl, ref Drawing.Point pbr, ref Drawing.Point ptl, ref Drawing.Point ptr)
		{
			//	Diminue la boîte qui contient le texte en fonction des marges.
			//	Retourne false si elle est trop petite.
			double mh = this.marginH;
			double mv = this.marginV;

			double offset = 0;
			if ( this.vertical == JustifVertical.Center )
			{
				offset = this.offsetV * Drawing.Point.Distance(pbl,ptl);
				mv = 0;
			}

			if ( Drawing.Point.Distance(pbl,pbr) <= mh*2 )  return false;
			if ( Drawing.Point.Distance(pbl,ptl) <= mv*2 )  return false;

			pbl = Drawing.Point.Move(pbl, pbr, mh);
			ptl = Drawing.Point.Move(ptl, ptr, mh);
			pbr = Drawing.Point.Move(pbr, pbl, mh);
			ptr = Drawing.Point.Move(ptr, ptl, mh);
			pbl = Drawing.Point.Move(pbl, ptl, mv+offset);
			pbr = Drawing.Point.Move(pbr, ptr, mv+offset);
			ptl = Drawing.Point.Move(ptl, pbl, mv-offset);
			ptr = Drawing.Point.Move(ptr, pbr, mv-offset);

			return true;
		}


		protected JustifHorizontal		horizontal;
		protected JustifVertical		vertical;
		protected JustifOrientation		orientation;
		protected double				marginH;
		protected double				marginV;
		protected double				offsetV;
	}
}
