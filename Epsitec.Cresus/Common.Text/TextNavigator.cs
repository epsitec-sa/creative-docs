//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	using OpletEventHandler = Epsitec.Common.Support.OpletEventHandler;
	using OpletEventArgs	= Epsitec.Common.Support.OpletEventArgs;
	
	/// <summary>
	/// La classe TextNavigator permet de manipuler un TextStory en vue de son
	/// édition.
	/// </summary>
	public class TextNavigator : System.IDisposable
	{
		public TextNavigator(TextStory story) : this (story, null)
		{
			this.fitter = new TextFitter (this.story);
		}
		
		public TextNavigator(TextStory story, TextFitter fitter)
		{
			this.story  = story;
			this.fitter = fitter;
			this.cursor = new Cursors.SimpleCursor ();
			
			this.story.NewCursor (this.cursor);
			
			this.story.OpletExecuted += new OpletEventHandler (this.HandleStoryOpletExecuted);
		}
		
		
		public int								TextLength
		{
			get
			{
				return this.story.TextLength;
			}
		}
		
		public Context							TextContext
		{
			get
			{
				return this.story.TextContext;
			}
		}
		
		public TextStyle[]						TextStyles
		{
			get
			{
				if ((this.current_styles == null) ||
					(this.current_properties == null))
				{
					this.UpdateCurrentStylesAndProperties ();
				}
				
				return this.current_styles.Clone () as TextStyle[];
			}
		}
		
		public Property[]						TextProperties
		{
			get
			{
				if ((this.current_styles == null) ||
					(this.current_properties == null))
				{
					this.UpdateCurrentStylesAndProperties ();
				}
				
				return this.current_properties.Clone () as Property[];
			}
		}
		
		public int								CursorPosition
		{
			get
			{
				return this.story.GetCursorPosition (this.ActiveCursor);
			}
		}
		
		public int								CursorDirection
		{
			get
			{
				return this.story.GetCursorDirection (this.ActiveCursor);
			}
		}
		
		public bool								IsSelectionActive
		{
			get
			{
				return this.active_selection_cursor == null ? false : true;
			}
		}
		
		
		public Common.Support.OpletQueue		OpletQueue
		{
			get
			{
				return this.story.OpletQueue;
			}
		}
		
		
		protected ICursor						ActiveCursor
		{
			get
			{
				if (this.active_selection_cursor != null)
				{
					return this.active_selection_cursor;
				}
				else
				{
					return this.cursor;
				}
			}
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		public void Insert(string text)
		{
			ulong[] styled_text;
			
			if ((this.current_styles == null) ||
				(this.current_properties == null))
			{
				this.UpdateCurrentStylesAndProperties ();
			}
			
			this.story.ConvertToStyledText (text, this.current_styles, this.current_properties, out styled_text);
			this.story.InsertText (this.cursor, styled_text);
		}
		
		public void Delete()
		{
			//	Supprime le contenu de la sélection (pour autant qu'il y en ait
			//	une qui soit définie).
			
			if (this.selection_cursors != null)
			{
				Internal.TextTable text = this.story.TextTable;
				
				using (this.story.OpletQueue.BeginAction ())
				{
					this.InternalInsertSelectionOplet ();
					
					for (int i = 0; i < this.selection_cursors.Count; i += 2)
					{
						//	Traite les tranches dans l'ordre, en les détruisant les
						//	unes après les autres.
						
						ICursor c1 = this.selection_cursors[i+0] as ICursor;
						ICursor c2 = this.selection_cursors[i+1] as ICursor;
						
						int p1 = text.GetCursorPosition (c1.CursorId);
						int p2 = text.GetCursorPosition (c2.CursorId);
						
						if (p1 > p2)
						{
							ICursor cc = c1;
							int     pp = p1;
							
							p1 = p2;	c1 = c2;
							p2 = pp;	c2 = cc;
						}
						
						if (i+2 == this.selection_cursors.Count)
						{
							//	C'est la dernière tranche. Il faut positionner le curseur
							//	de travail au début de la zone et hériter des styles actifs
							//	à cet endroit :
							
							this.story.SetCursorPosition (this.cursor, p1, 0);
							this.UpdateCurrentStylesAndProperties ();
						}
						
						this.story.DeleteText (c1, p2-p1);
					}
					
					this.story.OpletQueue.ValidateAction ();
				}
				
				this.InternalClearSelection ();
				this.UpdateSelectionMarkers ();
			}
		}
		
		public void MoveTo(Target target, int count)
		{
			System.Diagnostics.Debug.Assert (count >= 0);
			
			int old_pos = this.CursorPosition;
			int old_dir = this.CursorDirection;
			
			switch (target)
			{
				case Target.CharacterNext:
					this.MoveCursor (count);
					break;
				
				case Target.CharacterPrevious:
					this.MoveCursor (-count);
					break;
				
				case Target.TextStart:
					this.story.SetCursorPosition (this.ActiveCursor, 0, -1);
					break;
				
				case Target.TextEnd:
					this.story.SetCursorPosition (this.ActiveCursor, this.TextLength, 1);
					break;
					
				case Target.ParagraphStart:
					this.MoveCursor (count, -1, new MoveCallback (this.IsParagraphStart));
					break;
				
				case Target.ParagraphEnd:
					this.MoveCursor (count, 1, new MoveCallback (this.IsParagraphEnd));
					break;
				
				case Target.LineStart:
					this.MoveCursor (count, -1, new MoveCallback (this.IsLineStart));
					break;
				
				case Target.LineEnd:
					this.MoveCursor (count, 1, new MoveCallback (this.IsLineEnd));
					break;
				
				case Target.WordStart:
					this.MoveCursor (count, -1, new MoveCallback (this.IsWordStart));
					break;
				
				case Target.WordEnd:
					this.MoveCursor (count, 1, new MoveCallback (this.IsWordEnd));
					break;
			}
			
			int new_pos = this.CursorPosition;
			int new_dir = this.CursorDirection;
			
			if ((old_pos != new_pos) ||
				(old_dir != new_dir))
			{
				if (this.IsSelectionActive)
				{
					this.UpdateSelectionMarkers ();
				}
				else
				{
					this.UpdateCurrentStylesAndProperties ();
				}
				
				this.OnCursorMoved ();
			}
		}
		
		
		public void StartSelection()
		{
			System.Diagnostics.Debug.Assert (! this.IsSelectionActive);
			
			Cursors.SelectionCursor c1 = this.NewSelectionCursor ();
			Cursors.SelectionCursor c2 = this.NewSelectionCursor ();
			
			this.selection_cursors.Add (c1);
			this.selection_cursors.Add (c2);
			
			int position = this.story.GetCursorPosition (this.cursor);
			
			this.story.SetCursorPosition (c1, position);
			this.story.SetCursorPosition (c2, position);
			
			this.active_selection_cursor = c2;
		}
		
		public void EndSelection()
		{
			System.Diagnostics.Debug.Assert (this.IsSelectionActive);
			
			this.active_selection_cursor = null;
		}
		
		public void ClearSelection()
		{
			//	Désélectionne tout le texte.
			
			if (this.selection_cursors != null)
			{
				//	Prend note de la position des curseurs de sélection pour
				//	pouvoir restaurer la sélection en cas de UNDO :
				
				using (this.story.OpletQueue.BeginAction ())
				{
					this.InternalInsertSelectionOplet ();
					this.story.OpletQueue.ValidateAction ();
				}
				
				this.InternalClearSelection ();
				this.UpdateSelectionMarkers ();
			}
		}
		
		
		public string[] GetSelectedTexts()
		{
			//	Retourne les textes sélectionnés, bruts, sans aucun formatage.
			//	S'il n'y en a pas, retourne un tableau vide.
			
			string[] texts;
			
			if (this.selection_cursors == null)
			{
				texts = new string[0];
			}
			else
			{
				int[] positions = this.GetSelectionCursorPositions ();
				
				texts = new string[positions.Length / 2];
				
				for (int i = 0; i < positions.Length; i += 2)
				{
					int p1 = positions[i+0];
					int p2 = positions[i+1];
					
					ICursor c1 = this.selection_cursors[i+0] as ICursor;
					ICursor c2 = this.selection_cursors[i+1] as ICursor;
					
					if (p1 > p2)
					{
						int     pp = p1; p1 = p2; p2 = pp;
						ICursor cc = c1; c1 = c2; c2 = cc;
					}
					
					string  text;
					ulong[] buffer = new ulong[p2-p1];
					
					this.story.ReadText (c1, p2-p1, buffer);
					
					TextConverter.ConvertToString (buffer, out text);
					
					texts[i/2] = text;
				}
			}
			
			return texts;
		}
		
		
		public void SetStyle(TextStyle style)
		{
			TextStyle[] styles     = new TextStyle[1];
			Property[]  properties = new Property[0];
			
			this.SetStyles (styles, properties);
		}
		
		public void SetStyle(TextStyle style, params Property[] properties)
		{
			TextStyle[] styles = new TextStyle[1];
			
			this.SetStyles (styles, properties);
		}
		
		public void SetStyles(System.Collections.ICollection styles, System.Collections.ICollection properties)
		{
			TextStyle[] s_array = new TextStyle[styles == null ? 0 : styles.Count];
			Property[]  p_array = new Property[properties == null ? 0 : properties.Count];
			
			if (styles != null) styles.CopyTo (s_array, 0);
			if (properties != null) properties.CopyTo (p_array, 0);
			
			this.SetStyles (s_array, p_array);
		}
		
		public void SetStyles(TextStyle[] styles, Property[] properties)
		{
			//	Change les styles et propriétés attachées à la position courante,
			//	ce qui va peut-être modifier les propriétés du paragraphe. En cas
			//	de sélection, la gestion est plus compliquée.
			
			//	TODO: gérer la sélection
			
			Property[] paragraph_properties = Property.FilterUniformParagraphProperties (properties);
			Property[] character_properties = Property.FilterOtherProperties (properties);
			
			TextStyle[] paragraph_styles = TextStyle.FilterStyles (styles, TextStyleClass.Paragraph);
			TextStyle[] character_styles = TextStyle.FilterStyles (styles, TextStyleClass.Text, TextStyleClass.Character);
			
			Internal.Navigator.SetParagraphStylesAndProperties (this.story, this.cursor, paragraph_styles, paragraph_properties);
			
			this.current_styles     = styles.Clone () as TextStyle[];
			this.current_properties = properties.Clone () as Property[];
		}
		
		
		public void GetCursorGeometry(out ITextFrame frame, out double cx, out double cy, out double ascender, out double descender, out double angle)
		{
			int para_line;
			int line_char;
			
			this.fitter.GetCursorGeometry (this.ActiveCursor, out frame, out cx, out cy, out para_line, out line_char);
			
			System.Collections.ArrayList properties = this.story.FlattenStylesAndProperties (this.current_styles, this.current_properties);
			
			if ((this.CursorPosition == this.story.TextLength) &&
				(para_line == 0) &&
				(line_char == 0))
			{
				//	Cas particulier : le curseur se trouve tout seul en fin de pavé,
				//	sans aucun autre caractère dans la ligne.
				
				Properties.MarginsProperty margins = null;
				
				foreach (Property property in properties)
				{
					if (property is Properties.MarginsProperty)
					{
						margins = property as Properties.MarginsProperty;
						break;
					}
				}
				
				double ox;
				double oy;
				double width;
				double next_y;
				
				frame.MapFromView (ref cx, ref cy);
				frame.ConstrainLineBox (cy, 0, 0, 0, 0, false, out ox, out oy, out width, out next_y);
				
				double mx1 = margins.LeftMarginFirstLine;
				double mx2 = margins.RightMarginFirstLine;
				double disposition = margins.Disposition;
				
				width -= mx1;
				width -= mx2;
				
				cx += mx1;
				cx += width * disposition;
				
				frame.MapToView (ref cx, ref cy);
			}
			
			Properties.FontProperty       font = null;
			Properties.FontSizeProperty   font_size = null;
			Properties.FontOffsetProperty font_offset = null;
			
			foreach (Property property in properties)
			{
				if (property is Properties.FontProperty)
				{
					font = property as Properties.FontProperty;
				}
				else if (property is Properties.FontSizeProperty)
				{
					font_size = property as Properties.FontSizeProperty;
				}
				else if (property is Properties.FontOffsetProperty)
				{
					font_offset = property as Properties.FontOffsetProperty;
				}
			}
			
			OpenType.Font ot_font;
			double        pt_size = font_size.SizeInPoints;
			
			this.story.TextContext.GetFont (font, out ot_font);
			
			ascender  = ot_font.GetAscender (pt_size);
			descender = ot_font.GetDescender (pt_size);
			angle     = ot_font.GetCaretAngle ();
			
			if (font_offset != null)
			{
				frame.MapFromView (ref cx, ref cy);
				cy += font_offset.Offset;
				frame.MapToView (ref cx, ref cy);
			}
		}
		
		
		protected virtual bool MoveCursor(int distance)
		{
			int count;
			int direction;
			int moved = 0;
			
			if (distance > 0)
			{
				count     = distance;
				direction = 1;
			}
			else
			{
				count     = -distance;
				direction = -1;
			}
			
			Context            context    = this.TextContext;
			Internal.TextTable text_table = this.story.TextTable;
			StyleList          style_list = context.StyleList;
			
			while (moved < count)
			{
				int pos = this.story.GetCursorPosition (this.ActiveCursor);
				
				if ((direction > 0) &&
					(pos == this.story.TextLength))
				{
					break;
				}
				if ((direction < 0) &&
					(pos == 0))
				{
					break;
				}
				
				//	Déplace le curseur dans la direction choisie, puis vérifie si
				//	l'on n'a pas atterri dans un fragment de texte marqué comme
				//	étant un texte automatique ou un texte produit par un générateur.
				
				//	TODO: utiliser un curseur temporaire pour les déplacements
				
				this.story.MoveCursor (this.ActiveCursor, direction);
				
				ulong code = text_table.ReadChar (this.ActiveCursor.CursorId);
				
				Styles.SimpleStyle simple_style = style_list[code];
				
				if (simple_style.Contains (code, Properties.WellKnownType.AutoText, Properties.PropertyType.ExtraSetting))
				{
					System.Diagnostics.Debug.Assert (simple_style.FindProperties (Properties.WellKnownType.AutoText).Length == 1);
					
					Property property = simple_style[Properties.WellKnownType.AutoText];
					
					this.SkipOverProperty (property, direction);
					
					if ((direction < 0) &&
						(this.story.GetCursorPosition (this.ActiveCursor) == 0))
					{
						//	Arrivé au début du texte en ayant sauté par-dessus un texte
						//	automatique; on n'a pas le droit de laisser le curseur ici,
						//	alors on le remet à sa position initiale et on s'arrête...
						
						this.story.SetCursorPosition (this.ActiveCursor, pos, -1);
						break;
					}
					
					//	Ne compte pas un texte automatique comme 'caractère sauté'.
					
					continue;
				}
				else if (simple_style.Contains (code, Properties.WellKnownType.Generator, Properties.PropertyType.ExtraSetting))
				{
					System.Diagnostics.Debug.Assert (simple_style.FindProperties (Properties.WellKnownType.Generator).Length == 1);
					
					Property property = simple_style[Properties.WellKnownType.Generator];
					
					this.SkipOverProperty (property, direction);
					
					//	Un texte produit par un générateur compte comme un caractère
					//	unique.
				}
				
				moved++;
			}
			
			return moved > 0;
		}
		
		protected virtual bool MoveCursor(int count, int direction, MoveCallback callback)
		{
			int moved   = 0;
			int old_pos = this.CursorPosition;
			int old_dir = this.CursorDirection;
			
			Context            context    = this.TextContext;
			Internal.TextTable text_table = this.story.TextTable;
			StyleList          style_list = context.StyleList;
			
			System.Diagnostics.Debug.Assert (count >= 0);
			System.Diagnostics.Debug.Assert ((direction == -1) || (direction == 1));
			
			if (direction > 0)
			{
				int max = this.story.TextLength - old_pos;
				
				for (int i = 0; i < max; i++)
				{
					if (callback (i))
					{
						if (count-- == 0)
						{
							break;
						}
					}
					else if ((i == 0) && (count > 0))
					{
						count--;
					}
					
					moved++;
				}
			}
			else
			{
				int max = old_pos;
				
				for (int i = 0; i < max; i++)
				{
					if (callback (-i))
					{
						if (count-- == 0)
						{
							break;
						}
					}
					else if ((i == 0) && (count > 0))
					{
						count--;
					}
					
					moved--;
				}
			}
			
			int new_pos = old_pos + moved;
			int new_dir = direction;
			
			if ((new_pos != old_pos) ||
				(new_dir != old_dir))
			{
				this.story.SetCursorPosition (this.ActiveCursor, new_pos, new_dir);
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
		protected virtual bool IsParagraphStart(int offset)
		{
			return Internal.Navigator.IsParagraphStart (this.story, this.ActiveCursor, offset);
		}
		
		protected virtual bool IsParagraphEnd(int offset)
		{
			return Internal.Navigator.IsParagraphEnd (this.story, this.ActiveCursor, offset);
		}
		
		protected virtual bool IsWordStart(int offset)
		{
			return Internal.Navigator.IsWordStart (this.story, this.ActiveCursor, offset);
		}
		
		protected virtual bool IsWordEnd(int offset)
		{
			//	Si nous sommes à la fin d'un paragraphe ou d'une ligne, nous
			//	sommes déjà à une fin de mot...
			
			//	TODO: gérer fins de lignes
			
			if (Internal.Navigator.IsParagraphEnd (this.story, this.ActiveCursor, offset))
			{
				return true;
			}
			
			//	On détermine que la fin d'un mot est la même chose que le début
			//	du mot suivant, pour la navigation :
			
			return Internal.Navigator.IsWordStart (this.story, this.ActiveCursor, offset);
		}
		
		protected virtual bool IsLineStart(int offset)
		{
			if (this.IsParagraphStart (offset))
			{
				return true;
			}
			
			if (Internal.Navigator.IsLineStart (this.story, this.fitter, this.ActiveCursor, offset))
			{
				return true;
			}
			
			return false;
		}
		
		protected virtual bool IsLineEnd(int offset)
		{
			if (this.IsParagraphEnd (offset))
			{
				return true;
			}
			
			if (Internal.Navigator.IsLineEnd (this.story, this.fitter, this.ActiveCursor, offset))
			{
				return true;
			}
			
			return false;
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.story != null)
				{
					this.InternalClearSelection ();
					this.UpdateSelectionMarkers ();
					
					this.story.OpletExecuted -= new OpletEventHandler (this.HandleStoryOpletExecuted);
					this.story.RecycleCursor (this.cursor);
					
					this.story  = null;
					this.cursor = null;
				}
			}
		}
		
		
		private void InternalInsertSelectionOplet()
		{
			int[] positions = this.GetSelectionCursorPositions ();
			this.story.OpletQueue.Insert (new ClearSelectionOplet (this, positions));
		}
		
		private void InternalClearSelection()
		{
			if (this.selection_cursors != null)
			{
				foreach (Cursors.SelectionCursor cursor in this.selection_cursors)
				{
					this.RecycleSelectionCursor (cursor);
				}
				
				this.selection_cursors.Clear ();
				this.selection_cursors = null;
				
				this.active_selection_cursor = null;
			}
		}
		
		private void InternalDefineSelection(int[] positions)
		{
			this.InternalClearSelection ();
			
			System.Diagnostics.Debug.Assert ((positions.Length % 2) == 0);
			
			for (int i = 0; i < positions.Length; i += 2)
			{
				Cursors.SelectionCursor c1 = this.NewSelectionCursor ();
				Cursors.SelectionCursor c2 = this.NewSelectionCursor ();
				
				this.selection_cursors.Add (c1);
				this.selection_cursors.Add (c2);
				
				this.story.SetCursorPosition (c1, positions[i+0]);
				this.story.SetCursorPosition (c2, positions[i+1]);
			}
		}
		
		
		protected virtual void SkipOverProperty(Property property, int direction)
		{
			//	Saute la propriété, en marche avant ou en marche arrière. En cas
			//	de marche avant, on s'arrête après la tranche. En cas de marche
			//	arrière, on s'arrête juste au début de la tranche.
			
			if (direction < 0)
			{
				//	La distance au début de la tranche de texte va de 0 à -n.
				
				int distance = Internal.Navigator.GetRunStartOffset (this.story, this.ActiveCursor, property);
				this.story.MoveCursor (this.ActiveCursor, distance);
			}
			else if (direction > 0)
			{
				//	La distance à la fin de la tranche de texte va de 1 à n.
				
				int distance = Internal.Navigator.GetRunEndLength (this.story, this.ActiveCursor, property);
				this.story.MoveCursor (this.ActiveCursor, distance);
			}
		}
		
		
		protected Cursors.SelectionCursor NewSelectionCursor()
		{
			//	Retourne un curseur utilisable pour une sélection. S'il existe
			//	encore des zombies, on les retourne à la vie plutôt que de
			//	créer de nouveaux curseurs.
			
			if (this.selection_cursors == null)
			{
				this.selection_cursors = new System.Collections.ArrayList ();
			}
			
			Cursors.SelectionCursor cursor = new Cursors.SelectionCursor ();
			
			this.story.NewCursor (cursor);
			
			return cursor;
		}
		
		protected void RecycleSelectionCursor(Cursors.SelectionCursor cursor)
		{
			this.story.RecycleCursor (cursor);
		}
		
		
		protected virtual void UpdateCurrentStylesAndProperties()
		{
			System.Collections.ArrayList styles     = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			if (this.TextLength == 0)
			{
				styles.Add (this.TextContext.DefaultStyle);
			}
			else
			{
				//	En marche arrière, on utilise le style du caractère courant, alors
				//	qu'en marche avant, on utilise le style du caractère précédent :
				
				int pos    = this.story.GetCursorPosition (this.cursor);
				int dir    = this.story.GetCursorDirection (this.cursor);
				int offset = ((pos > 0) && (dir > 0)) ? -1 : 0;
				
				Internal.Navigator.GetStyles (this.story, this.cursor, offset, styles);
				Internal.Navigator.GetProperties (this.story, this.cursor, offset, properties);
			}
			
			int n_styles     = styles.Count;
			int n_properties = properties.Count;
			
			this.current_styles     = new TextStyle[n_styles];
			this.current_properties = new Property[n_properties];
			
			styles.CopyTo (this.current_styles);
			properties.CopyTo (this.current_properties);
		}
		
		protected virtual void UpdateSelectionMarkers()
		{
			//	Met à jour les marques de sélection dans le texte. On va opérer
			//	en deux passes; d'abord on les enlève toutes, ensuite on génère
			//	celles comprises entre deux marques de sélection.
			
			ulong marker = this.TextContext.Markers.Selected;
			
			this.story.ChangeAllMarkers (marker, false);
			
			int[] positions = this.GetSelectionCursorPositions ();
			
			for (int i = 0; i < positions.Length; i += 2)
			{
				int p1 = positions[i+0];
				int p2 = positions[i+1];
				
				if (p1 > p2)
				{
					int pp = p1; p1 = p2; p2 = pp;
				}
					
				this.story.ChangeMarkers (p1, p2-p1, marker, true);
			}
		}
		
		
		private int[] GetSelectionCursorPositions()
		{
			int[] positions;
			
			if (this.selection_cursors == null)
			{
				positions = new int[0];
			}
			else
			{
				positions = new int[this.selection_cursors.Count];
				
				for (int i = 0; i < this.selection_cursors.Count; i++)
				{
					ICursor cursor = this.selection_cursors[i] as ICursor;
					
					positions[i] = this.story.GetCursorPosition (cursor);
				}
			}
			
			System.Diagnostics.Debug.Assert ((positions.Length % 2) == 0);
			
			return positions;
		}

		
		protected virtual void OnCursorMoved()
		{
		}
		
		
		private void HandleStoryOpletExecuted(object sender, OpletEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.story == sender);
			
			TextStory.ICursorOplet cursor_oplet = e.Oplet as TextStory.ICursorOplet;
			
			if ((cursor_oplet != null) &&
				(cursor_oplet.Cursor == this.cursor))
			{
				this.UpdateCurrentStylesAndProperties ();
			}
		}
		
		
		#region ClearSelectionOplet Class
		protected class ClearSelectionOplet : Common.Support.AbstractOplet
		{
			public ClearSelectionOplet(TextNavigator navigator, int[] positions)
			{
				this.navigator = navigator;
				this.positions = positions;
			}
			
			
			public override Epsitec.Common.Support.IOplet Undo()
			{
				this.navigator.InternalDefineSelection (this.positions);
				this.navigator.UpdateSelectionMarkers ();
				
				return this;
			}
			
			public override Epsitec.Common.Support.IOplet Redo()
			{
				this.navigator.InternalClearSelection ();
				this.navigator.UpdateSelectionMarkers ();
				
				return this;
			}
			
			public override void Dispose()
			{
				base.Dispose ();
			}
			
			
			private TextNavigator				navigator;
			private int[]						positions;
		}
		#endregion
		
		#region Target Enumeration
		public enum Target
		{
			None,
			
			CharacterNext,
			CharacterPrevious,
			
			TextStart,
			TextEnd,
			
			ParagraphStart,
			ParagraphEnd,
			
			LineStart,
			LineEnd,
			
			WordStart,
			WordEnd,
		}
		#endregion
		
		protected delegate bool MoveCallback(int offset);
		
		
		private TextStory						story;
		private TextFitter						fitter;
		private Cursors.SimpleCursor			cursor;
		private Cursors.SelectionCursor			active_selection_cursor;
		private System.Collections.ArrayList	selection_cursors;
		
		private TextStyle[]						current_styles;
		private Property[]						current_properties;
	}
}
