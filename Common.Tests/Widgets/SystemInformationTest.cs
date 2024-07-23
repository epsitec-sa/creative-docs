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

using Epsitec.Common.Widgets;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Widgets
{
    [TestFixture]
    public class SystemInformationTest
    {
        [Test]
        public void CheckSettings()
        {
            System.Console.Out.WriteLine(
                "DoubleClickDelay:             {0}",
                SystemInformation.DoubleClickDelay
            );
            System.Console.Out.WriteLine(
                "DoubleClickRadius2:           {0}",
                SystemInformation.DoubleClickRadius2
            );
            System.Console.Out.WriteLine(
                "CursorBlinkDelay:             {0}",
                SystemInformation.CursorBlinkDelay
            );
            System.Console.Out.WriteLine(
                "InitialKeyboardDelay:         {0}",
                SystemInformation.InitialKeyboardDelay
            );
            System.Console.Out.WriteLine(
                "KeyboardRepeatPeriod:         {0}",
                SystemInformation.KeyboardRepeatPeriod
            );
            System.Console.Out.WriteLine(
                "MenuShowDelay:                {0}",
                SystemInformation.MenuShowDelay
            );
            System.Console.Out.WriteLine(
                "MenuAnimation:                {0}",
                SystemInformation.MenuAnimation
            );
            System.Console.Out.WriteLine(
                "PreferRightAlignedMenus:      {0}",
                SystemInformation.PreferRightAlignedMenus
            );
            System.Console.Out.WriteLine(
                "SupportsLayeredWindows:       {0}",
                SystemInformation.SupportsLayeredWindows
            );
            System.Console.Out.WriteLine(
                "IsMenuAnimationEnabled:       {0}",
                SystemInformation.IsMenuAnimationEnabled
            );
            System.Console.Out.WriteLine(
                "IsComboAnimationEnabled:      {0}",
                SystemInformation.IsComboAnimationEnabled
            );
            System.Console.Out.WriteLine(
                "IsSmoothScrollEnabled:        {0}",
                SystemInformation.IsSmoothScrollEnabled
            );
            System.Console.Out.WriteLine(
                "IsMetaUnderlineEnabled:       {0}",
                SystemInformation.IsMetaUnderlineEnabled
            );
            System.Console.Out.WriteLine(
                "IsMenuFadingEnabled:          {0}",
                SystemInformation.IsMenuFadingEnabled
            );
            System.Console.Out.WriteLine(
                "IsMenuSelectionFadingEnabled: {0}",
                SystemInformation.IsMenuSelectionFadingEnabled
            );
            System.Console.Out.WriteLine(
                "IsMenuShadowEnabled:          {0}",
                SystemInformation.IsMenuShadowEnabled
            );
        }
    }
}
