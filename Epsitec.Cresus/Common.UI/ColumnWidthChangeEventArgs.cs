//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// 
	/// </summary>
	public class ColumnWidthChangeEventArgs : DependencyPropertyChangedEventArgs
	{
		public ColumnWidthChangeEventArgs(int column, double oldWidth, double newWidth)
			: base ("ColumnWidth", oldWidth, newWidth)
		{
			this.column = column;
			this.width  = newWidth;
		}

		public int Column
		{
			get
			{
				return this.column;
			}
		}

		public double OldWidth
		{
			get
			{
				return (double) this.OldValue;
			}
		}

		public double NewWidth
		{
			get
			{
				return (double) this.NewValue;
			}
		}

		public double Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		private int column;
		private double width;
	}
}
