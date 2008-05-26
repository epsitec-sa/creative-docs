//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	using OpCodes=System.Reflection.Emit.OpCodes;

	public delegate void PropertySetter(object target, object value);
	public delegate object PropertyGetter(object target);
	
	public delegate T Allocator<T> ();
	public delegate T Allocator<T, P> (P value);

	public delegate int PropertyComparer(object a, object b);

	//	Links:
	//	How to: Examine and Instanciate Generic Types with Reflection
	//	ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.VisualStudio.v80.en/dv_fxadvance/html/f93b03b0-1778-43fc-bc6d-35983d210e74.htm

	/// <summary>
	/// The <c>DynamicCodeFactory</c> class generates dynamic methods used to
	/// access properties, create objects, etc.
	/// </summary>
	public static class DynamicCodeFactory
	{
		/// <summary>
		/// Creates a property setter for the specified property; setters are
		/// cached and reused whenever possible.
		/// </summary>
		/// <param name="type">The type of the object to access.</param>
		/// <param name="propertyName">The name of the property.</param>
		/// <returns>
		/// A <see cref="PropertySetter"/> for the property.
		/// </returns>
		public static PropertySetter CreatePropertySetter(System.Type type, string propertyName)
		{
			PropertySetter propertySetter;

			string key = string.Concat (type.FullName, "/", propertyName);

			if (DynamicCodeFactory.setterCache.TryGetValue (key, out propertySetter))
			{
				return propertySetter;
			}

			propertySetter = DynamicCodeFactory.CreatePropertySetter (type.GetProperty (propertyName));

			DynamicCodeFactory.setterCache[key] = propertySetter;

			return propertySetter;
		}

		/// <summary>
		/// Creates a property setter for the specified property.
		/// </summary>
		/// <param name="propertyInfo">The property info.</param>
		/// <returns>
		/// A <see cref="PropertySetter"/> for the property.
		/// </returns>
		public static PropertySetter CreatePropertySetter(System.Reflection.PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				return null;
			}

			System.Reflection.MethodInfo method = propertyInfo.GetSetMethod (false);

			if ((method == null) ||
				(propertyInfo.CanWrite == false))
			{
				return null;
			}

			System.Type   objType   = typeof (object);
			System.Type   hostType  = propertyInfo.DeclaringType;
			System.Type   propType  = propertyInfo.PropertyType;
			System.Type[] arguments = new System.Type[2];
			
			arguments[0] = objType;
			arguments[1] = objType;

			string name = string.Concat ("DynamicCode_PropertySetter_", propertyInfo.Name);

			System.Reflection.Emit.DynamicMethod setter;
			setter = new System.Reflection.Emit.DynamicMethod (name, null, arguments, hostType);

			System.Reflection.Emit.ILGenerator generator = setter.GetILGenerator ();

			generator.Emit (OpCodes.Ldarg_0);
			
			if (hostType.IsValueType)
			{
				throw new System.InvalidOperationException ("Cannot set property of boxed ValueType");
			}
			
			generator.Emit (OpCodes.Castclass, hostType);
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

		/// <summary>
		/// Creates a property getter for the specified property; getters are
		/// cached and reused whenever possible.
		/// </summary>
		/// <param name="type">The type of the object to access.</param>
		/// <param name="propertyName">The name of the property.</param>
		/// <returns>
		/// A <see cref="PropertyGetter"/> for the property.
		/// </returns>
		public static PropertyGetter CreatePropertyGetter(System.Type type, string propertyName)
		{
			PropertyGetter propertyGetter;

			string key = string.Concat (type.FullName, "/", propertyName);

			if (DynamicCodeFactory.getterCache.TryGetValue (key, out propertyGetter))
			{
				return propertyGetter;
			}
			
			propertyGetter = DynamicCodeFactory.CreatePropertyGetter (type.GetProperty (propertyName));

			DynamicCodeFactory.getterCache[key] = propertyGetter;
			
			return propertyGetter;
		}

		/// <summary>
		/// Creates a property getter for the specified property.
		/// </summary>
		/// <param name="propertyInfo">The property info.</param>
		/// <returns>
		/// A <see cref="PropertyGetter"/> for the property.
		/// </returns>
		public static PropertyGetter CreatePropertyGetter(System.Reflection.PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				return null;
			}

			System.Reflection.MethodInfo method = propertyInfo.GetGetMethod ();

			if ((method == null) ||
				(propertyInfo.CanRead == false))
			{
				return null;
			}

			System.Type   objType   = typeof (object);
			System.Type   hostType  = propertyInfo.DeclaringType;
			System.Type   propType  = propertyInfo.PropertyType;
			System.Type[] arguments = new System.Type[1];
			
			arguments[0] = objType;

			string name = string.Concat ("DynamicCode_PropertyGetter_", propertyInfo.Name);

			System.Reflection.Emit.DynamicMethod getter;
			getter = new System.Reflection.Emit.DynamicMethod (name, objType, arguments, hostType);

			System.Reflection.Emit.ILGenerator generator = getter.GetILGenerator ();
			
			if (hostType.IsValueType)
			{
				System.Reflection.Emit.LocalBuilder local = generator.DeclareLocal (hostType);

				generator.Emit (OpCodes.Ldarg_0);
				generator.Emit (OpCodes.Unbox_Any, hostType);
				generator.Emit (OpCodes.Stloc_S, local);
				generator.Emit (OpCodes.Ldloca_S, local);
				generator.EmitCall (OpCodes.Call, method, null);
			}
			else
			{
				generator.Emit (OpCodes.Ldarg_0);
				generator.Emit (OpCodes.Castclass, hostType);
				generator.EmitCall (OpCodes.Callvirt, method, null);
			}

			if (propType.IsClass || propType.IsInterface)
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

		/// <summary>
		/// Creates an allocator for some type <c>T</c>.
		/// </summary>
		/// <returns>
		/// An <see cref="Allocator&lt;T&gt;"/> for the type.
		/// </returns>
		public static Allocator<T> CreateAllocator<T>()
		{
			return DynamicCodeFactory.CreateAllocator<T> (typeof (T));
		}

		/// <summary>
		/// Creates an allocator for some type.
		/// </summary>
		/// <param name="type">The type of the object which must be allocated.</param>
		/// <returns>
		/// An <see cref="Allocator&lt;T&gt;"/> for the type.
		/// </returns>
		public static Allocator<T> CreateAllocator<T>(System.Type type)
		{
			//	Create a small piece of dynamic code which does simply "new T()"
			//	for the underlying system type. This code relies on lightweight
			//	code generation and results in a very fast dynamic allocator.

			string name = string.Concat ("DynamicCode_Allocator_", type.Name);

			System.Reflection.Module module = type.Module;
			System.Reflection.Emit.DynamicMethod allocator = new System.Reflection.Emit.DynamicMethod (name, type, System.Type.EmptyTypes, module, true);
			System.Reflection.Emit.ILGenerator generator = allocator.GetILGenerator ();
			System.Reflection.ConstructorInfo constructor = type.GetConstructor (System.Type.EmptyTypes);

			if (constructor == null)
			{
				throw new System.InvalidOperationException (string.Format ("Class {0} has no constructor", type.Name));
			}

			generator.Emit (OpCodes.Newobj, constructor);
			generator.Emit (OpCodes.Ret);

			return (Allocator<T>) allocator.CreateDelegate (typeof (Allocator<T>));
		}

		/// <summary>
		/// Creates an allocator for some type <c>T</c>, using a constructor taking
		/// an argument of type <c>P</c>.
		/// </summary>
		/// <returns>
		/// An <see cref="Allocator&lt;T, P&gt;"/> for the type.
		/// </returns>
		public static Allocator<T, P> CreateAllocator<T, P>()
		{
			return DynamicCodeFactory.CreateAllocator<T, P> (typeof (T));
		}

		/// <summary>
		/// Creates an allocator for some type, using a constructor taking an argument
		/// of type <c>P</c>.
		/// </summary>
		/// <param name="type">The type of the object which must be allocated.</param>
		/// <returns>
		/// An <see cref="Allocator&lt;T, P&gt;"/> for the type.
		/// </returns>
		public static Allocator<T, P> CreateAllocator<T, P>(System.Type type)
		{
			System.Type[] constructorArgumentTypes = new System.Type[] { typeof (P) };

			string name = string.Concat ("DynamicCode_Allocator_", type.Name);

			System.Reflection.Module module = type.Module;
			System.Reflection.Emit.DynamicMethod allocator = new System.Reflection.Emit.DynamicMethod (name, type, constructorArgumentTypes, module, true);
			System.Reflection.Emit.ILGenerator generator = allocator.GetILGenerator ();
			System.Reflection.ConstructorInfo constructor = type.GetConstructor (constructorArgumentTypes);

			if (constructor == null)
			{
				throw new System.InvalidOperationException (string.Format ("Class {0} has no matching constructor", type.Name));
			}

			generator.Emit (OpCodes.Ldarg_0);
			generator.Emit (OpCodes.Newobj, constructor);
			generator.Emit (OpCodes.Ret);

			return (Allocator<T, P>) allocator.CreateDelegate (typeof (Allocator<T, P>));
		}

		/// <summary>
		/// Creates a property comparer for the specified property.
		/// </summary>
		/// <param name="type">The type of the object to access.</param>
		/// <param name="propertyName">The name of the property.</param>
		/// <returns>
		/// A <see cref="PropertyComparer"/> for the property.
		/// </returns>
		public static PropertyComparer CreatePropertyComparer(System.Type type, string propertyName)
		{
			System.Reflection.PropertyInfo info = type.GetProperty (propertyName);

			if (info == null)
			{
				if (Types.TypeRosetta.DoesTypeImplementInterface (type, typeof (Types.IStructuredData)))
				{
					return Types.StructuredData.CreatePropertyComparer (propertyName);
				}
				else
				{
					return null;
				}
			}
			else
			{
				return DynamicCodeFactory.CreatePropertyComparer (info);
			}
		}

		/// <summary>
		/// Creates a property comparer for the specified property.
		/// </summary>
		/// <param name="propertyInfo">The property info.</param>
		/// <returns>
		/// A <see cref="PropertyComparer"/> for the property.
		/// </returns>
		public static PropertyComparer CreatePropertyComparer(System.Reflection.PropertyInfo propertyInfo)
		{
			System.Reflection.MethodInfo method = propertyInfo.GetGetMethod (false);

			if ((method == null) ||
				(propertyInfo.CanRead == false))
			{
				return null;
			}
			
			System.Type   objType   = typeof (object);
			System.Type   hostType  = propertyInfo.DeclaringType;
			System.Type   propType  = propertyInfo.PropertyType;
			System.Type[] arguments = new System.Type[2];

			arguments[0] = objType;
			arguments[1] = objType;

			System.Type genericComparable = typeof (System.IComparable<>);
			System.Type typedComparable = genericComparable.MakeGenericType (propType);

			System.Reflection.MethodInfo compareToMethod = typedComparable.GetMethod ("CompareTo");

			string name = string.Concat ("DynamicCode_PropertyComparer_", propertyInfo.Name);

			System.Reflection.Emit.DynamicMethod comparer;
			comparer = new System.Reflection.Emit.DynamicMethod (name, typeof (int), arguments, hostType);

			System.Reflection.Emit.ILGenerator generator = comparer.GetILGenerator ();

			if (propType.IsClass)
			{
				System.Reflection.Emit.LocalBuilder l1 = generator.DeclareLocal (propType);
				System.Reflection.Emit.LocalBuilder l2 = generator.DeclareLocal (propType);

				System.Reflection.Emit.Label label1 = generator.DefineLabel ();
				System.Reflection.Emit.Label label2 = generator.DefineLabel ();
				System.Reflection.Emit.Label label3 = generator.DefineLabel ();

				if (hostType.IsValueType)
				{
					System.Reflection.Emit.LocalBuilder temp1 = generator.DeclareLocal (hostType);
					System.Reflection.Emit.LocalBuilder temp2 = generator.DeclareLocal (hostType);

					generator.Emit (OpCodes.Ldarg_0);
					generator.Emit (OpCodes.Unbox_Any, hostType);
					generator.Emit (OpCodes.Stloc_S, temp1);
					
					generator.Emit (OpCodes.Ldarg_1);
					generator.Emit (OpCodes.Unbox_Any, hostType);
					generator.Emit (OpCodes.Stloc_S, temp2);

					generator.Emit (OpCodes.Ldloca_S, temp1);
					generator.EmitCall (OpCodes.Call, method, null);
					generator.Emit (OpCodes.Stloc_0);
					
					generator.Emit (OpCodes.Ldloca_S, temp2);
					generator.EmitCall (OpCodes.Call, method, null);
					generator.Emit (OpCodes.Stloc_1);
				}
				else
				{
					generator.Emit (OpCodes.Ldarg_0);
					generator.Emit (OpCodes.Castclass, hostType);
					generator.EmitCall (OpCodes.Callvirt, method, null);
					generator.Emit (OpCodes.Stloc_0);
					
					generator.Emit (OpCodes.Ldarg_1);
					generator.Emit (OpCodes.Castclass, hostType);
					generator.EmitCall (OpCodes.Callvirt, method, null);
					generator.Emit (OpCodes.Stloc_1);
				}

				generator.Emit (OpCodes.Ldloc_0);
				generator.Emit (OpCodes.Ldloc_1);
				generator.Emit (OpCodes.Bne_Un_S, label1);

				generator.Emit (OpCodes.Ldc_I4_0);
				generator.Emit (OpCodes.Ret);

				generator.MarkLabel (label1);
				generator.Emit (OpCodes.Ldloc_0);
				generator.Emit (OpCodes.Brtrue_S, label2);
				generator.Emit (OpCodes.Ldc_I4_M1);
				generator.Emit (OpCodes.Ret);

				generator.MarkLabel (label2);
				generator.Emit (OpCodes.Ldloc_1);
				generator.Emit (OpCodes.Brtrue_S, label3);
				generator.Emit (OpCodes.Ldc_I4_1);
				generator.Emit (OpCodes.Ret);

				generator.MarkLabel (label3);
				generator.Emit (OpCodes.Ldloc_0);
				generator.Emit (OpCodes.Ldloc_1);
				generator.EmitCall (OpCodes.Callvirt, compareToMethod, null);
				generator.Emit (OpCodes.Ret);
			}
			else if (propType.IsValueType)
			{
				System.Reflection.Emit.LocalBuilder l1 = generator.DeclareLocal (propType);
				System.Reflection.Emit.LocalBuilder l2 = generator.DeclareLocal (propType);

				if (hostType.IsValueType)
				{
					System.Reflection.Emit.LocalBuilder temp1 = generator.DeclareLocal (hostType);
					System.Reflection.Emit.LocalBuilder temp2 = generator.DeclareLocal (hostType);

					generator.Emit (OpCodes.Ldarg_0);
					generator.Emit (OpCodes.Unbox_Any, hostType);
					generator.Emit (OpCodes.Stloc_S, temp1);

					generator.Emit (OpCodes.Ldarg_1);
					generator.Emit (OpCodes.Unbox_Any, hostType);
					generator.Emit (OpCodes.Stloc_S, temp2);

					generator.Emit (OpCodes.Ldloca_S, temp1);
					generator.EmitCall (OpCodes.Call, method, null);
					generator.Emit (OpCodes.Stloc_0);

					generator.Emit (OpCodes.Ldloca_S, temp2);
					generator.EmitCall (OpCodes.Call, method, null);
					generator.Emit (OpCodes.Stloc_1);
				}
				else
				{
					generator.Emit (OpCodes.Ldarg_0);
					generator.Emit (OpCodes.Castclass, hostType);
					generator.EmitCall (OpCodes.Callvirt, method, null);
					generator.Emit (OpCodes.Stloc_0);

					generator.Emit (OpCodes.Ldarg_1);
					generator.Emit (OpCodes.Castclass, hostType);
					generator.EmitCall (OpCodes.Callvirt, method, null);
					generator.Emit (OpCodes.Stloc_1);
				}

				if (propType.IsPrimitive)
				{
					//	For primitive types, it is worth to check for equality
					//	using a simply "bne" instruction, which could save us a
					//	call to the generic CompareTo method.
					
					System.Reflection.Emit.Label label1 = generator.DefineLabel ();
					
					generator.Emit (OpCodes.Ldloc_0);
					generator.Emit (OpCodes.Ldloc_1);
					generator.Emit (OpCodes.Bne_Un_S, label1);
					generator.Emit (OpCodes.Ldc_I4_0);
					generator.Emit (OpCodes.Ret);
					
					generator.MarkLabel (label1);
				}

				generator.Emit (OpCodes.Ldloc_0);
				generator.Emit (OpCodes.Box, propType);
				generator.Emit (OpCodes.Ldloc_1);
				generator.EmitCall (OpCodes.Callvirt, compareToMethod, null);
				generator.Emit (OpCodes.Ret);
			}

			return (PropertyComparer) comparer.CreateDelegate (typeof (PropertyComparer));
		}


		[System.ThreadStatic]
		private static Dictionary<string, PropertyGetter> getterCache = new Dictionary<string, PropertyGetter> ();

		[System.ThreadStatic]
		private static Dictionary<string, PropertySetter> setterCache = new Dictionary<string, PropertySetter> ();
	}
}
