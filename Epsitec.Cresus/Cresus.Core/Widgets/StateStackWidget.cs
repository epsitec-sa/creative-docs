//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.States;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Widgets
{
	public class StateStackWidget : FrameBox
	{
		public StateStackWidget()
		{
			this.stateRecords = new Dictionary<CoreState, Record> ();
			this.randomizer = new System.Random (0);
		}



		public StateDeck StateDeck
		{
			get;
			set;
		}

		public StateManager StateManager
		{
			get;
			set;
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			switch (this.StateDeck)
			{
				case StateDeck.History:
					this.PaintHistoryDeck (graphics);
					break;

				case StateDeck.StandAlone:
					this.PaintStandAloneDeck (graphics);
					break;
			}
		}

		private void PaintHistoryDeck(Graphics graphics)
		{
			Transform transform = new Transform (graphics.Transform);
			
			Size   size   = this.Client.Size;
			double dim    = System.Math.Min (size.Width, size.Height);
			double center = 80;
			double scale  = dim / (2*center);
			
			foreach (CoreState state in this.StateManager.GetDeckStates (StateDeck.History))
			{
				Record record;

				if (this.stateRecords.TryGetValue (state, out record) == false)
				{
					this.stateRecords[state] = record = new Record (state, this.randomizer);
				}

				graphics.ScaleTransform (scale, scale, 0, 0);
				graphics.TranslateTransform (center - 50 + record.JitterX, center - 50 + record.JitterY);
				graphics.RotateTransformDeg (record.Angle, 50, 50);

				state.PaintMiniature (graphics);

				graphics.Transform = transform;
			}
		}

		private void PaintStandAloneDeck(Graphics graphics)
		{
			Transform transform = new Transform (graphics.Transform);

			Size   size   = this.Client.Size;
			double dim    = System.Math.Min (size.Width, size.Height);
			double center = 80;
			double scale  = dim / (2*center);
			double range  = System.Math.Max (size.Width, size.Height) - dim;

			List<CoreState> states = new List<CoreState> (this.StateManager.GetAllStates (StateDeck.StandAlone));

			double distance = 0;
			double offset   = 0;

			int n = states.Count;
			int i = n;

			if (n > 1)
			{
				distance = System.Math.Min ((100+20) * scale, range / (n-1));
				offset   = (range - (n-1) * distance) / 2;
			}

			foreach (CoreState state in states)
			{
				Record record;

				if (this.stateRecords.TryGetValue (state, out record) == false)
				{
					record = new Record (state, this.randomizer);
				}

				record.JitterY = 0.0;
				record.JitterX = offset / scale;
				
				offset += distance;

				this.stateRecords[state] = record;
			}

			foreach (CoreState state in this.StateManager.GetDeckStates (StateDeck.StandAlone))
			{
				Record record;

				if (this.stateRecords.TryGetValue (state, out record))
				{
#if false
					if (--i == 0)
					{
						graphics.ScaleTransform (1.1, 1.1, 0, 0);
					}
#endif

					graphics.ScaleTransform (scale, scale, 0, 0);
					graphics.TranslateTransform (center - 100/2 + record.JitterX, center - 100/2);
					graphics.RotateTransformDeg (record.Angle, 50, 50);
				}

				state.PaintMiniature (graphics);

				graphics.Transform = transform;
			}
		}

		struct Record
		{
			public Record(CoreState state, System.Random randomizer)
				: this ()
			{
				this.State   = state;
				this.Angle   = (randomizer.NextDouble () - 0.5) * 30;
				this.JitterX = (randomizer.NextDouble () - 0.5) * 40;
				this.JitterY = (randomizer.NextDouble () - 0.5) * 20;
			}

			public CoreState State
			{
				get;
				set;
			}

			public double Angle
			{
				get;
				set;
			}

			public double JitterX
			{
				get;
				set;
			}

			public double JitterY
			{
				get;
				set;
			}
		}


		private Dictionary<CoreState, Record> stateRecords;
		private readonly System.Random randomizer;
	}
}
