//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>StructuredType</c> class describes the type of the data stored in
	/// a <see cref="T:StructuredData"/> class.
	/// </summary>
	public class StructuredType : NamedDependencyObject, INamedType, IStructuredType
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

		#region INamedType Members

		public string DefaultController
		{
			get
			{
				return null;
			}
		}

		public string DefaultControllerParameter
		{
			get
			{
				return null;
			}
		}

		#endregion
		
		#region ISystemType Members

		public System.Type SystemType
		{
			get
			{
				return null;
			}
		}

		#endregion
		
		private void NotifyFieldInserted(string name, INamedType type)
		{
		}

		private void NotifyFieldRemoved(string name, INamedType type)
		{
		}

		private Collections.HostedDictionary<string, INamedType> fields;
	}
}
