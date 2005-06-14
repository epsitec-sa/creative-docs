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
			TextStory.CodeCallback callback = new TextStory.CodeCallback (Navigator.IsParagraphSeparator);
			
			int distance = story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, - distance, callback);
			
			return (offset == -1) ? -distance : -offset;
		}
		
		public static int GetParagraphEndOffset(TextStory story, ICursor cursor)
		{
			TextStory.CodeCallback callback = new TextStory.CodeCallback (Navigator.IsParagraphSeparator);
			
			int distance = story.TextLength - story.GetCursorPosition (cursor);
			int offset   = story.TextTable.TraverseText (cursor.CursorId, distance, callback);
			
			return (offset == -1) ? distance : offset;
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
		
		public static void SetParagraphStylesAndProperties(TextStory story, ICursor cursor, TextStyle[] styles, Properties.BaseProperty[] properties)
		{
			int offset_start = Navigator.GetParagraphStartOffset (story, cursor);
			int offset_end   = Navigator.GetParagraphEndOffset (story, cursor);
			
			int length = offset_end - offset_start;
			
			ulong[] text = new ulong[length];
			ulong[] copy = new ulong[length];
			
			story.ReadText (cursor, offset_start, length, text);
			
			//	...
			
			story.WriteText (cursor, offset_start, copy);
		}
	}
}
