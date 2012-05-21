//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Objects
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

			this.uniqueId = this.editor.GetNextUniqueId ();

			this.colorFactory = new ColorFactory (ColorItem.Blue);
			this.hilitedElement = ActiveElement.None;

			this.magnetConstrains = new List<MagnetConstrain> ();
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

		public int UniqueId
		{
			get
			{
				return this.uniqueId;
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

		public List<MagnetConstrain> MagnetConstrains
		{
			get
			{
				return this.magnetConstrains;
			}
		}

		public virtual Rectangle Bounds
		{
			//	Retourne la boîte de l'objet.
			get
			{
				return this.bounds;
			}
			set
			{
				this.bounds = value;
				this.UpdateGeometry ();
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

		public virtual Margins RedimMargin
		{
			get
			{
				return new Margins (AbstractObject.redimMargin);
			}
		}

		public bool IsVerticalMagneted
		{
			get;
			set;
		}

		public bool IsHorizontalMagneted
		{
			get;
			set;
		}

		public virtual void Move(double dx, double dy)
		{
			//	Déplace l'objet.
		}

		protected void ChangeBoundsWidth(double posx, double minWidth)
		{
			double width = System.Math.Max (posx-this.bounds.Left, minWidth);
			width = System.Math.Floor (width/2)*2;  // la largeur doit être pair

			this.Bounds = new Rectangle (this.bounds.Left, this.bounds.Bottom, width, this.bounds.Height);
		}

		public virtual void ContextMenu()
		{
		}

		public virtual void MenuAction(string name)
		{
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
			this.DrawMagnetConstrains (graphics);
			this.DrawButtons (graphics);
		}

		protected virtual void DrawAsOriginForMagnetConstrain(Graphics graphics)
		{
			//	Dessine l'objet comme étant l'origine d'une contrainte.
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
			}
		}

		public bool ProcessDimmed(double step)
		{
			//	Fait évoluer l'intensité de l'objet. Retourne true si un changement est intervenu.
			double newIntensity;

			if (this.isDimmed && !this.isHilitedForLinkChanging)
			{
				newIntensity = System.Math.Min (this.dimmedIntensity+step, 1);  // diminue l'intensité
			}
			else
			{
				newIntensity = System.Math.Max (this.dimmedIntensity-step, 0);  // augmente l'intensité
			}

			if (this.dimmedIntensity == newIntensity)
			{
				return false;
			}
			else
			{
				this.dimmedIntensity = newIntensity;
				this.colorFactory.DimmedIntensity = this.dimmedIntensity;
				return true;
			}
		}


		public virtual List<AbstractObject> FriendObjects
		{
			get
			{
				return null;
			}
		}


		public string GetToolTipText()
		{
			//	Retourne le texte pour le tooltip.
			if (this.draggingMode == DraggingMode.None)
			{
				return this.GetToolTipText (this.hilitedElement);
			}
			else
			{
				return null;
			}
		}

		protected virtual string GetToolTipText(ActiveElement element)
		{
			return null;  // pas de tooltip
		}

		public virtual bool MouseMove(Message message, Point pos)
		{
			if (this.isMouseDown && !this.isMouseDownForDrag)
			{
				double mx = System.Math.Abs (pos.X-this.initialPos.X);
				double my = System.Math.Abs (pos.Y-this.initialPos.Y);
				double mm = System.Math.Max (mx, my);

				if (mm >= AbstractObject.minimalMove)
				{
					this.isMouseDownForDrag = true;
				}
			}

			return false;
		}

		public virtual void MouseDown(Message message, Point pos)
		{
			//	Le bouton de la souris est pressé.
			this.initialPos = pos;
			this.isMouseDown = true;
			this.isMouseDownForDrag = false;
		}

		public virtual void MouseUp(Message message, Point pos)
		{
			//	Le bouton de la souris est relâché.
			this.isMouseDown = false;
			this.isMouseDownForDrag = false;
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


		public virtual MouseCursorType MouseCursor
		{
			get
			{
				return MouseCursorType.Finger;
			}
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


		protected static  bool DetectRoundRectangle(Rectangle rect, double radius, Point pos)
		{
			//	Détecte si une position est à l'intérieur d'un rectangle arrondi.
			radius = System.Math.Min (radius, System.Math.Min (rect.Width, rect.Height) / 2);

			if (!rect.Contains (pos))
			{
				return false;
			}

			Rectangle r;

			r = rect;
			r.Deflate (radius, 0);
			if (r.Contains (pos))
			{
				return true;
			}

			r = rect;
			r.Deflate (0, radius);
			if (r.Contains (pos))
			{
				return true;
			}

			if (AbstractObject.DetectCircle (new Point (rect.Left+radius, rect.Bottom+radius), radius, pos))
			{
				return true;
			}

			if (AbstractObject.DetectCircle (new Point (rect.Left+radius, rect.Top-radius), radius, pos))
			{
				return true;
			}

			if (AbstractObject.DetectCircle (new Point (rect.Right-radius, rect.Bottom+radius), radius, pos))
			{
				return true;
			}

			if (AbstractObject.DetectCircle (new Point (rect.Right-radius, rect.Top-radius), radius, pos))
			{
				return true;
			}

			return false;
		}

		protected static bool DetectCircle(Point center, double radius, Point pos)
		{
			//	Détecte si une position est à l'intérieur d'un cercle.
			return Point.Distance (center, pos) <= radius;
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


		public void UpdateGeometry()
		{
			this.UpdateButtonsGeometry ();
			this.UpdateMagnetConstrains ();
		}


		protected virtual void CreateButtons()
		{
		}

		private void UpdateButtonsGeometry()
		{
			foreach (var button in this.buttons)
			{
				button.UpdateGeometry ();
			}
		}

		protected void UpdateButtonsState()
		{
			foreach (var button in this.buttons)
			{
				button.UpdateState ();
			}
		}

		private void DrawButtons(Graphics graphics)
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

		protected bool IsButtonEnable(ActiveElement element)
		{
			var button = this.GetButton (element);

			if (button != null)
			{
				button.UpdateState ();
				return button.State.Enable;
			}

			return false;
		}

		protected ActiveButton GetButton(ActiveElement element)
		{
			return this.buttons.Where (x => x.Element == element).FirstOrDefault ();
		}


		protected bool IsDragging
		{
			get
			{
				return this.draggingMode != DraggingMode.None || this.editor.IsEditing;
			}
		}


		#region Magnet constrains
		public Point GetCenter(Point pos)
		{
			return pos-this.draggingOffset;
		}

		private void DrawMagnetConstrains(Graphics graphics)
		{
			bool origin = false;

			foreach (var mc in this.magnetConstrains)
			{
				this.DrawMagnetConstrains (graphics, mc);
				origin |= mc.Active;
			}

			if (origin)
			{
				this.DrawAsOriginForMagnetConstrain (graphics);
			}

			//	Dessine les contraintes imposées sur l'objet.
			if (this.IsVerticalMagneted)
			{
				var rect = new Rectangle (this.bounds.Center.X-1, this.bounds.Bottom, 2, this.bounds.Height);
				rect.Inflate (0.5, 0);

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (Color.FromRgb (1, 0, 0));  // rouge
			}

			if (this.IsHorizontalMagneted)
			{
				var rect = new Rectangle (this.bounds.Left, this.bounds.Center.Y-1, this.bounds.Width, 2);
				rect.Inflate (0, 0.5);

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (Color.FromRgb (1, 0, 0));  // rouge
			}
		}

		private void DrawMagnetConstrains(Graphics graphics, MagnetConstrain mc)
		{
			if (mc.Active)
			{
				Point p1 = mc.P1 (this.editor.AreaSize);
				Point p2 = mc.P2 (this.editor.AreaSize);

				p1 = Point.GridAlign (p1, 0.5, 1);
				p2 = Point.GridAlign (p2, 0.5, 1);

				graphics.AddLine (p1, p2);
				graphics.RenderSolid (Color.FromAlphaRgb (0.3, 1, 0, 0));  // rouge semi-transparent
			}
		}

		protected virtual void UpdateMagnetConstrains()
		{
		}
		#endregion


		#region Serialize
		public virtual void Serialize(XElement xml)
		{
			xml.Add (new XAttribute ("uid", this.uniqueId));
			xml.Add (new XAttribute ("b", this.bounds.ToString ()));
			xml.Add (new XAttribute ("c", this.colorFactory.ColorItem.ToString ()));
		}

		public virtual void Deserialize(XElement xml)
		{
			this.uniqueId = (int) xml.Attribute ("uid");

			string bounds = (string) xml.Attribute ("b");
			if (!string.IsNullOrEmpty (bounds))
			{
				this.bounds = Rectangle.Parse (bounds);
			}

			string color = (string) xml.Attribute ("c");
			if (!string.IsNullOrEmpty (color))
			{
				this.colorFactory.ColorItem = (ColorItem) System.Enum.Parse (typeof (ColorItem), color);
			}
		}
		#endregion


		protected static readonly double			lengthStumpLink = 60;
		protected static readonly double			commentMinWidth = 50;
		protected static readonly double			infoMinWidth = 50;
		protected static readonly double			redimMargin = 40;
		protected static readonly double			shadowOffset = 6;
		private static readonly double				minimalMove = 3;

		protected readonly Editor					editor;
		protected AbstractEntity					entity;

		private int									uniqueId;
		protected Rectangle							bounds;
		protected readonly List<MagnetConstrain>	magnetConstrains;
		protected readonly List<ActiveButton>		buttons;
		protected ActiveElement						hilitedElement;
		protected bool								isHilitedForLinkChanging;
		private bool								isDimmed;
		private double								dimmedIntensity;
		protected ColorFactory						colorFactory;
		protected DraggingMode						draggingMode;
		protected Point								initialPos;
		private bool								isMouseDown;
		protected bool								isMouseDownForDrag;
		protected Point								draggingOffset;
	}
}
