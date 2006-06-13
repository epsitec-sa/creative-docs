//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// The <c>RowDefinition</c> class defines row-specific properties that apply
	/// to a <see cref="T:GridLayoutEngine"/> object.
	/// </summary>
	public sealed class RowDefinition : DependencyObject
	{
		public RowDefinition()
		{
		}

		public RowDefinition(GridLength height)
		{
			if (this.Height != height)
			{
				this.Height = height;
			}
		}

		public RowDefinition(double minHeight, double maxHeight)
		{
			if (this.MinHeight != minHeight)
			{
				this.MinHeight = minHeight;
			}
			if (this.MaxHeight != maxHeight)
			{
				this.MaxHeight = maxHeight;
			}
		}

		public RowDefinition(GridLength height, double minHeight, double maxHeight)
		{
			if (this.Height != height)
			{
				this.Height = height;
			}
			if (this.MinHeight != minHeight)
			{
				this.MinHeight = minHeight;
			}
			if (this.MaxHeight != maxHeight)
			{
				this.MaxHeight = maxHeight;
			}
		}

		public double MinHeight
		{
			get
			{
				return (double) this.GetValue (RowDefinition.MinHeightProperty);
			}
			set
			{
				this.SetValue (RowDefinition.MinHeightProperty, value);
			}
		}

		public double MaxHeight
		{
			get
			{
				return (double) this.GetValue (RowDefinition.MaxHeightProperty);
			}
			set
			{
				this.SetValue (RowDefinition.MaxHeightProperty, value);
			}
		}

		public GridLength Height
		{
			get
			{
				return (GridLength) this.GetValue (RowDefinition.HeightProperty);
			}
			set
			{
				this.SetValue (RowDefinition.HeightProperty, value);
			}
		}

		public double TopBorder
		{
			get
			{
				return (double) this.GetValue (RowDefinition.TopBorderProperty);
			}
			set
			{
				this.SetValue (RowDefinition.TopBorderProperty, value);
			}
		}

		public double BottomBorder
		{
			get
			{
				return (double) this.GetValue (RowDefinition.BottomBorderProperty);
			}
			set
			{
				this.SetValue (RowDefinition.BottomBorderProperty, value);
			}
		}

		public double ActualHeight
		{
			get
			{
				return this.actualHeight;
			}
		}

		public double ActualOffset
		{
			get
			{
				return this.actualOffset;
			}
		}

		internal void DefineActualHeight(double value)
		{
			this.actualHeight = value;
		}
		
		internal void DefineActualOffset(double value)
		{
			this.actualOffset = value;
		}

		public static readonly DependencyProperty MinHeightProperty		= DependencyProperty.Register ("MinHeight", typeof (double), typeof (RowDefinition), new DependencyPropertyMetadata (0.0));
		public static readonly DependencyProperty MaxHeightProperty		= DependencyProperty.Register ("MaxHeight", typeof (double), typeof (RowDefinition), new DependencyPropertyMetadata (double.PositiveInfinity));
		public static readonly DependencyProperty HeightProperty		= DependencyProperty.Register ("Height", typeof (GridLength), typeof (RowDefinition), new DependencyPropertyMetadata (GridLength.Auto));
		public static readonly DependencyProperty TopBorderProperty		= DependencyProperty.Register ("TopBorder", typeof (double), typeof (RowDefinition), new DependencyPropertyMetadata (0.0));
		public static readonly DependencyProperty BottomBorderProperty	= DependencyProperty.Register ("BottomBorder", typeof (double), typeof (RowDefinition), new DependencyPropertyMetadata (0.0));

		private double actualOffset;
		private double actualHeight;
	}
}
