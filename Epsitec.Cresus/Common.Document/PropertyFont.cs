using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe PropertyFont représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class PropertyFont : AbstractProperty
	{
		public PropertyFont(Document document) : base(document)
		{
			this.fontName  = "Arial";
			this.fontSize  = 1.0;
			this.fontColor = Color.FromBrightness(0);
		}

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
					this.NotifyBefore();
					this.fontName = value;
					this.NotifyAfter();
				}
			}
		}

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
					this.NotifyBefore();
					this.fontSize = value;
					this.NotifyAfter();
				}
			}
		}

		public Color FontColor
		{
			get
			{
				return this.fontColor;
			}

			set
			{
				if ( this.fontColor != value )
				{
					this.NotifyBefore();
					this.fontColor = value;
					this.NotifyAfter();
				}
			}
		}

		// Détermine le nom de la propriété dans la liste (Lister).
		public string GetListName()
		{
			return this.fontName;
		}

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyFont p = property as PropertyFont;
			p.fontName  = this.fontName;
			p.fontSize  = this.fontSize;
			p.fontColor = this.fontColor;
		}

		// Compare deux propriétés.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyFont p = property as PropertyFont;
			if ( p.fontName  != this.fontName  )  return false;
			if ( p.fontSize  != this.fontSize  )  return false;
			if ( p.fontColor != this.fontColor )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override AbstractPanel CreatePanel(Document document)
		{
			return new PanelFont(document);
		}


		// Retourne la fonte à utiliser.
		public Font GetFont()
		{
			return Font.GetFont(this.fontName, "Regular");
		}


		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("FontName", this.fontName);
			info.AddValue("FontSize", this.fontSize);
			info.AddValue("FontColor", this.fontColor);
		}

		// Constructeur qui désérialise la propriété.
		protected PropertyFont(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fontName = info.GetString("FontName");
			this.fontSize = info.GetDouble("FontSize");
			this.fontColor = (Color) info.GetValue("FontColor", typeof(Color));
		}
		#endregion

	
		protected string				fontName;
		protected double				fontSize;
		protected Color					fontColor;
	}
}
