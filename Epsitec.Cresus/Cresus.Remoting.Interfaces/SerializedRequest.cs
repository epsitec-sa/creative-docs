//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
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
}
