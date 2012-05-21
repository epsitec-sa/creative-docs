//	Copyright © 2007-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Designer;
using Epsitec.Common.Designer.ModuleSupport;
using Epsitec.Common.Identity;
using Epsitec.Common.Support;

using System.Collections.Generic;

namespace Epsitec.Designer
{
	static partial class Program
	{
		private static void Start(string[] args)
		{
			System.Diagnostics.Debug.Assert (System.Windows.Forms.Application.ExecutablePath.Contains ("vshost") == false,
				"Designer ne doit pas être lancé dans le processus de hosting de Visual Studio, car il ne trouvera pas les ressources.\n\n" +
				"Il faut enlever la coche '[x] Enable the Visual Studio hosting process' dans l'onglet 'Debug' des propriétés du projet 'App.CresusDesigner'.");
			
			Epsitec.Common.Widgets.Widget.Initialize ();
			Epsitec.Common.Document.Engine.Initialize ();

			var pool  = Program.CreateResourceManagerPool ();
			var paths = new List<string> ();
			
			paths.Add (System.IO.Path.Combine (Globals.Directories.ExecutableRoot, "Resources"));

			//	Juste pour forcer le chargement des ressources manifest:... correspondantes.

//-			var loadCresusAssets = typeof (Epsitec.Cresus.Assets.Res);
//-			var loadCresusCoreApplication = typeof (Epsitec.Cresus.Core.CoreApplication);
			var loadCresusGraphApplication = typeof (Epsitec.Cresus.Graph.GraphApplication);
			var loadCresusDocumentsApplication = typeof (Epsitec.App.CresusDocuments.Application);
			var loadCommonDocumentEditorApplication = typeof (Epsitec.Common.DocumentEditor.Application);
//-			var loadCommonSupportUnitTests = typeof (Epsitec.Common.Support.UnitTests.UnitTestStringPacker);
//-			var loadCresusDataLayerUnitTests = typeof (Epsitec.Cresus.DataLayer.UnitTests.Context.UnitTestDataContextEventArgs);
//-			var loadCresusWebServer = typeof (Epsitec.Cresus.WebServer.Component);
//-			var loadProductAider = typeof (Epsitec.Product.Aider.Res);

			var  addPaths       = new List<string> ();
			bool noDefaultPaths = false;
			
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].StartsWith (";"))
				{
					continue;
				}

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
							pool.AddModuleRootPath (args[i+1], args[i+2]);
						}
						i+=2;
						break;

					case "-dev-id":
						int devId;
						if (int.TryParse (args[++i], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out devId))
						{
							Epsitec.Common.Designer.Settings.Default.IdentityCard = IdentityRepository.Default.FindIdentityCard (devId);
						}
						break;

					case "-merge-module":
						if (ModuleMerger.Merge (pool, args[++i], args[++i]) == false)
						{
							System.Environment.Exit (1);
						}
						break;

					case "-quit":
						return;

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

			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
			Epsitec.Common.Support.Implementation.FileProvider.DefineGlobalProbingPath (string.Join (";", paths.ToArray ()));

			var designerMainWindow = new DesignerApplication (pool)
			{
				Mode = DesignerMode.Build,
				Standalone = true,
			};

			designerMainWindow.Show (null);

			Program.StartProtocolService (designerMainWindow);
			
			designerMainWindow.Window.Run ();

			Program.StopProtocolService ();
		}

		private static ResourceManagerPool CreateResourceManagerPool()
		{
			var pool = new ResourceManagerPool ("Common.Designer")
			{
				DefaultPrefix = "file"
			};

			pool.SetupDefaultRootPaths ();
			pool.ScanForAllModules ();
			
			return pool;
		}
		
		private static void StartProtocolService(DesignerApplication designerMainWindow)
		{
			Epsitec.Designer.Protocol.NavigatorService.DefineNavigateToStringAction (
				delegate (string arg)
				{
					Druid id;
					if (Druid.TryParse (arg, out id))
					{
						Epsitec.Common.Widgets.Application.QueueAsyncCallback (
							delegate ()
							{
								designerMainWindow.NavigateToString (id);
							});
					}
				});

			Epsitec.Designer.Protocol.NavigatorService.DefineNavigateToCaptionAction (
				delegate (string arg)
				{
					Druid id;
					if (Druid.TryParse (arg, out id))
					{
						Epsitec.Common.Widgets.Application.QueueAsyncCallback (
							delegate ()
							{
								designerMainWindow.NavigateToCaption (id);
							});
					}
				});

			Epsitec.Designer.Protocol.NavigatorService.DefineNavigateToEntityFieldAction (
				delegate (string arg)
				{
					string[] args = arg.Split (':');

					Druid entityId;
					Druid fieldId;

					if ((Druid.TryParse (args[0], out entityId)) &&
						(Druid.TryParse (args[1], out fieldId)))
					{
						//	Exemples de champs navigables designer:fld/700H1/70022 ou designer:fld/7013/700J2.

						Epsitec.Common.Widgets.Application.QueueAsyncCallback (
							delegate ()
							{
								designerMainWindow.NavigateToEntityField (entityId, fieldId);
							});
					}
				});

			Program.protocolServer = new Epsitec.Designer.Protocol.Server (false);
			Program.protocolServer.Open ();
		}

		private static void StopProtocolService()
		{
			if (Program.protocolServer != null)
			{
				Program.protocolServer.Dispose ();
				Program.protocolServer = null;
			}
		}

		static Epsitec.Designer.Protocol.Server protocolServer;
	}
}
