//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Exportation au format html.
	/// </summary>
	public class HtmlExport<T> : AbstractExport<T>
		where T : struct
	{
		public HtmlExport()
		{
		}


		public override void Export(AbstractTreeTableFiller<T> filler)
		{
			this.FillArray (filler);
			var data = this.GetData (filler);
			this.WriteData (data);
		}


		private string GetData(AbstractTreeTableFiller<T> filler)
		{
			var columnDescriptions = filler.Columns;

			var builder = new System.Text.StringBuilder ();

			for (int row=0; row<this.rowCount; row++)
			{
				builder.Append (string.Concat ("<record>", HtmlExport<T>.eol));

				for (int column=0; column<this.columnCount; column++)
				{
					var description = columnDescriptions[column];

					var content = this.GetOutputString (array[column, row]);
					if (!string.IsNullOrEmpty (content))
					{
						builder.Append (string.Concat ("\t<", description.Header, ">"));
						builder.Append (content);
						builder.Append (string.Concat ("</", description.Header, ">", HtmlExport<T>.eol));
					}
				}

				builder.Append (string.Concat ("</record>", HtmlExport<T>.eol));
			}

			return builder.ToString ();
		}


		private void WriteData(string data)
		{
			System.IO.File.WriteAllText (this.Instructions.Filename, data);
		}


		private string GetOutputString(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}
			else
			{
				text = text.Replace ("<", "&lt;");
				text = text.Replace (">", "&gt;");
				return text;
			}
		}


		private static string eol = "\r\n";
	}
}