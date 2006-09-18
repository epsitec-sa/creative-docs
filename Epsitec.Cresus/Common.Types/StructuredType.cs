//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.StructuredType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>StructuredType</c> class describes the type of the data stored in
	/// a <see cref="T:StructuredData"/> class.
	/// </summary>
	public class StructuredType : AbstractType, IStructuredType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:StructuredType"/> class.
		/// </summary>
		public StructuredType() : base ("Structure")
		{
			this.fields = new Collections.HostedDictionary<string, INamedType> (this.NotifyFieldInserted, this.NotifyFieldRemoved);
		}

		/// <summary>
		/// Adds a field definition the the structured type.
		/// </summary>
		/// <param name="name">The field name.</param>
		/// <param name="type">The field type.</param>
		public void AddField(string name, INamedType type)
		{
			if (string.IsNullOrEmpty (name))
			{
				throw new System.ArgumentException ("Invalid field name");
			}

			if (this.fields.ContainsKey (name))
			{
				throw new System.ArgumentException ("Duplicate definition for field '{0}'", name);
			}

			this.fields[name] = type;
		}

		/// <summary>
		/// Gets the field definition dictionary. This instance is writable.
		/// </summary>
		/// <value>The fields.</value>
		public Collections.HostedDictionary<string, INamedType> Fields
		{
			get
			{
				return this.fields;
			}
		}

		#region IStructuredType Members

		public object GetFieldTypeObject(string name)
		{
			INamedType type;

			if (this.fields.TryGetValue (name, out type))
			{
				return type;
			}
			else
			{
				return null;
			}
		}

		public string[] GetFieldNames()
		{
			string[] names = new string[this.fields.Count];
			this.fields.Keys.CopyTo (names, 0);

			System.Array.Sort (names);
			
			return names;
		}

		#endregion
		
		#region ISystemType Members

		public override System.Type SystemType
		{
			get
			{
				return null;
			}
		}

		#endregion

		public override bool IsValidValue(object value)
		{
			StructuredData data = value as StructuredData;

			return (data != null) && (data.StructuredType == this);
		}
		
		private void NotifyFieldInserted(string name, INamedType type)
		{
		}

		private void NotifyFieldRemoved(string name, INamedType type)
		{
		}

		private Collections.HostedDictionary<string, INamedType> fields;

		private static object GetFieldsValue(DependencyObject obj)
		{
			StructuredType that = obj as StructuredType;
			return new StructuredTypeFieldCollection (that);
		}

		public static DependencyProperty FieldsProperty = DependencyProperty.RegisterReadOnly ("Fields", typeof (StructuredTypeFieldCollection), typeof (StructuredType), new DependencyPropertyMetadata (StructuredType.GetFieldsValue).MakeReadOnlySerializable ());
	}
}
