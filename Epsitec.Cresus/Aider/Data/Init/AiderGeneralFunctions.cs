//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.IO;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Data.Groups
{
	/// <summary>
	/// This job create missing Aider Users groups if needed
	/// </summary>
	public static class AiderGeneralFunctions
	{		
		public static void ApplyFemininForm(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var functionsGroupDef = AiderGroupDefEntity.FindFunctions (businessContext);
				foreach (var groupDef in functionsGroupDef)
				{
					if (groupDef.IsFunction ())
					{
						Console.WriteLine ("{0} found", groupDef.Name);
						switch (groupDef.Name)
						{
							case "Président" :
							groupDef.FunctionNameInTheFeminine = "Présidente";
							break;
							case "Vice-président" :
							groupDef.FunctionNameInTheFeminine = "Vice-présidente";
							break;
							case "Scrutateur" :
							groupDef.FunctionNameInTheFeminine = "Scrutatrice";
							break;
							case "Trésorier" :
							groupDef.FunctionNameInTheFeminine = "Trésorière";
							break;
							case "Suppléant" :
							groupDef.FunctionNameInTheFeminine = "Suppléante";
							break;
							case "Animateur" :
							groupDef.FunctionNameInTheFeminine = "Animatrice";
							break;
							case "Collaborateur" :
							groupDef.FunctionNameInTheFeminine = "Collaboratrice";
							break;
							case "Conseiller":
							groupDef.FunctionNameInTheFeminine = "Conseillère";
							break;
							case "Directeur":
							groupDef.FunctionNameInTheFeminine = "Directrice";
							break;
							case "Doyen":
							groupDef.FunctionNameInTheFeminine = "Doyenne";
							break;
							default:
								groupDef.FunctionNameInTheFeminine = groupDef.Name;
							break;
						}
						Console.WriteLine ("{0} applyed", groupDef.FunctionNameInTheFeminine);
					}
					
				}
				

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}
	}
}
