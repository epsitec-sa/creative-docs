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
			scratch.BreakMode = context.BreakMode;
			scratch.FenceMinX = context.LineWidth-context.RightMargin-context.BreakFenceBefore;
			scratch.FenceMaxX = context.LineWidth-context.RightMargin+context.BreakFenceAfter;
			
			scratch.Advance        = context.LineCurrentX;
			scratch.WordBreakInfo  = Unicode.BreakInfo.No;
			scratch.StretchProfile = new StretchProfile ();
			
			bool has_visible_text = false;
			bool has_hidden_text  = false;
			
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
							
							if (scratch.BreakMode == BreakMode.Default)
							{
								scratch.BreakMode = BreakMode.Hyphenate;
							}
						}
						
						//	Détermine la prochaine tranche de caractères contigus utilisant
						//	exactement le même style :
						
						scratch.RunLength = this.GetRunLength (scratch.Text, scratch.TextStart, scratch.TextLength);
						scratch.TextWidth = 0;
						
						ulong code = scratch.Text[scratch.TextStart];
						
						if (! context.TextContext.TestConditions (code))
						{
							if (scratch.TextLength <= scratch.RunLength)
							{
								//	Si on touche à la fin de la ligne (point de découpe atteint) et que
								//	la ligne contient du texte visible, considère le texte caché comme
								//	étant insécable. Ceci permet d'assurer qu'une zone de texte cachée
								//	à cheval entre deux paragraphes ne détecte pas la fin du premier
								//	paragraphe.
								
								if (has_visible_text)
								{
									scratch.WordBreakInfo = Unicode.BreakInfo.No;
								}
							}
							
							has_hidden_text = true;
							
							goto advance_next;
						}
						
						has_visible_text = true;
						
						Layout.BaseEngine         engine;
						Properties.LayoutProperty layout;
						double font_scale;
						double font_advance;
						
						context.TextContext.GetFontAndSize (code, out scratch.Font, out scratch.FontSize, out font_scale, out font_advance);
						context.TextContext.GetFontOffsets (code, out scratch.FontBaseline, out scratch.FontAdvance);
						context.TextContext.GetLayoutEngine (code, out engine, out layout);
						
						scratch.FontAdvance += font_advance;
						scratch.FontSize    *= font_scale;
						
						if ((engine != this) ||
							(layout != context.LayoutProperty))
						{
							//	Change de moteur de layout. Il faut par conséquent mémoriser où on
							//	s'arrête pour que le suivant sache où reprendre :
							
							context.RecordAscender (scratch.Ascender);
							context.RecordDescender (scratch.Descender);
							context.RecordLineHeight (System.Math.Max (scratch.LineHeight, (scratch.Ascender - scratch.Descender) * 1.2));
							
							context.MoveTo (scratch.Advance, scratch.Offset);
							context.SwitchLayoutEngine (engine, layout);
							
							context.TextStretchProfile.Add (scratch.StretchProfile);
							
							return Layout.Status.SwitchLayout;
						}
						
						if (this.FitRun (context, ref scratch, ref result))
						{
							goto stop; // --------------------------------------------------------------------------.
						}              //																			|
						//																							|
						//	Avance au morceau suivant :																:
						
advance_next:
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
						
						if (has_visible_text)
						{
							context.RecordAscender (scratch.Ascender);
							context.RecordDescender (scratch.Descender);
							context.RecordLineHeight (System.Math.Max (scratch.LineHeight, (scratch.Ascender - scratch.Descender) * 1.2));
							
							result.Add (new Layout.Break (scratch.Offset, scratch.Advance, 0, 0, scratch.StretchProfile));
						}
						else if (has_hidden_text)
						{
							//	Aucun texte visible; uniquement du texte invisible.
							
							return Layout.Status.OkHiddenFitEnded;
						}
						else
						{
							throw new System.InvalidOperationException ("Neither visible nor hidden text found");
						}
						
						context.MoveTo (scratch.Advance, scratch.Offset);
						
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
			double  ox = context.LineCurrentX;
			double  oy = context.LineBaseY;
			ulong[] text;
			
			if (context.GetTextCopy (context.TextOffset, out text, ref length))
			{
				int offset = 0;
				
				while (length > 0)
				{
					//	Détermine la prochaine tranche de caractères contigus utilisant
					//	exactement le même style :
					
					int run_length = this.GetRunLength (text, offset, length);
					
					if (! context.TextContext.TestConditions (text[offset]))
					{
						goto advance_next;
					}
					
					Layout.BaseEngine         engine;
					Properties.LayoutProperty layout;
					
					context.TextContext.GetLayoutEngine (text[offset], out engine, out layout);
					context.UpdateXlineProperties (offset, ox, true);
					
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
advance_next:		
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
			
			if ((run_length == 1) &&
				(Unicode.Bits.GetCode (text[offset]) == (int) Unicode.Code.ObjectReplacement))
			{
				Properties.ImageProperty image;
				profile = scratch.StretchProfile;
				
				context.TextContext.GetImage (text[offset], out image);
				
				double ascender;
				double descender;
				double advance;
				double x1, x2;
				
				image.GetGeometry (out ascender, out descender, out advance, out x1, out x2);
				
				scratch.TextWidth = advance;
				
				profile.Add (Unicode.StretchClass.NoStretch, advance);
				
				if (scratch.Advance+scratch.TextWidth > scratch.FenceMaxX)
				{
					return true;
				}
				
				scratch.RecordAscender (ascender);
				scratch.RecordDescender (descender);
				scratch.RecordLineHeight ((ascender - descender) * 1.2);
				
				return false;
			}
			
			scratch.RecordAscender (scratch.Font.GetAscender (scratch.FontSize) + scratch.FontBaseline);
			scratch.RecordDescender (scratch.Font.GetDescender (scratch.FontSize) + scratch.FontBaseline);
			scratch.RecordLineHeight (scratch.FontSize * 1.2);
			
			while (frag_length < run_length)
			{
				double break_penalty;
				
				bool can_break = false;
				bool add_break = false;
				
				if ((scratch.BreakMode == BreakMode.Hyphenate) &&
					(context.EnableHyphenation))
				{
					frag_length = this.GetNextFragmentLength (text, offset, run_length, frag_length, out break_penalty);
				}
				else if (scratch.BreakMode == BreakMode.Break)
				{
					//	Break catastrophique. Evalue si le résultat est mauvais ou très
					//	mauvais...
					
					int code = Unicode.Bits.GetCode (text[offset+frag_length]);
					
					if (Unicode.DefaultBreakAnalyzer.IsSpace (code))
					{
						break_penalty = 1*1000.0;
					}
					else if (Unicode.DefaultBreakAnalyzer.IsZeroWidth (code))
					{
						break_penalty = 10*1000.0;
					}
					else
					{
						break_penalty = 100*1000.0;
					}
					
					frag_length++;
				}
				else
				{
					frag_length   = run_length;
					break_penalty = 0;
				}
				
				ulong    hyphen = scratch.Font.HyphenChar;
				ushort[] glyphs;
				byte[]   attr;
				
				bool special_glyphs = BaseEngine.ContainsSpecialGlyphs (text, offset, frag_length);
				
				System.Diagnostics.Debug.Assert ((special_glyphs == false) || (frag_length == 1));
				
				if ((frag_length < run_length) &&
					(scratch.BreakMode == BreakMode.Hyphenate) &&
					(special_glyphs == false))
				{
					//	Produit la césure manuellement (il faudrait faire mieux pour gérer
					//	correctement des langues comme le suédois ou l'ancien allemand
					//	qui peuvent provoquer le dédoublement de certains caractères) :
					
					ulong[] temp = this.GetHyphenatedText (text, offset, frag_length, hyphen);
					
					if (BaseEngine.GenerateGlyphsAndStretchClassAttributes (context.TextContext, scratch.Font, temp, 0, frag_length + 1, out glyphs, out attr))
					{
						throw new System.InvalidOperationException ();
					}
					
					can_break  = true;
					add_break  = true;
					profile    = new StretchProfile (scratch.StretchProfile);
					
					profile.Add (scratch.Font, scratch.FontSize, glyphs, attr, scratch.FontAdvance);
					
					scratch.TextWidth = scratch.Font.GetTotalWidth (glyphs, scratch.FontSize) + glyphs.Length * scratch.FontAdvance;
				}
				else
				{
					if (scratch.BreakMode == BreakMode.Break)
					{
						profile = new StretchProfile (scratch.StretchProfile);
					}
					else
					{
						profile = scratch.StretchProfile;
					}
					
					if (BaseEngine.GenerateGlyphsAndStretchClassAttributes (context.TextContext, scratch.Font, text, offset, frag_length, out glyphs, out attr))
					{
						throw new System.InvalidOperationException ();
					}
					
					scratch.TextWidth = scratch.Font.GetTotalWidth (glyphs, scratch.FontSize) + glyphs.Length * scratch.FontAdvance;
					
					if ((scratch.Advance+scratch.TextWidth > scratch.FenceMinX) &&
						(scratch.BreakMode == BreakMode.Default))
					{
						//	Le fragment de mot déborde dans la zone nécessitant une
						//	découpe :
						
						scratch.BreakMode = BreakMode.Hyphenate;
						
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
					
					can_break = (run_length == text_length) || (scratch.BreakMode == BreakMode.Break);
					add_break = (scratch.BreakMode != BreakMode.Default) && (scratch.WordBreakInfo == Unicode.BreakInfo.Optional);
					
					profile.Add (scratch.Font, scratch.FontSize, glyphs, attr, scratch.FontAdvance);
				}
				
				if (scratch.Advance+scratch.TextWidth-profile.WidthEndSpace > scratch.FenceMaxX)
				{
					//	Dépassé la marge de droite. Arrête immédiatement sans tenir compte
					//	du résultat :
					
					return true;
				}
				
				if (can_break)
				{
					context.RecordAscender (scratch.Ascender);
					context.RecordDescender (scratch.Descender);
					context.RecordLineHeight (System.Math.Max (scratch.LineHeight, (scratch.Ascender - scratch.Descender) * 1.2));
					
					//	TODO: il faudrait enregistrer les hauteurs de la ligne avec l'information
					//	Layout.Break si on voulait faire les choses correctement ici !
					
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

		private void RenderRun(Layout.Context context, ITextRenderer renderer, ref double ox, double oy, ulong[] text, int offset, int length, bool is_last_run)
		{
			string color;
			context.TextContext.GetColor (text[offset], out color);
			
			if ((length == 1) &&
				(Unicode.Bits.GetCode (text[offset]) == (int) Unicode.Code.ObjectReplacement))
			{
				Properties.ImageProperty image;
				
				context.TextContext.GetImage (text[offset], out image);
				
				double ascender;
				double descender;
				double advance;
				double x1, x2;
				
				image.GetGeometry (out ascender, out descender, out advance, out x1, out x2);
				
				//	Le rendu réel est fait par le renderer, ce qui lui permet d'appliquer
				//	éventuellement des modifications :
				
				renderer.Render (context, image, color, ox, oy, is_last_run);
				
				ox += advance;
				
				return;
			}
			
			
			//	Détermine la fonte qu'il faudra utiliser pour le fragment de texte
			//	dont il faut faire le rendu :
			
			OpenType.Font font;
			double        font_size;
			double        font_baseline;
			double        font_advance;
			double        font_scale;
			double        font_advance_offset;
			
			context.TextContext.GetFontAndSize (text[offset], out font, out font_size, out font_scale, out font_advance);
			context.TextContext.GetFontOffsets (text[offset], out font_baseline, out font_advance_offset);
			
			font_size    *= font_scale;
			font_advance += font_advance_offset;
			
			//	Si l'appelant a désactivé le décalage vertical de la ligne de base
			//	(par ex. pour déterminer la position du curseur), on en tient compte
			//	ici :
			
			if (context.IsFontBaselineOffsetDisabled)
			{
				font_baseline = 0;
			}
			
			//	Gérer l'étirement des glyphes en fonction de la fonte sélectionnée :
			
			StretchProfile.Scales scales = context.TextStretchScales;
			double                glue   = context.TextStretchGlue;
			
			if ((glue > 0) || (font_advance != 0))
			{
				font.PushActiveFeatures ();
				font.DisableActiveFeatures ("liga", "clig", "hlig", "dlig");
			}
			
			//	Génère les glyphes et les informations relatives à l'extensibilité
			//	pour le fragment de texte :
			
			TextToGlyphMapping mapping = null;
			
			ushort[] glyphs     = null;
			byte[]   attributes = new byte[length+10];
			
			Unicode.BreakAnalyzer.GetStretchClass (text, offset, length, attributes);
			
			bool special_glyphs = BaseEngine.ContainsSpecialGlyphs (text, offset, length);
			int  extra_glyphs   = 0;
			
			System.Diagnostics.Debug.Assert ((special_glyphs == false) || (length == 1));
			
			if ((is_last_run) &&
				(context.EnableHyphenation) &&
				(special_glyphs == false))
			{
				//	Produit la césure manuellement (il faudrait faire mieux pour gérer
				//	correctement des langues comme le suédois ou l'ancien allemand
				//	qui peuvent provoquer le dédoublement de certains caractères) :
				
				ulong[] temp = this.GetHyphenatedText (text, offset, length, font.HyphenChar);
				int     end  = temp.Length - 1;
				extra_glyphs = 1;
				
				attributes[end] = Unicode.BreakAnalyzer.GetStretchClass (temp[end]);
				
				if (BaseEngine.GenerateGlyphs (context, font, temp, 0, length+1, out glyphs, ref attributes))
				{
					throw new System.InvalidOperationException ();
				}
				
				if (context.IsSimpleRenderingDisabled)
				{
					int     num = length+1;
					short[] map = new short[num+1];
					
					for (int i = 0; i < num-1; i++)
					{
						map[i] = (short) i;
					}
					
					map[num-1] = map[num-2];				//	considère que le tiret ne compte pas
					map[num-0] = (short) (map[num-2] + 1);
					
					BaseEngine.GenerateGlyphs (context, font, temp, 0, length+1, out glyphs, ref map);
					
					mapping = new TextToGlyphMapping (text, offset, length, glyphs, map);
				}
			}
			else
			{
				if (BaseEngine.GenerateGlyphs (context, font, text, offset, length, out glyphs, ref attributes))
				{
					throw new System.InvalidOperationException ();
				}
				
				if (context.IsSimpleRenderingDisabled)
				{
					int     num = length;
					short[] map = new short[num+1];
					
					for (int i = 0; i < map.Length; i++)
					{
						map[i] = (short) i;
					}
					
					BaseEngine.GenerateGlyphs (context, font, text, offset, length, out glyphs, ref map);
					
					mapping = new TextToGlyphMapping (text, offset, length, glyphs, map);
				}
			}
			
			if (glue > 0)
			{
				font.PopActiveFeatures ();
			}
			
			//	Détermine les mises à l'échelle des divers glyphes, selon leur
			//	classe d'élasticité :
			
			int n = glyphs.Length;
			
			double[] x_scale = new double[n];
			double[] x_glue  = new double[n];
			double[] x_adj   = new double[n];
			double[] x_pos   = new double[n+1];		//	un élément de plus pour permettre de..
			double[] y_pos   = new double[n];		//	..calculer la largeur du dernier glyphe
			
			int skip_glue_at = n;
			
			if (is_last_run)
			{
				skip_glue_at -= 1;
				
				while ((skip_glue_at > 0) && (glyphs[skip_glue_at] == 0xffff))
				{
					skip_glue_at--;
				}
				
				StretchProfile profile = context.TextStretchProfile;
				
				//	Une ligne qui se termine par un caractère spécial (symbole de
				//	fin de ligne, par exemple) possède une longueur 'length' qui
				//	dépasse le nombre de caractères dénombrés par le profil.
				//
				//	On peut donc utiliser cette différence pour ajuster le point
				//	de fin de 'glue' pour ne pas mettre d'espace supplémentaire
				//	avant le pilcrow (¶).
				
				if (context.ShowControlCharacters)
				{
					skip_glue_at -= offset + length + extra_glyphs - profile.TotalCount;
				}
				else
				{
					skip_glue_at -= profile.CountEndSpace;
				}
				
				if (skip_glue_at < 0)
				{
					skip_glue_at = 0;
				}
			}
			
			this.GenerateXScale (attributes, scales, x_scale, skip_glue_at + 1);
			
			for (int i = 0; i < n; i++)
			{
				if (i == skip_glue_at)
				{
					glue = 0;
				}
				
				y_pos[i]  = oy + font_baseline;
				x_glue[i] = glue;
				x_adj[i]  = font_advance;
			}
			
			//	Détermine la position horizontale de chaque glyphe :
			
			double dx = font.GetPositions (glyphs, font_size, ox, x_pos, x_scale, x_adj, x_glue);
			
			ox      += dx;
			x_pos[n] = x_pos[0] + dx;
			
			//	Demande à ITextRenderer de faire le rendu avec les positions que
			//	nous venons de déterminer :
			
			renderer.Render (context, font, font_size, color, mapping, glyphs, x_pos, y_pos, x_scale, null, is_last_run);
		}
		
		
		private ulong[] GetHyphenatedText(ulong[] text, int offset, int length, ulong hyphen)
		{
			ulong[] copy = new ulong[length+1];
			
			System.Buffer.BlockCopy (text, offset * 8, copy, 0, length * 8);
			
			copy[length] = hyphen;
			
			return copy;
		}
		
		private void FillProfileWithRun(Layout.Context context, ulong[] text, int offset, int length, bool is_last_run, StretchProfile profile)
		{
			OpenType.Font font;
			double        font_size;
			double        font_scale;
			double        font_advance;
			double        font_baseline;
			double        font_advance_offset;
			
			context.TextContext.GetFontAndSize (text[offset], out font, out font_size, out font_scale, out font_advance);
			context.TextContext.GetFontOffsets (text[offset], out font_baseline, out font_advance_offset);
			
			font_size    *= font_scale;
			font_advance += font_advance_offset;
			
			double glue = context.TextStretchGlue;
			
			if ((glue > 0) || (font_advance != 0))
			{
				font.PushActiveFeatures ();
				font.DisableActiveFeatures ("liga", "clig", "hlig", "dlig");
			}
			
			//	Génère les glyphes et les informations relatives à l'extensibilité
			//	pour le fragment de texte :
			
			if ((is_last_run) &&
				(context.EnableHyphenation))
			{
				//	Produit la césure manuellement (il faudrait faire mieux pour gérer
				//	correctement des langues comme le suédois ou l'ancien allemand
				//	qui peuvent provoquer le dédoublement de certains caractères) :
				
				ulong[] temp = this.GetHyphenatedText (text, offset, length, font.HyphenChar);
				
				profile.Add (context, font, font_size, temp, 0, length + 1, font_advance);
			}
			else
			{
				profile.Add (context, font, font_size, text, offset, length, font_advance);
			}
			
			if (glue > 0)
			{
				font.PopActiveFeatures ();
			}
		}
		
		
		private int GenerateXScale(byte[] attributes, StretchProfile.Scales scales, double[] x_scale, int fence)
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
				
				if (i >= fence)
				{
					x_scale[i] = 1.0;
				}
			}
			
			return kashida_count;
		}
		
		
		#region FitScratch Structure
		private struct FitScratch
		{
			public void RecordAscender(double value)
			{
				if (value > this.Ascender)
				{
					this.Ascender = value;
				}
			}
			
			public void RecordDescender(double value)
			{
				if (value < this.Descender)
				{
					this.Descender = value;
				}
			}
			
			public void RecordLineHeight(double value)
			{
				if (value > this.LineHeight)
				{
					this.LineHeight = value;
				}
			}

			
			public int							Offset;
			public double						Advance;
			public StretchProfile				StretchProfile;
			
			public int							LastBreakOffset;
			public double						LastBreakAdvance;
			public double						LastBreakPenalty;
			public StretchProfile				LastStretchProfile;
			
			public BreakMode					BreakMode;
			
			public ulong[]						Text;
			public int							TextStart;
			public int							TextLength;
			public double						TextWidth;
			
			public double						FenceMinX;
			public double						FenceMaxX;
			
			public double						Ascender;
			public double						Descender;
			public double						LineHeight;
			
			public Unicode.BreakInfo			WordBreakInfo;
			
			public int							RunLength;
			
			public OpenType.Font				Font;
			public double						FontSize;
			public double						FontBaseline;
			public double						FontAdvance;
		}
		#endregion
	}
}
