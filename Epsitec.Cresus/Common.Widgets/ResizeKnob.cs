//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ResizeKnob g�re le coin inf/droite de redimensionnement d'une
	/// fen�tre.
	/// </summary>
	public class ResizeKnob : Button
	{
		public ResizeKnob()
		{
			this.ButtonStyle = ButtonStyle.Icon;
			this.AutoEngage  = true;
			
			this.InternalState |= InternalState.Engageable;

			this.MouseCursor = MouseCursor.AsSizeNWSE;
		}
		
		public ResizeKnob(string command) : this (command, null)
		{
		}
		
		public ResizeKnob(string command, string name) : this ()
		{
			this.Command = command;
			this.Name    = name;
		}
		
		public ResizeKnob(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override double					DefaultWidth
		{
			get { return 15; }
		}
		
		public override double					DefaultHeight
		{
			get { return 15; }
		}
		
		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if ( !this.IsEnabled )  return;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.isDragging = true;
					this.initialPos = this.MapClientToScreen(pos);
					this.initialBounds = this.Window.WindowBounds;
					message.Consumer = this;
					break;

				case MessageType.MouseMove:
					if ( this.isDragging )
					{
						Drawing.Rectangle rect = this.initialBounds;
						Drawing.Point move = this.MapClientToScreen(pos)-this.initialPos;
						rect.BottomRight += move;
						this.Window.WindowBounds = rect;
						message.Consumer = this;
					}
					break;

				case MessageType.MouseUp:
					this.isDragging = false;
					message.Consumer = this;
					break;
			}
		}


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			if ( (this.PaintState & WidgetState.Enabled) != 0 )
			{
				adorner.PaintGlyph(graphics, rect, this.PaintState, GlyphShape.ResizeKnob, PaintTextStyle.Button);
			}
		}


		protected bool					isDragging = false;
		protected Drawing.Point			initialPos;
		protected Drawing.Rectangle		initialBounds;
	}
}
