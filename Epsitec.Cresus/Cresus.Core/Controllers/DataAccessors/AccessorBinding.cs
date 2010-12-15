//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	public sealed class AccessorBinding : System.IEquatable<AccessorBinding>
	{
		private AccessorBinding(string name, System.Action action)
		{
			this.name = name;
			this.action = action;
		}

		public void Execute()
		{
			this.action ();
		}

		public static AccessorBinding Create<T>(Accessor<T> accessor, Expression<System.Func<FormattedText>> getterExpression, System.Action<T> setter)
		{
			string name = ExpressionAnalyzer.GetLambdaPropertyInfo (getterExpression).Name;

			return new AccessorBinding (name, () => setter (accessor.ExecuteGetter ()));
		}

		#region IEquatable<AccessorBinding> Members

		public bool Equals(AccessorBinding other)
		{
			if (other == null)
			{
				return false;
			}
			else
			{
				return this.name == other.name;
			}
		}

		#endregion

		public override bool Equals(object obj)
		{
			return base.Equals (obj as AccessorBinding);
		}

		public override int GetHashCode()
		{
			return this.name.GetHashCode ();
		}

		private readonly string name;
		private readonly System.Action action;
	}
}
