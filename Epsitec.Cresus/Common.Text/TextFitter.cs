//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// Summary description for TextFitter.
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
			this.Process (new Execute (this.ExecuteClear));
			this.frame_list.ClearCursorMap ();
		}
		
		public void GenerateAllMarks()
		{
			this.frame_index = 0;
			this.frame_y     = 0;
			
			this.line_skip_before = 0;
			this.line_fence       = -1;
			this.keep_with_prev   = false;
			
			this.Process (new Execute (this.ExecuteGenerate));
			
			this.frame_list.ClearCursorMap ();
		}
		
		
		public void RenderParagraph(ICursor cursor, ITextRenderer renderer)
		{
			this.RenderParagraphInTextFrame (cursor, renderer, null);
		}
		
		public void RenderParagraphInTextFrame(ICursor cursor, ITextRenderer renderer, ITextFrame frame)
		{
			Cursors.FitterCursor c = cursor as Cursors.FitterCursor;
			
			if (c == null)
			{
				throw new System.ArgumentException ("Not a valid FitterCursor.", "cursor");
			}
			
			ulong[] text;
			int length = c.ParagraphLength;
			
			text   = new ulong[length];
			length = this.story.ReadText (c, length, text);
			
			Layout.Context layout = new Layout.Context (this.story.TextContext, text, 0, this.frame_list);
			
			renderer.RenderBeginParagraph (layout);
			
			int n = c.Elements.Length;
			
			for (int i = 0; i < n; i++)
			{
				int index = c.Elements[i].FrameIndex;
				int count = c.Elements[i].Length;
				
				layout.SelectFrame (index, 0);
				
				if ((frame == null) ||
					(layout.Frame == frame))
				{
					double ox    = c.Elements[i].LineBaseX;
					double oy    = c.Elements[i].LineBaseY;
					double width = c.Elements[i].LineWidth;
					double asc   = c.Elements[i].LineAscender;
					double desc  = c.Elements[i].LineDescender;
					
					layout.Frame.MapToView (ref ox, ref oy);
					
					if ((count > 0) &&
						(renderer.IsFrameAreaVisible (layout.Frame, ox, oy+desc, width, asc+desc)))
					{
						bool is_tab  = (i > 0) ? c.Elements[i-1].IsTabulation : false;
						bool is_last = (i == n-1) || (c.Elements[i].IsTabulation);
						
						Layout.StretchProfile profile = c.Elements[i].Profile;
						
						layout.Y             = oy;
						layout.LineAscender  = asc;
						layout.LineDescender = desc;
						
						layout.RenderLine (renderer, profile, count, ox, oy, width, i, is_tab, is_last);
					}
					else
					{
						layout.InvisibleLine (renderer, count, ox, oy);
					}
				}
				
				layout.TextOffset += count;
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
		
		
		protected void Process(Execute method)
		{
			//	Ex�cute une m�thode pour tout le texte, en proc�dant par tranches
			//	(ex�cution it�rative).
			
			int pos = 0;
			
			Cursors.TempCursor cursor = new Cursors.TempCursor ();
			
			this.story.NewCursor (cursor);
			
			try
			{
				for (;;)
				{
					//	TODO: lock et d�tection d'alt�rations du texte (et de la liste des
					//	ITextFrame li�s � ce TextFitter).
					
					int max  = this.story.TextLength;
					int step = 10000;
					
				again:
					int  length = System.Math.Min (max - pos, step);
					bool restart;
					
					if (length <= 0)
					{
						break;
					}
					
					method (cursor, pos, ref length, out restart);
					
					if (restart)
					{
						//	Si la m�thode a demand� une seconde chance, il se peut que
						//	le curseur ait �t� d�plac�; il faut donc resynchroniser la
						//	position :
						
						pos = this.story.GetCursorPosition (cursor);
						goto again;
					}
					
					if (length == 0)
					{
						//	La m�thode de traitement n'a rien pu faire avec le texte pass�
						//	en entr�e, vraisemblablement parce que le paragraphe mesure plus
						//	de 'step' caract�res. Il faut donc augmenter cette limite et
						//	tenter un nouvel essai.
						
						Debug.Assert.IsTrue (length < max-pos);
						
						step += 10000;
						goto again;
					}
					
					if (length < 0)
					{
						//	Avortement demand� par la m�thode de traitement.
						
						break;
					}
					
					this.story.MoveCursor (cursor, length);
					pos += length;
				}
			}
			finally
			{
				this.story.RecycleCursor (cursor);
			}
		}
		
		protected void ExecuteClear(Cursors.TempCursor temp_cursor, int pos, ref int length, out bool restart)
		{
			//	Supprime les marques de d�coupe de lignes repr�sent�es par des
			//	curseurs (instances de Cursors.FitterCursor).
			
			CursorInfo[] cursors = this.story.TextTable.FindCursors (pos, length, Cursors.FitterCursor.Filter);
			
			for (int i = 0; i < cursors.Length; i++)
			{
				ICursor cursor = this.story.TextTable.GetCursorInstance (cursors[i].CursorId);
				this.RecycleFitterCursor (cursor);
			}
			
			restart = false;
		}
		
		protected void ExecuteGenerate(Cursors.TempCursor cursor, int pos, ref int length, out bool restart)
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
			
			restart = false;
			
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
				
				Layout.Status status = layout.Fit (ref result, line_count, continuation);
				
				bool tab_new_line = false;
				bool end_of_text  = false;
				
				this.frame_index = layout.FrameIndex;
				this.frame_y     = layout.FrameY;
				
				switch (status)
				{
					case Layout.Status.ErrorNeedMoreText:
						this.frame_index = paragraph_start_frame_index;
						this.frame_y     = paragraph_start_frame_y;
						length = paragraph_start_offset;
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
					
					case Layout.Status.RestartParagraphLayout:
						
						if (layout.FenceLineCount > 0)
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
						
						this.RewindToPreviousParagraph (cursor, pos, paragraph_start_offset);
						
						restart = true;
						length  = 0;
						
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
						
						layout.TextContext.GetTab (text[layout.TextOffset-1], out tab_property);
						
						Debug.Assert.IsNotNull (tab_property);
						
						double tab_x;
						double tab_dx;
						
						tab_status = this.MeasureTabTextWidth (layout, tab_property, line_count, false, out tab_x, out tab_dx);
						
						if (tab_status == TabStatus.ErrorNeedMoreText)
						{
							length = paragraph_start_offset;
							return;
						}
						else if (tab_status == TabStatus.ErrorNeedMoreRoom)
						{
							//	Le tabulateur ne tient plus sur cette ligne. Force un passage
							//	� la ligne.
							
							tab_new_line = true;
							tab_status   = this.MeasureTabTextWidth (layout, tab_property, line_count, true, out tab_x, out tab_dx);
							
							if (tab_status == TabStatus.ErrorNeedMoreRoom)
							{
								//	M�me sur une ligne nouvelle, il n'y a pas la place pour
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
							//	Le tabulateur occupe la m�me ligne que le texte qui pr�c�de.
							
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
						throw new System.InvalidOperationException (string.Format ("Invalid layout status received: {0}.", status));
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
						Debug.Assert.IsTrue (status == Layout.Status.OkFitEnded);
						
						offset     -= 1;
						end_of_text = true;
					}
					
					element.Length        = offset - line_start_offset;
					element.Profile       = profile;
					element.FrameIndex    = layout.FrameIndex;
					element.LineBaseX     = layout.LineBeginX;
					element.LineBaseY     = layout.Y;
					element.LineWidth     = (continuation && !tab_new_line) ? result[result.Count-1].Profile.TotalWidth : layout.AvailableWidth;
					element.LineAscender  = layout.LineAscender;
					element.LineDescender = layout.LineDescender;
					element.IsTabulation  = status == Layout.Status.OkTabReached;
					
					list.Add (element);
				}
				
				layout.TextOffset = offset;
				
				if (status == Layout.Status.OkTabReached)
				{
					line_start_offset = offset;
					
					if (tab_new_line)
					{
						line_count++;
					}
					else
					{
						layout.SelectFrame (def_frame_index, def_frame_y);
					}
				}
				else
				{
					if (status == Layout.Status.ErrorNeedMoreRoom)
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
							return;
						}
						
						element.Length     = paragraph_length;
						element.FrameIndex = -1;
						
						list.Add (element);
					}
					
					if ((status == Layout.Status.OkFitEnded) ||
						(status == Layout.Status.ErrorNeedMoreRoom))
					{
						Debug.Assert.IsTrue (list.Count > 0);
						
						Cursors.FitterCursor mark = this.NewFitterCursor ();
						
						mark.AddRange (list);
						mark.DefineParagraphY (paragraph_start_frame_y);
						list.Clear ();
						
						story.MoveCursor (mark, pos + paragraph_start_offset);
						
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
					return;
				}
			}
		}
		
		
		protected void RewindToPreviousParagraph(Cursors.TempCursor cursor, int position, int offset)
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
			
			this.story.MoveCursor (cursor, offset - para_length);
			
			//	Il faut encore supprimer le curseur correspondant � la marque de
			//	d�but du paragraphe :
			
			this.RecycleFitterCursor (para_1);
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
		
		
		protected enum TabStatus
		{
			Ok,
			ErrorNeedMoreText,
			ErrorNeedMoreRoom,
			ErrorCannotFit
		}
		
		protected TabStatus MeasureTabTextWidth(Layout.Context layout, Properties.TabProperty tab_property, int line_count, bool start_of_line, out double tab_x, out double width)
		{
			tab_x = 0;
			width = 0;
			
			double d = tab_property.Disposition;
			
			double x1 = start_of_line ? layout.LeftMargin : layout.X;
			double x2 = tab_property.Position;
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
				
				if (x_before < room_before)
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
		
		protected double ComputePenalty(double space_penalty, double break_penalty)
		{
			return space_penalty + break_penalty;
		}
		
		protected int ComputeParagraphLength(ulong[] text, int offset)
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
				
				if (analyzer.IsBreak (code))
				{
					return i - offset + 1;
				}
			}
			
			return -1;
		}
		
		
		protected Cursors.FitterCursor NewFitterCursor()
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
				cursor = new Cursors.FitterCursor ();
				this.cursors.Add (cursor);
			}
			
			this.story.NewCursor (cursor);
			
			return cursor;
		}
		
		protected void RecycleFitterCursor(ICursor cursor)
		{
			//	Recycle le curseur pass� en entr�e. Il est simplement plac�
			//	dans la pile des curseurs disponibles.
			
			Debug.Assert.IsTrue (this.cursors.Contains (cursor));
			Debug.Assert.IsFalse (this.free_cursors.Contains (cursor));
			
			this.story.RecycleCursor (cursor);
			
			cursor.Clear ();
			
			this.free_cursors.Push (cursor);
		}
		
		
		protected delegate void Execute(Cursors.TempCursor cursor, int pos, ref int length, out bool restart);
		
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
