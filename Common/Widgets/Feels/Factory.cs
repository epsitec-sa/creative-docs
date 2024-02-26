//	Copyright © 2003-2013, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

namespace Epsitec.Common.Widgets.Feel
{
    /// <summary>
    /// La classe Feel.Factory donne accès à l'interface IFeel actuellement active.
    /// De plus, elle liste et crée automatiquement des instances de chaque classe
    /// implémentant IFeel dans l'assembly actuelle...
    /// </summary>
    public static class Factory
    {
        static Factory()
        {
            Factory.feelTable = new System.Collections.Hashtable();
            Factory.feelList = new System.Collections.ArrayList();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(
                typeof(Factory)
            );

            Factory.AnalyzeAssembly(assembly);

            Factory.SetActive("Default");

            System.Diagnostics.Debug.Assert(Factory.feelTable.ContainsKey("Default"));
            System.Diagnostics.Debug.Assert(Factory.activeFeel != null);
        }

        internal static int AnalyzeAssembly(System.Reflection.Assembly assembly)
        {
            int n = 0;

            var allTypesInAssembly = assembly.GetTypes();

            //	Cherche dans tous les types connus les classes qui implémentent l'interface
            //	IFeel, et crée une instance unique de chacune de ces classes.

            foreach (System.Type type in allTypesInAssembly)
            {
                if (type.IsClass && type.IsPublic)
                {
                    if (type.ContainsInterface<IFeel>())
                    {
                        if (!Factory.feelList.Contains(type.Name))
                        {
                            Factory.feelList.Add(type.Name);
                            Factory.feelTable[type.Name] = System.Activator.CreateInstance(type);
                            n++;
                        }
                    }
                }
            }

            return n;
        }

        public static IFeel Active
        {
            get { return Factory.activeFeel; }
        }

        public static string ActiveName
        {
            get { return Factory.Active.GetType().Name; }
        }

        public static string[] FeelNames
        {
            get
            {
                string[] names = new string[Factory.feelList.Count];
                Factory.feelList.CopyTo(names);
                return names;
            }
        }

        public static bool SetActive(string name)
        {
            IFeel feel = Factory.feelTable[name] as IFeel;

            if (feel == null)
            {
                return false;
            }

            if (Factory.activeFeel != feel)
            {
                Factory.activeFeel = feel;
            }

            return true;
        }

        private static IFeel activeFeel;
        private static System.Collections.Hashtable feelTable;
        private static System.Collections.ArrayList feelList;
    }
}
