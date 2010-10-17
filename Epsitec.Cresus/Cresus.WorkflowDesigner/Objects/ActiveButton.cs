//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public class ActiveButton
	{
		public ActiveButton(ActiveElement element, ColorEngine parentColorEngine, GlyphShape glyph, System.Action<ActiveButton> geometryUpdater, System.Action<ActiveButton> stateUpdater)
		{
			this.element           = element;
			this.parentColorEngine = parentColorEngine;
			this.radius            = ActiveButton.buttonRadius;
			this.glyph             = glyph;
			this.colorEngine       = new ColorEngine (MainColor.None);
			this.roundButton       = true;
			this.geometryUpdater   = geometryUpdater;
			this.stateUpdater      = stateUpdater;

			this.state = new ActiveButtonState ();
		}

		public ActiveButton(ActiveElement element, ColorEngine parentColorEngine, string text, System.Action<ActiveButton> geometryUpdater, System.Action<ActiveButton> stateUpdater)
		{
			this.element           = element;
			this.parentColorEngine = parentColorEngine;
			this.radius            = ActiveButton.buttonRadius;
			this.glyph             = GlyphShape.None;
			this.colorEngine       = new ColorEngine (MainColor.None);
			this.text              = text;
			this.roundButton       = true;
			this.geometryUpdater   = geometryUpdater;
			this.stateUpdater      = stateUpdater;

			this.state = new ActiveButtonState ();
		}

		public ActiveButton(ActiveElement element, ColorEngine parentColorEngine, MainColor color, System.Action<ActiveButton> geometryUpdater, System.Action<ActiveButton> stateUpdater)
		{
			this.element           = element;
			this.parentColorEngine = parentColorEngine;
			this.radius            = ActiveButton.buttonSquare;
			this.colorEngine       = new ColorEngine (color);
			this.roundButton       = false;
			this.geometryUpdater   = geometryUpdater;
			this.stateUpdater      = stateUpdater;

			this.state = new ActiveButtonState ();
		}


		public ActiveButtonState State
		{
			get
			{
				return this.state;
			}
		}

		public ActiveElement Element
		{
			get
			{
				return this.element;
			}
			set
			{
				this.element = value;
			}
		}

		public Point Center
		{
			get
			{
				return this.center;
			}
			set
			{
				this.center = value;
			}
		}

		public double Radius
		{
			get
			{
				return this.radius;
			}
			set
			{
				this.radius = value;
			}
		}

		public bool Shadow
		{
			get
			{
				return this.shadow;
			}
			set
			{
				this.shadow = value;
			}
		}

		public bool RoundButton
		{
			get
			{
				return this.roundButton;
			}
			set
			{
				this.roundButton = value;
			}
		}

		public GlyphShape Glyph
		{
			get
			{
				return this.glyph;
			}
			set
			{
				this.glyph = value;
			}
		}

		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		public MainColor Color
		{
			get
			{
				return this.colorEngine.MainColor;
			}
			set
			{
				this.colorEngine.MainColor = value;
			}
		}


		public void UpdateGeometry()
		{
			this.geometryUpdater (this);
		}

		public void UpdateState()
		{
			this.stateUpdater (this);
		}

		public bool Detect(Point pos)
		{
			if (this.state.Visible && !this.center.IsZero)
			{
				if (this.roundButton)
				{
					return this.DetectRoundButton (pos);
				}
				else
				{
					return this.DetectSquareButton (pos);
				}
			}
			else
			{
				return false;
			}
		}


		public void Draw(Graphics graphics)
		{
			if (this.state.Visible && !this.center.IsZero)
			{
				if (this.glyph != GlyphShape.None)
				{
					this.DrawGlyphRoundButton (graphics);
				}
				else if (this.text != null)
				{
					this.DrawTextRoundButton (graphics);
				}
				else if (this.colorEngine.MainColor != MainColor.None)
				{
					this.DrawSquareButton (graphics);
				}
			}
		}


		private bool DetectSquareButton(Point pos)
		{
			//	Détecte si la souris est dans un bouton carré.
			if (this.center.IsZero)
			{
				return false;
			}
			else
			{
				Rectangle rect = new Rectangle (this.center.X-this.radius, this.center.Y-this.radius, this.radius*2, this.radius*2);
				rect.Inflate (0.5);
				return rect.Contains (pos);
			}
		}

		private void DrawSquareButton(Graphics graphics)
		{
			//	Dessine un bouton carré avec une couleur.
			if (this.center.IsZero)
			{
				return;
			}

			Rectangle rect = new Rectangle (this.center.X-this.radius, this.center.Y-this.radius, this.radius*2, this.radius*2);
			rect.Inflate (0.5);

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (this.colorEngine.GetColorMain (this.colorEngine.MainColor, 0.8));

			graphics.AddRectangle (rect);
			graphics.RenderSolid (this.colorEngine.GetColor (0));

			if (this.state.Selected)
			{
				rect.Deflate (1);
				graphics.AddRectangle (rect);
				graphics.RenderSolid (this.colorEngine.GetColor (1));
				rect.Inflate (1);
			}

			if (this.state.Hilited)
			{
				rect.Deflate (2);
				graphics.AddRectangle (rect);
				graphics.RenderSolid (this.colorEngine.GetColor (1));
			}
		}


		private bool DetectRoundButton(Point pos)
		{
			//	Détecte si la souris est dans un bouton circulaire.
			if (this.center.IsZero)
			{
				return false;
			}
			else
			{
				return Point.Distance (this.center, pos) <= this.radius+1;
			}
		}

		private void DrawGlyphRoundButton(Graphics graphics)
		{
			//	Dessine un bouton circulaire avec un glyph.
			if (this.center.IsZero)
			{
				return;
			}

			this.DrawEmptyRoundButton (graphics);

			if (this.glyph != GlyphShape.None)
			{
				Color colorShape;
				if (this.state.Enable)
				{
					colorShape = this.state.Hilited ? this.parentColorEngine.GetColor (1) : this.parentColorEngine.GetColor (0);
				}
				else
				{
					colorShape = this.parentColorEngine.GetColor (0.7);
				}

				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
				Rectangle rect = new Rectangle (this.center.X-this.radius, this.center.Y-this.radius, this.radius*2, this.radius*2);
				adorner.PaintGlyph (graphics, rect, WidgetPaintState.Enabled, colorShape, this.glyph, PaintTextStyle.Button);
			}
		}

		private void DrawTextRoundButton(Graphics graphics)
		{
			//	Dessine un bouton circulaire avec un texte (généralement une seule lettre).
			if (this.center.IsZero)
			{
				return;
			}

			this.DrawEmptyRoundButton (graphics);

			if (!string.IsNullOrEmpty (this.text))
			{
				Color colorShape;
				if (this.state.Enable)
				{
					colorShape = this.state.Hilited ? this.parentColorEngine.GetColor (1) : this.parentColorEngine.GetColor (0);
				}
				else
				{
					colorShape = this.parentColorEngine.GetColor (0.7);
				}

				Rectangle rect = new Rectangle (this.center.X-this.radius, this.center.Y-this.radius, this.radius*2, this.radius*2);
				double size = 14;

				if (this.text == "*")  // texte étoile pour une relation privée ?
				{
					size = 30;  // beaucoup plus grand
					rect.Offset (0, -4);  // légèrement plus bas
				}

				graphics.AddText (rect.Left, rect.Bottom+1, rect.Width, rect.Height, this.text, Font.GetFont (Font.DefaultFontFamily, "Bold"), size, ContentAlignment.MiddleCenter);
				graphics.RenderSolid (colorShape);
			}
		}

		private void DrawEmptyRoundButton(Graphics graphics)
		{
			//	Dessine un bouton circulaire vide.
			if (this.center.IsZero)
			{
				return;
			}

			if (this.shadow)
			{
				Rectangle rect = new Rectangle (this.center.X-this.radius, this.center.Y-this.radius, this.radius*2, this.radius*2);
				rect.Inflate (this.radius*0.2);
				rect.Offset (0, -this.radius*0.7);
				//?this.DrawRoundShadow (graphics, rect, rect.Width/2, (int) (this.radius*0.7), 0.5);
			}

			Color colorSurface;
			Color colorFrame;
			Color colorShape;

			if (this.state.Enable)
			{
				colorSurface = this.state.Hilited ? this.parentColorEngine.GetColorMain () : this.parentColorEngine.GetColor (1);
				colorFrame = this.parentColorEngine.GetColor (0);
				colorShape = this.state.Hilited ? this.parentColorEngine.GetColor (1) : this.parentColorEngine.GetColor (0);
			}
			else
			{
				colorSurface = this.parentColorEngine.GetColor (0.9);
				colorFrame   = this.parentColorEngine.GetColor (0.5);
				colorShape   = this.parentColorEngine.GetColor (0.7);
			}

			graphics.AddFilledCircle (this.center, this.radius);
			graphics.RenderSolid (colorSurface);

			graphics.AddCircle (this.center, this.radius);
			graphics.RenderSolid (colorFrame);
		}


		private static readonly double		buttonRadius = 10;
		private static readonly double		buttonSquare = 5;

		private readonly ActiveButtonState				state;
		private readonly System.Action<ActiveButton>	geometryUpdater;
		private readonly System.Action<ActiveButton>	stateUpdater;

		private ActiveElement				element;
		private Point						center;
		private double						radius;
		private bool						shadow;
		private bool						roundButton;
		private GlyphShape					glyph;
		private string						text;
		private ColorEngine					colorEngine;
		private ColorEngine					parentColorEngine;
	}
}
