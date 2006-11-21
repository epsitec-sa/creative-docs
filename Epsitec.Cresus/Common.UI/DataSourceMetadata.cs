//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;

[assembly: DependencyClass (typeof (DataSourceMetadata))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>DataSourceMetadata</c> class describes the fields found in the data source
	/// attached to a <see cref="Panel"/>.
	/// </summary>
	public class DataSourceMetadata : DependencyObject, IStructuredType
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

		/// <summary>
		/// Gets or sets the default data type for the data source.
		/// </summary>
		/// <value>The default data type.</value>
		public IStructuredType DefaultDataType
		{
			get
			{
				return this.GetField ("*").Type as IStructuredType;
			}
			set
			{
				INamedType namedType = value as INamedType;

				if ((value != null) &&
					(namedType == null))
				{
					throw new System.ArgumentException ("Specified default data type does not implement INamedType");
				}

				if (this.fields != null)
				{
					for (int i = 0; i < this.fields.Count; i++)
					{
						if (this.fields[i].Id == "*")
						{
							this.fields[i] = new StructuredTypeField ("*", namedType);
							return;
						}
					}
				}

				this.Fields.Add (new StructuredTypeField ("*", namedType));
			}
		}

		#region IStructuredType Members

		/// <summary>
		/// Gets the field descriptor for the specified field identifier.
		/// </summary>
		/// <param name="fieldId">The field identifier.</param>
		/// <returns>
		/// The matching field descriptor; otherwise, <c>null</c>.
		/// </returns>
		public StructuredTypeField GetField(string fieldId)
		{
			if (this.fields != null)
			{
				foreach (StructuredTypeField field in this.fields)
				{
					if (field.Id == fieldId)
					{
						return field;
					}
				}
			}
			
			return StructuredTypeField.Empty;
		}

		/// <summary>
		/// Gets a collection of field identifiers.
		/// </summary>
		/// <returns>A collection of field identifiers.</returns>
		public IEnumerable<string> GetFieldIds()
		{
			if (this.fields != null)
			{
				foreach (StructuredTypeField field in this.fields)
				{
					yield return field.Id;
				}
			}
		}

		#endregion
		
		private static object GetFieldsValue(DependencyObject obj)
		{
			DataSourceMetadata metadata = (DataSourceMetadata) obj;
			return metadata.Fields;
		}

		public static DependencyProperty FieldsProperty = DependencyProperty.RegisterReadOnly ("Fields", typeof (List<StructuredTypeField>), typeof (DataSourceMetadata), new DependencyPropertyMetadata (DataSourceMetadata.GetFieldsValue).MakeReadOnlySerializable ());

		private List<StructuredTypeField> fields;
	}
}
