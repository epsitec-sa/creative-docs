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
using Epsitec.Common.Types;

namespace Epsitec.Aider.Data.Groups
{
	/// <summary>
	/// This job create default employee and jobs for each user
	/// </summary>
	public static class AiderCollaborators
	{	
		public static void Start(CoreData coreData)
		{
			System.Console.WriteLine ("Starting job...");
			AiderCollaborators.InitEmployeeFromUsers (coreData);
			AiderCollaborators.InitJobsFromUsers (coreData);
			System.Console.WriteLine ("Finished!");
		}

		private static void InitEmployeeFromUsers(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var exempleUser = new AiderUserEntity ()
				{
					Disabled = false
				};

				var users = businessContext.GetByExample<AiderUserEntity> (exempleUser);

				foreach(var user in users)
				{
					System.Console.WriteLine ("///:" + user.DisplayName);
					if(user.Contact.IsNotNull())
					{
						var contact = user.Contact;
						System.Console.WriteLine ("contact found");
						var person = contact.Person;
						if (person.IsNotNull ())
						{
							if (person.Employee.IsNull ())
							{
								System.Console.WriteLine ("create employee");
								AiderEmployeeEntity.Create (
												businessContext,
												contact.Person,
												user,
												Enumerations.EmployeeType.BenevoleAIDER,
												"",
												Enumerations.EmployeeActivity.None,
												"");
							}
							else
							{
								System.Console.WriteLine ("employee already existing... nothing to do");
							}
						}
						else
						{
							System.Console.WriteLine ("contact target a missing person... nothing to do");
						}
					}
					else
					{
						System.Console.WriteLine ("contact missing... nothing to do");
					}
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		private static void InitJobsFromUsers(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var exempleUser = new AiderUserEntity ()
				{
					Disabled = false
				};

				var users = businessContext.GetByExample<AiderUserEntity> (exempleUser);

				foreach (var user in users)
				{
					System.Console.WriteLine ("///:" + user.DisplayName);
					if (user.Contact.IsNotNull ())
					{
						var contact = user.Contact;
						var person = user.Contact.Person;
						if (person.IsNotNull ())
						{
							var employee = person.Employee;
							if (employee.IsNotNull ())
							{
								if (user.Office.IsNotNull ())
								{
									AiderEmployeeJobEntity.CreateOfficeManager (
												businessContext,
												employee,
												user.Office,
												"");
								}
								else
								{
									if (user.Parish.IsNotNull ())
									{
										var officeExemple = new AiderOfficeManagementEntity
										{
											ParishGroup = user.Parish
										};

										var offices = businessContext.GetByExample<AiderOfficeManagementEntity> (officeExemple);
										if (offices.Any ())
										{
											AiderEmployeeJobEntity.CreateOfficeUser (
												businessContext,
												employee,
												offices.First (),
												"");
										}
									}
								}
							}
						}
						else
						{
							System.Console.WriteLine ("contact target a missing person... nothing to do");
						}
						
					}
					else
					{
						System.Console.WriteLine ("contact missing... nothing to do");
					}
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}
	}
}
