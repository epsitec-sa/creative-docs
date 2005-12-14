//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextFitter s'occupe de la mise en page du texte au
	/// moyen des m�thodes ClearAllMarks/GenerateAllMarks et du rendu
	/// du texte gr�ce aux m�thodes RenderXxx.
	/// </summary>
	public sealed class TextFitter
	{
		public TextFitter(TextStory story)
		{
			this.TextStory = story;
			
			this.frame_list      = new FrameList (this);
			this.page_collection = new DefaultPageCollection ();
			
			this.geometry_cache_version = new TextVersion (this);
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
					this.free_cursors = new System.Collections.Stack ();
				}
			}
		}
		
		public int								CursorCount
		{
			get
			{
				return this.cursors.Count - this.free_cursors.Count;
			}
		}
		
		
		public FrameList						FrameList
		{
			get
			{
				return this.frame_list;
			}
		}
		
		public IPageCollection					PageCollection
		{
			get
			{
				return this.page_collection;
			}
			set
			{
				if (this.page_collection != value)
				{
					this.page_collection = value;
				}
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
			
			this.frame_list.ClearCursorMap ();
			this.Invalidate ();
		}
		
		public void GenerateAllMarks()
		{
			this.frame_index = 0;
			this.frame_y     = 0;
			
			this.line_skip_before = 0;
			this.line_fence       = -1;
			this.keep_with_prev   = false;
			
			if (this.frame_list.Count > 0)
			{
				TextProcessor processor = new TextProcessor (this.story);
				processor.Process (new TextProcessor.Executor (this.ExecuteGenerate));
			}
			
			this.story.ClearTextChangeMarkPositions ();
			this.frame_list.ClearCursorMap ();
			this.Invalidate ();
		}
		
		public void GenerateMarks()
		{
			System.Diagnostics.Debug.WriteLine ("GenerateMarks.");
			
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
				//	Il n'y a rien � faire, car le texte n'a pas chang� de mani�re
				//	significative depuis la derni�re fois.
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
				
				Cursors.FitterCursor           fitter_cursor = text.GetCursorInstance (infos[0].CursorId) as Cursors.FitterCursor;
				Cursors.FitterCursor.Element[] elements      = fitter_cursor.Elements;
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Optimized GenerateMarks: {0}/{1}, Cursor[0].pos={2} (n={3})",
					/**/										   start, end,
					/**/										   this.story.GetCursorPosition (fitter_cursor),
					/**/										   infos.Length));
				
				int n = elements.Length - 1;
				
				this.line_fence       = -1;
				this.line_skip_before = fitter_cursor.SpaceAfterParagraph;
				this.keep_with_prev   = fitter_cursor.KeepWithNextParagraph;
				this.frame_index      = elements[n].FrameIndex;
				this.frame_y          = fitter_cursor.ParagraphNextY;
				
				System.Diagnostics.Debug.Assert (this.story.TextLength > 0);
				
				TextProcessor processor = new TextProcessor (this.story);
				processor.DefineStartFence (start);
				processor.Process (new TextProcessor.Executor (this.ExecuteClear));
				
				try
				{
					if (this.frame_list.Count > 0)
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
				
				this.frame_list.ClearCursorMap ();
				this.Invalidate ();
			}
		}
		
		
		public void RenderParagraph(ICursor cursor, ITextRenderer renderer)
		{
			this.RenderParagraphInTextFrame (cursor, renderer, null);
		}
		
		public void RenderParagraphInTextFrame(ICursor fitter_cursor, ITextRenderer renderer, ITextFrame frame)
		{
			Cursors.FitterCursor cursor = fitter_cursor as Cursors.FitterCursor;
			
			if (cursor == null)
			{
				throw new System.ArgumentException ("Not a valid FitterCursor.", "fitter_cursor");
			}
			
			int     length = cursor.ParagraphLength;
			ulong[] text   = new ulong[length];
			
			this.story.ReadText (cursor, length, text);
			
			Layout.Context                 layout   = new Layout.Context (this.story.TextContext, text, 0, this.frame_list);
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
			int index = this.frame_list.IndexOf (frame);
			
			if (index < 0)
			{
				throw new System.ArgumentException ("Not a valid ITextFrame.", "frame");
			}
			
			Cursors.FitterCursor cursor = this.frame_list.FindFirstCursor (index);
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
		
		
		public bool HitTestTextFrame(ITextFrame frame, double x, double y, bool skip_invisible, ref int position, ref int direction)
		{
			int index = this.frame_list.IndexOf (frame);
			
			if (index < 0)
			{
				throw new System.ArgumentException ("Not a valid ITextFrame.", "frame");
			}
			
			Cursors.FitterCursor cursor = this.frame_list.FindFirstCursor (index);
			Internal.TextTable   text   = this.story.TextTable;
			
			while (cursor != null)
			{
				if (cursor.ContainsFrameIndex (index) == false)
				{
					break;
				}
				
				if (this.HitTestParagraphInTextFrame (cursor, frame, x, y, skip_invisible, ref position, ref direction))
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
			
			return false;
		}
		
		public bool HitTestParagraphInTextFrame(ICursor fitter_cursor, ITextFrame frame, double x, double y, bool skip_invisible, ref int position, ref int direction)
		{
			//	D�termine o� se trouve le curseur dans le frame sp�cifi�. La recherche
			//	se base sur les positions des lignes, dans un premier temps, puis sur
			//	les informations d�taill�es de position au sein de la ligne.
			
			Cursors.FitterCursor cursor = fitter_cursor as Cursors.FitterCursor;
			
			if (cursor == null)
			{
				throw new System.ArgumentException ("Not a valid FitterCursor.", "fitter_cursor");
			}
			
			Cursors.FitterCursor.Element[] elements = cursor.Elements;
			
			int para_start  = this.story.GetCursorPosition (fitter_cursor);
			int line_offset = 0;
			
			double view_x = x;
			double view_y = y;
			
			//	Convertit les coordonn�es [x,y] en coordonn�es internes, pour la
			//	premi�re recherche approximative. Les recherches d�taill�es bas�es
			//	sur GetCursorGeometry se basent ensuite sur des coordonn�es [view]
			//	standard.
			
			frame.MapFromView (ref x, ref y);
			
			double dy_after = cursor.SpaceAfterParagraph;
			
			for (int i = 0; i < elements.Length; i++)
			{
				double l_yb = elements[i].LineBaseY;
				double l_y1 = System.Math.Min (elements[i].LineY1 - dy_after, l_yb + elements[i].LineDescender);
				double l_y2 = System.Math.Max (elements[i].LineY2, l_yb + elements[i].LineAscender);
				
				if ((y >= l_y1) &&
					(y <= l_y2))
				{
					//	La bande horizontale qui comprend cette ligne se trouve dans la
					//	zone d'int�r�t. Analysons-la plus � fond :
					
					double cx1 = 0;
					double cy  = 0;
					double cy1 = 0;
					double cy2 = 0;
					
					this.GetCursorGeometry (frame, para_start + line_offset, 0, cursor, elements, i, ref cx1, ref cy, ref cy1, ref cy2, dy_after);
					
					if ((view_y >= cy1) &&
						(view_y <= cy2))
					{
						//	Verticalement, le curseur est bien dans la ligne, en tenant
						//	compte de toutes les finesses (interligne, espace avant et
						//	apr�s, etc.)
						
						if (view_x < cx1)
						{
							//	On se trouve trop � gauche --> d�but de ligne.
							
							position  = para_start + line_offset;
							direction = -1;
							
							break;
						}
						else
						{
							int last_valid_offset = 0;
							
							//	Passe toute la ligne au crible fin, caract�re par carac-
							//	t�re, jusqu'� trouver celui qui est sous le curseur :
							
							for (int glyph_offset = 1; glyph_offset <= elements[i].Length; glyph_offset++)
							{
								int    pos = para_start + line_offset;
								double cx2 = cx1;
								
								this.GetCursorGeometry (frame, pos, glyph_offset, cursor, elements, i, ref cx2, ref cy, ref cy1, ref cy2, dy_after);
								
								if ((view_x >= cx1) &&
									(view_x <= cx2))
								{
									//	Le clic a �t� fait dans le corps de la ligne.
									
									//	Cas particulier : si la ligne est vide (juste un caract�re
									//	de fin de ligne), cx1 == cx2 et il faut g�n�rer une position
									//	telle que le curseur r�sultant sera plac� au d�but de la
									//	ligne (position avant le saut de ligne, direction = 1).
									
									int adjust = (2*view_x <= (cx1 + cx2)) ? -1 : 0;
									
									position  = para_start + line_offset + glyph_offset + adjust;
									direction = ((glyph_offset + adjust > 0) || (position == 0) || (cx1 == cx2)) ? 1 : -1;
									
									return true;
								}
								
								if ((cx2 > cx1) ||
									(skip_invisible))
								{
									last_valid_offset = glyph_offset;
								}
								
								cx1 = cx2;
							}
							
							//	On se trouve trop � droite --> fin de ligne.
							
							if (view_x > cx1)
							{
								position  = para_start + line_offset + last_valid_offset;
								direction = 1;
							}
						}
					}
				}
				
				line_offset += elements[i].Length;
			}
			
			return false;
		}
		
		
		public bool GetCursorGeometry(ICursor cursor, out ITextFrame frame, out double x, out double y, out int paragraph_line, out int line_character)
		{
			//	D�termine o� se trouve le curseur sp�cifi� : frame, position [x;y],
			//	num�ro de ligne dans le paragraphe et num�ro de caract�re dans la
			//	ligne.
			
			//	NB: La position verticale est toujours align�e sur la ligne de base
			//		de la fonte, sans tenir compte d'un �ventuel offset vertical.
			
			int position  = this.story.GetCursorPosition (cursor);
			int direction = this.story.GetCursorDirection (cursor);
			
			Internal.TextTable text   = this.story.TextTable;
			CursorInfo.Filter  filter = Cursors.FitterCursor.GetFitterFilter (this);
			CursorInfo[]       infos  = text.FindCursorsBefore (position + 1, filter);
			
			if (infos.Length > 0)
			{
				System.Diagnostics.Debug.Assert (infos.Length == 1);
				
				Cursors.FitterCursor fitter_cursor = text.GetCursorInstance (infos[0].CursorId) as Cursors.FitterCursor;
				
				int paragraph_start = text.GetCursorPosition (infos[0].CursorId);
				int line_start      = paragraph_start;
				int tab_count       = 0;
				int tab_char_count  = 0;
				
				if (paragraph_start + fitter_cursor.ParagraphLength < position)
				{
					//	Le curseur se trouve dans une tranche de texte qui n'appartient
					//	� aucun paragraphe.
				}
				else
				{
					Cursors.FitterCursor.Element[] elements = fitter_cursor.Elements;
					
					//	D�termine dans quelle ligne du paragraphe se trouve le curseur :
					
					for (int i = 0; i < elements.Length; i++)
					{
						int line_length = elements[i].Length;
						int line_end    = line_start + line_length;
						int frame_index = elements[i].FrameIndex;
						
						if (frame_index < 0)
						{
							break;
						}
						
						if ((position >= line_start) &&
							(  (position < line_end)
							|| ((position == line_end) && (direction >= 0) && (! elements[i].IsTabulation))
							|| ((position == line_end) && (line_length == 0))
							|| ((position == line_end) && (line_end == this.story.TextLength)) ))
						{
							//	Le curseur se trouve dans la ligne en cours d'analyse.
							//	On tient compte de la direction de d�placement pour
							//	d�terminer si le curseur se trouve � la fin de la ligne
							//	en cours ou au d�but de la ligne suivante.
							
							frame = this.FrameList[frame_index];
							
							x = elements[i].LineStartX;
							y = elements[i].LineBaseY;
							
							paragraph_line = i - tab_count;
							line_character = position - line_start + tab_char_count;
							
							double y1 = 0;
							double y2 = 0;
							
							this.GetCursorGeometry (line_start, position - line_start, fitter_cursor, elements, i, ref x, ref y, ref y1, ref y2, fitter_cursor.SpaceAfterParagraph);
							
							return true;
						}
						
						if (elements[i].IsTabulation)
						{
							tab_char_count += line_length;
							tab_count++;
						}
						else
						{
							tab_char_count = 0;
						}
						
						line_start = line_end;
						
						if (line_start > position)
						{
							break;
						}
					}
				}
			}
			
			frame = null;
			
			x = 0;
			y = 0;
			
			paragraph_line = 0;
			line_character = 0;
			
			return false;
		}
		
		
		public void GetStatistics(out int paragraph_count, out int line_count)
		{
			paragraph_count = 0;
			line_count      = 0;
			
			CursorInfo[] infos = this.story.TextTable.FindCursors (0, this.story.TextLength, Cursors.FitterCursor.Filter);
			
			for (int i = 0; i < infos.Length; i++)
			{
				Cursors.FitterCursor cursor = this.story.TextTable.GetCursorInstance (infos[i].CursorId) as Cursors.FitterCursor;
				
				paragraph_count += 1;
				line_count      += cursor.LineCount;
			}
		}
		
		
		private void Invalidate()
		{
			this.version++;
			this.geometry_cache_renderer = null;
		}
		
		private void ExecuteClear(Cursors.TempCursor temp_cursor, int pos, ref int length, out TextProcessor.Status status)
		{
			//	Supprime les marques de d�coupe de lignes repr�sent�es par des
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
			//	G�n�re les marques de d�coupe de lignes et ins�re les curseurs
			//	correspondants.
			
			ulong[] text;
			
			if (pos + length < story.TextLength)
			{
				text = new ulong[length];
				this.story.ReadText (cursor, length, text);
			}
			else
			{
				//	On arrive au bout du texte: il faut donc synth�tiser un caract�re
				//	suppl�mentaire de fin de texte pour que l'algorithme de layout
				//	soit satisfait :
				
				text = new ulong[length+1];
				this.story.ReadText (cursor, length, text);
				
				ulong code = text[length-1];
				
				code &= 0xffffffff00000000ul;
				code |= (int) Unicode.Code.EndOfText;
				
				Unicode.Bits.SetBreakInfo (ref code, Unicode.BreakInfo.Yes);
				
				text[length] = code;
			}
			
			Layout.Context         layout = new Layout.Context (this.story.TextContext, text, 0, this.frame_list);
			Layout.BreakCollection result = new Layout.BreakCollection ();
			
			layout.SelectFrame (this.frame_index, this.frame_y);
			
			layout.DefineLineSkipBefore (this.line_skip_before);
			layout.DefineKeepWithPreviousParagraph (this.keep_with_prev);
			layout.DefineFenceLineCount (this.line_fence);
			
			int    paragraph_start_offset      = 0;
			int    paragraph_start_frame_index = layout.FrameIndex;;
			double paragraph_start_frame_y     = layout.FrameY;
			
restart_paragraph_layout:
			
			layout.DefineTabIndentation (false, 0);
			
			int line_count        = 0;
			int line_start_offset = paragraph_start_offset;
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			bool continuation = false;
			bool reset_line_h = true;
			
			int def_line_count        = line_count;
			int def_line_start_offset = line_start_offset;
			int def_list_count        = list.Count;
			
			int    def_frame_index = layout.FrameIndex;
			double def_frame_y     = layout.FrameY;
			
			for (;;)
			{
				if (reset_line_h)
				{
					layout.ResetLineHeight ();
					reset_line_h = false;
				}
				
				Properties.TabProperty tab_property;
				TextFitter.TabStatus   tab_status;
				
				Layout.Status layout_status = layout.Fit (ref result, line_count, continuation);
				
				bool tab_new_line = false;
				bool end_of_text  = false;
				
				this.frame_index = layout.FrameIndex;
				this.frame_y     = layout.FrameY;
				
				switch (layout_status)
				{
					case Layout.Status.ErrorNeedMoreText:
						this.frame_index = paragraph_start_frame_index;
						this.frame_y     = paragraph_start_frame_y;
						length = paragraph_start_offset;
						status = TextProcessor.Status.Continue;
						return;
					
					case Layout.Status.ErrorCannotFit:
						System.Diagnostics.Debug.WriteLine ("Cannot fit, throwing an exception.");
						throw new System.InvalidOperationException ("Cannot fit.");
					
					case Layout.Status.Ok:
						continuation = false;
						reset_line_h = true;
						break;
					
					case Layout.Status.OkFitEnded:
						
						continuation = false;
						reset_line_h = true;
						
						this.line_skip_before = layout.LineSpaceAfter;
						this.keep_with_prev   = layout.KeepWithNextParagraph;
						this.line_fence       = -1;
						
						layout.DefineLineSkipBefore (this.line_skip_before);
						layout.DefineKeepWithPreviousParagraph (this.keep_with_prev);
						layout.DefineTabIndentation (false, 0);
						
						break;
					
					case Layout.Status.OkHiddenFitEnded:
						
						continuation = false;
						reset_line_h = true;
						
						break;
					
					case Layout.Status.RestartParagraphLayout:
						
						if (layout.FenceLineCount > -1)
						{
							//	Il faut se replacer dans le frame correspondant au d�but
							//	du paragraphe en cours :
							
							layout.SelectFrame (paragraph_start_frame_index, paragraph_start_frame_y);
						}
						
						layout.MoveTo (0, paragraph_start_offset);
						goto restart_paragraph_layout;
					
					case Layout.Status.RewindParagraphAndRestartLayout:
						
						//	Retourne en arri�re, jusqu'au d�but du paragraphe qui pr�c�de
						//	le paragraphe actuel, puis relance l'op�ration au complet.
						
						length  = this.RewindToPreviousParagraph (cursor, pos, paragraph_start_offset);
						status  = TextProcessor.Status.Continue;
						
						return;
					
					case Layout.Status.RestartLineLayout:
						
						Debug.Assert.IsTrue (continuation);
						
						continuation      = false;
						line_count        = def_line_count;
						line_start_offset = def_line_start_offset;
						
						layout.MoveTo (0, line_start_offset);
						layout.SelectFrame (def_frame_index, def_frame_y);
						
						list.RemoveRange (def_list_count, list.Count - def_list_count);
						
						continue;
					
					case Layout.Status.OkTabReached:
						
						//	On vient de trouver une marque de tabulation. Il faut
						//	l'analyser et d�terminer si elle peut �tre plac�e sur
						//	la m�me ligne que le texte qui pr�c�de :
						
						System.Diagnostics.Debug.Assert (layout.TextOffset >= 0);
						System.Diagnostics.Debug.Assert (layout.TextOffset <= text.Length);
						
						//	Si le syst�me de layout change de frame en cours de route,
						//	cela implique que la ligne �tait vide et que l'on doit con-
						//	sid�rer le d�but de frame comme notre nouvelle r�f�rence.
						
						if (def_frame_index != this.frame_index)
						{
							def_frame_index = this.frame_index;
							def_frame_y     = 0;
						}
						
						def_frame_index = this.frame_index;
						def_frame_y     = layout.FrameYLine;
						
						layout.TextContext.GetTab (text[layout.TextOffset-1], out tab_property);
						
						Debug.Assert.IsNotNull (tab_property);
						
						double tab_x;
						double tab_dx;
						bool   tab_at_line_start = (layout.TextOffset == line_start_offset+1);
						bool   tab_indents;
						
						tab_status = this.MeasureTabTextWidth (layout, tab_property, line_count, tab_at_line_start, out tab_x, out tab_dx, out tab_indents);
						
						if (tab_status == TabStatus.ErrorNeedMoreText)
						{
							length = paragraph_start_offset;
							status = TextProcessor.Status.Continue;
							return;
						}
						else if ((tab_status == TabStatus.ErrorNeedMoreRoom) ||
							/**/ (tab_status == TabStatus.ErrorCannotFit))
						{
							//	Le tabulateur ne tient plus sur cette ligne. Force un passage
							//	� la ligne.
							
							tab_new_line = true;
							tab_status   = this.MeasureTabTextWidth (layout, tab_property, line_count, true, out tab_x, out tab_dx, out tab_indents);
							
							if (tab_status == TabStatus.ErrorNeedMoreRoom)
							{
								//	M�me sur une ligne nouvelle, il n'y a pas la place pour
								//	positionner le texte selon les besoins du tabulateur;
								//	on le cale simplement sur la marge :
								
								layout.MoveTo (layout.LineOriginX, layout.TextOffset);
							}
							else
							{
								layout.MoveTo (tab_x, layout.TextOffset);
								layout.DefineTabIndentation (tab_indents, tab_x);
							}
						}
						else if (tab_status == TabStatus.Ok)
						{
							//	Le tabulateur occupe la m�me ligne que le texte qui pr�c�de.
							
							layout.MoveTo (tab_x, layout.TextOffset);
							layout.DefineTabIndentation (tab_indents, tab_x);
						}
						else
						{
							throw new System.NotImplementedException (string.Format ("TabStatus.{0}", tab_status));
						}
						
						continuation = true;
						
						break;
					
					case Layout.Status.ErrorNeedMoreRoom:
						end_of_text = true;
						break;
					
					default:
						throw new System.InvalidOperationException (string.Format ("Invalid layout status received: {0}.", layout_status));
				}
				
				//	Le syst�me de layout propose un certain nombre de points de d�coupe
				//	possibles pour la ligne. Il faut maintenant d�terminer lequel est le
				//	meilleur.
				
				int offset   = line_start_offset;
				int n_breaks = result.Count;
				
				Layout.StretchProfile profile = null;
				
				if (n_breaks > 1)
				{
					double penalty = Layout.StretchProfile.MaxPenalty;
					double advance = -1;
					int    p_index = -1;
					
					for (int i = 0; i < n_breaks; i++)
					{
						double computed_penalty = this.ComputePenalty (result[i].SpacePenalty, result[i].BreakPenalty);
						
						if (computed_penalty < penalty)
						{
							penalty = computed_penalty;
							p_index = i;
						}
					}
					
					if (p_index < 0)
					{
						p_index = 0;
						
						for (int i = 0; i < n_breaks; i++)
						{
							if (result[i].Advance > advance)
							{
								advance = result[i].Advance;
								p_index = i;
							}
						}
					}
					
					offset  = result[p_index].Offset;
					profile = result[p_index].Profile;
				}
				else if (n_breaks == 1)
				{
					offset  = result[0].Offset;
					profile = result[0].Profile;
				}
				
				if (n_breaks > 0)
				{
					Cursors.FitterCursor.Element element = new Cursors.FitterCursor.Element ();
					
					if (pos + offset > story.TextLength)
					{
						Debug.Assert.IsTrue (layout_status == Layout.Status.OkFitEnded);
						
						offset     -= 1;
						end_of_text = true;
					}
					
					//	G�n�re un �l�ment d�crivant la ligne (ou le morceau de ligne
					//	qui pr�c�de un tabulateur) :
					
					element.Length        = offset - line_start_offset;
					element.Profile       = profile;
					element.FrameIndex    = layout.FrameIndex;
					element.LineStartX    = layout.LineStartX;
					element.LineOriginX   = layout.LineOriginX;
					element.LineBaseY     = layout.LineBaseY;
					element.LineY1        = layout.LineY1;
					element.LineY2        = layout.LineY2;
					element.LineWidth     = (continuation && !tab_new_line) ? result[result.Count-1].Profile.TotalWidth : layout.AvailableWidth;
					element.LineAscender  = layout.LineAscender;
					element.LineDescender = layout.LineDescender;
					element.IsTabulation  = layout_status == Layout.Status.OkTabReached;
					element.IsNewLine     = (layout_status != Layout.Status.OkTabReached) | tab_new_line;
					
					list.Add (element);
				}
				
				if (layout_status == Layout.Status.OkHiddenFitEnded)
				{
					offset = layout.TextOffset;
				}
				else
				{
					layout.DefineTextOffset (offset);
				}
				
				if (layout_status == Layout.Status.OkTabReached)
				{
					line_start_offset = offset;
					
					if (tab_new_line)
					{
						line_count++;
						
						def_line_count        = line_count;
						def_line_start_offset = line_start_offset;
						def_list_count        = list.Count;
					
						def_frame_index = layout.FrameIndex;
						def_frame_y     = layout.FrameY;
					}
					else
					{
						layout.SelectFrame (def_frame_index, def_frame_y);
					}
				}
				else
				{
					if (layout_status == Layout.Status.ErrorNeedMoreRoom)
					{
						//	Le paragraphe a �t� tronqu�, car il n'y a plus assez de place dans
						//	les ITextFrame pour y placer le texte. Il faut g�n�rer un Element
						//	d�crivant la fin du paragraphe :
						
						Cursors.FitterCursor.Element element = new Cursors.FitterCursor.Element ();
						
						int paragraph_length = this.ComputeParagraphLength (text, line_start_offset);
						
						if (paragraph_length < 0)
						{
							//	La fin du paragraphe n'a pas pu �tre trouv�e; demande � l'appelant
							//	une nouvelle passe avec plus de texte.
							
							length = paragraph_start_offset;
							status = TextProcessor.Status.Continue;
							return;
						}
						
						element.Length     = paragraph_length;
						element.FrameIndex = -1;
						
						list.Add (element);
					}
					
					if ((layout_status == Layout.Status.OkFitEnded) ||
						(layout_status == Layout.Status.ErrorNeedMoreRoom))
					{
						System.Diagnostics.Debug.Assert (list.Count > 0);
						
						Cursors.FitterCursor mark = this.NewFitterCursor ();
						
						//	Prend note des informations de position relatives aux lignes
						//	constituant le paragraphe, ainsi que des informations globales
						//	sur le paragraphe :
						
						mark.AddRange (list);
						mark.DefineParagraphY (paragraph_start_frame_y);
						mark.DefineParagraphNextY (layout.FrameY);
						mark.DefineKeepWithNextParagraph (layout.KeepWithNextParagraph);
						mark.DefineSpaceAfterParagraph (layout.LineSpaceAfter);
						list.Clear ();
						
						story.MoveCursor (mark, pos + paragraph_start_offset);
					}
					
					if ((layout_status == Layout.Status.OkFitEnded) ||
						(layout_status == Layout.Status.OkHiddenFitEnded) ||
						(layout_status == Layout.Status.ErrorNeedMoreRoom))
					{
						line_start_offset           = offset;
						paragraph_start_offset      = offset;
						paragraph_start_frame_index = layout.FrameIndex;
						paragraph_start_frame_y     = layout.FrameY;
						line_count                  = 0;
						
						if ((layout_status == Layout.Status.OkFitEnded) &&
							(layout.LastProcessedCode == Unicode.Code.PageSeparator))
						{
							Properties.BreakProperty break_property;
							this.story.TextContext.GetBreak (layout.LastProcessedCharacter, out break_property);
							
							if (break_property == null)
							{
								layout.DefineParagraphStartMode (Properties.ParagraphStartMode.Anywhere);
							}
							else
							{
								layout.DefineParagraphStartMode (break_property.ParagraphStartMode);
							}
						}
						else
						{
							layout.DefineParagraphStartMode (Properties.ParagraphStartMode.Anywhere);
						}
					}
					else
					{
						line_start_offset = offset;
						line_count++;
					}
					
					def_line_count        = line_count;
					def_line_start_offset = line_start_offset;
					def_list_count        = list.Count;
					
					def_frame_index = layout.FrameIndex;
					def_frame_y     = layout.FrameY;
				}

				if (end_of_text)
				{
					length = -1;
					status = TextProcessor.Status.Abort;
					return;
				}
			}
		}
		
		
		private int RewindToPreviousParagraph(Cursors.TempCursor cursor, int position, int offset)
		{
			//	Revient au paragraphe pr�c�dent et supprime le curseur du paragraphe
			//	cr�� le plus r�cemment :
			
			Cursors.FitterCursor para_1 = this.GetPreviousFitterCursor (position + offset);
			Cursors.FitterCursor para_2 = this.GetPreviousFitterCursor (para_1);
			
			Properties.KeepProperty    keep_1 = this.GetKeepProperty (para_1);
			Properties.KeepProperty    keep_2 = this.GetKeepProperty (para_2);
			Properties.LeadingProperty lead_2 = this.GetLeadingProperty (para_2);
			
			Debug.Assert.IsNotNull (para_1);
			
			bool keep_1_with_prev = (keep_1 == null) ? false : (keep_1.KeepWithPreviousParagraph == Properties.ThreeState.True);
			bool keep_2_with_next = (keep_2 == null) ? false : (keep_2.KeepWithNextParagraph == Properties.ThreeState.True);
			
			Cursors.FitterCursor.Element[] elems = para_1.Elements;
			
			int para_line_count = elems.Length;
			int para_last_line  = para_line_count - 1;
			int para_length     = para_1.ParagraphLength;
			
			this.line_skip_before = ((lead_2 == null) || (double.IsNaN (lead_2.SpaceAfter))) ? 0 : lead_2.SpaceAfterInPoints;
			this.line_fence       = para_last_line;
			this.keep_with_prev   = keep_1_with_prev || keep_2_with_next;
			
			this.frame_index = elems[0].FrameIndex;
			this.frame_y     = para_1.ParagraphY;
			
			int distance = offset - para_length;
			
			//	Il faut encore supprimer le curseur correspondant � la marque de
			//	d�but du paragraphe :
			
			this.RecycleFitterCursor (para_1);
			
			return distance;
		}
		
		
		private void GetCursorGeometry(int position, int cursor_offset, Cursors.FitterCursor cursor, Cursors.FitterCursor.Element[] elements, int i, ref double x, ref double y, ref double y1, ref double y2, double space_after)
		{
			ITextFrame frame = this.FrameList[elements[i].FrameIndex];
			
			this.GetCursorGeometry (frame, position, cursor_offset, cursor, elements, i, ref x, ref y, ref y1, ref y2, space_after);
		}
		
		private Internal.GeometryRenderer GenerateGeometryInformation(ITextFrame frame, int position, Cursors.FitterCursor cursor, Cursors.FitterCursor.Element[] elements, int i, double space_after)
		{
			int     length = elements[i].Length;
			ulong[] text   = new ulong[length];
			
			this.story.ReadText (position, length, text);
				
			//	Analyse le layout pr�cis en r�alisant un rendu du texte avec
			//	GeometryRenderer, lequel enregistre les positions de tous les
			//	caract�res trait�s :
			
			Layout.Context            layout   = new Layout.Context (this.story.TextContext, text, 0, this.frame_list);
			Internal.GeometryRenderer renderer = new Internal.GeometryRenderer ();
			
			layout.DisableFontBaselineOffset ();
			layout.DisableSimpleRendering ();
			
			this.RenderElement (renderer, frame, cursor, elements, i, layout, space_after);
			
			return renderer;
		}
		
		private void GetCursorGeometry(ITextFrame frame, int position, int cursor_offset, Cursors.FitterCursor cursor, Cursors.FitterCursor.Element[] elements, int i, ref double x, ref double y, ref double y1, ref double y2, double space_after)
		{
			//	Pour �viter de redemander � chaque fois au geometry renderer de produire
			//	les informations de position des caract�res, on utilise un cache ici :
			
			if ((this.geometry_cache_version.HasAnythingChanged) ||
				(this.geometry_cache_start != position))
			{
				Internal.GeometryRenderer renderer = this.GenerateGeometryInformation (frame, position, cursor, elements, i, space_after);
				
				int n = renderer.ElementCount;
				
				if ((n > 1) &&
					(elements[i].IsTabulation) &&
					(elements[i].IsNewLine == false))
				{
					//	La tranche de texte se termine par une marque de tabulation.
					//	On doit mettre � jour sa largeur en d�terminant o� commence
					//	la ligne suivante :
					
					System.Diagnostics.Debug.Assert (renderer[n-2].Unicode == Unicode.Code.HorizontalTab);
					System.Diagnostics.Debug.Assert (renderer[n-1].Unicode == Unicode.Code.Null);
					System.Diagnostics.Debug.Assert (i+1 < elements.Length);
					
					int length = elements[i].Length;
					int next_pos = position + length;
					int next_i   = i + 1;
					
					Internal.GeometryRenderer next = this.GenerateGeometryInformation (frame, next_pos, cursor, elements, next_i, space_after);
					
					System.Diagnostics.Debug.Assert (next.HasTabBeforeText);
					
					renderer.DefineTab (next.TabOrigin, next.TabStop);
				}
				
				this.geometry_cache_version.Update ();
				
				this.geometry_cache_renderer = renderer;
				this.geometry_cache_start    = position;
			}
			
			System.Diagnostics.Debug.Assert (this.geometry_cache_renderer != null);
			System.Diagnostics.Debug.Assert (this.geometry_cache_version.HasFitterChanged == false);
			
			Internal.GeometryRenderer.Element item = this.geometry_cache_renderer[cursor_offset];
			
			if (item != null)
			{
				x  = item.X;
				y  = item.Y;
				y1 = item.Y1;
				y2 = item.Y2;
			}
			else
			{
				//	Pas d'�l�ment trouv� : on prend la ligne de base comme
				//	r�f�rence :
				
				x  = elements[i].LineStartX;
				y  = elements[i].LineBaseY;
				y1 = y + elements[i].LineDescender;
				y2 = y + elements[i].LineAscender;
				
				frame.MapToView (ref x, ref y);
			}
		}
		
		
		private void RenderElement(ITextRenderer renderer, ITextFrame frame, Cursors.FitterCursor cursor, Cursors.FitterCursor.Element[] elements, int i, Layout.Context layout, double space_after)
		{
			int last_i = elements.Length - 1;
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
				
				//	Transforme les coordonn�es internes en coordonn�es utilisables pour
				//	l'affichage :
				
				y1 -= oy;
				y2 -= oy;
				
				layout.Frame.MapToView (ref ox, ref oy);
				
				y1 += oy;
				y2 += oy;
				
				if (Cursors.FitterCursor.IsLastLine (elements, i))
				{
					//	C'est la derni�re ligne du paragraphe, donc il faut tenir compte
					//	de l'espace qui est ajout� apr�s et descendre y1 en cons�quence :
					
					y1 -= space_after;
				}
				
				if ((length > 0) &&
					(renderer.IsFrameAreaVisible (layout.Frame, ox, oy+desc, width, asc+desc)))
				{
					bool is_tab  = (i > 0) ? elements[i-1].IsTabulation : false;
					bool is_last = (i == last_i) || (elements[i].IsTabulation);
					
					Layout.StretchProfile profile = elements[i].Profile;
					
					//	Le render a besoin de pouvoir acc�der � diverses informations
					//	sur la g�om�trie de la ligne. On les initialise � cet effet :
					
					layout.DefineLineGeometry (oy, y1, y2, asc, desc);
					
					//	S'il y a une marque de tabulation � la fin de l'�l�ment pr�c�-
					//	dent, on va donner une chance au renderer de la peindre mainte-
					//	nant :
					
					if (is_tab)
					{
						double tab_origin = elements[i-1].IsNewLine ? elements[i].LineOriginX : elements[i-1].LineStartX + elements[i-1].LineWidth;
						double tab_stop   = elements[i].LineStartX;
						ulong  tab_code   = this.story.ReadChar (cursor, cursor.GetElementStartPosition (i) - 1);
						
						Properties.TabProperty  tab_property;
						Properties.TabsProperty tabs_property;
						
						this.story.TextContext.GetTabAndTabs (tab_code, out tab_property, out tabs_property);
						
						string tag = tab_property == null ? null : tab_property.TabTag;
						bool is_tab_defined = tabs_property == null ? false : tabs_property.ContainsTabTag (tag);
						
						renderer.RenderTab (layout, tag, tab_origin, tab_stop, tab_code, is_tab_defined);
					}
					
					layout.RenderLine (renderer, profile, length, ox, oy, width, i, is_tab, is_last);
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
		
		
		private TabStatus MeasureTabTextWidth(Layout.Context layout, Properties.TabProperty tab_property, int line_count, bool start_of_line, out double tab_x, out double width, out bool tab_indents)
		{
			//	D�termine la position de d�part du texte apr�s le tabulateur, sa
			//	largeur et l'indentation �ventuellement � appliquer � la suite du
			//	texte :
			
			tab_x = 0;
			width = 0;
			
			TabList tabs = layout.TextContext.TabList;
			
			double d = tabs.GetTabDisposition (tab_property);
			string docking_mark = tabs.GetTabDockingMark (tab_property);
			
			if ((docking_mark != null) &&
				(docking_mark.Length > 0))
			{
				d = 1.0;
			}
			
			double x1 = start_of_line ? layout.LineOriginX : layout.LineCurrentX;
			double x2 = tabs.GetTabPositionInPoints (tab_property);
			double x3 = layout.LineWidth - layout.RightMargin;
			
			//	Gestion des modes absolus/relatifs :
			
			switch (tabs.GetTabPositionMode (tab_property))
			{
				case TabPositionMode.Absolute:
				case TabPositionMode.AbsoluteIndent:
					break;
				
				case TabPositionMode.LeftRelative:
				case TabPositionMode.LeftRelativeIndent:
					
					//	Tabulateur relatif � la marge de gauche; on ajuste la
					//	position :
					
					x2 = layout.LeftBodyMargin + x2;
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("Tab position mode {0} not supported", tabs.GetTabPositionMode (tab_property)));
			}
			
			//	Gestion de l'indentation du paragraphe apr�s la marque de tabulation :
			
			switch (tabs.GetTabPositionMode (tab_property))
			{
				case TabPositionMode.Absolute:
				case TabPositionMode.LeftRelative:
					tab_indents = false;
					break;
				
				case TabPositionMode.AbsoluteIndent:
				case TabPositionMode.LeftRelativeIndent:
					tab_indents = true;
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("Tab position mode {0} not supported", tabs.GetTabPositionMode (tab_property)));
			}
			
			if ((x2 <= x1) &&
				(start_of_line))
			{
				//	Tabulateur en d�but de ligne, mais � gauche de la marge de
				//	gauche => tant pis, on le place hors des marges.
				
				x1 = x2;
			}
			
			double x_before = x2 - x1;
			double x_after  = x3 - x2;
			
			TabStatus              status  = TabStatus.Ok;
			Layout.BreakCollection result  = new Layout.BreakCollection ();
			Layout.Context         scratch = new Layout.Context (layout.TextContext, layout.Text, 0, null);
			
			scratch.ResetLineHeight ();
			scratch.RecordAscender (layout.LineAscender);
			scratch.RecordDescender (layout.LineDescender);
			scratch.RecordLineHeight (layout.LineHeight);
			
			if ((x_before < 0) ||
				(x_after < 0))
			{
				//	Tabulateur mal plac�... Demande un saut de ligne ! Mais on
				//	calcule encore au pr�alable la position qu'occupera le texte
				//	tabul� sur la ligne suivante :
				
				scratch.SelectMargins (line_count);
				
				x1 = scratch.LeftMargin;
				
				x_before = x2 - x1;
				x_after  = x3 - x2;
				
				status = TabStatus.ErrorNeedMoreRoom;
			}
			
			double room;
			double room_after;
			double room_before;

			
			//	D�termine la place disponible entre le texte qui se trouve
			//	avant le tabulateur et la marge droite, en tenant compte de
			//	la mani�re dont le texte est dispos�.
			
			if (d < 0.5)
			{
				double ratio = d / (1-d);				//	plut�t tabulateur align� � gauche
				
				room_after  = x_after;
				room_before = x_after * ratio;
				
				if ((x_before < room_before) &&
					(x_before > 0))
				{
					room_before = x_before;
					room_after  = x_before / ratio;
				}
			}
			else
			{
				double ratio = (1-d) / d;				//	plut�t tabulateur align� � droite
				
				room_before = x_before;
				room_after  = x_before * ratio;
				
				if ((x_after < room_after) &&
					(x_after > 0))
				{
					room_after  = x_after;
					room_before = x_after / ratio;
				}
			}
			
			room = room_before + room_after;
			
			scratch.DefineAvailableWidth (room);
			scratch.MoveTo (0, layout.TextOffset);
			scratch.DefineTabDockingMark (docking_mark);
			
			Layout.Status fit_status = scratch.Fit (ref result, line_count, true);
			
			if (fit_status == Layout.Status.ErrorNeedMoreText)
			{
				return TabStatus.ErrorNeedMoreText;
			}
			
			if ((fit_status == Layout.Status.OkFitEnded) ||
				(fit_status == Layout.Status.OkTabReached))
			{
				//	TODO: s�lectionner le r�sultat optimal
				
				width = result[result.Count-1].Profile.TotalWidth;
				tab_x = x2 - d * width;
				
				return status;
			}
			
			if (fit_status == Layout.Status.Ok)
			{
				if ((d == 0.0) ||
					(start_of_line))
				{
					//	TODO: s�lectionner le r�sultat optimal
					
					width = result[result.Count-1].Profile.TotalWidth;
					tab_x = x2 - d * width;
					
					return (d > 0) ? TabStatus.ErrorNeedMoreRoom : status;
				}
				else
				{
					//	On n'arrive pas � placer le texte du tabulateur sur la
					//	ligne en cours; il faut donc demander un passage � la
					//	ligne suivante :
					
					return TabStatus.ErrorNeedMoreRoom;
				}
			}
			
			return TabStatus.ErrorCannotFit;
		}
		
		
		private double ComputePenalty(double space_penalty, double break_penalty)
		{
			return space_penalty + break_penalty;
		}
		
		private int    ComputeParagraphLength(ulong[] text, int offset)
		{
			//	D�termine la longueur du paragraphe dans le texte pass� en entr�e, en
			//	commen�ant � l'offset indiqu�. Une marque de fin de texte ne fait pas
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
			//	Retourne un curseur tout neuf (ou reprend un curseur qui a �t�
			//	recycl� pr�c�demment, pour �viter de devoir en allouer � tour
			//	de bras).
			
			Cursors.FitterCursor cursor;
			
			if (this.free_cursors.Count > 0)
			{
				cursor = this.free_cursors.Pop () as Cursors.FitterCursor;
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
			//	Recycle le curseur pass� en entr�e. Il est simplement plac�
			//	dans la pile des curseurs disponibles.
			
			Debug.Assert.IsTrue (this.cursors.Contains (cursor));
			Debug.Assert.IsFalse (this.free_cursors.Contains (cursor));
			
			this.story.RecycleCursor (cursor);
			
			cursor.Clear ();
			
			this.free_cursors.Push (cursor);
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
		private System.Collections.Stack		free_cursors;
		
		private FrameList						frame_list;
		private int								frame_index;
		private double							frame_y;
		
		private double							line_skip_before;
		private int								line_fence;
		private bool							keep_with_prev;
		
		private IPageCollection					page_collection;
		
		private TextVersion						geometry_cache_version;
		private int								geometry_cache_start;
		private Internal.GeometryRenderer		geometry_cache_renderer;
	}
}
