//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

[assembly: DependencyClass (typeof (VSplitView))]

namespace Epsitec.Common.Widgets
{
	public partial class VSplitView
	{
		class DragProcessor
		{
			public DragProcessor(VSplitView view, Widget source, MessageEventArgs e)
			{
				this.view = view;
				this.source = source;

				this.originalY = this.source.MapClientToRoot (e.Point).Y;
				this.originalH = this.Offset;
				
				this.source.PreProcessing += this.HandleSourcePreProcessing;
				this.view.separator.Visibility = true;
				
				e.Message.Captured = true;
			}


			private double Offset
			{
				get
				{
					return this.view.frame1Container.PreferredHeight;
				}
				set
				{
					this.view.Ratio = value / this.view.Client.Height;
				}
			}

			private void HandleSourcePreProcessing(object sender, MessageEventArgs e)
			{
				if (e.Message.IsMouseType)
				{
					switch (e.Message.MessageType)
					{
						case MessageType.MouseUp:
							this.DragEnd ();
							break;

						case MessageType.MouseMove:
							this.Drag (e.Point);
							break;
					}
				}
				
				e.Cancel = true;
			}

			private void Drag(Point point)
			{
				var currentY = this.source.MapClientToRoot (point).Y;

				this.Offset = System.Math.Max (0, this.originalH - currentY + this.originalY);
			}

			private void DragEnd()
			{
				this.source.PreProcessing -= this.HandleSourcePreProcessing;

				double hMin = this.view.collapseHeight;
				double hMax = this.view.Client.Height - this.view.collapseHeight;

				if (this.Offset < hMin)
				{
					this.Offset = 0;
					this.view.dragButton.Visibility = true;
					this.view.dragButton.Parent = this.view.scroller2.Parent;
					this.view.dragButton.Dock = DockStyle.Top;
					this.view.separator.Visibility = false;
				}
				else if (this.Offset > hMax)
				{
					this.Offset = this.view.Client.Height;
					this.view.dragButton.Visibility = true;
					this.view.dragButton.Parent = this.view.scroller1.Parent;
					this.view.dragButton.Dock = DockStyle.Bottom;
					this.view.separator.Visibility = false;
				}
				else
				{
					this.view.dragButton.Visibility = false;
				}
			}


			private readonly VSplitView			view;
			private readonly Widget				source;
			private readonly double				originalY;
			private readonly double				originalH;
		}
	}
}
