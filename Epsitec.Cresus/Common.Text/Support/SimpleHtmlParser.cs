//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Support
{
	/// <summary>
	/// Summary description for SimpleHtmlParser.
	/// </summary>
	public class SimpleHtmlParser
	{
		public SimpleHtmlParser(TextStory story, ICursor cursor)
		{
			this.story  = story;
			this.cursor = cursor;
			
			Debug.Assert.IsTrue (cursor == this.story.TextTable.GetCursorInstance (cursor.CursorId));
		}
		
		
		public TextStyle						DefaultTextStyle
		{
			get
			{
				return this.style;
			}
			set
			{
				if (this.style != value)
				{
					this.style = value;
				}
			}
		}
		
		
		public void Parse(string text)
		{
			int offset = 0;
			
			while (offset < text.Length)
			{
				char c = text[offset];
				
				if (c == '<')
				{
					//	Isole le tag depuis le '<' jusqu'au '>' :
					
					int end = text.IndexOf ('>', offset);
					
					if (end < offset)
					{
						throw new System.FormatException (string.Format ("Unmatched '<' in XML source."));
					}
					
					this.ParseTag (text, offset, end - offset + 1);
					
					offset = end + 1;
				}
				else
				{
					this.Append (Common.Support.Utilities.ParseCharOrXmlEntity (text, ref offset));
				}
			}
		}
		
		
		private void ParseTag(string text, int offset, int count)
		{
			System.Diagnostics.Debug.Assert (text[offset] == '<');
			System.Diagnostics.Debug.Assert (text[offset + count - 1] == '>');
			
			bool is_simple = (text[offset + count - 2] == '/');
			int  space_pos = text.IndexOf (' ', offset, count);
			int  tag_length;
			
			if (space_pos > offset)
			{
				tag_length = space_pos - offset - 1;
			}
			else
			{
				tag_length = is_simple ? count - 3 : count - 2;
			}
			
			string tag_name = text.Substring (offset + 1, tag_length);
			string tag_args = text.Substring (offset + 1 + tag_length, count - tag_length - (is_simple ? 2 : 1) - 1).Trim ();
			
			if (is_simple)
			{
				switch (tag_name)
				{
					case "br":
						this.Append (Unicode.Code.LineSeparator);
						return;
					
					case "tab":
						this.Append (Unicode.Code.HorizontalTab);
						return;
					
					default:
						break;
				}
			}
			
			if (this.buffer.Length > 0)
			{
				this.GenerateStyledText (this.buffer.ToString ());
				this.buffer.Length = 0;
			}
			
			System.Diagnostics.Debug.Assert (is_simple == false);
			
			string[] args;
			
			Common.Support.Utilities.StringToTokens (tag_args, ' ', out args);
			
			if (tag_name[0] == '/')
			{
				this.PopElement (tag_name.Substring (1));
			}
			else
			{
				this.PushElement (tag_name, args);
			}
		}
		
		private void PushElement(string tag, string[] args)
		{
			this.stack.Push (new Element (tag, args));
		}
		
		private void PopElement(string tag)
		{
			Element elem = this.stack.Pop () as Element;
			
			if (elem.Tag != tag)
			{
				throw new System.FormatException (string.Format ("Found '</{0}>' instead of '</{1}>' in XML source.", tag, elem.Tag));
			}
		}
		
		
		private class Element
		{
			public Element(string tag, string[] args)
			{
				this.tag  = tag;
				this.args = args;
			}
			
			
			public string						Tag
			{
				get
				{
					return this.tag;
				}
			}
			
			public string[]						Args
			{
				get
				{
					return this.args;
				}
			}
			
			
			private string						tag;
			private string[]					args;
		}
		
		
		
		private void Append(char c)
		{
			switch (c)
			{
				case (char) Unicode.Code.LineSeparator:
				case (char) Unicode.Code.LineFeed:
				case (char) Unicode.Code.ParagraphSeparator:
				case (char) Unicode.Code.PageSeparator:
					this.tab_count = 0;
					break;
				
				case (char) Unicode.Code.HorizontalTab:
					this.AppendHorizontalTab ();
					return;
			}
			
			this.buffer.Append (c);
		}
		
		private void Append(Unicode.Code c)
		{
			this.Append ((char) c);
		}
		
		private void AppendHorizontalTab()
		{
			//	TODO: ajouter la définition du tabulateur en question
			
			this.tab_count++;
		}
		
		
		private void GenerateStyledText(string text)
		{
			ulong[] styled_text;
			
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			Element[] elements = new Element[this.stack.Count];
			
			this.stack.CopyTo (elements, 0);
			
			foreach (Element element in elements)
			{
				switch (element.Tag)
				{
					case "b":
					case "i":
					case "u":
					case "w":
					case "m":
					case "font":
					case "a":
					case "list":
						break;
				}
			}
			
			this.story.ConvertToStyledText (text, this.style, properties, out styled_text);
			this.story.InsertText (this.cursor, styled_text);
		}
		
		
		private TextStory						story;
		private ICursor							cursor;
		private TextStyle						style;
		private System.Text.StringBuilder		buffer			= new System.Text.StringBuilder ();
		private System.Collections.Stack		stack			= new System.Collections.Stack ();
		
		private int								tab_count;
	}
}
