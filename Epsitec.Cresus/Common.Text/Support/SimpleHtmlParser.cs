//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			
			bool isSimple = (text[offset + count - 2] == '/');
			int  spacePos = text.IndexOf (' ', offset, count);
			int  tagLength;
			
			if (spacePos > offset)
			{
				tagLength = spacePos - offset - 1;
			}
			else
			{
				tagLength = isSimple ? count - 3 : count - 2;
			}
			
			string tagName = text.Substring (offset + 1, tagLength);
			string tagArgs = text.Substring (offset + 1 + tagLength, count - tagLength - (isSimple ? 2 : 1) - 1).Trim ();
			
			if (isSimple)
			{
				switch (tagName)
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
			
			System.Diagnostics.Debug.Assert (isSimple == false);
			
			string[] args;
			
			Common.Support.Utilities.StringToTokens (tagArgs, ' ', out args);
			
			if (tagName[0] == '/')
			{
				this.PopElement (tagName.Substring (1));
			}
			else
			{
				this.PushElement (tagName, args);
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
					this.tabCount = 0;
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
			
			this.tabCount++;
		}
		
		
		private void GenerateStyledText(string text)
		{
			ulong[] styledText;
			
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
			
			this.story.ConvertToStyledText (text, this.style, properties, out styledText);
			this.story.InsertText (this.cursor, styledText);
		}
		
		
		private TextStory						story;
		private ICursor							cursor;
		private TextStyle						style;
		private System.Text.StringBuilder		buffer			= new System.Text.StringBuilder ();
		private System.Collections.Stack		stack			= new System.Collections.Stack ();
		
		private int								tabCount;
	}
}
