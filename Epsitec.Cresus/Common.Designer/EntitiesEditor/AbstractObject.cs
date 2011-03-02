using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
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

			BoxInside,
			BoxSources,
			BoxComment,
			BoxInfo,
			BoxParameters,
			BoxExtend,
			BoxClose,
			BoxHeader,
			BoxFieldName,
			BoxFieldType,
			BoxFieldExpression,
			BoxFieldAdd,
			BoxFieldRemove,
			BoxFieldMovable,
			BoxFieldMoving,
			BoxFieldTitle,
			BoxFieldAddInterface,
			BoxFieldRemoveInterface,
			BoxFieldGroup,
			BoxChangeWidth,
			BoxMoveColumnsSeparator1,
			BoxColor1,
			BoxColor2,
			BoxColor3,
			BoxColor4,
			BoxColor5,
			BoxColor6,
			BoxColor7,
			BoxColor8,

			ConnectionOpenLeft,
			ConnectionOpenRight,
			ConnectionClose,
			ConnectionHilited,
			ConnectionMove1,
			ConnectionMove2,
			ConnectionComment,

			CommentEdit,
			CommentMove,
			CommentWidth,
			CommentClose,
			CommentColor1,
			CommentColor2,
			CommentColor3,
			CommentColor4,
			CommentColor5,
			CommentColor6,
			CommentColor7,
			CommentColor8,
			CommentAttachToConnection,

			InfoEdit,
			InfoMove,
			InfoWidth,
			InfoClose,
		}

		public enum MainColor
		{
			Blue,
			Green,
			Red,
			Grey,
			DarkGrey,
			Yellow,
			Orange,
			Lilac,
			Purple,
		}


		public AbstractObject(Editor editor)
		{
			//	Constructeur.
			this.editor = editor;
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

		public DesignerApplication Application
		{
			get
			{
				return this.editor.Module.DesignerApplication;
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

		public int HilitedFieldRank
		{
			get
			{
				return this.hilitedFieldRank;
			}
		}

		public string GetToolTipText(Point pos)
		{
			//	Retourne le texte pour le tooltip.
			ActiveElement element;
			int fieldRank;
			this.MouseDetect(pos, out element, out fieldRank);
			return this.GetToolTipText(element, fieldRank);
		}

		protected virtual string GetToolTipText(ActiveElement element, int fieldRank)
		{
			switch (element)
			{
				case AbstractObject.ActiveElement.BoxHeader:
					return Res.Strings.Entities.Action.BoxHeader;

				case AbstractObject.ActiveElement.BoxSources:
					return Res.Strings.Entities.Action.BoxSources;

				case AbstractObject.ActiveElement.BoxComment:
					return Res.Strings.Entities.Action.BoxComment;

				case AbstractObject.ActiveElement.BoxInfo:
					return Res.Strings.Entities.Action.BoxInfo;

				case AbstractObject.ActiveElement.BoxParameters:
					//?return Res.Strings.Entities.Action.BoxParameters;

				case AbstractObject.ActiveElement.BoxColor1:
					return Res.Strings.Entities.Action.BoxColor1;

				case AbstractObject.ActiveElement.BoxColor2:
					return Res.Strings.Entities.Action.BoxColor2;

				case AbstractObject.ActiveElement.BoxColor3:
					return Res.Strings.Entities.Action.BoxColor3;

				case AbstractObject.ActiveElement.BoxColor4:
					return Res.Strings.Entities.Action.BoxColor4;

				case AbstractObject.ActiveElement.BoxColor5:
					return Res.Strings.Entities.Action.BoxColor5;

				case AbstractObject.ActiveElement.BoxColor6:
					return Res.Strings.Entities.Action.BoxColor6;

				case AbstractObject.ActiveElement.BoxColor7:
					return Res.Strings.Entities.Action.BoxColor7;

				case AbstractObject.ActiveElement.BoxColor8:
					return Res.Strings.Entities.Action.BoxColor8;

				case AbstractObject.ActiveElement.BoxExtend:
					return Res.Strings.Entities.Action.BoxExtend;

				case AbstractObject.ActiveElement.BoxClose:
					return Res.Strings.Entities.Action.BoxClose;

				case AbstractObject.ActiveElement.BoxFieldName:
					if (this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked)
					{
						if (this.IsMousePossible(element, fieldRank))
						{
							return Res.Strings.Entities.Action.BoxFieldName3;
						}
						else
						{
							return Res.Strings.Entities.Action.BoxFieldName2;
						}
					}
					else
					{
						return Res.Strings.Entities.Action.BoxFieldName1;
					}

				case AbstractObject.ActiveElement.BoxFieldType:
					if (this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked)
					{
						if (this.IsMousePossible(element, fieldRank))
						{
							return Res.Strings.Entities.Action.BoxFieldType3;
						}
						else
						{
							return Res.Strings.Entities.Action.BoxFieldType2;
						}
					}
					else
					{
						return Res.Strings.Entities.Action.BoxFieldType1;
					}

				case AbstractObject.ActiveElement.BoxFieldExpression:
					return Res.Strings.Entities.Action.BoxFieldExpression;

				case AbstractObject.ActiveElement.BoxFieldAdd:
					return Res.Strings.Entities.Action.BoxFieldAdd;

				case AbstractObject.ActiveElement.BoxFieldRemove:
					return Res.Strings.Entities.Action.BoxFieldRemove;

				case AbstractObject.ActiveElement.BoxFieldMovable:
					return Res.Strings.Entities.Action.BoxFieldMovable;

				case AbstractObject.ActiveElement.BoxFieldTitle:
					if (this.editor.CurrentModifyMode == Editor.ModifyMode.Unlocked)
					{
						return Res.Strings.Entities.Action.BoxFieldTitle2;
					}
					else
					{
						return Res.Strings.Entities.Action.BoxFieldTitle1;
					}

				case AbstractObject.ActiveElement.BoxFieldAddInterface:
					return Res.Strings.Entities.Action.BoxFieldAddInterface;

				case AbstractObject.ActiveElement.BoxFieldRemoveInterface:
					return Res.Strings.Entities.Action.BoxFieldRemoveInterface;

				case AbstractObject.ActiveElement.BoxChangeWidth:
					return Res.Strings.Entities.Action.BoxChangeWidth;

				case AbstractObject.ActiveElement.BoxMoveColumnsSeparator1:
					return Res.Strings.Entities.Action.BoxMoveColumnsSeparator;

				case AbstractObject.ActiveElement.ConnectionOpenLeft:
					return Res.Strings.Entities.Action.ConnectionOpenLeft;

				case AbstractObject.ActiveElement.ConnectionOpenRight:
					return Res.Strings.Entities.Action.ConnectionOpenRight;

				case AbstractObject.ActiveElement.ConnectionClose:
					return Res.Strings.Entities.Action.ConnectionClose;

				case AbstractObject.ActiveElement.ConnectionMove1:
				case AbstractObject.ActiveElement.ConnectionMove2:
					return Res.Strings.Entities.Action.ConnectionMove;

				case AbstractObject.ActiveElement.ConnectionComment:
					return Res.Strings.Entities.Action.ConnectionComment;

				case AbstractObject.ActiveElement.CommentEdit:
					return Res.Strings.Entities.Action.CommentEdit;

				case AbstractObject.ActiveElement.CommentMove:
					return Res.Strings.Entities.Action.CommentMove;

				case AbstractObject.ActiveElement.CommentWidth:
					return Res.Strings.Entities.Action.CommentWidth;

				case AbstractObject.ActiveElement.CommentClose:
					return Res.Strings.Entities.Action.CommentClose;

				case AbstractObject.ActiveElement.CommentColor1:
					return Res.Strings.Entities.Action.CommentColor1;

				case AbstractObject.ActiveElement.CommentColor2:
					return Res.Strings.Entities.Action.CommentColor2;

				case AbstractObject.ActiveElement.CommentColor3:
					return Res.Strings.Entities.Action.CommentColor3;

				case AbstractObject.ActiveElement.CommentColor4:
					return Res.Strings.Entities.Action.CommentColor4;

				case AbstractObject.ActiveElement.CommentColor5:
					return Res.Strings.Entities.Action.CommentColor5;

				case AbstractObject.ActiveElement.CommentColor6:
					return Res.Strings.Entities.Action.CommentColor6;

				case AbstractObject.ActiveElement.CommentColor7:
					return Res.Strings.Entities.Action.CommentColor7;

				case AbstractObject.ActiveElement.CommentColor8:
					return Res.Strings.Entities.Action.CommentColor8;

				case AbstractObject.ActiveElement.CommentAttachToConnection:
					return Res.Strings.Entities.Action.CommentAttachToConnection;

				case AbstractObject.ActiveElement.InfoMove:
					return Res.Strings.Entities.Action.InfoMove;

				case AbstractObject.ActiveElement.InfoWidth:
					return Res.Strings.Entities.Action.InfoWidth;

				case AbstractObject.ActiveElement.InfoClose:
					return Res.Strings.Entities.Action.InfoClose;
			}

			return null;  // pas de tooltip
		}

		public virtual bool MouseMove(Message message, Point pos)
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

		public virtual void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est pressé.
		}

		public virtual void MouseUp(Message message, Point pos)
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

		public virtual bool IsMousePossible(ActiveElement element, int fieldRank)
		{
			//	Indique si l'opération est possible.
			return true;
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
				AbstractObject.DrawShadow (graphics, rect, rect.Width/2, (int) (radius*0.7), 0.5);
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

		
		protected static void DrawShadow(Graphics graphics, Rectangle rect, double radius, int smooth, double alpha)
		{
			//	Dessine une ombre douce.
			alpha /= smooth;

			for (int i=0; i<smooth; i++)
			{
				Path path = AbstractObject.PathRoundRectangle (rect, radius);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(Color.FromAlphaRgb(alpha, 0, 0, 0));

				rect.Deflate(1);
				radius -= 1;
			}
		}

		public static void DrawStartingArrow(Graphics graphics, Point start, Point end, FieldRelation relation)
		{
			//	Dessine une flèche selon le type de la relation.
//-			if (relation == FieldRelation.Inclusion)
//-			{
//-				AbstractObject.DrawArrowBase(graphics, end, start);
//-			}
		}

		public static void DrawEndingArrow(Graphics graphics, Point start, Point end, FieldRelation relation, bool isPrivateRelation)
		{
			//	Dessine une flèche selon le type de la relation.
			AbstractObject.DrawArrowBase(graphics, start, end);

			if (relation == FieldRelation.Collection)
			{
				Point p1 = Point.Move(end, start, AbstractObject.arrowLength);
				Point p2 = Point.Move(end, start, AbstractObject.arrowLength*0.75);
				AbstractObject.DrawArrowBase(graphics, p1, p2);
			}

			if (!isPrivateRelation)
			{
				AbstractObject.DrawArrowStar(graphics, start, end);
			}
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
				return AbstractObject.StaticIsDarkColorMain (this.boxColor);
			}
		}

		protected static bool StaticIsDarkColorMain(MainColor boxColor)
		{
			//	Indique si la couleur pour les mises en évidence est foncée.
			return boxColor == MainColor.DarkGrey;
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
			return AbstractObject.GetColorMain (boxColor, alpha, this.isDimmed);
		}

		protected static Color GetColorMain(MainColor boxColor, double alpha, bool isDimmed)
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

			if (isDimmed)
			{
				color = AbstractObject.GetColorLighter (color, 0.3);
			}

			return color;
		}

		protected Color GetColor(double brightness, bool text = false)
		{
			return AbstractObject.GetColor (brightness, this.isDimmed, text);
		}

		protected static Color GetColor(double brightness, bool isDimmed, bool text)
		{
			//	Retourne un niveau de gris.
			Color color = Color.FromBrightness(brightness);

			if (isDimmed)
			{
				color = AbstractObject.GetColorLighter (color, text ? 0.7 : 0.3);
			}

			return color;
		}

		protected Color GetColorAdjusted(Color color, double factor)
		{
			//	Retourne une couleur ajustée, sans changer la transparence.
			if (this.IsDarkColorMain)
			{
				return AbstractObject.GetColorDarker(color, factor);
			}
			else
			{
				return AbstractObject.GetColorLighter (color, factor);
			}
		}

		private static Color GetColorLighter(Color color, double factor)
		{
			//	Retourne une couleur éclaircie, sans changer la transparence.
			return Color.FromAlphaRgb(color.A, 1-(1-color.R)*factor, 1-(1-color.G)*factor, 1-(1-color.B)*factor);
		}

		private static Color GetColorDarker(Color color, double factor)
		{
			//	Retourne une couleur assombrie, sans changer la transparence.
			factor = 0.5+(factor*0.5);
			return Color.FromAlphaRgb(color.A, color.R*factor, color.G*factor, color.B*factor);
		}


		protected static void RenderHorizontalGradient(Graphics graphics, Rectangle rect, Color leftColor, Color rightColor)
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

		protected static void RenderVerticalGradient(Graphics graphics, Rectangle rect, Color bottomColor, Color topColor)
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

		protected static Path PathRoundRectangle(Rectangle rect, double radius)
		{
			//	Retourne le chemin d'un rectangle à coins arrondis.
			return AbstractObject.PathRoundRectangle(rect, radius, true, true);  // coins arrondis partout
		}

		protected static Path PathRoundRectangle(Rectangle rect, double radius, bool isTopRounded, bool isBottomRounded)
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


		public static readonly double minAttach = 20;
		protected static readonly double headerHeight = 32;
		protected static readonly double footerHeight = 16;
		protected static readonly double buttonRadius = 10;
		protected static readonly double bulletRadius = 4;
		protected static readonly double buttonSquare = 5;
		protected static readonly double lengthClose = 30;
		protected static readonly double arrowLength = 12;
		protected static readonly double arrowAngle = 25;
		protected static readonly double commentMinWidth = 50;
		protected static readonly double infoMinWidth = 50;

		protected Editor editor;
		protected ActiveElement hilitedElement;
		protected MainColor boxColor;
		protected bool isDimmed;
		protected int hilitedFieldRank;
	}
}
