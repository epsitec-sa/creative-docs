//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script
{
	/// <summary>
	/// La classe Source repr�sente le source complet d'un script, lequel est
	/// constitu� d'une s�rie de m�thodes.
	/// </summary>
	public class Source
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
				buffer.Append ("\", ");
				buffer.Append (value.DataType.SystemType);
				buffer.Append ("); }\n");
				buffer.Append ("}\n");
			}
		}
		
		private void GenerateExecuteMethod(System.Text.StringBuilder buffer)
		{
			buffer.Append ("public override bool Execute(string name, object[] in_args)\n");
			buffer.Append ("{\n");
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
			buffer.Append ("this.");
			buffer.Append (method.Name);
			buffer.Append (" ();\n");
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
					case ParameterDirection.InOut:
					case ParameterDirection.Out:
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
			int section_id = 1;
			
			foreach (CodeSection section in method.CodeSections)
			{
				System.Diagnostics.Debug.Assert (section.Location == CodeLocation.Local);
				
				buffer.Append ("// Section #");
				buffer.Append (section_id++);
				buffer.Append ("\n");
				buffer.Append (section.Code);
				buffer.Append ("\n\n");
			}
		}
		
		
		internal void DefineName(string name)
		{
			this.name = name;
		}
		
		internal void DefineMethods(Method[] methods)
		{
			this.methods = methods;
		}
		
		internal void DefineValues(Types.IDataValue[] values)
		{
			this.values = values;
		}
		
		internal void DefineAbout(string about)
		{
			this.about = about;
		}
		
		
		#region ParameterDirection and CodeLocation Enumerations
		public enum ParameterDirection
		{
			None			= 0,
			
			In				= 1,
			Out				= 2,
			InOut			= 3,
			
			ReturnValue		= 4,
		}
		
		public enum CodeLocation
		{
			None,
			
			Local			= 1,
			Server			= 2,
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
			
			public CodeSection(CodeLocation location, string code)
			{
				this.DefineLocation (location);
				this.DefineCode (code);
			}
			
			
			public CodeLocation					Location
			{
				get
				{
					return this.location;
				}
			}
			
			public string						Code
			{
				get
				{
					return this.code;
				}
			}
			
			
			internal void DefineLocation(CodeLocation location)
			{
				if (location == CodeLocation.None)
				{
					throw new System.ArgumentException ("Invalid code location.", "location");
				}
				
				this.location = location;
			}
			
			internal void DefineCode(string code)
			{
				this.code = code;
			}
			
			
			private CodeLocation				location;
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
		
		private string							name;
		private Method[]						methods;
		private Types.IDataValue[]				values;
		private string							about;
		
		internal class Text
		{
			public const string					DynamicNamespace	= "Epsitec.Dynamic.Script";
			public const string					DynamicClass		= "DynamicScript";
			public const string					DynamicClassBase	= "Epsitec.Common.Script.Glue.AbstractScriptBase";
		}
	}
}
