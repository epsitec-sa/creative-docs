//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -

using System.Reflection;
using Nancy;

namespace Epsitec.Cresus.Core.Server.NancyComponents
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
