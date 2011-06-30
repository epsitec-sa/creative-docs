//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Epsitec.Cresus.Core.Bricks.Helpers
{
	using TypeAndMethodName = System.Tuple<System.Type, string>;

	/// <summary>
	/// The <c>Invoker</c> class is used to (quickly) invoke a parameterless method returning
	/// a <see cref="FormattedText"/>, if it is supported by the target type.
	/// </summary>
	internal sealed class Invoker
	{
		private Invoker(System.Type targetType, string methodName)
		{
			this.targetType = targetType;
			this.methodName = methodName;
			this.methodInfo = this.targetType.GetMethod (this.methodName);
		}

		
		public bool								IsValid
		{
			get
			{
				return this.methodInfo != null;
			}
		}

		
		public void Bind(Brick brick, BrickPropertyKey property)
		{
			Expression<System.Func<AbstractEntity, FormattedText>> expression = x => this.Invoke (x);
			Brick.AddProperty (brick, new BrickProperty (property, expression));
		}
		
		public static Invoker GetInvoker(System.Type type, string methodName)
		{
			lock (Invoker.exclusion)
			{
				var     key = new TypeAndMethodName (type, methodName);
				Invoker invoker;

				if (Invoker.invokers.TryGetValue (key, out invoker))
				{
					return invoker;
				}
				else
				{
					invoker = Invoker.invokers[key] = new Invoker (type, methodName);
					
					return invoker;
				}
			}
		}

		
		private FormattedText Invoke(AbstractEntity entity)
		{
			return (FormattedText) this.methodInfo.Invoke (entity, Invoker.EmptyArgs);
		}

		#region InvokerCache Class

		private class InvokerCache : Dictionary<TypeAndMethodName, Invoker>
		{
		}

		#endregion

		private static readonly object			exclusion = new object ();
		private static readonly object[]		EmptyArgs = new object[0];
		private static readonly InvokerCache	invokers  = new InvokerCache ();

		private readonly System.Type			targetType;
		private readonly string					methodName;
		private readonly MethodInfo				methodInfo;
	}
}
