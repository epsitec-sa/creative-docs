using NUnit.Framework;

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.AddressBook.Entities;

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
			Dialog dialog = Dialog.Load (this.resourceManager, Druid.Parse ("_631"));

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
			Dialog dialog = Dialog.Load (this.resourceManager, Druid.Parse ("_8V1"));

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


		ResourceManager resourceManager;
	}
}
