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
		public void Check02SimpleForm()
		{
			Dialog dialog = Dialog.Load (this.resourceManager, Druid.Parse ("_8V1"));	//	mask for AdresseEntity, from Cresus.AddressBook

			StringType.Default.DefineMaximumLength (25);

			//	Mask with :
			//	- Rue
			//	- CasePostale
			//	- Localit�.R�sum�

			DialogSearchController searchController = new DialogSearchController ();
			searchController.Resolver = DialogTest.CreateSuggestions ();

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
							searchController.ClearSuggestions ();
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
				};

			Assert.IsNotNull (dialog);
			Assert.IsFalse (dialog.HasWindow);

			Window window = dialog.DialogWindow;

			Assert.IsNotNull (window);
			Assert.IsTrue (dialog.HasWindow);

			dialog.IsModal = false;
			dialog.Data = DialogTest.CreateDefaultDialogData ();
			dialog.SearchController = searchController;
			dialog.OpenDialog ();
			Window.RunInTestEnvironment (window);

			System.Console.Out.WriteLine ("Raw dialog data:");
			System.Console.Out.WriteLine (dialog.Data.Data.Dump ());
			
			switch (dialog.DialogResult)
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
		}

		
		internal static AdresseEntity CreateDefaultAdresseEntity()
		{
			AdresseEntity adresse = EntityContext.Current.CreateEntity<AdresseEntity> ();

			adresse.Rue = "Ch. du Fontenay 6";
			adresse.Localit�.Nom = "Yverdon-les-Bains";
			adresse.Localit�.Num�ro = "1400";
			adresse.Localit�.Pays.Code = "CH";
			adresse.Localit�.Pays.Nom = "Suisse";

			return adresse;
		}

		internal static DialogData CreateDefaultDialogData()
		{
			return new DialogData (DialogTest.CreateDefaultAdresseEntity (), DialogDataMode.Isolated);
		}

		private static IEntityResolver CreateSuggestions()
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

			resolver.Localit�Suggestions.Add (new Localit�Entity ()
			{
				Nom = "Yverdon-les-Bains",
				Num�ro = "1400",
				Pays = countryCh
			});

			resolver.Localit�Suggestions.Add (new Localit�Entity ()
			{
				Nom = "Susc�vaz",
				Num�ro = "1437",
				Pays = countryCh
			});

			resolver.Localit�Suggestions.Add (new Localit�Entity ()
			{
				Nom = "Treycovagnes",
				Num�ro = "1436",
				Pays = countryCh
			});

			resolver.Localit�Suggestions.Add (new Localit�Entity ()
			{
				Nom = "Yvonand",
				Num�ro = "1462",
				Pays = countryCh
			});
			
			return resolver;
		}


		[Test]
		public void Check80Speed()
		{
			EntityContext context = EntityContext.Current;

			PaysEntity pays = context.CreateEmptyEntity<PaysEntity> ();
			pays.Code = "CH";
			pays.Nom = "Suisse";

			Localit�Entity localit� = context.CreateEmptyEntity<Localit�Entity> ();

			localit�.Num�ro = "1400";
			localit�.Nom = "Yverdon-les-Bains";
			localit�.Pays = pays;

			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

			int n = 1000*1000;
			string result = localit�.R�sum�;

			watch.Start ();

			for (int i = 0; i < n; i++)
			{
				if (localit�.R�sum� != result)
				{
					break;
				}
			}

			watch.Stop ();

			System.Console.WriteLine ("{1} iterations: {0} ms --> {2}", watch.ElapsedMilliseconds, n, result);
			watch.Reset ();
			
			result = localit�.Nom;

			watch.Start ();

			for (int i = 0; i < n; i++)
			{
				if (localit�.Nom != result)
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
				this.localit�Suggestions = new List<Localit�Entity> ();
				this.paysSuggestions = new List<PaysEntity> ();
			}

			public IList<Localit�Entity> Localit�Suggestions
			{
				get
				{
					return this.localit�Suggestions;
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

			public IEnumerable<AbstractEntity> Resolve(AbstractEntity template)
			{
				Localit�Entity loc  = template as Localit�Entity;
				PaysEntity     pays = template as PaysEntity;

				if (loc != null)
				{
					System.Diagnostics.Debug.WriteLine ("Search for Localit� :\n" + loc.Dump ());
					
					foreach (Localit�Entity item in this.localit�Suggestions)
					{
						if ((TestResolver.Match (item.Num�ro, loc.Num�ro)) &&
							(TestResolver.Match (item.Nom, loc.Nom)) &&
							(TestResolver.Match (item.R�sum�, loc.R�sum�)))
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

			private readonly List<Localit�Entity> localit�Suggestions;
			private readonly List<PaysEntity> paysSuggestions;
		}


		ResourceManager resourceManager;
	}
}
