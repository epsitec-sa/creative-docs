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


namespace Epsitec.Common.Text
{
    /// <summary>
    /// La classe ParagraphManager implémente l'ossature de base de toute les
    /// classes d'implémentation de IParagraphManager.
    /// </summary>
    public abstract class ParagraphManager : IParagraphManager
    {
        protected ParagraphManager() { }

        public virtual string Name
        {
            get
            {
                string name = this.GetType().Name;
                string manager = "Manager";

                if (name.EndsWith(manager))
                {
                    return name.Substring(0, name.Length - manager.Length);
                }

                return name;
            }
        }

        public abstract void AttachToParagraph(
            TextStory story,
            ICursor cursor,
            Properties.ManagedParagraphProperty property
        );
        public abstract void DetachFromParagraph(
            TextStory story,
            ICursor cursor,
            Properties.ManagedParagraphProperty property
        );
        public abstract void RefreshParagraph(
            TextStory story,
            ICursor cursor,
            Properties.ManagedParagraphProperty property
        );
    }
}
