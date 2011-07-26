using Nancy;
using System.Reflection;

namespace Epsitec.Cresus.Core.Server.Nancy
{
	public class FileSystemRootPathProvider : IRootPathProvider
	{
		public string GetRootPath()
		{
			var assembly = Assembly.GetEntryAssembly ();

			return assembly == null ? System.Environment.CurrentDirectory : System.IO.Path.GetDirectoryName (Assembly.GetEntryAssembly ().Location);
		}
	}
}
