//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeCompilation;
using Epsitec.Common.Support.CodeGeneration;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeCompilation
{
	/// <summary>
	/// The <c>CodeProjectSource</c> class defines a source file as used by the
	/// <see cref="BuildDriver"/>.
	/// </summary>
	public class CodeProjectSource : System.IEquatable<CodeProjectSource>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeProjectSource"/> class.
		/// </summary>
		public CodeProjectSource()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeProjectSource"/> class.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		public CodeProjectSource(string fileName)
		{
			this.fileName = fileName;
		}


		/// <summary>
		/// Gets or sets the file name. This should be a project-relative path.
		/// </summary>
		/// <value>The file name.</value>
		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}


		/// <summary>
		/// Returns a <c>&lt;Compile Include="..." /&gt;</c> string describing
		/// the project source file.
		/// </summary>
		/// <returns>
		/// A string describing the project source file.
		/// </returns>
		public override string ToString()
		{
			if (this.fileName != null)
			{
				string name = this.fileName.Trim ();

				if (name.Length > 0)
				{
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

					buffer.Append (@"<Compile Include=""");
					buffer.Append (name);
					buffer.Append (@""" />");

					return buffer.ToString ();
				}
			}

			return "";
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
			CodeProjectSource other = obj as CodeProjectSource;
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

		#region IEquatable<CodeProjectSource> Members

		public bool Equals(CodeProjectSource other)
		{
			if (other == null)
			{
				return false;
			}
			else if (this.fileName == other.fileName)
			{
				return true;
			}

			string n1 = this.fileName ?? "";
			string n2 = other.fileName ?? "";

			return string.Equals (n1, n2, System.StringComparison.OrdinalIgnoreCase);
		}

		#endregion

		private string fileName;
	}
}
