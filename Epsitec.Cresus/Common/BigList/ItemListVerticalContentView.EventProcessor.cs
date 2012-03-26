//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList.Processors;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public partial class ItemListVerticalContentView
	{
		private class EventProcessor : IEventProcessorHost, IDetectionProcessor, ISelectionProcessor, IScrollingProcessor
		{
			public EventProcessor(ItemListVerticalContentView view)
			{
				this.view = view;
			}


			public bool ProcessMessage(Message message, Point pos)
			{
				if (this.view.ItemList == null)
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

					case MessageType.MouseWheel:
						return this.ProcessMouseWheel (message, pos);

					case MessageType.KeyDown:
						return this.ProcessKeyDown (message, pos);

				}

				return false;
			}

			private bool ProcessMouseDown(Message message, Point pos)
			{
				return MouseDownProcessor.Attach (this, this.view.Client.Bounds, message, pos);
			}

			private bool ProcessMouseWheel(Message message, Point pos)
			{
				double amplitude = message.WheelAmplitude;
				this.view.Scroll (amplitude, ScrollUnit.Line, ScrollMode.MoveVisible);
				return true;
			}

			private bool ProcessKeyDown(Message message, Point pos)
			{
				return KeyDownProcessor.Attach (this, message, pos);
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
				return this.view.GetPolicy<TPolicy> () ?? new TPolicy ();
			}

			#endregion

			#region IDetectionProcessor Members

			public int Detect(Point pos)
			{
				var row = this.view.ItemList.VisibleRows.FirstOrDefault (x => this.view.GetRowBounds (x).Contains (pos));

				return row == null ? -1 : row.Index;
			}

			#endregion
		
			#region ISelectionProcessor Members

			public bool IsSelected(int index)
			{
				return this.view.ItemList.IsSelected (index);
			}

			public void Select(int index, ItemSelection selection)
			{
				if (index < 0)
				{
					return;
				}

				switch (selection)
				{
					case ItemSelection.Activate:
						this.view.ActivateRow (index);
						break;

					case ItemSelection.Focus:
						this.view.FocusRow (index);
						break;

					case ItemSelection.Select:
					case ItemSelection.Deselect:
					case ItemSelection.Toggle:
						this.view.SelectRow (index, selection);
						break;
				}
			}

			#endregion

			#region IScrollingProcessor Members

			void IScrollingProcessor.Scroll(Point amplitude, ScrollUnit scrollUnit, ScrollMode scrollMode)
			{
				this.view.Scroll (amplitude.Y, scrollUnit, scrollMode);
			}

			#endregion


			private readonly ItemListVerticalContentView view;
			private IEventProcessor				processor;
		}
	}
}