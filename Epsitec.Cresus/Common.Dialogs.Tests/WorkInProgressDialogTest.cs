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
		public void CheckProgress()
		{
			WorkInProgressDialog dialog = new WorkInProgressDialog ("Test Sleep", false);
			bool executed = false;

			dialog.Action =
				delegate (IWorkInProgressReport report)
				{
					report.DefineOperation ("Waiting");

					for (int i = 5; i >= 0; i--)
					{
						report.DefineProgress ((5-i)/5.0, string.Format ("{0} seconds remaining", i));
						System.Threading.Thread.Sleep (1*1000);
					}
					report.DefineProgress (1.0, "done");
					executed = true;
				};

			dialog.OpenDialog ();

			Assert.IsTrue (executed);
		}
		
		[Test]
		public void CheckCancellableProgress()
		{
			WorkInProgressDialog dialog = new WorkInProgressDialog ("Test Sleep", true);
			bool executed = false;

			dialog.Action =
				delegate (IWorkInProgressReport report)
				{
					report.DefineOperation ("Waiting");

					for (int i = 5; i >= 0; i--)
					{
						report.DefineProgress ((5-i)/5.0, string.Format ("{0} seconds remaining", i));
						System.Threading.Thread.Sleep (1*1000);
						
						if (report.Cancelled)
						{
							return;
						}
					}
					report.DefineProgress (1.0, "done");
					executed = true;
				};

			dialog.OpenDialog ();

			Assert.IsTrue (executed);
		}
	}
}
