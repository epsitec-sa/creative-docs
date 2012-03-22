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
	public class ItemListVerticalContentViewEventProcessor : IEventProcessorHost, IDetectionProcessor, ISelectionProcessor
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
			
			new MouseDownProcessor (this, message, pos);
			
			return true;
		}


		private void ProcessMouseWheel(double amplitude)
		{
			this.view.Scroll (amplitude * this.view.DefaultLineHeight);
		}


		#region IEventProcessorHost Members

		public IEventProcessor					EventProcessor
		{
			get;
			set;
		}

		void IEventProcessorHost.Register(IEventProcessor processor)
		{
			this.EventProcessor = processor;
		}

		void IEventProcessorHost.Remove(IEventProcessor processor)
		{
			if (this.EventProcessor == processor)
			{
				this.EventProcessor = null;
			}
		}

		TPolicy IEventProcessorHost.GetPolicy<TPolicy>()
		{
			return new TPolicy ();
		}

		#endregion



		private readonly ItemListVerticalContentView view;

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

	}

	public interface IDetectionProcessor
	{
		int Detect(Point pos);
	}

	public interface ISelectionProcessor
	{
		bool IsSelected(int index);
		void Select(int index, ItemSelection selection);
	}

	public interface IEventProcessor
	{
		bool ProcessMessage(Message message, Point pos);
	}

	public abstract class EventProcessor : IEventProcessor
	{
		public abstract bool ProcessMessage(Message message, Point pos);
	}

	public abstract class EventProcessorPolicy
	{
	}

	public class MouseDownProcessorPolicy : EventProcessorPolicy
	{
		public MouseDownProcessorPolicy()
		{
			this.AutoFollow = true;
		}

		public bool AutoFollow
		{
			get;
			set;
		}
	}

	public interface IEventProcessorHost
	{
		void Register(IEventProcessor processor);
		void Remove(IEventProcessor processor);

		IEventProcessor EventProcessor
		{
			get;
			set;
		}

		TPolicy GetPolicy<TPolicy>()
			where TPolicy : EventProcessorPolicy, new ();
	}
	
	public class MouseDownProcessor : EventProcessor
	{
		public MouseDownProcessor(IEventProcessorHost host, Message message, Point pos)
		{
			this.host   = host;
			this.selectionProcessor = host as ISelectionProcessor;
			this.detectionProcessor = host as IDetectionProcessor;
			this.policy = host.GetPolicy<MouseDownProcessorPolicy> ();
			this.button = message.Button;
			this.origin = pos;

			int index = this.detectionProcessor.Detect (pos);

			if (index < 0)
			{
				return;
			}

			this.host.Register (this);

			this.originalIndex = index;
			this.originalSelection = this.selectionProcessor.IsSelected (index);
			
			this.selectionProcessor.Select (index, ItemSelection.Toggle);
		}

		public override bool ProcessMessage(Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseMove:
					this.ProcessMove (pos);
					break;
				
				case MessageType.MouseUp:
					
					this.ProcessMove (pos);
					
					if (message.Button == this.button)
					{
						this.selectionProcessor.Select (this.originalIndex, ItemSelection.Focus);
						this.host.Remove (this);
						
						return true;
					}
					break;
			}

			return false;
		}

		private void ProcessMove(Point pos)
		{
			if (this.policy.AutoFollow)
			{
				int oldIndex = this.originalIndex;
				int newIndex = this.detectionProcessor.Detect (pos);

				if (newIndex != oldIndex)
				{
					bool newSelection = this.selectionProcessor.IsSelected (newIndex);
					
					this.selectionProcessor.Select (newIndex, ItemSelection.Toggle);
					this.selectionProcessor.Select (oldIndex, this.originalSelection ? ItemSelection.Select : ItemSelection.Deselect);
					this.selectionProcessor.Select (newIndex, ItemSelection.Activate);

					this.originalIndex     = newIndex;
					this.originalSelection = newSelection;
				}
			}
		}

		private readonly IEventProcessorHost	host;
		private readonly ISelectionProcessor	selectionProcessor;
		private readonly IDetectionProcessor	detectionProcessor;
		private readonly MouseDownProcessorPolicy policy;
		private readonly MouseButtons			button;
		private readonly Point					origin;
		private bool							originalSelection;
		private int								originalIndex;
	}
}
