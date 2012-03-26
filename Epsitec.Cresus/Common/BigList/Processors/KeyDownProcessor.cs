//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public sealed class KeyDownProcessor : EventProcessor
	{
		private KeyDownProcessor(IEventProcessorHost host)
		{
			this.host = host;
			this.policy = this.host.GetPolicy<KeyDownProcessorPolicy> ();
			this.scrollingProcessor = this.host as IScrollingProcessor;
		}

		public static bool Attach(IEventProcessorHost host, Message message, Point pos)
		{
			if (host.EventProcessors.OfType<KeyDownProcessor> ().Any ())
			{
				return false;
			}
			
			if ((message.IsAltPressed) ||
				(message.IsShiftPressed))
			{
				return false;
			}

			var proc = new KeyDownProcessor (host);

			proc.host.Register (proc);
			proc.Process (message, pos);

			return true;
		}


		protected override bool Process(Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.KeyDown:
					return this.ProcessKeyDown (message);
			}
			
			return false;
		}

		private bool ProcessKeyDown(Message message)
		{
			if ((message.IsAltPressed) ||
				(message.IsShiftPressed))
			{
				return false;
			}

			if (message.IsControlPressed)
			{
				if (this.scrollingProcessor == null)
				{
					return false;
				}

				switch (this.policy.ScrollMode)
				{
					case ScrollMode.MoveActive:
						break;
					
					case ScrollMode.MoveFocus:
						break;
					
					case ScrollMode.MoveVisible:
						return this.ProcessScrollVisible (message.KeyCodeOnly);
				}

				return false;
			}
#if false
			int index = this.view.ActiveIndex;

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
#endif

			return false;
		}

		private bool ProcessScrollVisible(KeyCode code)
		{

#if false
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
			}
#endif
			switch (code)
			{
				case KeyCode.Home:
					this.scrollingProcessor.Scroll (new Point (0, -1), ScrollUnit.Document);
					return true;

				case KeyCode.End:
					this.scrollingProcessor.Scroll (new Point (0, 1), ScrollUnit.Document);
					return true;

				case KeyCode.ArrowUp:
					this.scrollingProcessor.Scroll (new Point (0, 1), ScrollUnit.Line);
					return true;

				case KeyCode.ArrowDown:
					this.scrollingProcessor.Scroll (new Point (0, -1), ScrollUnit.Line);
					return true;

				case KeyCode.PageUp:
					this.scrollingProcessor.Scroll (new Point (0, 1), ScrollUnit.Page);
					return true;

				case KeyCode.PageDown:
					this.scrollingProcessor.Scroll (new Point (0, -1), ScrollUnit.Page);
					return true;
			}

			return false;
		}

		private readonly IEventProcessorHost	host;
		private readonly IScrollingProcessor	scrollingProcessor;
		private readonly KeyDownProcessorPolicy policy;
	}
}
