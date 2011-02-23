//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
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
		/// <typeparam name="T">The return type of the accessor.</typeparam>
		/// <param name="accessor">The accessor.</param>
		/// <param name="getterExpression">The property getter.</param>
		/// <param name="setter">The property setter.</param>
		/// <returns>The accessor binding.</returns>
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

		private readonly string					name;
		private readonly System.Action			action;
	}
}
