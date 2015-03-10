//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Data.Serialization;

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
			this.Field         = reader.ReadObjectFieldAttribute (X.Attr.Field);
			this.OriginalWidth = reader.ReadIntAttribute         (X.Attr.OriginalWidth).GetValueOrDefault ();
			this.Hide          = reader.ReadBoolAttribute        (X.Attr.Hide);

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

			writer.WriteObjectFieldAttribute (X.Attr.Field,         this.Field);
			writer.WriteIntAttribute         (X.Attr.OriginalWidth, this.OriginalWidth);
			writer.WriteBoolAttribute        (X.Attr.Hide,          this.Hide);

			writer.WriteEndElement ();
		}


		public static ColumnState Empty = new ColumnState (ObjectField.Unknown, 0, false);

		public readonly ObjectField				Field;
		public readonly int						OriginalWidth;
		public readonly bool					Hide;
	}
}
