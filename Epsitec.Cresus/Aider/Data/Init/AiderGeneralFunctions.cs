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
								groupDef.NameFeminine = "Présidente";
								groupDef.NameArticle  = FrenchArticle.SingularMasculineBeforeConsonant;
								break;
							
							case "Vice-président" :
								groupDef.NameFeminine = "Vice-présidente";
								groupDef.NameArticle  = FrenchArticle.SingularMasculineBeforeConsonant;
								break;
							
							case "Scrutateur" :
								groupDef.NameFeminine = "Scrutatrice";
								groupDef.NameArticle  = FrenchArticle.SingularMasculineBeforeConsonant;
								break;
							
							case "Trésorier" :
								groupDef.NameFeminine = "Trésorière";
								groupDef.NameArticle  = FrenchArticle.SingularMasculineBeforeConsonant;
								break;
							
							case "Suppléant" :
								groupDef.NameFeminine = "Suppléante";
								groupDef.NameArticle  = FrenchArticle.SingularMasculineBeforeConsonant;
								break;
							
							case "Animateur" :
								groupDef.NameFeminine = "Animatrice";
								groupDef.NameArticle  = FrenchArticle.SingularMasculineBeforeVowelOrMute;
								break;
							
							case "Collaborateur" :
								groupDef.NameFeminine = "Collaboratrice";
								groupDef.NameArticle  = FrenchArticle.SingularMasculineBeforeConsonant;
								break;
							
							case "Conseiller":
								groupDef.NameFeminine = "Conseillère";
								groupDef.NameArticle  = FrenchArticle.SingularMasculineBeforeConsonant;
								break;
							
							case "Directeur":
								groupDef.NameFeminine = "Directrice";
								groupDef.NameArticle  = FrenchArticle.SingularMasculineBeforeConsonant;
								break;
							
							case "Doyen":
								groupDef.NameFeminine = "Doyenne";
								groupDef.NameArticle  = FrenchArticle.SingularMasculineBeforeConsonant;
								break;

							case "Membre":
							case "Membre du Bureau":
							case "Secrétaire":
								groupDef.NameArticle = FrenchArticle.SingularMasculineBeforeConsonant;
								break;
							
							default:
								groupDef.NameFeminine = groupDef.Name;
								break;
						}
						
						Console.WriteLine ("{0} applyed", groupDef.NameFeminine);
					}
					
				}
				

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}
	}
}
