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
			this.path = "";
		}

		public EntityFieldPath(string path)
		{
			this.path = path ?? "";
		}

		public EntityFieldPath(EntityFieldPath path)
		{
			this.path = path.path;
		}

		public EntityFieldPath(params string[] fields)
		{
			this.path = string.Join (".", fields);
		}

		public EntityFieldPath(IEnumerable<string> fields)
		{
			this.path = string.Join (".", Collection.ToArray (fields));
		}


		public bool IsEmpty
		{
			get
			{
				return this.path.Length == 0;
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

		
		#region IEquatable<EntityFieldPath> Members

		public bool Equals(EntityFieldPath other)
		{
			if (object.ReferenceEquals (other, null))
			{
				return false;
			}
			else
			{
				return this.path == other.path;
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
				return string.CompareOrdinal (this.path, other.path);
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


		private readonly string path;
	}
}
