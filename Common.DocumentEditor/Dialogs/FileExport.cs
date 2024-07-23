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

namespace Epsitec.Common.DocumentEditor.Dialogs
{
    /// <summary>
    /// Dialogue pour exporter un fichier.
    /// </summary>
    public class FileExport : AbstractFile
    {
        public FileExport(DocumentEditor editor)
            : base(editor)
        {
            this.owner = this.editor.Window;
            this.enableNavigation = true;
            this.enableMultipleSelection = false;
            this.hasOptions = false;
            this.fileDialogType = Epsitec.Common.Dialogs.FileDialogType.Save;
        }

        protected override string ActionButtonName
        {
            get { return Res.Strings.Dialog.Export.Button.OK; }
        }
    }
}
