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
			this.text_context = text_context;
			
			this.text  = text;
			this.start = start;
			
			this.oy_base  = oy_base;
			this.mx_left  = mx_left;
			this.mx_right = mx_right;
			
			this.break_fence_before = break_fence_before;
			this.break_fence_after  = break_fence_after;
			
			this.ox = this.mx_left;
			
			this.left_to_right = 0;
		}
		
		
		public Text.Context						TextContext
		{
			get
			{
				return this.text_context;
			}
		}
		
		public ulong							this[int offset]
		{
			get
			{
				if (this.start + offset < this.text.Length)
				{
					return this.text[this.start+offset];
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
		
		
		
		public bool UpdateOffset(ref int offset, int distance)
		{
			//	TODO: gérer le sens <-- pour l'avance du texte
			
			offset += distance;
			
			if ((offset < 0) ||
				(this.start + offset >= this.text.Length))
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
			int pos   = this.start + offset;
			
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
			start  = this.start + offset;
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
		
		
		
		private Text.Context					text_context;
		private ulong[]							text;
		private int								start;
		private int								left_to_right;
		
		private double							oy_base;
		private double							ox;
		private double							mx_left;
		private double							mx_right;
		private double							break_fence_before;
		private double							break_fence_after;
	}
}
