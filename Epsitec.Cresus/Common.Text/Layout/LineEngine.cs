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
			scratch.FenceMinX = context.LineWidth-context.RightMargin-context.BreakFenceBefore;
			scratch.FenceMaxX = context.LineWidth-context.RightMargin+context.BreakFenceAfter;
			
			scratch.Advance        = context.X;
			scratch.WordBreakInfo  = Unicode.BreakInfo.No;
			scratch.StretchProfile = new StretchProfile ();
			
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
						
						Layout.BaseEngine         engine;
						Properties.LayoutProperty layout;
						
						context.TextContext.GetFont (code, out scratch.Font, out scratch.FontSize);
						context.TextContext.GetLayoutEngine (code, out engine, out layout);
						
						if ((engine != this) ||
							(layout != context.LayoutProperty))
						{
							//	Change de moteur de layout. Il faut par conséquent mémoriser où on
							//	s'arrête pour que le suivant sache où reprendre :
							
							context.X              = scratch.Advance;
							context.TextOffset     = scratch.Offset;
							context.LayoutEngine   = engine;
							context.LayoutProperty = layout;
							
							context.TextStretchProfile.Add (scratch.StretchProfile);
							
							return Layout.Status.SwitchLayout;
						}
						
						context.RecordAscender (scratch.Font.GetAscender (scratch.FontSize));
						context.RecordDescender (scratch.Font.GetDescender (scratch.FontSize));
						context.RecordLineHeight (scratch.FontSize * 1.2);
						
						if (this.FitRun (context, ref scratch, ref result))
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
			
			result.Add (new Layout.Break (scratch.Offset, scratch.Advance, 0, scratch.StretchProfile));
			
			return Layout.Status.OkFitEnded;
			
			//																										|
stop:		//	Le texte ne tient pas entièrement dans l'espace disponible. <---------------------------------------'
			//	Retourne les points de découpe (s'il y en a) :
			
			if (result.Count == 0)
			{
				if (scratch.LastBreakOffset > 0)
				{
					StretchProfile profile = scratch.LastStretchProfile;
					
					double total_width   = context.AvailableWidth;
					double total_penalty = scratch.LastBreakPenalty * profile.ComputePenalty (total_width, context.BreakFenceBefore, context.BreakFenceAfter);
						
					result.Add (new Layout.Break (scratch.LastBreakOffset, scratch.LastBreakAdvance, total_penalty, profile));
				}
				else
				{
					return Layout.Status.ErrorCannotFit;
				}
			}
			
			return Layout.Status.Ok;
		}
		
		public override Layout.Status Render(Layout.Context context, ITextRenderer renderer, int end_offset)
		{
			FitScratch scratch = new FitScratch ();
			
			scratch.Offset  = context.TextOffset;
			scratch.Advance = context.X;
			
			while (scratch.Offset < end_offset)
			{
				if (context.GetNextWord (scratch.Offset, out scratch.Text, out scratch.TextLength, out scratch.WordBreakInfo))
				{
					int word_end = scratch.Offset + scratch.TextLength;
					
					if (word_end > end_offset)
					{
						scratch.TextLength -= word_end;
						scratch.TextLength += end_offset;
					}
					
					scratch.TextStart = 0;
					
					while (scratch.TextLength > 0)
					{
						//	Détermine la prochaine tranche de caractères contigus utilisant
						//	le même style de fonte :
						
						scratch.RunLength = this.GetRunLength (scratch.Text, scratch.TextStart, scratch.TextLength);
						
						ulong code = scratch.Text[scratch.TextStart];
						
						Layout.BaseEngine         engine;
						Properties.LayoutProperty layout;
						
						context.TextContext.GetFont (code, out scratch.Font, out scratch.FontSize);
						context.TextContext.GetLayoutEngine (code, out engine, out layout);
						
						if ((engine != this) ||
							(layout != context.LayoutProperty))
						{
							//	Change de moteur de layout. Il faut par conséquent mémoriser où on
							//	s'arrête pour que le suivant sache où reprendre :
							
							context.X              = scratch.Advance;
							context.TextOffset     = scratch.Offset;
							context.LayoutEngine   = engine;
							context.LayoutProperty = layout;
							
							return Layout.Status.SwitchLayout;
						}
						
						this.RenderRun (context, renderer, ref scratch);
						
						//	Avance au morceau suivant :
						
						scratch.Advance    += scratch.TextWidth;
						scratch.Offset     += scratch.RunLength;
						scratch.TextStart  += scratch.RunLength;
						scratch.TextLength -= scratch.RunLength;
					}
				}
				else
				{
					//	Il n'y a plus aucun résultat disponible, sans pour autant
					//	que l'on ait atteint la fin du paragraphe; abandonne.
					
					//	Ceci est une erreur fatale dans ce contexte.
					
					return Layout.Status.ErrorNeedMoreText;
				}
			}
			
			//	Nous avons atteint la fin du texte spécifiée par l'appelant. La ligne
			//	a donc été "rendue" avec succès.
			
			return Layout.Status.Ok;
		}
		
		
		private bool FitRun(Layout.Context context, ref FitScratch scratch, ref Layout.BreakCollection result)
		{
			//	Analyse le texte constituté de caractères de style identique (même
			//	fonte, même layout). Si une césure est requise, procède par petites
			//	étapes progressives, sinon traite toute la tranche d'un coup.
			
			//	Retourne true s'il faut stopper immédiatement l'analyse (le texte
			//	est trop long pour l'espace de justification disponible).
			
			bool hyphenate = scratch.Hyphenate;
			
			StretchProfile profile = null;
			
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
				double   penalty;
				
				if (frag_length < scratch.RunLength)
				{
					//	Produit la césure manuellement (il faudrait faire mieux pour gérer
					//	correctement des langues comme le norvégien ou l'ancien allemand
					//	qui peuvent provoquer le dédoublement de certains caractères) :
					
					ulong save = scratch.Text[scratch.TextStart+frag_length];
					scratch.Text[scratch.TextStart+frag_length] = '-';
					
					glyphs    = scratch.Font.GenerateGlyphs (scratch.Text, scratch.TextStart, frag_length+1);
					can_break = true;
					penalty   = -1;
					profile   = new StretchProfile (scratch.StretchProfile);
					
					profile.Add (scratch.Font, scratch.FontSize, scratch.Text, scratch.TextStart, frag_length+1);
					
					scratch.Text[scratch.TextStart+frag_length] = save;
				}
				else
				{
					glyphs    = scratch.Font.GenerateGlyphs (scratch.Text, scratch.TextStart, frag_length);
					can_break = (scratch.RunLength == scratch.TextLength);
					hyphenate = hyphenate && (scratch.WordBreakInfo == Unicode.BreakInfo.Optional);
					penalty   = 1;
					profile   = scratch.StretchProfile;
					
					scratch.StretchProfile.Add (scratch.Font, scratch.FontSize, scratch.Text, scratch.TextStart, frag_length);
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
#if false
					double total_width = context.AvailableWidth;
					double text_width  = scratch.Advance + scratch.TextWidth - context.LeftMargin;
					
					Debug.Assert.IsTrue (total_width > 0);
					Debug.Assert.IsTrue (text_width > 0);
					
					double badness;
					
					//	Si le texte est plus large que l'espace disponible, c'est pire du
					//	point de vue qualitatif que s'il occupe trop peu de place (il est
					//	plus facile d'étendre des espaces que de les compresser).
					
					//	TODO: idéalement, le calcul de la "badness" devrait dépendre de
					//	l'élasticité du texte contenu dans cette ligne...
					
					if (text_width > total_width)
					{
						badness = 10 * (text_width - total_width) / total_width;
					}
					else
					{
						badness = (total_width - text_width) / total_width;
					}
					
					Debug.Assert.IsTrue (badness >=  0.0);
					Debug.Assert.IsTrue (badness <= 10.0);
					
					penalty = penalty * (100 * badness);
#endif
					scratch.LastBreakOffset    = scratch.Offset + frag_length;
					scratch.LastBreakAdvance   = scratch.Advance + scratch.TextWidth;
					scratch.LastBreakPenalty   = penalty;
					scratch.LastStretchProfile = new StretchProfile (profile);
					
					if (hyphenate)
					{
						double total_width   = context.AvailableWidth;
						double total_penalty = scratch.LastBreakPenalty * profile.ComputePenalty (total_width, context.BreakFenceBefore, context.BreakFenceAfter);
						
						result.Add (new Layout.Break (scratch.LastBreakOffset, scratch.LastBreakAdvance, total_penalty, profile));
					}
				}
			}
			
			return false;
		}
		
		private void RenderRun(Layout.Context context, ITextRenderer renderer, ref FitScratch scratch)
		{
			int length = scratch.RunLength;
			int pos    = scratch.TextStart;
			
			ushort[] glyphs     = null;
			byte[]   attributes = new byte[length+10];
			
			for (int i = 0; i < length; i++)
			{
				attributes[i] = (byte) Unicode.BreakAnalyzer.GetStretchClass (Unicode.Bits.GetCode (scratch.Text[pos+i]));
			}
			
			if (context.Hyphenate)
			{
				//	Produit la césure manuellement (il faudrait faire mieux pour gérer
				//	correctement des langues comme le norvégien ou l'ancien allemand
				//	qui peuvent provoquer le dédoublement de certains caractères) :
				
				ulong save = scratch.Text[pos+length];
				
				scratch.Text[pos+length] = '-';
				attributes[pos+length]   = (byte) Unicode.BreakAnalyzer.GetStretchClass ('-');
				
				scratch.Font.GenerateGlyphs (scratch.Text, pos, length, out glyphs, attributes);
				
				scratch.Text[pos+length] = save;
			}
			else
			{
				scratch.Font.GenerateGlyphs (scratch.Text, pos, length, out glyphs, attributes);
			}
			
			StretchProfile.Scales scales = context.TextStretchScales;
			
			int n = glyphs.Length;
			
			double y = context.Y;
			
			double[] x_scale = new double[n];
			double[] x_pos   = new double[n];
			double[] y_pos   = new double[n];
			
			for (int i = 0; i < n; i++)
			{
				switch ((Unicode.StretchClass) attributes[i])
				{
					case Unicode.StretchClass.NoStretch:		x_scale[i] = scales.ScaleNoStretch;	break;
					case Unicode.StretchClass.Character:		x_scale[i] = scales.ScaleCharacter;	break;
					case Unicode.StretchClass.CharacterSpace:	x_scale[i] = scales.ScaleCharacter;	break;
					case Unicode.StretchClass.Space:			x_scale[i] = scales.ScaleSpace;		break;
					case Unicode.StretchClass.Kashida:			x_scale[i] = scales.ScaleKashida;		break;
					
					default:
						throw new System.InvalidOperationException ();
				}
				
				y_pos[i] = y;
			}
			
			scratch.Advance += scratch.Font.GetPositions (glyphs, scratch.FontSize, scratch.Advance, x_pos, x_scale);
			
			renderer.Render (context.Frame, scratch.Font, scratch.FontSize, glyphs, x_pos, y_pos, x_scale, null);
		}
		
		
		
		private struct FitScratch
		{
			public int							Offset;
			public double						Advance;
			public StretchProfile				StretchProfile;
			
			public int							LastBreakOffset;
			public double						LastBreakAdvance;
			public double						LastBreakPenalty;
			public StretchProfile				LastStretchProfile;
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
