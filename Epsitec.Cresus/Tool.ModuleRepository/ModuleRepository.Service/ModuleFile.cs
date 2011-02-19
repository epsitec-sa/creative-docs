//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Epsitec.ModuleRepository
{
	[DataContract]
	public class ModuleFile
	{
		[DataMember]
		public string Path
		{
			get;
			set;
		}

		[DataMember]
		public byte[] Data
		{
			get;
			set;
		}
	}
}
