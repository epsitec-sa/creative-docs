//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// La classe TextContext stocke le contexte de layout lié à un paragraphe
	/// entier.
	/// </summary>
	public class Context
	{
		public Context(Text.TextContext text_context, ulong[] text, int start, double oy_base, double line_height, double line_width, double mx_left, double mx_right, double break_fence_before, double break_fence_after)
		{
			this.buffer = null;
			
			this.text_context = text_context;
			
			this.text       = text;
			this.text_start = start;
			
			this.oy_base = oy_base;
			this.oy_max  = oy_base;
			this.oy_min  = oy_base;
			
			this.line_height = line_height;
			this.line_width  = line_width;
			
			this.ox       = mx_left;
			this.mx_left  = mx_left;
			this.mx_left_body = mx_left;
			this.mx_right = mx_right;
			this.x_origin = this.mx_left;
			
			this.break_fence_before = break_fence_before;
			this.break_fence_after  = break_fence_after;
			
			this.left_to_right = 0;
		}
		
		public Context(Text.TextContext context, ulong[] text, int start, FrameList frame_list)
		{
			this.text_context = context;
			this.text         = text;
			this.text_start   = start;
			this.frame_list   = frame_list;
		}
		
		
		public Layout.BaseEngine				LayoutEngine
		{
			get
			{
				return this.layout_engine;
			}
		}
		
		public Properties.LayoutProperty		LayoutProperty
		{
			get
			{
				return this.layout_property;
			}
		}
		
		
		public ulong[]							Text
		{
			get
			{
				return this.text;
			}
		}
		
		public Text.TextContext					TextContext
		{
			get
			{
				return this.text_context;
			}
		}
		
		public int								TextOffset
		{
			get
			{
				return this.text_offset;
			}
		}
		
		public StretchProfile					TextStretchProfile
		{
			get
			{
				return this.text_profile;
			}
		}
		
		public StretchProfile.Scales			TextStretchScales
		{
			get
			{
				return this.text_scales;
			}
		}
		
		public double							TextStretchGlue
		{
			get
			{
				return this.text_glue;
			}
		}
		
		public double							TextWidth
		{
			get
			{
				return this.text_width;
			}
		}
		
		
		public Unicode.Code						LastProcessedCode
		{
			get
			{
				return (Unicode.Code) Unicode.Bits.GetCode (this.LastProcessedCharacter);
			}
		}
		
		public ulong							LastProcessedCharacter
		{
			get
			{
				if ((this.text_offset > 0) &&
					(this.text != null) &&
					(this.text.Length >= this.text_offset))
				{
					return this.text[this.text_offset-1];
				}
				
				return 0;
			}
		}
		
		
		public double							LineCurrentX
		{
			get
			{
				return this.ox;
			}
		}
		
		public double							LineStartX
		{
			get
			{
				return this.ox_line_start;
			}
		}
		
		public double							LineOriginX
		{
			get
			{
				return this.x_origin;
			}
		}
		
		public double							LineBaseY
		{
			get
			{
				return this.oy_base;
			}
		}
		
		public double							LineY1
		{
			get
			{
				return this.line_y1;
			}
		}
		
		public double							LineY2
		{
			get
			{
				return this.line_y2;
			}
		}
		
		public double							MaxY
		{
			get
			{
				return this.oy_max;
			}
		}
		
		public double							MinY
		{
			get
			{
				return this.oy_min;
			}
		}
		
		
		public double							Justification
		{
			get
			{
				return this.justification;
			}
		}
		
		public double							Disposition
		{
			get
			{
				return this.disposition;
			}
		}
		
		public double							BreakFenceBefore
		{
			get
			{
				return this.break_fence_before;
			}
		}
		
		public double							BreakFenceAfter
		{
			get
			{
				return this.break_fence_after;
			}
		}
		
		public bool								EnableHyphenation
		{
			get
			{
				return this.enable_hyphenation;
			}
		}
		
		public BreakMode						BreakMode
		{
			get
			{
				return this.break_mode;
			}
		}
		
		
		public double							LeftMargin
		{
			get
			{
				return this.use_tab_indentation ? this.tab_indentation_x : this.mx_left;
			}
		}
		
		public double							LeftBodyMargin
		{
			get
			{
				return this.mx_left_body;
			}
		}
		
		public double							RightMargin
		{
			get
			{
				return this.mx_right;
			}
		}
		
		public double							LineHeight
		{
			get
			{
				return this.line_height;
			}
		}
		
		public double							LineWidth
		{
			get
			{
				return this.line_width;
			}
		}
		
		public double							LineAscender
		{
			get
			{
				return this.oy_max - this.oy_base;
			}
		}
		
		public double							LineDescender
		{
			get
			{
				return this.oy_min - this.oy_base;
			}
		}
		
		public double							LineSkipBefore
		{
			//	Ne pas confondre LineSpaceBefore qui provient de LeadingProperty
			//	et LineSkipBefore qui est la valeur imposée par le TextFitter en
			//	début de paragraphe, à cause d'une fin de paragraphe précédente.
			
			get
			{
				return this.line_skip_before;
			}
		}
		
		public double							LineSpaceBefore
		{
			get
			{
				return this.line_space_before;
			}
		}
		
		public double							LineSpaceAfter
		{
			get
			{
				return this.line_space_after;
			}
		}
		
		
		public Layout.FrameLineFenceDictionary	FrameFences
		{
			get
			{
				return this.frame_fences;
			}
		}
		
		public bool								KeepWithPreviousParagraph
		{
			get
			{
				return this.keep_with_prev_para;
			}
		}
		
		public bool								KeepWithNextParagraph
		{
			get
			{
				return this.keep_with_next_para;
			}
		}
		
		
		public bool								ShowControlCharacters
		{
			get
			{
				return this.show_control_characters;
			}
		}
		
		public bool								IsSimpleRenderingDisabled
		{
			get
			{
				return this.disable_simple_rendering;
			}
		}
		
		public bool								IsFontBaselineOffsetDisabled
		{
			get
			{
				return this.disable_font_baseline_offset;
			}
		}
		
		
		public double							AvailableWidth
		{
			get
			{
				return this.line_width - this.LeftMargin - this.RightMargin;
			}
		}
		
		public bool								IsLeftToRight
		{
			get
			{
				return (this.left_to_right & 1) == 0;
			}
		}
		
		public bool								IsRightToLeft
		{
			get
			{
				return (this.left_to_right & 1) == 1;
			}
		}
		
		
		public ITextFrame						Frame
		{
			get
			{
				return this.frame;
			}
		}
		
		public int								FrameIndex
		{
			get
			{
				return this.frame_index;
			}
		}
		
		public double							FrameY
		{
			get
			{
				return this.frame_y;
			}
		}
		
		public double							FrameYLine
		{
			get
			{
				//	Retourne la position Y de la ligne dans le frame, alors que FrameY retourne
				//	la prochaine position Y à utiliser...
				
				return this.frame_y_line;
			}
		}
		
		
		public Properties.AbstractXlineProperty[] XlineProperties
		{
			get
			{
				if ((this.xline_properties == null) ||
					(this.xline_properties.Length == 0))
				{
					return new Properties.AbstractXlineProperty[0];
				}
				
				Properties.AbstractXlineProperty[] properties = new Properties.AbstractXlineProperty[this.xline_properties.Length];
				
				this.xline_properties.CopyTo (properties, 0);
				
				return properties;
			}
		}
		
		public Layout.XlineRecord[]				XlineRecords
		{
			get
			{
				if ((this.xline_records == null) ||
					(this.xline_records.Count == 0))
				{
					return new Layout.XlineRecord[0];
				}
				
				Layout.XlineRecord[] records = new Layout.XlineRecord[this.xline_records.Count];
				
				this.xline_records.CopyTo (records);
				
				return records;
			}
		}
		
		
		public Layout.Status Fit(ref Layout.BreakCollection result, int paragraph_line_count)
		{
			this.ResetLineHeight ();
			
			return this.Fit (ref result, paragraph_line_count, false);
		}

		public Layout.Status Fit(ref Layout.BreakCollection result, int paragraph_line_count, bool continuation)
		{
			//	Détermine les points de découpe pour le texte, selon le contexte
			//	courant.
			
			if (this.text_start + this.text_offset >= this.text.Length)
			{
				return Layout.Status.ErrorNeedMoreText;
			}
			
			double initial_line_ascender  = this.LineAscender;
			double initial_line_descender = this.LineDescender;
			double initial_line_height    = this.LineHeight;
			
			this.SelectLayoutEngine (this.text_offset);
			
			if (continuation)
			{
				//	TODO: gérer la continuation...
				
				this.mx_left = this.ox;
			}
			else
			{
				this.SelectMarginsAndJustification (this.text_offset, paragraph_line_count, false);
				this.SelectLineHeightAndLeading (this.text_offset, initial_line_height, initial_line_ascender, initial_line_descender);
				this.SelectVerticalAlignment (paragraph_line_count);
				this.SelectKeep (this.text_offset);
				
				this.ox = this.LeftMargin;
			}
			
			this.ox_line_start = this.ox;
			this.break_mode    = BreakMode.Default;
			
			Debug.Assert.IsNotNull (this.layout_engine);
			Debug.Assert.IsNotNull (this.text);
			Debug.Assert.IsNotNull (this.text_context);

			Snapshot snapshot = new Snapshot (this);
			
			if (result == null)
			{
				result = new Layout.BreakCollection ();
			}
			else
			{
				result.Clear ();
			}
			
			this.text_profile = new StretchProfile ();
			
			int frame_index = this.frame_index;
			
restart:	
			double def_line_height = 0;
			double def_line_width  = 0;
			
			if (! continuation)
			{
				int fence = this.frame_fences[frame_index];
				
				while ((fence > -1) &&
					   (paragraph_line_count >= fence))
				{
					frame_index++;
					fence = this.frame_fences[frame_index];
				}
				
				if (paragraph_line_count == 0)
				{
					//	Sélectionne le frame qui convient pour ce paragraphe (selon
					//	les réglages de la propriété Keep.ParagraphStartMode) :
					
					this.UpdateFrameIndex (ref frame_index, this.frame_y == 0, this.para_start_mode);
				}
			}
			
			if (frame_index != this.frame_index)
			{
				if ((frame_index < this.frame_list.Count) &&
					(frame_index > -1))
				{
					//	Reprend avec un autre frame. Vérifions d'abord si ce changement
					//	de frame est permis ici :
					
					if ((this.frame_list[frame_index-1] is SingleLineTextFrame) ||
						(this.frame_list[frame_index-0] is SingleLineTextFrame))
					{
						goto select_new_frame;
					}
					
					if ((paragraph_line_count < this.keep_start_lines) &&
						(paragraph_line_count > 0))
					{
						//	Il n'y a pas assez de lignes de texte consécutives en début
						//	de paragraphe !
						
						this.frame_fences.Add (frame_index-1, 0);
						this.frame_first_line = 0;
						
						if (this.keep_with_prev_para)
						{
							return Layout.Status.RewindParagraphAndRestartLayout;
						}
						
						this.SelectFrame (frame_index, 0);
						
						return Layout.Status.RestartParagraphLayout;
					}
					if ((paragraph_line_count == 0) &&
						(this.keep_with_prev_para) &&
						(this.para_start_mode == Properties.ParagraphStartMode.Anywhere))
					{
						//	Le paragraphe ne peut pas être dissocié de celui qui précède.
						//	Un changement de frame ici va affecter le paragraphe qui
						//	précède immédiatement (récursivement)
						
						this.frame_fences.Add (frame_index-1, 0);
						
						return Layout.Status.RewindParagraphAndRestartLayout;
					}
					
				select_new_frame:
					
					//	On reprend tout à zéro depuis ici :
					
					this.SelectLayoutEngine (this.text_offset);
					this.SelectMarginsAndJustification (this.text_offset, paragraph_line_count, false);
					this.SelectFrame (frame_index, 0);
					this.SelectLineHeightAndLeading (this.text_offset, initial_line_height, initial_line_ascender, initial_line_descender);
					this.SelectVerticalAlignment (paragraph_line_count);
					
					//	Prend note du nouveau frame de référence et de la position dans
					//	le paragraphe au sommet du frame :
					
					this.frame_first_line = paragraph_line_count;
					
					snapshot.FixFrame (this.frame_index, this.frame_y, this.frame_first_line);
				}
				else
				{
					//	Il n'y a plus de ITextFrame ! On s'arrête donc immédiatement
					//	avec une erreur.
					
					return Layout.Status.ErrorNeedMoreRoom;
				}
			}
			
			int pass = 0;
			
			for (;;)
			{
				if ((pass > 1) &&
					(result.Count > 0))
				{
					return Layout.Status.Ok;
				}
				
				if (this.frame != null)
				{
					this.SelectLineHeightAndLeading (this.text_offset, initial_line_height, initial_line_ascender, initial_line_descender);
					
					double line_ascender  = this.LineAscender;
					double line_descender = this.LineDescender;
					double line_height    = this.LineHeight;
					
					double ox, oy, dx;
					double next_frame_y;
					
					oy = this.frame_y;
					
					this.line_y2 = oy;
					
					if ((paragraph_line_count == 0) &&
						(this.frame_y != 0))
					{
						//	A la première ligne du paragraphe, on ajoute l'espace "avant"
						//	tel que défini par la propriété de "leading". Mais on n'ajoute
						//	cet espace que si on n'est pas en sommet de frame :
						
						if (! continuation)
						{
							oy -= this.line_space_before;
							oy -= this.line_skip_before;
							
							this.line_y2 -= this.line_skip_before;
						}
						else
						{
							this.line_y2 += this.line_space_before;
						}
					}
					
					this.frame_y_line = oy;
					
					while ((! this.frame.ConstrainLineBox (oy, line_ascender, line_descender, line_height, this.line_leading, this.line_sync_to_grid, out ox, out oy, out dx, out next_frame_y))
						|| (dx < this.LeftMargin + this.RightMargin)
						|| (pass > 1))
					{
						if (continuation)
						{
							//	Cas spécial: il n'y a plus de place dans le ITextFrame,
							//	mais nous n'avons pas commencé en début de ligne (suite
							//	à un TAB, par exemple). Il faut que l'appelant relance
							//	tout le processus depuis le début de la ligne.
							
							return Layout.Status.RestartLineLayout;
						}
						
						//	Il n'y a plus de place dans le ITextFrame courant, passe au
						//	suivant, s'il en reste encore un (ou plus)...
						
						frame_index       = this.frame_index + 1;
						this.frame_y      = 0;
						this.frame_y_line = 0;
						
						goto restart;
					}
					
					this.ox          = ox + this.LeftMargin;
					this.oy_base     = oy;
					this.oy_max      = oy + line_ascender;
					this.oy_min      = oy + line_descender;
					this.frame_y     = next_frame_y;
					this.line_y1     = next_frame_y;
					this.line_width  = dx;
					this.line_height = line_height;
					
					if ((def_line_height == 0) &&
						(def_line_width == 0))
					{
						def_line_height = this.line_height;
						def_line_width  = this.line_width;
					}
				}
				
				if (pass > 1)
				{
					break;
				}
				
				Layout.Status status = this.layout_engine.Fit (this, ref result);
				
				switch (status)
				{
					case Layout.Status.Ok:
					case Layout.Status.OkFitEnded:
					case Layout.Status.OkTabReached:
						
						if (this.auto_leading)
						{
							if ((continuation) &&
								(this.frame != null) &&
								(this.line_height > def_line_height))
							{
								//	Si on est en train de traiter une ligne avec des TABs et
								//	que la hauteur de la ligne a changé, il faut demander à
								//	l'appellant de refaire une seconde passe complète de la
								//	ligne :
								
								return Layout.Status.RestartLineLayout;
							}
							
							if ((this.frame_list != null) &&
								(this.line_height > def_line_height))
							{
								//	Oups. On vient de réaliser un fit idéal, mais qui ne tient
								//	pas dans l'espace alloué verticalement. Il faut forcer une
								//	nouvelle passe :
								
								def_line_height = this.line_height;
								result.Clear ();
								this.break_mode = BreakMode.Default;
								pass = 0;
								
								initial_line_ascender  = this.LineAscender;
								initial_line_descender = this.LineDescender;
								
								snapshot.Restore (this);
								continue;
							}
						}
						
						if ((status == Layout.Status.OkFitEnded) &&
							(this.LastProcessedCode == Unicode.Code.LineSeparator))
						{
							//	C'est une fin de ligne forcée, pas une fin de paragraphe...
							
							status = Layout.Status.Ok;
						}
						
						if ((status == Layout.Status.OkFitEnded) &&
							(this.frame != null) &&
							(this.frame_first_line > 0) &&
							(frame_index > 0) &&
							(paragraph_line_count > 0) &&
							(paragraph_line_count - this.frame_first_line + 1 < this.keep_end_lines))
						{
							//	Les lignes qui constituent cette fin de paragraphe se trouvent
							//	seules dans un frame et il y en a moins que le minimum requis.
							
							if ((this.frame_list[frame_index-1] is SingleLineTextFrame) ||
								(this.frame_list[frame_index-0] is SingleLineTextFrame))
							{
								//	N'applique pas les contraintes dans le cas de lignes de texte
								//	isolées; cela n'aurait aucun sens !
							}
							else
							{
								this.frame_fences.Add (frame_index-1, System.Math.Max (0, paragraph_line_count + 1 - this.keep_end_lines));
								
								return Layout.Status.RestartParagraphLayout;
							}
						}
						
						if (status == Layout.Status.OkFitEnded)
						{
							this.frame_first_line = 0;
							this.frame_fences.Clear ();
						}
						
						return status;
					
					case Layout.Status.OkHiddenFitEnded:
						
						//	Arrivé à la fin d'un paragraphe entièrement vide; il n'a pas de
						//	contenu visible, mais il fait quand-même avancer la position dans
						//	le texte...
						
						{
							int offset = this.text_offset;
							snapshot.Restore (this);
							this.text_offset = offset;
						}
						return status;
					
					case Layout.Status.ErrorNeedMoreText:
						snapshot.Restore (this);
						return status;
						
					case Layout.Status.SwitchLayout:
						continue;
					
					case Layout.Status.ErrorCannotFit:
						if (this.break_mode == BreakMode.Hyphenate)
						{
							if (this.text_context.IsDegradedLayoutEnabled)
							{
								this.break_mode = BreakMode.Break;
							}
							
							pass++;
						}
						else
						{
							this.break_mode = BreakMode.Hyphenate;
						}
						
//-						System.Diagnostics.Debug.WriteLine ("Cannot fit, break mode : " + this.break_mode);
						
						snapshot.Restore (this, false);
						continue;
					
					default:
						break;
				}
				
				throw new System.InvalidOperationException ();
			}
			
			Debug.Assert.IsTrue (result.Count == 0);
			
			snapshot.Restore (this);
			
			return Layout.Status.ErrorCannotFit;
		}
		
		
		public void UpdateXlineProperties(int offset, double ox, bool is_visible)
		{
			Properties.AbstractXlineProperty[] current;
			Properties.AbstractXlineProperty[] previous = this.xline_properties;
			
			Properties.FontColorProperty current_color  = this.xline_text_color;
			Properties.FontColorProperty previous_color = this.xline_text_color;
			
			offset += this.text_offset;
			
			this.text_context.GetXlines (this.text[offset], out current);
			
			if ((current != null) &&
				(current.Length > 0))
			{
				//	Ne s'occupe de l'information liée à la couleur du texte que si
				//	un soulignement est en cours :
				
				string color;
				this.text_context.GetColor (this.text[offset], out color);
				
				current_color = new Properties.FontColorProperty (color);
			}
			
			//	Supprime les définitions qui donnent lieu à des soulignements
			//	invisibles :
			
			if ((current != null) &&
				(current.Length > 0))
			{
				Properties.AbstractXlineProperty.RemoveInvisible (ref current);
			}
			
			if (! Property.CompareEqualContents (previous, current) ||
				! Property.CompareEqualContents (previous_color, current_color))
			{
				//	Enregistre le changement d'état de soulignement.
				
				double oy   = this.LineBaseY;
				double asc  = this.LineAscender;
				double desc = this.LineDescender;
				
				ox = System.Math.Min (this.xline_end_x, ox);
				
				this.AddXlineRecord (new XlineRecord (XlineRecord.RecordType.Change, offset, current, current_color, ox, oy, asc, desc, this.frame_index, is_visible));
				
				this.xline_properties = current;
				this.xline_text_color = current_color;
			}
		}
		
		
		private void AddXlineRecord(XlineRecord record)
		{
			if (this.xline_records == null)
			{
				this.xline_records = new System.Collections.ArrayList ();
			}
			
			this.xline_records.Add (record);
		}
		
		
		public void DefineLineGeometry(double y, double y1, double y2, double ascender, double descender)
		{
			this.oy_base = y;
			this.oy_max  = y + ascender;
			this.oy_min  = y + descender;
			this.line_y1 = y1;
			this.line_y2 = y2;
		}
		
		public void DefineLineSkipBefore(double value)
		{
			this.line_skip_before = value;
		}
		
		public void DefineFrameFences(Layout.FrameLineFenceDictionary value)
		{
			this.frame_fences.Clear ();
			this.frame_fences.Add (value);
		}
		
		public void DefineKeepWithPreviousParagraph(bool value)
		{
			this.keep_with_prev_para = value;
		}
		
		public void DefineTextOffset(int value)
		{
			this.text_offset = value;
		}
		
		public void DefineShowControlCharacters()
		{
			this.show_control_characters = this.text_context.ShowControlCharacters;
		}
		
		public void DefineParagraphStartMode(Properties.ParagraphStartMode mode)
		{
			if ((mode != Properties.ParagraphStartMode.Anywhere) &&
				(mode != Properties.ParagraphStartMode.Undefined))
			{
				System.Diagnostics.Debug.WriteLine ("Force paragraph start mode: " + mode);
			}
			
			this.para_default_start_mode = mode;
		}
		
		
		public void DisableSimpleRendering()
		{
			this.disable_simple_rendering = true;
		}
		
		public void DisableFontBaselineOffset()
		{
			this.disable_font_baseline_offset = true;
		}
		
		
		public void InvisibleLine(ITextRenderer renderer, int length, double line_base_x, double line_base_y)
		{
			//	Appelé lorsqu'une ligne ne doit pas être affichée parce qu'elle
			//	est entièrement hors du frame.
			
			Debug.Assert.IsNotNull (this.text);
			Debug.Assert.IsTrue (this.text_start + this.text_offset + length <= this.text.Length);
			
			if (length > 0)
			{
				this.UpdateXlineProperties (length - 1, 0, false);
			}
		}
		
		public void RenderLine(ITextRenderer renderer, Layout.StretchProfile profile, int length, double line_base_x, double line_base_y, double line_width, int paragraph_line_count, bool is_tab, bool is_last_line)
		{
			//	Réalise le rendu de la ligne, en appelant les divers moteurs de
			//	layout associés au texte.
			
			Debug.Assert.IsNotNull (this.text);
			
			Debug.Assert.IsTrue (length > 0);
			Debug.Assert.IsTrue (this.text_start + this.text_offset + length <= this.text.Length);
			
			this.SelectLayoutEngine (this.text_offset);
			this.SelectMarginsAndJustification (this.text_offset, paragraph_line_count, is_last_line);
			this.SelectLineHeightAndLeading (this.text_offset, 0, 0, 0);
			
			this.ox      = line_base_x;
			this.oy_base = line_base_y;
			
			this.line_width = line_width + this.LeftMargin + this.RightMargin;
			
			Debug.Assert.IsNotNull (this.layout_engine);
			Debug.Assert.IsNotNull (this.text_context);
			
			this.text_profile = profile;
			this.break_mode   = BreakMode.Default;
			
			double space;
			
			space  = line_width - profile.TotalWidth;
			space *= 1.0 - this.justification;
			
			if (! is_tab)
			{
				this.ox += space * this.disposition;
			}
			
			this.text_width  = line_width - space;
			this.xline_end_x = this.ox + this.text_width;
			
			this.text_profile.ComputeScales (this.text_width, out this.text_scales);
			
			int               end             = this.text_offset + length;
			int               end_space_count = this.text_profile.CountEndSpace;
			Unicode.BreakInfo end_break_info  = Unicode.Bits.GetBreakInfo (this.text[this.text_start + end - 1]);
			
			switch (end_break_info)
			{
				case Unicode.BreakInfo.HyphenateGoodChoice:
				case Unicode.BreakInfo.HyphenatePoorChoice:
					this.enable_hyphenation = true;
					break;
				
				default:
					this.enable_hyphenation = false;
					break;
			}
			
			
			//	Détermine si le contenu de la ligne peut être représenté avec les
			//	facteurs d'étirement calculés dans la phase de 'fit'. En cas de
			//	problèmes, on peut introduire des cales ('glue') entre les carac-
			//	tères et modifier la gestion des substitutions de glyphes :
			
			double would_be_character_width  = this.text_profile.WidthCharacter * this.text_scales.ScaleCharacter;
			double would_be_no_stretch_width = this.text_profile.WidthNoStretch * this.text_scales.ScaleNoStretch;
			
			this.text_scales.ScaleCharacter = System.Math.Min (1.10, this.text_scales.ScaleCharacter);
			this.text_scales.ScaleNoStretch = System.Math.Min (1.05, this.text_scales.ScaleNoStretch);
			
			double clipped_character_width  = this.text_profile.WidthCharacter * this.text_scales.ScaleCharacter;
			double clipped_no_stretch_width = this.text_profile.WidthNoStretch * this.text_scales.ScaleNoStretch;
			
			double would_be_width = would_be_character_width + would_be_no_stretch_width;
			double clipped_width  = clipped_character_width  + clipped_no_stretch_width;
			
			if (would_be_width > clipped_width)
			{
				int            glue_count   = System.Math.Max (1, this.text_profile.TotalCount - 1);
				double         glue_width   = would_be_width - clipped_width;
				StretchProfile glue_profile = new StretchProfile ();
				
				this.text_glue = glue_width / glue_count;
				
				//	Le nombre de caractères à analyser correspond en principe à la
				//	longueur de la ligne (length), mais si la ligne se termine par
				//	une marque de fin de ligne, c'est l'info de text_profile qu'il
				//	faut utiliser.
				
				int fill_count = System.Math.Min (length, this.text_profile.TotalCount);
				
				this.layout_engine.FillProfile (this, fill_count, glue_profile);
				
				glue_count = glue_profile.TotalCount;
				glue_count = System.Math.Max (1, glue_count - 1);
				
				clipped_character_width  = glue_profile.WidthCharacter * this.text_scales.ScaleCharacter;
				clipped_no_stretch_width = glue_profile.WidthNoStretch * this.text_scales.ScaleNoStretch;
				
				clipped_width = clipped_character_width + clipped_no_stretch_width;
				glue_width    = would_be_width - clipped_width;
				
				this.text_glue = glue_width / glue_count;
			}
			else
			{
				this.text_glue = 0;
			}
			
			renderer.RenderStartLine (this);
			
			if ((this.xline_properties != null) &&
				(this.xline_properties.Length > 0))
			{
				int offset = this.TextOffset;
				
				double ox   = this.LineCurrentX;
				double oy   = this.LineBaseY;
				double asc  = this.LineAscender;
				double desc = this.LineDescender;
				
				this.AddXlineRecord (new XlineRecord (XlineRecord.RecordType.LineStart, offset, this.xline_properties, this.xline_text_color, ox, oy, asc, desc, this.frame_index, true));
			}
			
			for (;;)
			{
				Layout.Status status = this.layout_engine.Render (this, renderer, end - this.text_offset);
				
				switch (status)
				{
					case Layout.Status.Ok:
						renderer.RenderEndLine (this);
						
						if ((this.xline_properties != null) &&
							(this.xline_properties.Length > 0))
						{
							int offset = this.TextOffset;
							
							double ox   = this.xline_end_x;
							double oy   = this.LineBaseY;
							double asc  = this.LineAscender;
							double desc = this.LineDescender;
							
							this.AddXlineRecord (new XlineRecord (XlineRecord.RecordType.LineEnd, offset, this.xline_properties, this.xline_text_color, ox, oy, asc, desc, this.frame_index, true));
						}
						
						return;
					
					case Layout.Status.SwitchLayout:
						continue;
					
					default:
						throw new System.InvalidOperationException ();
				}
			}
		}
		
		
		public void MoveTo(double x, int offset)
		{
			this.ox = x;
			this.text_offset = offset;
		}
		
		public void DefineTabIndentation(bool tab_indents, double x)
		{
			this.use_tab_indentation = tab_indents;
			this.tab_indentation_x   = x;
		}
		
		public void SwitchLayoutEngine(Layout.BaseEngine engine, Properties.LayoutProperty property)
		{
			this.layout_engine   = engine;
			this.layout_property = property;
		}
		
		
		public void ResetLineHeight()
		{
			this.oy_max = this.oy_base;
			this.oy_min = this.oy_base;
			
			this.line_height = 0;
		}
		
		public void RecordAscender(double value)
		{
			double y = this.oy_base + value;
			
			if (y > this.oy_max)
			{
				this.oy_max = y;
			}
		}
		
		public void RecordLineHeight(double value)
		{
			if (value > this.line_height)
			{
				this.line_height = value;
				
				if (this.auto_leading)
				{
					this.line_leading = System.Math.Max (this.line_height, this.line_leading);
				}
			}
		}
		
		public void RecordDescender(double value)
		{
			double y = this.oy_base + value;
			
			if (y < this.oy_min)
			{
				this.oy_min = y;
			}
		}
		
		
		public bool GetNextWord(int offset, out ulong[] text, out int length, out Unicode.BreakInfo word_break_info)
		{
			word_break_info = Unicode.BreakInfo.No;
			
			//	TODO: gérer le sens <-- pour l'avance du texte
			
			int end   = this.text.Length;
			int pos   = this.text_start + offset;
			
			while (pos < end)
			{
				word_break_info = Unicode.Bits.GetBreakInfo (this.text[pos]);
				
				pos++;
				
				if ((word_break_info == Unicode.BreakInfo.Yes) ||
					(word_break_info == Unicode.BreakInfo.Optional) ||
					(word_break_info == Unicode.BreakInfo.HorizontalTab))
				{
					break;
				}
			}
			
			length = pos - (this.text_start + offset);
			text   = this.GetInternalBuffer (length);
			
			if (length == 0)
			{
				return false;
			}
			
			//	Copie le texte dans le buffer temporaire. C'est plus rapide de faire
			//	la copie à la main que d'appeler System.Array.Copy :
			
			pos = this.text_start + offset;
			
			for (int i = 0; i < length; i++)
			{
				text[i] = this.text[pos+i];
			}
			
			return true;
		}
		
		public bool GetTextCopy(int offset, out ulong[] text, ref int length)
		{
			//	TODO: gérer le sens <-- pour l'avance du texte
			
			int end = System.Math.Min (this.text_start + offset + length, this.text.Length);
			int pos = this.text_start + offset;

			length = end - pos;
			text = this.GetInternalBuffer(length);
			
			if (length == 0)
			{
				return false;
			}
			
			//	Copie le texte dans le buffer temporaire. C'est plus rapide de faire
			//	la copie à la main que d'appeler System.Array.Copy :
			
			for (int i = 0; i < length; i++)
			{
				text[i] = this.text[pos + i];
			}
			
			return true;
		}
		
		
		public void SelectFrame(int frame_index, double y)
		{
			if (frame_index == -1)
			{
				this.frame_index  = -1;
				this.frame        = null;
				this.frame_y      = 0;
				this.frame_y_line = 0;
			}
			else
			{
				this.frame_index  = frame_index;
				this.frame        = this.frame_list[this.frame_index];
				this.frame_y      = y;
				this.frame_y_line = y;
			}
			
			this.frame_first_line = 0;
		}
		
		public void DefineAvailableWidth(double width)
		{
			this.line_width = width + this.LeftMargin + this.RightMargin;
		}
		
		public void DefineTabDockingMark(string tab_docking_mark)
		{
			if ((tab_docking_mark != null) &&
				(tab_docking_mark.Length > 0))
			{
				uint[] utf32;
				
				TextConverter.ConvertFromString (tab_docking_mark, out utf32);
				
				int start = this.text_start + this.text_offset;
				
				for (int i = start; i < this.text.Length; i++)
				{
					uint code = (uint) Unicode.Bits.GetCode (this.text[i]);
					
					for (int j = 0; j < utf32.Length; j++)
					{
						if (code == utf32[j])
						{
							//	Trouvé un terminateur dans le texte. Il faut raccourcir
							//	le texte en le tronquant ici :
							
							int     count = i - start;
							ulong[] copy  = new ulong[count+1];
							
							System.Array.Copy (this.text, start, copy, 0, count);
							
							//	Ajoute une fin de texte synthétique, au cas où :
							
							copy[count] = this.text[i];
							
							Unicode.Bits.SetCode (ref copy[count], Unicode.Code.EndOfText);
							Unicode.Bits.SetBreakInfo (ref copy[count], Unicode.BreakInfo.Yes);
							
							this.text        = copy;
							this.text_start  = 0;
							this.text_offset = 0;
							
							return;
						}
					}
				}
			}
		}
		
		public void SelectMargins(int paragraph_line_index)
		{
			ulong code = this.text[this.text_start + this.text_offset];
			
			Properties.MarginsProperty margins;
			double font_size_in_points;
			
			this.text_context.GetMargins (code, out margins);
			this.text_context.GetFontSize (code, out font_size_in_points);
			
			if (margins != null)
			{
				if (Properties.UnitsTools.IsScale (margins.Units))
				{
					this.mx_left  = font_size_in_points * Properties.UnitsTools.ConvertToScale (paragraph_line_index == 0 ? margins.LeftMarginFirstLine  : margins.LeftMarginBody, margins.Units);
					this.mx_right = font_size_in_points * Properties.UnitsTools.ConvertToScale (paragraph_line_index == 0 ? margins.RightMarginFirstLine : margins.RightMarginBody, margins.Units);
					this.x_origin = this.mx_left;
				
					this.break_fence_before = font_size_in_points * Properties.UnitsTools.ConvertToScale (margins.BreakFenceBefore, margins.Units);
					this.break_fence_after  = font_size_in_points * Properties.UnitsTools.ConvertToScale (margins.BreakFenceAfter, margins.Units);
					this.enable_hyphenation = margins.EnableHyphenation == Properties.ThreeState.True;
				}
				else
				{
					this.mx_left  = Properties.UnitsTools.ConvertToPoints (paragraph_line_index == 0 ? margins.LeftMarginFirstLine  : margins.LeftMarginBody, margins.Units);
					this.mx_right = Properties.UnitsTools.ConvertToPoints (paragraph_line_index == 0 ? margins.RightMarginFirstLine : margins.RightMarginBody, margins.Units);
					this.x_origin = this.mx_left;
				
					this.break_fence_before = Properties.UnitsTools.ConvertToPoints (margins.BreakFenceBefore, margins.Units);
					this.break_fence_after  = Properties.UnitsTools.ConvertToPoints (margins.BreakFenceAfter, margins.Units);
					this.enable_hyphenation = margins.EnableHyphenation == Properties.ThreeState.True;
				}
			}
		}
		
		
		private void SelectLayoutEngine(int offset)
		{
			ulong code = this.text[this.text_start + offset];
			this.text_context.GetLayoutEngine (code, out this.layout_engine, out this.layout_property);
		}
		
		private void SelectMarginsAndJustification(int offset, int paragraph_line_index, bool is_last_line)
		{
			ulong code = this.text[this.text_start + offset];
			
			Properties.MarginsProperty margins;
			double font_size_in_points;
			
			this.text_context.GetMargins (code, out margins);
			this.text_context.GetFontSize (code, out font_size_in_points);
			
			if (margins != null)
			{
				if (Properties.UnitsTools.IsScale (margins.Units))
				{
					this.mx_left      = font_size_in_points * Properties.UnitsTools.ConvertToScale (paragraph_line_index == 0 ? margins.LeftMarginFirstLine  : margins.LeftMarginBody, margins.Units);
					this.mx_right     = font_size_in_points * Properties.UnitsTools.ConvertToScale (paragraph_line_index == 0 ? margins.RightMarginFirstLine : margins.RightMarginBody, margins.Units);
					this.mx_left_body = font_size_in_points * Properties.UnitsTools.ConvertToScale (margins.LeftMarginBody, margins.Units);
					this.x_origin     = this.mx_left;
				
					this.break_fence_before = font_size_in_points * Properties.UnitsTools.ConvertToScale (margins.BreakFenceBefore, margins.Units);
					this.break_fence_after  = font_size_in_points * Properties.UnitsTools.ConvertToScale (margins.BreakFenceAfter, margins.Units);
					this.enable_hyphenation = margins.EnableHyphenation == Properties.ThreeState.True;
				}
				else
				{
					this.mx_left      = Properties.UnitsTools.ConvertToPoints (paragraph_line_index == 0 ? margins.LeftMarginFirstLine  : margins.LeftMarginBody, margins.Units);
					this.mx_right     = Properties.UnitsTools.ConvertToPoints (paragraph_line_index == 0 ? margins.RightMarginFirstLine : margins.RightMarginBody, margins.Units);
					this.mx_left_body = Properties.UnitsTools.ConvertToPoints (margins.LeftMarginBody, margins.Units);
					this.x_origin     = this.mx_left;
					
					this.break_fence_before = Properties.UnitsTools.ConvertToPoints (margins.BreakFenceBefore, margins.Units);
					this.break_fence_after  = Properties.UnitsTools.ConvertToPoints (margins.BreakFenceAfter, margins.Units);
					this.enable_hyphenation = margins.EnableHyphenation == Properties.ThreeState.True;
				}
					
				
				this.justification = is_last_line ? margins.JustificationLastLine : margins.JustificationBody;
				this.disposition   = margins.Disposition;
			}
		}
		
		private void SelectKeep(int offset)
		{
			ulong code = this.text[this.text_start + offset];
			
			Properties.KeepProperty keep;
			
			this.text_context.GetKeep (code, out keep);
			
			if (keep != null)
			{
				this.para_start_mode  = keep.ParagraphStartMode;
				this.keep_start_lines = System.Math.Max (1, keep.StartLines);
				this.keep_end_lines   = System.Math.Max (1, keep.EndLines);
				
				//	Le TextFitter peut forcer un 'keep with previous paragraph' basé
				//	sur les informations relatives au paragraphe précédent; on doit
				//	donc conserver l'état 'keep_with_prev_para = true'...
				
				this.keep_with_prev_para |= keep.KeepWithPreviousParagraph == Properties.ThreeState.True;
				this.keep_with_next_para  = keep.KeepWithNextParagraph == Properties.ThreeState.True;
			}
			else
			{
				this.para_start_mode     = Properties.ParagraphStartMode.Anywhere;
				this.keep_start_lines    = 1;
				this.keep_end_lines      = 1;
				
				this.keep_with_next_para = false;
			}
			
			if ((this.para_default_start_mode == Properties.ParagraphStartMode.Undefined) ||
				(this.para_default_start_mode == Properties.ParagraphStartMode.Anywhere))
			{
			}
			else
			{
				//	Combine les deux modes de début de paragraphe :
				
				switch (this.para_default_start_mode)
				{
					case Properties.ParagraphStartMode.NewFrame:
						
						//	Si le paragraphe est précédé d'un saut de frame, on force
						//	un saut uniquement si le paragraphe lui-même ne force pas
						//	déjà un saut de page :
						
						switch (this.para_start_mode)
						{
							case Properties.ParagraphStartMode.Undefined:
							case Properties.ParagraphStartMode.Anywhere:
								this.para_start_mode = this.para_default_start_mode;
								break;
						}
						break;
					
					case Properties.ParagraphStartMode.NewPage:
					case Properties.ParagraphStartMode.NewOddPage:
					case Properties.ParagraphStartMode.NewEvenPage:
						
						//	Si le paragraphe est précédé d'un saut de page, on force
						//	un saut uniquement si le paragraphe lui-même ne force pas
						//	déjà un saut de page précis (page paire/impaire) :
						
						switch (this.para_start_mode)
						{
							case Properties.ParagraphStartMode.Undefined:
							case Properties.ParagraphStartMode.Anywhere:
							case Properties.ParagraphStartMode.NewFrame:
							case Properties.ParagraphStartMode.NewPage:
								this.para_start_mode = this.para_default_start_mode;
								break;
						}
						break;
					
				}
			}
		}
		
		private void SelectLineHeightAndLeading(int offset, double line_height, double ascender, double descender)
		{
			ulong code = this.text[this.text_start + offset];
			
			OpenType.Font font;
			double        font_size;
			double        font_scale;
			
			Properties.LeadingProperty leading_property;
			
			this.text_context.GetFontAndSize (code, out font, out font_size, out font_scale);
			this.text_context.GetLeading (code, out leading_property);
			
			font_size *= font_scale;
			
			if (font != null)
			{
				ascender  = System.Math.Max (ascender, font.GetAscender (font_size));
				descender = System.Math.Min (descender, font.GetDescender (font_size));
				font_size = System.Math.Max (font_size, (ascender - descender) * 1.2);
				
				double auto_scale = 1.0;
				double leading    = font_size * auto_scale;
				
				if (leading_property != null)
				{
					if ((double.IsNaN (leading_property.Leading)) ||
						(leading_property.Leading == 0))
					{
						//	Pas d'interligne spécifié : utilise l'interligne automatique.
					}
					else
					{
						if ((leading_property.LeadingUnits == Properties.SizeUnits.Percent) ||
							(leading_property.LeadingUnits == Properties.SizeUnits.PercentNotCombining))
						{
							leading   *= leading_property.Leading;
							auto_scale = leading_property.Leading;
						}
						else
						{
							leading    = leading_property.LeadingInPoints;
							auto_scale = 0;
						}
					}
					
					this.line_space_before = double.IsNaN (leading_property.SpaceBefore) ? 0 : leading_property.SpaceBeforeInPoints;
					this.line_space_after  = double.IsNaN (leading_property.SpaceAfter)  ? 0 : leading_property.SpaceAfterInPoints;
				}
				else
				{
					this.line_space_before = 0;
					this.line_space_after  = 0;
				}
				
				this.auto_leading = auto_scale != 0;
				this.line_leading = leading;
				this.line_align   = leading_property == null ? Properties.AlignMode.None : leading_property.AlignMode;
				this.line_height  = System.Math.Max (line_height, leading);
			}
			
			this.oy_max = this.oy_base + ascender;
			this.oy_min = this.oy_base + descender;
		}
		
		private void SelectVerticalAlignment(int paragraph_line_index)
		{
			switch (this.line_align)
			{
				case Properties.AlignMode.First:
					if ((paragraph_line_index == 0) ||
						(this.frame_y == 0))
					{
						this.line_sync_to_grid = true;
						return;
					}
					break;
				
				case Properties.AlignMode.All:
					this.line_sync_to_grid = true;
					return;
				
				default:
					break;
			}
			
			this.line_sync_to_grid = false;
		}
		
		
		private ulong[] GetInternalBuffer(int length)
		{
			if (this.buffer == null)
			{
				this.buffer = new ulong[System.Math.Max (length, 32)];
			}
			else if (this.buffer.Length < length)
			{
				this.buffer = new ulong[length];
			}
				
			return this.buffer;
		}
		
		
		private void UpdateFrameIndex(ref int frame_index, bool is_top_of_frame, Properties.ParagraphStartMode para_start_mode)
		{
			//	Un paragraphe peut imposer des contraintes quant à sa position dans
			//	un frame (début de frame, frame en début de page/page paire/page
			//	impaire).
			
			switch (para_start_mode)
			{
				case Properties.ParagraphStartMode.NewFrame:
					if (! is_top_of_frame)
					{
						frame_index++;
					}
					break;
				
				case Properties.ParagraphStartMode.NewPage:
					if ((this.IsFirstFrameInPage (frame_index)) &&
						(is_top_of_frame))
					{
						//	OK : déjà au début d'une page.
					}
					else
					{
						frame_index++;
						
						while ((frame_index < this.frame_list.Count)
							&& (this.IsFirstFrameInPage (frame_index) == false))
						{
							frame_index++;
						}
					}
					break;
				
				case Properties.ParagraphStartMode.NewEvenPage:
					if ((this.IsFirstFrameInPage (frame_index)) &&
						(this.IsFrameInEvenPage (frame_index)) &&
						(is_top_of_frame))
					{
						//	OK : déjà au début d'une page paire.
					}
					else
					{
						frame_index++;
						
						while ((frame_index < this.frame_list.Count)
							&& ((this.IsFrameInEvenPage (frame_index) == false) || (this.IsFirstFrameInPage (frame_index) == false)))
						{
							frame_index++;
						}
					}
					break;
				
				case Properties.ParagraphStartMode.NewOddPage:
					if ((this.IsFirstFrameInPage (frame_index)) &&
						(this.IsFrameInOddPage (frame_index)) &&
						(is_top_of_frame))
					{
						//	OK : déjà au début d'une page impaire.
					}
					else
					{
						frame_index++;
						
						while ((frame_index < this.frame_list.Count)
							&& ((this.IsFrameInOddPage (frame_index) == false) || (this.IsFirstFrameInPage (frame_index) == false)))
						{
							frame_index++;
						}
					}
					break;
			}
		}
		
		
		private bool IsFirstFrameInPage(int frame_index)
		{
			if (frame_index == 0)
			{
				return true;
			}
			
			if ((frame_index > 0) &&
				(frame_index < this.frame_list.Count))
			{
				ITextFrame frame_a = this.frame_list[frame_index-1];
				ITextFrame frame_b = this.frame_list[frame_index];
				
				System.Diagnostics.Debug.Assert (frame_a != null);
				System.Diagnostics.Debug.Assert (frame_b != null);
				
				return frame_a.PageNumber != frame_b.PageNumber;
			}
			
			return false;
		}
		
		private bool IsFrameInEvenPage(int frame_index)
		{
			if ((frame_index > -1) &&
				(frame_index < this.frame_list.Count))
			{
				ITextFrame frame = this.frame_list[frame_index];
				
				System.Diagnostics.Debug.Assert (frame != null);
				
				int page = frame.PageNumber;
				
				return (page > 0) && ((page & 0x1) == 0);
			}
			
			return false;
		}
		
		private bool IsFrameInOddPage(int frame_index)
		{
			if ((frame_index > -1) &&
				(frame_index < this.frame_list.Count))
			{
				ITextFrame frame = this.frame_list[frame_index];
				
				System.Diagnostics.Debug.Assert (frame != null);
				
				int page = frame.PageNumber;
				
				return (page > 0) && ((page & 0x1) == 0x1);
			}
			
			return false;
		}
		
		
		#region Snapshot Class
		private class Snapshot
		{
			public Snapshot(Context context)
			{
				this.snapshot      = context.snapshot;
				this.text_offset   = context.text_offset;
				this.layout_engine = context.layout_engine;
				this.ox            = context.ox;
				this.oy_base       = context.oy_base;
				
				this.frame_index      = context.frame_index;
				this.frame_y          = context.frame_y;
				this.frame_first_line = context.frame_first_line;
			}
			
			
			public void Restore(Context context)
			{
				this.Restore (context, true);
			}
			
			public void Restore(Context context, bool restore_frame)
			{
				double ascender  = context.oy_max - context.oy_base;
				double descender = context.oy_min - context.oy_base;
				
				context.snapshot      = this.snapshot;
				context.text_offset   = this.text_offset;
				context.layout_engine = this.layout_engine;
				
				context.ox      = this.ox;
				context.oy_base = this.oy_base;
				context.oy_max  = this.oy_base + ascender;
				context.oy_min  = this.oy_base + descender;
				
				if (restore_frame)
				{
					context.SelectFrame (this.frame_index, this.frame_y);
				}
				else
				{
					context.SelectFrame (context.frame_index, this.frame_y);
				}
				
				context.frame_first_line = this.frame_first_line;
			}
			
			public void FixFrame(int frame_index, double frame_y, int frame_first_line)
			{
				this.frame_index      = frame_index;
				this.frame_y          = frame_y;
				this.frame_first_line = frame_first_line;
			}
			
			
			private Snapshot					snapshot;
			private int							text_offset;
			private Layout.BaseEngine			layout_engine;
			private double						ox, oy_base;
			private int							frame_index;
			private double						frame_y;
			private int							frame_first_line;
		}
		#endregion
		
		
		private TextContext						text_context;
		private ulong[]							text;
		private int								text_start;
		private int								text_offset;
		private StretchProfile					text_profile;
		private StretchProfile.Scales			text_scales;
		private double							text_width;
		private double							text_glue;
		
		private FrameList						frame_list;
		private int								frame_index = -1;
		private ITextFrame						frame;
		private double							frame_y;
		private double							frame_y_line;
		private int								frame_first_line;			//	# première ligne du paragraphe dans ce frame
		
		private int								left_to_right;
		
		private double							ox;
		private double							ox_line_start;
		private double							oy_base;
		private double							oy_max;
		private double							oy_min;
		
		private double							line_y1;
		private double							line_y2;
		
		private double							line_height;
		private double							line_width;
		private double							line_leading;
		private bool							auto_leading;
		private Properties.AlignMode			line_align;
		private bool							line_sync_to_grid;
		private double							line_skip_before;
		private double							line_space_before;
		private double							line_space_after;
		
		private Properties.ParagraphStartMode	para_default_start_mode;
		private Properties.ParagraphStartMode	para_start_mode;
		private int								keep_start_lines;
		private int								keep_end_lines;
		private bool							keep_with_prev_para;
		private bool							keep_with_next_para;
		
		private Layout.FrameLineFenceDictionary	frame_fences = new FrameLineFenceDictionary ();
		
		private double							mx_left;
		private double							mx_left_body;
		private double							mx_right;
		private double							x_origin;
		
		private bool							use_tab_indentation;
		private double							tab_indentation_x;
		
		private double							break_fence_before;
		private double							break_fence_after;
		
		private double							justification;
		private double							disposition;
		
		private bool							enable_hyphenation;
		private BreakMode						break_mode;
		
		private bool							show_control_characters;
		
		private bool							disable_simple_rendering;
		private bool							disable_font_baseline_offset;
		
		private Layout.BaseEngine				layout_engine;
		private Properties.LayoutProperty		layout_property;
		
		private Snapshot						snapshot;
		
		private ulong[]							buffer;
		
		Properties.AbstractXlineProperty[]		xline_properties;
		Properties.FontColorProperty			xline_text_color;
		System.Collections.ArrayList			xline_records;
		private double							xline_end_x;
	}
}
