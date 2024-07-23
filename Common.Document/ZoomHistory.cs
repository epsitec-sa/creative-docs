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

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
    /// <summary>
    /// La classe ZoomHistory permet de mémoriser l'historique des zooms.
    /// </summary>
    public class ZoomHistory
    {
        public class ZoomElement
        {
            public double Zoom;
            public Point Center;
        }

        public ZoomHistory() { }

        public void Clear()
        {
            this.list.Clear();
        }

        public int Count
        {
            get { return list.Count; }
        }

        public void Add(ZoomElement item)
        {
            //	Ajoute un élément à la fin de la liste.
            int total = this.list.Count;
            if (total > 0)
            {
                ZoomElement last = this.list[total - 1] as ZoomElement;
                if (ZoomHistory.IsNearlyEqual(last, item))
                {
                    return;
                }
            }
            this.list.Add(item);
        }

        public ZoomElement Remove()
        {
            //	Enlève et retourne le dernier élément de la liste.
            //	Retourne null s'il n'y en a plus.
            int total = this.list.Count;
            if (total == 0)
            {
                return null;
            }

            ZoomElement item = this.list[total - 1] as ZoomElement;
            this.list.RemoveAt(total - 1);
            return item;
        }

        public static bool IsNearlyEqual(ZoomElement a, ZoomElement b)
        {
            //	Retourne true si deux éléments sont presque égaux.
            if (System.Math.Abs(a.Zoom - b.Zoom) > 0.001)
            {
                return false;
            }

            if (System.Math.Abs(a.Center.X - b.Center.X) > 0.01)
            {
                return false;
            }

            if (System.Math.Abs(a.Center.Y - b.Center.Y) > 0.01)
            {
                return false;
            }

            return true;
        }

        protected System.Collections.ArrayList list = new System.Collections.ArrayList();
    }
}
