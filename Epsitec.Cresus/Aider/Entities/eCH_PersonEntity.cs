//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Threading;


namespace Epsitec.Aider.Entities
{
	public partial class eCH_PersonEntity
	{
		internal static string GetDefaultFirstName(eCH_PersonEntity person)
		{
			if (string.IsNullOrWhiteSpace (person.PersonFirstNames))
			{
				return "";
			}
			else
			{
				string[] names = person.PersonFirstNames.Split (' ');
				return names[0];
			}
		}
	}
}
