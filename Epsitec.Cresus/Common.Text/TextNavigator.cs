//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
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
					this.UpdateCurrentStylesAndProperties (0);
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
					this.UpdateCurrentStylesAndProperties (0);
				}
				
				return this.current_properties.Clone () as Property[];
			}
		}
		
		public int								CursorPosition
		{
			get
			{
				return this.story.GetCursorPosition (this.cursor);
			}
		}
		
		public int								CursorDirection
		{
			get
			{
				return this.current_direction;
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
				this.UpdateCurrentStylesAndProperties (0);
			}
			
			this.story.ConvertToStyledText (text, this.current_styles, this.current_properties, out styled_text);
			this.story.InsertText (this.cursor, styled_text);
		}
		
		public void MoveTo(Target target, int count)
		{
			System.Diagnostics.Debug.Assert (count >= 0);
			
			int old_pos   = this.CursorPosition;
			int direction = 0;
			
			switch (target)
			{
				case Target.NextCharacter:
					this.MoveCursor (count);
					direction = 1;
					break;
				
				case Target.PreviousCharacter:
					this.MoveCursor (-count);
					direction = -1;
					break;
				
				case Target.TextStart:
					this.story.SetCursorPosition (this.cursor, 0);
					direction = -1;
					break;
				
				case Target.TextEnd:
					this.story.SetCursorPosition (this.cursor, this.TextLength);
					direction = 1;
					break;
					
				case Target.ParagraphStart:
					this.MoveCursor (count, -1, new MoveCallback (this.IsParagraphStart));
					direction = -1;
					break;
				
				case Target.ParagraphEnd:
					this.MoveCursor (count, 1, new MoveCallback (this.IsParagraphEnd));
					direction = 1;
					break;
				
				case Target.LineStart:
					this.MoveCursor (count, -1, new MoveCallback (this.IsLineStart));
					direction = -1;
					break;
				
				case Target.LineEnd:
					this.MoveCursor (count, 1, new MoveCallback (this.IsLineEnd));
					direction = 1;
					break;
				
				case Target.WordStart:
					this.MoveCursor (count, -1, new MoveCallback (this.IsWordStart));
					direction = -1;
					break;
				
				case Target.WordEnd:
					this.MoveCursor (count, 1, new MoveCallback (this.IsWordEnd));
					direction = 1;
					break;
			}
			
			int new_pos = this.CursorPosition;
			
			if ((old_pos != new_pos) ||
				(direction != this.current_direction))
			{
				this.UpdateCurrentStylesAndProperties (direction);
				this.OnCursorMoved ();
			}
		}
		
		
		public void Deselect()
		{
			//	Désélectionne tout le texte.
			
			if (this.selection_cursors != null)
			{
				foreach (Cursors.SelectionCursor cursor in this.selection_cursors)
				{
					this.story.RecycleCursor (cursor);
				}
				
				this.selection_cursors.Clear ();
				this.selection_cursors = null;
				
				this.UpdateSelectionMarkers ();
			}
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
				int pos = this.story.GetCursorPosition (this.cursor);
				
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
				
				this.story.MoveCursor (this.cursor, direction);
				
				ulong code = text_table.ReadChar (this.cursor.CursorId);
				
				Styles.SimpleStyle simple_style = style_list[code];
				
				if (simple_style.Contains (code, Properties.WellKnownType.AutoText, Properties.PropertyType.ExtraSetting))
				{
					System.Diagnostics.Debug.Assert (simple_style.FindProperties (Properties.WellKnownType.AutoText).Length == 1);
					
					Property property = simple_style[Properties.WellKnownType.AutoText];
					
					this.SkipOverProperty (property, direction);
					
					if ((direction < 0) &&
						(this.story.GetCursorPosition (this.cursor) == 0))
					{
						//	Arrivé au début du texte en ayant sauté par-dessus un texte
						//	automatique; on n'a pas le droit de laisser le curseur ici,
						//	alors on le remet à sa position initiale et on s'arrête...
						
						this.story.SetCursorPosition (this.cursor, pos);
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
			int moved = 0;
			int pos   = this.story.GetCursorPosition (this.cursor);
			
			Context            context    = this.TextContext;
			Internal.TextTable text_table = this.story.TextTable;
			StyleList          style_list = context.StyleList;
			
			System.Diagnostics.Debug.Assert (count >= 0);
			System.Diagnostics.Debug.Assert ((direction == -1) || (direction == 1));
			
			if (direction > 0)
			{
				int max = this.story.TextLength - pos;
				
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
				int max = pos;
				
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
			
			if (moved != 0)
			{
				this.story.MoveCursor (this.cursor, moved);
			}
			
			return moved != 0;
		}
		
		
		protected virtual bool IsParagraphStart(int offset)
		{
			return Internal.Navigator.IsParagraphStart (this.story, this.cursor, offset);
		}
		
		protected virtual bool IsParagraphEnd(int offset)
		{
			return Internal.Navigator.IsParagraphEnd (this.story, this.cursor, offset);
		}
		
		protected virtual bool IsWordStart(int offset)
		{
			return Internal.Navigator.IsWordStart (this.story, this.cursor, offset);
		}
		
		protected virtual bool IsWordEnd(int offset)
		{
			//	Si nous sommes à la fin d'un paragraphe ou d'une ligne, nous
			//	sommes déjà à une fin de mot...
			
			//	TODO: gérer fins de lignes
			
			if (Internal.Navigator.IsParagraphEnd (this.story, this.cursor, offset))
			{
				return true;
			}
			
			//	On détermine que la fin d'un mot est la même chose que le début
			//	du mot suivant, pour la navigation :
			
			return Internal.Navigator.IsWordStart (this.story, this.cursor, offset);
		}
		
		protected virtual bool IsLineStart(int offset)
		{
			if (this.IsParagraphStart (offset))
			{
				return true;
			}
			
			if (Internal.Navigator.IsLineStart (this.story, this.fitter, this.cursor, offset))
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
			
			if (Internal.Navigator.IsLineEnd (this.story, this.fitter, this.cursor, offset))
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
					this.story.RecycleCursor (this.cursor);
					
					this.story  = null;
					this.cursor = null;
				}
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
				
				int distance = Internal.Navigator.GetRunStartOffset (this.story, this.cursor, property);
				this.story.MoveCursor (this.cursor, distance);
			}
			else if (direction > 0)
			{
				//	La distance à la fin de la tranche de texte va de 1 à n.
				
				int distance = Internal.Navigator.GetRunEndLength (this.story, this.cursor, property);
				this.story.MoveCursor (this.cursor, distance);
			}
		}
		
		
		protected virtual void UpdateCurrentStylesAndProperties(int direction)
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
				int offset = ((pos > 0) && (direction > 0)) ? -1 : 0;
				
				Internal.Navigator.GetStyles (this.story, this.cursor, offset, styles);
				Internal.Navigator.GetProperties (this.story, this.cursor, offset, properties);
			}
			
			int n_styles     = styles.Count;
			int n_properties = properties.Count;
			
			this.current_styles     = new TextStyle[n_styles];
			this.current_properties = new Property[n_properties];
			this.current_direction  = direction;
			
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
			
			if (this.selection_cursors != null)
			{
				this.selection_cursors.Sort (Cursors.SimpleCursor.GetPositionComparer (this.story));
				
				System.Diagnostics.Debug.Assert ((this.selection_cursors.Count % 2) == 0);
				
				for (int i = 0; i < this.selection_cursors.Count; i += 2)
				{
					ICursor c1 = this.selection_cursors[i+0] as ICursor;
					ICursor c2 = this.selection_cursors[i+1] as ICursor;
					
					int p1 = this.story.GetCursorPosition (c1);
					int p2 = this.story.GetCursorPosition (c2);
					
					this.story.ChangeMarkers (c1, p2-p1, marker, true);
				}
			}
		}

		
		protected virtual void OnCursorMoved()
		{
		}
		
		
		#region Target Enumeration
		public enum Target
		{
			None,
			
			NextCharacter,
			PreviousCharacter,
			
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
		private System.Collections.ArrayList	selection_cursors;
		
		private TextStyle[]						current_styles;
		private Property[]						current_properties;
		private int								current_direction;
	}
}
