//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeCompilation;
using Epsitec.Common.Support.CodeGeneration;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeCompilation
{
	/// <summary>
	/// The <c>CodeProject</c> class defines a compilation project (library file
	/// name, source files, references, etc.).
	/// </summary>
	public class CodeProject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeProject"/> class.
		/// </summary>
		public CodeProject()
		{
			this.templateItems = new Dictionary<TemplateItem, string> ();
		}

		public CodeProject(CodeProjectSettings settings)
			: this ()
		{
			this.SetProjectSettings (settings);
		}

		/// <summary>
		/// Gets the specified template item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public string GetItem(TemplateItem item)
		{
			string value;
			
			if (this.templateItems.TryGetValue (item, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Adds the specified template item to the project file. Some template
		/// items may only be added once; trying to add them more than once will
		/// throw a <see cref="System.NotSupportedException"/> exception.
		/// </summary>
		/// <param name="item">The template item.</param>
		/// <param name="value">The template value.</param>
		public void Add(TemplateItem item, string value)
		{
			if (! string.IsNullOrEmpty (value))
			{
				string oldValue;
				string newValue = value.Trim ();

				if (this.templateItems.TryGetValue (item, out oldValue))
				{
					switch (item)
					{
						case TemplateItem.CompileInsertionPoint:
						case TemplateItem.ReferenceInsertionPoint:
							this.templateItems[item] = string.Concat (oldValue, CodeProject.LineSeparator, newValue);
							break;

						default:
							throw new System.NotSupportedException (string.Format ("Cannot add duplicate item {0}", item));
					}
				}
				else
				{
					this.templateItems[item] = newValue;
				}
			}
		}

		/// <summary>
		/// Sets the project settings for this project. This will add all the
		/// template items as defined in the settings.
		/// </summary>
		/// <param name="settings">The project settings.</param>
		public void SetProjectSettings(CodeProjectSettings settings)
		{
			settings.DefineSettings (this);
		}

		/// <summary>
		/// Creates the project file based on an internal template file. The
		/// template items inserted with the <see cref="Add"/> method are
		/// replaced by their values.
		/// </summary>
		/// <returns>The source for the project file.</returns>
		public string CreateProjectSource()
		{
			string source = CodeProject.GetProjectFileTemplate ();

			foreach (TemplateItem item in Types.EnumType.GetAllEnumValues<TemplateItem> ())
			{
				string value;

				if (this.templateItems.TryGetValue (item, out value))
				{
				}
				else
				{
					value = "";
				}
				
				source = CodeProject.Replace (source, item, value);
			}
			
			return source;
		}


		/// <summary>
		/// Gets the project file template from the resources (this is really
		/// a .NET manifest resource stream).
		/// </summary>
		/// <returns></returns>
		private static string GetProjectFileTemplate()
		{
			System.Type                hostType = typeof (CodeProject);
			System.Reflection.Assembly assembly = hostType.Assembly;
			
			using (System.IO.Stream resourceStream = assembly.GetManifestResourceStream ("Epsitec.Common.Support.Resources.ProjectFileTemplate.xml"))
			{
				using (System.IO.StreamReader resourceReader = new System.IO.StreamReader (resourceStream, System.Text.Encoding.UTF8))
				{
					return resourceReader.ReadToEnd ();
				}
			}
		}

		/// <summary>
		/// Replaces the specified template item through its (multi-lined) value
		/// in the template source.
		/// </summary>
		/// <param name="source">The template source.</param>
		/// <param name="item">The template item.</param>
		/// <param name="value">The value.</param>
		/// <returns>The updated template source.</returns>
		private static string Replace(string source, TemplateItem item, string value)
		{
			string search = CodeProject.GetTemplateItemString (item);

			int pos = source.IndexOf (search);
			int len = search.Length;

			System.Diagnostics.Debug.Assert (pos > 0);
			System.Diagnostics.Debug.Assert (len > 0);

			int lineStartPos = source.LastIndexOfAny (new char[] { '\r', '\n' }, pos-1, pos) + 1;
			string lineStart = source.Substring (lineStartPos, pos - lineStartPos);

			if (lineStart.Trim ().Length > 0)
			{
				//	The template item is inserted within an existing line; just
				//	insert the value where the placeholder was.

				return string.Concat (source.Remove (pos), value, source.Substring (pos+len));
			}
			else if (value.Length == 0)
			{
				//	The template item is empty and is inserted in an empty line.
				//	The empty line will simply be removed from the source.

				string end = source.Substring (pos+len);

				if (end.StartsWith (CodeProject.LineSeparator))
				{
					end = end.Substring (CodeProject.LineSeparator.Length);
				}

				return string.Concat (source.Remove (lineStartPos), end);
			}
			else
			{
				//	The template item has to be inserted as a liste of properly
				//	indented lines. This needs a little bit of reformating.

				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

				buffer.Append (source.Remove (pos));
				
				bool   first  = true;
				string indent = CodeProject.LineSeparator + lineStart;

				foreach (string line in value.Split (new string[] { CodeProject.LineSeparator }, System.StringSplitOptions.RemoveEmptyEntries))
				{
					if (first)
					{
						first = false;
					}
					else
					{
						buffer.Append (indent);
					}

					buffer.Append (line);
				}

				buffer.Append (source.Substring (pos+len));

				return buffer.ToString ();
			}
		}

		/// <summary>
		/// Gets the string for a template item. This builds a <c>&lt;--XYZ-ABC--&gt;</c>
		/// string for an item named <c>XyzAbc</c>.
		/// </summary>
		/// <param name="item">The template item.</param>
		/// <returns>The string representation of the template item.</returns>
		private static string GetTemplateItemString(TemplateItem item)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();

			buffer.Append ("<!--");
			bool first = true;

			foreach (char c in item.ToString ())
			{
				if (first)
				{
					first = false;
				}
				else if (char.IsUpper (c))
				{
					buffer.Append ("-");
				}
				buffer.Append (char.ToUpperInvariant (c));
			}

			buffer.Append ("-->");

			return buffer.ToString ();
		}

		internal const string LineSeparator = "\r\n";

		private Dictionary<TemplateItem, string> templateItems;
	}
}
