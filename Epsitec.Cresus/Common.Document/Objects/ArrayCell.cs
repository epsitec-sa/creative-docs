using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe ArrayCell est la classe qui peuple l'objet ObjectArray.
	/// </summary>
	public class ArrayCell
	{
		public ArrayCell(Document document)
		{
			this.document = document;

			this.textLayout = new TextLayout();
			this.textNavigator = new TextNavigator(this.textLayout);
			this.textLayout.BreakMode = TextBreakMode.Hyphenate;
			if ( this.document.Modifier != null )
			{
				this.textNavigator.OpletQueue = this.document.Modifier.OpletQueue;
			}
		}

		public string Content
		{
			get { return this.textLayout.Text; }
			set
			{
				this.textLayout.Text = value;
				this.textNavigator.ValidateCursors();
			}
		}

		public bool Selected
		{
			get { return this.selected; }
			set { this.selected = value; }
		}

		public TextLayout TextLayout
		{
			get { return this.textLayout; }
			set { this.textLayout = value; }
		}

		public TextNavigator TextNavigator
		{
			get { return this.textNavigator; }
			set { this.textNavigator = value; }
		}

		public Transform Transform
		{
			get { return this.transform; }
			set { this.transform = value; }
		}

		public Point BottomLeft
		{
			get { return this.bottomLeft; }
			set { this.bottomLeft = value; }
		}

		public Point BottomRight
		{
			get { return this.bottomRight; }
			set { this.bottomRight = value; }
		}

		public Point TopLeft
		{
			get { return this.topLeft; }
			set { this.topLeft = value; }
		}

		public Point TopRight
		{
			get { return this.topRight; }
			set { this.topRight = value; }
		}

		public Properties.Line LeftLine
		{
			get { return this.leftLine; }
			set { this.leftLine = value; }
		}

		public Properties.Line BottomLine
		{
			get { return this.bottomLine; }
			set { this.bottomLine = value; }
		}

		public Properties.Color LeftColor
		{
			get { return this.leftColor; }
			set { this.leftColor = value; }
		}

		public Properties.Color BottomColor
		{
			get { return this.bottomColor; }
			set { this.bottomColor = value; }
		}

		public Properties.Color BackColor
		{
			get { return this.backColor; }
			set { this.backColor = value; }
		}

		public Properties.Justif TextJustif
		{
			get { return this.textJustif; }
			set { this.textJustif = value; }
		}

		public void SetProperty(Properties.Abstract property, int sub)
		{
			Properties.Abstract actual = this.Property(property.Type, sub);
			if ( actual == null )  return;
			property.CopyTo(actual);
		}

		public Properties.Abstract Property(Properties.Type type, int sub)
		{
			Properties.Abstract p = null;
			if ( sub < 2 )
			{
				switch ( type )
				{
					case Properties.Type.LineMode:    p = (sub==0) ? this.leftLine   : this.bottomLine;   break;
					case Properties.Type.LineColor:   p = (sub==0) ? this.leftColor  : this.bottomColor;  break;
					case Properties.Type.BackColor:   p = (sub==0) ? this.backColor  : null;              break;
					case Properties.Type.TextJustif:  p = (sub==0) ? this.textJustif : null;              break;
				}
			}
			return p;
		}

		public void CopyTo(ArrayCell dst)
		{
			dst.textLayout.Text = this.textLayout.Text;
			this.textNavigator.Context.CopyTo(dst.textNavigator.Context);
			this.leftLine   .CopyTo(dst.leftLine);
			this.bottomLine .CopyTo(dst.bottomLine);
			this.leftColor  .CopyTo(dst.leftColor);
			this.bottomColor.CopyTo(dst.bottomColor);
			this.backColor  .CopyTo(dst.backColor);
			this.textJustif .CopyTo(dst.textJustif);
		}


		protected Document				document;
		protected bool					selected = false;
		protected TextLayout			textLayout;
		protected TextNavigator			textNavigator;
		protected Transform				transform;
		protected Point					bottomLeft;
		protected Point					bottomRight;
		protected Point					topLeft;
		protected Point					topRight;
		protected Properties.Line		leftLine;
		protected Properties.Line		bottomLine;
		protected Properties.Color		leftColor;
		protected Properties.Color		bottomColor;
		protected Properties.Color		backColor;
		protected Properties.Justif		textJustif;
	}
}
