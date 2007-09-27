//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeCompilation;
using Epsitec.Common.Support.CodeGeneration;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeCompilation
{
	public class CodeProject
	{
		public CodeProject()
		{
			this.templateItems = new Dictionary<TemplateItem, string> ();
		}

		public void Add(TemplateItem item, string value)
		{
			if (value != null)
			{
				string oldValue;
				string newValue = value.Trim ();

				if (this.templateItems.TryGetValue (item, out oldValue))
				{
					switch (item)
					{
						case TemplateItem.CompileInsertionPoint:
						case TemplateItem.ReferenceInsertionPoint:
							this.templateItems[item] = string.Concat (oldValue, "\r\n", newValue);
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

		public string CreateProjectFile()
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

		private static string GetProjectFileTemplate()
		{
			System.Type                hostType = typeof (CodeProject);
			System.Reflection.Assembly assembly = hostType.Assembly;
			
			using (System.IO.Stream resourceStream = assembly.GetManifestResourceStream ("Common.Support.Resources.ProjectFileTemplate.xml"))
			{
				using (System.IO.StreamReader resourceReader = new System.IO.StreamReader (resourceStream, System.Text.Encoding.UTF8))
				{
					return resourceReader.ReadToEnd ();
				}
			}
		}

		private static string Replace(string source, TemplateItem item, string value)
		{
			string search = CodeProject.GetTemplateItemString (item);

			int pos = source.IndexOf (search);
			int len = search.Length;

			System.Diagnostics.Debug.Assert (pos > 0);
			System.Diagnostics.Debug.Assert (len > 0);

			int lineStartPos = source.LastIndexOfAny (new char[] { '\r', '\n' }, 0, pos) + 1;
			string lineStart = source.Substring (lineStartPos, pos - lineStartPos);

			if (lineStart.Trim ().Length > 0)
			{
				return string.Concat (source.Remove (pos), value, source.Substring (pos+len));
			}
			else
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

				buffer.Append (source.Remove (pos));
				bool first = true;

				foreach (string line in value.Split (new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries))
				{
					if (first)
					{
						first = false;
					}
					else
					{
						buffer.Append ("\r\n");
						buffer.Append (lineStart);
					}

					buffer.Append (line);
				}

				buffer.Append (source.Substring (pos+len));

				return buffer.ToString ();
			}
		}

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

		Dictionary<TemplateItem, string> templateItems;
	}
}
