using Epsitec.Common.Designer;
using Epsitec.Common.Identity;
using Epsitec.Common.Support;

using System.Collections.Generic;

namespace App.Designer
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main(string[] args)
		{
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Document.Engine.Initialize ();

			string execPath = Epsitec.Common.Support.Globals.Directories.ExecutableRoot;
			List<string> paths;

			Epsitec.Common.Support.ResourceManagerPool pool = new Epsitec.Common.Support.ResourceManagerPool("Common.Designer");
			pool.DefaultPrefix = "file";
			pool.SetupDefaultRootPaths();
			pool.ScanForAllModules();

#if false
			if (Epsitec.Common.Support.Globals.IsDebugBuild)
			{
				paths = new List<string> (new string[]
				{
					@"S:\Epsitec.Cresus\Demo\Resources",
					@"S:\Epsitec.Cresus\Common.Dialogs\Resources",
					@"S:\Epsitec.Cresus\Common.Designer\Resources",
					@"S:\Epsitec.Cresus\Common.Document\Resources",
					@"S:\Epsitec.Cresus\Common.Support\Resources",
					@"S:\Epsitec.Cresus\Common.Types\Resources",
					@"S:\Epsitec.Cresus\Common.Widgets\Resources",
					@"S:\Epsitec.Cresus\Common.DocumentEditor\Resources"
				});
			}
			else
#endif
			{
				paths = new List<string> ();
				paths.Add (System.IO.Path.Combine (execPath, "Resources"));
			}

			List<string> addPaths = new List<string> ();
			bool noDefaultPaths = false;
			int devId;

			for (int i = 0; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "-command-file":
						if (System.IO.File.Exists (args[++i]))
						{
							args = System.IO.File.ReadAllLines (args[i]);
							i = -1;
						}
						break;

					case "-no-default-paths":
						noDefaultPaths = true;
						break;

					case "-add-path":
						if (System.IO.Directory.Exists (args[++i]))
						{
							addPaths.Add (args[i]);
						}
						break;

					case "-define-symbolic-root":
						if (System.IO.Directory.Exists (args[i+2]))
						{
							pool.AddModuleRootPath(args[i+1], args[i+2]);
						}
						i+=2;
						break;

					case "-dev-id":
						if (int.TryParse (args[++i], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out devId))
						{
							Settings.Default.IdentityCard = IdentityRepository.Default.FindIdentityCard (devId);
						}
						break;

					case "-merge-module":
						if (Program.MergeModule (pool, args[++i], args[++i]) == false)
						{
							System.Environment.Exit (1);
						}
						break;

					default:
						throw new System.NotSupportedException (string.Format ("Option {0} not supported", args[i]));
				}
			}

			if (addPaths.Count > 0)
			{
				if (noDefaultPaths)
				{
					paths.Clear ();
				}

				paths.AddRange (addPaths);
			}
			
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
			Epsitec.Common.Support.Implementation.FileProvider.DefineGlobalProbingPath (string.Join (";", paths.ToArray ()));
			
			DesignerApplication designerMainWindow;
			
			designerMainWindow = new DesignerApplication (pool);
			designerMainWindow.Mode = DesignerMode.Build;
			designerMainWindow.Standalone = true;
			designerMainWindow.Show (null);
			designerMainWindow.Window.Run ();
		}

		private static bool MergeModule(ResourceManagerPool pool, string modulePath, string outputPath)
		{
			ResourceModuleInfo info = pool.GetModuleInfo (modulePath);

			if (info == null)
			{
				System.Diagnostics.Debug.WriteLine ("Failed to locate module " + modulePath);
				return false;
			}
			if (string.IsNullOrEmpty (info.ReferenceModulePath))
			{
				System.Diagnostics.Debug.WriteLine ("Not a patch module : "+ modulePath);
				return false;
			}

			System.IO.Directory.CreateDirectory (outputPath);

			try
			{
				Module.Merge (info.FullId, outputPath);
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("Merge failed : " + ex.Message);
				return false;
			}

			return true;
		}
	}
}