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
		public static GenericSetter CreateSetMethod(System.Reflection.PropertyInfo propertyInfo)
		{
			System.Reflection.MethodInfo method = propertyInfo.GetSetMethod ();

			if (method == null)
			{
				return null;
			}

			System.Type[] arguments = new System.Type[2];
			
			arguments[0] = typeof (object);
			arguments[1] = typeof (object);

			string name = string.Concat ("_Set_", propertyInfo.Name, "_");

			System.Reflection.Emit.DynamicMethod setter;
			setter = new System.Reflection.Emit.DynamicMethod (name, typeof (void), arguments, propertyInfo.DeclaringType);

			System.Reflection.Emit.ILGenerator generator = setter.GetILGenerator ();
			
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

			generator.EmitCall (OpCodes.Callvirt, method, null);
			generator.Emit (OpCodes.Ret);
			
			return (GenericSetter) setter.CreateDelegate (typeof (GenericSetter));
		}

		public static GenericGetter CreateGetMethod(System.Type type, string propertyName)
		{
			return DynamicCodeFactory.CreateGetMethod (type.GetProperty (propertyName, BindingFlags.Public));
		}
		
		public static GenericGetter CreateGetMethod(System.Reflection.PropertyInfo propertyInfo)
		{
			System.Reflection.MethodInfo getMethod = propertyInfo.GetGetMethod ();

			if (getMethod == null)
			{
				return null;
			}

			System.Type[] arguments = new System.Type[1];
			
			arguments[0] = typeof (object);
			
			string name = string.Concat ("_Get", propertyInfo.Name, "_");

			System.Reflection.Emit.DynamicMethod getter;
			getter = new System.Reflection.Emit.DynamicMethod (name, typeof (object), arguments, propertyInfo.DeclaringType);

			System.Reflection.Emit.ILGenerator generator = getter.GetILGenerator ();
			
			generator.DeclareLocal (typeof (object));
			generator.Emit (OpCodes.Ldarg_0);
			generator.Emit (OpCodes.Castclass, propertyInfo.DeclaringType);
			generator.EmitCall (OpCodes.Callvirt, getMethod, null);

			if (propertyInfo.PropertyType.IsClass)
			{
				//	No conversion needed.
			}
			else
			{
				generator.Emit (OpCodes.Box, propertyInfo.PropertyType);
			}

			generator.Emit (OpCodes.Ret);

			return (GenericGetter) getter.CreateDelegate (typeof (GenericGetter));
		}
	}
}
