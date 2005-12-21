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
			this.fontName  = "Arial";
			this.fontSize  = 1.0;
			this.fontColor = Drawing.Color.FromBrightness(0);
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

		public string GetListName()
		{
			//	Détermine le nom de la propriété dans la liste (Lister).
			return this.fontName;
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
			PropertyFont p = property as PropertyFont;
			p.FontName  = this.fontName;
			p.FontSize  = this.fontSize;
			p.FontColor = this.fontColor;
		}

		public override bool Compare(AbstractProperty property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			PropertyFont p = property as PropertyFont;
			if ( p.FontName  != this.fontName  )  return false;
			if ( p.FontSize  != this.fontSize  )  return false;
			if ( p.FontColor != this.fontColor )  return false;

			return true;
		}

		public override AbstractPanel CreatePanel(Drawer drawer)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			return new PanelFont(drawer);
		}


		public Drawing.Font GetFont()
		{
			//	Retourne la fonte à utiliser.
			return Drawing.Font.GetFont(this.fontName, "Regular");
		}


		protected string				fontName;
		protected double				fontSize;

		[XmlAttribute]
		protected Drawing.Color			fontColor;
	}
}
