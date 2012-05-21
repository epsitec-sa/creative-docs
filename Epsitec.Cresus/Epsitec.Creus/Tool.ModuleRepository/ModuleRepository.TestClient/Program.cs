//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.ModuleRepository;

namespace TestClient
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				System.Console.WriteLine ("Press a key to connect");
				System.Console.ReadKey ();

				IModuleRepositoryService service = ModuleRepositoryClient.GetService ();

				int moduleId = service.GetNewModuleId ("ModuleRepositoryTest", "Pierre Arnaud");
				ModuleDirectory directory = service.CreateEmptyModule (moduleId, "A", "Epsitec.ModuleRepository");

				System.Console.WriteLine ("ModuleDirectory.Name = {0}", directory.Name);
				System.Console.WriteLine ("ModuleDirectory.Files.Length = {0}", directory.Files.Length);

				foreach (ModuleFile file in directory.Files)
				{
					System.Console.WriteLine ("  ModuleFile.Path = {0}", file.Path);

					string path = System.IO.Path.Combine (@"S:\Epsitec.Cresus\xxx", file.Path);

					System.IO.Directory.CreateDirectory (System.IO.Path.GetDirectoryName (path));
					System.IO.File.WriteAllBytes (path, file.Data);
				}

				System.Console.ReadKey ();

				System.Diagnostics.Debug.Assert (service.RecycleModuleId (moduleId, "Daniel Roux") == false);
				System.Diagnostics.Debug.Assert (service.RecycleModuleId (moduleId, "Pierre Arnaud") == true);

				ModuleRepositoryClient.CloseService (service);
			}
			catch (System.Exception ex)
			{
				System.Console.WriteLine ("Exception: {0}\n\n{1}", ex.Message, ex.StackTrace);
			}

			System.Console.ReadKey ();
		}
	}
}
