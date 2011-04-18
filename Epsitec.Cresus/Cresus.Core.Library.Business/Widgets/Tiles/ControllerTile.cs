//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	public class ControllerTile : Tile
	{
		protected ControllerTile(Direction direction)
			: base (direction)
		{
		}

		public virtual ITileController			Controller
		{
			get
			{
				return null;
			}
			set
			{
				throw new System.InvalidOperationException ("ControllerTile.Controller is read-only");
			}
		}
		
		public virtual bool						IsDraggable
		{
			get
			{
				if ((this.Controller == null) ||
					(this.IsSelected))
				{
					return false;
				}
				else
				{
					return this.IsDragAndDropEnabled;
				}
			}
		}

		protected virtual bool					IsDragAndDropEnabled
		{
			get
			{
				return false;
			}
		}

		private int								GroupedItemIndex
		{
			get
			{
				var grouped = this.Controller as IGroupedItem;

				if (grouped == null)
				{
					return -1;
				}
				else
				{
					return grouped.GroupedItemIndex;
				}
			}
			set
			{
				var grouped = this.Controller as IGroupedItem;

				if (grouped != null)
				{
					grouped.GroupedItemIndex = value;
				}
			}
		}

		private string							GroupId
		{
			get
			{
				var grouped = this.Controller as IGroupedItem;

				if (grouped == null)
				{
					return null;
				}
				else
				{
					return grouped.GetGroupId ();
				}
			}
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			if ((this.dragHelper == null) &&
				(ControllerTile.MessageMightStartDrag (message)))
			{
				this.dragHelper = new DragHelper (this);
			}

			if ((this.dragHelper == null) ||
				(this.dragHelper.DragBehavior.ProcessMessage (message, pos) == false))
			{
				base.ProcessMessage (message, pos);
			}
		}

		private static bool MessageMightStartDrag(Message message)
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


		#region DragHelper Class

		private class DragHelper : Common.Widgets.Behaviors.IDragBehaviorHost
		{
			public DragHelper(ControllerTile tile)
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

				if (this.host.GroupId == null)
				{
					return false;
				}

				this.dragBeginPoint = mouseCursor;
				this.dragBeginSize  = this.host.PreferredSize;

				return true;
			}

			public void OnDragging(DragEventArgs e)
			{
				// TODO: IsSelected pas suffisant
				if (this.host.IsDraggable == false)
				{
					return;
				}

				Point mouseCursor = DragHelper.MouseCursorLocation;
				//?mouseCursor.X = this.dragBeginPoint.X;  // essai pour forcer un déplacement vertical

				if (this.dragWindow == null)
				{
					this.dragGroupId = this.host.GroupId;

					double distance = Point.Distance (this.dragBeginPoint, mouseCursor);
					if (distance >= DragHelper.dragBeginMinimalMove)  // déplacement minimal atteint ?
					{
						this.dragWindowSourceBeginPosition = mouseCursor;
						this.dragWindowSourceOffset = this.host.MapScreenToClient (mouseCursor);
						this.dragWindowSize = this.host.ActualSize;

						this.dragErsatzTile = new ErsatzTile ()
						{
							Margins       = this.host.Margins,
							PreferredSize = this.dragWindowSize,
							Dock          = this.host.Dock,
							Anchor        = this.host.Anchor,
						};

						this.dragErsatzIndex = this.host.Parent.Children.IndexOf (this.host);

						if (this.dragErsatzIndex != -1)
						{
							this.host.Parent.Children[this.dragErsatzIndex] = this.dragErsatzTile;  // remplace la vraie tuile (this) par l'ersatz
						}

						var box = new DragTile ()
						{
							PreferredSize = this.dragWindowSize,
							Dock          = DockStyle.Fill,
						};

						this.host.Parent        = box;
						this.host.PreferredSize = this.dragWindowSize;
						this.host.Margins       = Margins.Zero;

						//	Crée la fenêtre qui contient la tuile déplacée.
						this.dragWindow = new DragWindow ();
						this.dragWindow.Alpha = 0.8;
						this.dragWindow.DefineWidget (box, this.dragWindowSize, Margins.Zero);
						this.dragWindow.WindowLocation = this.dragWindowSourceBeginPosition - this.dragWindowSourceOffset;
						this.dragWindow.Owner = this.dragErsatzTile.Window;
						this.dragWindow.FocusWidget (this.host);
						this.dragWindow.Show ();

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
					this.dragWindow.WindowLocation = mouseCursor - this.dragWindowSourceOffset;

					var target = this.FindDropTarget (mouseCursor);

					if (target is ErsatzTile || target == null || target.GroupId != this.dragGroupId || !target.IsDraggable)
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

			private ControllerTile FindDropTarget(Point screenPoint)
			{
				//	Cherche un widget Tile destinataire du drag & drop.
				Widget widget = DragHelper.FindChildAtLocation (this.dragErsatzTile.Window.Root, screenPoint);

				if (widget == null)
				{
					return null;
				}

				if (!(widget is ControllerTile) || widget.IsFrozen)
				{
					//	Si on a trouvé un widget qui n'est pas une tuile ou une tuile gelée, il faut remonter
					//	jusqu'à le prochaine tuile non gelée.
					while (widget.Parent != null)
					{
						widget = widget.Parent;

						if (widget is ControllerTile && !widget.IsFrozen)
						{
							return widget as ControllerTile;
						}
					}
				}

				return widget as ControllerTile;
			}

			public void OnDragEnd()
			{
				if (this.dragWindow != null)
				{
					bool doDrag = this.dragWindowTarget.IsVisible;

					this.host.Margins = this.dragErsatzTile.Margins;
					this.host.Dock    = this.dragErsatzTile.Dock;
					this.host.Anchor  = this.dragErsatzTile.Anchor;
					this.host.PreferredSize = this.dragBeginSize;

					this.dragErsatzTile.Parent.Children[this.dragErsatzIndex] = this.host;  // remet la vraie tuile à sa place

					this.dragWindow.Hide ();
					this.dragWindow.Dispose ();
					this.dragWindow = null;

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
							if (deep is ControllerTile || deep.HasChildren)
							{
								return deep;
							}
						}

						return child;
					}
				}

				return null;
			}

			#region ErsatzTile Class

			private class ErsatzTile : Tile
			{
				public ErsatzTile(Direction direction = Direction.Left)
					: base (direction)
				{
				}
			}

			#endregion

			/// <summary>
			/// Tuile simple (sans flèche) avec toujours un cadre et un fond neutre.
			/// </summary>
			private class DragTile : ErsatzTile
			{
				public DragTile()
					: base (Direction.Right)
				{
				}

				protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
				{
					Rectangle rect = this.ContainerBounds;
					rect.Deflate (0.5);

					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (TileColors.SurfaceSummaryColors.First ());

					graphics.AddRectangle (rect);
					graphics.RenderSolid (TileColors.BorderColors.First ());
				}
			}


			private static readonly double dragBeginMinimalMove = 4;
			private static readonly Size dragTargetMarkerSize = new Size (250, 21);


			private readonly ControllerTile							host;
			private readonly Common.Widgets.Behaviors.DragBehavior	dragBehavior;

			private Point											dragBeginPoint;
			private Size											dragBeginSize;
			private string											dragGroupId;
			private int												dragTargetIndex;

			private DragWindow										dragWindow;
			private Point											dragWindowSourceBeginPosition;
			private Point											dragWindowSourceOffset;
			private Size											dragWindowSize;

			private DragWindow										dragWindowTarget;

			private Tile											dragErsatzTile;
			private int												dragErsatzIndex;
			private DragTargetMarker								dragTargetMarker;
		}

		#endregion

		private DragHelper										dragHelper;
	}
}
