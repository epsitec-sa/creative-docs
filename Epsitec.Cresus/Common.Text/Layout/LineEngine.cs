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
							//	Nous sommes dans la zone sensible n�cessitant la c�sure :
							
							scratch.Hyphenate = true;
						}
						
						//	D�termine la prochaine tranche de caract�res contigus utilisant
						//	exactement le m�me style :
						
						scratch.RunLength = this.GetRunLength (scratch.Text, scratch.TextStart, scratch.TextLength);
						
						ulong code = scratch.Text[scratch.TextStart];
						
						Layout.BaseEngine         engine;
						Properties.LayoutProperty layout;
						
						context.TextContext.GetFont (code, out scratch.Font, out scratch.FontSize);
						context.TextContext.GetLayoutEngine (code, out engine, out layout);
						
						if ((engine != this) ||
							(layout != context.LayoutProperty))
						{
							//	Change de moteur de layout. Il faut par cons�quent m�moriser o� on
							//	s'arr�te pour que le suivant sache o� reprendre :
							
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
			
			result.Add (new Layout.Break (scratch.Offset, scratch.Advance, 0, scratch.StretchProfile));
			
			return Layout.Status.OkFitEnded;
			
			//																										|
stop:		//	Le texte ne tient pas enti�rement dans l'espace disponible. <---------------------------------------'
			//	Retourne les points de d�coupe (s'il y en a) :
			
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
		
		public override Layout.Status Render(Layout.Context context, ITextRenderer renderer, int length)
		{
			double  ox = context.X;
			double  oy = context.Y;
			ulong[] text;
			
			if (context.GetTextCopy (context.TextOffset, out text, ref length))
			{
				int offset = 0;
				
				while (length > 0)
				{
					//	D�termine la prochaine tranche de caract�res contigus utilisant
					//	exactement le m�me style :
					
					int run_length = this.GetRunLength (text, offset, length);
					
					Layout.BaseEngine         engine;
					Properties.LayoutProperty layout;
					
					context.TextContext.GetLayoutEngine (text[offset], out engine, out layout);
					
					if ((engine != this) ||
						(layout != context.LayoutProperty))
					{
						//	Change de moteur de layout. Il faut par cons�quent m�moriser o� on
						//	s'arr�te pour que le suivant sache o� reprendre :
						
						context.X              = ox;
						context.TextOffset    += offset;
						context.LayoutEngine   = engine;
						context.LayoutProperty = layout;
						
						return Layout.Status.SwitchLayout;
					}
					
					this.RenderRun (context, renderer, ref ox, oy, text, offset, run_length);
					
					//	Avance au morceau suivant :
					
					offset += run_length;
					length -= run_length;
				}
			}
			else
			{
				//	Il n'y a plus aucun r�sultat disponible, sans pour autant que
				//	l'on ait atteint la fin du paragraphe; abandonne.
				
				//	Ceci est une erreur fatale dans ce contexte.
				
				return Layout.Status.ErrorNeedMoreText;
			}
			
			//	Nous avons atteint la fin du texte sp�cifi�e par l'appelant. La ligne
			//	a donc �t� "rendue" avec succ�s.
			
			return Layout.Status.Ok;
		}
		
		
		private bool FitRun(Layout.Context context, ref FitScratch scratch, ref Layout.BreakCollection result)
		{
			//	Analyse le texte constitut� de caract�res de style identique (m�me
			//	fonte, m�me layout). Si une c�sure est requise, proc�de par petites
			//	�tapes progressives, sinon traite toute la tranche d'un coup.
			
			//	Retourne true s'il faut stopper imm�diatement l'analyse (le texte
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
				
				ulong    hyphen = scratch.Font.GetHyphen ();
				ushort[] glyphs;
				bool     can_break;
				double   penalty;
				
				if (frag_length < scratch.RunLength)
				{
					//	Produit la c�sure manuellement (il faudrait faire mieux pour g�rer
					//	correctement des langues comme le norv�gien ou l'ancien allemand
					//	qui peuvent provoquer le d�doublement de certains caract�res) :
					
					ulong save = scratch.Text[scratch.TextStart+frag_length];
					scratch.Text[scratch.TextStart+frag_length] = hyphen;
					
					glyphs    = scratch.Font.GenerateGlyphs (scratch.Text, scratch.TextStart, frag_length+1);
					can_break = true;
					penalty   = -1;
					profile   = new StretchProfile (scratch.StretchProfile);
					
					profile.Add (scratch.Font, scratch.FontSize, scratch.Text, scratch.TextStart, frag_length+1);
					
					scratch.Text[scratch.TextStart+frag_length] = save;
				}
				else
				{
					//	D�termine si le texte se termine par des espaces. Si oui, il
					//	ne faut pas en tenir compte dans le calcul de la p�nalit�, ni
					//	dans le calcul de la largeur.
					
					int black_length = frag_length;
					int white_length = 0;
					
					can_break = (scratch.RunLength == scratch.TextLength);
					hyphenate = hyphenate && (scratch.WordBreakInfo == Unicode.BreakInfo.Optional);
					penalty   = 1;
					
					//	TODO: g�rer les espaces qui d�passent de la ligne...
					
					profile = scratch.StretchProfile;
					glyphs  = scratch.Font.GenerateGlyphs(scratch.Text, scratch.TextStart, black_length);
					
					scratch.StretchProfile.Add (scratch.Font, scratch.FontSize, scratch.Text, scratch.TextStart, black_length);
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

		private void RenderRun(Layout.Context context, ITextRenderer renderer, ref double ox, double oy, ulong[] text, int offset, int length)
		{
			//	D�termine la font qu'il faudra utiliser pour le fragment de texte
			//	dont il faut faire le rendu :
			
			OpenType.Font font;
			double        font_size;
			context.TextContext.GetFont (text[offset], out font, out font_size);
			
			//	G�n�re les glyphes et les informations relatives � l'extensibilit�
			//	pour le fragment de texte :
			
			ulong    hyphen     = font.GetHyphen ();
			ushort[] glyphs     = null;
			byte[]   attributes = new byte[length+10];
			
			Unicode.BreakAnalyzer.GetStretchClass (text, offset, length, attributes);
			
			if (context.Hyphenate)
			{
				//	Produit la c�sure manuellement (il faudrait faire mieux pour g�rer
				//	correctement des langues comme le norv�gien ou l'ancien allemand
				//	qui peuvent provoquer le d�doublement de certains caract�res) :
				
				int   end = offset + length;
				ulong old = text[end];
				
				text[end]       = hyphen;
				attributes[end] = Unicode.BreakAnalyzer.GetStretchClass (hyphen);
				
				font.GenerateGlyphs (text, offset, length, out glyphs, attributes);
				
				text[end] = old;
			}
			else
			{
				font.GenerateGlyphs (text, offset, length, out glyphs, attributes);
			}
			
			//	D�termine les mises � l'�chelle des divers glyphes, selon leur
			//	classe d'�lasticit� :
			
			StretchProfile.Scales scales = context.TextStretchScales;
			
			int n = glyphs.Length;
			
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
					case Unicode.StretchClass.Kashida:			x_scale[i] = scales.ScaleKashida;	break;
					
					default:
						throw new System.InvalidOperationException ();
				}
				
				y_pos[i] = oy;
			}
			
			//	D�termine la position horizontale de chaque glyphe :
			
			ox += font.GetPositions (glyphs, font_size, ox, x_pos, x_scale);
			
			//	Demande � ITextRenderer de faire le rendu avec les positions que
			//	nous venons de d�terminer :
			
			renderer.Render (context.Frame, font, font_size, glyphs, x_pos, y_pos, x_scale, null);
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
