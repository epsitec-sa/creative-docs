//	Copyright © 2006-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	public class LayoutMeasure : DependencyObject
	{
		internal LayoutMeasure(int passId)
		{
			this.Reset (passId);
		}
		
		
		public double							Min
		{
			get
			{
				return this.min;
			}
		}
		
		public double							Max
		{
			get
			{
				return this.max;
			}
		}
		
		public double							Desired
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

		public bool								HasChanged
		{
			get
			{
				return this.hasChanged;
			}
		}

		public int								PassId
		{
			get
			{
				return this.passId;
			}
		}
		
		
		public override string ToString()
		{
			return string.Format ("{0} in [{1}:{2}], pass={3}, desired={4}", this.Desired, this.Min, this.Max, this.passId, this.desired);
		}


		internal void Reset(int passId)
		{
			this.min     = 0;
			this.max     = double.PositiveInfinity;
			this.desired = double.NaN;
			this.passId  = passId;
		}
		
		internal void ClearHasChanged()
		{
			this.hasChanged = false;
		}

		internal void UpdatePassId(int id)
		{
			this.passId = id;
		}
		
		internal void UpdateMin(int passId, double value, bool forceChange = false)
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

			if ((this.min != old) ||
				(forceChange))
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
		
		internal void UpdateDesired(double value)
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
		
		public static DependencyProperty WidthProperty  = DependencyProperty<LayoutMeasure>.RegisterAttached ("Width", typeof (LayoutMeasure), new DependencyPropertyMetadata ().MakeNotSerializable ());
		public static DependencyProperty HeightProperty = DependencyProperty<LayoutMeasure>.RegisterAttached ("Height", typeof (LayoutMeasure), new DependencyPropertyMetadata ().MakeNotSerializable ());
		
		private double							min;
		private double							max;
		private int								passId;
		private double							desired;
		private bool							hasChanged;
	}
}