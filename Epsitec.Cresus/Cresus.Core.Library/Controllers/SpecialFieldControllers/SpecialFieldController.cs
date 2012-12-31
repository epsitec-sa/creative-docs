using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Epsitec.Cresus.Core.Controllers.SpecialFieldControllers
{
	public abstract class SpecialFieldController
	{
		protected SpecialFieldController(BusinessContext businessContext)
		{
			this.businessContext = businessContext;
		}

		public BusinessContext BusinessContext
		{
			get
			{
				return this.businessContext;
			}
		}

		public MethodInfo GetWebMethod(string name)
		{
			return this.GetWebMethods ().First (m => m.Name == name);
		}

		public IEnumerable<MethodInfo> GetWebMethods()
		{
			var flags = BindingFlags.Instance | BindingFlags.Public;
			var attributeType = typeof (SpecialFieldWebMethodAttribute);

			return from type in this.GetType ().GetBaseTypes ()
				   from method in type.GetMethods (flags)
				   where method.GetCustomAttributes (attributeType, true).Any ()
				   select method;
		}

		public static SpecialFieldController Create(Type controllerType, BusinessContext businessContext, AbstractEntity entity)
		{
			var types = new Type[0];
			var method = SpecialFieldController.GetMethod (controllerType, types);
			var lambda = (LambdaExpression) method.Invoke (null, new object[0]);

			return SpecialFieldController.Create (controllerType, businessContext, entity, lambda);
		}

		public static SpecialFieldController Create(Type controllerType, BusinessContext businessContext, AbstractEntity entity, object value)
		{
			var types = new Type[] { typeof (object) };
			var method = SpecialFieldController.GetMethod (controllerType, types);
			var lambda = (LambdaExpression) method.Invoke (null, new object[] { value });

			return SpecialFieldController.Create (controllerType, businessContext, entity, lambda);
		}

		public static SpecialFieldController Create(Type controllerType, BusinessContext businessContext, AbstractEntity entity, LambdaExpression lambda)
		{
			var arguments = new object[]
			{
				businessContext,
				entity,
				lambda,
			};

			return (SpecialFieldController) Activator.CreateInstance (controllerType, arguments);
		}

		private static MethodInfo GetMethod(Type type, Type[] types)
		{
			MethodInfo result;

			var flags = BindingFlags.NonPublic | BindingFlags.Static;
			var callingConventions = CallingConventions.Standard;

			do
			{
				result = type.GetMethod ("CreateLambda", flags, null, callingConventions, types, null);
				type = type.BaseType;
			}
			while (type != null && result == null);

			return result;
		}

		public abstract Widget GetDesktopField();

		public abstract string GetWebFieldName();
		
		private readonly BusinessContext businessContext;
	}
}
