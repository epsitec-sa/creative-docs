//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.ModuleRepository
{
	/// <summary>
	/// The <c>ModuleRecord</c> class stores identity information about a given
	/// module.
	/// </summary>
	public sealed class ModuleRecord
	{
		public ModuleRecord()
		{
		}

		public int ModuleId
		{
			get;
			set;
		}

		public string ModuleName
		{
			get;
			set;
		}

		public string DeveloperName
		{
			get;
			set;
		}

		public ModuleState ModuleState
		{
			get;
			set;
		}
	}
}
