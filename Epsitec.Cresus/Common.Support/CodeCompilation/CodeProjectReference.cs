//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
	public class CodeProjectReference : System.IEquatable<CodeProjectReference>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeProjectReference"/> class.
		/// </summary>
		public CodeProjectReference()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeProjectReference"/> class.
		/// </summary>
		/// <param name="assemblyName">Name of the assembly.</param>
		public CodeProjectReference(string assemblyName)
		{
			this.assemblyName = assemblyName;
		}


		/// <summary>
		/// Gets or sets the assembly name.
		/// </summary>
		/// <value>The name of the assembly.</value>
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

		/// <summary>
		/// Gets or sets the assembly version.
		/// </summary>
		/// <value>The assembly version.</value>
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

		/// <summary>
		/// Gets or sets the assembly public key token.
		/// </summary>
		/// <value>The assembly public key token.</value>
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

		/// <summary>
		/// Gets or sets the assembly hint path.
		/// </summary>
		/// <value>The assembly hint path.</value>
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


		/// <summary>
		/// Determines whether this instance references a framework assembly.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance references a framework assembly; otherwise, <c>false</c>.
		/// </returns>
		public bool IsFrameworkAssembly()
		{
			return BuildDriver.FindReferenceAssemblyPath (this.AssemblyName) == null ? false : true;
		}


		/// <summary>
		/// Sets the public key token for the assembly. This converts the raw
		/// byte array into a string representation.
		/// </summary>
		/// <param name="token">The binary key token.</param>
		public void SetAssemblyPublicKeyToken(byte[] token)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (byte b in token)
			{
				buffer.AppendFormat ("{0:x2}", b);
			}

			this.AssemblyPublicKeyToken = buffer.ToString ();
		}

		/// <summary>
		/// Returns a short <c>&lt;Reference Include="..."&gt;</c> string, that
		/// represents the detailed reference.
		/// </summary>
		/// <returns>
		/// A string that represents this instance.
		/// </returns>
		public string ToSimpleString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			buffer.Append (@"<Reference Include=""");
			buffer.Append (this.assemblyName);
			buffer.Append (@""" />");

			return buffer.ToString ();
		}

		/// <summary>
		/// Returns a full <c>&lt;Reference Include="..."&gt;</c> string, that
		/// represents the detailed reference, with all the detailed versioning
		/// information.
		/// </summary>
		/// <returns>
		/// A string that represents this instance.
		/// </returns>
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

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			CodeProjectReference other = obj as CodeProjectReference;
			return this.Equals (other);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		#region IEquatable<CodeProjectReference> Members

		public bool Equals(CodeProjectReference other)
		{
			if (other == null)
			{
				return false;
			}
			else
			{
				return this.ToString () == other.ToString ();
			}
		}

		#endregion


		/// <summary>
		/// Creates a <see cref="CodeProjectReference"/> based on an assembly.
		/// The special <c>mscrolib</c> assembly will produce the more user friendly
		/// <c>"System"</c> name.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		/// <returns>The corresponding <see cref="CodeProjectReference"/>.</returns>
		public static CodeProjectReference FromAssembly(System.Reflection.Assembly assembly)
		{
			string assemblyPath = assembly.Location;
			CodeProjectReference reference = CodeProjectReference.FromAssemblyName (assembly.GetName ());

			if (!BuildDriver.IsFrameworkPath (assemblyPath))
			{
				reference.AssemblyHintPath = assemblyPath;
			}

			return reference;
		}

		/// <summary>
		/// Creates a <see cref="CodeProjectReference"/> based on an assembly name.
		/// The special <c>mscrolib</c> assembly will produce the more user friendly
		/// <c>"System"</c> name.
		/// </summary>
		/// <param name="assemblyName">Name of the assembly.</param>
		/// <returns>
		/// The corresponding <see cref="CodeProjectReference"/>.
		/// </returns>
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
