//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>StructuredType</c> class describes the type of the data stored in
	/// a <see cref="T:StructuredRecord"/> class.
	/// </summary>
	public class StructuredType : INamedType, IStructuredType
	{
		public StructuredType()
		{
			this.fields = new HostedDictionary<string, INamedType> (this.NotifyFieldInserted, this.NotifyFieldRemoved);
			this.name = null;
			this.caption = null;
			this.description = null;
		}

		public HostedDictionary<string, INamedType> Fields
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

		public System.Type SystemType
		{
			get
			{
				return null;
			}
		}

		#endregion

		#region INameCaption Members

		public string Caption
		{
			get
			{
				return this.caption;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		#endregion

		#region IName Members

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		#endregion
		
		private void NotifyFieldInserted(string name, INamedType type)
		{
		}

		private void NotifyFieldRemoved(string name, INamedType type)
		{
		}

		private string name;
		private string caption;
		private string description;
		private HostedDictionary<string, INamedType> fields;
	}
}
