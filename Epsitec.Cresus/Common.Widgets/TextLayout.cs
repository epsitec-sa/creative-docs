namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe TextLayout permet de stocker et d'afficher des contenus
	/// riches (un sous-ensemble très restreint de HTML).
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
		
		
		public string					CleanText
		{
			get
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				//	TODO: épure le texte en supprimant les tags <> et en remplaçant les
				//	tags &gt; et &lt; par leurs caractères équivalents.
				
				return buffer.ToString ();
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
				//	(y compris les lignes qui débordent)
				
				return 0;
			}
		}
		
		public Drawing.Rectangle		TotalRectangle
		{
			get
			{
				this.UpdateLayout ();
				
				//	TODO: retourne le rectangle englobant du layout courant; ce
				//	rectangle comprend toutes les lignes, même celles qui débordent.
				//	Si le texte est aligné sur le bord droit, rectangle.X n'est pas
				//	forcément égal à 0.
				
				return Drawing.Rectangle.Empty;
			}
		}
		
		public int						VisibleLineCount
		{
			get
			{
				this.UpdateLayout ();
				
				//	TODO: retourne le nombre de lignes visibles dans le layout courant
				//	(sans compter les lignes qui débordent)
				
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
				//	Si le texte est aligné sur le bord droit, rectangle.X n'est pas
				//	forcément égal à 0.
				
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
			
			//	TODO: trouve l'offset dans le texte interne qui correspond à la
			//	position indiquée. Retourne -1 en cas d'échec.
			
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
					//	TODO: détecte s'il y a un lien hypertexte dans la liste des
					//	tags actifs à la position en question. Si oui, extrait la chaîne
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
				//	spécifié par son début et sa fin. On ne compte pas les balises.
				//	En général, il y a un rectangle par ligne.
			}
			
			return new Drawing.Rectangle[0];
		}
		
		
		public int FindOffsetFromIndex(int text_index)
		{
			//	TODO: retourne l'offset dans le texte interne, correspondant à l'index
			//	spécifié pour le texte sans tags. On saute tous les tags qui précèdent
			//	le caractère indiqué (text_index = 0 => premier caractère non tag dans
			//	le texte).
			
			return 0;
		}
		
		public int FindIndexFromOffset(int tagged_text_offset)
		{
			//	TODO: retourne l'index dans le texte propre, correspondant à l'offset
			//	spécifié dans le texte avec tags.
			
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
					
						//	TODO: gère les autres codes, entre autres les codes numériques...
					
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
						
						//	Enlève la fin du tag, comme ça on n'a réellement plus que les arguments.
						
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
		
		protected virtual void UpdateLayout()
		{
			if (this.is_dirty)
			{
				//	TODO: met à jour le cache interne en fonction du layout actuel
				
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
