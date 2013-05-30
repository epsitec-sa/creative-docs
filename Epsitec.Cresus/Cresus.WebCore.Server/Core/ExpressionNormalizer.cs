using System.Collections.Generic;
using System.Linq.Expressions;

namespace Epsitec.Cresus.WebCore.Server.Core
{
	public sealed class ExpressionNormalizer : ExpressionVisitor
	{
		private ExpressionNormalizer()
		{
			this.mapping = new Dictionary<string, string> ();
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			var newName = this.GetParameterName (node);

			return Expression.Parameter (node.Type, newName);
		}

		private string GetParameterName(ParameterExpression node)
		{
			var oldName = node.Name;

			string newName;

			if (!this.mapping.TryGetValue (oldName, out newName))
			{
				newName = "x" + mapping.Count;

				this.mapping[oldName] = newName;
			}

			return newName;
		}

		/// <summary>
		/// This method takes an expression and normalizes it so that it would become similar to
		/// another one with the same structure.
		/// </summary>
		/// <remarks>
		/// For now, this method only normalizes the parameter names. If we wanted more advanced
		/// normalization, we could get insipration from this article:
		/// http://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries
		/// Interrestingly, this article does not normalizes the parameter names.
		/// </remarks>
		public static Expression Normalize(Expression expression)
		{
			var normalizer = new ExpressionNormalizer ();

			return normalizer.Visit (expression);
		}

		private readonly Dictionary<string, string> mapping;
	}
}
