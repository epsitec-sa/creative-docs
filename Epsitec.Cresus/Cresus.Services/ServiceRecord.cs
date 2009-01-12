//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// The <c>ServiceRecord</c> structure describes a service hosted by the
	/// <see cref="Engine"/> class.
	/// </summary>
	[System.Serializable]
	public struct ServiceRecord
	{
		internal ServiceRecord(AbstractServiceEngine service, System.Type type)
		{
			int uniqueId = System.Threading.Interlocked.Increment (ref ServiceRecord.uniqueId);

			this.serviceInstance = service;
			this.serviceType     = type;
			this.serviceId       = service.GetServiceId ();
			this.uniqueName      = string.Concat (type.Name, "-", uniqueId.ToString ());
		}

		
		public System.MarshalByRefObject	ServiceInstance
		{
			get
			{
				return this.serviceInstance;
			}
		}

		public System.Type					ServiceType
		{
			get
			{
				return this.serviceType;
			}
		}

		public System.Guid					ServiceId
		{
			get
			{
				return this.serviceId;
			}
		}

		public string						UniqueName
		{
			get
			{
				return this.uniqueName;
			}
		}

		
		static int							uniqueId;

		System.MarshalByRefObject			serviceInstance;
		System.Type							serviceType;
		System.Guid							serviceId;
		string								uniqueName;
	}
}
