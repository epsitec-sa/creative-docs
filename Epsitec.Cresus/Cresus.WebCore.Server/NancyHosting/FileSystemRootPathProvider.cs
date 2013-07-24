using Nancy;

using System;

using System.IO;

using System.Reflection;


namespace Epsitec.Cresus.WebCore.Server.NancyHosting
{


	/// <summary>
	/// This class defines the root path that is used by the Nancy server when dealing with files.
	/// </summary>
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
