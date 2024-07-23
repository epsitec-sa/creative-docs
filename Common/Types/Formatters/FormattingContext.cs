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


namespace Epsitec.Common.Types.Formatters
{
    /// <summary>
    /// The <c>FormattingContext</c> class is tightly coupled with the <see cref="FormatterHelper"/>.
    /// It stores temporary information about the current formatting operation.
    /// </summary>
    public sealed class FormattingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattingContext"/> class, in
        /// the context of a number generation.
        /// </summary>
        /// <param name="idFunc">The function which will return the ID, if property ID is accessed.</param>
        public FormattingContext(System.Func<long> idFunc)
        {
            this.idFunc = idFunc;
            this.data = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormattingContext"/> class, in
        /// the context of a text formatter.
        /// </summary>
        /// <param name="data">The associated data.</param>
        public FormattingContext(object data)
        {
            this.idFunc = null;
            this.data = data;
        }

        /// <summary>
        /// Gets the id which will have to be pretty printed. This is only possible in
        /// the context of a number generation.
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public long Id
        {
            get
            {
                if (this.idFunc == null)
                {
                    throw new System.InvalidOperationException(
                        "Cannot generate ID without an idFunc"
                    );
                }

                if (this.id == null)
                {
                    this.id = this.idFunc();
                }

                return this.id.Value;
            }
        }

        /// <summary>
        /// Gets or sets the arguments for the formatting function, if any.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public string Args
        {
            get { return this.args; }
        }

        /// <summary>
        /// Gets the associated data. This is only meaningful in the context of a text
        /// formatter.
        /// </summary>
        public object Data
        {
            get { return this.data; }
        }

        /// <summary>
        /// Defines the formatting arguments.
        /// </summary>
        /// <param name="args">The formatting arguments.</param>
        public void DefineArgs(string args)
        {
            this.args = args;
        }

        private readonly System.Func<long> idFunc;
        private readonly object data;
        private long? id;
        private string args;
    }
}
