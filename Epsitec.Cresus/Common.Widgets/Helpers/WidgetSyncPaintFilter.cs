//	Copyright � 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		public WidgetSyncPaintFilter()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:WidgetSyncPaintFilter"/>
		/// class and adds an initial widget to the list of widgets which will not
		/// be discarded.
		/// </summary>
		/// <param name="widget">The widget to add.</param>
		public WidgetSyncPaintFilter(Widget widget) : this ()
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
				
				return false;
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
				
				return false;
			}
			else
			{
				return true;
			}
		}
		
		void IPaintFilter.NotifyAboutToProcessChildren()
		{
			this.enableAllChildren++;
		}

		void IPaintFilter.NotifyChildrenProcessed()
		{
			this.enableAllChildren--;
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

		private List<Widget> allowed = new List<Widget> ();
		private List<Widget> parents = new List<Widget> ();
		private int enableAllChildren = 0;
	}
}
