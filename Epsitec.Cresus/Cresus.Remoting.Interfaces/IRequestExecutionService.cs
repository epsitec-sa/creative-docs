//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IRequestExecutionService donne accès au service d'exécution
	/// des requêtes, comme son nom l'indique.
	/// </summary>
	public interface IRequestExecutionService
	{
		void EnqueueRequest(SerializedRequest[] requests);
		
		void QueryRequestStates(int client_id, out RequestState[] states);
		void ClearRequestStates(RequestState[] states);
		
		void Ping(string text, out string reply);
	}
	
	
	[System.Serializable]
	public struct SerializedRequest
	{
		public SerializedRequest(long id, byte[] data)
		{
			this.id   = id;
			this.data = data;
		}
		
		
		public long								Identifier
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}
		
		public byte[]							Data
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}
		
		
		private long							id;
		private byte[]							data;
	}
	
	
	[System.Serializable]
	public struct RequestState
	{
		public RequestState(long id, int state)
		{
			this.id    = id;
			this.state = state;
		}
		
		
		public long								Identifier
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}
		
		public int								State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}
		
		
		private long							id;
		private int								state;
	}
}
