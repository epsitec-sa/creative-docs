//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe Navigator g�re les d�placements au sein d'un texte.
	/// </summary>
	public sealed class Navigator
	{
		public static bool IsParagraphStart(TextStory story, ICursor cursor, int offset)
		{
			Unicode.Code code = Unicode.Bits.GetUnicodeCode (story.TextTable.ReadChar (cursor.CursorId, offset - 1));
			
			return (code == Unicode.Code.Null) || Navigator.IsParagraphSeparator (code);
		}
		
		public static bool IsParagraphEnd(TextStory story, ICursor cursor, int offset)
		{
			Unicode.Code code = Unicode.Bits.GetUnicodeCode (story.TextTable.ReadChar (cursor.CursorId, offset));
			
			return Navigator.IsParagraphSeparator (code);
		}
		
		public static bool IsParagraphSeparator(Unicode.Code code)
		{
			switch (code)
			{
				case Unicode.Code.PageSeparator:
				case Unicode.Code.ParagraphSeparator:
				case Unicode.Code.LineSeparator:
					return true;
			}
			
			return false;
		}
		
		public static bool IsParagraphSeparator(ulong code)
		{
			return Navigator.IsParagraphSeparator (Unicode.Bits.GetUnicodeCode (code));
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
		
		
		public static int GetRunLength(TextStory story, ICursor cursor, Property property)
		{
			//	Retourne la longueur du texte auquel est appliqu�e la propri�t�
			//	pass�e en entr�e.
			
			Navigator.PropertyFinder finder = new PropertyFinder (story.StyleList, property, story.TextTable.ReadChar (cursor.CursorId));
			TextStory.CodeCallback callback = new TextStory.CodeCallback (finder.MissingProperty);
			
			int distance = story.TextLength - story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, distance, callback);
			
			return (offset == -1) ? distance : offset;
		}
		
		
		public static bool GetFlattenedProperties(TextStory story, ICursor cursor, int offset, out Property[] properties)
		{
			//	Retourne toutes les propri�t�s (fusionn�es, telles que stock�es
			//	dans le texte) pour la position indiqu�e.
			
			ulong code = story.TextTable.ReadChar (cursor.CursorId, offset);
			
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
		
		public static bool GetParagraphStyles(TextStory story, ICursor cursor, int offset, out TextStyle[] styles)
		{
			//	Retourne les styles de paragraphe attach�s au paragraphe � la
			//	position indiqu�e.
			
			ulong code = story.TextTable.ReadChar (cursor.CursorId, offset);
			
			if (code == 0)
			{
				styles = null;
				return false;
			}
			
			Styles.SimpleStyle        simple_style    = story.StyleList[code];
			Properties.StylesProperty styles_property = simple_style[Properties.WellKnownType.Styles] as Properties.StylesProperty;
			TextStyle[]               all_styles      = styles_property.Styles;
			
			int count = 0;
			int index = 0;
			
			foreach (TextStyle style in all_styles)
			{
				if (style.TextStyleClass == TextStyleClass.Paragraph)
				{
					count++;
				}
			}
			
			styles = new TextStyle[count];
			
			foreach (TextStyle style in all_styles)
			{
				if (style.TextStyleClass == TextStyleClass.Paragraph)
				{
					styles[index++] = style;
					
					if (index == count)
					{
						break;
					}
				}
			}
			
			return true;
		}
		
		public static bool GetParagraphProperties(TextStory story, ICursor cursor, int offset, out Property[] properties)
		{
			//	Retourne les propri�t�s attach�es au paragraphe de la position
			//	indiqu�e, en excluant les propri�t�s d�riv�es � partir des
			//	styles.
			
			ulong code = story.TextTable.ReadChar (cursor.CursorId, offset);
			
			if (code == 0)
			{
				properties = null;
				return false;
			}
			
			Styles.SimpleStyle            simple_style   = story.StyleList[code];
			Properties.PropertiesProperty props_property = simple_style[Properties.WellKnownType.Properties] as Properties.PropertiesProperty;
			
			if (props_property == null)
			{
				properties = new Properties.PropertiesProperty[0];
			}
			else
			{
				Context  context    = story.TextContext;
				string[] serialized = props_property.SerializedUniformParagraphProperties;
				
				properties = Properties.PropertiesProperty.DeserializeProperties (context, serialized);
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
		
		
		public static void SetParagraphStylesAndProperties(TextStory story, ICursor cursor, System.Collections.ICollection styles, System.Collections.ICollection properties)
		{
			TextStyle[] s_array = new TextStyle[styles == null ? 0 : styles.Count];
			Property[]  p_array = new Property[properties == null ? 0 : properties.Count];
			
			if (styles != null) styles.CopyTo (s_array, 0);
			if (properties != null) properties.CopyTo (p_array, 0);
			
			Navigator.SetParagraphStylesAndProperties (story, cursor, s_array, p_array);
		}
		
		public static void SetParagraphStylesAndProperties(TextStory story, ICursor cursor, TextStyle[] styles, Property[] properties)
		{
			if (styles == null)     styles = new TextStyle[0];
			if (properties == null) properties = new Property[0];
			
			int offset_start = Navigator.GetParagraphStartOffset (story, cursor);
			int offset_end   = Navigator.GetParagraphEndLength (story, cursor);
			
			int length = offset_end - offset_start;
			
			ulong[] text = new ulong[length];
			
			story.ReadText (cursor, offset_start, length, text);
			
			ulong code  = 0;
			int   start = 0;
			int   count = 0;
			
			for (int i = 0; i < length; i++)
			{
				ulong next = Internal.CharMarker.ExtractStyleAndSettings (text[i]);
				
				if (code != next)
				{
					Navigator.SetParagraphStylesAndProperties (story, text, code, start, count, styles, properties);
					
					start = i;
					count = 1;
					code  = next;
				}
				else
				{
					count++;
				}
			}
			
			Navigator.SetParagraphStylesAndProperties (story, text, code, start, count, styles, properties);
			
			story.WriteText (cursor, offset_start, text);
		}
		
		
		private static void SetParagraphStylesAndProperties(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] paragraph_styles, Property[] paragraph_properties)
		{
			if (length == 0)
			{
				return;
			}
			
			Styles.SimpleStyle            simple = story.StyleList[code];
			Properties.StylesProperty     s_prop = simple[Properties.WellKnownType.Styles] as Properties.StylesProperty;
			Properties.PropertiesProperty p_prop = simple[Properties.WellKnownType.Properties] as Properties.PropertiesProperty;
			
			string[]   serialized_other_properties = (p_prop == null) ? null : p_prop.SerializedOtherProperties;
			Property[] other_properties = Properties.PropertiesProperty.DeserializeProperties (story.TextContext, serialized_other_properties);
			
			//	Cr�e la table des styles � utiliser en retirant les anciens styles
			//	de paragraphe et en ins�rant les nouveaux � la place :
			
			TextStyle[] old_styles = s_prop.Styles;
			TextStyle[] new_styles = new TextStyle[paragraph_styles.Length + s_prop.CountOtherStyles];
			
			System.Array.Copy (paragraph_styles, 0, new_styles, 0, paragraph_styles.Length);
			
			int index = paragraph_styles.Length;
			
			for (int i = 0; i < old_styles.Length; i++)
			{
				if (old_styles[i].TextStyleClass != TextStyleClass.Paragraph)
				{
					new_styles[index++] = old_styles[i];
				}
			}
			
			System.Diagnostics.Debug.Assert (index == new_styles.Length);
			
			//	Cr�e la table des propri�t�s � utiliser :
			
			Property[] new_properties = new Property[paragraph_properties.Length + other_properties.Length];
			
			System.Array.Copy (paragraph_properties, 0, new_properties, 0, paragraph_properties.Length);
			System.Array.Copy (other_properties, 0, new_properties, paragraph_properties.Length, other_properties.Length);
			
			System.Collections.ArrayList flat = story.FlattenStylesAndProperties (new_styles, new_properties);
			
			ulong style_bits;
			
			story.ConvertToStyledText (flat, out style_bits);
			
			for (int i = 0; i < length; i++)
			{
				text[offset+i] &= ~ Internal.CharMarker.StyleAndSettingsMask;
				text[offset+i] |= style_bits;
			}
		}
		
		
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
	}
}
