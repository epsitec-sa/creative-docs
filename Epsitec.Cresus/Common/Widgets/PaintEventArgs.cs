//	Copyright © 2003-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>PaintEventArgs</c> class describes the events related to painting,
	/// which provide a graphic context and the current clipping rectangle.
	/// </summary>
	public class PaintEventArgs : Support.CancelEventArgs
	{
		public PaintEventArgs(Graphics graphics, Rectangle clipRect)
		{
			this.graphics = graphics;
			this.clipRect = clipRect;
		}


		public Graphics							Graphics
		{
			get
			{
				return this.graphics;
			}
		}

		public Rectangle						ClipRectangle
		{
			get
			{
				return this.clipRect;
			}
		}


		private readonly Graphics				graphics;
		private readonly Rectangle				clipRect;
	}
}
