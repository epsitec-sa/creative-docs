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


using Epsitec.Common.Widgets.Behaviors;

namespace Epsitec.Common.Widgets
{
    public class SlimFieldMenuItemCommand : SlimFieldMenuItem
    {
        public SlimFieldMenuItemCommand(
            SlimFieldMenuItemCommandCode code,
            System.Action<SlimFieldMenuBehavior> command = null
        )
            : base(
                SlimFieldMenuItemCommand.GetSymbolText(code),
                style: SlimFieldMenuItemCommand.GetSymbolStyle(code)
            )
        {
            if (command == null)
            {
                command = SlimFieldMenuItemCommand.GetDefaultCommand(code);
            }

            this.code = code;
            this.command = command;
        }

        public SlimFieldMenuItemCommandCode Code
        {
            get { return this.code; }
        }

        public override bool ExecuteCommand(SlimFieldMenuBehavior source)
        {
            if (this.command == null)
            {
                return false;
            }
            else
            {
                this.command(source);
                return true;
            }
        }

        private static string GetSymbolText(SlimFieldMenuItemCommandCode code)
        {
            switch (code)
            {
                case SlimFieldMenuItemCommandCode.Clear:
                    return "✘";

                case SlimFieldMenuItemCommandCode.Extra:
                    return "plus…";

                default:
                    return "";
            }
        }

        private static SlimFieldMenuItemStyle GetSymbolStyle(SlimFieldMenuItemCommandCode code)
        {
            switch (code)
            {
                case SlimFieldMenuItemCommandCode.Clear:
                    return SlimFieldMenuItemStyle.Symbol;

                case SlimFieldMenuItemCommandCode.Extra:
                    return SlimFieldMenuItemStyle.Extra;

                default:
                    return SlimFieldMenuItemStyle.Extra;
            }
        }

        private static System.Action<SlimFieldMenuBehavior> GetDefaultCommand(
            SlimFieldMenuItemCommandCode code
        )
        {
            switch (code)
            {
                case SlimFieldMenuItemCommandCode.Clear:
                    return item => item.Clear();

                default:
                    return null;
            }
        }

        private readonly SlimFieldMenuItemCommandCode code;
        private readonly System.Action<SlimFieldMenuBehavior> command;
    }
}
