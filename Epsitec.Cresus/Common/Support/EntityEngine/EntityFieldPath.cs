//	Copyright © 2007-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Linq;

using System.Collections.Generic;
using System.Reflection;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityFieldPath</c> class represents a path to a given field in
	/// an entity.
	/// </summary>
	public sealed class EntityFieldPath : System.IEquatable<EntityFieldPath>, System.IComparable<EntityFieldPath>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityFieldPath"/> class.
		/// </summary>
		public EntityFieldPath()
			: this (Druid.Empty, "")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityFieldPath"/> class.
		/// </summary>
		/// <param name="path">The path.</param>
		public EntityFieldPath(EntityFieldPath path)
			: this (path.EntityId, path.path)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityFieldPath"/> class.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <param name="path">The path.</param>
		private EntityFieldPath(Druid entityId, string path)
		{
			this.entityId = entityId;
			this.path = path ?? "";
		}


		/// <summary>
		/// Gets a value indicating whether the path is empty.
		/// </summary>
		/// <value><c>true</c> if the path is empty; otherwise, <c>false</c>.</value>
		public bool								IsEmpty
		{
			get
			{
				return (this.entityId.IsEmpty)
					&& (this.path.Length == 0);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the path is relative.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the path is relative; otherwise, <c>false</c>.
		/// </value>
		public bool								IsRelative
		{
			get
			{
				return (this.path.Length > 0)
					&& (this.entityId.IsEmpty);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the path is absolute.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the path is absolute; otherwise, <c>false</c>.
		/// </value>
		public bool								IsAbsolute
		{
			get
			{
				return this.entityId.IsValid;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the path contains an index.
		/// </summary>
		/// <value><c>true</c> if the path contains an index; otherwise, <c>false</c>.</value>
		public bool								ContainsIndex
		{
			get
			{
				return this.path.Contains ("(");
			}
		}

		/// <summary>
		/// Gets the field count. An empty path will return zero.
		/// </summary>
		/// <value>The count.</value>
		public int								Count
		{
			get
			{
				if (this.path.Length == 0)
				{
					return 0;
				}
				else
				{
					int count = 1;

					for (int i = 0; i < this.path.Length; i++)
					{
						if (this.path[i] == '.')
						{
							count++;
						}
					}

					return count;
				}
			}
		}

		/// <summary>
		/// Gets the entity id if the path is an absolute (rooted) path.
		/// </summary>
		/// <value>The entity id.</value>
		public Druid							EntityId
		{
			get
			{
				return this.entityId;
			}
		}

		/// <summary>
		/// Gets the fields relative to the root entity.
		/// </summary>
		/// <value>The fields or an empty array.</value>
		public string[]							FieldIds
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


		public IEnumerable<EntityField> ExplodeFields()
		{
			var entityId = this.EntityId;
			var fieldIds = this.FieldIds;

			foreach (var fieldId in fieldIds)
			{
				var field = new EntityField (entityId, fieldId);

				yield return field;

				entityId = EntityInfo.GetStructuredTypeField (entityId, fieldId).TypeId;
			}
		}


		/// <summary>
		/// Navigates from the specified root entity to the leaf entity.
		/// </summary>
		/// <param name="root">The root entity.</param>
		/// <param name="leaf">The leaf entity.</param>
		/// <param name="id">The id of the field in the leaf entity.</param>
		/// <returns><c>true</c> if the path could be resolved; otherwise, <c>false</c>.</returns>
		public bool Navigate(AbstractEntity root, out AbstractEntity leaf, out string id)
		{
			string[] fields = this.FieldIds;
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
				string rawId   = fields[i];
				string fieldId = EntityFieldPath.ParseFieldId (rawId);

				switch (node.InternalGetFieldRelation (fieldId))
				{
					case FieldRelation.Collection:
						node = EntityFieldPath.NavigateCollection (node, fieldId, EntityFieldPath.ParseCollectionIndex (rawId));
						break;

					case FieldRelation.Reference:
						node = node.GetField<AbstractEntity> (fieldId);
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

		/// <summary>
		/// Navigates from the specified root entity to the leaf entity.
		/// </summary>
		/// <param name="root">The root entity id.</param>
		/// <param name="leaf">The leaf entity id.</param>
		/// <param name="id">The id of the field in the leaf entity.</param>
		/// <returns><c>true</c> if the path could be resolved; otherwise, <c>false</c>.</returns>
		public bool NavigateSchema(Druid root, out Druid leaf, out string id)
		{
			return this.NavigateSchema (new EntityContext (), root, out leaf, out id);
		}

		/// <summary>
		/// Navigates from the local root entity to the leaf entity. This is
		/// only possible if the <see cref="IsAbsolute"/> property returns
		/// <c>true</c>.
		/// </summary>
		/// <param name="leaf">The leaf entity id.</param>
		/// <param name="id">The id of the field in the leaf entity.</param>
		/// <returns><c>true</c> if the path could be resolved; otherwise, <c>false</c>.</returns>
		public bool NavigateSchema(out Druid leaf, out string id)
		{
			return this.NavigateSchema (new EntityContext (), this.entityId, out leaf, out id);
		}

		/// <summary>
		/// Navigates from the local root entity to the leaf entity. This is
		/// only possible if the <see cref="IsAbsolute"/> property returns
		/// <c>true</c>.
		/// </summary>
		/// <param name="context">The specific entity context used to resolve entity ids.</param>
		/// <param name="leaf">The leaf entity id.</param>
		/// <param name="id">The id of the field in the leaf entity.</param>
		/// <returns>
		/// 	<c>true</c> if the path could be resolved; otherwise, <c>false</c>.
		/// </returns>
		public bool NavigateSchema(EntityContext context, out Druid leaf, out string id)
		{
			return this.NavigateSchema (context, this.entityId, out leaf, out id);
		}

		/// <summary>
		/// Navigates from the root entity to the leaf entity.
		/// </summary>
		/// <param name="context">The specific entity context used to resolve entity ids.</param>
		/// <param name="root">The root entity id.</param>
		/// <param name="leaf">The leaf entity id.</param>
		/// <param name="id">The id of the field in the leaf entity.</param>
		/// <returns>
		/// 	<c>true</c> if the path could be resolved; otherwise, <c>false</c>.
		/// </returns>
		public bool NavigateSchema(EntityContext context, Druid root, out Druid leaf, out string id)
		{
			if (root.IsEmpty)
			{
				throw new System.ArgumentException ("Invalid root");
			}

			string[] fields = this.FieldIds;
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

		/// <summary>
		/// Navigates from the root entity to the leaf entity, then reads the
		/// value using the dynamic getter and returns the value.
		/// </summary>
		/// <param name="root">The root entity.</param>
		/// <returns>The value or <c>UnknownValue.Value</c>.</returns>
		public object NavigateRead(AbstractEntity root)
		{
			object value = UnknownValue.Value;

			AbstractEntity leaf;
			string id;
			
			if (this.Navigate (root, out leaf, out id))
			{
				IValueStore store = leaf;
				value = store.GetValue (id);
			}

			return value;
		}

		/// <summary>
		/// Navigates from the root entity to the leaf entity, then gets the
		/// field description.
		/// </summary>
		/// <param name="root">The root entity.</param>
		/// <returns>The field or <c>null</c>.</returns>
		public StructuredTypeField NavigateReadSchema(AbstractEntity root)
		{
			AbstractEntity leaf;
			string id;

			if (this.Navigate (root, out leaf, out id))
			{
				EntityContext context = leaf.GetEntityContext ();
				return context.GetStructuredTypeField (leaf, id);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Navigates from the root entity to the leaf entity, then writes the
		/// value using the dynamic setter.
		/// </summary>
		/// <param name="root">The root entity.</param>
		/// <returns>The value or <c>UnknownValue.Value</c>.</returns>
		public bool NavigateWrite(AbstractEntity root, object value)
		{
			AbstractEntity leaf;
			string id;

			if (this.Navigate (root, out leaf, out id))
			{
				if ((leaf.InternalGetFieldSource (id) == FieldSource.Value) ||
					(leaf.CalculationsDisabled))
				{
					IValueStore store = leaf;
					store.SetValue (id, value, ValueStoreSetMode.Default);
					return true;
				}
			}

			return false;
		}

		public void CreateMissingNodes(AbstractEntity root)
		{
			EntityContext context = root.GetEntityContext ();
			
			//	If the root has disabled calculations, we will propagate this setting
			//	down to every child entity.

			bool disableCalculations = root.CalculationsDisabled;

			string[] fields = this.FieldIds;
			int      last   = fields.Length - 1;

			AbstractEntity node = root;
			AbstractEntity next = null; 

			for (int i = 0; i < last; i++)
			{
				string              fieldId   = EntityFieldPath.ParseFieldId (fields[i]);
				StructuredTypeField fieldType = context.GetStructuredTypeField (node, fieldId);

				switch (node.InternalGetFieldRelation (fieldId))
				{
					case FieldRelation.Collection:
						throw new System.NotImplementedException ();

					case FieldRelation.Reference:
						next = node.GetField<AbstractEntity> (fieldId);

						if (next == null)
						{
							next = context.CreateEmptyEntity (fieldType.TypeId);
							node.SetField<AbstractEntity> (fieldId, next);

							if (disableCalculations)
							{
								next.DisableCalculations ();
							}
						}

						node = next;
						break;

					default:
						throw new System.NotSupportedException ();
				}
				
				if (node == null)
				{
					throw new System.ArgumentException (string.Format ("Unresolved node type for node {0} in {1}", fieldId, this.path));
				}
			}
		}

		public string GetRoot()
		{
			if (this.path.Length == 0)
			{
				return null;
			}
			else
			{
				int pos = this.path.IndexOf ('.');
				
				if (pos < 0)
				{
					return this.path;
				}
				else
				{
					return this.path.Substring (0, pos);
				}
			}
		}

		/// <summary>
		/// Gets the parent path.
		/// </summary>
		/// <returns>The parent path or <c>null</c> if the path is empty.</returns>
		public EntityFieldPath GetParentPath()
		{
			string[] fields = this.FieldIds;

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

		/// <summary>
		/// Gets the root path. This is the path defined by the first field.
		/// </summary>
		/// <returns>The root path or <c>null</c> if the path is empty.</returns>
		public EntityFieldPath GetRootPath()
		{
			string[] fields = this.FieldIds;

			if (fields.Length == 0)
			{
				return this;
			}
			else
			{
				return new EntityFieldPath (this.entityId, fields[0]);
			}
		}

		/// <summary>
		/// Strips the start of this path by removing the specified path; the paths
		/// must match. A new path will be returned.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>The stripped (relative) path.</returns>
		/// <exception cref="System.ArgumentException">Throws <see cref="System.ArgumentException"/> if the path does not start with the specified path.</exception>
		public EntityFieldPath StripStart(EntityFieldPath path)
		{
			if ((path == null) ||
				(string.IsNullOrEmpty (path.path)))
			{
				return new EntityFieldPath (Druid.Empty, this.path);
			}

			if (this.StartsWith (path))
			{
				return new EntityFieldPath (Druid.Empty, this.path.Substring (System.Math.Min (path.path.Length + 1, this.path.Length)));
			}
			else
			{
				throw new System.ArgumentException ("This path does not start with given path", "path");
			}
		}

		/// <summary>
		/// Determines whether the beginning of this path matches the specified
		/// path using an ordinal comparison.
		/// </summary>
		/// <param name="path">The path to compare.</param>
		/// <returns><c>true</c> if the beginning of this path matches the
		/// specified path; otherwise, <c>false</c>.</returns>
		public bool StartsWith(EntityFieldPath path)
		{
			if ((path == null) ||
				(string.IsNullOrEmpty (path.path)))
			{
				return true;
			}

			if (System.Object.ReferenceEquals (path.path, this.path))
			{
				return true;
			}
			
			string search = path.path;
			int    length = search.Length;

			if (this.path.Length < length)
			{
				return false;
			}
			
			for (int i = 0; i < length; i++)
			{
				if (this.path[i] != search[i])
				{
					return false;
				}
			}

			if ((this.path.Length == length) ||
				(this.path[length] == '.'))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		
		/// <summary>
		/// Creates an absolute path.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <param name="fields">The fields.</param>
		/// <returns>The path instance.</returns>
		public static EntityFieldPath CreateAbsolutePath(Druid entityId, params string[] fields)
		{
			return new EntityFieldPath (entityId, string.Join (".", fields));
		}

		/// <summary>
		/// Creates an absolute path.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <param name="fields">The fields.</param>
		/// <returns>The path instance.</returns>
		public static EntityFieldPath CreateAbsolutePath(Druid entityId, IEnumerable<string> fields)
		{
			return new EntityFieldPath (entityId, string.Join (".", fields.ToArray ()));
		}

		/// <summary>
		/// Creates an absolute path.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <param name="relativePath">The relative path.</param>
		/// <returns>The path instance.</returns>
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

		public static EntityFieldPath CreateAbsolutePath(IEnumerable<EntityField> fieldCollection)
		{
			var fields = fieldCollection.ToArray ();

			if (fields.Length == 0)
			{
				return new EntityFieldPath ();
			}

			return EntityFieldPath.CreateAbsolutePath (fields[0].EntityId, fields.Select (x => x.FieldId));
		}

		public static EntityFieldPath CreateAbsolutePath(IEnumerable<PropertyInfo> properties)
		{
			return EntityFieldPath.CreateAbsolutePath (properties.Select (x => (EntityField) x));
		}
		

		/// <summary>
		/// Creates a relative path.
		/// </summary>
		/// <param name="fields">The fields.</param>
		/// <returns>The path instance.</returns>
		public static EntityFieldPath CreateRelativePath(params string[] fields)
		{
			return new EntityFieldPath (Druid.Empty, string.Join (".", fields));
		}

		/// <summary>
		/// Creates a relative path.
		/// </summary>
		/// <param name="fields">The fields.</param>
		/// <returns>The path instance.</returns>
		public static EntityFieldPath CreateRelativePath(IEnumerable<string> fields)
		{
			return new EntityFieldPath (Druid.Empty, string.Join (".", fields.ToArray ()));
		}

		/// <summary>
		/// Creates a relative path.
		/// </summary>
		/// <param name="path">The original path.</param>
		/// <param name="fields">The fields.</param>
		/// <returns>The path instance.</returns>
		public static EntityFieldPath CreateRelativePath(EntityFieldPath path, params string[] fields)
		{
			if (path.IsEmpty)
			{
				return EntityFieldPath.CreateRelativePath (fields);
			}
			else if (fields.Length == 0)
			{
				return new EntityFieldPath (Druid.Empty, path.path);
			}
			else
			{
				return new EntityFieldPath (Druid.Empty, string.Concat (path.path, ".", string.Join (".", fields)));
			}
		}

		
		/// <summary>
		/// Parses the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>The path instance.</returns>
		public static EntityFieldPath Parse(string path)
		{
			if (string.IsNullOrEmpty (path))
			{
				return new EntityFieldPath ();
			}
			else
			{
				int pos = path.IndexOf (':');

				if (pos < 1)
				{
					return new EntityFieldPath (Druid.Empty, path);
				}
				else
				{
					string entityId = path.Substring (0, pos);
					string fieldIds = path.Substring (pos+1);
					
					return new EntityFieldPath (Druid.Parse (entityId), fieldIds);
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
					long a = this.entityId.IsValid ? this.entityId.ToLong () : -1;
					long b = other.entityId.IsValid ? other.entityId.ToLong () : -1;

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
			return this.entityId.GetHashCode () ^ this.path.GetHashCode ();
		}

		public override string ToString()
		{
			if (this.entityId.IsEmpty)
			{
				return string.Concat (":", this.path);
			}
			else
			{
				return string.Concat (this.entityId.ToString (), ":", this.path);
			}
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

		

		private readonly Druid					entityId;
		private readonly string					path;
	}
}
