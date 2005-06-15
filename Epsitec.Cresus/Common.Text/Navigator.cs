//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe Navigator gère les déplacements au sein d'un texte.
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
			//	Retourne l'offset au début du paragraphe. L'offset est négatif
			//	ou nul, dans tous les cas, et correspond à une distance relative
			//	entre la position courante et le premier caractère du paragraphe.
			
			TextStory.CodeCallback callback = new TextStory.CodeCallback (Navigator.IsParagraphSeparator);
			
			int distance = story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, - distance, callback);
			
			return (offset == -1) ? -distance : -offset;
		}
		
		public static int GetParagraphEndLength(TextStory story, ICursor cursor)
		{
			//	Retourne la longueur du paragraph depuis la position courante
			//	jusqu'à sa fin, y compris le caractère de terminaison.
			
			TextStory.CodeCallback callback = new TextStory.CodeCallback (Navigator.IsParagraphSeparator);
			
			int distance = story.TextLength - story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, distance, callback);
			
			return (offset == -1) ? distance : offset+1;
		}
		
		
		public static bool GetParagraphStyles(TextStory story, ICursor cursor, int offset, out TextStyle[] styles)
		{
			//	Retourne les styles de paragraphe attachés au paragraphe à la
			//	position indiquée.
			
			ulong code = story.TextTable.ReadChar (cursor.CursorId, offset);
			
			if (code == 0)
			{
				styles = null;
				return false;
			}
			
			Styles.SimpleStyle        simple_style    = story.TextContext.StyleList[code];
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
		
		public static bool GetParagraphProperties(TextStory story, ICursor cursor, int offset, out Properties.BaseProperty[] properties)
		{
			//	Retourne les propriétés attachées au paragraphe de la position
			//	indiquée, en excluant les propriétés dérivées à partir des
			//	styles.
			
			ulong code = story.TextTable.ReadChar (cursor.CursorId, offset);
			
			if (code == 0)
			{
				properties = null;
				return false;
			}
			
			Styles.SimpleStyle            simple_style   = story.TextContext.StyleList[code];
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
			
			int offset = Navigator.GetParagraphStartOffset (story, cursor);
			
			TextStyle[]               paragraph_styles;
			Properties.BaseProperty[] paragraph_properties;
			
			if ((Navigator.GetParagraphStyles (story, cursor, offset, out paragraph_styles)) &&
				(Navigator.GetParagraphProperties (story, cursor, offset, out paragraph_properties)))
			{
				ulong[] text;
				
				//	Insère un saut de paragraphe :
				
				story.ConvertToStyledText ("\u2029", paragraph_styles, paragraph_properties, out text);
				story.InsertText (cursor, text);
			}
		}
		
		public static void SetParagraphStylesAndProperties(TextStory story, ICursor cursor, System.Collections.ICollection styles, System.Collections.ICollection properties)
		{
			TextStyle[]               s_array = new TextStyle[styles == null ? 0 : styles.Count];
			Properties.BaseProperty[] p_array = new Properties.BaseProperty[properties == null ? 0 : properties.Count];
			
			if (styles != null) styles.CopyTo (s_array, 0);
			if (properties != null) properties.CopyTo (p_array, 0);
			
			Navigator.SetParagraphStylesAndProperties (story, cursor, s_array, p_array);
		}
		
		public static void SetParagraphStylesAndProperties(TextStory story, ICursor cursor, TextStyle[] styles, Properties.BaseProperty[] properties)
		{
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
		
		private static void SetParagraphStylesAndProperties(TextStory story, ulong[] text, ulong code, int offset, int length, TextStyle[] paragraph_styles, Properties.BaseProperty[] paragraph_properties)
		{
			if (length == 0)
			{
				return;
			}
			
			Styles.SimpleStyle            simple = story.TextContext.StyleList[code];
			Properties.StylesProperty     s_prop = simple[Properties.WellKnownType.Styles] as Properties.StylesProperty;
			Properties.PropertiesProperty p_prop = simple[Properties.WellKnownType.Properties] as Properties.PropertiesProperty;
			
			string[]       serialized_other_properties = (p_prop == null) ? null : p_prop.SerializedOtherProperties;
			Properties.BaseProperty[] other_properties = Properties.PropertiesProperty.DeserializeProperties (story.TextContext, serialized_other_properties);
			
			//	Crée la table des styles à utiliser en retirant les anciens styles
			//	de paragraphe et en insérant les nouveaux à la place :
			
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
			
			//	Crée la table des propriétés à utiliser :
			
			Properties.BaseProperty[] new_properties = new Properties.BaseProperty[paragraph_properties.Length + other_properties.Length];
			
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
	}
}
