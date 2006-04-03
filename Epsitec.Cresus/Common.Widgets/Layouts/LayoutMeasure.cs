//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

		internal void UpdateMin(int passId, double value)
		{
			this.passId = passId;
			this.min = System.Math.Max (this.min, value);
		}
		internal void UpdateMax(int passId, double value)
		{
			this.passId = passId;
			this.max = System.Math.Min (this.max, value);
		}
		
		public static Types.DependencyProperty WidthProperty  = Types.DependencyProperty.RegisterAttached ("Width", typeof (LayoutMeasure), typeof (LayoutMeasure));
		public static Types.DependencyProperty HeightProperty = Types.DependencyProperty.RegisterAttached ("Height", typeof (LayoutMeasure), typeof (LayoutMeasure));
		
		private double min;
		private double max;
		private int passId;
		private double value;
	}
}
