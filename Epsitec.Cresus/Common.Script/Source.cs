//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script
{
	/// <summary>
	/// La classe Source représente le source complet d'un script, lequel est
	/// constitué d'une série de méthodes et d'une source de données.
	/// </summary>
	public class Source : Types.INotifyChanged
	{
		public Source()
		{
		}
		
		public Source(string name, Method[] methods, Types.IDataValue[] values, string about)
		{
			this.DefineName (name);
			this.DefineMethods (methods);
			this.DefineValues (values);
			this.DefineAbout (about);
		}
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public Method[]							Methods
		{
			get
			{
				return this.methods;
			}
		}
		
		public Types.IDataValue[]				Values
		{
			get
			{
				return this.values;
			}
		}
		
		public string							About
		{
			get
			{
				return this.about;
			}
		}
		
		
		public string GenerateAssemblySource()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("// Code generated dynamically based on '");
			buffer.Append (this.Name);
			buffer.Append ("'.\n\n");
			buffer.Append ("namespace ");
			buffer.Append (Text.DynamicNamespace);
			buffer.Append ("\n{\n");
			buffer.Append ("public sealed class ");
			buffer.Append (Text.DynamicClass);
			buffer.Append (" : ");
			buffer.Append (Text.DynamicClassBase);
			buffer.Append ("\n{\n");
			
			this.GenerateConstructor (buffer, Text.DynamicClass);
			this.GenerateDataAccessors (buffer);
			this.GenerateExecuteMethod (buffer);
			this.GenerateScriptMethods (buffer);
			
			buffer.Append ("}\n");
			buffer.Append ("}\n");
			
			return buffer.ToString ();
		}
		
		public void NotifyChanged()
		{
			this.OnChanged ();
		}
		
		public Method FindMethod(string signature)
		{
			foreach (Method method in this.Methods)
			{
				if (method.Signature == signature)
				{
					return method;
				}
			}
			
			return null;
		}
		
		
		public static bool Find(string[] lines, int line, ref int column, out string method_signature, out int section_id, out int line_id)
		{
			method_signature = null;
			section_id       = -1;
			line_id          = -1;
			
			int i = line;
			
			if ((line+4 < lines.Length) &&
				(lines[line+2].StartsWith ("//$MethodBegin=")) &&
				(lines[line+3].StartsWith ("//$SectionBegin=")))
			{
				//	Ajustons la ligne de l'erreur de manière à ce qu'elle pointe dans le
				//	corps de la méthode. C'est probablement un problème lié à un 'return'
				//	manquant, qui signale une erreur sur le début du bloc de la méthode.
				
				column = -1;
				line  +=  4;
				
				i = line;
			}
			
			while (i >= 0)
			{
				if ((lines[i].StartsWith ("//$SectionBegin=")) &&
					(section_id == -1))
				{
					string arg = lines[i].Substring (16);
					arg = arg.Substring (0, arg.IndexOf ("$"));
					
					Types.InvariantConverter.Convert (arg, out section_id);
					
					line_id = line - i - 1;
				}
				
				if (lines[i].StartsWith ("//$MethodBegin="))
				{
					method_signature = lines[i].Substring (15, lines[i].Length-16);
					break;
				}
				
				i--;
			}
			
			return line_id >= 0;
		}
		
		
		private void GenerateConstructor(System.Text.StringBuilder buffer, string name)
		{
			buffer.Append ("public ");
			buffer.Append (name);
			buffer.Append ("()\n{\n");
			
			//	Corps du constructeur ici...
			
			buffer.Append ("}\n");
		}
		
		private void GenerateDataAccessors(System.Text.StringBuilder buffer)
		{
			if (this.values == null)
			{
				return;
			}
			
			foreach (Types.IDataValue value in this.values)
			{
				buffer.Append ("private ");
				buffer.Append (value.DataType.SystemType.FullName);
				buffer.Append (" ");
				buffer.Append (value.Name);
				buffer.Append ("\n");
				buffer.Append ("{\n");
				
				buffer.Append ("get { return (");
				buffer.Append (value.DataType.SystemType.FullName);
				buffer.Append (") this.ReadData (\"");
				buffer.Append (value.Name);
				buffer.Append ("\", typeof (");
				buffer.Append (value.DataType.SystemType);
				buffer.Append (")); }\n");
				
				buffer.Append ("set { this.WriteData (\"");
				buffer.Append (value.Name);
				buffer.Append ("\", value); }\n");
				
				buffer.Append ("}\n");
			}
		}
		
		private void GenerateExecuteMethod(System.Text.StringBuilder buffer)
		{
			buffer.Append ("public override bool Execute(string name, object[] in_args, out object[] out_args)\n");
			buffer.Append ("{\n");
			buffer.Append ("out_args = null;\n");
			buffer.Append ("switch (name)\n{\n");
			
			foreach (Method method in this.methods)
			{
				buffer.Append ("case \"");
				buffer.Append (method.Name);
				buffer.Append ("\":\n");
				buffer.Append ("{\n");
				
				this.GenerateExecuteMethodCase (buffer, method);
				
				buffer.Append ("}\n");
				buffer.Append ("break;\n");
			}
			
			buffer.Append ("}\n");
			buffer.Append ("return false;\n");
			buffer.Append ("}\n");
		}
		
		private void GenerateExecuteMethodCase(System.Text.StringBuilder buffer, Method method)
		{
			int in_index  = 0;
			int out_index = 0;
			
			for (int i = 0; i < method.Parameters.Length; i++)
			{
				Source.ParameterInfo parameter = method.Parameters[i];
				
				buffer.Append (parameter.Type.SystemType.FullName);
				buffer.Append (" p_");
				buffer.Append (parameter.Name);
				buffer.Append (";\n");
				
				if (parameter.IsIn)
				{
					if (parameter.Type is Types.IEnumType)
					{
						buffer.Append ("if (! Epsitec.Common.Types.InvariantConverter.SafeConvert (");
						buffer.Append ("in_args[");
						buffer.Append (in_index.ToString (System.Globalization.CultureInfo.InvariantCulture));
						buffer.Append ("], typeof (");
						buffer.Append (parameter.Type.SystemType.FullName);
						buffer.Append ("), out p_");
						buffer.Append (parameter.Name);
						buffer.Append (")) return false;\n");
					}
					else
					{
						buffer.Append ("if (! Epsitec.Common.Types.InvariantConverter.SafeConvert (");
						buffer.Append ("in_args[");
						buffer.Append (in_index.ToString (System.Globalization.CultureInfo.InvariantCulture));
						buffer.Append ("], out p_");
						buffer.Append (parameter.Name);
						buffer.Append (")) return false;\n");
					}
					
					in_index++;
				}
				if (parameter.IsOut)
				{
					out_index++;
				}
			}
			
			if (out_index == 0)
			{
				buffer.Append ("out_args = null;\n");
			}
			else
			{
				buffer.Append ("out_args = new object[");
				buffer.Append (out_index.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append ("];\n");
			}
			
			buffer.Append ("this.");
			buffer.Append (method.Name);
			buffer.Append (" (");
			
			for (int i = 0; i < method.Parameters.Length; i++)
			{
				if (i > 0)
				{
					buffer.Append (", ");
				}
				
				Source.ParameterInfo parameter = method.Parameters[i];
				
				switch (parameter.Direction)
				{
					case ParameterDirection.In:
						buffer.Append ("p_");
						buffer.Append (parameter.Name);
						break;
					
					case ParameterDirection.Out:
						buffer.Append ("out p_");
						buffer.Append (parameter.Name);
						break;
					
					case ParameterDirection.InOut:
						buffer.Append ("ref p_");
						buffer.Append (parameter.Name);
						break;
				}
			}
			
			buffer.Append (");\n");
			
			out_index = 0;
			
			for (int i = 0; i < method.Parameters.Length; i++)
			{
				Source.ParameterInfo parameter = method.Parameters[i];
				
				if (parameter.IsOut)
				{
					buffer.Append ("out_args[");
					buffer.Append (out_index.ToString (System.Globalization.CultureInfo.InvariantCulture));
					buffer.Append ("] = p_");
					buffer.Append (parameter.Name);
					buffer.Append (";\n");
					
					out_index++;
				}
			}
			
			buffer.Append ("return true;\n");
		}

		private void GenerateScriptMethods(System.Text.StringBuilder buffer)
		{
			foreach (Method method in this.methods)
			{
				this.GenerateScriptMethod (buffer, method);
			}
		}
		
		private void GenerateScriptMethod(System.Text.StringBuilder buffer, Method method)
		{
			buffer.Append ("public ");
			
			if (method.ReturnType.SystemType == typeof (void))
			{
				buffer.Append ("void");
			}
			else
			{
				buffer.Append (method.ReturnType.SystemType.FullName);
			}
			
			buffer.Append (" ");
			buffer.Append (method.Name);
			buffer.Append ("(");
			
			for (int i = 0; i < method.Parameters.Length; i++)
			{
				if (i > 0)
				{
					buffer.Append (", ");
				}
				
				ParameterInfo parameter = method.Parameters[i];
				
				switch (parameter.Direction)
				{
					case ParameterDirection.In:
						break;
					case ParameterDirection.InOut:
						buffer.Append ("ref ");
						break;
					case ParameterDirection.Out:
						buffer.Append ("out ");
						break;
					default:
						throw new System.InvalidOperationException (string.Format ("Method {0} has invalid parameter {1}, direction is {2}.", method.Name, parameter.Name, parameter.Direction));
				}
				
				buffer.Append (parameter.Type.SystemType.FullName);
				buffer.Append (" ");
				buffer.Append (parameter.Name);
			}
			
			buffer.Append (")\n");
			buffer.Append ("{\n");
			
			this.GenerateScriptMethodBody (buffer, method);
			
			buffer.Append ("}\n");
		}
		
		private void GenerateScriptMethodBody(System.Text.StringBuilder buffer, Method method)
		{
			int section_id = 0;
			
			string method_signature = method.Signature;
			
			buffer.Append ("//$MethodBegin=");
			buffer.Append (method_signature);
			buffer.Append ("$\n");
			
			foreach (CodeSection section in method.CodeSections)
			{
				System.Diagnostics.Debug.Assert (section.CodeType == CodeType.Local);
				
				buffer.Append ("//$SectionBegin=");
				buffer.Append (section_id.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append ("$, CodeType is ");
				buffer.Append (section.CodeType);
				buffer.Append ("\n");
				
				switch (section.CodeType)
				{
					case CodeType.Local:
						buffer.Append (Support.Utilities.XmlBreakToText (section.Code).Replace ('\u00A0', ' '));
						buffer.Append ("\n\n");
						break;
					case CodeType.Comment:
						break;
					case CodeType.None:
						break;
					case CodeType.Server:
						throw new System.NotImplementedException ("Support for Server CodeType is not implemented yet.");
					default:
						throw new System.ArgumentException (string.Format ("Code Section type {0} not recognized.", section.CodeType));
				}
				
				buffer.Append ("//$SectionEnd=");
				buffer.Append (section_id.ToString (System.Globalization.CultureInfo.InvariantCulture));
				buffer.Append ("$\n");
				
				section_id++;
			}
			
			buffer.Append ("//$MethodEnd=");
			buffer.Append (method_signature);
			buffer.Append ("$\n");
		}
		
		
		internal void DefineName(string name)
		{
			this.name = name;
			this.OnChanged ();
		}
		
		internal void DefineMethods(Method[] methods)
		{
			this.methods = methods;
			this.OnChanged ();
		}
		
		internal void DefineValues(Types.IDataValue[] values)
		{
			this.values = values;
			this.OnChanged ();
		}
		
		internal void DefineAbout(string about)
		{
			this.about = about;
			this.OnChanged ();
		}
		
		
		protected virtual void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		
		#region INotifyChanged Members
		public event Support.EventHandler		Changed;
		#endregion
		
		#region ParameterDirection and CodeType Enumerations
		public enum ParameterDirection
		{
			None			= 0,
			
			In				= 1,
			Out				= 2,
			InOut			= 3,
			
			ReturnValue		= 4,
		}
		
		public enum CodeType
		{
			None,
			
			Local			= 1,
			Server			= 2,
			
			Comment			= 3,
		}
		#endregion
		
		#region ParameterInfo Class
		public class ParameterInfo
		{
			public ParameterInfo()
			{
			}
			
			public ParameterInfo(ParameterDirection direction, Types.INamedType type, string name)
			{
				this.DefineDirection (direction);
				this.DefineType (type);
				this.DefineName (name);
			}
			
			
			public ParameterDirection			Direction
			{
				get
				{
					return this.direction;
				}
			}
			
			public Types.INamedType				Type
			{
				get
				{
					return this.type;
				}
			}
			
			public string						Name
			{
				get
				{
					return this.name;
				}
			}
			
			
			public bool							IsIn
			{
				get
				{
					return (this.direction == ParameterDirection.In) || (this.direction == ParameterDirection.InOut);
				}
			}
			
			public bool							IsOut
			{
				get
				{
					return (this.direction == ParameterDirection.Out) || (this.direction == ParameterDirection.InOut);
				}
			}
			
			public bool							IsReturnValue
			{
				get
				{
					return (this.direction == ParameterDirection.ReturnValue);
				}
			}
			
			
			internal void DefineDirection(ParameterDirection direction)
			{
				this.direction = direction;
			}
			
			internal void DefineType(Types.INamedType type)
			{
				this.type = type;
			}
			
			internal void DefineName(string name)
			{
				this.name = name;
			}
			
			
			private ParameterDirection			direction;
			private Types.INamedType			type;
			private string						name;
		}
		#endregion
		
		#region CodeSection Class
		public class CodeSection
		{
			public CodeSection()
			{
			}
			
			public CodeSection(CodeType code_type, string code)
			{
				this.DefineCodeType (code_type);
				this.DefineCode (code);
			}
			
			
			public CodeType						CodeType
			{
				get
				{
					return this.code_type;
				}
			}
			
			public string						Code
			{
				get
				{
					return this.code;
				}
			}
			
			
			internal void DefineCodeType(CodeType code_type)
			{
				if (code_type == CodeType.None)
				{
					throw new System.ArgumentException ("Invalid code code_type.", "code_type");
				}
				
				this.code_type = code_type;
			}
			
			internal void DefineCode(string code)
			{
				this.code = code;
			}
			
			
			public static string RemoveWaveTag(string text)
			{
				for (;;)
				{
					int pos = text.IndexOf ("<w");
					
					if (pos == -1)
					{
						break;
					}
					
					int end = text.IndexOf (">", pos);
					
					text = string.Concat (text.Substring (0, pos), text.Substring (end+1));
				}
				
				return text.Replace ("</w>", "");
			}
			
			
			public void HiliteError(int line, int column, int tag_id)
			{
				string clean;
				
				clean = this.code.Replace ("<br/>", "\n");
				
				if (line == -1)
				{
					clean = CodeSection.RemoveWaveTag (clean);
				}
				
				string[] lines = clean.Split ('\n');
				
				if ((line >= 0) &&
					(line < lines.Length))
				{
					string text = lines[line];
					int    len  = Support.Utilities.SkipXmlChars (text, column);
					
					string before = text.Substring (0, len);
					string middle = text.Substring (len);
					string after  = "";
					
					if (column < 0)
					{
						after  = middle;
						middle = "";
					}
					else
					{
						for (int i = 0; i < middle.Length; i++)
						{
							if ((System.Char.IsLetterOrDigit (middle[i])) ||
								(middle[i] == '_'))
							{
								continue;
							}
							
							after  = middle.Substring (i);
							middle = middle.Substring (0, i);
						}
					}
					
					text = string.Concat (before, "<w id=\"", tag_id.ToString (System.Globalization.CultureInfo.InvariantCulture), "\">", middle, "</w>", after);
					
					lines[line] = text;
				}
				
				clean = string.Join ("<br/>", lines);
				
				this.code = clean;
			}
			
			private CodeType					code_type;
			private string						code;
		}
		#endregion
		
		#region Method Class
		public class Method
		{
			public Method()
			{
			}
			
			public Method(string name, Types.INamedType return_type, ParameterInfo[] parameters, CodeSection[] code_sections)
			{
				this.DefineName (name);
				this.DefineReturnType (return_type);
				this.DefineParameters (parameters);
				this.DefineCodeSections (code_sections);
			}
			
			
			public string						Name
			{
				get
				{
					return this.name;
				}
			}
			
			public Types.INamedType				ReturnType
			{
				get
				{
					return this.return_parameter == null ? Types.VoidType.Default : this.return_parameter.Type;
				}
			}
			
			public ParameterInfo				ReturnParameter
			{
				get
				{
					return this.return_parameter;
				}
			}
			
			public ParameterInfo[]				Parameters
			{
				get
				{
					return this.parameters;
				}
			}
			
			public CodeSection[]				CodeSections
			{
				get
				{
					return this.code_sections;
				}
			}
			
			public string						Signature
			{
				get
				{
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
					
					buffer.Append (this.Name);
					buffer.Append (":");
					buffer.Append (this.ReturnType.Name);
					buffer.Append ("(");
					
					int i = 0;
					
					foreach (ParameterInfo info in this.Parameters)
					{
						if (i++ > 0)
						{
							buffer.Append (",");
						}
						
						buffer.Append (info.Type.Name);
					}
					
					buffer.Append (")");
					
					return buffer.ToString ();
				}
			}
			
			
			internal void DefineName(string name)
			{
				this.name = name;
			}
			
			internal void DefineParameters(ParameterInfo[] parameters)
			{
				if (parameters == null)
				{
					parameters = new ParameterInfo[0];
				}
				
				this.parameters = parameters;
			}
			
			internal void DefineReturnType(Types.INamedType type)
			{
				if (type == null)
				{
					type = Types.VoidType.Default;
				}
				
				this.return_parameter = new ParameterInfo (ParameterDirection.ReturnValue, type, null);
			}
			
			internal void DefineCodeSections(CodeSection[] code_sections)
			{
				if (code_sections == null)
				{
					code_sections = new CodeSection[0];
				}
				
				this.code_sections = code_sections;
			}
			
			
			private string						name;
			private ParameterInfo				return_parameter;
			private ParameterInfo[]				parameters;
			private CodeSection[]				code_sections;
		}
		#endregion
		
		#region Text Class
		internal class Text
		{
			public const string					DynamicNamespace	= "Epsitec.Dynamic.Script";
			public const string					DynamicClass		= "DynamicScript";
			public const string					DynamicClassBase	= "Epsitec.Common.Script.Glue.AbstractScriptBase";
		}
		#endregion
		
		private string							name;
		private Method[]						methods;
		private Types.IDataValue[]				values;
		private string							about;
	}
}
