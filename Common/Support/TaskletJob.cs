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
    /// <summary>
    /// The <c>TaskletJob</c> associates an action with a run mode.
    /// </summary>
    public sealed class TaskletJob
    {
        public TaskletJob(System.Action action, TaskletRunMode runMode)
        {
            this.action = action;
            this.runMode = runMode;
        }

        public TaskletJob(IIsDisposed owner, System.Action action, TaskletRunMode runMode)
            : this(action, runMode)
        {
            this.Owner = owner;
        }

        public TaskletRunMode RunMode
        {
            get { return this.runMode; }
        }

        public bool IsBefore
        {
            get
            {
                return this.RunMode == TaskletRunMode.Before
                    || this.RunMode == TaskletRunMode.BeforeAndAfter;
            }
        }

        public bool IsAsync
        {
            get { return this.RunMode == TaskletRunMode.Async; }
        }

        public bool IsAfter
        {
            get
            {
                return this.RunMode == TaskletRunMode.After
                    || this.RunMode == TaskletRunMode.BeforeAndAfter;
            }
        }

        public System.Action Action
        {
            get { return this.action; }
        }

        public IIsDisposed Owner { get; set; }

        private readonly System.Action action;
        private readonly TaskletRunMode runMode;
    }
}
