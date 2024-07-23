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

using Epsitec.Common.Dialogs;
using Epsitec.Common.Widgets;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Dialogs
{
    using CommandAttribute = Epsitec.Common.Support.CommandAttribute;

    [TestFixture]
    public class MessageTest
    {
        [SetUp]
        public void Initialize()
        {
            Common.Widgets.Widget.Initialize();
            Common.Document.Engine.Initialize();
            Common.Widgets.Adorners.Factory.SetActive("LookMetal");
        }

        [Test]
        public void AutomatedTestEnvironment()
        {
            Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
        }

        public MessageTest()
        {
            this.dispatcher = new CommandDispatcher(
                "MessageTestDispatcher",
                CommandDispatcherLevel.Secondary
            );
            this.dispatcher.RegisterController(this);
        }

        [Test]
        public void CheckYesNoCancel()
        {
            IDialog dialog = MessageDialog.CreateYesNoCancel(
                "Notepad",
                "manifest:Epsitec.Common.Dialogs.Images.Warning.icon",
                "The text in the <i>c:\\Documents and Settings\\Tester\\Desktop\\dummy file.txt</i> file has changed.<br/><br/>Do you want to save the changes ?",
                "ExecYes",
                "ExecNo",
                this.dispatcher
            );

            using (Tool.InjectKey(System.Windows.Forms.Keys.Return))
            {
                dialog.OpenDialog();
            }
        }

        [Test]
        public void CheckYesNo()
        {
            IDialog dialog = MessageDialog.CreateYesNo(
                "Notepad",
                "manifest:Epsitec.Common.Dialogs.Images.Warning.icon",
                "The text in the <i>c:\\Documents and Settings\\Tester\\Desktop\\dummy file.txt</i> file has changed.<br/><br/>Do you want to save the changes ?",
                "ExecYes",
                "ExecNo",
                this.dispatcher
            );

            using (Tool.InjectKey(System.Windows.Forms.Keys.Return))
            {
                dialog.OpenDialog();
            }
        }

        [Test]
        public void CheckOkCancel()
        {
            IDialog dialog = MessageDialog.CreateOkCancel(
                "Notepad",
                "manifest:Epsitec.Common.Dialogs.Images.Warning.icon",
                "About to mogrify this document...",
                "ExecYes",
                this.dispatcher
            );

            using (Tool.InjectKey(System.Windows.Forms.Keys.Return))
            {
                dialog.OpenDialog();
            }
        }

        [Test]
        public void CheckOk()
        {
            IDialog dialog = MessageDialog.CreateOk(
                "Notepad",
                "manifest:Epsitec.Common.Dialogs.Images.Warning.icon",
                "About to mogrify this document...",
                "ExecYes",
                this.dispatcher
            );

            using (Tool.InjectKey(System.Windows.Forms.Keys.Return))
            {
                dialog.OpenDialog();
            }
        }

        [Command("ExecYes")]
        private void CommandExecYes()
        {
            System.Diagnostics.Debug.WriteLine("Yes...");
        }

        [Command("ExecNo")]
        private void CommandExecNo()
        {
            System.Diagnostics.Debug.WriteLine("No...");
        }

        private CommandDispatcher dispatcher;
    }
}
