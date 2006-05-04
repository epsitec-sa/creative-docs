//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>StructuredRecordType</c> class describes the type of the data stored in
	/// a <see cref="T:StructuredRecord"/> class.
	/// </summary>
	public class StructuredRecordType : INamedType, IStructuredTree
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
			return this.GetFieldType (path) != null;
		}
		
		public INamedType GetFieldType(string path)
		{
			string[] names = StructuredRecordType.SplitPath (path);

			if (names.Length == 0)
			{
				return null;
			}

			INamedType type = null;
			StructuredRecordType record = this;

			for (int i = 0; i < names.Length; i++)
			{
				if (record == null)
				{
					return null;
				}

				if (record.Fields.TryGetValue (names[i], out type))
				{
					record = type as StructuredRecordType;
				}
				else
				{
					return null;
				}
			}

			return type;
		}

		#region IStructuredTree Members

		public string[] GetFieldNames()
		{
			string[] names = new string[this.fields.Count];
			
			this.fields.Keys.CopyTo (names, 0);

			System.Array.Sort (names);
			
			return names;
		}

		public string[] GetFieldPaths(string path)
		{
			StructuredRecordType type = this.GetFieldType (path) as StructuredRecordType;

			return StructuredRecordType.GetFieldPaths (path, type);
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

		public static string[] SplitPath(string path)
		{
			if ((path == null) ||
				(path.Length == 0))
			{
				return StructuredRecordType.EmptyPath;
			}
			else
			{
				return path.Split ('.');
			}
		}

		public static string CreatePath(params string[] names)
		{
			if (names.Length == 0)
			{
				return "";
			}
			else
			{
				return string.Join (".", names);
			}
		}

		public static string GetSubPath(string path, int start)
		{
			if (start < 1)
			{
				return path;
			}
			
			int pos = -1;

			for (int i = 0; i < start; i++)
			{
				pos = path.IndexOf ('.', pos+1);

				if (pos < 0)
				{
					return "";
				}
			}

			return path.Substring (pos+1);
		}

		public static string GetRootName(string path)
		{
			if (path == null)
			{
				return path;
			}

			int pos = path.IndexOf ('.');

			if (pos < 0)
			{
				return path;
			}
			else
			{
				return path.Substring (0, pos);
			}
		}

		public static string[] GetFieldPaths(string path, IStructuredTree root)
		{
			if (root == null)
			{
				return null;
			}

			string[] names = root.GetFieldNames ();

			for (int i = 0; i < names.Length; i++)
			{
				names[i] = StructuredRecordType.CreatePath (path, names[i]);
			}

			return names;
		}

		private void NotifyFieldInserted(string name, INamedType type)
		{
		}

		private void NotifyFieldRemoved(string name, INamedType type)
		{
		}

		public static readonly string[] EmptyPath = new string[0];
		
		private string name;
		private string caption;
		private string description;
		private HostedDictionary<string, INamedType> fields;
	}
}
