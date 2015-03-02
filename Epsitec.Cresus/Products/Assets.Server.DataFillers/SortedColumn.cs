//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Data.Serialization;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class SortedColumn
	{
		public SortedColumn(ObjectField field, SortedType type)
		{
			this.Field = field;
			this.Type  = type;
		}

		public SortedColumn(System.Xml.XmlReader reader)
		{
			this.Field = IOHelpers.ReadObjectFieldAttribute (reader, X.Attr.Field);
			this.Type  = (SortedType) IOHelpers.ReadTypeAttribute (reader, X.Attr.Type, typeof (SortedType));

			reader.Read ();
		}


		public bool								IsEmpty
		{
			get
			{
				return this.Field == ObjectField.Unknown
					&& this.Type  == SortedType.None;
			}
		}

		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			IOHelpers.WriteObjectFieldAttribute (writer, X.Attr.Field, this.Field);
			IOHelpers.WriteTypeAttribute        (writer, X.Attr.Type,  this.Type);

			writer.WriteEndElement ();
		}


		public static SortedColumn Empty = new SortedColumn (ObjectField.Unknown, SortedType.None);

		public readonly ObjectField				Field;
		public readonly SortedType				Type;
	};
}
