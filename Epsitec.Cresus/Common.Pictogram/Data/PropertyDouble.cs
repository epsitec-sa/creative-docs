using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyDouble représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyDouble : AbstractProperty
	{
		public PropertyDouble()
		{
			this.doubleValue = 0.0;
		}

		[XmlAttribute]
		public double Value
		{
			get
			{
				return this.doubleValue;
			}

			set
			{
				this.doubleValue = value;
			}
		}

		[XmlIgnore]
		public double MinRange
		{
			get
			{
				return this.minRange;
			}

			set
			{
				this.minRange = value;
			}
		}

		[XmlIgnore]
		public double MaxRange
		{
			get
			{
				return this.maxRange;
			}

			set
			{
				this.maxRange = value;
			}
		}

		[XmlIgnore]
		public double Step
		{
			get
			{
				return this.step;
			}

			set
			{
				this.step = value;
			}
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyDouble p = property as PropertyDouble;
			p.Value = this.doubleValue;
			p.MinRange = this.minRange;
			p.MaxRange = this.maxRange;
			p.Step = this.step;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyDouble p = property as PropertyDouble;
			if ( p.Value != this.doubleValue )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel()
		{
			return new PanelDouble();
		}

		protected double			doubleValue = 1;
		protected double			minRange = 0;
		protected double			maxRange = 10;
		protected double			step = 1;
	}
}
