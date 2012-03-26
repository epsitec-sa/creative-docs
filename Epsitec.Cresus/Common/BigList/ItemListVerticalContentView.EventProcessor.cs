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
						return this.ProcessMouseWheel (message.WheelAmplitude);

					case MessageType.KeyDown:
						return this.ProcessKeyDown (message);

				}

				return false;
			}

			private bool ProcessMouseDown(Message message, Point pos)
			{
				return MouseDownProcessor.Attach (this, this.view.Client.Bounds, message, pos);
			}

			private bool ProcessMouseWheel(double amplitude)
			{
				this.view.Scroll (amplitude, ScrollUnit.Line);
				return true;
			}

			private bool ProcessKeyDown(Message message)
			{
				if ((message.IsAltPressed) ||
					(message.IsShiftPressed))
				{
					return false;
				}

				int index = this.view.ActiveIndex;

				if (message.IsControlPressed)
				{
					return this.ProcessScrollWithKeyboard (message.KeyCodeOnly);
				}

				switch (message.KeyCodeOnly)
				{
					case KeyCode.Home:
						index = 0;
						break;
					
					case KeyCode.End:
						index = this.view.ItemList.Count - 1;
						break;

					case KeyCode.ArrowUp:
						index--;
						break;
					case KeyCode.ArrowDown:
						index++;
						break;
					case KeyCode.PageUp:
						index = this.view.ItemList.VisibleRows.First ().Index - 1;
						break;
					case KeyCode.PageDown:
						index = this.view.ItemList.VisibleRows.Last ().Index + 1;
						break;

					default:
						return false;
				}

				this.view.ActivateRow (index);
				this.view.SelectRow (index, ItemSelection.Select);
				
				this.view.FocusRow (index);

				return true;
			}

			private bool ProcessScrollWithKeyboard(KeyCode code)
			{
				switch (code)
				{
					case KeyCode.Home:
						this.view.FocusRow (0);
						return true;

					case KeyCode.End:
						this.view.FocusRow (this.view.ItemList.Count-1);
						return true;

					case KeyCode.ArrowUp:
						this.view.Scroll (1, ScrollUnit.Line);
						return true;

					case KeyCode.ArrowDown:
						this.view.Scroll (-1, ScrollUnit.Line);
						return true;

					case KeyCode.PageUp:
						this.view.Scroll (1, ScrollUnit.Page);
						return true;

					case KeyCode.PageDown:
						this.view.Scroll (-1, ScrollUnit.Page);
						return true;
				}

				return false;
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
				return new TPolicy ();
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

			void IScrollingProcessor.ScrollByAmplitude(Point amplitude)
			{
				this.view.Scroll (amplitude.Y, ScrollUnit.Line);
			}

			#endregion


			private readonly ItemListVerticalContentView view;
			private IEventProcessor				processor;
		}
	}
}