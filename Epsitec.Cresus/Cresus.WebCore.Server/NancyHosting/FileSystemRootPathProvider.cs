//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Nancy;

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

			return entryAssembly == null ? System.Environment.CurrentDirectory : Path.GetDirectoryName (entryAssembly.Location);
		}
	}
}
