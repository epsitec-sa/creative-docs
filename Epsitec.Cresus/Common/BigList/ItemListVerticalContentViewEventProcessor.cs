//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemListVerticalContentViewEventProcessor : IEventProcessorHost
	{
		public ItemListVerticalContentViewEventProcessor(ItemListVerticalContentView view)
		{
			this.view = view;
		}


		public bool ProcessMessage(Message message, Point pos)
		{
			if (this.view.ItemList == null)
			{
				return false;
			}

			if (this.EventProcessor != null)
			{
				if (this.EventProcessor.ProcessMessage (message, pos))
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
					this.ProcessMouseWheel (message.WheelAmplitude);
					return true;
			}

			return false;
		}

		private bool ProcessMouseDown(Message message, Point pos)
		{
			if (this.EventProcessor != null)
			{
				return false;
			}

			var row = this.view.ItemList.VisibleRows.FirstOrDefault (x => this.view.GetRowBounds (x).Contains (pos));

			if (row == null)
			{
				return false;
			}

			this.view.ItemList.Select (row.Index, ItemSelection.Toggle);
			this.view.Invalidate ();

			this.EventProcessor = new MouseDownProcessor (this, message, pos);
			
			return true;
		}


		private void ProcessMouseWheel(double amplitude)
		{
			this.view.ItemList.MoveVisibleContent ((int)(amplitude * this.view.DefaultLineHeight));
			this.view.Invalidate ();
		}


		#region IEventProcessorHost Members

		public EventProcessor					EventProcessor
		{
			get;
			set;
		}

		void IEventProcessorHost.Remove(IEventProcessor processor)
		{
			if (this.EventProcessor == processor)
			{
				this.EventProcessor = null;
			}
		}

		#endregion


		private readonly ItemListVerticalContentView view;
	}

	public interface IEventProcessor
	{
	}

	public abstract class EventProcessor : IEventProcessor
	{
		public abstract bool ProcessMessage(Message message, Point pos);
	}

	public interface IEventProcessorHost
	{
		void Remove(IEventProcessor processor);

		EventProcessor EventProcessor
		{
			get;
			set;
		}
	}
	
	public class MouseDownProcessor : EventProcessor
	{
		public MouseDownProcessor(IEventProcessorHost host, Message message, Point pos)
		{
			this.host   = host;
			this.button = message.Button;
			this.origin = pos;
		}

		public override bool ProcessMessage(Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseMove:
					break;
				
				case MessageType.MouseUp:
					if (message.Button == this.button)
					{
						this.host.Remove (this);
						return true;
					}
					break;
			}

			return false;
		}

		private readonly IEventProcessorHost	host;
		private readonly MouseButtons			button;
		private readonly Point					origin;
	}
}
