//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>StructuredRecordType</c> class describes the type of the data stored in
	/// a <see cref="T:StructuredRecord"/> class.
	/// </summary>
	public class StructuredRecordType : INamedType
	{
		public StructuredRecordType()
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

		public bool IsPathValid(string path)
		{
			string[] names = StructuredRecordType.SplitPath (path);

			if (names.Length == 0)
			{
				return false;
			}

			StructuredRecordType record = this;

			for (int i = 0; i < names.Length; i++)
			{
				if (record == null)
				{
					return false;
				}
				
				INamedType value;

				if (record.Fields.TryGetValue (names[i], out value))
				{
					record = value as StructuredRecordType;
				}
				else
				{
					return false;
				}
			}
			
			return true;
		}

		public static string[] SplitPath(string path)
		{
			if ((path == null) ||
				(path.Length == 0))
			{
				return StructuredRecordType.emptyPath;
			}
			else
			{
				return path.Split ('.');
			}
		}

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

		private static string[] emptyPath = new string[0];
		private string name;
		private string caption;
		private string description;
		private HostedDictionary<string, INamedType> fields;
	}
}
