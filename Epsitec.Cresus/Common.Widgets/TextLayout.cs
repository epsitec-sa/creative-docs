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
					this.is_dirty = true;
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
					this.is_dirty = true;
				}
			}
		}
		
		public double					FontSize
		{
			get { return this.font_size; }
			set
			{
				if (this.font_size != value)
				{
					this.font_size = value;
					this.is_dirty = true;
				}
			}
		}
		
		
		public Drawing.Size				LayoutSize
		{
			get { return this.layout_size; }
			set
			{
				if (this.layout_size != value)
				{
					this.layout_size = value;
					this.is_dirty = true;
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
			get { return this.image_provider; }
			set { this.image_provider = value; }
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
			int offset = this.DetectOffset (pos);
			
			if (offset >= 0)
			{
				string[] tags;
				
				if (this.AnalyseTagsAtOffset (offset, out tags))
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
		
		
		public int FindOffsetFromIndex(int text_index)
		{
			//	TODO: retourne l'offset dans le texte interne, correspondant � l'index
			//	sp�cifi� pour le texte sans tags. On saute tous les tags qui pr�c�dent
			//	le caract�re indiqu� (text_index = 0 => premier caract�re non tag dans
			//	le texte).
			
			return 0;
		}
		
		public int FindIndexFromOffset(int tagged_text_offset)
		{
			//	TODO: retourne l'index dans le texte propre, correspondant � l'offset
			//	sp�cifi� dans le texte avec tags.
			
			return 0;
		}
		
		
		public bool AnalyseTagsAtOffset(int offset, out string[] tags)
		{
			tags = null;
			
			if ((offset >= 0) &&
				(offset <= this.Text.Length))
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				//	TODO: parcourt le texte et accumule les informations sur les tags <>
				//	reconnus. Il faut :
				//
				//	  - annuler un tag <x> lorsque l'on rencontre son tag </x>.
				//	  - sauter les tags <xx/>
				
				//	...
				
				tags = new string[list.Count];
				list.CopyTo (tags);
				
				return true;
			}
			
			return false;
		}
		
		
		public static char AnalyseMetaChar(string text, ref int index)
		{
			if (text[index] == '&')
			{
				int length = text.IndexOf (";", index) - index + 1;
				
				if (length < 3)
				{
					throw new System.FormatException ("Bad meta");
				}
				
				char code;
				string meta = text.Substring (index, length).ToLower ();
				
				switch (meta)
				{
					case "&lt;":	code = '<';			break;
					case "&gt;":	code = '>';			break;
					case "&amp;":	code = '&';			break;
					case "&quot;":	code = '"';			break;
					case "&nbsp;":	code = (char) 160;	break;
					
						//	TODO: g�re les autres codes, entre autres les codes num�riques...
					
					default:
						throw new System.FormatException ("Bad meta: " + meta);
				}
				
				index += length;
				
				return code;
			}
			
			return text[index++];
		}
		
		public static Tag ParseTag(string text, ref int index, out System.Collections.Hashtable parameters)
		{
			parameters = null;
			
			if ((text.Length > index) &&
				(text[index] == '<'))
			{
				int length = text.IndexOf (">", index) - index + 1;
				
				if (length > 0)
				{
					string tag = text.Substring (index, length);
					string tag_lower = tag.ToLower ();
					
					index += length;
					
					switch (tag_lower)
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
					
					int space = tag.IndexOf (" ");
					
					if (space > 0)
					{
						string pfx = tag_lower.Substring (0, space);
						string end = tag_lower.Remove (0, space).TrimStart (' ');
						Tag tag_id = Tag.Unknown;
						
						string close = ">";
						
						switch (pfx)
						{
							case "<a":		tag_id = Tag.Anchor;	close = ">";	break;
							case "<img":	tag_id = Tag.Image;		close = "/>";	break;
							case "<font":	tag_id = Tag.Font;		close = ">";	break;
						}
						
						if (! end.EndsWith (close))
						{
							return Tag.SyntaxError;
						}
						
						//	Enl�ve la fin du tag, comme �a on n'a r�ellement plus que les arguments.
						
						string arg = end.Remove (end.Length - close.Length, close.Length);
						parameters = new System.Collections.Hashtable ();
						
						for (int pos = 0; pos < arg.Length; pos++)
						{
							if (arg[pos] == ' ')
							{
								continue;
							}
							
							//	TODO: extrait et saute l'argument actuel.
							
							string arg_name  = "";
							string arg_value = "";
							
							parameters[arg_name] = arg_value;
						}
						
						return tag_id;
					}
					
					return Tag.Unknown;
				}
			}
			
			index++;
			
			return Tag.None;
		}
		
		public static char ExtractMnemonic(string text)
		{
			//	TODO: trouve la s�quence <m>x</m> dans le texte et retourne le premier caract�re
			//	de x comme code mn�monique (en majuscules).
			
			return '\0';
		}
		
		public static string ConvertToTaggedText(string text)
		{
			return TextLayout.ConvertToTaggedText (text, false);
		}
		
		public static string ConvertToTaggedText(string text, bool auto_mnemonic)
		{
			//	Convertit le texte simple en un texte compatible avec les tags. Supprime
			//	toute occurrence de "<", "&" et ">" dans le texte.
			
			if (auto_mnemonic)
			{
				//	TODO: cherche les occurrences de "&" dans le texte et g�re comme suit:
				//
				//	- Remplace "&x" par "<m>x</m>" (le tag <m> sp�cifie un code mn�monique)
				//	- Remplace "&&" par "&"
			}
			
			text = text.Replace ("&", "&amp;");
			text = text.Replace ("<", "&lt;");
			text = text.Replace (">", "&gt;");
			
			return text;
		}
		
		public static string ConvertToSimpleText(string text)
		{
			return TextLayout.ConvertToSimpleText (text, "");
		}
		
		public static string ConvertToSimpleText(string text, string image_replacement)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			//	TODO: �pure le texte en supprimant les tags <> et en rempla�ant les
			//	tags &gt; et &lt; (et autres) par leurs caract�res �quivalents.
			//	En plus, les images sont remplac�es par le texte 'image_replacement'
			
			return buffer.ToString ();
		}
		
		
		protected virtual void UpdateLayout()
		{
			if (this.is_dirty)
			{
				//	TODO: met � jour le cache interne en fonction du layout actuel
				
				this.is_dirty = false;
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
		
		
		protected bool						is_dirty;
		protected string					text;
		protected Drawing.Font				font;
		protected double					font_size;
		protected Drawing.Size				layout_size;
		protected Drawing.IImageProvider	image_provider;
		protected Drawing.ContentAlignment	alignment			= Drawing.ContentAlignment.TopLeft;
	}
}
