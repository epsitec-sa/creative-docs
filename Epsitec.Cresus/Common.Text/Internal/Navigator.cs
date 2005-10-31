//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe Navigator g�re les d�placements au sein d'un texte.
	/// </summary>
	public sealed class Navigator
	{
		public static bool IsParagraphStart(TextStory story, ICursor cursor, int offset)
		{
			Unicode.Code code = Unicode.Bits.GetUnicodeCode (story.ReadChar (cursor, offset - 1));
			
			return (code == Unicode.Code.Null) || Navigator.IsParagraphSeparator (code);
		}
		
		public static bool IsParagraphEnd(TextStory story, ICursor cursor, int offset)
		{
			Unicode.Code code = Unicode.Bits.GetUnicodeCode (story.ReadChar (cursor, offset));
			
			return Navigator.IsParagraphSeparator (code);
		}
		
		public static bool IsParagraphSeparator(Unicode.Code code)
		{
			switch (code)
			{
				case Unicode.Code.LineSeparator:
					return false;
				
				case Unicode.Code.PageSeparator:
				case Unicode.Code.ParagraphSeparator:
					return true;
				
				case Unicode.Code.EndOfText:
					return true;
			}
			
			return false;
		}
		
		public static bool IsParagraphSeparator(ulong code)
		{
			return Navigator.IsParagraphSeparator (Unicode.Bits.GetUnicodeCode (code));
		}
		
		
		public static bool IsEndOfText(TextStory story, ICursor cursor, int offset)
		{
			int pos = story.GetCursorPosition (cursor) + offset;
			
			if (pos >= story.TextLength)
			{
				return true;
			}
			
			Unicode.Code code = Unicode.Bits.GetUnicodeCode (story.ReadChar (cursor, offset));
			
			return Navigator.IsEndOfText (code);
		}
		
		public static bool IsEndOfText(Unicode.Code code)
		{
			return Unicode.Code.EndOfText == code;
		}
		
		public static bool IsEndOfText(ulong code)
		{
			return Navigator.IsEndOfText (Unicode.Bits.GetUnicodeCode (code));
		}
		
		
		public static bool IsWordStart(TextStory story, ICursor cursor, int offset)
		{
			Unicode.Code code_0 = Unicode.Bits.GetUnicodeCode (story.ReadChar (cursor, offset));
			Unicode.Code code_1 = Unicode.Bits.GetUnicodeCode (story.ReadChar (cursor, offset - 1));
			
			if (code_1 == Unicode.Code.Null)
			{
				return true;
			}
			
			if (! Navigator.IsWordSeparator (code_0) &&
				Navigator.IsWordSeparator (code_1))
			{
				return true;
			}
			
			return false;
		}
		
		public static bool IsWordEnd(TextStory story, ICursor cursor, int offset)
		{
			Unicode.Code code_0 = Unicode.Bits.GetUnicodeCode (story.ReadChar (cursor, offset));
			Unicode.Code code_1 = Unicode.Bits.GetUnicodeCode (story.ReadChar (cursor, offset - 1));
			
			if (code_0 == Unicode.Code.Null)
			{
				return true;
			}
			
			if (Navigator.IsWordSeparator (code_0) &&
				! Navigator.IsWordSeparator (code_1))
			{
				return true;
			}
			
			return false;
		}
		
		public static bool IsWordSeparator(Unicode.Code code)
		{
			if ((int) code > 65535)
			{
				return false;
			}
			
			//	TODO: am�liorer cette liste... UNICODE doit certainement avoir pens�
			//	� la question.
			
			switch ((char) code)
			{
				case ' ':
				case '!':
				case '\"':
				case '#':	case '$':	case '%':	case '&':
				case '(':	case ')':	case '*':	case '+':
				case ',':	case '-':	case '.':	case ':':	case ';':
				case '/':	case '<':	case '=':	case '>':	case '?':
				case '[':	case '\\':	case ']':
				case '{':	case '|':	case '}':
					return true;
			}
			
			return Navigator.IsParagraphSeparator (code);
		}
		
		public static bool IsWordSeparator(ulong code)
		{
			return Navigator.IsWordSeparator (Unicode.Bits.GetUnicodeCode (code));
		}
		
		
		public static bool IsLineStart(TextStory story, TextFitter fitter, ICursor cursor, int offset, int direction)
		{
			if (direction > 0)
			{
				//	C'est forc�ment une fin de ligne en cas de "hit", puisque
				//	d�but et fin sont confondus et que la direction seule permet
				//	de les discriminer.
				
				return false;
			}
			
			offset += story.GetCursorPosition (cursor);
			
			Internal.TextTable text   = story.TextTable;
			CursorInfo.Filter  filter = Cursors.FitterCursor.GetFitterFilter (fitter);
			CursorInfo[]       infos  = text.FindCursorsBefore (offset + 1, filter);
			
			if (infos.Length > 0)
			{
				System.Diagnostics.Debug.Assert (infos.Length == 1);
				
				for (int i = 0; i < infos.Length; i++)
				{
					Cursors.FitterCursor fitter_cursor = text.GetCursorInstance (infos[i].CursorId) as Cursors.FitterCursor;
					
					//	V�rifie o� il y a des d�buts de lignes dans le paragraphe mis
					//	en page. La derni�re position correspond � la fin du paragraphe
					//	et doit donc �tre ignor�e :
					
					int[] positions = fitter_cursor.GetLineStartPositions (text);
					
					for (int j = 0; j < positions.Length - 1; j++)
					{
						if (positions[j] == offset)
						{
							return true;
						}
						if (positions[j] > offset)
						{
							break;
						}
					}
				}
			}
			
			return false;
		}
		
		public static bool IsLineEnd(TextStory story, TextFitter fitter, ICursor cursor, int offset, int direction)
		{
			if (direction < 0)
			{
				//	C'est forc�ment un d�but de ligne en cas de "hit", puisque
				//	d�but et fin sont confondus et que la direction seule permet
				//	de les discriminer.
				
				return false;
			}
			
			if (Navigator.IsParagraphEnd (story, cursor, offset))
			{
				return true;
			}
			
			offset += story.GetCursorPosition (cursor);
			
			Internal.TextTable text   = story.TextTable;
			CursorInfo.Filter  filter = Cursors.FitterCursor.GetFitterFilter (fitter);
			CursorInfo[]       infos  = text.FindCursorsBefore (offset + 1, filter);
			
			if (infos.Length > 0)
			{
				for (int i = 0; i < infos.Length; i++)
				{
					Cursors.FitterCursor fitter_cursor = text.GetCursorInstance (infos[i].CursorId) as Cursors.FitterCursor;
					
					//	V�rifie o� il y a des d�buts de lignes dans le paragraphe mis
					//	en page. La premi�re position correspond au d�but du paragraphe
					//	et n'est donc pas une fin de ligne, par contre tous les autres
					//	d�buts de lignes correspondent � la fin de la ligne pr�c�dente :
					
					int[] positions = fitter_cursor.GetLineStartPositions (text);
					
					for (int j = 1; j < positions.Length; j++)
					{
						if (positions[j] == offset)
						{
							return true;
						}
						if (positions[j] > offset)
						{
							break;
						}
					}
				}
			}
			
			return false;
		}
		
		
		public static int GetParagraphStartOffset(TextStory story, ICursor cursor)
		{
			//	Retourne l'offset au d�but du paragraphe. L'offset est n�gatif
			//	ou nul, dans tous les cas, et correspond � une distance relative
			//	entre la position courante et le premier caract�re du paragraphe.
			
			TextStory.CodeCallback callback = new TextStory.CodeCallback (Navigator.IsParagraphSeparator);
			
			int distance = story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, - distance, callback);
			
			return (offset == -1) ? -distance : -offset;
		}
		
		public static int GetParagraphEndLength(TextStory story, ICursor cursor)
		{
			//	Retourne la longueur du paragraphe depuis la position courante
			//	jusqu'� sa fin, y compris le caract�re de terminaison.
			
			TextStory.CodeCallback callback = new TextStory.CodeCallback (Navigator.IsParagraphSeparator);
			
			int distance = story.TextLength - story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, distance, callback);
			
			return (offset == -1) ? distance : offset+1;
		}
		
		
		public static int GetRunStartOffset(TextStory story, ICursor cursor, Property property)
		{
			//	Retourne l'offset au d�but du texte auquel est appliqu�e la
			//	propri�t� pass�e en entr�e.
			
			Navigator.PropertyFinder finder = new PropertyFinder (story.StyleList, property, story.ReadChar (cursor));
			TextStory.CodeCallback callback = new TextStory.CodeCallback (finder.MissingProperty);
			
			int distance = story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, - distance, callback);
			
			return (offset == -1) ? -distance : -offset;
		}
		
		public static int GetRunEndLength(TextStory story, ICursor cursor, Property property)
		{
			//	Retourne la longueur du texte auquel est appliqu�e la propri�t�
			//	pass�e en entr�e.
			
			Navigator.PropertyFinder finder = new PropertyFinder (story.StyleList, property, story.ReadChar (cursor));
			TextStory.CodeCallback callback = new TextStory.CodeCallback (finder.MissingProperty);
			
			int distance = story.TextLength - story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, distance, callback);
			
			return (offset == -1) ? distance : offset;
		}
		
		
		public static bool GetFlattenedProperties(TextStory story, ICursor cursor, int offset, out Property[] properties)
		{
			//	Retourne toutes les propri�t�s (fusionn�es, telles que stock�es
			//	dans le texte) pour la position indiqu�e.
			
			return Navigator.GetFlattenedProperties (story, story.ReadChar (cursor, offset), out properties);
		}
		
		public static bool GetFlattenedProperties(TextStory story, ulong code, out Property[] properties)
		{
			if (code == 0)
			{
				properties = null;
				return false;
			}
			else
			{
				properties = story.StyleList[code].Flatten (code);
				return true;
			}
		}
		
		public static bool GetFlattenedPropertiesExcludingStylesAndProperties(TextStory story, ICursor cursor, int offset, out Property[] properties)
		{
			//	Retourne toutes les propri�t�s (fusionn�es, telles que stock�es
			//	dans le texte) pour la position indiqu�e.
			
			ulong code = story.ReadChar (cursor, offset);
			
			if (code == 0)
			{
				properties = null;
				return false;
			}
			else
			{
				properties = story.StyleList[code].Flatten (code);
				
				properties = Properties.StylesProperty.RemoveStylesProperties (properties);
				properties = Properties.PropertiesProperty.RemovePropertiesProperties (properties);
				
				return true;
			}
		}
		
		
		public static bool GetManagedParagraphProperties(TextStory story, ICursor cursor, int offset, out Properties.ManagedParagraphProperty[] properties)
		{
			ulong code = story.ReadChar (cursor, offset);
			return Navigator.GetManagedParagraphProperties (story, code, out properties);
		}
		
		public static bool GetManagedParagraphProperties(TextStory story, ulong code, out Properties.ManagedParagraphProperty[] properties)
		{
			//	Cr�e la liste (tri�e) des propri�t�s de type ManagedParagraphProperty
			//	qui d�crivent le paragraphe actuel.
			
			Property[] props;
			
			if (Navigator.GetFlattenedProperties (story, code, out props))
			{
				properties = Properties.ManagedParagraphProperty.FilterManagedParagraphProperties (props);
				
				System.Array.Sort (properties, Properties.ManagedParagraphProperty.Comparer);
				
				return true;
			}
			else
			{
				properties = null;
				return false;
			}
		}
		
		public static void HandleManagedParagraphPropertiesChange(TextStory story, ICursor cursor, int offset, Properties.ManagedParagraphProperty[] old_properties, Properties.ManagedParagraphProperty[] new_properties)
		{
			//	G�re le passage d'un jeu de propri�t�s ManagedParagraphProperty �
			//	un autre (old_properties --> new_properties).
			
			//	Pour chaque propri�t� qui dispara�t, le gestionnaire correspondant
			//	sera appel� (DetachFromParagaph); de m�me, pour chaque propri�t�
			//	nouvelle, c'est AttachToParagraph qui sera appel�.
			
			System.Diagnostics.Debug.Assert (Navigator.IsParagraphStart (story, cursor, offset));
			
			int n_old = old_properties == null ? 0 : old_properties.Length;
			int n_new = new_properties == null ? 0 : new_properties.Length;
			
			if ((n_new == 0) &&
				(n_old == 0))
			{
				return;
			}
			
			if (offset != 0)
			{
				Cursors.TempCursor temp_cursor = new Cursors.TempCursor ();
				story.NewCursor (temp_cursor);
				
				try
				{
					Navigator.HandleManagedParagraphPropertiesChange (story, temp_cursor, 0, old_properties, new_properties);
				}
				finally
				{
					story.RecycleCursor (temp_cursor);
				}
				
				return;
			}
			
			System.Diagnostics.Debug.Assert (offset == 0);
			System.Diagnostics.Debug.Assert (Navigator.IsParagraphStart (story, cursor, 0));
			
			Text.TextContext     context = story.TextContext;
			ParagraphManagerList list    = context.ParagraphManagerList;
			
			for (int i = 0; i < n_old; i++)
			{
				for (int j = 0; j < n_new; j++)
				{
					if (Property.CompareEqualContents (old_properties[i], new_properties[j]))
					{
						goto next_old;
					}
				}
				
				//	Cette ancienne propri�t� n'a pas d'�quivalent dans la liste des
				//	nouvelles propri�t�s.
				
				list[old_properties[i].ManagerName].DetachFromParagraph (story, cursor, old_properties[i]);
				
			next_old:
				continue;
			}
			
			for (int i = 0; i < n_new; i++)
			{
				for (int j = 0; j < n_old; j++)
				{
					if (Property.CompareEqualContents (new_properties[i], old_properties[j]))
					{
						goto next_old;
					}
				}
				
				//	Cette nouvelle propri�t� n'a pas d'�quivalent dans la liste des
				//	anciennes propri�t�s.
				
				list[new_properties[i].ManagerName].AttachToParagraph (story, cursor, new_properties[i]);
				
			next_old:
				continue;
			}
		}
		
		
		public static bool GetParagraphStyles(TextStory story, ICursor cursor, int offset, out TextStyle[] styles)
		{
			//	Retourne les styles de paragraphe attach�s au paragraphe � la
			//	position indiqu�e.
			
			ulong code = story.ReadChar (cursor, offset);
			
			if (code == 0)
			{
				styles = null;
				return false;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] all_styles;
			
			context.GetStyles (code, out all_styles);
			
			int count = Properties.StylesProperty.CountMatchingStyles (all_styles, TextStyleClass.Paragraph);
			
			styles = new TextStyle[count];
			
			for (int i = 0, j = 0; j < count; i++)
			{
				if (all_styles[i].TextStyleClass == TextStyleClass.Paragraph)
				{
					styles[j++] = all_styles[i];
				}
			}
			
			return true;
		}
		
		public static bool GetParagraphProperties(TextStory story, ICursor cursor, int offset, out Property[] properties)
		{
			//	Retourne les propri�t�s attach�es au paragraphe de la position
			//	indiqu�e, en excluant les propri�t�s d�riv�es � partir des
			//	styles.
			
			ulong code = story.ReadChar (cursor, offset);
			
			if (code == 0)
			{
				properties = null;
				return false;
			}
			
			Text.TextContext context = story.TextContext;
			
			Property[] all_properties;
			int        count = 0;
			
			context.GetProperties (code, out all_properties);
			
			for (int i = 0; i < all_properties.Length; i++)
			{
				if (all_properties[i].RequiresUniformParagraph)
				{
					count++;
				}
			}
			
			properties = new Property[count];
			
			for (int i = 0, j = 0; j < count; i++)
			{
				if (all_properties[i].RequiresUniformParagraph)
				{
					properties[j++] = all_properties[i];
				}
			}
			
			return true;
		}
		
		
		public static void StartParagraphIfNeeded(TextStory story, ICursor cursor)
		{
			if (Navigator.IsParagraphStart (story, cursor, 0))
			{
				return;
			}
			
			//	Ajoute une fin de paragraphe au point d'insertion courant, afin
			//	de remplir la condition :
			
			Navigator.Insert (story, cursor, Unicode.Code.ParagraphSeparator, null, null);
		}
		
		public static void Insert(TextStory story, ICursor cursor, Unicode.Code code, System.Collections.ICollection styles, System.Collections.ICollection properties)
		{
			uint[] utf32 = new uint[] { (uint) code };
			Navigator.Insert (story, cursor, utf32, styles, properties);
		}
		
		public static void Insert(TextStory story, ICursor cursor, string simple_text, System.Collections.ICollection styles, System.Collections.ICollection properties)
		{
			uint[] utf32;
			TextConverter.ConvertFromString (simple_text, out utf32);
			Navigator.Insert (story, cursor, utf32, styles, properties);
		}
		
		public static void Insert(TextStory story, ICursor cursor, uint[] utf32, System.Collections.ICollection styles, System.Collections.ICollection properties)
		{
			int offset = Navigator.GetParagraphStartOffset (story, cursor);
			
			TextStyle[] paragraph_styles;
			Property[]  paragraph_properties;
			
			if ((Navigator.GetParagraphStyles (story, cursor, offset, out paragraph_styles)) &&
				(Navigator.GetParagraphProperties (story, cursor, offset, out paragraph_properties)))
			{
				System.Collections.ArrayList all_styles     = new System.Collections.ArrayList ();
				System.Collections.ArrayList all_properties = new System.Collections.ArrayList ();
				
				all_styles.AddRange (paragraph_styles);
				all_properties.AddRange (paragraph_properties);
				
				if (styles != null) all_styles.AddRange (styles);
				if (properties != null) all_properties.AddRange (properties);
				
				ulong[] text;
				
				story.ConvertToStyledText (utf32, story.FlattenStylesAndProperties (all_styles, all_properties), out text);
				story.InsertText (cursor, text);
			}
		}
		
		
		public static void SetParagraphStyles(TextStory story, ICursor cursor, System.Collections.ICollection styles)
		{
			TextStyle[] s_array = new TextStyle[styles == null ? 0 : styles.Count];
			
			if (styles != null)
			{
				styles.CopyTo (s_array, 0);
			}
			
			Navigator.SetParagraphStyles (story, cursor, s_array);
		}
		
		public static void SetParagraphStyles(TextStory story, ICursor cursor, params TextStyle[] styles)
		{
			if (styles == null)
			{
				styles = new TextStyle[0];
			}
			
			int offset_start = Navigator.GetParagraphStartOffset (story, cursor);
			int offset_end   = Navigator.GetParagraphEndLength (story, cursor);
			
			int length = offset_end - offset_start;
			
			if (length == 0)
			{
				//	Cas particulier : l'appelant essaie de modifier le style du paragraphe
				//	en fin de texte, alors que le paragraphe a une longueur nulle (donc il
				//	n'existe pas encore en tant que tel).
				
				return;
			}
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offset_start, length, text);
			
			//	D�termine l'�tat des propri�t�s "ManagedParagraph" qui d�terminent
			//	si/comment un paragraphe est g�r� (liste � puces, etc.) et dont tout
			//	changement requiert une gestion explicite.
			
			Properties.ManagedParagraphProperty[] old_props;
			Properties.ManagedParagraphProperty[] new_props;
			
			Navigator.GetManagedParagraphProperties (story, text[0], out old_props);
			
			ulong code  = 0;
			int   start = 0;
			int   count = 0;
			
			//	Change le style par tranches (une tranche partage exactement le m�me
			//	ensemble de styles et propri�t�s) pour �tre plus efficace :
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractStyleAndSettings (text[i]);
				
				if (code != next)
				{
					Navigator.SetParagraphStyles (story, text, code, start, count, styles);
					
					start = i;
					count = 1;
					code  = next;
				}
				else
				{
					count++;
				}
			}
			
			//	Change encore le style de la derni�re (ou de l'unique) tranche :
			
			Navigator.SetParagraphStyles (story, text, code, start, count, styles);
			Navigator.GetManagedParagraphProperties (story, text[0], out new_props);
			
			story.WriteText (cursor, offset_start, text);
			
			//	Finalement, g�re encore les changements de propri�t�s "ManagedParagraph"
			//	afin d'ajouter ou de supprimer les textes automatiques :
			
			Navigator.HandleManagedParagraphPropertiesChange (story, cursor, offset_start, old_props, new_props);
		}
		
		public static void SetCharacterStyles(TextStory story, ICursor cursor, int length, params TextStyle[] styles)
		{
			//	D�finit les styles � utiliser pour les caract�res sp�cifi�s. Remplace
			//	tous les styles de caract�res pr�c�demment appliqu�s.
			
			if (length == 0)
			{
				//	Aucun caract�re n'est affect� par la modification; ne fait
				//	rien du tout.
				
				return;
			}
			
			if (styles == null) styles = new TextStyle[0];
			
			int offset_start = 0;
			int offset_end   = length;
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offset_start, length, text);
			
			ulong code  = 0;
			int   start = 0;
			int   count = 0;
			
			//	Change le style par tranches (une tranche partage exactement le m�me
			//	ensemble de styles et propri�t�s) pour �tre plus efficace :
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractStyleAndSettings (text[i]);
				
				if (code != next)
				{
					Navigator.SetCharacterStyles (story, text, code, start, count, styles);
					
					start = i;
					count = 1;
					code  = next;
				}
				else
				{
					count++;
				}
			}
			
			//	Change encore le style de la derni�re (ou de l'unique) tranche :
			
			Navigator.SetCharacterStyles (story, text, code, start, count, styles);
			
			story.WriteText (cursor, offset_start, text);
		}
		
		public static void SetTextStyles(TextStory story, ICursor cursor, int length, params TextStyle[] styles)
		{
			//	D�finit les styles � utiliser pour les caract�res sp�cifi�s. Remplace tous
			//	les styles de texte pr�c�demment appliqu�s.
			
			if (length == 0)
			{
				//	Aucun caract�re n'est affect� par la modification; ne fait
				//	rien du tout.
				
				return;
			}
			
			if (styles == null) styles = new TextStyle[0];
			
			int offset_start = 0;
			int offset_end   = length;
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offset_start, length, text);
			
			ulong code  = 0;
			int   start = 0;
			int   count = 0;
			
			//	Change le style par tranches (une tranche partage exactement le m�me
			//	ensemble de styles et propri�t�s) pour �tre plus efficace :
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractStyleAndSettings (text[i]);
				
				if (code != next)
				{
					Navigator.SetTextStyles (story, text, code, start, count, styles);
					
					start = i;
					count = 1;
					code  = next;
				}
				else
				{
					count++;
				}
			}
			
			//	Change encore le style de la derni�re (ou de l'unique) tranche :
			
			Navigator.SetTextStyles (story, text, code, start, count, styles);
			
			story.WriteText (cursor, offset_start, text);
		}
		
		
		public static void SetTextProperties(TextStory story, ICursor cursor, int length, Properties.ApplyMode mode, params Property[] properties)
		{
			if (length == 0)
			{
				//	Aucun caract�re n'est affect� par la modification; ne fait
				//	rien du tout.
				
				return;
			}
			
			if (properties == null) properties = new Property[0];
			
			int offset_start = 0;
			int offset_end   = length;
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offset_start, length, text);
			
			ulong code  = 0;
			int   start = 0;
			int   count = 0;
			
			//	Change le style par tranches (une tranche partage exactement le m�me
			//	ensemble de styles et propri�t�s) pour �tre plus efficace :
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractStyleAndSettings (text[i]);
				
				if (code != next)
				{
					Navigator.SetTextProperties (story, text, code, start, count, properties, mode);
					
					start = i;
					count = 1;
					code  = next;
				}
				else
				{
					count++;
				}
			}
			
			//	Change encore le style de la derni�re (ou de l'unique) tranche :
			
			Navigator.SetTextProperties (story, text, code, start, count, properties, mode);
			
			story.WriteText (cursor, offset_start, text);
		}
		
		public static void SetMetaProperties(TextStory story, ICursor cursor, int length, Properties.ApplyMode mode, params TextStyle[] meta_properties)
		{
			if (length == 0)
			{
				//	Aucun caract�re n'est affect� par la modification; ne fait
				//	rien du tout.
				
				return;
			}
			
			if (meta_properties == null) meta_properties = new TextStyle[0];
			
			int offset_start = 0;
			int offset_end   = length;
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offset_start, length, text);
			
			ulong code  = 0;
			int   start = 0;
			int   count = 0;
			
			//	Change le style par tranches (une tranche partage exactement le m�me
			//	ensemble de styles et propri�t�s) pour �tre plus efficace :
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractStyleAndSettings (text[i]);
				
				if (code != next)
				{
					Navigator.SetMetaProperties (story, text, code, start, count, meta_properties, mode);
					
					start = i;
					count = 1;
					code  = next;
				}
				else
				{
					count++;
				}
			}
			
			//	Change encore le style de la derni�re (ou de l'unique) tranche :
			
			Navigator.SetMetaProperties (story, text, code, start, count, meta_properties, mode);
			
			story.WriteText (cursor, offset_start, text);
		}
		
		public static void SetParagraphProperties(TextStory story, ICursor cursor, Properties.ApplyMode mode, params Property[] properties)
		{
			int offset_start = Navigator.GetParagraphStartOffset (story, cursor);
			int offset_end   = Navigator.GetParagraphEndLength (story, cursor);
			
			Navigator.SetParagraphProperties (story, cursor, offset_start, offset_end, mode, properties);
		}
		
		public static void SetParagraphProperties(TextStory story, ICursor cursor, int length, Properties.ApplyMode mode, params Property[] properties)
		{
			int offset_start = 0;
			int offset_end   = length;
			
			Navigator.SetParagraphProperties (story, cursor, offset_start, offset_end, mode, properties);
		}
		
		public static void SetParagraphProperties(TextStory story, ICursor cursor, int offset_start, int offset_end, Properties.ApplyMode mode, params Property[] properties)
		{
			int length = offset_end - offset_start;
			
			if (length == 0)
			{
				//	Aucun caract�re n'est affect� par la modification; ne fait
				//	rien du tout.
				
				return;
			}
			
			if (properties == null) properties = new Property[0];
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offset_start, length, text);
			
			ulong code  = 0;
			int   start = 0;
			int   count = 0;
			
			//	Change le style par tranches (une tranche partage exactement le m�me
			//	ensemble de styles et propri�t�s) pour �tre plus efficace :
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractStyleAndSettings (text[i]);
				
				if (code != next)
				{
					Navigator.SetParagraphProperties (story, text, code, start, count, properties, mode);
					
					start = i;
					count = 1;
					code  = next;
				}
				else
				{
					count++;
				}
			}
			
			//	Change encore le style de la derni�re (ou de l'unique) tranche :
			
			Navigator.SetParagraphProperties (story, text, code, start, count, properties, mode);
			
			story.WriteText (cursor, offset_start, text);
		}
		
		
		private static void SetParagraphStyles(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] paragraph_styles)
		{
			//	Change le style de paragraphe pour une tranche donn�e.
			
			if (length == 0)
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			Property[]  properties;
			TextStyle[] old_styles;
			
			context.GetProperties (code, out properties);
			context.GetStyles (code, out old_styles);
			
			//	Cr�e la table des styles � utiliser en retirant les anciens styles
			//	de paragraphe et en ins�rant les nouveaux � la place :
			
			int n_para_styles  = Properties.StylesProperty.CountMatchingStyles (old_styles, TextStyleClass.Paragraph);
			int n_other_styles = old_styles.Length - n_para_styles;
			
			TextStyle[] new_styles = new TextStyle[paragraph_styles.Length + n_other_styles];
			
			System.Array.Copy (paragraph_styles, 0, new_styles, 0, paragraph_styles.Length);
			
			for (int i = 0, j = paragraph_styles.Length; i < old_styles.Length; i++)
			{
				if (old_styles[i].TextStyleClass != TextStyleClass.Paragraph)
				{
					new_styles[j++] = old_styles[i];
				}
			}
			
			//	Cr�e la table des propri�t�s � utiliser :
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (new_styles, Property.Filter (properties, Properties.PropertyFilter.NonUniformOnly));
			
			ulong style_bits;
			
			story.ConvertToStyledText (flat, out style_bits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.StyleAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		
		private static void SetTextProperties(TextStory story, ulong[] text, ulong code, int offset, int length, Property[] text_properties, Properties.ApplyMode mode)
		{
			//	Modifie les propri�t�s du texte en utilisant celles pass�es en
			//	entr�e, en se basant sur le mode de combinaison sp�cifi�.
			
			if ((length == 0) ||
				(mode == Properties.ApplyMode.None))
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] current_styles;
			Property[]  current_properties;
			
			//	R�cup�re d'abord les styles et les propri�t�s courantes :
			
			context.GetStyles (code, out current_styles);
			context.GetProperties (code, out current_properties);
			
			foreach (Property p in current_properties)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Current: {0} - {1}.", p.WellKnownType, p.ToString ()));
			}
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (text_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (text_properties) == false);
			
			//	Dans un premier temps, on conserve tous les styles et ne garde que les
			//	propri�t�s associ�es directement au paragraphe. Les autres propri�t�s
			//	vont devoir �tre filtr�es :
			
			System.Collections.ArrayList all_styles = new System.Collections.ArrayList ();
			System.Collections.ArrayList all_properties = new System.Collections.ArrayList ();
			
			all_styles.AddRange (current_styles);
			
			all_properties.AddRange (Property.Filter (current_properties, Properties.PropertyFilter.UniformOnly));
			all_properties.AddRange (Navigator.Combine (Property.Filter (current_properties, Properties.PropertyFilter.NonUniformOnly), text_properties, mode));
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (all_styles, all_properties);
			
			ulong style_bits;
			
			story.ConvertToStyledText (flat, out style_bits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.StyleAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		private static void SetMetaProperties(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] meta_properties, Properties.ApplyMode mode)
		{
			//	Modifie les m�ta propri�t�s du texte en utilisant celles pass�es en
			//	entr�e, en se basant sur le mode de combinaison sp�cifi�.
			
			if ((length == 0) ||
				(mode == Properties.ApplyMode.None))
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] current_styles;
			Property[]  current_properties;
			
			//	R�cup�re d'abord les styles et les propri�t�s courantes :
			
			context.GetStyles (code, out current_styles);
			context.GetProperties (code, out current_properties);
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (current_properties) == false);
			
			System.Collections.ArrayList all_styles = new System.Collections.ArrayList ();
			System.Collections.ICollection combined = Navigator.Combine (TextStyle.FilterStyles (current_styles, TextStyleClass.MetaProperty), meta_properties, mode);
			
			all_styles.AddRange (TextStyle.FilterStyles (current_styles, TextStyleClass.Paragraph));
			all_styles.AddRange (TextStyle.FilterStyles (current_styles, TextStyleClass.Text));
			all_styles.AddRange (TextStyle.FilterStyles (current_styles, TextStyleClass.Character));
			all_styles.AddRange (Navigator.Combine (TextStyle.FilterStyles (current_styles, TextStyleClass.MetaProperty), meta_properties, mode));
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (all_styles, current_properties);
			
			ulong style_bits;
			
			story.ConvertToStyledText (flat, out style_bits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.StyleAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		private static void SetParagraphProperties(TextStory story, ulong[] text, ulong code, int offset, int length, Property[] paragraph_properties, Properties.ApplyMode mode)
		{
			//	Modifie les propri�t�s du paragraphe en utilisant celles pass�es en
			//	entr�e, en se basant sur le mode de combinaison sp�cifi�.
			
			if ((length == 0) ||
				(mode == Properties.ApplyMode.None))
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] current_styles;
			Property[]  current_properties;
			
			//	R�cup�re d'abord les styles et les propri�t�s courantes :
			
			context.GetStyles (code, out current_styles);
			context.GetProperties (code, out current_properties);
			
			foreach (Property p in current_properties)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Current: {0} - {1}.", p.WellKnownType, p.ToString ()));
			}
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (paragraph_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (paragraph_properties) == false);
			
			//	Dans un premier temps, on ne conserve tous les styles et que les
			//	propri�t�s associ�s directement au paragraphe. Les autres propri�t�s
			//	vont devoir �tre filtr�es :
			
			System.Collections.ArrayList all_styles = new System.Collections.ArrayList ();
			System.Collections.ArrayList all_properties = new System.Collections.ArrayList ();
			
			all_styles.AddRange (current_styles);
			
			all_properties.AddRange (Navigator.Combine (Property.Filter (current_properties, Properties.PropertyFilter.UniformOnly), paragraph_properties, mode));
			all_properties.AddRange (Property.Filter (current_properties, Properties.PropertyFilter.NonUniformOnly));
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (all_styles, all_properties);
			
			ulong style_bits;
			
			story.ConvertToStyledText (flat, out style_bits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.StyleAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		
		
		private static void SetCharacterStyles(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] character_styles)
		{
			//	Remplace les styles de caract�res par ceux pass�s en entr�e.
			
			if (length == 0)
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] current_styles;
			Property[]  current_properties;
			
			//	R�cup�re d'abord les styles et les propri�t�s associ�es au texte
			//	actuel :
			
			context.GetStyles (code, out current_styles);
			context.GetProperties (code, out current_properties);
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (current_properties) == false);
			
			//	Ne conserve que les styles associ�s directement au paragraphe et
			//	au texte. En fait, on supprime les styles li�s aux caract�res :
			
			TextStyle[] filtered_styles = TextStyle.FilterStyles (current_styles, TextStyleClass.Paragraph, TextStyleClass.Text, TextStyleClass.MetaProperty);
			TextStyle[] all_styles      = new TextStyle[filtered_styles.Length + character_styles.Length];
			Property[]  all_properties  = current_properties;
			
			System.Array.Copy (filtered_styles, 0, all_styles, 0, filtered_styles.Length);
			System.Array.Copy (character_styles, 0, all_styles, filtered_styles.Length, character_styles.Length);
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (all_styles, all_properties);
			
			ulong style_bits;
			
			story.ConvertToStyledText (flat, out style_bits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.StyleAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		private static void SetTextStyles(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] text_styles)
		{
			//	Remplace les styles de texte par ceux pass�s en entr�e.
			
			if (length == 0)
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] current_styles;
			Property[]  current_properties;
			
			//	R�cup�re d'abord les styles et les propri�t�s associ�es au texte
			//	actuel :
			
			context.GetStyles (code, out current_styles);
			context.GetProperties (code, out current_properties);
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (current_properties) == false);
			
			//	Ne conserve que les styles associ�s directement au paragraphe et
			//	aux caract�res. En fait, on supprime les styles li�s au texte :
			
			TextStyle[] filtered_styles = TextStyle.FilterStyles (current_styles, TextStyleClass.Paragraph, TextStyleClass.Character, TextStyleClass.MetaProperty);
			TextStyle[] all_styles      = new TextStyle[filtered_styles.Length + text_styles.Length];
			Property[]  all_properties  = current_properties;
			
			System.Array.Copy (filtered_styles, 0, all_styles, 0, filtered_styles.Length);
			System.Array.Copy (text_styles, 0, all_styles, filtered_styles.Length, text_styles.Length);
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (all_styles, all_properties);
			
			ulong style_bits;
			
			story.ConvertToStyledText (flat, out style_bits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.StyleAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		
		internal static System.Collections.ICollection Combine(Property[] a, Property[] b, Properties.ApplyMode mode)
		{
			switch (mode)
			{
				case Properties.ApplyMode.Overwrite:	return Navigator.CombineOverwrite (a, b);
				case Properties.ApplyMode.Clear:		return Navigator.CombineClear (a, b);
				case Properties.ApplyMode.Set:			return Navigator.CombineSet (a, b);
				case Properties.ApplyMode.Combine:		return Navigator.CombineAccumulate (a, b);
				
				default:
					throw new System.InvalidOperationException (string.Format ("ApplyMode.{0} not valid here.", mode));
			}
		}
		
		internal static System.Collections.ICollection Combine(TextStyle[] a, TextStyle[] b, Properties.ApplyMode mode)
		{
			switch (mode)
			{
				case Properties.ApplyMode.Overwrite:	return Navigator.CombineOverwrite (a, b);
				case Properties.ApplyMode.Clear:		return Navigator.CombineClear (a, b);
				case Properties.ApplyMode.Set:			return Navigator.CombineSet (a, b);
				case Properties.ApplyMode.Combine:		return Navigator.CombineAccumulate (a, b);
				
				default:
					throw new System.InvalidOperationException (string.Format ("ApplyMode.{0} not valid here.", mode));
			}
		}
		
		
		private static System.Collections.ICollection CombineOverwrite(Property[] a, Property[] b)
		{
			//	Un overwrite �crase toutes les propri�t�s source, sauf si la source
			//	contient des propri�t�s avec affinit� PropertyAffinity.Symbol; dans
			//	ce cas, la propri�t� survit � l'overwrite...
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			foreach (Property p in a)
			{
				if (p.PropertyAffinity == Properties.PropertyAffinity.Symbol)
				{
					list.Add (p);
				}
			}
			
			foreach (Property p in b)
			{
				//	Si des propri�t�s ont surv�cu au filtrage ci-dessus, elles sont
				//	tout de m�me des candidates au remplacement si des propri�t�s
				//	de la m�me classe sont ins�r�es.
				
				for (int i = 0; i < list.Count; )
				{
					Property test = list[i] as Property;
					
					if (test.WellKnownType == p.WellKnownType)
					{
						list.RemoveAt (i);
					}
					else
					{
						i++;
					}
				}
				
				list.Add (p);
			}
			
			return list;
		}
		
		private static System.Collections.ICollection CombineClear(Property[] a, Property[] b)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			list.AddRange (a);
			
			foreach (Property p in a)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Found: {0} - {1}.", p.WellKnownType, p.ToString ()));
			}
			
			foreach (Property p in b)
			{
				//	V�rifie si cette propri�t� existe dans la liste. Si oui, on
				//	doit la supprimer.
				
				for (int i = 0; i < list.Count; )
				{
					Property test = list[i] as Property;
					
					if (test.WellKnownType == p.WellKnownType)
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Removed: {0} - {1}.", p.WellKnownType, p.ToString ()));
						list.RemoveAt (i);
					}
					else
					{
						i++;
					}
				}
			}
			
			return list;
		}
		
		private static System.Collections.ICollection CombineSet(Property[] a, Property[] b)
		{
			System.Collections.ArrayList list = Navigator.CombineClear (a, b) as System.Collections.ArrayList;
			list.AddRange (b);
			foreach (Property p in b)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Added: {0} - {1}.", p.WellKnownType, p.ToString ()));
			}
			return list;
		}
		
		private static System.Collections.ICollection CombineAccumulate(Property[] a, Property[] b)
		{
			Styles.PropertyContainer.Accumulator accumulator = new Styles.PropertyContainer.Accumulator ();
			
			accumulator.Accumulate (a);
			accumulator.Accumulate (b);
			
			return accumulator.AccumulatedProperties;
		}
		

		private static System.Collections.ICollection CombineOverwrite(TextStyle[] a, TextStyle[] b)
		{
			//	Un overwrite �crase toutes les propri�t�s source.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			list.AddRange (a);
			
			foreach (TextStyle p in b)
			{
				for (int i = 0; i < list.Count; )
				{
					TextStyle test = list[i] as TextStyle;
					
					if (test.MetaId == p.MetaId)
					{
						list.RemoveAt (i);
					}
					else
					{
						i++;
					}
				}
				
				list.Add (p);
			}
			
			return list;
		}
		
		private static System.Collections.ICollection CombineClear(TextStyle[] a, TextStyle[] b)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			list.AddRange (a);
			
			foreach (TextStyle p in b)
			{
				for (int i = 0; i < list.Count; )
				{
					TextStyle test = list[i] as TextStyle;
					
					if (test.MetaId == p.MetaId)
					{
						list.RemoveAt (i);
					}
					else
					{
						i++;
					}
				}
			}
			
			return list;
		}
		
		private static System.Collections.ICollection CombineSet(TextStyle[] a, TextStyle[] b)
		{
			System.Collections.ArrayList list = Navigator.CombineClear (a, b) as System.Collections.ArrayList;
			
			list.AddRange (b);
			
			return list;
		}
		
		private static System.Collections.ICollection CombineAccumulate(TextStyle[] a, TextStyle[] b)
		{
			throw new System.InvalidOperationException ("Combine is impossible on meta-properties");
		}
		

		#region PropertyFinder Class
		private class PropertyFinder
		{
			public PropertyFinder(StyleList styles, Property property, ulong code)
			{
				this.styles   = styles;
				this.property = property;
				this.code     = Internal.CharMarker.ExtractStyleAndSettings (code);
			}
			
			
			public bool MissingProperty(ulong code)
			{
				code = Internal.CharMarker.ExtractStyleAndSettings (code);
				
				if (this.code == code)
				{
					return false;
				}
				
				if (this.styles[code].Contains (code, this.property))
				{
					this.code = code;	//	propri�t� trouv�e, continue...
					return false;
				}
				else
				{
					return true;		//	propri�t� manquante, arr�te ici
				}
			}
			
			
			private StyleList					styles;
			private Property					property;
			private ulong						code;
		}
		#endregion
	}
}
