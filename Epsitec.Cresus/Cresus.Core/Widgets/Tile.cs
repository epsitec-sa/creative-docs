//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>Tile</c> class paints a widget with an arrow; the logic which defines
	/// how and where the arrow should be painted can be overridden by the derived
	/// classes.
	/// </summary>
	public class Tile : FrameBox, Common.Widgets.Behaviors.IDragBehaviorHost
	{
		public Tile()
		{
			this.directArrow = new TileArrow ();
			this.reverseArrow = new TileArrow ();
			this.dragBehavior = new Common.Widgets.Behaviors.DragBehavior (this, true, true);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this tile is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this tile is read only; otherwise, <c>false</c>.
		/// </value>
		public bool IsReadOnly
		{
			get;
			set;
		}


		/// <summary>
		/// Détermine le côté sur lequel s'affiche la flèche. Si la flèche n'est pas dessinée, le côté
		/// correspondant aura un vide.
		/// </summary>
		/// <value>Position de la flèche.</value>
		public Direction ArrowDirection
		{
			get
			{
				return this.arrowDirection;
			}
			set
			{
				if (this.arrowDirection != value)
				{
					this.arrowDirection = value;
					this.Invalidate ();
				}
			}
		}

		public virtual TileArrowMode ArrowMode
		{
			get
			{
				return this.arrowMode;
			}
			set
			{
				if (this.arrowMode != value)
				{
					this.arrowMode = value;
					this.Invalidate ();
				}
			}
		}

		public virtual TileArrow DirectArrow
		{
			get
			{
				return this.directArrow;
			}
		}

		public virtual TileArrow ReverseArrow
		{
			get
			{
				return this.reverseArrow;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.DragHost != null)  // tuile en cours de déplacement pendant un drag ?
			{
				Rectangle rect = this.Client.Bounds;
				rect.Width -= TileArrow.Breadth;
				rect.Deflate (0.5);

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (Color.FromName ("LightGray"));

				graphics.AddRectangle (rect);
				graphics.RenderSolid (Color.FromName ("Black"));

				return;
			}


			var arrowMode = this.ArrowMode;

			switch (arrowMode)
			{
				case TileArrowMode.None:
				case TileArrowMode.Hilite:
				case TileArrowMode.VisibleDirect:
					this.DirectArrow.Paint (graphics, this.Client.Bounds, arrowMode, this.ArrowDirection);
					break;

				case TileArrowMode.VisibleReverse:
					this.DirectArrow.Paint (graphics, this.Client.Bounds, TileArrowMode.None, this.ArrowDirection);
					break;
			}

			if (this.dragInfo != null)
			{
				Rectangle rect = this.Client.Bounds;
				rect.Deflate (2);

				graphics.AddRectangle (rect);
				graphics.RenderSolid (Color.FromName ("Red"));
			}
		}

		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			switch (this.ArrowMode)
			{
				case TileArrowMode.VisibleReverse:
					this.ReverseArrow.Paint (graphics, this.Client.Bounds, this.ArrowMode, this.ArrowDirection);
					break;
			}
		}

		#region Colors

		public static Color BorderColor
		{
			get
			{
				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
				return adorner.ColorBorder;
			}
		}

		public static Color SurfaceSummaryColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("ffffff");
			}
		}

		public static Color SurfaceEditingColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("eef6ff");
			}
		}

		public static Color SurfaceSelectedGroupingColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("d8e8fe");
			}
		}

		public static Color SurfaceSelectedContainerColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("c6defe");
			}
		}

		public static Color SurfaceHilitedColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("ffeec2");  // orange pâle
			}
		}

		public static Color ThicknessHilitedColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("ffc83c");  // orange
			}
		}

		#endregion


		#region Drag & drop

		public bool DragSourceEnable
		{
			get
			{
				return (bool) this.GetValue (Tile.DragSourceEnableProperty);
			}
			set
			{
				if (value)
				{
					this.ClearValue (Tile.DragSourceEnableProperty);
				}
				else
				{
					this.SetValue (Tile.DragSourceEnableProperty, value);
				}
			}
		}


		public Tile DragHost
		{
			get
			{
				return this.GetValue (Tile.DragHostProperty) as Tile;
			}
			set
			{
				if (value == null)
				{
					this.ClearValue (Tile.DragHostProperty);
				}
				else
				{
					this.SetValue (Tile.DragHostProperty, value);
				}
			}
		}

		private Tile FindDropTarget(Point mouse)
		{
			//	Cherche un widget Tile destinataire du drag & drop.
			Tile tile = Tile.FindChild (this.Window.Root, this.MapClientToRoot (mouse)) as Tile;

			if (tile != null && tile.IsFrozen)
			{
				//	Si on a trouvé une tuile gelée, il faut remonter jusqu'à le prochaine tuile
				//	non gelée.
				Widget widget = tile;

				while (widget.Parent != null)
				{
					widget = widget.Parent;

					if (widget is Tile && !widget.IsFrozen)
					{
						return widget as Tile;
					}
				}
			}

			return tile;
		}

		private static Widget FindChild(Widget widget, Point point)
		{
			if (widget.HasChildren == false)
			{
				return null;
			}

			Widget[] childrens = widget.Children.Widgets;

			for (int i=childrens.Length-1; i>=0; i--)
			{
				Widget children = childrens[i];

				if (children.HitTest (point))
				{
					Widget deep = Tile.FindChild (children, children.MapParentToClient (point));

					if (deep != null)
					{
						if (deep is Tile || deep.HasChildren)
						{
							return deep;
						}
					}

					return children;
				}
			}

			return null;
		}

		private void DragHilite(Tile dst, Tile src, bool enable)
		{
			//	Met en évidence le widget Tile destinataire du drag & drop.
			if (dst == null || src == null || src == dst)
			{
				return;
			}

			if (enable)
			{
				//?RichColor color = src.Color;
				RichColor color = RichColor.FromName ("Blue");

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

		protected override void ProcessMessage(Message message, Point pos)
		{
			Tile dragHost = this.DragHost;

			//	Est-ce que l'événement clavier est reçu dans un échantillon en
			//	cours de drag dans un DragWindow ? C'est possible, car le focus
			//	clavier change quand on montre le DragWindow.

			if (dragHost != null && message.IsKeyType)
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
						if ((message.MessageType == MessageType.KeyDown) &&
							(this.ProcessKeyDown (message.KeyCode)))
						{
							message.Consumer = this;
							return;
						}

						break;
				}

				if (this.DragSourceEnable == false || !this.dragBehavior.ProcessMessage (message, pos))
				{
					base.ProcessMessage (message, pos);
				}
			}
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
					//?return palette.Navigate (this, key);
					return false;
			}

			return false;
		}

		#endregion


		#region IDragBehaviorHost Members

		public Point DragLocation
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

			this.dragBeginPoint = cursor;
			return true;
		}

		public void OnDragging(DragEventArgs e)
		{
			if (this.dragInfo == null)  // drag pas véritablement commencé ?
			{
				double distance = Point.Distance (this.dragBeginPoint, e.ToPoint);
				if (distance >= Tile.dragBeginMinimalMove)  // déplacement minimal atteint ?
				{
					Tile widget = new Tile ();

					//?widget.Color = this.Color;
					widget.PreferredSize = this.ActualSize;
					widget.DragHost = this;

					if (this.dragInfo != null)
					{
						this.dragInfo.Dispose ();
						this.dragInfo = null;
					}

					this.dragInfo = new DragInfo (e.ToPoint, widget);
				}
			}
			else
			{
				this.dragInfo.Window.WindowLocation = this.dragInfo.Origin + e.Offset;

				Tile target = this.FindDropTarget (e.ToPoint);

				if (target != this.dragInfo.Target)
				{
					this.DragHilite (this.dragInfo.Target, this, false);
					this.dragInfo.DefineTarget (target);
					this.DragHilite (this.dragInfo.Target, this, true);
				}
			}
		}

		public void OnDragEnd()
		{
			if (this.dragInfo != null)
			{
				this.DragHilite (this, this.dragInfo.Target, false);
				this.DragHilite (this.dragInfo.Target, this, false);

				if (this.dragInfo.Target != null)
				{
					if (this.dragInfo.Target != this)
					{
						//?this.dragInfo.Target.Color = this.Color;
						//?this.dragInfo.Target.OnColorChanged ();
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
		}

		#endregion


		#region DragInfo Class

		/// <summary>
		/// The <c>DragInfo</c> classe stores information needed only while drag
		/// and drop is in progress.
		/// </summary>
		private class DragInfo : System.IDisposable
		{
			public DragInfo(Tile host)
			{
				this.host = host;
			}

			public DragInfo(Point cursor, Tile widget)
			{
				System.Diagnostics.Debug.Assert (widget.DragHost != null);

				this.host   = widget.DragHost;
				this.target = null;
				this.origin = widget.DragHost.MapClientToScreen (new Point (-cursor.X, -cursor.Y));

				this.window = new DragWindow ();
				this.window.Alpha = 0.5;
				this.window.DefineWidget (widget, widget.PreferredSize, Margins.Zero);
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

			public Point Origin
			{
				get
				{
					return this.origin;
				}
			}

			public Tile Target
			{
				get
				{
					return this.target;
				}
			}

			public RichColor Color
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

			public void DefineTarget(Tile target)
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

			private Tile				host;
			private DragWindow			window;
			private Point				origin;
			private Tile				target;
			private RichColor			color;
		}

		#endregion


		public static readonly DependencyProperty DragHostProperty         = DependencyProperty.Register ("DragHost", typeof (Tile), typeof (Tile), new DependencyPropertyMetadata ().MakeNotSerializable ());
		public static readonly DependencyProperty ColorProperty            = DependencyProperty.Register ("Color", typeof (RichColor), typeof (Tile), new Common.Widgets.Helpers.VisualPropertyMetadata (Common.Widgets.Helpers.VisualPropertyMetadataOptions.AffectsDisplay));
		public static readonly DependencyProperty DragSourceEnableProperty = DependencyProperty.Register ("DragSourceEnable", typeof (bool), typeof (Tile), new DependencyPropertyMetadata (true));

		private static readonly double dragBeginMinimalMove = 8;

		private readonly TileArrow								directArrow;
		private readonly TileArrow								reverseArrow;
		private readonly Common.Widgets.Behaviors.DragBehavior	dragBehavior;
		
		private Direction										arrowDirection;
		private TileArrowMode									arrowMode;
		private DragInfo										dragInfo;
		private Point											dragBeginPoint;
	}
}
