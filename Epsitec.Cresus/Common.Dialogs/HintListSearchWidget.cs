//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Dialogs
{
	public class HintListSearchWidget : FrameBox
	{
		public HintListSearchWidget()
		{
			this.textField = new FlatTextField ()
			{
				Embedder = this,
				Dock = DockStyle.Fill,
				Margins = new Margins (5, 5, 1, 1),
				VerticalAlignment = VerticalAlignment.Center
			};
		}


		/// <summary>
		/// Defines the source widget which has delegated this search widget to
		/// interact with the user instead of itself.
		/// </summary>
		/// <param name="widget">The source widget.</param>
		public void DefineSourceWidget(Widget widget)
		{
			if (this.sourceWidget != widget)
			{
				this.sourceWidget = widget;

				if (this.sourceWidget == null)
				{
					this.InternalState &= ~InternalState.Focusable;
				}
				else
				{
					this.InternalState |= InternalState.Focusable;
				}
			}
		}

		/// <summary>
		/// Sets the focus on the text field used by the search widget.
		/// </summary>
		/// <returns><c>true</c> if the focus was successfully set; otherwise, <c>false</c>.</returns>
		public bool StealFocus()
		{
			return this.textField.Focus ();
		}


		/// <summary>
		/// Processes the exit of the first or last child when navigating with the
		/// TAB key. This class overrides TAB navigation in order to tunnel the
		/// focus back into the source widget TAB-chain.
		/// </summary>
		/// <param name="dir">The tab navigation direction.</param>
		/// <param name="mode">The tab navigation mode.</param>
		/// <param name="focus">The widget with should get the focus.</param>
		/// <returns>
		/// 	<c>true</c> if the method has a focus suggestion; otherwise, <c>false</c>.
		/// </returns>
		protected override bool ProcessTabChildrenExit(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			if (this.sourceWidget == null)
			{
				return base.ProcessTabChildrenExit (dir, mode, out focus);
			}
			else
			{
				focus = this.sourceWidget.FindTabWidget (dir, mode);

				HashSet<Widget> stop = new HashSet<Widget> ();

				while (Widgets.Helpers.VisualTree.IsAncestor (focus, this.sourceWidget))
				{
					stop.Add (focus);
					
					focus = focus.FindTabWidget (dir, mode);

					if ((focus == null) ||
						(stop.Contains (focus)))
					{
						System.Diagnostics.Debug.Fail ("Cannot TAB-navigate to child");
						break;
					}
				}

				return true;
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle frame = this.Client.Bounds;
			
			frame.Deflate (0.5);
			
			using (Path path = new Path ())
			{
				path.AppendRoundedRectangle (frame, 6);
				graphics.Rasterizer.AddSurface (path);
				graphics.RenderSolid (Color.FromBrightness (1));

				graphics.Rasterizer.AddOutline (path, 1, CapStyle.Round, JoinStyle.Round);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
		}

		
		private readonly FlatTextField textField;
		private Widget sourceWidget;
	}
}
