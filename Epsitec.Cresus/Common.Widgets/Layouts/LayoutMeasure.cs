//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	public class LayoutMeasure : Types.DependencyObject
	{
		internal LayoutMeasure(int passId)
		{
			this.min     = 0;
			this.max     = double.PositiveInfinity;
			this.h1      = 0;
			this.h2      = 0;
			this.desired = double.NaN;
			this.passId  = passId;
		}
		
		public double Min
		{
			get
			{
				return this.min;
			}
		}
		public double Max
		{
			get
			{
				return this.max;
			}
		}
		public double Desired
		{
			get
			{
				if (double.IsNaN (this.desired))
				{
					return this.desired;
				}
				else
				{
					double value;
					value = System.Math.Min (this.max, this.desired);
					value = System.Math.Max (this.min, value);
					return value;
				}
			}
		}
		public double H1
		{
			get
			{
				return this.h1;
			}
		}
		public double H2
		{
			get
			{
				return this.h2;
			}
		}
		
		public bool HasChanged
		{
			get
			{
				return this.hasChanged;
			}
		}

		public bool SamePassIdAsLayoutContext(Visual visual)
		{
			LayoutContext context = Helpers.VisualTree.FindLayoutContext (visual);
			
			if (context == null)
			{
				return false;
			}
			
			return context.PassId == this.passId;
		}
		
		internal void ClearHasChanged()
		{
			this.hasChanged = false;
		}

		internal void UpdatePassId(int id)
		{
			this.passId = id;
		}
		
		internal void UpdateMin(int passId, double value)
		{
			double old = this.min;
			
			if (this.passId == passId)
			{
				this.min = System.Math.Max (old, value);
			}
			else
			{
				this.min = value;
			}

			if (this.min != old)
			{
				this.hasChanged = true;
			}
		}
		internal void UpdateMax(int passId, double value)
		{
			double old = this.max;

			if (this.passId == passId)
			{
				this.max = System.Math.Min (old, value);
			}
			else
			{
				this.max = value;
			}

			if (this.max != old)
			{
				this.hasChanged = true;
			}
		}
		internal void UpdateBaseLine(int passId, double h1, double h2)
		{
			double oldH1 = this.h1;
			double oldH2 = this.h2;

			if (this.passId == passId)
			{
				this.h1 = System.Math.Max (oldH1, h1);
				this.h2 = System.Math.Max (oldH2, h2);
			}
			else
			{
				this.h1 = h1;
				this.h2 = h2;
			}

			if ((this.h1 != oldH1) ||
				(this.h2 != oldH2))
			{
				this.hasChanged = true;
			}
		}
		internal void UpdateDesired(int passId, double value)
		{
			if (double.IsNaN (this.desired))
			{
				if (double.IsNaN (value))
				{
					return;
				}

				this.desired = value;
				this.hasChanged = true;
			}
			else if (this.desired != value)
			{
				this.desired = value;
				this.hasChanged	= true;
			}
		}

		public static LayoutMeasure GetWidth(Visual visual)
		{
			return visual.GetValue (LayoutMeasure.WidthProperty) as LayoutMeasure;
		}
		public static LayoutMeasure GetHeight(Visual visual)
		{
			return visual.GetValue (LayoutMeasure.HeightProperty) as LayoutMeasure;
		}
		
		public static Types.DependencyProperty WidthProperty  = Types.DependencyProperty.RegisterAttached ("Width", typeof (LayoutMeasure), typeof (LayoutMeasure));
		public static Types.DependencyProperty HeightProperty = Types.DependencyProperty.RegisterAttached ("Height", typeof (LayoutMeasure), typeof (LayoutMeasure));
		
		private double min;
		private double max;
		private double h1, h2;
		private int passId;
		private double desired;
		private bool hasChanged;
	}
}
