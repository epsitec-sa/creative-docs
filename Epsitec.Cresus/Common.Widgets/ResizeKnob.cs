//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ResizeKnob</c> class provides a special button which can be used to
	/// drag the bottom right corner of a window in order to resize it.
	/// </summary>
	public class ResizeKnob : Button
	{
		public ResizeKnob()
		{
			this.ButtonStyle = ButtonStyle.Icon;
			this.AutoEngage  = true;
			
			this.InternalState |= WidgetInternalState.Engageable;

			this.MouseCursor = MouseCursor.AsSizeNWSE;
		}
		
		public ResizeKnob(string command) : this (command, null)
		{
		}
		
		public ResizeKnob(string command, string name) : this ()
		{
			this.CommandObject = Command.Get (command);
			this.Name    = name;
		}
		
		public ResizeKnob(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		static ResizeKnob()
		{
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDx.DefineDefaultValue (15.0);
			metadataDy.DefineDefaultValue (15.0);

			Visual.PreferredWidthProperty.OverrideMetadata (typeof (ResizeKnob), metadataDx);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (ResizeKnob), metadataDy);
		}
		
		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			if ( !this.IsEnabled )  return;

			switch ( message.MessageType )
			{
				case MessageType.MouseDown:
					if (this.Window.StartWindowManagerOperation (Platform.WindowManagerOperation.ResizeBottomRight))
					{
						break;
					}
					
					this.isDragging = true;
					this.initialPos = this.MapClientToScreen(pos);
					this.initialBounds = this.Window.WindowBounds;
					this.Window.PlatformWindow.StartSizeMove ();
					message.Consumer = this;
					break;

				case MessageType.MouseMove:
					if ( this.isDragging )
					{
						Drawing.Rectangle rect = this.initialBounds;
						Drawing.Point move = this.MapClientToScreen(pos)-this.initialPos;
						rect.BottomRight += move;
						this.Window.WindowBounds = rect;
						
						this.Window.PlatformWindow.Update ();
						
						if (this.Window.Owner != null)
						{
							this.Window.Owner.PlatformWindow.Update ();
						}
						
						message.Consumer = this;
					}
					break;

				case MessageType.MouseUp:
					if (this.isDragging)
					{
						this.Window.PlatformWindow.StopSizeMove ();
						this.isDragging = false;
					}
					message.Consumer = this;
					break;
			}
		}


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			WidgetPaintState paintState = this.GetPaintState ();
			if ((paintState & WidgetPaintState.Enabled) != 0)
			{
				adorner.PaintGlyph (graphics, rect, paintState, GlyphShape.ResizeKnob, PaintTextStyle.Button);
			}
		}


		protected bool					isDragging = false;
		protected Drawing.Point			initialPos;
		protected Drawing.Rectangle		initialBounds;
	}
}
