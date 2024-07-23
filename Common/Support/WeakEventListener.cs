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


using Epsitec.Common.Types;

namespace Epsitec.Common.Support
{
    internal struct WeakEventListener : System.IEquatable<WeakEventListener>
    {
        public WeakEventListener(System.Delegate listener)
        {
            this.info = listener.Method;
            this.target = new Weak<object>(listener.Target);
        }

        public bool IsDead
        {
            get { return this.target.IsAlive == false; }
        }

        public bool Equals(System.Delegate listener)
        {
            return listener.Method == this.info && listener.Target == this.target.Target;
        }

        public bool Invoke(params object[] parameters)
        {
            object target = this.target.Target;

            if (target == null)
            {
                return false;
            }
            else
            {
                this.info.Invoke(target, parameters);
                return true;
            }
        }

        #region IEquatable<WeakEventListener> Members

        public bool Equals(WeakEventListener other)
        {
            return this.info == other.info && this.target.Target == other.target.Target;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj is WeakEventListener)
            {
                return this.Equals((WeakEventListener)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.info.GetHashCode();
        }

        public System.Reflection.MethodInfo info;
        public Weak<object> target;
    }
}
