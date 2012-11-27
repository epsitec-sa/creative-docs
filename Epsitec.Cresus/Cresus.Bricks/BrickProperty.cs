//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;

namespace Epsitec.Cresus.Bricks
{
	public struct BrickProperty
	{
		public BrickProperty(BrickPropertyKey key)
			: this (key, "")
		{
		}

		public BrickProperty(BrickPropertyKey key, string value)
			: this (key, value, false)
		{
		}

		public BrickProperty(BrickPropertyKey key, FormattedText value)
			: this (key, value.IsNull () ? null : value.ToString (), false)
		{
		}

		public BrickProperty(BrickPropertyKey key, int value)
			: this (key, value, false)
		{
		}

		public BrickProperty(BrickPropertyKey key, System.Collections.IEnumerable value)
			: this (key, value, false)
		{
		}

		public BrickProperty(BrickPropertyKey key, Brick value)
			: this (key, value, false)
		{
		}

		public BrickProperty(BrickPropertyKey key, Expression value)
			: this (key, value, false)
		{
		}

		public BrickProperty(BrickPropertyKey key, Mortar value)
			: this (key, (object) value.GetExpression () ?? value.GetString (), false)
		{
		}

		public BrickProperty(BrickPropertyKey key, AttributeValue value)
			: this (key, value, false)
		{
		}

		public BrickProperty(BrickPropertyKey key, ExpandoObject value)
			: this (key, value, false)
		{
		}

		public BrickProperty(BrickPropertyKey key, System.Type value)
			: this (key, value, false)
		{
		}

		public BrickProperty(BrickPropertyKey key, object value)
			: this (key, value, false)
		{
		}

		private BrickProperty(BrickProperty property, bool isDefaultProperty)
			: this (property.key, property.value, isDefaultProperty)
		{
		}

		private BrickProperty(BrickPropertyKey key, object value, bool isDefaultProperty)
		{
			this.key = key;
			this.value = value;
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

		public ExpandoObject					ExpandoValue
		{
			get
			{
				return this.value as ExpandoObject;
			}
		}

		public System.Type						TypeValue
		{
			get
			{
				return this.value as System.Type;
			}
		}

		public object							ObjectValue
		{
			get
			{
				return this.value;
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