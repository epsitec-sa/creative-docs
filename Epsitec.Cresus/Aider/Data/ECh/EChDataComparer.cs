//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel Loup, Maintainer: Samuel Loup


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epsitec.Aider.Data.ECh
{

	/// <summary>
	/// Compare Ech XML data files, provide extraction methods for diff based on buisiness rules
	/// </summary>
	class EChDataComparer
	{
		
		/// <summary>
		/// Initialize a new EChDataComparer with two files
		/// </summary>
		/// <param name="OldEchFile">old ECh XML data file</param>
		/// <param name="NewEchFile">new ECh XML data file</param>
		public EChDataComparer(string OldEchFile,string NewEchFile)
		{
			this.EchFileA = OldEchFile;
			this.EchFileB = NewEchFile;

			this.LoadFilesToCompare ();
			this.CreateDictionaryFromEntity ();
		}

		/// <summary>
		/// Create a list of ECh ReportedPerson to remove from database
		/// </summary>
		/// <returns>a list of ReportedPerson to remove</returns>
		public List<EChReportedPerson> GetFamilyToRemove()
		{
			var FamilyToBeRemoved = (from e in DicFamilyA
									 where !DicFamilyB.ContainsKey (e.Key)
									 select e.Value).ToList ();

			return FamilyToBeRemoved;
		}

		/// <summary>
		/// Create a list of ECh ReportedPerson to add in the database
		/// </summary>
		/// <returns>a list of ReportedPerson to add</returns>
		public List<EChReportedPerson> GetFamilyToAdd()
		{
			var FamilyToBeAdded = (from n in DicFamilyB
								   where !DicFamilyA.ContainsKey (n.Key)
								   select n.Value).ToList ();

			return FamilyToBeAdded;
		}

		/// <summary>
		/// Create a list of ECh ReportedPerson to modify
		/// </summary>
		/// <returns>a list of modified ECh ReportedPerson</returns>
		public List<System.Tuple<EChReportedPerson,EChReportedPerson>> GetFamilyToChange()
		{
			var FamilyToCheck = from e in DicFamilyB
								where DicFamilyA.ContainsKey (e.Key)
								select e;

			var FamilyToChange = (from c in FamilyToCheck
								  join e in DicFamilyA on c.Key equals e.Key
								  where !c.Value.CheckData (e.Value.Address.HouseNumber, e.Value.Address.CountryCode, e.Value.Address.AddressLine1, e.Value.Address.Street, e.Value.Address.SwissZipCode, e.Value.Address.SwissZipCodeAddOn, e.Value.Address.SwissZipCodeId, e.Value.Address.Town)
								  select System.Tuple.Create(c.Value, e.Value)).ToList();

			return FamilyToChange;
		}

		/// <summary>
		/// Create a list of EChPerson to remove from database
		/// </summary>
		/// <returns>a list of EChPerson to remove</returns>
		public List<EChPerson> GetPersonToRemove()
		{
			var PersonToBeRemoved = (from e in DicPersonA
									 where !DicPersonB.ContainsKey (e.Key)
									 select e.Value).ToList ();

			return PersonToBeRemoved;
		}

		/// <summary>
		/// Create a list of EChPerson to add in the database
		/// </summary>
		/// <returns>a list of EChPerson to add</returns>
		public List<EChPerson> GetPersonToAdd()
		{
			var PersonToBeAdded = (from n in DicPersonB
								   where !DicPersonA.ContainsKey (n.Key)
								   select n.Value).ToList ();

			return PersonToBeAdded;
		}

		/// <summary>
		/// Create a list of tuple containing new EChPerson and old EChPerson
		/// </summary>
		/// <returns>a list of modified EChPerson</returns>
		public List<System.Tuple<EChPerson,EChPerson>> GetPersonToChange()
		{
			var PersonToCheck =  from e in DicPersonB
								 where DicPersonA.ContainsKey (e.Key)
								 select e;

			var PersonToChange = (from c in PersonToCheck
								  join e in DicPersonA on c.Key equals e.Key
								  where !c.Value.CheckData (e.Value.OfficialName, e.Value.FirstNames, e.Value.DateOfBirth, e.Value.Sex, e.Value.NationalityStatus, e.Value.NationalCountryCode, e.Value.MaritalStatus,  e.Value.OriginPlaces)
								  select System.Tuple.Create(c.Value,e.Value)).ToList ();

			return PersonToChange;
		}


		public void AnalyseChanges()
		{
			var familyToAdd = new Dictionary<string, EChReportedPerson> ();
			var familyToRemove = new Dictionary<string, EChReportedPerson> ();
			var personToAdd = new Dictionary<string, EChPerson> ();
			var personToRemove = new Dictionary<string, EChPerson> ();

			var newUnion= new List<EChReportedPerson> ();
			var newFamily = new List<EChReportedPerson> ();
			var newFamilyWithChildren = new List<EChReportedPerson> ();
			var newFamilyMono = new List<EChReportedPerson> ();
			var newFamilyMonoWithChildren = new List<EChReportedPerson>();
			var familyWithNewChilds = new List<EChReportedPerson>();
			var familyWithChildMissing = new List<EChReportedPerson> ();
			var newChilds = new List<Tuple<EChReportedPerson,EChPerson>> ();
			var childMissing = new List<Tuple<EChReportedPerson, EChPerson>> ();
			var gainMajority = new List<EChReportedPerson> ();
			var missingUnion= new List<EChReportedPerson> ();
			var missingFamily = new List<EChReportedPerson> ();
			var missingFamilyWithChildren = new List<EChReportedPerson> ();
			var missingFamilyMono = new List<EChReportedPerson> ();
			var missingFamilyMonoWithChildren = new List<EChReportedPerson> ();
			var errorFamily = new List<EChReportedPerson> ();

			var addCaseToResolve = new List<EChReportedPerson> ();



			foreach (var person in this.GetPersonToAdd())
			{
				personToAdd.Add(person.Id, person);
			}

			foreach (var person in this.GetPersonToRemove())
			{
				personToRemove.Add(person.Id, person);
			}

			foreach (var family in this.GetFamilyToAdd())
			{
				familyToAdd.Add(family.Adult1.Id, family);
			}

			foreach (var family in this.GetFamilyToRemove())
			{
				familyToRemove.Add (family.Adult1.Id, family);
			}



			foreach (var family in familyToAdd.Values)
			{

				EChReportedPerson removedFamily = null;
				var monoParental = false;
				var hasChildren = false;
				var structuralChange = false;
				var unclassified = false;

				var isNew = false;
				var isDivided = false;
				var isWidow = false;
				var isMajority = false;
				var isMajoritySideEffect = false;
				var isChildMove = false;
				var isChildMissing = false;
				var isNewBorn = false;
				var isUnion = false;


				if (family.Adult2 == null)
				{
					monoParental = true;
				}

				if (family.Children.Count > 0)
				{
					hasChildren = true;
				}

				if (familyToRemove.ContainsKey (family.Adult1.Id))
				{
					structuralChange = true;
					removedFamily = familyToRemove[family.Adult1.Id];
				}

				//check cases
				if (!structuralChange)
				{
					if (monoParental)
					{
						if (personToAdd.ContainsKey (family.Adult1.Id))
						{
							isNew = true;
						}
						else
						{
							if (DateTime.Now.Year - family.Adult1.DateOfBirth.Year == 18)
							{
								isMajority = true;
							}
							else
							{
								isWidow = true;
							}
						}
					}
					else
					{
						if (personToAdd.ContainsKey (family.Adult1.Id) && personToAdd.ContainsKey (family.Adult2.Id))
						{
							isNew = true;
						}
						else if (personToAdd.ContainsKey (family.Adult1.Id)&&!personToAdd.ContainsKey (family.Adult2.Id))
						{
							isUnion = true;
						}
						else
						{
							unclassified = true;
						}
					}

				}
				else
				{
					if (hasChildren)
					{
						if (family.Children.Count > removedFamily.Children.Count)
						{
							foreach (var child in family.Children)
							{
								if (personToAdd.ContainsKey (child.Id))
								{
									isNewBorn = true;
									newChilds.Add (Tuple.Create(family,child));
								}
								else
								{
									isChildMove = true;
								}
							}
						}
						if (family.Children.Count < removedFamily.Children.Count)
						{
							foreach (var child in family.Children)
							{
								if (personToRemove.ContainsKey (child.Id))
								{
									isChildMissing = true;
								}
								else
								{
									if (familyToAdd.ContainsKey (child.Id))
									{
										isMajoritySideEffect = true;
									}
									else
									{
										isChildMove = true;
									}

								}
							}
						}
					}
					else
					{
						if (family.Children.Count < removedFamily.Children.Count)
						{
							foreach (var child in family.Children)
							{
								if (personToRemove.ContainsKey (child.Id))
								{
									isChildMissing = true;
								}
								else
								{
									if (familyToAdd.ContainsKey (child.Id))
									{
										isMajoritySideEffect = true;
									}
									else
									{
										isChildMove = true;
									}
								}
							}
						}
					}
					
					if (monoParental)
					{				
						if (removedFamily.Adult2!=null)
						{
							isDivided = true;
						}
					}
					else
					{
						if (removedFamily.Adult2==null)
						{
							isUnion = true;
						}
					}
				}


				if (!unclassified)
				{
					if (isNew)
					{
						if (monoParental)
						{
							if (hasChildren)
							{
								newFamilyMonoWithChildren.Add (family);
							}
							else
							{
								newFamilyMono.Add (family);
							}
							
						}
						else
						{
							if (hasChildren)
							{
								newFamilyWithChildren.Add (family);
							}
							else
							{
								newFamily.Add (family);
							}
						}
					}

					if (isDivided)
					{
						missingUnion.Add (family);
					}

					if (isWidow)
					{
						missingUnion.Add (family);
					}

					if (isUnion)
					{
						newUnion.Add (family);
					}

					if (isMajority)
					{
						gainMajority.Add (family);
					}

					if (isNewBorn)
					{
						familyWithNewChilds.Add (family);
					}

					if (isChildMissing)
					{
						familyWithChildMissing.Add (family);
					}
				}
				else
				{
					if (!isMajoritySideEffect&&!isChildMove&&!isWidow)
					{
						addCaseToResolve.Add (family);
					}
					
				}
			}
				

			foreach (var family in familyToRemove.Values)
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

				if(familyToAdd.ContainsKey(family.Adult1.Id))
				{
					if (family.Adult2 != null)
					{
						if (familyToAdd.ContainsKey (family.Adult2.Id))
						{
							hasChange = true;
						}
					}
					else
					{
						hasChange = true;
					}
					
				}

				if (!hasChange)
				{
					if (monoParental)
					{
						if (personToRemove.ContainsKey (family.Adult1.Id))
						{
							if (hasChildren)
							{
								missingFamilyMonoWithChildren.Add (family);
							}
							else
							{
								missingFamilyMono.Add (family);
							}
							
						}
					}
					else
					{
						if (personToRemove.ContainsKey (family.Adult1.Id) && personToRemove.ContainsKey (family.Adult2.Id))
						{
							if (hasChildren)
							{
								missingFamilyWithChildren.Add (family);
							}
							else
							{
								missingFamily.Add (family);
							}

						}
					}
					
				}
				else
				{
					//not treat
				}
			}


			//A LITTLE REPORT IN MARKDOWN ;)
			TextWriter tw = new StreamWriter("s:\\EChUpdateAnalyse.md");

			tw.WriteLine("# Rapport Analyse ECH du " + DateTime.Now);

			tw.WriteLine ("## Résumé des départs et arrivées");
			tw.WriteLine (newFamily.Count + " familles sans enfants sont arrivées dans le registre");
			tw.WriteLine ("");
			tw.WriteLine (missingFamily.Count + " familles sans enfants sont sorties du registre");
			tw.WriteLine ("");
			tw.WriteLine (newFamilyWithChildren.Count + " familles avec enfants sont arrivées dans le registre");
			tw.WriteLine ("");
			tw.WriteLine (missingFamilyWithChildren.Count + " familles avec enfants sont sorties du registre");
			tw.WriteLine ("");
			tw.WriteLine (newFamilyMonoWithChildren.Count + " familles monoparentales avec enfants sont arrivées dans le registre");
			tw.WriteLine ("");
			tw.WriteLine (missingFamilyMonoWithChildren.Count + " familles monoparentales avec enfants sont sorties du registre");
			tw.WriteLine ("");
			tw.WriteLine (newFamilyMono.Count + " personnes seules sont arrivées dans le registre");
			tw.WriteLine ("");
			tw.WriteLine (missingFamilyMono.Count + " personnes seules sont sorties du registre");
			tw.WriteLine ("");
			tw.WriteLine ("## Résumé des cas impactants les familles");
			tw.WriteLine (newUnion.Count + " unions");
			tw.WriteLine ("");
			tw.WriteLine (missingUnion.Count + " séparatations");
			tw.WriteLine ("");
			tw.WriteLine (gainMajority.Count + " cas de majorité");
			tw.WriteLine ("");
			tw.WriteLine (newChilds.Count + " enfants ajoutés au registre");
			tw.WriteLine ("");
			tw.WriteLine (childMissing.Count + " enfants sortis du registre");
			tw.WriteLine ("");


			tw.WriteLine ("## Résumé des cas non-résolus");
			tw.WriteLine(addCaseToResolve.Count + " ajouts non-résolus");
			tw.WriteLine("");
			
			tw.WriteLine ("## Détail des arrivées");

			tw.WriteLine ("### Nouvelles familles sans enfants dans le registre (" + newFamily.Count + ")");
			foreach (var family in newFamily)
			{
				tw.WriteLine ("#### Famille " + family.Adult1.OfficialName);
				tw.WriteLine (" * (A) "+ family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
				tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
				tw.WriteLine ("");
				foreach (var child in family.Children)
				{
					tw.WriteLine (" * (E) " + child.FirstNames + " " + child.OfficialName);
				}
			}

			tw.WriteLine("### Nouvelles familles avec enfants dans le registre (" + newFamilyWithChildren.Count + ")");
			foreach (var family in newFamilyWithChildren)
			{
				tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
				tw.WriteLine(" * (A) "+ family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
				tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
				tw.WriteLine("");
				foreach (var child in family.Children)
				{
					tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
				}
			}

			tw.WriteLine("### Nouvelles familles monoparentales dans le registre (" + newFamilyMonoWithChildren.Count + ")");
			foreach (var family in newFamilyMonoWithChildren)
			{
				tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
				tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
				if (family.Adult2 != null)
				{
					tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
				}
				tw.WriteLine("");
				foreach (var child in family.Children)
				{
					tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
				}
			}

			tw.WriteLine ("### Nouvelles personnes seules dans le registre (" + newFamilyMono.Count + ")");
			foreach (var family in newFamilyMono)
			{
				tw.WriteLine ("* (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
				if (family.Adult2 != null)
				{
					tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
				}
			}

			tw.WriteLine("### Nouveaux enfants (" + newChilds.Count + ")");
			foreach (var child in newChilds)
			{
				tw.WriteLine ("#### Famille " + child.Item1.Adult1.OfficialName);
				tw.WriteLine(" * (E) " + child.Item2.FirstNames + " " + child.Item2.OfficialName  + " né le " + child.Item2.DateOfBirth);
			}

			
			tw.WriteLine("### Majorités (" + gainMajority.Count + ")");
			foreach (var family in gainMajority)
			{
				tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " né le " + family.Adult1.DateOfBirth);
			}

			tw.WriteLine ("## Détail des départs");

			tw.WriteLine ("### Départ de familles sans enfants dans le registre (" + missingFamily.Count + ")");
			foreach (var family in missingFamily)
			{
				tw.WriteLine ("#### Famille " + family.Adult1.OfficialName);
				tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
				tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
				tw.WriteLine ("");
				foreach (var child in family.Children)
				{
					tw.WriteLine (" * (E) " + child.FirstNames + " " + child.OfficialName);
				}
			}

			tw.WriteLine("### Départ de familles avec enfants dans le registre (" + missingFamilyWithChildren.Count + ")");
			foreach (var family in missingFamilyWithChildren)
			{
				tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
				tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
				tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
				tw.WriteLine("");
				foreach (var child in family.Children)
				{
					tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
				}
			}

			tw.WriteLine("### Départ de familles monoparentales dans le registre (" + missingFamilyMonoWithChildren.Count + ")");
			foreach (var family in missingFamilyMonoWithChildren)
			{
				tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
				tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
				if (family.Adult2 != null)
				{
					tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
				}
				tw.WriteLine("");
				foreach (var child in family.Children)
				{
					tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
				}
			}

			tw.WriteLine("### Personnes seules sortantes du registre (" + missingFamilyMono.Count + ")");
			foreach (var family in missingFamilyMono)
			{
				tw.WriteLine("* (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
				if (family.Adult2 != null)
				{
					tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
				}
			}

			tw.WriteLine ("### Enfants sortis du registre (" + childMissing.Count + ")");
			foreach (var child in childMissing)
			{
				tw.WriteLine ("#### Famille " + child.Item1.Adult1.OfficialName);
				tw.WriteLine (" * (E) " + child.Item2.FirstNames + " " + child.Item2.OfficialName  + " né le " + child.Item2.DateOfBirth);
			}

			
			tw.WriteLine ("## Détail des cas impactants les familles");

			tw.WriteLine ("### Nouvelles union (" + newUnion.Count + ")");
			foreach (var family in newUnion)
			{
				tw.WriteLine ("#### Famille " + family.Adult1.OfficialName);
				tw.WriteLine (" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
				tw.WriteLine (" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
				foreach (var child in family.Children)
				{
					tw.WriteLine (" * (E) " + child.FirstNames + " " + child.OfficialName);
				}
			}

			tw.WriteLine ("### Séparations / perte d'un conjoint (" + missingUnion.Count + ")");
			foreach (var family in missingUnion)
			{
				tw.WriteLine ("#### Famille " + family.Adult1.OfficialName);
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
			tw.Close();

		}

		private void LoadFilesToCompare()
		{
			this.OrigineEch = EChDataComparerLoader.Load (new FileInfo (this.EchFileA), int.MaxValue).ToList ();
			this.VersionEch = EChDataComparerLoader.Load (new FileInfo (this.EchFileB), int.MaxValue).ToList ();
		}

		private void CreateDictionaryFromEntity()
		{

			this.DicFamilyA = new Dictionary<string, EChReportedPerson> ();
			this.DicFamilyB = new Dictionary<string, EChReportedPerson> ();
			this.DicPersonA = new Dictionary<string, EChPerson> ();
			this.DicPersonB = new Dictionary<string, EChPerson> ();

			foreach (EChReportedPerson Fam in OrigineEch)
			{
				this.DicFamilyA.Add (Fam.FamilyKey, Fam);

				this.DicPersonA.Add (Fam.Adult1.Id, Fam.Adult1);
				if (Fam.Adult2!=null)
				{
					this.DicPersonA.Add (Fam.Adult2.Id, Fam.Adult2);
				}
				foreach (EChPerson per in Fam.Children)
				{
					if (!this.DicPersonA.ContainsKey (per.Id))
						this.DicPersonA.Add (per.Id, per);
				}
			}
			

			foreach (EChReportedPerson Fam in VersionEch)
			{
				this.DicFamilyB.Add (Fam.FamilyKey, Fam);
				this.DicPersonB.Add (Fam.Adult1.Id, Fam.Adult1);
				if (Fam.Adult2!=null)
				{
					this.DicPersonB.Add (Fam.Adult2.Id, Fam.Adult2);
				}
				foreach (EChPerson per in Fam.Children)
				{
					if (!this.DicPersonB.ContainsKey (per.Id))
						this.DicPersonB.Add (per.Id, per);
				}
			}
			
		}

		//Filespath
		private string EchFileA;
		private string EchFileB;

		//Entity List
		private List<EChReportedPerson> OrigineEch;
		private List<EChReportedPerson> VersionEch;


		//Comparison dictionary
		private Dictionary<string,EChReportedPerson> DicFamilyA;
		private Dictionary<string,EChReportedPerson> DicFamilyB;
		private Dictionary<string,EChPerson> DicPersonA;
		private Dictionary<string,EChPerson> DicPersonB;

	}
}
