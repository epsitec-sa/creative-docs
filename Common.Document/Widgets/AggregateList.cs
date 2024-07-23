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

namespace Epsitec.Common.Document.Widgets
{
    /// <summary>
    /// AggregateList représente la liste des styles graphiques.
    /// </summary>
    public class AggregateList : AbstractStyleList
    {
        public AggregateList()
            : base() { }

        public NewUndoableList List
        {
            //	Liste des aggrégats représentés dans la liste.
            get { return this.list; }
            set { this.list = value; }
        }

        protected override int ListCount
        {
            //	Nombre de lignes de la liste.
            get
            {
                int count = 0;

                if (this.list != null)
                {
                    count = this.list.Count;
                    ;
                }

                if (this.excludeRank != -1)
                {
                    count--;
                }

                if (this.isNoneLine)
                {
                    count++;
                }

                return count;
            }
        }

        protected override string ListName(int rank)
        {
            //	Nom d'une ligne de la liste.
            if (rank == -1 || this.list == null)
            {
                return Res.Strings.Aggregates.NoneLine;
            }
            else
            {
                Properties.Aggregate agg = this.list[rank] as Properties.Aggregate;
                return agg.AggregateName;
            }
        }

        protected override AbstractSample CreateSample()
        {
            //	Crée un échantillon.
            return new Sample();
        }

        protected override void ListSample(AbstractSample sample, int rank)
        {
            //	 Met à jour l'échantillon d'une ligne de la liste.
            Sample sm = sample as Sample;

            if (rank == -1 || this.list == null)
            {
                sm.Aggregate = null;
            }
            else
            {
                sm.Aggregate = this.list[rank] as Properties.Aggregate;
            }

            sm.Invalidate();
        }

        protected NewUndoableList list;
    }
}
