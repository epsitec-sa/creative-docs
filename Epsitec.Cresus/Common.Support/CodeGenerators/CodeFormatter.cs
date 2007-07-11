//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeGenerators
{
	class CodeFormatter
	{
		public CodeFormatter(System.Text.StringBuilder buffer)
		{
			this.output = buffer;
		}

		
		public int IndentationLevel
		{
			get
			{
				return this.indentationLevel;
			}
			set
			{
				if (this.indentationLevel != value)
				{
					this.indentationLevel  = value;
					this.indentationString = new string ('\t', this.indentationLevel);
				}
			}
		}

		public string IndentationString
		{
			get
			{
				return this.indentationString;
			}
		}


		public void IncreaseIndentation()
		{
			this.IndentationLevel++;
		}

		public void DecreaseIndentation()
		{
			this.IndentationLevel--;
		}

		public void WriteBeginBlock()
		{
			this.WriteBeginLine ();
			this.output.Append (Strings.BlockBegin);
			this.WriteEndLine ();
			this.IncreaseIndentation ();
		}

		public void WriteEndBlock()
		{
			this.DecreaseIndentation ();
			this.WriteBeginLine ();
			this.output.Append (Strings.BlockEnd);
			this.WriteEndLine ();
		}

		public void WriteBeginLine()
		{
			if (this.outputState != State.EmptyLineWithIndentation)
			{
				this.WriteEndLine ();
				this.output.Append (this.IndentationString);
				this.outputState = State.EmptyLineWithIndentation;
			}
		}

		public void WriteEndLine()
		{
			if (this.outputState != State.EmptyLine)
			{
				this.output.Append (Strings.LineSeparator);
				this.outputState = State.EmptyLine;
			}
		}

		public void WriteBeginProperty(string code)
		{
			this.WriteBeginLine ();
			this.WriteCode (code);
			this.WriteBeginBlock ();
		}

		public void WriteEndProperty()
		{
			this.WriteEndBlock ();
		}

		public void WriteBeginSetter(CodeAttributes attributes)
		{
			this.WriteBeginLine ();

			if (attributes.Visibility != CodeVisibility.Public)
			{
				this.WriteCode (attributes.ToString ());
			}

			this.WriteCode ("set");
			this.WriteBeginBlock ();
		}
		
		public void WriteEndSetter()
		{
			this.WriteEndBlock ();
		}

		public void WriteBeginGetter(CodeAttributes attributes)
		{
			this.WriteBeginLine ();
			
			if (attributes.Visibility != CodeVisibility.Public)
			{
				this.WriteCode (attributes.ToString ());
			}
			
			this.WriteCode ("get");
			this.WriteBeginBlock ();
		}

		public void WriteEndGetter()
		{
			this.WriteEndBlock ();
		}
		
		public void WriteCode(string code)
		{
			if (string.IsNullOrEmpty (code))
			{
				return;
			}

			char codeStart = code[0];

			if (char.IsLetterOrDigit (codeStart))
			{
				//	When inserting symbols or numbers, we want to make sure there is
				//	a space before the current text :

				if (this.output.Length > 0)
				{
					char lastChar = this.output[this.output.Length-1];

					switch (lastChar)
					{
						case ' ':
						case '\t':
						case '\n':
						case '\r':
						case '(':
							//	No need to insert a space character...
							break;
						
						default:
							this.output.Append (" ");
							break;
					}
				}
			}

			this.output.Append (code);
		}

		
		
		enum State
		{
			EmptyLine,
			EmptyLineWithIndentation,
			PartialLine
		}


		static class Strings
		{
			public const string LineSeparator = "\n";
			public const string BlockBegin = "{";
			public const string BlockEnd = "}";
		}
		
		System.Text.StringBuilder output;
		int indentationLevel;
		string indentationString;
		State outputState;
	}
}
