using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// La classe AbstractObject est la classe de base des objets graphiques.
	/// </summary>
	public abstract class AbstractObject
	{
		public enum ActiveElement
		{
			None,
			Inside,
			ExtendButton,
			CloseButton,
			ParentsButton,
			CommentButton,
			HeaderDragging,
			FieldNameSelect,
			FieldTypeSelect,
			FieldAdd,
			FieldRemove,
			FieldMovable,
			FieldMoving,
			ChangeWidth,
			MoveColumnsSeparator,
			ConnectionOpenLeft,
			ConnectionOpenRight,
			ConnectionClose,
			ConnectionHilited,
			ConnectionChangeRelation,
			ConnectionMove1,
			ConnectionMove2,
			ConnectionComment,
			CommentEdit,
			CommentMove,
			CommentSize,
			CommentClose,
		}


		public AbstractObject(Editor editor)
		{
			//	Constructeur.
			this.editor = editor;
			this.hilitedElement = ActiveElement.None;
		}


		public virtual Rectangle Bounds
		{
			//	Retourne la boîte de l'objet.
			get
			{
				return Rectangle.Empty;
			}
		}

		public virtual void DrawBackground(Graphics graphics)
		{
			//	Dessine le fond de l'objet.
		}

		public virtual void DrawForeground(Graphics graphics)
		{
			//	Dessine le dessus de l'objet.
		}


		public bool IsReadyForAction
		{
			//	Est-ce que l'objet est prêt pour une action.
			get
			{
				return (this.hilitedElement != ActiveElement.None);
			}
		}

		public virtual bool IsReadyForDragging
		{
			//	Est-ce que l'objet est dragable ?
			get
			{
				return false;
			}
		}

		public ActiveElement HilitedElement
		{
			get
			{
				return this.hilitedElement;
			}
		}

		public virtual bool MouseMove(Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			ActiveElement element;
			int fieldRank;
			this.MouseDetect(pos, out element, out fieldRank);

			if (this.hilitedElement != element || this.hilitedFieldRank != fieldRank)
			{
				this.hilitedElement = element;
				this.hilitedFieldRank = fieldRank;
				this.editor.Invalidate();
			}

			return (this.hilitedElement != ActiveElement.None);
		}

		public virtual void MouseDown(Point pos)
		{
			//	Le bouton de la souris est pressé.
		}

		public virtual void MouseUp(Point pos)
		{
			//	Le bouton de la souris est relâché.
		}

		protected virtual bool MouseDetect(Point pos, out ActiveElement element, out int fieldRank)
		{
			//	Détecte l'élément actif visé par la souris.
			element = ActiveElement.None;
			fieldRank = -1;
			return false;
		}


		protected void SetDirty()
		{
			//	Active la commande d'enregistrement, lorsqu'une modification a été effectuée.
			this.editor.Module.AccessEntities.IsDirty = true;
			this.editor.Module.AccessEntities.Accessor.PersistChanges();
		}


		protected bool DetectRoundButton(Point center, Point pos)
		{
			//	Détecte si la souris est dans un bouton circulaire.
			return (Point.Distance(center, pos) <= AbstractObject.buttonRadius+1);
		}

		protected void DrawRoundButton(Graphics graphics, Point center, double radius, GlyphShape shape, bool hilited, bool shadow)
		{
			//	Dessine un bouton circulaire avec un glyph.
			this.DrawRoundButton(graphics, center, radius, shape, hilited, shadow, true);
		}

		protected void DrawRoundButton(Graphics graphics, Point center, double radius, GlyphShape shape, bool hilited, bool shadow, bool enable)
		{
			//	Dessine un bouton circulaire avec un glyph.
			this.DrawRoundButton(graphics, center, radius, hilited, shadow, enable);

			if (shape != GlyphShape.None)
			{
				Color colorShape;
				if (enable)
				{
					colorShape = hilited ? Color.FromBrightness(1) : Color.FromBrightness(0);
				}
				else
				{
					colorShape = Color.FromBrightness(0.7);
				}

				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
				Rectangle rect = new Rectangle(center.X-radius, center.Y-radius, radius*2, radius*2);
				adorner.PaintGlyph(graphics, rect, WidgetPaintState.Enabled, colorShape, shape, PaintTextStyle.Button);
			}
		}

		protected void DrawRoundButton(Graphics graphics, Point center, double radius, string text, bool hilited, bool shadow)
		{
			//	Dessine un bouton circulaire avec un texte (généralement une seule lettre).
			this.DrawRoundButton(graphics, center, radius, text, hilited, shadow, true);
		}

		protected void DrawRoundButton(Graphics graphics, Point center, double radius, string text, bool hilited, bool shadow, bool enable)
		{
			//	Dessine un bouton circulaire avec un texte (généralement une seule lettre).
			this.DrawRoundButton(graphics, center, radius, hilited, shadow, enable);

			if (!string.IsNullOrEmpty(text))
			{
				Color colorShape;
				if (enable)
				{
					colorShape = hilited ? Color.FromBrightness(1) : Color.FromBrightness(0);
				}
				else
				{
					colorShape = Color.FromBrightness(0.7);
				}

				Rectangle rect = new Rectangle(center.X-radius, center.Y-radius, radius*2, radius*2);
				graphics.AddText(rect.Left, rect.Bottom+1, rect.Width, rect.Height, text, Font.GetFont(Font.DefaultFontFamily, "Bold"), 14, ContentAlignment.MiddleCenter);
				graphics.RenderSolid(colorShape);
			}
		}

		protected void DrawRoundButton(Graphics graphics, Point center, double radius, bool hilited, bool shadow, bool enable)
		{
			//	Dessine un bouton circulaire vide.
			if (shadow)
			{
				Rectangle rect = new Rectangle(center.X-radius, center.Y-radius, radius*2, radius*2);
				rect.Inflate(radius*0.2);
				rect.Offset(0, -radius*0.7);
				this.DrawShadow(graphics, rect, rect.Width/2, (int) (radius*0.7), 0.5);
			}

			Color colorSurface;
			Color colorFrame;
			Color colorShape;

			if (enable)
			{
				colorSurface = hilited ? this.GetColorCaption() : Color.FromBrightness(1);
				colorFrame = Color.FromBrightness(0);
				colorShape = hilited ? Color.FromBrightness(1) : Color.FromBrightness(0);
			}
			else
			{
				colorSurface = Color.FromBrightness(0.9);
				colorFrame = Color.FromBrightness(0.5);
				colorShape = Color.FromBrightness(0.7);
			}

			graphics.AddFilledCircle(center, radius);
			graphics.RenderSolid(colorSurface);

			graphics.AddCircle(center, radius);
			graphics.RenderSolid(colorFrame);
		}

		protected void DrawShadow(Graphics graphics, Rectangle rect, double radius, int smooth, double alpha)
		{
			//	Dessine une ombre douce.
			alpha /= smooth;

			for (int i=0; i<smooth; i++)
			{
				Path path = this.PathRoundRectangle(rect, radius);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(Color.FromAlphaRgb(alpha, 0, 0, 0));

				rect.Deflate(1);
				radius -= 1;
			}
		}

		protected void DrawStartingArrow(Graphics graphics, Point start, Point end, FieldRelation relation)
		{
			//	Dessine une flèche selon le type de la relation.
			if (relation == FieldRelation.Inclusion)
			{
				this.DrawArrowBase(graphics, end, start);
			}
		}

		protected void DrawEndingArrow(Graphics graphics, Point start, Point end, FieldRelation relation)
		{
			//	Dessine une flèche selon le type de la relation.
			this.DrawArrowBase(graphics, start, end);

			if (relation == FieldRelation.Collection)
			{
				end = Point.Move(end, start, AbstractObject.arrowLength*0.75);
				this.DrawArrowBase(graphics, start, end);
			}
		}

		protected void DrawArrowBase(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche à l'extrémité 'end'.
			Point p = Point.Move(end, start, AbstractObject.arrowLength);

			Point e1 = Transform.RotatePointDeg(end, AbstractObject.arrowAngle, p);
			Point e2 = Transform.RotatePointDeg(end, -AbstractObject.arrowAngle, p);

			graphics.AddLine(end, e1);
			graphics.AddLine(end, e2);
		}

		protected Color GetColorCaption()
		{
			//	Retourne la couleur pour les mises en évidence.
			return this.GetColorCaption(1.0);
		}

		protected Color GetColorCaption(double alpha)
		{
			//	Retourne la couleur pour les mises en évidence.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Color color = adorner.ColorCaption;
			color.A = alpha;

			if (color.GetBrightness() > 0.7)  // couleur très claire ?
			{
				color.R *= 0.5;  // fonce la couleur
				color.G *= 0.5;
				color.B *= 0.5;
			}

			return color;
		}

		protected void RenderHorizontalGradient(Graphics graphics, Rectangle rect, Color leftColor, Color rightColor)
		{
			//	Peint la surface avec un dégradé horizontal.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.X;
			graphics.GradientRenderer.SetColors(leftColor, rightColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = new Transform();
			Point center = rect.Center;
			t.Scale(rect.Width/100/2, rect.Height/100/2);
			t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected void RenderVerticalGradient(Graphics graphics, Rectangle rect, Color bottomColor, Color topColor)
		{
			//	Peint la surface avec un dégradé vertical.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors(bottomColor, topColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = new Transform();
			Point center = rect.Center;
			t.Scale(rect.Width/100/2, rect.Height/100/2);
			t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected Path PathRoundRectangle(Rectangle rect, double radius)
		{
			//	Retourne le chemin d'un rectangle à coins arrondis.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			radius = System.Math.Min(radius, System.Math.Min(dx, dy)/2);

			Path path = new Path();
			path.MoveTo (ox+radius, oy);
			path.LineTo (ox+dx-radius, oy);
			path.CurveTo(ox+dx, oy, ox+dx, oy+radius);
			path.LineTo (ox+dx, oy+dy-radius);
			path.CurveTo(ox+dx, oy+dy, ox+dx-radius, oy+dy);
			path.LineTo (ox+radius, oy+dy);
			path.CurveTo(ox, oy+dy, ox, oy+dy-radius);
			path.LineTo (ox, oy+radius);
			path.CurveTo(ox, oy, ox+radius, oy);
			path.Close();

			return path;
		}


		protected static readonly double headerHeight = 32;
		protected static readonly double footerHeight = 13;
		protected static readonly double buttonRadius = 10;
		protected static readonly double bulletRadius = 4;
		protected static readonly double lengthClose = 30;
		protected static readonly double arrowLength = 12;
		protected static readonly double arrowAngle = 25;
		protected static readonly double commentMinWidth = 120;

		protected Editor editor;
		protected ActiveElement hilitedElement;
		protected int hilitedFieldRank;
	}
}
