/*
 * A modifier:
 * 
 * - Remplacer les appels à Check() par des appels à TextLayout.Check() pour
 *   bien montrer que c'est un appel à une méthode statique.
 * 
 * - La propriété Text {set} devrait mettre isDirty à true. Tout le boulôt
 *   ne devrait être fait que si value != this.text (cf. Font {set}).
 * 
 * - Dans Text toujours, utiliser un if (...) throw new ... à la place de
 *   l'assertion actuelle. Par exemple :
 * 
 *		throw new System.FormatException("Syntax error at char " + offsetError.ToString ());
 * 
 * - Propriété AnchorColor, tu as oublié les "this." devant anchorColor.
 * 
 * - Remplace les codes d'initialisation 'totalRect = Drawing.Rectangle.Empty'
 *   par 'totalRect = new Drawing.Rectangle ()'.
 * 
 * - Remplace 'totalRect = Drawing.Rectangle.Union(totalRect, blockRect)' par
 *   'totalRect.MergeWith(blockRect)', ce qui évite la création d'un rectangle
 *   temporaire et une copie inutile.
 * 
 * - La boucle :
 * 
 *		for ( int i=0 ; i<this.blocks.Count ; i++ )
 *		{
 * 			JustifBlock block = (JustifBlock)this.blocks[i];
 * 
 *   peut être remplacée aventageusement par :
 * 
 *		foreach (JustifBlock block in this.blocks)
 *		{
 * 
 *   ce qui est plus lisible et plus élégant. Notamment dans RectangleBounds
 *   et dans Paint. Peut-être ailleurs aussi. Aussi DetectOffset, mais avec
 *   JustifLine.
 * 
 * - L'appel à RectangleBounds manque de this.
 * 
 * - Dans Paint, il manque plein de this.
 *
 * - Dans Paint, tu fais à de nombreuses reprises 'e.Graphics.xxx'. Il vaut
 *   mieux faire :
 * 
 *		Drawing.Graphics graphics = e.Graphics;
 *		...
 *		graphics.xxx
 * 
 *   Il ne faut pas oublier que 'e.Graphics' est en fait un appel de méthode
 *   qui prend du temps...
 * 
 * - Dans Paint toujours, la position du souligné n'est pas proportionnelle
 *   à la hauteur en dessous de la ligne de base; on en avait discuté... Tu
 *   peux corriger cela ?
 * 
 *   De plus, je suggère que tu fasses un 'graphics.Align (ref x, ref y)'
 *   pour aligner le trait sur une frontière ronde, puis que tu ajoutes
 *   0.5 à y.
 * 
 * - Commentaire '// faire mieux !' devrait être '// TODO: faire mieux !',
 *   comme ça tu peux voir dans la Task List ce qui reste à faire (un clic
 *   avec le bouton de droite dans 'Task List' / Show Tasks / All...
 * 
 * - FindOffsetFromIndex pourrait être épuré en regroupant toutes les
 *   clauses communes en fin de boucle, par exemple.
 * 
 * - ConvertToSimpleText devrait reprendre AnalyseMetaChar pour le code
 *   commun. C'est ridicule d'avoir ce type de code à double.
 * 
 * - Le code de ConvertToTaggedText aurait pu être un peu plus élégant dans
 *   le case autoMnemonic = true (les tests pour '<', '>' et '\n' du cas
 *   false vont très bien). Et j'avais oublié les conversions '"' -> '&quot;'
 *   et 160 -> &nbsp, qu'il faut rajouter aussi...
 * 
 * - Les classes internes JustifBlock, JustifLine et FontItem peuvent toutes
 *   apparaître en fin de classe. En les regroupant on les retrouve plus
 *   facilement. En général, j'essaie de les mettre entre les méthodes et
 *   les variables d'instance.
 * 
 * - Les méthodes JustifLines et JustifBlocks n'ont pas d'intérêt pour les
 *   utilisateurs externes à la classe : il faut donc les déclarer comme
 *   'protected'.
 * 
 *   Ce sont des sacrées méthodes !!!
 * 
 * - Check doit encore être implémenté. Je suggère de la nommer CheckSyntax
 *   pour que ce soit plus clair.
 * 
 * - Détail: j'ai renommé 'test.png' en '..\..\icon.png' pour me permettre
 *   d'y accéder à la fois depuis la version debug et depuis la version
 *   release. Et comme il y avait déjà ma tomate (test.png)...
 * 
 * - J'ai modifié TextLayoutTest.NewTextLayout pour utiliser un vrai nom
 *   d'image à la place de "x", car deux méthodes de test échouaient parce
 *   qu'elles ne trouvaient pas le fichier image "x".
 */

namespace Epsitec.Common.Widgets
{
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
		

		// Texte associé, contenant des commandes xml.
		public string Text
		{
			get
			{
				return this.text;
			}

			set
			{
				int offsetError;
				System.Diagnostics.Debug.Assert(Check(value, out offsetError));
				this.text = value;
			}
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
				if ( this.font != value )
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
				return anchorColor;
			}

			set
			{
				anchorColor = value;
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
		// Si le texte est aligné sur le bord droit, rectangle.X n'est pas
		// forcément égal à 0.
		protected Drawing.Rectangle RectangleBounds(bool all)
		{
			this.UpdateLayout();

			Drawing.Rectangle totalRect = Drawing.Rectangle.Empty;
			for ( int i=0 ; i<this.blocks.Count ; i++ )
			{
				JustifBlock block = (JustifBlock)this.blocks[i];
				if ( !all && !block.visible )  continue;

				Drawing.Rectangle blockRect = block.font.GetTextBounds(block.text);
				blockRect.Scale(block.fontSize);
				blockRect.Offset(block.pos.X, block.pos.Y);
				totalRect = Drawing.Rectangle.Union(totalRect, blockRect);
			}
			return totalRect;
		}

		// Retourne le rectangle englobant du layout courant; ce
		// rectangle comprend toutes les lignes, même celles qui débordent.
		public Drawing.Rectangle TotalRectangle
		{
			get
			{
				return RectangleBounds(true);
			}
		}
		
		// Retourne le rectangle englobant du layout courant; ce
		// rectangle comprend uniquement les lignes visibles.
		public Drawing.Rectangle VisibleRectangle
		{
			get
			{
				return RectangleBounds(false);
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
		public void Paint(Drawing.Point pos, PaintEventArgs e, Drawing.Color uniqueColor)
		{
			this.UpdateLayout();

			for ( int i=0 ; i<this.blocks.Count ; i++ )
			{
				JustifBlock block = (JustifBlock)this.blocks[i];
				if ( !block.visible )  continue;

				if ( block.image )
				{
					Drawing.Image image = this.imageProvider.GetImage(block.text);
					int dx = image.Width;
					int dy = image.Height;
					double ix = pos.X+block.pos.X;
					double iy = pos.Y+block.pos.Y+block.imageDescender;
					e.Graphics.Align (ref ix, ref iy);
					e.Graphics.AddFilledRectangle(ix, iy, dx, dy);
					Drawing.Transform t = new Drawing.Transform();
					t.Translate(ix, iy);
					e.Graphics.ImageRenderer.Transform = t;
					e.Graphics.ImageRenderer.Bitmap = image.BitmapImage;
					e.Graphics.RenderImage();
					continue;
				}

				Drawing.Color color;
				if ( uniqueColor.IsEmpty )
				{
					if ( block.anchor )  color = anchorColor;
					else                 color = block.fontColor;
				}
				else
				{
					color = uniqueColor;
				}

				double x = pos.X+block.pos.X;
				double y = pos.Y+block.pos.Y;
				e.Graphics.AddText(x, y, block.text, block.font, block.fontSize);
				e.Graphics.RenderSolid(color);

				if ( block.underline )
				{
					double p1x = pos.X+block.pos.X;
					double p2x = p1x+block.width;
					double py = pos.Y+block.pos.Y-1.5;  // un poil en dessous !
					e.Graphics.LineWidth = 1;
					e.Graphics.AddLine(p1x, py, p2x, py);
					e.Graphics.RenderSolid(color);
				}
			}
		}
		
		// Trouve l'offset dans le texte interne qui correspond à la
		// position indiquée. Retourne -1 en cas d'échec.
		public int DetectOffset(Drawing.Point pos)
		{
			if ( pos.X < 0                      ||
				 pos.X > this.layoutSize.Width  ||
				 pos.Y < 0                      ||
				 pos.Y > this.layoutSize.Height )  return -1;

			this.UpdateLayout();

			for ( int i=0 ; i<this.lines.Count ; i++ )
			{
				JustifLine line = (JustifLine)this.lines[i];
				if ( !line.visible )  continue;

				if ( pos.Y <= line.pos.Y+line.ascender  &&
					 pos.Y >= line.pos.Y-line.descender )
				{
					for ( int j=line.firstBlock ; j<=line.lastBlock ; j++ )
					{
						if ( pos.X >= line.pos.X            &&
							 pos.X <= line.pos.X+line.width )
						{
							JustifBlock block = (JustifBlock)this.blocks[j];
							double[] charsWidth = block.font.GetTextCharEndX(block.text);
							for ( int k=0 ; k<charsWidth.Length ; k++ )
							{
								if ( pos.X-line.pos.X <= charsWidth[k]*block.fontSize )  // faire mieux !
								{
									return block.beginOffset+k;
								}
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
			int offset = this.DetectOffset(pos);
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
		

		// Retourne la position horizontale correspondant à un offset dans un bloc.
		protected double OffsetToPosX(JustifBlock block, int offset)
		{
			if ( offset <= block.beginOffset )  return block.pos.X;
			if ( offset >  block.endOffset   )  return block.pos.X+block.width;
			double[] charsWidth = block.font.GetTextCharEndX(block.text);
			return block.pos.X+charsWidth[offset-block.beginOffset-1]*block.fontSize;
		}

		// Retourne un tableau avec les rectangles englobant le texte
		// spécifié par son début et sa fin. Il y a un rectangle par ligne.
		public Drawing.Rectangle[] FindTextRange(int offsetBegin, int offsetEnd)
		{
			if ( offsetBegin >= offsetEnd )  return new Drawing.Rectangle[0];

			this.UpdateLayout();

			System.Collections.ArrayList list = new System.Collections.ArrayList();
			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Top    = -100000;
			rect.Bottom =  100000;
			rect.Left   =  100000;
			rect.Right  = -100000;
			for ( int i=0 ; i<this.blocks.Count ; i++ )
			{
				JustifBlock block = (JustifBlock)this.blocks[i];
				JustifLine line = (JustifLine)this.lines[block.indexLine];
				if ( !block.visible )  continue;

				int localBegin = System.Math.Max(offsetBegin, block.beginOffset);
				int localEnd   = System.Math.Min(offsetEnd,   block.endOffset  );

				if ( localBegin >= localEnd )  continue;

				double top    = line.pos.Y+line.ascender;
				double bottom = line.pos.Y+line.descender;

				if ( rect.Top    != top    ||
					 rect.Bottom != bottom )  // rectangle dans autre ligne ?
				{
					if ( rect.Top > -100000 && rect.Left < 100000 )
					{
						list.Add(rect);
					}

					rect.Top    = top;
					rect.Bottom = bottom;
					rect.Left   =  100000;
					rect.Right  = -100000;
				}

				rect.Left  = System.Math.Min(rect.Left,  OffsetToPosX(block, localBegin));
				rect.Right = System.Math.Max(rect.Right, OffsetToPosX(block, localEnd  ));
			}
			
			if ( rect.Top > -100000 && rect.Left < 100000 )
			{
				list.Add(rect);
			}

			Drawing.Rectangle[] rects = new Drawing.Rectangle[list.Count];
			list.CopyTo(rects);
			return rects;
		}
		
		
		// Retourne l'offset dans le texte interne, correspondant à l'index
		// spécifié pour le texte sans tags. On saute tous les tags qui précèdent
		// le caractère indiqué (textIndex=0 => premier caractère non tag dans
		// le texte).
		public int FindOffsetFromIndex(int textIndex)
		{
			int    index = 0;
			int    beginOffset;

			for ( int endOffset=0 ; endOffset<this.text.Length ; )
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
						if ( index == textIndex )  return beginOffset;
						index ++;
					}
				}
				else if ( this.text[endOffset] == '&' )
				{
					int length = this.text.IndexOf(";", endOffset)-endOffset+1;
					if ( length < 0 )  return -1;
					endOffset += length;
					if ( index == textIndex )  return beginOffset;
					index ++;
				}
				else
				{
					endOffset ++;
					if ( index == textIndex )  return beginOffset;
					index ++;
				}
			}
			
			return -1;
		}
		
		// Retourne l'index dans le texte propre, correspondant à l'offset
		// spécifié dans le texte avec tags.
		public int FindIndexFromOffset(int taggedTextOffset)
		{
			int    index = 0;
			int    beginOffset;

			for ( int endOffset=0 ; endOffset<this.text.Length ; )
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
		protected void DeleteTagsList(string endTag, System.Collections.ArrayList list)
		{
			System.Diagnostics.Debug.Assert(endTag.StartsWith("</"));
			System.Diagnostics.Debug.Assert(endTag.EndsWith(">"));

			endTag = endTag.Substring(2, endTag.Length-3);  // </b> -> b

			for ( int i=list.Count-1 ; i>=0 ; i-- )
			{
				string s = (string)list[i];
				if ( s.IndexOf(endTag) == 1 )
				{
					list.RemoveAt(i);
					return;
				}
			}
		}

		// Parcourt le texte et accumule les informations sur les tags <>
		// reconnus.
		public bool AnalyseTagsAtOffset(int offset, out string[] tags)
		{
			if ( offset < 0 || offset > this.Text.Length )
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
						this.DeleteTagsList(sTag, list);
						break;
				}
			}

			tags = new string[list.Count];
			list.CopyTo(tags);
			return true;
		}
		
		
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
					
						//	TODO: gère les autres codes, entre autres les codes numériques...
					
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
			parameters = null;
			
			if ( offset < text.Length && text[offset] == '<' )
			{
				int length = text.IndexOf(">", offset)-offset+1;
				
				if ( length > 0 )
				{
					string tag = text.Substring(offset, length);
					string tagLower = tag.ToLower();
					
					offset += length;
					
					switch ( tagLower )
					{
						case "<br/>":	return Tag.LineBreak;
						case "<b>":		return Tag.Bold;
						case "</b>":	return Tag.BoldEnd;
						case "<i>":		return Tag.Italic;
						case "</i>":	return Tag.ItalicEnd;
						case "<u>":		return Tag.Underline;
						case "</u>":	return Tag.UnderlineEnd;
						case "<m>":		return Tag.Mnemonic;
						case "<m/>":	return Tag.MnemonicEnd;
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
		
		// Convertit le texte simple en un texte compatible avec les tags. Supprime
		// toute occurrence de "<", "&" et ">" dans le texte.
		public static string ConvertToTaggedText(string text, bool autoMnemonic)
		{
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
					}
					else if ( text[pos] == '<' )
					{
						buffer.Append("&lt;");
					}
					else if ( text[pos] == '>' )
					{
						buffer.Append("&gt;");
					}
					else if ( text[pos] == '\n' )
					{
						buffer.Append("<br/>");
					}
					else
					{
						buffer.Append(text[pos]);
					}
				}
				return buffer.ToString();
			}
			else
			{
				text = text.Replace("&",  "&amp;");
				text = text.Replace("<",  "&lt;");
				text = text.Replace(">",  "&gt;");
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
					int length = text.IndexOf(";", offset)-offset+1;
					if ( length > 0 )
					{
						string tag = text.Substring(offset, length);
						string tagLower = tag.ToLower();
					
						offset += length;
					
						switch ( tagLower )
						{
							case "&lt;":	buffer.Append('<');			break;
							case "&gt;":	buffer.Append('>');			break;
							case "&amp;":	buffer.Append('&');			break;
							case "&quot;":	buffer.Append('"');			break;
							case "&nbsp;":	buffer.Append((char)160);	break;
						}
					}
				}
				else
				{
					buffer.Append(text[offset++]);
				}
			}

			return buffer.ToString();
		}


		// Fonte servant à refléter les commandes xml rencontrées.
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
			public int				beginOffset;
			public int				endOffset;
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

		// Met à jour this.blocks en fonction du texte, de la fonte et des dimensions.
		public void JustifBlocks()
		{
			System.Collections.Stack		fontStack;
			FontItem						fontItem;
			System.Text.StringBuilder		buffer;
			System.Collections.Hashtable	parameters;

			this.blocks.Clear();
			fontStack = new System.Collections.Stack();
			buffer = new System.Text.StringBuilder();

			// Prépare la fonte initiale par défaut.
			fontItem = new FontItem();
			fontItem.fontName  = this.font.FaceName;
			fontItem.fontSize  = this.fontSize;
//?			fontItem.fontColor = Drawing.Color.FromRGB(0,0,0);  // noir
			fontItem.fontColor = Drawing.Color.FromBrightness(0);  // noir
			fontItem.bold      = 0;
			fontItem.italic    = 0;
			fontItem.underline = 0;
			fontItem.anchor    = 0;

			fontStack.Push(fontItem);  // push la fonte initiale (jamais de pop)

			double restWidth = this.layoutSize.Width;
			bool bol = true;

			int		beginOffset;
			int		textOffset = 0;
			int		endOffset = 0;
			while ( endOffset <= this.text.Length )
			{
				beginOffset = endOffset;
				TextLayout.Tag tag = TextLayout.ParseTag(this.text, ref endOffset, out parameters);

				if ( tag != TextLayout.Tag.None && buffer.Length > 0 )
				{
					fontItem = (FontItem)fontStack.Peek();
					Drawing.Font blockFont = fontItem.RetFont();

					Drawing.TextBreakMode mode = bol ? Drawing.TextBreakMode.Split : Drawing.TextBreakMode.None;
					Drawing.TextBreak tb = new Drawing.TextBreak(blockFont, buffer.ToString(), fontItem.fontSize, mode);

					string breakText;
					double breakWidth;
					while ( tb.GetNextBreak(restWidth, out breakText, out breakWidth) )
					{
						if ( breakWidth == 0 )  // pas la place ?
						{
							restWidth = this.layoutSize.Width;
							bol = true;
							continue;
						}

						JustifBlock block = new JustifBlock();
						block.bol         = bol;
						block.image       = false;
						block.text        = breakText;
						block.beginOffset = textOffset;
						block.endOffset   = textOffset+breakText.Length;
						block.indexLine   = 0;
						block.font        = blockFont;
						block.fontSize    = fontItem.fontSize;
						block.fontColor   = fontItem.fontColor;
						block.underline   = fontItem.underline > 0;
						block.anchor      = fontItem.anchor > 0;
						block.width       = breakWidth;
						block.pos         = new Drawing.Point(0,0);
						block.visible     = false;
						this.blocks.Add(block);

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

					buffer = new System.Text.StringBuilder();
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
						break;

					case TextLayout.Tag.Image:
						if ( parameters != null && this.imageProvider != null )
						{
							if ( parameters.ContainsKey("src") )
							{
								string imageName = "file:" + parameters["src"] as string;
								Drawing.Image image = this.imageProvider.GetImage(imageName);
								int dx = image.Width;
								int dy = image.Height;

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
								block.beginOffset    = beginOffset;
								block.endOffset      = endOffset;
								block.indexLine      = 0;
								block.font           = blockFont;
								block.fontSize       = fontItem.fontSize;
								block.fontColor      = fontItem.fontColor;
								block.underline      = fontItem.underline > 0;
								block.anchor         = fontItem.anchor > 0;
								block.width          = dx;
								block.imageAscender  = dy*fontAscender/fontHeight;
								block.imageDescender = dy*fontDescender/fontHeight;
								block.pos            = new Drawing.Point(0,0);
								block.visible        = false;
								this.blocks.Add(block);

								restWidth -= dx;
								bol = false;
							}
						}
						break;

					case TextLayout.Tag.None:
						if ( buffer.Length == 0 )  textOffset = beginOffset;
						buffer.Append(this.text[beginOffset]);
						break;
				}
			}
		}

		// Descripteur d'une ligne de texte. Une ligne est composée
		// d'un ou plusieurs blocs.
		protected class JustifLine
		{
			public int				firstBlock;	// index du premier bloc
			public int				lastBlock;	// index du dernier bloc
			public Drawing.Point	pos;		// position sur la ligne de base
			public double			width;		// largeur occupée par la ligne
			public double			height;		// interligne
			public double			ascender;	// hauteur en-dessus de la ligne de base (+)
			public double			descender;	// hauteur en-dessous de la ligne de base (-)
			public bool				visible;
		}

		// Met à jour this.lines en fonction de this.blocks.
		// Détermine la position des blocs en fonction de l'alignement.
		// Détermine également quels sont les blocs et les lignes visibles.
		public void JustifLines()
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
				if ( pos.Y-ascender+descender >= 0 )
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
			for ( int i=0 ; i<this.blocks.Count ; i++ )
			{
				JustifBlock block = (JustifBlock)blocks[i];
				string bol = block.bol ? "BOL: " : "";
				System.Console.Out.WriteLine(bol + block.font.FullName + " " + block.fontSize + "     " + "pos=" + block.pos.X + ";" + block.pos.Y + " width=" + block.width + "     " + "\"" + block.text + "\"");
			}
		}

		// Met à jour le layout si nécessaire.
		protected virtual void UpdateLayout()
		{
			if ( this.isDirty )
			{
				JustifBlocks();
				JustifLines();
				this.isDirty = false;
			}
		}


		// Vérifie la syntaxe d'un texte.
		public static bool Check(string text, out int offsetError)
		{
			// TODO:
			offsetError = -1;  // ok
			return true;
		}
		
		
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
			Mnemonic, MnemonicEnd,		//	<m>...</m>				--> comme <u>...</u>
			Font, FontEnd,				//	<font ...>...</font>
			Anchor, AnchorEnd,			//	<a href="x">...</a>
			Image,						//	<img src="x"/>
		}
		
		
		protected bool							isDirty;
		protected string						text;
		protected Drawing.Font					font;
		protected double						fontSize;
		protected Drawing.Size					layoutSize;
		protected int							totalLine;
		protected int							visibleLine;
		protected Drawing.IImageProvider		imageProvider = Drawing.ImageProvider.Default;
		protected Drawing.ContentAlignment		alignment = Drawing.ContentAlignment.TopLeft;
		protected System.Collections.ArrayList	blocks = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	lines = new System.Collections.ArrayList();

		protected static Drawing.Color			anchorColor = new Drawing.Color(0,0,1);
	}
}
