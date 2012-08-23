//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>XmlExtractor</c> class can be used to extract a piece of XML contained
	/// within a collection of lines. It recognizes the end of the XML. 
	/// </summary>
	/// <remarks>
	/// Any trailing XML comments will not be taken in account by the extractor; as
	/// soon as a valid end of XML is reached, the extractor will consider that any
	/// remaining text is in excess.
	/// </remarks>
	public class XmlExtractor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlExtractor"/> class.
		/// </summary>
		public XmlExtractor()
		{
			this.buffer = new StringBuilder ();
			this.depth = -1;
		}


		/// <summary>
		/// Gets a value indicating whether this <see cref="XmlExtractor"/> is finished.
		/// </summary>
		/// <value>
		///   <c>true</c> if finished; otherwise, <c>false</c>.
		/// </value>
		public bool								Finished
		{
			get
			{
				return this.open == 0 && this.depth == 0 && this.elementCount > 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="XmlExtractor"/> has started
		/// extracting XML. This will be the case as soon as an XML comment or an XML
		/// element has been processed. The XML declaration does not count.
		/// </summary>
		/// <value>
		///   <c>true</c> if the extraction started; otherwise, <c>false</c>.
		/// </value>
		public bool								Started
		{
			get
			{
				return this.open > 0 || this.depth > -1;
			}
		}

		/// <summary>
		/// Gets the excess text, after the last piece of XML.
		/// </summary>
		public string							ExcessText
		{
			get
			{
				return this.excessText ?? "";
			}
		}


		/// <summary>
		/// Appends a line, including the <c>\n</c> character.
		/// </summary>
		/// <param name="line">The line.</param>
		public void AppendLine(string line)
		{
			this.Append (line);

			if (this.Finished)
			{
				if (this.excessText != null)
				{
					this.excessText = this.excessText + "\n";
				}
				else
				{
					this.buffer.Append ('\n');
				}
			}
			else
			{
				this.buffer.Append ('\n');
			}
		}

		/// <summary>
		/// Appends some text.
		/// </summary>
		/// <param name="text">The text.</param>
		public void Append(string text)
		{
			if (this.Finished)
			{
				throw new System.InvalidOperationException ("The XML extractor cannot accept additional lines: it is already in the finished state");
			}

			char prev0 = ' ';
			char prev1 = ' ';
			char prev2 = ' ';
			
			int length = 0;

			foreach (char c in text)
			{
				if ((this.Finished) &&
					(char.IsWhiteSpace (c) == false))
				{
					this.excessText = text.Substring (length);
					break;
				}

				if (this.inComment)
				{
					if ((prev1 == '-') &&			//	"...-->"
						(prev0 == '-') &&
						(c == '>'))
					{
						this.inComment = false;
						this.open--;
						this.depth--;
					}

					goto append;
				}
				
				if (c == '<')
				{
					if ((this.inDeclaration) ||
						(this.open > 0))
					{
						throw new System.FormatException ("Invalid XML syntax");
					}
					
					this.open++;
				}
				else if (c == '>')
				{
					if (this.open != 1)
					{
						throw new System.FormatException ("Invalid XML syntax");
					}

					this.open--;
					
					if (this.inDeclaration)
					{
						if (prev0 == '?')		//	"..?>"
						{
							if (prev1 == '<')
							{
								throw new System.FormatException ("The XML declaration is invalid");
							}

							this.inDeclaration = false;
						}
						else
						{
							throw new System.FormatException ("The XML declaration is invalid");
						}
					}
					else
					{
						if (prev0 == '/')		//	"<xxx/>"
						{						//	     ^
							this.elementCount++;
							this.depth--;
						}
					}
				}
				else if (c == '/')
				{
					if (prev0 == '<')			//	"</xxx>"
					{							//	  ^
						this.elementCount++;
						this.depth--;
					}
				}
				else if (prev0 == '<')			//	"<xxx.."
				{								//	  ^
					if (c == '?')
					{							//	"<?.."
						if (this.depth > 0)
						{
							throw new System.FormatException ("The XML cannot contain a declaration inside of valid the XML tree");
						}
						this.inDeclaration = true;
					}
					else
					{
						if (this.depth == -1)
						{
							this.depth = 0;
						}
						this.depth++;
					}
				}
				else if ((prev2 == '<') && (prev1 == '!') && (prev0 == '-') && (c == '-'))
				{
					this.inComment = true;		//	"<!--..."
					
					//	Avoid that part of the "<!--" is used as the start of "-->" by clearing
					//	the history:
					
					prev2 = ' ';
					prev1 = ' ';
					prev0 = ' ';
					
					this.buffer.Append ('-');
					length++;
					
					continue;
				}
				else if (char.IsWhiteSpace (c) == false)
				{
					if ((this.open == 0) &&
						(this.depth < 1))
					{
						if ((this.inComment == false) &&
							(this.inDeclaration == false))
						{
							throw new System.FormatException ("The XML data contains text outside of an XML element");
						}
					}
				}

			append:
				prev2 = prev1;
				prev1 = prev0;
				prev0 = c;

				this.buffer.Append (c);
				length++;
			}
		}


		/// <summary>
		/// Returns the extracted text.
		/// </summary>
		/// <returns>
		/// The extracted text.
		/// </returns>
		public override string ToString()
		{
			return this.buffer.ToString ();
		}


		private readonly StringBuilder			buffer;
		private string							excessText;
		private int								depth;
		private int								open;
		private int								elementCount;
		private bool							inDeclaration;
		private bool							inComment;
	}
}
