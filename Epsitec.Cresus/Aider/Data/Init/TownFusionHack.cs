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
	/// This job change TownName in case of fusion
	/// </summary>
	public static class TownFusionHack
	{		
		public static void FusionArzier(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{

				var townExample = new AiderTownEntity ()
				{
					SwissZipCode = 1273
				};

				foreach (var town in businessContext.GetByExample<AiderTownEntity> (townExample))
				{
					town.Name = "Arzier-Le Muids";
					Console.WriteLine ("Renamed {0} to {1}", town.Name, "Arzier-Le Muids");
					var contactExample = new AiderContactEntity ()
					{
						DisplayZipCode = "1273"
					};

					foreach (var contact in businessContext.GetByExample<AiderContactEntity> (contactExample))
					{
						contact.RefreshCache ();
						Console.WriteLine ("Cache refresh for {0}", contact.GetCompactSummary ());
					}

					var goodNewsExample = new AiderSubscriptionEntity ()
					{
						DisplayZipCode = "1273"
					};

					foreach (var sub in businessContext.GetByExample<AiderSubscriptionEntity> (goodNewsExample))
					{
						sub.RefreshCache ();
						Console.WriteLine ("Cache refresh for {0}", sub.GetCompactSummary ());
					}

				}
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				Console.WriteLine ("hacked! press any key...");
				Console.ReadLine ();
			}
		}
	}
}
