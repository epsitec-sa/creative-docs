//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// Summary description for LineEngine.
	/// </summary>
	public class LineEngine : Layout.BaseEngine
	{
		public LineEngine()
		{
		}
		
		
		public override Layout.Status Fit(Layout.Context context, out Layout.BreakCollection result)
		{
			result = new Layout.BreakCollection ();
			
			int    offset  = 0;
			double advance = context.X;
			
			bool hyphenate = false;
			
			int    last_break_offset  = 0;
			double last_break_advance = 0.0;
			
			for (;;)
			{
				ulong[] text;
				int text_start;
				int text_length;
				
				Unicode.BreakInfo word_break_info;
				
				if (context.GetNextWord (offset, out text, out text_start, out text_length, out word_break_info))
				{
					while (text_length > 0)
					{
						if (advance > context.RightMargin-context.BreakFenceWidth)
						{
							//	La position actuelle est dans la zone sensible
							//	pour la découpe; il faut analyser aussi les
							//	césures et les mémoriser.
							
							hyphenate = true;
						}
						
						//	Détermine la prochaine tranche de caractères contigus
						//	utilisant le même style de fonte :
						
						int     run_length = this.GetRunLength (text, text_start, text_length);
						ulong[] tmp_buffer = hyphenate ? new ulong[run_length] : null;
						
						OpenType.Font font;
						double        font_size;
						
						context.TextContext.GetFont (text[text_start], out font, out font_size);
						
						//	Si une analyse des découpes est requise, procède par
						//	petites étapes progressives, sinon traite toute la
						//	tranche d'un coup :
						
						double width = 0;
						
						for (int frag_length = 0; frag_length < run_length; )
						{
							frag_length = hyphenate ? this.GetNextFragmentLength (text, text_start, run_length, frag_length) : run_length;
							
							ushort[] glyphs;
							bool can_break = (run_length == text_length);
							
							if (frag_length < run_length)
							{
								System.Array.Copy (text, text_start, tmp_buffer, 0, frag_length);
								
								tmp_buffer[frag_length] = '-';
								
								glyphs    = font.GenerateGlyphs (tmp_buffer, 0, frag_length+1);
								can_break = true;
							}
							else
							{
								glyphs = font.GenerateGlyphs (text, text_start, frag_length);
							}
							
							width = font.GetTotalWidth (glyphs, font_size);
							
							if (advance+width > context.RightMargin)
							{
								//	Dépassé la marge de droite. Arrête sans tenir
								//	compte de ce résultat.
								
								goto stop;
							}
							
							if (can_break)
							{
								if (hyphenate)
								{
									result.Add (new Layout.Break ());
								}
								else
								{
									last_break_offset  = offset + frag_length;
									last_break_advance = advance + width;
								}
							}
						}
						
						advance     += width;
						offset      += run_length;
						text_start  += run_length;
						text_length -= run_length;
					}
					
					if ((word_break_info == Unicode.BreakInfo.Yes) ||
						(word_break_info == Unicode.BreakInfo.HorizontalTab))
					{
					}
				}
				else
				{
					//	Il n'y a plus aucun résultat disponible, sans pour autant
					//	que l'on ait atteint la fin du paragraphe; abandonne et
					//	demande plus de texte !
					
					return Layout.Status.ErrorNeedMoreText;
				}
			}
			
stop:
			
			return Layout.Status.ErrorCannotFit;
		}

	}
}
