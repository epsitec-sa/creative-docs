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
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Dialogs
{
    /// <summary>
    /// Dialogue pour importer une image bitmap.
    /// </summary>
    public class FileOpenImage : AbstractFile
    {
        public FileOpenImage(Document document, Window ownerWindow)
            : base(document, ownerWindow)
        {
            //	Il faut mettre en premier les extensions qu'on souhaite voir.
            this.title = Res.Strings.Dialog.OpenImage.Title;
            this.owner = this.ownerWindow;
            this.enableNavigation = true;
            this.enableMultipleSelection = false;
            this.fileDialogType = FileDialogType.Open;

            this.Filters.Add(
                new FilterItem(
                    "x",
                    "Image",
                    "*.tif|*.jpg|*.gif|*.png|*.bmp|*.wmf|*.emf|*.tiff|*.jpeg"
                )
            );
        }

        protected override string ActionButtonName
        {
            get { return Res.Strings.Dialog.OpenImage.ActionButtonName; }
        }
    }
}
