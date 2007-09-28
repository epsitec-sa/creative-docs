//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
	public class CodeProjectSource
	{
		public CodeProjectSource()
		{
		}

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


		private string fileName;
	}
}
