using Epsitec.Common.Widgets;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ArrayCell est la classe qui peuple l'objet ObjectArray.
	/// </summary>
	public class ArrayCell
	{
		public ArrayCell()
		{
		}

		[XmlIgnore]
		public bool Selected
		{
			get { return this.selected; }
			set { this.selected = value; }
		}

		[XmlIgnore]
		public TextLayout TextLayout
		{
			get { return this.textLayout; }
			set { this.textLayout = value; }
		}

		[XmlIgnore]
		public Drawing.Point BottomLeft
		{
			get { return this.bottomLeft; }
			set { this.bottomLeft = value; }
		}

		[XmlIgnore]
		public Drawing.Point BottomRight
		{
			get { return this.bottomRight; }
			set { this.bottomRight = value; }
		}

		[XmlIgnore]
		public Drawing.Point TopLeft
		{
			get { return this.topLeft; }
			set { this.topLeft = value; }
		}

		[XmlIgnore]
		public Drawing.Point TopRight
		{
			get { return this.topRight; }
			set { this.topRight = value; }
		}

		public PropertyLine LeftLine
		{
			get { return this.leftLine; }
			set { this.leftLine = value; }
		}

		public PropertyLine BottomLine
		{
			get { return this.bottomLine; }
			set { this.bottomLine = value; }
		}

		public PropertyColor LeftColor
		{
			get { return this.leftColor; }
			set { this.leftColor = value; }
		}

		public PropertyColor BottomColor
		{
			get { return this.bottomColor; }
			set { this.bottomColor = value; }
		}

		public PropertyColor BackColor
		{
			get { return this.backColor; }
			set { this.backColor = value; }
		}

		public PropertyString TextString
		{
			get { return this.textString; }
			set { this.textString = value; }
		}

		public PropertyFont TextFont
		{
			get { return this.textFont; }
			set { this.textFont = value; }
		}

		public PropertyJustif TextJustif
		{
			get { return this.textJustif; }
			set { this.textJustif = value; }
		}

		public void SetProperty(AbstractProperty property, int sub)
		{
			AbstractProperty actual = this.Property(property.Type, sub);
			if ( actual == null )  return;
			property.CopyTo(actual);
		}

		public AbstractProperty Property(PropertyType type, int sub)
		{
			AbstractProperty p = null;
			if ( sub < 2 )
			{
				switch ( type )
				{
					case PropertyType.LineMode:      p = (sub==0) ? this.leftLine   : this.bottomLine;   break;
					case PropertyType.LineColor:     p = (sub==0) ? this.leftColor  : this.bottomColor;  break;
					case PropertyType.BackColor:     p = (sub==0) ? this.backColor  : null;              break;
					case PropertyType.TextString:    p = (sub==0) ? this.textString : null;              break;
					case PropertyType.TextFont:      p = (sub==0) ? this.textFont   : null;              break;
					case PropertyType.TextJustif:    p = (sub==0) ? this.textJustif : null;              break;
				}
			}
			return p;
		}

		public void CopyTo(ArrayCell dst)
		{
			this.leftLine   .CopyTo(dst.leftLine);
			this.bottomLine .CopyTo(dst.bottomLine);
			this.leftColor  .CopyTo(dst.leftColor);
			this.bottomColor.CopyTo(dst.bottomColor);
			this.backColor  .CopyTo(dst.backColor);
			this.textString .CopyTo(dst.textString);
			this.textFont   .CopyTo(dst.textFont);
			this.textJustif .CopyTo(dst.textJustif);
		}


		protected bool					selected = false;
		protected TextLayout			textLayout;
		protected Drawing.Point			bottomLeft;
		protected Drawing.Point			bottomRight;
		protected Drawing.Point			topLeft;
		protected Drawing.Point			topRight;

		[XmlAttribute]
		protected PropertyLine			leftLine;
		protected PropertyLine			bottomLine;
		protected PropertyColor			leftColor;
		protected PropertyColor			bottomColor;
		protected PropertyColor			backColor;
		protected PropertyString		textString;
		protected PropertyFont			textFont;
		protected PropertyJustif		textJustif;
	}
}
