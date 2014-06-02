﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Export.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Exportation au format xml.
	/// </summary>
	public class XmlExport<T> : AbstractExport<T>
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

			builder.Append (this.GetOpenTag (this.Profile.BodyTag));
			builder.Append (this.Profile.EndOfLine);

			for (int row=0; row<this.rowCount; row++)
			{
				if (!this.Profile.Compact)
				{
					builder.Append (this.Profile.Indent);
				}
				builder.Append (this.GetOpenTag (this.Profile.RecordTag));

				if (!this.Profile.Compact)
				{
					builder.Append (this.Profile.EndOfLine);
				}

				for (int column=0; column<this.columnCount; column++)
				{
					var description = columnDescriptions[column];

					var content = this.GetOutputString (array[column, row]);
					if (!string.IsNullOrEmpty (content))
					{
						if (!this.Profile.Compact)
						{
							builder.Append (this.Profile.Indent);
							builder.Append (this.Profile.Indent);
						}
						builder.Append (this.GetOpenTag (description.Header));
						builder.Append (content);
						builder.Append (this.GetCloseTag (description.Header));

						if (!this.Profile.Compact)
						{
							builder.Append (this.Profile.EndOfLine);
						}
					}
				}

				if (!this.Profile.Compact)
				{
					builder.Append ("\t");
				}
				builder.Append (this.GetCloseTag (this.Profile.RecordTag));
				builder.Append (this.Profile.EndOfLine);
			}

			builder.Append (this.GetCloseTag (this.Profile.BodyTag));
			builder.Append (this.Profile.EndOfLine);

			return builder.ToString ();
		}


		private void WriteData(string data)
		{
			System.IO.File.WriteAllText (this.instructions.Filename, data, System.Text.Encoding.UTF8);
		}


		private string GetOpenTag(string tag)
		{
			return string.Concat ("<", this.GetTag (tag), ">");
		}

		private string GetCloseTag(string tag)
		{
			return string.Concat ("</", this.GetTag (tag), ">");
		}

		private string GetTag(string tag)
		{
			if (this.Profile.CamelCase)
			{
				return tag.ToCamelCase ();
			}
			else
			{
				return tag;
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
				text = text.Replace ("<", "&lt;");
				text = text.Replace (">", "&gt;");
				return text;
			}
		}

		private XmlExportProfile Profile
		{
			get
			{
				return this.profile as XmlExportProfile;
			}
		}
	}
}