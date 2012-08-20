//	Copyright © 2006-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>StructuredTree</c> class is used to walk through a tree built
	/// with <c>IStructuredData</c> and <c>IStructuredType</c> objects.
	/// </summary>
	[System.Obsolete ("The StructuredTree class should no longer be used. It will be removed.")]
	public static class StructuredTree
	{
		public static string[] SplitPath(string path)
		{
			if ((path == null) ||
				(path.Length == 0))
			{
				return StructuredTree.EmptyPath;
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

		public static string GetLeafPath(string path, out string leafName)
		{
			string leafPath;
			
			leafPath = null;
			leafName = null;

			if (!string.IsNullOrEmpty (path))
			{
				int pos = path.LastIndexOf ('.');

				if (pos < 0)
				{
					leafName = path;
				}
				else
				{
					leafPath = path.Substring (0, pos);
					leafName = path.Substring (pos+1);
				}
			}
			
			return leafPath;
		}

		public static bool IsPathValid(IStructuredType root, string path)
		{
			StructuredTypeField field = StructuredTree.GetField (root, path);
			return (field != null) && (field.Type != null);
		}

		public static object GetValue(IStructuredData root, string path)
		{
			if (string.IsNullOrEmpty (path))
			{
				throw new System.ArgumentException ();
			}

			string originalPath = path;

			while (root != null)
			{
				string name  = StructuredTree.GetRootName (path);
				object value = root.GetValue (name);

				if (UnknownValue.IsUnknownValue (value))
				{
					IStructuredTypeProvider provider = root as IStructuredTypeProvider;
					IStructuredType type = provider == null ? root as IStructuredType : provider.GetStructuredType ();

					if ((type == null) ||
						(type.GetField (name) == null))
					{
						return value;
					}
					else
					{
						return UndefinedValue.Value;
					}
				}
				else if (UndefinedValue.IsUndefinedValue (value))
				{
					return UndefinedValue.Value;
				}

				if (name == path)
				{
					return value;
				}

				root = value as IStructuredData;
				path = StructuredTree.GetSubPath (path, 1);
			}

			return UnknownValue.Value;
		}

		public static void SetValue(IStructuredData root, string path, object value)
		{
			if (string.IsNullOrEmpty (path))
			{
				throw new System.ArgumentException ("Empty path");
			}
			if (UnknownValue.IsUnknownValue (value))
			{
				throw new System.ArgumentException ("UnknownValue specified");
			}

			string originalPath = path;

			while (root != null)
			{
				string name = StructuredTree.GetRootName (path);

				if (name == path)
				{
					root.SetValue (name, value);
					return;
				}

				object item = root.GetValue (name);

				if (UnknownValue.IsUnknownValue (item))
				{
					break;
				}
				
				root = item as IStructuredData;
				path = StructuredTree.GetSubPath (path, 1);
			}

			throw new System.ArgumentException (string.Format ("Path {0} cannot be resolved", originalPath));
		}

		/// <summary>
		/// Gets a sample value for the specified path. If the structured data
		/// has some undefined nodes along the path, they will be replaced with
		/// empty <c>StructuredData</c> records whenever this is applicable.
		/// </summary>
		/// <param name="root">The root.</param>
		/// <param name="path">The path.</param>
		/// <returns>The sample value.</returns>
		public static object GetSampleValue(IStructuredData root, string path)
		{
			if (string.IsNullOrEmpty (path))
			{
				throw new System.ArgumentException ();
			}

			string originalPath = path;

			while (root != null)
			{
				string name  = StructuredTree.GetRootName (path);
				object value = root.GetValue (name);

				if ((UnknownValue.IsUnknownValue (value)) ||
					(UndefinedValue.IsUndefinedValue (value)))
				{
					IStructuredTypeProvider provider = root as IStructuredTypeProvider;
					IStructuredType type = provider == null ? root as IStructuredType : provider.GetStructuredType ();

					if (type == null)
					{
						return UndefinedValue.Value;
					}

					StructuredTypeField field = type.GetField (name);

					AbstractType    fieldType      = field.Type as AbstractType;
					IStructuredType fieldStructure = fieldType as IStructuredType;

					if (fieldType == null)
					{
						return UndefinedValue.Value;
					}

					value = fieldType.SampleValue;

					if ((value == null) &&
						(fieldStructure != null))
					{
						value = new StructuredData (fieldStructure);
					}
				}

				if (name == path)
				{
					return value;
				}

				root = value as IStructuredData;
				path = StructuredTree.GetSubPath (path, 1);
			}

			return UnknownValue.Value;
		}



		public static StructuredTypeField GetField(IStructuredType root, string path)
		{
			string leafName;
			string leafPath = StructuredTree.GetLeafPath (path, out leafName);

			if (string.IsNullOrEmpty (leafName))
			{
				return null;
			}
			
			root = StructuredTree.GetSubTreeType (root, leafPath);
			
			if (root == null)
			{
				return null;
			}
			
			return root.GetField (leafName);
		}

		public static IStructuredType GetSubTreeType(IStructuredType root, string path)
		{
			string[] names = StructuredTree.SplitPath (path);

			if (names.Length == 0)
			{
				return root;
			}

			IStructuredType item = root;

			for (int i = 0; i < names.Length; i++)
			{
				if (item == null)
				{
					return null;
				}

				StructuredTypeField field = item.GetField (names[i]);

				if (field == null)
				{
					return null;
				}
				
				item = field.Type as IStructuredType;
			}

			return item;
		}

		public static IEnumerable<string> GetFieldPaths(IStructuredType root, string path)
		{
			root = StructuredTree.GetSubTreeType (root, path);
			
			if (root == null)
			{
				yield break;
			}

			foreach (string id in root.GetFieldIds ())
			{
				yield return StructuredTree.CreatePath (path, id);
			}
		}

		public static readonly string[] EmptyPath = Epsitec.Common.Types.Collections.EmptyArray<string>.Instance;
	}
}
