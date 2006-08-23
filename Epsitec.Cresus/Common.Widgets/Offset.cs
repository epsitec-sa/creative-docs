//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Offset permet de modifier un offset (x;y).
	/// </summary>
	public class Offset : AbstractGroup
	{
		protected enum Part
		{
			None,
			Left,
			Right,
			Down,
			Up,
			Center,
		}


		public Offset()
		{
			this.AutoEngage = true;
			this.AutoRepeat = true;
			this.InternalState |= InternalState.Engageable;
		}

		public Offset(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}


		public Point OffsetValue
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.offset = value;
			}
		}

		public double StepValue
		{
			get
			{
				return this.step;
			}
			set
			{
				this.step = value;
			}
		}


		protected Rectangle UsedRectangle
		{
			get
			{
				Rectangle rect = this.Client.Bounds;

				double dim = System.Math.Min(rect.Width, rect.Height);
				rect.Width = dim;
				rect.Height = dim;  // doit être carré

				return rect;
			}
		}

		protected Rectangle PartRectangle(Part part)
		{
			Rectangle rect = this.UsedRectangle;
			double w = rect.Width/3;
			double h = rect.Height/3;

			switch (part)
			{
				case Part.Left:
					return new Rectangle(rect.Left, rect.Center.Y-h/2, h, w);

				case Part.Right:
					return new Rectangle(rect.Right-w, rect.Center.Y-h/2, h, w);

				case Part.Down:
					return new Rectangle(rect.Center.X-w/2, rect.Bottom, h, w);

				case Part.Up:
					return new Rectangle(rect.Center.X-w/2, rect.Top-h, h, w);

				case Part.Center:
					return new Rectangle(rect.Center.X-w/2, rect.Center.Y-h/2, h, w);
			}

			return Rectangle.Empty;
		}

		protected Part PartDetect(Point pos)
		{
			Part[] parts = { Part.Center, Part.Left, Part.Right, Part.Down, Part.Up };
			foreach (Part part in parts)  // TODO: faire mieux, sans parts !
			{
				if (this.PartRectangle(part).Contains(pos))
				{
					return part;
				}
			}

			return Part.None;
		}

		protected Part HilitedPart
		{
			get
			{
				return this.hilitedPart;
			}
			set
			{
				if (this.hilitedPart != value)
				{
					this.hilitedPart = value;
					this.Invalidate();
				}
			}
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			switch (message.Type)
			{
				case MessageType.MouseDown:
					if (this.HilitedPart != Part.None)
					{
						this.mouseDown = true;
						this.offset = Point.Zero;
						this.OnOffsetValueStarting();
						this.initialPos = pos;
						this.ButtonAction(this.HilitedPart);
					}
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseMove:
					if (this.mouseDown)
					{
						if (this.HilitedPart == Part.Center)
						{
							this.offset = pos-this.initialPos;
							this.OnOffsetValueChanging();
							this.Invalidate();
						}
					}
					else
					{
						this.HilitedPart = this.PartDetect(pos);
					}
					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					this.mouseDown = false;
					message.Consumer = this;
					this.OnOffsetValueChanged();
					break;

				case MessageType.MouseLeave:
					if (!this.mouseDown)
					{
						this.HilitedPart = Part.None;
					}
					break;
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Rectangle rect;

			Color hiliteColor1 = adorner.ColorCaption;
			Color hiliteColor2 = hiliteColor1;
			hiliteColor2.A *= 0.3;  // comme hiliteColor1, mais plus transparent
			Color centerColor = Color.FromRgb(1, 0, 0);  // rouge

			if (this.mouseDown && this.hilitedPart == Part.Center)
			{
				rect = this.UsedRectangle;
				graphics.AddFilledCircle(rect.Center, rect.Width*0.5);
				graphics.RenderSolid(hiliteColor2);

				rect.Deflate(1);
				graphics.AddCircle(rect.Center, rect.Width*0.5);
				graphics.AddLine(rect.Left, rect.Center.Y, rect.Right, rect.Center.Y);
				graphics.AddLine(rect.Center.X, rect.Bottom, rect.Center.X, rect.Top);
				graphics.RenderSolid(adorner.ColorBorder);

				if (this.offset == Point.Zero)
				{
					graphics.AddFilledCircle(rect.Center, 4);
					graphics.RenderSolid(centerColor);
				}
				else
				{
					Point mark = rect.Center+this.offset;
					Point ext = Point.Move(rect.Center, mark, rect.Width*0.5);

					if (Point.Distance(rect.Center, mark) > rect.Width*0.5)
					{
						mark = Point.Move(rect.Center, mark, rect.Width*0.5);
					}

					graphics.AddLine(rect.Center, ext);
					graphics.RenderSolid(centerColor);

					graphics.AddFilledCircle(mark, 4);
					graphics.RenderSolid(centerColor);
				}
			}
			else
			{
				if (this.hilitedPart != Part.None)
				{
					rect = this.UsedRectangle;
					graphics.AddFilledCircle(rect.Center, rect.Width*0.5);
					graphics.RenderSolid(hiliteColor2);

					rect = this.PartRectangle(this.hilitedPart);
					graphics.AddFilledCircle(rect.Center, rect.Width*0.5);
					graphics.RenderSolid(hiliteColor1);
				}

				if (this.Enable)
				{
					rect = this.UsedRectangle;
					rect.Deflate(1);
					graphics.AddCircle(rect.Center, rect.Width*0.5);
					graphics.RenderSolid(adorner.ColorBorder);

					rect = this.PartRectangle(Part.Center);
					graphics.AddCircle(rect.Center, rect.Width*0.4);
					graphics.RenderSolid(adorner.ColorBorder);
				}

				rect = this.PartRectangle(Part.Left);
				rect.Inflate(2);
				adorner.PaintGlyph(graphics, rect, this.PaintState, GlyphShape.ArrowLeft, PaintTextStyle.Button);

				rect = this.PartRectangle(Part.Right);
				rect.Inflate(2);
				adorner.PaintGlyph(graphics, rect, this.PaintState, GlyphShape.ArrowRight, PaintTextStyle.Button);

				rect = this.PartRectangle(Part.Down);
				rect.Inflate(2);
				adorner.PaintGlyph(graphics, rect, this.PaintState, GlyphShape.ArrowDown, PaintTextStyle.Button);

				rect = this.PartRectangle(Part.Up);
				rect.Inflate(2);
				adorner.PaintGlyph(graphics, rect, this.PaintState, GlyphShape.ArrowUp, PaintTextStyle.Button);
			}
		}


		private void ButtonAction(Part part)
		{
			Point move = Point.Zero;

			if (part == Part.Left)
			{
				move.X = -this.step;
			}

			if (part == Part.Right)
			{
				move.X = this.step;
			}

			if (part == Part.Down)
			{
				move.Y = -this.step;
			}

			if (part == Part.Up)
			{
				move.Y = this.step;
			}

			this.offset += move;
		}


		#region Events handler
		protected virtual void OnOffsetValueStarting()
		{
			//	Génère un événement pour dire que l'offset va changer.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("OffsetValueStarting");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler OffsetValueStarting
		{
			add
			{
				this.AddUserEventHandler("OffsetValueStarting", value);
			}
			remove
			{
				this.RemoveUserEventHandler("OffsetValueStarting", value);
			}
		}

		protected virtual void OnOffsetValueChanging()
		{
			//	Génère un événement pour dire que l'offset est en train de changer.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("OffsetValueChanging");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler OffsetValueChanging
		{
			add
			{
				this.AddUserEventHandler("OffsetValueChanging", value);
			}
			remove
			{
				this.RemoveUserEventHandler("OffsetValueChanging", value);
			}
		}

		protected virtual void OnOffsetValueChanged()
		{
			//	Génère un événement pour dire que l'offset a changé.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("OffsetValueChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler OffsetValueChanged
		{
			add
			{
				this.AddUserEventHandler("OffsetValueChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("OffsetValueChanged", value);
			}
		}
		#endregion


		protected Point						offset;
		protected double					step = 1;

		protected Part						hilitedPart = Part.None;
		protected bool						mouseDown = false;
		protected Point						initialPos;
	}
}
