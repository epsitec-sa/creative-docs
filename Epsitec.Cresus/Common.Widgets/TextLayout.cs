namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextLayout permet de stocker et d'afficher des contenus
	/// riches (un sous-ensemble tr�s restreint de HTML).
	/// </summary>
	public class TextLayout
	{
		public TextLayout()
		{
			//	TODO: initialise
		}
		
		
		public string					Text
		{
			get { return this.text; }
			set { this.text = value; }
		}
		
		public Drawing.Font				Font
		{
			get { return this.font; }
			set
			{
				if (this.font != value)
				{
					this.font = value;
					this.isDirty = true;
				}
			}
		}
		
		public Drawing.ContentAlignment	Alignment
		{
			get { return this.alignment; }
			set
			{
				if (this.alignment != value)
				{
					this.alignment = value;
					this.isDirty = true;
				}
			}
		}
		
		public double					FontSize
		{
			get { return this.fontSize; }
			set
			{
				if (this.fontSize != value)
				{
					this.fontSize = value;
					this.isDirty = true;
				}
			}
		}
		
		
		public Drawing.Size				LayoutSize
		{
			get { return this.layoutSize; }
			set
			{
				if (this.layoutSize != value)
				{
					this.layoutSize = value;
					this.isDirty = true;
				}
			}
		}
		
		public int						TotalLineCount
		{
			get
			{
				this.UpdateLayout ();
				
				//	TODO: retourne le nombre de lignes total dans le layout courant
				//	(y compris les lignes qui d�bordent)
				
				return 0;
			}
		}
		
		public Drawing.Rectangle		TotalRectangle
		{
			get
			{
				this.UpdateLayout ();
				
				//	TODO: retourne le rectangle englobant du layout courant; ce
				//	rectangle comprend toutes les lignes, m�me celles qui d�bordent.
				//	Si le texte est align� sur le bord droit, rectangle.X n'est pas
				//	forc�ment �gal � 0.
				
				return Drawing.Rectangle.Empty;
			}
		}
		
		public int						VisibleLineCount
		{
			get
			{
				this.UpdateLayout ();
				
				//	TODO: retourne le nombre de lignes visibles dans le layout courant
				//	(sans compter les lignes qui d�bordent)
				
				return 0;
			}
		}
		
		public Drawing.Rectangle		VisibleRectangle
		{
			get
			{
				this.UpdateLayout ();
				
				//	TODO: retourne le rectangle englobant du layout courant; ce
				//	rectangle comprend uniquement les lignes visibles.
				//	Si le texte est align� sur le bord droit, rectangle.X n'est pas
				//	forc�ment �gal � 0.
				
				return Drawing.Rectangle.Empty;
			}
		}
		
		
		public Drawing.IImageProvider	ImageProvider
		{
			get { return this.imageProvider; }
			set { this.imageProvider = value; }
		}
		
		
		public void Paint(PaintEventArgs e)
		{
			this.UpdateLayout ();
			
			//	TODO: dessine le texte, en fonction du layout...
		}
		
		
		public int DetectOffset(Drawing.Point pos)
		{
			this.UpdateLayout ();
			
			//	TODO: trouve l'offset dans le texte interne qui correspond � la
			//	position indiqu�e. Retourne -1 en cas d'�chec.
			
			return -1;
		}
		
		public string DetectAnchor(Drawing.Point pos)
		{
			int offset = this.DetectOffset(pos);
			
			if ( offset >= 0 )
			{
				string[] tags;
				
				if ( this.AnalyseTagsAtOffset(offset, out tags) )
				{
					//	TODO: d�tecte s'il y a un lien hypertexte dans la liste des
					//	tags actifs � la position en question. Si oui, extrait la cha�ne
					//	de l'argument href, en supprimant les guillemets (simples ou
					//	doubles).
					
					//	...
				}
			}
			
			return null;
		}
		
		
		public Drawing.Rectangle[] FindTextRange(int offset_begin, int offset_end)
		{
			if (offset_begin < offset_end)
			{
				this.UpdateLayout ();
				
				//	TODO: retourne un tableau avec les rectangles englobant le texte
				//	sp�cifi� par son d�but et sa fin. On ne compte pas les balises.
				//	En g�n�ral, il y a un rectangle par ligne.
			}
			
			return new Drawing.Rectangle[0];
		}
		
		
		// Retourne l'offset dans le texte interne, correspondant � l'index
		// sp�cifi� pour le texte sans tags. On saute tous les tags qui pr�c�dent
		// le caract�re indiqu� (textIndex=0 => premier caract�re non tag dans
		// le texte).
		public int FindOffsetFromIndex(int textIndex)
		{
			int    index = 0;
			int    startOffset;

			for ( int endOffset=0 ; endOffset<this.text.Length ; )
			{
				startOffset = endOffset;

				if ( this.text[endOffset] == '<' )
				{
					int length = this.text.IndexOf(">", endOffset)-endOffset+1;
					if ( length > 0 )
					{
						endOffset += length;
					}
					if ( this.text[endOffset-2] == '/' )  // <br/> ?
					{
						if ( index == textIndex )  return startOffset;
						index ++;
					}
				}
				else if ( this.text[endOffset] == '&' )
				{
					int length = this.text.IndexOf(";", endOffset)-endOffset+1;
					if ( length > 0 )
					{
						endOffset += length;
						if ( index == textIndex )  return startOffset;
						index ++;
					}
				}
				else
				{
					endOffset ++;
					if ( index == textIndex )  return startOffset;
					index ++;
				}
			}
			
			return -1;
		}
		
		// Retourne l'index dans le texte propre, correspondant � l'offset
		// sp�cifi� dans le texte avec tags.
		public int FindIndexFromOffset(int taggedTextOffset)
		{
			int    index = 0;
			int    startOffset;

			for ( int endOffset=0 ; endOffset<this.text.Length ; )
			{
				startOffset = endOffset;

				if ( this.text[endOffset] == '<' )
				{
					int length = this.text.IndexOf(">", endOffset)-endOffset+1;
					if ( length > 0 )
					{
						endOffset += length;
					}
					if ( this.text[endOffset-2] == '/' )  // <br/> ?
					{
						index ++;
					}
				}
				else if ( this.text[endOffset] == '&' )
				{
					int length = this.text.IndexOf(";", endOffset)-endOffset+1;
					if ( length > 0 )
					{
						endOffset += length;
						index ++;
					}
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
		
		
		// Enl�ve un tag � la fin de la liste.
		private void DeleteTagsList(string endTag, System.Collections.ArrayList list)
		{
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
			int    startOffset;
			int    endOffset = 0;
			while ( endOffset < offset )
			{
				startOffset = endOffset;
				TextLayout.Tag tag = TextLayout.ParseTag(text, ref endOffset, out parameters);
				if ( tag == TextLayout.Tag.None )  continue;

				string sTag = text.Substring(startOffset, endOffset-startOffset);

				if ( tag == TextLayout.Tag.Bold      ||
					 tag == TextLayout.Tag.Italic    ||
					 tag == TextLayout.Tag.Underline ||
					 tag == TextLayout.Tag.Mnemonic  ||
					 tag == TextLayout.Tag.Font      ||
					 tag == TextLayout.Tag.Anchor    )
				{
					list.Add(sTag);
				}

				if ( tag == TextLayout.Tag.BoldEnd      ||
					 tag == TextLayout.Tag.ItalicEnd    ||
					 tag == TextLayout.Tag.UnderlineEnd ||
					 tag == TextLayout.Tag.MnemonicEnd  ||
					 tag == TextLayout.Tag.FontEnd      ||
					 tag == TextLayout.Tag.AnchorEnd    )
				{
					DeleteTagsList(sTag, list);
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
					
						//	TODO: g�re les autres codes, entre autres les codes num�riques...
					
					default:
						throw new System.FormatException("Bad meta: " + meta);
				}
				
				offset += length;
				
				return code;
			}
			
			return text[offset++];
		}
		
		// Avance d'un caract�re ou d'un tag dans le texte.
		public static Tag ParseTag(string text, ref int offset, out System.Collections.Hashtable parameters)
		{
			parameters = null;
			
			if ( offset < text.Length && text[offset] == '<' )
			{
				int length = text.IndexOf(">", offset)-offset+1;
				
				if ( length > 0 )
				{
					string tag = text.Substring(offset, length);
					string tag_lower = tag.ToLower();
					
					offset += length;
					
					switch ( tag_lower )
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
					}
					
					int space = tag.IndexOf(" ");
					
					if ( space > 0 )
					{
						string pfx = tag_lower.Substring(0, space);
						string end = tag.Remove(0, space).TrimStart(' ');
						Tag tag_id = Tag.Unknown;
						
						string close = ">";
						
						switch ( pfx )
						{
							case "<a":		tag_id = Tag.Anchor;	close = ">";	break;
							case "<img":	tag_id = Tag.Image;		close = "/>";	break;
							case "<font":	tag_id = Tag.Font;		close = ">";	break;
						}
						
						if ( !end.EndsWith(close) )
						{
							return Tag.SyntaxError;
						}
						
						//	Enl�ve la fin du tag, comme �a on n'a r�ellement plus que les arguments.
						string arg = end.Remove(end.Length-close.Length, close.Length);
						parameters = new System.Collections.Hashtable();
						
						string arg_name;
						string arg_value;
						int pos = 0;
						while ( pos < arg.Length )
						{
							int	i;

							while ( pos < arg.Length && arg[pos] == ' ' )  pos++;

							i = arg.IndexOf("='", pos);
							if ( i < 0 )  break;
							i -= pos;
							arg_name = arg.Substring(pos, i);
							pos += i+2;

							i = arg.IndexOf("'", pos);
							if ( i < 0 )  break;
							i -= pos;
							arg_value = arg.Substring(pos, i);
							pos += i+1;

							parameters[arg_name] = arg_value;
						}
						return tag_id;
					}
					
					return Tag.Unknown;
				}
			}
			
			offset ++;
			return Tag.None;
		}
		
		// Trouve la s�quence <m>x</m> dans le texte et retourne le premier caract�re
		// de x comme code mn�monique (en majuscules).
		public static char ExtractMnemonic(string text)
		{
			System.Collections.Hashtable parameters;

			int    offset = 0;
			while ( offset < text.Length )
			{
				TextLayout.Tag tag = TextLayout.ParseTag(text, ref offset, out parameters);
				if ( tag == TextLayout.Tag.Mnemonic )
				{
					string s = "";		// on doit pouvoir faire bcp + simple !!!
					s += text[offset];	//  "
					s = s.ToUpper();	//  "
					return s[0];		//  "
				}
			}
			
			return '\0';
		}
		
		public static string ConvertToTaggedText(string text)
		{
			return TextLayout.ConvertToTaggedText (text, false);
		}
		
		// Convertit le texte simple en un texte compatible avec les tags. Supprime
		// toute occurrence de "<", "&" et ">" dans le texte.
		public static string ConvertToTaggedText(string text, bool auto_mnemonic)
		{
			if ( auto_mnemonic )
			{
				// Cherche les occurrences de "&" dans le texte et g�re comme suit:
				// - Remplace "&x" par "<m>x</m>" (le tag <m> sp�cifie un code mn�monique)
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
				text = text.Replace("&", "&amp;");
				text = text.Replace("<", "&lt;");
				text = text.Replace(">", "&gt;");
				text = text.Replace("\n", "<br/>");
				return text;
			}
		}
		
		public static string ConvertToSimpleText(string text)
		{
			return TextLayout.ConvertToSimpleText (text, "");
		}
		
		// Epure le texte en supprimant les tags <> et en rempla�ant les
		// tags &gt; et &lt; (et autres) par leurs caract�res �quivalents.
		// En plus, les images sont remplac�es par le texte 'imageReplacement'
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
						string tag_lower = tag.ToLower();
					
						offset += length;

						if ( tag_lower.IndexOf("<img ") == 0 )
						{
							buffer.Append(imageReplacement);
						}
						if ( tag_lower.IndexOf("<br") == 0 )
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
						string tag_lower = tag.ToLower();
					
						offset += length;
					
						switch ( tag_lower )
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

		
		protected virtual void UpdateLayout()
		{
			if (this.isDirty)
			{
				//	TODO: met � jour le cache interne en fonction du layout actuel
				
				this.isDirty = false;
			}
		}
		
		
		public enum Tag
		{
			None,						//	pas un tag
			Unknown,					//	tag pas reconnu
			SyntaxError,				//	syntaxe du tag pas correcte
			
			LineBreak,					//	<br/>
			Bold, BoldEnd,				//	<b>...</b>
			Italic, ItalicEnd,			//	<i>...</i>
			Underline, UnderlineEnd,	//	<u>...</u>
			Mnemonic, MnemonicEnd,		//	<m>...</m>				--> comme <u>...</u>
			Font, FontEnd,				//	<font ...>...</font>
			Anchor, AnchorEnd,			//	<a href='x'>...</a>
			Image,						//	<img src='x'/>
		}
		
		
		protected bool						isDirty;
		protected string					text;
		protected Drawing.Font				font;
		protected double					fontSize;
		protected Drawing.Size				layoutSize;
		protected Drawing.IImageProvider	imageProvider;
		protected Drawing.ContentAlignment	alignment = Drawing.ContentAlignment.TopLeft;
	}
}
