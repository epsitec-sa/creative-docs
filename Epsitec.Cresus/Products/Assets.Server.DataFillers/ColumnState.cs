//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	/// <summary>
	/// Etat d'une colonne, qui peut être visible ou cachée. Lorsqu'une colonne est cachée,
	/// on conserve sa largeur originale, utilisée lorsqu'elle est rendue visible à nouveau.
	/// </summary>
	public struct ColumnState
	{
		public ColumnState(ObjectField field, int originalWidth, bool hide)
		{
			this.Field         = field;
			this.OriginalWidth = originalWidth;
			this.Hide          = hide;
		}

		public ColumnState(System.Xml.XmlReader reader)
		{
			this.Field         = (ObjectField) IOHelpers.ReadTypeAttribute (reader, "Field", typeof (ObjectField));
			this.OriginalWidth = IOHelpers.ReadIntAttribute (reader, "OriginalWidth").GetValueOrDefault ();
			this.Hide          = IOHelpers.ReadBoolAttribute (reader, "Hide");

			reader.Read ();
		}


		public int								FinalWidth
		{
			get
			{
				return this.Hide ? 0 : this.OriginalWidth;
			}
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Field == ObjectField.Unknown;
			}
		}


		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			IOHelpers.WriteTypeAttribute (writer, "Field",         this.Field);
			IOHelpers.WriteIntAttribute  (writer, "OriginalWidth", this.OriginalWidth);
			IOHelpers.WriteBoolAttribute (writer, "Hide",          this.Hide);

			writer.WriteEndElement ();
		}


		public static ColumnState Empty = new ColumnState (ObjectField.Unknown, 0, false);

		public readonly ObjectField				Field;
		public readonly int						OriginalWidth;
		public readonly bool					Hide;
	}
}
