//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using System.Xml;
using System.Xml.Serialization;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	/// <summary>
	/// La classe AbstractObject est la classe de base des objets graphiques.
	/// </summary>
	public abstract class AbstractObject
	{
		public AbstractObject(Editor editor, AbstractEntity entity)
		{
			//	Constructeur.
			this.editor = editor;
			this.entity = entity;

			this.boxColor = MainColor.Blue;
			this.isDimmed = false;
			this.hilitedElement = ActiveElement.None;
		}


		public Editor Editor
		{
			get
			{
				return this.editor;
			}
		}

		public AbstractEntity AbstractEntity
		{
			get
			{
				return this.entity;
			}
		}

		public virtual Rectangle Bounds
		{
			//	Retourne la boîte de l'objet.
			get
			{
				return Rectangle.Empty;
			}
		}

		public virtual void Move(double dx, double dy)
		{
			//	Déplace l'objet.
		}

		public virtual MainColor BackgroundMainColor
		{
			//	Couleur de fond de la boîte.
			get
			{
				return this.boxColor;
			}
			set
			{
				if (this.boxColor != value)
				{
					this.boxColor = value;
					this.editor.Invalidate();
				}
			}
		}

		public bool IsDimmed
		{
			//	Détermine si l'objet apparaît en estompé (couleurs plus claires).
			get
			{
				return this.isDimmed;
			}
			set
			{
				if (this.isDimmed != value)
				{
					this.isDimmed = value;
					this.editor.Invalidate();
				}
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


		public ActiveElement HilitedElement
		{
			get
			{
				return this.hilitedElement;
			}
			set
			{
				if (this.hilitedElement != value)
				{
					this.hilitedElement = value;
					this.editor.Invalidate ();
				}
			}
		}

		public string GetToolTipText(Point pos)
		{
			//	Retourne le texte pour le tooltip.
			return this.GetToolTipText (this.hilitedElement);
		}

		protected virtual string GetToolTipText(ActiveElement element)
		{
			return null;  // pas de tooltip
		}

		public virtual bool MouseMove(Message message, Point pos)
		{
			return false;
		}

		public virtual void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est pressé.
		}

		public virtual void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
		}

		public virtual ActiveElement MouseDetectBackground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			return ActiveElement.None;
		}

		public virtual ActiveElement MouseDetectForeground(Point pos)
		{
			//	Détecte l'élément actif visé par la souris.
			return ActiveElement.None;
		}

		public virtual bool IsMousePossible(ActiveElement element)
		{
			//	Indique si l'opération est possible.
			return true;
		}


		public virtual void AcceptEdition()
		{
		}

		public virtual void CancelEdition()
		{
		}


		public virtual string DebugInformations
		{
			get
			{
				return null;
			}
		}

		public virtual string DebugInformationsBase
		{
			get
			{
				return null;
			}
		}

		public string DebugInformationsEntityKey
		{
			get
			{
				var key = this.editor.BusinessContext.DataContext.GetNormalizedEntityKey (this.entity);

				if (key.HasValue)
				{
					return key.Value.ToString ();
				}
				else
				{
					return "no key";
				}
			}
		}


		protected bool DetectSquareButton(Point center, Point pos)
		{
			//	Détecte si la souris est dans un bouton carré.
			if (center.IsZero)
			{
				return false;
			}
			else
			{
				double radius = AbstractObject.buttonSquare;
				Rectangle rect = new Rectangle(center.X-radius, center.Y-radius, radius*2, radius*2);
				rect.Inflate(0.5);
				return rect.Contains(pos);
			}
		}

		protected void DrawSquareButton(Graphics graphics, Point center, MainColor color, bool selected, bool hilited)
		{
			//	Dessine un bouton carré avec une couleur.
			if (center.IsZero)
			{
				return;
			}

			double radius = AbstractObject.buttonSquare;
			Rectangle rect = new Rectangle(center.X-radius, center.Y-radius, radius*2, radius*2);
			rect.Inflate(0.5);

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.GetColorMain(color, 0.8));

			graphics.AddRectangle(rect);
			graphics.RenderSolid(this.GetColor(0));

			if (selected)
			{
				rect.Deflate(1);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.GetColor(1));
				rect.Inflate(1);
			}

			if (hilited)
			{
				rect.Deflate(2);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(this.GetColor(1));
			}
		}


		protected bool DetectRoundButton(Point center, Point pos)
		{
			//	Détecte si la souris est dans un bouton circulaire.
			if (center.IsZero)
			{
				return false;
			}
			else
			{
				return (Point.Distance(center, pos) <= AbstractObject.buttonRadius+1);
			}
		}

		protected void DrawRoundButton(Graphics graphics, Point center, double radius, GlyphShape shape, bool hilited, bool shadow)
		{
			//	Dessine un bouton circulaire avec un glyph.
			this.DrawRoundButton(graphics, center, radius, shape, hilited, shadow, true);
		}

		protected void DrawRoundButton(Graphics graphics, Point center, double radius, GlyphShape shape, bool hilited, bool shadow, bool enable)
		{
			//	Dessine un bouton circulaire avec un glyph.
			if (center.IsZero)
			{
				return;
			}

			this.DrawRoundButton(graphics, center, radius, hilited, shadow, enable);

			if (shape != GlyphShape.None)
			{
				Color colorShape;
				if (enable)
				{
					colorShape = hilited ? this.GetColor(1) : this.GetColor(0);
				}
				else
				{
					colorShape = this.GetColor(0.7);
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
			if (center.IsZero)
			{
				return;
			}

			this.DrawRoundButton(graphics, center, radius, hilited, shadow, enable);

			if (!string.IsNullOrEmpty(text))
			{
				Color colorShape;
				if (enable)
				{
					colorShape = hilited ? this.GetColor(1) : this.GetColor(0);
				}
				else
				{
					colorShape = this.GetColor(0.7);
				}

				Rectangle rect = new Rectangle(center.X-radius, center.Y-radius, radius*2, radius*2);
				double size = 14;

				if (text == "*")  // texte étoile pour une relation privée ?
				{
					size = 30;  // beaucoup plus grand
					rect.Offset(0, -4);  // légèrement plus bas
				}

				graphics.AddText(rect.Left, rect.Bottom+1, rect.Width, rect.Height, text, Font.GetFont(Font.DefaultFontFamily, "Bold"), size, ContentAlignment.MiddleCenter);
				graphics.RenderSolid(colorShape);
			}
		}

		protected void DrawRoundButton(Graphics graphics, Point center, double radius, bool hilited, bool shadow, bool enable)
		{
			//	Dessine un bouton circulaire vide.
			if (center.IsZero)
			{
				return;
			}

			if (shadow)
			{
				Rectangle rect = new Rectangle(center.X-radius, center.Y-radius, radius*2, radius*2);
				rect.Inflate(radius*0.2);
				rect.Offset(0, -radius*0.7);
				this.DrawRoundShadow(graphics, rect, rect.Width/2, (int) (radius*0.7), 0.5);
			}

			Color colorSurface;
			Color colorFrame;
			Color colorShape;

			if (enable)
			{
				colorSurface = hilited ? this.GetColorMain() : this.GetColor(1);
				colorFrame = this.GetColor(0);
				colorShape = hilited ? this.GetColor(1) : this.GetColor(0);
			}
			else
			{
				colorSurface = this.GetColor(0.9);
				colorFrame = this.GetColor(0.5);
				colorShape = this.GetColor(0.7);
			}

			graphics.AddFilledCircle(center, radius);
			graphics.RenderSolid(colorSurface);

			graphics.AddCircle(center, radius);
			graphics.RenderSolid(colorFrame);
		}


		protected void DrawNode2Shadow(Graphics graphics, Rectangle rect, int smooth, double alpha)
		{
			//	Dessine une ombre douce pour un objet noeud (ObjectNode).
			alpha /= smooth;

			for (int i=0; i<smooth; i++)
			{
				Path path = this.PathNode2Rectangle (rect);
				graphics.Rasterizer.AddSurface (path);
				graphics.RenderSolid (Color.FromAlphaRgb (alpha, 0, 0, 0));

				rect.Deflate (1);
			}
		}

		protected void DrawNodeShadow(Graphics graphics, Rectangle rect, double radius, int smooth, double alpha)
		{
			//	Dessine une ombre douce pour un objet noeud (ObjectNode).
			alpha /= smooth;

			for (int i=0; i<smooth; i++)
			{
				Path path = this.PathNodeRectangle (rect, radius);
				graphics.Rasterizer.AddSurface (path);
				graphics.RenderSolid (Color.FromAlphaRgb (alpha, 0, 0, 0));

				rect.Deflate (1);
				radius -= 1;
			}
		}

		protected void DrawRoundShadow(Graphics graphics, Rectangle rect, double radius, int smooth, double alpha)
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

		public static void DrawStartingArrow(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche selon le type.
		}

		public static void DrawEndingArrow(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche selon le type.
			AbstractObject.DrawArrowBase(graphics, start, end);

#if false
			if (false)
			{
				Point p1 = Point.Move(end, start, AbstractObject.arrowLength);
				Point p2 = Point.Move(end, start, AbstractObject.arrowLength*0.75);
				AbstractObject.DrawArrowBase(graphics, p1, p2);
			}

			if (false)
			{
				AbstractObject.DrawArrowStar(graphics, start, end);
			}
#endif
		}

		protected static void DrawArrowBase(Graphics graphics, Point start, Point end)
		{
			//	Dessine une flèche à l'extrémité 'end'.
			Point p = Point.Move(end, start, AbstractObject.arrowLength);

			Point e1 = Transform.RotatePointDeg(end, AbstractObject.arrowAngle, p);
			Point e2 = Transform.RotatePointDeg(end, -AbstractObject.arrowAngle, p);

			graphics.AddLine(end, e1);
			graphics.AddLine(end, e2);
		}

		protected static void DrawArrowStar(Graphics graphics, Point start, Point end)
		{
			//	Dessine une étoile à l'extrémité 'end'.
			Point p = Point.Move(end, start, AbstractObject.arrowLength*0.85);
			Point e = Transform.RotatePointDeg(end, AbstractObject.arrowAngle*2.5, p);
			Point q = Point.Move(e, new Point(e.X, e.Y+1), AbstractObject.arrowLength*0.25);

			for (int i=0; i<5; i++)
			{
				Point f = Transform.RotatePointDeg(e, 360.0/5.0*i, q);
				graphics.AddLine(e, f);
			}
		}

		protected bool IsDarkColorMain
		{
			//	Indique si la couleur pour les mises en évidence est foncée.
			get
			{
				return this.boxColor == MainColor.DarkGrey;
			}
		}

		protected Color GetColorMain()
		{
			//	Retourne la couleur pour les mises en évidence.
			return this.GetColorMain(1.0);
		}

		protected Color GetColorMain(double alpha)
		{
			//	Retourne la couleur pour les mises en évidence.
			return this.GetColorMain(this.boxColor, alpha);
		}

		protected Color GetColorMain(MainColor boxColor)
		{
			//	Retourne la couleur pour les mises en évidence.
			return this.GetColorMain(boxColor, 1.0);
		}

		protected Color GetColorMain(MainColor boxColor, double alpha)
		{
			//	Retourne la couleur pour les mises en évidence.
			Color color = Color.FromAlphaRgb(alpha, 128.0/255.0, 128.0/255.0, 128.0/255.0);

			switch (boxColor)
			{
				case MainColor.Blue:
					color = Color.FromAlphaRgb(alpha, 0.0/255.0, 90.0/255.0, 160.0/255.0);
					break;

				case MainColor.Green:
					color = Color.FromAlphaRgb(alpha, 0.0/255.0, 130.0/255.0, 20.0/255.0);
					break;

				case MainColor.Red:
					color = Color.FromAlphaRgb(alpha, 140.0/255.0, 30.0/255.0, 0.0/255.0);
					break;

				case MainColor.Grey:
					color = Color.FromAlphaRgb(alpha, 100.0/255.0, 100.0/255.0, 100.0/255.0);
					break;

				case MainColor.DarkGrey:
					color = Color.FromAlphaRgb(alpha, 100.0/255.0, 100.0/255.0, 100.0/255.0);
					break;

				case MainColor.Yellow:
					color = Color.FromAlphaRgb(alpha, 200.0/255.0, 200.0/255.0, 0.0/255.0);
					break;

				case MainColor.Orange:
					color = Color.FromAlphaRgb(alpha, 200.0/255.0, 150.0/255.0, 0.0/255.0);
					break;

				case MainColor.Lilac:
					color = Color.FromAlphaRgb(alpha, 100.0/255.0, 0.0/255.0, 150.0/255.0);
					break;

				case MainColor.Purple:
					color = Color.FromAlphaRgb(alpha, 30.0/255.0, 0.0/255.0, 200.0/255.0);
					break;
			}

			if (this.isDimmed)
			{
				color = this.GetColorLighter(color, 0.3);
			}

			return color;
		}

		protected Color GetColor(double brightness)
		{
			//	Retourne un niveau de gris.
			Color color = Color.FromBrightness(brightness);

			if (this.isDimmed)
			{
				color = this.GetColorLighter(color, 0.3);
			}

			return color;
		}

		protected Color GetColorAdjusted(Color color, double factor)
		{
			//	Retourne une couleur ajustée, sans changer la transparence.
			if (this.IsDarkColorMain)
			{
				return this.GetColorDarker(color, factor);
			}
			else
			{
				return this.GetColorLighter(color, factor);
			}
		}

		private Color GetColorLighter(Color color, double factor)
		{
			//	Retourne une couleur éclaircie, sans changer la transparence.
			return Color.FromAlphaRgb(color.A, 1-(1-color.R)*factor, 1-(1-color.G)*factor, 1-(1-color.B)*factor);
		}

		private Color GetColorDarker(Color color, double factor)
		{
			//	Retourne une couleur assombrie, sans changer la transparence.
			factor = 0.5+(factor*0.5);
			return Color.FromAlphaRgb(color.A, color.R*factor, color.G*factor, color.B*factor);
		}


		protected void RenderHorizontalGradient(Graphics graphics, Rectangle rect, Color leftColor, Color rightColor)
		{
			//	Peint la surface avec un dégradé horizontal.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.X;
			graphics.GradientRenderer.SetColors(leftColor, rightColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = Transform.Identity;
			Point center = rect.Center;
			t = t.Scale(rect.Width/100/2, rect.Height/100/2);
			t = t.Translate(center);
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
			Transform t = Transform.Identity;
			Point center = rect.Center;
			t = t.Scale(rect.Width/100/2, rect.Height/100/2);
			t = t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected Path PathNode2Rectangle(Rectangle rect)
		{
			var path = new Path ();

			path.AppendCircle (rect.Center, rect.Width/2, rect.Height/2);

			return path;
		}

		protected Path PathNodeRectangle(Rectangle rect, double radius)
		{
			//	Retourne le chemin d'un rectangle pour un ObjectNode.
			//?return this.PathBevelRectangle (rect, radius*0.5, radius, radius*0.5, radius, true, true);  // coins biseautés
			return this.PathBevelRectangle (rect, AbstractObject.headerHeight*0.25, AbstractObject.headerHeight, 0.5, 0.5, true, true);  // coins biseautés seulement en haut
		}

		protected Path PathRoundRectangle(Rectangle rect, double radius)
		{
			//	Retourne le chemin d'un rectangle à coins arrondis.
			return this.PathRoundRectangle(rect, radius, true, true);  // coins arrondis partout
		}

		protected Path PathRoundRectangle(Rectangle rect, double radius, bool isTopRounded, bool isBottomRounded)
		{
			//	Retourne le chemin d'un rectangle, avec des coins arrondis en haut et/ou en bas.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			radius = System.Math.Min(radius, System.Math.Min(dx, dy)/2);

			Path path = new Path();

			if (isBottomRounded)  // coins bas/gauche et bas/droite arrondis ?
			{
				path.MoveTo(ox, oy+radius);
				path.CurveTo(ox, oy, ox+radius, oy);
				path.LineTo(ox+dx-radius, oy);
				path.CurveTo(ox+dx, oy, ox+dx, oy+radius);
			}
			else  // coins bas/gauche et bas/droite droits ?
			{
				path.MoveTo(ox, oy);
				path.LineTo(ox+dx, oy);
			}

			if (isTopRounded)  // coins haut/gauche et haut/droite arrondis ?
			{
				path.LineTo(ox+dx, oy+dy-radius);
				path.CurveTo(ox+dx, oy+dy, ox+dx-radius, oy+dy);
				path.LineTo(ox+radius, oy+dy);
				path.CurveTo(ox, oy+dy, ox, oy+dy-radius);
			}
			else  // coins haut/gauche et haut/droite droits ?
			{
				path.LineTo(ox+dx, oy+dy);
				path.LineTo(ox, oy+dy);
			}

			path.Close();

			return path;
		}

		protected Path PathBevelRectangle(Rectangle rect, double topBevelX, double topBevelY, double bottomBevelX, double bottomBevelY, bool isTopBevel, bool isBottomBevel)
		{
			//	Retourne le chemin d'un rectangle, avec des coins biseautés en haut et/ou en bas.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			double hopeTopBevelX = System.Math.Min (topBevelX, dx/2);
			double hopeTopBevelY = System.Math.Min (topBevelY, dy/2);

			double hopeBottomBevelX = System.Math.Min (bottomBevelX, dx/2);
			double hopeBottomBevelY = System.Math.Min (bottomBevelY, dy/2);

			double scale = System.Math.Min (System.Math.Min (hopeTopBevelX/topBevelX, hopeTopBevelY/topBevelY),
											System.Math.Min (hopeBottomBevelX/bottomBevelX, hopeBottomBevelY/bottomBevelY));

			topBevelX *= scale;
			topBevelY *= scale;

			bottomBevelX *= scale;
			bottomBevelY *= scale;

			Path path = new Path ();

			if (isBottomBevel)  // coins bas/gauche et bas/droite biseautés ?
			{
				path.MoveTo (ox, oy+bottomBevelY);
				path.LineTo (ox+bottomBevelX, oy);
				path.LineTo (ox+dx-bottomBevelX, oy);
				path.LineTo (ox+dx, oy+bottomBevelY);
			}
			else  // coins bas/gauche et bas/droite droits ?
			{
				path.MoveTo (ox, oy);
				path.LineTo (ox+dx, oy);
			}

			if (isTopBevel)  // coins haut/gauche et haut/droite biseautés ?
			{
				path.LineTo (ox+dx, oy+dy-topBevelY);
				path.LineTo (ox+dx-topBevelX, oy+dy);
				path.LineTo (ox+topBevelX, oy+dy);
				path.LineTo (ox, oy+dy-topBevelY);
			}
			else  // coins haut/gauche et haut/droite droits ?
			{
				path.LineTo (ox+dx, oy+dy);
				path.LineTo (ox, oy+dy);
			}

			path.Close ();

			return path;
		}


		protected static readonly double		headerHeight = 32;
		protected static readonly double		footerHeight = 16;
		protected static readonly double		buttonRadius = 10;
		protected static readonly double		bulletRadius = 4;
		protected static readonly double		buttonSquare = 5;
		protected static readonly double		lengthStumpLink = 60;
		protected static readonly double		arrowLength = 12;
		protected static readonly double		arrowAngle = 25;
		protected static readonly double		commentMinWidth = 50;
		protected static readonly double		infoMinWidth = 50;

		protected readonly Editor				editor;
		protected readonly AbstractEntity		entity;

		protected ActiveElement					hilitedElement;
		protected MainColor						boxColor;
		protected bool							isDimmed;
	}
}
