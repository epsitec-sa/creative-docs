namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Animator permet d'animer des widgets de manière simplifiée,
	/// en offrant le support nécessaire pour faire évoluer une valeur de A à B
	/// progressivement.
	/// </summary>
	public class Animator : System.IDisposable
	{
		public Animator(double time_span)
		{
			System.Diagnostics.Debug.Assert (time_span > 0);
			
			this.time_span = time_span;
		}
		
		
		
		public void SetCallback(System.Delegate callback_changed, System.Delegate callback_finished)
		{
			this.callback_changed  = callback_changed;
			this.callback_finished = callback_finished;
		}
		
		public void SetValue(object begin, object end)
		{
			this.SetValue (0, begin, end);
		}
		
		public void SetValue(int i, object begin, object end)
		{
			int old_count = this.Count;
			
			if (i >= old_count)
			{
				Value[] old_values = this.values;
				
				this.values  = new Value[i+1];
				
				for (int j = 0; j < old_count; j++)
				{
					this.values[j] = old_values[j];
				}
			}
			
			Value v = new Value ();
			
			v.BeginValue = begin;
			v.EndValue   = end;
			
			if (v.IsValid == false)
			{
				throw new System.ArgumentException ("Invalid begin/end");
			}
			
			this.values[i] = v;
		}
		
		
		public object GetValue()
		{
			return this.GetValue (0, this.ratio);
		}
		
		public object GetValue(int i)
		{
			return this.GetValue (i, this.ratio);
		}
		
		
		public void Start()
		{
			if (this.timer == null)
			{
				this.timer = new Timer ();
				this.timer.Delay = 0.010;
				this.timer.AutoRepeat = 0.010;
				this.timer.TimeElapsed += new EventHandler (HandleTimerTimeElapsed);
			}
			
			this.tick_begin  = System.DateTime.Now.Ticks;
			this.tick_end    = this.tick_begin + (long)(this.time_span * 10000000);
			
			this.timer.Start ();
		}
		
		public void Stop()
		{
			if (this.timer != null)
			{
				this.timer.Stop ();
			}
		}
		
		
		public int								Count
		{
			get { return this.values == null ? 0 : this.values.Length; }
		}
		
		public double							Ratio
		{
			get { return this.ratio; }
		}
		
		public double							TimeSpan
		{
			get { return this.time_span; }
		}
		
		
		public event EventHandler				Changed;
		public event EventHandler				Finished;
		
		
		protected virtual void HandleTimerTimeElapsed(object sender)
		{
			long now = System.DateTime.Now.Ticks;
			
			if (now > this.tick_end)
			{
				this.timer.Stop ();
				now = this.tick_end;
			}
			
			this.ratio = (double)(now - this.tick_begin) / (double)(this.tick_end - this.tick_begin);
			
			if (this.Changed != null)
			{
				this.Changed (this);
			}
			
			if (this.callback_changed != null)
			{
				object[] args = new object[this.Count];
				
				for (int i = 0; i < args.Length; i++)
				{
					args[i] = this.GetValue (i);
				}
				
				this.callback_changed.DynamicInvoke (args);
			}
			
			if (now == this.tick_end)
			{
				if (this.Finished != null)
				{
					this.Finished (this);
				}
				if (this.callback_finished != null)
				{
					this.callback_finished.DynamicInvoke (new object[] { this });
				}
			}
		}
		
		protected virtual object GetValue(int i, double ratio)
		{
			return this.values[i].Interpolate (ratio);
		}
		
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.timer != null)
				{
					this.timer.TimeElapsed -= new EventHandler (HandleTimerTimeElapsed);
					this.timer.Dispose ();
					this.timer = null;
				}
			}
		}
		
		
		
		public class Value
		{
			public Value()
			{
			}
			
			
			public object						BeginValue
			{
				get { return this.v1; }
				set
				{
					if (this.Validate (value))
					{
						this.v1 = value;
					}
				}
			}
			
			public object						EndValue
			{
				get { return this.v2; }
				set
				{
					if (this.Validate (value))
					{
						this.v2 = value;
					}
				}
			}
			
			public bool							IsValid
			{
				get { return (this.v1 != null) && (this.v2 != null) && (this.v1.GetType () == this.v2.GetType ()); }
			}
			
			
			public object Interpolate(double ratio)
			{
				return this.Compute (this.v1, this.v2, ratio);
			}
			
			
			protected virtual bool Validate(object value)
			{
				if (value == null)
				{
					return false;
				}
				
				if ((value is int) ||
					(value is double))
				{
					return true;
				}
				
				if ((value is Drawing.Point) ||
					(value is Drawing.Size) ||
					(value is Drawing.Rectangle))
				{
					return true;
				}
				
				if (value is Drawing.Color)
				{
					return true;
				}
				
				return false;
			}
			
			protected virtual object Compute(object a, object b, double ratio)
			{
				System.Diagnostics.Debug.Assert (a != null);
				System.Diagnostics.Debug.Assert (b != null);
				System.Diagnostics.Debug.Assert (a.GetType () == b.GetType ());
				System.Diagnostics.Debug.Assert (ratio >= 0.0);
				System.Diagnostics.Debug.Assert (ratio <= 1.0);
				
				double compl = 1.0 - ratio;
				
				if (a is int)
				{
					int va = (int) a;
					int vb = (int) b;
					int vc = (int) (compl*va + ratio*vb);
					return vc;
				}
				
				if (a is double)
				{
					double va = (double) a;
					double vb = (double) b;
					double vc = (double) (compl*va + ratio*vb);
					return vc;
				}
				
				if (a is Drawing.Point)
				{
					Drawing.Point va = (Drawing.Point) a;
					Drawing.Point vb = (Drawing.Point) b;
					
					return new Drawing.Point (compl*va.X + ratio*vb.X, compl*va.Y + ratio*vb.Y);
				}
				
				if (a is Drawing.Size)
				{
					Drawing.Size va = (Drawing.Size) a;
					Drawing.Size vb = (Drawing.Size) b;
					
					return new Drawing.Size (compl*va.Width + ratio*vb.Width, compl*va.Height + ratio*vb.Height);
				}
				
				if (a is Drawing.Rectangle)
				{
					Drawing.Rectangle va = (Drawing.Rectangle) a;
					Drawing.Rectangle vb = (Drawing.Rectangle) b;
					
					return new Drawing.Rectangle (compl*va.X + ratio*vb.X, compl*va.Y + ratio*vb.Y, compl*va.Width + ratio*vb.Width, compl*va.Height + ratio*vb.Height);
				}
				
				if (a is Drawing.Color)
				{
					Drawing.Color va = (Drawing.Color) a;
					Drawing.Color vb = (Drawing.Color) b;
					
					return new Drawing.Color (compl*va.A + ratio*vb.A, compl*va.R + ratio*vb.R, compl*va.G + ratio*vb.G, compl*va.B + ratio*vb.B);
				}
				
				throw new System.Exception ("Illegal type: " + a.GetType ().Name);
			}
			
			
			protected object					v1;
			protected object					v2;
		}
		
		
		
		protected Timer							timer;
		protected Value[]						values;
		protected double						ratio;
		protected double						time_span;
		protected long							tick_begin;
		protected long							tick_end;
		protected System.Delegate				callback_changed;
		protected System.Delegate				callback_finished;
	}
}
