//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityFieldPath</c> class represents a path to a given field in
	/// an entity.
	/// </summary>
	public sealed class EntityFieldPath : System.IEquatable<EntityFieldPath>, System.IComparable<EntityFieldPath>
	{
		public EntityFieldPath()
		{
			this.entityId = Druid.Empty;
			this.path = "";
		}

		public EntityFieldPath(EntityFieldPath path)
		{
			this.entityId = path.entityId;
			this.path = path.path;
		}

		private EntityFieldPath(Druid entityId, string path)
		{
			this.entityId = entityId;
			this.path = path;
		}

		public bool IsEmpty
		{
			get
			{
				return (this.entityId.IsEmpty)
					&& (this.path.Length == 0);
			}
		}

		public bool IsRelative
		{
			get
			{
				return (this.path.Length > 0)
					&& (this.entityId.IsEmpty);
			}
		}

		public bool IsAbsolute
		{
			get
			{
				return this.entityId.IsValid;
			}
		}

		public bool ContainsIndex
		{
			get
			{
				return this.path.Contains ("(");
			}
		}

		public Druid EntityId
		{
			get
			{
				return this.entityId;
			}
		}

		public string[] Fields
		{
			get
			{
				if (this.path.Length == 0)
				{
					return new string[0];
				}
				else
				{
					return this.path.Split ('.');
				}
			}
		}


		public bool Navigate(AbstractEntity root, out AbstractEntity leaf, out string id)
		{
			string[] fields = this.Fields;
			int      last   = fields.Length - 1;

			leaf = null;
			id   = null;
			
			if (last < 0)
			{
				return false;
			}

			AbstractEntity node = root;

			for (int i = 0; i < last; i++)
			{
				string fieldId = EntityFieldPath.ParseFieldId (fields[i]);

				switch (root.InternalGetFieldRelation (fieldId))
				{
					case FieldRelation.Collection:
						node = EntityFieldPath.NavigateCollection (node, fieldId, EntityFieldPath.ParseCollectionIndex (fields[i]));
						break;

					case FieldRelation.Reference:
						node = root.GetField<AbstractEntity> (fieldId);
						break;

					default:
						node = null;
						break;
				}
				
				if (node == null)
				{
					return false;
				}
			}

			leaf = node;
			id   = fields[last];

			return true;
		}

		public bool Navigate(Druid root, out Druid leaf, out string id)
		{
			return this.Navigate (EntityContext.Current, root, out leaf, out id);
		}

		public bool Navigate(out Druid leaf, out string id)
		{
			return this.Navigate (EntityContext.Current, this.entityId, out leaf, out id);
		}

		public bool Navigate(EntityContext context, out Druid leaf, out string id)
		{
			return this.Navigate (EntityContext.Current, this.entityId, out leaf, out id);
		}

		public bool Navigate(EntityContext context, Druid root, out Druid leaf, out string id)
		{
			if (root.IsEmpty)
			{
				throw new System.ArgumentException ("Invalid root");
			}

			string[] fields = this.Fields;
			int      last   = fields.Length - 1;

			leaf = Druid.Empty;
			id   = null;

			if (last < 0)
			{
				return false;
			}

			Druid node = root;

			for (int i = 0; i < last; i++)
			{
				string fieldId = EntityFieldPath.ParseFieldId (fields[i]);

				IStructuredType     type  = context.GetStructuredType (node);
				StructuredTypeField field = type.GetField (fieldId);

				switch (field.Relation)
				{
					case FieldRelation.Collection:
					case FieldRelation.Reference:
						node = field.TypeId;
						break;

					default:
						node = Druid.Empty;
						break;
				}

				if (node.IsEmpty)
				{
					return false;
				}
			}

			leaf = node;
			id   = fields[last];

			return true;
		}


		public EntityFieldPath GetParentPath()
		{
			string[] fields = this.Fields;

			switch (fields.Length)
			{
				case 0:
					return null;
				case 1:
					return new EntityFieldPath (this.entityId, "");
				default:
					return new EntityFieldPath (this.entityId, string.Join (".", fields, 0, fields.Length-1));
			}
		}


		public static EntityFieldPath CreateAbsolutePath(Druid entityId, params string[] fields)
		{
			return new EntityFieldPath (entityId, string.Join (".", fields));
		}

		public static EntityFieldPath CreateAbsolutePath(Druid entityId, IEnumerable<string> fields)
		{
			return new EntityFieldPath (entityId, string.Join (".", Collection.ToArray (fields)));
		}

		public static EntityFieldPath CreateAbsolutePath(Druid entityId, EntityFieldPath relativePath)
		{
			if (relativePath == null)
			{
				throw new System.ArgumentNullException ();
			}
			if (relativePath.IsRelative == false)
			{
				throw new System.ArgumentException ("Relative path expected");
			}

			return EntityFieldPath.CreateAbsolutePath (entityId, relativePath.path);
		}

		public static EntityFieldPath CreateRelativePath(params string[] fields)
		{
			return new EntityFieldPath (Druid.Empty, string.Join (".", fields));
		}

		public static EntityFieldPath CreateRelativePath(IEnumerable<string> fields)
		{
			return new EntityFieldPath (Druid.Empty, string.Join (".", Collection.ToArray (fields)));
		}

		public static EntityFieldPath Parse(string path)
		{
			if (path == null)
			{
				return new EntityFieldPath ();
			}
			else
			{
				int pos = path.IndexOf (':');

				if (pos < 0)
				{
					return new EntityFieldPath (Druid.Empty, path);
				}
				else
				{
					return new EntityFieldPath (Druid.Parse (path.Substring (0, pos)), path.Substring (pos+1));
				}
			}
		}

		#region IEquatable<EntityFieldPath> Members

		public bool Equals(EntityFieldPath other)
		{
			if (object.ReferenceEquals (other, null))
			{
				return false;
			}
			else
			{
				return (this.entityId == other.entityId)
					&& (this.path == other.path);
			}
		}

		#endregion

		#region IComparable<EntityFieldPath> Members

		public int CompareTo(EntityFieldPath other)
		{
			if (object.ReferenceEquals (other, null))
			{
				return 1;
			}
			else
			{
				int result = string.CompareOrdinal (this.path, other.path);

				if (result == 0)
				{
					long a = this.entityId.ToLong ();
					long b = other.entityId.ToLong ();

					if (a < b)
					{
						return -1;
					}
					else if (a > b)
					{
						return 1;
					}
					else
					{
						return 0;
					}
				}
				else
				{
					return result;
				}
			}
		}

		#endregion

		public override bool Equals(object obj)
		{
			return this.Equals (obj as EntityFieldPath);
		}

		public override int GetHashCode()
		{
			return this.path.GetHashCode ();
		}

		public override string ToString()
		{
			return this.path;
		}

		public static bool operator==(EntityFieldPath a, EntityFieldPath b)
		{
			if (object.ReferenceEquals (a, b))
			{
				return true;
			}
			else if (object.ReferenceEquals (a, null))
			{
				return false;
			}
			else
			{
				return a.Equals (b);
			}
		}

		public static bool operator!=(EntityFieldPath a, EntityFieldPath b)
		{
			return (a == b) ? false : true;
		}


		private static int? ParseCollectionIndex(string id)
		{
			int pos = id.IndexOf ('(');

			if (pos < 0)
			{
				return null;
			}
			else
			{
				int    end = id.IndexOf (')', pos);
				string num = end < pos ? "" : id.Substring (pos+1, end-pos-1);

				if (num == "*")
				{
					return null;
				}
				else if (num.Length > 0)
				{
					int result;

					if (int.TryParse (num, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out result))
					{
						return result;
					}
				}

				throw new System.FormatException (string.Format ("Invalid index specified: '{0}'", id));
			}
		}

		private static string ParseFieldId(string id)
		{
			int pos = id.IndexOf ('(');

			if (pos < 0)
			{
				return id;
			}
			else
			{
				return id.Substring (0, pos);
			}
		}

		private static AbstractEntity NavigateCollection(AbstractEntity node, string id, int? index)
		{
			System.Collections.IList list = node.InternalGetFieldCollection (id);

			if (list == null)
			{
				return null;
			}

			int i;

			if (index.HasValue == false)
			{
				//	TODO: find index for this collection

				i = 0;
			}
			else
			{
				i = index.Value;
			}

			if ((i < 0) || (i >= list.Count))
			{
				return null;
			}
			else
			{
				return list[i] as AbstractEntity;
			}
		}

		

		private readonly Druid entityId;
		private readonly string path;
	}
}
