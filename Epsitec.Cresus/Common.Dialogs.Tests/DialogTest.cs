using NUnit.Framework;

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

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

			Assert.IsNotNull (dialog);
			Assert.IsFalse (dialog.HasWindow);

			Window window = dialog.DialogWindow;

			Assert.IsNotNull (window);
			Assert.IsTrue (dialog.HasWindow);

			dialog.IsModal = false;
			dialog.OpenDialog ();
			Window.RunInTestEnvironment (window);
		}


		ResourceManager resourceManager;
	}
}
