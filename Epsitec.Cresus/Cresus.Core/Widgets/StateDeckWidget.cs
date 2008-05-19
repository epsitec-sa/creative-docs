//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.States;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>StateDeckWidget</c> class represents a deck of states; every state
	/// looks like a card. The cards can be represented as a pile or as a spread.
	/// </summary>
	public class StateDeckWidget : FrameBox
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StateDeckWidget"/> class.
		/// </summary>
		public StateDeckWidget()
		{
			this.stateRecords = new Dictionary<CoreState, Record> ();
			this.randomizer = new System.Random (0);
		}


		/// <summary>
		/// Gets or sets the deck type.
		/// </summary>
		/// <value>The deck type.</value>
		public StateDeck						StateDeck
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the state manager.
		/// </summary>
		/// <value>The state manager.</value>
		public StateManager						StateManager
		{
			get
			{
				return this.manager;
			}
			set
			{
				if (this.manager != value)
				{
					if (this.manager != null)
					{
						this.manager.StackChanged -= this.HandleManagerStackChanged;
					}

					this.manager = value;

					if (this.manager != null)
					{
						this.manager.StackChanged += this.HandleManagerStackChanged;
					}
				}
			}
		}


		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.StateManager = null;
			}

			base.Dispose (disposing);
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

		private void HandleManagerStackChanged(object sender, StateStackChangedEventArgs e)
		{
			if ((e.State == null) ||
				(e.State.StateDeck == this.StateDeck))
			{
				this.Invalidate ();

				int i = 0;

				foreach (string title in from state in this.StateManager.GetAllStates ()
										 select state.Title)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Stack {0}: {1}", i++, title));
				}

				i = 0;
				
				foreach (string title in from state in this.StateManager.GetHistoryStates (ListSortDirection.Descending)
										 select state.Title)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("History {0}: {1}", i++, title));
				}

				System.Diagnostics.Debug.WriteLine ("");
			}
		}


		/// <summary>
		/// Paints the deck for the history pile. The cards will be piled one
		/// on top of the other, with variations in their orientation and
		/// offset.
		/// </summary>
		/// <param name="graphics">The graphics context.</param>
		private void PaintHistoryDeck(Graphics graphics)
		{
			Transform transform = new Transform (graphics.Transform);
			
			Size   size   = this.Client.Size;
			double dim    = System.Math.Min (size.Width, size.Height);
			double center = 80;
			double scale  = dim / (2*center);
			
			foreach (CoreState state in this.StateManager.GetZOrderStates (StateDeck.History))
			{
				Record record;

				if (this.stateRecords.TryGetValue (state, out record) == false)
				{
					this.stateRecords[state] = record = new Record (state, this.randomizer);
				}

				graphics.ScaleTransform (scale, scale, 0, 0);
				graphics.TranslateTransform (center - 50 + record.OffsetX, center - 50 + record.OffsetY);
				graphics.RotateTransformDeg (record.Angle, 50, 50);

				state.PaintMiniature (graphics);

				graphics.Transform = transform;
			}
		}

		/// <summary>
		/// Paints the deck for a spread. The cards will be spread side by side
		/// if there is enough room, with variations in their orientation.
		/// </summary>
		/// <param name="graphics">The graphics context.</param>
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

				this.stateRecords[state] = new Record (record, offset / scale, 0.0);
				
				offset += distance;
			}

			foreach (CoreState state in this.StateManager.GetZOrderStates (StateDeck.StandAlone))
			{
				Record record;

				if (this.stateRecords.TryGetValue (state, out record))
				{
					graphics.ScaleTransform (scale, scale, 0, 0);
					graphics.TranslateTransform (center - 100/2 + record.OffsetX, center - 100/2);
					graphics.RotateTransformDeg (record.Angle, 50, 50);
				}

				state.PaintMiniature (graphics);

				graphics.Transform = transform;
			}
		}

		
		#region Record Structure

		/// <summary>
		/// The <c>Record</c> structure defines how a state's card gets laid out
		/// in the deck (angle and offset).
		/// </summary>
		struct Record : System.IEquatable<Record>
		{
			public Record(CoreState state, System.Random randomizer)
			{
				this.state   = state;
				this.angle   = (randomizer.NextDouble () - 0.5) * 30;
				this.offsetX = (randomizer.NextDouble () - 0.5) * 40;
				this.offsetY = (randomizer.NextDouble () - 0.5) * 20;
			}

			public Record(Record model, double offsetX, double offsetY)
			{
				this.state = model.State;
				this.angle = model.Angle;
				this.offsetX = offsetX;
				this.offsetY = offsetY;
			}

			public CoreState State
			{
				get
				{
					return this.state;
				}
			}

			public double Angle
			{
				get
				{
					return this.angle;
				}
			}

			public double OffsetX
			{
				get
				{
					return this.offsetX;
				}
			}

			public double OffsetY
			{
				get
				{
					return this.offsetY;
				}
			}

			#region IEquatable<Record> Members

			public bool Equals(Record other)
			{
				return this.state == other.state
					&& this.angle == other.angle
					&& this.offsetX == other.offsetX
					&& this.offsetY == other.offsetY;
			}

			#endregion

			public override bool Equals(object obj)
			{
				if (obj is Record)
				{
					return this.Equals ((Record) obj);
				}
				else
				{
					return false;
				}
			}

			public override int GetHashCode()
			{
				return this.state == null ? 0 : this.state.GetHashCode ();
			}

			private readonly CoreState state;
			private readonly double angle;
			private readonly double offsetX;
			private readonly double offsetY;
		}

		#endregion


		private readonly Dictionary<CoreState, Record> stateRecords;
		private readonly System.Random randomizer;
		private StateManager manager;
	}
}
