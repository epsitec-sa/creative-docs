//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// La classe Context stocke le contexte de layout lié à un paragraphe
	/// entier.
	/// </summary>
	public class Context
	{
		public Context(Text.Context text_context, ulong[] text, int start, double oy_base, double mx_left, double mx_right, double break_fence_before, double break_fence_after)
		{
			this.text_context  = text_context;
			
			this.text       = text;
			this.text_start = start;
			
			this.oy_base  = oy_base;
			this.mx_left  = mx_left;
			this.mx_right = mx_right;
			
			this.break_fence_before = break_fence_before;
			this.break_fence_after  = break_fence_after;
			
			this.ox = this.mx_left;
			
			this.left_to_right = 0;
		}
		
		
		public Layout.BaseEngine				LayoutEngine
		{
			get
			{
				return this.layout_engine;
			}
			set
			{
				this.layout_engine = value;
			}
		}
		
		
		public ulong[]							Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}
		
		public Text.Context						TextContext
		{
			get
			{
				return this.text_context;
			}
		}
		
		public int								TextStart
		{
			get
			{
				return this.text_start;
			}
			set
			{
				this.text_start = value;
			}
		}
		
		public ulong							this[int offset]
		{
			get
			{
				if (this.text_start + offset < this.text.Length)
				{
					return this.text[this.text_start+offset];
				}
				
				return 0;
			}
		}
		
		
		public double							X
		{
			get
			{
				return this.ox;
			}
			set
			{
				this.ox = value;
			}
		}
		
		public double							Y
		{
			get
			{
				return this.oy_base;
			}
		}
		
		
		public double							BreakFenceBefore
		{
			get
			{
				return this.break_fence_before;
			}
			set
			{
				this.break_fence_before = value;
			}
		}
		
		public double							BreakFenceAfter
		{
			get
			{
				return this.break_fence_after;
			}
			set
			{
				this.break_fence_after = value;
			}
		}
		
		public double							LeftMargin
		{
			get
			{
				return this.mx_left;
			}
		}
		
		public double							RightMargin
		{
			get
			{
				return this.mx_right;
			}
		}
		
		
		public bool								IsLeftToRight
		{
			get
			{
				return (this.left_to_right & 1) == 0;
			}
		}
		
		public bool								IsRightToLeft
		{
			get
			{
				return (this.left_to_right & 1) == 1;
			}
		}
		
		public bool								Hyphenate
		{
			get
			{
				return this.hyphenate;
			}
			set
			{
				this.hyphenate = value;
			}
		}
		
		
		public bool UpdateOffset(ref int offset, int distance)
		{
			//	TODO: gérer le sens <-- pour l'avance du texte
			
			offset += distance;
			
			if ((offset < 0) ||
				(this.text_start + offset >= this.text.Length))
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		public bool GetNextWord(int offset, out ulong[] text, out int start, out int length, out Unicode.BreakInfo word_break_info)
		{
			word_break_info = Unicode.BreakInfo.No;
			
			//	TODO: gérer le sens <-- pour l'avance du texte
			
			int end   = this.text.Length;
			int pos   = this.text_start + offset;
			
			while (pos < end)
			{
				word_break_info = Unicode.Bits.GetBreakInfo (this.text[pos]);
				
				pos++;
				
				if ((word_break_info == Unicode.BreakInfo.Yes) ||
					(word_break_info == Unicode.BreakInfo.Optional) ||
					(word_break_info == Unicode.BreakInfo.HorizontalTab))
				{
					break;
				}
			}
			
			text   = this.text;
			start  = this.text_start + offset;
			length = pos - start;
			
			if (length == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		
		public Layout.Status Fit(Layout.BaseEngine engine, ref Layout.BreakCollection result)
		{
			this.hyphenate = false;
			this.layout_engine = engine;
			
			if (result == null)
			{
				result = new Layout.BreakCollection ();
			}
			
			for (int pass = 0; pass < 2; )
			{
				Layout.Status status = this.layout_engine.Fit (this, ref result);
				
				switch (status)
				{
					case Layout.Status.Ok:
						return status;
					
					case Layout.Status.ErrorNeedMoreText:
						this.RewindAllSnapshots ();
						return status;
						
					case Layout.Status.SwitchLayout:
						break;
					
					case Layout.Status.ErrorCannotFit:
						this.hyphenate = true;
						pass++;
						break;
					
					default:
						throw new System.InvalidOperationException ();
				}
			}
			
			if (result.Count > 0)
			{
				return Layout.Status.Ok;
			}
			
			return Layout.Status.ErrorCannotFit;
		}
		
		
		public void SaveSnapshot(double advance)
		{
			this.snapshot = new Snapshot (this, advance);
		}
		
		public void RewindSnapshot()
		{
			this.snapshot.Restore (this);
		}
		
		public void RewindAllSnapshots()
		{
			while (this.snapshot != null)
			{
				this.snapshot.Restore (this);
			}
		}
		
		
		private class Snapshot
		{
			public Snapshot(Context context, double advance)
			{
				this.snapshot = context.snapshot;
				this.text_start = context.text_start;
			}
			
			
			
			public void Restore(Context context)
			{
				context.snapshot   = this.snapshot;
				context.text_start = this.text_start;
			}
			
			
			private Snapshot					snapshot;
			private int							text_start;
		}
		
		
		private Text.Context					text_context;
		private ulong[]							text;
		private int								text_start;
		private int								left_to_right;
		
		private double							oy_base;
		private double							ox;
		private double							mx_left;
		private double							mx_right;
		private double							break_fence_before;
		private double							break_fence_after;
		
		private bool							hyphenate;
		
		private Layout.BaseEngine				layout_engine;
		private Snapshot						snapshot;
	}
}
