//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// The <c>SyncPaintFilter</c> class is used by the synchronous painting
	/// algorithm to filter out all widgets which do not belong to the paint
	/// list.
	/// </summary>
	public class WidgetSyncPaintFilter : IPaintFilter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:WidgetSyncPaintFilter"/>
		/// class.
		/// </summary>
		/// <param name="parent">The parent filter.</param>
		public WidgetSyncPaintFilter(IPaintFilter parent)
		{
			this.parent  = parent;
			this.allowed = new List<Widget> ();
			this.parents = new List<Widget> ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:WidgetSyncPaintFilter"/>
		/// class and adds an initial widget to the list of widgets which will not
		/// be discarded.
		/// </summary>
		/// <param name="widget">The widget to add.</param>
		/// <param name="parent">The parent filter.</param>
		public WidgetSyncPaintFilter(Widget widget, IPaintFilter parent)
			: this (parent)
		{
			this.Add (widget);
		}

		/// <summary>
		/// Adds the specified widget the the list of widgets which will be
		/// painted (they won't be discarded).
		/// </summary>
		/// <param name="widget">The widget to add.</param>
		public void Add(Widget widget)
		{
			if (widget != null)
			{
				if (this.allowed.Contains (widget) == false)
				{
					this.allowed.Add (widget);
					this.AddHierarchy (widget.Parent);
				}
			}
		}
		
		
		#region IPaintFilter Members
		
		bool IPaintFilter.IsWidgetFullyDiscarded(Widget widget)
		{
			if ((this.enableAllChildren > 0) ||
				(this.allowed.Contains (widget)) ||
				(this.parents.Contains (widget)))
			{
				//	Process all children, allowed widgets and parents. This will
				//	not necessarily paint them, but their children PaintHandler
				//	will be called.

				if (this.parent == null)
				{
					return false;
				}
				else
				{
					return this.parent.IsWidgetFullyDiscarded (widget);
				}
			}
			else
			{
				return true;
			}
		}
		
		bool IPaintFilter.IsWidgetPaintDiscarded(Widget widget)
		{
			if ((this.enableAllChildren > 0) ||
				(this.allowed.Contains (widget)))
			{
				//	Paint all children and all allowed widgets.

				if (this.parent == null)
				{
					return false;
				}
				else
				{
					return this.parent.IsWidgetPaintDiscarded (widget);
				}
			}
			else
			{
				return true;
			}
		}
		
		void IPaintFilter.NotifyAboutToProcessChildren(Widget sender, PaintEventArgs e)
		{
			this.enableAllChildren++;

			if (this.parent != null)
			{
				this.parent.NotifyAboutToProcessChildren (sender, e);
			}
		}

		void IPaintFilter.NotifyChildrenProcessed(Widget sender, PaintEventArgs e)
		{
			this.enableAllChildren--;

			if (this.parent != null)
			{
				this.parent.NotifyChildrenProcessed (sender, e);
			}
		}
		
		#endregion
		
		private void AddHierarchy(Widget widget)
		{
			while (widget != null)
			{
				if (this.parents.Contains (widget))
				{
					break;
				}
				
				this.parents.Add (widget);
				widget = widget.Parent;
			}
		}

		private readonly IPaintFilter			parent;
		private readonly List<Widget>			allowed;
		private readonly List<Widget>			parents;
		private int								enableAllChildren;
	}
}
