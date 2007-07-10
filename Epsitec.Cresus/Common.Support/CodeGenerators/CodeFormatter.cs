//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeGenerators
{
	public class CodeFormatter
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

		public void BeginBlock()
		{
			this.BeginLine ();
			this.output.Append (Strings.BlockBegin);
			this.EndLine ();
			this.IncreaseIndentation ();
		}

		public void EndBlock()
		{
			this.DecreaseIndentation ();
			this.BeginLine ();
			this.output.Append (Strings.BlockEnd);
			this.EndLine ();
		}

		public void BeginLine()
		{
			if (this.outputState != State.EmptyLineWithIndentation)
			{
				this.EndLine ();
				this.output.Append (this.IndentationString);
				this.outputState = State.EmptyLineWithIndentation;
			}
		}

		public void EndLine()
		{
			if (this.outputState != State.EmptyLine)
			{
				this.output.Append (Strings.LineSeparator);
				this.outputState = State.EmptyLine;
			}
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
