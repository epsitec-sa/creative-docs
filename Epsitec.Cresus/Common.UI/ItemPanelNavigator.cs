//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemPanelNavigator</c> class implements the keyboard navigation
	/// through an <see cref="ItemPanel"/>.
	/// </summary>
	public class ItemPanelNavigator
	{
		internal ItemPanelNavigator(ItemPanel rootPanel)
		{
			System.Diagnostics.Debug.Assert (rootPanel.IsRootPanel);

			this.rootPanel = rootPanel;
		}

		/// <summary>
		/// Gets the current item view.
		/// </summary>
		/// <value>The current item view, or <c>null</c>.</value>
		public ItemView Current
		{
			get
			{
				return this.currentView;
			}
		}

		/// <summary>
		/// Navigates to the specified item view. This will clear the cached
		/// position information.
		/// </summary>
		/// <param name="view">The item view.</param>
		public void NavigateTo(ItemView view)
		{
			if (this.currentView != view)
			{
				this.currentView  = view;
				this.currentPanel = view == null ? null : view.Owner;
				
				this.isCurrentXValid = false;
				this.isCurrentYValid = false;
			}
		}

		/// <summary>
		/// Navigates to a new item view in the specified direction.
		/// </summary>
		/// <param name="direction">The direction.</param>
		/// <returns><c>true</c> if the navigation was successful; otherwise,
		/// <c>false</c></returns>
		public bool Navigate(Widgets.Direction direction)
		{
			this.UpdateCurrentPosition ();

			ItemView view = this.FindNeighbour (direction);

			if (view != null)
			{
				this.currentView  = view;
				this.currentPanel = view.Owner;

				switch (direction)
				{
					case Epsitec.Common.Widgets.Direction.Up:
					case Epsitec.Common.Widgets.Direction.Down:
						this.isCurrentYValid = false;
						break;

					case Epsitec.Common.Widgets.Direction.Left:
					case Epsitec.Common.Widgets.Direction.Right:
						this.isCurrentXValid = false;
						break;
				}
				
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Finds the neighbour. This will first look for immediate neighbours
		/// in the same container; if none can be found, a new search will be
		/// done in the parent containers.
		/// </summary>
		/// <param name="direction">The search direction.</param>
		/// <returns>The item view or <c>null</c> if none can be found.</returns>
		private ItemView FindNeighbour(Widgets.Direction direction)
		{
			List<ItemPanel> exclusionList = new List<ItemPanel> ();
			ItemPanel panel = this.currentPanel;

			while (panel != null)
			{
				ItemView view = this.FindNeighbour (direction, panel, exclusionList);

				if (view != null)
				{
					return view;
				}
				
				//	There is no valid neighbour in the specified direction; search
				//	one level higher in the tree.

				exclusionList.Add (panel);

				ItemPanelGroup group = panel.ParentGroup;

				if ((group == null) ||
					(group.ItemView == null))
				{
					break;
				}
				
				panel = group.ItemView.Owner;
			}

			return null;
		}

		private ItemView FindNeighbour(Widgets.Direction direction, ItemPanel panel, List<ItemPanel> exclusionList)
		{
			BestRecord best = new BestRecord ();

			System.Predicate<ItemView> viewPredicate  = this.GetMatchItemViewFocusBoundsPredicateAndRecordBest (direction, best);
			System.Predicate<ItemView> groupPredicate = this.GetMatchItemViewBoundsPredicate (direction, exclusionList);
			
			//	Visit all local views and all children views not in the exclusion
			//	list, recording the distance from the current view. The best match
			//	is recorded by the "best" instance :
			
			IList<ItemView> views = panel.FindItemViews (viewPredicate, groupPredicate);

			return best.View;
		}

		#region BestRecord Class

		/// <summary>
		/// The <c>BestRecord</c> class is used to record the best item view
		/// based on geometric considerations.
		/// </summary>
		private class BestRecord
		{
			public BestRecord()
			{
				this.MinDistance =  1000000000.0;
				this.MaxOverlap  = -1000000000.0;
			}

			/// <summary>
			/// Record the best view based on the minimum distance and, in case of
			/// equality, the view with the highest overlap.
			/// </summary>
			/// <param name="view">The view.</param>
			/// <param name="distance">The distance.</param>
			/// <param name="overlap">The overlap.</param>
			public void Merge(ItemView view, double distance, double overlap)
			{
				if (distance < this.MinDistance)
				{
					this.View        = view;
					this.MinDistance = distance;
					this.MaxOverlap  = overlap;
				}
				else if (distance == this.MinDistance)
				{
					if (this.MaxOverlap < overlap)
					{
						this.View       = view;
						this.MaxOverlap = overlap;
					}
				}
			}

			public ItemView View;
			public double MinDistance;
			public double MaxOverlap;
		}

		#endregion

		private bool FilterGroups(ItemView view)
		{
			if ((view.IsGroup) &&
				(view.IsExpanded))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		private System.Predicate<ItemView> GetMatchItemViewFocusBoundsPredicateAndRecordBest(Widgets.Direction direction, BestRecord best)
		{
			System.Predicate<ItemView> predicate;

			switch (direction)
			{
				case Epsitec.Common.Widgets.Direction.Up:
					predicate = delegate (ItemView view)
					{
						if (this.FilterGroups (view))
						{
							Drawing.Rectangle bounds = this.MapToRoot (view.Owner, view.FocusBounds);

							double distance = bounds.Center.Y - this.currentY;

							if (distance <= 0)
							{
								return false;
							}

							double overlap = ItemPanelNavigator.GetOverlap (bounds.Left, bounds.Right, this.currentX1, this.currentX2);

							best.Merge (view, distance, overlap);

							return true;
						}
						else
						{
							return false;
						}
					};
					break;

				case Epsitec.Common.Widgets.Direction.Down:
					predicate = delegate (ItemView view)
					{
						if (this.FilterGroups (view))
						{
							Drawing.Rectangle bounds = this.MapToRoot (view.Owner, view.FocusBounds);

							double distance = this.currentY - bounds.Center.Y;

							if (distance <= 0)
							{
								return false;
							}

							double overlap = ItemPanelNavigator.GetOverlap (bounds.Left, bounds.Right, this.currentX1, this.currentX2);

							best.Merge (view, distance, overlap);

							return true;
						}
						else
						{
							return false;
						}
					};
					break;

				case Epsitec.Common.Widgets.Direction.Right:
					predicate = delegate (ItemView view)
					{
						if (this.FilterGroups (view))
						{
							Drawing.Rectangle bounds = this.MapToRoot (view.Owner, view.FocusBounds);

							double distance = bounds.Center.X - this.currentX;

							if (distance <= 0)
							{
								return false;
							}

							double overlap = ItemPanelNavigator.GetOverlap (bounds.Bottom, bounds.Top, this.currentY1, this.currentY2);

							best.Merge (view, distance, overlap);

							return true;
						}
						else
						{
							return false;
						}
					};
					break;

				case Epsitec.Common.Widgets.Direction.Left:
					predicate = delegate (ItemView view)
					{
						if (this.FilterGroups (view))
						{
							Drawing.Rectangle bounds = this.MapToRoot (view.Owner, view.FocusBounds);

							double distance = this.currentX - bounds.Center.X;

							if (distance <= 0)
							{
								return false;
							}

							double overlap = ItemPanelNavigator.GetOverlap (bounds.Bottom, bounds.Top, this.currentY1, this.currentY2);

							best.Merge (view, distance, overlap);

							return true;
						}
						else
						{
							return false;
						}
					};
					break;

				default:
					throw new System.ArgumentException ("Invalid direction " + direction.ToString (), "direction");
			}

			return predicate;
		}

		private static double GetOverlap(double a1, double a2, double b1, double b2)
		{
			double c1 = System.Math.Max (a1, b1);
			double c2 = System.Math.Min (a2, b2);

			return c2-c1;
		}

		private System.Predicate<ItemView> GetMatchItemViewBoundsPredicate(Widgets.Direction direction, IList<ItemPanel> exclusionList)
		{
			System.Predicate<ItemView> predicate;

			switch (direction)
			{
				case Epsitec.Common.Widgets.Direction.Up:
					predicate = delegate (ItemView view)
					{
						if ((!view.IsExpanded) ||
							(exclusionList.Contains (view.Owner)))
						{
							return false;
						}

						Drawing.Rectangle bounds = this.MapToRoot (view.Owner, view.Bounds);

						if ((bounds.Right < this.currentX1) ||
							(bounds.Left > this.currentX2) ||
							(bounds.Center.Y <= this.currentY))
						{
							return false;
						}
						else
						{
							return true;
						}
					};
					break;

				case Epsitec.Common.Widgets.Direction.Down:
					predicate = delegate (ItemView view)
					{
						if ((!view.IsExpanded) ||
							(exclusionList.Contains (view.Owner)))
						{
							return false;
						}
						
						Drawing.Rectangle bounds = this.MapToRoot (view.Owner, view.Bounds);

						if ((bounds.Right < this.currentX1) ||
							(bounds.Left > this.currentX2) ||
							(bounds.Center.Y >= this.currentY))
						{
							return false;
						}
						else
						{
							return true;
						}
					};
					break;

				case Epsitec.Common.Widgets.Direction.Right:
					predicate = delegate (ItemView view)
					{
						if ((!view.IsExpanded) ||
							(exclusionList.Contains (view.Owner)))
						{
							return false;
						}

						Drawing.Rectangle bounds = this.MapToRoot (view.Owner, view.Bounds);

						if ((bounds.Top < this.currentY1) ||
							(bounds.Bottom > this.currentY2) ||
							(bounds.Center.X <= this.currentX))
						{
							return false;
						}
						else
						{
							return true;
						}
					};
					break;

				case Epsitec.Common.Widgets.Direction.Left:
					predicate = delegate (ItemView view)
					{
						if ((!view.IsExpanded) ||
							(exclusionList.Contains (view.Owner)))
						{
							return false;
						}

						Drawing.Rectangle bounds = this.MapToRoot (view.Owner, view.Bounds);

						if ((bounds.Top < this.currentY1) ||
							(bounds.Bottom > this.currentY2) ||
							(bounds.Center.X >= this.currentX))
						{
							return false;
						}
						else
						{
							return true;
						}
					};
					break;

				default:
					throw new System.ArgumentException ("Invalid direction " + direction.ToString (), "direction");
			}

			return predicate;
		}

		/// <summary>
		/// Updates the cached X and Y positions based on the current item view
		/// and the validity of the positions.
		/// </summary>
		private void UpdateCurrentPosition()
		{
			if (this.currentView != null)
			{
				Drawing.Rectangle bounds = this.MapToRoot (this.currentPanel, this.currentView.FocusBounds);

				if (!this.isCurrentXValid)
				{
					this.currentX1 = bounds.Left;
					this.currentX2 = bounds.Right;
					
					if (this.currentView.IsGroup)
					{
						this.currentX = this.currentX1;
					}
					else
					{
						this.currentX = (this.currentX1 + this.currentX2) / 2;
					}
					
					this.isCurrentXValid = true;
				}

				if (!this.isCurrentYValid)
				{
					this.currentY1 = bounds.Bottom;
					this.currentY2 = bounds.Top;
					this.currentY  = (this.currentY1 + this.currentY2) / 2;
					
					this.isCurrentYValid = true;
				}
			}
		}

		private Drawing.Point MapFromRoot(ItemPanel container, Drawing.Point position)
		{
			return this.MapCoordinates (container, position, MapMode.FromRoot,
				delegate (Widgets.Widget widget, Drawing.Point pos)
				{
					return widget.MapParentToClient (pos);
				});
		}

		private Drawing.Rectangle MapFromRoot(ItemPanel container, Drawing.Rectangle bounds)
		{
			return this.MapCoordinates (container, bounds, MapMode.FromRoot,
				delegate (Widgets.Widget widget, Drawing.Rectangle rect)
				{
					return widget.MapParentToClient (rect);
				});
		}

		private Drawing.Point MapToRoot(ItemPanel container, Drawing.Point position)
		{
			return this.MapCoordinates (container, position, MapMode.ToRoot,
				delegate (Widgets.Widget widget, Drawing.Point pos)
				{
					return widget.MapClientToParent (pos);
				});
		}

		private Drawing.Rectangle MapToRoot(ItemPanel container, Drawing.Rectangle bounds)
		{
			return this.MapCoordinates (container, bounds, MapMode.ToRoot,
				delegate (Widgets.Widget widget, Drawing.Rectangle rect)
				{
					return widget.MapClientToParent (rect);
				});
		}

		#region Templatized MapCoordinates Method

		delegate T Transform<T>(Widgets.Widget widget, T t);

		private enum MapMode
		{
			ToRoot,
			FromRoot,
		}

		/// <summary>
		/// Maps the coordinates between the root and the specified container.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="coord">The coordinates.</param>
		/// <param name="mode">The mapping mode.</param>
		/// <param name="transform">The transform used to map the coordinates between
		/// a widget and its parent.</param>
		/// <returns>The mapped coordinates.</returns>
		private T MapCoordinates<T>(ItemPanel container, T coord, MapMode mode, Transform<T> transform)
		{
			List<Widgets.Widget> widgets = new List<Widgets.Widget> ();

			while (container != null)
			{
				if (container == this.rootPanel)
				{
					if (mode == MapMode.FromRoot)
					{
						widgets.Reverse ();
					}

					foreach (Widgets.Widget widget in widgets)
					{
						coord = transform (widget, coord);
					}

					return coord;
				}

				ItemPanelGroup group = container.ParentGroup;

				if (group == null)
				{
					break;
				}

				widgets.Add (container);
				widgets.Add (group);

				container = group.ItemView.Owner;
			}

			throw new System.InvalidOperationException ("Panel does not have a common root.");
		}

		#endregion

		private ItemPanel						rootPanel;
		private ItemView						currentView;
		private ItemPanel						currentPanel;

		private double							currentX1, currentX2, currentX;
		private double							currentY1, currentY2, currentY;
		
		private bool							isCurrentXValid;
		private bool							isCurrentYValid;
	}
}
