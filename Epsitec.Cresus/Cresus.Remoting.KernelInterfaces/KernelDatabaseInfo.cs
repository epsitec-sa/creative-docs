//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	[System.Serializable]
	public struct KernelDatabaseInfo
	{
		public KernelDatabaseInfo(System.Guid id, string name)
		{
			this.id = id;
			this.name = name;
		}

		public System.Guid Id
		{
			get
			{
				return this.id;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}


		private readonly System.Guid id;
		private readonly string name;
	}
}
