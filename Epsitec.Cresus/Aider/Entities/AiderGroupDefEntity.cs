//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;

namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupDefEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return TextFormatter.FormatText (this.Name);
		}

		public AiderGroupEntity Instantiate(BusinessContext businessContext)
		{
			var group = AiderGroupEntity.Create (businessContext, this, this.Name, 0, this.PathTemplate);

			this.InstantiateSubgroups (businessContext, group);

			return group;
		}

		public AiderGroupEntity InstantiateRegion(BusinessContext businessContext, string name, int code)
		{
			var path = AiderGroupIds.GetRegionId (code);
			var group = AiderGroupEntity.Create (businessContext, this, name, 0, path);

			this.InstantiateSubgroups (businessContext, group);

			return group;
		}

		public AiderGroupEntity InstantiateParish(BusinessContext businessContext, AiderGroupEntity region, string name, int code)
		{
			var pathPart = AiderGroupIds.GetParishId (code);
			var group = AiderGroupEntity.Create (businessContext, region, this, name, pathPart);

			this.InstantiateSubgroups (businessContext, group);

			return group;
		}

		private void InstantiateSubgroups(BusinessContext businessContext, AiderGroupEntity group)
		{
			// We discard the parish subgroup because it is a special template subgroup that we want
			// to instantiate manually later on.
			var subgroups = this.Subgroups
				.Where (g => !g.IsParish ())
				.ToList ();

			AiderGroupDefEntity.InstantiateSubgroups (businessContext, group, subgroups);

			if (this.Function.IsNotNull ())
			{
				AiderGroupDefEntity.InstantiateSubgroups (businessContext, group, this.Function.Subgroups);
			}
		}

		private static void InstantiateSubgroups(BusinessContext businessContext, AiderGroupEntity group, IEnumerable<AiderGroupDefEntity> childDefinitions)
		{
			foreach (var childDefinition in childDefinitions)
			{
				var childGroup = group.CreateSubgroup (businessContext, childDefinition);

				childDefinition.InstantiateSubgroups (businessContext, childGroup);
			}
		}

		
		public static void InstantiateFunctionSubGroup(BusinessContext businessContext, AiderGroupDefEntity functionDef, AiderGroupDefEntity functionSubGroupDef)
		{
			var existingDefsWithThisFunction = AiderGroupDefEntity.FindGroupDefByFunction (businessContext, functionDef);
			foreach (var existingGroupDef in existingDefsWithThisFunction)
			{
				//Create groups at right place
				var groupsToComplete = AiderGroupEntity.FindGroupsFromGroupDef (businessContext, existingGroupDef);
				foreach (var group in groupsToComplete)
				{
					group.CreateSubgroup (businessContext, functionSubGroupDef);
				}
			}
		}

		public static AiderGroupDefEntity CreateFunctionSubGroup(BusinessContext businessContext, AiderGroupDefEntity functionDef,AiderGroupDefEntity parent, string name,
			 bool subgroupsAllowed, bool membersAllowed, Mutability mutability)
		{
			var aiderGroupDef = businessContext.CreateAndRegisterEntity<AiderGroupDefEntity> ();

			aiderGroupDef.Name = name;
			aiderGroupDef.Number = ""; //?
			aiderGroupDef.Level = parent.Level + 1;
			aiderGroupDef.SubgroupsAllowed = subgroupsAllowed;
			aiderGroupDef.MembersAllowed = membersAllowed;

			var number = AiderGroupIds.FindNextSubGroupDefNumber (parent.Subgroups.Select (g => g.PathTemplate), 'F');
			aiderGroupDef.PathTemplate = AiderGroupIds.CreateFunctionSubgroupPath (parent.PathTemplate,number);

			aiderGroupDef.Classification = GroupClassification.Function;
			aiderGroupDef.Mutability = mutability;
			aiderGroupDef.Function = functionDef;

			//uplink
			parent.Subgroups.Add (aiderGroupDef);

			return aiderGroupDef;
		}

		public static void DeleteFunctionSubGroup(BusinessContext businessContext, AiderGroupDefEntity functionToDelete, bool force)
		{
			if ((!functionToDelete.IsFunction ()) || (functionToDelete.Level <= 1))
			{
				throw new BusinessRuleException ("Action impossible");
			}

			var parentDef = AiderGroupDefEntity.FindParent (businessContext, functionToDelete);
			var existingDefsWithThisFunction = AiderGroupDefEntity.FindGroupDefByFunction (businessContext, parentDef);
			foreach (var existingGroupDef in existingDefsWithThisFunction)
			{
				var groupsToProcess = AiderGroupEntity.FindGroupsFromGroupDef (businessContext, existingGroupDef);
				foreach (var group in groupsToProcess)
				{
					var subGroupsToRemove = group.Subgroups.Where (g => g.GroupDef == functionToDelete);

					foreach (var subGroup in subGroupsToRemove)
					{
						AiderGroupEntity.Delete (businessContext, subGroup, force);
					}
				}
			}

			parentDef.Subgroups.Remove (functionToDelete);
			businessContext.DeleteEntity (functionToDelete);
		}

		public static AiderGroupDefEntity CreateDefinitionSubGroup(BusinessContext businessContext, AiderGroupDefEntity parent, string name, 
			GroupClassification groupClass, bool subgroupsAllowed, bool membersAllowed, Mutability mutability)
		{
			var aiderGroupDef = businessContext.CreateAndRegisterEntity<AiderGroupDefEntity> ();

			aiderGroupDef.Name = name;
			aiderGroupDef.Number = ""; //?
			aiderGroupDef.Level = parent.Level + 1;
			aiderGroupDef.SubgroupsAllowed = subgroupsAllowed;
			aiderGroupDef.MembersAllowed = membersAllowed;

			if ((groupClass == GroupClassification.Parish) || (groupClass == GroupClassification.ParishOfGermanLanguage))
			{
				aiderGroupDef.PathTemplate = AiderGroupIds.CreateParishSubgroupPath (parent.PathTemplate);
			}
			else
			{
				var number = AiderGroupIds.FindNextSubGroupDefNumber (parent.Subgroups.Select (g => g.PathTemplate), 'D');
				aiderGroupDef.PathTemplate = AiderGroupIds.CreateDefinitionSubgroupPath (parent.PathTemplate, number);
			}
			

			aiderGroupDef.Classification = groupClass;
			aiderGroupDef.Mutability = mutability;

			//uplink
			parent.Subgroups.Add (aiderGroupDef);

			return aiderGroupDef;
		}

		public static AiderGroupDefEntity CreateFunctionSubGroup(BusinessContext businessContext, AiderGroupDefEntity parent, string name)
		{
			var aiderGroupDef = businessContext.CreateAndRegisterEntity<AiderGroupDefEntity> ();

			aiderGroupDef.Name = name;
			aiderGroupDef.Number = ""; //EERV internal field, deprecated
			aiderGroupDef.Level = parent.Level + 1;
			
			
			aiderGroupDef.Classification = GroupClassification.Function;
			aiderGroupDef.Mutability = Mutability.None;

			if (parent.Level == 0)
			{
				var number = AiderGroupIds.FindNextSubGroupDefNumber (parent.Subgroups.Select (g => g.PathTemplate), 'D');
				aiderGroupDef.PathTemplate = AiderGroupIds.CreateDefinitionSubgroupPath (parent.PathTemplate, number);
				aiderGroupDef.MembersAllowed = false;
				aiderGroupDef.SubgroupsAllowed = true;
			}
			else
			{
				var number = AiderGroupIds.FindNextSubGroupDefNumber (parent.Subgroups.Select (g => g.PathTemplate), 'F');
				aiderGroupDef.PathTemplate = AiderGroupIds.CreateFunctionSubgroupPath (parent.PathTemplate, number);
				aiderGroupDef.MembersAllowed = true;
				aiderGroupDef.SubgroupsAllowed = false;
			}
			
			//uplink
			parent.Subgroups.Add (aiderGroupDef);

			return aiderGroupDef;
		}

		public static AiderGroupDefEntity CreateDefinitionRootGroup(BusinessContext businessContext, string name, GroupClassification groupClass, bool isMutable)
		{
			var aiderGroupDef = businessContext.CreateAndRegisterEntity<AiderGroupDefEntity> ();

			aiderGroupDef.Name = name;
			aiderGroupDef.Number = ""; //?
			aiderGroupDef.Level = AiderGroupIds.TopLevel;
			aiderGroupDef.SubgroupsAllowed = true;
			aiderGroupDef.MembersAllowed = false;

			var prefixChar = 'D';
			if ((groupClass == GroupClassification.Parish) || (groupClass == GroupClassification.ParishOfGermanLanguage))
			{
				prefixChar = 'P';
			}

			var rootGroupsDefs = AiderGroupDefEntity.FindRootGroupsDefinitions (businessContext);
			var number = AiderGroupIds.FindNextSubGroupDefNumber (rootGroupsDefs.Select (g => g.PathTemplate), prefixChar);
			aiderGroupDef.PathTemplate = AiderGroupIds.CreateTopLevelPathTemplate (number);

			aiderGroupDef.Classification = groupClass;
			aiderGroupDef.Mutability = isMutable ? Mutability.Customizable : Mutability.None;

			return aiderGroupDef;
		}

		public static AiderGroupDefEntity FindParent(BusinessContext businessContext, AiderGroupDefEntity entity)
		{
			var oldParentPath = AiderGroupIds.GetParentPath (entity.PathTemplate);
			var example = new AiderGroupDefEntity ()
			{
				PathTemplate = oldParentPath
			};

			return businessContext.GetByExample<AiderGroupDefEntity>(example).First ();
		}

		public static IEnumerable<AiderGroupDefEntity> FindFunctions(BusinessContext businessContext)
		{
			var dataContext = businessContext.DataContext;

			var example = new AiderGroupDefEntity ();
			var request = Request.Create (example);

			var path  = AiderGroupIds.CreateTopLevelPathTemplate (GroupClassification.Function) + "____.____.";

			request.AddCondition (dataContext, example, x => SqlMethods.Like (x.PathTemplate, path));

			return dataContext.GetByRequest (request);
		}

		public static IEnumerable<AiderGroupDefEntity> FindGroupDefByFunction (BusinessContext businessContext, AiderGroupDefEntity functionDef)
		{
			if(!functionDef.IsFunction ())
			{
				throw new BusinessRuleException ("Action impossible sur cette définition de groupe");
			}

			var dataContext = businessContext.DataContext;

			var example = new AiderGroupDefEntity ()
			{
				Function = functionDef
			};

			return dataContext.GetByExample <AiderGroupDefEntity> (example);
		}

		public bool IsChildOf(AiderGroupDefEntity groupDef)
		{
			return AiderGroupIds.IsSameOrWithinGroup (this.PathTemplate, groupDef.PathTemplate)
				&& this.Level > groupDef.Level;
		}

		public bool IsRegion()
		{
			return this.Level == AiderGroupIds.RegionLevel
				&& this.Classification == GroupClassification.Region;
		}

		public bool IsParish()
		{
			return this.Level == AiderGroupIds.ParishLevel
				&& this.Classification == GroupClassification.Parish;
		}

		public bool IsFunction()
		{
			return this.Level > 0 && this.Classification == GroupClassification.Function;
		}

		public bool IsParishOfGermanLanguage()
		{
			return this.Level == AiderGroupIds.ParishOfGermanLanguageLevel
				&& this.Classification == GroupClassification.ParishOfGermanLanguage;
		}

		public bool IsNoParish()
		{
			return this.Level == AiderGroupIds.NoParishLevel
				&& this.Classification == GroupClassification.NoParish;
		}

		public bool IsGlobal()
		{
			return this.Level == AiderGroupIds.TopLevel
				&& this.Classification == GroupClassification.None;
		}

		public static IList<AiderGroupDefEntity> FindRootGroupsDefinitions(BusinessContext businessContext)
		{
			var dataContext = businessContext.DataContext;

			var example = new AiderGroupDefEntity ()
			{
				Level = AiderGroupIds.TopLevel,
			};

			var request = Request.Create (example);
			request.AddSortClause (ValueField.Create (example, x => x.Name));

			return dataContext.GetByRequest (request);
		}
	}
}
