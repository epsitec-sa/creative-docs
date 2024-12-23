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


using Epsitec.Common.Drawing;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using NUnit.Framework;

namespace Epsitec.Common.Tests.UI
{
    [TestFixture]
    public class MetaButtonTest
    {
        [SetUp]
        public void Initialize()
        {
            Epsitec.Common.Document.Engine.Initialize();
            Epsitec.Common.Widgets.Widget.Initialize();
            Epsitec.Common.Widgets.Adorners.Factory.SetActive("LookMetal");
        }

        [Test]
        public void AutomatedTestEnvironment()
        {
            Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
        }

        [Test]
        public void Check01Variations()
        {
            Window window = new Window();

            window.Text = "MetaButtonTest.Check01Variations";
            window.ClientSize = new Size(300, 600);

            FrameBox box = new FrameBox()
            {
                ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
                Margins = new Margins(4, 4, 8, 8),
                Dock = DockStyle.Fill,
                Embedder = window.Root
            };

            System.Action<MetaButton>[] modes = new System.Action<MetaButton>[]
            {
                button =>
                {
                    button.MarkDisposition = ButtonMarkDisposition.None;
                    button.BulletColor = Color.Empty;
                },
                button =>
                {
                    button.MarkDisposition = ButtonMarkDisposition.Left;
                    button.BulletColor = Color.Empty;
                },
                button =>
                {
                    button.MarkDisposition = ButtonMarkDisposition.Below;
                    button.BulletColor = Color.Empty;
                    button.PreferredHeight += button.MarkLength;
                },
                button =>
                {
                    button.MarkDisposition = ButtonMarkDisposition.None;
                    button.BulletColor = Color.FromName("Lime");
                }
            };

            foreach (System.Action<MetaButton> setup in modes)
            {
                MetaButton b1 = new MetaButton()
                {
                    ButtonClass = ButtonClass.DialogButton,
                    Dock = DockStyle.Stacked,
                    Embedder = box,
                    Text = "Text, DialogButton",
                    Margins = new Margins(0, 0, 0, 2)
                };

                MetaButton b2 = new MetaButton()
                {
                    ButtonClass = ButtonClass.DialogButton,
                    Dock = DockStyle.Stacked,
                    Embedder = box,
                    Text = "Text+Icon, DialogButton",
                    IconUri = "manifest:Common/Widgets/Images/TableEdition.icon",
                    Margins = new Margins(0, 0, 0, 2)
                };

                MetaButton b3 = new MetaButton()
                {
                    ButtonClass = ButtonClass.RichDialogButton,
                    Dock = DockStyle.Stacked,
                    Embedder = box,
                    Text = "Text, RichDialogButton",
                    Margins = new Margins(0, 0, 0, 2),
                    PreferredHeight = 28
                };

                MetaButton b4 = new MetaButton()
                {
                    ButtonClass = ButtonClass.RichDialogButton,
                    Dock = DockStyle.Stacked,
                    Embedder = box,
                    Text = "Text+Icon, RichDialogButton",
                    IconUri = "manifest:Common/Widgets/Images/TableEdition.icon",
                    Margins = new Margins(0, 0, 0, 2),
                    PreferredHeight = 28
                };

                MetaButton b5 = new MetaButton()
                {
                    ButtonClass = ButtonClass.FlatButton,
                    Dock = DockStyle.Stacked,
                    Embedder = box,
                    Text = "Text, FlatButton",
                    Margins = new Margins(0, 0, 0, 2)
                };

                MetaButton b6 = new MetaButton()
                {
                    ButtonClass = ButtonClass.FlatButton,
                    Dock = DockStyle.Stacked,
                    Embedder = box,
                    Text = "Text+Icon, FlatButton",
                    IconUri = "manifest:Common/Widgets/Images/TableEdition.icon",
                    Margins = new Margins(0, 0, 0, 2)
                };

                setup(b1);
                setup(b2);
                setup(b3);
                setup(b4);
                setup(b5);
                setup(b6);

                b1.Clicked += (s, e) =>
                {
                    b1.ActiveState =
                        b1.ActiveState == ActiveState.Yes ? ActiveState.No : ActiveState.Yes;
                };
                b2.Clicked += (s, e) =>
                {
                    b2.ActiveState =
                        b2.ActiveState == ActiveState.Yes ? ActiveState.No : ActiveState.Yes;
                };
                b3.Clicked += (s, e) =>
                {
                    b3.ActiveState =
                        b3.ActiveState == ActiveState.Yes ? ActiveState.No : ActiveState.Yes;
                };
                b4.Clicked += (s, e) =>
                {
                    b4.ActiveState =
                        b4.ActiveState == ActiveState.Yes ? ActiveState.No : ActiveState.Yes;
                };
                b5.Clicked += (s, e) =>
                {
                    b5.ActiveState =
                        b3.ActiveState == ActiveState.Yes ? ActiveState.No : ActiveState.Yes;
                };
                b6.Clicked += (s, e) =>
                {
                    b6.ActiveState =
                        b4.ActiveState == ActiveState.Yes ? ActiveState.No : ActiveState.Yes;
                };
            }

            window.Show();

            Window.RunInTestEnvironment(window);
        }

        [Test]
        public void Check02CreateFromCommandId()
        {
            Window window = new Window();

            window.Text = "MetaButtonTest.Check02CreateFromCommandId";
            window.ClientSize = new Size(400, 300);

            FrameBox box = new FrameBox()
            {
                ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
                Margins = new Margins(4, 4, 8, 8),
                Dock = DockStyle.Fill,
                Embedder = window.Root
            };

            MetaButton b1 = new MetaButton()
            {
                ButtonClass = ButtonClass.DialogButton,
                Dock = DockStyle.Stacked,
                Embedder = box,
                CommandId = ApplicationCommands.Cut.Caption.Id,
                Margins = new Margins(0, 0, 0, 2),
                PreferredHeight = 32
            };

            MetaButton b2 = new MetaButton()
            {
                ButtonClass = ButtonClass.FlatButton,
                Dock = DockStyle.Stacked,
                Embedder = box,
                CommandId = ApplicationCommands.Cut.Caption.Id,
                Margins = new Margins(0, 0, 0, 2),
                PreferredHeight = 32
            };

            MetaButton b3 = new MetaButton()
            {
                ButtonClass = ButtonClass.RichDialogButton,
                Dock = DockStyle.Stacked,
                Embedder = box,
                CommandId = ApplicationCommands.Cut.Caption.Id,
                Margins = new Margins(0, 0, 0, 2),
                PreferredHeight = 32
            };

            Separator sep = new Separator()
            {
                Dock = DockStyle.Stacked,
                Embedder = box,
                PreferredHeight = 1,
                Margins = new Margins(0, 0, 0, 2)
            };

            MetaButton b4 = new MetaButton()
            {
                Dock = DockStyle.Stacked,
                Embedder = box,
                CommandId = ApplicationCommands.Cut.Caption.Id,
                Margins = new Margins(0, 0, 0, 2),
                PreferredHeight = 32
            };

            window.Show();

            Window.RunInTestEnvironment(window);
        }
    }
}
