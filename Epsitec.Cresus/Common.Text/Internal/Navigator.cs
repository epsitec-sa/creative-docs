//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			int code0 = Unicode.Bits.GetCode (story.ReadChar (cursor, offset));
			int code1 = Unicode.Bits.GetCode (story.ReadChar (cursor, offset - 1));
			 
			return Navigator.IsWordStart (code0, code1);
		}
		
		public static bool IsWordStart(int code0, int code1)
		{
			if (code1 == 0)
			{
				return true;
			}
			
			CodeClass class0 = Navigator.GetCodeClass (code0);
			CodeClass class1 = Navigator.GetCodeClass (code1);
			
			if (class1 == CodeClass.ParagraphSeparator)
			{
				return true;
			}
			
			if (class0 != class1)
			{
				if (class0 == CodeClass.Space)
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
			int code0 = Unicode.Bits.GetCode (story.ReadChar (cursor, offset));
			int code1 = Unicode.Bits.GetCode (story.ReadChar (cursor, offset - 1));
			 
			return Navigator.IsWordEnd (code0, code1);
		}
		
		public static bool IsWordEnd(int code0, int code1)
		{
			if (code0 == 0)
			{
				return true;
			}
			
			CodeClass class0 = Navigator.GetCodeClass (code0);
			CodeClass class1 = Navigator.GetCodeClass (code1);
			
			if (class0 == CodeClass.ParagraphSeparator)
			{
				return true;
			}
			
			if (class0 != class1)
			{
				if (class1 == CodeClass.Space)
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
					Cursors.FitterCursor fitterCursor = text.GetCursorInstance (infos[i].CursorId) as Cursors.FitterCursor;
					
					//	Vérifie où il y a des débuts de lignes dans le paragraphe mis
					//	en page. La dernière position correspond à la fin du paragraphe
					//	et doit donc être ignorée :
					
					int[] positions = fitterCursor.GetLineStartPositions (text);
					
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
					Cursors.FitterCursor fitterCursor = text.GetCursorInstance (infos[i].CursorId) as Cursors.FitterCursor;
					
					//	Vérifie où il y a des débuts de lignes dans le paragraphe mis
					//	en page. La première position correspond au début du paragraphe
					//	et n'est donc pas une fin de ligne, par contre tous les autres
					//	débuts de lignes correspondent à la fin de la ligne précédente :
					
					int[] positions = fitterCursor.GetLineStartPositions (text);
					
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
				this.lastCode = 0;
			}
			
			
			public bool Check(ulong code)
			{
				ulong last = this.lastCode;
				
				this.lastCode = code;
				
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
			private ulong						lastCode;
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
		
		public static void HandleManagedParagraphPropertiesChange(TextStory story, ICursor cursor, int offset, Properties.ManagedParagraphProperty[] oldProperties, Properties.ManagedParagraphProperty[] newProperties)
		{
			//	Gère le passage d'un jeu de propriétés ManagedParagraphProperty à
			//	un autre (oldProperties --> newProperties).
			
			//	Pour chaque propriété qui disparaît, le gestionnaire correspondant
			//	sera appelé (DetachFromParagaph); de même, pour chaque propriété
			//	nouvelle, c'est AttachToParagraph qui sera appelé.
			
			System.Diagnostics.Debug.Assert (Navigator.IsParagraphStart (story, cursor, offset));
			
			int nOld = oldProperties == null ? 0 : oldProperties.Length;
			int nNew = newProperties == null ? 0 : newProperties.Length;
			
			if ((nNew == 0) &&
				(nOld == 0))
			{
				return;
			}
			
			if (offset != 0)
			{
				//	Cette méthode travaille toujours avec un offset nul. Si ce
				//	n'est pas le cas au départ, on crée un curseur temporaire
				//	qui satisfait à cette condition :
				
				Cursors.TempCursor tempCursor = new Cursors.TempCursor ();
				
				story.NewCursor (tempCursor);
				story.SetCursorPosition (tempCursor, story.GetCursorPosition (cursor) + offset);
				
				try
				{
					Navigator.HandleManagedParagraphPropertiesChange (story, tempCursor, 0, oldProperties, newProperties);
				}
				finally
				{
					story.RecycleCursor (tempCursor);
				}
				
				return;
			}
			
			System.Diagnostics.Debug.Assert (offset == 0);
			System.Diagnostics.Debug.Assert (Navigator.IsParagraphStart (story, cursor, 0));
			
			Text.TextContext     context = story.TextContext;
			ParagraphManagerList list    = context.ParagraphManagerList;
			
			for (int i = 0; i < nOld; i++)
			{
				bool same = false;
				
				for (int j = 0; j < nNew; j++)
				{
					if (Property.CompareEqualContents (oldProperties[i], newProperties[j]))
					{
						same = true;
						break;
					}
				}
				
				//	Cette ancienne propriété n'a pas d'équivalent dans la liste des
				//	nouvelles propriétés.
				
				if (! same)
				{
					list[oldProperties[i].ManagerName].DetachFromParagraph (story, cursor, oldProperties[i]);
				}
			}
			
			for (int i = 0; i < nNew; i++)
			{
				bool same = false;
				
				for (int j = 0; j < nOld; j++)
				{
					if (Property.CompareEqualContents (newProperties[i], oldProperties[j]))
					{
						same = true;
						break;
					}
				}
				
				//	Cette nouvelle propriété n'a pas d'équivalent dans la liste des
				//	anciennes propriétés.
				
				if (same)
				{
					list[newProperties[i].ManagerName].RefreshParagraph (story, cursor, newProperties[i]);
				}
				else
				{
					list[newProperties[i].ManagerName].AttachToParagraph (story, cursor, newProperties[i]);
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
			
			TextStyle[] allStyles;
			
			context.GetStyles (code, out allStyles);
			
			int count = Properties.StylesProperty.CountMatchingStyles (allStyles, TextStyleClass.Paragraph);
			
			styles = new TextStyle[count];
			
			for (int i = 0, j = 0; j < count; i++)
			{
				if (allStyles[i].TextStyleClass == TextStyleClass.Paragraph)
				{
					styles[j++] = allStyles[i];
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
			
			TextStyle[] allStyles;
			
			context.GetStyles (code, out allStyles);
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < allStyles.Length; i++)
			{
				if ((allStyles[i].TextStyleClass == TextStyleClass.MetaProperty) &&
					(allStyles[i].RequiresUniformParagraph))
				{
					list.Add (allStyles[i]);
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
			
			Property[] allProperties;
			int        count = 0;
			
			context.GetProperties (code, out allProperties);
			
			for (int i = 0; i < allProperties.Length; i++)
			{
				if (allProperties[i].RequiresUniformParagraph)
				{
					count++;
				}
			}
			
			properties = new Property[count];
			
			for (int i = 0, j = 0; j < count; i++)
			{
				if (allProperties[i].RequiresUniformParagraph)
				{
					properties[j++] = allProperties[i];
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
		
		public static void Insert(TextStory story, ICursor cursor, string simpleText, System.Collections.ICollection styles, System.Collections.ICollection properties)
		{
			uint[] utf32;
			TextConverter.ConvertFromString (simpleText, out utf32);
			Navigator.Insert (story, cursor, utf32, styles, properties);
		}
		
		public static void Insert(TextStory story, ICursor cursor, uint[] utf32, System.Collections.ICollection styles, System.Collections.ICollection properties)
		{
			int offset = Navigator.GetParagraphStartOffset (story, cursor);
			
			TextStyle[] paragraphStyles;
			TextStyle[] paragraphMeta;
			Property[]  paragraphProperties;
			
			if ((Navigator.GetParagraphStyles (story, cursor, offset, out paragraphStyles)) &&
				(Navigator.GetUniformMetaProperties (story, cursor, offset, out paragraphMeta)) &&
				(Navigator.GetParagraphProperties (story, cursor, offset, out paragraphProperties)))
			{
				System.Collections.ArrayList allStyles     = new System.Collections.ArrayList ();
				System.Collections.ArrayList allProperties = new System.Collections.ArrayList ();
				
				allStyles.AddRange (paragraphStyles);
				allProperties.AddRange (paragraphProperties);
				
				if (styles != null) allStyles.AddRange (styles);
				if (properties != null) allProperties.AddRange (properties);
				
				allStyles.AddRange (paragraphMeta);
				
				ulong[] text;
				
				story.ConvertToStyledText (utf32, story.FlattenStylesAndProperties (allStyles, allProperties), out text);
				story.InsertText (cursor, text);
			}
		}
		
		
		public static void SetParagraphStyles(TextStory story, ICursor cursor, System.Collections.ICollection styles)
		{
			TextStyle[] sArray = new TextStyle[styles == null ? 0 : styles.Count];
			
			if (styles != null)
			{
				styles.CopyTo (sArray, 0);
			}
			
			Navigator.SetParagraphStyles (story, cursor, sArray);
		}
		
		public static void SetParagraphStyles(TextStory story, ICursor cursor, params TextStyle[] styles)
		{
			if (styles == null)
			{
				styles = new TextStyle[0];
			}
			
			int offsetStart = Navigator.GetParagraphStartOffset (story, cursor);
			int offsetEnd   = Navigator.GetParagraphEndLength (story, cursor);
			
			int length = offsetEnd - offsetStart;
			
			if (length == 0)
			{
				//	Cas particulier : l'appelant essaie de modifier le style du paragraphe
				//	en fin de texte, alors que le paragraphe a une longueur nulle (donc il
				//	n'existe pas encore en tant que tel).
				
				return;
			}
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offsetStart, length, text);
			
			//	Détermine l'état des propriétés "ManagedParagraph" qui déterminent
			//	si/comment un paragraphe est géré (liste à puces, etc.) et dont tout
			//	changement requiert une gestion explicite.
			
			Properties.ManagedParagraphProperty[] oldProps;
			Properties.ManagedParagraphProperty[] newProps;
			
			Navigator.GetManagedParagraphProperties (story, text[0], out oldProps);
			
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
			Navigator.GetManagedParagraphProperties (story, text[0], out newProps);
			
			System.Diagnostics.Debug.Assert (cursor.Attachment == CursorAttachment.Temporary);
			
			int pos = story.GetCursorPosition (cursor);
			story.WriteText (cursor, offsetStart, text);
			story.SetCursorPosition (cursor, pos);
			
			//	Finalement, gère encore les changements de propriétés "ManagedParagraph"
			//	afin d'ajouter ou de supprimer les textes automatiques :
			
			Navigator.HandleManagedParagraphPropertiesChange (story, cursor, offsetStart, oldProps, newProps);
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
			
			int offsetStart = 0;
			int offsetEnd   = length;
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offsetStart, length, text);
			
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
			
			story.WriteText (cursor, offsetStart, text);
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
			
			int offsetStart = 0;
			int offsetEnd   = length;
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offsetStart, length, text);
			
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
			
			story.WriteText (cursor, offsetStart, text);
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
			
			int offsetStart = 0;
			int offsetEnd   = length;
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offsetStart, length, text);
			
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
			
			story.WriteText (cursor, offsetStart, text);
		}
		
		public static void SetParagraphMetaProperties(TextStory story, ICursor cursor, Properties.ApplyMode mode, params TextStyle[] metaProperties)
		{
			int length = Navigator.GetParagraphEndLength (story, cursor);
			int pos    = story.GetCursorPosition (cursor);
			
			Properties.ManagedParagraphProperty[] oldProps;
			Properties.ManagedParagraphProperty[] newProps;
			
			Navigator.GetManagedParagraphProperties (story, story.ReadChar (cursor), out oldProps);
			Navigator.SetMetaProperties (story, cursor, length, mode, metaProperties);
			
			story.SetCursorPosition (cursor, pos);
			
			Navigator.GetManagedParagraphProperties (story, story.ReadChar (cursor), out newProps);
			
			//	Met encore à jour les managed paragraphs (si nécessaire) :
			
			Navigator.HandleManagedParagraphPropertiesChange (story, cursor, 0, oldProps, newProps);
		}
		
		public static void SetMetaProperties(TextStory story, ICursor cursor, int length, Properties.ApplyMode mode, params TextStyle[] metaProperties)
		{
			if (length == 0)
			{
				//	Aucun caractère n'est affecté par la modification; ne fait
				//	rien du tout.
				
				return;
			}
			
			if (metaProperties == null) metaProperties = new TextStyle[0];
			
			int offsetStart = 0;
			int offsetEnd   = length;
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offsetStart, length, text);
			
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
					Navigator.SetMetaProperties (story, text, code, start, count, metaProperties, mode);
					
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
			
			Navigator.SetMetaProperties (story, text, code, start, count, metaProperties, mode);
			
			story.WriteText (cursor, offsetStart, text);
		}
		
		public static void SetParagraphProperties(TextStory story, ICursor cursor, Properties.ApplyMode mode, params Property[] properties)
		{
			int offsetStart = Navigator.GetParagraphStartOffset (story, cursor);
			int offsetEnd   = Navigator.GetParagraphEndLength (story, cursor);
			
			Navigator.SetParagraphProperties (story, cursor, offsetStart, offsetEnd, mode, properties);
		}
		
		public static void SetParagraphProperties(TextStory story, ICursor cursor, int length, Properties.ApplyMode mode, params Property[] properties)
		{
			int offsetStart = 0;
			int offsetEnd   = length;
			
			Navigator.SetParagraphProperties (story, cursor, offsetStart, offsetEnd, mode, properties);
		}
		
		public static void SetParagraphProperties(TextStory story, ICursor cursor, int offsetStart, int offsetEnd, Properties.ApplyMode mode, params Property[] properties)
		{
			int length = offsetEnd - offsetStart;
			
			if (length == 0)
			{
				//	Aucun caractère n'est affecté par la modification; ne fait
				//	rien du tout.
				
				return;
			}
			
			if (properties == null) properties = new Property[0];
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offsetStart, length, text);
			
			Properties.ManagedParagraphProperty[] oldProps;
			Properties.ManagedParagraphProperty[] newProps;
			
			Navigator.GetManagedParagraphProperties (story, text[0], out oldProps);
			
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
			Navigator.GetManagedParagraphProperties (story, text[0], out newProps);
			
			int pos = story.GetCursorPosition (cursor);
			story.WriteText (cursor, offsetStart, text);
			story.SetCursorPosition (cursor, pos);
			
			//	Met encore à jour les managed paragraphs (si nécessaire) :
			
			Navigator.HandleManagedParagraphPropertiesChange (story, cursor, offsetStart, oldProps, newProps);
		}
		
		
		private static void SetParagraphStyles(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] paragraphStyles)
		{
			//	Change le style de paragraphe pour une tranche donnée.
			
			if (length == 0)
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			Property[]  properties;
			TextStyle[] oldStyles;
			
			context.GetProperties (code, out properties);
			context.GetStyles (code, out oldStyles);
			
			//	Crée la table des styles à utiliser en retirant les anciens styles
			//	de paragraphe et en insérant les nouveaux à la place :
			
			int nParaStyles  = Properties.StylesProperty.CountMatchingStyles (oldStyles, TextStyleClass.Paragraph);
			int nOtherStyles = oldStyles.Length - nParaStyles;
			
			TextStyle[] newStyles = new TextStyle[paragraphStyles.Length + nOtherStyles];
			
			System.Array.Copy (paragraphStyles, 0, newStyles, 0, paragraphStyles.Length);
			
			for (int i = 0, j = paragraphStyles.Length; i < oldStyles.Length; i++)
			{
				if (oldStyles[i].TextStyleClass != TextStyleClass.Paragraph)
				{
					newStyles[j++] = oldStyles[i];
				}
			}
			
			//	Crée la table des propriétés à utiliser :
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (newStyles, Property.Filter (properties, Properties.PropertyFilter.NonUniformOnly));
			
			ulong styleBits;
			
			story.ConvertToStyledText (flat, out styleBits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.CoreAndSettingsMask;
				text[offset+i] |= styleBits;
			}
		}
		
		
		private static void SetTextProperties(TextStory story, ulong[] text, ulong code, int offset, int length, Property[] textProperties, Properties.ApplyMode mode)
		{
			//	Modifie les propriétés du texte en utilisant celles passées en
			//	entrée, en se basant sur le mode de combinaison spécifié.
			
			if ((length == 0) ||
				(mode == Properties.ApplyMode.None))
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] currentStyles;
			Property[]  currentProperties;
			
			//	Récupère d'abord les styles et les propriétés courantes :
			
			context.GetStyles (code, out currentStyles);
			context.GetProperties (code, out currentProperties);
			
//			foreach (Property p in currentProperties)
//			{
//				System.Diagnostics.Debug.WriteLine (string.Format ("Current: {0} - {1}.", p.WellKnownType, p.ToString ()));
//			}
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (currentProperties) == false);
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (textProperties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (currentProperties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (textProperties) == false);
			
			//	Dans un premier temps, on conserve tous les styles et ne garde que les
			//	propriétés associées directement au paragraphe. Les autres propriétés
			//	vont devoir être filtrées :

			List<TextStyle> allStyles = new List<TextStyle> ();
			List<Property>  allProperties = new List<Property> ();
			
			allStyles.AddRange (currentStyles);
			
			allProperties.AddRange (Property.Filter (currentProperties, Properties.PropertyFilter.UniformOnly));
			allProperties.AddRange (Navigator.Combine (Property.Filter (currentProperties, Properties.PropertyFilter.NonUniformOnly), textProperties, mode));
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (allStyles, allProperties);
			
			ulong styleBits;
			
			story.ConvertToStyledText (flat, out styleBits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.CoreAndSettingsMask;
				text[offset+i] |= styleBits;
			}
		}
		
		private static void SetMetaProperties(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] metaProperties, Properties.ApplyMode mode)
		{
			//	Modifie les méta propriétés du texte en utilisant celles passées en
			//	entrée, en se basant sur le mode de combinaison spécifié.
			
			if ((length == 0) ||
				(mode == Properties.ApplyMode.None))
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] currentStyles;
			Property[]  currentProperties;
			
			//	Récupère d'abord les styles et les propriétés courantes :
			
			context.GetStyles (code, out currentStyles);
			context.GetProperties (code, out currentProperties);
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (currentProperties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (currentProperties) == false);

			List<TextStyle> allStyles = new List<TextStyle> ();
			ICollection<TextStyle> combined = Navigator.Combine (TextStyle.FilterStyles (currentStyles, TextStyleClass.MetaProperty), metaProperties, mode);
			
			allStyles.AddRange (TextStyle.FilterStyles (currentStyles, TextStyleClass.Paragraph));
			allStyles.AddRange (TextStyle.FilterStyles (currentStyles, TextStyleClass.Text));
			allStyles.AddRange (TextStyle.FilterStyles (currentStyles, TextStyleClass.Symbol));
			allStyles.AddRange (Navigator.Combine (TextStyle.FilterStyles (currentStyles, TextStyleClass.MetaProperty), metaProperties, mode));
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (allStyles, currentProperties);
			
			ulong styleBits;
			
			story.ConvertToStyledText (flat, out styleBits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.CoreAndSettingsMask;
				text[offset+i] |= styleBits;
			}
		}
		
		private static void SetParagraphProperties(TextStory story, ulong[] text, ulong code, int offset, int length, Property[] paragraphProperties, Properties.ApplyMode mode)
		{
			//	Modifie les propriétés du paragraphe en utilisant celles passées en
			//	entrée, en se basant sur le mode de combinaison spécifié.
			
			if ((length == 0) ||
				(mode == Properties.ApplyMode.None))
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] currentStyles;
			Property[]  currentProperties;
			
			//	Récupère d'abord les styles et les propriétés courantes :
			
			context.GetStyles (code, out currentStyles);
			context.GetProperties (code, out currentProperties);
			
//			foreach (Property p in currentProperties)
//			{
//				System.Diagnostics.Debug.WriteLine (string.Format ("Current: {0} - {1}.", p.WellKnownType, p.ToString ()));
//			}
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (currentProperties) == false);
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (paragraphProperties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (currentProperties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (paragraphProperties) == false);
			
			//	Dans un premier temps, on ne conserve tous les styles et que les
			//	propriétés associés directement au paragraphe. Les autres propriétés
			//	vont devoir être filtrées :
			
			List<TextStyle> allStyles = new List<TextStyle> ();
			List<Property> allProperties = new List<Property> ();
			
			allStyles.AddRange (currentStyles);
			
			allProperties.AddRange (Navigator.Combine (Property.Filter (currentProperties, Properties.PropertyFilter.UniformOnly), paragraphProperties, mode));
			allProperties.AddRange (Property.Filter (currentProperties, Properties.PropertyFilter.NonUniformOnly));
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (allStyles, allProperties);
			
			ulong styleBits;
			
			story.ConvertToStyledText (flat, out styleBits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.CoreAndSettingsMask;
				text[offset+i] |= styleBits;
			}
		}
		
		
		
		private static void SetSymbolStyles(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] characterStyles)
		{
			//	Remplace les styles de symboles par ceux passés en entrée.
			
			if (length == 0)
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] currentStyles;
			Property[]  currentProperties;
			
			//	Récupère d'abord les styles et les propriétés associées au texte
			//	actuel :
			
			context.GetStyles (code, out currentStyles);
			context.GetProperties (code, out currentProperties);
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (currentProperties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (currentProperties) == false);
			
			//	Ne conserve que les styles associés directement au paragraphe et
			//	au texte. En fait, on supprime les styles liés aux symboles :
			
			TextStyle[] filteredStyles = TextStyle.FilterStyles (currentStyles, TextStyleClass.Paragraph, TextStyleClass.Text, TextStyleClass.MetaProperty);
			TextStyle[] allStyles      = new TextStyle[filteredStyles.Length + characterStyles.Length];
			Property[]  allProperties  = currentProperties;
			
			System.Array.Copy (filteredStyles, 0, allStyles, 0, filteredStyles.Length);
			System.Array.Copy (characterStyles, 0, allStyles, filteredStyles.Length, characterStyles.Length);
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (allStyles, allProperties);
			
			ulong styleBits;
			
			story.ConvertToStyledText (flat, out styleBits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.CoreAndSettingsMask;
				text[offset+i] |= styleBits;
			}
		}
		
		private static void SetTextStyles(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] textStyles)
		{
			//	Remplace les styles de texte par ceux passés en entrée.
			
			if (length == 0)
			{
				return;
			}
			
			Text.TextContext context = story.TextContext;
			
			TextStyle[] currentStyles;
			Property[]  currentProperties;
			
			//	Récupère d'abord les styles et les propriétés associées au texte
			//	actuel :
			
			context.GetStyles (code, out currentStyles);
			context.GetProperties (code, out currentProperties);
			
			System.Diagnostics.Debug.Assert (Properties.PropertiesProperty.ContainsPropertiesProperties (currentProperties) == false);
			System.Diagnostics.Debug.Assert (Properties.StylesProperty.ContainsStylesProperties (currentProperties) == false);
			
			//	Ne conserve que les styles associés directement au paragraphe et
			//	aux symboles. En fait, on supprime les styles liés au texte :
			
			TextStyle[] filteredStyles = TextStyle.FilterStyles (currentStyles, TextStyleClass.Paragraph, TextStyleClass.Symbol, TextStyleClass.MetaProperty);
			TextStyle[] allStyles      = new TextStyle[filteredStyles.Length + textStyles.Length];
			Property[]  allProperties  = currentProperties;
			
			System.Array.Copy (filteredStyles, 0, allStyles, 0, filteredStyles.Length);
			System.Array.Copy (textStyles, 0, allStyles, filteredStyles.Length, textStyles.Length);
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (allStyles, allProperties);
			
			ulong styleBits;
			
			story.ConvertToStyledText (flat, out styleBits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.CoreAndSettingsMask;
				text[offset+i] |= styleBits;
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
