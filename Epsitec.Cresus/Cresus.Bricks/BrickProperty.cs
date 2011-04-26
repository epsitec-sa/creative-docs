//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	public struct BrickProperty
	{
		public BrickProperty(BrickPropertyKey key)
			: this (key, "")
		{
		}

		public BrickProperty(BrickPropertyKey key, string value)
		{
			this.key = key;
			this.value = value;
		}

		public BrickProperty(BrickPropertyKey key, FormattedText value)
		{
			this.key = key;
			this.value = value.IsNull ? null : value.ToString ();
		}

		public BrickProperty(BrickPropertyKey key, int value)
		{
			this.key = key;
			this.value = value;
		}

		public BrickProperty(BrickPropertyKey key, Brick value)
		{
			this.key = key;
			this.value = value;
		}

		public BrickProperty(BrickPropertyKey key, Expression expression)
		{
			this.key = key;
			this.value = expression;
		}

		public BrickProperty(BrickPropertyKey key, Mortar mortar)
		{
			this.key = key;
			
			var text = mortar.GetString ();
			var expr = mortar.GetExpression ();

			if (expr != null)
			{
				this.value = expr;
			}
			else
			{
				this.value = text;
			}
		}

		public BrickProperty(BrickPropertyKey key, AttributeValue value)
		{
			this.key = key;
			this.value = value;
		}


		public BrickPropertyKey					Key
		{
			get
			{
				return this.key;
			}
		}
		
		public Brick							Brick
		{
			get
			{
				return this.value as Brick;
			}
		}

		public string							StringValue
		{
			get
			{
				return this.value as string;
			}
		}

		public int?								IntValue
		{
			get
			{
				if (this.value is int)
				{
					return (int) this.value;
				}
				else
				{
					return null;
				}
			}
		}

		public AttributeValue					AttributeValue
		{
			get
			{
				return this.value as AttributeValue;
			}
		}

		public Expression						ExpressionValue
		{
			get
			{
				return this.value as Expression;
			}
		}

		
		public System.Func<T, FormattedText> GetFormatter<T>()
		{
			var value = this.ExpressionValue as LambdaExpression;

			if (value != null)
			{
				Expression<System.Func<T, FormattedText>> expression = value as Expression<System.Func<T, FormattedText>>;

				if (expression != null)
				{
					//	OK, expression is already what we need.
				}
				else
				{
					expression = Expression.Lambda<System.Func<T, FormattedText>> (value.Body, value.Parameters);
				}
				
				return expression.Compile ();
			}
			else
			{
				return null;
			}
		}
		
		public override string ToString()
		{
			return string.Format ("{0} = {1}", this.key, this.value ?? "<null>");
		}

		private readonly BrickPropertyKey		key;
		private readonly object					value;
	}
}