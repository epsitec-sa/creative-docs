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


using System.Collections.Generic;

namespace Epsitec.Common.IO
{
    /// <summary>
    /// The <c>StringLineTable</c> class can be used to transform a stream of lines into a
    /// table consisting of a header followed by a stream of rows. This simplifies the logic
    /// to extract the first line and the following lines and does not require multiple
    /// enumerations of the input collection, unless <see cref="Rows"/> gets enumerated
    /// multiple times.
    /// </summary>
    public sealed class StringLineTable
    {
        public StringLineTable(IEnumerable<string> lines)
        {
            this.enumerator = lines.GetEnumerator();

            if (this.enumerator.MoveNext())
            {
                this.header = this.enumerator.Current;
                this.state = State.Ready;
            }
            else
            {
                this.state = State.Empty;
            }
        }

        public string Header
        {
            get { return this.header; }
        }

        public IEnumerable<string> Rows
        {
            get
            {
                switch (this.state)
                {
                    case State.Enumerating:
                        throw new System.InvalidOperationException(
                            "Cannot enumerate several times concurrently"
                        );

                    case State.Empty:
                        yield break;

                    case State.Done:
                        this.enumerator.Reset();
                        this.enumerator.MoveNext();
                        break;
                }

                this.state = State.Enumerating;

                while (true)
                {
                    if (this.enumerator.MoveNext())
                    {
                        yield return this.enumerator.Current;
                    }
                    else
                    {
                        this.state = State.Done;
                        break;
                    }
                }
            }
        }

        private enum State
        {
            Ready,
            Empty,
            Enumerating,
            Done,
        }

        private readonly IEnumerator<string> enumerator;
        private readonly string header;
        private State state;
    }
}
