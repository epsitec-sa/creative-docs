//	Copyright © 2007-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeGeneration
{
	/// <summary>
	/// The <c>CodeFormatter</c> class is used to generate formatted source
	/// code. The caller provides the structure and the pieces of code and
	/// the formatter manages the indentation, blocks, etc. and checks for
	/// common mistakes which would lead to broken code.
	/// </summary>
	public class CodeFormatter : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeFormatter"/> class.
		/// </summary>
		public CodeFormatter()
			: this (new System.Text.StringBuilder ())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeFormatter"/> class.
		/// </summary>
		/// <param name="buffer">The buffer used to store the formatted source.</param>
		public CodeFormatter(System.Text.StringBuilder buffer)
		{
			this.output = buffer;
			this.indentationChars = "\t";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeFormatter"/> class.
		/// </summary>
		/// <param name="stream">The output stream.</param>
		public CodeFormatter(System.IO.TextWriter stream)
			: this ()
		{
			this.stream = stream;
		}


		/// <summary>
		/// Gets (or sets) the indentation level. Zero means no indentation at
		/// all.
		/// </summary>
		/// <value>The indentation level.</value>
		public int								IndentationLevel
		{
			get
			{
				return this.indentationLevel;
			}
			private set
			{
				if (this.indentationLevel != value)
				{
					this.indentationLevel  = value;
					
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

					for (int i = 0; i < this.indentationLevel; i++)
					{
						buffer.Append (this.indentationChars);
					}

					this.indentationString = buffer.ToString ();
				}
			}
		}

		/// <summary>
		/// Gets the indentation string. This is the text that gets inserted
		/// at the beginning of an empty line at the current indentation level.
		/// </summary>
		/// <value>The indentation string.</value>
		public string							IndentationString
		{
			get
			{
				return this.indentationString;
			}
		}

		/// <summary>
		/// Gets or sets the indentation character(s). The default is just one
		/// tab character <c>"\t"</c>.
		/// </summary>
		/// <value>The indentation character(s).</value>
		public string							IndentationChars
		{
			get
			{
				return this.indentationChars;
			}
			set
			{
				if (this.indentationChars != value)
				{
					if (this.indentationLevel > 0)
					{
						throw new System.InvalidOperationException ("Cannot change IndentationChars while outputting code");
					}

					this.indentationChars = value;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the formatter is outputting code
		/// within in a class definition.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if within a class definition; otherwise, <c>false</c>.
		/// </value>
		public bool								IsInClass
		{
			get
			{
				return (this.isInClassCount > 0) && (!this.isInInterface);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the formatter is output code within
		/// an interface definition.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if within an interface; otherwise, <c>false</c>.
		/// </value>
		public bool								IsInInterface
		{
			get
			{
				return this.isInInterface;
			}
		}



		/// <summary>
		/// Clears this instance and puts the formatter back into a clean state.
		/// </summary>
		public void Clear()
		{
			this.Flush ();
			
			this.indentationLevel = 0;
			this.indentationString = "";
			this.lineState = LineState.EmptyLine;
			this.elementStates.Clear ();
			this.isInInterface = false;
			this.isAbstract = false;
			this.isInClassCount = 0;
		}

		/// <summary>
		/// Flushes the contents of the internal buffer to the output stream,
		/// if the <see cref="CodeFormatter"/> was initialized to write to a
		/// stream; otherwise, this method does nothing.
		/// </summary>
		public void Flush()
		{
			if (this.stream != null)
			{
				this.stream.Write (this.output.ToString ());
				this.stream.Flush ();
				
				this.output.Length = 0;
			}
		}

		/// <summary>
		/// Saves the generated code to the specified text file. This does
		/// not call method <see cref="Clear"/>; the internal buffer remains
		/// valid.
		/// </summary>
		/// <param name="path">The path of the text file.</param>
		/// <param name="encoding">The text file encoding.</param>
		public void SaveCodeToTextFile(string path, System.Text.Encoding encoding)
		{
			string source = this.SaveCodeToString ();
			System.IO.File.WriteAllText (path, source, encoding);
		}

		/// <summary>
		/// Saves the generated code into a string. This does not call method
		/// <see cref="Clear"/>; the internal buffer remains valid.
		/// </summary>
		/// <returns>A <c>string</c> with the generated code.</returns>
		public string SaveCodeToString()
		{
			this.Flush ();

			if (this.stream == null)
			{
				return this.output.ToString ();
			}
			else
			{
				throw new System.InvalidOperationException ();
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
			this.WriteCode (Strings.Keywords.Class);
			this.WriteCode (code);
			this.WriteBeginBlock ();
		}

		public void WriteBeginClass(CodeAttributes attributes, string code, string specifiers)
		{
			if (specifiers != null)
			{
				specifiers = specifiers.Trim ();

				if (specifiers.Length > 0)
				{
					code = string.Concat (code, " : ", specifiers);
				}
			}

			this.WriteBeginClass (attributes, code);
		}

		public void WriteBeginClass(CodeAttributes attributes, string code, IEnumerable<string> specifiers)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (string item in specifiers)
			{
				string specifier = item == null ? "" : item.Trim ();
				
				if (specifier.Length > 0)
				{
					if (buffer.Length > 0)
					{
						buffer.Append (", ");
					}

					buffer.Append (specifier);
				}
			}

			this.WriteBeginClass (attributes, code, buffer.ToString ());
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
			this.WriteCode (Strings.Keywords.Interface);
			this.WriteCode (code);
			this.WriteBeginBlock ();
		}

		public void WriteBeginInterface(CodeAttributes attributes, string code, string specifiers)
		{
			if (specifiers != null)
			{
				specifiers = specifiers.Trim ();

				if (specifiers.Length > 0)
				{
					code = string.Concat (code, " : ", specifiers);
				}
			}

			this.WriteBeginInterface (attributes, code);
		}

		public void WriteBeginInterface(CodeAttributes attributes, string code, IEnumerable<string> specifiers)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (string item in specifiers)
			{
				string specifier = item == null ? "" : item.Trim ();

				if (specifier.Length > 0)
				{
					if (buffer.Length > 0)
					{
						buffer.Append (", ");
					}

					buffer.Append (specifier);
				}
			}

			this.WriteBeginInterface (attributes, code, buffer.ToString ());
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
			this.UpdateIsAbstract (attributes);

			if (this.IsInInterface)
			{
				this.WriteBeginLine ();
				this.WriteCode (code);
				this.WriteBeginBlock ();
			}
			else
			{
				this.WriteBeginLine ();
				this.WriteCode (attributes.ToString ());
				this.WriteCode (code);
				this.WriteBeginBlock ();
			}
		}

		public void WriteEndProperty()
		{
			this.PopElementState (ElementState.Property);
			this.WriteEndBlock ();
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

		public void WriteField(CodeAttributes attributes, params string[] fragments)
		{
			if (this.IsInInterface)
			{
				throw new System.InvalidOperationException ("Trying to generate field for an interface");
			}

			this.WriteBeginLine ();
			this.WriteCode (attributes.ToString ());
			this.WriteCode (fragments);
			this.WriteEndLine ();
		}

		public void WriteCodeLine(string line)
		{
			string trimmed = line.TrimStart ();

			if ((this.isAbstract) &&
				(trimmed.Length > 0))
			{
				if ((trimmed.StartsWith ("#")) ||
					(trimmed.StartsWith ("//")) ||
					(trimmed.StartsWith ("[") && trimmed.EndsWith ("]")))
				{
					//	OK, accept # directive or comment start, even if we are currently
					//	in an abstract construct where no code may be emitted.
					//	Also accept [attribute] insertion.
				}
				else
				{
					throw new System.InvalidOperationException ("Trying to generate code for an abstract item");
				}
			}
			
			this.WriteBeginLine ();
			this.WriteCode (line);
			this.WriteEndLine ();
		}

		public void WriteCodeLine(params string[] fragments)
		{
			this.WriteCodeLine (string.Concat (fragments));
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

		public void WriteCode(params string[] fragments)
		{
			this.WriteCode (string.Concat (fragments));
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

		public void WriteAssemblyAttribute(params string[] fragments)
		{
			string code = string.Concat (fragments) + Strings.LineSeparator;
			int    pos  = this.FindCodeStartPos ();

			this.output.Insert (pos, code);
		}

		private int FindCodeStartPos()
		{
			int pos = 0;
			
			bool inComment = false;
			int  inAttribute = 0;

			while (pos < this.output.Length)
			{
				switch (this.output[pos])
				{
					case ' ':
					case '\t':
						pos++;
						continue;

					case '\r':
					case '\n':
						inComment = false;
						pos++;
						continue;

					case '/':
						if ((pos < this.output.Length-1) &&
							(this.output[pos+1] == '/'))
						{
							inComment = true;
							pos += 2;
							continue;
						}
						break;

					case '[':
						inAttribute++;
						pos++;
						continue;

					case ']':
						inAttribute--;
						pos++;
						continue;
				}

				if ((inComment) ||
					(inAttribute > 0))
				{
					pos++;
					continue;
				}

				break;
			}
			
			return pos;
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Clear ();
			}
		}
		
		private void IncreaseIndentation()
		{
			this.IndentationLevel++;
		}

		private void DecreaseIndentation()
		{
			this.IndentationLevel--;
		}

		private void UpdateIsAbstract(CodeAttributes attributes)
		{
			if (this.IsInInterface)
			{
				this.isAbstract = true;
			}
			else if (this.IsInClass)
			{
				this.isAbstract = (attributes.Accessibility == CodeAccessibility.Abstract) || (attributes.IsPartialDefinition);
			}
			else
			{
				this.isAbstract = false;
			}
		}

		private void WriteBeginBlockOrSemicolumnIfAbstract(CodeAttributes attributes, string code)
		{
			this.UpdateIsAbstract (attributes);

			if ((this.elementStates.Count > 0) &&
				(this.elementStates.Peek () == ElementState.Method) &&
				(attributes.IsPartial))
			{
				//	This is a partial method definition or implementation. Check that
				//	the attributes are properly specified :

				if (attributes.Visibility == CodeVisibility.None)
				{
					//	OK
				}
				else
				{
					throw new System.InvalidOperationException (string.Format ("Partial method defined with {0} visibility", attributes.Visibility));
				}
				
				if ((attributes.Accessibility == CodeAccessibility.Final) ||
					(attributes.Accessibility == CodeAccessibility.Default) ||
					(attributes.Accessibility == CodeAccessibility.Static))
				{
					//	OK
				}
				else
				{
					throw new System.InvalidOperationException (string.Format ("Partial method defined with {0} accessibility", attributes.Accessibility));
				}
			}

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
					if ((previousState == ElementState.Class) ||
						(previousState == ElementState.Interface))
					{
						//	OK.
					}
					else
					{
						throw new System.InvalidOperationException (string.Format ("{0} not defined in a class or an interface", state));
					}
					break;

				case ElementState.PropertyGetter:
				case ElementState.PropertySetter:
					if (previousState != ElementState.Property)
					{
						throw new System.InvalidOperationException (string.Format ("{0} not defined in a property", state));
					}
					break;

				case ElementState.Namespace:
					if (previousState != ElementState.None)
					{
						throw new System.InvalidOperationException (string.Format ("Namespace cannot be defined in {0}", previousState));
					}
					break;

				case ElementState.Interface:
					if ((previousState == ElementState.Class) ||
						(previousState == ElementState.Namespace))
					{
						//	OK.
					}
					else
					{
						throw new System.InvalidOperationException (string.Format ("Trying to define an interface in {0}, not a class or a namespace", previousState));
					}
					this.isInInterface = true;
					break;

				case ElementState.Class:
					if ((previousState == ElementState.Class) ||
						(previousState == ElementState.Namespace))
					{
						//	OK.
					}
					else
					{
						throw new System.InvalidOperationException (string.Format ("Trying to define a class in {0}, not a class or a namespace", previousState));
					}
					this.isInClassCount++;
					break;

				case ElementState.InstanceVariable:
					if (previousState == ElementState.Class)
					{
						//	OK.
					}
					else
					{
						throw new System.InvalidOperationException (string.Format ("Trying to define an instance variable in {0}, not a class", previousState));
					}
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
		
		
		private enum LineState
		{
			EmptyLine,
			EmptyLineWithIndentation,
			PartialLine
		}

		private enum ElementState
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


		internal static class Strings
		{
			public const string LineSeparator = "\r\n";
			public const string BlockBegin = "{";
			public const string BlockEnd = "}";
			public const string Semicolumn = ";";

			public static class Keywords
			{
				public const string Abstract = "abstract";
				public const string Class = "class";
				public const string Const = "const";
				public const string Get = "get";
				public const string Interface = "interface";
				public const string Internal = "internal";
				public const string Namespace = "namespace";
				public const string New = "new";
				public const string Override = "override";
				public const string Partial = "partial";
				public const string Private = "private";
				public const string Protected = "protected";
				public const string Public = "public";
				public const string Readonly = "readonly";
				public const string Sealed = "sealed";
				public const string Set = "set";
				public const string Static = "static";
				public const string Virtual = "virtual";
			}
		}

		private System.Text.StringBuilder output;
		private System.IO.TextWriter stream;
		private int indentationLevel;
		private string indentationChars;
		private string indentationString;
		private LineState lineState;
		private Stack<ElementState> elementStates = new Stack<ElementState> ();
		private bool isInInterface;
		private int isInClassCount;
		private bool isAbstract;
	}
}
