using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe PropertyFont représente une propriété d'un objet graphique.
	/// </summary>
	public class PropertyFont : AbstractProperty
	{
		public PropertyFont()
		{
			this.fontName    = "Arial";
			this.fontOptical = "Regular";
			this.fontSize    = 1.0;
			this.fontColor   = Drawing.Color.FromBrightness(0);
		}

		[XmlAttribute]
		public string FontName
		{
			get
			{
				return this.fontName;
			}

			set
			{
				if ( this.fontName != value )
				{
					this.fontName = value;
					this.OnChanged();
				}
			}
		}

		[XmlAttribute]
		public string FontOptical
		{
			get
			{
				return this.fontOptical;
			}

			set
			{
				if ( this.fontOptical != value )
				{
					this.fontOptical = value;
					this.OnChanged();
				}
			}
		}

		[XmlAttribute]
		public double FontSize
		{
			get
			{
				return this.fontSize;
			}

			set
			{
				if ( this.fontSize != value )
				{
					this.fontSize = value;
					this.OnChanged();
				}
			}
		}

		public Drawing.Color FontColor
		{
			get
			{
				return this.fontColor;
			}

			set
			{
				if ( this.fontColor != value )
				{
					this.fontColor = value;
					this.OnChanged();
				}
			}
		}

		// Détermine le nom de la propriété dans la liste (Lister).
		public string GetListName()
		{
			return this.fontName;
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
			PropertyFont p = property as PropertyFont;
			p.FontName    = this.fontName;
			p.FontOptical = this.fontOptical;
			p.FontSize    = this.fontSize;
			p.FontColor   = this.fontColor;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyFont p = property as PropertyFont;
			if ( p.FontName    != this.fontName    )  return false;
			if ( p.FontOptical != this.fontOptical )  return false;
			if ( p.FontSize    != this.fontSize    )  return false;
			if ( p.FontColor   != this.fontColor   )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			return new PanelFont(drawer);
		}


		// Retourne la fonte à utiliser.
		public Drawing.Font GetFont()
		{
			return Drawing.Font.GetFont(this.fontName, this.fontOptical);
		}


		protected string				fontName;
		protected string				fontOptical;
		protected double				fontSize;

		[XmlAttribute]
		protected Drawing.Color			fontColor;
	}
}
