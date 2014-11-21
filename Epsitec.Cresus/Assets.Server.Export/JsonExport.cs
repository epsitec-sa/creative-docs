//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Export.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Exportation au format yaml.
	/// </summary>
	public class JsonExport<T> : AbstractExport<T>
		where T : struct
	{
		public override void Export(DataAccessor accessor, ExportInstructions instructions, AbstractExportProfile profile, AbstractTreeTableFiller<T> filler, ColumnsState columnsState)
		{
			base.Export (accessor, instructions, profile, filler, columnsState);

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

			builder.Append ("/**");
			builder.Append (this.Profile.FinalEndOfLine);
			builder.Append (" Export to JSON");
			builder.Append (this.Profile.FinalEndOfLine);
			builder.Append (" @version 0.1");
			builder.Append (this.Profile.FinalEndOfLine);
			builder.Append (" */");
			builder.Append (this.Profile.FinalEndOfLine);
			builder.Append (this.Profile.FinalEndOfLine);

			builder.Append ("[");

			for (int row=0; row<this.rowCount; row++)
			{
				if (row > 0)
				{
					builder.Append (", ");
				}

				builder.Append ("{");

				for (int column=0; column<this.columnCount; column++)
				{
					var description = columnDescriptions[column];

					var content = this.GetOutputString (this.array[column, row]);
					if (!string.IsNullOrEmpty (content))
					{
						if (column > 0)
						{
							builder.Append (", ");
						}

						builder.Append (this.GetTag (description.Header));
						builder.Append (": ");
						builder.Append (content);
					}
				}

				builder.Append ("}");
			}

			builder.Append ("]");

			return builder.ToString ();
		}


		private void WriteData(string data)
		{
			System.IO.File.WriteAllText (this.instructions.Filename, data, this.Profile.Encoding);
		}


		private string GetTag(string text)
		{
			if (this.Profile.CamelCase)
			{
				text = text.ToCamelCase ();
			}

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

		private JsonExportProfile Profile
		{
			get
			{
				return this.profile as JsonExportProfile;
			}
		}
	}
}