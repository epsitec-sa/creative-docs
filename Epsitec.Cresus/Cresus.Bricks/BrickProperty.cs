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

		public Expression						ExpressionValue
		{
			get
			{
				return this.value as Expression;
			}
		}

		public System.Func<T, FormattedText> GetFormatter<T>()
		{
			var value = this.ExpressionValue;

			if ((value != null) &&
				(value is Expression<System.Func<T, FormattedText>>))
			{
				var expression = value as Expression<System.Func<T, FormattedText>>;
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

		private readonly BrickPropertyKey key;
		private readonly object value;
	}
}
