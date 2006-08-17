namespace Epsitec.Common.Widgets
{
	#region AnchorEvent Definitions
	public class AnchorEventArgs : Support.EventArgs
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
	#endregion


	/// <summary>
	/// La classe TextLayout permet de stocker et d'afficher des contenus
	/// riches (un sous-ensemble très restreint de HTML).
	/// </summary>
	public class TextLayout
	{
		public TextLayout()
		{
			this.style = Drawing.TextStyle.Default;
			this.drawingScale = 1.0;
			this.verticalMark = double.NaN;
		}
		
		public TextLayout(TextLayout model) : this()
		{
			if ( model != null )
			{
				this.ResourceManager = model.ResourceManager;
				this.style           = model.style;
				this.DrawingScale    = model.DrawingScale;
				this.VerticalMark    = model.VerticalMark;
				this.LayoutSize      = model.LayoutSize;
				this.Alignment       = model.Alignment;
			}
		}
		
		public TextLayout(Support.ResourceManager resource_manager) : this ()
		{
			this.resourceManager = resource_manager;
		}

		internal Widget							Embedder
		{
			set
			{
				this.embedder = value;
			}
		}

		public string							Text
		{
			//	Texte associé, contenant des commandes HTML, mais sans les commandes <put>.
			get
			{
				if ( this.text == null )
				{
					return "";
				}
				
				if ( this.isPrepareDirty )  // contient des commandes <put> ?
				{
					return this.GetSimplify();
				}

				return this.text;
			}

			set
			{
				if ( value == null )
				{
					value = "";
				}
				
				if ((this.text != value) &&
					(this.SimplifiedText != value))
				{
					int offsetError;
					if ( TextLayout.CheckSyntax(value, out offsetError) )
					{
						this.SetText (value);
						this.MarkContentsAsDirty();
					}
					else
					{
						throw new System.FormatException(string.Format ("Syntax error at char {0}.", offsetError.ToString()));
					}
				}
			}
		}

		private string SimplifiedText
		{
			get
			{
				string text = this.text ?? "";
				return text.Contains ("<put ") ? this.GetSimplify () : text;
			}
		}

		public string							InternalText
		{
			//	Texte associé, contenant des commandes HTML, y compris les commandes <put>.
			get
			{
				if ( this.text == null )
				{
					return "";
				}
				
				return this.text;
			}
		}
		
		public string							InternalSimpleText
		{
			get
			{
				this.UpdateLayout();
				return this.simpleText;
			}
		}
		
		public int								MaxTextOffset			// offset maximum (position "physique" dans le texte brut)
		{
			get { return (this.text == null) ? 0 : this.text.Length; }
		}
		
		public int								MaxTextIndex			// index maximum (position "logique", indépendante des tags de formatage)
		{
			get { return this.FindIndexFromOffset(this.MaxTextOffset); }
		}
		
		
		public Support.ResourceManager			ResourceManager
		{
			get
			{
				if (this.resourceManager == null)
				{
					return Support.Resources.DefaultManager;
				}
				
				return this.resourceManager;
			}
			set
			{
				this.resourceManager = value;
			}
		}
		
		public Drawing.TextStyle				Style
		{
			get
			{
				return this.style;
			}
		}

		public Drawing.Font						DefaultFont
		{
			//	Fonte par défaut.
			get
			{
				return this.style.Font;
			}
			set
			{
				if ( this.DefaultFont != value )
				{
					this.CloneStyleIfDefaultStyleInUse();
					this.style.Font = value;
				}
			}
		}
		
		public double							DefaultFontSize
		{
			//	Taille de la fonte par défaut.
			get
			{
				return this.style.Size;
			}
			set
			{
				if ( this.DefaultFontSize != value )
				{
					this.CloneStyleIfDefaultStyleInUse();
					this.style.Size = value;
				}
			}
		}

		public Drawing.RichColor				DefaultRichColor
		{
			get
			{
				return this.style.RichColor;
			}
			set
			{
				if ( this.DefaultRichColor != value )
				{
					this.CloneStyleIfDefaultStyleInUse();
					this.style.RichColor = value;
				}
			}
		}

		public Drawing.Color					DefaultColor
		{
			get
			{
				return this.style.Color;
			}
			set
			{
				if ( this.DefaultColor != value )
				{
					this.CloneStyleIfDefaultStyleInUse();
					this.style.Color = value;
				}
			}
		}

		public Drawing.Color					AnchorColor
		{
			//	Couleur pour les liens.
			get
			{
				return this.style.AnchorColor;
			}
			set
			{
				if ( this.AnchorColor != value )
				{
					this.CloneStyleIfDefaultStyleInUse();
					this.style.AnchorColor = value;
				}
			}
		}

		public Drawing.Color					WaveColor
		{
			//	Couleur pour les vagues.
			get
			{
				return this.style.WaveColor;
			}
			set
			{
				if ( this.WaveColor != value )
				{
					this.CloneStyleIfDefaultStyleInUse();
					this.style.WaveColor = value;
				}
			}
		}

		public Drawing.ContentAlignment			Alignment
		{
			//	Alignement du texte dans le rectangle.
			get
			{
				return this.style.Alignment;
			}
			set
			{
				if ( this.Alignment != value )
				{
					this.CloneStyleIfDefaultStyleInUse();
					this.style.Alignment = value;
				}
			}
		}

		public Drawing.TextBreakMode			BreakMode
		{
			//	Mode de césure.
			get
			{
				return this.style.BreakMode;
			}
			set
			{
				if ( this.BreakMode != value )
				{
					this.CloneStyleIfDefaultStyleInUse();
					this.style.BreakMode = value;
				}
			}
		}

		public Drawing.TextJustifMode			JustifMode
		{
			//	Mode de justification.
			get
			{
				return this.style.JustifMode;
			}
			set
			{
				if ( this.JustifMode != value )
				{
					this.CloneStyleIfDefaultStyleInUse();
					this.style.JustifMode = value;
				}
			}
		}

		public bool								ShowLineBreak
		{
			//	Détermine si les <br/> sont visibles ou non.
			get
			{
				return this.style.ShowLineBreak;
			}
			set
			{
				if ( this.ShowLineBreak != value )
				{
					this.CloneStyleIfDefaultStyleInUse();
					this.style.ShowLineBreak = value;
				}
			}
		}
		
		public bool								ShowTab
		{
			//	Détermine si les tabulateurs sont visibles ou non.
			get
			{
				return this.style.ShowTab;
			}
			set
			{
				if ( this.ShowTab != value )
				{
					this.CloneStyleIfDefaultStyleInUse();
					this.style.ShowTab = value;
				}
			}
		}
		
		public double							DrawingScale
		{
			//	Détermine l'échelle de dessin.
			get
			{
				return this.drawingScale;
			}
			set
			{
				if ( this.drawingScale != value )
				{
					this.drawingScale = value;
				}
			}
		}
		
		public double							VerticalMark
		{
			//	Détermine la position du marqueur vertical.
			get
			{
				return this.verticalMark;
			}
			set
			{
				if ( this.verticalMark != value )
				{
					this.verticalMark = value;
				}
			}
		}
		

		public int TabInsert(Drawing.TextStyle.Tab tab)
		{
			this.CloneStyleIfDefaultStyleInUse();
			return this.style.TabInsert(tab);
		}

		public int TabCount
		{
			get
			{
				return this.style.TabCount;
			}
		}

		public void TabRemoveAt(int rank)
		{
			this.CloneStyleIfDefaultStyleInUse();
			this.style.TabRemoveAt(rank);
		}

		public Drawing.TextStyle.Tab GetTab(int rank)
		{
			return this.style.GetTab(rank);
		}

		public void SetTabPosition(int rank, double pos)
		{
			this.CloneStyleIfDefaultStyleInUse();
			this.style.SetTabPosition(rank, pos);
		}

		
		public Drawing.Size						LayoutSize
		{
	//		public Drawing.IImageProvider			ImageProvider
	//		{
	//			// Gestionnaire d'images.
	//			get
	//			{
	//				return this.style.ImageProvider;
	//			}
	//			set
	//			{
	//				if ( this.ImageProvider != value )
	//				{
	//					this.CloneStyleIfDefaultStyleInUse();
	//					this.style.ImageProvider = value;
	//				}
	//			}
	//		}
	
			//	Dimensions du rectangle.
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
			//	Retourne les dimensions du texte indépendament de LayoutSize,
			//	s'il est mis sur une seule ligne.
			get
			{
				double ascender;
				double descender;

				Drawing.Point origin;
				Drawing.Size size = new Drawing.Size (TextLayout.Infinite, TextLayout.Infinite);
				
				this.GetSingleLineGeometry (size, Drawing.ContentAlignment.TopLeft, out ascender, out descender, out origin, out size);

				return size;
			}
		}

		public void GetSingleLineGeometry(Drawing.Size boxSize, Drawing.ContentAlignment alignment, out double ascender, out double descender, out Drawing.Point origin, out Drawing.Size size)
		{
			Drawing.Size originalSize = this.LayoutSize;
			Drawing.ContentAlignment originalAlignment = this.Alignment;

			this.LayoutSize = boxSize;
			this.Alignment  = alignment;

			Drawing.Rectangle end = this.StandardRectangle;

			double width;
			
			this.GetLineGeometry (0, out origin, out ascender, out descender, out width);

			this.LayoutSize = originalSize;
			this.Alignment  = originalAlignment;

			size = end.Size;
		}
		
		public int								TotalLineCount
		{
			//	Retourne le nombre de lignes total dans le layout courant
			//	(y compris les lignes qui débordent).
			get
			{
				this.UpdateLayout();
				return this.totalLine;
			}
		}
		
		public int								VisibleLineCount
		{
			//	Retourne le nombre de lignes visibles dans le layout courant
			//	(sans compter les lignes qui débordent).
			get
			{
				this.UpdateLayout();
				return this.visibleLine;
			}
		}
		
		public Drawing.Rectangle				TotalRectangle
		{
			//	Retourne le rectangle englobant du layout courant; ce
			//	rectangle comprend toutes les lignes, même celles qui débordent.
			get { return this.GetRectangleBounds(true); }
		}
		
		public Drawing.Rectangle				VisibleRectangle
		{
			//	Retourne le rectangle englobant du layout courant; ce
			//	rectangle comprend uniquement les lignes visibles.
			get { return this.GetRectangleBounds(false); }
		}
		
		public Drawing.Rectangle				StandardRectangle
		{
			//	Retourne le rectangle standard englobant du layout courant; ce
			//	rectangle ne dépend pas de la hauteur des lettres du texte.
			//	Le rectangle aura la même hauteur avec "ace" ou "Ap".
			get
			{
				this.UpdateLayout();

				Drawing.Rectangle totalRect = Drawing.Rectangle.Empty;
				foreach ( JustifBlock block in this.blocks )
				{
					if ( !block.Visible )  continue;

					Drawing.Rectangle blockRect = new Drawing.Rectangle();
					blockRect.Left  = 0;
					blockRect.Right = block.Width;
					if ( block.IsImage )
					{
						blockRect.Top    = block.ImageAscender;
						blockRect.Bottom = block.ImageDescender;
					}
					else
					{
						blockRect.Top    = block.FontSize*block.Font.Ascender;
						blockRect.Bottom = block.FontSize*block.Font.Descender;
					}
					blockRect.Offset(block.Pos.X, block.Pos.Y);
					totalRect.MergeWith(blockRect);
				}
				return totalRect;
			}
		}
		

		protected Drawing.Rectangle GetRectangleBounds(bool all)
		{
			//	Retourne le rectangle englobant du layout, en tenant compte de
			//	toutes les lignes (all=true) ou seulement des lignes visibles (all=false).
			//	Si le texte est aligné sur le bord gauche, rectangle.Left n'est pas
			//	forcément égal à 0.
			this.UpdateLayout();

			Drawing.Rectangle totalRect = Drawing.Rectangle.Empty;
			foreach ( JustifBlock block in this.blocks )
			{
				if ( !all && !block.Visible )  continue;

				Drawing.Rectangle blockRect;
				if ( block.IsImage )
				{
					blockRect = new Drawing.Rectangle();
					blockRect.Left   = 0;
					blockRect.Right  = block.Width;
					blockRect.Top    = block.ImageAscender;
					blockRect.Bottom = block.ImageDescender;
				}
				else
				{
					blockRect = block.Font.GetTextBounds(block.Text);
					blockRect.Scale(block.FontSize);
				}
				blockRect.Offset(block.Pos.X, block.Pos.Y);
				totalRect.MergeWith(blockRect);
			}
			return totalRect;
		}


		internal bool IsSelectionBold(TextLayout.Context context)
		{
			//	Indique si les caractères sélectionnés sont gras.
			if ( context.PrepareOffset != -1 )  // préparation pour l'insertion ?
			{
				return this.IsPrepared(context, "bold");
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context);
				if ( block == null )  return false;
				return block.Bold;
				//?int i = block.Font.StyleName.IndexOf("Bold");
				//?return ( i >= 0 );
			}
		}

		internal void SetSelectionBold(TextLayout.Context context, bool bold)
		{
			//	Met en gras ou en normal tous les caractères sélectionnés.
			string state = bold ? "yes" : "no";
			this.InsertPutCommand(context, "bold=\"" + state + "\"");
		}

		internal bool IsSelectionItalic(TextLayout.Context context)
		{
			//	Indique si les caractères sélectionnés sont italiques.
			if ( context.PrepareOffset != -1 )  // préparation pour l'insertion ?
			{
				return this.IsPrepared(context, "italic");
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context);
				if ( block == null )  return false;
				return block.Italic;
				//?int i = block.Font.StyleName.IndexOf("Italic");
				//?int j = block.Font.StyleName.IndexOf("Oblique");
				//?return ( i >= 0 || j >= 0 );
			}
		}

		internal void SetSelectionItalic(TextLayout.Context context, bool italic)
		{
			//	Met en italique ou en normal tous les caractères sélectionnés.
			string state = italic ? "yes" : "no";
			this.InsertPutCommand(context, "italic=\"" + state + "\"");
		}

		internal bool IsSelectionUnderlined(TextLayout.Context context)
		{
			//	Indique si les caractères sélectionnés sont soulignés.
			if ( context.PrepareOffset != -1 )  // préparation pour l'insertion ?
			{
				return this.IsPrepared(context, "underlined");
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context);
				if ( block == null )  return false;
				return block.Underlined;
			}
		}

		internal void SetSelectionUnderlined(TextLayout.Context context, bool underlined)
		{
			//	Met en souligné ou en normal tous les caractères sélectionnés.
			string state = underlined ? "yes" : "no";
			this.InsertPutCommand(context, "underlined=\"" + state + "\"");
		}

		internal string GetSelectionFontName(TextLayout.Context context)
		{
			//	Indique le nom de la fonte des caractères sélectionnés.
			//	Une chaîne vide indique la fonte par défaut.
			if ( context.PrepareOffset != -1 )  // préparation pour l'insertion ?
			{
				string s = this.SearchPrepared(context, "face");
				if ( s == "" )  return this.DefaultFont.FaceName;
				return s;
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context);
				if ( block == null )  return this.DefaultFont.FaceName;
				if ( block.IsDefaultFontName )
				{
					return "";
				}
				return block.FontName;
			}
		}

		internal void SetSelectionFontName(TextLayout.Context context, string name)
		{
			//	Modifie le nom de la fonte des caractères sélectionnés.
			//	Une chaîne vide indique la fonte par défaut.
			if ( name == "" )  name = TextLayout.CodeDefault + this.DefaultFont.FaceName;
			this.InsertPutCommand(context, "face=\"" + name + "\"");
		}

		internal double GetSelectionFontScale(TextLayout.Context context)
		{
			//	Indique l'échelle des caractères sélectionnés.
			//	1 indique l'échelle par défaut.
			if ( context.PrepareOffset != -1 )  // préparation pour l'insertion ?
			{
				string s = this.SearchPrepared(context, "size");
				if ( s == "" )  return 1;
				return this.ParseScale(s);
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context);
				if ( block == null )  return 1;
				return block.FontScale;
			}
		}

		internal void SetSelectionFontScale(TextLayout.Context context, double scale)
		{
			//	Modifie l'échelle des caractères sélectionnés.
			//	1 indique l'échelle par défaut.
			scale = System.Math.Max(scale,   0.01);  // min 1%
			scale = System.Math.Min(scale, 100.00);  // max 10000%
			this.InsertPutCommand(context, "size=\"" + scale*100.0 + "%\"");
		}

		internal Drawing.RichColor GetSelectionFontRichColor(TextLayout.Context context)
		{
			//	Indique la couleur des caractères sélectionnés.
			//	Une couleur vide indique la couleur par défaut.
			if ( context.PrepareOffset != -1 )  // préparation pour l'insertion ?
			{
				string s = this.SearchPrepared(context, "color");
				return Drawing.RichColor.FromName(s);
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context);
				if ( block == null )  return Drawing.RichColor.Empty;
				return block.FontColor;
			}
		}
		
		internal Drawing.Color GetSelectionFontColor(TextLayout.Context context)
		{
			//	Indique la couleur des caractères sélectionnés.
			//	Une couleur vide indique la couleur par défaut.
			if ( context.PrepareOffset != -1 )  // préparation pour l'insertion ?
			{
				string s = this.SearchPrepared(context, "color");
				return Drawing.Color.FromName(s);
			}
			else
			{
				JustifBlock block = this.SearchJustifBlock(context);
				if ( block == null )  return Drawing.Color.Empty;
				return block.FontColor.Basic;
			}
		}
		
		internal void SetSelectionFontRichColor(TextLayout.Context context, Drawing.RichColor color)
		{
			//	Modifie la couleur des caractères sélectionnés.
			//	Une couleur vide indique la couleur par défaut.
			string s;
			if ( color.IsEmpty )
			{
				s = TextLayout.CodeDefault + "#" + Drawing.RichColor.ToHexa(this.DefaultRichColor);
			}
			else
			{
				s = "#" + Drawing.RichColor.ToHexa(color);
			}
			this.InsertPutCommand(context, "color=\"" + s + "\"");
		}

		internal void SetSelectionFontColor(TextLayout.Context context, Drawing.Color color)
		{
			//	Modifie la couleur des caractères sélectionnés.
			//	Une couleur vide indique la couleur par défaut.
			string s;
			if ( color.IsEmpty )
			{
				s = TextLayout.CodeDefault + "#" + Drawing.Color.ToHexa(this.DefaultColor);
			}
			else
			{
				s = "#" + Drawing.Color.ToHexa(color);
			}
			this.InsertPutCommand(context, "color=\"" + s + "\"");
		}

		internal Drawing.TextListType GetSelectionList(TextLayout.Context context)
		{
			//	Indique si les caractères sélectionnés commencent par une puce.
			int i = this.SearchJustifBlockRank(context);
			if ( i == -1 )  return Drawing.TextListType.None;

			bool firstBlock = true;
			while ( i >= 0 )
			{
				JustifBlock block = this.blocks[i] as JustifBlock;

				if ( block.LineBreak && !firstBlock )
				{
					return Drawing.TextListType.None;
				}

				if ( block.List )
				{
					return this.ProcessListType(block.Parameters, "type", Drawing.TextListType.Fix);
				}

				i --;
				firstBlock = false;
			}
			return Drawing.TextListType.None;
		}

		internal void SetSelectionList(TextLayout.Context context, Drawing.TextListType list)
		{
			//	Met ou enlève la puce au début des caractères sélectionnés.
			this.Simplify(context);
			this.ListDelete(context);

			if ( list == Drawing.TextListType.Fix )
			{
				this.ListInsert(context, "<list type=\"fix\"/>");
			}

			if ( list == Drawing.TextListType.Num )
			{
				this.ListInsert(context, "<list type=\"num\"/>");
			}
		}


		public bool IsSelectionWaved(TextLayout.Context context)
		{
			JustifBlock block = this.SearchJustifBlock(context);
			if ( block == null )  return false;
			return block.Wave;
		}
		
		public bool IsSelectionWaved(int index)
		{
			return this.IsSelectionWaved(index, false);
		}
		
		public bool IsSelectionWaved(int index, bool after)
		{
			TextLayout.Context context = new TextLayout.Context(this);
			context.CursorAfter = after;
			context.CursorFrom  = index;
			context.CursorTo    = index;
			return this.IsSelectionWaved(context);
		}

		
		protected JustifBlock SearchJustifBlock(Context context)
		{
			//	Cherche le premier bloc correspondant à un index.
			int i = this.SearchJustifBlockRank(context);
			if ( i == -1 )  return null;
			return this.blocks[i] as JustifBlock;
		}

		protected int SearchJustifBlockRank(Context context)
		{
			//	Cherche le premier bloc correspondant à un index.
			this.UpdateLayout();

			int  from  = context.CursorFrom;
			int  to    = context.CursorTo;
			bool after = context.CursorAfter;

			if ( from != to )  // zone sélectionnée ?
			{
				from = System.Math.Min(from, to);  // cherche le début en marche arrière
				after = true;
			}

			if ( after )  // cherche en marche arrière ?
			{
				int index = from;
				for ( int i=this.blocks.Count-1 ; i>=0 ; i-- )
				{
					JustifBlock block = this.blocks[i] as JustifBlock;
					if ( index >= block.BeginIndex && index <= block.EndIndex )
					{
						return i;
					}
				}
			}
			else	// cherche en marche avant ?
			{
				int index = to;
				for ( int i=0 ; i<this.blocks.Count ; i++ )
				{
					JustifBlock block = this.blocks[i] as JustifBlock;
					if ( index >= block.BeginIndex && index <= block.EndIndex )
					{
						return i;
					}
				}
			}
			return -1;
		}

		protected bool IsPrepared(TextLayout.Context context, string key)
		{
			//	Cherche si la préparation contient une commande donnée.
			if ( context.PrepareOffset == -1 )  return false;
			return (this.SearchPrepared(context, key) == "yes");
		}

		protected string SearchPrepared(TextLayout.Context context, string key)
		{
			//	Cherche si la préparation contient une commande donnée.
			if ( context.PrepareOffset == -1 )  return "";

			System.Collections.Hashtable parameters;
			string text = this.InternalText;
			string found = "";  // rien encore trouvé
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
			return found;  // retourne la *dernière* préparation trouvée
		}

		protected void InsertPutCommand(TextLayout.Context context, string cmd)
		{
			//	Modifie les caractères sélectionnés avec une commande <put..>..</put>.
			string begin = "<put " + cmd + ">";
			string end   = "</put>";

			if ( context.PrepareOffset != -1 )
			{
				this.DeletePutCommand(context, begin, end);
			}

			if ( context.CursorFrom == context.CursorTo )  // prépare l'insertion ?
			{
				int cursor1 = this.FindOffsetFromIndex(context.CursorFrom, false);
				int cursor2 = cursor1;
				if ( context.PrepareOffset == -1 )  // première préparation ?
				{
					context.PrepareOffset  = cursor1;
					context.PrepareLength1 = 0;
					context.PrepareLength2 = 0;
				}
				else	// ajoute à une préparation existante ?
				{
					cursor1 = context.PrepareOffset + context.PrepareLength1;
					cursor2 = cursor1 + context.PrepareLength2;
				}
				string text = this.InternalText;
				text = text.Insert(cursor2, end);
				text = text.Insert(cursor1, begin);
				context.PrepareLength1 += begin.Length;
				context.PrepareLength2 += end.Length;
				this.Text = text;
				this.isPrepareDirty = true;
				//	Il ne faut surtout pas faire de this.Simplify() ici !
			}
			else	// modifie les caractères sélectionnés ?
			{
				int cursorFrom = System.Math.Min(context.CursorFrom, context.CursorTo);
				int cursorTo   = System.Math.Max(context.CursorFrom, context.CursorTo);

				int from = this.FindOffsetFromIndex(cursorFrom, false);
				int to   = this.FindOffsetFromIndex(cursorTo, true);

				string text = this.InternalText;
				text = text.Insert(to, end);
				text = text.Insert(from, begin);
				this.Text = text;
				this.Simplify(context);
			}
		}

		protected void DeletePutCommand(TextLayout.Context context, string beginCmd, string endCmd)
		{
			int i, len;

			i = beginCmd.IndexOf("=\"");
			if ( i > 0 )
			{
				beginCmd = beginCmd.Substring(0, i);  // <put bold="yes"> -> <put bold
			}

			i = this.text.IndexOf(beginCmd, context.PrepareOffset, context.PrepareLength1);
			if ( i < 0 )  return;

			len = this.text.IndexOf("\">", i) + 2 - i;
			this.SetText (this.text.Remove(i, len));
			context.PrepareLength1 -= len;

			i = this.text.IndexOf(endCmd, context.PrepareOffset, context.PrepareLength1+context.PrepareLength2);
			if ( i < 0 )  return;

			len = endCmd.Length;
			this.SetText (this.text.Remove(i, len));
			context.PrepareLength2 -= len;
		}

		protected void ListInsert(TextLayout.Context context, string cmd)
		{
			//	Insère des puces dans tous les paragraphes sélectionnés.
			int from = System.Math.Min(context.CursorFrom, context.CursorTo);
			int to   = System.Math.Max(context.CursorFrom, context.CursorTo);
			int initialFrom = from;
			int initialTo   = to;
			this.AlignToBreakLines(ref from, ref to);
			int cursorFrom = this.FindOffsetFromIndex(from, true);
			int cursorTo   = this.FindOffsetFromIndex(to,   true);
			
			int cursor = from;
			string text = this.InternalText;
			System.Collections.Hashtable parameters;
			while ( cursorFrom <= cursorTo )
			{
				text = text.Insert(cursorFrom, cmd);
				cursorFrom += cmd.Length;
				cursorTo   += cmd.Length;

				if ( context.CursorFrom >= cursor )  context.CursorFrom ++;
				if ( context.CursorTo   >= cursor )  context.CursorTo   ++;
				cursor ++;

				Tag tag = Tag.None;
				while ( cursorFrom <= cursorTo )
				{
					tag = TextLayout.ParseTag(text, ref cursorFrom, out parameters);
					cursor ++;
					if ( tag == Tag.EndOfText )  break;
					if ( tag == Tag.LineBreak )  break;
				}
				if ( tag == Tag.EndOfText )  break;
			}
			this.Text = text;

			if ( initialFrom == from && initialFrom < initialTo )
			{
				context.CursorFrom --;  // englobe la première puce créée
			}
		}

		protected void ListDelete(TextLayout.Context context)
		{
			//	Supprime les puces dans tous les paragraphes sélectionnés.
			int from = System.Math.Min(context.CursorFrom, context.CursorTo);
			int to   = System.Math.Max(context.CursorFrom, context.CursorTo);
			this.AlignToBreakLines(ref from, ref to);
			int cursorFrom = this.FindOffsetFromIndex(from, true);
			int cursorTo   = this.FindOffsetFromIndex(to,   true);

			int cursor = from;
			string text = this.InternalText;
			System.Collections.Hashtable parameters;
			while ( cursorFrom <= cursorTo )
			{
				int cursorStart = cursorFrom;
				Tag tag = TextLayout.ParseTag(text, ref cursorFrom, out parameters);
				if ( tag == Tag.EndOfText )  break;
				if ( tag == Tag.List )
				{
					text = text.Remove(cursorStart, cursorFrom-cursorStart);

					cursorTo -= cursorFrom-cursorStart;
					cursorFrom = cursorStart;

					if ( context.CursorFrom > cursor )  context.CursorFrom --;
					if ( context.CursorTo   > cursor )  context.CursorTo   --;
				}
				else
				{
					cursor ++;
				}
			}
			this.Text = text;
		}

		protected void AlignToBreakLines(ref int from, ref int to)
		{
			//	Adapte les index from/to pour englober des paragraphes entiers.
			//	from: début d'un paragraphe (après un <br/>)
			//	to:   fin d'un paragraphe (avant un <br/>)
			string simple = this.InternalSimpleText;

			if ( to > from )  to --;  // ignore le dernier <br/>

			while ( from > 0 )
			{
				if ( simple[from-1] == '\n' )  break;
				from --;
			}

			int len = this.MaxTextIndex;
			while ( to < len )
			{
				if ( simple[to] == '\n' )  break;
				to ++;
			}
		}


		public void SelectAll(TextLayout.Context context)
		{
			//	Sélectionne tout le texte.
			if ( this.text == null )  return;

			context.CursorFrom  = 0;
			context.CursorTo    = this.MaxTextIndex;
			context.CursorAfter = false;
		}

		public void SelectLine(TextLayout.Context context)
		{
			//	Sélectionne toute la ligne.
			this.MoveExtremity(context, -1, false);
			int from = context.CursorFrom;
			this.MoveExtremity(context, 1, false);
			context.CursorFrom = from;
			context.CursorAfter = false;
		}

		public void SelectWord(TextLayout.Context context)
		{
			//	Sélectionne tout le mot.
			string simple = this.InternalSimpleText;

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
			//	Supprime des caractères, tout en conservant les commandes. S'il reste
			//	des commandes superflues, elles seront supprimées par Symplify().
			//	Retourne l'index où insérer les éventuels caractères remplaçants.
			System.Collections.Hashtable parameters;
			string text = this.InternalText;

			int ins = -1;
			while ( from < to )
			{
				int begin = from;
				Tag tag = TextLayout.ParseTag(text, ref from, out parameters);
				if ( tag == Tag.None      ||
					 tag == Tag.LineBreak ||
					 tag == Tag.Tab       ||
					 tag == Tag.List      ||
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
			//	Supprime les caractères sélectionnés dans le texte.
			bool simplified = false;
			if ( context.PrepareOffset != -1 )  // préparation pour l'insertion ?
			{
				this.Simplify(context);
				simplified = true;
			}
			context.CursorAfter = false;
			JustifBlock initialBlock = this.SearchJustifBlock(context);

			int cursorFrom = this.FindOffsetFromIndex(context.CursorFrom);
			int cursorTo   = this.FindOffsetFromIndex(context.CursorTo);
			
			int from = System.Math.Min(cursorFrom, cursorTo);
			int to   = System.Math.Max(cursorFrom, cursorTo);
			
			if ( from == to )
			{
				if ( simplified )  this.MarkLayoutAsDirty();
				return false;
			}
			
			this.DeleteText(from, to);
			from = this.FindIndexFromOffset(from);
			context.CursorTo    = from;
			context.CursorFrom  = from;
			context.CursorAfter = false;

			this.MemoriseDeletedStyle(context, initialBlock);
			return true;
		}

		public bool ReplaceSelection(TextLayout.Context context, string ins)
		{
			//	Insère une chaîne correspondant à un caractère ou un tag (jamais plus).
			int cursorFrom = this.FindOffsetFromIndex(context.CursorFrom);
			int cursorTo   = this.FindOffsetFromIndex(context.CursorTo);
			
			System.Diagnostics.Debug.Assert(cursorFrom >= 0);
			System.Diagnostics.Debug.Assert(cursorTo >= 0);
			
			int from = System.Math.Min(cursorFrom, cursorTo);
			int to   = System.Math.Max(cursorFrom, cursorTo);
			
			if ( from < to )  // caractères sélectionnés à supprimer ?
			{
				bool us = context.UndoSeparator;
				int cursor = this.DeleteText(from, to);
				from = this.FindIndexFromOffset(from);
				context.CursorTo   = from;
				context.CursorFrom = from;

				if ( this.MaxTextOffset+ins.Length > context.MaxChar )  return false;

				string text = this.InternalText;
				text = text.Insert(cursor, ins);
				this.Text = text;

				context.CursorTo    = this.FindIndexFromOffset(cursor + ins.Length);
				context.CursorFrom  = context.CursorTo;
				context.CursorAfter = false;
				context.UndoSeparator = us;

				//	Simplifie seulement après avoir supprimé la sélection puis inséré
				//	le caractère, afin qu'il utilise les mêmes attributs typographiques.
				this.Simplify(context);
			}
			else
			{
				if ( this.MaxTextOffset+ins.Length > context.MaxChar )  return false;
			
				string text = this.InternalText;
				int cursor = this.FindOffsetFromIndex(context.CursorTo, context.CursorAfter);
				bool prepare = false;
				if ( context.PrepareOffset != -1 )  // a-t-on préparé des attributs typographiques ?
				{
					cursor = context.PrepareOffset + context.PrepareLength1;
					prepare = true;
				}
				text = text.Insert(cursor, ins);
				this.Text = text;

				bool us = context.UndoSeparator;
				context.CursorTo    = this.FindIndexFromOffset(cursor + ins.Length);
				context.CursorFrom  = context.CursorTo;
				context.CursorAfter = false;
				context.UndoSeparator = us;

				if ( prepare )  this.Simplify(context);  // supprime les préparations <put...>
			}

			this.DefineCursorPosX(context);
			return true;
		}

		public bool InsertCharacter(TextLayout.Context context, char character)
		{
			//	Insère un caractère.
			return this.ReplaceSelection(context, TextLayout.ConvertToTaggedText(character));
		}

		public bool DeleteCharacter(TextLayout.Context context, int dir)
		{
			//	Supprime le caractère à gauche ou à droite du curseur.
			if ( this.DeleteSelection(context) )  return false;

			if ( context.PrepareOffset != -1 )  // préparation pour l'insertion ?
			{
				this.Simplify(context);
			}
			context.CursorAfter = (dir > 0);
			JustifBlock initialBlock = this.SearchJustifBlock(context);

			int from, to;
			if ( dir < 0 )  // à gauche du curseur ?
			{
				if ( context.CursorTo == 0 )  return false;
				from = this.FindOffsetFromIndex(context.CursorTo-1);
				to   = this.FindOffsetFromIndex(context.CursorTo);
			}
			else	// à droite du curseur ?
			{
				from = this.FindOffsetFromIndex(context.CursorTo);
				to   = this.FindOffsetFromIndex(context.CursorTo+1);
			}

			if ( from == to )  return false;
			this.DeleteText(from, to);
			int cursor = this.FindIndexFromOffset(from);

			bool us = context.UndoSeparator;
			context.CursorTo    = cursor;
			context.CursorFrom  = cursor;
			context.CursorAfter = (dir < 0);
			context.UndoSeparator = us;

			this.MemoriseDeletedStyle(context, initialBlock);
			return true;
		}

		protected void MemoriseDeletedStyle(TextLayout.Context context, JustifBlock block)
		{
			if ( block == null )  return;

			this.SetSelectionFontName(context, block.FontName);
			this.SetSelectionFontScale(context, block.FontScale);
			this.SetSelectionFontRichColor(context, block.FontColor);
			this.SetSelectionBold(context, block.Bold);
			this.SetSelectionItalic(context, block.Italic);
			this.SetSelectionUnderlined(context, block.Underlined);
		}
		
		
		public bool MoveLine(TextLayout.Context context, int move, bool select)
		{
			//	Déplace le curseur par lignes.
			int index;
			bool after;
			int line = context.CursorLine+move;
			
			if ( line < 0 ||
				 line >= this.TotalLineCount ||
				 !this.DetectIndex(context.CursorPosX, line, false, out index, out after) )
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
			//	Déplace le curseur au début ou à la fin d'une ligne.
			double posx;
			if ( move < 0 )  posx = 0;
			else             posx = this.LayoutSize.Width;
			int index;
			bool after;
			if ( !this.DetectIndex(posx, context.CursorLine, false, out index, out after) )
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
			//	Indique si un caractère est un séparateur pour les déplacements
			//	avec Ctrl+flèche.
			character = System.Char.ToLower(character);
			if ( character == '_' ||
				 character == 'á' || character == 'à' || character == 'â' || character == 'ä' ||
				 character == 'ç' ||
				 character == 'é' || character == 'è' || character == 'ê' || character == 'ë' ||
				 character == 'í' || character == 'ì' || character == 'î' || character == 'ï' ||
				 character == 'ó' || character == 'ò' || character == 'ô' || character == 'ö' ||
				 character == 'ú' || character == 'ù' || character == 'û' || character == 'ü' )  return false;
			//	TODO: généraliser avec tous les accents exotiques ?
			if ( character >= 'a' && character <= 'z' )  return false;
			if ( character >= '0' && character <= '9' )  return false;
			return true;
		}

		protected bool IsDualCursor(int index)
		{
			//	Indique s'il existe 2 positions différentes pour un index.
			//	L'une avec CursorAfter = false, et l'autre avec CursorAfter = true.
			this.UpdateLayout();

			int total = 0;
			int rankLine = 0;
			foreach ( JustifBlock block in this.blocks )
			{
				if ( block.Visible && index >= block.BeginIndex && index <= block.EndIndex )
				{
					JustifLine line = (JustifLine)this.lines[block.IndexLine];

					if ( total == 0 )
					{
						rankLine = line.Rank;
					}
					else if ( total == 1 )
					{
						return ( rankLine != line.Rank );
					}
					total ++;
				}
			}
			return false;
		}

		
		public bool MoveCursor(TextLayout.Context context, int move, bool select, bool word)
		{
			//	Déplace le curseur.
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
			string simple = this.InternalSimpleText;

			if ( word )  // déplacement par mots ?
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
			else	// déplacement par caractères ?
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
			//	Mémorise la position horizontale du curseur, afin de pouvoir y
			//	revenir en cas de déplacement par lignes.
			Drawing.Point p1, p2;
			if ( this.FindTextCursor(context, out p1, out p2) )
			{
				context.CursorPosX = p1.X;
			}
		}


		public void Paint(Drawing.Point pos, Drawing.IPaintPort graphics)
		{
			//	Dessine le texte, en fonction du layout...
			//	Si une couleur est donnée avec uniqueColor, tout le texte est peint
			//	avec cette couleur, en ignorant les <font color=...>.
			this.Paint(pos, graphics, Drawing.Rectangle.MaxValue, Drawing.RichColor.Empty, Drawing.GlyphPaintStyle.Normal);
		}

		public void Paint(Drawing.Point pos, Drawing.IPaintPort graphics, Drawing.Rectangle clipRect, Drawing.Color uniqueColor, Drawing.GlyphPaintStyle paintStyle)
		{
			this.Paint(pos, graphics, clipRect, new Drawing.RichColor(uniqueColor), paintStyle);
		}

		public void Paint(Drawing.Point pos, Drawing.IPaintPort graphics, Drawing.Rectangle clipRect, Drawing.RichColor uniqueColor, Drawing.GlyphPaintStyle paintStyle)
		{
			this.UpdateLayout();

			IAdorner adorner = Adorners.Factory.Active;
			double listValue = 0.0;
			bool listEncounter = false;
			foreach ( JustifBlock block in this.blocks )
			{
				if ( !block.Visible )  continue;

				Drawing.Rectangle blockRect = new Drawing.Rectangle();
				
				if ( block.IsImage )
				{
					blockRect.Top    = pos.Y+block.Pos.Y+block.ImageAscender;
					blockRect.Bottom = pos.Y+block.Pos.Y+block.ImageDescender;
				}
				else
				{
					blockRect.Top    = pos.Y+block.Pos.Y+block.Font.Ascender*block.FontSize;
					blockRect.Bottom = pos.Y+block.Pos.Y+block.Font.Descender*block.FontSize;
				}
				
				blockRect.Left   = pos.X+block.Pos.X;
				blockRect.Width  = block.Width;
				
				if ( !blockRect.IntersectsWith(clipRect) )  continue;

				if ( block.IsImage )
				{
					Drawing.Image image = block.Image;

					if ( image.IsPaintStyleDefined(paintStyle) )
					{
						image = image.GetImageForPaintStyle(paintStyle);
					}
					
					image.DefineZoom(graphics.Transform.GetZoom());
					image.DefineColor(uniqueColor.Basic);
					image.DefineAdorner(adorner);
					
					double dx = image.Width;
					double dy = image.Height;
					double ix = pos.X+block.Pos.X;
					double iy = pos.Y+block.Pos.Y+block.ImageDescender; //+block.VerticalOffset;
					
					if ( block.Anchor )
					{
						this.OnAnchor(new AnchorEventArgs(ix, iy, dx, dy, block.BeginIndex));
					}
					
					graphics.Align(ref ix, ref iy);
					graphics.PaintImage(image.BitmapImage, ix, iy, dx, dy, 0, 0, image.Width, image.Height);
					continue;
				}

				Drawing.RichColor color;
				if ( uniqueColor.IsEmpty )
				{
					if ( block.Anchor )
					{
						color = new Drawing.RichColor(this.AnchorColor);
					}
					else
					{
						color = block.GetFontColor;
					}
				}
				else
				{
					color = uniqueColor;
				}
				
				if ( block.Tab )
				{
					if ( this.ShowTab )
					{
						graphics.LineWidth = 1.0/this.drawingScale;
						graphics.RichColor = new Drawing.RichColor(0.35, color.R, color.G, color.B);
						graphics.PaintOutline(this.PathTab(graphics, blockRect));
					}
					continue;
				}

				if ( block.List )
				{
					graphics.RichColor = color;
					this.PaintList(graphics, blockRect, pos.Y+block.Pos.Y, block, ref listValue);
					listEncounter = true;
					continue;
				}

				double x = pos.X+block.Pos.X;
				double y = pos.Y+block.Pos.Y;
				
				if ( block.Anchor )
				{
					double ascender  = block.Font.Ascender  * block.FontSize;
					double descender = block.Font.Descender * block.FontSize;
					this.OnAnchor(new AnchorEventArgs(x, y+descender, block.Width, ascender-descender, block.BeginIndex));
				}

				graphics.RichColor = color;

				if ( block.Infos == null )
				{
					graphics.PaintText(x, y, block.Text, block.Font, block.FontSize);
				}
				else
				{
					graphics.PaintText(x, y, block.Text, block.Font, block.FontSize, block.Infos);
				}

				if ( block.Underlined )
				{
					Drawing.Point p1, p2;
					this.UnderlinedPoints(graphics, block, pos, out p1, out p2);
					graphics.LineWidth = 1.0;
					graphics.RichColor = color;
					graphics.PaintOutline(Drawing.Path.FromLine(p1, p2));
				}

				if ( block.Wave )
				{
					Drawing.Point p1, p2;
					this.UnderlinedPoints(graphics, block, pos, out p1, out p2);
					graphics.LineWidth = 0.75;
					if ( block.WaveColor.IsEmpty )
					{
						graphics.Color = this.WaveColor;;
					}
					else
					{
						graphics.Color = block.WaveColor;
					}
					graphics.PaintOutline(this.PathWave(p1, p2));
				}

				if ( block.LineBreak )
				{
					if ( !listEncounter )  listValue = 0.0;
					listEncounter = false;

					if ( this.ShowLineBreak )
					{
						double width = block.Width;

						if ( block.Infos != null )
						{
							double[] charsWidth;
							block.Font.GetTextCharEndX(block.Text, block.Infos, out charsWidth);
							width = charsWidth[charsWidth.Length-1]*block.FontSize;
						}

						string text = "\u00B6";  // caractère unicode 182
						graphics.PaintText(x+width, y, text, block.Font, block.FontSize);
					}
				}
			}

			if ( !double.IsNaN(this.verticalMark) )  // dessine le marqueur vertical ?
			{
				Drawing.Path path = new Drawing.Path();
				double x = pos.X+this.verticalMark;
				double y = pos.Y;
				graphics.Align(ref x, ref y);
				path.MoveTo(x+0.5, pos.Y);
				path.LineTo(x+0.5, pos.Y+this.layoutSize.Height);
				Drawing.Color color = adorner.ColorCaption;
				color.A = 0.5;
				graphics.Color = color;
				graphics.PaintOutline(path);
			}
		}

		
		protected void UnderlinedPoints(Drawing.IPaintPort graphics, JustifBlock block,
										Drawing.Point pos,
										out Drawing.Point p1, out Drawing.Point p2)
		{
			//	Calcule les points de la ligne pour souligner un bloc.
			double width = block.Width;

			if ( block.Infos != null )
			{
				double[] charsWidth;
				block.Font.GetTextCharEndX(block.Text, block.Infos, out charsWidth);
				width = charsWidth[charsWidth.Length-1]*block.FontSize;
			}

			p1 = new Drawing.Point();
			p2 = new Drawing.Point();
			p1.X = pos.X+block.Pos.X;
			p2.X = p1.X+width;
			p1.Y = pos.Y+block.Pos.Y;

			JustifLine line = (JustifLine)this.lines[block.IndexLine];
			p1.Y += line.Descender/2;
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
			//	Génère le chemin d'une vague "/\/\/\/\/\/\".
			//	Le début "montant" de la vague est toujours aligné sur x=0, afin que
			//	deux vagues successives soient jointives.
			Drawing.Path path = new Drawing.Path();
			double len = 4;  // période d'une vague
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

					if ( pp1.X < p1.X )  // dépasse à gauche ?
					{
						pp1.Y += (pp2.Y-pp1.Y)*(p1.X-pp1.X)/(pp2.X-pp1.X);
						pp1.X = p1.X;
					}

					if ( pp2.X > p2.X )  // dépasse à droite ?
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

		protected Drawing.Path PathTab(Drawing.IPaintPort graphics, Drawing.Rectangle rect)
		{
			//	Génère le chemin pour représenter un tabulateur.
			double x1 = rect.Left+1.0/this.drawingScale;
			double x2 = rect.Right-1.0/this.drawingScale;
			double y  = rect.Bottom+rect.Height*0.5;
			graphics.Align(ref x1, ref y);
			graphics.Align(ref x2, ref y);
			x1 += 0.5/this.drawingScale;
			x2 -= 0.5/this.drawingScale;
			y  -= 0.5/this.drawingScale;
			double len = rect.Height*0.25;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(x1, y);
			path.LineTo(x2, y);
			path.LineTo(x2-len, y+len);
			path.MoveTo(x2, y);
			path.LineTo(x2-len, y-len);
			return path;
		}
		
		protected void PaintList(Drawing.IPaintPort graphics,
								 Drawing.Rectangle rect, double baseLine,
								 JustifBlock block, ref double listValue)
		{
			//	Dessine une puce.
			Drawing.TextListType type = this.ProcessListType(block.Parameters, "type", Drawing.TextListType.Fix);

			if ( type == Drawing.TextListType.Fix )
			{
				Drawing.TextListGlyph glyph = this.ProcessListGlyph(block.Parameters, "glyph", Drawing.TextListGlyph.Circle);
				Drawing.Path path = new Drawing.Path();

				double radius = rect.Height*0.25;
				double cy = baseLine+radius;
				radius *= this.ProcessNum(block.Parameters, "size", 1.0);
				double cx1 = rect.Left+radius;
				double cx2 = rect.Right-radius;
				double cx = cx1+(cx2-cx1)*this.ProcessNum(block.Parameters, "pos", 0.0);

				if ( glyph == Drawing.TextListGlyph.Circle )
				{
					path.AppendCircle(cx, cy, radius);
				}

				if ( glyph == Drawing.TextListGlyph.Square )
				{
					path.AppendRectangle(cx-radius, cy-radius, radius*2, radius*2);
				}

				if ( glyph == Drawing.TextListGlyph.Dash )
				{
					path.MoveTo(cx-radius, cy-radius*0.25);
					path.LineTo(cx-radius, cy+radius*0.25);
					path.LineTo(cx+radius, cy+radius*0.25);
					path.LineTo(cx+radius, cy-radius*0.25);
					path.Close();
				}

				if ( glyph == Drawing.TextListGlyph.Triangle )
				{
					path.MoveTo(cx-radius, cy-radius);
					path.LineTo(cx-radius, cy+radius);
					path.LineTo(cx+radius, cy);
					path.Close();
				}

				if ( glyph == Drawing.TextListGlyph.Arrow )
				{
					path.MoveTo(cx-radius*1.0, cy-radius*0.5);
					path.LineTo(cx-radius*1.0, cy+radius*0.5);
					path.LineTo(cx+radius*0.0, cy+radius*0.5);
					path.LineTo(cx+radius*0.0, cy+radius*1.0);
					path.LineTo(cx+radius*1.0, cy+radius*0.0);
					path.LineTo(cx+radius*0.0, cy-radius*1.0);
					path.LineTo(cx+radius*0.0, cy-radius*0.5);
					path.Close();
				}

				graphics.PaintSurface(path);
			}

			if ( type == Drawing.TextListType.Num )
			{
				string format = this.ProcessString(block.Parameters, "format", "#.");
				format = format.Replace("#", "{0}");
				double start = this.ProcessNum(block.Parameters, "start", 1.0);
				double step  = this.ProcessNum(block.Parameters, "step",  1.0);
				string s = string.Format(format, listValue+start);
				graphics.PaintText(rect.Left, baseLine, s, block.Font, block.FontSize);
				listValue += step;
			}
		}
		
		
		protected void OnAnchor(AnchorEventArgs e)
		{
			if ( this.Anchor != null )
			{
				this.Anchor(this, e);
			}
		}
		
		
		public bool DetectIndex(Drawing.Point pos, bool selZone, out int index, out bool after)
		{
			return this.DetectIndex(pos, -1, selZone, out index, out after);
		}
		
		public bool DetectIndex(double posx, int posLine, bool selZone, out int index, out bool after)
		{
			return this.DetectIndex(new Drawing.Point(posx, 0), posLine, selZone, out index, out after);
		}

		
		protected bool DetectIndex(Drawing.Point pos, int posLine, bool selZone, out int index, out bool after)
		{
			//	Trouve l'index dans le texte interne qui correspond à la
			//	position indiquée. Retourne false en cas d'échec.
			this.UpdateLayout();
			
			pos.Y = System.Math.Max(pos.Y, 0);
			pos.Y = System.Math.Min(pos.Y, this.layoutSize.Height);
			pos.X = System.Math.Max(pos.X, 0);
			pos.X = System.Math.Min(pos.X, this.layoutSize.Width);

			if ( posLine == -1 && this.lines.Count > 0 )
			{
				JustifLine line;

				line = this.lines[0] as JustifLine;
				if ( pos.Y > line.Pos.Y+line.Ascender )  // avant la première ligne ?
				{
					index = 0;
					after = true;
					return true;
				}

				line = this.lines[this.lines.Count-1] as JustifLine;
				if ( pos.Y < line.Pos.Y+line.Descender )  // après la dernière ligne ?
				{
					index = this.MaxTextIndex;
					after = false;
					return true;
				}
			}

			foreach ( JustifLine line in this.lines )
			{
				if ( !line.Visible )  continue;

				if ( (posLine == -1                     &&
					  pos.Y <= line.Pos.Y+line.Ascender &&
					  pos.Y >= line.Pos.Y+line.Descender) ||
					 posLine == line.Rank )
				{
					for ( int j=line.FirstBlock ; j<=line.LastBlock ; j++ )
					{
						JustifBlock block = (JustifBlock)this.blocks[j];

						double before = 0;
						if ( block.BoL )
						{
							before = block.Pos.X;
						}

						double width = block.Width;
						if ( j == this.blocks.Count-1 )  // dernier bloc ?
						{
							width = this.layoutSize.Width-block.Pos.X;
						}
						else
						{
							JustifBlock nextBlock = (JustifBlock)this.blocks[j+1];
							if ( nextBlock.BoL )
							{
								width = this.layoutSize.Width-block.Pos.X;
							}
							else
							{
								width = nextBlock.Pos.X-block.Pos.X;
							}
						}

						if ( pos.X >= block.Pos.X-before &&
							 pos.X <= block.Pos.X+width  )
						{
							if ( block.IsImage || block.Tab || block.List )
							{
								index = ( pos.X-block.Pos.X > width/2 ) ? block.EndIndex : block.BeginIndex;
								after = this.IsAfter(j, index);
								return true;
							}
							else
							{
								double[] charsWidth;
								if ( block.Infos == null )
								{
									block.Font.GetTextCharEndX(block.Text, out charsWidth);
								}
								else
								{
									block.Font.GetTextCharEndX(block.Text, block.Infos, out charsWidth);
								}
								double left = 0;
								double right;
								int max = System.Math.Min(charsWidth.Length, block.EndIndex-block.BeginIndex);
								for ( int k=0 ; k<max ; k++ )
								{
									right = charsWidth[k]*block.FontSize;
									if ( pos.X-block.Pos.X <= left+(right-left)/2 )
									{
										index = block.BeginIndex+k;
										after = this.IsAfter(j, index);
										return true;
									}
									left = right;
								}
								index = block.BeginIndex+max;
								if ( selZone && block.LineBreak && pos.X > block.Pos.X+block.Width )
								{
									index ++;  // après le <br/>
									after = false;
								}
								else
								{
									after = this.IsAfter(j, index);
								}
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
			//	Teste si le bloc précédent contient aussi le même index. Si oui,
			//	retourne true, car l'index trouvé correspond à la 2ème position possible.
			if ( blockRank > 0 )
			{
				JustifBlock prevBlock = (JustifBlock)this.blocks[blockRank-1];
				if ( index >= prevBlock.BeginIndex && index <= prevBlock.EndIndex )
				{
					return true;
				}
			}
			return false;
		}

		
		public string DetectAnchor(Drawing.Point pos)
		{
			//	Détecte s'il y a un lien hypertexte dans la liste des
			//	tags actifs à la position en question. Si oui, extrait la chaîne
			//	de l'argument href, en supprimant les guillemets.
			int index;
			bool after;
			this.DetectIndex(pos, false, out index, out after);
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
			//	Retourne la position horizontale correspondant à un index dans un bloc.
			if ( block.LineBreak && index == block.EndIndex+1 )
			{
				return block.Pos.X + block.Width + line.Height/2;
			}

			if ( index <= block.BeginIndex )  return block.Pos.X;
			if ( index >  block.EndIndex   )  index = block.BeginIndex+block.Text.Length;

			double[] charsWidth;
			if ( block.Infos == null )
			{
				block.Font.GetTextCharEndX(block.Text, out charsWidth);
			}
			else
			{
				block.Font.GetTextCharEndX(block.Text, block.Infos, out charsWidth);
			}
			return block.Pos.X+charsWidth[index-block.BeginIndex-1]*block.FontSize;
		}

		
		public SelectedArea[] FindTextRange(Drawing.Point pos, int indexBegin, int indexEnd)
		{
			//	Retourne un tableau avec les rectangles englobant le texte
			//	spécifié par son début et sa fin. Il y a un rectangle par ligne.
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
				JustifLine line = (JustifLine)this.lines[block.IndexLine];
				if ( !block.Visible )  continue;

				int bbi = block.BeginIndex;
				int bei = bbi + block.Text.Length;
				if ( block.LineBreak )  bei ++;
				int localBegin = System.Math.Max(indexBegin, bbi);
				int localEnd   = System.Math.Min(indexEnd,   bei);

				if ( localBegin >= localEnd )                            continue;
				if ( localBegin >= block.EndIndex && !block.LineBreak )  continue;

				double top    = line.Pos.Y+line.Ascender;
				double bottom = line.Pos.Y+line.Descender;
				Drawing.Color color = block.GetFontColor.Basic;

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

				if ( block.IsImage || block.Tab || block.List )
				{
					area.Rect.Left  = System.Math.Min(area.Rect.Left,  block.Pos.X);
					area.Rect.Right = System.Math.Max(area.Rect.Right, block.Pos.X+block.Width);
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
			
			return Drawing.Point.Zero;
		}
		
		public bool GetLineGeometry(int line, out Drawing.Point pos, out double ascender, out double descender, out double width)
		{
			this.UpdateLayout();
			
			if ( line >= 0 && line < this.lines.Count )
			{
				JustifLine info = (JustifLine)this.lines[line];
				
				pos       = info.Pos;
				ascender  = info.Ascender;
				descender = info.Descender;
				width     = info.Width;
				
				return true;
			}
			
			pos       = Drawing.Point.Zero;
			ascender  = 0;
			descender = 0;
			width     = 0;
			
			return false;
		}
		
		
		
		public bool FindTextCursor(Context context, out Drawing.Point p1, out Drawing.Point p2)
		{
			//	Retourne les deux extrémités du curseur.
			//	Indique également le numéro de la ligne (0..n).
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
				JustifLine line = (JustifLine)this.lines[block.IndexLine];
				if ( block.Visible && index >= block.BeginIndex && index <= block.EndIndex )
				{
					p2.Y = line.Pos.Y+line.Ascender;
					p1.Y = line.Pos.Y+line.Descender;
					if ( block.IsImage || block.Tab || block.List )
					{
						if ( index == block.BeginIndex )
						{
							p1.X = block.Pos.X;
							p2.X = p1.X;
						}
						else
						{
							p1.X = block.Pos.X+block.Width;
							p2.X = p1.X;
						}
					}
					else
					{
						p1.X = this.IndexToPosX(block, line, index);
						p2.X = p1.X;
					}

					double angle = 0.0;
					if ( block.Italic )
					{
						angle = 90.0-block.Font.CaretSlope;
					}
					if ( context.PrepareOffset != -1 )
					{
						angle = this.ScanItalic(context.PrepareOffset+context.PrepareLength1) ? Drawing.Font.DefaultObliqueAngle : 0.0;
					}
					if ( angle != 0.0 )
					{
						angle *= System.Math.PI/180.0;  // en radians
						double f = System.Math.Sin(angle);
						p2.X += line.Ascender*f;
						p1.X += line.Descender*f;
					}

					context.CursorLine = line.Rank;
					return true;
				}
				i += after ? -1 : 1;
			}

			return false;
		}

		protected bool ScanItalic(int offset)
		{
			System.Collections.Hashtable parameters;
			string text = this.InternalText;

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
				
				if ( tag == Tag.EndOfText )
				{
					break;
				}
			}

			return italic;
		}

		protected double ScanFontScale(int offset)
		{
			System.Collections.Hashtable parameters;
			string text = this.InternalText;

			int from = 0;
			int to   = offset;
			double scale = 1.0;  // 100%
			while ( from < to )
			{
				Tag tag = TextLayout.ParseTag(text, ref from, out parameters);

				if ( tag == Tag.Put )
				{
					if ( parameters.ContainsKey("size") )
					{
						string s = (string)parameters["size"];
						scale = this.ParseScale(s);
					}
				}
				
				if ( tag == Tag.PutEnd    ||
					 tag == Tag.EndOfText )
				{
					break;
				}
			}

			return scale;
		}

		public Drawing.Point FindTextEnd()
		{
			//	Retourne le coin inférieur/droite du dernier caractère.
			this.UpdateLayout();

			if ( this.blocks.Count == 0 )
			{
				return new Drawing.Point();
			}

			JustifBlock block = (JustifBlock)this.blocks[this.blocks.Count-1];
			Drawing.Point pos = new Drawing.Point(block.Pos.X+block.Width, block.Pos.Y+block.Font.Descender*block.FontSize);
			return pos;
		}
		
		
#if false
		public int AdvanceTag(int offset)
		{
			//	Si on est au début d'un tag, donne la longueur jusqu'à la fin.
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
			//	Si on est à la fin d'un tag, donne la longueur jusqu'au début.
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
			//	Retourne l'offset dans le texte interne, correspondant à l'index
			//	spécifié pour le texte sans tags. Si after=true, on saute tous les
			//	tags qui précèdent le caractère indiqué (textIndex=0 => premier
			//	caractère non tag dans le texte).
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
					//                 |<-5->|
					if ( startOfTag != "<br/>" &&
						 startOfTag != "<tab/" &&
						 startOfTag != "<list" &&
						 startOfTag != "<img " )  continue;
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
			//	Retourne l'index dans le texte propre, correspondant à l'offset
			//	spécifié dans le texte avec tags.
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
					//                 |<-5->|
					if ( startOfTag == "<br/>" ||
						 startOfTag == "<tab/" ||
						 startOfTag == "<list" ||
						 startOfTag == "<img " )
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
			//	Enlève un tag à la fin de la liste.
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

		
		#region SubstringComplete
		public static string SubstringComplete(string text, int from, int to)
		{
			//	Extrait une sous-chaîne et complète les tags manquants.
			from = System.Math.Min(from, text.Length);
			to   = System.Math.Min(to,   text.Length);
			string result = text.Substring(from, to-from);

			TextLayout.TagComplete(ref result, Tag.Bold,       Tag.BoldEnd,       "<b>", "</b>");
			TextLayout.TagComplete(ref result, Tag.Italic,     Tag.ItalicEnd,     "<i>", "</i>");
			TextLayout.TagComplete(ref result, Tag.Underlined, Tag.UnderlinedEnd, "<u>", "</u>");
			TextLayout.TagComplete(ref result, Tag.Mnemonic,   Tag.MnemonicEnd,   "<m>", "</m>");
			TextLayout.TagComplete(ref result, Tag.Wave,       Tag.WaveEnd,       "<w>", "</w>");

			string fot = TextLayout.TagCapturePrev(text, 0, from, Tag.Font);
			TextLayout.TagComplete(ref result, Tag.Font, Tag.FontEnd, fot, "</font>");

			string aot = TextLayout.TagCapturePrev(text, 0, from, Tag.Anchor);
			TextLayout.TagComplete(ref result, Tag.Anchor, Tag.AnchorEnd, aot, "</a>");

			return result;
		}

		protected static string TagCapturePrev(string text, int from, int to, Tag search)
		{
			int i = TextLayout.TagSearchPrev(text, from, to, search);
			if ( i == -1 )  return null;
			int j = text.IndexOf(">", i);
			if ( j == -1 )  return null;
			return text.Substring(i, j-i+1);
		}

		protected static void TagComplete(ref string text, Tag open, Tag close, string ot, string ct)
		{
			if ( ot != null && TextLayout.TagCompleteNext(text, open, close) )
			{
				text = text.Insert(0, ot);
			}
			
			if ( ct != null && TextLayout.TagCompletePrev(text, open, close) )
			{
				text = text.Insert(text.Length, ct);
			}
		}

		protected static bool TagCompleteNext(string text, Tag open, Tag close)
		{
			int ic = TextLayout.TagSearchNext(text, 0, text.Length, close);
			if ( ic == -1 )  return false;
			int io = TextLayout.TagSearchNext(text, 0, text.Length, open);
			return ( io == -1 || io > ic );
		}

		protected static bool TagCompletePrev(string text, Tag open, Tag close)
		{
			int io = TextLayout.TagSearchPrev(text, 0, text.Length, open);
			if ( io == -1 )  return false;
			int ic = TextLayout.TagSearchPrev(text, 0, text.Length, close);
			return ( ic == -1 || io > ic );
		}

		protected static int TagSearchNext(string text, int from, int to, Tag search)
		{
			//	Cherche un tag en avant.
			System.Collections.Hashtable parameters;
			int offset = from;
			while ( offset < to )
			{
				Tag tag = TextLayout.ParseTag(text, ref offset, out parameters);
				if ( tag == search )
				{
					return offset;
				}
			}
			return -1;
		}

		protected static int TagSearchPrev(string text, int from, int to, Tag search)
		{
			//	Cherche un tag en arrière.
			System.Collections.Hashtable parameters;
			int last = -1;
			int offset = from;
			while ( offset < to )
			{
				int o = offset;
				Tag tag = TextLayout.ParseTag(text, ref offset, out parameters);
				if ( tag == search )
				{
					last = o;
				}
			}
			return last;
		}
		#endregion


		public bool AnalyseTagsAtOffset(int offset, out string[] tags)
		{
			//	Parcourt le texte et accumule les informations sur les tags <>
			//	reconnus.
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
					case Tag.Underlined:
					case Tag.Mnemonic:
					case Tag.Wave:
					case Tag.Font:
					case Tag.Anchor:
						list.Add(sTag);
						break;

					case Tag.BoldEnd:
					case Tag.ItalicEnd:
					case Tag.UnderlinedEnd:
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
			//	Retourne le caractère à un offset quelconque, en interprétant les
			//	commandes &...;
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
			//	Avance d'un caractère ou d'un tag dans le texte.
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
						case "<tab/>":   return Tag.Tab;
						case "<b>":      return Tag.Bold;
						case "<i>":      return Tag.Italic;
						case "<u>":      return Tag.Underlined;
						case "<m>":      return Tag.Mnemonic;
						case "<w>":      return Tag.Wave;
						
						case "</b>":     return Tag.BoldEnd;
						case "</i>":     return Tag.ItalicEnd;
						case "</u>":     return Tag.UnderlinedEnd;
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
							case "<list":  tagId = Tag.List;    close = "/>";  break;
							case "<img":   tagId = Tag.Image;   close = "/>";  break;
							case "<font":  tagId = Tag.Font;    close = ">";   break;
							case "<w":     tagId = Tag.Wave;    close = ">";   break;
							case "<put":   tagId = Tag.Put;     close = ">";   break;
						}
						
						if ( !end.EndsWith(close) )
						{
							return Tag.SyntaxError;
						}
						
						//	Enlève la fin du tag, comme ça on n'a réellement plus que les arguments.
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
			
			if ( text[offset] == TextLayout.CodeLineBreak )
			{
				return Tag.LineBreak;
			}

			if ( text[offset] == TextLayout.CodeTab )
			{
				return Tag.Tab;
			}

			offset ++;
			return Tag.None;
		}
		
		public static char ExtractMnemonic(string text)
		{
			//	Trouve la séquence <m>x</m> dans le texte et retourne le premier caractère
			//	de x comme code mnémonique (en majuscules).
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
			
			return '\0';  // rien trouvé
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
			if (text == null)
			{
				return null;
			}
			
			//	Convertit le texte simple en un texte compatible avec les tags. Supprime
			//	toute occurrence de "<", "&" et ">" dans le texte.
			System.Diagnostics.Debug.Assert(text != null);
			if ( autoMnemonic )
			{
				//	Cherche les occurrences de "&" dans le texte et gère comme suit:
				//	- Remplace "&x" par "<m>x</m>" (le tag <m> spécifie un code mnémonique)
				//	- Remplace "&&" par "&"

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
						case '\t':  buffer.Append("<tab/>");   break;
						case '\r':  buffer.Append(" ");        break;
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
						case '\t':  buffer.Append("<tab/>");  break;
						case '\r':  buffer.Append(' ');       break;
						default:    buffer.Append(c);         break;
					}
				}
				
				return buffer.ToString();
			}
		}
		
		public static string ConvertToSimpleText(string text)
		{
			return TextLayout.ConvertToSimpleText(text, TextLayout.CodeObject.ToString());
		}
		
		public static string ConvertToSimpleText(string text, string imageReplacement)
		{
			//	Epure le texte en supprimant les tags <> et en remplaçant les
			//	tags &gt; et &lt; (et autres) par leurs caractères équivalents.
			//	En plus, les images sont remplacées par le texte 'imageReplacement'
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
						if ( tag.IndexOf("<br/>") == 0 )
						{
							buffer.Append("\n");
						}
						if ( tag.IndexOf("<tab/>") == 0 ||
							 tag.IndexOf("<list")  == 0 )
						{
							buffer.Append("\t");
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

		private void SetText(string newText)
		{
			System.Diagnostics.Debug.Assert (newText != null);

			string oldText = this.text;
			
			if (oldText != newText)
			{
				this.text = newText;
				this.NotifyTextChanges ();
			}
		}

		public void NotifyTextChanges()
		{
			if ((this.textChangeNotificationSuspendCounter == 0) &&
				(this.embedder != null))
			{
				string oldSimplifiedText = this.simplifiedText;
				string newSimplifiedText = this.SimplifiedText;

				if (oldSimplifiedText != newSimplifiedText)
				{
					this.simplifiedText = newSimplifiedText;
					this.embedder.InternalNotifyTextLayoutTextChanged (oldSimplifiedText, newSimplifiedText);
				}
			}
		}

		public void SuspendTextChangeNotifications()
		{
			this.textChangeNotificationSuspendCounter++;
		}

		public void ResumeTextChangeNotifications()
		{
			System.Diagnostics.Debug.Assert (this.textChangeNotificationSuspendCounter > 0);
			
			this.textChangeNotificationSuspendCounter--;
			this.NotifyTextChanges ();
		}
		
		protected void MarkContentsAsDirty()
		{
			this.isContentsDirty = true;
			this.isSimpleDirty = true;
			this.isLayoutDirty = true;
		}
		
		protected void MarkLayoutAsDirty()
		{
			this.isLayoutDirty = true;
		}
		
		protected void UpdateLayout()
		{
			if ( this.isSimpleDirty )
			{
				this.simpleText = TextLayout.ConvertToSimpleText(this.InternalText);
				this.isSimpleDirty = false;
			}

			if ( this.layoutSize.Width > 0 )
			{
				if ( this.isContentsDirty )
				{
					this.GenerateRuns();
					//?this.GenerateTextBreaks();
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

		public void Simplify(TextLayout.Context context)
		{
			if ( this.text == null )  return;
			this.SetText (this.GetSimplify());
			this.isPrepareDirty = false;
			context.PrepareOffset  = -1;  // annule la préparation pour l'insertion
			context.PrepareLength1 = 0;
			context.PrepareLength2 = 0;
		}

		public string GetSimplify()
		{
			//	Simplifie et met à plat toutes les commandes HTML du texte.
			//	Les commandes <put..>..</put> sont intégrées puis supprimées.
			System.Collections.Stack		fontStack;
			FontSimplify					fontDefault;
			FontSimplify					fontItem;
			FontSimplify					fontCurrent;
			System.Text.StringBuilder		buffer;
			System.Collections.Hashtable	parameters;

			fontStack = new System.Collections.Stack();

			//	Prépare la fonte initiale par défaut.
			fontDefault = new FontSimplify();
			fontDefault.FontName  = TextLayout.CodeDefault + this.DefaultFont.FaceName;
			fontDefault.FontScale = "100%";
			fontDefault.FontColor = TextLayout.CodeDefault + "#" + Drawing.RichColor.ToHexa(this.DefaultRichColor);

			fontItem = new FontSimplify();
			fontItem = fontDefault.Copy();
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
							if ( parameters.ContainsKey("face")  )  fontItem.FontName  = (string)parameters["face"];
							if ( parameters.ContainsKey("size")  )  fontItem.FontScale = (string)parameters["size"];
							if ( parameters.ContainsKey("color") )  fontItem.FontColor = (string)parameters["color"];
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
						supplItem.Bold ++;
						break;
					case Tag.BoldEnd:
						supplItem.Bold --;
						break;

					case Tag.Italic:
						supplItem.Italic ++;
						break;
					case Tag.ItalicEnd:
						supplItem.Italic --;
						break;

					case Tag.Underlined:
						supplItem.Underlined ++;
						break;
					case Tag.UnderlinedEnd:
						supplItem.Underlined --;
						break;

					case Tag.Put:
						if ( parameters != null )
						{
							if ( parameters.ContainsKey("face")       )  supplItem.PutFontName   = (string)parameters["face"];
							if ( parameters.ContainsKey("size")       )  supplItem.PutfontScale  = (string)parameters["size"];
							if ( parameters.ContainsKey("color")      )  supplItem.PutFontColor  = (string)parameters["color"];
							if ( parameters.ContainsKey("bold")       )  supplItem.PutBold       = (string)parameters["bold"];
							if ( parameters.ContainsKey("italic")     )  supplItem.PutItalic     = (string)parameters["italic"];
							if ( parameters.ContainsKey("underlined") )  supplItem.PutUnderlined = (string)parameters["underlined"];
						}
						break;
					case Tag.PutEnd:
						supplItem.PutFontName   = "";
						supplItem.PutfontScale  = "";
						supplItem.PutFontColor  = "";
						supplItem.PutBold       = "";
						supplItem.PutItalic     = "";
						supplItem.PutUnderlined = "";
						break;

					case Tag.None:
					case Tag.Image:
					case Tag.LineBreak:
					case Tag.Tab:
					case Tag.List:
					case Tag.Mnemonic:
					case Tag.MnemonicEnd:
					case Tag.Anchor:
					case Tag.AnchorEnd:
					case Tag.Wave:
					case Tag.WaveEnd:
						this.SimplifyPutFont(buffer, fontDefault, fontCurrent, fontItem, supplCurrent, supplItem, ref fontCmd);
						this.SimplifyPutSuppl(buffer, supplCurrent, supplItem);
						fontCurrent = fontItem.Copy();
						supplCurrent = supplItem.Copy();
						buffer.Append(this.text.Substring(begin, offset-begin));
						break;
				}
			}

			if ( fontCmd )
			{
				buffer.Append("</font>");
			}

			supplItem.Bold       = 0;
			supplItem.Italic     = 0;
			supplItem.Underlined = 0;
			supplItem.Anchor     = 0;
			supplItem.Wave       = 0;
			this.SimplifyPutSuppl(buffer, supplCurrent, supplItem);

			return buffer.ToString();
		}

		protected void SimplifyPutFont(System.Text.StringBuilder buffer,
									   FontSimplify fontDefault,
									   FontSimplify fontCurrent, FontSimplify fontItem,
									   SupplSimplify supplCurrent, SupplSimplify supplItem,
									   ref bool fontCmd)
		{
			string itemFontName  = (supplItem.PutFontName  != "") ? supplItem.PutFontName  : fontItem.FontName;
			string itemFontScale = (supplItem.PutfontScale != "") ? supplItem.PutfontScale : fontItem.FontScale;
			string itemFontColor = (supplItem.PutFontColor != "") ? supplItem.PutFontColor : fontItem.FontColor;

			string currFontName  = (supplCurrent.PutFontName  != "") ? supplCurrent.PutFontName  : fontCurrent.FontName;
			string currFontScale = (supplCurrent.PutfontScale != "") ? supplCurrent.PutfontScale : fontCurrent.FontScale;
			string currFontColor = (supplCurrent.PutFontColor != "") ? supplCurrent.PutFontColor : fontCurrent.FontColor;

			if ( itemFontName  != currFontName  ||
				 itemFontScale != currFontScale ||
				 itemFontColor != currFontColor )
			{
				if ( fontCmd )
				{
					buffer.Append("</font>");
					fontCmd = false;

					currFontName  = "";
					currFontScale = "";
					currFontColor = "";
				}

				if ( (itemFontName  != "" && itemFontName  != fontDefault.FontName  && itemFontName  != currFontName ) ||
					 (itemFontScale != "" && itemFontScale != fontDefault.FontScale && itemFontScale != currFontScale) ||
					 (itemFontColor != "" && itemFontColor != fontDefault.FontColor && itemFontColor != currFontColor) )
				{
					buffer.Append("<font");

					if ( itemFontName != "" && itemFontName != fontDefault.FontName && itemFontName != currFontName )
					{
						buffer.Append(" face=\"");
						buffer.Append(itemFontName);
						buffer.Append("\"");
					}

					if ( itemFontScale != "" && itemFontScale != fontDefault.FontScale && itemFontScale != currFontScale )
					{
						buffer.Append(" size=\"");
						buffer.Append(itemFontScale);
						buffer.Append("\"");
					}

					if ( itemFontColor != "" && itemFontColor != fontDefault.FontColor && itemFontColor != currFontColor )
					{
						buffer.Append(" color=\"");
						buffer.Append(itemFontColor);
						buffer.Append("\"");
					}

					buffer.Append(">");
					fontCmd = true;
				}
			}
		}

		protected void SimplifyPutSuppl(System.Text.StringBuilder buffer,
										SupplSimplify supplCurrent, SupplSimplify supplItem)
		{
			if ( supplItem.IsBold != supplCurrent.IsBold )
			{
				buffer.Append(supplItem.IsBold ? "<b>" : "</b>");
			}

			if ( supplItem.IsItalic != supplCurrent.IsItalic )
			{
				buffer.Append(supplItem.IsItalic ? "<i>" : "</i>");
			}

			if ( supplItem.IsUnderlined != supplCurrent.IsUnderlined )
			{
				buffer.Append(supplItem.IsUnderlined ? "<u>" : "</u>");
			}

			if ( (supplItem.Mnemonic != 0) != (supplCurrent.Mnemonic != 0) )
			{
				buffer.Append((supplItem.Mnemonic != 0) ? "<m>" : "</m>");
			}

			if ( (supplItem.Anchor != 0) != (supplCurrent.Anchor != 0) )
			{
				if ( supplItem.Anchor != 0 )
				{
					if ( supplItem.StringAnchor == "" )
					{
						buffer.Append("<a>");
					}
					else
					{
						buffer.Append("<a href=\">");
						buffer.Append(supplItem.StringAnchor);
						buffer.Append("\"");
					}
				}
				else
				{
					buffer.Append("</a>");
				}
			}

			if ( (supplItem.Wave != 0) != (supplCurrent.Wave != 0) )
			{
				if ( supplItem.Wave != 0 )
				{
					if ( supplItem.WaveColor == "" )
					{
						buffer.Append("<w>");
					}
					else
					{
						buffer.Append("<w color=\">");
						buffer.Append(supplItem.WaveColor);
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
			
			font.FontName  = TextLayout.CodeDefault + this.DefaultFont.FaceName;
			font.FontScale = 1;  // 100%
			font.FontColor = Drawing.RichColor.Empty;
			
			stack.Push(font);  // push la fonte initiale (jamais de pop)
			return stack;
		}
		
		
		protected double ParseScale(string scale)
		{
			if ( scale.EndsWith("%") )
			{
				return System.Double.Parse(scale.Substring(0, scale.Length-1), System.Globalization.CultureInfo.InvariantCulture) / 100.0;
			}
			else
			{
				return System.Double.Parse(scale, System.Globalization.CultureInfo.InvariantCulture) / this.DefaultFontSize;
			}
		}
		
		protected void ProcessFontTag(System.Collections.Stack stack, System.Collections.Hashtable parameters)
		{
			if ( parameters != null )
			{
				FontItem font = stack.Peek() as FontItem;
				font = font.Copy();
				
				if ( parameters.ContainsKey("face") )
				{
					font.FontName = (string)parameters["face"];
				}
				if ( parameters.ContainsKey("size") )
				{
					string s = parameters["size"] as string;
					font.FontScale = this.ParseScale(s);
				}
				if ( parameters.ContainsKey("color") )
				{
					string s = parameters["color"] as string;
					Drawing.RichColor color = Drawing.RichColor.FromName(s);
					if ( !color.IsEmpty )  font.FontColor = color;
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

				case Tag.Bold:			supplItem.Bold ++;		break;
				case Tag.BoldEnd:		supplItem.Bold --;		break;

				case Tag.Italic:		supplItem.Italic ++;	break;
				case Tag.ItalicEnd:		supplItem.Italic --;	break;

				case Tag.Underlined:
				case Tag.Mnemonic:		supplItem.Underlined ++;	break;
					
				case Tag.UnderlinedEnd:
				case Tag.MnemonicEnd:	supplItem.Underlined --;	break;

				case Tag.Anchor:		supplItem.Anchor ++;	supplItem.Underlined ++;		break;
				case Tag.AnchorEnd:		supplItem.Anchor --;	supplItem.Underlined --;		break;

				case Tag.Wave:
					supplItem.Wave ++;
					if ( parameters == null )
					{
						supplItem.WaveColor = Drawing.Color.Empty;
					}
					else
					{
						if ( parameters.ContainsKey("color") )
						{
							string s = parameters["color"] as string;
							supplItem.WaveColor = Drawing.Color.FromName(s);
						}
					}
					break;
					
				case Tag.WaveEnd:
					supplItem.Wave --;
					break;

				default:
					return false;
			}
			
			return true;
		}

		protected double ProcessNum(System.Collections.Hashtable parameters, string key, double def)
		{
			if ( parameters == null )  return def;
			if ( !parameters.ContainsKey(key) )  return def;

			string s = parameters[key] as string;
			return System.Double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
		}

		protected string ProcessString(System.Collections.Hashtable parameters, string key, string def)
		{
			if ( parameters == null )  return def;
			if ( !parameters.ContainsKey(key) )  return def;

			return parameters[key] as string;
		}

		protected Drawing.TextListType ProcessListType(System.Collections.Hashtable parameters, string key, Drawing.TextListType def)
		{
			if ( parameters == null )  return def;
			if ( !parameters.ContainsKey(key) )  return def;

			string s = parameters[key] as string;
			switch ( s )
			{
				case "fix":  return Drawing.TextListType.Fix;
				case "num":  return Drawing.TextListType.Num;
			}
			return def;
		}

		protected Drawing.TextListGlyph ProcessListGlyph(System.Collections.Hashtable parameters, string key, Drawing.TextListGlyph def)
		{
			if ( parameters == null )  return def;
			if ( !parameters.ContainsKey(key) )  return def;

			string s = parameters[key] as string;
			switch ( s )
			{
				case "circle":    return Drawing.TextListGlyph.Circle;
				case "square":    return Drawing.TextListGlyph.Square;
				case "dash":      return Drawing.TextListGlyph.Dash;
				case "triangle":  return Drawing.TextListGlyph.Triangle;
				case "arrow":     return Drawing.TextListGlyph.Arrow;
			}
			return def;
		}


		protected void FontToRun(Drawing.TextBreak.XRun run, Drawing.Font font, FontItem fontItem, SupplItem supplItem)
		{
			run.Font       = font;
			run.FontName   = fontItem.FontName;
			run.FontScale  = fontItem.FontScale;
			run.FontSize   = this.DefaultFontSize*fontItem.FontScale;
			run.FontColor  = fontItem.FontColor;
			run.Bold       = supplItem.Bold > 0;
			run.Italic     = supplItem.Italic > 0;
			run.Underlined = supplItem.Underlined > 0;
			run.Anchor     = supplItem.Anchor > 0;
			run.Wave       = supplItem.Wave > 0;
			run.WaveColor  = supplItem.WaveColor;
		}

		protected void PutRun(System.Collections.ArrayList runList,
							  System.Collections.Stack fontStack,
							  SupplItem supplItem,
							  int partIndex, ref int startIndex, int currentIndex,
							  Drawing.Image image, double verticalOffset)
		{
			if ( currentIndex-startIndex == 0 )  return;

			FontItem fontItem = (FontItem)fontStack.Peek();
			Drawing.Font font = fontItem.GetFont(supplItem.Bold>0, supplItem.Italic>0);

			Drawing.TextBreak.XRun run = new Drawing.TextBreak.XRun ();
			run.Start  = startIndex-partIndex;
			run.Length = currentIndex-startIndex;
			run.Image  = image;
			run.VerticalOffset = verticalOffset;
			this.FontToRun(run, font, fontItem, supplItem);
			runList.Add(run);

			startIndex = currentIndex;
		}

		protected void PutTab(System.Collections.Stack fontStack,
							  SupplItem supplItem,
							  int startIndex,
							  Tag tag, System.Collections.Hashtable parameters)
		{
			FontItem fontItem = (FontItem)fontStack.Peek();
			Drawing.Font font = fontItem.GetFont(supplItem.Bold>0, supplItem.Italic>0);

			JustifBlock block = new JustifBlock(this);
			block.Tab        = (tag == Tag.Tab);
			block.List       = (tag == Tag.List);
			block.Parameters = parameters;
			block.Text       = TextLayout.CodeTab.ToString();
			block.BeginIndex = startIndex;
			block.EndIndex   = startIndex+1;
			block.IsDefaultFontName = TextLayout.IsDefaultFontName(fontItem.FontName);
			block.Font       = font;
			block.FontScale  = fontItem.FontScale;
			block.FontColor  = fontItem.FontColor;
			block.Bold       = supplItem.Bold>0;
			block.Italic     = supplItem.Italic>0;
			block.Underlined = supplItem.Underlined>0;
			block.Anchor     = supplItem.Anchor>0;
			block.Wave       = supplItem.Wave>0;
			block.WaveColor  = supplItem.WaveColor;
			this.tabs.Add(block);
		}

		protected void GenerateRuns()
		{
			//	Génère les listes suivantes:
			//	this.parts   parties entre 2 tabs contenant une liste de runs
			//	this.tabs    JustifBlocks des tabulateurs rencontrés
			if ( this.MaxTextOffset == 0 )
			{
				this.parts = null;
				this.tabs  = null;
				return;
			}
			
			//	Crée la liste des parties.
			if ( this.parts == null )
			{
				this.parts = new System.Collections.ArrayList();
			}
			else
			{
				this.parts.Clear();
			}

			//	Crée la liste des tabulateurs (liste de JustifBlocks).
			if ( this.tabs == null )
			{
				this.tabs = new System.Collections.ArrayList();
			}
			else
			{
				this.tabs.Clear();
			}

			System.Collections.Stack		fontStack = this.CreateFontStack();
			System.Collections.Hashtable	parameters;
			SupplItem						supplItem = new SupplItem();

			int		textLength = this.MaxTextOffset;
			int		beginOffset, endOffset = 0;
			int		partIndex = 0;
			int		startIndex = 0, currentIndex = 0;
			Tag 	tagEnding;

			System.Text.StringBuilder		buffer  = new System.Text.StringBuilder(textLength);
			System.Collections.ArrayList	runList = new System.Collections.ArrayList();
			do
			{
				buffer.Length = 0;
				runList.Clear ();
				
				tagEnding = Tag.None;

				while ( endOffset <= textLength )
				{
					beginOffset = endOffset;
					Tag tag = TextLayout.ParseTag(this.text, ref endOffset, out parameters);
			
					if ( tag != Tag.None && tag != Tag.LineBreak )
					{
						this.PutRun(runList, fontStack, supplItem, partIndex, ref startIndex, currentIndex, null, 0);
					}

					if ( tag == Tag.EndOfText )  // fin du texte ?
					{
						tagEnding = Tag.None;
						break;
					}

					if ( tag == Tag.Tab  ||  // partie terminée par un tabulateur ?
						 tag == Tag.List )
					{
						this.PutTab(fontStack, supplItem, startIndex, tag, parameters);
						currentIndex ++;
						startIndex = currentIndex;
						tagEnding = tag;
						break;
					}

					if ( !this.ProcessFormatTags(tag, fontStack, supplItem, parameters) )
					{
						switch ( tag )
						{
							case Tag.Image:
								System.Diagnostics.Debug.Assert( parameters != null && parameters.ContainsKey("src") );
								string imageName = parameters["src"] as string;
								double verticalOffset = 0;
							
								Drawing.Image image = Support.ImageProvider.Default.GetImage(imageName, this.resourceManager);
								
								if ( image == null )
								{
									image = Support.ImageProvider.Default.GetImage ("file:images/missing.icon", this.resourceManager);
									System.Diagnostics.Debug.WriteLine (string.Format("<img> tag references unknown image '{0}' while painting. Current directory is {1}.", imageName, System.IO.Directory.GetCurrentDirectory()));
//-									throw new System.FormatException(string.Format("<img> tag references unknown image '{0}' while painting. Current directory is {1}.", imageName, System.IO.Directory.GetCurrentDirectory()));
								}

								if (parameters.ContainsKey("voff"))
								{
									string s = (string)parameters["voff"];
									verticalOffset = double.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
								}
								
								if ( image is Drawing.Canvas )
								{
									Drawing.Canvas canvas = image as Drawing.Canvas;
									Drawing.Canvas.IconKey key = new Drawing.Canvas.IconKey();

									if ( parameters.ContainsKey("dx") )
									{
										string s = (string)parameters["dx"];
										key.Size.Width = System.Double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
									}

									if ( parameters.ContainsKey("dy") )
									{
										string s = (string)parameters["dy"];
										key.Size.Height = System.Double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
									}

									if ( parameters.ContainsKey("lang") )
									{
										key.Language = (string)parameters["lang"];
									}

									if ( parameters.ContainsKey("style") )
									{
										key.Style = (string)parameters["style"];
									}

									image = canvas.GetImageForIconKey(key);  // cherche la meilleure image
								}
								else if ( image is Drawing.DynamicImage )
								{
									Drawing.DynamicImage dynamic = image as Drawing.DynamicImage;
									
									double width = 0;
									double height = 0;
									
									if ( parameters.ContainsKey("dx") )
									{
										string s = (string)parameters["dx"];
										width = System.Double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
									}

									if ( parameters.ContainsKey("dy") )
									{
										string s = (string)parameters["dy"];
										height = System.Double.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
									}
									
									if ((width != 0) ||
										(height != 0))
									{
										if (width == 0)
										{
											width = dynamic.Width * height / dynamic.Height;
										}
										if (height == 0)
										{
											height = dynamic.Height * width / dynamic.Width;
										}
										
										image = dynamic.GetImageForSize (width, height);
									}
								}
							
								buffer.Append(TextLayout.CodeObject);
								currentIndex ++;
								this.PutRun(runList, fontStack, supplItem, partIndex, ref startIndex, currentIndex, image, verticalOffset);
								break;

							case Tag.LineBreak:
								buffer.Append(TextLayout.CodeLineBreak);
								currentIndex ++;
								break;

							case Tag.None:
								endOffset = beginOffset;
								char c = TextLayout.AnalyseEntityChar(this.text, ref endOffset);
								buffer.Append(c);
								currentIndex ++;
								break;
						}
					}
				}

#if false
				string s = buffer.ToString();
				foreach ( Drawing.TextBreak.XRun run in runList )
				{
					run.DebugText = s.Substring(run.Start, run.Length);
				}
#endif

				TabPart part = new TabPart();
				part.ListEnding = (tagEnding == Tag.List);
				part.Index      = partIndex;
				part.Text       = buffer.ToString();
				part.Runs       = new Drawing.TextBreak.XRun[runList.Count];
				runList.CopyTo(part.Runs);
				this.parts.Add(part);

				partIndex = currentIndex;
			}
			while ( tagEnding != Tag.None );
		}

		protected void GenerateBlocks()
		{
			//	Met à jour this.blocks en fonction du texte, de la fonte et des dimensions.
			this.blocks.Clear();
			int textLength = this.MaxTextOffset;
noText:
			if ( textLength == 0 )
			{
				//	Si le texte n'existe pas, met quand même un bloc vide,
				//	afin de voir apparaître le curseur (FindTextCursor).
				FontItem fontItem = new FontItem(this);
				fontItem.FontName  = TextLayout.CodeDefault + this.DefaultFont.FaceName;
				fontItem.FontScale = 1;  // 100%
				fontItem.FontColor = Drawing.RichColor.Empty;
				if ( this.isPrepareDirty )
				{
					fontItem.FontScale = this.ScanFontScale(this.MaxTextOffset);
				}
				Drawing.Font font = fontItem.GetFont(false, false);

				JustifBlock block = new JustifBlock(this);
				block.BoL       = true;
				block.LineBreak = false;
				block.Text      = "";
				block.IsDefaultFontName = true;
				block.Font      = font;
				block.FontScale = fontItem.FontScale;
				block.FontColor = fontItem.FontColor;
				this.blocks.Add(block);
				return;
			}

			double pos = 0.0;  // position horizontale dans la ligne
			double indent = 0.0;  // pas encore d'indentation
			int tabIndex = 0;  // index dans this.tabs
			bool lineBreak = false;  // bloc terminé par <br/> ?
			bool beginOfText = true;
			bool listEnding = false;

			foreach ( TabPart part in this.parts )
			{
				int partIndex = part.Index;

				Drawing.TextTabType tabType = Drawing.TextTabType.None;
				double tabWidth = 0.0;
				if ( !beginOfText )  // ajoute un bloc "tab" avant la partie ?
				{
					System.Diagnostics.Debug.Assert(tabIndex < this.tabs.Count);
					JustifBlock tb = this.tabs[tabIndex++] as JustifBlock;

					Drawing.TextStyle.Tab tab;
					if ( listEnding )  // partie précédente terminée par <list/> ?
					{
						tab = new Drawing.TextStyle.Tab();
						tab.Type = Drawing.TextTabType.Indent;
						tab.Pos  = pos+tb.FontSize*this.ProcessNum(tb.Parameters, "width", 2.0);
					}
					else
					{
						tab = this.style.FindTabAfterPosition(pos);
					}
					tabType = tab.Type;

					if ( tab.Pos > this.layoutSize.Width )
					{
						pos = indent;
						lineBreak = true;
						tab = this.style.FindTabAfterPosition(pos);
						if ( tab.Pos > this.layoutSize.Width )
						{
							textLength = 0;
							goto noText;  // y'a plus rien à faire !
						}
					}

					JustifBlock block = tb.Copy();
					block.BoL    = lineBreak;  // dernier bloc terminé par <br/> ?
					block.Indent = block.BoL ? indent : 0.0;
					block.Width  = tab.Pos-pos;
					this.blocks.Add(block);

					if ( tabType == Drawing.TextTabType.Left    ||
						 tabType == Drawing.TextTabType.Center  ||
						 tabType == Drawing.TextTabType.Decimal )
					{
						tabWidth = block.Width;  // largeur év. "mangeable"
					}
					if ( tabType == Drawing.TextTabType.Indent )
					{
						indent = tab.Pos;
					}
					lineBreak = false;
					pos = tab.Pos;
				}

				listEnding = part.ListEnding;

				if ( part.Runs.Length == 0 )  // partie totalement vide ?
				{
					beginOfText = false;
					continue;
				}

				bool beginOfPart = beginOfText;

				Drawing.TextBreak textBreak = new Drawing.TextBreak ();
				textBreak.SetText(part.Text, this.BreakMode);
				textBreak.SetRuns(part.Runs);

				//	Essaie de caser le texte dans la largeur restante.
				double restWidth = this.layoutSize.Width-pos+tabWidth;
				Drawing.TextBreak.Line[] lines = textBreak.GetLines (restWidth, this.layoutSize.Width-indent, this.layoutSize.Width);
			
				if ( lines == null || lines.Length == 0 )
				{
					//	Essaie de caser le texte dans la largeur totale.
					pos = indent;
					beginOfPart = true;
					lines = textBreak.GetLines(this.layoutSize.Width-indent, this.layoutSize.Width-indent, this.layoutSize.Width);
					if ( lines == null || lines.Length == 0 )
					{
						textLength = 0;
						goto noText;  // y'a plus rien à faire !
					}
					tabWidth = 0.0;
				}

				if ( tabWidth != 0.0 )  // est-on après un tabulateur spécial ?
				{
					JustifBlock block = this.blocks[this.blocks.Count-1] as JustifBlock;
					System.Diagnostics.Debug.Assert(block.Tab);
					Drawing.TextBreak.Line line = lines[0] as Drawing.TextBreak.Line;
					double dist = 0.0;
					if ( tabType == Drawing.TextTabType.Left )
					{
						dist = line.Width;
					}
					if ( tabType == Drawing.TextTabType.Center )
					{
						dist = line.Width/2;
					}
					if ( tabType == Drawing.TextTabType.Decimal )
					{
						dist = this.DecimalTabLength(part.Text, part.Runs);
					}
					dist = System.Math.Min(dist, block.Width-0.1);
					block.Width -= dist;  // diminue la largeur du tab précédent
					pos -= dist;
				}

				int lineStart = 0;
				int runIndex  = 0;
				foreach (Drawing.TextBreak.Line line in lines)
				{
					int brutIndex = lineStart;
					bool beginOfLine = beginOfPart;
					beginOfPart = true;
					do
					{
						Drawing.TextBreak.XRun run = part.Runs[runIndex];
						while ( brutIndex >= run.Start+run.Length )
						{
							run = part.Runs[++runIndex] as Drawing.TextBreak.XRun;
						}

						int start = System.Math.Max(run.Start, lineStart);
						int next  = System.Math.Min(run.Start+run.Length, lineStart+line.Skip);
						int end   = System.Math.Min(run.Start+run.Length, lineStart+line.Text.Length);

						if ( end-start > 0 )
						{
							string text = line.Text.Substring(start-lineStart, end-start);

							lineBreak = false;
							if ( text.Length > 0 && text[text.Length-1] == TextLayout.CodeLineBreak )
							{
								text = text.Substring(0, text.Length-1);
								end --;
								lineBreak = true;
								pos = indent;
							}
							if ( beginOfLine )
							{
								pos = indent;
							}

							Drawing.Font font = run.Font;

							JustifBlock block = new JustifBlock(this);
							block.BoL        = beginOfLine;
							block.Indent     = block.BoL ? indent : 0.0;
							block.LineBreak  = lineBreak;
							block.Text       = text;
							block.BeginIndex = partIndex+start;
							block.EndIndex   = partIndex+System.Math.Min(end, next);
							block.Width      = font.GetTextAdvance(text)*run.FontSize;
							block.IsDefaultFontName = TextLayout.IsDefaultFontName(run.FontName);
							block.Font       = font;
							block.FontScale  = run.FontScale;
							block.FontColor  = run.FontColor;
							block.Bold       = run.Bold;
							block.Italic     = run.Italic;
							block.Underlined = run.Underlined;
							block.Anchor     = run.Anchor;
							block.Wave       = run.Wave;
							block.WaveColor  = run.WaveColor;
					
							if ( run.Image != null )
							{
								Drawing.Image image = run.Image;
								double dx = image.Width;
								double dy = image.Height;

								double fontAscender  = font.Ascender;
								double fontDescender = font.Descender;
								double fontHeight    = fontAscender-fontDescender;

								block.Image = image;
								block.Text  = TextLayout.CodeObject.ToString();
								block.Width = dx;
							
								if ( image.IsOriginDefined )
								{
									block.ImageAscender  = dy - image.Origin.Y;
									block.ImageDescender = -image.Origin.Y;
								}
								else
								{
									block.ImageAscender  = dy*fontAscender/fontHeight;
									block.ImageDescender = dy*fontDescender/fontHeight;
								}
								
								block.ImageAscender  += run.VerticalOffset;
								block.ImageDescender += run.VerticalOffset;
								
//-								block.VerticalOffset = run.VerticalOffset;
							
								if ( this.JustifMode != Drawing.TextJustifMode.NoLine )
								{
									double width = dx/run.FontSize;
									block.Infos = new Drawing.FontClassInfo[1];
									block.Infos[0] = new Drawing.FontClassInfo(Drawing.GlyphClass.PlainText, 1, width, 0.0);
									block.InfoWidth = width;
									block.InfoElast = 0.0;
								}
							}
							else
							{
								if ( this.JustifMode == Drawing.TextJustifMode.NoLine )
								{
									block.Infos     = null;
									block.InfoWidth = 0;
									block.InfoElast = 0;
								}
								else if ( block.Text != "" )
								{
									block.Font.GetTextClassInfos(block.Text, out block.Infos, out block.InfoWidth, out block.InfoElast);
								}
							}
				
							this.blocks.Add(block);
						}
						brutIndex += next-start;
						beginOfLine = false;
					}
					while ( brutIndex < lineStart+line.Skip );

					if ( lineBreak )
					{
						indent = 0.0;
						pos = 0.0;
					}

					lineStart += line.Skip;
				}

				if ( !lineBreak && lines.Length > 0 )
				{
					Drawing.TextBreak.Line lastLine = lines[lines.Length-1] as Drawing.TextBreak.Line;
					pos += lastLine.Width;
				}

				beginOfText = false;
			}

			if ( this.blocks.Count == 0 )  // texte totalement vide ?
			{
				textLength = 0;
				goto noText;
			}

			if ( this.blocks.Count > 0 )
			{
				JustifBlock block = this.blocks[this.blocks.Count-1] as JustifBlock;
				if ( block.LineBreak )  // texte terminé par un <br/> ?
				{
					//	Ajoute un bloc vide afin de pouvoir mettre le curseur au début de
					//	la ligne vide qui suit le <br/>.
					JustifBlock lb = block.Copy();
					lb.BeginIndex = block.EndIndex+1;
					lb.EndIndex   = block.EndIndex+1;
					lb.Text       = "";
					lb.BoL        = true;
					lb.Indent     = indent;
					lb.LineBreak  = false;
					lb.Width      = 0.0;
					this.blocks.Add(lb);
				}
			}
		}

		protected double DecimalTabLength(string text, Drawing.TextBreak.XRun[] runs)
		{
			//	Retourne la distance jusqu'au point décimal.
			if ( text.Length == 0 )  return 0.0;

			string sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			int index = text.IndexOf(sep);
			if ( index < 0 )  return 0.0;
			string part = text.Substring(0, index);

			Drawing.TextBreak textBreak = new Drawing.TextBreak ();
			textBreak.SetText(part, Drawing.TextBreakMode.None);
			textBreak.SetRuns(runs);
			Drawing.TextBreak.Line[] lines = textBreak.GetLines (1000000);
			if ( lines == null || lines.Length == 0 )  return 0.0;

			Drawing.TextBreak.Line line = lines[0] as Drawing.TextBreak.Line;
			return line.Width;
		}

		protected void GenerateJustification()
		{
			//	Met à jour this.lines en fonction de this.blocks.
			//	Détermine la position des blocs en fonction de l'alignement.
			//	Détermine également quels sont les blocs et les lignes visibles.
			this.lines.Clear();

			this.totalLine = 0;
			this.visibleLine = 0;

			Drawing.Point pos = new Drawing.Point(0,0);
			pos.Y += this.layoutSize.Height;  // coin sup/gauche

			JustifBlock	block;
			JustifLine	line;

			double totalHeight = 0;
			double overflow = 0;
			int beginLineBlock = 0;
			bool containTab = false;
			while ( beginLineBlock < this.blocks.Count )
			{
				//	Avance tous les blocs de la ligne.
				double width = 0;
				double height = 0;
				double ascender = 0;
				double descender = 0;
				int nextLineBlock = beginLineBlock;
				int beginJustifBlock = beginLineBlock;
				while ( true )
				{
					block = (JustifBlock)this.blocks[nextLineBlock];
					block.IndexLine = totalLine;
					width += block.Width;
					if ( block.IsImage )
					{
						if (height == 0)
						{
							height    = block.ImageAscender-block.ImageDescender;
							ascender  = block.ImageAscender;
							descender = block.ImageDescender;
						}
						else
						{
							height    = System.Math.Max(height,    block.ImageAscender-block.ImageDescender);
							ascender  = System.Math.Max(ascender,  block.ImageAscender);
							descender = System.Math.Min(descender, block.ImageDescender);
						}
					}
					else
					{
						if (height == 0)
						{
							height    = block.Font.LineHeight*block.FontSize;
							ascender  = block.Font.Ascender  *block.FontSize;
							descender = block.Font.Descender *block.FontSize;
						}
						else
						{
							height    = System.Math.Max(height,    block.Font.LineHeight*block.FontSize);
							ascender  = System.Math.Max(ascender,  block.Font.Ascender  *block.FontSize);
							descender = System.Math.Min(descender, block.Font.Descender *block.FontSize);
						}
					}
					if ( block.Tab || block.List )
					{
						containTab = true;
						beginJustifBlock = nextLineBlock+1;
					}

					nextLineBlock ++;
					if ( nextLineBlock >= this.blocks.Count )  break;
					block = (JustifBlock)this.blocks[nextLineBlock];
					if ( block.BoL )  break;  // break si début nouvelle ligne
				}

				bool visible;
				this.totalLine ++;
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

				JustifBlock firstBlock = (JustifBlock)this.blocks[beginLineBlock];
				pos.Y -= ascender;  // sur la ligne de base
				pos.X = firstBlock.Indent;

				double lineWidth = this.layoutSize.Width - firstBlock.Indent;

				line = new JustifLine();
				line.FirstBlock = beginLineBlock;
				line.LastBlock  = nextLineBlock-1;
				line.Rank       = this.totalLine-1;
				line.Pos        = pos;
				line.Width      = width;
				line.Height     = height;
				line.Ascender   = ascender;
				line.Descender  = descender;
				line.Visible    = visible;
				this.lines.Add(line);

				bool justif = false;
				JustifBlock lastBlock = (JustifBlock)this.blocks[nextLineBlock-1];
				bool lastLine = lastBlock.LineBreak;
				if ( nextLineBlock-1 >= this.blocks.Count-1 )  lastLine = true;
				if ( (this.JustifMode == Drawing.TextJustifMode.AllButLast && !lastLine) ||
					 this.JustifMode == Drawing.TextJustifMode.All )
				{
					double w = 0;
					double e = 0;
					for ( int i=beginJustifBlock ; i<nextLineBlock ; i++ )
					{
						block = (JustifBlock)this.blocks[i];
						w += block.InfoWidth * block.FontSize;
						e += block.InfoElast * block.FontSize;
					}
					double delta = lineWidth-w;

					if ( e > 0 && delta != 0 )
					{
						for ( int i=beginLineBlock ; i<nextLineBlock ; i++ )
						{
							block = (JustifBlock)this.blocks[i];

							if ( i < beginJustifBlock )
							{
								block.Pos = pos;
								block.Visible = visible;
								block.Infos = null;
								pos.X += block.Width;
								delta -= block.Width;
							}
							else
							{
								containTab = false;

								if ( block.Infos != null )
								{
									for ( int ii=0 ; ii<block.Infos.Length ; ii++ )
									{
										double widthRatio = block.Infos[ii].Width/delta;
										double elastRatio = block.Infos[ii].Elasticity/e;
										block.Infos[ii].Scale = 1.0+elastRatio/widthRatio;
									}
								}
								block.Pos = pos;
								block.Visible = visible;

								if ( block.IsImage || block.Tab || block.List )
								{
									pos.X += block.Width;
								}
								else
								{
									double[] charsWidth;
									block.Font.GetTextCharEndX(block.Text, block.Infos, out charsWidth);
									pos.X += charsWidth[block.Text.Length-1] * block.FontSize;
								}
							}
						}
						justif = true;
					}
				}

				if ( !justif || containTab )
				{
					Drawing.ContentAlignment alignment = this.Alignment;
					if ( containTab )  alignment = Drawing.ContentAlignment.TopLeft;

					switch ( alignment )
					{
						case Drawing.ContentAlignment.TopLeft:
						case Drawing.ContentAlignment.MiddleLeft:
						case Drawing.ContentAlignment.BottomLeft:
							pos.X = firstBlock.Indent;
							break;
				
						case Drawing.ContentAlignment.TopCenter:
						case Drawing.ContentAlignment.MiddleCenter:
						case Drawing.ContentAlignment.BottomCenter:
							pos.X = firstBlock.Indent + (lineWidth-width)/2;
							break;
				
						case Drawing.ContentAlignment.TopRight:
						case Drawing.ContentAlignment.MiddleRight:
						case Drawing.ContentAlignment.BottomRight:
							pos.X = firstBlock.Indent + lineWidth-width;
							break;
					}

					line.Pos = pos;

					for ( int i=beginLineBlock ; i<nextLineBlock ; i++ )
					{
						block = (JustifBlock)this.blocks[i];
						block.Pos = pos;
						block.Visible = visible;
						block.Infos = null;
						pos.X += block.Width;
					}
				}

				pos.Y += ascender;
				pos.Y -= height;  // position haut de la ligne suivante

				beginLineBlock = nextLineBlock;  // index début ligne suivante
			}

			//	Effectue l'alignement vertical.
			totalHeight -= overflow;
			double offset = 0;
			switch ( this.Alignment )
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
				for ( int i=0 ; i<this.blocks.Count ; i++ )
				{
					block = (JustifBlock)this.blocks[i];
					block.Pos.Y -= offset;  // descend le bloc
				}
				for ( int i=0 ; i<this.lines.Count ; i++ )
				{
					line = (JustifLine)this.lines[i];
					line.Pos.Y -= offset;  // descend la ligne
				}
			}
		}

		
		public void DebugDumpJustif(System.IO.TextWriter stream)
		{
			//	Affiche le contenu du tableau this.blocks, pour le debug.
			this.UpdateLayout();
			stream.WriteLine("Total blocks = " + this.blocks.Count);
			foreach ( JustifBlock block in this.blocks )
			{
				string text = string.Format
				(
					"bol={0},  br={1},  font={2} {3},  pos={4};{5},  width={6},  index={7};{8},  text=\"{9}\"",
					block.BoL, block.LineBreak,
					block.Font.FullName, block.FontSize.ToString("F2"),
					block.Pos.X.ToString("F2"), block.Pos.Y.ToString("F2"),
					block.Width.ToString("F2"),
					block.BeginIndex, block.EndIndex,
					block.Text
				);
				stream.WriteLine(text);
			}
		}


		public static bool CheckSyntax(string text, out int offsetError)
		{
			//	Vérifie la syntaxe d'un texte.
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
					case Tag.Underlined:
					case Tag.Mnemonic:
					case Tag.Wave:
					case Tag.Font:
					case Tag.Anchor:
						list.Add(sTag);
						break;

					case Tag.BoldEnd:
					case Tag.ItalicEnd:
					case Tag.UnderlinedEnd:
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


		public void FillFontFaceList(System.Collections.ArrayList list)
		{
			//	Ajoute toutes les fontes utilisées dans une liste.
			if ( this.text == null )  return;
			int from = 0;
			int to   = this.text.Length;
			System.Collections.Hashtable parameters;
			while ( from < to )
			{
				Tag tag = TextLayout.ParseTag(text, ref from, out parameters);
				if ( tag == Tag.Font )
				{
					if ( parameters.ContainsKey("face") )
					{
						string s = (string)parameters["face"];
						if ( !list.Contains(s) )
						{
							list.Add(s);
						}
					}
				}
			}
		}


		public OneCharStructure[] ComputeStructure()
		{
			//	Génère la structure pour chaque caractère du texte, ce qui permet
			//	de gérer les textes spéciaux non rectangulaires, tels que l'objet
			//	ObjectTextLine de Pictogram.
			if ( this.layoutSize.Width == 0 )
			{
				return new OneCharStructure[0];
			}

			this.UpdateLayout();

			int len = this.MaxTextIndex;
			if ( len == 0 )  len ++;
			OneCharStructure[] array = new OneCharStructure[len];

			if ( this.blocks.Count == 1 )
			{
				JustifBlock block = this.blocks[0] as JustifBlock;
				if ( block.Text.Length == 0 )  // texte vide ?
				{
					array[0] = new OneCharStructure();
					array[0].Character  = TextLayout.CodeEndOfText;
					array[0].Font       = block.Font;
					array[0].FontSize   = block.FontSize;
					array[0].FontColor  = block.GetFontColor;
					array[0].Bold       = block.Bold;
					array[0].Italic     = block.Italic;
					array[0].Underlined = block.Underlined;
					return array;
				}
			}

			int index = 0;
			foreach ( JustifBlock block in this.blocks )
			{
				for ( int i=0 ; i<block.Text.Length ; i++ )
				{
					System.Diagnostics.Debug.Assert(index < len);
					array[index] = new OneCharStructure();
					array[index].Character  = block.Text[i];
					array[index].Font       = block.Font;
					array[index].FontSize   = block.FontSize;
					array[index].FontColor  = block.GetFontColor;
					array[index].Bold       = block.Bold;
					array[index].Italic     = block.Italic;
					array[index].Underlined = block.Underlined;
					index ++;
				}
			}
			//?System.Diagnostics.Debug.Assert(index == len);

			return array;
		}

		public class OneCharStructure
		{
			public char						Character;
			public Drawing.Font				Font;
			public double					FontSize;
			public Drawing.RichColor		FontColor = Drawing.RichColor.Empty;
			public bool						Bold;
			public bool						Italic;
			public bool						Underlined;
		}
		

		//	Tous les tags possibles.
		public enum Tag
		{
			None,							// pas un tag
			Unknown,						// tag pas reconnu
			SyntaxError,					// syntaxe du tag incorrecte
			EndOfText,						// fin du texte
			
			LineBreak,						// <br/>
			Tab,							// <tab/>
			List,							// <list type="fix" glyph="circle" width="1.5" pos="0.0" size="1"/>
			
			Bold,		BoldEnd,			// <b>...</b>
			Italic,		ItalicEnd,			// <i>...</i>
			Underlined,	UnderlinedEnd,		// <u>...</u>
			Mnemonic,	MnemonicEnd,		// <m>...</m>  --> comme <u>...</u>
			Wave,		WaveEnd,			// <w>...</w> ou <w color="#FF00FF">
			Font,		FontEnd,			// <font ...>...</font>
			Anchor,		AnchorEnd,			// <a href="x">...</a>
			Image,							// <img src="x"/>

			Put,		PutEnd,				// <put ...>...</put>
		}
		
		
		//	Fonte servant à refléter simplifier les commandes HTML rencontrées.
		//	Un stack de FontSimplify est créé.
		protected class FontSimplify
		{
			public FontSimplify Copy()
			{
				return this.MemberwiseClone() as FontSimplify;
			}

			
			public string		FontName;
			public string		FontScale;
			public string		FontColor;
		}

		protected class SupplSimplify
		{
			public SupplSimplify Copy()
			{
				return this.MemberwiseClone() as SupplSimplify;
			}

			
			public bool		IsBold
			{
				get
				{
					if ( this.PutBold == "yes" )  return true;
					if ( this.PutBold == "no"  )  return false;
					return (this.Bold > 0);
				}
			}

			public bool		IsItalic
			{
				get
				{
					if ( this.PutItalic == "yes" )  return true;
					if ( this.PutItalic == "no"  )  return false;
					return (this.Italic > 0);
				}
			}

			public bool		IsUnderlined
			{
				get
				{
					if ( this.PutUnderlined == "yes" )  return true;
					if ( this.PutUnderlined == "no"  )  return false;
					return (this.Underlined > 0);
				}
			}

			
			public int		Bold          = 0;	// gras si > 0
			public int		Italic        = 0;	// italique si > 0
			public int		Underlined    = 0;	// souligné si > 0
			public int		Mnemonic      = 0;	// souligné si > 0
			public int		Anchor        = 0;	// lien si > 0
			public string	StringAnchor  = "";
			public int		Wave          = 0;	// vague si > 0
			public string	WaveColor     = "";
			public string	PutFontName   = "";
			public string	PutfontScale  = "";
			public string	PutFontColor  = "";
			public string	PutBold       = "";
			public string	PutItalic     = "";
			public string	PutUnderlined = "";
		}

		//	Fonte servant à refléter les commandes HTML rencontrées.
		//	Un stack de FontItem est créé.
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

			public Drawing.Font GetFont(bool bold, bool italic)
			{
				Drawing.Font font     = null;
				string       fontFace = TextLayout.FontName(this.FontName);
				
				Drawing.FontFaceInfo face = Drawing.Font.GetFaceInfo(fontFace);
				
				if ( face != null )
				{
					font = face.GetFont(bold, italic);
				}
				
				if ( font == null )
				{
					font = Drawing.Font.GetFontFallback(fontFace);
				}
				
				if ( font == null )
				{
					font = this.host.DefaultFont;
				}
				
				return font;
			}

			public double FontSize
			{
				get
				{
					return host.DefaultFontSize * this.FontScale;
				}
			}

			protected TextLayout	host;
			
			public string				FontName;
			public double				FontScale;
			public Drawing.RichColor	FontColor;
		}

		protected class SupplItem
		{
			public int				Bold       = 0;  // gras si > 0
			public int				Italic     = 0;  // italique si > 0
			public int				Underlined = 0;  // souligné si > 0
			public int				Anchor     = 0;  // lien si > 0
			public int				Wave       = 0;  // vague si > 0
			public Drawing.Color	WaveColor  = Drawing.Color.Empty;
		}

		//	Descripteur d'un bloc de texte. Tous les caractères du bloc ont
		//	la même fonte, même taille et même couleur.
		protected class JustifBlock
		{
			public JustifBlock(TextLayout host)
			{
				this.host = host;
			}
			
			public JustifBlock Copy()
			{
				return this.MemberwiseClone() as JustifBlock;
			}

			public bool						IsImage
			{
				get
				{
					return this.Image != null;
				}
			}

			public void DefineFont(Drawing.Font blockFont, FontItem fontItem, SupplItem supplItem)
			{
				this.IsDefaultFontName = TextLayout.IsDefaultFontName(fontItem.FontName);
				this.Font       = blockFont;
				this.FontScale  = fontItem.FontScale;
				this.FontColor  = fontItem.FontColor;
				this.Bold       = supplItem.Bold > 0;
				this.Italic     = supplItem.Italic > 0;
				this.Underlined = supplItem.Underlined > 0;
				this.Anchor     = supplItem.Anchor > 0;
				this.Wave       = supplItem.Wave > 0;
				this.WaveColor  = supplItem.WaveColor;
			}

			public string FontName
			{
				get
				{
					if ( this.IsDefaultFontName )
					{
						return TextLayout.CodeDefault + this.Font.FaceName;
					}
					return this.Font.FaceName;
				}
			}

			public double FontSize
			{
				get
				{
					return this.host.DefaultFontSize * this.FontScale;
				}
			}

			public Drawing.RichColor GetFontColor
			{
				get
				{
					if ( this.FontColor.IsEmpty )
					{
						return host.DefaultRichColor;
					}
					return this.FontColor;
				}
			}
			
			protected TextLayout			host;

			public bool						BoL;		// begin of line
			public bool						LineBreak;	// ending with br
			public string					Text;
			public int						BeginIndex;
			public int						EndIndex;
			public int						IndexLine;	// index dans this.lines
			public bool						IsDefaultFontName;
			public Drawing.Font				Font;
			public double					FontScale;
			public Drawing.RichColor		FontColor = Drawing.RichColor.Empty;
			public bool						Bold;
			public bool						Italic;
			public bool						Underlined;
			public bool						Anchor;
			public bool						Wave;
			public Drawing.Color			WaveColor = Drawing.Color.Empty;
			public double					Indent;		// largeur à sauter pour l'indentation
			public double					Width;		// largeur du bloc
			public Drawing.Point			Pos;		// sur la ligne de base
			public Drawing.Image			Image;		// image bitmap
			public double					ImageAscender;
			public double					ImageDescender;
//-			public double					VerticalOffset;
			public bool						Tab;		// contient un tabulateur
			public bool						List;		// contient une puce
			public System.Collections.Hashtable Parameters;
			public bool						Visible;
			public Drawing.FontClassInfo[]	Infos;
			public double					InfoWidth;
			public double					InfoElast;
		}

		//	Descripteur d'une ligne de texte. Une ligne est composée
		//	d'un ou plusieurs blocs.
		protected class JustifLine
		{
			public int						FirstBlock;	// index du premier bloc
			public int						LastBlock;	// index du dernier bloc
			public int						Rank;		// rang de la ligne (0..n)
			public Drawing.Point			Pos;		// position sur la ligne de base
			public double					Width;		// largeur occupée par la ligne
			public double					Height;		// interligne
			public double					Ascender;	// hauteur en-dessus de la ligne de base (+)
			public double					Descender;	// hauteur en-dessous de la ligne de base (-)
			public bool						Visible;
		}

		//	Descripteur d'une partie comprise entre 2 tabulateurs.
		protected class TabPart
		{
			public bool						ListEnding;
			public int						Index;
			public string					Text;
			public Drawing.TextBreak.XRun[] Runs;
		}

		public class Context
		{
			public Context(TextLayout textLayout)
			{
				this.textLayout = textLayout;
			}

			public void CopyTo(Context context)
			{
				context.cursorFrom     = this.cursorFrom;
				context.cursorTo       = this.cursorTo;
				context.CursorAfter    = this.CursorAfter;
				context.CursorLine     = this.CursorLine;
				context.CursorPosX     = this.CursorPosX;
				context.PrepareOffset  = this.PrepareOffset;
				context.PrepareLength1 = this.PrepareLength1;
				context.PrepareLength2 = this.PrepareLength2;
				context.MaxChar        = this.MaxChar;
				context.UndoSeparator  = this.UndoSeparator;
			}

			public static Context Copy(Context context)
			{
				Context copy = new Context(context.textLayout);
				context.CopyTo(copy);
				return copy;
			}

			public int CursorFrom
			{
				get
				{
					return this.cursorFrom;
				}

				set
				{
					this.cursorFrom = value;
					this.UndoSeparator = true;

					if ( this.PrepareOffset != -1 )
					{
						textLayout.Simplify(this);
					}
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
					this.UndoSeparator = true;

					if ( this.PrepareOffset != -1 )
					{
						textLayout.Simplify(this);
					}
				}
			}

			protected TextLayout			textLayout;
			protected int					cursorFrom     = 0;
			protected int					cursorTo       = 0;
			public bool						CursorAfter    = false;
			public int						CursorLine     = 0;
			public double					CursorPosX     = 0;
			public int						PrepareOffset  = -1;
			public int						PrepareLength1 = 0;
			public int						PrepareLength2 = 0;
			public int						MaxChar        = 1000;
			public bool						UndoSeparator;
		}

		public class SelectedArea
		{
			public Drawing.Rectangle		Rect;
			public Drawing.Color			Color;
		}
		
		public event AnchorEventHandler			Anchor;
		
		
		protected void CloneStyleIfDefaultStyleInUse()
		{
			if ( this.style.IsDefaultStyle )
			{
				this.style = new Drawing.TextStyle();
				this.style.Changed += new Support.EventHandler(this.HandleTextStyleChanged);
			}
		}
		
		private void HandleTextStyleChanged(object sender)
		{
			this.MarkContentsAsDirty();
		}


		protected static bool IsDefaultFontName(string name)
		{
			return name.StartsWith(TextLayout.CodeDefault.ToString());
		}
		
		protected static string FontName(string name)
		{
			if ( TextLayout.IsDefaultFontName(name) )
			{
				return name.Substring(1);
			}
			return name;
		}


		protected Widget						embedder;
		protected Support.ResourceManager		resourceManager;
		protected Drawing.TextStyle				style;
		
		protected bool							isContentsDirty;
		protected bool							isSimpleDirty;
		protected bool							isLayoutDirty;
		protected bool							isPrepareDirty;
		protected string						text;
		protected string						simpleText;
		protected string						simplifiedText;
		protected int							textChangeNotificationSuspendCounter;
		protected Drawing.Size					layoutSize;
		protected double						drawingScale;
		protected double						verticalMark;
		protected int							totalLine;
		protected int							visibleLine;
		protected System.Collections.ArrayList  parts  = new System.Collections.ArrayList();
		protected System.Collections.ArrayList  tabs   = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	blocks = new System.Collections.ArrayList();
		protected System.Collections.ArrayList	lines  = new System.Collections.ArrayList();
		
		public const double						Infinite		= 1000000;
		
		protected const char					CodeDefault		= '*';
		public const char						CodeNull		= '\u0000';
		public const char						CodeSpace		= '\u0020';
		public const char						CodeObject		= '\ufffc';
		public const char						CodeLineBreak	= '\u2028';
		public const char						CodeTab			= '\u0009';
		public const char						CodeEndOfText	= '\u0003';
	}
}
