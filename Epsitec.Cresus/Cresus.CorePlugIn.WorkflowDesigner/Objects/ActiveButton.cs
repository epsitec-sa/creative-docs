//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Objects
{
	public class ActiveButton
	{
		/// <summary>
		/// Crée un bouton rond avec un glyphe.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="parentColorFactory"></param>
		/// <param name="glyph"></param>
		/// <param name="geometryUpdater"></param>
		/// <param name="stateUpdater"></param>
		public ActiveButton(ActiveElement element, ColorFactory parentColorFactory, GlyphShape glyph, System.Action<ActiveButton> geometryUpdater, System.Action<ActiveButton> stateUpdater)
		{
			this.element            = element;
			this.parentColorFactory = parentColorFactory;
			this.radius             = ActiveButton.buttonRadius;
			this.glyph              = glyph;
			this.colorFactory       = new ColorFactory (ColorItem.None);
			this.roundButton        = true;
			this.geometryUpdater    = geometryUpdater;
			this.stateUpdater       = stateUpdater;

			this.state = new ActiveButtonState ();
		}

		/// <summary>
		/// Crée un bouton rond avec un texte d'un seul caractère.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="parentColorFactory"></param>
		/// <param name="text"></param>
		/// <param name="geometryUpdater"></param>
		/// <param name="stateUpdater"></param>
		public ActiveButton(ActiveElement element, ColorFactory parentColorFactory, string text, System.Action<ActiveButton> geometryUpdater, System.Action<ActiveButton> stateUpdater)
		{
			this.element            = element;
			this.parentColorFactory = parentColorFactory;
			this.radius             = ActiveButton.buttonRadius;
			this.glyph              = GlyphShape.None;
			this.colorFactory       = new ColorFactory (ColorItem.None);
			this.text               = text;
			this.roundButton        = true;
			this.geometryUpdater    = geometryUpdater;
			this.stateUpdater       = stateUpdater;

			this.state = new ActiveButtonState ();
		}

		/// <summary>
		/// Crée un petit bouton carré pour choisir une couleur.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="parentColorFactory"></param>
		/// <param name="color"></param>
		/// <param name="geometryUpdater"></param>
		/// <param name="stateUpdater"></param>
		public ActiveButton(ActiveElement element, ColorFactory parentColorFactory, ColorItem color, System.Action<ActiveButton> geometryUpdater, System.Action<ActiveButton> stateUpdater)
		{
			this.element            = element;
			this.parentColorFactory = parentColorFactory;
			this.radius             = ActiveButton.buttonSquare;
			this.colorFactory       = new ColorFactory (color);
			this.roundButton        = false;
			this.geometryUpdater    = geometryUpdater;
			this.stateUpdater       = stateUpdater;

			this.state = new ActiveButtonState ();
		}


		public ActiveElement Element
		{
			get
			{
				return this.element;
			}
		}

		public ActiveButtonState State
		{
			get
			{
				return this.state;
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

		public ColorItem ColorItem
		{
			get
			{
				return this.colorFactory.ColorItem;
			}
			set
			{
				this.colorFactory.ColorItem = value;
			}
		}


		/// <summary>
		/// Met à jour la géométrie du bouton, c'est-à-dire sa position.
		/// </summary>
		public void UpdateGeometry()
		{
			this.geometryUpdater (this);
		}

		/// <summary>
		/// Met à jour l'état du bouton (ActiveButtonState).
		/// </summary>
		public void UpdateState()
		{
			this.stateUpdater (this);
		}

		/// <summary>
		/// Détecte si une position (généralement la souris) est dans le bouton.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public bool Detect(Point pos)
		{
			//	Il faut détecter les boutons invisibles !
			if (this.state.Detectable && !this.center.IsZero)
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

		/// <summary>
		/// Dessine le bouton.
		/// </summary>
		/// <param name="graphics"></param>
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
				else if (this.colorFactory.ColorItem != ColorItem.None)
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
			graphics.RenderSolid (this.colorFactory.GetColorMain (this.colorFactory.ColorItem, 0.8));

			graphics.AddRectangle (rect);
			graphics.RenderSolid (this.colorFactory.GetColor (0));

			if (this.state.Selected)
			{
				rect.Deflate (1);
				graphics.AddRectangle (rect);
				graphics.RenderSolid (this.colorFactory.GetColor (1));
				rect.Inflate (1);
			}

			if (this.state.Hilited)
			{
				rect.Deflate (2);
				graphics.AddRectangle (rect);
				graphics.RenderSolid (this.colorFactory.GetColor (1));
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
					colorShape = this.state.Hilited ? this.parentColorFactory.GetColor (1) : this.parentColorFactory.GetColor (0);
				}
				else
				{
					colorShape = this.parentColorFactory.GetColor (0.7);
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
					colorShape = this.state.Hilited ? this.parentColorFactory.GetColor (1) : this.parentColorFactory.GetColor (0);
				}
				else
				{
					colorShape = this.parentColorFactory.GetColor (0.7);
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

			Color colorSurface;
			Color colorFrame;

			if (this.state.Enable)
			{
				colorSurface = this.state.Hilited ? this.parentColorFactory.GetColorMain () : this.parentColorFactory.GetColor (1);
				colorFrame = this.parentColorFactory.GetColor (0);
			}
			else
			{
				colorSurface = this.parentColorFactory.GetColor (0.9);
				colorFrame   = this.parentColorFactory.GetColor (0.5);
			}

			graphics.AddFilledCircle (this.center, this.radius);
			graphics.RenderSolid (colorSurface);

			graphics.AddCircle (this.center, this.radius);
			graphics.RenderSolid (colorFrame);
		}


		public static readonly double					buttonRadius = 10;
		public static readonly double					buttonSquare = 5;

		private readonly ActiveElement					element;
		private readonly ActiveButtonState				state;
		private readonly double							radius;
		private readonly bool							roundButton;

		private readonly ColorFactory					parentColorFactory;

		private readonly System.Action<ActiveButton>	geometryUpdater;
		private readonly System.Action<ActiveButton>	stateUpdater;

		private Point									center;
		private GlyphShape								glyph;
		private string									text;
		private ColorFactory							colorFactory;
	}
}
