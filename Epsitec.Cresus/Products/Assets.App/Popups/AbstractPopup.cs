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
		public void Create(Widget target)
		{
			//	Crée le popup, dont la queue pointera vers le widget target.
			var parent = AbstractPopup.GetParent (target);

			this.Parent = parent;
			this.Anchor = AnchorStyles.All;
			this.Name   = "PopupWidget";

			var r1 = parent.MapClientToScreen (parent.ActualBounds);
			var r2 = target.MapClientToScreen (new Rectangle (0, 0, target.ActualWidth, target.ActualHeight));

			var x = r2.Left - r1.Left;
			var y = r2.Bottom - r1.Bottom;

			this.targetRect = new Rectangle (x, y, target.ActualWidth, target.ActualHeight);

			this.InitializeDialogRect ();
			this.CreateUI ();
		}

		private void InitializeDialogRect()
		{
			var x = this.targetRect.Center.X - this.DialogSize.Width/2;

			x = System.Math.Max (x, AbstractPopup.dialogThickness);
			x = System.Math.Min (x, this.Parent.ActualWidth - this.DialogSize.Height - AbstractPopup.dialogThickness);

			if (this.targetRect.Center.Y > this.Parent.ActualHeight/2)
			{
				var y = this.targetRect.Bottom - AbstractPopup.queueLength - this.DialogSize.Height;
				this.dialogRect = new Rectangle (x, y, this.DialogSize.Width, this.DialogSize.Height);
			}
			else
			{
				var y = this.targetRect.Top + AbstractPopup.queueLength;
				this.dialogRect = new Rectangle (x, y, this.DialogSize.Width, this.DialogSize.Height);
			}
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
				PreferredSize    = new Size (this.dialogRect.Width, dy),
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
				PreferredSize    = new Size (this.dialogRect.Width, dy),
				Margins          = new Margins (x, 0, 0, y),
				BackColor        = ColorManager.SelectionColor,
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


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.MessageType == MessageType.MouseMove)
			{
				if (!this.TotalRect.Contains (pos))
				{
					this.ClosePopup ();
				}
			}
			else if (message.MessageType == MessageType.KeyPress)  // TODO: ne fonctionne pas !
			{
				if (message.KeyCode == KeyCode.Escape)
				{
					this.ClosePopup ();
				}
			}

			message.Captured = true;
			message.Consumer = this;
		}

		private void ClosePopup()
		{
			var parent = AbstractPopup.GetParent (this);
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
				this.PaintSmoothShadow (graphics);
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
			graphics.RenderSolid (ColorManager.SelectionColor);

			//	Dessine le cadre jaune du popup, avec la queue.
			graphics.AddFilledPath (BalloonPath.GetPath (this.ExternalRect, this.targetRect));
			graphics.RenderSolid (ColorManager.SelectionColor);

			//	Dessine le fond blanc du popup.
			graphics.AddFilledRectangle (this.dialogRect);
			graphics.RenderSolid (ColorManager.GetBackgroundColor ());
		}

		private void PaintSmoothShadow(Graphics graphics)
		{
			for (double step=0.0; step<=1.0; step+=0.02)  // 0..1
			{
				var rect = this.ExternalRect;
				rect.Offset (0, -20);
				rect.Inflate (-20 + step*80.0);

				var path = AbstractPopup.GetPathRoundRectangle (rect, 10.0 + step*80.0);
				var alpha = System.Math.Pow (1.0-step, 2.0) * 0.03;

				graphics.AddFilledPath (path);
				graphics.RenderSolid (Color.FromAlphaRgb (alpha, 0.0, 0.0, 0.0));
			}
		}

		private static Path GetPathRoundRectangle(Rectangle rect, double radius)
		{
			//	Crée le chemin d'un rectangle à coins arrondis.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			radius = System.Math.Max (radius, 0.1);

			var path = new Path ();
			path.MoveTo (ox+radius+0.5, oy+0.5);
			path.LineTo (ox+dx-radius-0.5, oy+0.5);
			path.CurveTo (ox+dx-0.5, oy+0.5, ox+dx-0.5, oy+radius+0.5);
			path.LineTo (ox+dx-0.5, oy+dy-radius-0.5);
			path.CurveTo (ox+dx-0.5, oy+dy-0.5, ox+dx-radius-0.5, oy+dy-0.5);
			path.LineTo (ox+radius+0.5, oy+dy-0.5);
			path.CurveTo (ox+0.5, oy+dy-0.5, ox+0.5, oy+dy-radius-0.5);
			path.LineTo (ox+0.5, oy+radius+0.5);
			path.CurveTo (ox+0.5, oy+0.5, ox+radius+0.5, oy+0.5);
			path.Close ();

			return path;
		}


		private Rectangle TotalRect
		{
			//	Retourne le rectangle hors duquel le popup est fermé automatiquement.
			get
			{
				var rect = this.ExternalRect;
				rect.MergeWith (this.targetRect);
				return rect;
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


		private static Widget GetParent(Widget widget)
		{
			Widget parent = widget;

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

		private Rectangle dialogRect;
		private Rectangle targetRect;
	}
}