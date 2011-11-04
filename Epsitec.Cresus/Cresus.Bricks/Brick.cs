//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Bricks
{
	/// <summary>
	/// The <c>Brick</c> class is the base class of the <see cref="Brick&lt;T&gt;"/> class,
	/// which provides the mechanisms required to transform bricks into a real UI.
	/// </summary>
	public abstract class Brick
	{
		protected Brick()
		{
			this.properties = new List<BrickProperty> ();
		}


		internal BrickWall						BrickWall
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

		public LambdaExpression GetLambda()
		{
			return this.resolver as LambdaExpression;
		}
		
		
		internal void AddProperty(BrickProperty property, bool notify = true)
		{
			this.properties.Add (property);

			if ((this.brickWall != null) &&
				(notify))
			{
				this.brickWall.NotifyBrickPropertyAdded (this, property);
			}
		}

		internal void InheritResolver(Brick brick)
		{
			this.resolver = brick.resolver;
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

		public static IEnumerable<BrickProperty> GetAllProperties(Brick brick)
		{
			return brick.properties;
		}



		private readonly List<BrickProperty>	properties;
		private BrickWall						brickWall;
		private Expression						resolver;
	}
}