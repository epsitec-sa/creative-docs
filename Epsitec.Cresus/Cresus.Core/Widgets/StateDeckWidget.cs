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
			this.centers = new List<Center> ();
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

		public States.CoreState HotState
		{
			get
			{
				return this.hotState;
			}
			set
			{
				if (this.hotState != value)
				{
					this.hotState = value;
					this.Invalidate ();
				}
			}
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

		protected override void DispatchMessage(Message message, Point pos)
		{
			base.DispatchMessage (message, pos);

			if ((this.StateDeck == StateDeck.StandAlone) &&
				(message.IsMouseType))
			{
				switch (message.MessageType)
				{
					case MessageType.MouseLeave:
						this.HotState = null;
						break;

					case MessageType.MouseMove:
						this.HotState = this.FindState (pos.X);
						break;
				}
			}
		}

		protected override void OnClicked(MessageEventArgs e)
		{
			base.OnClicked (e);

			if ((e.Message.Button == MouseButtons.Left) &&
				(e.Message.ModifierKeys == ModifierKeys.None) &&
				(this.HotState != null))
			{
				this.StateManager.Push (this.HotState);
			}
		}

		private void HandleManagerStackChanged(object sender, StateStackChangedEventArgs e)
		{
			if ((e.State == null) ||
				(e.State.StateDeck == this.StateDeck))
			{
				this.Invalidate ();
			}
		}
		
		private States.CoreState FindState(double x)
		{
			return (from item in this.centers
					let delta = System.Math.Abs (x - item.X)
					orderby delta ascending
					select item.State).FirstOrDefault ();
		}


		/// <summary>
		/// Paints the deck for the history pile. The cards will be piled one
		/// on top of the other, with variations in their orientation and
		/// offset.
		/// </summary>
		/// <param name="graphics">The graphics context.</param>
		private void PaintHistoryDeck(Graphics graphics)
		{
			Transform transform = graphics.Transform;
			
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

				this.PaintStateMiniature (graphics, state, null);

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
			Transform transform = graphics.Transform;

			Size   size   = this.Client.Size;
			double dim    = System.Math.Min (size.Width, size.Height);
			double center = 80;
			double scale  = dim / (2*center);
			double range  = System.Math.Max (size.Width, size.Height) - dim;

			List<CoreState> states = new List<CoreState> (this.StateManager.GetAllStates (StateDeck.StandAlone));

			double distance = 0;
			double offset   = range/2;
			double angleInc = 0;
			double angleR   = 0;
			double angle    = 0;
			double deltaY   = 0;

			int n = states.Count;
			int i = n;

			if (n > 1)
			{
				distance = System.Math.Min ((100+20) * scale, range / (n-1));
				offset   = (range - (n-1) * distance) / 2;
				angleR   = System.Math.Min (30, n * 5);
				angleInc = angleR / (n-1);
				angle    = angleR / 2;
				deltaY   = 16 / (1-System.Math.Cos (angle * System.Math.PI/180));
			}

			this.centers.Clear ();

			foreach (CoreState state in states)
			{
				double oy = deltaY * (1-System.Math.Cos (angle * System.Math.PI/180));
				this.stateRecords[state] = new Record (state, offset / scale, 16-oy, angle);
				
				offset += distance;
				angle  -= angleInc;
			}

			bool paintHotState = false;

			foreach (CoreState state in this.StateManager.GetZOrderStates (StateDeck.StandAlone))
			{
				if (this.hotState == state)
				{
					paintHotState = true;
				}
				else
				{
					this.TransformAndPaintMiniature (graphics, transform, center, scale, state, null);
				}
			}

			if (paintHotState)
			{
				this.TransformAndPaintMiniature (graphics, transform, center, scale, this.hotState,
					frame =>
					{
						graphics.AddFilledRectangle (frame);
						graphics.RenderSolid (Color.FromAlphaRgb (0.5, 1, 0.6, 0));
					});
			}
		}

		private void TransformAndPaintMiniature(Graphics graphics, Transform transform, double center, double scale, CoreState state, System.Action<Rectangle> overlayPainter)
		{
			Record record;

			if (this.stateRecords.TryGetValue (state, out record))
			{
				double hh = 100 / 2;
				double cx = center + record.OffsetX;
				double cy = center + record.OffsetY;

				graphics.ScaleTransform (scale, scale, 0, 0);
				graphics.TranslateTransform (cx - hh, cy - hh);
				graphics.RotateTransformDeg (record.Angle, hh, hh);
				
				this.PaintStateMiniature (graphics, state, overlayPainter);

				this.centers.Add (new Center (state, cx*scale, cy*scale));
			}

			graphics.Transform = transform;
		}

		private void PaintStateMiniature(Graphics graphics, CoreState state, System.Action<Rectangle> overlayPainter)
		{
			Rectangle frame = new Rectangle (0, 0, 100, 100);

			if (this.StateManager.ActiveState == state)
			{
				Rectangle hilite = Rectangle.Inflate (frame, 5, 5);

				graphics.AddFilledRectangle (hilite);
				graphics.RenderSolid (Color.FromRgb (1.0, 0.5, 0.0));

				hilite.Inflate (5, 5);

				graphics.AddFilledRectangle (hilite);
				graphics.RenderSolid (Color.FromAlphaRgb (0.5, 1.0, 0.5, 0.0));
			}

			state.PaintMiniature (graphics, frame);

			if (overlayPainter != null)
			{
				overlayPainter (frame);
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

			public Record(CoreState state, double offsetX, double offsetY, double angle)
			{
				this.state = state;
				this.angle = angle;
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

		#region Center Structure

		/// <summary>
		/// The <c>Center</c> structure stores just the center position of the
		/// states used in the stand alone deck.
		/// </summary>
		private struct Center
		{
			public Center(States.CoreState state, double x, double y)
			{
				this.state = state;
				this.x = x;
				this.y = y;
			}

			public States.CoreState State
			{
				get
				{
					return this.state;
				}
			}

			public double X
			{
				get
				{
					return this.x;
				}
			}

			public double Y
			{
				get
				{
					return this.y;
				}
			}

			private readonly States.CoreState state;
			private readonly double x;
			private readonly double y;
		}

		#endregion


		private readonly Dictionary<CoreState, Record> stateRecords;
		private readonly System.Random randomizer;
		private readonly List<Center> centers;
		
		private StateManager manager;
		private States.CoreState hotState;
	}
}
