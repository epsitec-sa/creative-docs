//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;
using System.Runtime.Serialization;

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
