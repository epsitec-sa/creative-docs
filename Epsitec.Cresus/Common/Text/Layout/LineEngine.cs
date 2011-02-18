//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			bool inhibitTabs = false;
			
			if ((context.Disposition != 0) ||
				(context.Justification != 0))
			{
//				inhibitTabs = true;
			}
			
			FitScratch scratch = new FitScratch ();
			
			scratch.Offset    = context.TextOffset;
			scratch.BreakMode = context.BreakMode;
			scratch.FenceMinX = context.LineWidth-context.RightMargin-context.BreakFenceBefore;
			scratch.FenceMaxX = context.LineWidth-context.RightMargin+context.BreakFenceAfter;
			
			scratch.Advance        = context.LineCurrentX;
			scratch.WordBreakInfo  = Unicode.BreakInfo.No;
			scratch.StretchProfile = new StretchProfile ();
			
			bool hasVisibleText = false;
			bool hasHiddenText  = false;
			
			for (;;)
			{
				if (context.GetNextWord (scratch.Offset, out scratch.Text, out scratch.TextLength, out scratch.WordBreakInfo))
				{
					if ((inhibitTabs) &&
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
								
								if (hasVisibleText)
								{
									scratch.WordBreakInfo = Unicode.BreakInfo.No;
								}
							}
							
							hasHiddenText = true;
							
							goto advance_next;
						}
						
						hasVisibleText = true;
						
						Layout.BaseEngine         engine;
						Properties.LayoutProperty layout;
						double fontScale;
						double fontAdvance;
						
						context.TextContext.GetFontAndSize (code, out scratch.Font, out scratch.FontSize, out fontScale, out fontAdvance);
						context.TextContext.GetFontOffsets (code, out scratch.FontBaseline, out scratch.FontAdvance);
						context.TextContext.GetLayoutEngine (code, out engine, out layout);
						
						scratch.FontAdvance += fontAdvance;
						scratch.FontSize    *= fontScale;
						
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
						
						if (hasVisibleText)
						{
							context.RecordAscender (scratch.Ascender);
							context.RecordDescender (scratch.Descender);
							context.RecordLineHeight (System.Math.Max (scratch.LineHeight, (scratch.Ascender - scratch.Descender) * 1.2));
							
							result.Add (new Layout.Break (scratch.Offset, scratch.Advance, 0, 0, scratch.StretchProfile));
						}
						else if (hasHiddenText)
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
					
					double totalWidth   = context.AvailableWidth;
					double spacePenalty = profile.ComputePenalty (totalWidth, context.BreakFenceBefore, context.BreakFenceAfter);
						
					result.Add (new Layout.Break (scratch.LastBreakOffset, scratch.LastBreakAdvance, spacePenalty, scratch.LastBreakPenalty, profile));
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
					
					int runLength = this.GetRunLength (text, offset, length);
					
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
					
					this.RenderRun (context, renderer, ref ox, oy, text, offset, runLength, length == runLength);
					
					//	Avance au morceau suivant :
advance_next:		
					offset += runLength;
					length -= runLength;
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
					
					int runLength = this.GetRunLength (text, offset, length);
					
					Layout.BaseEngine         engine;
					Properties.LayoutProperty layout;
					
					context.TextContext.GetLayoutEngine (text[offset], out engine, out layout);
					
					if ((engine != this) ||
						(layout != context.LayoutProperty))
					{
						throw new System.NotImplementedException ("Engine change not supported in LineEngine.FillProfile.");
					}
					
					this.FillProfileWithRun (context, text, offset, runLength, length == runLength, profile);
					
					//	Avance au morceau suivant :
					
					offset += runLength;
					length -= runLength;
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
			int runLength  = scratch.RunLength;
			int textLength = scratch.TextLength;
			int fragLength = 0;
			
			if ((runLength == 1) &&
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
			
			while (fragLength < runLength)
			{
				double breakPenalty;
				
				bool canBreak = false;
				bool addBreak = false;
				
				if ((scratch.BreakMode == BreakMode.Hyphenate) &&
					(context.EnableHyphenation))
				{
					fragLength = this.GetNextFragmentLength (text, offset, runLength, fragLength, out breakPenalty);
				}
				else if (scratch.BreakMode == BreakMode.Break)
				{
					//	Break catastrophique. Evalue si le résultat est mauvais ou très
					//	mauvais...
					
					int code = Unicode.Bits.GetCode (text[offset+fragLength]);
					
					if (Unicode.DefaultBreakAnalyzer.IsSpace (code))
					{
						breakPenalty = 1*1000.0;
					}
					else if (Unicode.DefaultBreakAnalyzer.IsZeroWidth (code))
					{
						breakPenalty = 10*1000.0;
					}
					else
					{
						breakPenalty = 100*1000.0;
					}
					
					fragLength++;
				}
				else
				{
					fragLength   = runLength;
					breakPenalty = 0;
				}
				
				ulong    hyphen = scratch.Font.HyphenChar;
				ushort[] glyphs;
				byte[]   attr;
				
				bool specialGlyphs = BaseEngine.ContainsSpecialGlyphs (text, offset, fragLength);
				
				System.Diagnostics.Debug.Assert ((specialGlyphs == false) || (fragLength == 1));
				
				if ((fragLength < runLength) &&
					(scratch.BreakMode == BreakMode.Hyphenate) &&
					(specialGlyphs == false))
				{
					//	Produit la césure manuellement (il faudrait faire mieux pour gérer
					//	correctement des langues comme le suédois ou l'ancien allemand
					//	qui peuvent provoquer le dédoublement de certains caractères) :
					
					ulong[] temp = this.GetHyphenatedText (text, offset, fragLength, hyphen);
					
					if (BaseEngine.GenerateGlyphsAndStretchClassAttributes (context.TextContext, scratch.Font, temp, 0, fragLength + 1, out glyphs, out attr))
					{
						throw new System.InvalidOperationException ();
					}
					
					canBreak  = true;
					addBreak  = true;
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
					
					if (BaseEngine.GenerateGlyphsAndStretchClassAttributes (context.TextContext, scratch.Font, text, offset, fragLength, out glyphs, out attr))
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
							
							fragLength = 0;
							continue;
						}
					}
					
					//	Détermine si une coupure de ligne est possible (comme c'est
					//	le cas à la frontière de mots) :
					
					canBreak = (runLength == textLength) || (scratch.BreakMode == BreakMode.Break);
					addBreak = (scratch.BreakMode != BreakMode.Default) && (scratch.WordBreakInfo == Unicode.BreakInfo.Optional);
					
					profile.Add (scratch.Font, scratch.FontSize, glyphs, attr, scratch.FontAdvance);
				}
				
				if (scratch.Advance+scratch.TextWidth-profile.WidthEndSpace > scratch.FenceMaxX)
				{
					//	Dépassé la marge de droite. Arrête immédiatement sans tenir compte
					//	du résultat :
					
					return true;
				}
				
				if (canBreak)
				{
					context.RecordAscender (scratch.Ascender);
					context.RecordDescender (scratch.Descender);
					context.RecordLineHeight (System.Math.Max (scratch.LineHeight, (scratch.Ascender - scratch.Descender) * 1.2));
					
					//	TODO: il faudrait enregistrer les hauteurs de la ligne avec l'information
					//	Layout.Break si on voulait faire les choses correctement ici !
					
					scratch.LastBreakOffset    = scratch.Offset + fragLength;
					scratch.LastBreakAdvance   = scratch.Advance + scratch.TextWidth - profile.WidthEndSpace;
					scratch.LastBreakPenalty   = breakPenalty;
					scratch.LastStretchProfile = new StretchProfile (profile);
					
					if (addBreak)
					{
						double totalWidth   = context.AvailableWidth;
						double spacePenalty = profile.ComputePenalty (totalWidth, context.BreakFenceBefore, context.BreakFenceAfter);
						
						result.Add (new Layout.Break (scratch.LastBreakOffset, scratch.LastBreakAdvance, spacePenalty, breakPenalty, profile));
					}
				}
			}
			
			return false;
		}

		private void RenderRun(Layout.Context context, ITextRenderer renderer, ref double ox, double oy, ulong[] text, int offset, int length, bool isLastRun)
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
				
				renderer.Render (context, image, color, ox, oy, isLastRun);
				
				ox += advance;
				
				return;
			}
			
			
			//	Détermine la fonte qu'il faudra utiliser pour le fragment de texte
			//	dont il faut faire le rendu :
			
			OpenType.Font font;
			double        fontSize;
			double        fontBaseline;
			double        fontAdvance;
			double        fontScale;
			double        fontAdvanceOffset;
			
			context.TextContext.GetFontAndSize (text[offset], out font, out fontSize, out fontScale, out fontAdvance);
			context.TextContext.GetFontOffsets (text[offset], out fontBaseline, out fontAdvanceOffset);
			
			fontSize    *= fontScale;
			fontAdvance += fontAdvanceOffset;
			
			//	Si l'appelant a désactivé le décalage vertical de la ligne de base
			//	(par ex. pour déterminer la position du curseur), on en tient compte
			//	ici :
			
			if (context.IsFontBaselineOffsetDisabled)
			{
				fontBaseline = 0;
			}
			
			//	Gérer l'étirement des glyphes en fonction de la fonte sélectionnée :
			
			StretchProfile.Scales scales = context.TextStretchScales;
			double                glue   = context.TextStretchGlue;
			
			if ((glue > 0) || (fontAdvance != 0))
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
			
			bool specialGlyphs = BaseEngine.ContainsSpecialGlyphs (text, offset, length);
			int  extraGlyphs   = 0;
			
			System.Diagnostics.Debug.Assert ((specialGlyphs == false) || (length == 1));
			
			if ((isLastRun) &&
				(context.EnableHyphenation) &&
				(specialGlyphs == false))
			{
				//	Produit la césure manuellement (il faudrait faire mieux pour gérer
				//	correctement des langues comme le suédois ou l'ancien allemand
				//	qui peuvent provoquer le dédoublement de certains caractères) :
				
				ulong[] temp = this.GetHyphenatedText (text, offset, length, font.HyphenChar);
				int     end  = temp.Length - 1;
				extraGlyphs = 1;
				
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
			
			double[] xScale = new double[n];
			double[] xGlue  = new double[n];
			double[] xAdj   = new double[n];
			double[] xPos   = new double[n+1];		//	un élément de plus pour permettre de..
			double[] yPos   = new double[n];		//	..calculer la largeur du dernier glyphe
			
			int skipGlueAt = n;
			
			if (isLastRun)
			{
				skipGlueAt -= 1;
				
				while ((skipGlueAt > 0) && (glyphs[skipGlueAt] == 0xffff))
				{
					skipGlueAt--;
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
					skipGlueAt -= offset + length + extraGlyphs - profile.TotalCount;
				}
				else
				{
					skipGlueAt -= profile.CountEndSpace;
				}
				
				if (skipGlueAt < 0)
				{
					skipGlueAt = 0;
				}
			}
			
			this.GenerateXScale (attributes, scales, xScale, skipGlueAt + 1);
			
			for (int i = 0; i < n; i++)
			{
				if (i == skipGlueAt)
				{
					glue = 0;
				}
				
				yPos[i]  = oy + fontBaseline;
				xGlue[i] = glue;
				xAdj[i]  = fontAdvance;
			}
			
			//	Détermine la position horizontale de chaque glyphe :
			
			double dx = font.GetPositions (glyphs, fontSize, ox, xPos, xScale, xAdj, xGlue);
			
			ox      += dx;
			xPos[n] = xPos[0] + dx;
			
			//	Demande à ITextRenderer de faire le rendu avec les positions que
			//	nous venons de déterminer :
			
			renderer.Render (context, font, fontSize, color, mapping, glyphs, xPos, yPos, xScale, null, isLastRun);
		}
		
		
		private ulong[] GetHyphenatedText(ulong[] text, int offset, int length, ulong hyphen)
		{
			ulong[] copy = new ulong[length+1];
			
			System.Buffer.BlockCopy (text, offset * 8, copy, 0, length * 8);
			
			copy[length] = hyphen;
			
			return copy;
		}
		
		private void FillProfileWithRun(Layout.Context context, ulong[] text, int offset, int length, bool isLastRun, StretchProfile profile)
		{
			OpenType.Font font;
			double        fontSize;
			double        fontScale;
			double        fontAdvance;
			double        fontBaseline;
			double        fontAdvanceOffset;
			
			context.TextContext.GetFontAndSize (text[offset], out font, out fontSize, out fontScale, out fontAdvance);
			context.TextContext.GetFontOffsets (text[offset], out fontBaseline, out fontAdvanceOffset);
			
			fontSize    *= fontScale;
			fontAdvance += fontAdvanceOffset;
			
			double glue = context.TextStretchGlue;
			
			if ((glue > 0) || (fontAdvance != 0))
			{
				font.PushActiveFeatures ();
				font.DisableActiveFeatures ("liga", "clig", "hlig", "dlig");
			}
			
			//	Génère les glyphes et les informations relatives à l'extensibilité
			//	pour le fragment de texte :
			
			if ((isLastRun) &&
				(context.EnableHyphenation))
			{
				//	Produit la césure manuellement (il faudrait faire mieux pour gérer
				//	correctement des langues comme le suédois ou l'ancien allemand
				//	qui peuvent provoquer le dédoublement de certains caractères) :
				
				ulong[] temp = this.GetHyphenatedText (text, offset, length, font.HyphenChar);
				
				profile.Add (context, font, fontSize, temp, 0, length + 1, fontAdvance);
			}
			else
			{
				profile.Add (context, font, fontSize, text, offset, length, fontAdvance);
			}
			
			if (glue > 0)
			{
				font.PopActiveFeatures ();
			}
		}
		
		
		private int GenerateXScale(byte[] attributes, StretchProfile.Scales scales, double[] xScale, int fence)
		{
			int kashidaCount = 0;
			
			for (int i = 0; i < xScale.Length; i++)
			{
				Unicode.StretchClass stretch = (Unicode.StretchClass) attributes[i];
				
				switch (stretch)
				{
					case Unicode.StretchClass.NoStretch:		xScale[i] = scales.ScaleNoStretch;							break;
					case Unicode.StretchClass.Character:		xScale[i] = scales.ScaleCharacter;							break;
					case Unicode.StretchClass.CharacterSpace:	xScale[i] = scales.ScaleCharacter;							break;
					case Unicode.StretchClass.Space:			xScale[i] = scales.ScaleSpace;								break;
					case Unicode.StretchClass.Kashida:			xScale[i] = scales.ScaleKashida;		kashidaCount++;	break;
				}
				
				if (i >= fence)
				{
					xScale[i] = 1.0;
				}
			}
			
			return kashidaCount;
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
