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
			this.buffer = null;
			
			this.text_context = text_context;
			
			this.text       = text;
			this.text_start = start;
			
			this.oy_base    = oy_base;
			
			this.mx_left  = mx_left;
			this.mx_right = mx_right;
			
			this.break_fence_before = break_fence_before;
			this.break_fence_after  = break_fence_after;
			
			this.Reset ();
			
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
		
		public Properties.LayoutProperty		LayoutProperty
		{
			get
			{
				return this.layout_property;
			}
			set
			{
				this.layout_property = value;
			}
		}
		
		
		public Text.Context						TextContext
		{
			get
			{
				return this.text_context;
			}
		}
		
		public int								TextOffset
		{
			get
			{
				return this.text_offset;
			}
			set
			{
				this.text_offset = value;
			}
		}
		
		public StretchProfile					TextStretchProfile
		{
			get
			{
				return this.text_profile;
			}
		}
		
		public StretchProfile.Scales			TextStretchScales
		{
			get
			{
				return this.text_scales;
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
		
		public double							MaxY
		{
			get
			{
				return this.oy_ascender;
			}
		}
		
		public double							MinY
		{
			get
			{
				return this.oy_descender;
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
		
		
		public Layout.Status Fit(ref Layout.BreakCollection result)
		{
			//	Détermine les points de découpe pour le texte, selon le contexte
			//	courant.
			
			if (this.text_start + this.text_offset >= this.text.Length)
			{
				return Layout.Status.ErrorNeedMoreText;
			}
			
			this.SelectLayoutEngine (this.text_offset);
			this.Reset ();
			
			Debug.Assert.IsNotNull (this.layout_engine);
			Debug.Assert.IsNotNull (this.text);
			Debug.Assert.IsNotNull (this.text_context);

			Snapshot snapshot = new Snapshot (this);
			
			if (result == null)
			{
				result = new Layout.BreakCollection ();
			}
			else
			{
				result.Clear ();
			}
			
			this.text_profile = new StretchProfile ();
			
			for (int pass = 0; pass < 2; )
			{
				if (pass > 0)
				{
					snapshot.Restore (this);
				}
				
				Layout.Status status = this.layout_engine.Fit (this, ref result);
				
				switch (status)
				{
					case Layout.Status.Ok:
					case Layout.Status.OkFitEnded:
						return status;
					
					case Layout.Status.ErrorNeedMoreText:
						snapshot.Restore (this);
						return status;
						
					case Layout.Status.SwitchLayout:
						continue;
					
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
			
			snapshot.Restore (this);
			
			return Layout.Status.ErrorCannotFit;
		}
		
		public void RenderLine(ITextRenderer renderer, Layout.StretchProfile profile, int length)
		{
			//	Réalise le rendu de la ligne, en appelant les divers moteurs de
			//	layout associés au texte.
			
			Debug.Assert.IsNotNull (this.text);
			
			Debug.Assert.IsTrue (length > 0);
			Debug.Assert.IsTrue (this.text_start + this.text_offset + length <= this.text.Length);
			
			this.SelectLayoutEngine (this.text_offset);
			this.Reset ();
			
			Debug.Assert.IsNotNull (this.layout_engine);
			Debug.Assert.IsNotNull (this.text_context);
			
			this.text_profile = profile;
			this.hyphenate    = false;
			
			profile.ComputeScales (this.RightMargin - this.LeftMargin, out this.text_scales);
			
			int               end            = this.text_offset + length;
			Unicode.BreakInfo end_break_info = Unicode.Bits.GetBreakInfo (this.text[this.text_start + end - 1]);
			
			switch (end_break_info)
			{
				case Unicode.BreakInfo.HyphenateGoodChoice:
				case Unicode.BreakInfo.HyphenatePoorChoice:
					this.hyphenate = true;
					break;
			}
			
			for (;;)
			{
				Layout.Status status = this.layout_engine.Render (this, renderer, end);
				
				switch (status)
				{
					case Layout.Status.Ok:
						return;
					
					case Layout.Status.SwitchLayout:
						continue;
					
					default:
						throw new System.InvalidOperationException ();
				}
			}
		}
		
		
		public void Reset()
		{
			this.hyphenate = false;
			
			this.ox = this.mx_left;
			
			this.oy_ascender  = oy_base;
			this.oy_descender = oy_base;
		}
		
		public void RecordAscender(double value)
		{
			double y = this.oy_base + value;
			
			if (y > this.oy_ascender)
			{
				this.oy_ascender = y;
			}
		}
		
		public void RecordDescender(double value)
		{
			double y = this.oy_base + value;
			
			if (y < this.oy_descender)
			{
				this.oy_descender = y;
			}
		}
		
		
		public bool GetNextWord(int offset, out ulong[] text, out int length, out Unicode.BreakInfo word_break_info)
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
			
			length = pos - (this.text_start + offset);
			text   = this.GetInternalBuffer (length);
			
			if (length == 0)
			{
				return false;
			}
			
			//	Copie le texte dans le buffer temporaire. C'est plus rapide de faire
			//	la copie à la main que d'appeler System.Array.Copy :
			
			pos = this.text_start + offset;
			
			for (int i = 0; i < length; i++)
			{
				text[i] = this.text[pos+i];
			}
			
			return true;
		}
		
		
		private void SelectLayoutEngine(int offset)
		{
			ulong code = this.text[this.text_start + offset];
			this.text_context.GetLayoutEngine (code, out this.layout_engine, out this.layout_property);
		}
		
		private ulong[] GetInternalBuffer(int length)
		{
			if (this.buffer == null)
			{
				this.buffer = new ulong[System.Math.Max (length, 32)];
			}
			else if (this.buffer.Length < length)
			{
				this.buffer = new ulong[length];
			}
				
			return this.buffer;
		}
		
		
		private class Snapshot
		{
			public Snapshot(Context context)
			{
				this.snapshot      = context.snapshot;
				this.text_offset   = context.text_offset;
				this.layout_engine = context.layout_engine;
				this.ox            = context.ox;
				this.oy_base       = context.oy_base;
			}
			
			
			public void Restore(Context context)
			{
				context.snapshot      = this.snapshot;
				context.text_offset   = this.text_offset;
				context.layout_engine = this.layout_engine;
				context.ox            = this.ox;
				context.oy_base       = this.oy_base;
			}
			
			
			private Snapshot					snapshot;
			private int							text_offset;
			private Layout.BaseEngine			layout_engine;
			private double						ox, oy_base;
		}
		
		
		
		private Text.Context					text_context;
		private ulong[]							text;
		private int								text_start;
		private int								text_offset;
		private StretchProfile					text_profile;
		private StretchProfile.Scales			text_scales;
		
		private int								left_to_right;
		
		private double							oy_base;
		private double							oy_ascender;
		private double							oy_descender;
		private double							ox;
		private double							mx_left;
		private double							mx_right;
		private double							break_fence_before;
		private double							break_fence_after;
		
		private bool							hyphenate;
		
		private Layout.BaseEngine				layout_engine;
		private Properties.LayoutProperty		layout_property;
		
		private Snapshot						snapshot;
		
		private ulong[]							buffer;
	}
}
