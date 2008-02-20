//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
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
