using NUnit.Framework;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
	[TestFixture]
	public class WorkInProgressDialogTest
	{
		[SetUp]
		public void Initialize()
		{
			string[] paths = new string[]
			{
				@"S:\Epsitec.Cresus\Common.Widgets\Resources",
				@"S:\Epsitec.Cresus\Common.Types\Resources",
				@"S:\Epsitec.Cresus\Common.Document\Resources",
				@"S:\Epsitec.Cresus\Common.Dialogs\Resources",
				@"S:\Epsitec.Cresus\Common.Designer\Resources",
				@"S:\Epsitec.Cresus\App.DocumentEditor\Resources",
			};

			Epsitec.Common.Support.Implementation.FileProvider.DefineGlobalProbingPath (string.Join (";", paths));
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookRoyale");
			Epsitec.Common.Document.Engine.Initialize ();
		}

		[Test]
		public void CheckSleep()
		{
			WorkInProgressDialog dialog = new WorkInProgressDialog ("Test Sleep");
			bool executed = false;
			
			dialog.Action =
				delegate (WorkInProgressDialog d)
				{
					d.DefineMessage ("Start waiting");
					System.Threading.Thread.Sleep (1*1000);
					d.DefineMessage ("2 more seconds");
					System.Threading.Thread.Sleep (1*1000);
					d.DefineMessage ("1 more second");
					System.Threading.Thread.Sleep (1*1000);
					d.DefineMessage ("Done");
					executed = true;
				};

			dialog.OpenDialog ();

			Assert.IsTrue (executed);
		}
	}
}
