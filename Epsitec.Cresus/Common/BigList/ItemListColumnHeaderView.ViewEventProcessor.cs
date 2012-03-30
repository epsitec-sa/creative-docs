//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.BigList.Processors;
using Epsitec.Common.BigList.Renderers;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

[assembly: DependencyClass (typeof (ItemListColumnHeaderView))]

namespace Epsitec.Common.BigList
{
	public partial class ItemListColumnHeaderView
	{
		private class ViewEventProcessor : IEventProcessorHost, IDetectionProcessor, ISelectionProcessor
		{
			public ViewEventProcessor(ItemListColumnHeaderView view)
			{
				this.view = view;
			}


			public bool ProcessMessage(Message message, Point pos)
			{
				if (this.view.Columns.Any () == false)
				{
					return false;
				}

				if (this.processor != null)
				{
					if (this.processor.ProcessMessage (message, pos))
					{
						return true;
					}
				}

				switch (message.MessageType)
				{
					case MessageType.MouseEnter:
					case MessageType.MouseHover:
					case MessageType.MouseLeave:
						break;

					case MessageType.MouseDown:
						return this.ProcessMouseDown (message, pos);

					case MessageType.MouseUp:
						break;

					case MessageType.MouseMove:
						break;
				}

				return false;
			}

			public void PaintOverlay(Graphics graphics, Rectangle clipRect)
			{
				if (this.processor != null)
				{
					this.processor.PaintOverlay (graphics, clipRect);
				}
			}

			private bool ProcessMouseDown(Message message, Point pos)
			{
				return MouseDownProcessor.Attach (this, this.view.Client.Bounds, message, pos);
			}


			#region IEventProcessorHost Members

			IEnumerable<IEventProcessor> IEventProcessorHost.EventProcessors
			{
				get
				{
					if (this.processor != null)
					{
						yield return this.processor;
					}
				}
			}

			void IEventProcessorHost.Register(IEventProcessor processor)
			{
				this.processor = processor;
			}

			void IEventProcessorHost.Remove(IEventProcessor processor)
			{
				if (this.processor == processor)
				{
					this.processor = null;
				}
			}

			TPolicy IEventProcessorHost.GetPolicy<TPolicy>()
			{
				return this.view.GetPolicy<TPolicy> ();
			}

			#endregion

			#region IDetectionProcessor Members

			public int Detect(Point pos)
			{
				var column = this.view.Columns.DetectColumn (pos);

				if (column == null)
				{
					return -1;
				}
				else
				{
					return column.Index;
				}
			}

			#endregion

			#region ISelectionProcessor Members

			public bool IsSelected(int index)
			{
				return false;
			}

			public void Select(int index, ItemSelection selection)
			{
				var column = this.view.Columns.GetColumn (index);

				if (column == null)
				{
					return;
				}

				switch (selection)
				{
					case ItemSelection.Activate:
						break;

					case ItemSelection.Focus:
						break;

					case ItemSelection.Select:
					case ItemSelection.Deselect:
						break;
					
					case ItemSelection.Toggle:
						this.view.SelectSortColumn (column);
						this.view.Invalidate ();
						break;
				}
			}

			#endregion

			private readonly ItemListColumnHeaderView view;
			private IEventProcessor				processor;
		}
	}
}