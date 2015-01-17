//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel Loup, Maintainer: Samuel Loup

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.ECh
{
	internal class EChDataAnalyzerReporter : EChDataAnalyzer
	{
		public EChDataAnalyzerReporter(string oldEchFile, string newEchFile, string reportFile)
			: base (oldEchFile, newEchFile)
		{
			this.reportFile = reportFile;

			try
			{
				this.AnalyseAllChangesAndReportTrace ();
				this.DisposeReporting ();
			}
			catch
			{
				throw;
			}

		}

		public void CreateReport()
		{
			System.Console.WriteLine ("ECH DATA UPDATER : CREATING REPORT OF CHANGES ON " + this.reportFile);
			//REPORT IN MARKDOWN (offline markdown reader: http://stackoverflow.com/questions/9843609/view-md-file-offline)
			using (System.IO.TextWriter tw = new System.IO.StreamWriter (this.reportFile))
			{
				tw.WriteLine ("# Rapport Analyse ECH du " + System.DateTime.Now);

				tw.WriteLine ("## R�sum� des d�parts et arriv�es");
				tw.WriteLine (this.NewFamily.Count + " familles sans enfants sont arriv�es dans le registre");
				tw.WriteLine ("");
				tw.WriteLine (this.MissingFamily.Count + " familles sans enfants sont sorties du registre");
				tw.WriteLine ("");
				tw.WriteLine (this.NewFamilyWithChildren.Count + " familles avec enfants sont arriv�es dans le registre");
				tw.WriteLine ("");
				tw.WriteLine (this.MissingFamilyWithChildren.Count + " familles avec enfants sont sorties du registre");
				tw.WriteLine ("");
				tw.WriteLine (this.NewFamilyMonoWithChildren.Count + " familles monoparentales avec enfants sont arriv�es dans le registre");
				tw.WriteLine ("");
				tw.WriteLine (this.MissingFamilyMonoWithChildren.Count + " familles monoparentales avec enfants sont sorties du registre");
				tw.WriteLine ("");
				tw.WriteLine (this.NewFamilyMono.Count + " personnes seules sont arriv�es dans le registre");
				tw.WriteLine ("");
				tw.WriteLine (this.MissingFamilyMono.Count + " personnes seules sont sorties du registre");
				tw.WriteLine ("");
				tw.WriteLine ("## R�sum� des cas impactants les familles");
				tw.WriteLine (this.NewUnion.Count + " unions");
				tw.WriteLine ("");
				tw.WriteLine (this.MissingUnion.Count + " s�paratations");
				tw.WriteLine ("");
				tw.WriteLine (this.WidowFamily.Count + " famille avec perte du conjoint");
				tw.WriteLine ("");
				tw.WriteLine (this.ChildrenMove.Count + " familles dont le ou les enfants a/ont chang� de foyer");
				tw.WriteLine ("");
				tw.WriteLine (this.GainMajority.Count + " cas de majorit�");
				tw.WriteLine ("");
				tw.WriteLine (this.NewChildren.Count + " enfants ajout�s au registre");
				tw.WriteLine ("");
				tw.WriteLine (this.ChildrenMissing.Count + " enfants sortis du registre");
				tw.WriteLine ("");


				tw.WriteLine ("## R�sum� des cas non-r�solus");
				tw.WriteLine (this.AddCaseToResolve.Count + " ajouts non-r�solus");
				tw.WriteLine ("");

				tw.WriteLine ("## D�tail des arriv�es");


				tw.WriteLine ("### Nouvelles familles sans enfants dans le registre (" + this.NewFamily.Count + ")");
				foreach (var family in this.NewFamily)
				{
					tw.WriteLine ("#### Famille " + family.Adult1.OfficialName);
					tw.WriteLine (family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
					tw.WriteLine ("");
					tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
					tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
					tw.WriteLine ("");
				}

				tw.WriteLine ("### Nouvelles familles avec enfants dans le registre (" + this.NewFamilyWithChildren.Count + ")");
				foreach (var family in this.NewFamilyWithChildren)
				{
					tw.WriteLine ("#### Famille " + family.Adult1.OfficialName);
					tw.WriteLine (family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
					tw.WriteLine ("");
					tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
					tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
					tw.WriteLine ("");
					foreach (var child in family.Children)
					{
						tw.WriteLine (" * (E) " + child.FirstNames + " " + child.OfficialName);
					}
				}

				tw.WriteLine ("### Nouvelles familles monoparentales dans le registre (" + this.NewFamilyMonoWithChildren.Count + ")");
				foreach (var family in this.NewFamilyMonoWithChildren)
				{
					tw.WriteLine ("#### Famille " + family.Adult1.OfficialName);
					tw.WriteLine (family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
					tw.WriteLine ("");
					tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
					if (family.Adult2 != null)
					{
						tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
					}
					tw.WriteLine ("");
					foreach (var child in family.Children)
					{
						tw.WriteLine (" * (E) " + child.FirstNames + " " + child.OfficialName);
					}
				}

				tw.WriteLine ("### Nouvelles personnes seules dans le registre (" + this.NewFamilyMono.Count + ")");
				foreach (var family in this.NewFamilyMono)
				{
					tw.WriteLine ("* (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
					if (family.Adult2 != null)
					{
						tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
					}
				}

				tw.WriteLine ("### Nouveaux enfants (" + this.NewChildren.Count + ")");
				foreach (var child in this.NewChildren)
				{
					tw.WriteLine ("#### Famille " + child.Item1.Adult1.OfficialName);
					tw.WriteLine (" * (E) " + child.Item2.FirstNames + " " + child.Item2.OfficialName + " n� le " + child.Item2.DateOfBirth);
				}


				tw.WriteLine ("### Majorit�s (" + this.GainMajority.Count + ")");
				foreach (var family in this.GainMajority)
				{
					tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " n� le " + family.Adult1.DateOfBirth);
				}

				tw.WriteLine ("## D�tail des d�parts");

				tw.WriteLine ("### D�part de familles sans enfants dans le registre (" + this.MissingFamily.Count + ")");
				foreach (var family in this.MissingFamily)
				{
					tw.WriteLine ("#### Famille " + family.Adult1.OfficialName);
					tw.WriteLine (family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
					tw.WriteLine ("");
					tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
					tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
					tw.WriteLine ("");
					foreach (var child in family.Children)
					{
						tw.WriteLine (" * (E) " + child.FirstNames + " " + child.OfficialName);
					}
				}

				tw.WriteLine ("### D�part de familles avec enfants dans le registre (" + this.MissingFamilyWithChildren.Count + ")");
				foreach (var family in this.MissingFamilyWithChildren)
				{
					tw.WriteLine ("#### Famille " + family.Adult1.OfficialName);
					tw.WriteLine (family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
					tw.WriteLine ("");
					tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
					tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
					tw.WriteLine ("");
					foreach (var child in family.Children)
					{
						tw.WriteLine (" * (E) " + child.FirstNames + " " + child.OfficialName);
					}
				}

				tw.WriteLine ("### D�part de familles monoparentales dans le registre (" + this.MissingFamilyMonoWithChildren.Count + ")");
				foreach (var family in this.MissingFamilyMonoWithChildren)
				{
					tw.WriteLine ("#### Famille " + family.Adult1.OfficialName);
					tw.WriteLine (family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
					tw.WriteLine ("");
					tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
					if (family.Adult2 != null)
					{
						tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
					}
					tw.WriteLine ("");
					foreach (var child in family.Children)
					{
						tw.WriteLine (" * (E) " + child.FirstNames + " " + child.OfficialName);
					}
				}

				tw.WriteLine ("### Personnes seules sortantes du registre (" + this.MissingFamilyMono.Count + ")");
				foreach (var family in this.MissingFamilyMono)
				{
					tw.WriteLine ("* (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
					if (family.Adult2 != null)
					{
						tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
					}
				}

				tw.WriteLine ("### Enfants sortis du registre (" + this.ChildrenMissing.Count + ")");
				foreach (var child in this.ChildrenMissing)
				{
					tw.WriteLine ("#### Famille " + child.Item1.Adult1.OfficialName);
					tw.WriteLine (" * (E) " + child.Item2.FirstNames + " " + child.Item2.OfficialName + " n� le " + child.Item2.DateOfBirth);
				}


				tw.WriteLine ("## D�tail des cas impactants les familles");

				tw.WriteLine ("### Nouvelles union (" + this.NewUnion.Count + ")");
				foreach (var family in this.NewUnion)
				{
					tw.WriteLine ("#### Famille " + family.Adult1.OfficialName);
					tw.WriteLine (family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
					tw.WriteLine ("");
					tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
					tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
					foreach (var child in family.Children)
					{
						tw.WriteLine (" * (E) " + child.FirstNames + " " + child.OfficialName);
					}
				}

				tw.WriteLine ("### S�parations / perte d'un conjoint (" + this.MissingUnion.Count + ")");
				foreach (var family in this.MissingUnion)
				{
					tw.WriteLine ("#### Famille " + family.Adult1.OfficialName);
					tw.WriteLine (family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
					tw.WriteLine ("");
					tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
					if (family.Adult2 != null)
					{
						tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
					}
					foreach (var child in family.Children)
					{
						tw.WriteLine (" * (E) " + child.FirstNames + " " + child.OfficialName);
					}
				}
				tw.Close ();
				System.Console.WriteLine ("ECH DATA UPDATER : DONE!");
			}
		}

		private void AnalyseAllChangesAndReportTrace()
		{
			System.IO.TextWriter tw = new System.IO.StreamWriter (this.reportFile);

			tw.WriteLine ("# Analyser Trace " + System.DateTime.Now);
			//We check all the family to add in the register,
			//this addition can result from an replacement of an old record
			//or be totaly new
			tw.WriteLine ("## For Each FamilyToAdd");
			foreach (var family in FamilyToAdd.Values)
			{
				EChReportedPerson removedFamily = null;
				var monoParental = false;           //household with only one adult
				var hasChildren = false;            //child detected
				var coreStructuralChange = false;   //indicate that the core family structure has changed (adult1 and adult2 composition)
				var unclassified = false;           //indicate that the analyser fail to find a matching "add change" case

				var isNew = false;                  //the family is new in the register
				var isSeparation = false;           //the family structure has change one adult missing now          
				var isWidow = false;                //special case
				var isMajority = false;             //special case
				var isMajoritySideEffect = false;   //special case
				var isChildMove = false;            //special case
				var isChildMissing = false;         //special case
				var isNewBorn = false;              //special case
				var isUnion = false;                //special case


				var composition = string.Join (" ", family.GetMembers ().Select (m => m.Id));



				tw.WriteLine ("### Family Key: " + composition);
				tw.WriteLine ("");

				if (family.Adult2 == null)
				{
					monoParental = true;
					tw.WriteLine ("MonoParentalStructure: " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
					tw.WriteLine ("");
				}
				else
				{
					tw.WriteLine ("StdParentalStructure: " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " & " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
					tw.WriteLine ("");
				}

				if (family.Children.Count > 0)
				{
					hasChildren = true;
					tw.WriteLine ("With Children");
					tw.WriteLine ("");
				}


				/*
				var oldFamiliesMatched = family.GetAdults ().Select (ad => ad.Id).Select (id => FamilyToRemove.ContainsKey(id) ? FamilyToRemove[id] : null).Where(rec => rec != null).Distinct ().ToList();

				//0
				if (oldFamiliesMatched.Count == 0)
				{
					this.NewHouseHolds.Add (family);
					foreach (var member in family.GetMembers())
					{
						this.PersonMovedIn.GetOrCreateList (member).Add (family);
					}
				}

				//1
				if (oldFamiliesMatched.Count == 1)
				{
					var oldFamily = oldFamiliesMatched.Single ();
					var membersMissing = oldFamily.GetMembers ().Except (family.GetMembers (), EChDataAnalyser.PersonComparer);
					foreach (var member in membersMissing)
					{
						this.PersonMovedOut.GetOrCreateList (member).Add (oldFamily);
					}

					var newMembers = family.GetMembers ().Except (oldFamily.GetMembers (), EChDataAnalyser.PersonComparer);
					foreach (var member in newMembers)
					{
						this.PersonMovedIn.GetOrCreateList (member).Add (family);
					}
				}
				//2
				if (oldFamiliesMatched.Count == 2)
				{

					foreach (var oldFamily in oldFamiliesMatched)
					{


					}

				}
				*/
				//Union and Separation Detector

				if (monoParental)
				{
					if (this.FamilyToRemove.ContainsKey (family.Adult1.Id))//if we found the opposed record to remove
					{
						coreStructuralChange = true;
						removedFamily = this.FamilyToRemove[family.Adult1.Id];

						var oldComposition = string.Join (" ", removedFamily.GetMembers ().Select (m => m.Id));

						tw.WriteLine ("we found the opposed record to remove, with family key: " + oldComposition);
						tw.WriteLine ("");

						if (removedFamily.Adult2 != null)//if the old record has an second adult
						{
							isSeparation = true;
							tw.WriteLine ("the old record has an second adult: " + removedFamily.Adult2.FirstNames + " " + removedFamily.Adult2.OfficialName);
							tw.WriteLine ("");
							tw.WriteLine ("separation detected!");
							tw.WriteLine ("");
						}
					}
					else
					{
						tw.WriteLine ("no opposed record found");
						tw.WriteLine ("");
						if (this.PersonToAdd.ContainsKey (family.Adult1.Id))//if the adult is in the new person list
						{
							tw.WriteLine ("the adult is in the new person list -> NEW FAMILY");
							tw.WriteLine ("");
							isNew = true;
						}
						else //in this case an child or an adult2 is now as first adult
						{
							tw.WriteLine ("in this case an child or an adult2 is now as first adult");
							tw.WriteLine ("");
							if (System.DateTime.Now.Year - family.Adult1.DateOfBirth.Year == 18)//if majority condition match
							{
								tw.WriteLine ("majority condition match -> MAJORITY");
								tw.WriteLine ("");
								isMajority = true;
							}
							else// in this case, this adult1 was an adult2
							{
								tw.WriteLine ("no majority condition match -> WIDOW");
								tw.WriteLine ("");
								isWidow = true;
							}
						}
					}
				}
				else //in case of two adult household
				{
					tw.WriteLine ("try to fing found the opposed record to remove, with adult1 Id and then with adult2");
					tw.WriteLine ("");
					if (this.FamilyToRemove.ContainsKey (family.Adult1.Id))// if the first adult is found in opposed record to remove
					{
						coreStructuralChange = true;
						removedFamily = this.FamilyToRemove[family.Adult1.Id];

						var oldComposition = string.Join (" ", removedFamily.GetMembers ().Select (m => m.Id));

						tw.WriteLine ("we found the opposed record to remove with adult1, with family key: " + oldComposition);
						tw.WriteLine ("");

						if (removedFamily.Adult2 != null)//if the old record has an second adult
						{
							tw.WriteLine ("the old record has an second adult: " + removedFamily.Adult2.FirstNames + " " + removedFamily.Adult2.OfficialName);
							tw.WriteLine ("");
							if (!removedFamily.Adult2.Id.Equals (family.Adult2.Id))//if this second adult is different to the actual second adult
							{
								tw.WriteLine ("this second adult is different to the actual second adult -> UNION");
								tw.WriteLine ("");
								isUnion = true;
							}
						}
						else //we don't have a second adult in the old record
						{
							tw.WriteLine ("we don't have a second adult in the old record -> UNION");
							tw.WriteLine ("");
							isUnion = true;
						}
					}
					else if (this.FamilyToRemove.ContainsKey (family.Adult2.Id)) // if the second adult is found in opposed record
					{
						coreStructuralChange = true;
						removedFamily = this.FamilyToRemove[family.Adult2.Id];

						var oldComposition = string.Join (" ", removedFamily.GetMembers ().Select (m => m.Id));

						tw.WriteLine ("we found the opposed record to remove with adult2, with family key: " + oldComposition);
						tw.WriteLine ("");
						if (removedFamily.Adult2 != null) // if the old record has a second adult
						{
							tw.WriteLine ("the old record has an second adult: " + removedFamily.Adult2.FirstNames + " " + removedFamily.Adult2.OfficialName);
							tw.WriteLine ("");
							if (!removedFamily.Adult2.Id.Equals (family.Adult1.Id)) //if this second adult is different to the actual first adult
							{
								tw.WriteLine ("this second adult is different to the actual first adult -> UNION");
								tw.WriteLine ("");
								isUnion = true;
							}
						}
						else //if we don't have a second adult in the old record
						{
							tw.WriteLine ("we don't have a second adult in the old record -> UNION");
							tw.WriteLine ("");
							isUnion = true;
						}
					}
					else //if we don't found any opposed record
					{
						tw.WriteLine ("we don't found any opposed record");
						tw.WriteLine ("");
						if (this.PersonToAdd.ContainsKey (family.Adult1.Id) && PersonToAdd.ContainsKey (family.Adult2.Id))//if two adults are new
						{
							tw.WriteLine ("two adults are new -> NEW FAMILY");
							tw.WriteLine ("");
							isNew = true;
						}
						else // in this case it's unclassified
						{
							tw.WriteLine ("this case it's unclassified");
							tw.WriteLine ("");
							unclassified = true;
						}
					}

				}

				tw.WriteLine ("Check Childrens Changes on changed core structure");
				tw.WriteLine ("");


				//Check Childrens Changes on changed core structure
				var oldChildren = removedFamily == null ? Enumerable.Empty<EChPerson> () : removedFamily.Children;
				var addedChildren = family.Children.Except (oldChildren, EChDataAnalyzer.PersonComparer);

				foreach (var child in addedChildren)
				{
					//Result
					List<EChReportedPerson> list;
					if (!this.PersonMovedIn.TryGetValue (child, out list))
					{
						list = new List<EChReportedPerson> ();
						this.PersonMovedIn[child] = list;
					}
					list.Add (family);

					if (this.PersonToAdd.ContainsKey (child.Id))//if the child is new
					{
						tw.WriteLine ("the child is new -> NEW BORN");
						tw.WriteLine ("");
						isNewBorn = true;
						this.NewChildren.Add (System.Tuple.Create (family, child));
						this.NewPersons.Add (child);
					}
					else
					{
						tw.WriteLine ("no new child -> CHILD MOVE");
						tw.WriteLine ("");
						isChildMove = true;
						this.ChildrenMove.Add (System.Tuple.Create (family, child));
					}

				}

				var removedChildren = oldChildren.Except (family.Children, EChDataAnalyzer.PersonComparer);

				foreach (var child in removedChildren)
				{
					//Result
					List<EChReportedPerson> list;
					if (!this.PersonMovedOut.TryGetValue (child, out list))
					{
						list = new List<EChReportedPerson> ();
						this.PersonMovedOut[child] = list;
					}
					list.Add (removedFamily);


					if (this.PersonToRemove.ContainsKey (child.Id))
					{
						tw.WriteLine ("person to remove contain child -> CHILD MISSING");
						tw.WriteLine ("");
						isChildMissing = true;
						this.ChildrenMissing.Add (System.Tuple.Create (removedFamily, child));
						this.MissingPersons.Add (child);
					}
					else
					{
						if (this.FamilyToAdd.ContainsKey (child.Id))//if we found the child as family
						{
							tw.WriteLine ("we found the child as family -> MAJORITY SIDE EFFECT");
							tw.WriteLine ("");
							isMajoritySideEffect = true;
							this.ChildrenLeaveHouseholdForMajority.Add (System.Tuple.Create (removedFamily, child));
						}
						else
						{
							tw.WriteLine ("no child to remove, and no majority -> CHILD MOVE");
							tw.WriteLine ("");
							isChildMove = true;
							this.ChildrenMove.Add (System.Tuple.Create (removedFamily, child));
						}
					}
				}

				//Classification for reporting purpose
				if (!unclassified)
				{
					if (isNew)
					{
						//Result 
						this.NewHouseHolds.Add (family);

						if (monoParental)
						{
							if (hasChildren)
							{
								this.NewFamilyMonoWithChildren.Add (family);
							}
							else
							{
								this.NewFamilyMono.Add (family);
							}

						}
						else
						{
							if (hasChildren)
							{
								this.NewFamilyWithChildren.Add (family);
							}
							else
							{
								this.NewFamily.Add (family);
							}
						}
					}

					if (isSeparation)
					{
						this.MissingUnion.Add (family);
					}

					if (isWidow)
					{
						this.WidowFamily.Add (family);
					}

					if (isUnion)
					{
						this.NewUnion.Add (family);
					}

					if (isMajority)
					{
						this.GainMajority.Add (family);
					}

					if (isNewBorn)
					{
						this.FamilyWithNewChildren.Add (family);
					}

					if (isChildMissing)
					{
						this.FamilyWithChildMissing.Add (family);
					}

				}
				else//for unclassified case
				{
					if (!isMajoritySideEffect && !isChildMove && !isWidow)//exclude some special case
					{
						this.AddCaseToResolve.Add (family);//add to the resolve list
					}

				}
			}

			//Analyse family to remove for finding family deletion
			foreach (var family in FamilyToRemove.Values)
			{
				var monoParental = false;
				var hasChildren = false;
				var hasChange = false;

				if (family.Adult2 == null)
				{
					monoParental = true;
				}

				if (family.Children.Count > 0)
				{
					hasChildren = true;
				}

				//we check for a change in the family to add (opposed side)
				if (monoParental)
				{
					if (this.FamilyToAdd.ContainsKey (family.Adult1.Id))
					{
						hasChange = true;
					}
				}
				else
				{
					if (this.FamilyToAdd.ContainsKey (family.Adult1.Id))
					{
						hasChange = true;


					}
					else if (this.FamilyToAdd.ContainsKey (family.Adult2.Id))
					{
						hasChange = true;
					}

				}

				if (!hasChange)//if nothing found in family to add
				{
					if (monoParental)
					{
						if (this.PersonToRemove.ContainsKey (family.Adult1.Id))//if we found the person to remove
						{
							this.MissingHouseHolds.Add (family);
							if (hasChildren)//and have children
							{
								this.MissingFamilyMonoWithChildren.Add (family);
							}
							else
							{
								this.MissingFamilyMono.Add (family);
							}

						}
					}
					else
					{
						if (this.PersonToRemove.ContainsKey (family.Adult1.Id) && this.PersonToRemove.ContainsKey (family.Adult2.Id))
						{
							this.MissingHouseHolds.Add (family);
							if (hasChildren)
							{
								this.MissingFamilyWithChildren.Add (family);
							}
							else
							{
								this.MissingFamily.Add (family);
							}

						}
					}

				}
			}

			tw.Close ();
		}

		private readonly string					reportFile;
	}
}

