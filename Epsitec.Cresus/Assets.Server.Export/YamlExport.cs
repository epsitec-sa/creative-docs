//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Export.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Exportation au format yaml.
	/// </summary>
	public class YamlExport<T> : AbstractExport<T>
		where T : struct
	{
		public override void Export(ExportInstructions instructions, AbstractExportProfile profile, AbstractTreeTableFiller<T> filler, ColumnsState columnsState)
		{
			base.Export (instructions, profile, filler, columnsState);

			this.FillArray (hasHeader: false);
			var data = this.GetData ();
			this.WriteData (data);
		}


		private string GetData()
		{
			//	Transforme le contenu du tableau en une string.
			var columnDescriptions = this.filler.Columns;
			System.Diagnostics.Debug.Assert (this.columnCount == columnDescriptions.Count ());

			var builder = new System.Text.StringBuilder ();

			builder.Append ("%YAML 1.1\r\n");
			builder.Append ("---\r\n");
			builder.Append ("# Assets export\r\n");

			for (int row=0; row<this.rowCount; row++)
			{
				builder.Append ("-\r\n");

				for (int column=0; column<this.columnCount; column++)
				{
					var description = columnDescriptions[column];

					var content = this.GetOutputString (array[column, row]);
					if (!string.IsNullOrEmpty (content))
					{
						builder.Append ("  ");
						builder.Append (this.GetTag (description.Header));
						builder.Append (": ");
						builder.Append (content);
						builder.Append ("\r\n");
					}
				}
			}

			builder.Append ("...\r\n");

			return builder.ToString ();
		}


		private void WriteData(string data)
		{
			System.IO.File.WriteAllText (this.instructions.Filename, data, System.Text.Encoding.UTF8);
		}


		private string GetTag(string text)
		{
			text = text.ToCamelCase ();

			if (string.IsNullOrEmpty (text))
			{
				return null;
			}
			else
			{
				text = text.Replace (" ", "_");
				text = text.Replace (":", "_");
				return text;
			}
		}


		private string GetOutputString(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}
			else
			{
				text = text.Replace ("\\", "\\\\");  // \ -> \\
				text = text.Replace ("\"", "\\\"");  // " -> \"
				return string.Concat ("\"", text, "\"");
			}
		}
	}
}