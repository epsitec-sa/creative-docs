//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	public partial class ColorSample
	{
		#region DragInfo Class

		/// <summary>
		/// The <c>DragInfo</c> classe stores information needed only while drag
		/// and drop is in progress.
		/// </summary>
		private class DragInfo : System.IDisposable
		{
			public DragInfo(ColorSample host)
			{
				this.host = host;
			}

			public DragInfo(Point cursor, ColorSample widget)
				: this (widget.DragHost)
			{
				System.Diagnostics.Debug.Assert (widget.DragHost != null);

				this.target = null;
				this.origin = this.host.MapClientToScreen (new Point (-5, -5));

				this.window = new DragWindow ()
				{
					Alpha = 1.0,
					WindowLocation = this.Origin + cursor,
					Owner = this.host.Window,
				};

				this.window.DefineWidget (widget, new Size (11, 11), Margins.Zero);
				this.window.FocusWidget (widget);
				this.window.Show ();
			}

			public DragWindow					Window
			{
				get
				{
					return this.window;
				}
			}

			public Point						Origin
			{
				get
				{
					return this.origin;
				}
			}

			public ColorSample					Target
			{
				get
				{
					return this.target;
				}
			}

			public RichColor					Color
			{
				get
				{
					return this.color;
				}
				set
				{
					if (this.color != value)
					{
						this.color = value;
						this.host.Invalidate ();
					}
				}
			}

			public void DefineTarget(ColorSample target)
			{
				this.target = target;
			}
			

			public void DissolveAndDispose()
			{
				if (this.window != null)
				{
					this.window.DissolveAndDisposeWindow ();
				}

				this.Dispose ();
			}

			#region IDisposable Members

			public void Dispose()
			{
				if ((this.window != null) &&
					(this.window.IsDisposed == false))
				{
					this.window.Hide ();
					this.window.Dispose ();
				}
				
				this.target = null;
			}

			#endregion

			private readonly ColorSample		host;
			private readonly Point				origin;
			private readonly DragWindow			window;
			private ColorSample					target;
			private RichColor					color;
		}

		#endregion
	}
}
