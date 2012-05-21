//	Copyright © 2004-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Animator permet d'animer des widgets de manière simplifiée,
	/// en offrant le support nécessaire pour faire évoluer une valeur de A à B
	/// progressivement.
	/// </summary>
	public sealed class Animator : System.IDisposable
	{
		public Animator(double timeSpan, AnimatorMode mode = AnimatorMode.OneShot)
		{
			System.Diagnostics.Debug.Assert (timeSpan > 0);
			
			this.timeSpan = timeSpan;
			this.mode     = mode;
			this.values   = new List<AnimatorValue> ();
		}


		public void SetCallback<T>(System.Action<T> callbackChanged, System.Action<Animator> callbackFinished = null)
		{
			this.callbackChanged  = () => callbackChanged (this.GetValue<T> (0));
			this.callbackFinished = callbackFinished;
		}

		public void SetCallback<T1, T2>(System.Action<T1, T2> callbackChanged, System.Action<Animator> callbackFinished = null)
		{
			this.callbackChanged  = () => callbackChanged (this.GetValue<T1> (0), this.GetValue<T2> (1));
			this.callbackFinished = callbackFinished;
		}
		
		
		
		public void SetValue<T>(T begin, T end)
			where T : struct
		{
			this.SetValue (0, begin, end);
		}
		
		public void SetValue<T>(int i, T begin, T end)
			where T : struct
		{
			int oldCount = this.Count;
			
			if (i >= oldCount)
			{
				this.values.AddRange (Enumerable.Repeat<AnimatorValue> (null, i-oldCount+1));
			}

			this.values[i] = new AnimatorValue<T> (begin, end);
		}
		
		public T GetValue<T>(int i)
		{
			return (T) this.GetValue (i, this.ratio);
		}
		
		
		public void Start()
		{
			if (this.timer == null)
			{
				this.timer            = new Timer ();
				this.timer.Delay      = 0.010;
				this.timer.AutoRepeat = 0.010;

				this.timer.TimeElapsed += this.HandleTimerTimeElapsed;

				this.tickBegin  = System.DateTime.Now.Ticks;
				this.tickEnd    = this.tickBegin + this.TickSpan;
				this.finished   = false;
			}
			
			this.timer.Start ();
		}
		
		public void Pause()
		{
			if (this.timer != null)
			{
				this.timer.Stop ();
			}
		}

		public void Stop()
		{
			if (this.finished)
			{
				return;
			}

			if (this.timer != null)
			{
				this.timer.TimeElapsed -= this.HandleTimerTimeElapsed;
				this.timer.Dispose ();
				this.timer = null;
			}
			
			this.finished = true;

			this.Finished.Raise (this);

			if (this.callbackFinished != null)
			{
				this.callbackFinished (this);
			}
		}
		
		
		public int								Count
		{
			get
			{
				return this.values.Count;
			}
		}
		
		public double							Ratio
		{
			get
			{
				return this.ratio;
			}
		}
		
		public double							TimeSpan
		{
			get
			{
				return this.timeSpan;
			}
		}


		private long							TickSpan
		{
			get
			{
				return (long) (this.timeSpan * 10000000);
			}
		}

		public event Support.EventHandler		Changed;
		public event Support.EventHandler		Finished;
		
		
		private void HandleTimerTimeElapsed(object sender)
		{
			long now = System.DateTime.Now.Ticks;
			bool stop = false;
			
			while (now > this.tickEnd)
			{
				switch (this.mode)
				{
					case AnimatorMode.AutoRestart:
						this.tickBegin += this.TickSpan;
						this.tickEnd   += this.TickSpan;
						break;
					
					default:
						this.timer.Stop ();
						now  = this.tickEnd;
						stop = true;
						break;
				}
			}
			
			this.ratio = (double)(now - this.tickBegin) / (double)(this.tickEnd - this.tickBegin);

			this.Changed.Raise (this);
			
			if (this.callbackChanged != null)
			{
				this.callbackChanged ();
			}
			
			if (stop)
			{
				this.Stop ();
			}
		}

		private object GetValue(int i, double ratio)
		{
			return this.values[i].Interpolate (ratio);
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Stop ();

			this.values.Clear ();
			this.callbackChanged  = null;
			this.callbackFinished = null;
		}

		#endregion


		private readonly List<AnimatorValue>	values;
		private readonly double					timeSpan;
		private readonly AnimatorMode			mode;
		private Timer							timer;
		private double							ratio;
		private long							tickBegin;
		private long							tickEnd;
		private System.Action					callbackChanged;
		private System.Action<Animator>			callbackFinished;
		private bool							finished;
	}
}
