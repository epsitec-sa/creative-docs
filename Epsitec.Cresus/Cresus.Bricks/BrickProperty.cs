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
			this.isDefaultProperty = false;
		}

		public BrickProperty(BrickPropertyKey key, FormattedText value)
		{
			this.key = key;
			this.value = value.IsNull ? null : value.ToString ();
			this.isDefaultProperty = false;
		}

		public BrickProperty(BrickPropertyKey key, int value)
		{
			this.key = key;
			this.value = value;
			this.isDefaultProperty = false;
		}

		public BrickProperty(BrickPropertyKey key, System.Collections.IEnumerable collection)
		{
			this.key = key;
			this.value = collection;
			this.isDefaultProperty = false;
		}

		public BrickProperty(BrickPropertyKey key, Brick value)
		{
			this.key = key;
			this.value = value;
			this.isDefaultProperty = false;
		}

		public BrickProperty(BrickPropertyKey key, Expression expression)
		{
			this.key = key;
			this.value = expression;
			this.isDefaultProperty = false;
		}

		public BrickProperty(BrickPropertyKey key, Mortar mortar)
		{
			this.key = key;
			this.isDefaultProperty = false;
			
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
			this.isDefaultProperty = false;
		}

		private BrickProperty(BrickProperty property, bool isDefaultProperty)
		{
			this.key = property.key;
			this.value = property.value;
			this.isDefaultProperty = isDefaultProperty;
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

		public System.Collections.IEnumerable	CollectionValue
		{
			get
			{
				return this.value as System.Collections.IEnumerable;
			}
		}

		public bool								IsDefaultProperty
		{
			get
			{
				return this.isDefaultProperty;
			}
		}
		
		public override string ToString()
		{
			return string.Format ("{0} = {1}", this.key, this.value ?? "<null>");
		}

		public BrickProperty MarkAsDefaultProperty()
		{
			return new BrickProperty (this, true);
		}

		private readonly BrickPropertyKey		key;
		private readonly object					value;
		private readonly bool					isDefaultProperty;
	}
}