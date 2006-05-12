//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	[System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple=true)]
	public class ControllerAttribute : System.Attribute
	{
		public ControllerAttribute(System.Type type)
		{
			this.type = type;
		}
		
		public System.Type						Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public static IEnumerable<System.Type> GetRegisteredTypes(System.Reflection.Assembly assembly)
		{
			System.Type controllerType = typeof (IController);
			
			foreach (ControllerAttribute attribute in assembly.GetCustomAttributes (typeof (ControllerAttribute), false))
			{
				//	Return only types which describe classes that implement the
				//	IController interface :
				
				System.Type[] interfaces = attribute.Type.GetInterfaces ();
				bool ok = false;

				for (int i = 0; i < interfaces.Length; i++)
				{
					if (interfaces[i] == controllerType)
					{
						ok = true;
						break;
					}
				}
				
				if (ok)
				{
					yield return attribute.Type;
				}
			}
		}

		private System.Type						type;
	}
}
