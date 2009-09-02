//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (ColorSample))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ColorSample permet de repr�senter une couleur rgb.
	/// </summary>
	public class ColorSample : AbstractButton, Behaviors.IDragBehaviorHost
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


		public Drawing.RichColor				Color
		{
			get
			{
				return UndefinedValue.GetValue<Drawing.RichColor> (this.GetValue (ColorSample.ColorProperty), Drawing.RichColor.Empty, true);
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

		public Drawing.Point					DragLocation
		{
			get
			{
				return Drawing.Point.Zero;
			}
		}


		public bool OnDragBegin(Drawing.Point cursor)
		{
			if (this.DragSourceEnable == false)
			{
				return false;
			}

			//	Cr�e un �chantillon utilisable pour l'op�ration de drag & drop (il
			//	va repr�senter visuellement l'�chantillon de couleur). On le place
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
				this.DragHilite (this.dragInfo.Target, this, false);
				this.dragInfo.DefineTarget (cs);
				this.DragHilite (this.dragInfo.Target, this, true);
				this.UpdateSwapping ();
			}
		}

		public void OnDragEnd()
		{
			this.DragHilite (this, this.dragInfo.Target, false);
			this.DragHilite (this.dragInfo.Target, this, false);

			if (this.dragInfo.Target != null)
			{
				if (this.dragInfo.Target != this)
				{
					if ((Message.CurrentState.IsShiftPressed) || 
						(Message.CurrentState.IsControlPressed))
					{
						Drawing.RichColor temp = this.Color;
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

		
		public override Drawing.Margins GetShapeMargins()
		{
			if (this.DragSourceFrame)
			{
				return new Drawing.Margins (5, 5, 5, 5);
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

			if ((palette != null) &&
				(sample != null) &&
				(sample != this) &&
				(mode == TabNavigationMode.ActivateOnTab))
			{
				//	Avoid recursive calls to self (sample != this)...

				return sample.AboutToGetFocus (dir, mode, out focus);
			}
			else
			{
				return base.AboutToGetFocus (dir, mode, out focus);
			}
		}

		protected override List<Widget> GetTabNavigationSiblings(TabNavigationDir dir, TabNavigationMode mode)
		{
			if (mode != TabNavigationMode.ActivateOnTab)
			{
				return base.GetTabNavigationSiblings (dir, mode);
			}

			List<Widget> list = new List<Widget> ();

			foreach (Widget widget in base.GetTabNavigationSiblings (dir, mode))
			{
				ColorSample sample = widget as ColorSample;

				if ((sample != null) &&
					(sample != this))
				{
					//	Samples which are in the same group must be skipped;
					//	they cannot be reached through the TAB key.
				}
				else
				{
					list.Add (widget);
				}
			}

			return list;
		}

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			ColorSample dragHost = this.DragHost;

			//	Est-ce que l'�v�nement clavier est re�u dans un �chantillon en
			//	cours de drag dans un DragWindow ? C'est possible, car le focus
			//	clavier change quand on montre le DragWindow.

			if ((dragHost != null) &&
				(message.IsKeyType))
			{
				//	Signalons l'�v�nement clavier � l'auteur du drag :

				dragHost.ProcessMessage (message, pos);
			}
			else
			{
				switch (message.MessageType)
				{
					case MessageType.KeyDown:
					case MessageType.KeyUp:
						if ((message.MessageType == MessageType.KeyDown) &&
							(this.ProcessKeyDown (message.KeyCode)))
						{
							message.Consumer = this;
							return;
						}

						if ((message.KeyCode == KeyCode.ShiftKey) ||
							(message.KeyCode == KeyCode.ControlKey))
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

				if ((this.DragSourceEnable == false) ||
					(!this.dragBehavior.ProcessMessage (message, pos)))
				{
					base.ProcessMessage (message, pos);
				}
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;

			if ((this.DragSourceFrame) &&
				(this.ActiveState == ActiveState.Yes))
			{
				Drawing.Rectangle r = rect;
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

				if ((this.IsFocused) &&
					(this.DragHost == null) &&
					(this.dragInfo == null))
				{
					graphics.AddRectangle (Drawing.Rectangle.Deflate (rect, 1, 1));
					graphics.RenderSolid (Drawing.Color.FromBrightness (1));
					adorner.PaintFocusBox (graphics, Drawing.Rectangle.Deflate (rect, 1, 1));
				}

				if ((this.dragInfo != null) &&
					(this.dragInfo.Color.IsValid))
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
				graphics.RenderSolid (adorner.ColorBorder);
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

		private static Drawing.Color GetHiliteColor(Drawing.Color color)
		{
			//	Trouve une couleur qui contraste avec la couleur sp�cifi�e,
			//	pour faire une mise en �vidence.

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
			//	Mise � jour apr�s un changement de mode swap d'un drag & drop.
			if (this.dragInfo != null)
			{
				if (this.dragInfo.Target != null)
				{
					bool swap = (Message.CurrentState.IsShiftPressed) || (Message.CurrentState.IsControlPressed);
					this.DragHilite (this, this.dragInfo.Target, swap);
				}
				else
				{
					this.dragInfo.Color = Drawing.RichColor.Empty;
					this.Invalidate ();
				}
			}
		}

		private ColorSample FindDropTarget(Drawing.Point mouse)
		{
			//	Cherche un widget ColorSample destinataire du drag & drop.
			
			return this.Window.Root.FindChild (this.MapClientToRoot (mouse), Widget.ChildFindMode.SkipHidden | Widget.ChildFindMode.Deep | Widget.ChildFindMode.SkipDisabled) as ColorSample;
		}

		private void DragHilite(ColorSample dst, ColorSample src, bool enable)
		{
			//	Met en �vidence le widget ColorSample destinataire du drag & drop.
			
			if ((dst == null) ||
				(src == null) ||
				(src == dst))
			{
				return;
			}

			if (enable)
			{
				Drawing.RichColor color = src.Color;

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
					dst.dragInfo.Color = Drawing.RichColor.Empty;
				}
				else
				{
					dst.dragInfo = null;
					dst.Invalidate ();
				}
			}
		}

		private void OnColorChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler (ColorSample.ColorChangedEvent);
			
			if (handler != null)
			{
				handler (this);
			}
		}


		#region DragInfo Class

		/// <summary>
		/// The <c>DragInfo</c> classe stores information needed only while drag
		/// and drop is in progress.
		/// </summary>
		private class DragInfo : System.IDisposable
		{
			public DragInfo(ColorSample host)
			{
				this.host = host;
			}

			public DragInfo(Drawing.Point cursor, ColorSample widget)
			{
				System.Diagnostics.Debug.Assert (widget.DragHost != null);

				this.host   = widget.DragHost;
				this.target = null;
				this.origin = widget.DragHost.MapClientToScreen (new Drawing.Point (-5, -5));

				this.window = new DragWindow ();
				this.window.Alpha = 1.0;
				this.window.DefineWidget (widget, new Drawing.Size (11, 11), Drawing.Margins.Zero);
				this.window.WindowLocation = this.Origin + cursor;
				this.window.Owner = widget.DragHost.Window;
				this.window.FocusWidget (widget);
				this.window.Show ();
			}

			public DragWindow Window
			{
				get
				{
					return this.window;
				}
			}

			public Drawing.Point Origin
			{
				get
				{
					return this.origin;
				}
			}

			public ColorSample Target
			{
				get
				{
					return this.target;
				}
			}

			public Drawing.RichColor Color
			{
				get
				{
					return this.color;
				}
				set
				{
					if (this.color != value)
					{
						this.color = value;
						this.host.Invalidate ();
					}
				}
			}

			public void DefineTarget(ColorSample target)
			{
				this.target = target;
			}
			

			public void DissolveAndDispose()
			{
				if (this.window != null)
				{
					this.window.DissolveAndDisposeWindow ();
					this.window = null;
				}

				this.Dispose ();
			}

			#region IDisposable Members

			public void Dispose()
			{
				if (this.window != null)
				{
					this.window.Hide ();
					this.window.Dispose ();
				}
				
				this.window = null;
				this.host   = null;
				this.target = null;
			}

			#endregion

			private ColorSample host;
			private DragWindow window;
			private Drawing.Point origin;
			private ColorSample target;
			private Drawing.RichColor color;
		}

		#endregion

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
		
		public static readonly DependencyProperty DragHostProperty         = DependencyProperty.Register ("DragHost", typeof (ColorSample), typeof (ColorSample), new DependencyPropertyMetadata ().MakeNotSerializable ());
		public static readonly DependencyProperty ColorProperty            = DependencyProperty.Register ("Color", typeof (Drawing.RichColor), typeof (ColorSample), new Helpers.VisualPropertyMetadata (Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty DragSourceFrameProperty  = DependencyProperty.Register ("DragSourceFrame", typeof (bool), typeof (ColorSample), new Helpers.VisualPropertyMetadata (false, Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty DragSourceEnableProperty = DependencyProperty.Register ("DragSourceEnable", typeof (bool), typeof (ColorSample), new DependencyPropertyMetadata (true));

		private const string					ColorChangedEvent = "ColorChanged";
		private const double					MarginSource = 4;

		private Behaviors.DragBehavior			dragBehavior;
		private DragInfo						dragInfo;
	}
}
