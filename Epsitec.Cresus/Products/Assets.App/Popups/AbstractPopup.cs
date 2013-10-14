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

		protected Button CreateButton(int x, int y, int dx, int dy, string name, string text)
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
				if (!this.CloseRect.Contains (pos))
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


		#region Painting
		private void PaintFrame(Graphics graphics)
		{
			graphics.AddFilledRectangle (this.ActualBounds);
			graphics.RenderSolid (Color.FromAlphaRgb (0.5, 0.5, 0.5, 0.5));

			var rect = this.targetRect;
			rect.Deflate (0.5);
			graphics.AddRectangle (rect);
			graphics.RenderSolid (ColorManager.SelectionColor);

			graphics.AddFilledPath (this.FramePath);
			graphics.RenderSolid (ColorManager.SelectionColor);

			graphics.AddFilledRectangle (this.dialogRect);
			graphics.RenderSolid (ColorManager.GetBackgroundColor ());
		}

		private Path FramePath
		{
			get
			{
				var path = new Path ();

				var mode    = this.GetAttachMode ();
				var himself = this.GetAttachHimself (mode);
				var other   = this.GetAttachOther (mode);

				double d = Point.Distance (himself, other);

				Rectangle bounds = this.ExternalRect;

				if (mode == AttachMode.None || himself.IsZero || other.IsZero || d <= 0)
				{
					path.AppendRectangle (bounds);
				}
				else if (mode == AttachMode.Left)
				{
					Point h1 = himself;
					Point h2 = himself;

					h1.Y -= AbstractPopup.queueThickness;
					h2.Y += AbstractPopup.queueThickness;

					if (h1.Y < bounds.Bottom)
					{
						h2.Y += bounds.Bottom-h1.Y;
						h1.Y = bounds.Bottom;
					}

					if (h2.Y > bounds.Top)
					{
						h1.Y -= h2.Y-bounds.Top;
						h2.Y = bounds.Top;
					}

					path.MoveTo (other);
					path.LineTo (h1);
					path.LineTo (bounds.BottomLeft);
					path.LineTo (bounds.BottomRight);
					path.LineTo (bounds.TopRight);
					path.LineTo (bounds.TopLeft);
					path.LineTo (h2);
					path.Close ();
				}
				else if (mode == AttachMode.Right)
				{
					Point h1 = himself;
					Point h2 = himself;

					h1.Y -= AbstractPopup.queueThickness;
					h2.Y += AbstractPopup.queueThickness;

					if (h1.Y < bounds.Bottom)
					{
						h2.Y += bounds.Bottom-h1.Y;
						h1.Y = bounds.Bottom;
					}

					if (h2.Y > bounds.Top)
					{
						h1.Y -= h2.Y-bounds.Top;
						h2.Y = bounds.Top;
					}

					path.MoveTo (other);
					path.LineTo (h1);
					path.LineTo (bounds.BottomRight);
					path.LineTo (bounds.BottomLeft);
					path.LineTo (bounds.TopLeft);
					path.LineTo (bounds.TopRight);
					path.LineTo (h2);
					path.Close ();
				}
				else if (mode == AttachMode.Bottom)
				{
					Point h1 = himself;
					Point h2 = himself;

					h1.X -= AbstractPopup.queueThickness;
					h2.X += AbstractPopup.queueThickness;

					if (h1.X < bounds.Left)
					{
						h2.X += bounds.Left-h1.X;
						h1.X = bounds.Left;
					}

					if (h2.X > bounds.Right)
					{
						h1.X -= h2.X-bounds.Right;
						h2.X = bounds.Right;
					}

					path.MoveTo (other);
					path.LineTo (h1);
					path.LineTo (bounds.BottomLeft);
					path.LineTo (bounds.TopLeft);
					path.LineTo (bounds.TopRight);
					path.LineTo (bounds.BottomRight);
					path.LineTo (h2);
					path.Close ();
				}
				else if (mode == AttachMode.Top)
				{
					Point h1 = himself;
					Point h2 = himself;

					h1.X -= AbstractPopup.queueThickness;
					h2.X += AbstractPopup.queueThickness;

					if (h1.X < bounds.Left)
					{
						h2.X += bounds.Left-h1.X;
						h1.X = bounds.Left;
					}

					if (h2.X > bounds.Right)
					{
						h1.X -= h2.X-bounds.Right;
						h2.X = bounds.Right;
					}

					path.MoveTo (other);
					path.LineTo (h1);
					path.LineTo (bounds.TopLeft);
					path.LineTo (bounds.BottomLeft);
					path.LineTo (bounds.BottomRight);
					path.LineTo (bounds.TopRight);
					path.LineTo (h2);
					path.Close ();
				}
				else if (mode == AttachMode.BottomLeft)
				{
					Point h1 = himself;
					Point h2 = himself;

					h1.Y += AbstractPopup.queueThickness*System.Math.Sqrt (2);
					h2.X += AbstractPopup.queueThickness*System.Math.Sqrt (2);

					path.MoveTo (other);
					path.LineTo (h1);
					path.LineTo (bounds.TopLeft);
					path.LineTo (bounds.TopRight);
					path.LineTo (bounds.BottomRight);
					path.LineTo (h2);
					path.Close ();
				}
				else if (mode == AttachMode.BottomRight)
				{
					Point h1 = himself;
					Point h2 = himself;

					h1.Y += AbstractPopup.queueThickness*System.Math.Sqrt (2);
					h2.X -= AbstractPopup.queueThickness*System.Math.Sqrt (2);

					path.MoveTo (other);
					path.LineTo (h1);
					path.LineTo (bounds.TopRight);
					path.LineTo (bounds.TopLeft);
					path.LineTo (bounds.BottomLeft);
					path.LineTo (h2);
					path.Close ();
				}
				else if (mode == AttachMode.TopLeft)
				{
					Point h1 = himself;
					Point h2 = himself;

					h1.Y -= AbstractPopup.queueThickness*System.Math.Sqrt (2);
					h2.X += AbstractPopup.queueThickness*System.Math.Sqrt (2);

					path.MoveTo (other);
					path.LineTo (h1);
					path.LineTo (bounds.BottomLeft);
					path.LineTo (bounds.BottomRight);
					path.LineTo (bounds.TopRight);
					path.LineTo (h2);
					path.Close ();
				}
				else if (mode == AttachMode.TopRight)
				{
					Point h1 = himself;
					Point h2 = himself;

					h1.Y -= AbstractPopup.queueThickness*System.Math.Sqrt (2);
					h2.X -= AbstractPopup.queueThickness*System.Math.Sqrt (2);

					path.MoveTo (other);
					path.LineTo (h1);
					path.LineTo (bounds.BottomRight);
					path.LineTo (bounds.BottomLeft);
					path.LineTo (bounds.TopLeft);
					path.LineTo (h2);
					path.Close ();
				}

				return path;
			}
		}

		private Point GetAttachHimself(AttachMode mode)
		{
			//	Retourne le point d'attache sur le commentaire.
			Point pos = Point.Zero;

			if (mode != AttachMode.None)
			{
				Rectangle bounds = this.ExternalRect;
				bounds.Inflate (0.5);

				if (mode == AttachMode.BottomLeft)
				{
					pos = bounds.BottomLeft;
				}

				if (mode == AttachMode.BottomRight)
				{
					pos = bounds.BottomRight;
				}

				if (mode == AttachMode.TopLeft)
				{
					pos = bounds.TopLeft;
				}

				if (mode == AttachMode.TopRight)
				{
					pos = bounds.TopRight;
				}

				if (mode == AttachMode.Left || mode == AttachMode.Right)
				{
					pos.X = (mode == AttachMode.Left) ? bounds.Left : bounds.Right;

					double miny = System.Math.Max (this.targetRect.Bottom, bounds.Bottom);
					double maxy = System.Math.Min (this.targetRect.Top, bounds.Top);

					if (miny <= maxy)
					{
						pos.Y = (miny+maxy)/2;
					}
					else
					{
						pos.Y = (bounds.Top < this.targetRect.Top) ? bounds.Top : bounds.Bottom;
					}
				}

				if (mode == AttachMode.Bottom || mode == AttachMode.Top)
				{
					pos.Y = (mode == AttachMode.Bottom) ? bounds.Bottom : bounds.Top;

					double minx = System.Math.Max (this.targetRect.Left, bounds.Left);
					double maxx = System.Math.Min (this.targetRect.Right, bounds.Right);

					if (minx <= maxx)
					{
						pos.X = (minx+maxx)/2;
					}
					else
					{
						pos.X = (bounds.Right < this.targetRect.Right) ? bounds.Right : bounds.Left;
					}
				}
			}

			return pos;
		}

		private Point GetAttachOther(AttachMode mode)
		{
			//	Retourne le point d'attache sur l'objet lié (boîte ou commentaire).
			var pos = Point.Zero;

			if (mode != AttachMode.None)
			{
				Rectangle bounds = this.ExternalRect;
				bounds.Inflate (0.5);

				if (mode == AttachMode.BottomLeft)
				{
					return this.targetRect.TopRight;
				}

				if (mode == AttachMode.BottomRight)
				{
					return this.targetRect.TopLeft;
				}

				if (mode == AttachMode.TopLeft)
				{
					return this.targetRect.BottomRight;
				}

				if (mode == AttachMode.TopRight)
				{
					return this.targetRect.BottomLeft;
				}

				Point himself = this.GetAttachHimself (mode);

				if (mode == AttachMode.Left || mode == AttachMode.Right)
				{
					pos.X = (mode == AttachMode.Left) ? this.targetRect.Right : this.targetRect.Left;

					if (himself.Y < this.targetRect.Bottom)
					{
						pos.Y = this.targetRect.Bottom;
					}
					else if (himself.Y > this.targetRect.Top)
					{
						pos.Y = this.targetRect.Top;
					}
					else
					{
						pos.Y = himself.Y;
					}
				}

				if (mode == AttachMode.Bottom || mode == AttachMode.Top)
				{
					pos.Y = (mode == AttachMode.Bottom) ? this.targetRect.Top : this.targetRect.Bottom;

					if (himself.X < this.targetRect.Left)
					{
						pos.X = this.targetRect.Left;
					}
					else if (himself.X > this.targetRect.Right)
					{
						pos.X = this.targetRect.Right;
					}
					else
					{
						pos.X = himself.X;
					}
				}
			}

			return pos;
		}

		private AttachMode GetAttachMode()
		{
			//	Cherche d'où doit partir la queue du commentaire (de quel côté).
			var insideRect = this.ExternalRect;

			if (!insideRect.IntersectsWith (this.targetRect))
			{
				if (insideRect.Bottom >= this.targetRect.Top && insideRect.Right <= this.targetRect.Left)
				{
					return AttachMode.BottomRight;
				}

				if (insideRect.Top <= this.targetRect.Bottom && insideRect.Right <= this.targetRect.Left)
				{
					return AttachMode.TopRight;
				}

				if (insideRect.Bottom >= this.targetRect.Top && insideRect.Left >= this.targetRect.Right)
				{
					return AttachMode.BottomLeft;
				}

				if (insideRect.Top <= this.targetRect.Bottom && insideRect.Left >= this.targetRect.Right)
				{
					return AttachMode.TopLeft;
				}

				if (insideRect.Bottom >= this.targetRect.Top)  // commentaire en dessus ?
				{
					return AttachMode.Bottom;
				}

				if (insideRect.Top <= this.targetRect.Bottom)  // commentaire en dessous ?
				{
					return AttachMode.Top;
				}

				if (insideRect.Left >= this.targetRect.Right)  // commentaire à droite ?
				{
					return AttachMode.Left;
				}

				if (insideRect.Right <= this.targetRect.Left)  // commentaire à gauche ?
				{
					return AttachMode.Right;
				}
			}

			return AttachMode.None;
		}


		private enum AttachMode
		{
			None,
			Left,
			Right,
			Bottom,
			Top,
			BottomLeft,
			BottomRight,
			TopLeft,
			TopRight,
		}
		#endregion


		private Rectangle CloseRect
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
		private static readonly double queueThickness  = 10;
		private static readonly double dialogThickness = 5;

		private Rectangle dialogRect;
		private Rectangle targetRect;
	}
}