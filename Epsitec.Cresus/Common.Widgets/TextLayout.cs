namespace Epsitec.Common.Widgets
{
	public class AnchorEventArgs : System.EventArgs
	{
		public AnchorEventArgs(double x, double y, double dx, double dy, int index)
		{
			this.rect  = new Drawing.Rectangle(x, y, dx, dy);
			this.index = index;
		}
		
		
		public Drawing.Rectangle				Bounds
		{
			get { return this.rect; }
		}
		
		public int								Index
		{
			get { return this.index; }
		}
		
		
		private Drawing.Rectangle				rect;
		private int								index;
	}
	
	public delegate void AnchorEventHandler(object sender, AnchorEventArgs e);


	public enum TextJustifMode
	{
		None,
		AllButLast,
		All,
	}

	/// <summary>
	/// La classe TextLayout permet de stocker et d'afficher des contenus
	/// riches (un sous-ensemble tr�s restreint de HTML).
	/// </summary>
	public class TextLayout
	{
		public TextLayout()
		{
		}
		

		public string							Text
		{
			// Texte associ�, contenant des commandes HTML.
			get
			{
				if ( this.text == null )
				{
					return "";
				}
				
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
						this.MarkContentsAsDirty();
					}
					else
					{
						throw new System.FormatException(string.Format ("Syntax error at char {0}.", offsetError.ToString()));
					}
				}
			}
		}
		
		public int								MaxTextOffset			// offset maximum (position "physique" dans le texte brut)
		{
			get { return (this.text == null) ? 0 : this.text.Length; }
		}
		
		public int								MaxTextIndex			// index maximum (position "logique", ind�pendante des tags de formatage)
		{
			get { return this.FindIndexFromOffset(this.MaxTextOffset); }
		}
		
		
		public Drawing.Font						Font
		{
			// Fonte par d�faut.
			get
			{
				return this.font;
			}

			set
			{
				if ( value != this.font )
				{
					this.font = value;
					this.MarkContentsAsDirty();
				}
			}
		}
		
		public double							FontSize
		{
			// Taille de la fonte par d�faut.
			get
			{
				return this.fontSize;
			}

			set
			{
				if ( this.fontSize != value )
				{
					this.fontSize = value;
					this.MarkContentsAsDirty();
				}
			}
		}

		public static Drawing.Color				DefaultColor
		{
			get { return TextLayout.defaultColor; }
			set { TextLayout.defaultColor = value; }
		}

		public static Drawing.Color				AnchorColor
		{
			// Couleur pour les liens.
			get { return TextLayout.anchorColor; }
			set { TextLayout.anchorColor = value; }
		}

		public static Drawing.Color				WaveColor
		{
			// Couleur pour les vagues.
			get { return TextLayout.waveColor; }
			set { TextLayout.waveColor = value; }
		}

		public Drawing.ContentAlignment			Alignment
		{
			// Alignement du texte dans le rectangle.
			get
			{
				return this.alignment;
			}

			set
			{
				if ( this.alignment != value )
				{
					this.alignment = value;
					this.MarkLayoutAsDirty();
				}
			}
		}

		public Drawing.TextBreakMode			BreakMode
		{
			// Mode de c�sure.
			get
			{
				return this.breakMode;
			}

			set
			{
				if ( this.breakMode != value )
				{
					this.breakMode = value;
					this.MarkContentsAsDirty();
				}
			}
		}

		public TextJustifMode					JustifMode
		{
			// Mode de justification.
			get
			{
				return this.justifMode;
			}

			set
			{
				if ( this.justifMode != value )
				{
					this.justifMode = value;
					this.MarkLayoutAsDirty();
				}
			}
		}

		public bool								ShowLineBreak
		{
			// D�termine si les <br/> sont visibles ou non.
			get
			{
				return this.showLineBreak;
			}

			set
			{
				this.showLineBreak = value;
			}
		}
		
		public Drawing.Size						LayoutSize
		{
			// Dimensions du rectangle.
			get
			{
				return this.layoutSize;
			}

			set
			{
				if ( this.layoutSize != value )
				{
					this.layoutSize = value;
					this.MarkLayoutAsDirty();
				}
			}
		}

		public Drawing.Size						SingleLineSize
		{
			// Retourne les dimensions du texte ind�pendament de LayoutSize,
			// s'il est mis sur une seule ligne.
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
		
		public int								TotalLineCount
		{
			// Retourne le nombre de lignes total dans le layout courant
			// (y compris les lignes qui d�bordent).
			get
			{
				this.UpdateLayout();
				return this.totalLine;
			}
		}
		
		public int								VisibleLineCount
		{
			// Retourne le nombre de lignes visibles dans le layout courant
			// (sans compter les lignes qui d�bordent).
			get
			{
				this.UpdateLayout();
				return this.visibleLine;
			}
		}
		
		public Drawing.Rectangle				TotalRectangle
		{
			// Retourne le rectangle englobant du layout courant; ce
			// rectangle comprend toutes les lignes, m�me celles qui d�bordent.
			get { return this.GetRectangleBounds(true); }
		}
		
		public Drawing.Rectangle				VisibleRectangle
		{
			// Retourne le rectangle englobant du layout courant; ce
			// rectangle comprend uniquement les lignes visibles.
			get { return this.GetRectangleBounds(false); }
		}
		
		public Drawing.Rectangle				StandardRectangle
		{
			// Retourne le rectangle standard englobant du layout courant; ce
			// rectangle ne d�pend pas de la hauteur des lettres du texte.
			// Le rectangle aura la m�me hauteur avec "ace" ou "Ap".
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
					if ( block.IsImage )
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
		
		public Drawing.IImageProvider			ImageProvider
		{
			// Gestionnaire d'images.
			get { return this.imageProvider; }
			set { this.imageProvider = value; }
		}


		protected Drawing.Rectangle GetRectangleBounds(bool all)
		{
			// Retourne le rectangle englobant du layout, en tenant compte de
			// toutes les lignes (all=true) ou seulement des lignes visibles (all=false).
			// Si le texte est align� sur le bord gauche, rectangle.Left n'est pas
			// forc�ment �gal � 0.
			this.UpdateLayout();

			Drawing.Rectangle totalRect = Drawing.Rectangle.Empty;
			foreach ( JustifBlock block in this.blocks )
			{
				if ( !all && !block.visible )  continue;

				Drawing.Rectangle blockRect;
				if ( block.IsImage )
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


		public bool IsSelectionBold(TextLayout.Context context)
		{
			// Indique si les caract�res s�lectionn�s sont gras.
			if ( context.PrepareOffset != -1 )  // pr�paration pour l'insertion ?
			{
				return this.IsPrepared(context, "bold");
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context.CursorFrom);
				if ( block == null )  return false;
				int i = block.font.StyleName.IndexOf("Bold");
				return ( i >= 0 );
			}
		}

		public void SetSelectionBold(TextLayout.Context context, bool bold)
		{
			// Met en gras ou en normal tous les caract�res s�lectionn�s.
			string state = bold ? "yes" : "no";
			this.PutSelect(context, "bold=\"" + state + "\"");
		}

		public bool IsSelectionItalic(TextLayout.Context context)
		{
			// Indique si les caract�res s�lectionn�s sont italiques.
			if ( context.PrepareOffset != -1 )  // pr�paration pour l'insertion ?
			{
				return this.IsPrepared(context, "italic");
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context.CursorFrom);
				if ( block == null )  return false;
				int i = block.font.StyleName.IndexOf("Italic");
				int j = block.font.StyleName.IndexOf("Oblique");
				return ( i >= 0 || j >= 0 );
			}
		}

		public void SetSelectionItalic(TextLayout.Context context, bool italic)
		{
			// Met en italique ou en normal tous les caract�res s�lectionn�s.
			string state = italic ? "yes" : "no";
			this.PutSelect(context, "italic=\"" + state + "\"");
		}

		public bool IsSelectionUnderlined(TextLayout.Context context)
		{
			// Indique si les caract�res s�lectionn�s sont soulign�s.
			if ( context.PrepareOffset != -1 )  // pr�paration pour l'insertion ?
			{
				return this.IsPrepared(context, "underline");
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context.CursorFrom);
				if ( block == null )  return false;
				return block.underline;
			}
		}

		public void SetSelectionUnderlined(TextLayout.Context context, bool underline)
		{
			// Met en soulign� ou en normal tous les caract�res s�lectionn�s.
			string state = underline ? "yes" : "no";
			this.PutSelect(context, "underline=\"" + state + "\"");
		}

		public string GetSelectionFontName(TextLayout.Context context)
		{
			// Indique le nom de la fonte des caract�res s�lectionn�s.
			if ( context.PrepareOffset != -1 )  // pr�paration pour l'insertion ?
			{
				return this.SearchPrepared(context, "face");
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context.CursorFrom);
				if ( block == null )  return "";
				return block.font.FaceName;
			}
		}

		public void SetSelectionFontName(TextLayout.Context context, string name)
		{
			// Modifie le nom de la fonte des caract�res s�lectionn�s.
			this.PutSelect(context, "face=\"" + name + "\"");
		}

		public double GetSelectionFontSize(TextLayout.Context context)
		{
			// Indique la taille des caract�res s�lectionn�s.
			if ( context.PrepareOffset != -1 )  // pr�paration pour l'insertion ?
			{
				string s = this.SearchPrepared(context, "size");
				if ( s == "" )  return 0.0;
				return System.Double.Parse(s);
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context.CursorFrom);
				if ( block == null )  return Drawing.Font.DefaultFontSize;
				return block.fontSize;
			}
		}

		public void SetSelectionFontSize(TextLayout.Context context, double size)
		{
			// Modifie la taille des caract�res s�lectionn�s.
			this.PutSelect(context, "size=\"" + size + "\"");
		}

		public Drawing.Color GetSelectionFontColor(TextLayout.Context context)
		{
			// Indique la couleur des caract�res s�lectionn�s.
			if ( context.PrepareOffset != -1 )  // pr�paration pour l'insertion ?
			{
				string s = this.SearchPrepared(context, "color");
				return Drawing.Color.FromName(s);
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context.CursorFrom);
				if ( block == null )  return Drawing.Color.Empty;
				return block.fontColor;
			}
		}

		public void SetSelectionFontColor(TextLayout.Context context, Drawing.Color color)
		{
			// Modifie la couleur des caract�res s�lectionn�s.
			string s = "#" + Drawing.Color.ToHexa(color);
			this.PutSelect(context, "color=\"" + s + "\"");
		}

		
		protected JustifBlock SearchJustifBlock(int index)
		{
			// Cherche le premier bloc correspondant � un index.
			this.UpdateLayout();

			JustifBlock found = null;
			foreach ( JustifBlock block in this.blocks )
			{
				if ( index >= block.beginIndex && index <= block.endIndex )
				{
					found = block;
				}
				if ( index < block.endIndex )
				{
					break;
				}
			}
			return found;
		}

		protected bool IsPrepared(TextLayout.Context context, string key)
		{
			// Cherche si la pr�paration contient une commande donn�e.
			if ( context.PrepareOffset == -1 )  return false;
			return (this.SearchPrepared(context, key) == "yes");
		}

		protected string SearchPrepared(TextLayout.Context context, string key)
		{
			// Cherche si la pr�paration contient une commande donn�e.
			if ( context.PrepareOffset == -1 )  return "";

			System.Collections.Hashtable parameters;
			string text = this.Text;
			string found = "";  // rien encore trouv�
			int from = context.PrepareOffset;
			int to   = context.PrepareOffset + context.PrepareLength1;
			while ( from < to )
			{
				Tag tag = TextLayout.ParseTag(text, ref from, out parameters);
				if ( tag == Tag.Put )
				{
					if ( parameters.ContainsKey(key) )  found = (string)parameters[key];
				}
			}
			return found;  // retourne la *derni�re* pr�paration trouv�e
		}

		protected void PutSelect(TextLayout.Context context, string cmd)
		{
			// Modifie les caract�res s�lectionn�s avec une commande <put..>..</put>.
			string begin = "<put " + cmd + ">";
			string end   = "</put>";

			if ( context.CursorFrom == context.CursorTo )  // pr�pare l'insertion ?
			{
				int cursor1 = this.FindOffsetFromIndex(context.CursorFrom, false);
				int cursor2 = cursor1;
				if ( context.PrepareOffset == -1 )  // premi�re pr�paration ?
				{
					context.PrepareOffset  = cursor1;
					context.PrepareLength1 = 0;
					context.PrepareLength2 = 0;
				}
				else	// ajoute � une pr�paration existante ?
				{
					cursor1 = context.PrepareOffset + context.PrepareLength1;
					cursor2 = cursor1 + context.PrepareLength2;
				}
				string text = this.Text;
				text = text.Insert(cursor2, end);
				text = text.Insert(cursor1, begin);
				context.PrepareLength1 += begin.Length;
				context.PrepareLength2 += end.Length;
				this.Text = text;
				// Il ne faut surtout pas faire de this.Simplify() ici !
			}
			else	// modifie les caract�res s�lectionn�s ?
			{
				int cursorFrom = System.Math.Min(context.CursorFrom, context.CursorTo);
				int cursorTo   = System.Math.Max(context.CursorFrom, context.CursorTo);

				int from = this.FindOffsetFromIndex(cursorFrom, false);
				int to   = this.FindOffsetFromIndex(cursorTo, true);

				string text = this.Text;
				text = text.Insert(to, end);
				text = text.Insert(from, begin);
				this.Text = text;
				this.Simplify();
			}
		}


		public void SelectAll(TextLayout.Context context)
		{
			// S�lectionne tout le texte.
			if ( this.text == null )  return;

			context.CursorFrom  = 0;
			context.CursorTo    = this.MaxTextOffset;
			context.CursorAfter = false;
		}

		public void SelectLine(TextLayout.Context context)
		{
			// S�lectionne toute la ligne.
			this.MoveExtremity(context, -1, false);
			int from = context.CursorFrom;
			this.MoveExtremity(context, 1, false);
			context.CursorFrom = from;
			context.CursorAfter = false;
		}

		public void SelectWord(TextLayout.Context context)
		{
			// S�lectionne tout le mot.
			string simple = TextLayout.ConvertToSimpleText(this.Text);

			while ( context.CursorFrom > 0 )
			{
				if ( this.IsWordSeparator(simple[context.CursorFrom-1]) )  break;
				context.CursorFrom --;
			}

			while ( context.CursorTo < simple.Length )
			{
				if ( this.IsWordSeparator(simple[context.CursorTo]) )  break;
				context.CursorTo ++;
			}

			context.CursorAfter = false;
		}

		
		protected int DeleteText(int from, int to)
		{
			// Supprime des caract�res, tout en conservant les commandes. S'il reste
			// des commandes superflues, elles seront supprim�es par Symplify().
			// Retourne l'index o� ins�rer les �ventuels caract�res rempla�ants.
			System.Collections.Hashtable parameters;
			string text = this.Text;

			int ins = -1;
			while ( from < to )
			{
				int begin = from;
				Tag tag = TextLayout.ParseTag(text, ref from, out parameters);
				if ( tag == Tag.None      ||
					 tag == Tag.LineBreak ||
					 tag == Tag.Image     )
				{
					text = text.Remove(begin, from-begin);
					to -= from-begin;
					from = begin;
					if ( ins == -1 )  ins = begin;
				}
			}

			this.Text = text;
			return (ins == -1) ? to : ins;
		}

		
		public bool HasSelection(TextLayout.Context context)
		{
			int cursorFrom = this.FindOffsetFromIndex(context.CursorFrom);
			int cursorTo   = this.FindOffsetFromIndex(context.CursorTo);
			
			return cursorFrom != cursorTo;
		}
		
		public bool DeleteSelection(TextLayout.Context context)
		{
			// Supprime les caract�res s�lectionn�s dans le texte.
			int cursorFrom = this.FindOffsetFromIndex(context.CursorFrom);
			int cursorTo   = this.FindOffsetFromIndex(context.CursorTo);
			
			int from = System.Math.Min(cursorFrom, cursorTo);
			int to   = System.Math.Max(cursorFrom, cursorTo);
			
			if ( from == to )  return false;
			
			this.DeleteText(from, to);
			from = this.FindIndexFromOffset(from);
			context.CursorTo   = from;
			context.CursorFrom = from;
			this.Simplify();
			return true;
		}

		public bool ReplaceSelection(TextLayout.Context context, string ins)
		{
			// Ins�re une cha�ne correspondant � un caract�re ou un tag (jamais plus).
			int cursorFrom = this.FindOffsetFromIndex(context.CursorFrom);
			int cursorTo   = this.FindOffsetFromIndex(context.CursorTo);
			
			int from = System.Math.Min(cursorFrom, cursorTo);
			int to   = System.Math.Max(cursorFrom, cursorTo);
			
			if ( from < to )  // caract�res s�lectionn�s � supprimer ?
			{
				int cursor = this.DeleteText(from, to);
				from = this.FindIndexFromOffset(from);
				context.CursorTo   = from;
				context.CursorFrom = from;

				if ( this.MaxTextOffset+ins.Length > context.MaxChar )  return false;

				string text = this.Text;
				text = text.Insert(cursor, ins);
				this.Text = text;
				context.CursorTo    = this.FindIndexFromOffset(cursor + ins.Length);
				context.CursorFrom  = context.CursorTo;
				context.CursorAfter = false;

				// Simplifie seulement apr�s avoir supprim� la s�lection puis ins�r�
				// le caract�re, afin qu'il utilise les m�mes attributs typographiques.
				this.Simplify();
			}
			else
			{
				if ( this.MaxTextOffset+ins.Length > context.MaxChar )  return false;
			
				string text = this.Text;
				int cursor = this.FindOffsetFromIndex(context.CursorTo, context.CursorAfter);
				bool prepare = false;
				if ( context.PrepareOffset != -1 )  // a-t-on pr�par� des attributs typographiques ?
				{
					cursor = context.PrepareOffset + context.PrepareLength1;
					prepare = true;
				}
				text = text.Insert(cursor, ins);
				this.Text = text;
				context.CursorTo    = this.FindIndexFromOffset(cursor + ins.Length);
				context.CursorFrom  = context.CursorTo;
				context.CursorAfter = false;

				if ( prepare )  this.Simplify();  // supprime les pr�parations <put...>
			}

			this.DefineCursorPosX(context);
			return true;
		}

		public bool InsertCharacter(TextLayout.Context context, char character)
		{
			// Ins�re un caract�re.
			return this.ReplaceSelection(context, TextLayout.ConvertToTaggedText(character));
		}

		public bool DeleteCharacter(TextLayout.Context context, int dir)
		{
			// Supprime le caract�re � gauche ou � droite du curseur.
			if ( this.DeleteSelection(context) )  return false;

			int from, to;
			if ( dir < 0 )  // � gauche du curseur ?
			{
				if ( context.CursorTo == 0 )  return false;
				from = this.FindOffsetFromIndex(context.CursorTo-1);
				to   = this.FindOffsetFromIndex(context.CursorTo);
			}
			else	// � droite du curseur ?
			{
				from = this.FindOffsetFromIndex(context.CursorTo);
				to   = this.FindOffsetFromIndex(context.CursorTo+1);
			}

			if ( from == to )  return false;
			this.DeleteText(from, to);
			int cursor = this.FindIndexFromOffset(from);
			context.CursorTo    = cursor;
			context.CursorFrom  = cursor;
			context.CursorAfter = (dir < 0);

			string text = this.Text;
			this.Simplify();
			if ( text == this.Text )
			{
				context.PrepareOffset  = from;
				context.PrepareLength1 = 0;
				context.PrepareLength2 = 0;
			}

			return true;
		}
		
		
		public bool MoveLine(TextLayout.Context context, int move, bool select)
		{
			// D�place le curseur par lignes.
			int index;
			bool after;
			int line = context.CursorLine+move;
			
			if ( line < 0 ||
				 line >= this.TotalLineCount ||
				 !this.DetectIndex(context.CursorPosX, line, out index, out after) )
			{
				return false;
			}

			context.CursorTo = index;
			if ( !select )  context.CursorFrom = index;
			context.CursorAfter = after;
			return true;
		}

		public bool MoveExtremity(TextLayout.Context context, int move, bool select)
		{
			// D�place le curseur au d�but ou � la fin d'une ligne.
			double posx;
			if ( move < 0 )  posx = 0;
			else             posx = this.LayoutSize.Width;
			int index;
			bool after;
			if ( !this.DetectIndex(posx, context.CursorLine, out index, out after) )
			{
				return false;
			}

			context.CursorTo = index;
			if ( !select )  context.CursorFrom = index;
			context.CursorAfter = after;

			this.DefineCursorPosX(context);
			return true;
		}

		
		protected bool IsWordSeparator(char character)
		{
			// Indique si un caract�re est un s�parateur pour les d�placements
			// avec Ctrl+fl�che.
			character = System.Char.ToLower(character);
			if ( character == '_' ||
				 character == '�' || character == '�' || character == '�' || character == '�' ||
				 character == '�' ||
				 character == '�' || character == '�' || character == '�' || character == '�' ||
				 character == '�' || character == '�' || character == '�' || character == '�' ||
				 character == '�' || character == '�' || character == '�' || character == '�' ||
				 character == '�' || character == '�' || character == '�' || character == '�' )  return false;
			// TODO: g�n�raliser avec tous les accents exotiques ?
			if ( character >= 'a' && character <= 'z' )  return false;
			if ( character >= '0' && character <= '9' )  return false;
			return true;
		}

		protected bool IsDualCursor(int index)
		{
			// Indique s'il existe 2 positions diff�rentes pour un index.
			// L'une avec CursorAfter = false, et l'autre avec CursorAfter = true.
			this.UpdateLayout();

			int total = 0;
			int rankLine = 0;
			foreach ( JustifBlock block in this.blocks )
			{
				if ( block.visible && index >= block.beginIndex && index <= block.endIndex )
				{
					JustifLine line = (JustifLine)this.lines[block.indexLine];

					if ( total == 0 )
					{
						rankLine = line.rank;
					}
					else if ( total == 1 )
					{
						return ( rankLine != line.rank );
					}
					total ++;
				}
			}
			return false;
		}

		
		public bool MoveCursor(TextLayout.Context context, int move, bool select, bool word)
		{
			// D�place le curseur.
			if ( move == 1 && !select && !word &&
				 context.CursorFrom == context.CursorTo &&
				 !context.CursorAfter &&
				 this.IsDualCursor(context.CursorTo) )
			{
				context.CursorAfter = true;
				this.DefineCursorPosX(context);
				return true;
			}

			if ( move == -1 && !select && !word &&
				 context.CursorFrom == context.CursorTo &&
				 context.CursorAfter &&
				 this.IsDualCursor(context.CursorTo) )
			{
				context.CursorAfter = false;
				this.DefineCursorPosX(context);
				return true;
			}

			context.CursorAfter = (move < 0);

			int from = System.Math.Min(context.CursorFrom, context.CursorTo);
			int to   = System.Math.Max(context.CursorFrom, context.CursorTo);
			int cursor = (move < 0) ? from : to;
			if ( select )  cursor = context.CursorTo;
			string simple = TextLayout.ConvertToSimpleText(this.Text);

			if ( word )  // d�placement par mots ?
			{
				if ( move < 0 )
				{
					while ( cursor > 0 )
					{
						if ( !this.IsWordSeparator(simple[cursor-1]) )  break;
						cursor --;
					}
					while ( cursor > 0 )
					{
						if ( this.IsWordSeparator(simple[cursor-1]) )  break;
						cursor --;
					}
				}
				else
				{
					while ( cursor < simple.Length )
					{
						if ( this.IsWordSeparator(simple[cursor]) )  break;
						cursor ++;
					}
					while ( cursor < simple.Length )
					{
						if ( !this.IsWordSeparator(simple[cursor]) )  break;
						cursor ++;
					}
				}
			}
			else	// d�placement par caract�res ?
			{
				cursor += move;
			}

			cursor = System.Math.Max(cursor, 0);
			cursor = System.Math.Min(cursor, simple.Length);
			if ( cursor == context.CursorTo && cursor == context.CursorFrom )  return false;

			context.CursorTo = cursor;
			if ( !select )  context.CursorFrom = cursor;

			this.DefineCursorPosX(context);
			return true;
		}

		public void DefineCursorPosX(TextLayout.Context context)
		{
			// M�morise la position horizontale du curseur, afin de pouvoir y
			// revenir en cas de d�placement par lignes.
			Drawing.Point p1, p2;
			if ( this.FindTextCursor(context, out p1, out p2) )
			{
				context.CursorPosX = p1.X;
			}
		}


		public void Paint(Drawing.Point pos, Drawing.IPaintPort graphics)
		{
			// Dessine le texte, en fonction du layout...
			// Si une couleur est donn�e avec uniqueColor, tout le texte est peint
			// avec cette couleur, en ignorant les <font color=...>.
			this.Paint(pos, graphics, Drawing.Rectangle.Infinite, Drawing.Color.Empty, Drawing.GlyphPaintStyle.Normal);
		}

		public void Paint(Drawing.Point pos, Drawing.IPaintPort graphics, Drawing.Rectangle clipRect, Drawing.Color uniqueColor, Drawing.GlyphPaintStyle paintStyle)
		{
			this.UpdateLayout();

			IAdorner adorner = Adorner.Factory.Active;
			foreach ( JustifBlock block in this.blocks )
			{
				if ( !block.visible )  continue;

				if ( block.IsImage )
				{
					Drawing.Image image = block.image;
					
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
						this.OnAnchor(new AnchorEventArgs(ix, iy, dx, dy, block.beginIndex));
					}
					
					graphics.Align(ref ix, ref iy);
					graphics.PaintImage(image.BitmapImage, ix, iy, dx, dy, 0, 0, image.Width, image.Height);
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
						if ( color.IsEmpty )
						{
							color = TextLayout.defaultColor;
						}
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

				graphics.Color = color;

				if ( block.infos == null )
				{
					graphics.PaintText(x, y, block.text, block.font, block.fontSize);
				}
				else
				{
					graphics.PaintText(x, y, block.text, block.font, block.fontSize, block.infos);
				}

				if ( block.underline )
				{
					Drawing.Point p1, p2;
					this.UnderlinePoints(graphics, block, pos, out p1, out p2);
					graphics.LineWidth = 1.0;
					graphics.Color = color;
					graphics.PaintOutline(Drawing.Path.FromLine(p1, p2));
				}

				if ( block.wave )
				{
					Drawing.Point p1, p2;
					this.UnderlinePoints(graphics, block, pos, out p1, out p2);
					graphics.LineWidth = 0.75;
					if ( block.waveColor.IsEmpty )
					{
						graphics.Color = TextLayout.waveColor;
					}
					else
					{
						graphics.Color = block.waveColor;
					}
					graphics.PaintOutline(this.PathWave(p1, p2));
				}

				if ( this.showLineBreak && block.lineBreak )
				{
					double width = block.width;

					if ( block.infos != null )
					{
						double[] charsWidth;
						block.font.GetTextCharEndX(block.text, block.infos, out charsWidth);
						width = charsWidth[charsWidth.Length-1]*block.fontSize;
					}

					JustifLine line = (JustifLine)this.lines[block.indexLine];
					Drawing.Rectangle rect = new Drawing.Rectangle();
					rect.Top    = pos.Y+block.pos.Y+block.font.Ascender*block.fontSize; //line.ascender;
					rect.Bottom = pos.Y+block.pos.Y+block.font.Descender*block.fontSize; //line.descender;
					rect.Left   = pos.X+block.pos.X+width;
					rect.Width  = rect.Height*0.5;
					graphics.Color = color;
					graphics.PaintSurface(this.PathLineBreak(rect));
				}
			}
		}

		
		protected void UnderlinePoints(Drawing.IPaintPort graphics, JustifBlock block,
									   Drawing.Point pos,
									   out Drawing.Point p1, out Drawing.Point p2)
		{
			// Calcule les points de la ligne pour souligner un bloc.
			double width = block.width;

			if ( block.infos != null )
			{
				double[] charsWidth;
				block.font.GetTextCharEndX(block.text, block.infos, out charsWidth);
				width = charsWidth[charsWidth.Length-1]*block.fontSize;
			}

			p1 = new Drawing.Point();
			p2 = new Drawing.Point();
			p1.X = pos.X+block.pos.X;
			p2.X = p1.X+width;
			p1.Y = pos.Y+block.pos.Y;

			JustifLine line = (JustifLine)this.lines[block.indexLine];
			p1.Y += line.descender/2;
			p2.Y = p1.Y;

			double x,y;
			x = p1.X;  y = p1.Y;
			graphics.Align(ref x, ref y);
			p1.X = x;  p1.Y = y;

			x = p2.X;  y = p2.Y;
			graphics.Align(ref x, ref y);
			p2.X = x;  p2.Y = y;

			p1.Y -= 0.5;  // pour feinter l'anti-aliasing !
			p2.Y -= 0.5;
		}

		protected Drawing.Path PathWave(Drawing.Point p1, Drawing.Point p2)
		{
			// G�n�re le chemin d'une vague "/\/\/\/\/\/\".
			// Le d�but "montant" de la vague est toujours align� sur x=0, afin que
			// deux vagues successives soient jointives.
			Drawing.Path path = new Drawing.Path();
			double len = 4;  // p�riode d'une vague
			p1.Y -= 1;
			double start = p1.X-p1.X%len-0.5;  // -0.5 pour feinter l'anti-aliasing !
			double end   = p2.X-p2.X%len+len+0.5;
			for ( double x=start ; x<=end ; x+=len )
			{
				for ( int i=0 ; i<2 ; i++ )
				{
					Drawing.Point pp1 = new Drawing.Point();
					Drawing.Point pp2 = new Drawing.Point();

					if ( i == 0 )  // segment montant / ?
					{
						pp1.X = x+len*0.0;
						pp2.X = x+len*0.5;
						pp1.Y = p1.Y-len*0.25;
						pp2.Y = p1.Y+len*0.25;
					}
					else	// segment descendant \ ?
					{
						pp1.X = x+len*0.5;
						pp2.X = x+len*1.0;
						pp1.Y = p1.Y+len*0.25;
						pp2.Y = p1.Y-len*0.25;
					}

					if ( pp1.X >= p2.X || pp2.X <= p1.X )  continue;

					if ( pp1.X < p1.X )  // d�passe � gauche ?
					{
						pp1.Y += (pp2.Y-pp1.Y)*(p1.X-pp1.X)/(pp2.X-pp1.X);
						pp1.X = p1.X;
					}

					if ( pp2.X > p2.X )  // d�passe � droite ?
					{
						pp2.Y += (pp1.Y-pp2.Y)*(p2.X-pp2.X)/(pp1.X-pp2.X);
						pp2.X = p2.X;
					}

					if ( path.IsEmpty )  path.MoveTo(pp1);
					path.LineTo(pp2);
				}
			}
			return path;
		}

		protected Drawing.Path PathLineBreak(Drawing.Rectangle rect)
		{
			// G�n�re le chemin pour repr�senter un <br/>.
			Drawing.Path path = new Drawing.Path();
			path.MoveTo(rect.Left+rect.Width*0.00, rect.Bottom+rect.Height*0.30);
			path.LineTo(rect.Left+rect.Width*0.60, rect.Bottom+rect.Height*0.00);
			path.LineTo(rect.Left+rect.Width*0.40, rect.Bottom+rect.Height*0.25);
			path.LineTo(rect.Left+rect.Width*1.00, rect.Bottom+rect.Height*0.25);
			path.LineTo(rect.Left+rect.Width*1.00, rect.Bottom+rect.Height*0.70);
			path.LineTo(rect.Left+rect.Width*0.80, rect.Bottom+rect.Height*0.70);
			path.LineTo(rect.Left+rect.Width*0.80, rect.Bottom+rect.Height*0.35);
			path.LineTo(rect.Left+rect.Width*0.40, rect.Bottom+rect.Height*0.35);
			path.LineTo(rect.Left+rect.Width*0.60, rect.Bottom+rect.Height*0.60);
			path.Close();
			return path;
		}
		
		
		protected void OnAnchor(AnchorEventArgs e)
		{
			if ( this.Anchor != null )
			{
				this.Anchor(this, e);
			}
		}
		
		
		public bool DetectIndex(Drawing.Point pos, out int index, out bool after)
		{
			return this.DetectIndex(pos, -1, out index, out after);
		}
		
		public bool DetectIndex(double posx, int posLine, out int index, out bool after)
		{
			return this.DetectIndex(new Drawing.Point(posx, 0), posLine, out index, out after);
		}

		
		protected bool DetectIndex(Drawing.Point pos, int posLine, out int index, out bool after)
		{
			// Trouve l'index dans le texte interne qui correspond � la
			// position indiqu�e. Retourne false en cas d'�chec.
			this.UpdateLayout();
			
			pos.Y = System.Math.Max(pos.Y, 0);
			pos.Y = System.Math.Min(pos.Y, this.layoutSize.Height);
			pos.X = System.Math.Max(pos.X, 0);
			pos.X = System.Math.Min(pos.X, this.layoutSize.Width);

			foreach ( JustifLine line in this.lines )
			{
				if ( !line.visible )  continue;

				if ( (posLine == -1                     &&
					  pos.Y <= line.pos.Y+line.ascender &&
					  pos.Y >= line.pos.Y+line.descender) ||
					 posLine == line.rank )
				{
					for ( int j=line.firstBlock ; j<=line.lastBlock ; j++ )
					{
						JustifBlock block = (JustifBlock)this.blocks[j];

						double before = 0;
						if ( block.bol )
						{
							before = block.pos.X;
						}

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
							else
							{
								width = nextBlock.pos.X-block.pos.X;
							}
						}

						if ( pos.X >= block.pos.X-before &&
							 pos.X <= block.pos.X+width  )
						{
							if ( block.IsImage )
							{
								index = ( pos.X-block.pos.X > width/2 ) ? block.endIndex : block.beginIndex;
								after = false;
								return true;
							}
							else
							{
								double[] charsWidth;
								if ( block.infos == null )
								{
									block.font.GetTextCharEndX(block.text, out charsWidth);
								}
								else
								{
									block.font.GetTextCharEndX(block.text, block.infos, out charsWidth);
								}
								double left = 0;
								double right;
								int max = System.Math.Min(charsWidth.Length, block.endIndex-block.beginIndex);
								for ( int k=0 ; k<max ; k++ )
								{
									right = charsWidth[k]*block.fontSize;
									if ( pos.X-block.pos.X <= left+(right-left)/2 )
									{
										index = block.beginIndex+k;
										after = this.IsAfter(j, index);
										return true;
									}
									left = right;
								}
								index = block.beginIndex+max;
								after = this.IsAfter(j, index);
								return true;
							}
						}
					}
				}
			}
			index = -1;
			after = false;
			return false;
		}

		protected bool IsAfter(int blockRank, int index)
		{
			// Teste si le bloc pr�c�dent contient aussi le m�me index. Si oui,
			// retourne true, car l'index trouv� correspond � la 2�me position possible.
			if ( blockRank > 0 )
			{
				JustifBlock prevBlock = (JustifBlock)this.blocks[blockRank-1];
				if ( index >= prevBlock.beginIndex && index <= prevBlock.endIndex )
				{
					return true;
				}
			}
			return false;
		}

		
		public string DetectAnchor(Drawing.Point pos)
		{
			// D�tecte s'il y a un lien hypertexte dans la liste des
			// tags actifs � la position en question. Si oui, extrait la cha�ne
			// de l'argument href, en supprimant les guillemets.
			int index;
			bool after;
			this.DetectIndex(pos, out index, out after);
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
		

		protected double IndexToPosX(JustifBlock block, JustifLine line, int index)
		{
			// Retourne la position horizontale correspondant � un index dans un bloc.
			if ( block.lineBreak && index == block.endIndex+1 )
			{
				return block.pos.X + block.width + line.height/2;
			}

			if ( index <= block.beginIndex )  return block.pos.X;
			if ( index >  block.endIndex   )  index = block.beginIndex+block.text.Length;

			double[] charsWidth;
			if ( block.infos == null )
			{
				block.font.GetTextCharEndX(block.text, out charsWidth);
			}
			else
			{
				block.font.GetTextCharEndX(block.text, block.infos, out charsWidth);
			}
			return block.pos.X+charsWidth[index-block.beginIndex-1]*block.fontSize;
		}

		
		public SelectedArea[] FindTextRange(Drawing.Point pos, int indexBegin, int indexEnd)
		{
			// Retourne un tableau avec les rectangles englobant le texte
			// sp�cifi� par son d�but et sa fin. Il y a un rectangle par ligne.
			if ( indexBegin >= indexEnd )  return new SelectedArea[0];

			this.UpdateLayout();

			System.Collections.ArrayList list = new System.Collections.ArrayList();
			SelectedArea area = new SelectedArea();
			area.Rect.Top    = -TextLayout.Infinite;
			area.Rect.Bottom =  TextLayout.Infinite;
			area.Rect.Left   =  TextLayout.Infinite;
			area.Rect.Right  = -TextLayout.Infinite;
			foreach ( JustifBlock block in this.blocks )
			{
				JustifLine line = (JustifLine)this.lines[block.indexLine];
				if ( !block.visible )  continue;

				int bbi = block.beginIndex;
				int bei = bbi + block.text.Length;
				if ( block.lineBreak )  bei ++;
				int localBegin = System.Math.Max(indexBegin, bbi);
				int localEnd   = System.Math.Min(indexEnd,   bei);

				if ( localBegin >= localEnd )                            continue;
				if ( localBegin >= block.endIndex && !block.lineBreak )  continue;

				double top    = line.pos.Y+line.ascender;
				double bottom = line.pos.Y+line.descender;
				Drawing.Color color = block.fontColor;

				if ( area.Rect.Top    != top    ||
					 area.Rect.Bottom != bottom ||  // rectangle dans autre ligne ?
					 area.Color       != color  )
				{
					if ( area.Rect.Top > -TextLayout.Infinite &&
						 area.Rect.Left < TextLayout.Infinite )
					{
						list.Add(area);
					}

					area = new SelectedArea();
					area.Rect.Top    = top;
					area.Rect.Bottom = bottom;
					area.Rect.Left   =  TextLayout.Infinite;
					area.Rect.Right  = -TextLayout.Infinite;
					area.Color       = color;
				}

				if ( block.IsImage )
				{
					area.Rect.Left  = System.Math.Min(area.Rect.Left,  block.pos.X);
					area.Rect.Right = System.Math.Max(area.Rect.Right, block.pos.X+block.width);
				}
				else
				{
					area.Rect.Left  = System.Math.Min(area.Rect.Left,  this.IndexToPosX(block, line, localBegin));
					area.Rect.Right = System.Math.Max(area.Rect.Right, this.IndexToPosX(block, line, localEnd  ));
				}
			}
			
			if ( area.Rect.Top > -TextLayout.Infinite &&
				 area.Rect.Left < TextLayout.Infinite )
			{
				list.Add(area);
			}
			
			SelectedArea[] areas = new SelectedArea[list.Count];
			list.CopyTo(areas);
			
			for ( int i=0 ; i<areas.Length ; i++ )
			{
				areas[i].Rect.Offset(pos.X, pos.Y);
			}
					
			return areas;
		}
		
		public Drawing.Point GetLineOrigin(int line)
		{
			Drawing.Point pos;
			double ascender, descender, width;
			
			if ( this.GetLineGeometry(line, out pos, out ascender, out descender, out width) )
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
		
		
		
		public bool FindTextCursor(Context context, out Drawing.Point p1, out Drawing.Point p2)
		{
			// Retourne les deux extr�mit�s du curseur.
			// Indique �galement le num�ro de la ligne (0..n).
			this.UpdateLayout();

			int  index = context.CursorTo;
			bool after = context.CursorAfter;
			p1 = new Drawing.Point();
			p2 = new Drawing.Point();
			context.CursorLine = -1;
			int i = after ? this.blocks.Count-1 : 0;
			while ( i >= 0 && i < this.blocks.Count )
			{
				JustifBlock block = (JustifBlock)this.blocks[i];
				JustifLine line = (JustifLine)this.lines[block.indexLine];
				if ( block.visible && index >= block.beginIndex && index <= block.endIndex )
				{
					p2.Y = line.pos.Y+line.ascender;
					p1.Y = line.pos.Y+line.descender;
					if ( block.IsImage )
					{
						if ( index == block.beginIndex )
						{
							p1.X = block.pos.X;
							p2.X = p1.X;
						}
						else
						{
							p1.X = block.pos.X+block.width;
							p2.X = p1.X;
						}
					}
					else
					{
						p1.X = this.IndexToPosX(block, line, index);
						p2.X = p1.X;
					}

					double angle = 0.0;
					if ( block.italic )
					{
						angle = 90.0-block.font.CaretSlope;
					}
					if ( context.PrepareOffset != -1 )
					{
						angle = this.ScanItalic(context.PrepareOffset+context.PrepareLength1) ? Drawing.Font.DefaultObliqueAngle : 0.0;
					}
					if ( angle != 0.0 )
					{
						angle *= System.Math.PI/180.0;  // en radians
						double f = System.Math.Sin(angle);
						p2.X += line.ascender*f;
						p1.X += line.descender*f;
					}

					context.CursorLine = line.rank;
					return true;
				}
				i += after ? -1 : 1;
			}

			return false;
		}

		protected bool ScanItalic(int offset)
		{
			System.Collections.Hashtable parameters;
			string text = this.Text;

			int from = 0;
			int to   = offset;
			bool italic = false;
			while ( from < to )
			{
				Tag tag = TextLayout.ParseTag(text, ref from, out parameters);

				if ( tag == Tag.Italic )
				{
					italic = true;
				}

				if ( tag == Tag.ItalicEnd )
				{
					italic = false;
				}

				if ( tag == Tag.Put )
				{
					if ( parameters.ContainsKey("italic") )
					{
						string s = (string)parameters["italic"];
						italic = (s == "yes");
					}
				}
			}

			return italic;
		}

		public Drawing.Point FindTextEnd()
		{
			// Retourne le coin inf�rieur/droite du dernier caract�re.
			this.UpdateLayout();

			if ( this.blocks.Count == 0 )
			{
				return new Drawing.Point();
			}

			JustifBlock block = (JustifBlock)this.blocks[this.blocks.Count-1];
			Drawing.Point pos = new Drawing.Point(block.pos.X+block.width, block.pos.Y+block.font.Descender*block.fontSize);
			return pos;
		}
		
		
#if false
		public int AdvanceTag(int offset)
		{
			// Si on est au d�but d'un tag, donne la longueur jusqu'� la fin.
			if ( offset >= this.MaxTextOffset )  return 0;

			if ( this.text[offset] == '<' )  // tag <xx> ?
			{
				int initial = offset;
				while ( this.text[offset] != '>' )
				{
					offset ++;
					if ( offset >= this.MaxTextOffset )  break;
				}
				return offset-initial+1;
			}

			if ( this.text[offset] == '&' )  // tag &xx; ?
			{
				int initial = offset;
				while ( this.text[offset] != ';' )
				{
					offset ++;
					if ( offset >= this.MaxTextOffset )  break;
				}
				return offset-initial+1;
			}

			return 1;
		}

		public int RecedeTag(int offset)
		{
			// Si on est � la fin d'un tag, donne la longueur jusqu'au d�but.
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
					if ( offset == 0 || initial-offset > 10 || this.text[offset-1] == ';' )  return 1;
					offset --;
				}
				return initial-offset+1;
			}

			return 1;
		}
#endif


		public int FindOffsetFromIndex(int textIndex)
		{
			return this.FindOffsetFromIndex(textIndex, true);
		}

		public int FindOffsetFromIndex(int textIndex, bool after)
		{
			// Retourne l'offset dans le texte interne, correspondant � l'index
			// sp�cifi� pour le texte sans tags. Si after=true, on saute tous les
			// tags qui pr�c�dent le caract�re indiqu� (textIndex=0 => premier
			// caract�re non tag dans le texte).
			int    index = 0;
			int    beginOffset;
			int    endOffset = 0;
			int    textLength = this.MaxTextOffset;
			
			while ( endOffset <= textLength )
			{
				if ( endOffset == textLength )
				{
					if ( index == textIndex )  return endOffset;
					
					break;
				}
				
				beginOffset = endOffset;

				if ( !after )
				{
					if ( index == textIndex )  return beginOffset;
				}

				if ( this.text[endOffset] == '<' )
				{
					int length = this.text.IndexOf(">", endOffset)-endOffset+1;
					if ( length < 0 )  return -1;
					int more = System.Math.Min(5, length);
					endOffset += length;
					string startOfTag = this.text.Substring(beginOffset, more);
					
					if ( startOfTag != "<br/>" && startOfTag != "<img " )  continue;
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

				if ( after )
				{
					if ( index == textIndex )  return beginOffset;
				}
				index ++;
			}
			
			return -1;
		}
		
		public int FindIndexFromOffset(int taggedTextOffset)
		{
			// Retourne l'index dans le texte propre, correspondant � l'offset
			// sp�cifi� dans le texte avec tags.
			int    index = 0;
			int    beginOffset;

			if ( taggedTextOffset == 0 )  return index;

			int endOffset = 0;
			int textLength = this.MaxTextOffset;
			
			while ( endOffset < textLength )
			{
				beginOffset = endOffset;

				if ( this.text[endOffset] == '<' )
				{
					int length = this.text.IndexOf(">", endOffset)-endOffset+1;
					if ( length < 0 )  return -1;
					endOffset += length;
					int more = System.Math.Min(5, length);

					string startOfTag = this.text.Substring(beginOffset, more);
					
					if ( startOfTag == "<br/>" || startOfTag == "<img " )
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
		
		
		protected static bool DeleteTagsList(string endTag, System.Collections.ArrayList list)
		{
			// Enl�ve un tag � la fin de la liste.
			System.Diagnostics.Debug.Assert(endTag.StartsWith("</"));
			System.Diagnostics.Debug.Assert(endTag.EndsWith(">"));

			endTag = endTag.Substring(2, endTag.Length-3);  // </b> -> b

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

		
		public bool AnalyseTagsAtOffset(int offset, out string[] tags)
		{
			// Parcourt le texte et accumule les informations sur les tags <>
			// reconnus.
			if ( offset < 0 || offset > this.MaxTextOffset )
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
				Tag tag = TextLayout.ParseTag(this.text, ref endOffset, out parameters);
				if ( tag == Tag.None )  continue;

				string sTag = this.text.Substring(beginOffset, endOffset-beginOffset);

				switch ( tag )
				{
					case Tag.Bold:
					case Tag.Italic:
					case Tag.Underline:
					case Tag.Mnemonic:
					case Tag.Wave:
					case Tag.Font:
					case Tag.Anchor:
						list.Add(sTag);
						break;

					case Tag.BoldEnd:
					case Tag.ItalicEnd:
					case Tag.UnderlineEnd:
					case Tag.MnemonicEnd:
					case Tag.WaveEnd:
					case Tag.FontEnd:
					case Tag.AnchorEnd:
						TextLayout.DeleteTagsList(sTag, list);
						break;
				}
			}

			tags = new string[list.Count];
			list.CopyTo(tags);
			return true;
		}
		
		
		public static char AnalyseEntityChar(string text, ref int offset)
		{
			// Retourne le caract�re � un offset quelconque, en interpr�tant les
			// commandes &...;
			if ( text[offset] == '&' )
			{
				int length = text.IndexOf(";", offset)-offset+1;
				
				if ( length < 3 )
				{
					throw new System.FormatException(string.Format("Invalid entity found (too short)."));
				}
				
				char code;
				string entity = text.Substring(offset, length);
				
				switch ( entity )
				{
					case "&lt;":    code = '<';   break;
					case "&gt;":    code = '>';   break;
					case "&amp;":   code = '&';   break;
					case "&quot;":  code = '"';   break;
					case "&apos;":  code = '\'';  break;
					
					default:
						if ( entity.StartsWith("&#") )
						{
							entity = entity.Substring(2, entity.Length-3);
							code   = (char)System.Int32.Parse(entity, System.Globalization.CultureInfo.InvariantCulture);
						}
						else
						{
							throw new System.FormatException(string.Format("Invalid entity {0} found.", entity));
						}
						break;
				}
				
				offset += length;
				
				return code;
			}
			
			return text[offset++];
		}
		
		public static Tag ParseTag(string text, ref int offset, out System.Collections.Hashtable parameters)
		{
			// Avance d'un caract�re ou d'un tag dans le texte.
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
					offset += length;
					
					switch ( tag )
					{
						case "<br/>":    return Tag.LineBreak;
						case "<b>":      return Tag.Bold;
						case "<i>":      return Tag.Italic;
						case "<u>":      return Tag.Underline;
						case "<m>":      return Tag.Mnemonic;
						case "<w>":      return Tag.Wave;
						
						case "</b>":     return Tag.BoldEnd;
						case "</i>":     return Tag.ItalicEnd;
						case "</u>":     return Tag.UnderlineEnd;
						case "</m>":     return Tag.MnemonicEnd;
						case "</w>":     return Tag.WaveEnd;
						
						case "</a>":     return Tag.AnchorEnd;
						case "</font>":  return Tag.FontEnd;
						case "</put>":   return Tag.PutEnd;
					}
					
					int space = tag.IndexOf(" ");
					
					if ( space > 0 )
					{
						string pfx = tag.Substring(0, space);
						string end = tag.Remove(0, space).TrimStart(' ');
						Tag tagId = Tag.Unknown;
						
						string close = ">";
						
						switch ( pfx )
						{
							case "<a":     tagId = Tag.Anchor;  close = ">";   break;
							case "<img":   tagId = Tag.Image;   close = "/>";  break;
							case "<font":  tagId = Tag.Font;    close = ">";   break;
							case "<w":     tagId = Tag.Wave;    close = ">";   break;
							case "<put":   tagId = Tag.Put;     close = ">";   break;
						}
						
						if ( !end.EndsWith(close) )
						{
							return Tag.SyntaxError;
						}
						
						// Enl�ve la fin du tag, comme �a on n'a r�ellement plus que les arguments.
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
				return Tag.EndOfText;
			}
			
			if (text[offset] == TextLayout.CodeLineBreak)
			{
				return Tag.LineBreak;
			}

			offset ++;
			return Tag.None;
		}
		
		public static char ExtractMnemonic(string text)
		{
			// Trouve la s�quence <m>x</m> dans le texte et retourne le premier caract�re
			// de x comme code mn�monique (en majuscules).
			System.Diagnostics.Debug.Assert(text != null);
			System.Collections.Hashtable parameters;

			int    offset = 0;
			while ( offset < text.Length )
			{
				Tag tag = TextLayout.ParseTag(text, ref offset, out parameters);
				if ( tag == Tag.Mnemonic )
				{
					return System.Char.ToUpper(text[offset]);
				}
			}
			
			return '\0';  // rien trouv�
		}
		
		public static string ConvertToTaggedText(string text)
		{
			return TextLayout.ConvertToTaggedText(text, false);
		}
		
		public static string ConvertToTaggedText(char c)
		{
			return TextLayout.ConvertToTaggedText(new string(c, 1), false);
		}
		
		public static string ConvertToTaggedText(string text, bool autoMnemonic)
		{
			// Convertit le texte simple en un texte compatible avec les tags. Supprime
			// toute occurrence de "<", "&" et ">" dans le texte.
			System.Diagnostics.Debug.Assert(text != null);
			if ( autoMnemonic )
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
						continue;
					}
					
					switch ( text[pos] )
					{
						case '&':   buffer.Append("&amp;");    break;
						case '<':   buffer.Append("&lt;");     break;
						case '>':   buffer.Append("&gt;");     break;
						case '\"':  buffer.Append("&quot;");   break;
						case '\'':  buffer.Append("&apos;");   break;
						case '\n':  buffer.Append("<br/>");    break;
						case '\r':                             break;
						case '\t':  buffer.Append(" ");        break;
						default:    buffer.Append(text[pos]);  break;
					}
				}
				
				return buffer.ToString();
			}
			else
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder();
				
				for ( int i=0 ; i<text.Length ; i++ )
				{
					char c = text[i];
					switch ( c )
					{
						case '&':   buffer.Append("&amp;");   break;
						case '<':   buffer.Append("&lt;");    break;
						case '>':   buffer.Append("&gt;");    break;
						case '\"':  buffer.Append("&quot;");  break;
						case '\'':  buffer.Append("&apos;");  break;
						case '\n':  buffer.Append("<br/>");   break;
						case '\r':                            break;
						case '\t':  buffer.Append(' ');       break;
						default:    buffer.Append(c);         break;
					}
				}
				
				return buffer.ToString();
			}
		}
		
		public static string ConvertToSimpleText(string text)
		{
			return TextLayout.ConvertToSimpleText(text, TextLayout.CodeObject.ToString ());
		}
		
		public static string ConvertToSimpleText(string text, string imageReplacement)
		{
			// Epure le texte en supprimant les tags <> et en rempla�ant les
			// tags &gt; et &lt; (et autres) par leurs caract�res �quivalents.
			// En plus, les images sont remplac�es par le texte 'imageReplacement'
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
					
						offset += length;

						if ( tag.IndexOf("<img ") == 0 )
						{
							buffer.Append(imageReplacement);
						}
						if ( tag.IndexOf("<br") == 0 )
						{
							buffer.Append("\n");
						}
					}
				}
				else if ( text[offset] == '&' )
				{
					buffer.Append(TextLayout.AnalyseEntityChar(text, ref offset));
				}
				else
				{
					buffer.Append(text[offset++]);
				}
			}

			return buffer.ToString();
		}
		
		protected void MarkContentsAsDirty()
		{
			this.isContentsDirty = true;
			this.isLayoutDirty = true;
		}
		
		protected void MarkLayoutAsDirty()
		{
			this.isLayoutDirty = true;
		}
		
		protected void UpdateLayout()
		{
			if ( this.layoutSize.Width > 0 )
			{
				if ( this.isContentsDirty )
				{
					this.GenerateTextBreaks();
					this.isContentsDirty = false;
				}
				
				if ( this.isLayoutDirty )
				{
					this.GenerateBlocks();
					this.GenerateJustification();
					this.isLayoutDirty = false;
				}
			}
		}

		public void Simplify()
		{
			// Simplifie et met � plat toutes les commandes HTML du texte.
			// Les commandes <put..>..</put> sont g�r�es puis supprim�es.
			System.Collections.Stack		fontStack;
			FontSimplify					fontItem;
			FontSimplify					fontCurrent;
			System.Text.StringBuilder		buffer;
			System.Collections.Hashtable	parameters;

			this.blocks.Clear();
			fontStack = new System.Collections.Stack();

			// Pr�pare la fonte initiale par d�faut.
			fontItem = new FontSimplify();
			fontItem.fontName  = "";
			fontItem.fontSize  = "";
			fontItem.fontColor = "";

			fontStack.Push(fontItem);  // push la fonte initiale (jamais de pop)
			fontCurrent = fontItem.Copy();

			SupplSimplify supplItem = new SupplSimplify();
			SupplSimplify supplCurrent = new SupplSimplify();
			buffer = new System.Text.StringBuilder();
			bool	fontCmd = false;
			int		offset = 0;
			int		textLength = this.MaxTextOffset;
			while ( offset <= textLength )
			{
				int begin = offset;
				Tag tag = TextLayout.ParseTag(this.text, ref offset, out parameters);
				if ( tag == Tag.EndOfText )  break;

				switch ( tag )
				{
					case Tag.Font:
						if ( parameters != null )
						{
							fontItem = ((FontSimplify)fontStack.Peek()).Copy();
							if ( parameters.ContainsKey("face")  )  fontItem.fontName  = (string)parameters["face"];
							if ( parameters.ContainsKey("size")  )  fontItem.fontSize  = (string)parameters["size"];
							if ( parameters.ContainsKey("color") )  fontItem.fontColor = (string)parameters["color"];
							fontStack.Push(fontItem);
						}
						break;

					case Tag.FontEnd:
						if ( fontStack.Count > 1 )
						{
							fontStack.Pop();
						}
						fontItem = (FontSimplify)fontStack.Peek();
						break;

					case Tag.Bold:
						supplItem.bold ++;
						break;
					case Tag.BoldEnd:
						supplItem.bold --;
						break;

					case Tag.Italic:
						supplItem.italic ++;
						break;
					case Tag.ItalicEnd:
						supplItem.italic --;
						break;

					case Tag.Underline:
						supplItem.underline ++;
						break;
					case Tag.UnderlineEnd:
						supplItem.underline --;
						break;

					case Tag.Mnemonic:
						supplItem.mnemonic ++;
						break;
					case Tag.MnemonicEnd:
						supplItem.mnemonic --;
						break;

					case Tag.Anchor:
						supplItem.anchor ++;
						supplItem.stringAnchor = "";
						if ( parameters != null && parameters.ContainsKey("href") )
						{
							supplItem.stringAnchor = (string)parameters["href"];
						}
						break;
					case Tag.AnchorEnd:
						supplItem.anchor --;
						break;

					case Tag.Wave:
						supplItem.wave ++;
						supplItem.waveColor = "";
						if ( parameters != null && parameters.ContainsKey("color") )
						{
							supplItem.waveColor = (string)parameters["color"];
						}
						break;
					case Tag.WaveEnd:
						supplItem.wave --;
						break;

					case Tag.Put:
						if ( parameters != null )
						{
							if ( parameters.ContainsKey("face")      )  supplItem.putFontName  = (string)parameters["face"];
							if ( parameters.ContainsKey("size")      )  supplItem.putFontSize  = (string)parameters["size"];
							if ( parameters.ContainsKey("color")     )  supplItem.putFontColor = (string)parameters["color"];
							if ( parameters.ContainsKey("bold")      )  supplItem.putBold      = (string)parameters["bold"];
							if ( parameters.ContainsKey("italic")    )  supplItem.putItalic    = (string)parameters["italic"];
							if ( parameters.ContainsKey("underline") )  supplItem.putUnderline = (string)parameters["underline"];
						}
						break;
					case Tag.PutEnd:
						supplItem.putFontName  = "";
						supplItem.putFontSize  = "";
						supplItem.putFontColor = "";
						supplItem.putBold      = "";
						supplItem.putItalic    = "";
						supplItem.putUnderline = "";
						break;

					case Tag.Image:
					case Tag.LineBreak:
					case Tag.None:
						this.SimplifyPutCommand(buffer, fontCurrent, fontItem, supplCurrent, supplItem, ref fontCmd);
						fontCurrent = fontItem.Copy();
						supplCurrent = supplItem.Copy();
						buffer.Append(this.text.Substring(begin, offset-begin));
						break;
				}
			}

			fontItem = (FontSimplify)fontStack.Peek();
			supplItem.bold   = 0;
			supplItem.italic = 0;
			this.SimplifyPutCommand(buffer, fontCurrent, fontItem, supplCurrent, supplItem, ref fontCmd);

			if ( fontCmd )
			{
				buffer.Append("</font>");
			}

			this.text = buffer.ToString();
		}

		protected void SimplifyPutCommand(System.Text.StringBuilder buffer,
										  FontSimplify fontCurrent, FontSimplify fontItem,
										  SupplSimplify supplCurrent, SupplSimplify supplItem,
										  ref bool fontCmd)
		{
			string itemFontName  = (supplItem.putFontName  != "") ? supplItem.putFontName  : fontItem.fontName;
			string itemFontSize  = (supplItem.putFontSize  != "") ? supplItem.putFontSize  : fontItem.fontSize;
			string itemFontColor = (supplItem.putFontColor != "") ? supplItem.putFontColor : fontItem.fontColor;

			string currFontName  = (supplCurrent.putFontName  != "") ? supplCurrent.putFontName  : fontCurrent.fontName;
			string currFontSize  = (supplCurrent.putFontSize  != "") ? supplCurrent.putFontSize  : fontCurrent.fontSize;
			string currFontColor = (supplCurrent.putFontColor != "") ? supplCurrent.putFontColor : fontCurrent.fontColor;

			if ( itemFontName  != currFontName  ||
				 itemFontSize  != currFontSize  ||
				 itemFontColor != currFontColor )
			{
				if ( fontCmd )
				{
					buffer.Append("</font>");
					fontCmd = false;
				}

				if ( itemFontName  != "" ||
					 itemFontSize  != "" ||
					 itemFontColor != "" )
				{
					buffer.Append("<font");

					if ( itemFontName != "" )
					{
						buffer.Append(" face=\"");
						buffer.Append(itemFontName);
						buffer.Append("\"");
					}

					if ( itemFontSize != "" )
					{
						buffer.Append(" size=\"");
						buffer.Append(itemFontSize);
						buffer.Append("\"");
					}

					if ( itemFontColor != "" )
					{
						buffer.Append(" color=\"");
						buffer.Append(itemFontColor);
						buffer.Append("\"");
					}

					buffer.Append(">");
					fontCmd = true;
				}
			}

			if ( supplItem.IsBold != supplCurrent.IsBold )
			{
				buffer.Append(supplItem.IsBold ? "<b>" : "</b>");
			}

			if ( supplItem.IsItalic != supplCurrent.IsItalic )
			{
				buffer.Append(supplItem.IsItalic ? "<i>" : "</i>");
			}

			if ( supplItem.IsUnderline != supplCurrent.IsUnderline )
			{
				buffer.Append(supplItem.IsUnderline ? "<u>" : "</u>");
			}

			if ( (supplItem.mnemonic != 0) != (supplCurrent.mnemonic != 0) )
			{
				buffer.Append((supplItem.mnemonic != 0) ? "<m>" : "</m>");
			}

			if ( (supplItem.anchor != 0) != (supplCurrent.anchor != 0) )
			{
				if ( supplItem.anchor != 0 )
				{
					if ( supplItem.stringAnchor == "" )
					{
						buffer.Append("<a>");
					}
					else
					{
						buffer.Append("<a href=\">");
						buffer.Append(supplItem.stringAnchor);
						buffer.Append("\"");
					}
				}
				else
				{
					buffer.Append("</a>");
				}
			}

			if ( (supplItem.wave != 0) != (supplCurrent.wave != 0) )
			{
				if ( supplItem.wave != 0 )
				{
					if ( supplItem.waveColor == "" )
					{
						buffer.Append("<w>");
					}
					else
					{
						buffer.Append("<w color=\">");
						buffer.Append(supplItem.waveColor);
						buffer.Append("\"");
					}
				}
				else
				{
					buffer.Append("</w>");
				}
			}
		}
		
		protected System.Collections.Stack CreateFontStack()
		{
			System.Collections.Stack stack = new System.Collections.Stack();
			FontItem font = new FontItem(this);
			
			font.fontName  = this.font.FaceName;
			font.fontSize  = this.fontSize;
			font.fontColor = Drawing.Color.Empty;
			
			stack.Push(font);  // push la fonte initiale (jamais de pop)
			
			return stack;
		}
		
		
		protected void FinishRun(System.Collections.ArrayList list, Drawing.TextBreak.Run run)
		{
			if ( run.Length > 0 )
			{
				list.Add(new Drawing.TextBreak.Run(run));
			}
			
			run.Reset();
		}
		
		protected void ProcessFontTag(System.Collections.Stack stack, System.Collections.Hashtable parameters)
		{
			if ( parameters != null )
			{
				FontItem font = stack.Peek() as FontItem;
				
				font = font.Copy();
				
				if ( parameters.ContainsKey("face") )
				{
					font.fontName = (string)parameters["face"];
				}
				if ( parameters.ContainsKey("size") )
				{
					string s = parameters["size"] as string;
								
					if ( s.EndsWith("%") )
					{
						font.fontSize = System.Double.Parse(s.Substring(0, s.Length-1), System.Globalization.CultureInfo.InvariantCulture) * this.fontSize / 100.0;
					}
					else
					{
						font.fontSize = System.Double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
					}
				}
				if ( parameters.ContainsKey("color") )
				{
					string s = parameters["color"] as string;
					Drawing.Color color = Drawing.Color.FromName(s);
					if ( !color.IsEmpty )  font.fontColor = color;
				}
				
				stack.Push(font);
			}
		}
		
		protected bool ProcessFormatTags(Tag tag, System.Collections.Stack fontStack, SupplItem supplItem, System.Collections.Hashtable parameters)
		{
			switch ( tag )
			{
				case Tag.Font:
					this.ProcessFontTag(fontStack, parameters);
					break;

				case Tag.FontEnd:
					if ( fontStack.Count > 1 )
					{
						fontStack.Pop();
					}
					break;

				case Tag.Bold:			supplItem.bold ++;		break;
				case Tag.BoldEnd:		supplItem.bold --;		break;

				case Tag.Italic:		supplItem.italic ++;	break;
				case Tag.ItalicEnd:		supplItem.italic --;	break;

				case Tag.Underline:
				case Tag.Mnemonic:		supplItem.underline ++;	break;
					
				case Tag.UnderlineEnd:
				case Tag.MnemonicEnd:	supplItem.underline --;	break;

				case Tag.Anchor:		supplItem.anchor ++;	supplItem.underline ++;		break;
				case Tag.AnchorEnd:		supplItem.anchor --;	supplItem.underline --;		break;

				case Tag.Wave:
					supplItem.wave ++;
					if ( parameters == null )
					{
						supplItem.waveColor = Drawing.Color.Empty;
					}
					else
					{
						if ( parameters.ContainsKey("color") )
						{
							string s = parameters["color"] as string;
							supplItem.waveColor = Drawing.Color.FromName(s);
						}
					}
					break;
					
				case Tag.WaveEnd:
					supplItem.wave --;
					break;

				default:
					return false;
			}
			
			return true;
		}

		
		protected void GenerateTextBreaks()
		{
			if ( this.MaxTextOffset == 0 )
			{
				this.textBreak = null;
				this.imageList = null;
				return;
			}
			
			//	Analyse le texte complet afin de g�n�rer une liste des "runs" pour TextBreak.
			//	Un "run" d�crit une suite de caract�re compos�s au moyen d'une m�me fonte.
			
			System.Collections.Stack		fontStack = this.CreateFontStack();
			System.Text.StringBuilder		buffer = new System.Text.StringBuilder();
			System.Collections.Hashtable	parameters;
			
			int       textLength = this.MaxTextOffset;
			SupplItem supplItem  = new SupplItem();
			
			int		beginOffset;
			int		endOffset = 0;
			int     fontIndex = -1;
			
			if ( this.imageList != null )
			{
				this.imageList.Clear();
			}
			
			System.Collections.ArrayList fontList  = new System.Collections.ArrayList();
			System.Collections.ArrayList runList   = new System.Collections.ArrayList();
			
			Drawing.TextBreak.Run run = new Drawing.TextBreak.Run();
			
			while ( endOffset <= textLength )
			{
				beginOffset = endOffset;
				
				Tag      tag      = TextLayout.ParseTag(this.text, ref endOffset, out parameters);
				FontItem fontItem = fontStack.Peek() as FontItem;
				
				if ( tag == Tag.EndOfText )  break;
				
				bool processedTag = this.ProcessFormatTags(tag, fontStack, supplItem, parameters);
				
				if ( tag != Tag.None || beginOffset == 0 )
				{
					Drawing.Font font = fontItem.RetFont(supplItem.bold>0, supplItem.italic>0);
					
					fontIndex = fontList.IndexOf(font);
					
					if ( fontIndex == -1 )
					{
						//	La fonte n'est pas encore connue. Il faut donc ins�rer la fonte dans la liste
						//	et prendre note du changement.
						
						fontIndex = fontList.Add(font);
					}
				}
				
				if ( fontIndex != run.FontId || fontItem.fontSize != run.FontScale )
				{
					this.FinishRun(runList, run);
					
					run.FontId    = fontIndex;
					run.FontScale = fontItem.fontSize;
				}
				
				if ( !processedTag )
				{
					switch ( tag )
					{
						case Tag.Image:
							System.Diagnostics.Debug.Assert( parameters != null && parameters.ContainsKey("src") );
							
							string imageName = parameters["src"] as string;
							
							if ( this.imageProvider == null )
							{
								throw new System.FormatException(string.Format("<img> tag for image '{0}' needs an Image Provider.", imageName));
							}
							
							Drawing.Image image = this.imageProvider.GetImage(imageName);
							
							if ( image == null )
							{
								throw new System.FormatException(string.Format("<img> tag references unknown image '{0}' while painting. Current directory is {1}.", imageName, System.IO.Directory.GetCurrentDirectory()));
							}
							
							//	Puisqu'on a trouv� l'image, on va la conserver; en effet, la recherche et la reconstruction
							//	de l'image est quelque chose de co�teux, et �a va nous servir plus tard pour l'affichage.
							
							if ( this.imageList == null )
							{
								this.imageList = new System.Collections.ArrayList();
							}
							
							this.imageList.Add(image);
							
							//	Astuce: on remplace l'image par un caract�re sp�cial [OBJ], dont on ne sp�cifie expr�s pas
							//	de fonte et dont la largeur correspond � la largeur de l'image :
							
							this.FinishRun(runList, run);
							this.FinishRun(runList, new Drawing.TextBreak.Run(1, -1, image.Width));
							buffer.Append(TextLayout.CodeObject);
							break;

						case Tag.LineBreak:
							buffer.Append(TextLayout.CodeLineBreak);	//	line separator
							run.Length++;
							break;

						case Tag.None:
							endOffset = beginOffset;
							char c = TextLayout.AnalyseEntityChar(this.text, ref endOffset);
							buffer.Append(c);
							run.Length++;
							break;
					}
				}
			}
			
			this.FinishRun(runList, run);
			
			//	Nous avons recueilli toute l'information n�cessaire pour initialiser
			//	TextBreak. Cette information ne changera que si le texte sous-jacent
			//	est modifi�.
			
			this.textBreak = new Drawing.TextBreak();
			this.textBreak.SetText(buffer.ToString(), this.breakMode);
			this.textBreak.SetFonts(fontList);
			this.textBreak.SetRuns(runList);
		}
		
		protected void GenerateBlocks()
		{
			// Met � jour this.blocks en fonction du texte, de la fonte et des dimensions.
			System.Collections.Stack		fontStack;
			FontItem						fontItem;
			System.Text.StringBuilder		buffer;
			System.Collections.Hashtable	parameters;

			this.blocks.Clear();
			fontStack = this.CreateFontStack();

			// Si le texte n'existe pas, met quand m�me un bloc vide,
			// afin de voir appara�tre le curseur (FindTextCursor).
			int textLength = this.MaxTextOffset;
noText:
			if ( textLength == 0 )
			{
				fontItem = (FontItem)fontStack.Peek();
				Drawing.Font blockFont = fontItem.RetFont(false, false);

				JustifBlock block = new JustifBlock();
				block.bol        = true;
				block.lineBreak  = false;
				block.text       = "";
				block.font       = blockFont;
				block.fontSize   = fontItem.fontSize;
				block.fontColor  = fontItem.fontColor;
				this.blocks.Add(block);
				return;
			}

			Drawing.TextBreak.Line[] lines = this.textBreak.GetLines(this.layoutSize.Width);
			
			if ( lines == null )
			{
				textLength = 0;
				goto noText;
			}
			
			int lineNumber = 0;
			int lineSkip   = 0;
			int sourceLength = 0;
			
			SupplItem supplItem = new SupplItem();
			buffer = new System.Text.StringBuilder();
			bool	bol = true;
			double	remainingWidth = this.layoutSize.Width;
			int		beginOffset;
			int		endOffset = 0;
			int		index = 0;
			int		textIndex = 0;
			int     imageIndex = 0;
			while ( endOffset <= textLength )
			{
				beginOffset = endOffset;
				Tag tag = TextLayout.ParseTag(this.text, ref endOffset, out parameters);
				bool autoLineBreak = false;
				char c = TextLayout.CodeNull;
				
				if ( tag == Tag.None )
				{
					if ( lineSkip <= lines[lineNumber].Skip )
					{
						if ( buffer.Length == 0 )  textIndex = index;
						endOffset = beginOffset;
						c = TextLayout.AnalyseEntityChar(this.text, ref endOffset);
					}
					else
					{
						System.Diagnostics.Debug.WriteLine("lineSkip error");
					}
				}
				else if ( tag == Tag.LineBreak )
				{
					if ( buffer.Length == 0 )  textIndex = index;
					lineNumber++;
					lineSkip = 0;
				}
				else if ( tag == Tag.Image )
				{
					if ( buffer.Length == 0 )  textIndex = index;
					c = TextLayout.CodeObject;
				}
				else if ( tag == Tag.EndOfText )
				{
					if ( buffer.Length == 0 )  textIndex = index;
				}
				
				if ( c != TextLayout.CodeNull )
				{
					if ( lineSkip == lines[lineNumber].Skip )
					{
						// Nous avons atteint la fin de la ligne courante. Il faut, par cons�quent, forcer
						// le passage � un nouveau bloc; d'abord, il faut copier les �ventuels caract�res
						// suppl�mentaires rajout�s automatiquement pour repr�senter la c�sure :
						
						string lineText = lines[lineNumber].Text;
						
						while ( lineSkip < lineText.Length )
						{
							buffer.Append(lineText[lineSkip++]);
						}
						
						lineNumber++;
						lineSkip = 1;
						autoLineBreak = true;
					}
					else
					{
						lineSkip++;
					}
				}
				
				if ( tag != Tag.None || autoLineBreak )
				{
					fontItem = (FontItem)fontStack.Peek();
					Drawing.Font blockFont = fontItem.RetFont(supplItem.bold>0, supplItem.italic>0);
					
					if ( (tag == Tag.LineBreak && buffer.Length == 0) ||
						 (tag == Tag.EndOfText && buffer.Length == 0 && bol) )
					{
						JustifBlock block = new JustifBlock();
						block.bol        = bol;
						block.lineBreak  = (tag == Tag.LineBreak);
						block.text       = "";
						block.beginIndex = textIndex;
						block.endIndex   = textIndex;
						block.DefineFont(blockFont, fontItem, supplItem);
						this.blocks.Add(block);
					}
					else
					{
						string text = buffer.ToString();
						
						if ( autoLineBreak )
						{
							sourceLength -= text.Length;
							text = text.TrimEnd(TextLayout.CodeSpace);
							sourceLength += text.Length;
						}
						
						JustifBlock block = new JustifBlock();
						block.bol        = bol;
						block.lineBreak  = tag == Tag.LineBreak;
						block.text       = text;
						block.beginIndex = textIndex;
						block.endIndex   = textIndex+sourceLength;
						block.width      = blockFont.GetTextAdvance(text)*fontItem.fontSize;
						block.DefineFont(blockFont, fontItem, supplItem);
						
						if ( this.justifMode == TextJustifMode.None )
						{
							block.infos     = null;
							block.infoWidth = 0;
							block.infoElast = 0;
						}
						else if ( block.text != "" )
						{
							block.font.GetTextClassInfos(block.text, out block.infos, out block.infoWidth, out block.infoElast);
						}
						
						if ( block.text != "" || block.lineBreak )
						{
							this.blocks.Add(block);
						}
						
						if ( autoLineBreak )
						{
							remainingWidth = this.layoutSize.Width;
							bol = true;
						}
						else if ( block.text != "" )
						{
							remainingWidth -= block.width;
							bol = false;
						}
						textIndex = index;
					}
					buffer.Length = 0;
					sourceLength = 0;
				}

				if ( tag == Tag.EndOfText )  break;

				if ( this.ProcessFormatTags(tag, fontStack, supplItem, parameters) == false )
				{
					switch ( tag )
					{
						case Tag.Image:
							if ( this.imageList != null && imageIndex < this.imageList.Count )
							{
								Drawing.Image image = this.imageList[imageIndex] as Drawing.Image;
								
								double dx = image.Width;
								double dy = image.Height;

								if ( dx > remainingWidth )
								{
									remainingWidth = this.layoutSize.Width;
									bol = true;
								}

								fontItem = (FontItem)fontStack.Peek();
								Drawing.Font blockFont = fontItem.RetFont(supplItem.bold>0, supplItem.italic>0);

								double fontAscender  = blockFont.Ascender;
								double fontDescender = blockFont.Descender;
								double fontHeight    = fontAscender-fontDescender;

								JustifBlock block = new JustifBlock();
								block.bol        = bol;
								block.image      = image;
								block.text       = "\ufffc";
								block.beginIndex = index;
								block.endIndex   = index+1;
								block.width      = dx;
								block.DefineFont(blockFont, fontItem, supplItem);
								
								if ( image.IsOriginDefined )
								{
									block.imageAscender  = image.Height - image.Origin.Y;
									block.imageDescender = -image.Origin.Y;
								}
								else
								{
									block.imageAscender  = dy*fontAscender/fontHeight;
									block.imageDescender = dy*fontDescender/fontHeight;
								}
								
								if ( this.justifMode != TextJustifMode.None )
								{
									double width = dx/fontItem.fontSize;
									block.infos = new Drawing.Font.ClassInfo[1];
									block.infos[0] = new Drawing.Font.ClassInfo(Drawing.Font.ClassId.PlainText, 1, width, 0.0);
									block.infoWidth = width;
									block.infoElast = 0.0;
								}
								
								this.blocks.Add(block);

								remainingWidth -= dx;
								bol = false;
								index ++;
							}
							break;

						case Tag.LineBreak:
							remainingWidth = this.layoutSize.Width;
							bol = true;
							index ++;
							break;

						case Tag.None:
							buffer.Append(c);
							sourceLength ++;
							index ++;
							break;
					}
				}
			}
		}


#if false
		protected void PurgeAdditionalSpace()
		{
			// Supprime l'�ventuel espace en trop � la fin de l'avant-dernier bloc
			// ins�r�. Dans l'exemple ci-dessous, les espaces sont repr�sent�s par
			// des soulign�s :
			//   |Petit_texte_    |
			//   |<b>ridicule</b> |
			// L'espace apr�s "texte" doit �tre supprim�, car il n'a pas pu �tre mang�
			// par TextBreak, � cause de la commande <b> qui a forc� 2 blocs distincts.
			if ( this.blocks.Count < 2 )  return;

			JustifBlock prevBlock = (JustifBlock)this.blocks[this.blocks.Count-2];
			JustifBlock lastBlock = (JustifBlock)this.blocks[this.blocks.Count-1];
			if ( prevBlock.infos == null )  return;
			if ( !lastBlock.bol )  return;
			if ( prevBlock.endIndex != lastBlock.beginIndex )  return;
			if ( prevBlock.endIndex == prevBlock.beginIndex )  return;
			if ( prevBlock.text[prevBlock.endIndex-prevBlock.beginIndex-1] != ' ' )  return;
			prevBlock.text = prevBlock.text.Substring(0, prevBlock.endIndex-prevBlock.beginIndex-1);
			prevBlock.endIndex --;
			prevBlock.font.GetTextClassInfos(prevBlock.text, out prevBlock.infos, out prevBlock.infoWidth, out prevBlock.infoElast);
		}
#endif

		protected void GenerateJustification()
		{
			// Met � jour this.lines en fonction de this.blocks.
			// D�termine la position des blocs en fonction de l'alignement.
			// D�termine �galement quels sont les blocs et les lignes visibles.
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
					if ( block.IsImage )
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
					if ( block.bol )  break;  // break si d�but nouvelle ligne
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

				pos.Y -= ascender;  // sur la ligne de base
				pos.X = 0;

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

				bool justif = false;
				JustifBlock lastBlock = (JustifBlock)this.blocks[j-1];
				bool lastLine = lastBlock.lineBreak;
				if ( j-1 >= this.blocks.Count-1 )  lastLine = true;
				if ( (this.justifMode == TextJustifMode.AllButLast && !lastLine) ||
					 this.justifMode == TextJustifMode.All )
				{
					double w = 0;
					double e = 0;
					for ( int k=i ; k<j ; k++ )
					{
						block = (JustifBlock)this.blocks[k];
						w += block.infoWidth * block.fontSize;
						e += block.infoElast * block.fontSize;
					}
					double delta = this.layoutSize.Width-w;

					if ( e > 0 && delta != 0 )
					{
						for ( int k=i ; k<j ; k++ )
						{
							block = (JustifBlock)this.blocks[k];

							for ( int ii=0 ; ii<block.infos.Length ; ii++ )
							{
								double widthRatio = block.infos[ii].Width/delta;
								double elastRatio = block.infos[ii].Elasticity/e;
								block.infos[ii].Scale = 1.0+elastRatio/widthRatio;
							}
							block.pos = pos;
							block.visible = visible;

							if ( block.IsImage )
							{
								pos.X += block.width;
							}
							else
							{
								double[] charsWidth;
								block.font.GetTextCharEndX(block.text, block.infos, out charsWidth);
								pos.X += charsWidth[block.text.Length-1] * block.fontSize;
							}
						}
						justif = true;
					}

				}

				if ( !justif )
				{
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

					line.pos = pos;

					for ( int k=i ; k<j ; k++ )
					{
						block = (JustifBlock)this.blocks[k];
						block.pos = pos;
						block.visible = visible;
						block.infos = null;
						pos.X += block.width;
					}
				}

				pos.Y += ascender;
				pos.Y -= height;  // position haut de la ligne suivante

				i = j;  // index d�but ligne suivante
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

		
		public void DebugDumpJustif(System.IO.TextWriter stream)
		{
			// Affiche le contenu du tableau this.blocks, pour le debug.
			this.UpdateLayout();
			stream.WriteLine("Total blocks = " + this.blocks.Count);
			foreach ( JustifBlock block in this.blocks )
			{
				string text = string.Format
				(
					"bol={0},  br={1},  font={2} {3},  pos={4};{5},  width={6},  index={7};{8},  text=\"{9}\"",
					block.bol, block.lineBreak,
					block.font.FullName, block.fontSize.ToString("F2"),
					block.pos.X.ToString("F2"), block.pos.Y.ToString("F2"),
					block.width.ToString("F2"),
					block.beginIndex, block.endIndex,
					block.text
				);
				stream.WriteLine(text);
			}
		}


		public static bool CheckSyntax(string text, out int offsetError)
		{
			// V�rifie la syntaxe d'un texte.
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
				
					string entity = text.Substring(endOffset, length);
					switch ( entity )
					{
						case "&amp;":   break;
						case "&lt;":    break;
						case "&gt;":    break;
						case "&quot;":  break;
						case "&apos;":  break;
					
						default:
							if ( entity.StartsWith("&#") )
							{
								for ( int i=2 ; i<entity.Length-1 ; i++ )
								{
									char c = entity[i];
									
									if ( c < '0' || c > '9' )
									{
										offsetError = beginOffset;
										return false;
									}
								}
							}
							else
							{
								offsetError = beginOffset;
								return false;
							}
							break;
					}
				
					endOffset += length;
					continue;
				}

				Tag tag = TextLayout.ParseTag(text, ref endOffset, out parameters);
				if ( tag == Tag.None )  continue;

				if ( tag == Tag.SyntaxError ||
					 tag == Tag.Unknown     )
				{
					offsetError = beginOffset;
					return false;
				}

				string sTag = text.Substring(beginOffset, endOffset-beginOffset);

				switch ( tag )
				{
					case Tag.Bold:
					case Tag.Italic:
					case Tag.Underline:
					case Tag.Mnemonic:
					case Tag.Wave:
					case Tag.Font:
					case Tag.Anchor:
						list.Add(sTag);
						break;

					case Tag.BoldEnd:
					case Tag.ItalicEnd:
					case Tag.UnderlineEnd:
					case Tag.MnemonicEnd:
					case Tag.WaveEnd:
					case Tag.FontEnd:
					case Tag.AnchorEnd:
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
			None,							// pas un tag
			Unknown,						// tag pas reconnu
			SyntaxError,					// syntaxe du tag pas correcte
			EndOfText,						// fin du texte
			
			LineBreak,						// <br/>
			
			Bold,		BoldEnd,			// <b>...</b>
			Italic,		ItalicEnd,			// <i>...</i>
			Underline,	UnderlineEnd,		// <u>...</u>
			Mnemonic,	MnemonicEnd,		// <m>...</m>  --> comme <u>...</u>
			Wave,		WaveEnd,			// <w>...</w> ou <w color="#FF00FF">
			Font,		FontEnd,			// <font ...>...</font>
			Anchor,		AnchorEnd,			// <a href="x">...</a>
			Image,							// <img src="x"/>

			Put,		PutEnd,				// <put ...>...</put>
		}
		
		
		// Fonte servant � refl�ter simplifier les commandes HTML rencontr�es.
		// Un stack de FontSimplify est cr��.
		protected class FontSimplify
		{
			public FontSimplify Copy()
			{
				return this.MemberwiseClone() as FontSimplify;
			}

			
			public string		fontName;
			public string		fontSize;
			public string		fontColor;
		}

		protected class SupplSimplify
		{
			public SupplSimplify Copy()
			{
				return this.MemberwiseClone() as SupplSimplify;
			}

			
			public bool IsBold
			{
				get
				{
					if ( this.putBold == "yes" )  return true;
					if ( this.putBold == "no"  )  return false;
					return (this.bold > 0);
				}
			}

			public bool IsItalic
			{
				get
				{
					if ( this.putItalic == "yes" )  return true;
					if ( this.putItalic == "no"  )  return false;
					return (this.italic > 0);
				}
			}

			public bool IsUnderline
			{
				get
				{
					if ( this.putUnderline == "yes" )  return true;
					if ( this.putUnderline == "no"  )  return false;
					return (this.underline > 0);
				}
			}

			
			public int		bold         = 0;	// gras si > 0
			public int		italic       = 0;	// italique si > 0
			public int		underline    = 0;	// soulign� si > 0
			public int		mnemonic     = 0;	// soulign� si > 0
			public int		anchor       = 0;	// lien si > 0
			public string	stringAnchor = "";
			public int		wave         = 0;	// vague si > 0
			public string	waveColor    = "";
			public string	putFontName  = "";
			public string	putFontSize  = "";
			public string	putFontColor = "";
			public string	putBold      = "";
			public string	putItalic    = "";
			public string	putUnderline = "";
		}

		// Fonte servant � refl�ter les commandes HTML rencontr�es.
		// Un stack de FontItem est cr��.
		protected class FontItem
		{
			public FontItem(TextLayout host)
			{
				this.host = host;
			}
			
			public FontItem Copy()
			{
				return this.MemberwiseClone() as FontItem;
			}

			public Drawing.Font RetFont(bool bold, bool italic)
			{
				Drawing.Font	font;
				string			fontStyle;

				if ( bold && !italic )
				{
					fontStyle = "Bold";
				}
				else if ( !bold && italic )
				{
					fontStyle = "Italic";
				}
				else if ( bold && italic )
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
				
				if ( font == null )
				{
					font = this.host.font;
				}
				
				return font;
			}

			
			protected TextLayout	host;
			
			public string			fontName;
			public double			fontSize;
			public Drawing.Color	fontColor;
		}

		protected class SupplItem
		{
			public int				bold      = 0;	// gras si > 0
			public int				italic    = 0;	// italique si > 0
			public int				underline = 0;	// soulign� si > 0
			public int				anchor    = 0;	// lien si > 0
			public int				wave      = 0;	// vague si > 0
			public Drawing.Color	waveColor = Drawing.Color.Empty;
		}

		// Descripteur d'un bloc de texte. Tous les caract�res du bloc ont
		// la m�me fonte, m�me taille et m�me couleur.
		protected class JustifBlock
		{
			public JustifBlock()
			{
			}
			
			
			public bool						IsImage
			{
				get
				{
					return this.image != null;
				}
			}

			
			public void DefineFont(Drawing.Font blockFont, FontItem fontItem, SupplItem supplItem)
			{
				this.font       = blockFont;
				this.fontSize   = fontItem.fontSize;
				this.fontColor  = fontItem.fontColor;
				this.bold       = supplItem.bold > 0;
				this.italic     = supplItem.italic > 0;
				this.underline  = supplItem.underline > 0;
				this.anchor     = supplItem.anchor > 0;
				this.wave       = supplItem.wave > 0;
				this.waveColor  = supplItem.waveColor;
			}
			
			
			public bool						bol;		// begin of line
			public bool						lineBreak;	// ending with br
			public string					text;
			public int						beginIndex;
			public int						endIndex;
			public int						indexLine;	// index dans this.lines
			public Drawing.Font				font;
			public double					fontSize;
			public Drawing.Color			fontColor = Drawing.Color.Empty;
			public bool						bold;
			public bool						italic;
			public bool						underline;
			public bool						anchor;
			public bool						wave;
			public Drawing.Color			waveColor = Drawing.Color.Empty;
			public double					width;		// largeur du bloc
			public Drawing.Point			pos;		// sur la ligne de base
			public Drawing.Image			image;		// image bitmap
			public double					imageAscender;
			public double					imageDescender;
			public bool						visible;
			public Drawing.Font.ClassInfo[]	infos;
			public double					infoWidth;
			public double					infoElast;
		}

		// Descripteur d'une ligne de texte. Une ligne est compos�e
		// d'un ou plusieurs blocs.
		protected class JustifLine
		{
			public int						firstBlock;	// index du premier bloc
			public int						lastBlock;	// index du dernier bloc
			public int						rank;		// rang de la ligne (0..n)
			public Drawing.Point			pos;		// position sur la ligne de base
			public double					width;		// largeur occup�e par la ligne
			public double					height;		// interligne
			public double					ascender;	// hauteur en-dessus de la ligne de base (+)
			public double					descender;	// hauteur en-dessous de la ligne de base (-)
			public bool						visible;
		}

		public class Context
		{
			public int CursorFrom
			{
				get
				{
					return this.cursorFrom;
				}

				set
				{
					this.cursorFrom = value;
					this.PrepareOffset = -1;  // annule la pr�paration pour l'insertion
				}
			}

			public int CursorTo
			{
				get
				{
					return this.cursorTo;
				}

				set
				{
					this.cursorTo = value;
					this.PrepareOffset = -1;  // annule la pr�paration pour l'insertion
				}
			}

			protected int					cursorFrom     = 0;
			protected int					cursorTo       = 0;
			public bool						CursorAfter    = false;
			public int						CursorLine     = 0;
			public double					CursorPosX     = 0;
			public int						PrepareOffset  = -1;
			public int						PrepareLength1 = 0;
			public int						PrepareLength2 = 0;
			public int						MaxChar        = 1000;
		}

		public class SelectedArea
		{
			public Drawing.Rectangle		Rect;
			public Drawing.Color			Color;
		}
		
		public event AnchorEventHandler			Anchor;
		
		protected bool							isContentsDirty;
		protected bool							isLayoutDirty;
		protected string						text;
		protected Drawing.Font					font			= Drawing.Font.DefaultFont;
		protected double						fontSize		= Drawing.Font.DefaultFontSize;
		protected Drawing.Size					layoutSize;
		protected Drawing.TextBreakMode			breakMode		= Drawing.TextBreakMode.Split;
		protected TextJustifMode				justifMode		= TextJustifMode.None;
		protected int							totalLine;
		protected int							visibleLine;
		protected Drawing.IImageProvider		imageProvider	= Support.ImageProvider.Default;
		protected Drawing.ContentAlignment		alignment		= Drawing.ContentAlignment.TopLeft;
		protected System.Collections.ArrayList	blocks			= new System.Collections.ArrayList();
		protected System.Collections.ArrayList	lines			= new System.Collections.ArrayList();
		protected static Drawing.Color			defaultColor	= new Drawing.Color(0,0,0);
		protected static Drawing.Color			anchorColor		= new Drawing.Color(0,0,1);
		protected static Drawing.Color			waveColor		= new Drawing.Color(1,0,0);
		protected bool							showLineBreak	= false;
		protected Drawing.TextBreak				textBreak;
		protected System.Collections.ArrayList	imageList;
		
		public const double						Infinite		= 1000000;
		
		public const char						CodeNull		= '\u0000';
		public const char						CodeSpace		= '\u0020';
		public const char						CodeObject		= '\ufffc';
		public const char						CodeLineBreak	= '\u2028';
	}
}
