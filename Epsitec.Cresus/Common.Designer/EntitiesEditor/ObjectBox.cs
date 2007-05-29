using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Boîte pour représenter une entité.
	/// </summary>
	public class ObjectBox : AbstractObject
	{
		public enum ConnectionAnchor
		{
			Left,
			Right,
			Bottom,
			Top,
		}


		public ObjectBox(Editor editor) : base(editor)
		{
			this.title = new TextLayout();
			this.title.DefaultFontSize = 12;
			this.title.Alignment = ContentAlignment.MiddleCenter;
			this.title.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

			this.fields = new List<Field>();

			this.isExtended = false;
		}


		public string Title
		{
			//	Titre au sommet de la boîte.
			get
			{
				return this.title.Text;
			}
			set
			{
				value = string.Concat("<b>", value, "</b>");

				if (this.title.Text != value)
				{
					this.title.Text = value;
				}
			}
		}

		public void SetContent(IList<StructuredData> fields)
		{
			foreach (StructuredData data in fields)
			{
				string s1 = data.GetValue(Support.Res.Fields.Field.Caption) as string;
				string s2 = data.GetValue(Support.Res.Fields.Field.CaptionId) as string;
				string s3 = data.GetValue(Support.Res.Fields.Field.Membership) as string;
				string s4 = data.GetValue(Support.Res.Fields.Field.Relation) as string;
				string s5 = data.GetValue(Support.Res.Fields.Field.SourceFieldId) as string;
				string s6 = data.GetValue(Support.Res.Fields.Field.TypeId) as string;
				// TODO: pourquoi est-ce que tous les strings retournés sont nuls ???

				Field field = new Field();
				field.Text = "Je n'arrive pas accéder aux noms des champs !!!";
				this.fields.Add(field);
			}
		}

		public void SetContent(string content)
		{
			//	Provisoire...
			string[] list = content.Split(';');

			foreach (string text in list)
			{
				Field field = new Field();
				field.Text = text;
				this.fields.Add(field);
			}
		}

		public bool IsExtended
		{
			//	Etat de la boîte (compact ou étendu).
			//	En mode compact, seul le titre est visible.
			//	En mode étendu, les champs sont visibles.
			get
			{
				return this.isExtended;
			}
			set
			{
				if (this.isExtended != value)
				{
					this.isExtended = value;
					this.editor.Invalidate();
				}
			}
		}

		public bool IsReadyForDragging
		{
			//	Est-ce que la boîte est survolée par la souris ?
			//	Si la boîte est survolée, on peut la déplacer globalement.
			get
			{
				return this.hilitedElement == ActiveElement.Header;
			}
		}


		public double GetBestHeight()
		{
			//	Retourne la hauteur requise selon le nombre de champs définis.
			if (this.isExtended)
			{
				return ObjectBox.headerHeight + ObjectBox.fieldHeight*this.fields.Count + ObjectBox.footerHeight + 20;
			}
			else
			{
				return ObjectBox.headerHeight;
			}
		}

		public double GetConnectionVerticalPosition(int rank)
		{
			//	Retourne la position verticale pour un trait de liaison.
			if (this.isExtended && rank < this.fields.Count)
			{
				Rectangle rect = this.GetFieldBounds(rank);
				return rect.Center.Y;
			}
			else
			{
				return this.bounds.Center.Y;
			}
		}

		public Point GetConnectionDestination(double posv, ConnectionAnchor anchor)
		{
			//	Retourne la position où accrocher la destination.
			Rectangle bounds = this.bounds;

			switch (anchor)
			{
				case ConnectionAnchor.Left:
					if (posv >= bounds.Bottom+ObjectBox.roundRectRadius && posv <= bounds.Top-ObjectBox.roundRectRadius)
					{
						return new Point(bounds.Left, posv);
					}
					else
					{
						return new Point(bounds.Left, bounds.Center.Y);
					}

				case ConnectionAnchor.Right:
					if (posv >= bounds.Bottom+ObjectBox.roundRectRadius && posv <= bounds.Top-ObjectBox.roundRectRadius)
					{
						return new Point(bounds.Right, posv);
					}
					else
					{
						return new Point(bounds.Right, bounds.Center.Y);
					}

				case ConnectionAnchor.Bottom:
					return new Point(bounds.Center.X, bounds.Bottom);

				case ConnectionAnchor.Top:
					return new Point(bounds.Center.X, bounds.Top);
			}

			return Point.Zero;
		}


		public override void MouseDown(Point pos)
		{
			//	Le bouton de la souris est pressé.
		}

		public override void MouseUp(Point pos)
		{
			//	Le bouton de la souris est relâché.
			if (this.hilitedElement == ActiveElement.ExtendButton)
			{
				this.IsExtended = !this.IsExtended;
				this.editor.UpdateAfterGeometryChanged(this);
			}
		}

		protected override bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	Détecte l'élément actif visé par la souris.
			element = ActiveElement.None;
			fieldRank = -1;

			if (pos.IsZero || !this.bounds.Contains(pos))
			{
				return false;
			}

			//	Souris dans le bouton compact/étendu ?
			Point center = new Point(this.bounds.Right-ObjectBox.buttonRadius-5, this.bounds.Top-ObjectBox.headerHeight/2);
			double d = Point.Distance(center, pos);
			if (d <= ObjectBox.buttonRadius+3)
			{
				element = ActiveElement.ExtendButton;
				return true;
			}

			//	Souris dans l'en-tête ?
			if (pos.Y >= this.bounds.Top-ObjectBox.headerHeight ||
				pos.Y <= this.bounds.Bottom+ObjectBox.footerHeight)
			{
				element = ActiveElement.Header;
				return true;
			}

			//	Souris dans un champ ?
			for (int i=0; i<this.fields.Count; i++)
			{
				Rectangle rect = this.GetFieldBounds(i);
				if (rect.Contains(pos))
				{
					element = ActiveElement.Field;
					fieldRank = i;
					return true;
				}
			}

			return false;
		}

		protected Rectangle GetFieldBounds(int rank)
		{
			//	Retourne le rectangle occupé par un champ.
			Rectangle rect = this.bounds;

			rect.Deflate(2, 0);
			rect.Bottom = rect.Top - ObjectBox.headerHeight - ObjectBox.fieldHeight*(rank+1) - 12;
			rect.Height = ObjectBox.fieldHeight;

			return rect;
		}


		public override void Draw(Graphics graphics)
		{
			//	Dessine l'objet.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect;

			//	Dessine l'ombre.
			rect = this.bounds;
			rect.Offset(ObjectBox.shadowOffset, -(ObjectBox.shadowOffset));
			this.PaintShadow(graphics, rect, ObjectBox.roundRectRadius+ObjectBox.shadowOffset, (int)ObjectBox.shadowOffset, 0.2);

			//	Construit le chemin du cadre arrondi.
			rect = this.bounds;
			rect.Deflate(1);
			Path path = this.PathRoundRectangle(rect, ObjectBox.roundRectRadius);

			//	Dessine l'intérieur en blanc.
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Color.FromBrightness(1));

			//	Dessine l'intérieur en dégradé.
			graphics.Rasterizer.AddSurface(path);
			Color c1 = adorner.ColorCaption;
			Color c2 = adorner.ColorCaption;
			c1.A = this.IsReadyForDragging ? 0.6 : 0.4;
			c2.A = this.IsReadyForDragging ? 0.2 : 0.1;
			this.RenderHorizontalGradient(graphics, this.bounds, c1, c2);

			//	Dessine en blanc la zone pour les champs.
			if (this.isExtended)
			{
				Rectangle inside = new Rectangle(this.bounds.Left+1, this.bounds.Bottom+ObjectBox.footerHeight, this.bounds.Width-2, this.bounds.Height-ObjectBox.footerHeight-ObjectBox.headerHeight);
				graphics.AddFilledRectangle(inside);
				graphics.RenderSolid(Color.FromBrightness(1));
				graphics.AddFilledRectangle(inside);
				Color ci1 = adorner.ColorCaption;
				Color ci2 = adorner.ColorCaption;
				ci1.A = this.IsReadyForDragging ? 0.2 : 0.1;
				ci2.A = 0.0;
				this.RenderHorizontalGradient(graphics, inside, ci1, ci2);

				Rectangle shadow = new Rectangle(this.bounds.Left+1, this.bounds.Top-ObjectBox.headerHeight-8, this.bounds.Width-2, 8);
				graphics.AddFilledRectangle(shadow);
				this.RenderVerticalGradient(graphics, shadow, Color.FromAlphaRgb(0.0, 0, 0, 0), Color.FromAlphaRgb(0.3, 0, 0, 0));
			}

			//	Dessine le titre.
#if false
			Font font = Font.GetFont("Tahoma", "Bold");

			graphics.AddText(this.bounds.Left+4, this.bounds.Top-ObjectBox.headerHeight+2, this.bounds.Width-ObjectBox.buttonRadius*2-5-6, ObjectBox.headerHeight-2, this.title, font, 14, ContentAlignment.MiddleCenter);
			graphics.RenderSolid(Color.FromBrightness(0));
#else
			rect = new Rectangle(this.bounds.Left+4, this.bounds.Top-ObjectBox.headerHeight+2, this.bounds.Width-ObjectBox.buttonRadius*2-5-6, ObjectBox.headerHeight-2);
			this.title.LayoutSize = rect.Size;
			this.title.Paint(rect.BottomLeft, graphics);
#endif

			//	Dessine le bouton compact/étendu.
			Point center = new Point(this.bounds.Right-ObjectBox.buttonRadius-5, this.bounds.Top-ObjectBox.headerHeight/2);

			graphics.AddFilledCircle(center, ObjectBox.buttonRadius);
			graphics.RenderSolid(this.hilitedElement == ActiveElement.ExtendButton ? adorner.ColorCaption : Color.FromBrightness(1));

			graphics.AddCircle(center, ObjectBox.buttonRadius);
			graphics.RenderSolid(Color.FromBrightness(0));

			rect = new Rectangle(center.X-ObjectBox.buttonRadius, center.Y-ObjectBox.buttonRadius, ObjectBox.buttonRadius*2, ObjectBox.buttonRadius*2);
			GlyphShape shape = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			Color cb = (this.hilitedElement == ActiveElement.ExtendButton) ? Color.FromBrightness(1) : Color.FromBrightness(0);
			adorner.PaintGlyph(graphics, rect, WidgetPaintState.Enabled, cb, shape, PaintTextStyle.Button);

			//	Dessine les noms des champs.
			Color hiliteColor = adorner.ColorCaption;
			hiliteColor.A = 0.1;

			if (this.isExtended)
			{
				//?font = Font.GetFont("Tahoma", "Regular");

				Color color = Color.FromBrightness(0.9);
				if (this.IsReadyForDragging)
				{
					color = adorner.ColorCaption;
					color.A = 0.3;
				}

				for (int i=0; i<this.fields.Count; i++)
				{
					rect = this.GetFieldBounds(i);

					if (this.hilitedElement == ActiveElement.Field && this.hilitedFieldRank == i)
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(hiliteColor);
					}

					rect.Deflate(10, 0);
					this.fields[i].TextLayout.LayoutSize = rect.Size;
					this.fields[i].TextLayout.Paint(rect.BottomLeft, graphics);

					rect.Inflate(10, 0);
					graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
					graphics.RenderSolid(color);

					rect.Offset(0, -ObjectBox.fieldHeight);
				}
			}

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline(path, 2);
			if (this.isExtended)
			{
				graphics.AddLine(this.bounds.Left+2, this.bounds.Top-ObjectBox.headerHeight-0.5, this.bounds.Right-2, this.bounds.Top-ObjectBox.headerHeight-0.5);
				graphics.AddLine(this.bounds.Left+2, this.bounds.Bottom+ObjectBox.footerHeight+0.5, this.bounds.Right-2, this.bounds.Bottom+ObjectBox.footerHeight+0.5);
			}
			graphics.RenderSolid(this.IsReadyForDragging ? adorner.ColorCaption : Color.FromBrightness(0));
		}



		protected class Field
		{
			public Field()
			{
				this.textLayout = new TextLayout();
				this.textLayout.DefaultFontSize = 10;
				this.textLayout.Alignment = ContentAlignment.MiddleLeft;
				this.textLayout.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			}

			public string Text
			{
				get
				{
					return this.textLayout.Text;
				}
				set
				{
					this.textLayout.Text = value;
				}
			}

			public TextLayout TextLayout
			{
				get
				{
					return this.textLayout;
				}
			}

			protected TextLayout textLayout;
		}


		protected static readonly double roundRectRadius = 12;
		protected static readonly double shadowOffset = 6;
		protected static readonly double headerHeight = 32;
		protected static readonly double footerHeight = 10;
		protected static readonly double buttonRadius = 10;
		protected static readonly double fieldHeight = 20;

		protected bool isExtended;
		protected TextLayout title;
		protected List<Field> fields;
	}
}
