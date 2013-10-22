//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Un popup permet de réaliser des dialogues modaux, sans qu'il soit dit.
	/// Dès que la souris quitte la surface, le popup est fermé.
	/// A la création, un popup s'attache à la fenêtre parent nommée "PopupParentFrame",
	/// qui doit remplir toute la fenêtre. Le popup lui-même occupe toute la surfce.
	/// </summary>
	public abstract class AbstractPopup : Widget
	{
		public void Create(Widget target, bool leftOrRight = false)
		{
			//	Crée le popup, dont la queue pointera vers le widget target.
			this.target = target;

			var parent = this.GetParent ();

			this.Parent = parent;
			this.Anchor = AnchorStyles.All;
			this.Name   = "PopupWidget";

			var r1 = parent.MapClientToScreen (parent.ActualBounds);
			var r2 = target.MapClientToScreen (new Rectangle (0, 0, target.ActualWidth, target.ActualHeight));

			var x = r2.Left - r1.Left;
			var y = r2.Bottom - r1.Bottom;

			this.targetRect = new Rectangle (x, y, target.ActualWidth, target.ActualHeight);
			this.targetRect.Inflate (1);

			this.InitializeDialogRect (leftOrRight);
			this.CreateUI ();
		}

		private void InitializeDialogRect(bool leftOrRight)
		{
			//	Calcule la position du popup.
			//	leftOrRight = false  ->  en haut ou en bas
			//	leftOrRight = true   ->  à gauche ou à droite
			const int margin = 10;

			if (leftOrRight)  // à gauche ou à droite ?
			{
				var y = this.targetRect.Center.Y - this.DialogSize.Height/2;

				y = System.Math.Max (y, AbstractPopup.dialogThickness + margin);
				y = System.Math.Min (y, this.Parent.ActualHeight - this.DialogSize.Height - AbstractPopup.dialogThickness - margin);
				y = System.Math.Floor (y);

				if (this.targetRect.Center.X > this.Parent.ActualWidth/2)  // popup à gauche ?
				{
					var x = this.targetRect.Left - AbstractPopup.queueLength - this.DialogSize.Width;
					this.dialogRect = new Rectangle (x, y, this.DialogSize.Width, this.DialogSize.Height);
				}
				else  // popup à droite ?
				{
					var x = this.targetRect.Right + AbstractPopup.queueLength;
					this.dialogRect = new Rectangle (x, y, this.DialogSize.Width, this.DialogSize.Height);
				}
			}
			else  // en haut ou en bas ?
			{
				var x = this.targetRect.Center.X - this.DialogSize.Width/2;

				x = System.Math.Max (x, AbstractPopup.dialogThickness + margin);
				x = System.Math.Min (x, this.Parent.ActualWidth - this.DialogSize.Width - AbstractPopup.dialogThickness - margin);
				x = System.Math.Floor (x);

				if (this.targetRect.Center.Y > this.Parent.ActualHeight/2)  // popup en dessous ?
				{
					var y = this.targetRect.Bottom - AbstractPopup.queueLength - this.DialogSize.Height;
					this.dialogRect = new Rectangle (x, y, this.DialogSize.Width, this.DialogSize.Height);
				}
				else  // popup en dessus ?
				{
					var y = this.targetRect.Top + AbstractPopup.queueLength;
					this.dialogRect = new Rectangle (x, y, this.DialogSize.Width, this.DialogSize.Height);
				}
			}

			this.initialDistance = this.Distance;
		}


		protected virtual Size DialogSize
		{
			get
			{
				return Size.Empty;
			}
		}

		protected virtual void CreateUI()
		{
		}

		protected void CreateCloseButton()
		{
			//	Crée le bouton de fermeture en haut à droite.
			const int size = 20;

			int x = (int) this.dialogRect.Right - size;
			int y = (int) this.dialogRect.Top - size;

			var button = new GlyphButton
			{
				Parent        = this,
				GlyphShape    = GlyphShape.Close,
				ButtonStyle   = ButtonStyle.ToolItem,
				Anchor        = AnchorStyles.BottomLeft,
				PreferredSize = new Size (size, size),
				Margins       = new Margins (x, 0, 0, y),
			};

			ToolTip.Default.SetToolTip (button, "Ferme la fenêtre");

			button.Clicked += delegate
			{
				this.ClosePopup ();
			};
		}

		protected StaticText CreateTitle(int dy, string text)
		{
			int x = (int) this.dialogRect.Left;
			int y = (int) (this.dialogRect.Bottom + this.dialogRect.Height - dy);

			var label = new StaticText
			{
				Parent           = this,
				Text             = text,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Anchor           = AnchorStyles.BottomLeft,
				PreferredSize    = new Size (this.dialogRect.Width, dy-4),
				Margins          = new Margins (x, 0, 0, y+4),
				BackColor        = ColorManager.SelectionColor,
			};

			new FrameBox
			{
				Parent           = this,
				Anchor           = AnchorStyles.BottomLeft,
				PreferredSize    = new Size (this.dialogRect.Width, 4),
				Margins          = new Margins (x, 0, 0, y),
				BackColor        = ColorManager.SelectionColor,
			};

			return label;
		}

		protected FrameBox CreateTitleFrame(int dy)
		{
			int x = (int) this.dialogRect.Left;
			int y = (int) (this.dialogRect.Bottom + this.dialogRect.Height - dy);

			var frame = new FrameBox
			{
				Parent           = this,
				Anchor           = AnchorStyles.BottomLeft,
				PreferredSize    = new Size (this.dialogRect.Width, dy-4),
				Margins          = new Margins (x, 0, 0, y+4),
				BackColor        = ColorManager.SelectionColor,
			};

			new FrameBox
			{
				Parent           = this,
				Anchor           = AnchorStyles.BottomLeft,
				PreferredSize    = new Size (this.dialogRect.Width, 4),
				Margins          = new Margins (x, 0, 0, y),
				BackColor        = ColorManager.SelectionColor,
			};

			return frame;
		}

		protected FrameBox CreateFullFrame()
		{
			int x = (int) this.dialogRect.Left;
			int y = (int) this.dialogRect.Bottom;

			var frame = new FrameBox
			{
				Parent           = this,
				Anchor           = AnchorStyles.BottomLeft,
				PreferredSize    = this.dialogRect.Size,
				Margins          = new Margins (x, 0, 0, y),
			};

			return frame;
		}

		protected Button CreateButton(int x, int y, int dx, int dy, string name, string text, string tooltip = null)
		{
			x += (int) this.dialogRect.Left;
			y += (int) this.dialogRect.Bottom;

			var button = new Button
			{
				Parent        = this,
				Name          = name,
				Text          = text,
				ButtonStyle   = ButtonStyle.Icon,
				Anchor        = AnchorStyles.BottomLeft,
				PreferredSize = new Size (dx, dy),
				Margins       = new Margins (x, 0, 0, y),
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (button, tooltip);
			}

			button.Clicked += delegate
			{
				this.ClosePopup ();
				this.OnButtonClicked (button.Name);
			};

			return button;
		}

		protected ColoredButton CreateItem(int x, int y, int dx, int dy, string text)
		{
			x += (int) this.dialogRect.Left;
			y += (int) this.dialogRect.Bottom;

			var button = new ColoredButton
			{
				Parent           = this,
				Text             = text,
				ContentAlignment = ContentAlignment.MiddleLeft,
				Anchor           = AnchorStyles.BottomLeft,
				PreferredSize    = new Size (dx, dy),
				Margins          = new Margins (x, 0, 0, y),
				NormalColor      = Color.Empty,
				SelectedColor    = ColorManager.SelectionColor,
				HoverColor       = ColorManager.HoverColor,
			};

			return button;
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.MessageType == MessageType.MouseDown)
			{
				this.MouseDown (pos);
			}
			else if (message.MessageType == MessageType.MouseMove)
			{
				this.MouseMove (pos);
			}
			else if (message.MessageType == MessageType.MouseUp)
			{
				this.MouseUp (pos);
			}
			else if (message.MessageType == MessageType.KeyPress)  // TODO: ne fonctionne pas toujours !
			{
				if (message.KeyCode == KeyCode.Escape)
				{
					this.ClosePopup ();
				}
			}

			message.Captured = true;
			message.Consumer = this;

			base.ProcessMessage (message, pos);
		}


		private void MouseDown(Point pos)
		{
			this.isDragging = true;
			this.lastPos = pos;
		}

		private void MouseMove(Point pos)
		{
			if (this.isDragging)
			{
				var delta = pos - this.lastPos;
				this.lastPos = pos;

				//	Déplace tous les widgets enfants.
				foreach (var widget in this.Children)
				{
					widget.Margins = new Margins (widget.Margins.Left+delta.X, 0, 0, widget.Margins.Bottom+delta.Y);
				}

				//	Déplace le rectngle du popup.
				this.dialogRect.Offset (delta);

				this.Invalidate ();
			}
		}

		private void MouseUp(Point pos)
		{
			this.isDragging = false;

			//	Un clic de la souris hors du popup le ferme.
			if (!this.ExternalRect.Contains (pos))
			{
				this.ClosePopup ();
			}
		}


		protected void ClosePopup()
		{
			var parent = this.GetParent ();
			parent.Children.Remove (this);
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.PaintFrame (graphics);
		}


		private void PaintFrame(Graphics graphics, bool smoothShadow = true)
		{
			if (smoothShadow)
			{
				//	Dessine une jolie ombre douce.
				SmoothShadow.Paint (graphics, this.ExternalRect);
			}
			else
			{
				//	Grise toute la fenêtre.
				graphics.AddFilledRectangle (this.ActualBounds);
				graphics.RenderSolid (Color.FromAlphaRgb (0.5, 0.5, 0.5, 0.5));
			}

			//	Met en évidence le bouton target à l'origine du popup.
			var rect = this.targetRect;
			rect.Deflate (0.5);
			graphics.AddRectangle (rect);
			rect.Deflate (1.0);
			graphics.AddRectangle (rect);
			graphics.RenderSolid (ColorManager.SelectionColor);

			//	Dessine le cadre jaune du popup, avec la queue.
			var alpha = this.Alpha;

			if (alpha == 1.0)
			{
				graphics.AddFilledPath (BalloonPath.GetPath (this.ExternalRect, this.targetRect, this.QueueThickness));
				graphics.RenderSolid (ColorManager.SelectionColor);
			}
			else if (alpha > 0.0)
			{
				graphics.AddFilledPath (BalloonPath.GetPath (this.ExternalRect, this.targetRect, this.QueueThickness));
				graphics.RenderSolid (Color.FromAlphaColor (alpha, ColorManager.SelectionColor));

				graphics.AddFilledRectangle (this.ExternalRect);
				graphics.RenderSolid (ColorManager.SelectionColor);
			}
			else
			{
				graphics.AddFilledRectangle (this.ExternalRect);
				graphics.RenderSolid (ColorManager.SelectionColor);
			}

			//	Dessine le fond blanc du popup.
			graphics.AddFilledRectangle (this.dialogRect);
			graphics.RenderSolid (ColorManager.GetBackgroundColor ());
		}


		private double QueueThickness
		{
			get
			{
				var delta = this.Distance - this.initialDistance + AbstractPopup.queueLength;
				return System.Math.Min (delta / 2.0, 10.0);
			}
		}

		private double Alpha
		{
			get
			{
				var delta = this.Distance - this.initialDistance;
				var factor = (delta-30.0) / 50.0;

				factor = System.Math.Max (factor, 0.0);
				factor = System.Math.Min (factor, 1.0);

				return 1.0 - factor;
			}
		}

		private double Distance
		{
			get
			{
				return BalloonPath.GetDistance (this.ExternalRect, this.targetRect);
			}
		}

		private Rectangle ExternalRect
		{
			//	Retourne le rectangle extérieur, auquel on ajoute une queue.
			get
			{
				var rect = this.dialogRect;
				rect.Inflate (AbstractPopup.dialogThickness);
				return rect;
			}
		}


		protected Widget GetParent()
		{
			//	Retourne un widget parent quelconque, dont la seule caractéristique
			//	importante est qu'il doit occuper toute la fenêtre.
			Widget parent = this.target;

			while (true)
			{
				if (parent.Name == "PopupParentFrame")
				{
					return parent;
				}

				parent = parent.Parent;
			}
		}


		#region Events handler
		private void OnButtonClicked(string name)
		{
			if (this.ButtonClicked != null)
			{
				this.ButtonClicked (this, name);
			}
		}

		public delegate void ButtonClickedEventHandler(object sender, string name);
		public event ButtonClickedEventHandler ButtonClicked;
		#endregion


		private static readonly double queueLength     = 20;
		private static readonly double dialogThickness = 5;

		private Widget							target;
		private Rectangle						dialogRect;
		private Rectangle						targetRect;

		private double							initialDistance;
		private bool							isDragging;
		private Point							lastPos;
	}
}