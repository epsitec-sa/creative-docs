//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// La structure ClientIdentity d�crit l'identit� d'un client. Elle
	/// est utilis�e lors de dialogues entre un client et son serveur.
	/// </summary>
	
	[System.Serializable]
	
	public struct ClientIdentity
	{
		public ClientIdentity(string name)
		{
			this.name      = name;
			this.client_id = ClientIdentity.DefaultClientId;
		}
		
		public ClientIdentity(string name, int client_id)
		{
			this.name      = name;
			this.client_id = client_id;
		}
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public int								ClientId
		{
			get
			{
				return this.client_id;
			}
		}
		
		
		public static int						DefaultClientId
		{
			get
			{
				return ClientIdentity.default_client_id;
			}
		}
		
		
		public static void DefineDefaultClientId(int client_id)
		{
			if (ClientIdentity.default_client_id == client_id)
			{
				return;
			}
			
			if (ClientIdentity.default_client_id == 0)
			{
				ClientIdentity.default_client_id = client_id;
			}
			else
			{
				throw new System.InvalidOperationException ("Client ID may only be defined once.");
			}
		}
		
		
		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "[{0}:{1}]", this.ClientId, this.Name);
		}
		
		
		private string							name;
		private int								client_id;
		
		private static int						default_client_id;
	}
}
