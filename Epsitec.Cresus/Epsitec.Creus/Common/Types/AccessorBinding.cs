//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>AccessorBinding</c> class encapsulates a property setter for a given
	/// property. Accessor bindings are considered to be equal if they apply to the
	/// same property (based on its name).
	/// </summary>
	public sealed class AccessorBinding : System.IEquatable<AccessorBinding>
	{
		private AccessorBinding(string name, System.Action action)
		{
			this.name   = name;
			this.action = action;
		}

		public void Execute()
		{
			this.action ();
		}

		/// <summary>
		/// Creates an accessor binding for the specified property.
		/// </summary>
		/// <typeparam name="TResult">The return type of the accessor.</typeparam>
		/// <param name="accessor">The accessor (used to provide the source data).</param>
		/// <param name="getterExpression">The property getter expression (used to retrieve the name of the property which will be set).</param>
		/// <param name="setter">The property setter action.</param>
		/// <returns>The accessor binding.</returns>
		public static AccessorBinding Create<TResult>(Accessor<TResult> accessor, Expression<System.Func<FormattedText>> getterExpression, System.Action<TResult> setter)
		{
			var propertyInfo = ExpressionAnalyzer.GetLambdaPropertyInfo (getterExpression);

			if (propertyInfo == null)
			{
				throw new System.ArgumentException (string.Format ("The expression {0} does not map to a property getter", getterExpression), "getterExpression");
			}

			return new AccessorBinding (propertyInfo.Name, () => setter (accessor.ExecuteGetter ()));
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

		private readonly string					name;
		private readonly System.Action			action;
	}
}
