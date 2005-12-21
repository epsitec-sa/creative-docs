using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	public enum ArcType
	{
		Full,
		Open,
		Close,
		Pie,
	}

	/// <summary>
	/// La classe PropertyArc représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyArc : AbstractProperty
	{
		public PropertyArc()
		{
			this.arcType       = ArcType.Full;
			this.startingAngle =  90.0;
			this.endingAngle   = 360.0;
		}

		[XmlAttribute]
		public ArcType ArcType
		{
			get
			{
				return this.arcType;
			}

			set
			{
				if ( this.arcType != value )
				{
					this.arcType = value;
					this.OnChanged();
				}
			}
		}

		[XmlAttribute]
		public double StartingAngle
		{
			get
			{
				return this.startingAngle;
			}
			
			set
			{
				if ( this.startingAngle != value )
				{
					this.startingAngle = value;
					this.OnChanged();
				}
			}
		}

		[XmlAttribute]
		public double EndingAngle
		{
			get
			{
				return this.endingAngle;
			}
			
			set
			{
				if ( this.endingAngle != value )
				{
					this.endingAngle = value;
					this.OnChanged();
				}
			}
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
			PropertyArc p = property as PropertyArc;
			p.ArcType       = this.arcType;
			p.StartingAngle = this.startingAngle;
			p.EndingAngle   = this.endingAngle;
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			PropertyArc p = property as PropertyArc;
			if ( p.ArcType       != this.arcType       )  return false;
			if ( p.StartingAngle != this.startingAngle )  return false;
			if ( p.EndingAngle   != this.endingAngle   )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			return new PanelArc(drawer);
		}


		protected ArcType				arcType;
		protected double				startingAngle;
		protected double				endingAngle;
	}
}
