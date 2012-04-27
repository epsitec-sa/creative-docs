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

				if (this.Offset < 20)
				{
					this.Offset = 0;
					this.view.dragButton.Visibility = true;
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
