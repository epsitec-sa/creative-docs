//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Services.Extensions
{
	internal static class SystemTypeExtensions
	{
		public static bool HasServiceBehaviorAttribute(this System.Type type)
		{
			return type.GetCustomAttributes (typeof (ServiceBehaviorAttribute), false).Length > 0;
		}
		
		public static bool HasServiceContractAttribute(this System.Type type)
		{
			return type.GetCustomAttributes (typeof (ServiceContractAttribute), false).Length > 0;
		}
	}
}
