//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		
		
		
		public override Layout.Status Fit(Layout.Context context, ref Layout.BreakCollection result)
		{
			FitScratch scratch = new FitScratch ();
			
			scratch.Hyphenate = context.Hyphenate;
			scratch.FenceMinX = context.RightMargin-context.BreakFenceBefore;
			scratch.FenceMaxX = context.RightMargin+context.BreakFenceAfter;
			
			scratch.Advance       = context.X;
			scratch.WordBreakInfo = Unicode.BreakInfo.No;
			
			for (;;)
			{
				if (context.GetNextWord (scratch.Offset, out scratch.Text, out scratch.TextStart, out scratch.TextLength, out scratch.WordBreakInfo))
				{
					while (scratch.TextLength > 0)
					{
						if (scratch.Advance > scratch.FenceMinX)
						{
							//	Nous sommes dans la zone sensible n�cessitant la c�sure :
							
							scratch.Hyphenate = true;
						}
						
						//	D�termine la prochaine tranche de caract�res contigus utilisant
						//	le m�me style de fonte :
						
						scratch.RunLength = this.GetRunLength (scratch.Text, scratch.TextStart, scratch.TextLength);
						
						ulong code = scratch.Text[scratch.TextStart];
						Layout.BaseEngine layout;
						
						context.TextContext.GetFont (code, out scratch.Font, out scratch.FontSize);
						context.TextContext.GetLayout (code, out layout);
						
						if ((layout != null) &&
							(layout != this))
						{
							context.SaveSnapshot (scratch.Advance);
							
							context.TextStart   += scratch.Offset;
							context.LayoutEngine = layout;
							
							return Layout.Status.SwitchLayout;
						}
						
						if (this.FitAnalyseRun (ref scratch, ref result, scratch.Hyphenate))
						{
							goto stop; // --------------------------------------------------------------------------.
						}              //																			|
						//																							|
						//	Avance au morceau suivant :																:
						
						scratch.Advance    += scratch.TextWidth;
						scratch.Offset     += scratch.RunLength;
						scratch.TextStart  += scratch.RunLength;
						scratch.TextLength -= scratch.RunLength;
					}
					
					if ((scratch.WordBreakInfo == Unicode.BreakInfo.Yes) ||
						(scratch.WordBreakInfo == Unicode.BreakInfo.HorizontalTab))
					{
						//	Le mot se termine par un saut forc� (ou une marque de tabulation, ce qui				:
						//	revient au m�me par rapport au traitement fait par le syst�me de layout) :				:
						
						break;
					}
				}
				else
				{
					//	Il n'y a plus aucun r�sultat disponible, sans pour autant									:
					//	que l'on ait atteint la fin du paragraphe; abandonne et										:
					//	demande plus de texte !																		:
					
					result = null;
					return Layout.Status.ErrorNeedMoreText;
				}
			}
			
			//	Le texte se termine par une fin forc�e avant d'arriver dans la marge droite.						:
			
			result.Add (new Layout.Break (scratch.Offset, scratch.Advance));
			
			return Layout.Status.Ok;
			
			//																										|
stop:		//	Le texte ne tient pas enti�rement dans l'espace disponible. <---------------------------------------'
			//	Retourne les points de d�coupe (s'il y en a) :
			
			if (result.Count == 0)
			{
				if (scratch.LastBreakOffset > 0)
				{
					result.Add (new Layout.Break (scratch.LastBreakOffset, scratch.LastBreakAdvance));
				}
				else
				{
					return Layout.Status.ErrorCannotFit;
				}
			}
			
			return Layout.Status.Ok;
		}
		
		
		private bool FitAnalyseRun(ref FitScratch scratch, ref Layout.BreakCollection result, bool hyphenate)
		{
			//	Analyse le texte constitut� de caract�res de style identique (m�me
			//	fonte, m�me layout). Si une c�sure est requise, proc�de par petites
			//	�tapes progressives, sinon traite toute la tranche d'un coup.
			
			//	Retourne true s'il faut stopper imm�diatement l'analyse (le texte
			//	est trop long pour l'espace de justification disponible).
			
			for (int frag_length = 0; frag_length < scratch.RunLength; )
			{
				if (hyphenate)
				{
					frag_length = this.GetNextFragmentLength (scratch.Text, scratch.TextStart, scratch.RunLength, frag_length);
				}
				else
				{
					frag_length = scratch.RunLength;
				}
				
				ushort[] glyphs;
				bool     can_break;
				
				if (frag_length < scratch.RunLength)
				{
					ulong[] buffer = scratch.GetBuffer (scratch.RunLength);
					
					System.Array.Copy (scratch.Text, scratch.TextStart, buffer, 0, frag_length);
					
					//	Produit la c�sure manuellement (il faudrait faire mieux pour g�rer
					//	correctement des langues comme le norv�gien ou l'ancien allemand
					//	qui peuvent provoquer le d�doublement de certains caract�res) :
					
					buffer[frag_length] = '-';
					
					glyphs    = scratch.Font.GenerateGlyphs (buffer, 0, frag_length+1);
					can_break = true;
				}
				else
				{
					glyphs    = scratch.Font.GenerateGlyphs (scratch.Text, scratch.TextStart, frag_length);
					can_break = (scratch.RunLength == scratch.TextLength);
					hyphenate = hyphenate && (scratch.WordBreakInfo == Unicode.BreakInfo.Optional);
				}
				
				scratch.TextWidth = scratch.Font.GetTotalWidth (glyphs, scratch.FontSize);
				
				if (scratch.Advance+scratch.TextWidth > scratch.FenceMaxX)
				{
					//	D�pass� la marge de droite. Arr�te imm�diatement sans tenir compte
					//	du r�sultat :
					
					return true;
				}
				
				if (can_break)
				{
					scratch.LastBreakOffset  = scratch.Offset + frag_length;
					scratch.LastBreakAdvance = scratch.Advance + scratch.TextWidth;
					
					if (hyphenate)
					{
						result.Add (new Layout.Break (scratch.LastBreakOffset, scratch.LastBreakAdvance));
					}
				}
			}
			
			return false;
		}
		
		
		private struct FitScratch
		{
			public ulong[] GetBuffer(int length)
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
			
			
			public int							Offset;
			public double						Advance;
			public int							LastBreakOffset;
			public double						LastBreakAdvance;
			public bool							Hyphenate;
			
			public ulong[]						Text;
			public int							TextStart;
			public int							TextLength;
			public double						TextWidth;
			
			public double						FenceMinX;
			public double						FenceMaxX;
			
			public Unicode.BreakInfo			WordBreakInfo;
			
			public int							RunLength;
			
			public OpenType.Font				Font;
			public double						FontSize;
			
			public ulong[]						buffer;
		}
	}
}
