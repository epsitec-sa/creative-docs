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
		
		
		
		
		public override Layout.Status Fit(Layout.Context context, ref Layout.BreakCollection result)
		{
			FitScratch scratch = new FitScratch ();
			
			scratch.Offset    = context.TextOffset;
			scratch.Hyphenate = context.Hyphenate;
			scratch.FenceMinX = context.RightMargin-context.BreakFenceBefore;
			scratch.FenceMaxX = context.RightMargin+context.BreakFenceAfter;
			
			scratch.Advance       = context.X;
			scratch.WordBreakInfo = Unicode.BreakInfo.No;
			
			for (;;)
			{
				if (context.GetNextWord (scratch.Offset, out scratch.Text, out scratch.TextLength, out scratch.WordBreakInfo))
				{
					scratch.TextStart = 0;
					
					while (scratch.TextLength > 0)
					{
						if (scratch.Advance > scratch.FenceMinX)
						{
							//	Nous sommes dans la zone sensible nécessitant la césure :
							
							scratch.Hyphenate = true;
						}
						
						//	Détermine la prochaine tranche de caractères contigus utilisant
						//	le même style de fonte :
						
						scratch.RunLength = this.GetRunLength (scratch.Text, scratch.TextStart, scratch.TextLength);
						
						ulong code = scratch.Text[scratch.TextStart];
						Layout.BaseEngine layout;
						
						context.TextContext.GetFont (code, out scratch.Font, out scratch.FontSize);
						context.TextContext.GetLayout (code, out layout);
						
						if ((layout != null) &&
							(layout != this))
						{
							//	Change de moteur de layout. Il faut par conséquent mémoriser où on
							//	s'arrête pour que le suivant sache où reprendre :
							
							context.X           += scratch.Advance;
							context.TextOffset  += scratch.Offset;
							context.LayoutEngine = layout;
							
							return Layout.Status.SwitchLayout;
						}
						
						context.RecordAscender (scratch.Font.GetAscender (scratch.FontSize));
						context.RecordDescender (scratch.Font.GetDescender (scratch.FontSize));
						
						if (this.FitAnalyseRun (ref scratch, ref result))
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
						//	Le mot se termine par un saut forcé (ou une marque de tabulation, ce qui				:
						//	revient au même par rapport au traitement fait par le système de layout) :				:
						
						break;
					}
				}
				else
				{
					//	Il n'y a plus aucun résultat disponible, sans pour autant									:
					//	que l'on ait atteint la fin du paragraphe; abandonne et										:
					//	demande plus de texte !																		:
					
					result = null;
					return Layout.Status.ErrorNeedMoreText;
				}
			}
			
			//	Le texte se termine par une fin forcée avant d'arriver dans la marge droite.						:
			
			result.Add (new Layout.Break (scratch.Offset, scratch.Advance));
			
			return Layout.Status.OkFitEnded;
			
			//																										|
stop:		//	Le texte ne tient pas entièrement dans l'espace disponible. <---------------------------------------'
			//	Retourne les points de découpe (s'il y en a) :
			
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
		
		
		private bool FitAnalyseRun(ref FitScratch scratch, ref Layout.BreakCollection result)
		{
			//	Analyse le texte constituté de caractères de style identique (même
			//	fonte, même layout). Si une césure est requise, procède par petites
			//	étapes progressives, sinon traite toute la tranche d'un coup.
			
			//	Retourne true s'il faut stopper immédiatement l'analyse (le texte
			//	est trop long pour l'espace de justification disponible).
			
			bool hyphenate = scratch.Hyphenate;
			
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
					//	Produit la césure manuellement (il faudrait faire mieux pour gérer
					//	correctement des langues comme le norvégien ou l'ancien allemand
					//	qui peuvent provoquer le dédoublement de certains caractères) :
					
					ulong save = scratch.Text[frag_length];
					scratch.Text[frag_length] = '-';
					
					glyphs    = scratch.Font.GenerateGlyphs (scratch.Text, scratch.TextStart, frag_length+1);
					can_break = true;
					
					scratch.Text[frag_length] = save;
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
					//	Dépassé la marge de droite. Arrête immédiatement sans tenir compte
					//	du résultat :
					
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
		}
	}
}
