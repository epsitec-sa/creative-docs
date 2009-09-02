//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// La classe TextContext stocke le contexte de layout lié à un paragraphe
	/// entier.
	/// </summary>
	public class Context
	{
		public Context(Text.TextContext textContext, ulong[] text, int start, double oyBase, double lineHeight, double lineWidth, double mxLeft, double mxRight, double breakFenceBefore, double breakFenceAfter)
		{
			this.buffer = null;
			
			this.textContext = textContext;
			
			this.text       = text;
			this.textStart = start;
			
			this.oyBase = oyBase;
			this.oyMax  = oyBase;
			this.oyMin  = oyBase;
			
			this.lineHeight = lineHeight;
			this.lineWidth  = lineWidth;
			
			this.ox       = mxLeft;
			this.mxLeft  = mxLeft;
			this.mxLeftBody = mxLeft;
			this.mxRight = mxRight;
			this.xOrigin = this.mxLeft;
			
			this.breakFenceBefore = breakFenceBefore;
			this.breakFenceAfter  = breakFenceAfter;
			
			this.leftToRight = 0;
		}
		
		public Context(Text.TextContext context, ulong[] text, int start, FrameList frameList)
		{
			this.textContext = context;
			this.text         = text;
			this.textStart   = start;
			this.frameList   = frameList;
		}
		
		
		public Layout.BaseEngine				LayoutEngine
		{
			get
			{
				return this.layoutEngine;
			}
		}
		
		public Properties.LayoutProperty		LayoutProperty
		{
			get
			{
				return this.layoutProperty;
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
				return this.textContext;
			}
		}
		
		public int								TextOffset
		{
			get
			{
				return this.textOffset;
			}
		}
		
		public StretchProfile					TextStretchProfile
		{
			get
			{
				return this.textProfile;
			}
		}
		
		public StretchProfile.Scales			TextStretchScales
		{
			get
			{
				return this.textScales;
			}
		}
		
		public double							TextStretchGlue
		{
			get
			{
				return this.textGlue;
			}
		}
		
		public double							TextWidth
		{
			get
			{
				return this.textWidth;
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
				if ((this.textOffset > 0) &&
					(this.text != null) &&
					(this.text.Length >= this.textOffset))
				{
					return this.text[this.textOffset-1];
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
				return this.oxLineStart;
			}
		}
		
		public double							LineOriginX
		{
			get
			{
				return this.xOrigin;
			}
		}
		
		public double							LineBaseY
		{
			get
			{
				return this.oyBase;
			}
		}
		
		public double							LineY1
		{
			get
			{
				return this.lineY1;
			}
		}
		
		public double							LineY2
		{
			get
			{
				return this.lineY2;
			}
		}
		
		public double							MaxY
		{
			get
			{
				return this.oyMax;
			}
		}
		
		public double							MinY
		{
			get
			{
				return this.oyMin;
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
				return this.breakFenceBefore;
			}
		}
		
		public double							BreakFenceAfter
		{
			get
			{
				return this.breakFenceAfter;
			}
		}
		
		public bool								EnableHyphenation
		{
			get
			{
				return this.enableHyphenation;
			}
		}
		
		public BreakMode						BreakMode
		{
			get
			{
				return this.breakMode;
			}
		}
		
		
		public double							LeftMargin
		{
			get
			{
				return this.useTabIndentation ? this.tabIndentationX : this.mxLeft;
			}
		}
		
		public double							LeftBodyMargin
		{
			get
			{
				return this.mxLeftBody;
			}
		}
		
		public double							RightMargin
		{
			get
			{
				return this.mxRight;
			}
		}
		
		public double							LineHeight
		{
			get
			{
				return this.lineHeight;
			}
		}
		
		public double							LineWidth
		{
			get
			{
				return this.lineWidth;
			}
		}
		
		public double							LineAscender
		{
			get
			{
				return this.oyMax - this.oyBase;
			}
		}
		
		public double							LineDescender
		{
			get
			{
				return this.oyMin - this.oyBase;
			}
		}
		
		public double							LineSkipBefore
		{
			//	Ne pas confondre LineSpaceBefore qui provient de LeadingProperty
			//	et LineSkipBefore qui est la valeur imposée par le TextFitter en
			//	début de paragraphe, à cause d'une fin de paragraphe précédente.
			
			get
			{
				return this.lineSkipBefore;
			}
		}
		
		public double							LineSpaceBefore
		{
			get
			{
				return this.lineSpaceBefore;
			}
		}
		
		public double							LineSpaceAfter
		{
			get
			{
				return this.lineSpaceAfter;
			}
		}
		
		
		public Layout.FrameLineFenceDictionary	FrameFences
		{
			get
			{
				return this.frameFences;
			}
		}
		
		public bool								KeepWithPreviousParagraph
		{
			get
			{
				return this.keepWithPrevPara;
			}
		}
		
		public bool								KeepWithNextParagraph
		{
			get
			{
				return this.keepWithNextPara;
			}
		}
		
		
		public bool								ShowControlCharacters
		{
			get
			{
				return this.showControlCharacters;
			}
		}
		
		public bool								IsSimpleRenderingDisabled
		{
			get
			{
				return this.disableSimpleRendering;
			}
		}
		
		public bool								IsFontBaselineOffsetDisabled
		{
			get
			{
				return this.disableFontBaselineOffset;
			}
		}
		
		
		public double							AvailableWidth
		{
			get
			{
				return this.lineWidth - this.LeftMargin - this.RightMargin;
			}
		}
		
		public bool								IsLeftToRight
		{
			get
			{
				return (this.leftToRight & 1) == 0;
			}
		}
		
		public bool								IsRightToLeft
		{
			get
			{
				return (this.leftToRight & 1) == 1;
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
				return this.frameIndex;
			}
		}
		
		public double							FrameY
		{
			get
			{
				return this.frameY;
			}
		}
		
		public double							FrameYLine
		{
			get
			{
				//	Retourne la position Y de la ligne dans le frame, alors que FrameY retourne
				//	la prochaine position Y à utiliser...
				
				return this.frameYLine;
			}
		}
		
		
		public Properties.AbstractXlineProperty[] XlineProperties
		{
			get
			{
				if ((this.xlineProperties == null) ||
					(this.xlineProperties.Length == 0))
				{
					return new Properties.AbstractXlineProperty[0];
				}
				
				Properties.AbstractXlineProperty[] properties = new Properties.AbstractXlineProperty[this.xlineProperties.Length];
				
				this.xlineProperties.CopyTo (properties, 0);
				
				return properties;
			}
		}
		
		public Layout.XlineRecord[]				XlineRecords
		{
			get
			{
				if ((this.xlineRecords == null) ||
					(this.xlineRecords.Count == 0))
				{
					return new Layout.XlineRecord[0];
				}
				
				Layout.XlineRecord[] records = new Layout.XlineRecord[this.xlineRecords.Count];
				
				this.xlineRecords.CopyTo (records);
				
				return records;
			}
		}
		
		
		public Layout.Status Fit(ref Layout.BreakCollection result, int paragraphLineCount)
		{
			this.ResetLineHeight ();
			
			return this.Fit (ref result, paragraphLineCount, false);
		}

		public Layout.Status Fit(ref Layout.BreakCollection result, int paragraphLineCount, bool continuation)
		{
			//	Détermine les points de découpe pour le texte, selon le contexte
			//	courant.
			
			if (this.textStart + this.textOffset >= this.text.Length)
			{
				return Layout.Status.ErrorNeedMoreText;
			}
			
			double initialLineAscender  = this.LineAscender;
			double initialLineDescender = this.LineDescender;
			double initialLineHeight    = this.LineHeight;
			
			this.SelectLayoutEngine (this.textOffset);
			
			if (continuation)
			{
				//	TODO: gérer la continuation...
				
				this.mxLeft = this.ox;
			}
			else
			{
				this.SelectMarginsAndJustification (this.textOffset, paragraphLineCount, false);
				this.SelectLineHeightAndLeading (this.textOffset, initialLineHeight, initialLineAscender, initialLineDescender);
				this.SelectVerticalAlignment (paragraphLineCount);
				this.SelectKeep (this.textOffset);
				
				this.ox = this.LeftMargin;
			}
			
			this.oxLineStart = this.ox;
			this.breakMode    = BreakMode.Default;
			
			Debug.Assert.IsNotNull (this.layoutEngine);
			Debug.Assert.IsNotNull (this.text);
			Debug.Assert.IsNotNull (this.textContext);

			Snapshot snapshot = new Snapshot (this);
			
			if (result == null)
			{
				result = new Layout.BreakCollection ();
			}
			else
			{
				result.Clear ();
			}
			
			this.textProfile = new StretchProfile ();
			
			int frameIndex = this.frameIndex;
			
restart:	
			double defLineHeight = 0;
			double defLineWidth  = 0;
			
			if (! continuation)
			{
				int fence = this.frameFences[frameIndex];
				
				while ((fence > -1) &&
					   (paragraphLineCount >= fence))
				{
					frameIndex++;
					fence = this.frameFences[frameIndex];
				}
				
				if (paragraphLineCount == 0)
				{
					//	Sélectionne le frame qui convient pour ce paragraphe (selon
					//	les réglages de la propriété Keep.ParagraphStartMode) :
					
					this.UpdateFrameIndex (ref frameIndex, this.frameY == 0, this.paraStartMode);
				}
			}
			
			if (frameIndex != this.frameIndex)
			{
				if ((frameIndex < this.frameList.Count) &&
					(frameIndex > -1))
				{
					//	Reprend avec un autre frame. Vérifions d'abord si ce changement
					//	de frame est permis ici :
					
					if ((this.frameList[frameIndex-1] is SingleLineTextFrame) ||
						(this.frameList[frameIndex-0] is SingleLineTextFrame))
					{
						goto select_new_frame;
					}
					
					if ((paragraphLineCount < this.keepStartLines) &&
						(paragraphLineCount > 0))
					{
						//	Il n'y a pas assez de lignes de texte consécutives en début
						//	de paragraphe !
						
						this.frameFences.Add (frameIndex-1, 0);
						this.frameFirstLine = 0;
						
						if (this.keepWithPrevPara)
						{
							return Layout.Status.RewindParagraphAndRestartLayout;
						}
						
						this.SelectFrame (frameIndex, 0);
						
						return Layout.Status.RestartParagraphLayout;
					}
					if ((paragraphLineCount == 0) &&
						(this.keepWithPrevPara) &&
						(this.paraStartMode == Properties.ParagraphStartMode.Anywhere))
					{
						//	Le paragraphe ne peut pas être dissocié de celui qui précède.
						//	Un changement de frame ici va affecter le paragraphe qui
						//	précède immédiatement (récursivement)
						
						this.frameFences.Add (frameIndex-1, 0);
						
						return Layout.Status.RewindParagraphAndRestartLayout;
					}
					
				select_new_frame:
					
					//	On reprend tout à zéro depuis ici :
					
					this.SelectLayoutEngine (this.textOffset);
					this.SelectMarginsAndJustification (this.textOffset, paragraphLineCount, false);
					this.SelectFrame (frameIndex, 0);
					this.SelectLineHeightAndLeading (this.textOffset, initialLineHeight, initialLineAscender, initialLineDescender);
					this.SelectVerticalAlignment (paragraphLineCount);
					
					//	Prend note du nouveau frame de référence et de la position dans
					//	le paragraphe au sommet du frame :
					
					this.frameFirstLine = paragraphLineCount;
					
					snapshot.FixFrame (this.frameIndex, this.frameY, this.frameFirstLine);
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
					this.SelectLineHeightAndLeading (this.textOffset, initialLineHeight, initialLineAscender, initialLineDescender);
					
					double lineAscender  = this.LineAscender;
					double lineDescender = this.LineDescender;
					double lineHeight    = this.LineHeight;
					
					double ox, oy, dx;
					double nextFrameY;
					
					oy = this.frameY;
					
					this.lineY2 = oy;
					
					if ((paragraphLineCount == 0) &&
						(this.frameY != 0))
					{
						//	A la première ligne du paragraphe, on ajoute l'espace "avant"
						//	tel que défini par la propriété de "leading". Mais on n'ajoute
						//	cet espace que si on n'est pas en sommet de frame :
						
						if (! continuation)
						{
							oy -= this.lineSpaceBefore;
							oy -= this.lineSkipBefore;
							
							this.lineY2 -= this.lineSkipBefore;
						}
						else
						{
							this.lineY2 += this.lineSpaceBefore;
						}
					}
					
					this.frameYLine = oy;
					
					while ((! this.frame.ConstrainLineBox (oy, lineAscender, lineDescender, lineHeight, this.lineLeading, this.lineSyncToGrid, out ox, out oy, out dx, out nextFrameY))
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
						
						frameIndex       = this.frameIndex + 1;
						this.frameY      = 0;
						this.frameYLine = 0;
						
						goto restart;
					}
					
					this.ox          = ox + this.LeftMargin;
					this.oyBase     = oy;
					this.oyMax      = oy + lineAscender;
					this.oyMin      = oy + lineDescender;
					this.frameY     = nextFrameY;
					this.lineY1     = nextFrameY;
					this.lineWidth  = dx;
					this.lineHeight = lineHeight;
					
					if ((defLineHeight == 0) &&
						(defLineWidth == 0))
					{
						defLineHeight = this.lineHeight;
						defLineWidth  = this.lineWidth;
					}
				}
				
				if (pass > 1)
				{
					break;
				}
				
				Layout.Status status = this.layoutEngine.Fit (this, ref result);
				
				switch (status)
				{
					case Layout.Status.Ok:
					case Layout.Status.OkFitEnded:
					case Layout.Status.OkTabReached:
						
						if (this.autoLeading)
						{
							if ((continuation) &&
								(this.frame != null) &&
								(this.lineHeight > defLineHeight))
							{
								//	Si on est en train de traiter une ligne avec des TABs et
								//	que la hauteur de la ligne a changé, il faut demander à
								//	l'appellant de refaire une seconde passe complète de la
								//	ligne :
								
								return Layout.Status.RestartLineLayout;
							}
							
							if ((this.frameList != null) &&
								(this.lineHeight > defLineHeight))
							{
								//	Oups. On vient de réaliser un fit idéal, mais qui ne tient
								//	pas dans l'espace alloué verticalement. Il faut forcer une
								//	nouvelle passe :
								
								defLineHeight = this.lineHeight;
								result.Clear ();
								this.breakMode = BreakMode.Default;
								pass = 0;
								
								initialLineAscender  = this.LineAscender;
								initialLineDescender = this.LineDescender;
								
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
							(this.frameFirstLine > 0) &&
							(frameIndex > 0) &&
							(paragraphLineCount > 0) &&
							(paragraphLineCount - this.frameFirstLine + 1 < this.keepEndLines))
						{
							//	Les lignes qui constituent cette fin de paragraphe se trouvent
							//	seules dans un frame et il y en a moins que le minimum requis.
							
							if ((this.frameList[frameIndex-1] is SingleLineTextFrame) ||
								(this.frameList[frameIndex-0] is SingleLineTextFrame))
							{
								//	N'applique pas les contraintes dans le cas de lignes de texte
								//	isolées; cela n'aurait aucun sens !
							}
							else
							{
								this.frameFences.Add (frameIndex-1, System.Math.Max (0, paragraphLineCount + 1 - this.keepEndLines));
								
								return Layout.Status.RestartParagraphLayout;
							}
						}
						
						if (status == Layout.Status.OkFitEnded)
						{
							this.frameFirstLine = 0;
							this.frameFences.Clear ();
						}
						
						return status;
					
					case Layout.Status.OkHiddenFitEnded:
						
						//	Arrivé à la fin d'un paragraphe entièrement vide; il n'a pas de
						//	contenu visible, mais il fait quand-même avancer la position dans
						//	le texte...
						
						{
							int offset = this.textOffset;
							snapshot.Restore (this);
							this.textOffset = offset;
						}
						return status;
					
					case Layout.Status.ErrorNeedMoreText:
						snapshot.Restore (this);
						return status;
						
					case Layout.Status.SwitchLayout:
						continue;
					
					case Layout.Status.ErrorCannotFit:
						if (this.breakMode == BreakMode.Hyphenate)
						{
							if (this.textContext.IsDegradedLayoutEnabled)
							{
								this.breakMode = BreakMode.Break;
							}
							
							pass++;
						}
						else
						{
							this.breakMode = BreakMode.Hyphenate;
						}
						
//-						System.Diagnostics.Debug.WriteLine ("Cannot fit, break mode : " + this.breakMode);
						
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
		
		
		public void UpdateXlineProperties(int offset, double ox, bool isVisible)
		{
			Properties.AbstractXlineProperty[] current;
			Properties.AbstractXlineProperty[] previous = this.xlineProperties;
			
			Properties.FontColorProperty currentColor  = this.xlineTextColor;
			Properties.FontColorProperty previousColor = this.xlineTextColor;
			
			offset += this.textOffset;
			
			this.textContext.GetXlines (this.text[offset], out current);
			
			if ((current != null) &&
				(current.Length > 0))
			{
				//	Ne s'occupe de l'information liée à la couleur du texte que si
				//	un soulignement est en cours :
				
				string color;
				this.textContext.GetColor (this.text[offset], out color);
				
				currentColor = new Properties.FontColorProperty (color);
			}
			
			//	Supprime les définitions qui donnent lieu à des soulignements
			//	invisibles :
			
			if ((current != null) &&
				(current.Length > 0))
			{
				Properties.AbstractXlineProperty.RemoveInvisible (ref current);
			}
			
			if (! Property.CompareEqualContents (previous, current) ||
				! Property.CompareEqualContents (previousColor, currentColor))
			{
				//	Enregistre le changement d'état de soulignement.
				
				double oy   = this.LineBaseY;
				double asc  = this.LineAscender;
				double desc = this.LineDescender;
				
				ox = System.Math.Min (this.xlineEndX, ox);
				
				this.AddXlineRecord (new XlineRecord (XlineRecord.RecordType.Change, offset, current, currentColor, ox, oy, asc, desc, this.frameIndex, isVisible));
				
				this.xlineProperties = current;
				this.xlineTextColor = currentColor;
			}
		}
		
		
		private void AddXlineRecord(XlineRecord record)
		{
			if (this.xlineRecords == null)
			{
				this.xlineRecords = new System.Collections.ArrayList ();
			}
			
			this.xlineRecords.Add (record);
		}
		
		
		public void DefineLineGeometry(double y, double y1, double y2, double ascender, double descender)
		{
			this.oyBase = y;
			this.oyMax  = y + ascender;
			this.oyMin  = y + descender;
			this.lineY1 = y1;
			this.lineY2 = y2;
		}
		
		public void DefineLineSkipBefore(double value)
		{
			this.lineSkipBefore = value;
		}
		
		public void DefineFrameFences(Layout.FrameLineFenceDictionary value)
		{
			this.frameFences.Clear ();
			this.frameFences.Add (value);
		}
		
		public void DefineKeepWithPreviousParagraph(bool value)
		{
			this.keepWithPrevPara = value;
		}
		
		public void DefineTextOffset(int value)
		{
			this.textOffset = value;
		}
		
		public void DefineShowControlCharacters()
		{
			this.showControlCharacters = this.textContext.ShowControlCharacters;
		}
		
		public void DefineParagraphStartMode(Properties.ParagraphStartMode mode)
		{
			if ((mode != Properties.ParagraphStartMode.Anywhere) &&
				(mode != Properties.ParagraphStartMode.Undefined))
			{
				System.Diagnostics.Debug.WriteLine ("Force paragraph start mode: " + mode);
			}
			
			this.paraDefaultStartMode = mode;
		}
		
		
		public void DisableSimpleRendering()
		{
			this.disableSimpleRendering = true;
		}
		
		public void DisableFontBaselineOffset()
		{
			this.disableFontBaselineOffset = true;
		}
		
		
		public void InvisibleLine(ITextRenderer renderer, int length, double lineBaseX, double lineBaseY)
		{
			//	Appelé lorsqu'une ligne ne doit pas être affichée parce qu'elle
			//	est entièrement hors du frame.
			
			Debug.Assert.IsNotNull (this.text);
			Debug.Assert.IsTrue (this.textStart + this.textOffset + length <= this.text.Length);
			
			if (length > 0)
			{
				this.UpdateXlineProperties (length - 1, 0, false);
			}
		}
		
		public void RenderLine(ITextRenderer renderer, Layout.StretchProfile profile, int length, double lineBaseX, double lineBaseY, double lineWidth, int paragraphLineCount, bool isTab, bool isLastLine)
		{
			//	Réalise le rendu de la ligne, en appelant les divers moteurs de
			//	layout associés au texte.
			
			Debug.Assert.IsNotNull (this.text);
			
			Debug.Assert.IsTrue (length > 0);
			Debug.Assert.IsTrue (this.textStart + this.textOffset + length <= this.text.Length);
			
			this.SelectLayoutEngine (this.textOffset);
			this.SelectMarginsAndJustification (this.textOffset, paragraphLineCount, isLastLine);
			this.SelectLineHeightAndLeading (this.textOffset, 0, 0, 0);
			
			this.ox      = lineBaseX;
			this.oyBase  = lineBaseY;
			
			this.lineWidth = lineWidth + this.LeftMargin + this.RightMargin;
			
			Debug.Assert.IsNotNull (this.layoutEngine);
			Debug.Assert.IsNotNull (this.textContext);
			
			this.textProfile = profile;
			this.breakMode   = BreakMode.Default;
			
			double space;
			
			space  = lineWidth - profile.TotalWidth;
			space *= 1.0 - this.justification;
			
			if (! isTab)
			{
				this.ox += space * this.disposition;
			}
			
			this.textWidth  = lineWidth - space;
			this.xlineEndX = this.ox + this.textWidth;
			
			this.textProfile.ComputeScales (this.textWidth, out this.textScales);
			
			int               end             = this.textOffset + length;
			int               endSpaceCount   = this.textProfile.CountEndSpace;
			Unicode.BreakInfo endBreakInfo    = Unicode.Bits.GetBreakInfo (this.text[this.textStart + end - 1]);
			
			switch (endBreakInfo)
			{
				case Unicode.BreakInfo.HyphenateGoodChoice:
				case Unicode.BreakInfo.HyphenatePoorChoice:
					this.enableHyphenation = true;
					break;
				
				default:
					this.enableHyphenation = false;
					break;
			}
			
			
			//	Détermine si le contenu de la ligne peut être représenté avec les
			//	facteurs d'étirement calculés dans la phase de 'fit'. En cas de
			//	problèmes, on peut introduire des cales ('glue') entre les carac-
			//	tères et modifier la gestion des substitutions de glyphes :
			
			double wouldBeCharacterWidth  = this.textProfile.WidthCharacter * this.textScales.ScaleCharacter;
			double wouldBeNoStretchWidth = this.textProfile.WidthNoStretch * this.textScales.ScaleNoStretch;
			
			this.textScales.ScaleCharacter = System.Math.Min (1.10, this.textScales.ScaleCharacter);
			this.textScales.ScaleNoStretch = System.Math.Min (1.05, this.textScales.ScaleNoStretch);
			
			double clippedCharacterWidth  = this.textProfile.WidthCharacter * this.textScales.ScaleCharacter;
			double clippedNoStretchWidth = this.textProfile.WidthNoStretch * this.textScales.ScaleNoStretch;
			
			double wouldBeWidth = wouldBeCharacterWidth + wouldBeNoStretchWidth;
			double clippedWidth  = clippedCharacterWidth  + clippedNoStretchWidth;
			
			if (wouldBeWidth > clippedWidth)
			{
				int            glueCount   = System.Math.Max (1, this.textProfile.TotalCount - 1);
				double         glueWidth   = wouldBeWidth - clippedWidth;
				StretchProfile glueProfile = new StretchProfile ();
				
				this.textGlue = glueWidth / glueCount;
				
				//	Le nombre de caractères à analyser correspond en principe à la
				//	longueur de la ligne (length), mais si la ligne se termine par
				//	une marque de fin de ligne, c'est l'info de textProfile qu'il
				//	faut utiliser.
				
				int fillCount = System.Math.Min (length, this.textProfile.TotalCount);
				
				this.layoutEngine.FillProfile (this, fillCount, glueProfile);
				
				glueCount = glueProfile.TotalCount;
				glueCount = System.Math.Max (1, glueCount - 1);
				
				clippedCharacterWidth  = glueProfile.WidthCharacter * this.textScales.ScaleCharacter;
				clippedNoStretchWidth = glueProfile.WidthNoStretch * this.textScales.ScaleNoStretch;
				
				clippedWidth = clippedCharacterWidth + clippedNoStretchWidth;
				glueWidth    = wouldBeWidth - clippedWidth;
				
				this.textGlue = glueWidth / glueCount;
			}
			else
			{
				this.textGlue = 0;
			}
			
			renderer.RenderStartLine (this);
			
			if ((this.xlineProperties != null) &&
				(this.xlineProperties.Length > 0))
			{
				int offset = this.TextOffset;
				
				double ox   = this.LineCurrentX;
				double oy   = this.LineBaseY;
				double asc  = this.LineAscender;
				double desc = this.LineDescender;
				
				this.AddXlineRecord (new XlineRecord (XlineRecord.RecordType.LineStart, offset, this.xlineProperties, this.xlineTextColor, ox, oy, asc, desc, this.frameIndex, true));
			}
			
			for (;;)
			{
				Layout.Status status = this.layoutEngine.Render (this, renderer, end - this.textOffset);
				
				switch (status)
				{
					case Layout.Status.Ok:
						renderer.RenderEndLine (this);
						
						if ((this.xlineProperties != null) &&
							(this.xlineProperties.Length > 0))
						{
							int offset = this.TextOffset;
							
							double ox   = this.xlineEndX;
							double oy   = this.LineBaseY;
							double asc  = this.LineAscender;
							double desc = this.LineDescender;
							
							this.AddXlineRecord (new XlineRecord (XlineRecord.RecordType.LineEnd, offset, this.xlineProperties, this.xlineTextColor, ox, oy, asc, desc, this.frameIndex, true));
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
			this.textOffset = offset;
		}
		
		public void DefineTabIndentation(bool tabIndents, double x)
		{
			this.useTabIndentation = tabIndents;
			this.tabIndentationX   = x;
		}
		
		public void SwitchLayoutEngine(Layout.BaseEngine engine, Properties.LayoutProperty property)
		{
			this.layoutEngine   = engine;
			this.layoutProperty = property;
		}
		
		
		public void ResetLineHeight()
		{
			this.oyMax = this.oyBase;
			this.oyMin = this.oyBase;
			
			this.lineHeight = 0;
		}
		
		public void RecordAscender(double value)
		{
			double y = this.oyBase + value;
			
			if (y > this.oyMax)
			{
				this.oyMax = y;
			}
		}
		
		public void RecordLineHeight(double value)
		{
			if (value > this.lineHeight)
			{
				this.lineHeight = value;
				
				if (this.autoLeading)
				{
					this.lineLeading = System.Math.Max (this.lineHeight, this.lineLeading);
				}
			}
		}
		
		public void RecordDescender(double value)
		{
			double y = this.oyBase + value;
			
			if (y < this.oyMin)
			{
				this.oyMin = y;
			}
		}
		
		
		public bool GetNextWord(int offset, out ulong[] text, out int length, out Unicode.BreakInfo wordBreakInfo)
		{
			wordBreakInfo = Unicode.BreakInfo.No;
			
			//	TODO: gérer le sens <-- pour l'avance du texte
			
			int end   = this.text.Length;
			int pos   = this.textStart + offset;
			
			while (pos < end)
			{
				wordBreakInfo = Unicode.Bits.GetBreakInfo (this.text[pos]);
				
				pos++;
				
				if ((wordBreakInfo == Unicode.BreakInfo.Yes) ||
					(wordBreakInfo == Unicode.BreakInfo.Optional) ||
					(wordBreakInfo == Unicode.BreakInfo.HorizontalTab))
				{
					break;
				}
			}
			
			length = pos - (this.textStart + offset);
			text   = this.GetInternalBuffer (length);
			
			if (length == 0)
			{
				return false;
			}
			
			//	Copie le texte dans le buffer temporaire. C'est plus rapide de faire
			//	la copie à la main que d'appeler System.Array.Copy :
			
			pos = this.textStart + offset;
			
			for (int i = 0; i < length; i++)
			{
				text[i] = this.text[pos+i];
			}
			
			return true;
		}
		
		public bool GetTextCopy(int offset, out ulong[] text, ref int length)
		{
			//	TODO: gérer le sens <-- pour l'avance du texte
			
			int end = System.Math.Min (this.textStart + offset + length, this.text.Length);
			int pos = this.textStart + offset;

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
		
		
		public void SelectFrame(int frameIndex, double y)
		{
			if (frameIndex == -1)
			{
				this.frameIndex  = -1;
				this.frame        = null;
				this.frameY      = 0;
				this.frameYLine = 0;
			}
			else
			{
				this.frameIndex  = frameIndex;
				this.frame        = this.frameList[this.frameIndex];
				this.frameY      = y;
				this.frameYLine = y;
			}
			
			this.frameFirstLine = 0;
		}
		
		public void DefineAvailableWidth(double width)
		{
			this.lineWidth = width + this.LeftMargin + this.RightMargin;
		}
		
		public void DefineTabDockingMark(string tabDockingMark)
		{
			if ((tabDockingMark != null) &&
				(tabDockingMark.Length > 0))
			{
				uint[] utf32;
				
				TextConverter.ConvertFromString (tabDockingMark, out utf32);
				
				int start = this.textStart + this.textOffset;
				
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
							this.textStart  = 0;
							this.textOffset = 0;
							
							return;
						}
					}
				}
			}
		}
		
		public void SelectMargins(int paragraphLineIndex)
		{
			ulong code = this.text[this.textStart + this.textOffset];
			
			Properties.MarginsProperty margins;
			double fontSizeInPoints;
			
			this.textContext.GetMargins (code, out margins);
			this.textContext.GetFontSize (code, out fontSizeInPoints);
			
			if (margins != null)
			{
				if (Properties.UnitsTools.IsScale (margins.Units))
				{
					this.mxLeft  = fontSizeInPoints * Properties.UnitsTools.ConvertToScale (paragraphLineIndex == 0 ? margins.LeftMarginFirstLine  : margins.LeftMarginBody, margins.Units);
					this.mxRight = fontSizeInPoints * Properties.UnitsTools.ConvertToScale (paragraphLineIndex == 0 ? margins.RightMarginFirstLine : margins.RightMarginBody, margins.Units);
					this.xOrigin = this.mxLeft;
				
					this.breakFenceBefore = fontSizeInPoints * Properties.UnitsTools.ConvertToScale (margins.BreakFenceBefore, margins.Units);
					this.breakFenceAfter  = fontSizeInPoints * Properties.UnitsTools.ConvertToScale (margins.BreakFenceAfter, margins.Units);
					this.enableHyphenation = margins.EnableHyphenation == Properties.ThreeState.True;
				}
				else
				{
					this.mxLeft  = Properties.UnitsTools.ConvertToPoints (paragraphLineIndex == 0 ? margins.LeftMarginFirstLine  : margins.LeftMarginBody, margins.Units);
					this.mxRight = Properties.UnitsTools.ConvertToPoints (paragraphLineIndex == 0 ? margins.RightMarginFirstLine : margins.RightMarginBody, margins.Units);
					this.xOrigin = this.mxLeft;
				
					this.breakFenceBefore = Properties.UnitsTools.ConvertToPoints (margins.BreakFenceBefore, margins.Units);
					this.breakFenceAfter  = Properties.UnitsTools.ConvertToPoints (margins.BreakFenceAfter, margins.Units);
					this.enableHyphenation = margins.EnableHyphenation == Properties.ThreeState.True;
				}
			}
		}
		
		
		private void SelectLayoutEngine(int offset)
		{
			ulong code = this.text[this.textStart + offset];
			this.textContext.GetLayoutEngine (code, out this.layoutEngine, out this.layoutProperty);
		}
		
		private void SelectMarginsAndJustification(int offset, int paragraphLineIndex, bool isLastLine)
		{
			ulong code = this.text[this.textStart + offset];
			
			Properties.MarginsProperty margins;
			double fontSizeInPoints;
			
			this.textContext.GetMargins (code, out margins);
			this.textContext.GetFontSize (code, out fontSizeInPoints);
			
			if (margins != null)
			{
				if (Properties.UnitsTools.IsScale (margins.Units))
				{
					this.mxLeft      = fontSizeInPoints * Properties.UnitsTools.ConvertToScale (paragraphLineIndex == 0 ? margins.LeftMarginFirstLine  : margins.LeftMarginBody, margins.Units);
					this.mxRight     = fontSizeInPoints * Properties.UnitsTools.ConvertToScale (paragraphLineIndex == 0 ? margins.RightMarginFirstLine : margins.RightMarginBody, margins.Units);
					this.mxLeftBody  = fontSizeInPoints * Properties.UnitsTools.ConvertToScale (margins.LeftMarginBody, margins.Units);
					this.xOrigin     = this.mxLeft;
				
					this.breakFenceBefore = fontSizeInPoints * Properties.UnitsTools.ConvertToScale (margins.BreakFenceBefore, margins.Units);
					this.breakFenceAfter  = fontSizeInPoints * Properties.UnitsTools.ConvertToScale (margins.BreakFenceAfter, margins.Units);
					this.enableHyphenation = margins.EnableHyphenation == Properties.ThreeState.True;
				}
				else
				{
					this.mxLeft      = Properties.UnitsTools.ConvertToPoints (paragraphLineIndex == 0 ? margins.LeftMarginFirstLine  : margins.LeftMarginBody, margins.Units);
					this.mxRight     = Properties.UnitsTools.ConvertToPoints (paragraphLineIndex == 0 ? margins.RightMarginFirstLine : margins.RightMarginBody, margins.Units);
					this.mxLeftBody = Properties.UnitsTools.ConvertToPoints (margins.LeftMarginBody, margins.Units);
					this.xOrigin     = this.mxLeft;
					
					this.breakFenceBefore = Properties.UnitsTools.ConvertToPoints (margins.BreakFenceBefore, margins.Units);
					this.breakFenceAfter  = Properties.UnitsTools.ConvertToPoints (margins.BreakFenceAfter, margins.Units);
					this.enableHyphenation = margins.EnableHyphenation == Properties.ThreeState.True;
				}
					
				
				this.justification = isLastLine ? margins.JustificationLastLine : margins.JustificationBody;
				this.disposition   = margins.Disposition;
			}
		}
		
		private void SelectKeep(int offset)
		{
			ulong code = this.text[this.textStart + offset];
			
			Properties.KeepProperty keep;
			
			this.textContext.GetKeep (code, out keep);
			
			if (keep != null)
			{
				this.paraStartMode  = keep.ParagraphStartMode;
				this.keepStartLines = System.Math.Max (1, keep.StartLines);
				this.keepEndLines   = System.Math.Max (1, keep.EndLines);
				
				//	Le TextFitter peut forcer un 'keep with previous paragraph' basé
				//	sur les informations relatives au paragraphe précédent; on doit
				//	donc conserver l'état 'keepWithPrevPara = true'...
				
				this.keepWithPrevPara |= keep.KeepWithPreviousParagraph == Properties.ThreeState.True;
				this.keepWithNextPara  = keep.KeepWithNextParagraph == Properties.ThreeState.True;
			}
			else
			{
				this.paraStartMode     = Properties.ParagraphStartMode.Anywhere;
				this.keepStartLines    = 1;
				this.keepEndLines      = 1;
				
				this.keepWithNextPara = false;
			}
			
			if ((this.paraDefaultStartMode == Properties.ParagraphStartMode.Undefined) ||
				(this.paraDefaultStartMode == Properties.ParagraphStartMode.Anywhere))
			{
			}
			else
			{
				//	Combine les deux modes de début de paragraphe :
				
				switch (this.paraDefaultStartMode)
				{
					case Properties.ParagraphStartMode.NewFrame:
						
						//	Si le paragraphe est précédé d'un saut de frame, on force
						//	un saut uniquement si le paragraphe lui-même ne force pas
						//	déjà un saut de page :
						
						switch (this.paraStartMode)
						{
							case Properties.ParagraphStartMode.Undefined:
							case Properties.ParagraphStartMode.Anywhere:
								this.paraStartMode = this.paraDefaultStartMode;
								break;
						}
						break;
					
					case Properties.ParagraphStartMode.NewPage:
					case Properties.ParagraphStartMode.NewOddPage:
					case Properties.ParagraphStartMode.NewEvenPage:
						
						//	Si le paragraphe est précédé d'un saut de page, on force
						//	un saut uniquement si le paragraphe lui-même ne force pas
						//	déjà un saut de page précis (page paire/impaire) :
						
						switch (this.paraStartMode)
						{
							case Properties.ParagraphStartMode.Undefined:
							case Properties.ParagraphStartMode.Anywhere:
							case Properties.ParagraphStartMode.NewFrame:
							case Properties.ParagraphStartMode.NewPage:
								this.paraStartMode = this.paraDefaultStartMode;
								break;
						}
						break;
					
				}
			}
		}
		
		private void SelectLineHeightAndLeading(int offset, double lineHeight, double ascender, double descender)
		{
			ulong code = this.text[this.textStart + offset];
			
			OpenType.Font font;
			double        fontSize;
			double        fontScale;
			
			Properties.LeadingProperty leadingProperty;
			
			this.textContext.GetFontAndSize (code, out font, out fontSize, out fontScale);
			this.textContext.GetLeading (code, out leadingProperty);
			
			fontSize *= fontScale;
			
			if (font != null)
			{
				ascender  = System.Math.Max (ascender, font.GetAscender (fontSize));
				descender = System.Math.Min (descender, font.GetDescender (fontSize));
				fontSize = System.Math.Max (fontSize, (ascender - descender) * 1.2);
				
				double autoScale = 1.0;
				double leading    = fontSize * autoScale;
				
				if (leadingProperty != null)
				{
					if ((double.IsNaN (leadingProperty.Leading)) ||
						(leadingProperty.Leading == 0))
					{
						//	Pas d'interligne spécifié : utilise l'interligne automatique.
					}
					else
					{
						if ((leadingProperty.LeadingUnits == Properties.SizeUnits.Percent) ||
							(leadingProperty.LeadingUnits == Properties.SizeUnits.PercentNotCombining))
						{
							leading   *= leadingProperty.Leading;
							autoScale = leadingProperty.Leading;
						}
						else
						{
							leading    = leadingProperty.LeadingInPoints;
							autoScale = 0;
						}
					}
					
					this.lineSpaceBefore = double.IsNaN (leadingProperty.SpaceBefore) ? 0 : leadingProperty.SpaceBeforeInPoints;
					this.lineSpaceAfter  = double.IsNaN (leadingProperty.SpaceAfter)  ? 0 : leadingProperty.SpaceAfterInPoints;
				}
				else
				{
					this.lineSpaceBefore = 0;
					this.lineSpaceAfter  = 0;
				}
				
				this.autoLeading = autoScale != 0;
				this.lineLeading = leading;
				this.lineAlign   = leadingProperty == null ? Properties.AlignMode.None : leadingProperty.AlignMode;
				this.lineHeight  = System.Math.Max (lineHeight, leading);
			}
			
			this.oyMax = this.oyBase + ascender;
			this.oyMin = this.oyBase + descender;
		}
		
		private void SelectVerticalAlignment(int paragraphLineIndex)
		{
			switch (this.lineAlign)
			{
				case Properties.AlignMode.First:
					if ((paragraphLineIndex == 0) ||
						(this.frameY == 0))
					{
						this.lineSyncToGrid = true;
						return;
					}
					break;
				
				case Properties.AlignMode.All:
					this.lineSyncToGrid = true;
					return;
				
				default:
					break;
			}
			
			this.lineSyncToGrid = false;
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
		
		
		private void UpdateFrameIndex(ref int frameIndex, bool isTopOfFrame, Properties.ParagraphStartMode paraStartMode)
		{
			//	Un paragraphe peut imposer des contraintes quant à sa position dans
			//	un frame (début de frame, frame en début de page/page paire/page
			//	impaire).
			
			switch (paraStartMode)
			{
				case Properties.ParagraphStartMode.NewFrame:
					if (! isTopOfFrame)
					{
						frameIndex++;
					}
					break;
				
				case Properties.ParagraphStartMode.NewPage:
					if ((this.IsFirstFrameInPage (frameIndex)) &&
						(isTopOfFrame))
					{
						//	OK : déjà au début d'une page.
					}
					else
					{
						frameIndex++;
						
						while ((frameIndex < this.frameList.Count)
							&& (this.IsFirstFrameInPage (frameIndex) == false))
						{
							frameIndex++;
						}
					}
					break;
				
				case Properties.ParagraphStartMode.NewEvenPage:
					if ((this.IsFirstFrameInPage (frameIndex)) &&
						(this.IsFrameInEvenPage (frameIndex)) &&
						(isTopOfFrame))
					{
						//	OK : déjà au début d'une page paire.
					}
					else
					{
						frameIndex++;
						
						while ((frameIndex < this.frameList.Count)
							&& ((this.IsFrameInEvenPage (frameIndex) == false) || (this.IsFirstFrameInPage (frameIndex) == false)))
						{
							frameIndex++;
						}
					}
					break;
				
				case Properties.ParagraphStartMode.NewOddPage:
					if ((this.IsFirstFrameInPage (frameIndex)) &&
						(this.IsFrameInOddPage (frameIndex)) &&
						(isTopOfFrame))
					{
						//	OK : déjà au début d'une page impaire.
					}
					else
					{
						frameIndex++;
						
						while ((frameIndex < this.frameList.Count)
							&& ((this.IsFrameInOddPage (frameIndex) == false) || (this.IsFirstFrameInPage (frameIndex) == false)))
						{
							frameIndex++;
						}
					}
					break;
			}
		}
		
		
		private bool IsFirstFrameInPage(int frameIndex)
		{
			if (frameIndex == 0)
			{
				return true;
			}
			
			if ((frameIndex > 0) &&
				(frameIndex < this.frameList.Count))
			{
				ITextFrame frameA = this.frameList[frameIndex-1];
				ITextFrame frameB = this.frameList[frameIndex];
				
				System.Diagnostics.Debug.Assert (frameA != null);
				System.Diagnostics.Debug.Assert (frameB != null);
				
				return frameA.PageNumber != frameB.PageNumber;
			}
			
			return false;
		}
		
		private bool IsFrameInEvenPage(int frameIndex)
		{
			if ((frameIndex > -1) &&
				(frameIndex < this.frameList.Count))
			{
				ITextFrame frame = this.frameList[frameIndex];
				
				System.Diagnostics.Debug.Assert (frame != null);
				
				int page = frame.PageNumber;
				
				return (page > 0) && ((page & 0x1) == 0);
			}
			
			return false;
		}
		
		private bool IsFrameInOddPage(int frameIndex)
		{
			if ((frameIndex > -1) &&
				(frameIndex < this.frameList.Count))
			{
				ITextFrame frame = this.frameList[frameIndex];
				
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
				this.textOffset    = context.textOffset;
				this.layoutEngine  = context.layoutEngine;
				this.ox            = context.ox;
				this.oyBase        = context.oyBase;
				
				this.frameIndex       = context.frameIndex;
				this.frameY           = context.frameY;
				this.frameFirstLine   = context.frameFirstLine;
			}
			
			
			public void Restore(Context context)
			{
				this.Restore (context, true);
			}
			
			public void Restore(Context context, bool restoreFrame)
			{
				double ascender  = context.oyMax - context.oyBase;
				double descender = context.oyMin - context.oyBase;
				
				context.snapshot      = this.snapshot;
				context.textOffset   = this.textOffset;
				context.layoutEngine = this.layoutEngine;
				
				context.ox      = this.ox;
				context.oyBase = this.oyBase;
				context.oyMax  = this.oyBase + ascender;
				context.oyMin  = this.oyBase + descender;
				
				if (restoreFrame)
				{
					context.SelectFrame (this.frameIndex, this.frameY);
				}
				else
				{
					context.SelectFrame (context.frameIndex, this.frameY);
				}
				
				context.frameFirstLine = this.frameFirstLine;
			}
			
			public void FixFrame(int frameIndex, double frameY, int frameFirstLine)
			{
				this.frameIndex      = frameIndex;
				this.frameY          = frameY;
				this.frameFirstLine = frameFirstLine;
			}
			
			
			private Snapshot					snapshot;
			private int							textOffset;
			private Layout.BaseEngine			layoutEngine;
			private double						ox, oyBase;
			private int							frameIndex;
			private double						frameY;
			private int							frameFirstLine;
		}
		#endregion
		
		
		private TextContext						textContext;
		private ulong[]							text;
		private int								textStart;
		private int								textOffset;
		private StretchProfile					textProfile;
		private StretchProfile.Scales			textScales;
		private double							textWidth;
		private double							textGlue;
		
		private FrameList						frameList;
		private int								frameIndex = -1;
		private ITextFrame						frame;
		private double							frameY;
		private double							frameYLine;
		private int								frameFirstLine;			//	# première ligne du paragraphe dans ce frame
		
		private int								leftToRight;
		
		private double							ox;
		private double							oxLineStart;
		private double							oyBase;
		private double							oyMax;
		private double							oyMin;
		
		private double							lineY1;
		private double							lineY2;
		
		private double							lineHeight;
		private double							lineWidth;
		private double							lineLeading;
		private bool							autoLeading;
		private Properties.AlignMode			lineAlign;
		private bool							lineSyncToGrid;
		private double							lineSkipBefore;
		private double							lineSpaceBefore;
		private double							lineSpaceAfter;
		
		private Properties.ParagraphStartMode	paraDefaultStartMode;
		private Properties.ParagraphStartMode	paraStartMode;
		private int								keepStartLines;
		private int								keepEndLines;
		private bool							keepWithPrevPara;
		private bool							keepWithNextPara;
		
		private Layout.FrameLineFenceDictionary	frameFences = new FrameLineFenceDictionary ();
		
		private double							mxLeft;
		private double							mxLeftBody;
		private double							mxRight;
		private double							xOrigin;
		
		private bool							useTabIndentation;
		private double							tabIndentationX;
		
		private double							breakFenceBefore;
		private double							breakFenceAfter;
		
		private double							justification;
		private double							disposition;
		
		private bool							enableHyphenation;
		private BreakMode						breakMode;
		
		private bool							showControlCharacters;
		
		private bool							disableSimpleRendering;
		private bool							disableFontBaselineOffset;
		
		private Layout.BaseEngine				layoutEngine;
		private Properties.LayoutProperty		layoutProperty;
		
		private Snapshot						snapshot;
		
		private ulong[]							buffer;
		
		Properties.AbstractXlineProperty[]		xlineProperties;
		Properties.FontColorProperty			xlineTextColor;
		System.Collections.ArrayList			xlineRecords;
		private double							xlineEndX;
	}
}
