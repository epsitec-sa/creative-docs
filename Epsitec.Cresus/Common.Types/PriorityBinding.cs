//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using System.Globalization;

namespace Epsitec.Common.Types
{
//-	[SerializationConverter (typeof (PriorityBinding.SerializationConverter))]
	public class PriorityBinding : AbstractBinding
	{
		public PriorityBinding()
		{
			this.bindings = new Collections.HostedList<Binding> (this.HandleBindingInsertion, this.HandleBindingRemoval);
		}

		public ICollection<Binding> Bindings
		{
			get
			{
				return this.bindings;
			}
		}

		private void HandleBindingInsertion(Binding binding)
		{
		}

		private void HandleBindingRemoval(Binding binding)
		{
		}

		protected override void AddExpression(BindingExpression expression)
		{
			if (this.expressions == null)
			{
				this.expressions = new List<Weak<BindingExpression>> ();
			}

			this.expressions.Add (new Weak<BindingExpression> (expression));
		}

		protected override void RemoveExpression(BindingExpression expression)
		{
			if (this.expressions != null)
			{
				this.expressions.RemoveAll (delegate (Weak<BindingExpression> item) { return !item.IsAlive || item.Target == expression; });

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
				List<BindingExpression> list =  new List<BindingExpression> ();

				this.expressions.RemoveAll
				(
					delegate (Weak<BindingExpression> item)
					{
						BindingExpression expression = item.Target;
						
						if (expression == null)
						{
							return true;
						}
						else
						{
							list.Add (expression);
							return false;
						}
					}
				);

				return list;
			}
		}

		protected override void AttachAfterChanges()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		protected override void DetachBeforeChanges()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		private Collections.HostedList<Binding> bindings;
		private List<Weak<BindingExpression>> expressions;
	}
}
