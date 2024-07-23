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

namespace Epsitec.Common.Types
{
    //-	[SerializationConverter (typeof (PriorityBinding.SerializationConverter))]
    public class PriorityBinding : AbstractBinding
    {
        public PriorityBinding()
        {
            this.bindings = new Collections.HostedList<Binding>(
                this.HandleBindingInsertion,
                this.HandleBindingRemoval
            );
        }

        public ICollection<Binding> Bindings
        {
            get { return this.bindings; }
        }

        private void HandleBindingInsertion(Binding binding) { }

        private void HandleBindingRemoval(Binding binding) { }

        protected override void AddExpression(BindingExpression expression)
        {
            if (this.expressions == null)
            {
                this.expressions = new List<Weak<BindingExpression>>();
            }

            this.expressions.Add(new Weak<BindingExpression>(expression));
        }

        protected override void RemoveExpression(BindingExpression expression)
        {
            if (this.expressions != null)
            {
                this.expressions.RemoveAll(
                    delegate(Weak<BindingExpression> item)
                    {
                        return !item.IsAlive || item.Target == expression;
                    }
                );

                if (this.expressions.Count == 0)
                {
                    this.expressions = null;
                }
            }
        }

        protected override IEnumerable<BindingExpression> GetExpressions()
        {
            if (this.expressions == null)
            {
                return new BindingExpression[0];
            }
            else
            {
                List<BindingExpression> list = new List<BindingExpression>();

                this.expressions.RemoveAll(
                    delegate(Weak<BindingExpression> item)
                    {
                        BindingExpression expression = item.Target;

                        if (expression == null)
                        {
                            return true;
                        }
                        else
                        {
                            list.Add(expression);
                            return false;
                        }
                    }
                );

                return list;
            }
        }

        protected override void AttachAfterChanges()
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        protected override void DetachBeforeChanges()
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        private Collections.HostedList<Binding> bindings;
        private List<Weak<BindingExpression>> expressions;
    }
}
