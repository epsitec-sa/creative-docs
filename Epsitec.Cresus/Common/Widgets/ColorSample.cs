//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (ColorSample))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ColorSample permet de représenter une couleur rgb.
	/// </summary>
	public partial class ColorSample : AbstractButton, Behaviors.IDragBehaviorHost
	{
		public ColorSample()
		{
			this.dragBehavior = new Behaviors.DragBehavior (this, true, true);
		}

		public ColorSample(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public RichColor						Color
		{
			get
			{
				return UndefinedValue.GetValue<RichColor> (this.GetValue (ColorSample.ColorProperty), RichColor.Empty, true);
			}
			set
			{
				if (value.IsEmpty)
				{
					this.ClearValue (ColorSample.ColorProperty);
				}
				else
				{
					this.SetValue (ColorSample.ColorProperty, value);
				}
			}
		}

		public bool								DragSourceFrame
		{
			get
			{
				return (bool) this.GetValue (ColorSample.DragSourceFrameProperty);
			}
			set
			{
				this.SetValue (ColorSample.DragSourceFrameProperty, value);
			}
		}

		public bool								DragSourceEnable
		{
			get
			{
				return (bool) this.GetValue (ColorSample.DragSourceEnableProperty);
			}
			set
			{
				if (value)
				{
					this.ClearValue (ColorSample.DragSourceEnableProperty);
				}
				else
				{
					this.SetValue (ColorSample.DragSourceEnableProperty, value);
				}
			}
		}

		public ColorSample						DragHost
		{
			get
			{
				return this.GetValue (ColorSample.DragHostProperty) as ColorSample;
			}
			set
			{
				if (value == null)
				{
					this.ClearValue (ColorSample.DragHostProperty);
				}
				else
				{
					this.SetValue (ColorSample.DragHostProperty, value);
				}
			}
		}


		#region IDragBehaviorHost Members

		public Point							DragLocation
		{
			get
			{
				return Point.Zero;
			}
		}


		public bool OnDragBegin(Point cursor)
		{
			if (this.DragSourceEnable == false)
			{
				return false;
			}

			//	Crée un échantillon utilisable pour l'opération de drag & drop (il
			//	va représenter visuellement l'échantillon de couleur). On le place
			//	dans un DragWindow et hop.

			ColorSample widget = new ColorSample ();

			widget.Color = this.Color;
			widget.DragHost = this;

			if (this.dragInfo != null)
			{
				this.dragInfo.Dispose ();
				this.dragInfo = null;
			}

			this.dragInfo = new DragInfo (cursor, widget);

			return true;
		}

		public void OnDragging(DragEventArgs e)
		{
			this.dragInfo.Window.WindowLocation = this.dragInfo.Origin + e.Offset;

			ColorSample cs = this.FindDropTarget (e.ToPoint);

			if (cs != this.dragInfo.Target)
			{
				ColorSample.DragHilite (this.dragInfo.Target, this, false);
				this.dragInfo.DefineTarget (cs);
				ColorSample.DragHilite (this.dragInfo.Target, this, true);
				this.UpdateSwapping ();
			}
		}

		public void OnDragEnd()
		{
			ColorSample.DragHilite (this, this.dragInfo.Target, false);
			ColorSample.DragHilite (this.dragInfo.Target, this, false);

			if (this.dragInfo.Target != null)
			{
				if (this.dragInfo.Target != this)
				{
					if (Message.CurrentState.IsShiftPressed || 
						Message.CurrentState.IsControlPressed)
					{
						RichColor temp = this.Color;
						this.Color = this.dragInfo.Target.Color;
						this.dragInfo.Target.Color = temp;

						this.OnColorChanged ();
						this.dragInfo.Target.OnColorChanged ();
					}
					else
					{
						this.dragInfo.Target.Color = this.Color;
						this.dragInfo.Target.OnColorChanged ();
					}
				}

				this.dragInfo.Dispose ();
				this.dragInfo = null;
			}
			else
			{
				this.dragInfo.DissolveAndDispose ();
				this.dragInfo = null;
			}
		}
		
		#endregion

		
		public override Margins GetShapeMargins()
		{
			if (this.DragSourceFrame)
			{
				return new Margins (5, 5, 5, 5);
			}
			else
			{
				return base.GetShapeMargins ();
			}
		}

		protected override bool AboutToGetFocus(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			ColorPalette palette = this.Parent as ColorPalette;
			ColorSample sample  = null;

			if (palette != null)
			{
				sample = palette.SelectedColorSample;
			}

			//	If this sample is inside a color palette, we let the selected
			//	sample of the palette handle the event instead :

			if (palette != null &&
				sample != null &&
				sample != this &&
				mode == TabNavigationMode.ActivateOnTab)
			{
				//	Avoid recursive calls to self (sample != this)...

				return sample.AboutToGetFocus (dir, mode, out focus);
			}
			else
			{
				return base.AboutToGetFocus (dir, mode, out focus);
			}
		}

		protected override bool FilterTabNavigationSibling(Widget sibling, TabNavigationDir dir, TabNavigationMode mode)
		{
			if ((mode == TabNavigationMode.ActivateOnTab) &&
				(sibling != this))
			{
				if (sibling is ColorSample)
				{
					//	Samples which are in the same group must be skipped;
					//	they cannot be reached through the TAB key.

					return false;
				}
			}

			return base.FilterTabNavigationSibling (sibling, dir, mode);
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			ColorSample dragHost = this.DragHost;

			//	Est-ce que l'événement clavier est reçu dans un échantillon en
			//	cours de drag dans un DragWindow ? C'est possible, car le focus
			//	clavier change quand on montre le DragWindow.

			if (dragHost != null &&
				message.IsKeyType)
			{
				//	Signalons l'événement clavier à l'auteur du drag :

				dragHost.ProcessMessage (message, pos);
			}
			else
			{
				switch (message.MessageType)
				{
					case MessageType.KeyDown:
					case MessageType.KeyUp:
						if (message.MessageType == MessageType.KeyDown &&
							this.ProcessKeyDown (message.KeyCode))
						{
							message.Consumer = this;
							return;
						}

						if (message.KeyCode == KeyCode.ShiftKey ||
							message.KeyCode == KeyCode.ControlKey)
						{
							if (this.dragInfo != null)
							{
								this.UpdateSwapping ();
								message.Consumer = this;
								return;
							}
						}
						break;
				}

				if (this.DragSourceEnable == false ||
					!this.dragBehavior.ProcessMessage (message, pos))
				{
					base.ProcessMessage (message, pos);
				}
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			if (this.DragSourceFrame &&
				this.ActiveState == ActiveState.Yes)
			{
				Rectangle r = rect;
				r.Inflate (ColorSample.MarginSource);
				graphics.AddFilledRectangle (r);
				graphics.RenderSolid (adorner.ColorCaption);
			}

			if (this.IsEnabled)
			{
				graphics.AddLine (rect.Left+0.5, rect.Bottom+0.5, rect.Right-0.5, rect.Top-0.5);
				graphics.AddLine (rect.Left+0.5, rect.Top-0.5, rect.Right-0.5, rect.Bottom+0.5);
				graphics.RenderSolid (adorner.ColorBorder);

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.Color.Basic);

				rect.Deflate (0.5, 0.5);
				graphics.AddRectangle (rect);
				graphics.RenderSolid (adorner.ColorBorder);

				if (this.IsSelected)
				{
					rect.Deflate (1, 1);
					graphics.AddRectangle (rect);
					graphics.RenderSolid (ColorSample.GetHiliteColor (this.Color.Basic));
				}

				if (this.IsFocused &&
					this.DragHost == null &&
					this.dragInfo == null)
				{
					graphics.AddRectangle (Rectangle.Deflate (rect, 1, 1));
					graphics.RenderSolid (Drawing.Color.FromBrightness (1));
					adorner.PaintFocusBox (graphics, Rectangle.Deflate (rect, 1, 1));
				}

				if (this.dragInfo != null &&
					this.dragInfo.Color.IsValid)
				{
					//	This sample is either a drop destination or a drag source with
					//	active source <-> destination swapping. Paint a smaller sample
					//	in a disk inside the original sample :
					
					rect.Deflate (1, 1);
					double radius = System.Math.Min (rect.Width, rect.Height)/2;

					graphics.AddLine (rect.Center.X-radius, rect.Center.Y, rect.Center.X+radius, rect.Center.Y);
					graphics.AddLine (rect.Center.X, rect.Center.Y-radius, rect.Center.X, rect.Center.Y+radius);
					graphics.RenderSolid (adorner.ColorBorder);

					graphics.AddFilledCircle (rect.Center, radius);
					graphics.RenderSolid (this.dragInfo.Color.Basic);

					graphics.AddCircle (rect.Center, radius);
					graphics.RenderSolid (ColorSample.GetHiliteColor (this.dragInfo.Color.Basic));
				}
			}
			else
			{
				rect.Deflate (0.5);
				graphics.AddRectangle (rect);
				graphics.RenderSolid (adorner.ColorTextFieldBorder (false));
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (this.dragInfo != null)
			{
				this.dragInfo.Dispose ();
				this.dragInfo = null;
			}
			
			base.Dispose (disposing);
		}

		protected virtual void OnColorChanged()
		{
			this.RaiseUserEvent (ColorSample.ColorChangedEvent);
		}

		private static Color GetHiliteColor(Color color)
		{
			//	Trouve une couleur qui contraste avec la couleur spécifiée,
			//	pour faire une mise en évidence.

			double h, s, v;
			color.GetHsv (out h, out s, out v);

			if ((s * color.A < 0.2) &&
				(v > 0.8 * color.A))
			{
				//	Very near to white; we can't use a white border
				//	for the hilite :

				color = Adorners.Factory.Active.ColorCaption;
			}
			else
			{
				color = Drawing.Color.FromBrightness(1);
			}
			
			return color;
		}
		
		private bool ProcessKeyDown(KeyCode key)
		{
			ColorPalette palette = this.Parent as ColorPalette;

			if (palette == null)
			{
				return false;
			}

			switch (key)
			{
				case KeyCode.ArrowUp:
				case KeyCode.ArrowDown:
				case KeyCode.ArrowLeft:
				case KeyCode.ArrowRight:
					return palette.Navigate (this, key);
			}
			
			return false;
		}

		private void UpdateSwapping()
		{
			//	Mise à jour après un changement de mode swap d'un drag & drop.
			if (this.dragInfo != null)
			{
				if (this.dragInfo.Target != null)
				{
					bool swap = (Message.CurrentState.IsShiftPressed) || (Message.CurrentState.IsControlPressed);
					ColorSample.DragHilite (this, this.dragInfo.Target, swap);
				}
				else
				{
					this.dragInfo.Color = RichColor.Empty;
					this.Invalidate ();
				}
			}
		}

		private ColorSample FindDropTarget(Point mouse)
		{
			//	Cherche un widget ColorSample destinataire du drag & drop.

			return this.Window.Root.FindChild (this.MapClientToRoot (mouse), WidgetChildFindMode.SkipHidden | WidgetChildFindMode.Deep | WidgetChildFindMode.SkipDisabled) as ColorSample;
		}

		private static void DragHilite(ColorSample dst, ColorSample src, bool enable)
		{
			//	Met en évidence le widget ColorSample destinataire du drag & drop.
			
			if ((dst == null) ||
				(src == null) ||
				(src == dst))
			{
				return;
			}

			if (enable)
			{
				RichColor color = src.Color;

				if (dst.dragInfo == null)
				{
					dst.dragInfo = new DragInfo (dst);
				}

				if (dst.dragInfo.Color != color)
				{
					dst.dragInfo.Color = color;
				}
			}
			else if (dst.dragInfo != null)
			{
				if (dst.dragInfo.Window != null)
				{
					dst.dragInfo.Color = RichColor.Empty;
				}
				else
				{
					dst.dragInfo = null;
					dst.Invalidate ();
				}
			}
		}


		public event EventHandler				ColorChanged
		{
			add
			{
				this.AddUserEventHandler (ColorSample.ColorChangedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (ColorSample.ColorChangedEvent, value);
			}
		}
		
		
		public static readonly DependencyProperty DragHostProperty         = DependencyProperty<ColorSample>.Register<ColorSample> (x => x.DragHost, new DependencyPropertyMetadata ().MakeNotSerializable ());
		public static readonly DependencyProperty ColorProperty            = DependencyProperty<ColorSample>.Register<RichColor> (x => x.Color, new Helpers.VisualPropertyMetadata (Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty DragSourceFrameProperty  = DependencyProperty<ColorSample>.Register<bool> (x => x.DragSourceFrame, new Helpers.VisualPropertyMetadata (false, Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty DragSourceEnableProperty = DependencyProperty<ColorSample>.Register<bool> (x => x.DragSourceEnable, new DependencyPropertyMetadata (true));

		private const string					ColorChangedEvent = "ColorChanged";
		private const double					MarginSource = 4;

		private readonly Behaviors.DragBehavior	dragBehavior;
		private DragInfo						dragInfo;
	}
}
