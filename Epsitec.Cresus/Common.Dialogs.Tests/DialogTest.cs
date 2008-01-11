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

			//	Mask with :
			//	- Rue
			//	- CasePostale
			//	- Localité.Résumé

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

			DialogSearchController searchController = new DialogSearchController ();
			searchController.Resolver = DialogTest.CreateSuggestions ();

			dialog.IsModal = false;
			dialog.DialogData.SearchController = searchController;
			dialog.OpenDialog ();
			Window.RunInTestEnvironment (window);
		}

		private static IEntityResolver CreateSuggestions()
		{
			TestResolver resolver = new TestResolver ();

			PaysEntity country = new PaysEntity ()
				{
					Code = "CH",
					Nom = "Suisse"
				};

			resolver.Suggestions.Add (new LocalitéEntity ()
			{
				Nom = "Yverdon-les-Bains",
				Numéro = "1400",
				Pays = country
			});

			resolver.Suggestions.Add (new LocalitéEntity ()
			{
				Nom = "Suscévaz",
				Numéro = "1437",
				Pays = country
			});

			resolver.Suggestions.Add (new LocalitéEntity ()
			{
				Nom = "Treycovagnes",
				Numéro = "1436",
				Pays = country
			});

			resolver.Suggestions.Add (new LocalitéEntity ()
			{
				Nom = "Yvonand",
				Numéro = "1462",
				Pays = country
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
				this.suggestions = new List<AbstractEntity> ();
			}

			public IList<AbstractEntity> Suggestions
			{
				get
				{
					return this.suggestions;
				}
			}

			#region IEntityResolver Members

			public IEnumerable<AbstractEntity> Resolve(AbstractEntity template)
			{
				LocalitéEntity loc = template as LocalitéEntity;

				foreach (LocalitéEntity item in this.suggestions)
				{
					if (item.Résumé.Contains (loc.Résumé))
					{
						yield return item;
					}
				}
			}

			#endregion

			private readonly List<AbstractEntity> suggestions;
		}


		ResourceManager resourceManager;
	}
}
