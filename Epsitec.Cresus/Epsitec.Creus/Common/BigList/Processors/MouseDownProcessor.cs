//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Behaviors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public sealed class MouseDownProcessor : EventProcessor
	{
		private MouseDownProcessor(IEventProcessorHost host, Rectangle bounds, Message message, Point pos)
		{
			this.host               = host;
			this.bounds             = bounds;
			this.selectionProcessor = this.host as ISelectionProcessor;
			this.detectionProcessor = this.host as IDetectionProcessor;
			this.scrollingProcessor = this.host as IScrollingProcessor;
			this.policy             = this.host.GetPolicy<MouseDownProcessorPolicy> ();
			this.button             = message.Button;
			this.origin             = pos;

			this.originalIndex     = this.detectionProcessor.Detect (pos);
			this.originalSelection = this.selectionProcessor == null ? false : this.selectionProcessor.IsSelected (this.originalIndex);

			if ((this.policy.AutoScroll) &&
				(this.scrollingProcessor != null))
			{
				this.autoScrollBehavior = new AutoScrollBehavior (this.HandleScrolling);
				
				this.autoScrollBehavior.InitialDelay = this.policy.AutoScrollDelay;
				this.autoScrollBehavior.RepeatPeriod = this.policy.AutoScrollRepeat;
			}
		}


		public static bool Attach(IEventProcessorHost host, Rectangle bounds, Message message, Point pos)
		{
			if (host.EventProcessors.OfType<MouseDownProcessor> ().Any ())
			{
				return false;
			}

			var proc = new MouseDownProcessor (host, bounds, message, pos);

			if (proc.originalIndex < 0)
			{
				return false;
			}

			proc.host.Add (proc);
			proc.Process (message, pos);
			
			return true;
		}

		protected override bool Process(Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseDown:
					if ((message.Button == this.button) &&
						(this.policy.SelectionPolicy == SelectionPolicy.OnMouseDown) &&
						(this.selectionProcessor != null))
					{
						this.selectionProcessor.Select (this.originalIndex, ItemSelection.Toggle);
					}
					
					this.ProcessAutoScroll (message, pos);
					this.ProcessMove (pos);
					break;

				case MessageType.MouseMove:
					this.ProcessAutoScroll (message, pos);
					this.ProcessMove (pos);
					break;

				case MessageType.MouseUp:

					this.ProcessMove (pos);

					if (message.Button == this.button)
					{
						this.ProcessAutoScrollEnd ();

						if (this.selectionProcessor != null)
						{
							if (this.policy.SelectionPolicy == SelectionPolicy.OnMouseUp)
							{
								this.selectionProcessor.Select (this.originalIndex, ItemSelection.Toggle);
							}

							this.selectionProcessor.Select (this.originalIndex, ItemSelection.Focus);
							this.selectionProcessor.Select (this.originalIndex, ItemSelection.Activate);
						}
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

		private void ProcessAutoScroll(Message message, Point pos)
		{
			if ((this.policy.AutoScroll) &&
				(this.scrollingProcessor != null))
			{
				var amplitude = Point.Zero;
				
				var bounds = this.bounds;
				var margin = 5;

				if (bounds.Bottom + margin > pos.Y)
				{
					amplitude = new Point (0, -1);
					pos       = new Point (pos.X, System.Math.Max (bounds.Bottom, pos.Y));
				}
				if (bounds.Top - margin < pos.Y)
				{
					amplitude = new Point (0, 1);
					pos       = new Point (pos.X, System.Math.Min (bounds.Top-1, pos.Y));
				}
				
				this.originalScrollPos = pos;

				this.autoScrollBehavior.ProcessEvent (amplitude, message);
			}
		}

		private void ProcessAutoScrollEnd()
		{
			if ((this.policy.AutoScroll) &&
				(this.scrollingProcessor != null))
			{
				this.autoScrollBehavior.ProcessEvent (Point.Zero);
			}
		}


		private void HandleScrolling(Point amplitude)
		{
			this.scrollingProcessor.Scroll (amplitude, ScrollUnit.Line, ScrollMode.MoveVisible);
			this.ProcessMove (this.originalScrollPos);
		}

		private readonly IEventProcessorHost	host;
		private readonly Rectangle				bounds;
		
		private readonly ISelectionProcessor	selectionProcessor;
		private readonly IDetectionProcessor	detectionProcessor;
		private readonly IScrollingProcessor	scrollingProcessor;

		private readonly MouseDownProcessorPolicy policy;
		private readonly MouseButtons			button;
		private readonly Point					origin;
		private readonly AutoScrollBehavior		autoScrollBehavior;

		private bool							originalSelection;
		private int								originalIndex;
		private Point							originalScrollPos;
	}
}
