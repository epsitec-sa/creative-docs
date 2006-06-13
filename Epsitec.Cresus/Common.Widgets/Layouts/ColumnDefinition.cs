//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// The <c>ColumnDefinition</c> class defines column-specific properties that
	/// apply to a <see cref="T:GridLayoutEngine"/> object.
	/// </summary>
	public sealed class ColumnDefinition : DependencyObject
	{
		public ColumnDefinition()
		{
		}

		public ColumnDefinition(GridLength width)
		{
			if (this.Width != width)
			{
				this.Width = width;
			}
		}

		public ColumnDefinition(double minWidth, double maxWidth)
		{
			if (this.MinWidth != minWidth)
			{
				this.MinWidth = minWidth;
			}
			if (this.MaxWidth != maxWidth)
			{
				this.MaxWidth = maxWidth;
			}
		}

		public ColumnDefinition(GridLength width, double minWidth, double maxWidth)
		{
			if (this.Width != width)
			{
				this.Width = width;
			}
			if (this.MinWidth != minWidth)
			{
				this.MinWidth = minWidth;
			}
			if (this.MaxWidth != maxWidth)
			{
				this.MaxWidth = maxWidth;
			}
		}

		public double MinWidth
		{
			get
			{
				return (double) this.GetValue (ColumnDefinition.MinWidthProperty);
			}
			set
			{
				this.SetValue (ColumnDefinition.MinWidthProperty, value);
			}
		}

		public double MaxWidth
		{
			get
			{
				return (double) this.GetValue (ColumnDefinition.MaxWidthProperty);
			}
			set
			{
				this.SetValue (ColumnDefinition.MaxWidthProperty, value);
			}
		}

		public GridLength Width
		{
			get
			{
				return (GridLength) this.GetValue (ColumnDefinition.WidthProperty);
			}
			set
			{
				this.SetValue (ColumnDefinition.WidthProperty, value);
			}
		}

		public double LeftBorder
		{
			get
			{
				return (double) this.GetValue (ColumnDefinition.LeftBorderProperty);
			}
			set
			{
				this.SetValue (ColumnDefinition.LeftBorderProperty, value);
			}
		}

		public double RightBorder
		{
			get
			{
				return (double) this.GetValue (ColumnDefinition.RightBorderProperty);
			}
			set
			{
				this.SetValue (ColumnDefinition.RightBorderProperty, value);
			}
		}

		public double ActualWidth
		{
			get
			{
				return this.actualWidth;
			}
		}

		public double ActualOffset
		{
			get
			{
				return this.actualOffset;
			}
		}

		internal void DefineActualWidth(double value)
		{
			this.actualWidth = value;
		}
		
		internal void DefineActualOffset(double value)
		{
			this.actualOffset = value;
		}
		
		public static readonly DependencyProperty MinWidthProperty		= DependencyProperty.Register ("MinWidth", typeof (double), typeof (ColumnDefinition), new DependencyPropertyMetadata (0.0));
		public static readonly DependencyProperty MaxWidthProperty		= DependencyProperty.Register ("MaxWidth", typeof (double), typeof (ColumnDefinition), new DependencyPropertyMetadata (double.PositiveInfinity));
		public static readonly DependencyProperty WidthProperty			= DependencyProperty.Register ("Width", typeof (GridLength), typeof (ColumnDefinition), new DependencyPropertyMetadata (GridLength.Auto));
		public static readonly DependencyProperty LeftBorderProperty	= DependencyProperty.Register ("LeftBorder", typeof (double), typeof (ColumnDefinition), new DependencyPropertyMetadata (0.0));
		public static readonly DependencyProperty RightBorderProperty	= DependencyProperty.Register ("RightBorder", typeof (double), typeof (ColumnDefinition), new DependencyPropertyMetadata (0.0));

		private double actualOffset;
		private double actualWidth;
	}
}
