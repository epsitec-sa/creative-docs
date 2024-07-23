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


using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets.Behaviors
{
    public abstract class SlimFieldBehavior
    {
        protected SlimFieldBehavior(SlimField host)
        {
            this.host = host;
            this.host.UpdatePreferredSize();
        }

        protected virtual void OnTextEditionStarting(CancelEventArgs e)
        {
            this.TextEditionStarting.Raise(this, e);
        }

        protected virtual void OnTextEditionStarted()
        {
            this.TextEditionStarted.Raise(this);
        }

        protected virtual void OnTextEditionEnded()
        {
            this.TextEditionEnded.Raise(this);
        }

        public event EventHandler<CancelEventArgs> TextEditionStarting;
        public event EventHandler TextEditionStarted;
        public event EventHandler TextEditionEnded;

        protected readonly SlimField host;
    }
}
