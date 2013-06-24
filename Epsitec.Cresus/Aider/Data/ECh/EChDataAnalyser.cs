using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Aider.Data.ECh
{
    class EChDataAnalyser : EChDataComparer
    {
        public EChDataAnalyser(string OldEchFile, string NewEchFile)
            : base(OldEchFile, NewEchFile) 
        {
            this.FamilyToAdd = new Dictionary<string, EChReportedPerson>();
            this.FamilyToRemove = new Dictionary<string, EChReportedPerson>();
            this.PersonToAdd = new Dictionary<string, EChPerson>();
            this.PersonToRemove = new Dictionary<string, EChPerson>();

            this.NewFamiliesDetected = new Dictionary<string, EChReportedPerson>();


            this.NewUnion = new List<EChReportedPerson>();
            this.NewFamily = new List<EChReportedPerson>();
            this.NewFamilyWithChildren = new List<EChReportedPerson>();
            this.NewFamilyMono = new List<EChReportedPerson>();
            this.NewFamilyMonoWithChildren = new List<EChReportedPerson>();
            this.FamilyWithNewChilds = new List<EChReportedPerson>();
            this.FamilyWithChildMissing = new List<EChReportedPerson>();
            this.NewChilds = new List<Tuple<EChReportedPerson, EChPerson>>();
            this.ChildMissing = new List<Tuple<EChReportedPerson, EChPerson>>();
            this.ChildMove = new List<EChReportedPerson>();
            this.GainMajority = new List<EChReportedPerson>();
            this.MissingUnion = new List<EChReportedPerson>();
            this.WidowFamily = new List<EChReportedPerson>();
            this.MissingFamily = new List<EChReportedPerson>();
            this.MissingFamilyWithChildren = new List<EChReportedPerson>();
            this.MissingFamilyMono = new List<EChReportedPerson>();
            this.MissingFamilyMonoWithChildren = new List<EChReportedPerson>();
            this.AddCaseToResolve = new List<EChReportedPerson>();

            foreach (var person in this.GetPersonToAdd())
            {
                PersonToAdd.Add(person.Id, person);
            }

            foreach (var person in this.GetPersonToRemove())
            {
                PersonToRemove.Add(person.Id, person);
            }

            foreach (var family in this.GetFamilyToAdd())
            {
                FamilyToAdd.Add(family.Adult1.Id, family);
            }

            foreach (var family in this.GetFamilyToRemove())
            {
                FamilyToRemove.Add(family.Adult1.Id, family);
            }
        }


        public IList<EChReportedPerson> GetNewFamilies()
        {
            return this.NewFamiliesDetected.Select(f => f.Value).ToList();
        }

        public void AnalyseAllChanges()
        {

            //We check all the family to add in the register,
            //this addition can result from an replacement of an old record
            //or be totaly new
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


                if (family.Adult2 == null)
                {
                    monoParental = true;
                }

                if (family.Children.Count > 0)
                {
                    hasChildren = true;
                }

                
                //Union and Separation Detector
                if (monoParental)
                {
                    if (this.FamilyToRemove.ContainsKey(family.Adult1.Id))//if we found the opposed record to remove
                    {
                        coreStructuralChange = true;
                        removedFamily = this.FamilyToRemove[family.Adult1.Id];

                        if (removedFamily.Adult2 != null)//if the old record has an second adult
                        {
                            isSeparation = true;
                        }
                    }
                    else
                    {
                        if (this.PersonToAdd.ContainsKey(family.Adult1.Id))//if the adult is in the new person list
                        {
                            isNew = true;
                        }
                        else //in this case an child or an adult2 is now as first adult
                        {
                            if (DateTime.Now.Year - family.Adult1.DateOfBirth.Year == 18)//if majority condition match
                            {
                                isMajority = true;
                            }
                            else// in this case, this adult1 was an adult2
                            {
                                isWidow = true;
                            }
                        }
                    }
                }
                else //in case of two adult household
                {
                    if (this.FamilyToRemove.ContainsKey(family.Adult1.Id))// if the first adult is found in opposed record to remove
                    {
                        coreStructuralChange = true;
                        removedFamily = this.FamilyToRemove[family.Adult1.Id];

                        if (removedFamily.Adult2 != null)//if the old record has an second adult
                        {
                            if (!removedFamily.Adult2.Id.Equals(family.Adult2.Id))//if this second adult is different to the actual second adult
                            {
                                isUnion = true;
                            }
                        }
                        else //we don't have a second adult in the old record
                        {
                            isUnion = true;
                        }
                    }
                    else if (this.FamilyToRemove.ContainsKey(family.Adult2.Id)) // if the second adult is found in opposed record
                    {
                        coreStructuralChange = true;
                        removedFamily = this.FamilyToRemove[family.Adult2.Id];
                        if (removedFamily.Adult2 != null) // if the old record has a second adult
                        {
                            if (!removedFamily.Adult2.Id.Equals(family.Adult1.Id)) //if this second adult is different to the actual first adult
                            {
                                isUnion = true;
                            }
                        }
                        else //if we don't have a second adult in the old record
                        {
                            isUnion = true;
                        }
                    }
                    else //if we don't found any opposed record
                    {
                        if (this.PersonToAdd.ContainsKey(family.Adult1.Id) && PersonToAdd.ContainsKey(family.Adult2.Id))//if two adults are new
                        {
                            isNew = true;
                        }
                        else // in this case it's unclassified
                        {
                            unclassified = true;
                        }
                    }

                }


                //Check Childrens Changes on changed core structure
                if (hasChildren&&coreStructuralChange)
                {
                    if (family.Children.Count > removedFamily.Children.Count)
                    {
                        foreach (var child in family.Children)
                        {
                            if (this.PersonToAdd.ContainsKey(child.Id))
                            {
                                isNewBorn = true;
                                this.NewChilds.Add(Tuple.Create(family, child));
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
                            if (this.PersonToRemove.ContainsKey(child.Id))
                            {
                                isChildMissing = true;
                            }
                            else
                            {
                                if (this.FamilyToAdd.ContainsKey(child.Id))
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
                else if (!hasChildren && coreStructuralChange)
                {
                    if (family.Children.Count < removedFamily.Children.Count)
                    {
                        foreach (var child in family.Children)
                        {
                            if (this.PersonToRemove.ContainsKey(child.Id))
                            {
                                isChildMissing = true;
                            }
                            else
                            {
                                if (this.FamilyToAdd.ContainsKey(child.Id))
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

                //Classification for reporting purpose
                if (!unclassified)
                {
                    if (isNew)
                    {


                        //Result 
                        this.NewFamiliesDetected.Add(family.FamilyKey,family);

                        if (monoParental)
                        {
                            if (hasChildren)
                            {
                                this.NewFamilyMonoWithChildren.Add(family);
                            }
                            else
                            {
                                this.NewFamilyMono.Add(family);
                            }

                        }
                        else
                        {
                            if (hasChildren)
                            {
                                this.NewFamilyWithChildren.Add(family);
                            }
                            else
                            {
                                this.NewFamily.Add(family);
                            }
                        }
                    }

                    if (isSeparation)
                    {
                        this.MissingUnion.Add(family);
                    }

                    if (isWidow)
                    {
                        this.WidowFamily.Add(family);
                    }

                    if (isUnion)
                    {
                        this.NewUnion.Add(family);
                    }

                    if (isMajority)
                    {
                        this.GainMajority.Add(family);
                    }

                    if (isNewBorn)
                    {
                        this.FamilyWithNewChilds.Add(family);
                    }

                    if (isChildMissing)
                    {
                        this.FamilyWithChildMissing.Add(family);
                    }

                    if (isChildMove)
                    {
                        this.ChildMove.Add(family);
                    }
                }
                else
                {
                    if (!isMajoritySideEffect && !isChildMove && !isWidow)
                    {
                        this.AddCaseToResolve.Add(family);
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
                    if (this.FamilyToAdd.ContainsKey(family.Adult1.Id))
                    {
                        hasChange = true;
                    }
                }
                else
                {
                    if (this.FamilyToAdd.ContainsKey(family.Adult1.Id))
                    {
                        hasChange = true;


                    }
                    else if (this.FamilyToAdd.ContainsKey(family.Adult2.Id))
                    {
                        hasChange = true;
                    }

                }

                if (!hasChange)//if nothing found in family to add
                {
                    if (monoParental)
                    {
                        if (this.PersonToRemove.ContainsKey(family.Adult1.Id))//if we found the person to remove
                        {
                            if (hasChildren)//and have children
                            {
                                this.MissingFamilyMonoWithChildren.Add(family);
                            }
                            else
                            {
                                this.MissingFamilyMono.Add(family);
                            }

                        }
                    }
                    else
                    {
                        if (this.PersonToRemove.ContainsKey(family.Adult1.Id) && this.PersonToRemove.ContainsKey(family.Adult2.Id))
                        {
                            if (hasChildren)
                            {
                                this.MissingFamilyWithChildren.Add(family);
                            }
                            else
                            {
                                this.MissingFamily.Add(family);
                            }

                        }
                    }

                }
            }
        }


        public void CreateReport(string reportFile)
        {
            //REPORT IN MARKDOWN
            System.IO.TextWriter tw = new System.IO.StreamWriter(reportFile);

            tw.WriteLine("# Rapport Analyse ECH du " + DateTime.Now);

            tw.WriteLine("## Résumé des départs et arrivées");
            tw.WriteLine(this.NewFamily.Count + " familles sans enfants sont arrivées dans le registre");
            tw.WriteLine("");
            tw.WriteLine(this.MissingFamily.Count + " familles sans enfants sont sorties du registre");
            tw.WriteLine("");
            tw.WriteLine(this.NewFamilyWithChildren.Count + " familles avec enfants sont arrivées dans le registre");
            tw.WriteLine("");
            tw.WriteLine(this.MissingFamilyWithChildren.Count + " familles avec enfants sont sorties du registre");
            tw.WriteLine("");
            tw.WriteLine(this.NewFamilyMonoWithChildren.Count + " familles monoparentales avec enfants sont arrivées dans le registre");
            tw.WriteLine("");
            tw.WriteLine(this.MissingFamilyMonoWithChildren.Count + " familles monoparentales avec enfants sont sorties du registre");
            tw.WriteLine("");
            tw.WriteLine(this.NewFamilyMono.Count + " personnes seules sont arrivées dans le registre");
            tw.WriteLine("");
            tw.WriteLine(this.MissingFamilyMono.Count + " personnes seules sont sorties du registre");
            tw.WriteLine("");
            tw.WriteLine("## Résumé des cas impactants les familles");
            tw.WriteLine(this.NewUnion.Count + " unions");
            tw.WriteLine("");
            tw.WriteLine(this.MissingUnion.Count + " séparatations");
            tw.WriteLine("");
            tw.WriteLine(this.WidowFamily.Count + " famille avec perte du conjoint");
            tw.WriteLine("");
            tw.WriteLine(this.ChildMove.Count + " familles dont le ou les enfants a/ont changé de foyer");
            tw.WriteLine("");
            tw.WriteLine(this.GainMajority.Count + " cas de majorité");
            tw.WriteLine("");
            tw.WriteLine(this.NewChilds.Count + " enfants ajoutés au registre");
            tw.WriteLine("");
            tw.WriteLine(this.ChildMissing.Count + " enfants sortis du registre");
            tw.WriteLine("");


            tw.WriteLine("## Résumé des cas non-résolus");
            tw.WriteLine(this.AddCaseToResolve.Count + " ajouts non-résolus");
            tw.WriteLine("");

            tw.WriteLine("## Détail des arrivées");


            tw.WriteLine("### Nouvelles familles sans enfants dans le registre (" + this.NewFamily.Count + ")");
            foreach (var family in this.NewFamily)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
                tw.WriteLine("");
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
                tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
                tw.WriteLine("");
            }

            tw.WriteLine("### Nouvelles familles avec enfants dans le registre (" + this.NewFamilyWithChildren.Count + ")");
            foreach (var family in this.NewFamilyWithChildren)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
                tw.WriteLine("");
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
                tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
                tw.WriteLine("");
                foreach (var child in family.Children)
                {
                    tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
                }
            }

            tw.WriteLine("### Nouvelles familles monoparentales dans le registre (" + this.NewFamilyMonoWithChildren.Count + ")");
            foreach (var family in this.NewFamilyMonoWithChildren)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
                tw.WriteLine("");
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
                if (family.Adult2 != null)
                {
                    tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
                }
                tw.WriteLine("");
                foreach (var child in family.Children)
                {
                    tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
                }
            }

            tw.WriteLine("### Nouvelles personnes seules dans le registre (" + this.NewFamilyMono.Count + ")");
            foreach (var family in this.NewFamilyMono)
            {
                tw.WriteLine("* (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
                if (family.Adult2 != null)
                {
                    tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
                }
            }

            tw.WriteLine("### Nouveaux enfants (" + this.NewChilds.Count + ")");
            foreach (var child in this.NewChilds)
            {
                tw.WriteLine("#### Famille " + child.Item1.Adult1.OfficialName);
                tw.WriteLine(" * (E) " + child.Item2.FirstNames + " " + child.Item2.OfficialName + " né le " + child.Item2.DateOfBirth);
            }


            tw.WriteLine("### Majorités (" + this.GainMajority.Count + ")");
            foreach (var family in this.GainMajority)
            {
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " né le " + family.Adult1.DateOfBirth);
            }

            tw.WriteLine("## Détail des départs");

            tw.WriteLine("### Départ de familles sans enfants dans le registre (" + this.MissingFamily.Count + ")");
            foreach (var family in this.MissingFamily)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
                tw.WriteLine("");
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
                tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
                tw.WriteLine("");
                foreach (var child in family.Children)
                {
                    tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
                }
            }

            tw.WriteLine("### Départ de familles avec enfants dans le registre (" + this.MissingFamilyWithChildren.Count + ")");
            foreach (var family in this.MissingFamilyWithChildren)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
                tw.WriteLine("");
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
                tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName);
                tw.WriteLine("");
                foreach (var child in family.Children)
                {
                    tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
                }
            }

            tw.WriteLine("### Départ de familles monoparentales dans le registre (" + this.MissingFamilyMonoWithChildren.Count + ")");
            foreach (var family in this.MissingFamilyMonoWithChildren)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
                tw.WriteLine("");
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
                if (family.Adult2 != null)
                {
                    tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
                }
                tw.WriteLine("");
                foreach (var child in family.Children)
                {
                    tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
                }
            }

            tw.WriteLine("### Personnes seules sortantes du registre (" + this.MissingFamilyMono.Count + ")");
            foreach (var family in this.MissingFamilyMono)
            {
                tw.WriteLine("* (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName);
                if (family.Adult2 != null)
                {
                    tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
                }
            }

            tw.WriteLine("### Enfants sortis du registre (" + this.ChildMissing.Count + ")");
            foreach (var child in this.ChildMissing)
            {
                tw.WriteLine("#### Famille " + child.Item1.Adult1.OfficialName);
                tw.WriteLine(" * (E) " + child.Item2.FirstNames + " " + child.Item2.OfficialName + " né le " + child.Item2.DateOfBirth);
            }


            tw.WriteLine("## Détail des cas impactants les familles");

            tw.WriteLine("### Nouvelles union (" + this.NewUnion.Count + ")");
            foreach (var family in this.NewUnion)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
                tw.WriteLine("");
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
                tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
                foreach (var child in family.Children)
                {
                    tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
                }
            }

            tw.WriteLine("### Séparations / perte d'un conjoint (" + this.MissingUnion.Count + ")");
            foreach (var family in this.MissingUnion)
            {
                tw.WriteLine("#### Famille " + family.Adult1.OfficialName);
                tw.WriteLine(family.Address.SwissZipCode + " " + family.Address.Town + " : " + family.Address.Street);
                tw.WriteLine("");
                tw.WriteLine(" * (A) " + family.Adult1.FirstNames + " " + family.Adult1.OfficialName + " ( " + family.Adult1.MaritalStatus + " ) ");
                if (family.Adult2 != null)
                {
                    tw.WriteLine(" * (A) " + family.Adult2.FirstNames + " " + family.Adult2.OfficialName + " ( " + family.Adult2.MaritalStatus + " ) ");
                }
                foreach (var child in family.Children)
                {
                    tw.WriteLine(" * (E) " + child.FirstNames + " " + child.OfficialName);
                }
            }
            tw.Close();
        }





        private Dictionary<string, EChReportedPerson> FamilyToAdd;
        private Dictionary<string, EChReportedPerson> FamilyToRemove;
        private Dictionary<string, EChPerson> PersonToAdd;
        private Dictionary<string, EChPerson> PersonToRemove;

        private List<EChReportedPerson> NewUnion;
        private List<EChReportedPerson> NewFamily;
        private List<EChReportedPerson> NewFamilyWithChildren;
        private List<EChReportedPerson> NewFamilyMono;
        private List<EChReportedPerson> NewFamilyMonoWithChildren;
        private List<EChReportedPerson> FamilyWithNewChilds;
        private List<EChReportedPerson> FamilyWithChildMissing;
        private List<EChReportedPerson> ChildMove;
        private List<EChReportedPerson> GainMajority;
        private List<EChReportedPerson> MissingUnion;
        private List<EChReportedPerson> WidowFamily;
        private List<EChReportedPerson> MissingFamily;
        private List<EChReportedPerson> MissingFamilyWithChildren;
        private List<EChReportedPerson> MissingFamilyMono;
        private List<EChReportedPerson> MissingFamilyMonoWithChildren;
        private List<EChReportedPerson> AddCaseToResolve;

        private List<Tuple<EChReportedPerson, EChPerson>> NewChilds;
        private List<Tuple<EChReportedPerson, EChPerson>> ChildMissing;


        //RESULT DICTIONARY
        private Dictionary<string, EChReportedPerson> NewFamiliesDetected;
    }
}
