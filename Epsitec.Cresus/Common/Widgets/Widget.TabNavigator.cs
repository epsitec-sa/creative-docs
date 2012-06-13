//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
	public partial class Widget
	{
		class TabNavigator
		{
			public TabNavigator(TabNavigationDir dir, TabNavigationMode mode)
			{
				this.dir   = dir;
				this.mode  = mode;
				this.cache = new HashSet<Widget> ();
			}

			
			public Widget FindTabWidget(Widget that, bool disableFirstEnter = false, bool acceptFocus = true)
			{
				if (this.cache.Add (that) == false)
				{
					return null;
				}

				if (that.ProcessTab (this.dir, this.mode))
				{
					return that;
				}

				var find = this.FindTabWidgetAutoRadio (that)
					    ?? this.FindTabWidgetEnterChildren (that, disableFirstEnter, acceptFocus);

				if (find != null)
				{
					return find;
				}

				if (acceptFocus)
				{
					if ((that.TabNavigationMode & this.mode) != 0)
					{
						if (that.TabNavigationMode.HasFlag (TabNavigationMode.ForwardOnly))
						{
							if (this.dir != TabNavigationDir.Backwards)
							{
								return null;
							}
						}
						else
						{
							return that;
						}
					}
				}

				//	Look for a sibling. If no immediate sibling can be found, look for the next
				//	sibling in the parent, or else cycle in the current siblings.

				System.Diagnostics.Debug.Assert (find == null);

				find = this.FindTabWidgetOverride (that);

				if (find != null)
				{
					return find;
				}

				var tabSiblings = this.GetTabNavigationSiblings (that);
				int index = this.FindIndex (that, tabSiblings);

				find = ((index == -1) ? this.FindTabWidgetInChildren (that)
					/* */             : this.FindTabWidgetInSiblings (that, tabSiblings, index))
					?? this.FindTabWidgetInParent (that)
					?? this.FindTabWidgetWarp (tabSiblings);

				if (find != null)
				{
					System.Diagnostics.Debug.Assert (find.TabNavigationMode.HasFlag (TabNavigationMode.ForwardOnly) == false);
				}

				return find;
			}

			/// <summary>
			/// Finds the tab widget based on one of the overrides (<see cref="ForwardTabOverride"/>
			/// and <see cref="BackwardTabOverride"/>), if there are any.
			/// </summary>
			/// <param name="that">The widget.</param>
			/// <returns>The sibling to navigate to.</returns>
			private Widget FindTabWidgetOverride(Widget that)
			{
				Widget find = null;

				switch (this.dir)
				{
					case TabNavigationDir.Forwards:
						find = that.ForwardTabOverride;
						break;

					case TabNavigationDir.Backwards:
						find = that.BackwardTabOverride;
						break;
				}

				if (find != null)
				{
					find = this.TabNavigateSibling (find);
				}

				return find;
			}
			
			
			private Widget[] GetTabNavigationSiblings(Widget that)
			{
				var list = new List<Widget> ();

				Widget parent = that.Parent;

				if (parent != null)
				{
					Widget[] siblings = parent.Children.Widgets;

					for (int i = 0; i < siblings.Length; i++)
					{
						Widget sibling = siblings[i];

						if (((sibling.TabNavigationMode & this.mode) != 0) &&
							(sibling.IsEnabled) &&
							(sibling.Visibility) &&
							(sibling.Client.Bounds.IsSurfaceZero == false) &&
							(sibling.AcceptsFocus))
						{
							Types.IReadOnly readOnly = sibling as Types.IReadOnly;

							if ((readOnly != null) &&
								(sibling.TabNavigationMode.HasFlag (TabNavigationMode.SkipIfReadOnly)) &&
								(readOnly.IsReadOnly))
							{
								//	Saute aussi les widgets qui déclarent être en lecture seule. Ils ne
								//	sont pas intéressants pour une navigation clavier :
								
								continue;
							}

							if (sibling != that)
							{
								//	Skip widgets which have an overridden tab order, since they would
								//	interfere with the logical flow :

								if (this.dir == TabNavigationDir.Forwards)
								{
									if (sibling.BackwardTabOverride != null)
									{
										continue;
									}
								}
								if (this.dir == TabNavigationDir.Backwards)
								{
									if (sibling.ForwardTabOverride != null)
									{
										continue;
									}
								}
							}

							if (that.FilterTabNavigationSibling (sibling, this.dir, this.mode))
							{
								list.Add (sibling);
							}
						}
					}
				}

				list.Sort (Widget.TabIndexComparer);

				if ((this.mode == TabNavigationMode.ActivateOnTab) &&
					(that.AutoRadio))
				{
					//	On recherche les frères de ce widget, pour déterminer lequel devra être activé par la
					//	pression de la touche TAB. Pour bien faire, il faut supprimer les autres boutons radio
					//	qui appartiennent à notre groupe :

					string group = that.Group;

					return list.Where (x => (x == that || x.Group != group)).ToArray ();
				}
				else
				{
					return list.ToArray ();
				}
			}

			/// <summary>
			/// Finds the index of the widget in the collection.
			/// </summary>
			/// <param name="that">The widget.</param>
			/// <param name="collection">The collection.</param>
			/// <returns>The index of the widget or <c>-1</c> if it does not belong to the collection.</returns>
			private int FindIndex(Widget that, Widget[] collection)
			{
				for (int i = 0; i < collection.Length; i++)
				{
					if (collection[i] == that)
					{
						return i;
					}
				}

				return -1;
			}

			private Widget FindTabWidgetEnterChildren(Widget that, bool disableFirstEnter, bool acceptFocus)
			{
				if ((!disableFirstEnter) &&
					((that.TabNavigationMode & TabNavigationMode.ForwardToChildren) != 0) &&
					(that.HasChildren))
				{
					//	Ce widget permet aux enfants d'entrer dans la liste accessible par la
					//	touche TAB.

					var find = this.TabNavigateEnterOverride (that);

					if (find != null)
					{
						return find;
					}

					Widget[] candidates = this.GetTabNavigationSiblings (that.Children.Widgets[0]);
					int count = candidates.Length;

					for (int i = 0; i < count; i++)
					{
						if (this.dir == TabNavigationDir.Forwards)
						{
							find = this.FindTabWidget (candidates[i]);
						}
						else if (acceptFocus)
						{
							find = this.FindTabWidget (candidates[count-i-1]);
						}

						if (find != null)
						{
							return find;
						}
					}
				}

				return null;
			}
			
			private Widget FindTabWidgetAutoRadio(Widget that)
			{
				if (that.AutoRadio == false)
				{
					return null;
				}

				Widget find = null;

				if (this.mode == TabNavigationMode.ActivateOnCursorX)
				{
					GroupController controller = GroupController.GetGroupController (that);

					find = controller.FindXWidget (that, this.dir == TabNavigationDir.Backwards ? -1 : 1);

					if ((find == null) &&
								(controller.FindXWidget (that, this.dir == TabNavigationDir.Backwards ? 1 : -1) == null))
					{
						//	L'utilisateur demande un déplacement horizontal bien que la disposition
						//	soit purement verticale. On corrige pour lui :

						find = controller.FindYWidget (that, this.dir == TabNavigationDir.Backwards ? -1 : 1);
					}

					if (find != null)
					{
						find.ActiveState = ActiveState.Yes;
						return find;
					}
				}

				if (this.mode == TabNavigationMode.ActivateOnCursorY)
				{
					GroupController controller = GroupController.GetGroupController (that);

					find = controller.FindYWidget (that, this.dir == TabNavigationDir.Backwards ? -1 : 1);

					if ((find == null) &&
						(controller.FindYWidget (that, this.dir == TabNavigationDir.Backwards ? 1 : -1) == null))
					{
						//	L'utilisateur demande un déplacement vertical bien que la disposition
						//	soit purement horizontale. On corrige pour lui :

						find = controller.FindXWidget (that, this.dir == TabNavigationDir.Backwards ? -1 : 1);
					}

					if (find != null)
					{
						find.ActiveState = ActiveState.Yes;
						return find;
					}
				}
				return null;
			}

			/// <summary>
			/// Finds the first or last tab widget, based on the direction. This is used to skip
			/// from the last sibling to the first (forward direction) and from the first to the
			/// last (backward direction).
			/// </summary>
			/// <param name="tabSiblings">The tab siblings.</param>
			/// <returns>The first or last widget, if there is more than one sibling.</returns>
			private Widget FindTabWidgetWarp(Widget[] tabSiblings)
			{
				if (tabSiblings.Length > 1)
				{
					switch (this.dir)
					{
						case TabNavigationDir.Forwards:
							return this.FindTabWidget (tabSiblings[0]);

						case TabNavigationDir.Backwards:
							return this.FindTabWidget (tabSiblings[tabSiblings.Length-1]);
					}
				}

				return null;
			}

			private Widget FindTabWidgetInParent(Widget that)
			{
				Widget find   = null;
				Widget parent = that.Parent;

				if (parent != null)
				{
					if (parent.ProcessTabChildrenExit (this.dir, this.mode, out find))
					{
						return find;
					}

					if ((parent.TabNavigationMode & TabNavigationMode.ForwardToChildren) != 0)
					{
						bool accept;

						switch (this.dir)
						{
							case TabNavigationDir.Backwards:
								accept = (parent.TabNavigationMode & TabNavigationMode.ForwardOnly) == 0;
								return this.FindTabWidget (parent, disableFirstEnter: true, acceptFocus: accept);

							case TabNavigationDir.Forwards:
								accept = false;
								return this.FindTabWidget (parent, disableFirstEnter: true, acceptFocus: accept);
						}
					}
					else
					{
						return this.FindTabWidget (parent, disableFirstEnter: true, acceptFocus: false);
					}
				}
				else if (that.HasChildren)
				{
					//	Il n'y a plus de parents au-dessus. C'est donc vraisemblablement WindowRoot et
					//	dans ce cas, il ne sert à rien de boucler. On va simplement tenter d'activer le
					//	premier descendant trouvé :

					Widget[] candidates = this.GetTabNavigationSiblings (that.Children.Widgets[0]);
					int count = candidates.Length;

					for (int i = 0; i < count; i++)
					{
						if (this.dir == TabNavigationDir.Forwards)
						{
							find = this.FindTabWidget (candidates[i]);
						}
						else
						{
							find = this.FindTabWidget (candidates[count-1-i]);
						}

						if (find != null)
						{
							return find;
						}
					}
				}

				return null;
			}

			private Widget FindTabWidgetInChildren(Widget that)
			{
				Widget find = that;

				while (true)
				{
					if (this.dir == TabNavigationDir.Forwards)
					{
						find = that.Children.FindNext (find) as Widget;
					}
					else if (this.dir == TabNavigationDir.Backwards)
					{
						find = that.Children.FindPrevious (find) as Widget;
					}

					if (find == null)
					{
						return null;
					}

					if ((find.TabNavigationMode & this.mode) != 0)
					{
						return find;
					}
				}
			}

			private Widget FindTabWidgetInSiblings(Widget that, Widget[] tabSiblings, int i)
			{
				Widget find = null;
				var visited = new HashSet<Widget> ();

				while (true)
				{
					switch (this.dir)
					{
						case TabNavigationDir.Backwards:
							find = that.TabNavigate (i--, this.dir, tabSiblings);
							break;

						case TabNavigationDir.Forwards:
							find = that.TabNavigate (i++, this.dir, tabSiblings);
							break;
					}

					if (find == null)
					{
						return find;
					}

					if (visited.Add (find) == false)
					{
						//	Do not visit a widget more than once, or else we might very well
						//	get stuck into an infinite loop here:

						return null;
					}

					find = this.TabNavigateSibling (find);

					if (find != null)
					{
						return find;
					}
				}
			}

			/// <summary>
			/// Finds the definitive widget based on an initial result obtained by
			/// navigating to a sibling. This method will enter groups, if needed.
			/// </summary>
			/// <param name="sibling">The sibling.</param>
			/// <returns>
			/// The definitive widget.
			/// </returns>
			private Widget TabNavigateSibling(Widget sibling)
			{
				if (sibling != null)
				{
					if (this.dir == TabNavigationDir.Backwards)
					{
						if ((sibling.TabNavigationMode & TabNavigationMode.ForwardToChildren) != 0)
						{
							//	Entre en marche arrière dans le widget...

							if (sibling.HasChildren)
							{
								Widget[] candidates = this.GetTabNavigationSiblings (sibling.Children.Widgets[0]);
								int      count      = candidates.Length;
								Widget   widget     = this.TabNavigateEnterOverride (sibling);

								if (widget != null)
								{
									return widget;
								}
								
								if (count > 0)
								{
									sibling = this.FindTabWidget (candidates[count-1]);
								}
								else if (sibling.TabNavigationMode.HasFlag (TabNavigationMode.ForwardOnly))
								{
									sibling = null;
								}
							}
							else if (sibling.TabNavigationMode.HasFlag (TabNavigationMode.ForwardOnly))
							{
								sibling = null;
							}
						}
					}
					else if (this.dir == TabNavigationDir.Forwards)
					{
						if (((sibling.TabNavigationMode & TabNavigationMode.ForwardToChildren) != 0) &&
							((sibling.TabNavigationMode & TabNavigationMode.ForwardOnly) != 0))
						{
							if (sibling.HasChildren)
							{
								//	Entre en marche avant dans le widget...

								Widget[] candidates = this.GetTabNavigationSiblings (sibling.Children.Widgets[0]);
								Widget   widget     = this.TabNavigateEnterOverride (sibling);

								if (widget != null)
								{
									return widget;
								}

								sibling = null;

								foreach (Widget candidate in candidates)
								{
									sibling = this.FindTabWidget (candidate);

									if (sibling != null)
									{
										break;
									}
								}
							}
							else
							{
								sibling = null;
							}
						}
					}
				}

				return sibling;
			}

			/// <summary>
			/// Navigates to the child specified by the enter override property.
			/// </summary>
			/// <param name="widget">The widget.</param>
			/// <returns>
			/// The override, or <c>null</c> if the default navigation should be used.
			/// </returns>
			private Widget TabNavigateEnterOverride(Widget widget)
			{
				System.Diagnostics.Debug.Assert (widget != null);

				Widget find = null;

				switch (this.dir)
				{
					case TabNavigationDir.Forwards:
						find = widget.ForwardEnterTabOverride;
						break;

					case TabNavigationDir.Backwards:
						find = widget.BackwardEnterTabOverride;
						break;
				}

				if (find != null)
				{
					find = this.FindTabWidget (find);
				}

				return find;
			}
			
			
			private readonly TabNavigationDir	dir;
			private readonly TabNavigationMode	mode;
			private readonly HashSet<Widget>	cache;
		}
	}
}
