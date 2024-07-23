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


using Epsitec.Common.Dialogs.Helpers;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
    internal sealed class YesNoCancelDialog : AbstractYesNoCancelDialog
    {
        internal YesNoCancelDialog(
            string dialogTitle,
            string yesText,
            string noText,
            string cancelText,
            string messageIcon,
            FormattedText messageText,
            string commandYesTemplate,
            string commandNoTemplate,
            CommandDispatcher commandDispatcher
        )
            : base(
                dialogTitle,
                yesText,
                noText,
                cancelText,
                commandYesTemplate,
                commandNoTemplate,
                commandDispatcher
            )
        {
            this.messageIcon = messageIcon;
            this.messageText = messageText;
        }

        public override string[] CommandArgs
        {
            get { return new string[0]; }
        }

        protected override void OnDialogOpened()
        {
            base.OnDialogOpened();
            Platform.Beep.MessageBeep(Platform.Beep.MessageType.Warning);
        }

        protected override Widgets.Widget CreateBodyWidget()
        {
            return MessageBuilder.CreateIconAndText(this.messageIcon, this.messageText);
        }

        private readonly string messageIcon;
        private readonly FormattedText messageText;
    }
}
