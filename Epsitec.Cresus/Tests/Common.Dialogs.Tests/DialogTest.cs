using NUnit.Framework;

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.AddressBook.Entities;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	[TestFixture]
	public class DialogTest
	{
		public DialogTest()
		{
			this.resourceManager = new ResourceManager ();
		}
		
		[SetUp]
		public void SetUp()
		{
			Epsitec.Common.Document.Engine.Initialize ();
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
			Epsitec.Common.Drawing.ImageManager.InitializeDefaultCache ();
		}
		
		[Test]
		public void AutomatedTestEnvironment()
		{
			Window.RunningInAutomatedTestEnvironment = true;
		}


		[Test]
		public void Check01SimpleForm()
		{
			Dialog dialog = Dialog.Load (this.resourceManager, Druid.Parse ("_631"));	//	mask for AdresseEntity, from Demo5juin

			dialog.DialogWindowCreated +=
				delegate
				{
					Button buttonCancel = new Button ();
					Button buttonOk     = new Button ();
					
					buttonCancel.CommandObject = Res.Commands.Dialog.Generic.Cancel;
					buttonOk.CommandObject     = Res.Commands.Dialog.Generic.Ok;

					buttonCancel.Dock = DockStyle.Stacked;
					buttonOk.Dock     = DockStyle.Stacked;

					FrameBox frame = new FrameBox ();

					frame.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

					frame.Children.Add (buttonOk);
					frame.Children.Add (buttonCancel);
					frame.PreferredHeight = 30;
					frame.Dock = DockStyle.Bottom;

					dialog.DialogWindow.Root.Children.Add (frame);
				};

			Assert.IsNotNull (dialog);
			Assert.IsFalse (dialog.HasWindow);

			Window window = dialog.DialogWindow;

			Assert.IsNotNull (window);
			Assert.IsTrue (dialog.HasWindow);

			dialog.IsModal = false;
			dialog.OpenDialog ();
			Window.RunInTestEnvironment (window);
		}

		[Test]
		public void Check02SimpleFormWithHintList()
		{
			Dialog dialog = Dialog.Load (this.resourceManager, Druid.Parse ("_8V1"));	//	mask for AdresseEntity, from Cresus.AddressBook

			StringType.NativeDefault.DefineMaximumLength (40);

			//	Mask with :
			//	- Rue
			//	- CasePostale
			//	- Localité.Résumé

			HintListController     hintListController = new HintListController ();
			DialogSearchController searchController   = hintListController.SearchController;

			hintListController.Visibility = HintListVisibilityMode.AutoHide;
			hintListController.ContentType = HintListContentType.Suggestions;

			TestResolver resolver = DialogTest.CreateSuggestions ();
			
			searchController.Resolver = resolver;
			
			LocalitéEntity yverdon = null;

			foreach (LocalitéEntity loc in resolver.LocalitéSuggestions)
			{
				if (loc.Résumé == "CH 1400 Yverdon-les-Bains")
				{
					yverdon = loc;
					break;
				}
			}

			Assert.IsNotNull (yverdon, "Could not resolve YVERDON-LES-BAINS");

			dialog.DialogWindowCreated +=
				delegate
				{
					FrameBox frame = new FrameBox ();

					Button buttonOk     = new Button (frame);
					Button buttonCancel = new Button (frame);
					Button buttonClear  = new Button (frame);
					Button buttonDump   = new Button (frame);

					buttonOk.CommandObject     = Res.Commands.Dialog.Generic.Ok;
					buttonCancel.CommandObject = Res.Commands.Dialog.Generic.Cancel;
					
					buttonClear.Text = "Clear";
					buttonClear.Clicked +=
						delegate
						{
							searchController.ResetSuggestions ();
						};

					buttonDump.Text = "Dump";
					buttonDump.Clicked +=
						delegate
						{
							System.Console.Out.WriteLine (dialog.Data.Data.Dump ());
						};

					buttonOk.Dock     = DockStyle.Stacked;
					buttonCancel.Dock = DockStyle.Stacked;
					buttonClear.Dock  = DockStyle.Stacked;
					buttonDump.Dock   = DockStyle.Stacked;

					frame.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

					frame.PreferredHeight = 30;
					frame.Dock = DockStyle.Bottom;

					dialog.DialogWindow.Root.Children.Add (frame);
					dialog.DialogWindow.Root.Padding = new Drawing.Margins (8, 8, 4, 4);
				};

			Assert.IsNotNull (dialog);
			Assert.IsFalse (dialog.HasWindow);

			Window window = dialog.DialogWindow;

			Assert.IsNotNull (window);
			Assert.IsTrue (dialog.HasWindow);

			dialog.IsModal = false;
			dialog.Data = DialogTest.CreateDefaultDialogData (yverdon);
			dialog.SearchController = searchController;
			dialog.OpenDialog ();
			Window.RunInTestEnvironment (window);

			System.Console.Out.WriteLine ("Raw dialog data:");
			System.Console.Out.WriteLine (dialog.Data.Data.Dump ());

			switch (dialog.Result)
			{
				case DialogResult.Accept:
					dialog.Data.ApplyChanges ();
					break;

				case DialogResult.Cancel:
					dialog.Data.RevertChanges ();
					break;
			}

			System.Console.Out.WriteLine ("Resulting dialog data:");
			System.Console.Out.WriteLine (dialog.Data.ExternalData.Dump ());

			hintListController.Dispose ();
		}


		internal static AdresseEntity CreateDefaultAdresseEntity()
		{
			AdresseEntity adresse = EntityContext.Current.CreateEntity<AdresseEntity> ();

			adresse.Rue = "Ch. du Fontenay 6";
			adresse.Localité.Nom = "Yverdon-les-Bains";
			adresse.Localité.Numéro = "1400";
			adresse.Localité.Pays.Code = "CH";
			adresse.Localité.Pays.Nom = "Suisse";

			return adresse;
		}

		internal static AdresseEntity CreateDefaultAdresseEntity(LocalitéEntity loc)
		{
			AdresseEntity adresse = EntityContext.Current.CreateEntity<AdresseEntity> ();

			adresse.Rue = "Ch. du Fontenay 6";
			adresse.Localité = loc;

			return adresse;
		}

		internal static DialogData CreateDefaultDialogData(LocalitéEntity loc)
		{
			return new DialogData (DialogTest.CreateDefaultAdresseEntity (loc), DialogDataMode.Isolated);
		}

		private static TestResolver CreateSuggestions()
		{
			TestResolver resolver = new TestResolver ();

			PaysEntity countryCh = new PaysEntity ()
			{
				Code = "CH",
				Nom = "Suisse"
			};

			PaysEntity countryF = new PaysEntity ()
			{
				Code = "F",
				Nom = "France"
			};

			PaysEntity countryDe = new PaysEntity ()
			{
				Code = "DE",
				Nom = "Deutschland"
			};

			resolver.PaysSuggestions.Add (countryCh);
			resolver.PaysSuggestions.Add (countryF);
			resolver.PaysSuggestions.Add (countryDe);

#if false
			resolver.LocalitéSuggestions.Add (new LocalitéEntity ()
			{
				Nom = "Yverdon-les-Bains",
				Numéro = "1400",
				Pays = countryCh
			});

			resolver.LocalitéSuggestions.Add (new LocalitéEntity ()
			{
				Nom = "Suscévaz",
				Numéro = "1437",
				Pays = countryCh
			});

			resolver.LocalitéSuggestions.Add (new LocalitéEntity ()
			{
				Nom = "Treycovagnes",
				Numéro = "1436",
				Pays = countryCh
			});

			resolver.LocalitéSuggestions.Add (new LocalitéEntity ()
			{
				Nom = "Yvonand",
				Numéro = "1462",
				Pays = countryCh
			});
#else
			List<LocalitéEntity> locs = new List<LocalitéEntity> (DialogTest.ReadNuPost ());

			locs.Sort ((a, b) => string.Compare (a.Nom, b.Nom));
			
			foreach (LocalitéEntity loc in locs)
			{
				resolver.LocalitéSuggestions.Add (loc);
			}
#endif
			
			return resolver;
		}


		[Test]
		public void Check80Speed()
		{
			EntityContext context = EntityContext.Current;

			PaysEntity pays = context.CreateEmptyEntity<PaysEntity> ();
			pays.Code = "CH";
			pays.Nom = "Suisse";

			LocalitéEntity localité = context.CreateEmptyEntity<LocalitéEntity> ();

			localité.Numéro = "1400";
			localité.Nom = "Yverdon-les-Bains";
			localité.Pays = pays;

			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

			int n = 1000*1000;
			string result = localité.Résumé;

			watch.Start ();

			for (int i = 0; i < n; i++)
			{
				if (localité.Résumé != result)
				{
					break;
				}
			}

			watch.Stop ();

			System.Console.WriteLine ("{1} iterations: {0} ms --> {2}", watch.ElapsedMilliseconds, n, result);
			watch.Reset ();
			
			result = localité.Nom;

			watch.Start ();

			for (int i = 0; i < n; i++)
			{
				if (localité.Nom != result)
				{
					break;
				}
			}

			watch.Stop ();

			System.Console.WriteLine ("{1} iterations: {0} ms --> {2}", watch.ElapsedMilliseconds, n, result);
		}


		private class TestResolver : IEntityResolver
		{
			public TestResolver()
			{
				this.localitéSuggestions = new List<LocalitéEntity> ();
				this.paysSuggestions = new List<PaysEntity> ();
			}

			public IList<LocalitéEntity> LocalitéSuggestions
			{
				get
				{
					return this.localitéSuggestions;
				}
			}

			public IList<PaysEntity> PaysSuggestions
			{
				get
				{
					return this.paysSuggestions;
				}
			}

			#region IEntityResolver Members

			public IEnumerable<AbstractEntity> Resolve(Druid entityId, string criteria)
			{
				yield break;
			}
			
			public IEnumerable<AbstractEntity> Resolve(AbstractEntity template)
			{
				LocalitéEntity loc  = AbstractEntity.Resolve<LocalitéEntity> (template);
				PaysEntity     pays = AbstractEntity.Resolve<PaysEntity> (template);

				if (loc != null)
				{
					System.Diagnostics.Debug.WriteLine ("Search for Localité :\n" + loc.Dump ());
					
					foreach (LocalitéEntity item in this.localitéSuggestions)
					{
						if ((TestResolver.Match (item.Numéro, loc.Numéro)) &&
							(TestResolver.Match (item.Nom, loc.Nom)) &&
							(TestResolver.Match (item.Résumé, loc.Résumé)))
						{
							yield return item;
						}
					}
				}

				if (pays != null)
				{
					System.Diagnostics.Debug.WriteLine ("Search for Pays :\n" + pays.Dump ());

					foreach (PaysEntity item in this.paysSuggestions)
					{
						if (item.Nom.Contains (pays.Nom))
						{
							yield return item;
						}
					}
				}
			}

			private static bool Match(string a, string b)
			{
				if (string.IsNullOrEmpty (b))
				{
					return true;
				}
				else
				{
					return a.Contains (b);
				}
			}

			#endregion

			private readonly List<LocalitéEntity> localitéSuggestions;
			private readonly List<PaysEntity> paysSuggestions;
		}

		public static IEnumerable<LocalitéEntity> ReadNuPost()
		{
			PaysEntity countryCh = new PaysEntity ()
			{
				Code = "CH",
				Nom = "Suisse"
			};
			
			foreach (string line in System.IO.File.ReadAllLines (@"S:\Epsitec.Cresus\External\NUPOST.TXT", System.Text.Encoding.Default))
			{
				string[] values = line.Split ('\t');

				LocalitéEntity loc = new LocalitéEntity ();

				using (loc.DefineOriginalValues ())
				{
					loc.Numéro = values[2];
					loc.Nom = values[5];
					loc.Pays = countryCh;
				}

				yield return loc;
			}
		}


		ResourceManager resourceManager;
	}
}
