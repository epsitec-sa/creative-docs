//	Copyright © 2006-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.Layouts.ColumnDefinition))]

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

		public ColumnDefinition(double width, GridUnitType gridUnitType = GridUnitType.Absolute)
			: this (new GridLength (width, gridUnitType))
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

		
		public double							MinWidth
		{
			get
			{
				if (this.Visibility)
				{
					return (double) this.GetValue (ColumnDefinition.MinWidthProperty);
				}
				else
				{
					return 0.0;
				}
			}
			set
			{
				this.SetValue (ColumnDefinition.MinWidthProperty, value);
			}
		}

		public double							MaxWidth
		{
			get
			{
				if (this.Visibility)
				{
					return (double) this.GetValue (ColumnDefinition.MaxWidthProperty);
				}
				else
				{
					return 0.0;
				}
			}
			set
			{
				this.SetValue (ColumnDefinition.MaxWidthProperty, value);
			}
		}

		public GridLength						Width
		{
			get
			{
				if (this.Visibility)
				{
					return (GridLength) this.GetValue (ColumnDefinition.WidthProperty);
				}
				else
				{
					return GridLength.Zero;
				}
			}
			set
			{
				this.SetValue (ColumnDefinition.WidthProperty, value);
			}
		}

		public double							LeftBorder
		{
			get
			{
				if (this.Visibility)
				{
					return (double) this.GetValue (ColumnDefinition.LeftBorderProperty);
				}
				else
				{
					return 0.0;
				}
			}
			set
			{
				this.SetValue (ColumnDefinition.LeftBorderProperty, value);
			}
		}

		public double							RightBorder
		{
			get
			{
				if (this.Visibility)
				{
					return (double) this.GetValue (ColumnDefinition.RightBorderProperty);
				}
				else
				{
					return 0.0;
				}
			}
			set
			{
				this.SetValue (ColumnDefinition.RightBorderProperty, value);
			}
		}

		public double							ActualWidth
		{
			get
			{
				return this.actualWidth;
			}
		}

		public double							ActualOffset
		{
			get
			{
				return this.actualOffset;
			}
		}

		public bool								Visibility
		{
			get
			{
				return (bool) this.GetValue (ColumnDefinition.VisibilityProperty);
			}
			set
			{
				this.SetValue (ColumnDefinition.VisibilityProperty, value);
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

		
		private void OnChanged()
		{
			this.Changed.Raise (this);
		}
		
		private static void NotifyPropertyInvalidated(DependencyObject o, object oldValue, object newValue)
		{
			ColumnDefinition def = (ColumnDefinition) o;
			def.OnChanged ();
		}

		public event Support.EventHandler		Changed;
		
		public static readonly DependencyProperty MinWidthProperty		= DependencyProperty.Register ("MinWidth", typeof (double), typeof (ColumnDefinition), new DependencyPropertyMetadata (0.0, ColumnDefinition.NotifyPropertyInvalidated));
		public static readonly DependencyProperty MaxWidthProperty		= DependencyProperty.Register ("MaxWidth", typeof (double), typeof (ColumnDefinition), new DependencyPropertyMetadata (double.PositiveInfinity, ColumnDefinition.NotifyPropertyInvalidated));
		public static readonly DependencyProperty WidthProperty			= DependencyProperty.Register ("Width", typeof (GridLength), typeof (ColumnDefinition), new DependencyPropertyMetadata (GridLength.Auto, ColumnDefinition.NotifyPropertyInvalidated));
		public static readonly DependencyProperty LeftBorderProperty	= DependencyProperty.Register ("LeftBorder", typeof (double), typeof (ColumnDefinition), new DependencyPropertyMetadata (0.0, ColumnDefinition.NotifyPropertyInvalidated));
		public static readonly DependencyProperty RightBorderProperty	= DependencyProperty.Register ("RightBorder", typeof (double), typeof (ColumnDefinition), new DependencyPropertyMetadata (0.0, ColumnDefinition.NotifyPropertyInvalidated));
		public static readonly DependencyProperty VisibilityProperty	= DependencyProperty.Register ("Visibility", typeof (bool), typeof (ColumnDefinition), new DependencyPropertyMetadata (true, ColumnDefinition.NotifyPropertyInvalidated));

		private double							actualOffset;
		private double							actualWidth;
	}
}
