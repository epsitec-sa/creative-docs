//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
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
			return StructuredTree.GetField (root, path).Type != null;
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

				if (name == path)
				{
					return value;
				}

				root = value as IStructuredData;
				path = StructuredTree.GetSubPath (path, 1);
			}

			throw new System.ArgumentException (string.Format ("Path {0} cannot be resolved", originalPath));
		}

		public static void SetValue(IStructuredData root, string path, object value)
		{
			if (string.IsNullOrEmpty (path))
			{
				throw new System.ArgumentException ();
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
				
				root = root.GetValue (name) as IStructuredData;
				path = StructuredTree.GetSubPath (path, 1);
			}

			throw new System.ArgumentException (string.Format ("Path {0} cannot be resolved", originalPath));
		}



		public static StructuredTypeField GetField(IStructuredType root, string path)
		{
			string leafName;
			string leafPath = StructuredTree.GetLeafPath (path, out leafName);

			if (string.IsNullOrEmpty (leafName))
			{
				return StructuredTypeField.Empty;
			}
			
			root = StructuredTree.GetSubTreeType (root, leafPath);
			
			if (root == null)
			{
				return StructuredTypeField.Empty;
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

				item = item.GetField (names[i]).Type as IStructuredType;
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
		
		public static readonly string[] EmptyPath = new string[0];
	}
}
