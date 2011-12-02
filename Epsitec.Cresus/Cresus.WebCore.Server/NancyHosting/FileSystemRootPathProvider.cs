using Nancy;

using System;

using System.IO;

using System.Reflection;


namespace Epsitec.Cresus.WebCore.Server.NancyHosting
{


	public class FileSystemRootPathProvider : IRootPathProvider
	{


		public string GetRootPath()
		{
			var entryAssembly = Assembly.GetEntryAssembly ();

			return entryAssembly == null
				? Environment.CurrentDirectory
				: Path.GetDirectoryName (entryAssembly.Location);
		}


	}


}
