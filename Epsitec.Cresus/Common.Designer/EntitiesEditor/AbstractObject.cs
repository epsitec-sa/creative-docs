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
			BoxSourcesButton,
			BoxCommentButton,
			BoxExtendButton,
			BoxCloseButton,
			BoxHeaderDragging,
			BoxFieldNameSelect,
			BoxFieldTypeSelect,
			BoxFieldAdd,
			BoxFieldRemove,
			BoxFieldMovable,
			BoxFieldMoving,
			BoxChangeWidth,
			BoxMoveColumnsSeparator,
			BoxColorButton1,
			BoxColorButton2,
			BoxColorButton3,
			BoxColorButton4,

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
			CommentWidth,
			CommentClose,
			CommentColorButton1,
			CommentColorButton2,
			CommentColorButton3,
			CommentColorButton4,
			CommentColorButton5,
			CommentColorButton6,
			CommentAttachToConnection,
		}

		public enum MainColor
		{
			Blue,
			Green,
			Red,
			Grey,

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

		public string GetToolTipText(Point pos)
		{
			//	Retourne le texte pour le tooltip.
			ActiveElement element;
			int fieldRank;
			this.MouseDetect(pos, out element, out fieldRank);
			return this.GetToolTipText(element);
		}

		protected virtual string GetToolTipText(ActiveElement element)
		{
			switch (element)
			{
				case AbstractObject.ActiveElement.BoxHeaderDragging:
					return "Déplace l'entité";

				case AbstractObject.ActiveElement.BoxSourcesButton:
					return "Ouvre une entité source";

				case AbstractObject.ActiveElement.BoxCommentButton:
					return "Montre ou cache le commentaire associé";

				case AbstractObject.ActiveElement.BoxColorButton1:
					return "Entité bleue";

				case AbstractObject.ActiveElement.BoxColorButton2:
					return "Entité verte";

				case AbstractObject.ActiveElement.BoxColorButton3:
					return "Entité rouge";

				case AbstractObject.ActiveElement.BoxColorButton4:
					return "Entité grise";

				case AbstractObject.ActiveElement.BoxExtendButton:
					return "Compacte ou étend l'entité";

				case AbstractObject.ActiveElement.BoxCloseButton:
					return "Ferme l'entité";

				case AbstractObject.ActiveElement.BoxFieldNameSelect:
					return "Change le nom du champ";

				case AbstractObject.ActiveElement.BoxFieldTypeSelect:
					return "Change le type du champ";

				case AbstractObject.ActiveElement.BoxFieldAdd:
					return "Ajoute un nouveau champ";

				case AbstractObject.ActiveElement.BoxFieldRemove:
					return "Supprime le champ";

				case AbstractObject.ActiveElement.BoxFieldMovable:
					return "Change l'ordre du champ dans la liste";

				case AbstractObject.ActiveElement.BoxChangeWidth:
					return "Modifie la largeur de l'entité";

				case AbstractObject.ActiveElement.BoxMoveColumnsSeparator:
					return "Déplace le séparateur des colonnes";

				case AbstractObject.ActiveElement.ConnectionOpenLeft:
					return "Ouvre l'entité liée sur la gauche";

				case AbstractObject.ActiveElement.ConnectionOpenRight:
					return "Ouvre l'entité liée sur la droite";

				case AbstractObject.ActiveElement.ConnectionClose:
					return "Ferme l'entité";

				case AbstractObject.ActiveElement.ConnectionChangeRelation:
					return "Change le type de la relation";

				case AbstractObject.ActiveElement.ConnectionMove1:
				case AbstractObject.ActiveElement.ConnectionMove2:
					return "Modifie le routage de la relation";

				case AbstractObject.ActiveElement.ConnectionComment:
					return "Montre ou cache le commentaire associé";

				case AbstractObject.ActiveElement.CommentEdit:
					return "Modifie le texte du commentaire";

				case AbstractObject.ActiveElement.CommentMove:
					return "Déplace le commentaire";

				case AbstractObject.ActiveElement.CommentWidth:
					return "Modifie la largeur du commentaire";

				case AbstractObject.ActiveElement.CommentClose:
					return "Cache le commentaire";

				case AbstractObject.ActiveElement.CommentColorButton1:
					return "Commentaire jaune";

				case AbstractObject.ActiveElement.CommentColorButton2:
					return "Commentaire orange";

				case AbstractObject.ActiveElement.CommentColorButton3:
					return "Commentaire rouge";

				case AbstractObject.ActiveElement.CommentColorButton4:
					return "Commentaire lilas";

				case AbstractObject.ActiveElement.CommentColorButton5:
					return "Commentaire violet";

				case AbstractObject.ActiveElement.CommentColorButton6:
					return "Commentaire bleu";

				case AbstractObject.ActiveElement.CommentAttachToConnection:
					return "Déplace le point d'attache du commentaire";
			}

			return null;  // pas de tooltip
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
			this.editor.DirtySerialization = true;
			this.editor.Module.AccessEntities.IsDirty = true;
			this.editor.Module.AccessEntities.Accessor.PersistChanges();
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
			graphics.RenderSolid(Color.FromBrightness(0));

			if (selected)
			{
				rect.Deflate(1);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(1));
				rect.Inflate(1);
			}

			if (hilited)
			{
				rect.Deflate(2);
				graphics.AddRectangle(rect);
				graphics.RenderSolid(Color.FromBrightness(1));
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
			if (center.IsZero)
			{
				return;
			}

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
				colorSurface = hilited ? this.GetColorMain() : Color.FromBrightness(1);
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
			Color color = Color.FromBrightness(0.5);

			switch (boxColor)
			{
				case MainColor.Blue:
					color = Color.FromRgb(0.0/255.0, 90.0/255.0, 160.0/255.0);
					break;

				case MainColor.Green:
					color = Color.FromRgb(0.0/255.0, 130.0/255.0, 20.0/255.0);
					break;

				case MainColor.Red:
					color = Color.FromRgb(140.0/255.0, 30.0/255.0, 0.0/255.0);
					break;

				case MainColor.Grey:
					color = Color.FromRgb(100.0/255.0, 100.0/255.0, 100.0/255.0);
					break;

				case MainColor.Yellow:
					color = Color.FromRgb(200.0/255.0, 200.0/255.0, 0.0/255.0);
					break;

				case MainColor.Orange:
					color = Color.FromRgb(200.0/255.0, 150.0/255.0, 0.0/255.0);
					break;

				case MainColor.Lilac:
					color = Color.FromRgb(100.0/255.0, 0.0/255.0, 150.0/255.0);
					break;

				case MainColor.Purple:
					color = Color.FromRgb(30.0/255.0, 0.0/255.0, 200.0/255.0);
					break;
			}

			color.A = alpha;

			return color;
		}

		protected Color GetColorLighter(Color color, double factor)
		{
			//	Retourne une couleur éclaircie, sans changer la transparence.
			color.R = 1-(1-color.R)*factor;
			color.G = 1-(1-color.G)*factor;
			color.B = 1-(1-color.B)*factor;
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

		protected Editor editor;
		protected ActiveElement hilitedElement;
		protected MainColor boxColor;
		protected int hilitedFieldRank;
	}
}
