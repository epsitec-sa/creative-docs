using System.Collections.Generic;
using System.Linq.Expressions;

namespace Epsitec.Cresus.WebCore.Server.Core
{
	/// <summary>
	/// This class is used to normalize expressions, so that two similar expression will map to the
	/// same normalized form. If we have (Person p) => p.Address and (Person x) => x.Address, these
	/// expressions are logically the same, even if their text is different. As we use this text
	/// to check if two expressions are the same, we must have a way to normalize them before.
	/// </summary>
	/// <remarks>
	/// For now, this class only normalizes the parameter names. If we wanted more advanced
	/// normalization, we could get insipration from this article:
	/// http://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries
	/// Interrestingly, this article does not normalizes the parameter names.
	/// </remarks>
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
		public static Expression Normalize(Expression expression)
		{
			var normalizer = new ExpressionNormalizer ();

			return normalizer.Visit (expression);
		}

		private readonly Dictionary<string, string> mapping;
	}
}
