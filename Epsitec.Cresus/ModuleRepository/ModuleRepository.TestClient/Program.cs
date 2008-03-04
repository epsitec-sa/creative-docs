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
			IModuleRepositoryService service = ModuleRepositoryClient.GetService ();

			int moduleId = service.GetNewModuleId ("ModuleRepositoryTest", "Pierre Arnaud");
			ModuleDirectory directory = service.CreateEmptyModule (moduleId, "A", "Epsitec.ModuleRepository");

			System.Console.WriteLine ("ModuleDirectory.Name = {0}", directory.Name);
			System.Console.WriteLine ("ModuleDirectory.Files.Length = {0}", directory.Files.Length);

			foreach (ModuleFile file in directory.Files)
			{
				System.Console.WriteLine ("  ModuleFile.Path = {0}", file.Path);
			}

			System.Console.ReadKey ();
		}
	}
}
