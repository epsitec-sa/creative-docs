//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Common.Support;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.UI.DataSourceMetadata))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>DataSourceMetadata</c> class describes the fields found in the data source
	/// attached to a <see cref="Panel"/>.
	/// </summary>
	public class DataSourceMetadata : DependencyObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataSourceMetadata"/> class.
		/// </summary>
		public DataSourceMetadata()
		{
		}

		/// <summary>
		/// Gets the structured type field collection; every field has a one-to-one
		/// mapping with the fields found in the panel's <see cref="DataSource"/>.
		/// </summary>
		/// <value>The structured type field collection.</value>
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
