using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plantée lors de la désérialisation.
	public enum JustifHorizontal
	{
		None    = 0,
		Left    = 1,			// |abc  |
		Center  = 2,			// | abc |
		Right   = 3,			// |  abc|
		Justif  = 4,			// |a b c| sauf la dernière ligne
		All     = 5,			// |a b c| avec la dernière ligne
		Stretch = 6,			// |abc| étendu pour ObjectTextLine
	}

	public enum JustifVertical
	{
		None   = 0,
		Top    = 1,				// en haut
		Center = 2,				// au milieu
		Bottom = 3,				// en bas
	}

	public enum JustifOrientation
	{
		None        = 0,
		LeftToRight = 1,		// -> (normal)
		BottomToTop = 2,		// ^
		RightToLeft = 3,		// <-
		TopToBottom = 4,		// v
	}

	/// <summary>
	/// La classe Justif représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Justif : Abstract
	{
		public Justif(Document document, Type type) : base(document, type)
		{
			this.horizontal  = JustifHorizontal.Left;
			this.vertical    = JustifVertical.Top;
			this.orientation = JustifOrientation.LeftToRight;
			this.marginH     = 0.2;
			this.marginV     = 0.1;
			this.offsetV     = 0.0;
		}

		public JustifHorizontal Horizontal
		{
			get
			{
				return this.horizontal;
			}
			
			set
			{
				if ( this.horizontal != value )
				{
					this.NotifyBefore();
					this.horizontal = value;
					this.NotifyAfter();
				}
			}
		}

		public JustifVertical Vertical
		{
			get
			{
				return this.vertical;
			}
			
			set
			{
				if ( this.vertical != value )
				{
					this.NotifyBefore();
					this.vertical = value;
					this.NotifyAfter();
				}
			}
		}

		public JustifOrientation Orientation
		{
			get
			{
				return this.orientation;
			}
			
			set
			{
				if ( this.orientation != value )
				{
					this.NotifyBefore();
					this.orientation = value;
					this.NotifyAfter();
				}
			}
		}

		public double MarginH
		{
			get
			{
				return this.marginH;
			}
			
			set
			{
				if ( this.marginH != value )
				{
					this.NotifyBefore();
					this.marginH = value;
					this.NotifyAfter();
				}
			}
		}

		public double MarginV
		{
			get
			{
				return this.marginV;
			}
			
			set
			{
				if ( this.marginV != value )
				{
					this.NotifyBefore();
					this.marginV = value;
					this.NotifyAfter();
				}
			}
		}

		public double OffsetV
		{
			get
			{
				return this.offsetV;
			}
			
			set
			{
				if ( this.offsetV != value )
				{
					this.NotifyBefore();
					this.offsetV = value;
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
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Justif p = property as Justif;
			p.horizontal  = this.horizontal;
			p.vertical    = this.vertical;
			p.orientation = this.orientation;
			p.marginH     = this.marginH;
			p.marginV     = this.marginV;
			p.offsetV     = this.offsetV;
		}

		// Compare deux propriétés.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Justif p = property as Justif;
			if ( p.horizontal  != this.horizontal  )  return false;
			if ( p.vertical    != this.vertical    )  return false;
			if ( p.orientation != this.orientation )  return false;
			if ( p.marginH     != this.marginH     )  return false;
			if ( p.marginV     != this.marginV     )  return false;
			if ( p.offsetV     != this.offsetV     )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override Panels.Abstract CreatePanel(Document document)
		{
			return new Panels.Justif(document);
		}


		// Diminue la boîte qui contient le texte en fonction des marges.
		// Retourne false si elle est trop petite.
		public bool DeflateBox(ref Point pbl, ref Point pbr, ref Point ptl, ref Point ptr)
		{
			double mh = this.marginH;
			double mv = this.marginV;

			double offset = 0;
			if ( this.vertical == JustifVertical.Center )
			{
				offset = this.offsetV * Point.Distance(pbl,ptl);
				mv = 0;
			}

			if ( Point.Distance(pbl,pbr) <= mh*2 )  return false;
			if ( Point.Distance(pbl,ptl) <= mv*2 )  return false;

			pbl = Point.Move(pbl, pbr, mh);
			ptl = Point.Move(ptl, ptr, mh);
			pbr = Point.Move(pbr, pbl, mh);
			ptr = Point.Move(ptr, ptl, mh);
			pbl = Point.Move(pbl, ptl, mv+offset);
			pbr = Point.Move(pbr, ptr, mv+offset);
			ptl = Point.Move(ptl, pbl, mv-offset);
			ptr = Point.Move(ptr, pbr, mv-offset);

			return true;
		}


		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Horizontal", this.horizontal);
			info.AddValue("Vertical", this.vertical);
			info.AddValue("Orientation", this.orientation);
			info.AddValue("MarginH", this.marginH);
			info.AddValue("MarginV", this.marginV);
			info.AddValue("OffsetV", this.offsetV);
		}

		// Constructeur qui désérialise la propriété.
		protected Justif(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.horizontal = (JustifHorizontal) info.GetValue("Horizontal", typeof(JustifHorizontal));
			this.vertical = (JustifVertical) info.GetValue("Vertical", typeof(JustifVertical));
			this.orientation = (JustifOrientation) info.GetValue("Orientation", typeof(JustifOrientation));
			this.marginH = info.GetDouble("MarginH");
			this.marginV = info.GetDouble("MarginV");
			this.offsetV = info.GetDouble("OffsetV");
		}
		#endregion

	
		protected JustifHorizontal		horizontal;
		protected JustifVertical		vertical;
		protected JustifOrientation		orientation;
		protected double				marginH;
		protected double				marginV;
		protected double				offsetV;
	}
}
