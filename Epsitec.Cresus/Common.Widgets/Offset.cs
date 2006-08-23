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
				if (this.offset != value)
				{
					this.offset = value;
					this.OnOffsetValueChanged();
				}
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


		protected Rectangle PartRectangle(Part part)
		{
			Rectangle rect = this.Client.Bounds;
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
			foreach (Part part in parts)
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
						this.initialOffset = this.offset;
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
							this.OffsetValue = this.initialOffset + (pos-this.initialPos);
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
					break;

				case MessageType.MouseLeave:
					this.HilitedPart = Part.None;
					break;
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Rectangle rect;

			if (this.hilitedPart != Part.None)
			{
				rect = this.PartRectangle(this.hilitedPart);
				graphics.AddFilledCircle(rect.Center.X, rect.Center.Y, rect.Width*0.5, rect.Height*0.5);
				graphics.RenderSolid(adorner.ColorCaption);
			}

			rect = this.Client.Bounds;
			rect.Deflate(1);
			graphics.AddCircle(rect.Center.X, rect.Center.Y, rect.Width*0.5, rect.Height*0.5);
			graphics.RenderSolid(adorner.ColorBorder);

			rect = this.PartRectangle(Part.Center);
			graphics.AddCircle(rect.Center.X, rect.Center.Y, rect.Width*0.4, rect.Height*0.4);
			graphics.RenderSolid(adorner.ColorBorder);

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

			if (this.mouseDown && this.hilitedPart == Part.Center)
			{
				graphics.LineWidth = 3;
				graphics.AddLine(this.initialPos, this.initialPos+this.offset);
				graphics.RenderSolid(adorner.ColorCaption);
				graphics.LineWidth = 1;
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

			this.OffsetValue += move;
		}


		#region Events handler
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
		protected Point						initialOffset;
		protected Point						initialPos;
	}
}
