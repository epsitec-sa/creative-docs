using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget de type 'groupe' avec un cadre.
	/// </summary>
	public class Frame : AbstractGroup
	{
		public Frame() : base()
		{
		}

		public Frame(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		static Frame()
		{
			Widgets.Helpers.VisualPropertyMetadata metadata = new Widgets.Helpers.VisualPropertyMetadata(ContentAlignment.TopLeft, Widgets.Helpers.VisualPropertyMetadataOptions.AffectsTextLayout);
			Widgets.Visual.ContentAlignmentProperty.OverrideMetadata(typeof(Frame), metadata);
		}


		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch (message.Type)
			{
				case MessageType.MouseDown:
					this.OnMouseDown(pos);
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseMove:
					this.OnMouseMove(pos);
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					this.OnMouseUp(pos);
					message.Captured = true;
					message.Consumer = this;
					break;
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le texte.
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		#region Mouse events
		protected virtual void OnMouseDown(Point pos)
		{
			EventHandler<Point> handler = (EventHandler<Point>) this.GetUserEventHandler("MouseDown");
			if (handler != null)
			{
				handler(this, pos);
			}
		}

		public event EventHandler<Point> MouseDown
		{
			add
			{
				this.AddUserEventHandler("MouseDown", value);
			}
			remove
			{
				this.RemoveUserEventHandler("MouseDown", value);
			}
		}

		protected virtual void OnMouseMove(Point pos)
		{
			EventHandler<Point> handler = (EventHandler<Point>) this.GetUserEventHandler("MouseMove");
			if (handler != null)
			{
				handler(this, pos);
			}
		}

		public event EventHandler<Point> MouseMove
		{
			add
			{
				this.AddUserEventHandler("MouseMove", value);
			}
			remove
			{
				this.RemoveUserEventHandler("MouseMove", value);
			}
		}

		protected virtual void OnMouseUp(Point pos)
		{
			EventHandler<Point> handler = (EventHandler<Point>) this.GetUserEventHandler("MouseUp");
			if (handler != null)
			{
				handler(this, pos);
			}
		}

		public event EventHandler<Point> MouseUp
		{
			add
			{
				this.AddUserEventHandler("MouseUp", value);
			}
			remove
			{
				this.RemoveUserEventHandler("MouseUp", value);
			}
		}
		#endregion
	}
}
