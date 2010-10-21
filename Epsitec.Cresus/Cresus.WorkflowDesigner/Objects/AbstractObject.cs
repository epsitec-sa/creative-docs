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

			this.colorFactory = new ColorFactory (ColorItem.Blue);
			this.hilitedElement = ActiveElement.None;

			this.buttons = new List<ActiveButton> ();
			this.CreateButtons ();
			this.UpdateButtonsState ();
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

		public ColorFactory ColorFartory
		{
			get
			{
				return this.colorFactory;
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

		public virtual Rectangle ExtendedBounds
		{
			//	Retourne la boîte de l'objet, éventuellement agrandie si l'objet est étendu.
			get
			{
				return this.Bounds;
			}
		}

		public virtual void Move(double dx, double dy)
		{
			//	Déplace l'objet.
		}

		public virtual ColorItem BackgroundColorItem
		{
			//	Couleur de fond de la boîte.
			get
			{
				return this.colorFactory.ColorItem;
			}
			set
			{
				if (this.colorFactory.ColorItem != value)
				{
					this.colorFactory.ColorItem = value;
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
					this.UpdateButtonsState ();
				}
			}
		}

		public bool IsDimmed
		{
			get
			{
				return this.isDimmed;
			}
			set
			{
				this.isDimmed = value;
				this.colorFactory.IsDimmed = this.isDimmed && !this.isHilitedForLinkChanging;
			}
		}

		public bool IsHilitedForLinkChanging
		{
			//	Indique si cet objet est mis en évidence pendant un changement de destination d'une connexion.
			get
			{
				return this.isHilitedForLinkChanging;
			}
			set
			{
				this.isHilitedForLinkChanging = value;
				this.colorFactory.IsDimmed = this.isDimmed && !this.isHilitedForLinkChanging;
			}
		}


		public virtual List<AbstractObject> FriendObjects
		{
			get
			{
				return null;
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


		public virtual void StartEdition()
		{
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


		protected void DrawEdgeShadow(Graphics graphics, Rectangle rect, int smooth, double alpha)
		{
			//	Dessine une ombre douce pour un objet edge (ObjectEdge).
			alpha /= smooth;

			for (int i=0; i<smooth; i++)
			{
				Path path = this.PathEdgeRectangle (rect);
				graphics.Rasterizer.AddSurface (path);
				graphics.RenderSolid (this.colorFactory.GetColor (0, alpha));

				rect.Deflate (1);
			}
		}

		protected void DrawNodeShadow(Graphics graphics, Rectangle rect, int smooth, double alpha)
		{
			//	Dessine une ombre douce pour un objet noeud (ObjectNode).
			alpha /= smooth;

			for (int i=0; i<smooth; i++)
			{
				Path path = this.PathNodeRectangle (rect);
				graphics.Rasterizer.AddSurface (path);
				graphics.RenderSolid (this.colorFactory.GetColor (0, alpha));

				rect.Deflate (1);
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
				graphics.RenderSolid (this.colorFactory.GetColor (0, alpha));

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

		protected Path PathEdgeRectangle(Rectangle rect)
		{
			var path = new Path ();

			double radius = System.Math.Min (rect.Width, rect.Height) / 2;
			path.AppendRoundedRectangle (rect, radius);

			return path;
		}

		protected Path PathNodeRectangle(Rectangle rect)
		{
			var path = new Path ();

			path.AppendCircle (rect.Center, rect.Width/2, rect.Height/2);

			return path;
		}

		protected Path PathRoundRectangle(Rectangle rect, double radius)
		{
			//	Retourne le chemin d'un rectangle à coins arrondis.
			var path = new Path ();

			radius = System.Math.Min (radius, System.Math.Min (rect.Width, rect.Height) / 2);
			path.AppendRoundedRectangle (rect, radius);

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


		protected virtual void CreateButtons()
		{
		}

		public void UpdateButtonsGeometry()
		{
			foreach (var button in this.buttons)
			{
				button.UpdateGeometry ();
			}
		}

		public void UpdateButtonsState()
		{
			foreach (var button in this.buttons)
			{
				button.UpdateState ();
			}
		}

		protected void DrawButtons(Graphics graphics)
		{
			foreach (var button in this.buttons)
			{
				button.Draw (graphics);
			}
		}

		protected ActiveElement DetectButtons(Point pos)
		{
			//	Détection dans l'ordre inverse du dessin !
			for (int i=this.buttons.Count-1; i>= 0; i--)
			{
				var button = this.buttons[i];

				if (button.Detect (pos))
				{
					return button.Element;
				}
			}

			return ActiveElement.None;
		}


		protected static readonly double		lengthStumpLink = 60;
		protected static readonly double		arrowLength = 12;
		protected static readonly double		arrowAngle = 25;
		protected static readonly double		commentMinWidth = 50;
		protected static readonly double		infoMinWidth = 50;

		protected readonly Editor				editor;
		protected readonly AbstractEntity		entity;

		protected List<ActiveButton>			buttons;
		protected ActiveElement					hilitedElement;
		protected bool							isHilitedForLinkChanging;
		protected bool							isDimmed;
		protected ColorFactory					colorFactory;
	}
}
