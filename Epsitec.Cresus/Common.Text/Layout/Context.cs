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
		public Context(Text.Context text_context, ulong[] text, int start, double oy_base, double line_height, double line_width, double mx_left, double mx_right, double break_fence_before, double break_fence_after)
		{
			this.buffer = null;
			
			this.text_context = text_context;
			
			this.text       = text;
			this.text_start = start;
			
			this.oy_base      = oy_base;
			this.oy_ascender  = oy_base;
			this.oy_descender = oy_base;
			
			this.line_height = line_height;
			this.line_width  = line_width;
			
			this.ox       = mx_left;
			this.mx_left  = mx_left;
			this.mx_right = mx_right;
			
			this.break_fence_before = break_fence_before;
			this.break_fence_after  = break_fence_after;
			
			this.left_to_right = 0;
		}
		
		public Context(Text.Context context, ulong[] text, int start, FrameList frame_list)
		{
			this.text_context = context;
			this.text         = text;
			this.text_start   = start;
			this.frame_list   = frame_list;
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
		
		
		public double							Justification
		{
			get
			{
				return this.justification;
			}
		}
		
		public double							Disposition
		{
			get
			{
				return this.disposition;
			}
		}
		
		public double							BreakFenceBefore
		{
			get
			{
				return this.break_fence_before;
			}
		}
		
		public double							BreakFenceAfter
		{
			get
			{
				return this.break_fence_after;
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
		
		public double							LineHeight
		{
			get
			{
				return this.line_height;
			}
		}
		
		public double							LineWidth
		{
			get
			{
				return this.line_width;
			}
		}
		
		public double							LineAscender
		{
			get
			{
				return this.oy_ascender - this.oy_base;
			}
		}
		
		public double							LineDescender
		{
			get
			{
				return this.oy_descender - this.oy_base;
			}
		}
		
		public double							AvailableWidth
		{
			get
			{
				return this.line_width - this.mx_left - this.mx_right;
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
		
		public bool								EnableHyphenation
		{
			get
			{
				return this.enable_hyphenation;
			}
		}
		
		public bool								BreakAnywhere
		{
			get
			{
				return this.break_anywhere;
			}
		}
		
		
		public ITextFrame						Frame
		{
			get
			{
				return this.frame;
			}
		}
		
		public int								FrameIndex
		{
			get
			{
				return this.frame_index;
			}
		}
		
		public double							FrameY
		{
			get
			{
				return this.frame_y;
			}
		}
		
		
		public void SelectFrame(int frame_index, double y)
		{
			if (frame_index == -1)
			{
				this.frame_index = -1;
				this.frame       = null;
				this.frame_y     = 0;
			}
			else
			{
				this.frame_index = frame_index;
				this.frame       = this.frame_list[this.frame_index];
				this.frame_y     = y;
			}
		}
		
		
		public Layout.Status Fit(ref Layout.BreakCollection result, int paragraph_line_count)
		{
			//	Détermine les points de découpe pour le texte, selon le contexte
			//	courant.
			
			if (this.text_start + this.text_offset >= this.text.Length)
			{
				return Layout.Status.ErrorNeedMoreText;
			}
			
			this.SelectLayoutEngine (this.text_offset);
			this.SelectMarginsAndJustification (this.text_offset, paragraph_line_count, false);
			this.SelectLineHeight (this.text_offset);
			
			this.ox             = this.mx_left;
			this.break_anywhere = false;
			
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
			
			double initial_line_height = 0;
			double initial_line_width  = 0;
restart:
			int pass = 0;
			
			for (;;)
			{
				if ((pass > 1) &&
					(result.Count > 0))
				{
					return Layout.Status.Ok;
				}
				
				if (this.frame != null)
				{
					double line_ascender  = this.oy_ascender - this.oy_base;
					double line_descender = this.oy_descender - this.oy_base;
					double line_height    = this.line_height;
					
					double ox, oy, dx;
					double next_frame_y;
					
					while ((this.frame.ConstrainLineBox (this.frame_y, line_ascender, line_descender, line_height, out ox, out oy, out dx, out next_frame_y) == false)
						|| (dx < this.mx_left + this.mx_right)
						|| (pass > 1))
					{
						//	Il n'y a plus de place dans le ITextFrame courant, passe au
						//	suivant, s'il en reste encore un (ou plus)...
						
						int frame_index = this.frame_index + 1;
						
						if (frame_index < this.frame_list.Count)
						{
							//	Reprend avec un autre cadre. On reprend tout à zéro depuis
							//	ici :
							
							this.SelectLayoutEngine (this.text_offset);
							this.SelectMarginsAndJustification (this.text_offset, paragraph_line_count, false);
							this.SelectFrame (frame_index, 0);
							this.SelectLineHeight (this.text_offset);
							
							goto restart;
						}
						
						//	Il n'y a plus de ITextFrame ! On s'arrête donc immédiatement
						//	avec une erreur.
						
						return Layout.Status.ErrorNeedMoreRoom;
					}
					
					this.ox           = ox + this.mx_left;
					this.oy_base      = oy;
					this.oy_ascender  = oy + line_ascender;
					this.oy_descender = oy + line_descender;
					this.frame_y      = next_frame_y;
					this.line_width   = dx;
					this.line_height  = line_height;
					
					if ((initial_line_height == 0) &&
						(initial_line_width == 0))
					{
						initial_line_height = this.line_height;
						initial_line_width  = this.line_width;
					}
				}
				
				if (pass > 1)
				{
					break;
				}
				
				Layout.Status status = this.layout_engine.Fit (this, ref result);
				
				switch (status)
				{
					case Layout.Status.Ok:
					case Layout.Status.OkFitEnded:
						if ((this.frame_list != null) &&
							(this.line_height > initial_line_height))
						{
							//	Oups. On vient de réaliser un fit idéal, mais qui ne tient
							//	pas dans l'espace alloué verticalement. Il faut forcer une
							//	seconde passe :
							
							initial_line_height = this.line_height;
							result.Clear ();
							this.break_anywhere = false;
							pass = 0;
							break;
						}
						
						return status;
					
					case Layout.Status.ErrorNeedMoreText:
						snapshot.Restore (this);
						return status;
						
					case Layout.Status.SwitchLayout:
						continue;
					
					case Layout.Status.ErrorCannotFit:
						this.break_anywhere = true;
						pass++;
						break;
					
					default:
						throw new System.InvalidOperationException ();
				}
				
				snapshot.Restore (this);
			}
			
			Debug.Assert.IsTrue (result.Count == 0);
			
			snapshot.Restore (this);
			
			return Layout.Status.ErrorCannotFit;
		}
		
		public void RenderLine(ITextRenderer renderer, Layout.StretchProfile profile, int length, double line_base_x, double line_base_y, double line_width, int paragraph_line_count, bool is_last_line)
		{
			//	Réalise le rendu de la ligne, en appelant les divers moteurs de
			//	layout associés au texte.
			
			Debug.Assert.IsNotNull (this.text);
			
			Debug.Assert.IsTrue (length > 0);
			Debug.Assert.IsTrue (this.text_start + this.text_offset + length <= this.text.Length);
			
			this.SelectLayoutEngine (this.text_offset);
			this.SelectMarginsAndJustification (this.text_offset, paragraph_line_count, is_last_line);
			this.SelectLineHeight (this.text_offset);
			
			this.ox      = line_base_x;
			this.oy_base = line_base_y;
			
			this.line_width = line_width + this.mx_left + this.mx_right;
			
			Debug.Assert.IsNotNull (this.layout_engine);
			Debug.Assert.IsNotNull (this.text_context);
			
			this.text_profile   = profile;
			this.break_anywhere = false;
			
			double space;
			
			space  = line_width - profile.TotalWidth;
			space *= 1.0 - this.justification;
			
			this.ox += space * this.disposition;
			
			profile.ComputeScales (line_width - space, out this.text_scales);
			
			int               end            = this.text_offset + length;
			Unicode.BreakInfo end_break_info = Unicode.Bits.GetBreakInfo (this.text[this.text_start + end - 1]);
			
			switch (end_break_info)
			{
				case Unicode.BreakInfo.HyphenateGoodChoice:
				case Unicode.BreakInfo.HyphenatePoorChoice:
					this.enable_hyphenation = true;
					break;
				
				default:
					this.enable_hyphenation = false;
					break;
			}
			
			for (;;)
			{
				Layout.Status status = this.layout_engine.Render (this, renderer, end - this.text_offset);
				
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
		
		
		public void RecordAscender(double value)
		{
			double y = this.oy_base + value;
			
			if (y > this.oy_ascender)
			{
				this.oy_ascender = y;
			}
		}
		
		public void RecordLineHeight(double value)
		{
			if (value > this.line_height)
			{
				this.line_height = value;
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
		
		public bool GetTextCopy(int offset, out ulong[] text, ref int length)
		{
			//	TODO: gérer le sens <-- pour l'avance du texte
			
			int end = System.Math.Min (this.text_start + offset + length, this.text.Length);
			int pos = this.text_start + offset;

			length = end - pos;
			text = this.GetInternalBuffer(length);
			
			if (length == 0)
			{
				return false;
			}
			
			//	Copie le texte dans le buffer temporaire. C'est plus rapide de faire
			//	la copie à la main que d'appeler System.Array.Copy :
			
			for (int i = 0; i < length; i++)
			{
				text[i] = this.text[pos + i];
			}
			
			return true;
		}
		
		
		private void SelectLayoutEngine(int offset)
		{
			ulong code = this.text[this.text_start + offset];
			this.text_context.GetLayoutEngine (code, out this.layout_engine, out this.layout_property);
		}
		
		private void SelectMarginsAndJustification(int offset, int paragraph_line_index, bool is_last_line)
		{
			ulong code = this.text[this.text_start + offset];
			
			Properties.MarginsProperty margins;
			
			this.text_context.GetMargins (code, out margins);
			
			if (margins != null)
			{
				this.mx_left  = paragraph_line_index == 0 ? margins.LeftMarginFirstLine  : margins.LeftMarginBody;
				this.mx_right = paragraph_line_index == 0 ? margins.RightMarginFirstLine : margins.RightMarginBody;
				
				this.justification      = is_last_line ? margins.JustificationLastLine : margins.JustificationBody;
				this.disposition        = margins.Disposition;
				this.break_fence_before = margins.BreakFenceBefore;
				this.break_fence_after  = margins.BreakFenceAfter;
				this.enable_hyphenation = margins.EnableHyphenation;
			}
		}
		
		private void SelectLineHeight(int offset)
		{
			ulong code = this.text[this.text_start + offset];
			
			OpenType.Font font;
			double        font_size;
			
			this.text_context.GetFont (code, out font, out font_size);
			
			if (font != null)
			{
				this.oy_ascender  = this.oy_base + font.GetAscender (font_size);
				this.oy_descender = this.oy_base + font.GetDescender (font_size);
				this.line_height  = font_size * 1.2;
			}
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
				
				this.frame_index   = context.frame_index;
				this.frame_y       = context.frame_y;
			}
			
			
			public void Restore(Context context)
			{
				double ascender  = context.oy_ascender  - context.oy_base;
				double descender = context.oy_descender - context.oy_base;
				
				context.snapshot      = this.snapshot;
				context.text_offset   = this.text_offset;
				context.layout_engine = this.layout_engine;
				context.ox            = this.ox;
				
				context.oy_base       = this.oy_base;
				context.oy_ascender   = this.oy_base + ascender;
				context.oy_descender  = this.oy_base + descender;
				
				context.SelectFrame (this.frame_index, this.frame_y);
			}
			
			
			private Snapshot					snapshot;
			private int							text_offset;
			private Layout.BaseEngine			layout_engine;
			private double						ox, oy_base;
			private int							frame_index;
			private double						frame_y;
		}
		
		
		
		private Text.Context					text_context;
		private ulong[]							text;
		private int								text_start;
		private int								text_offset;
		private StretchProfile					text_profile;
		private StretchProfile.Scales			text_scales;
		
		private FrameList						frame_list;
		private int								frame_index = -1;
		private ITextFrame						frame;
		private double							frame_y;
		
		private int								left_to_right;
		
		private double							ox;
		private double							oy_base;
		private double							oy_ascender;
		private double							oy_descender;
		private double							line_height;
		private double							line_width;
		private double							mx_left;
		private double							mx_right;
		private double							justification;
		private double							disposition;
		
		private double							break_fence_before;
		private double							break_fence_after;
		
		private bool							enable_hyphenation;
		private bool							break_anywhere;
		
		private Layout.BaseEngine				layout_engine;
		private Properties.LayoutProperty		layout_property;
		
		private Snapshot						snapshot;
		
		private ulong[]							buffer;
	}
}
