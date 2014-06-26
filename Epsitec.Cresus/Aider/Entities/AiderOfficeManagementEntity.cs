//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;

using System.Linq;
using System.Collections.Generic;
using Epsitec.Aider.Data.Common;

namespace Epsitec.Aider.Entities
{
	public partial class AiderOfficeManagementEntity
	{
		public AiderOfficeManagementEntity()
		{
		}

		
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.OfficeMainContact.GetAddressLabelText ());
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.OfficeName);
		}


		public FormattedText GetDocumentTitleSummary()
		{
			return TextFormatter.FormatText ("Documents");
		}

		public FormattedText GetDocumentsSummary()
		{
			switch (this.Documents.Count)
			{
				case 0:
					return TextFormatter.FormatText ("Aucun");
				case 1:
					return TextFormatter.FormatText ("Un document");

				default:
					return TextFormatter.FormatText (this.Documents.Count, "documents");
			}
		}

		public FormattedText GetSettingsTitleSummary()
		{
			return TextFormatter.FormatText ("Expéditeurs");
		}

		public FormattedText GetSettingsSummary()
		{
			switch (this.OfficeSenders.Count)
			{
				case 0:
					return TextFormatter.FormatText ("Aucun");
				case 1:
					return TextFormatter.FormatText ("Un expéditeur");
				default:
					return TextFormatter.FormatText (this.OfficeSenders.Count, "expéditeurs");
			}
		}
		
		public static void JoinOfficeManagement(BusinessContext businessContext, AiderOfficeManagementEntity office, AiderUserEntity user)
		{
			var currentOffice = user.Office;
			var currentSender = user.OfficeSender;
			var contact		  = user.Contact;

			if (currentOffice.IsNotNull ())
			{
				//Stop old usergroup participation
				var currentUserGroup = currentOffice.ParishGroup.Subgroups.Single (s => s.GroupDef.Classification == Enumerations.GroupClassification.Users);
				AiderGroupEntity.RemoveParticipations (businessContext, currentUserGroup.FindParticipationsByGroup (businessContext, contact, currentUserGroup));

				//try to remap old sender settings
				var oldSender = AiderOfficeSenderEntity.Find (businessContext, contact);
				if (oldSender.IsNotNull ())
				{
					oldSender.Office = office;
					user.OfficeSender = oldSender;
					office.AddSenderInternal (oldSender);
				}
				else
				{
					//Create sender
					user.OfficeSender = AiderOfficeSenderEntity.Create (businessContext, office, user.Contact);
				}
			}
			else
			{
				//Create sender
				user.OfficeSender = AiderOfficeSenderEntity.Create (businessContext, office, user.Contact);
			}

			//Create usergroup participation
			var newUserGroup = office.ParishGroup.Subgroups.Single (s => s.GroupDef.Classification == Enumerations.GroupClassification.Users);
			var participationData = new List<ParticipationData> ();
			participationData.Add (new ParticipationData (contact));
			newUserGroup.AddParticipations (businessContext, participationData, Date.Today, FormattedText.Null);

			//Join parish
			user.Office = office;
			user.Parish = office.ParishGroup;
		}

		public static void LeaveOfficeManagement(BusinessContext businessContext, AiderOfficeManagementEntity office, AiderUserEntity user)
		{
			var currentOffice = user.Office;
			var currentSender = user.OfficeSender;
			var contact		  = user.Contact;

			if (currentOffice.IsNotNull ())
			{
				//Stop old usergroup participation
				var currentUserGroup = currentOffice.ParishGroup.Subgroups.Single (s => s.GroupDef.Classification == Enumerations.GroupClassification.Users);
				AiderGroupEntity.RemoveParticipations (businessContext, currentUserGroup.FindParticipationsByGroup (businessContext, contact, currentUserGroup));	
			}
			
			if(currentSender.IsNotNull ())
			{
				//Remove sender
				office.RemoveSenderInternal (currentSender);
				AiderOfficeSenderEntity.Delete (businessContext, currentSender);
			}
	
			//Leave parish
			user.Office = null;
		}

		public static AiderOfficeManagementEntity Find(BusinessContext businessContext, AiderGroupEntity group)
		{
			var officeExample = new AiderOfficeManagementEntity
			{
				ParishGroup = group
			};

			return businessContext.DataContext.GetByExample (officeExample).First ();
		}

		public static AiderOfficeManagementEntity Create(BusinessContext businessContext,string name,AiderGroupEntity parishGroup)
		{
			var office = businessContext.CreateAndRegisterEntity<AiderOfficeManagementEntity> ();

			office.OfficeName = name;
			office.ParishGroup = parishGroup;
			office.ParishGroupPathCache = parishGroup.Path;
			office.RefreshOfficeShortName ();

			return office;
		}

		partial void GetOfficeSenders(ref IList<AiderOfficeSenderEntity> value)
		{
			value = this.GetOfficeSenders ().AsReadOnlyCollection ();
		}

		partial void GetEmployees(ref IList<AiderEmployeeEntity> value)
		{
			value = this.EmployeeJobs.Select (x => x.Employee).Distinct ().OrderBy (x => x.Person.DisplayName).AsReadOnlyCollection ();
		}

		partial void GetEmployeeJobs(ref IList<AiderEmployeeJobEntity> value)
		{
			value = this.GetVirtualCollection (ref this.employeeJobs, x => x.Office = this).AsReadOnlyCollection ();
		}

		public void RefreshOfficeShortName()
		{
			var name = this.OfficeName;
			var text = name.ToLowerInvariant ();
			
			var parishPrefixes = new string[] { "paroisse d'", "paroisse de ", "paroisse du ", "paroisse des ", "pla " };
			var parishMatch    = parishPrefixes.FirstOrDefault (x => text.StartsWith (x));

			if (parishMatch != null)
			{
				name = name.Substring (parishMatch.Length);

				this.OfficeType = Enumerations.OfficeType.Parish;
			}
			else
			{
				if (this.ParishGroup.IsRegion ())
				{
					this.OfficeType = Enumerations.OfficeType.Region;
				}
			}

			this.OfficeShortName = name.Trim ();
		}


		internal void AddSenderInternal(AiderOfficeSenderEntity settings)
		{
			if (!this.GetOfficeSenders ().Any (s => s == settings))
			{
				this.GetOfficeSenders ().Add (settings);
			}
		}

		internal void RemoveSenderInternal(AiderOfficeSenderEntity settings)
		{
			this.GetOfficeSenders ().Remove (settings);
		}

		private IList<AiderOfficeSenderEntity> GetOfficeSenders()
		{
			if (this.senders == null)
			{
				this.senders = this.ExecuteWithDataContext (d => this.FindOfficeSenders (d), () => new List<AiderOfficeSenderEntity> ());
			}

			return this.senders;
		}

		private IList<AiderOfficeSenderEntity> FindOfficeSenders(DataContext dataContext)
		{
			var example = new AiderOfficeSenderEntity
			{
				Office = this
			};

			return dataContext.GetByExample (example)
							  .OrderBy (x => x.Name)
							  .ToList ();
		}


		internal void AddDocumentInternal(AiderOfficeReportEntity document)
		{
			this.GetDocuments ().Add (document);
		}

		internal void RemoveDocumentInternal(AiderOfficeReportEntity document)
		{
			this.GetDocuments ().Remove (document);
		}

		partial void GetDocuments(ref IList<AiderOfficeReportEntity> value)
		{
			value = this.GetDocuments ().AsReadOnlyCollection ();
		}

		partial void GetRegionalReferees(ref IList<AiderRefereeEntity> value)
		{
			value = this.GetVirtualCollection (ref this.regionalReferees, x => x.Group = this.ParishGroup).AsReadOnlyCollection ();
		}

		partial void GetAssociatedGroups(ref IList<AiderGroupEntity> value)
		{
			value = this.ExecuteWithDataContext (c => this.GetAssociatedGroups (c), null);
		}

		public IList<AiderGroupEntity> GetAssociatedGroups(DataContext context = null)
		{
			if (this.associatedGroups == null)
			{
				if (context == null)
				{
					context = DataContextPool.GetDataContext (this);
				}

				var groups = new HashSet<AiderGroupEntity> ();
				if (this.OfficeType == Enumerations.OfficeType.Parish)
				{
					groups.UnionWith (AiderGroupEntity.FindRegionalGroupsGloballyVisibleToParishes (context, this.ParishGroupPathCache));
					groups.UnionWith (AiderGroupEntity.FindGroupsGloballyVisibleToParishes (context));				
				}
				if (this.OfficeType == Enumerations.OfficeType.Region)
				{
					groups.UnionWith (AiderGroupEntity.FindAllGroupsGloballyVisibleToRegions (context));	
				}

				this.associatedGroups = groups.ToList();
			}

			return this.associatedGroups;
		}

		partial void GetRegion(ref string value)
		{
			if(this.ParishGroup.Parents.Any ())
			{
				value = this.ParishGroup.Parents.Last ().Name;
			}
			else
			{
				value = this.ParishGroup.Name;
			}
			
		}

		private IList<AiderOfficeReportEntity> GetDocuments()
		{
			if (this.documents == null)
			{
				this.documents = this.ExecuteWithDataContext (d => this.FindDocuments (d), () => new List<AiderOfficeReportEntity> ());
			}

			return this.documents;
		}

		private IList<AiderOfficeReportEntity> FindDocuments(DataContext dataContext)
		{
			var example = new AiderOfficeReportEntity
			{
				Office = this
			};

			return dataContext.GetByExample (example)
							  .OrderBy (x => x.Name)
							  .ToList ();
		}
		
		
		private IList<AiderOfficeSenderEntity>	senders;
		private IList<AiderOfficeReportEntity>	documents;
		private IList<AiderEmployeeJobEntity>	employeeJobs;
		private IList<AiderRefereeEntity>		regionalReferees;
		private IList<AiderGroupEntity>			associatedGroups;
	}
}
