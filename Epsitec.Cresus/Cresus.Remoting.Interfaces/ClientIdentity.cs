//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// La structure ClientIdentity décrit l'identité d'un client. Elle
	/// est utilisée lors de dialogues entre un client et son serveur.
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
		
		
		private string							name;
		private int								client_id;
		
		private static int						default_client_id;
	}
}
