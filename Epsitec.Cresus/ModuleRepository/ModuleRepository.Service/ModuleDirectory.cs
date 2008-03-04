//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Epsitec.ModuleRepository
{
	[DataContract]
	public class ModuleDirectory
	{
		public ModuleDirectory(string name)
		{
			this.Name = name;
		}

		[DataMember]
		public string Name
		{
			get;
			set;
		}

		[DataMember]
		public ModuleFile[] Files
		{
			get
			{
				if (this.files == null)
				{
					return new ModuleFile[0];
				}
				else
				{
					return this.files.ToArray ();
				}
			}
			set
			{
				if (this.files == null)
				{
					this.files = new List<ModuleFile> (value);
				}
				else
				{
					this.files.Clear ();
					this.files.AddRange (value);
				}
			}
		}

		public void AddFile(ModuleFile file)
		{
			if (this.files == null)
			{
				this.files = new List<ModuleFile> ();
			}
			
			this.files.Add (file);
		}

		private List<ModuleFile> files;
	}
}
