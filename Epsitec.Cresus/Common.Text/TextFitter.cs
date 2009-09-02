//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextFitter s'occupe de la mise en page du texte au
	/// moyen des méthodes ClearAllMarks/GenerateAllMarks et du rendu
	/// du texte grâce aux méthodes RenderXxx.
	/// </summary>
	public sealed class TextFitter
	{
		public TextFitter(TextStory story)
		{
			this.TextStory = story;
			
			this.frameList      = new FrameList (this);
			this.pageCollection = new DefaultPageCollection ();
			
			this.geometryCacheVersion = new TextVersion (this);
		}
		
		
		public long								Version
		{
			get
			{
				return this.version;
			}
		}
		
		public TextStory						TextStory
		{
			get
			{
				return this.story;
			}
			set
			{
				if (this.story != value)
				{
					this.story        = value;
					this.cursors      = new System.Collections.ArrayList ();
					this.freeCursors = new System.Collections.Stack ();
				}
			}
		}
		
		public int								CursorCount
		{
			get
			{
				return this.cursors.Count - this.freeCursors.Count;
			}
		}
		
		
		public FrameList						FrameList
		{
			get
			{
				return this.frameList;
			}
		}
		
		public IPageCollection					PageCollection
		{
			get
			{
				return this.pageCollection;
			}
			set
			{
				if (this.pageCollection != value)
				{
					this.pageCollection = value;
				}
			}
		}


		public bool CheckNeedsMoreRoomForFullFit()
		{
			Cursors.FitterCursor cursor = this.frameList.FindLastCursor (-1);

			if (cursor != null)
			{
				cursor = this.frameList.FindLastCursor (this.frameList.Count - 1);
				
				if (cursor != null)
				{
					int len = 0;

					foreach (var element in cursor.Elements)
					{
						if (element.FrameIndex == -1)
						{
							break;
						}
						len += element.Length;
					}

					string text = this.story.GetDebugText ().Substring (len);

					foreach (char c in text)
					{
						if (!char.IsWhiteSpace (c) &&
							(c != (char) Unicode.Code.EndOfText))
						{
							return true;
						}
					}

					return false;
				}

				return true;
			}
			else
			{
				cursor = this.frameList.FindLastCursor (this.frameList.Count - 1);
				return cursor == null ? false : cursor.NeedsMoreRoom;
			}
		}
		
		public void ClearAllMarks()
		{
			if (this.story.TextLength == 0)
			{
				TextProcessor.Status status;
				int                  length = 0;
				
				this.ExecuteClear (null, 0, ref length, out status);
			}
			else
			{
				TextProcessor processor = new TextProcessor (this.story);
				processor.Process (new TextProcessor.Executor (this.ExecuteClear));
			}
			
			System.Diagnostics.Debug.Assert (this.CursorCount == 0);
			
			this.frameList.ClearCursorMap ();
			this.Invalidate ();
		}
		
		public void GenerateAllMarks()
		{
			this.frameIndex = 0;
			this.frameY     = 0;
			
			this.lineSkipBefore = 0;
			this.frameFences.Clear ();
			this.keepWithPrev   = false;
			
			if (this.frameList.Count > 0)
			{
				TextProcessor processor = new TextProcessor (this.story);
				processor.Process (new TextProcessor.Executor (this.ExecuteGenerate));
			}
			
			this.story.ClearTextChangeMarkPositions ();
			this.frameList.ClearCursorMap ();
			this.Invalidate ();
		}
		
		public void GenerateMarks()
		{
			int start = this.story.TextChangeMarkStart;
			int end   = this.story.TextChangeMarkEnd;
			
			if ((start == 0) &&
				(end == this.story.TextLength))
			{
				this.ClearAllMarks ();
				this.GenerateAllMarks ();
			}
			else if (start > end)
			{
				//	Il n'y a rien à faire, car le texte n'a pas changé de manière
				//	significative depuis la dernière fois.
			}
			else
			{
				Internal.TextTable text   = this.story.TextTable;
				CursorInfo.Filter  filter = Cursors.FitterCursor.GetFitterFilter (this);
				CursorInfo[]       infos  = text.FindCursorsBefore (start, filter);
				
				if (infos.Length == 0)
				{
					this.ClearAllMarks ();
					this.GenerateAllMarks ();
					
					return;
				}
				
				Cursors.FitterCursor           fitterCursor = text.GetCursorInstance (infos[0].CursorId) as Cursors.FitterCursor;
				Cursors.FitterCursor.Element[] elements      = fitterCursor.Elements;
				
#if false
				System.Diagnostics.Debug.WriteLine (string.Format ("Optimized GenerateMarks: {0}/{1}, Cursor[0].pos={2} (n={3})",
					/**/										   start, end,
					/**/										   this.story.GetCursorPosition (fitterCursor),
					/**/										   infos.Length));
#endif
				
				int n = elements.Length - 1;
				
				this.frameFences.Clear ();
				this.lineSkipBefore = fitterCursor.SpaceAfterParagraph;
				this.keepWithPrev   = fitterCursor.KeepWithNextParagraph;
				this.frameIndex      = elements[n].FrameIndex;
				this.frameY          = fitterCursor.ParagraphNextY;
				
				System.Diagnostics.Debug.Assert (this.story.TextLength > 0);
				
				TextProcessor processor = new TextProcessor (this.story);
				processor.DefineStartFence (start);
				processor.Process (new TextProcessor.Executor (this.ExecuteClear));
				
				try
				{
					if (this.frameList.Count > 0)
					{
						processor = new TextProcessor (this.story);
						processor.DefineStartFence (start);
						processor.Process (new TextProcessor.Executor (this.ExecuteGenerate));
					}
				}
				catch (System.InvalidOperationException)
				{
					this.ClearAllMarks ();
					this.GenerateAllMarks ();
					
					return;
				}
				
				this.frameList.ClearCursorMap ();
				this.story.ClearTextChangeMarkPositions ();
				this.Invalidate ();
			}
		}
		
		public void RenderParagraph(ICursor cursor, ITextRenderer renderer)
		{
			this.RenderParagraphInTextFrame (cursor, renderer, null);
		}
		
		public void RenderParagraphInTextFrame(ICursor fitterCursor, ITextRenderer renderer, ITextFrame frame)
		{
			System.Diagnostics.Debug.Assert (this.story.HasTextChangeMarks == false);
			
			Cursors.FitterCursor cursor = fitterCursor as Cursors.FitterCursor;
			
			if (cursor == null)
			{
				throw new System.ArgumentException ("Not a valid FitterCursor.", "fitter_cursor");
			}
			
			int     length = cursor.ParagraphLength;
			ulong[] text   = new ulong[length];
			
			this.story.ReadText (cursor, length, text);
			
			Layout.Context                 layout   = new Layout.Context (this.story.TextContext, text, 0, this.frameList);
			Cursors.FitterCursor.Element[] elements = cursor.Elements;
			
			layout.DefineShowControlCharacters ();
			
			renderer.RenderStartParagraph (layout);
			
			for (int i = 0; i < elements.Length; i++)
			{
				this.RenderElement (renderer, frame, cursor, elements, i, layout, cursor.SpaceAfterParagraph);
			}
			
			renderer.RenderEndParagraph (layout);
		}
		
		public void RenderTextFrame(ITextFrame frame, ITextRenderer renderer)
		{
			this.GenerateMarksIfNeeded ();
			
			int index = this.frameList.IndexOf (frame);
			
			if (index < 0)
			{
				throw new System.ArgumentException ("Not a valid ITextFrame.", "frame");
			}
			
			Cursors.FitterCursor cursor = this.frameList.FindFirstCursor (index);
			Internal.TextTable   text   = this.story.TextTable;
			
			while (cursor != null)
			{
				if (cursor.ContainsFrameIndex (index) == false)
				{
					break;
				}
				
				this.RenderParagraphInTextFrame (cursor, renderer, frame);
				
				//	Trouve le curseur du paragraphe suivant :
				
				CursorInfo[] cursors = text.FindNextCursor (cursor.CursorId, Cursors.FitterCursor.Filter);
				
				if (cursors.Length == 1)
				{
					cursor = text.GetCursorInstance (cursors[0].CursorId) as Cursors.FitterCursor;
				}
				else
				{
					break;
				}
			}
		}
		
		
		public bool HitTestTextFrame(ITextFrame frame, double x, double y, bool skipInvisible, ref int position, ref int direction)
		{
			this.GenerateMarksIfNeeded ();
			
			int index = this.frameList.IndexOf (frame);
			
			if (index < 0)
			{
				throw new System.ArgumentException ("Not a valid ITextFrame.", "frame");
			}
			
			bool acceptOutOfLine = false;
			
			for (;;)
			{
				Cursors.FitterCursor cursor = this.frameList.FindFirstCursor (index);
				Internal.TextTable   text   = this.story.TextTable;
				
				while (cursor != null)
				{
					if (cursor.ContainsFrameIndex (index) == false)
					{
						break;
					}
					
					if (this.HitTestParagraphInTextFrame (cursor, frame, x, y, skipInvisible, acceptOutOfLine, ref position, ref direction))
					{
						return true;
					}
					
					//	Trouve le curseur du paragraphe suivant :
					
					CursorInfo[] cursors = text.FindNextCursor (cursor.CursorId, Cursors.FitterCursor.Filter);
					
					if (cursors.Length == 1)
					{
						cursor = text.GetCursorInstance (cursors[0].CursorId) as Cursors.FitterCursor;
					}
					else
					{
						break;
					}
				}
				
				if (acceptOutOfLine)
				{
					break;
				}
				
				acceptOutOfLine = true;
			}
			
			return false;
		}
		
		public bool HitTestParagraphInTextFrame(ICursor fitterCursor, ITextFrame frame, double x, double y, bool skipInvisible, bool acceptOutOfLine, ref int position, ref int direction)
		{
			//	Détermine où se trouve le curseur dans le frame spécifié. La recherche
			//	se base sur les positions des lignes, dans un premier temps, puis sur
			//	les informations détaillées de position au sein de la ligne.
			
			this.GenerateMarksIfNeeded ();
			
			Cursors.FitterCursor cursor = fitterCursor as Cursors.FitterCursor;
			
			if (cursor == null)
			{
				throw new System.ArgumentException ("Not a valid FitterCursor.", "fitter_cursor");
			}
			
			Cursors.FitterCursor.Element[] elements = cursor.Elements;
			
			int paraStart  = this.story.GetCursorPosition (fitterCursor);
			int lineOffset = 0;
			
			double viewX = x;
			double viewY = y;
			
			//	Convertit les coordonnées [x,y] en coordonnées internes, pour la
			//	première recherche approximative. Les recherches détaillées basées
			//	sur GetCursorGeometry se basent ensuite sur des coordonnées [view]
			//	standard.
			
			frame.MapFromView (ref x, ref y);
			
			double dyAfter = cursor.SpaceAfterParagraph;
			
			for (int i = 0; i < elements.Length; i++)
			{
				int frameIndex = elements[i].FrameIndex;
				
				//	N'analyse que les éléments qui correspondent au frame qui nous
				//	intéresse :
				
				if ((frameIndex < 0) ||
					(frameIndex >= this.frameList.Count) ||
					(this.frameList[frameIndex] != frame))
				{
					lineOffset += elements[i].Length;
					continue;
				}
				
				double lYb = elements[i].LineBaseY;
				double lY1 = System.Math.Min (elements[i].LineY1 - dyAfter, lYb + elements[i].LineDescender);
				double lY2 = System.Math.Max (elements[i].LineY2, lYb + elements[i].LineAscender);

				if (y <= lY2)
				{
					position  = paraStart + lineOffset + elements[i].Length;
					direction = 1;
				}
				
				if ((y >= lY1) &&
					(y <= lY2))
				{
					//	La bande horizontale qui comprend cette ligne se trouve dans la
					//	zone d'intérêt. Analysons-la plus à fond :
					
					double cx1 = 0;
					double cy  = 0;
					double cy1 = 0;
					double cy2 = 0;
					
					this.GetCursorGeometry (frame, paraStart + lineOffset, 0, cursor, elements, i, ref cx1, ref cy, ref cy1, ref cy2, dyAfter);
					
					if ((viewY >= cy1) &&
						(viewY <= cy2))
					{
						//	Verticalement, le curseur est bien dans la ligne, en tenant
						//	compte de toutes les finesses (interligne, espace avant et
						//	après, etc.)
						
						if (viewX < cx1)
						{
							//	On se trouve trop à gauche --> début de ligne.
							
							position  = paraStart + lineOffset;
							direction = -1;
							
							if ((acceptOutOfLine) &&
								(elements[i].IsTabulation == false))
							{
								return true;
							}
							
							break;
						}
						else
						{
							int lastValidOffset = 0;
							
							//	Passe toute la ligne au crible fin, caractère par carac-
							//	tère, jusqu'à trouver celui qui est sous le curseur :
							
							for (int glyphOffset = 1; glyphOffset <= elements[i].Length; glyphOffset++)
							{
								int    pos = paraStart + lineOffset;
								double cx2 = cx1;
								
								this.GetCursorGeometry (frame, pos, glyphOffset, cursor, elements, i, ref cx2, ref cy, ref cy1, ref cy2, dyAfter);
								
								if ((viewX >= cx1) &&
									(viewX <= cx2))
								{
									//	Le clic a été fait dans le corps de la ligne.
									
									//	Cas particulier : si la ligne est vide (juste un caractère
									//	de fin de ligne), cx1 == cx2 et il faut générer une position
									//	telle que le curseur résultant sera placé au début de la
									//	ligne (position avant le saut de ligne, direction = 1).
									
									int adjust = (2*viewX <= (cx1 + cx2)) ? -1 : 0;
									
									position  = paraStart + lineOffset + glyphOffset + adjust;
									direction = ((glyphOffset + adjust > 0) || (position == 0) || (cx1 == cx2)) ? 1 : -1;
									
									return true;
								}
								
								if ((cx2 > cx1) ||
									(skipInvisible))
								{
									lastValidOffset = glyphOffset;
								}
								
								cx1 = cx2;
							}
							
							//	On se trouve trop à droite --> fin de ligne.
							
							if (viewX > cx1)
							{
								position  = paraStart + lineOffset + lastValidOffset;
								direction = 1;
								
								if ((acceptOutOfLine) &&
									(elements[i].IsTabulation == false))
								{
									return true;
								}
							}
						}
					}
				}
				
				lineOffset += elements[i].Length;
			}
			
			return false;
		}
		
		
		public bool GetCursorGeometry(ICursor cursor, out ITextFrame frame, out double x, out double y, out int paragraphLine, out int lineCharacter)
		{
			//	Détermine où se trouve le curseur spécifié : frame, position [x;y],
			//	numéro de ligne dans le paragraphe et numéro de caractère dans la
			//	ligne.
			
			this.GenerateMarksIfNeeded ();
			
			//	NB: La position verticale est toujours alignée sur la ligne de base
			//		de la fonte, sans tenir compte d'un éventuel offset vertical.
			
			int position  = this.story.GetCursorPosition (cursor);
			int direction = this.story.GetCursorDirection (cursor);
			
			Internal.TextTable text   = this.story.TextTable;
			CursorInfo.Filter  filter = Cursors.FitterCursor.GetFitterFilter (this);
			CursorInfo[]       infos  = text.FindCursorsBefore (position + 1, filter);
			
			if (infos.Length > 0)
			{
				System.Diagnostics.Debug.Assert (infos.Length == 1);
				
				Cursors.FitterCursor fitterCursor = text.GetCursorInstance (infos[0].CursorId) as Cursors.FitterCursor;
				
				int paragraphStart = text.GetCursorPosition (infos[0].CursorId);
				int lineStart      = paragraphStart;
				int tabCount       = 0;
				int tabCharCount  = 0;
				
				if (paragraphStart + fitterCursor.ParagraphLength < position)
				{
					//	Le curseur se trouve dans une tranche de texte qui n'appartient
					//	à aucun paragraphe.
				}
				else
				{
					Cursors.FitterCursor.Element[] elements = fitterCursor.Elements;
					
					//	Détermine dans quelle ligne du paragraphe se trouve le curseur :
					
					for (int i = 0; i < elements.Length; i++)
					{
						int lineLength = elements[i].Length;
						int lineEnd    = lineStart + lineLength;
						int frameIndex = elements[i].FrameIndex;
						
						if (frameIndex < 0)
						{
							break;
						}
						
						if ((position >= lineStart) &&
							(  (position < lineEnd)
							|| ((position == lineEnd) && (direction >= 0) && (! elements[i].IsTabulation) && (! Internal.Navigator.IsAfterLineBreak (this.story, fitterCursor, position - paragraphStart)))
							|| ((position == lineEnd) && (lineLength == 0))
							|| ((position == lineEnd) && (lineEnd == this.story.TextLength)) ))
						{
							//	Le curseur se trouve dans la ligne en cours d'analyse.
							//	On tient compte de la direction de déplacement pour
							//	déterminer si le curseur se trouve à la fin de la ligne
							//	en cours ou au début de la ligne suivante.
							
							frame = this.FrameList[frameIndex];
							
							x = elements[i].LineStartX;
							y = elements[i].LineBaseY;
							
							paragraphLine = i - tabCount;
							lineCharacter = position - lineStart + tabCharCount;
							
							double y1 = 0;
							double y2 = 0;
							
							this.GetCursorGeometry (lineStart, position - lineStart, fitterCursor, elements, i, ref x, ref y, ref y1, ref y2, fitterCursor.SpaceAfterParagraph);
							
							return true;
						}
						
						if (elements[i].IsTabulation)
						{
							tabCharCount += lineLength;
							tabCount++;
						}
						else
						{
							tabCharCount = 0;
						}
						
						lineStart = lineEnd;
						
						if (lineStart > position)
						{
							break;
						}
					}
				}
			}
			
			frame = null;
			
			x = 0;
			y = 0;
			
			paragraphLine = 0;
			lineCharacter = 0;
			
			return false;
		}
		
		
		public void GetStatistics(out int paragraphCount, out int lineCount)
		{
			paragraphCount = 0;
			lineCount      = 0;
			
			CursorInfo[] infos = this.story.TextTable.FindCursors (0, this.story.TextLength, Cursors.FitterCursor.Filter);
			
			for (int i = 0; i < infos.Length; i++)
			{
				Cursors.FitterCursor cursor = this.story.TextTable.GetCursorInstance (infos[i].CursorId) as Cursors.FitterCursor;
				
				paragraphCount += 1;
				lineCount      += cursor.LineCount;
			}
		}
		
		
		private void Invalidate()
		{
			this.version++;
			this.geometryCacheRenderer = null;
		}
		
		private void ExecuteClear(Cursors.TempCursor tempCursor, int pos, ref int length, out TextProcessor.Status status)
		{
			//	Supprime les marques de découpe de lignes représentées par des
			//	curseurs (instances de Cursors.FitterCursor).
			
			CursorInfo[] cursors = this.story.TextTable.FindCursors (pos, length, Cursors.FitterCursor.Filter);
			
			for (int i = 0; i < cursors.Length; i++)
			{
				ICursor cursor = this.story.TextTable.GetCursorInstance (cursors[i].CursorId);
				this.RecycleFitterCursor (cursor);
			}
			
			status = TextProcessor.Status.Continue;
		}
		
		private void ExecuteGenerate(Cursors.TempCursor cursor, int pos, ref int length, out TextProcessor.Status status)
		{
			//	Génère les marques de découpe de lignes et insère les curseurs
			//	correspondants.
			
			ulong[] text;
			
			if (pos + length < story.TextLength)
			{
				text = new ulong[length];
				this.story.ReadText (cursor, length, text);
			}
			else
			{
				//	On arrive au bout du texte: il faut donc synthétiser un caractère
				//	supplémentaire de fin de texte pour que l'algorithme de layout
				//	soit satisfait :
				
				text = new ulong[length+1];
				this.story.ReadText (cursor, length, text);
				
				ulong code = text[length-1];
				
				code &= 0xffffffff00000000ul;
				code |= (int) Unicode.Code.EndOfText;
				
				Unicode.Bits.SetBreakInfo (ref code, Unicode.BreakInfo.Yes);
				
				text[length] = code;
			}
			
			Layout.Context         layout = new Layout.Context (this.story.TextContext, text, 0, this.frameList);
			Layout.BreakCollection result = new Layout.BreakCollection ();
			
			layout.SelectFrame (this.frameIndex, this.frameY);
			
			layout.DefineLineSkipBefore (this.lineSkipBefore);
			layout.DefineKeepWithPreviousParagraph (this.keepWithPrev);
			layout.DefineFrameFences (this.frameFences);
			
			int    paragraphStartOffset      = 0;
			int    paragraphStartFrameIndex  = layout.FrameIndex;;
			double paragraphStartFrameY      = layout.FrameY;
			
restart_paragraph_layout:
			
			layout.DefineTabIndentation (false, 0);
			
			int lineCount        = 0;
			int lineStartOffset  = paragraphStartOffset;
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			bool continuation = false;
			bool resetLineH   = true;
			
			int defLineCount        = lineCount;
			int defLineStartOffset  = lineStartOffset;
			int defListCount        = list.Count;
			
			int    defFrameIndex = layout.FrameIndex;
			double defFrameY     = layout.FrameY;
			
			for (;;)
			{
				if (resetLineH)
				{
					layout.ResetLineHeight ();
					resetLineH = false;
				}
				
				Properties.TabProperty     tabProperty;
				Properties.MarginsProperty marginsProperty;
				TextFitter.TabStatus       tabStatus;
				
				double fontSizeInPoints;
				
				Layout.Status layoutStatus = layout.Fit (ref result, lineCount, continuation);
				
				bool tabNewLine = false;
				bool endOfText  = false;
				
				this.frameIndex = layout.FrameIndex;
				this.frameY     = layout.FrameY;
				
				switch (layoutStatus)
				{
					case Layout.Status.ErrorNeedMoreText:
						this.frameIndex = paragraphStartFrameIndex;
						this.frameY     = paragraphStartFrameY;
						length = paragraphStartOffset;
						status = TextProcessor.Status.Continue;
						return;
					
					case Layout.Status.ErrorCannotFit:
						System.Diagnostics.Debug.WriteLine ("Cannot fit, throwing an exception.");
						throw new System.InvalidOperationException ("Cannot fit.");
					
					case Layout.Status.Ok:
						continuation = false;
						resetLineH = true;
						break;
					
					case Layout.Status.OkFitEnded:
						
						continuation = false;
						resetLineH = true;
						
						this.lineSkipBefore = layout.LineSpaceAfter;
						this.keepWithPrev   = layout.KeepWithNextParagraph;
						this.frameFences.Clear ();
						
						layout.DefineLineSkipBefore (this.lineSkipBefore);
						layout.DefineKeepWithPreviousParagraph (this.keepWithPrev);
						layout.DefineTabIndentation (false, 0);
						
						break;
					
					case Layout.Status.OkHiddenFitEnded:
						
						continuation = false;
						resetLineH = true;
						
						break;
					
					case Layout.Status.RestartParagraphLayout:
						
						if (layout.FrameFences.Count > 0)
						{
							//	Il faut se replacer dans le frame correspondant au début
							//	du paragraphe en cours :
							
							layout.SelectFrame (paragraphStartFrameIndex, paragraphStartFrameY);
						}
						
						layout.MoveTo (0, paragraphStartOffset);
						goto restart_paragraph_layout;
					
					case Layout.Status.RewindParagraphAndRestartLayout:
						
						//	Retourne en arrière, jusqu'au début du paragraphe qui précède
						//	le paragraphe actuel, puis relance l'opération au complet.
						
						length  = this.RewindToPreviousParagraph (cursor, pos, paragraphStartOffset);
						status  = TextProcessor.Status.Continue;
						
						return;
					
					case Layout.Status.RestartLineLayout:
						
						Debug.Assert.IsTrue (continuation);
						
						continuation      = false;
						lineCount        = defLineCount;
						lineStartOffset = defLineStartOffset;
						
						layout.MoveTo (0, lineStartOffset);
						layout.SelectFrame (defFrameIndex, defFrameY);
						
						list.RemoveRange (defListCount, list.Count - defListCount);
						
						continue;
					
					case Layout.Status.OkTabReached:
						
						//	On vient de trouver une marque de tabulation. Il faut
						//	l'analyser et déterminer si elle peut être placée sur
						//	la même ligne que le texte qui précède :
						
						System.Diagnostics.Debug.Assert (layout.TextOffset >= 0);
						System.Diagnostics.Debug.Assert (layout.TextOffset <= text.Length);
						
						//	Si le système de layout change de frame en cours de route,
						//	cela implique que la ligne était vide et que l'on doit con-
						//	sidérer le début de frame comme notre nouvelle référence.
						
						if (defFrameIndex != this.frameIndex)
						{
							defFrameIndex = this.frameIndex;
							defFrameY     = 0;
						}
						
						defFrameIndex = this.frameIndex;
						defFrameY     = layout.FrameYLine;
						
						layout.TextContext.GetTab (text[layout.TextOffset-1], out tabProperty);
						layout.TextContext.GetMargins (text[layout.TextOffset-1], out marginsProperty);
						layout.TextContext.GetFontSize (text[layout.TextOffset-1], out fontSizeInPoints);
						
						System.Diagnostics.Debug.Assert (tabProperty != null);
						
						double tabX;
						double tabDx;
						bool   tabAtLineStart = (!continuation) && (layout.TextOffset == lineStartOffset+1);
						bool   tabIndents;
						
						tabStatus = this.MeasureTabTextWidth (layout, tabProperty, marginsProperty, fontSizeInPoints, lineCount, tabAtLineStart, out tabX, out tabDx, out tabIndents);
						
						if (tabStatus == TabStatus.ErrorNeedMoreText)
						{
							length = paragraphStartOffset;
							status = TextProcessor.Status.Continue;
							return;
						}
						else if ((tabStatus == TabStatus.ErrorNeedMoreRoom) ||
							/**/ (tabStatus == TabStatus.ErrorCannotFit))
						{
							//	Le tabulateur ne tient plus sur cette ligne. Force un passage
							//	à la ligne.
							
							tabNewLine = true;
							tabStatus   = this.MeasureTabTextWidth (layout, tabProperty, marginsProperty, fontSizeInPoints, lineCount, true, out tabX, out tabDx, out tabIndents);
							
							if (tabStatus == TabStatus.ErrorNeedMoreRoom)
							{
								//	Même sur une ligne nouvelle, il n'y a pas la place pour
								//	positionner le texte selon les besoins du tabulateur;
								//	on le cale simplement sur la marge :
								
								layout.MoveTo (layout.LineOriginX, layout.TextOffset);
							}
							else
							{
								layout.MoveTo (tabX, layout.TextOffset);
								layout.DefineTabIndentation (tabIndents, tabX);
							}
						}
						else if (tabStatus == TabStatus.Ok)
						{
							//	Le tabulateur occupe la même ligne que le texte qui précède.
							
							layout.MoveTo (tabX, layout.TextOffset);
							layout.DefineTabIndentation (tabIndents, tabX);
						}
						else
						{
							throw new System.NotImplementedException (string.Format ("TabStatus.{0}", tabStatus));
						}
						
						continuation = true;
						
						break;
					
					case Layout.Status.ErrorNeedMoreRoom:
						endOfText = true;
						break;
					
					default:
						throw new System.InvalidOperationException (string.Format ("Invalid layout status received: {0}.", layoutStatus));
				}
				
				//	Le système de layout propose un certain nombre de points de découpe
				//	possibles pour la ligne. Il faut maintenant déterminer lequel est le
				//	meilleur.
				
				int offset   = lineStartOffset;
				int nBreaks = result.Count;
				
				Layout.StretchProfile profile = null;
				
				if (nBreaks > 1)
				{
					double penalty = Layout.StretchProfile.MaxPenalty;
					double advance = -1;
					int    pIndex = -1;
					
					for (int i = 0; i < nBreaks; i++)
					{
						double computedPenalty = this.ComputePenalty (result[i].SpacePenalty, result[i].BreakPenalty);
						
						if (computedPenalty < penalty)
						{
							penalty = computedPenalty;
							pIndex = i;
						}
					}
					
					if (pIndex < 0)
					{
						pIndex = 0;
						
						for (int i = 0; i < nBreaks; i++)
						{
							if (result[i].Advance > advance)
							{
								advance = result[i].Advance;
								pIndex = i;
							}
						}
					}
					
					offset  = result[pIndex].Offset;
					profile = result[pIndex].Profile;
				}
				else if (nBreaks == 1)
				{
					offset  = result[0].Offset;
					profile = result[0].Profile;
				}
				
				if (nBreaks > 0)
				{
					Cursors.FitterCursor.Element element = new Cursors.FitterCursor.Element ();
					
					if (pos + offset > story.TextLength)
					{
						Debug.Assert.IsTrue (layoutStatus == Layout.Status.OkFitEnded);
						
						offset     -= 1;
						endOfText = true;
					}
					
					//	Génère un élément décrivant la ligne (ou le morceau de ligne
					//	qui précède un tabulateur) :
					
					element.Length        = offset - lineStartOffset;
					element.Profile       = profile;
					element.FrameIndex    = layout.FrameIndex;
					element.LineStartX    = layout.LineStartX;
					element.LineOriginX   = layout.LineOriginX;
					element.LineBaseY     = layout.LineBaseY;
					element.LineY1        = layout.LineY1;
					element.LineY2        = layout.LineY2;
					element.LineWidth     = (continuation && !tabNewLine) ? result[result.Count-1].Profile.TotalWidth : layout.AvailableWidth;
					element.LineAscender  = layout.LineAscender;
					element.LineDescender = layout.LineDescender;
					element.IsTabulation  = layoutStatus == Layout.Status.OkTabReached;
					element.IsNewLine     = (layoutStatus != Layout.Status.OkTabReached) | tabNewLine;
					
					list.Add (element);
				}
				
				if (layoutStatus == Layout.Status.OkHiddenFitEnded)
				{
					offset = layout.TextOffset;
				}
				else
				{
					layout.DefineTextOffset (offset);
				}
				
				if (layoutStatus == Layout.Status.OkTabReached)
				{
					lineStartOffset = offset;
					
					if (tabNewLine)
					{
						lineCount++;
						
						defLineCount        = lineCount;
						defLineStartOffset = lineStartOffset;
						defListCount        = list.Count;
					
						defFrameIndex = layout.FrameIndex;
						defFrameY     = layout.FrameY;
					}
					else
					{
						layout.SelectFrame (defFrameIndex, defFrameY);
					}
				}
				else
				{
					if (layoutStatus == Layout.Status.ErrorNeedMoreRoom)
					{
						//	Le paragraphe a été tronqué, car il n'y a plus assez de place dans
						//	les ITextFrame pour y placer le texte. Il faut générer un Element
						//	décrivant la fin du paragraphe :
						
						Cursors.FitterCursor.Element element = new Cursors.FitterCursor.Element ();
						
						int paragraphLength = this.ComputeParagraphLength (text, lineStartOffset);
						
						if (paragraphLength < 0)
						{
							//	La fin du paragraphe n'a pas pu être trouvée; demande à l'appelant
							//	une nouvelle passe avec plus de texte.
							
							length = paragraphStartOffset;
							status = TextProcessor.Status.Continue;
							return;
						}
						
						element.Length     = paragraphLength;
						element.FrameIndex = -1;
						
						list.Add (element);
					}
					
					if ((layoutStatus == Layout.Status.OkFitEnded) ||
						(layoutStatus == Layout.Status.ErrorNeedMoreRoom))
					{
						System.Diagnostics.Debug.Assert (list.Count > 0);
						
						Cursors.FitterCursor mark = this.NewFitterCursor ();
						
						//	Prend note des informations de position relatives aux lignes
						//	constituant le paragraphe, ainsi que des informations globales
						//	sur le paragraphe :
						
						mark.AddRange (list);
						mark.DefineParagraphY (paragraphStartFrameY);
						mark.DefineParagraphNextY (layout.FrameY);
						mark.DefineKeepWithNextParagraph (layout.KeepWithNextParagraph);
						mark.DefineSpaceAfterParagraph (layout.LineSpaceAfter);
						list.Clear ();
						
						story.MoveCursor (mark, pos + paragraphStartOffset);
					}
					
					if ((layoutStatus == Layout.Status.OkFitEnded) ||
						(layoutStatus == Layout.Status.OkHiddenFitEnded) ||
						(layoutStatus == Layout.Status.ErrorNeedMoreRoom))
					{
						lineStartOffset           = offset;
						paragraphStartOffset      = offset;
						paragraphStartFrameIndex = layout.FrameIndex;
						paragraphStartFrameY     = layout.FrameY;
						lineCount                  = 0;
						
						if ((layoutStatus == Layout.Status.OkFitEnded) &&
							(layout.LastProcessedCode == Unicode.Code.PageSeparator))
						{
							Properties.BreakProperty breakProperty;
							this.story.TextContext.GetBreak (layout.LastProcessedCharacter, out breakProperty);
							
							if (breakProperty == null)
							{
								layout.DefineParagraphStartMode (Properties.ParagraphStartMode.Anywhere);
							}
							else
							{
								layout.DefineParagraphStartMode (breakProperty.ParagraphStartMode);
							}
						}
						else
						{
							layout.DefineParagraphStartMode (Properties.ParagraphStartMode.Anywhere);
						}
					}
					else
					{
						lineStartOffset = offset;
						lineCount++;
					}
					
					defLineCount        = lineCount;
					defLineStartOffset = lineStartOffset;
					defListCount        = list.Count;
					
					defFrameIndex = layout.FrameIndex;
					defFrameY     = layout.FrameY;
				}

				if (endOfText)
				{
					length = -1;
					status = TextProcessor.Status.Abort;
					return;
				}
			}
		}
		
		
		private int RewindToPreviousParagraph(Cursors.TempCursor cursor, int position, int offset)
		{
			//	Revient au paragraphe précédent et supprime le curseur du paragraphe
			//	créé le plus récemment :
			
			Cursors.FitterCursor para1 = this.GetPreviousFitterCursor (position + offset);
			Cursors.FitterCursor para2 = this.GetPreviousFitterCursor (para1);
			
			Properties.KeepProperty    keep1 = this.GetKeepProperty (para1);
			Properties.KeepProperty    keep2 = this.GetKeepProperty (para2);
			Properties.LeadingProperty lead2 = this.GetLeadingProperty (para2);
			
			Debug.Assert.IsNotNull (para1);
			
			bool keep1WithPrev = (keep1 == null) ? false : (keep1.KeepWithPreviousParagraph == Properties.ThreeState.True);
			bool keep2WithNext = (keep2 == null) ? false : (keep2.KeepWithNextParagraph == Properties.ThreeState.True);
			
			Cursors.FitterCursor.Element[] elems = para1.Elements;
			
			int paraLineCount = elems.Length;
			int paraLastLine  = paraLineCount - 1;
			int paraLength    = para1.ParagraphLength;
			
			this.lineSkipBefore = ((lead2 == null) || (double.IsNaN (lead2.SpaceAfter))) ? 0 : lead2.SpaceAfterInPoints;
			this.keepWithPrev   = keep1WithPrev || keep2WithNext;
			
			this.frameIndex = elems[0].FrameIndex;
			this.frameY     = para1.ParagraphY;
			
			this.frameFences.Add (this.frameIndex, paraLastLine);
			
			int distance = offset - paraLength;
			
			//	Il faut encore supprimer le curseur correspondant à la marque de
			//	début du paragraphe :
			
			this.RecycleFitterCursor (para1);
			
			return distance;
		}
		
		
		private void GetCursorGeometry(int position, int cursorOffset, Cursors.FitterCursor cursor, Cursors.FitterCursor.Element[] elements, int i, ref double x, ref double y, ref double y1, ref double y2, double spaceAfter)
		{
			ITextFrame frame = this.FrameList[elements[i].FrameIndex];
			
			this.GetCursorGeometry (frame, position, cursorOffset, cursor, elements, i, ref x, ref y, ref y1, ref y2, spaceAfter);
		}
		
		private Internal.GeometryRenderer GenerateGeometryInformation(ITextFrame frame, int position, Cursors.FitterCursor cursor, Cursors.FitterCursor.Element[] elements, int i, double spaceAfter)
		{
			int     length = elements[i].Length;
			ulong[] text   = new ulong[length];
			
			this.story.ReadText (position, length, text);
				
			//	Analyse le layout précis en réalisant un rendu du texte avec
			//	GeometryRenderer, lequel enregistre les positions de tous les
			//	caractères traités :
			
			Layout.Context            layout   = new Layout.Context (this.story.TextContext, text, 0, this.frameList);
			Internal.GeometryRenderer renderer = new Internal.GeometryRenderer ();
			
			layout.DisableFontBaselineOffset ();
			layout.DisableSimpleRendering ();
			
			this.RenderElement (renderer, frame, cursor, elements, i, layout, spaceAfter);
			
			return renderer;
		}
		
		private void GetCursorGeometry(ITextFrame frame, int position, int cursorOffset, Cursors.FitterCursor cursor, Cursors.FitterCursor.Element[] elements, int i, ref double x, ref double y, ref double y1, ref double y2, double spaceAfter)
		{
			//	Pour éviter de redemander à chaque fois au geometry renderer de produire
			//	les informations de position des caractères, on utilise un cache ici :
			
			if ((this.geometryCacheVersion.HasAnythingChanged) ||
				(this.geometryCacheStart != position))
			{
				Internal.GeometryRenderer renderer = this.GenerateGeometryInformation (frame, position, cursor, elements, i, spaceAfter);
				
				int n = renderer.ElementCount;
				
				if ((n > 1) &&
					(elements[i].IsTabulation) &&
					(elements[i].IsNewLine == false))
				{
					//	La tranche de texte se termine par une marque de tabulation.
					//	On doit mettre à jour sa largeur en déterminant où commence
					//	la ligne suivante :
					
					System.Diagnostics.Debug.Assert (renderer[n-2].Unicode == Unicode.Code.HorizontalTab);
					System.Diagnostics.Debug.Assert (renderer[n-1].Unicode == Unicode.Code.Null);
					System.Diagnostics.Debug.Assert (i+1 < elements.Length);
					
					int length = elements[i].Length;
					int nextPos = position + length;
					int nextI   = i + 1;
					
					Internal.GeometryRenderer next = this.GenerateGeometryInformation (frame, nextPos, cursor, elements, nextI, spaceAfter);
					
					System.Diagnostics.Debug.Assert (next.HasTabBeforeText);
					
					renderer.DefineTab (next.TabOrigin, next.TabStop);
				}
				
				this.geometryCacheVersion.Update ();
				
				this.geometryCacheRenderer = renderer;
				this.geometryCacheStart    = position;
			}
			
			System.Diagnostics.Debug.Assert (this.geometryCacheRenderer != null);
			System.Diagnostics.Debug.Assert (this.geometryCacheVersion.HasFitterChanged == false);
			
			Internal.GeometryRenderer.Element item = this.geometryCacheRenderer[cursorOffset];
			
			if (item != null)
			{
				x  = item.X;
				y  = item.Y;
				y1 = item.Y1;
				y2 = item.Y2;
			}
			else
			{
				//	Pas d'élément trouvé : on prend la ligne de base comme
				//	référence :
				
				x  = elements[i].LineStartX;
				y  = elements[i].LineBaseY;
				y1 = y + elements[i].LineDescender;
				y2 = y + elements[i].LineAscender;
				
				frame.MapToView (ref x, ref y);
			}
		}
		
		
		private void GenerateMarksIfNeeded()
		{
			if (this.story.HasTextChangeMarks)
			{
				this.GenerateMarks ();
				
				System.Diagnostics.Debug.Assert (this.story.HasTextChangeMarks == false);
			}
		}
		
		private void RenderElement(ITextRenderer renderer, ITextFrame frame, Cursors.FitterCursor cursor, Cursors.FitterCursor.Element[] elements, int i, Layout.Context layout, double spaceAfter)
		{
			int lastI = elements.Length - 1;
			int index  = elements[i].FrameIndex;
			int length = elements[i].Length;
			
			layout.SelectFrame (index, 0);
			
			if ((frame == null) ||
				(layout.Frame == frame))
			{
				double ox    = elements[i].LineStartX;
				double oy    = elements[i].LineBaseY;
				double width = elements[i].LineWidth;
				double asc   = elements[i].LineAscender;
				double desc  = elements[i].LineDescender;
				double y1    = elements[i].LineY1;
				double y2    = elements[i].LineY2;
				
				//	Transforme les coordonnées internes en coordonnées utilisables pour
				//	l'affichage :
				
				y1 -= oy;
				y2 -= oy;
				
				layout.Frame.MapToView (ref ox, ref oy);
				
				y1 += oy;
				y2 += oy;
				
				if (Cursors.FitterCursor.IsLastLine (elements, i))
				{
					//	C'est la dernière ligne du paragraphe, donc il faut tenir compte
					//	de l'espace qui est ajouté après et descendre y1 en conséquence :
					
					y1 -= spaceAfter;
				}
				
				if ((length > 0) &&
					(renderer.IsFrameAreaVisible (layout.Frame, ox, oy+desc, width, asc+desc)))
				{
					bool isTab  = (i > 0) ? elements[i-1].IsTabulation : false;
					bool isLast = (i == lastI) || (elements[i].IsTabulation);
					
					Layout.StretchProfile profile = elements[i].Profile;
					
					//	Le render a besoin de pouvoir accéder à diverses informations
					//	sur la géométrie de la ligne. On les initialise à cet effet :
					
					layout.DefineLineGeometry (oy, y1, y2, asc, desc);
					
					//	S'il y a une marque de tabulation à la fin de l'élément précé-
					//	dent, on va donner une chance au renderer de la peindre mainte-
					//	nant :
					
					if (isTab)
					{
						double tabOrigin = elements[i-1].IsNewLine ? elements[i].LineOriginX : elements[i-1].LineStartX + elements[i-1].LineWidth;
						double tabStop   = elements[i].LineStartX;
						ulong  tabCode   = this.story.ReadChar (cursor, cursor.GetElementStartPosition (i) - 1);
						
						Properties.TabProperty      tabProperty;
						Properties.TabsProperty     tabsProperty;
						Properties.AutoTextProperty autoProperty;
						
						TextContext context = this.story.TextContext;
						
						context.GetTabAndTabs (tabCode, out tabProperty, out tabsProperty);
						context.GetAutoText (tabCode, out autoProperty);
						
						string tag = tabProperty == null ? null : tabProperty.TabTag;
						
						bool isTabDefined = tabsProperty == null ? false : tabsProperty.ContainsTabTag (tag);
						bool isTabAuto    = autoProperty == null ? false : true;
						
						renderer.RenderTab (layout, tag, tabOrigin, tabStop, tabCode, isTabDefined, isTabAuto);
					}
					
					layout.RenderLine (renderer, profile, length, ox, oy, width, i, isTab, isLast);
				}
				else
				{
					layout.DefineLineGeometry (oy, y1, y2, asc, desc);
					layout.InvisibleLine (renderer, length, ox, oy);
				}
			}
			
			layout.DefineTextOffset (layout.TextOffset + length);
		}
		
		
		private Cursors.FitterCursor GetPreviousFitterCursor(int position)
		{
			CursorInfo[] info = this.story.TextTable.FindCursorsBefore (position, Cursors.FitterCursor.Filter);
			
			if (info.Length < 1)
			{
				return null;
			}
			
			return this.story.TextTable.GetCursorInstance (info[0].CursorId) as Cursors.FitterCursor;
		}
		
		private Cursors.FitterCursor GetPreviousFitterCursor(ICursor cursor)
		{
			return (cursor == null) ? null : this.GetPreviousFitterCursor (this.story.GetCursorPosition (cursor));
		}
		
		
		private Properties.KeepProperty    GetKeepProperty(ICursor cursor)
		{
			if (cursor == null)
			{
				return null;
			}
			
			Properties.KeepProperty keep = null;
			ulong[]                 text = new ulong[1];
			
			this.story.ReadText (cursor, 1, text);
			this.story.TextContext.GetKeep (text[0], out keep);
			
			return keep;
		}
		
		private Properties.LeadingProperty GetLeadingProperty(ICursor cursor)
		{
			if (cursor == null)
			{
				return null;
			}
			
			Properties.LeadingProperty leading = null;
			ulong[]                    text    = new ulong[1];
			
			this.story.ReadText (cursor, 1, text);
			this.story.TextContext.GetLeading (text[0], out leading);
			
			return leading;
		}
		
		
		private TabStatus MeasureTabTextWidth(Layout.Context layout, Properties.TabProperty tabProperty, Properties.MarginsProperty marginsProperty, double fontSizeInPoints, int lineCount, bool startOfLine, out double tabX, out double width, out bool tabIndents)
		{
			//	Détermine la position de départ du texte après le tabulateur, sa
			//	largeur et l'indentation éventuellement à appliquer à la suite du
			//	texte :
			
			tabX = 0;
			width = 0;
			
			TabList tabs = layout.TextContext.TabList;
			
			double d = tabs.GetTabDisposition (tabProperty);
			string dockingMark = tabs.GetTabDockingMark (tabProperty);
			
			if ((dockingMark != null) &&
				(dockingMark.Length > 0))
			{
				d = 1.0;
			}
			
			double x1 = startOfLine ? layout.LineOriginX : (layout.LineCurrentX + 0.001);
			double x2 = tabs.GetTabPositionInPoints (tabProperty);
			double x3 = layout.LineWidth - layout.RightMargin;
			
			//	Gestion des attributs spéciaux :
			
			string tabAttr = tabs.GetTabAttribute (tabProperty);
			
			if ((tabAttr != null) &&
				(tabAttr.Length > 0))
			{
				int    level  = marginsProperty == null ? 0 : marginsProperty.Level;
				double offset = 0;
				
				if (level < 0)
				{
					level = 0;
				}

				//	Les attributs spéciaux peuvent définir un offset dépendant de
				//	l'indentation ou encore un offset relatif additionnel qui est
				//	alors dépendant de la taille de la fonte.
				
				offset += TabList.GetLevelOffset (fontSizeInPoints, level, tabAttr);
				offset += TabList.GetRelativeOffset (fontSizeInPoints, tabAttr);
				
				x2 += offset;
			}
			
			//	Gestion des modes absolus/relatifs :
			
			switch (tabs.GetTabPositionMode (tabProperty))
			{
				case TabPositionMode.Absolute:
				case TabPositionMode.AbsoluteIndent:
					break;
				
				case TabPositionMode.Force:
				case TabPositionMode.ForceIndent:
					x1 = 0;
					break;
				
				case TabPositionMode.LeftRelative:
				case TabPositionMode.LeftRelativeIndent:
					
					//	Tabulateur relatif à la marge de gauche; on ajuste la
					//	position :
					
					x2 = layout.LeftBodyMargin + x2;
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("Tab position mode {0} not supported", tabs.GetTabPositionMode (tabProperty)));
			}
			
			//	Gestion de l'indentation du paragraphe après la marque de tabulation :
			
			switch (tabs.GetTabPositionMode (tabProperty))
			{
				case TabPositionMode.Absolute:
				case TabPositionMode.LeftRelative:
				case TabPositionMode.Force:
					tabIndents = false;
					break;
				
				case TabPositionMode.AbsoluteIndent:
				case TabPositionMode.LeftRelativeIndent:
				case TabPositionMode.ForceIndent:
					tabIndents = true;
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("Tab position mode {0} not supported", tabs.GetTabPositionMode (tabProperty)));
			}
			
			if ((x2 <= x1) &&
				(startOfLine))
			{
				//	Tabulateur en début de ligne, mais à gauche de la marge de
				//	gauche => tant pis, on le place hors des marges.
				
				x1 = x2;
			}
			
			double xBefore = x2 - x1;
			double xAfter  = x3 - x2;
			
			TabStatus              status  = TabStatus.Ok;
			Layout.BreakCollection result  = new Layout.BreakCollection ();
			Layout.Context         scratch = new Layout.Context (layout.TextContext, layout.Text, 0, null);
			
			scratch.ResetLineHeight ();
			scratch.RecordAscender (layout.LineAscender);
			scratch.RecordDescender (layout.LineDescender);
			scratch.RecordLineHeight (layout.LineHeight);
			
			if ((xBefore < 0) ||
				(xAfter < 0))
			{
				//	Tabulateur mal placé... Demande un saut de ligne ! Mais on
				//	calcule encore au préalable la position qu'occupera le texte
				//	tabulé sur la ligne suivante :
				
				scratch.SelectMargins (lineCount);
				
				x1 = scratch.LeftMargin;
				
				xBefore = x2 - x1;
				xAfter  = x3 - x2;
				
				status = TabStatus.ErrorNeedMoreRoom;
			}
			
			double room;
			double roomAfter;
			double roomBefore;

			
			//	Détermine la place disponible entre le texte qui se trouve
			//	avant le tabulateur et la marge droite, en tenant compte de
			//	la manière dont le texte est disposé.
			
			if (d < 0.5)
			{
				double ratio = d / (1-d);				//	plutôt tabulateur aligné à gauche
				
				roomAfter  = xAfter;
				roomBefore = xAfter * ratio;
				
				if ((xBefore < roomBefore) &&
					(xBefore > 0))
				{
					roomBefore = xBefore;
					roomAfter  = xBefore / ratio;
				}
			}
			else
			{
				double ratio = (1-d) / d;				//	plutôt tabulateur aligné à droite
				
				roomBefore = xBefore;
				roomAfter  = xBefore * ratio;
				
				if ((xAfter < roomAfter) &&
					(xAfter > 0))
				{
					roomAfter  = xAfter;
					roomBefore = xAfter / ratio;
				}
			}
			
			room = roomBefore + roomAfter;
			
			scratch.DefineAvailableWidth (room);
			scratch.MoveTo (0, layout.TextOffset);
			scratch.DefineTabDockingMark (dockingMark);
			
			Layout.Status fitStatus = scratch.Fit (ref result, lineCount, true);
			
			if (fitStatus == Layout.Status.ErrorNeedMoreText)
			{
				return TabStatus.ErrorNeedMoreText;
			}
			
			if ((fitStatus == Layout.Status.OkFitEnded) ||
				(fitStatus == Layout.Status.OkTabReached))
			{
				//	TODO: sélectionner le résultat optimal
				
				Layout.StretchProfile profile = result[result.Count-1].Profile;
				
				width = profile.TotalWidth + profile.WidthEndSpace;
				tabX = x2 - d * width;
				
				return status;
			}
			
			if (fitStatus == Layout.Status.Ok)
			{
				if ((d == 0.0) ||
					(startOfLine))
				{
					//	TODO: sélectionner le résultat optimal
					
					width = result[result.Count-1].Profile.TotalWidth;
					tabX = x2 - d * width;
					
					return (d > 0) ? TabStatus.ErrorNeedMoreRoom : status;
				}
				else
				{
					//	On n'arrive pas à placer le texte du tabulateur sur la
					//	ligne en cours; il faut donc demander un passage à la
					//	ligne suivante :
					
					return TabStatus.ErrorNeedMoreRoom;
				}
			}
			
			return TabStatus.ErrorCannotFit;
		}
		
		
		private double ComputePenalty(double spacePenalty, double breakPenalty)
		{
			return spacePenalty + breakPenalty;
		}
		
		private int    ComputeParagraphLength(ulong[] text, int offset)
		{
			//	Détermine la longueur du paragraphe dans le texte passé en entrée, en
			//	commençant à l'offset indiqué. Une marque de fin de texte ne fait pas
			//	partie du paragraphe, alors qu'une marque de fin de paragraphe si.
			
			Unicode.BreakAnalyzer analyzer = Unicode.DefaultBreakAnalyzer;
			
			for (int i = offset; i < text.Length; i++)
			{
				int code = Unicode.Bits.GetCode (text[i]);
				
				if (code == (int) Unicode.Code.EndOfText)
				{
					return i - offset;
				}
				
				if (analyzer.IsParagraphBreak (code))
				{
					return i - offset + 1;
				}
			}
			
			return -1;
		}
		
		
		private Cursors.FitterCursor NewFitterCursor()
		{
			//	Retourne un curseur tout neuf (ou reprend un curseur qui a été
			//	recyclé précédemment, pour éviter de devoir en allouer à tour
			//	de bras).
			
			Cursors.FitterCursor cursor;
			
			if (this.freeCursors.Count > 0)
			{
				cursor = this.freeCursors.Pop () as Cursors.FitterCursor;
			}
			else
			{
				cursor = new Cursors.FitterCursor (this);
				this.cursors.Add (cursor);
			}
			
			this.story.NewCursor (cursor);
			
			return cursor;
		}
		
		private void RecycleFitterCursor(ICursor cursor)
		{
			//	Recycle le curseur passé en entrée. Il est simplement placé
			//	dans la pile des curseurs disponibles.
			
			Debug.Assert.IsTrue (this.cursors.Contains (cursor));
			Debug.Assert.IsFalse (this.freeCursors.Contains (cursor));
			
			this.story.RecycleCursor (cursor);
			
			cursor.Clear ();
			
			this.freeCursors.Push (cursor);
		}
		
		
		#region TabStatus Enumeration
		private enum TabStatus
		{
			Ok,
			ErrorNeedMoreText,
			ErrorNeedMoreRoom,
			ErrorCannotFit
		}
		#endregion
		
		private long							version;
		
		private TextStory						story;
		private System.Collections.ArrayList	cursors;
		private System.Collections.Stack		freeCursors;
		
		private FrameList						frameList;
		private int								frameIndex;
		private double							frameY;
		
		private double							lineSkipBefore;
		private Layout.FrameLineFenceDictionary	frameFences = new Layout.FrameLineFenceDictionary ();
		private bool							keepWithPrev;
		
		private IPageCollection					pageCollection;
		
		private TextVersion						geometryCacheVersion;
		private int								geometryCacheStart;
		private Internal.GeometryRenderer		geometryCacheRenderer;
	}
}
