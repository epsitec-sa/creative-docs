//	Copyright © 2003-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>DragEventArgs</c> class describes a drag and drop event produced
	/// by the <c>Widgets</c> framework (contrast with <see cref="WindowDragEventArgs"/>
	/// which is produced by the <c>WinForms</c> framework).
	/// </summary>
	public class DragEventArgs : Support.EventArgs
	{
		public DragEventArgs(Point p1, Point p2)
		{
			this.p1 = p1;
			this.p2 = p2;
		}


		public Point							FromPoint
		{
			get
			{
				return this.p1;
			}
		}

		public Point							ToPoint
		{
			get
			{
				return this.p2;
			}
		}

		public Point							Offset
		{
			get
			{
				return this.p2 - this.p1;
			}
		}


		private readonly Point					p1;
		private readonly Point					p2;
	}
}
