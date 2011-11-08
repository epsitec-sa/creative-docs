//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -


using Nancy;

using System;

using System.IO;

using System.Reflection;


namespace Epsitec.Cresus.Core.Server.NancyHosting
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
