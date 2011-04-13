//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// The <c>Tile</c> class paints a widget with an arrow; the logic which defines
	/// how and where the arrow should be painted can be overridden by the derived
	/// classes.
	/// 
	/// Résumé de l'héritage des différents widgets 'Tile':
	/// 
	/// o--Common.Widgets.FrameBox
	///    |
	///    o--Tiles.Tile
  	///       |
	///       o--Tiles.GenericTile (abstract)
  	///       |  |
	///       |  o--Tiles.EditionTile
	///       |  |
	///       |  o--Tiles.SummaryTile
  	///       |  |  |
	///       |  |  o--Tiles.CollectionItemTile
  	///       | 
	///       o--Tiles.StaticTitleTile (abstract)
  	///       |  |
	///       |  o--Tiles.PanelTitleTile
	///       |  |
	///       |  o--Tiles.TitleTile
	///       | 
	///       o--Tiles.FrameTile
	///       | 
	///       o--ArrowedFrame
	///       | 
	///       o--TilePageButton
	/// 
	/// </summary>
	public abstract class Tile : FrameBox, IReadOnly
	{
		protected Tile(Direction arrowDirection)
		{
			this.tileArrow    = new TileArrow (arrowDirection);
		}

		public virtual bool IsDraggable
		{
			get
			{
				return false;
			}
		}

		public bool Frameless
		{
			get;
			set;
		}


		/// <summary>
		/// Retourne le rectangle pour le contenu, qui exclu la zone occupée par la flèche.
		/// </summary>
		public Rectangle ContainerBounds
		{
			get
			{
				var bounds = this.Client.Bounds;
				bounds.Deflate (this.ContainerPadding);

				return bounds;
			}
		}

		/// <summary>
		/// Retourne les marges pour le contenu, qui excluent la zone occupée par la flèche.
		/// </summary>
		public Margins ContainerPadding
		{
			get
			{
				return Tile.GetContainerPadding (this.tileArrow.ArrowDirection);
			}
		}

		/// <summary>
		/// Retourne les marges pour le contenu, qui excluent la zone occupée par la flèche.
		/// </summary>
		/// <param name="arrowDirection">Position de la flèche</param>
		/// <returns>Marges sans la zone de la flèche</returns>
		public static Margins GetContainerPadding(Direction arrowDirection)
		{
			switch (arrowDirection)
			{
				case Direction.Left:
					return new Margins (TileArrow.Breadth, 0, 0, 0);

				case Direction.Right:
					return new Margins (0, TileArrow.Breadth, 0, 0);

				case Direction.Up:
					return new Margins (0, 0, TileArrow.Breadth, 0);

				case Direction.Down:
					return new Margins (0, 0, 0, TileArrow.Breadth);

				default:
					return new Margins (0);
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

		public virtual TileArrow TileArrow
		{
			get
			{
				return this.tileArrow;
			}
		}

		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.TileArrow.Paint (graphics, this.Client.Bounds, this.ArrowMode, this.Frameless);
		}



		public virtual bool IsDragAndDropEnabled
		{
			get
			{
				return false;
			}
		}

		public bool IsClickForDrag
		{
			//	Indique si l'événement Clicked doit être ignoré parce qu'il a servi à faire du drag & drop.
			//	On devrait pouvoir faire cela plus proprement (avec Swallowed ?).
			get
			{
				if (this.dragHelper != null && this.dragHelper.IsClickForDrag)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			if ((this.dragHelper == null) &&
				(this.MessageMightStartDrag (message)))
			{
				this.dragHelper = new DragHelper (this);
			}

			if ((this.dragHelper == null) ||
				(this.dragHelper.DragBehavior.ProcessMessage (message, pos) == false))
			{
				base.ProcessMessage (message, pos);
			}
		}

		private bool MessageMightStartDrag(Message message)
		{
			if ((message.MessageType == MessageType.MouseDown) &&
				(message.Button == MouseButtons.Left) &&
				(message.ButtonDownCount == 1))
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		#region IReadOnly Members

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

		#endregion

		#region DragHelper Class

		private class DragHelper : Common.Widgets.Behaviors.IDragBehaviorHost
		{
			public DragHelper(Tile tile)
			{
				this.host = tile;
				this.dragBehavior = new Common.Widgets.Behaviors.DragBehavior (this, this.host, isRelative: true, isZeroBased: true);
			}

			public Common.Widgets.Behaviors.DragBehavior DragBehavior
			{
				get
				{
					return this.dragBehavior;
				}
			}

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
				Point mouseCursor = DragHelper.MouseCursorLocation;

				this.isClickForDrag = false;

				if (this.host.GroupId == null)
				{
					return false;
				}

				this.dragBeginPoint = mouseCursor;
				this.dragBeginSize  = this.host.PreferredSize;
				
				return true;
			}

			public bool IsClickForDrag
			{
				get
				{
					return this.isClickForDrag;
				}
			}

			public void OnDragging(DragEventArgs e)
			{
				// TODO: IsSelected pas suffisant
				if (this.host.IsDraggable)
				{
					return;
				}

				Point mouseCursor = DragHelper.MouseCursorLocation;
				//?mouseCursor.X = this.dragBeginPoint.X;  // essai pour forcer un déplacement vertical

				if (this.dragWindowSource == null)
				{
					this.dragGroupId = this.host.GroupId;

					double distance = Point.Distance (this.dragBeginPoint, mouseCursor);
					if (distance >= DragHelper.dragBeginMinimalMove)  // déplacement minimal atteint ?
					{
						this.isClickForDrag = true;

						this.dragWindowSourceBeginPosition = mouseCursor;
						this.dragWindowSourceOffset = this.host.MapScreenToClient (mouseCursor);
						this.dragWindowSourceSize = this.host.ActualSize;

						this.dragErsatzTile = new ErsatzTile (Direction.Left)
						{
							Margins       = this.host.Margins,
							PreferredSize = this.dragWindowSourceSize,
							Dock          = this.host.Dock,
							Anchor        = this.host.Anchor,
						};

						this.dragErsatzIndex = this.host.Parent.Children.IndexOf (this.host);

						if (this.dragErsatzIndex != -1)
						{
							this.host.Parent.Children[this.dragErsatzIndex] = this.dragErsatzTile;  // remplace la vraie tuile (this) par l'ersatz
						}

						var box = new Tiles.FrameTile ()
						{
							PreferredSize = this.dragWindowSourceSize,
							Dock          = DockStyle.Fill,
						};

						this.host.Parent        = box;
						this.host.PreferredSize = this.dragWindowSourceSize;
						this.host.Margins       = Margins.Zero;

						//	Crée la fenêtre qui contient la tuile déplacée.
						this.dragWindowSource = new DragWindow ();
						this.dragWindowSource.Alpha = 0.8;
						this.dragWindowSource.DefineWidget (box, this.dragWindowSourceSize, Margins.Zero);
						this.dragWindowSource.WindowLocation = this.dragWindowSourceBeginPosition - this.dragWindowSourceOffset;
						this.dragWindowSource.Owner = this.dragErsatzTile.Window;
						this.dragWindowSource.FocusWidget (this.host);
						this.dragWindowSource.Show ();

						this.dragTargetMarker = new DragTargetMarker ()
						{
							MarkerColor   = Color.FromHexa ("ff6600"),  // orange vif
							PreferredSize = DragHelper.dragTargetMarkerSize,
							Dock          = DockStyle.Fill,
						};

						//	Crée la fenêtre qui contient le marqueur '>------'.
						this.dragWindowTarget = new DragWindow ();
						this.dragWindowTarget.Alpha = 1.0;
						this.dragWindowTarget.DefineWidget (this.dragTargetMarker, this.dragTargetMarker.PreferredSize, Margins.Zero);
						this.dragWindowTarget.WindowLocation = this.dragWindowSourceBeginPosition - this.dragWindowSourceOffset;
						this.dragWindowTarget.Owner = this.dragErsatzTile.Window;
						this.dragWindowTarget.FocusWidget (this.dragTargetMarker);
					}
				}
				else
				{
					this.dragWindowSource.WindowLocation = mouseCursor - this.dragWindowSourceOffset;

					Tile target = this.FindDropTarget (mouseCursor);

					if (target == null || target.GroupId != this.dragGroupId || !target.IsDraggable)
					{
						this.dragWindowTarget.Hide ();
					}
					else
					{
						Rectangle bounds = target.MapClientToScreen (target.Client.Bounds);
						bool dragOnTargetTop = mouseCursor.Y > bounds.Center.Y;

						this.dragTargetIndex = target.GroupedItemIndex;

						if (!dragOnTargetTop)
						{
							this.dragTargetIndex++;
						}

						if (this.host.GroupedItemIndex == this.dragTargetIndex ||
							this.host.GroupedItemIndex == this.dragTargetIndex-1)
						{
							this.dragWindowTarget.Hide ();
						}
						else
						{
							if (target.Margins.Bottom == -1)
							{
								bounds.Top--;
							}

							Point location;
							if (dragOnTargetTop)
							{
								location = bounds.TopLeft;
							}
							else
							{
								location = bounds.BottomLeft;
							}

							this.dragWindowTarget.WindowLocation = location - this.dragTargetMarker.HotSpot;
							this.dragWindowTarget.Show ();
						}
					}
				}
			}

			private Tile FindDropTarget(Point screenPoint)
			{
				//	Cherche un widget Tile destinataire du drag & drop.
				Widget widget = DragHelper.FindChildAtLocation (this.dragErsatzTile.Window.Root, screenPoint);

				if (widget == null)
				{
					return null;
				}

				if (!(widget is Tile) || widget.IsFrozen)
				{
					//	Si on a trouvé un widget qui n'est pas une tuile ou une tuile gelée, il faut remonter
					//	jusqu'à le prochaine tuile non gelée.
					while (widget.Parent != null)
					{
						widget = widget.Parent;

						if (widget is Tile && !widget.IsFrozen)
						{
							return widget as Tile;
						}
					}
				}

				return widget as Tile;
			}

			public void OnDragEnd()
			{
				if (this.dragWindowSource != null)
				{
					bool doDrag = this.dragWindowTarget.IsVisible;

					this.host.Margins = this.dragErsatzTile.Margins;
					this.host.Dock    = this.dragErsatzTile.Dock;
					this.host.Anchor  = this.dragErsatzTile.Anchor;
					this.host.PreferredSize = this.dragBeginSize;

					this.dragErsatzTile.Parent.Children[this.dragErsatzIndex] = this.host;  // remet la vraie tuile à sa place

					this.dragWindowSource.Hide ();
					this.dragWindowSource.Dispose ();
					this.dragWindowSource = null;

					this.dragWindowTarget.Hide ();
					this.dragWindowTarget.Dispose ();
					this.dragWindowTarget = null;

					this.dragErsatzTile = null;
					this.dragTargetMarker = null;

					if (doDrag)
					{
						this.host.GroupedItemIndex = this.dragTargetIndex;
					}
				}

				this.host.dragHelper = null;
			}

			#endregion

			private static Point MouseCursorLocation
			{
				get
				{
					var message = Message.GetLastMessage ();
					Point mouseCuror = message.Cursor;

					if (message.InWidget != null)
					{
						mouseCuror = message.InWidget.MapRootToClient (mouseCuror);
						mouseCuror = message.InWidget.MapClientToScreen (mouseCuror);
					}

					return mouseCuror;
				}
			}
			
			private static Widget FindChildAtLocation(Widget widget, Point screenPoint)
			{
				if (widget.HasChildren == false)
				{
					return null;
				}

				Widget[] children = widget.Children.Widgets;

				for (int i = children.Length-1; i >= 0; i--)
				{
					Widget child = children[i];

					Rectangle bounds = child.MapClientToScreen (child.Client.Bounds);

					if (bounds.Contains (screenPoint))
					{
						Widget deep = DragHelper.FindChildAtLocation (child, screenPoint);

						if (deep != null)
						{
							if (deep is Tile || deep.HasChildren)
							{
								return deep;
							}
						}

						return child;
					}
				}

				return null;
			}

			private static readonly double dragBeginMinimalMove = 4;
			private static readonly Size dragTargetMarkerSize = new Size (250, 21);


			private readonly Tile									host;
			private readonly Common.Widgets.Behaviors.DragBehavior	dragBehavior;
			
			private bool											isClickForDrag;
			
			private Point											dragBeginPoint;
			private Size											dragBeginSize;
			private string											dragGroupId;
			private int												dragTargetIndex;

			private DragWindow										dragWindowSource;
			private Point											dragWindowSourceBeginPosition;
			private Point											dragWindowSourceOffset;
			private Size											dragWindowSourceSize;

			private DragWindow										dragWindowTarget;

			private Tile											dragErsatzTile;
			private int												dragErsatzIndex;
			private DragTargetMarker								dragTargetMarker;
		}

		private class ErsatzTile : Tile
		{
			public ErsatzTile(Direction direction)
				: base (direction)
			{
			}

			protected override int GroupedItemIndex
			{
				get
				{
					throw new System.NotImplementedException ();
				}
				set
				{
					throw new System.NotImplementedException ();
				}
			}
			protected override string GroupId
			{
				get
				{
					throw new System.NotImplementedException ();
				}
			}
		}

		#endregion

		protected abstract int GroupedItemIndex { get; set; }

		protected abstract string GroupId { get; }



		protected readonly TileArrow							tileArrow;
		
		
		private TileArrowMode									arrowMode;
		private DragHelper										dragHelper;
	}
}
