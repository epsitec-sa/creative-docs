//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	public class WidgetPaintFilter : IPaintFilter
	{
		public WidgetPaintFilter()
		{
		}
		
		public WidgetPaintFilter(Widget widget) : this ()
		{
			this.Add (widget);
		}
		
		
		public void Add(Widget widget)
		{
			if (widget != null)
			{
				if (this.allowed.Contains (widget) == false)
				{
					this.allowed.Add (widget);
					this.AddHierarchy (widget);
				}
			}
		}
		
		
		#region IPaintFilter Members
		public bool IsWidgetFullyDiscarded(Widget widget)
		{
			if (this.enable_children > 0)
			{
				return false;
			}
			return this.parents.Contains (widget) ? false : true;
		}
		
		public bool IsWidgetPaintDiscarded(Widget widget)
		{
			if (this.enable_children > 0)
			{
				return false;
			}
			return this.allowed.Contains (widget) ? false : true;
		}
		public void EnableChildren()
		{
			this.enable_children++;
		}
		
		public void DisableChildren()
		{
			this.enable_children--;
		}
		#endregion
		
		protected void AddHierarchy(Widget widget)
		{
			if (widget != null)
			{
				if (this.parents.Contains (widget) == false)
				{
					this.parents.Add (widget);
					this.AddHierarchy (widget.Parent);
				}
			}
		}
		
		
		private System.Collections.ArrayList	allowed = new System.Collections.ArrayList ();
		private System.Collections.ArrayList	parents = new System.Collections.ArrayList ();
		private int								enable_children = 0;
	}
}
