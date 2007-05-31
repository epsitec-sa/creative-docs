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
		protected enum ActiveElement
		{
			None,
			Inside,
			ExtendButton,
			HeaderDragging,
			FieldSelect,
			FieldAdd,
			FieldRemove,
			FieldMoving,
			ConnectionOpenLeft,
			ConnectionOpenRight,
			ConnectionClose,
			ChangeWidth,
			MoveColumnsSeparator,
		}


		public AbstractObject(Editor editor)
		{
			//	Constructeur.
			this.editor = editor;
			this.hilitedElement = ActiveElement.None;
		}


		public virtual void Draw(Graphics graphics)
		{
			//	Dessine l'objet.
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


		protected void CloseBoxes(ObjectBox box)
		{
			//	Ferme récursivement toutes les boîtes liées.
			if (box != null)
			{
				foreach (ObjectBox.Field field in box.Fields)
				{
					if (field.Relation != FieldRelation.None)
					{
						if (field.DstBox != null)
						{
							this.CloseBoxes(field.DstBox);
						}
					}
				}

				this.editor.RemoveBox(box);
			}
		}

		protected void DrawRoundButton(Graphics graphics, Point center, double radius, GlyphShape shape, bool hilited, bool shadow)
		{
			//	Dessine un bouton circulaire.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect;

			if (shadow)
			{
				rect = new Rectangle(center.X-radius, center.Y-radius, radius*2, radius*2);
				rect.Inflate(radius*0.2);
				rect.Offset(0, -radius*0.7);
				this.DrawShadow(graphics, rect, rect.Width/2, (int) (radius*0.7), 0.5);
			}

			graphics.AddFilledCircle(center, radius);
			graphics.RenderSolid(hilited ? this.ColorCaption : Color.FromBrightness(1));

			graphics.AddCircle(center, radius);
			graphics.RenderSolid(Color.FromBrightness(0));

			if (shape != GlyphShape.None)
			{
				rect = new Rectangle(center.X-radius, center.Y-radius, radius*2, radius*2);
				Color color = hilited ? Color.FromBrightness(1) : Color.FromBrightness(0);
				adorner.PaintGlyph(graphics, rect, WidgetPaintState.Enabled, color, shape, PaintTextStyle.Button);
			}
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

		protected Color ColorCaption
		{
			//	Retourne la couleur pour les mises en évidence.
			get
			{
				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

				Color color = adorner.ColorCaption;
				color.A = 1;

				if (color.GetBrightness() > 0.7)  // couleur très claire ?
				{
					color.R *= 0.5;  // fonce la couleur
					color.G *= 0.5;
					color.B *= 0.5;
				}

				return color;
			}
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


		protected Editor editor;
		protected ActiveElement hilitedElement;
		protected int hilitedFieldRank;
	}
}
