//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Text
{
	internal class TextBreakEngine : Drawing.ITextBreakEngine, ILanguageRecognizer
	{
		public TextBreakEngine()
		{
		}

		#region ITextBreakEngine Members

		void Epsitec.Common.Drawing.ITextBreakEngine.SetText(string text, Epsitec.Common.Drawing.TextBreakMode mode)
		{
			this.SetText (text, mode);
		}

		void Epsitec.Common.Drawing.ITextBreakEngine.SetRuns(ICollection<Epsitec.Common.Drawing.TextBreakRun> runs)
		{
			foreach (Drawing.TextBreakRun run in runs)
			{
				this.AddRun (run);
			}
		}

		void Epsitec.Common.Drawing.ITextBreakEngine.Rewind()
		{
			this.Rewind ();
		}

		bool Epsitec.Common.Drawing.ITextBreakEngine.GetNextBreak(double maxWidth, out string text, out double textWidth, out int textLength)
		{
			textWidth = maxWidth;
			text = this.FindNextBreak (ref textWidth, out textLength);
			
			if (text == null)
			{
				text = "";
				return false;
			}
			else
			{
				return true;
			}
		}

		#endregion

		private void SetText(string text, Drawing.TextBreakMode mode)
		{
			this.text = string.IsNullOrEmpty (text) ? "" : text;
			this.mode = mode;
			this.runs = new List<Drawing.TextBreakRun> ();
			this.dirty = true;
		}

		private void AddRun(Drawing.TextBreakRun run)
		{
			this.runs.Add (run);
			this.dirty = true;
		}

		private void AnalyzeTextBreaks()
		{
			ulong[] utf32;

			TextConverter.ConvertFromString (this.text, out utf32);

			System.Diagnostics.Debug.Assert (this.text.Length == utf32.Length);
			
			this.breaks = new Unicode.BreakInfo[utf32.Length];
			
			Unicode.DefaultBreakAnalyzer.GenerateBreaks (utf32, 0, utf32.Length, this.breaks);

			if ((this.mode & Drawing.TextBreakMode.Hyphenate) != 0)
			{
				LanguageEngine.GenerateHyphens (this, utf32, 0, utf32.Length, this.breaks);
			}

			if (((this.mode & Drawing.TextBreakMode.SingleLine) != 0) &&
				((this.mode & Drawing.TextBreakMode.Split) != 0))
			{
				for (int i = 0; i < this.breaks.Length; i++)
				{
					if ((this.breaks[i] == Unicode.BreakInfo.Yes) ||
						(this.breaks[i] == Unicode.BreakInfo.Optional))
					{
						this.breaks[i] = Unicode.BreakInfo.No;
					}
				}
			}
		}

		private void Rewind()
		{
			this.pos = 0;
			this.runIndex = 0;
			this.runOffset = 0;

			if (this.dirty)
			{
				this.AnalyzeTextBreaks ();
				this.dirty = false;
			}
		}

		private string FindNextBreak(ref double width, out int nChars)
		{
			bool addEllipsis = false;
			char ellipsis = '\0';

		restart:
			if (this.pos >= this.text.Length)
			{
				width = 0.0;
				nChars = 0;
				return null; 
			}

			int runIndex = this.runIndex;
			Drawing.TextBreakRun run = this.runs[runIndex];
			Drawing.Font font = run.Font;

			double advance = 0;
			int offset = 0;
			int runOffset = this.runOffset;
			double scale = run.Scale;
			bool hyphenate = false;

			int breakOffset = 0;
			Drawing.TextBreakRun breakRun;
			int breakRunIndex = 0;
			int breakRunOffset = 0;
			double breakAdvance = 0;
			double breakWidth = 0;
			char breakChar = (char) 0;

			while (runOffset >= run.Length)
			{
				runIndex++;
				run = this.runs[runIndex];
				runOffset = 0;
				font = run.Font;
				scale = run.Scale;
			}

			for (;;)
			{
				char unicode = this.text[this.pos+offset];
				ushort glyph = font == null ? (ushort)0 : font.GetGlyphIndex (unicode);
				double charWidth = font == null ? scale : font.GetGlyphAdvance (glyph) * scale;
				Unicode.BreakInfo breakMode = this.breaks[this.pos+offset];

				if (glyph == 0)
				{
					switch ((Unicode.Code) unicode)
					{
						case Unicode.Code.PageSeparator:
						case Unicode.Code.LineSeparator:
						case Unicode.Code.ParagraphSeparator:
						case Unicode.Code.ZeroWidthJoiner:
						case Unicode.Code.ZeroWidthNonJoiner:
						case Unicode.Code.ZeroWidthSpace:
							charWidth = 0;
							break;
					}
				}

				if ((breakMode != Unicode.BreakInfo.No) &&
					(breakMode != Unicode.BreakInfo.NoAlpha) &&
					(font != null))
				{
					if ((breakMode == Unicode.BreakInfo.HyphenateGoodChoice) ||
						(breakMode == Unicode.BreakInfo.HyphenatePoorChoice))
					{
						//	Check to see if the current text, including the hyphen, fits
						//	into the given space. If not, give up and use the previous
						//	best fit.
						
						if (width < advance+font.OpenTypeFont.HyphenWidth*scale)
						{
							goto try_fit;
						}

						breakOffset  = offset+1;
						breakAdvance = advance+charWidth;
						breakWidth   = font.OpenTypeFont.HyphenWidth*scale;
						breakChar    = font.OpenTypeFont.HyphenChar;
						hyphenate    = true;
					}
					else
					{
						double tempWidth = charWidth;
						
						//	Spaces at the end of a line are not counted for the line length computation :
						
						switch ((Unicode.Code) unicode)
						{
							case Unicode.Code.Space:
								tempWidth = 0;
								break;
						}
						
						//	Can we fit the glyph on the line and possibly break after it, or do we have
						//	to break immediately, relying on the previous possible break ?
						
						if (width < advance+tempWidth)
						{
							goto try_fit;
						}

						breakOffset  = offset;
						breakAdvance = advance;
						breakWidth   = tempWidth;
						breakChar    = unicode;
						hyphenate    = false;
					}
					
					//	We have found a position where it is possible to break the text. Remember it; it
					//	might be used as the best fit.
					
					breakRun       = run;
					breakRunIndex  = runIndex;
					breakRunOffset = runOffset;
				}

			try_fit:
				if ((breakOffset == 0) &&
					(offset >= 0) &&
					(width < (advance + charWidth)))
				{
					//	Oops... The word is too long to fit in the given space, and there is no position
					//	where a break would be possible before. What can we do?

					if (((this.mode & Drawing.TextBreakMode.Ellipsis) != 0) &&
						(font != null) &&
						(addEllipsis == false))
					{
						//	The caller specified that an ellipsis should be added if a word gets
						//	truncated.

						if ((font.OpenTypeFont.EllipsisWidth > 0) &&
							(width > font.OpenTypeFont.EllipsisWidth*scale))
						{
							//	Pretend that there is an ellipsis (...) at the end of the text, reduce the
							//	available width and start all the process over again.

							width -= font.OpenTypeFont.EllipsisWidth*scale;
							ellipsis = font.OpenTypeFont.EllipsisChar;
							addEllipsis = true;
							goto restart;
						}
					}
					
					if ((addEllipsis) &&
						(ellipsis != font.OpenTypeFont.PeriodChar) &&
						(offset == 0))
					{
						if ((font.OpenTypeFont.PeriodWidth > 0) &&
							(width > font.OpenTypeFont.PeriodWidth*scale))
						{
							width += font.OpenTypeFont.EllipsisWidth*scale;
							width -= font.OpenTypeFont.PeriodWidth*scale;
							ellipsis = font.OpenTypeFont.PeriodChar;
							goto restart;
						}
					}

					if (((this.mode & Drawing.TextBreakMode.Split) != 0) ||
						(addEllipsis))
					{
						//	Truncation required, just stop here.

						breakOffset    = offset;
						breakAdvance   = advance;
						breakRun       = run;
						breakRunIndex  = runIndex;
						breakRunOffset = runOffset;
						breakChar      = ellipsis;

						goto break_text;
					}
				}
				
				//	TODO: handle surrogate pairs...
		
				advance   += charWidth;
				offset    += 1;
				runOffset += 1;
		
				if ((advance > width) ||
					(this.breaks[this.pos+offset-1] == Unicode.BreakInfo.Yes) ||
					(pos+offset >= this.text.Length))
				{
					break;
				}
				
				//	When we reach the end of the current run, we must move to the next run, which
				//	might have a different font face and a different font scale.
				
				if (runOffset >= run.Length)
				{
					runIndex++;
					run = this.runs[runIndex];
					
					runOffset = 0;
					font      = run.Font;
					scale     = run.Scale;
				}
			}
			
			if (advance <= width)
			{
				//	Everything just fits into the specified width. That's nice and easy to handle.

				string text = this.text.Substring (this.pos, offset);
				
				this.pos += offset;
				this.runIndex = runIndex;
				this.runOffset = runOffset;
				
				width   = advance;
				nChars = offset;
				
				return text;
			}
			
			if (breakOffset == 0)
			{
				//	There was no break position before the current point; we must give up here,
				//	and hope that the caller will be able to handle the condition.
				
				width   = 0.0;
				nChars = 0;
				
				return "";
			}
			
			//	Make a copy of the text, either in order to add an ellipsis or a hyphen.

		break_text:

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			buffer.Append (this.text.Substring (this.pos, breakOffset));
			buffer.Append (breakChar);
			
			width = breakAdvance + breakWidth;
			
			if (this.text[this.pos+breakOffset] == ' ')
			{
				//	If the line ends with a space, strip the space and skip it, to be sure we don't
				//	reconsider it on the next iteration.
				
				buffer.Length = buffer.Length-1;
				
				breakOffset++;
				breakRunOffset++;
				
				if (breakRunOffset >= this.runs[breakRunIndex].Length)
				{
					breakRunIndex++;
					breakRunOffset = 0;
				}

				while ((buffer.Length > 0) && (buffer[buffer.Length-1] == ' '))
				{
					buffer.Length = buffer.Length-1;
				}
			}
			else if (!hyphenate && !addEllipsis)
			{
				//	The line breaks with a breaking character. Since it has been placed on the
				//	current line, we must skip it in order not to reconsider it on the next
				//	iteration.
				
				breakOffset++;
			}
			
			this.pos       += breakOffset;
			this.runIndex   = breakRunIndex;
			this.runOffset  = breakRunOffset;
			
			width   = breakAdvance;
			nChars = breakOffset;
			
			return buffer.ToString ();
		}

		#region ILanguageRecognizer Members

		bool ILanguageRecognizer.GetLanguage(ulong[] text, int offset, out double hyphenation, out string locale)
		{
			int runIndex = 0;

			while ((runIndex < this.runs.Count)
				&& (this.runs[runIndex].Length <= offset))
			{
				offset -= this.runs[runIndex++].Length;
			}

			if (runIndex < this.runs.Count)
			{
				hyphenation = 1.0;
				locale = this.runs[runIndex].Locale;
				return locale == null ? false : true;
			}
			else
			{
				hyphenation = 0;
				locale = null;
				return false;
			}
		}

		#endregion

		private bool dirty;
		private string text;
		private Drawing.TextBreakMode mode;
		private List<Drawing.TextBreakRun> runs;
		private Unicode.BreakInfo[] breaks;

		private int runIndex;
		private int runOffset;
		private int pos;
	}
}
