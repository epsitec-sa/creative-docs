//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextNavigator permet de manipuler un TextStory en vue de son
	/// �dition.
	/// </summary>
	public class TextNavigator : System.IDisposable
	{
		public TextNavigator(TextStory story)
		{
			this.story  = story;
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
		
		public int								CursorPosition
		{
			get
			{
				return this.story.GetCursorPosition (this.cursor);
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
					direction = -1;
					break;
				
				case Target.LineEnd:
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
			
			if (old_pos != new_pos)
			{
				this.UpdateCurrentStylesAndProperties (direction);
				this.OnCursorMoved ();
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
				
				//	D�place le curseur dans la direction choisie, puis v�rifie si
				//	l'on n'a pas atterri dans un fragment de texte marqu� comme
				//	�tant un texte automatique ou un texte produit par un g�n�rateur.
				
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
						//	Arriv� au d�but du texte en ayant saut� par-dessus un texte
						//	automatique; on n'a pas le droit de laisser le curseur ici,
						//	alors on le remet � sa position initiale et on s'arr�te...
						
						this.story.SetCursorPosition (this.cursor, pos);
						break;
					}
					
					//	Ne compte pas un texte automatique comme 'caract�re saut�'.
					
					continue;
				}
				else if (simple_style.Contains (code, Properties.WellKnownType.Generator, Properties.PropertyType.ExtraSetting))
				{
					System.Diagnostics.Debug.Assert (simple_style.FindProperties (Properties.WellKnownType.Generator).Length == 1);
					
					Property property = simple_style[Properties.WellKnownType.Generator];
					
					this.SkipOverProperty (property, direction);
					
					//	Un texte produit par un g�n�rateur compte comme un caract�re
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
			//	Si nous sommes � la fin d'un paragraphe ou d'une ligne, nous
			//	sommes d�j� � une fin de mot...
			
			//	TODO: g�rer fins de lignes
			
			if (Internal.Navigator.IsParagraphEnd (this.story, this.cursor, offset))
			{
				return true;
			}
			
			//	On d�termine que la fin d'un mot est la m�me chose que le d�but
			//	du mot suivant :
			
			return Internal.Navigator.IsWordStart (this.story, this.cursor, offset);
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
			//	Saute la propri�t�, en marche avant ou en marche arri�re. En cas
			//	de marche avant, on s'arr�te apr�s la tranche. En cas de marche
			//	arri�re, on s'arr�te juste au d�but de la tranche.
			
			if (direction < 0)
			{
				//	La distance au d�but de la tranche de texte va de 0 � -n.
				
				int distance = Internal.Navigator.GetRunStartOffset (this.story, this.cursor, property);
				this.story.MoveCursor (this.cursor, distance);
			}
			else if (direction > 0)
			{
				//	La distance � la fin de la tranche de texte va de 1 � n.
				
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
				//	En marche arri�re, on utilise le style du caract�re courant, alors
				//	qu'en marche avant, on utilise le style du caract�re pr�c�dent :
				
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
		
		
		protected virtual void OnCursorMoved()
		{
		}
		
		
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
		
		protected delegate bool MoveCallback(int offset);
		
		
		private TextStory						story;
		private Cursors.SimpleCursor			cursor;
		
		private TextStyle[]						current_styles;
		private Property[]						current_properties;
		private int								current_direction;
	}
}
