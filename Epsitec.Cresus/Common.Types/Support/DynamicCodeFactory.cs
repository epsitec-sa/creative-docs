using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace Epsitec.Common.Support
{
	public delegate void GenericSetter(object target, object value);
	public delegate object GenericGetter(object target);

	public static class DynamicCodeFactory
	{
		public static GenericSetter CreateSetMethod(PropertyInfo propertyInfo)
		{
			MethodInfo setMethod = propertyInfo.GetSetMethod ();

			if (setMethod == null)
			{
				return null;
			}

			System.Type[] arguments = new System.Type[2];
			arguments[0] = arguments[1] = typeof (object);

			string name = string.Concat ("_Set", propertyInfo.Name, "_");

			DynamicMethod setter = new DynamicMethod (name, typeof (void), arguments, propertyInfo.DeclaringType);
			ILGenerator generator = setter.GetILGenerator ();
			generator.Emit (OpCodes.Ldarg_0);
			generator.Emit (OpCodes.Castclass, propertyInfo.DeclaringType);
			generator.Emit (OpCodes.Ldarg_1);

			if (propertyInfo.PropertyType.IsClass)
			{
				generator.Emit (OpCodes.Castclass, propertyInfo.PropertyType);
			}
			else
			{
				generator.Emit (OpCodes.Unbox_Any, propertyInfo.PropertyType);
			}

			generator.EmitCall (OpCodes.Callvirt, setMethod, null);
			generator.Emit (OpCodes.Ret);

			/*
			* Create the delegate and return it
			*/
			return (GenericSetter) setter.CreateDelegate (typeof (GenericSetter));
		}

		///
		/// Creates a dynamic getter for the property
		///
		private static GenericGetter CreateGetMethod(PropertyInfo propertyInfo)
		{
			/*
			* If there’s no getter return null
			*/
			MethodInfo getMethod = propertyInfo.GetGetMethod ();
			if (getMethod == null)
				return null;

			/*
			* Create the dynamic method
			*/
			System.Type[] arguments = new System.Type[1];
			arguments[0] = typeof (object);
			string name = string.Concat ("_Get", propertyInfo.Name, "_");
			DynamicMethod getter = new DynamicMethod (
			  name,
			  typeof (object), arguments, propertyInfo.DeclaringType);
			ILGenerator generator = getter.GetILGenerator ();
			generator.DeclareLocal (typeof (object));
			generator.Emit (OpCodes.Ldarg_0);
			generator.Emit (OpCodes.Castclass, propertyInfo.DeclaringType);
			generator.EmitCall (OpCodes.Callvirt, getMethod, null);

			if (!propertyInfo.PropertyType.IsClass)
				generator.Emit (OpCodes.Box, propertyInfo.PropertyType);

			generator.Emit (OpCodes.Ret);

			/*
			* Create the delegate and return it
			*/
			return (GenericGetter) getter.CreateDelegate (typeof (GenericGetter));
		}
	}
}
