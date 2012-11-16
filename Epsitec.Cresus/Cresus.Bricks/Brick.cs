//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

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
		protected Brick(BrickWall brickWall, Expression resolver)
		{
			this.properties = new List<BrickProperty> ();
			this.brickWall = brickWall;
			this.resolver = resolver;
		}


		internal BrickWall						BrickWall
		{
			get
			{
				return this.brickWall;
			}
		}

		internal Expression						Resolver
		{
			get
			{
				return this.resolver;
			}
		}


		public abstract System.Type GetBrickType();


		public System.Delegate GetResolver(System.Type expectedReturnType)
		{
			var lambda = this.GetLambda ();

			if (lambda == null)
			{
				return null;
			}

			return lambda.Compile ();
		}

		public System.Delegate CreateResolverSetter(System.Type expectedTargetType)
		{
			var lambda = this.GetLambda ();

			if (lambda == null)
			{
				return null;
			}
			
			lambda = ExpressionAnalyzer.CreateSetter (lambda);

			return lambda.Compile ();
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


		protected static T AddProperty<T>(T brick, BrickProperty brickProperty)
			where T : Brick
		{
			brick.AddProperty(brickProperty);
			return brick;
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
		private readonly BrickWall				brickWall;
		private readonly Expression				resolver;
	}
}