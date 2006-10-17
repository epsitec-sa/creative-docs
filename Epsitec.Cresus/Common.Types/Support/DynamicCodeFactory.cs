//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	using OpCodes=System.Reflection.Emit.OpCodes;

	public delegate void PropertySetter(object target, object value);
	public delegate object PropertyGetter(object target);

	public static class DynamicCodeFactory
	{
		public static PropertySetter CreatePropertySetter(System.Type type, string propertyName)
		{
			return DynamicCodeFactory.CreatePropertySetter (type.GetProperty (propertyName));
		}
		
		public static PropertySetter CreatePropertySetter(System.Reflection.PropertyInfo propertyInfo)
		{
			System.Reflection.MethodInfo method = propertyInfo.GetSetMethod (false);

			if ((method == null) ||
				(propertyInfo.CanWrite == false))
			{
				return null;
			}

			System.Type   hostType  = propertyInfo.DeclaringType;
			System.Type   propType  = propertyInfo.PropertyType;
			System.Type[] arguments = new System.Type[2];
			
			arguments[0] = typeof (object);
			arguments[1] = typeof (object);

			string name = string.Concat ("_PropertySetter_", propertyInfo.Name);

			System.Reflection.Emit.DynamicMethod setter;
			setter = new System.Reflection.Emit.DynamicMethod (name, null, arguments, hostType);

			System.Reflection.Emit.ILGenerator generator = setter.GetILGenerator ();
			
			generator.Emit (OpCodes.Ldarg_0);
			generator.Emit (OpCodes.Castclass, propertyInfo.DeclaringType);
			generator.Emit (OpCodes.Ldarg_1);

			if (propType.IsClass)
			{
				generator.Emit (OpCodes.Castclass, propType);
			}
			else if (propType.IsValueType)
			{
				generator.Emit (OpCodes.Unbox_Any, propType);
			}
			else
			{
				throw new System.InvalidOperationException ("Invalid code path");
			}

			generator.EmitCall (OpCodes.Callvirt, method, null);
			generator.Emit (OpCodes.Ret);
			
			return (PropertySetter) setter.CreateDelegate (typeof (PropertySetter));
		}

		public static PropertyGetter CreatePropertyGetter(System.Type type, string propertyName)
		{
			return DynamicCodeFactory.CreatePropertyGetter (type.GetProperty (propertyName));
		}
		
		public static PropertyGetter CreatePropertyGetter(System.Reflection.PropertyInfo propertyInfo)
		{
			System.Reflection.MethodInfo method = propertyInfo.GetGetMethod ();

			if ((method == null) ||
				(propertyInfo.CanRead == false))
			{
				return null;
			}

			System.Type   hostType  = propertyInfo.DeclaringType;
			System.Type   propType  = propertyInfo.PropertyType;
			System.Type[] arguments = new System.Type[1];
			
			arguments[0] = typeof (object);
			
			string name = string.Concat ("_PropertyGetter_", propertyInfo.Name);

			System.Reflection.Emit.DynamicMethod getter;
			getter = new System.Reflection.Emit.DynamicMethod (name, typeof (object), arguments, hostType);

			System.Reflection.Emit.ILGenerator generator = getter.GetILGenerator ();
			
			generator.Emit (OpCodes.Ldarg_0);
			generator.Emit (OpCodes.Castclass, propertyInfo.DeclaringType);
			generator.EmitCall (OpCodes.Callvirt, method, null);

			if (propType.IsClass)
			{
				//	No conversion needed.
			}
			else if (propType.IsValueType)
			{
				generator.Emit (OpCodes.Box, propType);
			}
			else
			{
				throw new System.InvalidOperationException ("Invalid code path");
			}

			generator.Emit (OpCodes.Ret);

			return (PropertyGetter) getter.CreateDelegate (typeof (PropertyGetter));
		}
	}
}
