//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeCompilation;
using Epsitec.Common.Support.CodeGeneration;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeCompilation
{
	/// <summary>
	/// The <c>CodeProjectReference</c> class describes an assembly reference
	/// used by the <see cref="CodeProjectSettings"/> class.
	/// </summary>
	public class CodeProjectReference
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeProjectReference"/> class.
		/// </summary>
		public CodeProjectReference()
		{
		}

		
		public string AssemblyName
		{
			get
			{
				return this.assemblyName;
			}
			set
			{
				this.assemblyName = value;
			}
		}

		public string AssemblyVersion
		{
			get
			{
				return this.assemblyVersion;
			}
			set
			{
				this.assemblyVersion = value;
			}
		}

		public string AssemblyPublicKeyToken
		{
			get
			{
				return this.assemblyPublicKeyToken;
			}
			set
			{
				this.assemblyPublicKeyToken = value;
			}
		}

		public string AssemblyHintPath
		{
			get
			{
				return this.assemblyHintPath;
			}
			set
			{
				this.assemblyHintPath = value;
			}
		}


		public bool IsFrameworkAssembly()
		{
			return BuildDriver.FindReferenceAssemblyPath (this.AssemblyName) == null ? false : true;
		}


		public void SetAssemblyPublicKeyToken(byte[] token)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (byte b in token)
			{
				buffer.AppendFormat ("{0:x2}", b);
			}

			this.AssemblyPublicKeyToken = buffer.ToString ();
		}
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			buffer.Append (@"<Reference Include=""");
			buffer.Append (this.assemblyName);
			buffer.Append (", Version=");
			buffer.Append (this.assemblyVersion);
			buffer.Append (", Culture=");
			buffer.Append ("neutral");
			buffer.Append (", PublicKeyToken=");
			buffer.Append (this.assemblyPublicKeyToken);
			buffer.Append (", processorArchitecture=");
			buffer.Append ("MSIL");

			if (string.IsNullOrEmpty (this.assemblyHintPath))
			{
				buffer.Append (@""" />");
			}
			else
			{
				buffer.Append (@""">");
				buffer.Append (CodeProject.LineSeparator);
				buffer.Append ("  ");
				buffer.Append ("<HintPath>");
				buffer.Append (this.assemblyHintPath);
				buffer.Append ("</HintPath>");
				buffer.Append (CodeProject.LineSeparator);
				buffer.Append ("</Reference>");
			}

			return buffer.ToString ();
		}

		public string ToSimpleString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			buffer.Append (@"<Reference Include=""");
			buffer.Append (this.assemblyName);
			buffer.Append (@""" />");

			return buffer.ToString ();
		}

		public static CodeProjectReference FromAssembly(System.Reflection.Assembly assembly)
		{
			return CodeProjectReference.FromAssemblyName (assembly.GetName ());
		}

		public static CodeProjectReference FromAssemblyName(System.Reflection.AssemblyName assemblyName)
		{
			CodeProjectReference reference = new CodeProjectReference ();

			reference.AssemblyName = assemblyName.Name == "mscorlib" ? "System" : assemblyName.Name;
			reference.AssemblyVersion = assemblyName.Version.ToString (4);
			reference.SetAssemblyPublicKeyToken (assemblyName.GetPublicKeyToken ());
			
			//	reference.AssemblyHintPath

			return reference;
		}

		
		private string assemblyName;
		private string assemblyVersion;
		private string assemblyPublicKeyToken;
		private string assemblyHintPath;
	}
}
