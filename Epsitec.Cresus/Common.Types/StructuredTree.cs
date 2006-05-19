//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

		public static bool IsPathValid(IStructuredTree root, string path)
		{
			return StructuredTree.GetFieldTypeFromPath (root, path) != null;
		}

		public static object GetFieldTypeFromPath(IStructuredTree root, string path)
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
			
			return root.GetFieldTypeObject (leafName);
		}

		public static IStructuredTree GetSubTreeType(IStructuredTree root, string path)
		{
			string[] names = StructuredTree.SplitPath (path);

			if (names.Length == 0)
			{
				return root;
			}

			IStructuredTree item = root;

			for (int i = 0; i < names.Length; i++)
			{
				if (item == null)
				{
					return null;
				}

				item = item.GetFieldTypeObject (names[i]) as IStructuredTree;
			}

			return item;
		}

		public static string[] GetFieldPaths(IStructuredTree root, string path)
		{
			root = StructuredTree.GetSubTreeType (root, path);
			
			if (root == null)
			{
				return null;
			}

			string[] names = root.GetFieldNames ();

			for (int i = 0; i < names.Length; i++)
			{
				names[i] = StructuredTree.CreatePath (path, names[i]);
			}

			return names;
		}
		
		public static readonly string[] EmptyPath = new string[0];
	}
}
