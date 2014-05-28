//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Exportation au format txt ou csv.
	/// </summary>
	public class TextExport<T> : AbstractExport<T>
		where T : struct
	{
		public override void Export(ExportInstructions instructions, AbstractExportProfile profile, AbstractTreeTableFiller<T> filler, ColumnsState columnsState)
		{
			base.Export (instructions, profile, filler, columnsState);

			this.FillArray (this.Profile.HasHeader);
			var data = this.GetData ();
			this.WriteData (data);
		}


		private string GetData()
		{
			//	Transforme le contenu du tableau en une string.
			var builder = new System.Text.StringBuilder ();

			if (this.Profile.Inverted)
			{
				for (int column=0; column<this.columnCount; column++)
				{
					for (int row=0; row<this.rowCount; row++)
					{
						builder.Append (this.GetOutputString (array[column, row]));

						if (row < rowCount-1)
						{
							builder.Append (this.Profile.ColumnSeparator);
						}
					}

					builder.Append (this.Profile.EndOfLine);
				}
			}
			else
			{
				for (int row=0; row<this.rowCount; row++)
				{
					for (int column=0; column<this.columnCount; column++)
					{
						builder.Append (this.GetOutputString (array[column, row]));

						if (column < this.columnCount-1)
						{
							builder.Append (this.Profile.ColumnSeparator);
						}
					}

					builder.Append (this.Profile.EndOfLine);
				}
			}

			return builder.ToString ();
		}


		private void WriteData(string data)
		{
			System.IO.File.WriteAllText (this.instructions.Filename, data, System.Text.Encoding.UTF8);
		}


		private string GetOutputString(string text)
		{
			if (string.IsNullOrEmpty (this.Profile.ColumnBracket))
			{
				if (string.IsNullOrEmpty (text))
				{
					return null;
				}
				else
				{
					if (this.Profile.ColumnSeparator.Length == 1)
					{
						return TextExport<T>.GetEscaped (text, this.Profile.ColumnSeparator[0], this.Profile.Escape);
					}
					else
					{
						return text;
					}
				}
			}
			else
			{
				if (string.IsNullOrEmpty (text))
				{
					return null;
				}
				else
				{
					//	Remplace " par "".
					text = text.Replace (this.Profile.ColumnBracket, this.Profile.ColumnBracket+this.Profile.ColumnBracket);

					//	A partir de Toto, retourne "Toto".
					return string.Concat (this.Profile.ColumnBracket, text, this.Profile.ColumnBracket);
				}
			}
		}

		private static string GetEscaped(string text, char separator, string escape)
		{
			var builder = new System.Text.StringBuilder ();

			foreach (char c in text)
			{
				if (c == separator)
				{
					builder.Append (escape);
				}

				builder.Append (c);
			}

			return builder.ToString ();
		}

		private TextExportProfile Profile
		{
			get
			{
				return this.profile as TextExportProfile;
			}
		}
	}
}