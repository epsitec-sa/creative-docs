/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System.Threading;
using System.Threading.Tasks;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Widgets;
using NUnit.Framework;

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

            Epsitec.Common.Support.Implementation.FileProvider.DefineGlobalProbingPath(
                string.Join(";", paths)
            );
            Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookMetal");
            Epsitec.Common.Document.Engine.Initialize();
            Common.Widgets.Platform.Timer.PendingTimers += (_) =>
            {
                Common.Widgets.Platform.SDLWrapper.SDLWindowManager.PushUserEvent(0, null);
            };
        }

        [Test]
        public void CheckProgress()
        {
            bool executed = false;
            WorkInProgressDialog.WIPTaskDelegate waitForProgress = async Task (
                IWorkInProgressReport report,
                CancellationToken ct
            ) =>
            {
                report.DefineOperation("Waiting");

                for (int i = 5; i >= 0; i--)
                {
                    report.DefineProgress((5 - i) / 5.0, string.Format("{0} seconds remaining", i));
                    await Task.Delay(1000, ct);

                    ct.ThrowIfCancellationRequested();
                }
                report.DefineProgress(1.0, "done");
                executed = true;
            };

            WorkInProgressDialog.Execute(
                "CheckProgress",
                ProgressIndicatorStyle.Default,
                waitForProgress
            );

            Assert.IsTrue(executed);
        }

        [Test]
        public void CheckCancellableProgress()
        {
            bool executed = false;
            WorkInProgressDialog.WIPTaskDelegate waitForProgress = async Task (
                IWorkInProgressReport report,
                CancellationToken ct
            ) =>
            {
                report.DefineOperation("Waiting");

                for (int i = 5; i >= 0; i--)
                {
                    report.DefineProgress((5 - i) / 5.0, string.Format("{0} seconds remaining", i));
                    await Task.Delay(1000, ct);

                    ct.ThrowIfCancellationRequested();
                }
                report.DefineProgress(1.0, "done");
                executed = true;
            };

            WorkInProgressDialog.ExecuteCancellable(
                "CheckCancellableProgress",
                ProgressIndicatorStyle.Default,
                waitForProgress
            );

            Assert.IsTrue(executed);
        }

        [Test]
        public void CheckCancellableProgressUnknownDuration()
        {
            bool executed = false;
            WorkInProgressDialog.WIPTaskDelegate waitForProgress = async Task (
                IWorkInProgressReport report,
                CancellationToken ct
            ) =>
            {
                report.DefineOperation("Waiting");

                for (int i = 5; i >= 0; i--)
                {
                    report.DefineProgress((5 - i) / 5.0, string.Format("{0} seconds remaining", i));
                    await Task.Delay(1000, ct);

                    ct.ThrowIfCancellationRequested();
                }
                report.DefineProgress(1.0, "done");
                executed = true;
            };

            WorkInProgressDialog.ExecuteCancellable(
                "CheckCancellableProgressUnknownDuration",
                ProgressIndicatorStyle.UnknownDuration,
                waitForProgress
            );

            Assert.IsTrue(executed);
        }
    }
}
