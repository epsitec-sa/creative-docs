//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Support;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.UI.DataSourceMetadata))]

namespace Epsitec.Common.UI
{
	public class DataSourceMetadata : DependencyObject
	{
		public DataSourceMetadata()
		{
		}


		public IList<StructuredTypeField> Fields
		{
			get
			{
				if (this.fields == null)
				{
					this.fields = new List<StructuredTypeField> ();
				}
				
				return this.fields;
			}
		}
		
		
		private static object GetFieldsValue(DependencyObject obj)
		{
			DataSourceMetadata metadata = (DataSourceMetadata) obj;
			return metadata.Fields;
		}

		public static DependencyProperty FieldsProperty = DependencyProperty.RegisterReadOnly ("Fields", typeof (List<StructuredTypeField>), typeof (DataSourceMetadata), new DependencyPropertyMetadata (DataSourceMetadata.GetFieldsValue).MakeReadOnlySerializable ());

		private List<StructuredTypeField> fields;
	}
}
