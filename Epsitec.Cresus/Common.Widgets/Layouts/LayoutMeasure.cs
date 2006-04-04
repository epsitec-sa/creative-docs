//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	public class LayoutMeasure : Types.DependencyObject
	{
		internal LayoutMeasure(int passId)
		{
			this.min    = 0;
			this.max    = double.PositiveInfinity;
			this.value  = double.NaN;
			this.passId = passId;
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
		public bool HasChanged
		{
			get
			{
				return this.hasChanged;
			}
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
				this.min = System.Math.Max (this.min, value);
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
				this.max = System.Math.Min (this.max, value);
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
		
		public static Types.DependencyProperty WidthProperty  = Types.DependencyProperty.RegisterAttached ("Width", typeof (LayoutMeasure), typeof (LayoutMeasure));
		public static Types.DependencyProperty HeightProperty = Types.DependencyProperty.RegisterAttached ("Height", typeof (LayoutMeasure), typeof (LayoutMeasure));
		
		private double min;
		private double max;
		private int passId;
		private double value;
		private bool hasChanged;
	}
}
