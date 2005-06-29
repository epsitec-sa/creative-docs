//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextFitter s'occupe de la mise en page du texte au
	/// moyen des méthodes ClearAllMarks/GenerateAllMarks et du rendu
	/// du texte grâce aux méthodes RenderXxx.
	/// </summary>
	public class TextFitter
	{
		public TextFitter(TextStory story)
		{
			this.TextStory = story;
			
			this.frame_list      = new FrameList (this);
			this.page_collection = new DefaultPageCollection ();
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
			TextProcessor processor = new TextProcessor (this.story);
			processor.Process (new TextProcessor.Executor (this.ExecuteClear));
			this.frame_list.ClearCursorMap ();
		}
		
		public void GenerateAllMarks()
		{
			this.frame_index = 0;
			this.frame_y     = 0;
			
			this.line_skip_before = 0;
			this.line_fence       = -1;
			this.keep_with_prev   = false;
			
			TextProcessor processor = new TextProcessor (this.story);
			processor.Process (new TextProcessor.Executor (this.ExecuteGenerate));
			
			this.frame_list.ClearCursorMap ();
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
			
			renderer.RenderStartParagraph (layout);
			
			for (int i = 0; i < elements.Length; i++)
			{
				this.RenderElement (renderer, frame, elements, i, layout);
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
		
		
		public bool GetCursorGeometry(ICursor cursor, out ITextFrame frame, out double x, out double y, out int paragraph_line, out int line_character)
		{
			//	Détermine où se trouve le curseur spécifié : frame, position [x;y],
			//	numéro de ligne dans le paragraphe et numéro de caractère dans la
			//	ligne.
			
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
				
				Cursors.FitterCursor fitter_cursor = text.GetCursorInstance (infos[0].CursorId) as Cursors.FitterCursor;
				
				int paragraph_start = text.GetCursorPosition (infos[0].CursorId);
				int line_start      = paragraph_start;
				int tab_count       = 0;
				int tab_char_count  = 0;
				
				if (paragraph_start + fitter_cursor.ParagraphLength < position)
				{
					//	Le curseur se trouve dans une tranche de texte qui n'appartient
					//	à aucun paragraphe.
				}
				else
				{
					Cursors.FitterCursor.Element[] elements = fitter_cursor.Elements;
					
					//	Détermine dans quelle ligne du paragraphe se trouve le curseur :
					
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
							((position < line_end) || ((position == line_end) && (direction >= 0) && (! elements[i].IsTabulation)) || ((position == line_end) && (line_length == 0))))
						{
							//	Le curseur se trouve dans la ligne en cours d'analyse.
							//	On tient compte de la direction de déplacement pour
							//	déterminer le curseur se trouve à la fin de la ligne
							//	en cours ou au début de la ligne suivante.
							
							frame = this.FrameList[frame_index];
							
							x = elements[i].LineBaseX;
							y = elements[i].LineBaseY;
							
							paragraph_line = i - tab_count;
							line_character = position - line_start + tab_char_count;
							
							this.GetCursorGeometry (line_start, position - line_start, elements, i, ref x, ref y);
							
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
		
		
		
		private void ExecuteClear(Cursors.TempCursor temp_cursor, int pos, ref int length, out TextProcessor.Status status)
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
			
			Layout.Context         layout = new Layout.Context (this.story.TextContext, text, 0, this.frame_list);
			Layout.BreakCollection result = new Layout.BreakCollection ();
			
			layout.SelectFrame (this.frame_index, this.frame_y);
			
			layout.LineSkipBefore            = this.line_skip_before;
			layout.KeepWithPreviousParagraph = this.keep_with_prev;
			layout.FenceLineCount            = this.line_fence;
			
			int    paragraph_start_offset      = 0;
			int    paragraph_start_frame_index = layout.FrameIndex;;
			double paragraph_start_frame_y     = layout.FrameY;
			
restart_paragraph_layout:
			
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
						
						layout.LineSkipBefore            = this.line_skip_before;
						layout.KeepWithPreviousParagraph = this.keep_with_prev;
						
						break;
					
					case Layout.Status.OkHiddenFitEnded:
						
						continuation = false;
						reset_line_h = true;
						
						break;
					
					case Layout.Status.RestartParagraphLayout:
						
						if (layout.FenceLineCount > 0)
						{
							//	Il faut se replacer dans le frame correspondant au début
							//	du paragraphe en cours :
							
							layout.SelectFrame (paragraph_start_frame_index, paragraph_start_frame_y);
						}
						
						layout.MoveTo (0, paragraph_start_offset);
						goto restart_paragraph_layout;
					
					case Layout.Status.RewindParagraphAndRestartLayout:
						
						//	Retourne en arrière, jusqu'au début du paragraphe qui précède
						//	le paragraphe actuel, puis relance l'opération au complet.
						
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
						//	l'analyser et déterminer si elle peut être placée sur
						//	la même ligne que le texte qui précède :
						
						System.Diagnostics.Debug.Assert (layout.TextOffset >= 0);
						System.Diagnostics.Debug.Assert (layout.TextOffset <= text.Length);
						
						//	Si le système de layout change de frame en cours de route,
						//	cela implique que la ligne était vide et que l'on doit con-
						//	sidérer le début de frame comme notre nouvelle référence.
						
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
						
						tab_status = this.MeasureTabTextWidth (layout, tab_property, line_count, false, out tab_x, out tab_dx);
						
						if (tab_status == TabStatus.ErrorNeedMoreText)
						{
							length = paragraph_start_offset;
							status = TextProcessor.Status.Continue;
							return;
						}
						else if (tab_status == TabStatus.ErrorNeedMoreRoom)
						{
							//	Le tabulateur ne tient plus sur cette ligne. Force un passage
							//	à la ligne.
							
							tab_new_line = true;
							tab_status   = this.MeasureTabTextWidth (layout, tab_property, line_count, true, out tab_x, out tab_dx);
							
							if (tab_status == TabStatus.ErrorNeedMoreRoom)
							{
								//	Même sur une ligne nouvelle, il n'y a pas la place pour
								//	positionner le texte selon les besoins du tabulateur;
								//	on le cale simplement sur la marge :
								
								layout.MoveTo (layout.LeftMargin, layout.TextOffset);
							}
							else
							{
								layout.MoveTo (tab_x, layout.TextOffset);
							}
						}
						else if (tab_status == TabStatus.Ok)
						{
							//	Le tabulateur occupe la même ligne que le texte qui précède.
							
							layout.MoveTo (tab_x, layout.TextOffset);
						}
						else
						{
							throw new System.NotImplementedException ();
						}
						
						continuation = true;
						
						break;
					
					case Layout.Status.ErrorNeedMoreRoom:
						end_of_text = true;
						break;
					
					default:
						throw new System.InvalidOperationException (string.Format ("Invalid layout status received: {0}.", layout_status));
				}
				
				//	Le système de layout propose un certain nombre de points de découpe
				//	possibles pour la ligne. Il faut maintenant déterminer lequel est le
				//	meilleur.
				
				int offset   = line_start_offset;
				int n_breaks = result.Count;
				
				Layout.StretchProfile profile = null;
				
				if (n_breaks > 1)
				{
					double penalty = Layout.StretchProfile.MaxPenalty;
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
					
					element.Length        = offset - line_start_offset;
					element.Profile       = profile;
					element.FrameIndex    = layout.FrameIndex;
					element.LineBaseX     = layout.LineStartX;
					element.LineBaseY     = layout.Y;
					element.LineWidth     = (continuation && !tab_new_line) ? result[result.Count-1].Profile.TotalWidth : layout.AvailableWidth;
					element.LineAscender  = layout.LineAscender;
					element.LineDescender = layout.LineDescender;
					element.IsTabulation  = layout_status == Layout.Status.OkTabReached;
					
					list.Add (element);
				}
				
				if (layout_status == Layout.Status.OkHiddenFitEnded)
				{
					offset = layout.TextOffset;
				}
				else
				{
					layout.TextOffset = offset;
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
						//	Le paragraphe a été tronqué, car il n'y a plus assez de place dans
						//	les ITextFrame pour y placer le texte. Il faut générer un Element
						//	décrivant la fin du paragraphe :
						
						Cursors.FitterCursor.Element element = new Cursors.FitterCursor.Element ();
						
						int paragraph_length = this.ComputeParagraphLength (text, line_start_offset);
						
						if (paragraph_length < 0)
						{
							//	La fin du paragraphe n'a pas pu être trouvée; demande à l'appelant
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
						
						mark.AddRange (list);
						mark.DefineParagraphY (paragraph_start_frame_y);
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
			
			//	Il faut encore supprimer le curseur correspondant à la marque de
			//	début du paragraphe :
			
			this.RecycleFitterCursor (para_1);
			
			return distance;
		}
		
		
		private void GetCursorGeometry(int position, int cursor_offset, Cursors.FitterCursor.Element[] elements, int i, ref double x, ref double y)
		{
			ITextFrame frame  = this.FrameList[elements[i].FrameIndex];
			int        length = elements[i].Length;
			ulong[]    text   = new ulong[length];
			
			this.story.ReadText (position, length, text);
			
			Layout.Context            layout   = new Layout.Context (this.story.TextContext, text, 0, this.frame_list);
			Internal.GeometryRenderer renderer = new Internal.GeometryRenderer ();
			
			layout.DisableFontBaselineOffset  = true;
			layout.RendererNeedsTextAndGlyphs = true;
			
			this.RenderElement (renderer, frame, elements, i, layout);
			
			Internal.GeometryRenderer.Element item = renderer[cursor_offset];
			
			if (item != null)
			{
				x = item.X;
				y = item.Y;
			}
			else
			{
				//	Pas d'élément trouvé : on prend la ligne de base comme
				//	référence :
				
				x = elements[i].LineBaseX;
				y = elements[i].LineBaseY;
				
				frame.MapToView (ref x, ref y);
			}
		}
		
		private void RenderElement(ITextRenderer renderer, ITextFrame frame, Cursors.FitterCursor.Element[] elements, int i, Layout.Context layout)
		{
			int last_i = elements.Length - 1;
			int index  = elements[i].FrameIndex;
			int length = elements[i].Length;
			
			layout.SelectFrame (index, 0);
			
			if ((frame == null) ||
				(layout.Frame == frame))
			{
				double ox    = elements[i].LineBaseX;
				double oy    = elements[i].LineBaseY;
				double width = elements[i].LineWidth;
				double asc   = elements[i].LineAscender;
				double desc  = elements[i].LineDescender;
				
				layout.Frame.MapToView (ref ox, ref oy);
				
				if ((length > 0) &&
					(renderer.IsFrameAreaVisible (layout.Frame, ox, oy+desc, width, asc+desc)))
				{
					bool is_tab  = (i > 0) ? elements[i-1].IsTabulation : false;
					bool is_last = (i == last_i) || (elements[i].IsTabulation);
					
					Layout.StretchProfile profile = elements[i].Profile;
					
					layout.Y             = oy;
					layout.LineAscender  = asc;
					layout.LineDescender = desc;
					
					layout.RenderLine (renderer, profile, length, ox, oy, width, i, is_tab, is_last);
				}
				else
				{
					layout.InvisibleLine (renderer, length, ox, oy);
				}
			}
			
			layout.TextOffset += length;
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
		
		private Properties.KeepProperty GetKeepProperty(ICursor cursor)
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
		
		
		private enum TabStatus
		{
			Ok,
			ErrorNeedMoreText,
			ErrorNeedMoreRoom,
			ErrorCannotFit
		}
		
		private TabStatus MeasureTabTextWidth(Layout.Context layout, Properties.TabProperty tab_property, int line_count, bool start_of_line, out double tab_x, out double width)
		{
			tab_x = 0;
			width = 0;
			
			double d = tab_property.Disposition;
			
			double x1 = start_of_line ? layout.LeftMargin : layout.X;
			double x2 = tab_property.PositionInPoints;
			double x3 = layout.LineWidth - layout.RightMargin;
			
			double x_before = x2 - x1;
			double x_after  = x3 - x2;
			
			TabStatus              status  = TabStatus.Ok;
			Layout.BreakCollection result  = new Layout.BreakCollection ();
			Layout.Context         scratch = new Layout.Context (layout.TextContext, layout.Text, 0, null);
			
			scratch.ResetLineHeight ();
			scratch.RecordAscender (layout.LineAscender);
			scratch.RecordDescender (layout.LineDescender);
			scratch.RecordLineHeight (layout.LineHeight);
			
//-			Debug.Assert.IsTrue (layout.Disposition == 0);
//-			Debug.Assert.IsTrue (layout.Justification == 0);
			
			if ((x_before <= 0) ||
				(x_after <= 0))
			{
				//	Tabulateur mal placé... Demande un saut de ligne ! Mais on
				//	calcule encore au préalable la position qu'occupera le texte
				//	tabulé sur la ligne suivante :
				
				scratch.SelectMargins (line_count);
				
				x1 = scratch.LeftMargin;
				
				x_before = x2 - x1;
				x_after  = x3 - x2;
				
				status = TabStatus.ErrorNeedMoreRoom;
			}
			
			double room;
			double room_after;
			double room_before;

			
			//	Détermine la place disponible entre le texte qui se trouve
			//	avant le tabulateur et la marge droite, en tenant compte de
			//	la manière dont le texte est disposé.
			
			if (d < 0.5)
			{
				double ratio = d / (1-d);				//	plutôt tabulateur aligné à gauche
				
				room_after  = x_after;
				room_before = x_after * ratio;
				
				if (x_before < room_before)
				{
					room_before = x_before;
					room_after  = x_before / ratio;
				}
			}
			else
			{
				double ratio = (1-d) / d;				//	plutôt tabulateur aligné à droite
				
				room_before = x_before;
				room_after  = x_before * ratio;
				
				if (x_after < room_after)
				{
					room_after  = x_after;
					room_before = x_after / ratio;
				}
			}
			
			room = room_before + room_after;
			
			scratch.DefineAvailableWidth (room);
			scratch.MoveTo (0, layout.TextOffset);
			
			Layout.Status fit_status = scratch.Fit (ref result, line_count, true);
			
			if (fit_status == Layout.Status.ErrorNeedMoreText)
			{
				return TabStatus.ErrorNeedMoreText;
			}
			
			if ((fit_status == Layout.Status.OkFitEnded) ||
				(fit_status == Layout.Status.OkTabReached))
			{
				//	TODO: sélectionner le résultat optimal
				
				width = result[result.Count-1].Profile.TotalWidth;
				tab_x = x2 - d * width;
				
				return status;
			}
			
			if (fit_status == Layout.Status.Ok)
			{
				if ((d == 0.0) ||
					(start_of_line))
				{
					//	TODO: sélectionner le résultat optimal
					
					width = result[result.Count-1].Profile.TotalWidth;
					tab_x = x2 - d * width;
					
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
		
		private double ComputePenalty(double space_penalty, double break_penalty)
		{
			return space_penalty + break_penalty;
		}
		
		private int ComputeParagraphLength(ulong[] text, int offset)
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
				
				if (analyzer.IsBreak (code))
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
			//	Recycle le curseur passé en entrée. Il est simplement placé
			//	dans la pile des curseurs disponibles.
			
			Debug.Assert.IsTrue (this.cursors.Contains (cursor));
			Debug.Assert.IsFalse (this.free_cursors.Contains (cursor));
			
			this.story.RecycleCursor (cursor);
			
			cursor.Clear ();
			
			this.free_cursors.Push (cursor);
		}
		
		
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
	}
}
