//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La classe Navigator gère les déplacements au sein d'un texte.
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
		
		public static bool IsParagraphSeparator(int code)
		{
			return Navigator.IsParagraphSeparator ((Unicode.Code) code);
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
			int code_0 = Unicode.Bits.GetCode (story.ReadChar (cursor, offset));
			int code_1 = Unicode.Bits.GetCode (story.ReadChar (cursor, offset - 1));
			 
			return Navigator.IsWordStart (code_0, code_1);
		}
		
		public static bool IsWordStart(int code_0, int code_1)
		{
			if (code_1 == 0)
			{
				return true;
			}
			
			CodeClass class_0 = Navigator.GetCodeClass (code_0);
			CodeClass class_1 = Navigator.GetCodeClass (code_1);
			
			if (class_1 == CodeClass.ParagraphSeparator)
			{
				return true;
			}
			
			if (class_0 != class_1)
			{
				if (class_0 == CodeClass.Space)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}
		
		public static bool IsWordEnd(TextStory story, ICursor cursor, int offset)
		{
			int code_0 = Unicode.Bits.GetCode (story.ReadChar (cursor, offset));
			int code_1 = Unicode.Bits.GetCode (story.ReadChar (cursor, offset - 1));
			 
			return Navigator.IsWordEnd (code_0, code_1);
		}
		
		public static bool IsWordEnd(int code_0, int code_1)
		{
			if (code_0 == 0)
			{
				return true;
			}
			
			CodeClass class_0 = Navigator.GetCodeClass (code_0);
			CodeClass class_1 = Navigator.GetCodeClass (code_1);
			
			if (class_0 == CodeClass.ParagraphSeparator)
			{
				return true;
			}
			
			if (class_0 != class_1)
			{
				if (class_1 == CodeClass.Space)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
		}
		
		
		#region CodeClass Enumeration
		private enum CodeClass
		{
			Space,
			Alphanumeric,
			Punctuation,
			ParagraphSeparator
		}
		#endregion
		
		private static CodeClass GetCodeClass(int code)
		{
			if (Navigator.IsParagraphSeparator (code))
			{
				return CodeClass.ParagraphSeparator;
			}
			
			if (Unicode.DefaultBreakAnalyzer.IsSpace (code))
			{
				return CodeClass.Space;
			}
			
			//	TODO: améliorer cette liste... UNICODE doit certainement avoir pensé
			//	à la question.
			
			switch (code)
			{
				case '!':	case '¡':	case '‼':
				case '\"':	case '«':	case '»':	case '‹':	case '›':
				case '‘':	case '’':	case '‚':	case '‛':
				case '“':	case '”':	case '„':
				
				case '#':	case '%':	case '‰':	case '&':
				case '(':	case ')':
				case '*':	case '+':	case '-':	case '±':	case '·':	case '×':	case '÷':
				case ',':	case '.':	case ':':	case ';':	case '…':
				case '/':	case '<':	case '=':	case '>':	case '?':	case '¿':
				case '[':	case '\\':	case ']':
				case '{':	case '|':	case '¦':	case '}':

				case '$':	case '£':	case '€':	case '¢':	case '¤':	case '¥':
				case '~':	case '^':	case '´':	case '`':	case '¯':	case '¨':	case '¸':
				case '°':	case '§':	case '¬':
				case '′':	case '″':				//	prime, double prime
				case '@':	case '_':
				case '–':	case '—':	case '―':	//	dashes
				case '†':	case '‡':
				case '•':
					
					return CodeClass.Punctuation;
			}
			
			return CodeClass.Alphanumeric;
		}
		
		
		public static bool IsLineStart(TextStory story, TextFitter fitter, ICursor cursor, int offset, int direction)
		{
			if (direction > 0)
			{
				//	C'est forcément une fin de ligne en cas de "hit", puisque
				//	début et fin sont confondus et que la direction seule permet
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
					
					//	Vérifie où il y a des débuts de lignes dans le paragraphe mis
					//	en page. La dernière position correspond à la fin du paragraphe
					//	et doit donc être ignorée :
					
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
		
		public static bool IsAfterLineBreak(TextStory story, ICursor cursor, int offset)
		{
			if (Unicode.Bits.GetUnicodeCode (story.ReadChar (cursor, offset-1)) == Unicode.Code.LineSeparator)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		public static bool IsLineEnd(TextStory story, TextFitter fitter, ICursor cursor, int offset, int direction)
		{
			if (direction < 0)
			{
				//	C'est forcément un début de ligne en cas de "hit", puisque
				//	début et fin sont confondus et que la direction seule permet
				//	de les discriminer.
				
				return false;
			}
			
			if (Navigator.IsParagraphEnd (story, cursor, offset))
			{
				return true;
			}
			
			int start = story.GetCursorPosition (cursor) + offset;
			
			Internal.TextTable text   = story.TextTable;
			CursorInfo.Filter  filter = Cursors.FitterCursor.GetFitterFilter (fitter);
			CursorInfo[]       infos  = text.FindCursorsBefore (start + 1, filter);
			
			if (infos.Length > 0)
			{
				for (int i = 0; i < infos.Length; i++)
				{
					Cursors.FitterCursor fitter_cursor = text.GetCursorInstance (infos[i].CursorId) as Cursors.FitterCursor;
					
					//	Vérifie où il y a des débuts de lignes dans le paragraphe mis
					//	en page. La première position correspond au début du paragraphe
					//	et n'est donc pas une fin de ligne, par contre tous les autres
					//	débuts de lignes correspondent à la fin de la ligne précédente :
					
					int[] positions = fitter_cursor.GetLineStartPositions (text);
					
					for (int j = 1; j < positions.Length; j++)
					{
						if (positions[j] == start)
						{
							return true;
						}
						if (positions[j] > start)
						{
							break;
						}
					}
				}
			}
			
			return false;
		}
		

		public static void GetParagraphPositions(TextStory story, int pos, out int start, out int end)
		{
			ICursor temp = new Cursors.TempCursor ();
			
			story.NewCursor (temp);
			story.SetCursorPosition (temp, pos);
			
			start = pos + Navigator.GetParagraphStartOffset (story, temp);
			
			story.SetCursorPosition (temp, start);
			
			end   = start + Navigator.GetParagraphEndLength (story, temp);
			
			story.RecycleCursor (temp);
		}
		
		public static void GetExtendedParagraphPositions(TextStory story, int pos, out int start, out int end)
		{
			ICursor temp = new Cursors.TempCursor ();
			
			story.NewCursor (temp);
			story.SetCursorPosition (temp, pos);
			
			int begin;
			
			//	'start' est le début du groupe de paragraphes liés, en prenant
			//	toujours un paragraphe de plus.
			//	'begin' est le début du paragraphe actuel.
			
			start = pos + Navigator.GetParagraphGroupStartOffset (story, temp, 1);
			begin = pos + Navigator.GetParagraphStartOffset (story, temp);
			
			story.SetCursorPosition (temp, begin);
			
			end   = begin + Navigator.GetParagraphEndLength (story, temp);
			
			story.RecycleCursor (temp);
		}
		
		public static int GetParagraphStartOffset(TextStory story, ICursor cursor)
		{
			//	Retourne l'offset au début du paragraphe. L'offset est négatif
			//	ou nul, dans tous les cas, et correspond à une distance relative
			//	entre la position courante et le premier caractère du paragraphe.
			
			TextStory.CodeCallback callback = new TextStory.CodeCallback (Navigator.IsParagraphSeparator);
			
			int distance = story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, - distance, callback);
			
			return (offset == -1) ? -distance : -offset;
		}
		
		public static int GetParagraphGroupStartOffset(TextStory story, ICursor cursor, int skip)
		{
			//	Retourne l'offset au début du groupe de paragraphes, en sautant
			//	le nombre de paragraphes indiqué et en suivant les paragraphes
			//	liés (Keep).
			
			//	Cf. GetParagraphStartOffset pour les détails.
			
			TextStory.CodeCallback callback = new TextStory.CodeCallback (new ParagraphGroupStartFinder (story.TextContext, skip).Check);
			
			int distance = story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, - distance, callback);
			
			return (offset == -1) ? -distance : -offset;
		}
		
		
		#region ParagraphGroupStartFinder Class
		private class ParagraphGroupStartFinder
		{
			public ParagraphGroupStartFinder(TextContext context, int skip)
			{
				this.context   = context;
				this.skip      = skip;
				this.last_code = 0;
			}
			
			
			public bool Check(ulong code)
			{
				ulong last = this.last_code;
				
				this.last_code = code;
				
				if (Navigator.IsParagraphSeparator (code))
				{
					if (this.skip > 0)
					{
						this.skip -= 1;
						return false;
					}
					
					//	Vérifie si le dernier code rencontré correspond à un paragraphe
					//	lié avec le précédent :
					
					Properties.KeepProperty keep;
					
					this.context.GetKeep (last, out keep);
					
					if ((keep != null) &&
						(keep.KeepWithPreviousParagraph == Properties.ThreeState.True))
					{
						//	Paragraphe actuel lié au précédent : on doit encore
						//	sauter un paragraphe de plus pour arriver au début
						//	du groupe.
						
						return false;
					}
					
					this.context.GetKeep (code, out keep);
					
					if ((keep != null) &&
						(keep.KeepWithNextParagraph == Properties.ThreeState.True))
					{
						//	Paragraphe précédent lié au courant : continue la
						//	recherche.
						
						return false;
					}
					
					return true;
				}
				
				return false;
			}
			
			
			private int							skip;
			private TextContext					context;
			private ulong						last_code;
		}
		#endregion
		
		public static int GetParagraphEndLength(TextStory story, ICursor cursor)
		{
			//	Retourne la longueur du paragraphe depuis la position courante
			//	jusqu'à sa fin, y compris le caractère de terminaison.
			
			TextStory.CodeCallback callback = new TextStory.CodeCallback (Navigator.IsParagraphSeparator);
			
			int distance = story.TextLength - story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, distance, callback);
			
			return (offset == -1) ? distance : offset+1;
		}
		
		
		public static int GetRunStartOffset(TextStory story, ICursor cursor, Property property)
		{
			//	Retourne l'offset au début du texte auquel est appliquée la
			//	propriété passée en entrée.
			
			Navigator.PropertyFinder finder = new PropertyFinder (story.StyleList, property, story.ReadChar (cursor));
			TextStory.CodeCallback callback = new TextStory.CodeCallback (finder.MissingProperty);
			
			int distance = story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, - distance, callback);
			
			return (offset == -1) ? -distance : -offset;
		}
		
		public static int GetRunEndLength(TextStory story, ICursor cursor, Property property)
		{
			//	Retourne la longueur du texte auquel est appliquée la propriété
			//	passée en entrée.
			
			Navigator.PropertyFinder finder = new PropertyFinder (story.StyleList, property, story.ReadChar (cursor));
			TextStory.CodeCallback callback = new TextStory.CodeCallback (finder.MissingProperty);
			
			int distance = story.TextLength - story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, distance, callback);
			
			return (offset == -1) ? distance : offset;
		}
		
		
		public static bool GetFlattenedProperties(TextStory story, ICursor cursor, int offset, out Property[] properties)
		{
			//	Retourne toutes les propriétés (fusionnées, telles que stockées
			//	dans le texte) pour la position indiquée.
			
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
			//	Retourne toutes les propriétés (fusionnées, telles que stockées
			//	dans le texte) pour la position indiquée.
			
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
			//	Crée la liste (triée) des propriétés de type ManagedParagraphProperty
			//	qui décrivent le paragraphe actuel.
			
			Property[] props;
			
			if (Navigator.GetFlattenedProperties (story, code, out props))
			{
				properties = Properties.ManagedParagraphProperty.Filter (props);
				
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
			//	Gère le passage d'un jeu de propriétés ManagedParagraphProperty à
			//	un autre (old_properties --> new_properties).
			
			//	Pour chaque propriété qui disparaît, le gestionnaire correspondant
			//	sera appelé (DetachFromParagaph); de même, pour chaque propriété
			//	nouvelle, c'est AttachToParagraph qui sera appelé.
			
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
				//	Cette méthode travaille toujours avec un offset nul. Si ce
				//	n'est pas le cas au départ, on crée un curseur temporaire
				//	qui satisfait à cette condition :
				
				Cursors.TempCursor temp_cursor = new Cursors.TempCursor ();
				
				story.NewCursor (temp_cursor);
				story.SetCursorPosition (temp_cursor, story.GetCursorPosition (cursor) + offset);
				
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
				bool same = false;
				
				for (int j = 0; j < n_new; j++)
				{
					if (Property.CompareEqualContents (old_properties[i], new_properties[j]))
					{
						same = true;
						break;
					}
				}
				
				//	Cette ancienne propriété n'a pas d'équivalent dans la liste des
				//	nouvelles propriétés.
				
				if (! same)
				{
					list[old_properties[i].ManagerName].DetachFromParagraph (story, cursor, old_properties[i]);
				}
			}
			
			for (int i = 0; i < n_new; i++)
			{
				bool same = false;
				
				for (int j = 0; j < n_old; j++)
				{
					if (Property.CompareEqualContents (new_properties[i], old_properties[j]))
					{
						same = true;
						break;
					}
				}
				
				//	Cette nouvelle propriété n'a pas d'équivalent dans la liste des
				//	anciennes propriétés.
				
				if (same)
				{
					list[new_properties[i].ManagerName].RefreshParagraph (story, cursor, new_properties[i]);
				}
				else
				{
					list[new_properties[i].ManagerName].AttachToParagraph (story, cursor, new_properties[i]);
				}
			}
		}
		
		
		public static bool GetParagraphStyles(TextStory story, ICursor cursor, int offset, out TextStyle[] styles)
		{
			//	Retourne les styles de paragraphe attachés au paragraphe à la
			//	position indiquée.
			
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
		
		public static bool GetUniformMetaProperties(TextStory story, ICursor cursor, int offset, out TextStyle[] styles)
		{
			//	Retourne les méta propriétés attachées au paragraphe à la position
			//	indiquée.
			
			ulong code = story.ReadChar (cursor, offset);
			
			if (code == 0)
			{
				styles = null;
				return false;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] all_styles;
			
			context.GetStyles (code, out all_styles);
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < all_styles.Length; i++)
			{
				if ((all_styles[i].TextStyleClass == TextStyleClass.MetaProperty) &&
					(all_styles[i].RequiresUniformParagraph))
				{
					list.Add (all_styles[i]);
				}
			}
			
			styles = (TextStyle[]) list.ToArray (typeof (TextStyle));
			
			return true;
		}
		
		public static bool GetParagraphProperties(TextStory story, ICursor cursor, int offset, out Property[] properties)
		{
			//	Retourne les propriétés attachées au paragraphe de la position
			//	indiquée, en excluant les propriétés dérivées à partir des
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
			TextStyle[] paragraph_meta;
			Property[]  paragraph_properties;
			
			if ((Navigator.GetParagraphStyles (story, cursor, offset, out paragraph_styles)) &&
				(Navigator.GetUniformMetaProperties (story, cursor, offset, out paragraph_meta)) &&
				(Navigator.GetParagraphProperties (story, cursor, offset, out paragraph_properties)))
			{
				System.Collections.ArrayList all_styles     = new System.Collections.ArrayList ();
				System.Collections.ArrayList all_properties = new System.Collections.ArrayList ();
				
				all_styles.AddRange (paragraph_styles);
				all_properties.AddRange (paragraph_properties);
				
				if (styles != null) all_styles.AddRange (styles);
				if (properties != null) all_properties.AddRange (properties);
				
				all_styles.AddRange (paragraph_meta);
				
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
			
			//	Détermine l'état des propriétés "ManagedParagraph" qui déterminent
			//	si/comment un paragraphe est géré (liste à puces, etc.) et dont tout
			//	changement requiert une gestion explicite.
			
			Properties.ManagedParagraphProperty[] old_props;
			Properties.ManagedParagraphProperty[] new_props;
			
			Navigator.GetManagedParagraphProperties (story, text[0], out old_props);
			
			ulong code  = 0;
			int   start = 0;
			int   count = 0;
			
			//	Change le style par tranches (une tranche partage exactement le même
			//	ensemble de styles et propriétés) pour être plus efficace :
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractCoreAndSettings (text[i]);
				
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
			
			//	Change encore le style de la dernière (ou de l'unique) tranche :
			
			Navigator.SetParagraphStyles (story, text, code, start, count, styles);
			Navigator.GetManagedParagraphProperties (story, text[0], out new_props);
			
			System.Diagnostics.Debug.Assert (cursor.Attachment == CursorAttachment.Temporary);
			
			int pos = story.GetCursorPosition (cursor);
			story.WriteText (cursor, offset_start, text);
			story.SetCursorPosition (cursor, pos);
			
			//	Finalement, gère encore les changements de propriétés "ManagedParagraph"
			//	afin d'ajouter ou de supprimer les textes automatiques :
			
			Navigator.HandleManagedParagraphPropertiesChange (story, cursor, offset_start, old_props, new_props);
		}
		
		public static void SetSymbolStyles(TextStory story, ICursor cursor, int length, params TextStyle[] styles)
		{
			//	Définit les styles à utiliser pour les caractères spécifiés. Remplace
			//	tous les styles de symboles précédemment appliqués.
			
			if (length == 0)
			{
				//	Aucun caractère n'est affecté par la modification; ne fait
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
			
			//	Change le style par tranches (une tranche partage exactement le même
			//	ensemble de styles et propriétés) pour être plus efficace :
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractCoreAndSettings (text[i]);
				
				if (code != next)
				{
					Navigator.SetSymbolStyles (story, text, code, start, count, styles);
					
					start = i;
					count = 1;
					code  = next;
				}
				else
				{
					count++;
				}
			}
			
			//	Change encore le style de la dernière (ou de l'unique) tranche :
			
			Navigator.SetSymbolStyles (story, text, code, start, count, styles);
			
			story.WriteText (cursor, offset_start, text);
		}
		
		public static void SetTextStyles(TextStory story, ICursor cursor, int length, params TextStyle[] styles)
		{
			//	Définit les styles à utiliser pour les caractères spécifiés. Remplace tous
			//	les styles de texte précédemment appliqués.
			
			if (length == 0)
			{
				//	Aucun caractère n'est affecté par la modification; ne fait
				//	rien du tout.
				
				return;
			}
			
			if (styles == null)
			{
				styles = new TextStyle[0];
			}
			
			int offset_start = 0;
			int offset_end   = length;
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offset_start, length, text);
			
			ulong code  = 0;
			int   start = 0;
			int   count = 0;
			
			//	Change le style par tranches (une tranche partage exactement le même
			//	ensemble de styles et propriétés) pour être plus efficace :
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractCoreAndSettings (text[i]);
				
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
			
			//	Change encore le style de la dernière (ou de l'unique) tranche :
			
			Navigator.SetTextStyles (story, text, code, start, count, styles);
			
			story.WriteText (cursor, offset_start, text);
		}
		
		
		public static void SetTextProperties(TextStory story, ICursor cursor, int length, Properties.ApplyMode mode, params Property[] properties)
		{
			if (length == 0)
			{
				//	Aucun caractère n'est affecté par la modification; ne fait
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
			
			//	Change le style par tranches (une tranche partage exactement le même
			//	ensemble de styles et propriétés) pour être plus efficace :
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractCoreAndSettings (text[i]);
				
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
			
			//	Change encore le style de la dernière (ou de l'unique) tranche :
			
			Navigator.SetTextProperties (story, text, code, start, count, properties, mode);
			
			story.WriteText (cursor, offset_start, text);
		}
		
		public static void SetParagraphMetaProperties(TextStory story, ICursor cursor, Properties.ApplyMode mode, params TextStyle[] meta_properties)
		{
			int length = Navigator.GetParagraphEndLength (story, cursor);
			int pos    = story.GetCursorPosition (cursor);
			
			Properties.ManagedParagraphProperty[] old_props;
			Properties.ManagedParagraphProperty[] new_props;
			
			Navigator.GetManagedParagraphProperties (story, story.ReadChar (cursor), out old_props);
			Navigator.SetMetaProperties (story, cursor, length, mode, meta_properties);
			
			story.SetCursorPosition (cursor, pos);
			
			Navigator.GetManagedParagraphProperties (story, story.ReadChar (cursor), out new_props);
			
			//	Met encore à jour les managed paragraphs (si nécessaire) :
			
			Navigator.HandleManagedParagraphPropertiesChange (story, cursor, 0, old_props, new_props);
		}
		
		public static void SetMetaProperties(TextStory story, ICursor cursor, int length, Properties.ApplyMode mode, params TextStyle[] meta_properties)
		{
			if (length == 0)
			{
				//	Aucun caractère n'est affecté par la modification; ne fait
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
			
			//	Change le style par tranches (une tranche partage exactement le même
			//	ensemble de styles et propriétés) pour être plus efficace :
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractCoreAndSettings (text[i]);
				
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
			
			//	Change encore le style de la dernière (ou de l'unique) tranche :
			
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
				//	Aucun caractère n'est affecté par la modification; ne fait
				//	rien du tout.
				
				return;
			}
			
			if (properties == null) properties = new Property[0];
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offset_start, length, text);
			
			Properties.ManagedParagraphProperty[] old_props;
			Properties.ManagedParagraphProperty[] new_props;
			
			Navigator.GetManagedParagraphProperties (story, text[0], out old_props);
			
			ulong code  = 0;
			int   start = 0;
			int   count = 0;
			
			//	Change le style par tranches (une tranche partage exactement le même
			//	ensemble de styles et propriétés) pour être plus efficace :
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractCoreAndSettings (text[i]);
				
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
			
			//	Change encore le style de la dernière (ou de l'unique) tranche :
			
			Navigator.SetParagraphProperties (story, text, code, start, count, properties, mode);
			Navigator.GetManagedParagraphProperties (story, text[0], out new_props);
			
			int pos = story.GetCursorPosition (cursor);
			story.WriteText (cursor, offset_start, text);
			story.SetCursorPosition (cursor, pos);
			
			//	Met encore à jour les managed paragraphs (si nécessaire) :
			
			Navigator.HandleManagedParagraphPropertiesChange (story, cursor, offset_start, old_props, new_props);
		}
		
		
		private static void SetParagraphStyles(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] paragraph_styles)
		{
			//	Change le style de paragraphe pour une tranche donnée.
			
			if (length == 0)
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			Property[]  properties;
			TextStyle[] old_styles;
			
			context.GetProperties (code, out properties);
			context.GetStyles (code, out old_styles);
			
			//	Crée la table des styles à utiliser en retirant les anciens styles
			//	de paragraphe et en insérant les nouveaux à la place :
			
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
			
			//	Crée la table des propriétés à utiliser :
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (new_styles, Property.Filter (properties, Properties.PropertyFilter.NonUniformOnly));
			
			ulong style_bits;
			
			story.ConvertToStyledText (flat, out style_bits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.CoreAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		
		private static void SetTextProperties(TextStory story, ulong[] text, ulong code, int offset, int length, Property[] text_properties, Properties.ApplyMode mode)
		{
			//	Modifie les propriétés du texte en utilisant celles passées en
			//	entrée, en se basant sur le mode de combinaison spécifié.
			
			if ((length == 0) ||
				(mode == Properties.ApplyMode.None))
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] current_styles;
			Property[]  current_properties;
			
			//	Récupère d'abord les styles et les propriétés courantes :
			
			context.GetStyles (code, out current_styles);
			context.GetProperties (code, out current_properties);
			
//			foreach (Property p in current_properties)
//			{
//				System.Diagnostics.Debug.WriteLine (string.Format ("Current: {0} - {1}.", p.WellKnownType, p.ToString ()));
//			}
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (text_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (text_properties) == false);
			
			//	Dans un premier temps, on conserve tous les styles et ne garde que les
			//	propriétés associées directement au paragraphe. Les autres propriétés
			//	vont devoir être filtrées :

			List<TextStyle> all_styles = new List<TextStyle> ();
			List<Property>  all_properties = new List<Property> ();
			
			all_styles.AddRange (current_styles);
			
			all_properties.AddRange (Property.Filter (current_properties, Properties.PropertyFilter.UniformOnly));
			all_properties.AddRange (Navigator.Combine (Property.Filter (current_properties, Properties.PropertyFilter.NonUniformOnly), text_properties, mode));
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (all_styles, all_properties);
			
			ulong style_bits;
			
			story.ConvertToStyledText (flat, out style_bits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.CoreAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		private static void SetMetaProperties(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] meta_properties, Properties.ApplyMode mode)
		{
			//	Modifie les méta propriétés du texte en utilisant celles passées en
			//	entrée, en se basant sur le mode de combinaison spécifié.
			
			if ((length == 0) ||
				(mode == Properties.ApplyMode.None))
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] current_styles;
			Property[]  current_properties;
			
			//	Récupère d'abord les styles et les propriétés courantes :
			
			context.GetStyles (code, out current_styles);
			context.GetProperties (code, out current_properties);
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (current_properties) == false);

			List<TextStyle> all_styles = new List<TextStyle> ();
			ICollection<TextStyle> combined = Navigator.Combine (TextStyle.FilterStyles (current_styles, TextStyleClass.MetaProperty), meta_properties, mode);
			
			all_styles.AddRange (TextStyle.FilterStyles (current_styles, TextStyleClass.Paragraph));
			all_styles.AddRange (TextStyle.FilterStyles (current_styles, TextStyleClass.Text));
			all_styles.AddRange (TextStyle.FilterStyles (current_styles, TextStyleClass.Symbol));
			all_styles.AddRange (Navigator.Combine (TextStyle.FilterStyles (current_styles, TextStyleClass.MetaProperty), meta_properties, mode));
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (all_styles, current_properties);
			
			ulong style_bits;
			
			story.ConvertToStyledText (flat, out style_bits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.CoreAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		private static void SetParagraphProperties(TextStory story, ulong[] text, ulong code, int offset, int length, Property[] paragraph_properties, Properties.ApplyMode mode)
		{
			//	Modifie les propriétés du paragraphe en utilisant celles passées en
			//	entrée, en se basant sur le mode de combinaison spécifié.
			
			if ((length == 0) ||
				(mode == Properties.ApplyMode.None))
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] current_styles;
			Property[]  current_properties;
			
			//	Récupère d'abord les styles et les propriétés courantes :
			
			context.GetStyles (code, out current_styles);
			context.GetProperties (code, out current_properties);
			
//			foreach (Property p in current_properties)
//			{
//				System.Diagnostics.Debug.WriteLine (string.Format ("Current: {0} - {1}.", p.WellKnownType, p.ToString ()));
//			}
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (paragraph_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (paragraph_properties) == false);
			
			//	Dans un premier temps, on ne conserve tous les styles et que les
			//	propriétés associés directement au paragraphe. Les autres propriétés
			//	vont devoir être filtrées :
			
			List<TextStyle> all_styles = new List<TextStyle> ();
			List<Property> all_properties = new List<Property> ();
			
			all_styles.AddRange (current_styles);
			
			all_properties.AddRange (Navigator.Combine (Property.Filter (current_properties, Properties.PropertyFilter.UniformOnly), paragraph_properties, mode));
			all_properties.AddRange (Property.Filter (current_properties, Properties.PropertyFilter.NonUniformOnly));
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (all_styles, all_properties);
			
			ulong style_bits;
			
			story.ConvertToStyledText (flat, out style_bits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.CoreAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		
		
		private static void SetSymbolStyles(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] character_styles)
		{
			//	Remplace les styles de symboles par ceux passés en entrée.
			
			if (length == 0)
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] current_styles;
			Property[]  current_properties;
			
			//	Récupère d'abord les styles et les propriétés associées au texte
			//	actuel :
			
			context.GetStyles (code, out current_styles);
			context.GetProperties (code, out current_properties);
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (current_properties) == false);
			
			//	Ne conserve que les styles associés directement au paragraphe et
			//	au texte. En fait, on supprime les styles liés aux symboles :
			
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
				text[offset+i] &= ~ Internal.CharMarker.CoreAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		private static void SetTextStyles(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] text_styles)
		{
			//	Remplace les styles de texte par ceux passés en entrée.
			
			if (length == 0)
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] current_styles;
			Property[]  current_properties;
			
			//	Récupère d'abord les styles et les propriétés associées au texte
			//	actuel :
			
			context.GetStyles (code, out current_styles);
			context.GetProperties (code, out current_properties);
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (current_properties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (current_properties) == false);
			
			//	Ne conserve que les styles associés directement au paragraphe et
			//	aux symboles. En fait, on supprime les styles liés au texte :
			
			TextStyle[] filtered_styles = TextStyle.FilterStyles (current_styles, TextStyleClass.Paragraph, TextStyleClass.Symbol, TextStyleClass.MetaProperty);
			TextStyle[] all_styles      = new TextStyle[filtered_styles.Length + text_styles.Length];
			Property[]  all_properties  = current_properties;
			
			System.Array.Copy (filtered_styles, 0, all_styles, 0, filtered_styles.Length);
			System.Array.Copy (text_styles, 0, all_styles, filtered_styles.Length, text_styles.Length);
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (all_styles, all_properties);
			
			ulong style_bits;
			
			story.ConvertToStyledText (flat, out style_bits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.CoreAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		
		internal static ICollection<Property> Combine(Property[] a, Property[] b, Properties.ApplyMode mode)
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

		internal static ICollection<TextStyle> Combine(TextStyle[] a, TextStyle[] b, Properties.ApplyMode mode)
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


		private static List<Property> CombineOverwrite(Property[] a, Property[] b)
		{
			//	Un overwrite écrase toutes les propriétés source, sauf si la source
			//	contient des propriétés avec affinité PropertyAffinity.Symbol; dans
			//	ce cas, la propriété survit à l'overwrite...

			List<Property> list = new List<Property> ();
			
			foreach (Property p in a)
			{
				if (p.PropertyAffinity == Properties.PropertyAffinity.Symbol)
				{
					list.Add (p);
				}
			}
			
			foreach (Property p in b)
			{
				//	Si des propriétés ont survécu au filtrage ci-dessus, elles sont
				//	tout de même des candidates au remplacement si des propriétés
				//	de la même classe sont insérées.
				
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

		private static List<Property> CombineClear(Property[] a, Property[] b)
		{
			List<Property> list = new List<Property> ();
			
			list.AddRange (a);
			
			foreach (Property p in a)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Found: {0} - {1}.", p.WellKnownType, p.ToString ()));
			}
			
			foreach (Property p in b)
			{
				//	Vérifie si cette propriété existe dans la liste. Si oui, on
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

		private static List<Property> CombineSet(Property[] a, Property[] b)
		{
			List<Property> list = Navigator.CombineClear (a, b) as List<Property>;
			list.AddRange (b);
			foreach (Property p in b)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Added: {0} - {1}.", p.WellKnownType, p.ToString ()));
			}
			return list;
		}

		private static List<Property> CombineAccumulate(Property[] a, Property[] b)
		{
			Styles.PropertyContainer.Accumulator accumulator = new Styles.PropertyContainer.Accumulator ();
			
			accumulator.Accumulate (a);
			accumulator.Accumulate (b);

			List<Property> list = new List<Property> ();
			list.AddRange (accumulator.AccumulatedProperties);
			
			return list;
		}


		private static List<TextStyle> CombineOverwrite(TextStyle[] a, TextStyle[] b)
		{
			//	Un overwrite écrase toutes les propriétés source.

			List<TextStyle> list = new List<TextStyle> ();
			
			list.AddRange (a);
			
			foreach (TextStyle p in b)
			{
				for (int i = 0; i < list.Count; )
				{
					TextStyle test = list[i];
					
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

		private static List<TextStyle> CombineClear(TextStyle[] a, TextStyle[] b)
		{
			List<TextStyle> list = new List<TextStyle> ();
			
			list.AddRange (a);
			
			foreach (TextStyle p in b)
			{
				for (int i = 0; i < list.Count; )
				{
					TextStyle test = list[i];
					
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

		private static List<TextStyle> CombineSet(TextStyle[] a, TextStyle[] b)
		{
			List<TextStyle> list = Navigator.CombineClear (a, b) as List<TextStyle>;
			
			list.AddRange (b);
			
			return list;
		}

		private static List<TextStyle> CombineAccumulate(TextStyle[] a, TextStyle[] b)
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
				this.code     = Internal.CharMarker.ExtractCoreAndSettings (code);
			}
			
			
			public bool MissingProperty(ulong code)
			{
				code = Internal.CharMarker.ExtractCoreAndSettings (code);
				
				if (this.code == code)
				{
					return false;
				}
				
				if (this.styles[code].Contains (code, this.property))
				{
					this.code = code;	//	propriété trouvée, continue...
					return false;
				}
				else
				{
					return true;		//	propriété manquante, arrête ici
				}
			}
			
			
			private StyleList					styles;
			private Property					property;
			private ulong						code;
		}
		#endregion
	}
}
