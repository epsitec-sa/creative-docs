namespace Epsitec.Common.Widgets
{
	public class AnchorEventArgs : System.EventArgs
	{
		public AnchorEventArgs(double x, double y, double dx, double dy, int index)
		{
			this.rect  = new Drawing.Rectangle(x, y, dx, dy);
			this.index = index;
		}
		
		
		public Drawing.Rectangle			Bounds
		{
			get
			{
				return this.rect;
			}
		}
		
		public int							Index
		{
			get
			{
				return this.index;
			}
		}
				
		
		private Drawing.Rectangle			rect;
		private int							index;
	}
	
	public delegate void AnchorEventHandler(object sender, AnchorEventArgs e);
	
	
	/// <summary>
	/// La classe TextLayout permet de stocker et d'afficher des contenus
	/// riches (un sous-ensemble très restreint de HTML).
	/// </summary>
	public class TextLayout
	{
		// Constructeur.
		public TextLayout()
		{
		}
		

		// Texte associé, contenant des commandes HTML.
		public string Text
		{
			get
			{
				return this.text;
			}

			set
			{
				if ( value == null )
				{
					value = "";
				}
				
				if ( this.text != value )
				{
					int offsetError;
					if ( TextLayout.CheckSyntax(value, out offsetError) )
					{
						this.text = value;
						this.textLength = value.Length;
						this.isDirty = true;
					}
					else
					{
						throw new System.FormatException("Syntax error at char " + offsetError.ToString());
					}
				}
			}
		}
		
		public int TextLength
		{
			get { return this.textLength; }
		}
		
		// Fonte par défaut.
		public Drawing.Font Font
		{
			get
			{
				return this.font;
			}

			set
			{
				if ( value != this.font )
				{
					this.font = value;
					this.isDirty = true;
				}
			}
		}
		
		// Taille de la fonte par défaut.
		public double FontSize
		{
			get
			{
				return this.fontSize;
			}

			set
			{
				if ( this.fontSize != value )
				{
					this.fontSize = value;
					this.isDirty = true;
				}
			}
		}

		// Couleur pour les liens.
		public static Drawing.Color AnchorColor
		{
			get
			{
				return TextLayout.anchorColor;
			}

			set
			{
				TextLayout.anchorColor = value;
			}
		}

		// Alignement du texte dans le rectangle.
		public Drawing.ContentAlignment	Alignment
		{
			get
			{
				return this.alignment;
			}

			set
			{
				if ( this.alignment != value )
				{
					this.alignment = value;
					this.isDirty = true;
				}
			}
		}

		// Mode de césure.
		public Drawing.TextBreakMode BreakMode
		{
			get
			{
				return this.breakMode;
			}

			set
			{
				this.breakMode = value;
			}
		}
		
		// Dimensions du rectangle.
		public Drawing.Size LayoutSize
		{
			get
			{
				return this.layoutSize;
			}

			set
			{
				if ( this.layoutSize != value )
				{
					this.layoutSize = value;
					this.isDirty = true;
				}
			}
		}

		// Retourne les dimensions du texte indépendament de LayoutSize,
		// s'il est mis sur une seule ligne.
		public Drawing.Size SingleLineSize
		{
			get
			{
				Drawing.Size originalSize = this.LayoutSize;
				Drawing.ContentAlignment originalAlignment = this.Alignment;

				this.LayoutSize = new Drawing.Size(TextLayout.Infinite, TextLayout.Infinite);
				this.Alignment  = Drawing.ContentAlignment.TopLeft;

				Drawing.Point end = this.FindTextEnd();
				
				this.LayoutSize = originalSize;
				this.Alignment  = originalAlignment;

				return new Drawing.Size(end.X, TextLayout.Infinite-end.Y);
			}
		}
		
		// Retourne le nombre de lignes total dans le layout courant
		// (y compris les lignes qui débordent).
		public int TotalLineCount
		{
			get
			{
				this.UpdateLayout();
				return this.totalLine;
			}
		}
		
		// Retourne le nombre de lignes visibles dans le layout courant
		// (sans compter les lignes qui débordent).
		public int VisibleLineCount
		{
			get
			{
				this.UpdateLayout();
				return this.visibleLine;
			}
		}
		
		// Retourne le rectangle englobant du layout, en tenant compte de
		// toutes les lignes (all=true) ou seulement des lignes visibles (all=false).
		// Si le texte est aligné sur le bord gauche, rectangle.Left n'est pas
		// forcément égal à 0.
		protected Drawing.Rectangle RectangleBounds(bool all)
		{
			this.UpdateLayout();

			Drawing.Rectangle totalRect = Drawing.Rectangle.Empty;
			foreach ( JustifBlock block in this.blocks )
			{
				if ( !all && !block.visible )  continue;

				Drawing.Rectangle blockRect;
				if ( block.image )
				{
					blockRect = new Drawing.Rectangle();
					blockRect.Left   = 0;
					blockRect.Right  = block.width;
					blockRect.Top    = block.imageAscender;
					blockRect.Bottom = block.imageDescender;
				}
				else
				{
					blockRect = block.font.GetTextBounds(block.text);
					blockRect.Scale(block.fontSize);
				}
				blockRect.Offset(block.pos.X, block.pos.Y);
				totalRect.MergeWith(blockRect);
			}
			return totalRect;
		}

		// Retourne le rectangle englobant du layout courant; ce
		// rectangle comprend toutes les lignes, même celles qui débordent.
		public Drawing.Rectangle TotalRectangle
		{
			get
			{
				return this.RectangleBounds(true);
			}
		}
		
		// Retourne le rectangle englobant du layout courant; ce
		// rectangle comprend uniquement les lignes visibles.
		public Drawing.Rectangle VisibleRectangle
		{
			get
			{
				return this.RectangleBounds(false);
			}
		}
		
		// Retourne le rectangle standard englobant du layout courant; ce
		// rectangle ne dépend pas de la hauteur des lettres du texte.
		// Le rectangle aura la même hauteur avec "ace" ou "Ap".
		public Drawing.Rectangle StandardRectangle
		{
			get
			{
				this.UpdateLayout();

				Drawing.Rectangle totalRect = Drawing.Rectangle.Empty;
				foreach ( JustifBlock block in this.blocks )
				{
					if ( !block.visible )  continue;

					Drawing.Rectangle blockRect = new Drawing.Rectangle();
					blockRect.Left  = 0;
					blockRect.Right = block.width;
					if ( block.image )
					{
						blockRect.Top    = block.imageAscender;
						blockRect.Bottom = block.imageDescender;
					}
					else
					{
						blockRect.Top    = block.fontSize*block.font.Ascender;
						blockRect.Bottom = block.fontSize*block.font.Descender;
					}
					blockRect.Offset(block.pos.X, block.pos.Y);
					totalRect.MergeWith(blockRect);
				}
				return totalRect;
			}
		}
		
		// Gestionnaire d'images.
		public Drawing.IImageProvider ImageProvider
		{
			get
			{
				return this.imageProvider;
			}

			set
			{
				this.imageProvider = value;
			}
		}
		
		
		// Dessine le texte, en fonction du layout...
		// Si une couleur est donnée avec uniqueColor, tout le texte est peint
		// avec cette couleur, en ignorant les <font color=...>.
		public void Paint(Drawing.Point pos, Drawing.Graphics graphics)
		{
			this.Paint(pos, graphics, Drawing.Rectangle.Infinite, Drawing.Color.Empty, Drawing.GlyphPaintStyle.Normal);
		}

		public void Paint(Drawing.Point pos, Drawing.Graphics graphics, Drawing.Rectangle clipRect, Drawing.Color uniqueColor, Drawing.GlyphPaintStyle paintStyle)
		{
			this.UpdateLayout();

			IAdorner adorner = Adorner.Factory.Active;
			foreach ( JustifBlock block in this.blocks )
			{
				if ( !block.visible )  continue;

				if ( block.image )
				{
					Drawing.Image image = this.imageProvider.GetImage(block.text);
					
					if ( image == null )
					{
						throw new System.FormatException(string.Format("<img> tag references unknown image '{0}' while painting. Current directory is {1}.", block.text, System.IO.Directory.GetCurrentDirectory ()));
					}
					
					if ( image.IsPaintStyleDefined(paintStyle) )
					{
						image = image.GetImageForPaintStyle(paintStyle);
					}
					
					image.DefineZoom(graphics.GetTransformZoom());
					image.DefineColor(uniqueColor);
					image.DefineAdorner(adorner);
					
					double dx = image.Width;
					double dy = image.Height;
					double ix = pos.X+block.pos.X;
					double iy = pos.Y+block.pos.Y+block.imageDescender;
					
					if ( block.anchor )
					{
						this.OnAnchor(new AnchorEventArgs (ix, iy, dx, dy, block.beginIndex));
					}
					
					graphics.Align(ref ix, ref iy);
					graphics.AddFilledRectangle(ix, iy, dx, dy);
					
					Drawing.Transform t = new Drawing.Transform();
					Drawing.Bitmap    b = image.BitmapImage;
					
					t.Translate(ix, iy);
					b.MergeTransform (t);
					
					graphics.ImageRenderer.BitmapImage = b;
					graphics.ImageRenderer.Transform = t;
					graphics.RenderImage();
					continue;
				}

				Drawing.Color color;
				if ( uniqueColor.IsEmpty )
				{
					if ( block.anchor )
					{
						color = TextLayout.anchorColor;
					}
					else
					{
						color = block.fontColor;
					}
				}
				else
				{
					color = uniqueColor;
				}
				
				double x = pos.X+block.pos.X;
				double y = pos.Y+block.pos.Y;
				
				if ( block.anchor )
				{
					double ascender  = block.font.Ascender * block.fontSize;
					double descender = block.font.Descender * block.fontSize;
					
					this.OnAnchor(new AnchorEventArgs(x, y+descender, block.width, ascender-descender, block.beginIndex));
				}

				graphics.PaintText(x, y, block.text, block.font, block.fontSize, color);

				if ( block.underline )
				{
					double p1x = pos.X+block.pos.X;
					double p2x = p1x+block.width;
					double py = pos.Y+block.pos.Y;
					JustifLine line = (JustifLine)this.lines[block.indexLine];
					py += line.descender/2;
					graphics.Align(ref p1x, ref py);
					graphics.Align(ref p2x, ref py);
					py -= 0.5;  // pour feinter l'anti-aliasing !
					graphics.LineWidth = 1;
					graphics.AddLine(p1x, py, p2x, py);
					graphics.RenderSolid(color);
				}
			}
		}
		
		
		public event AnchorEventHandler Anchor;
		
		protected virtual void OnAnchor(AnchorEventArgs e)
		{
			if ( this.Anchor != null )
			{
				this.Anchor(this, e);
			}
		}
		
		
		// Trouve l'index dans le texte interne qui correspond à la
		// position indiquée. Retourne -1 en cas d'échec.
		public int DetectIndex(Drawing.Point pos)
		{
			if ( pos.X < 0                      ||
				 pos.X > this.layoutSize.Width  ||
				 pos.Y < 0                      ||
				 pos.Y > this.layoutSize.Height )  return -1;

			this.UpdateLayout();

			foreach ( JustifLine line in this.lines )
			{
				if ( !line.visible )  continue;

				if ( pos.Y <= line.pos.Y+line.ascender  &&
					 pos.Y >= line.pos.Y+line.descender )
				{
					for ( int j=line.firstBlock ; j<=line.lastBlock ; j++ )
					{
						JustifBlock block = (JustifBlock)this.blocks[j];

						double width = block.width;
						if ( j == this.blocks.Count-1 )  // dernier bloc ?
						{
							width = this.layoutSize.Width-block.pos.X;
						}
						else
						{
							JustifBlock nextBlock = (JustifBlock)this.blocks[j+1];
							if ( nextBlock.bol )
							{
								width = this.layoutSize.Width-block.pos.X;
							}
						}

						if ( pos.X >= block.pos.X       &&
							 pos.X <= block.pos.X+width )
						{
							if ( block.image )
							{
								return block.beginIndex;
							}
							else
							{
								double[] charsWidth = block.font.GetTextCharEndX(block.text);
								double left = 0;
								double right;
								for ( int k=0 ; k<charsWidth.Length ; k++ )
								{
									right = charsWidth[k]*block.fontSize;
									if ( pos.X-line.pos.X <= left+(right-left)/2 )
									{
										return block.beginIndex+k;
									}
									left = right;
								}
								return block.beginIndex+charsWidth.Length;
							}
						}
					}
				}
			}
			return -1;
		}
		
		// Trouve l'index dans le texte interne qui correspond à la
		// position indiquée. Retourne -1 en cas d'échec.
		public int DetectIndex(double posx, int posLine)
		{
			if ( posx < 0                     ||
				 posx > this.layoutSize.Width )  return -1;

			this.UpdateLayout();

			foreach ( JustifLine line in this.lines )
			{
				if ( !line.visible )  continue;

				if ( posLine == line.rank )
				{
					for ( int j=line.firstBlock ; j<=line.lastBlock ; j++ )
					{
						JustifBlock block = (JustifBlock)this.blocks[j];

						double width = block.width;
						if ( j == this.blocks.Count-1 )  // dernier bloc ?
						{
							width = this.layoutSize.Width-block.pos.X;
						}
						else
						{
							JustifBlock nextBlock = (JustifBlock)this.blocks[j+1];
							if ( nextBlock.bol )
							{
								width = this.layoutSize.Width-block.pos.X;
							}
						}

						if ( posx >= block.pos.X       &&
							 posx <= block.pos.X+width )
						{
							if ( block.image )
							{
								return block.beginIndex;
							}
							else
							{
								double[] charsWidth = block.font.GetTextCharEndX(block.text);
								double left = 0;
								double right;
								for ( int k=0 ; k<charsWidth.Length ; k++ )
								{
									right = charsWidth[k]*block.fontSize;
									if ( posx-line.pos.X <= left+(right-left)/2 )
									{
										return block.beginIndex+k;
									}
									left = right;
								}
								return block.beginIndex+charsWidth.Length;
							}
						}
					}
				}
			}
			return -1;
		}

		// Détecte s'il y a un lien hypertexte dans la liste des
		// tags actifs à la position en question. Si oui, extrait la chaîne
		// de l'argument href, en supprimant les guillemets.
		public string DetectAnchor(Drawing.Point pos)
		{
			int index = this.DetectIndex(pos);
			return this.FindAnchor(index);
		}
		
		public string FindAnchor(int index)
		{
			if ( index < 0 )  return null;
			int offset = this.FindOffsetFromIndex(index);
			if ( offset < 0 )  return null;
			
			string[] tags;
			if ( !this.AnalyseTagsAtOffset(offset, out tags) )  return null;
			for ( int i=0 ; i<tags.Length ; i++ )
			{
				if ( !tags[i].StartsWith("<a ") )  continue;
				int beginIndex = tags[i].IndexOf("href=\"");
				if ( beginIndex < 0 )  continue;
				beginIndex += 6;
				int endIndex = tags[i].IndexOf("\"", beginIndex);
				if ( endIndex < 0 )  continue;
				return tags[i].Substring(beginIndex, endIndex-beginIndex);
			}
			return null;
		}
		

		// Retourne la position horizontale correspondant à un index dans un bloc.
		protected double IndexToPosX(JustifBlock block, int index)
		{
			if ( index <= block.beginIndex )  return block.pos.X;
			if ( index >  block.endIndex   )  return block.pos.X+block.width;
			double[] charsWidth = block.font.GetTextCharEndX(block.text);
			return block.pos.X+charsWidth[index-block.beginIndex-1]*block.fontSize;
		}

		// Retourne un tableau avec les rectangles englobant le texte
		// spécifié par son début et sa fin. Il y a un rectangle par ligne.
		public Drawing.Rectangle[] FindTextRange(Drawing.Point pos, int indexBegin, int indexEnd)
		{
			if ( indexBegin >= indexEnd )  return new Drawing.Rectangle[0];

			this.UpdateLayout();

			System.Collections.ArrayList list = new System.Collections.ArrayList();
			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Top    = -TextLayout.Infinite;
			rect.Bottom =  TextLayout.Infinite;
			rect.Left   =  TextLayout.Infinite;
			rect.Right  = -TextLayout.Infinite;
			foreach ( JustifBlock block in this.blocks )
			{
				JustifLine line = (JustifLine)this.lines[block.indexLine];
				if ( !block.visible )  continue;

				int localBegin = System.Math.Max(indexBegin, block.beginIndex);
				int localEnd   = System.Math.Min(indexEnd,   block.endIndex  );

				if ( localBegin >= localEnd )  continue;

				double top    = line.pos.Y+line.ascender;
				double bottom = line.pos.Y+line.descender;

				if ( rect.Top    != top    ||
					 rect.Bottom != bottom )  // rectangle dans autre ligne ?
				{
					if ( rect.Top > -TextLayout.Infinite && rect.Left < TextLayout.Infinite )
					{
						list.Add(rect);
					}

					rect.Top    = top;
					rect.Bottom = bottom;
					rect.Left   =  TextLayout.Infinite;
					rect.Right  = -TextLayout.Infinite;
				}

				if ( block.image )
				{
					rect.Left  = System.Math.Min(rect.Left,  block.pos.X);
					rect.Right = System.Math.Max(rect.Right, block.pos.X+block.width);
				}
				else
				{
					rect.Left  = System.Math.Min(rect.Left,  IndexToPosX(block, localBegin));
					rect.Right = System.Math.Max(rect.Right, IndexToPosX(block, localEnd  ));
				}
			}
			
			if ( rect.Top > -TextLayout.Infinite && rect.Left < TextLayout.Infinite )
			{
				list.Add(rect);
			}
			
			Drawing.Rectangle[] rects = new Drawing.Rectangle[list.Count];
			list.CopyTo(rects);
			
			for (int i = 0; i < rects.Length; i++)
			{
				rects[i].Offset (pos.X, pos.Y);
			}
					
			return rects;
		}
		
		public Drawing.Point GetLineOrigin(int line)
		{
			Drawing.Point pos;
			
			double ascender;
			double descender;
			double width;
			
			if (this.GetLineGeometry (line, out pos, out ascender, out descender, out width))
			{
				return pos;
			}
			
			return Drawing.Point.Empty;
		}
		
		public bool GetLineGeometry(int line, out Drawing.Point pos, out double ascender, out double descender, out double width)
		{
			this.UpdateLayout();
			
			if ( line >= 0 && line < this.lines.Count )
			{
				JustifLine info = (JustifLine)this.lines[line];
				
				pos       = info.pos;
				ascender  = info.ascender;
				descender = info.descender;
				width     = info.width;
				
				return true;
			}
			
			pos       = Drawing.Point.Empty;
			ascender  = 0;
			descender = 0;
			width     = 0;
			
			return false;
		}
		
		
		
		// Retourne le rectangle correspondant au curseur.
		// Indique également le numéro de la ligne (0..n).
		public Drawing.Rectangle FindTextCursor(int index, out int rankLine)
		{
			this.UpdateLayout();

			rankLine = -1;
			foreach ( JustifBlock block in this.blocks )
			{
				JustifLine line = (JustifLine)this.lines[block.indexLine];
				if ( !block.visible )  continue;

				if ( index >= block.beginIndex && index <= block.endIndex )
				{
					Drawing.Rectangle rect = new Drawing.Rectangle();
					rect.Top    = line.pos.Y+line.ascender;
					rect.Bottom = line.pos.Y+line.descender;
					if ( block.image )
					{
						rect.Left  = block.pos.X;
						rect.Right = block.pos.X+block.width;
					}
					else
					{
						rect.Left  = IndexToPosX(block, index);
						rect.Right = rect.Left;
						rect.Left  -= 0.5;
						rect.Right += 0.5;
					}
					rankLine = line.rank;
					return rect;
				}
			}
			
			return Drawing.Rectangle.Empty;
		}

		// Retourne le coin inférieur/droite du dernier caractère.
		public Drawing.Point FindTextEnd()
		{
			this.UpdateLayout();

			if ( this.blocks.Count == 0 )
			{
				return new Drawing.Point(0, 0);
			}

			JustifBlock block = (JustifBlock)this.blocks[this.blocks.Count-1];
			Drawing.Point pos = new Drawing.Point(block.pos.X+block.width, block.pos.Y+block.font.Descender*block.fontSize);
			return pos;
		}
		
		
		// Si on est au début d'un tag, donne la longueur jusqu'à la fin.
		public int AdvanceTag(int offset)
		{
			if ( offset >= this.textLength )  return 0;

			if ( this.text[offset] == '<' )  // tag <xx> ?
			{
				int initial = offset;
				while ( this.text[offset] != '>' )
				{
					offset ++;
					if ( offset >= this.textLength )  break;
				}
				return offset-initial+1;
			}

			if ( this.text[offset] == '&' )  // tag &xx; ?
			{
				int initial = offset;
				while ( this.text[offset] != ';' )
				{
					offset ++;
					if ( offset >= this.textLength )  break;
				}
				return offset-initial+1;
			}

			return 1;
		}

		// Si on est à la fin d'un tag, donne la longueur jusqu'au début.
		public int RecedeTag(int offset)
		{
			if ( offset <= 0 )  return 0;
			offset --;

			if ( this.text[offset] == '>' )  // tag <xx> ?
			{
				int initial = offset;
				while ( this.text[offset] != '<' )
				{
					offset --;
					if ( offset == 0 )  break;
				}
				return initial-offset+1;
			}

			if ( this.text[offset] == ';' )  // tag &xx; ?
			{
				int initial = offset;
				while ( this.text[offset] != '&' )
				{
					offset --;
					if ( initial-offset > 10 || this.text[offset] == ';' )  return 1;
					if ( offset == 0 )  break;
				}
				return initial-offset+1;
			}

			return 1;
		}


		// Retourne l'offset dans le texte interne, correspondant à l'index
		// spécifié pour le texte sans tags. On saute tous les tags qui précèdent
		// le caractère indiqué (textIndex=0 => premier caractère non tag dans
		// le texte).
		public int FindOffsetFromIndex(int textIndex)
		{
			int    index = 0;
			int    beginOffset;
			int    endOffset = 0;
			
			while ( endOffset <= this.textLength )
			{
				if ( endOffset == this.textLength )  return endOffset;
				beginOffset = endOffset;

				if ( this.text[endOffset] == '<' )
				{
					int length = this.text.IndexOf(">", endOffset)-endOffset+1;
					if ( length < 0 )  return -1;
					endOffset += length;

					string tag = this.text.Substring(beginOffset, length).ToLower();
					if ( tag != "<br/>" )  continue;
				}
				else if ( this.text[endOffset] == '&' )
				{
					int length = this.text.IndexOf(";", endOffset)-endOffset+1;
					if ( length < 0 )  return -1;
					endOffset += length;
				}
				else
				{
					endOffset ++;
				}

				if ( index == textIndex )  return beginOffset;
				index ++;
			}
			
			return -1;
		}
		
		// Retourne l'index dans le texte propre, correspondant à l'offset
		// spécifié dans le texte avec tags.
		public int FindIndexFromOffset(int taggedTextOffset)
		{
			int    index = 0;
			int    beginOffset;

			if ( taggedTextOffset == 0 )  return index;

			int endOffset = 0;
			
			while ( endOffset < this.textLength )
			{
				beginOffset = endOffset;

				if ( this.text[endOffset] == '<' )
				{
					int length = this.text.IndexOf(">", endOffset)-endOffset+1;
					if ( length < 0 )  return -1;
					endOffset += length;

					string tag = this.text.Substring(beginOffset, length).ToLower();
					if ( tag == "<br/>" )
					{
						index ++;
					}
				}
				else if ( this.text[endOffset] == '&' )
				{
					int length = this.text.IndexOf(";", endOffset)-endOffset+1;
					if ( length < 0 )  return -1;
					endOffset += length;
					index ++;
				}
				else
				{
					endOffset ++;
					index ++;
				}

				if ( endOffset >= taggedTextOffset )  return index;
			}
			
			return -1;
		}
		
		
		// Enlève un tag à la fin de la liste.
		protected static bool DeleteTagsList(string endTag, System.Collections.ArrayList list)
		{
			System.Diagnostics.Debug.Assert(endTag.StartsWith("</"));
			System.Diagnostics.Debug.Assert(endTag.EndsWith(">"));

			endTag = endTag.Substring(2, endTag.Length-3).ToLower();  // </b> -> b

			for ( int i=list.Count-1 ; i>=0 ; i-- )
			{
				string s = (string)list[i];
				if ( s.IndexOf(endTag) == 1 )
				{
					list.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		// Parcourt le texte et accumule les informations sur les tags <>
		// reconnus.
		public bool AnalyseTagsAtOffset(int offset, out string[] tags)
		{
			if ( offset < 0 || offset > this.textLength )
			{
				tags = null;
				return false;
			}

			System.Collections.Hashtable parameters;
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			int    beginOffset;
			int    endOffset = 0;
			while ( endOffset < offset )
			{
				beginOffset = endOffset;
				TextLayout.Tag tag = TextLayout.ParseTag(this.text, ref endOffset, out parameters);
				if ( tag == TextLayout.Tag.None )  continue;

				string sTag = this.text.Substring(beginOffset, endOffset-beginOffset);

				switch ( tag )
				{
					case TextLayout.Tag.Bold:
					case TextLayout.Tag.Italic:
					case TextLayout.Tag.Underline:
					case TextLayout.Tag.Mnemonic:
					case TextLayout.Tag.Font:
					case TextLayout.Tag.Anchor:
						list.Add(sTag);
						break;

					case TextLayout.Tag.BoldEnd:
					case TextLayout.Tag.ItalicEnd:
					case TextLayout.Tag.UnderlineEnd:
					case TextLayout.Tag.MnemonicEnd:
					case TextLayout.Tag.FontEnd:
					case TextLayout.Tag.AnchorEnd:
						TextLayout.DeleteTagsList(sTag, list);
						break;
				}
			}

			tags = new string[list.Count];
			list.CopyTo(tags);
			return true;
		}
		
		// Retourne le caractère à un offset quelconque, en interprétant les
		// commandes &...;
		public static char AnalyseMetaChar(string text, ref int offset)
		{
			if ( text[offset] == '&' )
			{
				int length = text.IndexOf(";", offset)-offset+1;
				
				if ( length < 3 )
				{
					throw new System.FormatException("Bad meta");
				}
				
				char code;
				string meta = text.Substring(offset, length).ToLower();
				
				switch ( meta )
				{
					case "&lt;":	code = '<';			break;
					case "&gt;":	code = '>';			break;
					case "&amp;":	code = '&';			break;
					case "&quot;":	code = '"';			break;
					case "&nbsp;":	code = (char)160;	break;
					
					default:
						throw new System.FormatException("Bad meta: " + meta);
				}
				
				offset += length;
				
				return code;
			}
			
			return text[offset++];
		}
		
		// Avance d'un caractère ou d'un tag dans le texte.
		public static Tag ParseTag(string text, ref int offset, out System.Collections.Hashtable parameters)
		{
			System.Diagnostics.Debug.Assert(text != null);
			parameters = null;
			
			if ( offset < text.Length && text[offset] == '<' )
			{
				int length = text.IndexOf(">", offset)-offset+1;

				if ( length <= 0 )
				{
					return Tag.SyntaxError;
				}
				else
				{
					string tag = text.Substring(offset, length);
					string tagLower = tag.ToLower();
					
					offset += length;
					
					switch ( tagLower )
					{
						case "<br/>":	return Tag.LineBreak;
						case "<b>":		return Tag.Bold;
						case "<i>":		return Tag.Italic;
						case "<u>":		return Tag.Underline;
						case "<m>":		return Tag.Mnemonic;
						
						case "</b>":	return Tag.BoldEnd;
						case "</i>":	return Tag.ItalicEnd;
						case "</u>":	return Tag.UnderlineEnd;
						case "</m>":	return Tag.MnemonicEnd;
						
						case "</a>":	return Tag.AnchorEnd;
						case "</font>":	return Tag.FontEnd;
					}
					
					int space = tag.IndexOf(" ");
					
					if ( space > 0 )
					{
						string pfx = tagLower.Substring(0, space);
						string end = tag.Remove(0, space).TrimStart(' ');
						Tag tagId = Tag.Unknown;
						
						string close = ">";
						
						switch ( pfx )
						{
							case "<a":		tagId = Tag.Anchor;		close = ">";	break;
							case "<img":	tagId = Tag.Image;		close = "/>";	break;
							case "<font":	tagId = Tag.Font;		close = ">";	break;
						}
						
						if ( !end.EndsWith(close) )
						{
							return Tag.SyntaxError;
						}
						
						// Enlève la fin du tag, comme ça on n'a réellement plus que les arguments.
						string arg = end.Remove(end.Length-close.Length, close.Length);
						parameters = new System.Collections.Hashtable();
						
						string argName;
						string argValue;
						int pos = 0;
						while ( pos < arg.Length )
						{
							int	i;

							while ( pos < arg.Length && arg[pos] == ' ' )  pos++;

							i = arg.IndexOf("=\"", pos);
							if ( i < 0 )  break;
							i -= pos;
							argName = arg.Substring(pos, i);
							pos += i+2;

							i = arg.IndexOf("\"", pos);
							if ( i < 0 )  break;
							i -= pos;
							argValue = arg.Substring(pos, i);
							pos += i+1;

							parameters[argName] = argValue;
						}
						return tagId;
					}
					
					return Tag.Unknown;
				}
			}
			
			if ( offset >= text.Length )
			{
				return Tag.Ending;
			}

			offset ++;
			return Tag.None;
		}
		
		// Trouve la séquence <m>x</m> dans le texte et retourne le premier caractère
		// de x comme code mnémonique (en majuscules).
		public static char ExtractMnemonic(string text)
		{
			System.Diagnostics.Debug.Assert(text != null);
			System.Collections.Hashtable parameters;

			int    offset = 0;
			while ( offset < text.Length )
			{
				TextLayout.Tag tag = TextLayout.ParseTag(text, ref offset, out parameters);
				if ( tag == TextLayout.Tag.Mnemonic )
				{
					return System.Char.ToUpper(text[offset]);
				}
			}
			
			return '\0';  // rien trouvé
		}
		
		public static string ConvertToTaggedText(string text)
		{
			return TextLayout.ConvertToTaggedText(text, false);
		}
		
		public static string ConvertToTaggedText(char c)
		{
			return TextLayout.ConvertToTaggedText(new string (c, 1), false);
		}
		
		// Convertit le texte simple en un texte compatible avec les tags. Supprime
		// toute occurrence de "<", "&" et ">" dans le texte.
		public static string ConvertToTaggedText(string text, bool autoMnemonic)
		{
			System.Diagnostics.Debug.Assert(text != null);
			if ( autoMnemonic )
			{
				// Cherche les occurrences de "&" dans le texte et gère comme suit:
				// - Remplace "&x" par "<m>x</m>" (le tag <m> spécifie un code mnémonique)
				// - Remplace "&&" par "&"

				System.Text.StringBuilder buffer = new System.Text.StringBuilder();
				
				for ( int pos=0 ; pos<text.Length ; pos++ )
				{
					if ( text[pos] == '&' && pos < text.Length-1 )
					{
						if ( text[pos+1] == '&' )
						{
							buffer.Append("&amp;");
						}
						else
						{
							buffer.Append("<m>");
							buffer.Append(text[pos+1]);
							buffer.Append("</m>");
						}
						pos ++;
						continue;
					}
					
					switch ( text[pos] )
					{
						case '<':		buffer.Append("&lt;");		break;
						case '>':		buffer.Append("&gt;");		break;
						case '\"':		buffer.Append("&quot;");	break;
						case (char)160:	buffer.Append("&nbsp;");	break;
						case '\n':		buffer.Append("<br/>");		break;
						default:		buffer.Append(text[pos]);	break;
					}
				}
				
				return buffer.ToString();
			}
			else
			{
				text = text.Replace("&", "&amp;");
				text = text.Replace("<", "&lt;");
				text = text.Replace(">", "&gt;");
				text = text.Replace("\"", "&quot;");
				text = text.Replace("\u00A0", "&nbsp;");  // (char)160
				text = text.Replace("\n", "<br/>");
				return text;
			}
		}
		
		public static string ConvertToSimpleText(string text)
		{
			return TextLayout.ConvertToSimpleText(text, "");
		}
		
		// Epure le texte en supprimant les tags <> et en remplaçant les
		// tags &gt; et &lt; (et autres) par leurs caractères équivalents.
		// En plus, les images sont remplacées par le texte 'imageReplacement'
		public static string ConvertToSimpleText(string text, string imageReplacement)
		{
			System.Diagnostics.Debug.Assert(text != null);
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();

			for ( int offset=0 ; offset<text.Length ; )
			{
				if ( text[offset] == '<' )
				{
					int length = text.IndexOf(">", offset)-offset+1;
					if ( length > 0 )
					{
						string tag = text.Substring(offset, length);
						string tagLower = tag.ToLower();
					
						offset += length;

						if ( tagLower.IndexOf("<img ") == 0 )
						{
							buffer.Append(imageReplacement);
						}
						if ( tagLower.IndexOf("<br") == 0 )
						{
							buffer.Append("\n");
						}
					}
				}
				else if ( text[offset] == '&' )
				{
					buffer.Append(AnalyseMetaChar(text, ref offset));
				}
				else
				{
					buffer.Append(text[offset++]);
				}
			}

			return buffer.ToString();
		}


		// Met à jour le layout si nécessaire.
		protected virtual void UpdateLayout()
		{
			if ( this.isDirty && this.layoutSize.Width > 0 )
			{
				this.JustifBlocks();
				this.JustifLines();
				this.isDirty = false;
			}
		}

		// Met à jour this.blocks en fonction du texte, de la fonte et des dimensions.
		protected void JustifBlocks()
		{
			System.Collections.Stack		fontStack;
			FontItem						fontItem;
			System.Text.StringBuilder		buffer;
			System.Collections.Hashtable	parameters;

			this.blocks.Clear();
			fontStack = new System.Collections.Stack();
			buffer = new System.Text.StringBuilder();
			bool stringExist = false;

			// Prépare la fonte initiale par défaut.
			fontItem = new FontItem();
			fontItem.fontName  = this.font.FaceName;
			fontItem.fontSize  = this.fontSize;
			fontItem.fontColor = Drawing.Color.FromBrightness(0);  // noir
			fontItem.bold      = 0;
			fontItem.italic    = 0;
			fontItem.underline = 0;
			fontItem.anchor    = 0;

			fontStack.Push(fontItem);  // push la fonte initiale (jamais de pop)

			// Si le texte n'existe pas, met quand même un bloc vide,
			// afin de voir apparaître le curseur (FindTextCursor).
			if ( this.textLength == 0 )
			{
				fontItem = (FontItem)fontStack.Peek();
				Drawing.Font blockFont = fontItem.RetFont();

				JustifBlock block = new JustifBlock();
				block.bol        = true;
				block.image      = false;
				block.text       = "";
				block.beginIndex = 0;
				block.endIndex   = 0;
				block.indexLine  = 0;
				block.font       = blockFont;
				block.fontSize   = fontItem.fontSize;
				block.fontColor  = fontItem.fontColor;
				block.underline  = false;
				block.anchor     = false;
				block.width      = 0;
				block.pos        = new Drawing.Point(0,0);
				block.visible    = false;
				this.blocks.Add(block);
				return;
			}

			double restWidth = this.layoutSize.Width;
			bool bol = true;

			int		beginOffset;
			int		endOffset = 0;
			int		index = 0;
			int		textIndex = 0;
			while ( endOffset <= this.textLength )
			{
				beginOffset = endOffset;
				TextLayout.Tag tag = TextLayout.ParseTag(this.text, ref endOffset, out parameters);

				if ( tag != TextLayout.Tag.None &&
					 (tag == TextLayout.Tag.LineBreak || stringExist) )
				{
					fontItem = (FontItem)fontStack.Peek();
					Drawing.Font blockFont = fontItem.RetFont();

					if ( buffer.Length == 0 )  // ligne vide ?
					{
						JustifBlock block = new JustifBlock();
						block.bol        = bol;
						block.image      = false;
						block.text       = "";
						block.beginIndex = index;
						block.endIndex   = index;
						block.indexLine  = 0;
						block.font       = blockFont;
						block.fontSize   = fontItem.fontSize;
						block.fontColor  = fontItem.fontColor;
						block.underline  = fontItem.underline > 0;
						block.anchor     = fontItem.anchor > 0;
						block.width      = 0;
						block.pos        = new Drawing.Point(0,0);
						block.visible    = false;
						this.blocks.Add(block);
					}
					else
					{
						//Drawing.TextBreakMode mode = bol ? Drawing.TextBreakMode.Split : Drawing.TextBreakMode.None;
						Drawing.TextBreakMode mode = this.breakMode;
						
						if (!bol)
						{
							mode &= ~Drawing.TextBreakMode.Split;
						}
						
						Drawing.TextBreak tb = new Drawing.TextBreak(blockFont, buffer.ToString(), fontItem.fontSize, mode);

						string breakText;
						double breakWidth;
						int    breakChars;
						
						while ( tb.GetNextBreak(restWidth, out breakText, out breakWidth, out breakChars) )
						{
							if ( breakWidth == 0 )  // pas la place ?
							{
								if ( restWidth == this.layoutSize.Width )  break;
								restWidth = this.layoutSize.Width;
								bol = true;
								continue;
							}

							JustifBlock block = new JustifBlock();
							block.bol        = bol;
							block.image      = false;
							block.text       = breakText;
							block.beginIndex = textIndex;
							block.endIndex   = textIndex+breakText.Length;
							block.indexLine  = 0;
							block.font       = blockFont;
							block.fontSize   = fontItem.fontSize;
							block.fontColor  = fontItem.fontColor;
							block.underline  = fontItem.underline > 0;
							block.anchor     = fontItem.anchor > 0;
							block.width      = breakWidth;
							block.pos        = new Drawing.Point(0,0);
							block.visible    = false;
							this.blocks.Add(block);

							textIndex += breakChars;

							if ( tb.MoreText )  // reste encore du texte ?
							{
								restWidth = this.layoutSize.Width;
								bol = true;
							}
							else
							{
								restWidth -= breakWidth;
								bol = false;
							}
						}
					}

					buffer = new System.Text.StringBuilder();
					stringExist = false;
				}

				if ( tag == TextLayout.Tag.Ending )  break;

				switch ( tag )
				{
					case TextLayout.Tag.Font:
						if ( parameters != null )
						{
							fontItem = ((FontItem)fontStack.Peek()).Copy();

							if ( parameters.ContainsKey("face") )
							{
								fontItem.fontName = (string)parameters["face"];
							}
							if ( parameters.ContainsKey("size") )
							{
								string s = parameters["size"] as string;
								fontItem.fontSize = System.Double.Parse(s);
							}
							if ( parameters.ContainsKey("color") )
							{
								string s = parameters["color"] as string;
								Drawing.Color color = Drawing.Color.FromName(s);
								if ( !color.IsEmpty )  fontItem.fontColor = color;
							}

							fontStack.Push(fontItem);
						}
						break;

					case TextLayout.Tag.FontEnd:
						if ( fontStack.Count > 1 )
						{
							fontStack.Pop();
						}
						break;

					case TextLayout.Tag.Bold:
						fontItem = (FontItem)fontStack.Peek();
						fontItem.bold ++;
						break;
					case TextLayout.Tag.BoldEnd:
						fontItem = (FontItem)fontStack.Peek();
						fontItem.bold --;
						break;

					case TextLayout.Tag.Italic:
						fontItem = (FontItem)fontStack.Peek();
						fontItem.italic ++;
						break;
					case TextLayout.Tag.ItalicEnd:
						fontItem = (FontItem)fontStack.Peek();
						fontItem.italic --;
						break;

					case TextLayout.Tag.Underline:
					case TextLayout.Tag.Mnemonic:
						fontItem = (FontItem)fontStack.Peek();
						fontItem.underline ++;
						break;
					case TextLayout.Tag.UnderlineEnd:
					case TextLayout.Tag.MnemonicEnd:
						fontItem = (FontItem)fontStack.Peek();
						fontItem.underline --;
						break;

					case TextLayout.Tag.Anchor:
						fontItem = (FontItem)fontStack.Peek();
						fontItem.anchor ++;
						fontItem.underline ++;
						break;
					case TextLayout.Tag.AnchorEnd:
						fontItem = (FontItem)fontStack.Peek();
						fontItem.anchor --;
						fontItem.underline --;
						break;

					case TextLayout.Tag.LineBreak:
						restWidth = this.layoutSize.Width;
						bol = true;
						stringExist = true;
						index ++;
						break;

					case TextLayout.Tag.Image:
						if ( parameters != null && this.imageProvider != null )
						{
							if ( parameters.ContainsKey("src") )
							{
								string imageName = parameters["src"] as string;
								Drawing.Image image = this.imageProvider.GetImage(imageName);
								
								if ( image == null )
								{
									throw new System.FormatException(string.Format("<img> tag references unknown image '{0}'.", imageName));
								}
								
								double dx = image.Width;
								double dy = image.Height;

								if ( dx > restWidth )
								{
									restWidth = this.layoutSize.Width;
									bol = true;
								}

								fontItem = (FontItem)fontStack.Peek();
								Drawing.Font blockFont = fontItem.RetFont();

								double fontAscender  = blockFont.Ascender;
								double fontDescender = blockFont.Descender;
								double fontHeight    = fontAscender-fontDescender;

								JustifBlock block = new JustifBlock();
								block.bol            = bol;
								block.image          = true;
								block.text           = imageName;
								block.beginIndex     = index;
								block.endIndex       = endOffset-beginOffset;
								block.indexLine      = 0;
								block.font           = blockFont;
								block.fontSize       = fontItem.fontSize;
								block.fontColor      = fontItem.fontColor;
								block.underline      = fontItem.underline > 0;
								block.anchor         = fontItem.anchor > 0;
								block.width          = dx;
								
								if (image.IsOriginDefined)
								{
									block.imageAscender  = image.Height - image.Origin.Y;
									block.imageDescender = - image.Origin.Y;
								}
								else
								{
									block.imageAscender  = dy*fontAscender/fontHeight;
									block.imageDescender = dy*fontDescender/fontHeight;
								}
								
								block.pos            = new Drawing.Point(0,0);
								block.visible        = false;
								this.blocks.Add(block);

								restWidth -= dx;
								bol = false;
							}
						}
						break;

					case TextLayout.Tag.None:
						if ( buffer.Length == 0 )  textIndex = index;
						endOffset = beginOffset;
						char c = AnalyseMetaChar(this.text, ref endOffset);
						buffer.Append(c);
						stringExist = true;
						index ++;
						break;
				}
			}
		}

		// Met à jour this.lines en fonction de this.blocks.
		// Détermine la position des blocs en fonction de l'alignement.
		// Détermine également quels sont les blocs et les lignes visibles.
		protected void JustifLines()
		{
			this.lines.Clear();

			this.totalLine = 0;
			this.visibleLine = 0;

			Drawing.Point pos = new Drawing.Point(0,0);
			pos.Y += this.layoutSize.Height;  // coin sup/gauche

			JustifBlock	block;
			JustifLine	line;

			double totalHeight = 0;
			double overflow = 0;
			int i = 0;
			while ( i < this.blocks.Count )
			{
				// Avance tous les blocs de la ligne.
				double width = 0;
				double height = 0;
				double ascender = 0;
				double descender = 0;
				int j = i;
				while ( true )
				{
					block = (JustifBlock)this.blocks[j];
					block.indexLine = totalLine;
					width += block.width;
					if ( block.image )
					{
						height    = System.Math.Max(height,    block.imageAscender-block.imageDescender);
						ascender  = System.Math.Max(ascender,  block.imageAscender);
						descender = System.Math.Min(descender, block.imageDescender);
					}
					else
					{
						height    = System.Math.Max(height,    block.font.LineHeight*block.fontSize);
						ascender  = System.Math.Max(ascender,  block.font.Ascender  *block.fontSize);
						descender = System.Math.Min(descender, block.font.Descender *block.fontSize);
					}

					j ++;
					if ( j >= this.blocks.Count )  break;
					block = (JustifBlock)this.blocks[j];
					if ( block.bol )  break;  // break si début nouvelle ligne
				}

				bool visible;
				this.totalLine ++;
//?				if ( pos.Y-ascender+descender >= 0 )
				if ( pos.Y-ascender+descender >= 0 || this.totalLine == 1 )
				{
					visible = true;
					this.visibleLine ++;
					totalHeight += height;
					overflow = height-(ascender-descender);
				}
				else
				{
					visible = false;
				}

				switch ( this.alignment )
				{
					case Drawing.ContentAlignment.TopLeft:
					case Drawing.ContentAlignment.MiddleLeft:
					case Drawing.ContentAlignment.BottomLeft:
						pos.X = 0;
						break;
				
					case Drawing.ContentAlignment.TopCenter:
					case Drawing.ContentAlignment.MiddleCenter:
					case Drawing.ContentAlignment.BottomCenter:
						pos.X = (this.layoutSize.Width-width)/2;
						break;
				
					case Drawing.ContentAlignment.TopRight:
					case Drawing.ContentAlignment.MiddleRight:
					case Drawing.ContentAlignment.BottomRight:
						pos.X = this.layoutSize.Width-width;
						break;
				}
				pos.Y -= ascender;  // sur la ligne de base

				line = new JustifLine();
				line.firstBlock = i;
				line.lastBlock  = j-1;
				line.rank       = this.totalLine-1;
				line.pos        = pos;
				line.width      = width;
				line.height     = height;
				line.ascender   = ascender;
				line.descender  = descender;
				line.visible    = visible;
				this.lines.Add(line);

				for ( int k=i ; k<j ; k++ )
				{
					block = (JustifBlock)this.blocks[k];
					block.pos = pos;
					block.visible = visible;
					pos.X += block.width;
				}

				pos.Y += ascender;
				pos.Y -= height;  // position haut de la ligne suivante

				i = j;  // index début ligne suivante
			}

			// Effectue l'alignement vertical.
			totalHeight -= overflow;
			double offset = 0;
			switch ( this.alignment )
			{
				case Drawing.ContentAlignment.TopLeft:
				case Drawing.ContentAlignment.TopCenter:
				case Drawing.ContentAlignment.TopRight:
					offset = 0;
					break;

				case Drawing.ContentAlignment.MiddleLeft:
				case Drawing.ContentAlignment.MiddleCenter:
				case Drawing.ContentAlignment.MiddleRight:
					offset = (this.layoutSize.Height-totalHeight)/2;
					break;
				
				case Drawing.ContentAlignment.BottomLeft:
				case Drawing.ContentAlignment.BottomCenter:
				case Drawing.ContentAlignment.BottomRight:
					offset = this.layoutSize.Height-totalHeight;
					break;
			}
			if ( offset != 0 )  // alignement Middle* ou Bottom* ?
			{
				for ( i=0 ; i<this.blocks.Count ; i++ )
				{
					block = (JustifBlock)this.blocks[i];
					block.pos.Y -= offset;  // descend le bloc
				}
				for ( i=0 ; i<this.lines.Count ; i++ )
				{
					line = (JustifLine)this.lines[i];
					line.pos.Y -= offset;  // descend la ligne
				}
			}
		}

		// Affiche le contenu du tableau this.blocks, pour le debug.
		public void JustifConsoleOut()
		{
			System.Console.Out.WriteLine("Total blocks = " + this.blocks.Count);
			foreach ( JustifBlock block in this.blocks )
			{
				string bol = block.bol ? "BOL: " : "";
				System.Console.Out.WriteLine(bol + block.font.FullName + " " + block.fontSize + "     " + "pos=" + block.pos.X + ";" + block.pos.Y + " width=" + block.width + "     " + "\"" + block.text + "\"");
			}
		}


		// Vérifie la syntaxe d'un texte.
		public static bool CheckSyntax(string text, out int offsetError)
		{
			System.Collections.Hashtable parameters;
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			int    beginOffset;
			for ( int endOffset=0 ; endOffset<text.Length ; )
			{
				beginOffset = endOffset;

				if ( text[endOffset] == '&' )
				{
					int length = text.IndexOf(";", endOffset)-endOffset+1;
					if ( length < 3 )
					{
						offsetError = beginOffset;
						return false;
					}
				
					string meta = text.Substring(endOffset, length).ToLower();
					switch ( meta )
					{
						case "&lt;":	break;
						case "&gt;":	break;
						case "&amp;":	break;
						case "&quot;":	break;
						case "&nbsp;":	break;
					
						default:
							offsetError = beginOffset;
							return false;
					}
				
					endOffset += length;
					continue;
				}

				TextLayout.Tag tag = TextLayout.ParseTag(text, ref endOffset, out parameters);
				if ( tag == TextLayout.Tag.None )  continue;

				if ( tag == TextLayout.Tag.SyntaxError ||
					 tag == TextLayout.Tag.Unknown     )
				{
					offsetError = beginOffset;
					return false;
				}

				string sTag = text.Substring(beginOffset, endOffset-beginOffset);

				switch ( tag )
				{
					case TextLayout.Tag.Bold:
					case TextLayout.Tag.Italic:
					case TextLayout.Tag.Underline:
					case TextLayout.Tag.Mnemonic:
					case TextLayout.Tag.Font:
					case TextLayout.Tag.Anchor:
						list.Add(sTag);
						break;

					case TextLayout.Tag.BoldEnd:
					case TextLayout.Tag.ItalicEnd:
					case TextLayout.Tag.UnderlineEnd:
					case TextLayout.Tag.MnemonicEnd:
					case TextLayout.Tag.FontEnd:
					case TextLayout.Tag.AnchorEnd:
						if ( !TextLayout.DeleteTagsList(sTag, list) )
						{
							offsetError = beginOffset;
							return false;
						}
						break;
				}
			}

			if ( list.Count != 0 )
			{
				offsetError = text.Length;
				return false;
			}

			offsetError = -1;
			return true;
		}
		

		// Tous les tags possibles.
		public enum Tag
		{
			None,						//	pas un tag
			Unknown,					//	tag pas reconnu
			SyntaxError,				//	syntaxe du tag pas correcte
			Ending,						//	fin du texte
			
			LineBreak,					//	<br/>
			Bold, BoldEnd,				//	<b>...</b>
			Italic, ItalicEnd,			//	<i>...</i>
			Underline, UnderlineEnd,	//	<u>...</u>
			Mnemonic, MnemonicEnd,		//	<m>...</m>  --> comme <u>...</u>
			Font, FontEnd,				//	<font ...>...</font>
			Anchor, AnchorEnd,			//	<a href="x">...</a>
			Image,						//	<img src="x"/>
		}
		
		
		// Fonte servant à refléter les commandes HTML rencontrées.
		// Un stack de FontItem est créé.
		protected class FontItem
		{
			public FontItem Copy()
			{
				return this.MemberwiseClone() as FontItem;
			}

			public Drawing.Font RetFont()
			{
				Drawing.Font	font;
				string			fontStyle;

				if ( this.bold > 0 && this.italic == 0 )
				{
					fontStyle = "Bold";
				}
				else if ( this.bold == 0 && this.italic > 0 )
				{
					fontStyle = "Italic";
				}
				else if ( this.bold > 0 && this.italic > 0 )
				{
					fontStyle = "Bold Italic";
				}
				else
				{
					fontStyle = "Regular";
				}

				font = Drawing.Font.GetFont(this.fontName, fontStyle);
				if ( font == null )
				{
					font = Drawing.Font.GetFont(this.fontName, "Regular");
				}
				return font;
			}

			public string			fontName;
			public double			fontSize;
			public Drawing.Color	fontColor;
			public int				bold;		// gras si > 0
			public int				italic;		// italique si > 0
			public int				underline;	// souligné si > 0
			public int				anchor;		// lien si > 0
		}

		// Descripteur d'un bloc de texte. Tous les caractères du bloc ont
		// la même fonte, même taille et même couleur.
		protected class JustifBlock
		{
			public bool				bol;		// begin of line
			public bool				image;		// image bitmap
			public string			text;
			public int				beginIndex;
			public int				endIndex;
			public int				indexLine;	// index dans this.lines
			public Drawing.Font		font;
			public double			fontSize;
			public Drawing.Color	fontColor;
			public bool				underline;
			public bool				anchor;
			public double			width;		// largeur du bloc
			public double			imageAscender;
			public double			imageDescender;
			public Drawing.Point	pos;		// sur la ligne de base
			public bool				visible;
		}

		// Descripteur d'une ligne de texte. Une ligne est composée
		// d'un ou plusieurs blocs.
		protected class JustifLine
		{
			public int				firstBlock;	// index du premier bloc
			public int				lastBlock;	// index du dernier bloc
			public int				rank;		// rang de la ligne (0..n)
			public Drawing.Point	pos;		// position sur la ligne de base
			public double			width;		// largeur occupée par la ligne
			public double			height;		// interligne
			public double			ascender;	// hauteur en-dessus de la ligne de base (+)
			public double			descender;	// hauteur en-dessous de la ligne de base (-)
			public bool				visible;
		}


		// Variables membres de TextLayout.
		protected bool							isDirty;
		protected string						text;
		protected int							textLength;
		protected Drawing.Font					font = Drawing.Font.DefaultFont;
		protected double						fontSize = Drawing.Font.DefaultFontSize;
		protected Drawing.Size					layoutSize;
		protected Drawing.TextBreakMode			breakMode = Drawing.TextBreakMode.Split;
		protected int							totalLine;
		protected int							visibleLine;
		protected Drawing.IImageProvider		imageProvider = Support.ImageProvider.Default;
		protected Drawing.ContentAlignment		alignment = Drawing.ContentAlignment.TopLeft;
		protected System.Collections.ArrayList	blocks = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	lines = new System.Collections.ArrayList();

		protected static Drawing.Color			anchorColor = new Drawing.Color(0,0,1);
		
		public const double						Infinite = 1000000;
	}
}
