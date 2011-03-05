//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Epsitec.Cresus.Bricks
{
	public abstract class Brick
	{
		protected Brick()
		{
			this.properties = new List<BrickProperty> ();
		}


		internal BrickWall BrickWall
		{
			get
			{
				return this.brickWall;
			}
		}

		public abstract System.Type GetFieldType();

		public System.Delegate GetResolver(System.Type expectedReturnType)
		{
			var lambda = this.resolver as LambdaExpression;

			if (lambda == null)
			{
				return null;
			}

#if false
			var templateType     = typeof (Zzz<,,>);
			var templateTypeArg1 = lambda.Parameters[0].Type;
			var templateTypeArg2 = expectedReturnType;
			var templateTypeArg3 = lambda.ReturnType;
			var constructedTemplateType = templateType.MakeGenericType (templateTypeArg1, templateTypeArg2, templateTypeArg3);

			var factory = System.Activator.CreateInstance (constructedTemplateType);

			var result  = constructedTemplateType.InvokeMember ("CreateFunction",
				BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
				null,
				factory,
				new object[] { lambda });

			return result as System.Delegate;
#else
			return lambda.Compile ();
#endif
		}

#if false
		class Zzz<T, TResult, TReal>
			where TReal : TResult
		{
			public Zzz()
			{
			}
			public System.Func<T, IList<TResult>> CreateFunction(LambdaExpression expression)
			{
				var source = Expression.Lambda<System.Func<T, IList<TReal>>> (expression.Body, expression.Parameters).Compile ();
				System.Func<T, IList<TResult>> output = x => source (x);
				return output;
			}
		}
#endif

		internal void AddProperty(BrickProperty property, bool notify = true)
		{
			this.properties.Add (property);

			if ((this.brickWall != null) &&
				(notify))
			{
				this.brickWall.NotifyBrickPropertyAdded (this, property);
			}
		}

		internal void DefineResolver(Expression resolver)
		{
			this.resolver = resolver;
		}

		internal void DefineBrickWall(BrickWall brickWall)
		{
			this.brickWall = brickWall;
		}

		internal void DebugDump(string prefix = "")
		{
			foreach (var property in this.properties)
			{
				if (property.Brick == null)
				{
					System.Diagnostics.Debug.WriteLine (prefix + property.ToString ());
				}
				else
				{
					property.Brick.DebugDump (prefix + property.Key.ToString () + ".");
				}
			}
		}


		public static void AddProperty(Brick brick, BrickProperty property)
		{
			brick.AddProperty (property, notify: false);
		}

		public static bool ContainsProperty(Brick brick, BrickPropertyKey key)
		{
			return brick.properties.Any (x => x.Key == key);
		}

		public static BrickProperty GetProperty(Brick brick, BrickPropertyKey key)
		{
			return brick.properties.FindAll (x => x.Key == key).LastOrDefault ();
		}

		public static BrickPropertyCollection GetProperties(Brick brick, params BrickPropertyKey[] keys)
		{
			return new BrickPropertyCollection (brick.properties, keys ?? new BrickPropertyKey[0]);
		}



		private readonly List<BrickProperty> properties;
		private BrickWall brickWall;
		private Expression resolver;
	}

	public sealed class BrickPropertyCollection : IEnumerable<BrickProperty>
	{
		internal BrickPropertyCollection(IList<BrickProperty> properties, BrickPropertyKey[] filter)
		{
			this.properties = properties;
			this.filter = new HashSet<BrickPropertyKey> (filter);
			this.index = -1;
		}

		public BrickProperty? PeekAfter(BrickPropertyKey adjacentPropertyKey)
		{
			int index = this.index + 1;

			while ((index > 0) && (index < this.properties.Count))
			{
				var property = this.properties[index++];

				if (property.Key == adjacentPropertyKey)
				{
					return property;
				}

				if (this.filter.Contains (property.Key))
				{
					break;
				}
			}

			return null;
		}


		public BrickProperty? PeekBefore(BrickPropertyKey adjacentPropertyKey)
		{
			int index = this.index;

			while ((index > 0) && (index <= this.properties.Count))
			{
				var property = this.properties[--index];

				if (property.Key == adjacentPropertyKey)
				{
					return property;
				}
				
				if (this.filter.Contains (property.Key))
				{
					break;
				}
			}

			return null;
		}

		private IEnumerable<BrickProperty> GetProperties()
		{
			while (this.index < this.properties.Count)
			{
				var property = this.properties[this.index++];
				
				if ((this.filter.Count == 0) ||
					(this.filter.Contains (property.Key)))
				{
					yield return property;
				}
			}
			
			this.index = -1;
		}

		#region IEnumerable<BrickProperty> Members

		public IEnumerator<BrickProperty> GetEnumerator()
		{
			this.index = 0;
			return this.GetProperties ().GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion

		private readonly IList<BrickProperty> properties;
		private readonly HashSet<BrickPropertyKey> filter;
		private int index;
	}
}