//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using NUnit.Framework;
using Epsitec.Common.Widgets;
using Epsitec.Common.Dialogs;

namespace Epsitec.Common.Tests.Dialogs
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
				@"S:\Epsitec.Cresus\Common.DocumentEditor\Resources",
			};

			Epsitec.Common.Support.Implementation.FileProvider.DefineGlobalProbingPath (string.Join (";", paths));
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
			Epsitec.Common.Document.Engine.Initialize ();
		}

		[Test]
		public void CheckProgress()
		{
			bool executed = false;

			WorkInProgressDialog.Execute ("CheckProgress", ProgressIndicatorStyle.Default,
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
				});

			Assert.IsTrue (executed);
		}

		[Test]
		public void CheckCancellableProgress()
		{
			bool executed = false;

			WorkInProgressDialog.ExecuteCancellable ("CheckCancellableProgress", ProgressIndicatorStyle.Default,
				delegate (IWorkInProgressReport report)
				{
					report.DefineOperation ("Waiting");

					for (int i = 5; i >= 0; i--)
					{
						report.DefineProgress ((5-i)/5.0, string.Format ("{0} seconds remaining", i));
						System.Threading.Thread.Sleep (1*1000);

						if (report.Canceled)
						{
							return;
						}
					}
					report.DefineProgress (1.0, "done");
					executed = true;
				});

			Assert.IsTrue (executed);
		}
		
		[Test]
		public void CheckCancellableProgressUnknownDuration()
		{
			bool executed = false;

			WorkInProgressDialog.ExecuteCancellable ("CheckCancellableProgressUnknownDuration", ProgressIndicatorStyle.UnknownDuration,
				delegate (IWorkInProgressReport report)
				{
					report.DefineOperation ("Waiting");

					for (int i = 0; i < 100; i++)
					{
						System.Threading.Thread.Sleep (100);

						if (report.Canceled)
						{
							return;
						}
					}
					executed = true;
				});

			Assert.IsTrue (executed);
		}
	}
}
