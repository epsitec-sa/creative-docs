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


namespace Epsitec.Common.Widgets.Behaviors
{
    /// <summary>
    /// La classe SelectItemBehavior gère la sélection automatique d'éléments
    /// dans une liste lors de frappe de texte (par ex. "A" --> sélectionne le
    /// premier élément commençant par "A", puis "b" --> sélectionne le premier
    /// élément commençant par "Ab", etc.).
    /// </summary>
    public sealed class SelectItemBehavior
    {
        public SelectItemBehavior(System.Action<string, bool> callback)
        {
            this.TimeoutMilliseconds = 500;

            this.callback = callback;
            this.text = "";
        }

        public int TimeoutMilliseconds { get; set; }

        public bool ProcessKeyPress(Message message)
        {
            if (message.KeyChar >= 32)
            {
                char key = (char)message.KeyChar;
                long now = Types.Time.Now.Ticks;

                long delta = now - this.lastEventTicks;
                long timeout = Types.Time.TicksPerSecond * this.TimeoutMilliseconds / 1000;

                if ((delta > timeout) || (this.text.Length == 0))
                {
                    this.ResetSearch(key.ToString());
                }
                else
                {
                    this.ExpandSearch(key.ToString());
                }

                this.lastEventTicks = now;

                return true;
            }
            else
            {
                this.ClearSearch();

                return false;
            }
        }

        public void ClearSearch()
        {
            this.text = "";
        }

        private void ResetSearch(string value)
        {
            this.text = value;
            this.Search(false);
        }

        private void ExpandSearch(string value)
        {
            this.text = string.Concat(this.text, value);
            this.Search(true);
        }

        private void Search(bool continued)
        {
            if (this.callback != null)
            {
                this.callback(this.text, continued);
            }
        }

        private readonly System.Action<string, bool> callback;
        private string text;
        private long lastEventTicks;
    }
}
