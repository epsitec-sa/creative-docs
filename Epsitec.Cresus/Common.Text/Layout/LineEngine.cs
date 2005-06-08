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
			bool inhibit_tabs = false;
			
			if ((context.Disposition != 0) ||
				(context.Justification != 0))
			{
//				inhibit_tabs = true;
			}
			
			FitScratch scratch = new FitScratch ();
			
			scratch.Offset    = context.TextOffset;
			scratch.AddBreak  = context.BreakAnywhere;
			scratch.FenceMinX = context.LineWidth-context.RightMargin-context.BreakFenceBefore;
			scratch.FenceMaxX = context.LineWidth-context.RightMargin+context.BreakFenceAfter;
			
			scratch.Advance        = context.X;
			scratch.WordBreakInfo  = Unicode.BreakInfo.No;
			scratch.StretchProfile = new StretchProfile ();
			
			for (;;)
			{
				if (context.GetNextWord (scratch.Offset, out scratch.Text, out scratch.TextLength, out scratch.WordBreakInfo))
				{
					if ((inhibit_tabs) &&
						(scratch.WordBreakInfo == Unicode.BreakInfo.HorizontalTab))
					{
						scratch.WordBreakInfo = Unicode.BreakInfo.Optional;
					}
					
					scratch.TextStart = 0;
					
					while (scratch.TextLength > 0)
					{
						if (scratch.Advance > scratch.FenceMinX)
						{
							//	Nous sommes dans la zone sensible nécessitant la césure :
							
							scratch.AddBreak = true;
						}
						
						//	Détermine la prochaine tranche de caractères contigus utilisant
						//	exactement le même style :
						
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
							
							context.MoveTo (scratch.Advance, scratch.Offset);
							context.SwitchLayoutEngine (engine, layout);
							
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
						
						context.MoveTo (scratch.Advance, scratch.Offset);
						
						result.Add (new Layout.Break (scratch.Offset, scratch.Advance, 0, 0, scratch.StretchProfile));
						
						return (scratch.WordBreakInfo == Unicode.BreakInfo.HorizontalTab) ? Layout.Status.OkTabReached : Layout.Status.OkFitEnded;
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
			
			//																										|
stop:		//	Le texte ne tient pas entièrement dans l'espace disponible. <---------------------------------------'
			//	Retourne les points de découpe (s'il y en a) :
			
			if (result.Count == 0)
			{
				if (scratch.LastBreakOffset > 0)
				{
					StretchProfile profile = scratch.LastStretchProfile;
					
					double total_width   = context.AvailableWidth;
					double space_penalty = profile.ComputePenalty (total_width, context.BreakFenceBefore, context.BreakFenceAfter);
						
					result.Add (new Layout.Break (scratch.LastBreakOffset, scratch.LastBreakAdvance, space_penalty, scratch.LastBreakPenalty, profile));
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
					//	Détermine la prochaine tranche de caractères contigus utilisant
					//	exactement le même style :
					
					int run_length = this.GetRunLength (text, offset, length);
					
					Layout.BaseEngine         engine;
					Properties.LayoutProperty layout;
					
					context.TextContext.GetLayoutEngine (text[offset], out engine, out layout);
					context.UpdateUnderlineProperties (offset, ox, true);
					
					if ((engine != this) ||
						(layout != context.LayoutProperty))
					{
						//	Change de moteur de layout. Il faut par conséquent mémoriser où on
						//	s'arrête pour que le suivant sache où reprendre :
						
						context.MoveTo (ox, offset + context.TextOffset);
						context.SwitchLayoutEngine (engine, layout);
						
						return Layout.Status.SwitchLayout;
					}
					
					this.RenderRun (context, renderer, ref ox, oy, text, offset, run_length, length == run_length);
					
					//	Avance au morceau suivant :
					
					offset += run_length;
					length -= run_length;
				}
			}
			else
			{
				//	Il n'y a plus aucun résultat disponible, sans pour autant que
				//	l'on ait atteint la fin du paragraphe; abandonne.
				
				//	Ceci est une erreur fatale dans ce contexte.
				
				return Layout.Status.ErrorNeedMoreText;
			}
			
			//	Nous avons atteint la fin du texte spécifiée par l'appelant. La ligne
			//	a donc été "rendue" avec succès.
			
			context.MoveTo (ox, context.TextOffset + length);
			
			return Layout.Status.Ok;
		}
		
		public override Layout.Status FillProfile(Layout.Context context, int length, StretchProfile profile)
		{
			ulong[] text;
			
			if (context.GetTextCopy (context.TextOffset, out text, ref length))
			{
				int offset = 0;
				
				while (length > 0)
				{
					//	Détermine la prochaine tranche de caractères contigus utilisant
					//	exactement le même style :
					
					int run_length = this.GetRunLength (text, offset, length);
					
					Layout.BaseEngine         engine;
					Properties.LayoutProperty layout;
					
					context.TextContext.GetLayoutEngine (text[offset], out engine, out layout);
					
					if ((engine != this) ||
						(layout != context.LayoutProperty))
					{
						throw new System.NotImplementedException ("Engine change not supported in LineEngine.FillProfile.");
					}
					
					this.FillProfileWithRun (context, text, offset, run_length, length == run_length, profile);
					
					//	Avance au morceau suivant :
					
					offset += run_length;
					length -= run_length;
				}
			}
			else
			{
				//	Il n'y a plus aucun résultat disponible, sans pour autant que
				//	l'on ait atteint la fin du paragraphe; abandonne.
				
				//	Ceci est une erreur fatale dans ce contexte.
				
				return Layout.Status.ErrorNeedMoreText;
			}
			
			return Layout.Status.Ok;
		}
		
		
		private bool FitRun(Layout.Context context, ref FitScratch scratch, ref Layout.BreakCollection result)
		{
			//	Analyse le fragment de texte constituté de caractères de style
			//	géométriquement identique (même fonte, même layout).
			
			//	Si une césure est requise, traite tous les points de découpe qui
			//	sont possibles, sinon traite toute la tranche d'un coup.
			
			//	Retourne true s'il faut stopper immédiatement l'analyse (le texte
			//	est trop long pour l'espace de justification disponible).
			
			StretchProfile profile = null;
			
			ulong[] text = scratch.Text;
			
			int offset      = scratch.TextStart;
			int run_length  = scratch.RunLength;
			int text_length = scratch.TextLength;
			int frag_length = 0;
			
			while (frag_length < run_length)
			{
				double break_penalty;
				
				if (scratch.AddBreak && context.EnableHyphenation)
				{
					frag_length = this.GetNextFragmentLength (text, offset, run_length, frag_length, out break_penalty);
				}
				else
				{
					frag_length   = run_length;
					break_penalty = 0;
				}
				
				ulong    hyphen = scratch.Font.GetHyphen ();
				ushort[] glyphs;
				byte[]   attr;
				bool     can_break;
				bool     add_break;
				
				if (frag_length < run_length)
				{
					//	Produit la césure manuellement (il faudrait faire mieux pour gérer
					//	correctement des langues comme le suédois ou l'ancien allemand
					//	qui peuvent provoquer le dédoublement de certains caractères) :
					
					ulong[] temp = this.GetHyphenatedText (text, offset, frag_length, hyphen);
					
					BaseEngine.GenerateGlyphsAndStretchClassAttributes (context.TextContext, scratch.Font, temp, 0, frag_length + 1, out glyphs, out attr);
					
					can_break = true;
					add_break = true;
					profile   = new StretchProfile (scratch.StretchProfile);
					
					profile.Add (scratch.Font, scratch.FontSize, glyphs, attr);
					
					scratch.TextWidth = scratch.Font.GetTotalWidth (glyphs, scratch.FontSize);
				}
				else
				{
					profile = scratch.StretchProfile;
					
					BaseEngine.GenerateGlyphsAndStretchClassAttributes (context.TextContext, scratch.Font, text, offset, frag_length, out glyphs, out attr);
					
					scratch.TextWidth = scratch.Font.GetTotalWidth (glyphs, scratch.FontSize);
					
					if ((scratch.Advance+scratch.TextWidth > scratch.FenceMinX) &&
						(scratch.AddBreak == false))
					{
						//	Le fragment de mot déborde dans la zone nécessitant une
						//	découpe :
						
						scratch.AddBreak = true;
						
						if (context.EnableHyphenation)
						{
							//	Reprend au début du fragment de mot pour traiter les
							//	points de césure :
							
							frag_length = 0;
							continue;
						}
					}
					
					//	Détermine si une coupure de ligne est possible (comme c'est
					//	le cas à la frontière de mots) :
					
					can_break = (run_length == text_length);
					add_break = scratch.AddBreak && (scratch.WordBreakInfo == Unicode.BreakInfo.Optional);
					
					profile.Add (scratch.Font, scratch.FontSize, glyphs, attr);
				}
				
				if (scratch.Advance+scratch.TextWidth-profile.WidthEndSpace > scratch.FenceMaxX)
				{
					//	Dépassé la marge de droite. Arrête immédiatement sans tenir compte
					//	du résultat :
					
					return true;
				}
				
				if (can_break)
				{
					scratch.LastBreakOffset    = scratch.Offset + frag_length;
					scratch.LastBreakAdvance   = scratch.Advance + scratch.TextWidth - profile.WidthEndSpace;
					scratch.LastBreakPenalty   = break_penalty;
					scratch.LastStretchProfile = new StretchProfile (profile);
					
					if (add_break)
					{
						double total_width   = context.AvailableWidth;
						double space_penalty = profile.ComputePenalty (total_width, context.BreakFenceBefore, context.BreakFenceAfter);
						
						result.Add (new Layout.Break (scratch.LastBreakOffset, scratch.LastBreakAdvance, space_penalty, break_penalty, profile));
					}
				}
			}
			
			return false;
		}

		private void RenderRun(Layout.Context context, ITextRenderer renderer, ref double ox, double oy, ulong[] text, int offset, int length, bool last_run)
		{
			//	Détermine la fonte qu'il faudra utiliser pour le fragment de texte
			//	dont il faut faire le rendu :
			
			Drawing.Color color;
			OpenType.Font font;
			double        font_size;
			
			context.TextContext.GetFont (text[offset], out font, out font_size);
			context.TextContext.GetColor (text[offset], out color);
			
			//	Gérer l'étirement des glyphes en fonction de la fonte sélectionnée :
			
			StretchProfile.Scales scales = context.TextStretchScales;
			double                glue   = context.TextStretchGlue;
			
			if (glue > 0)
			{
				font.PushActiveFeatures ();
				font.DisableActiveFeatures ("liga", "dlig");
			}
			
			//	Génère les glyphes et les informations relatives à l'extensibilité
			//	pour le fragment de texte :
			
			ushort[] glyphs     = null;
			byte[]   attributes = new byte[length+10];
			
			Unicode.BreakAnalyzer.GetStretchClass (text, offset, length, attributes);
			
			if ((last_run) &&
				(context.EnableHyphenation)) // TODO: tirer au clair, où doit-on ajouter la césure ???
			{
				//	Produit la césure manuellement (il faudrait faire mieux pour gérer
				//	correctement des langues comme le suédois ou l'ancien allemand
				//	qui peuvent provoquer le dédoublement de certains caractères) :
				
				ulong[] temp = this.GetHyphenatedText (text, offset, length, font.GetHyphen ());
				int     end  = temp.Length - 1;
				
				attributes[end] = Unicode.BreakAnalyzer.GetStretchClass (temp[end]);
				
				BaseEngine.GenerateGlyphs (context.TextContext, font, temp, 0, length+1, out glyphs, attributes);
			}
			else
			{
				BaseEngine.GenerateGlyphs (context.TextContext, font, text, offset, length, out glyphs, attributes);
			}
			
			if (glue > 0)
			{
				font.PopActiveFeatures ();
			}
			
			//	Détermine les mises à l'échelle des divers glyphes, selon leur
			//	classe d'élasticité :
			
			int n = glyphs.Length;
			
			double[] x_scale = new double[n];
			double[] x_pos   = new double[n];
			double[] x_glue  = new double[n];
			double[] y_pos   = new double[n];
			
			this.GenerateXScale (attributes, scales, x_scale);
			
			for (int i = 0; i < n; i++)
			{
				y_pos[i]  = oy;
				x_glue[i] = glue;
			}
			
			//	Détermine la position horizontale de chaque glyphe :
			
			ox += font.GetPositions (glyphs, font_size, ox, x_pos, x_scale, x_glue);
			
			//	Demande à ITextRenderer de faire le rendu avec les positions que
			//	nous venons de déterminer :
			
			renderer.Render (context.Frame, font, font_size, color, glyphs, x_pos, y_pos, x_scale, null);
		}
		
		private ulong[] GetHyphenatedText(ulong[] text, int offset, int length, ulong hyphen)
		{
			ulong[] copy = new ulong[length+1];
			
			System.Buffer.BlockCopy (text, offset * 8, copy, 0, length * 8);
			
			copy[length] = hyphen;
			
			return copy;
		}
		
		private void FillProfileWithRun(Layout.Context context, ulong[] text, int offset, int length, bool last_run, StretchProfile profile)
		{
			OpenType.Font font;
			double        font_size;
			
			context.TextContext.GetFont (text[offset], out font, out font_size);
			
			double glue = context.TextStretchGlue;
			
			if (glue > 0)
			{
				font.PushActiveFeatures ();
				font.DisableActiveFeatures ("liga", "dlig");
			}
			
			//	Génère les glyphes et les informations relatives à l'extensibilité
			//	pour le fragment de texte :
			
			if ((last_run) &&
				(context.EnableHyphenation)) // TODO: tirer au clair, où doit-on ajouter la césure ???
			{
				//	Produit la césure manuellement (il faudrait faire mieux pour gérer
				//	correctement des langues comme le suédois ou l'ancien allemand
				//	qui peuvent provoquer le dédoublement de certains caractères) :
				
				ulong[] temp = this.GetHyphenatedText (text, offset, length, font.GetHyphen ());
				
				profile.Add (context.TextContext, font, font_size, temp, 0, length + 1);
			}
			else
			{
				profile.Add (context.TextContext, font, font_size, text, offset, length);
			}
			
			if (glue > 0)
			{
				font.PopActiveFeatures ();
			}
		}
		
		
		private int GenerateXScale(byte[] attributes, StretchProfile.Scales scales, double[] x_scale)
		{
			int kashida_count = 0;
			
			for (int i = 0; i < x_scale.Length; i++)
			{
				Unicode.StretchClass stretch = (Unicode.StretchClass) attributes[i];
				
				switch (stretch)
				{
					case Unicode.StretchClass.NoStretch:		x_scale[i] = scales.ScaleNoStretch;							break;
					case Unicode.StretchClass.Character:		x_scale[i] = scales.ScaleCharacter;							break;
					case Unicode.StretchClass.CharacterSpace:	x_scale[i] = scales.ScaleCharacter;							break;
					case Unicode.StretchClass.Space:			x_scale[i] = scales.ScaleSpace;								break;
					case Unicode.StretchClass.Kashida:			x_scale[i] = scales.ScaleKashida;		kashida_count++;	break;
				}
			}
			
			return kashida_count;
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
			
			public bool							AddBreak;
			
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
