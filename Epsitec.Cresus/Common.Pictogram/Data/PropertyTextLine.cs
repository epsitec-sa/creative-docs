using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyTextLine repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	public class PropertyTextLine : AbstractProperty
	{
		public PropertyTextLine()
		{
			this.horizontal = JustifHorizontal.Left;
			this.offset     = 0.0;
			this.add        = 0.0;
		}

		[XmlAttribute]
		public JustifHorizontal Horizontal
		{
			get { return this.horizontal; }
			set { this.horizontal = value; }
		}

		[XmlAttribute]
		public double Offset
		{
			get { return this.offset; }
			set { this.offset = value; }
		}

		[XmlAttribute]
		public double Add
		{
			get { return this.add; }
			set { this.add = value; }
		}

		[XmlIgnore]
		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propri�t� modifie la bbox de l'objet.
			get { return true; }
		}

		public override void CopyTo(AbstractProperty property)
		{
			//	Effectue une copie de la propri�t�.
			base.CopyTo(property);
			PropertyTextLine p = property as PropertyTextLine;
			p.Horizontal = this.horizontal;
			p.Offset     = this.offset;
			p.Add        = this.add;
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propri�t�s.
			if ( !base.Compare(property) )  return false;

			PropertyTextLine p = property as PropertyTextLine;
			if ( p.Horizontal != this.horizontal )  return false;
			if ( p.Offset     != this.offset     )  return false;
			if ( p.Add        != this.add        )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Cr�e le panneau permettant d'�diter la propri�t�.
			return new PanelTextLine(drawer);
		}


		protected JustifHorizontal		horizontal;
		protected double				offset;
		protected double				add;
	}
}
