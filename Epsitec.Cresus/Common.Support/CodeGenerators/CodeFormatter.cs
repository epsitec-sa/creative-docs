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

		public bool IsInClass
		{
			get
			{
				return (this.isInClassCount > 0) && (!this.isInInterface);
			}
		}

		public bool IsInInterface
		{
			get
			{
				return this.isInInterface;
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
			if (this.lineState != LineState.EmptyLineWithIndentation)
			{
				this.WriteEndLine ();
				this.output.Append (this.IndentationString);
				this.lineState = LineState.EmptyLineWithIndentation;
			}
		}

		public void WriteEndLine()
		{
			if (this.lineState != LineState.EmptyLine)
			{
				this.output.Append (Strings.LineSeparator);
				this.lineState = LineState.EmptyLine;
			}
		}

		public void WriteBeginNamespace(string code)
		{
			this.PushElementState (ElementState.Namespace);
			this.WriteBeginLine ();
			this.WriteCode (Strings.Keywords.Namespace);
			this.WriteCode (code);
			this.WriteBeginBlock ();
		}

		public void WriteEndNamespace()
		{
			this.PopElementState (ElementState.Namespace);
			this.WriteEndBlock ();
		}
		
		public void WriteBeginClass(CodeAttributes attributes, string code)
		{
			this.PushElementState (ElementState.Class);
			this.WriteBeginLine ();
			this.WriteCode (attributes.ToString ());
			this.WriteCode (code);
			this.WriteBeginBlock ();
		}

		public void WriteEndClass()
		{
			this.PopElementState (ElementState.Class);
			this.WriteEndBlock ();
		}

		public void WriteBeginInterface(CodeAttributes attributes, string code)
		{
			this.PushElementState (ElementState.Interface);
			this.WriteBeginLine ();
			this.WriteCode (attributes.ToString ());
			this.WriteCode (code);
			this.WriteBeginBlock ();
		}

		public void WriteEndInterface()
		{
			this.PopElementState (ElementState.Interface);
			this.WriteEndBlock ();
		}

		public void WriteBeginMethod(CodeAttributes attributes, string code)
		{
			this.PushElementState (ElementState.Method);
			this.WriteBeginBlockOrSemicolumnIfAbstract (attributes, code);
		}

		public void WriteEndMethod()
		{
			this.PopElementState (ElementState.Method);
			this.WriteEndBlockOrNothingIfAbstract ();
		}

		public void WriteBeginProperty(CodeAttributes attributes, string code)
		{
			this.PushElementState (ElementState.Property);
			this.WriteBeginBlockOrSemicolumnIfAbstract (attributes, code);
		}

		public void WriteEndProperty()
		{
			this.PopElementState (ElementState.Property);
			this.WriteEndBlockOrNothingIfAbstract ();
		}

		public void WriteBeginSetter(CodeAttributes attributes)
		{
			this.PushElementState (ElementState.PropertySetter);
			this.WriteBeginLine ();

			if (attributes.Visibility != CodeVisibility.Public)
			{
				this.WriteCode (attributes.ToString ());
			}

			if (this.isAbstract)
			{
				this.WriteCode (Strings.Keywords.Set);
				this.WriteCode (Strings.Semicolumn);
			}
			else
			{
				this.WriteCode (Strings.Keywords.Set);
				this.WriteBeginBlock ();
			}
		}
		
		public void WriteEndSetter()
		{
			this.PopElementState (ElementState.PropertySetter);
			
			if (!this.isAbstract)
			{
				this.WriteEndBlock ();
			}
		}

		public void WriteBeginGetter(CodeAttributes attributes)
		{
			this.PushElementState (ElementState.PropertyGetter);
			this.WriteBeginLine ();
			
			if (attributes.Visibility != CodeVisibility.Public)
			{
				this.WriteCode (attributes.ToString ());
			}

			if (this.isAbstract)
			{
				this.WriteCode (Strings.Keywords.Get);
				this.WriteCode (Strings.Semicolumn);
			}
			else
			{
				this.WriteCode (Strings.Keywords.Get);
				this.WriteBeginBlock ();
			}
		}

		public void WriteEndGetter()
		{
			this.PopElementState (ElementState.PropertyGetter);

			if (!this.isAbstract)
			{
				this.WriteEndBlock ();
			}
		}

		public void WriteInstanceVariable(CodeAttributes attributes, string code)
		{
			this.PushElementState (ElementState.InstanceVariable);
			this.WriteBeginLine ();
			this.WriteCode (attributes.ToString ());
			this.WriteCode (code);
			this.WriteCode (Strings.Semicolumn);
			this.PopElementState (ElementState.InstanceVariable);
		}

		public void WriteLine(string code)
		{
			this.WriteBeginLine ();
			this.WriteCode (code);
			this.WriteEndLine ();
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
			this.lineState = LineState.PartialLine;
		}

		public void WriteCodeText(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return;
			}

			this.output.Append (text);
			this.lineState = LineState.PartialLine;
		}

		private void UpdateIsAbstract(CodeAttributes attributes)
		{
			if (this.IsInInterface)
			{
				this.isAbstract = true;
			}
			else if (this.IsInClass)
			{
				this.isAbstract = (attributes.Accessibility == CodeAccessibility.Abstract);
			}
			else
			{
				this.isAbstract = false;
			}
		}

		private void WriteBeginBlockOrSemicolumnIfAbstract(CodeAttributes attributes, string code)
		{
			this.UpdateIsAbstract (attributes);

			if (this.IsInInterface)
			{
				this.WriteBeginLine ();
				this.WriteCode (code);
				this.WriteCode (";");
			}
			else if (this.isAbstract)
			{
				this.WriteBeginLine ();
				this.WriteCode (attributes.ToString ());
				this.WriteCode (code);
				this.WriteCode (";");
			}
			else
			{
				this.WriteBeginLine ();
				this.WriteCode (attributes.ToString ());
				this.WriteCode (code);
				this.WriteBeginBlock ();
			}
		}

		private void WriteEndBlockOrNothingIfAbstract()
		{
			if (this.isAbstract)
			{
				this.isAbstract = false;
			}
			else
			{
				this.WriteEndBlock ();
			}
		}

		private void PushElementState(ElementState state)
		{
			ElementState previousState = this.elementStates.Count == 0 ? ElementState.None : this.elementStates.Peek ();

			switch (state)
			{
				case ElementState.Method:
				case ElementState.Property:
					System.Diagnostics.Debug.Assert ((previousState == ElementState.Class) || (previousState == ElementState.Interface));
					break;

				case ElementState.PropertyGetter:
				case ElementState.PropertySetter:
					System.Diagnostics.Debug.Assert (previousState == ElementState.Property);
					break;

				case ElementState.Interface:
					this.isInInterface = true;
					break;

				case ElementState.Class:
					this.isInClassCount++;
					break;
			}
			
			
			this.elementStates.Push (state);
		}

		private void PopElementState(ElementState state)
		{
			if (this.elementStates.Count == 0)
			{
				throw new System.InvalidOperationException (string.Format ("Ending element {0}, but none was started", state));
			}

			ElementState expectedState = this.elementStates.Pop ();
			
			if (state == expectedState)
			{
				if (state == ElementState.Interface)
				{
					this.isInInterface = false;
				}
				if (state == ElementState.Class)
				{
					this.isInClassCount--;
				}

				return;
			}

			throw new System.InvalidOperationException (string.Format ("Ending element {0}, but expected {1}", state, expectedState));
		}

		private ElementState PeekElementState(int distance)
		{
			if (distance == 0)
			{
				if (this.elementStates.Count > 0)
				{
					return this.elementStates.Peek ();
				}
				else
				{
					return ElementState.None;
				}
			}

			ElementState[] states = this.elementStates.ToArray ();
			
			int count = states.Length;
			int index = count - 1 - distance;

			if (index < 0)
			{
				return ElementState.None;
			}
			else
			{
				return states[index];
			}
		}
		
		
		enum LineState
		{
			EmptyLine,
			EmptyLineWithIndentation,
			PartialLine
		}

		enum ElementState
		{
			None,
			Namespace,
			Class,
			Interface,
			Property,
			PropertyGetter,
			PropertySetter,
			Method,
			InstanceVariable,
		}


		static class Strings
		{
			public const string LineSeparator = "\n";
			public const string BlockBegin = "{";
			public const string BlockEnd = "}";
			public const string Semicolumn = ";";

			public static class Keywords
			{
				public const string Namespace = "namespace";
				public const string Get = "get";
				public const string Set = "set";
			}
		}
		
		System.Text.StringBuilder output;
		int indentationLevel;
		string indentationString;
		LineState lineState;
		Stack<ElementState> elementStates = new Stack<ElementState> ();
		bool isInInterface;
		int isInClassCount;
		bool isAbstract;
	}
}
