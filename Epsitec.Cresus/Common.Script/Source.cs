//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script
{
	/// <summary>
	/// La classe Source représente le source complet d'un script, lequel est
	/// constitué d'une série de méthodes.
	/// </summary>
	public class Source
	{
		public Source()
		{
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
			
			Client			= 1,
			Server			= 2,
		}
		#endregion
		
		#region ParameterInfo Class
		public class ParameterInfo
		{
			internal ParameterInfo()
			{
			}
			
			internal ParameterInfo(ParameterDirection direction, Types.INamedType type, string name)
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
			internal CodeSection()
			{
			}
			
			internal CodeSection(CodeLocation location, string code)
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
			internal Method()
			{
			}
			
			internal Method(string name, Types.INamedType return_type, ParameterInfo[] parameters, CodeSection[] code_sections)
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
					return this.return_parameter == null ? null : this.return_parameter.Type;
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
				this.parameters = parameters;
			}
			
			internal void DefineReturnType(Types.INamedType type)
			{
				this.return_parameter = new ParameterInfo (ParameterDirection.ReturnValue, type, null);
			}
			
			internal void DefineCodeSections(CodeSection[] code_sections)
			{
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
	}
}
