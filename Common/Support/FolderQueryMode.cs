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


namespace Epsitec.Common.Support
{
    public struct FolderQueryMode
    {
        public FileInfoIconSelection IconSelection
        {
            get { return this.iconSelection; }
            set { this.iconSelection = value; }
        }

        public FileInfoIconSize IconSize
        {
            get { return this.iconSize; }
            set { this.iconSize = value; }
        }

        public bool AsOpenFolder
        {
            get { return this.asOpenFolder; }
            set { this.asOpenFolder = value; }
        }

        public static FolderQueryMode LargeIcons
        {
            get
            {
                FolderQueryMode mode = new FolderQueryMode();

                mode.IconSelection = FileInfoIconSelection.Normal;
                mode.IconSize = FileInfoIconSize.Large;

                return mode;
            }
        }

        public static FolderQueryMode SmallIcons
        {
            get
            {
                FolderQueryMode mode = new FolderQueryMode();

                mode.IconSelection = FileInfoIconSelection.Normal;
                mode.IconSize = FileInfoIconSize.Small;

                return mode;
            }
        }

        public static FolderQueryMode NoIcons
        {
            get
            {
                FolderQueryMode mode = new FolderQueryMode();

                mode.IconSelection = FileInfoIconSelection.Normal;
                mode.IconSize = FileInfoIconSize.None;

                return mode;
            }
        }

        public FolderQueryMode Open()
        {
            this.asOpenFolder = true;
            return this;
        }

        private FileInfoIconSize iconSize;
        private bool asOpenFolder;
        private FileInfoIconSelection iconSelection;
    }
}
