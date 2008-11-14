//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;
using System.Runtime.Serialization;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>ClientIdentity</c> structure defines the client identity used by
	/// the remoting infrastructure.
	/// </summary>
	
	[System.Serializable]
	[DataContract]
	
	public struct ClientIdentity
	{
		public ClientIdentity(string name)
		{
			this.name = name;
			this.id   = ClientIdentity.DefaultId;
		}
		
		public ClientIdentity(string name, int id)
		{
			this.name = name;
			this.id   = id;
		}
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public int								Id
		{
			get
			{
				return this.id;
			}
		}
		
		
		public static int						DefaultId
		{
			get
			{
				return ClientIdentity.defaultClientId;
			}
		}
		
		
		public static void DefineDefaultClientId(int clientId)
		{
			if (ClientIdentity.defaultClientId == clientId)
			{
				return;
			}
			
			if (ClientIdentity.defaultClientId == 0)
			{
				ClientIdentity.defaultClientId = clientId;
			}
			else
			{
				throw new System.InvalidOperationException ("The default client ID may only be defined once.");
			}
		}
		
		
		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "[{0}:{1}]", this.Id, this.Name);
		}
		
		
		private string							name;
		private int								id;
		
		private static int						defaultClientId;
	}
}
