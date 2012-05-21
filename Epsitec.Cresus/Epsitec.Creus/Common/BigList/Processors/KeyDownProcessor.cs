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

			proc.host.Add (proc);
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

			if (this.scrollingProcessor == null)
			{
				return false;
			}

			if (message.IsControlPressed)
			{
				switch (this.policy.PassiveScrollMode)
				{
					case ScrollMode.MoveActive:
					case ScrollMode.MoveFocus:
					case ScrollMode.MoveVisible:
						return this.ProcessScroll (message.KeyCodeOnly, this.policy.PassiveScrollMode);
				}

				return false;
			}

			return this.ProcessScroll (message.KeyCodeOnly, ScrollMode.MoveActiveAndSelect);
		}

		private bool ProcessScroll(KeyCode code, ScrollMode scrollMode)
		{
			switch (code)
			{
				case KeyCode.Home:
					this.scrollingProcessor.Scroll (new Point (0, 1), ScrollUnit.Document, scrollMode);
					return true;

				case KeyCode.End:
					this.scrollingProcessor.Scroll (new Point (0, -1), ScrollUnit.Document, scrollMode);
					return true;

				case KeyCode.ArrowUp:
					this.scrollingProcessor.Scroll (new Point (0, 1), ScrollUnit.Line, scrollMode);
					return true;

				case KeyCode.ArrowDown:
					this.scrollingProcessor.Scroll (new Point (0, -1), ScrollUnit.Line, scrollMode);
					return true;

				case KeyCode.PageUp:
					this.scrollingProcessor.Scroll (new Point (0, 1), ScrollUnit.Page, scrollMode);
					return true;

				case KeyCode.PageDown:
					this.scrollingProcessor.Scroll (new Point (0, -1), ScrollUnit.Page, scrollMode);
					return true;
			}

			return false;
		}

		private readonly IEventProcessorHost	host;
		private readonly IScrollingProcessor	scrollingProcessor;
		private readonly KeyDownProcessorPolicy policy;
	}
}
