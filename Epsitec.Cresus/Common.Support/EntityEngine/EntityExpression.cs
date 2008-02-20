//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityExpression</c> class abstracts the representation of an
	/// expression (calculation, lambda function, etc.) as stored in entity
	/// fields.
	/// </summary>
	public sealed class EntityExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityExpression"/> class.
		/// </summary>
		public EntityExpression()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityExpression"/> class.
		/// </summary>
		/// <param name="encodedExpression">The encoded expression.</param>
		private EntityExpression(EncodedExpression encodedExpression)
		{
			this.encodedExpression = encodedExpression;
		}


		/// <summary>
		/// Gets the encoding for the expression.
		/// </summary>
		/// <value>The encoding.</value>
		public EntityExpressionEncoding Encoding
		{
			get
			{
				return this.encodedExpression.Encoding;
			}
		}

		/// <summary>
		/// Gets the source code for the expression.
		/// </summary>
		/// <value>The source code.</value>
		public string SourceCode
		{
			get
			{
				if (this.sourceCode == null)
				{
					this.UpdateSourceCode ();
				}

				return this.sourceCode;
			}
		}


		/// <summary>
		/// Creates an expression based on an encoded <c>string</c>.
		/// </summary>
		/// <param name="encodedExpression">The encoded expression.</param>
		/// <returns>The expression.</returns>
		public static EntityExpression FromEncodedExpression(string encodedExpression)
		{
			return new EntityExpression (new EncodedExpression (encodedExpression));
		}

		/// <summary>
		/// Creates an expression based on an encoding and source code.
		/// </summary>
		/// <param name="encoding">The encoding.</param>
		/// <param name="sourceCode">The source code.</param>
		/// <returns>The expression.</returns>
		public static EntityExpression FromSourceCode(EntityExpressionEncoding encoding, string sourceCode)
		{
			switch (encoding)
			{
				case EntityExpressionEncoding.LambdaCSharpSourceCode:
					break;

				default:
					throw new System.ArgumentException (string.Format ("Invalid encoding '{0}' in this context", encoding));
			}

			return new EntityExpression (new EncodedExpression (encoding, sourceCode));
		}


		/// <summary>
		/// Gets the encoded expression.
		/// </summary>
		/// <returns>The <c>string</c> which represents the encoded expression.</returns>
		public string GetEncodedExpression()
		{
			return this.encodedExpression.ToString ();
		}

		/// <summary>
		/// Updates the cached source code by analyzing the encoded expression.
		/// </summary>
		private void UpdateSourceCode()
		{
			switch (this.encodedExpression.Encoding)
			{
				case EntityExpressionEncoding.LambdaCSharpSourceCode:
					this.sourceCode = this.encodedExpression.Expression;
					break;

				default:
					this.sourceCode = null;
					break;
			}
		}

		#region Private Static Helpers Methods

		static EntityExpression()
		{
			EntityExpression.stringToEncoding = new Dictionary<string, EntityExpressionEncoding> ();
			EntityExpression.encodingToString = new Dictionary<EntityExpressionEncoding, string> ();
			
			EntityExpression.stringToEncoding["λ/c#"] = EntityExpressionEncoding.LambdaCSharpSourceCode;

			foreach (KeyValuePair<string, EntityExpressionEncoding> pair in EntityExpression.stringToEncoding)
			{
				EntityExpression.encodingToString[pair.Value] = pair.Key;
			}
		}

		private static EntityExpressionEncoding EncodingFromHeader(string value)
		{
			EntityExpressionEncoding encoding;

			if (EntityExpression.stringToEncoding.TryGetValue (value, out encoding))
			{
				return encoding;
			}
			else
			{
				return EntityExpressionEncoding.Invalid;
			}
		}

		private static string HeaderFromEncoding(EntityExpressionEncoding encoding)
		{
			string value;

			if (EntityExpression.encodingToString.TryGetValue (encoding, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}

		#endregion

		#region EncodedExpression Structure

		private struct EncodedExpression : System.IEquatable<EncodedExpression>
		{
			public EncodedExpression(string encodedExpression)
			{
				int pos = encodedExpression == null ? -1 : encodedExpression.IndexOf ('\n');

				if (pos < 0)
				{
					this.encoding   = EntityExpression.EncodingFromHeader (encodedExpression ?? "");
					this.expression = null;
				}
				else
				{
					string header = encodedExpression.Substring (0, pos).TrimEnd ('\r', ' ');
					string source = encodedExpression.Substring (pos + 1);

					this.encoding   = EntityExpression.EncodingFromHeader (header);
					this.expression = source;
				}
			}

			public EncodedExpression(EntityExpressionEncoding encoding, string expression)
			{
				this.encoding   = encoding;
				this.expression = expression;
			}

			public EntityExpressionEncoding Encoding
			{
				get
				{
					return this.encoding;
				}
			}

			public string Expression
			{
				get
				{
					return this.expression;
				}
			}

			#region IEquatable<EncodedExpression> Members

			public bool Equals(EncodedExpression other)
			{
				return (this.encoding == other.encoding)
					&& (this.expression == other.expression);
			}

			#endregion

			public override bool Equals(object obj)
			{
				if (obj is EncodedExpression)
				{
					return this.Equals ((EncodedExpression) obj);
				}
				else
				{
					return false;
				}
			}

			public override int GetHashCode()
			{
				return ((int) this.encoding) ^ (this.expression == null ? 0 : this.expression.GetHashCode ());
			}

			public override string ToString()
			{
				if (this.encoding == EntityExpressionEncoding.Invalid)
				{
					return "?";
				}
				else if (this.expression == null)
				{
					return EntityExpression.HeaderFromEncoding (this.encoding);
				}
				else
				{
					return string.Concat (EntityExpression.HeaderFromEncoding (this.encoding), "\r\n", this.expression);
				}
			}

			
			private EntityExpressionEncoding encoding;
			private string expression;
		}

		#endregion

		private static readonly Dictionary<string, EntityExpressionEncoding> stringToEncoding;
		private static readonly Dictionary<EntityExpressionEncoding, string> encodingToString;

		private EncodedExpression encodedExpression;
		private string sourceCode;
	}
}
