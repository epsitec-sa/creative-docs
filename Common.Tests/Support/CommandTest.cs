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

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Support
{
    [TestFixture]
    public class CommandTest
    {
        [SetUp]
        public void Initialize()
        {
            Epsitec.Common.Widgets.Widget.Initialize();
        }

        [Test]
        public void CheckCommandContext()
        {
            Command command = Command.Get("TestSave");

            command.ManuallyDefineCommand(
                "Enregistre le document ouvert",
                "save.icon",
                null,
                false
            );
            command.Shortcuts.Add(new Shortcut('S', ModifierKeys.Control));

            Assert.AreEqual("TestSave", command.CommandId);
            Assert.IsTrue(command.IsReadWrite);
            Assert.IsFalse(command.IsReadOnly);

            CommandContext contextA = new CommandContext();
            CommandContext contextB = new CommandContext();

            Visual v1 = new Visual();
            Visual v2 = new Visual();
            Visual v3 = new Visual();

            v1.Children.Add(v2);
            v2.Children.Add(v3);

            CommandContext.SetContext(v1, contextA);
            CommandContext.SetContext(v2, contextB);

            CommandContextChain chain;

            chain = CommandContextChain.BuildChain(v1);

            Assert.AreEqual(1, Collection.Count(chain.Contexts));
            Assert.AreEqual(contextA, Collection.ToArray(chain.Contexts)[0]);

            chain = CommandContextChain.BuildChain(v2);

            Assert.AreEqual(2, Collection.Count(chain.Contexts));
            Assert.AreEqual(contextB, Collection.ToArray(chain.Contexts)[0]);
            Assert.AreEqual(contextA, Collection.ToArray(chain.Contexts)[1]);

            chain = CommandContextChain.BuildChain(v3);

            Assert.AreEqual(2, Collection.Count(chain.Contexts));
            Assert.AreEqual(contextB, Collection.ToArray(chain.Contexts)[0]);
            Assert.AreEqual(contextA, Collection.ToArray(chain.Contexts)[1]);

            v3.CommandObject = command;

            Assert.AreEqual("TestSave", v3.CommandName);

            CommandCache.Instance.Synchronize();

            Assert.IsTrue(v3.Enable);

            CommandState stateA;
            CommandState stateB;

            stateA = contextA.GetCommandState(command);

            Assert.IsNotNull(stateA);
            Assert.AreEqual("SimpleState", stateA.GetType().Name);

            stateA.Enable = false;

            Assert.IsTrue(v3.Enable);
            CommandCache.Instance.Synchronize();
            Assert.IsFalse(v3.Enable);
            Assert.AreEqual(stateA, chain.GetCommandState(command.CommandId));

            //	En créant un stateB dans contextB, on va se trouver plus près de v3
            //	dans la chaîne des contextes des commandes. Du coup, c'est l'état de
            //	stateB (enabled) qui l'emportera sur stateA (disabled) :

            stateB = contextB.GetCommandState(command);

            Assert.AreNotEqual(stateA, stateB);

            Assert.IsFalse(v3.Enable);
            CommandCache.Instance.Synchronize();
            Assert.IsTrue(v3.Enable);
            Assert.AreEqual(stateB, chain.GetCommandState(command.CommandId));

            contextB.ClearCommandState(command);

            Assert.IsTrue(v3.Enable);
            CommandCache.Instance.Synchronize();
            Assert.IsFalse(v3.Enable);
            Assert.AreEqual(stateA, chain.GetCommandState(command.CommandId));
        }
    }
}
