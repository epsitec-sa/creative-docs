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


using System;
using System.Collections.Generic;
using Epsitec.Common.Dialogs.Helpers;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
    /// <summary>
    /// Summary description for Message.
    /// </summary>
    public static class MessageDialog
    {
        public static IDialog CreateYesNoCancel(
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
        {
            return new YesNoCancelDialog(
                dialogTitle,
                yesText,
                noText,
                cancelText,
                messageIcon,
                messageText,
                commandYesTemplate,
                commandNoTemplate,
                commandDispatcher
            );
        }

        public static IDialog CreateYesNoCancel(
            string dialogTitle,
            string messageIcon,
            FormattedText messageText,
            string commandYesTemplate,
            string commandNoTemplate,
            CommandDispatcher commandDispatcher
        )
        {
            return new YesNoCancelDialog(
                dialogTitle,
                null,
                null,
                null,
                messageIcon,
                messageText,
                commandYesTemplate,
                commandNoTemplate,
                commandDispatcher
            );
        }

        public static IDialog CreateYesNoCancel(
            string dialogTitle,
            DialogIcon icon,
            FormattedText messageText
        )
        {
            return CreateYesNoCancel(dialogTitle, icon, messageText, null, null, null);
        }

        public static IDialog CreateYesNoCancel(
            string dialogTitle,
            DialogIcon icon,
            FormattedText messageText,
            string commandYesTemplate,
            string commandNoTemplate,
            CommandDispatcher commandDispatcher
        )
        {
            string messageIcon = MessageBuilder.GetIconUri(icon);

            return CreateYesNoCancel(
                dialogTitle,
                messageIcon,
                messageText,
                commandYesTemplate,
                commandNoTemplate,
                commandDispatcher
            );
        }

        public static IDialog CreateYesNo(
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
        {
            AbstractMessageDialog dialog = new YesNoCancelDialog(
                dialogTitle,
                yesText,
                noText,
                cancelText,
                messageIcon,
                messageText,
                commandYesTemplate,
                commandNoTemplate,
                commandDispatcher
            );
            dialog.HideCancelButton();
            return dialog;
        }

        public static IDialog CreateYesNo(
            string dialogTitle,
            string messageIcon,
            FormattedText messageText,
            string commandYesTemplate,
            string commandNoTemplate,
            CommandDispatcher commandDispatcher
        )
        {
            AbstractMessageDialog dialog = new YesNoCancelDialog(
                dialogTitle,
                null,
                null,
                null,
                messageIcon,
                messageText,
                commandYesTemplate,
                commandNoTemplate,
                commandDispatcher
            );
            dialog.HideCancelButton();
            return dialog;
        }

        public static IDialog CreateYesNo(
            string dialogTitle,
            DialogIcon icon,
            FormattedText messageText
        )
        {
            return MessageDialog.CreateYesNo(dialogTitle, icon, messageText, null, null, null);
        }

        public static IDialog CreateYesNo(
            string dialogTitle,
            DialogIcon icon,
            FormattedText messageText,
            string commandYesTemplate,
            string commandNoTemplate,
            CommandDispatcher commandDispatcher
        )
        {
            string messageIcon = MessageBuilder.GetIconUri(icon);

            return MessageDialog.CreateYesNo(
                dialogTitle,
                messageIcon,
                messageText,
                commandYesTemplate,
                commandNoTemplate,
                commandDispatcher
            );
        }

        public static IDialog CreateOkCancel(
            string dialogTitle,
            string okText,
            string cancelText,
            string messageIcon,
            FormattedText messageText,
            string commandOkTemplate,
            CommandDispatcher commandDispatcher
        )
        {
            return new OkCancelDialog(
                dialogTitle,
                okText,
                cancelText,
                messageIcon,
                messageText,
                commandOkTemplate,
                commandDispatcher
            );
        }

        public static IDialog CreateOkCancel(
            string dialogTitle,
            string messageIcon,
            FormattedText messageText,
            string commandOkTemplate,
            CommandDispatcher commandDispatcher
        )
        {
            return new OkCancelDialog(
                dialogTitle,
                null,
                null,
                messageIcon,
                messageText,
                commandOkTemplate,
                commandDispatcher
            );
        }

        public static IDialog CreateOkCancel(
            string dialogTitle,
            DialogIcon icon,
            FormattedText messageText
        )
        {
            return MessageDialog.CreateOkCancel(dialogTitle, icon, messageText, null, null);
        }

        public static IDialog CreateOkCancel(
            string dialogTitle,
            DialogIcon icon,
            FormattedText messageText,
            string commandOkTemplate,
            CommandDispatcher commandDispatcher
        )
        {
            string messageIcon = MessageBuilder.GetIconUri(icon);

            return MessageDialog.CreateOkCancel(
                dialogTitle,
                messageIcon,
                messageText,
                commandOkTemplate,
                commandDispatcher
            );
        }

        public static IDialog CreateOk(
            string dialogTitle,
            string okText,
            string cancelText,
            string messageIcon,
            FormattedText messageText,
            string commandOkTemplate,
            CommandDispatcher commandDispatcher
        )
        {
            AbstractMessageDialog dialog = new OkCancelDialog(
                dialogTitle,
                okText,
                cancelText,
                messageIcon,
                messageText,
                commandOkTemplate,
                commandDispatcher
            );
            dialog.HideCancelButton();
            return dialog;
        }

        public static IDialog CreateOk(
            string dialogTitle,
            string messageIcon,
            FormattedText messageText,
            string commandOkTemplate,
            CommandDispatcher commandDispatcher
        )
        {
            AbstractMessageDialog dialog = new OkCancelDialog(
                dialogTitle,
                null,
                null,
                messageIcon,
                messageText,
                commandOkTemplate,
                commandDispatcher
            );
            dialog.HideCancelButton();
            return dialog;
        }

        public static IDialog CreateOk(
            string dialogTitle,
            DialogIcon icon,
            FormattedText messageText
        )
        {
            return MessageDialog.CreateOk(dialogTitle, icon, messageText, null, null);
        }

        public static IDialog CreateOk(
            string dialogTitle,
            DialogIcon icon,
            FormattedText messageText,
            string commandOkTemplate,
            CommandDispatcher commandDispatcher
        )
        {
            string messageIcon = MessageBuilder.GetIconUri(icon);

            return MessageDialog.CreateOk(
                dialogTitle,
                messageIcon,
                messageText,
                commandOkTemplate,
                commandDispatcher
            );
        }

        public static IDialog CreateConfirmation(
            string title,
            string header,
            List<string> questions,
            bool hasCancelButton
        )
        {
            ConfirmationDialog dialog = new ConfirmationDialog(
                title,
                header,
                questions,
                hasCancelButton
            );
            return dialog;
        }

        public static DialogResult ShowError(FormattedText formattedErrorMessage, Window owner)
        {
            return MessageDialog.ShowError(formattedErrorMessage, null, owner);
        }

        public static DialogResult ShowError(
            FormattedText formattedErrorMessage,
            string title,
            Window owner
        )
        {
            if (formattedErrorMessage.IsNullOrEmpty())
            {
                return DialogResult.None;
            }

            IDialog dialog = MessageDialog.CreateOk(
                title ?? MessageBuilder.GetDialogTitle(owner),
                DialogIcon.Warning,
                formattedErrorMessage
            );

            Console.WriteLine($"{title}: {formattedErrorMessage}");
            dialog.OwnerWindow = owner;
            dialog.OpenDialog();

            return dialog.Result;
        }

        public static DialogResult ShowMessage(FormattedText formattedMessage, Window owner)
        {
            return MessageDialog.ShowMessage(formattedMessage, null, owner);
        }

        public static DialogResult ShowMessage(
            FormattedText formattedMessage,
            string title,
            Window owner
        )
        {
            if (formattedMessage.IsNullOrEmpty())
            {
                return DialogResult.None;
            }

            IDialog dialog = MessageDialog.CreateOk(
                title ?? MessageBuilder.GetDialogTitle(owner),
                DialogIcon.Warning,
                formattedMessage
            );

            dialog.OwnerWindow = owner;
            dialog.OpenDialog();

            return dialog.Result;
        }

        public static DialogResult ShowQuestion(FormattedText formattedQuestion, Window owner)
        {
            if (formattedQuestion.IsNullOrEmpty())
            {
                return DialogResult.None;
            }

            IDialog dialog = MessageDialog.CreateYesNo(
                MessageBuilder.GetDialogTitle(owner),
                DialogIcon.Question,
                formattedQuestion
            );

            dialog.OwnerWindow = owner;
            dialog.OpenDialog();

            return dialog.Result;
        }
    }
}
