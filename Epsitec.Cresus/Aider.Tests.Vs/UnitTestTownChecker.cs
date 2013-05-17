using Epsitec.Aider.Data.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Aider.Tests.Vs
{


	[TestClass]
	public class UnitTestTownChecker
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			UnitTestTownChecker.townChecker = new TownChecker ();
		}


		[TestMethod]
		public void Test()
		{
			this.Check ("1095", "LUTRY", "1095", "Lutry");
			this.Check ("1093", "LA CONVERSION", "1093", "La Conversion");
			this.Check ("1092", "BELMONT", "1092", "Belmont-sur-Lausanne");
			this.Check ("1090", "LA CROIX", "1090", "La Croix (Lutry)");
			this.Check ("1613", "MARACON", "1613", "Maracon");
			this.Check ("1041", "BOTTENS", "1041", "Bottens");
			this.Check ("1009", "PULLY", "1009", "Pully");
			this.Check ("1083", "MEZIERES", "1083", "Mézières VD");
			this.Check ("1112", "ECHICHENS", "1112", "Echichens");
			this.Check ("1052", "LE MONT/LAUSANNE", "1052", "Le Mont-sur-Lausanne");
			this.Check ("1096", "CULLY", "1096", "Cully");
			this.Check ("1400", "YVERDON", "1400", "Yverdon-les-Bains");
			this.Check ("1094", "PAUDEX", "1094", "Paudex");
			this.Check ("1073", "SAVIGNY", "1073", "Savigny");
			this.Check ("1012", "LAUSANNE", "1012", "Lausanne");
			this.Check ("1052", "LE MONT", "1052", "Le Mont-sur-Lausanne");
			this.Check ("1006", "LAUSANNE", "1006", "Lausanne");
			this.Check ("1091", "ARAN", "1091", "Aran");
			this.Check ("1073", "MOLLIE-MARGOT", "1073", "Mollie-Margot");
			this.Check ("1814", "LA TOUR-DE-PEILZ", "1814", "La Tour-de-Peilz");
			this.Check ("1003", "LAUSANNE", "1003", "Lausanne");
			this.Check ("1803", "CHARDONNE", "1803", "Chardonne");
			this.Check ("1800", "VEVEY", "1800", "Vevey");
			this.Check ("1110", "MORGES", "1110", "Morges");
			this.Check ("1004", "LAUSANNE", "1004", "Lausanne");
			this.Check ("1091", "GRANDVAUX", "1091", "Grandvaux");
			this.Check ("1028", "PREVERENGES", "1028", "Préverenges");
			this.Check ("1127", "CLARMONT", "1127", "Clarmont");
			this.Check ("1005", "LAUSANNE", "1005", "Lausanne");
			this.Check ("1042", "ASSENS", "1042", "Assens");
			this.Check ("1072", "FOREL", "1072", "Forel (Lavaux)");
			this.Check ("1096", "VILLETTE", "1096", "Villette (Lavaux)");
			this.Check ("1070", "PUIDOUX", "1070", "Puidoux");
			this.Check ("1008", "PRILLY", "1008", "Prilly");
			this.Check ("1027", "LONAY", "1027", "Lonay");
			this.Check ("1603", "GRANDVAUX", "1091", "Grandvaux");
			this.Check ("1030", "BUSSIGNY", "1030", "Bussigny-près-Lausanne");
			this.Check ("1000", "LAUSANNE 26", "1000", "Lausanne 26");
			this.Check ("1071", "CHEXBRES", "1071", "Chexbres");
			this.Check ("1018", "LAUSANNE", "1018", "Lausanne");
			this.Check ("1820", "MONTREUX", "1820", "Montreux");
			this.Check ("1066", "EPALINGES", "1066", "Epalinges");
			this.Check ("1097", "RIEX", "1097", "Riex");
			this.Check ("1071", "SAINT-SAPHORIN", "1071", "St-Saphorin (Lavaux)");
			this.Check ("1801", "LE MONT-PELERIN", "1801", "Le Mont-Pèlerin");
			this.Check ("1000", "LAUSANNE 2", "1002", "Lausanne");
			this.Check ("1010", "LAUSANNE", "1010", "Lausanne");
			this.Check ("1002", "LAUSANNE", "1002", "Lausanne");
			this.Check ("1009", "PULLY", "1009", "Pully");
			this.Check ("1020", "Renens 1", "1020", "Renens VD 1");
			this.Check ("1092", "Belmont", "1092", "Belmont-sur-Lausanne");
			this.Check ("1071", "St-Saphorin", "1071", "St-Saphorin (Lavaux)");
			this.Check ("1814", "La Tour de Peilz", "1814", "La Tour-de-Peilz");
			this.Check ("1071", "CHEXBRES", "1071", "Chexbres");
			this.Check ("1070", "PUIDOUX", "1070", "Puidoux");
			this.Check ("1071", "SAINT-SAPHORIN", "1071", "St-Saphorin (Lavaux)");
			this.Check ("1607", "LES THIOLEYRES", "1607", "Les Thioleyres");
			this.Check ("1071", "RIVAZ", "1071", "Rivaz");
			this.Check ("1096", "CULLY", "1096", "Cully");
			this.Check ("1607", "PALEZIEUX", "1607", "Palézieux");
			this.Check ("1070", "PUIDOUX-GARE", "1070", "Puidoux");
			this.Check ("1098", "EPESSES", "1098", "Epesses");
			this.Check ("1607", "PALEZIEUX-GARE", "1607", "Palézieux");
			this.Check ("1009", "PULLY", "1009", "Pully");
			this.Check ("1822", "Chernex-sur-Montreux", "1822", "Chernex");
			this.Check ("1096", "CULLY-Le Treytorrens", "1096", "Cully");
			this.Check ("1803", "CHARDONNE", "1803", "Chardonne");
			this.Check ("1020", "Renens", "1020", "Renens VD");
			this.Check ("1096", "Villette", "1096", "Villette (Lavaux)");
			this.Check ("1814", "LaTour-de-Peilz", "1814", "La Tour-de-Peilz");
			this.Check ("1801", "Le Mont-Pélerin", "1801", "Le Mont-Pèlerin");
			this.Check ("1443", "Essert-s-Champvent", "1443", "Essert-sous-Champvent");
			this.Check ("1526", "Forel", "1526", "Forel-sur-Lucens");
			this.Check ("1116", "Cottens", "1116", "Cottens VD");
			this.Check ("1114", "Colombier", "1114", "Colombier VD");
			this.Check ("1515", "Neyruz", "1515", "Neyruz-sur-Moudon");
			this.Check ("1588", "Champmartin", "1588", "Cudrefin");
			this.Check ("1867", "Ollon", "1867", "Ollon VD");
			this.Check ("1417", "Essertines", "1417", "Essertines-sur-Yverdon");
			this.Check ("1512", "Chavannes-s-Moudon", "1512", "Chavannes-sur-Moudon");
			this.Check ("1882", "Les Posses-sur-Bex", "1880", "Les Posses-sur-Bex");
			this.Check ("1063", "Chapelle-s-Moudon", "1063", "Chapelle-sur-Moudon");
			this.Check ("1186", "Essertines-Rolle", "1186", "Essertines-sur-Rolle");
			this.Check ("1412", "Valeyres-Ursins", "1412", "Valeyres-sous-Ursins");
			this.Check ("1261", "Burtigny", "1268", "Burtigny");
			this.Check ("1682", "Prevonloup", "1682", "Prévonloup");
			this.Check ("1377", "Oulens", "1377", "Oulens-sous-Echallens");
			this.Check ("1535", "Combremont-Grand", "1535", "Combremont-le-Grand");
			this.Check ("1866", "La Forclaz", "1866", "La Forclaz VD");
			this.Check ("1410", "Saint-Cierges", "1410", "St-Cierges");
			this.Check ("1683", "Chesalles-Moudon", "1683", "Chesalles-sur-Moudon");
			this.Check ("1536", "Combremont-Petit", "1536", "Combremont-le-Petit");
			this.Check ("1523", "Granges-Marnand", "1523", "Granges-près-Marnand");
			this.Check ("1442", "Montagny-Yverdon", "1442", "Montagny-près-Yverdon");
			this.Check ("1867", "Verschiez", "1867", "Ollon VD");
			this.Check ("1116", "Bussy-Chardonney", "1136", "Bussy-Chardonney");
			this.Check ("1022", "Chavannes-Renens", "1022", "Chavannes-près-Renens");
			this.Check ("1358", "Valeyres-Rances", "1358", "Valeyres-sous-Rances");
			this.Check ("1492", "Giez", "1429", "Giez");
			this.Check ("1610", "Oron", "1610", "Oron-la-Ville");
			this.Check ("1029", "Villars-Sainte-Croix", "1029", "Villars-Ste-Croix");
			this.Check ("1032", "Romanel-s-Lausanne", "1032", "Romanel-sur-Lausanne");
			this.Check ("1890", "Saint-Maurice", "1890", "St-Maurice");
			this.Check ("1022", "Chavannes", "1022", "Chavannes-près-Renens");
			this.Check ("1014", "Lausanne", "1014", "Lausanne Adm cant VD");
			this.Check ("1020", "Renens", "1020", "Renens VD");
			this.Check ("1450", "Sainte-Croix", "1450", "Ste-Croix");
			this.Check ("1817", "Clarens", "1815", "Clarens");
			this.Check ("1025", "Saint-Sulpice", "1025", "St-Sulpice VD");
			this.Check ("1024", "Ecublens", "1024", "Ecublens VD");
			this.Check ("1804", "Corsier", "1804", "Corsier-sur-Vevey");
			this.Check ("1844", "Villeneuve", "1844", "Villeneuve VD");
			this.Check ("1040", "Saint-Barthélémy", "1040", "St-Barthélemy VD");
			this.Check ("1052", "Mont-sur-Lausanne", "1052", "Le Mont-sur-Lausanne");
			this.Check ("1852", "Roche", "1852", "Roche VD");
			this.Check ("1264", "Saint-Cergue", "1264", "St-Cergue");
			this.Check ("3012", "Berne", "3012", "Bern");
			this.Check ("1132", "Lully", "1132", "Lully VD");
			this.Check ("1033", "Cheseaux-Lausanne", "1033", "Cheseaux-sur-Lausanne");
			this.Check ("1867", "Les Combes", "1867", "Ollon VD");
			this.Check ("1740", "Neyruz", "1740", "Neyruz FR");
			this.Check ("1033", "Cheseaux", "1033", "Cheseaux-sur-Lausanne");
			this.Check ("1086", "Vucherens", "1509", "Vucherens");
			this.Check ("1422", "Les Tuileries", "1422", "Grandson");
			this.Check ("1059", "Vulliens", "1085", "Vulliens");
			this.Check ("1113", "St-Saphorin-Morges", "1113", "St-Saphorin-sur-Morges");
			this.Check ("1222", "Vesenaz", "1222", "Vésenaz");
			this.Check ("1806", "St-Légier", "1806", "St-Légier-La Chiésaz");
			this.Check ("1806", "St-Légier-Chiésaz", "1806", "St-Légier-La Chiésaz");
			this.Check ("1008", "Lausanne", "1000", "Lausanne");
			this.Check ("1588", "Montet", "1588", "Cudrefin");
			this.Check ("1030", "Bussigny", "1030", "Bussigny-près-Lausanne");
			this.Check ("1162", "Saint-Prex", "1162", "St-Prex");
			this.Check ("1024", "Eculbens", "1024", "Ecublens VD");
			this.Check ("1148", "Moiry", "1148", "Moiry VD");
			this.Check ("1122", "Romanel", "1122", "Romanel-sur-Morges");
			this.Check ("1030", "Bussigny-Lausanne", "1030", "Bussigny-près-Lausanne");
			this.Check ("1053", "Cugy", "1053", "Cugy VD");
			this.Check ("1008", "Jouxtens", "1008", "Jouxtens-Mézery");
			this.Check ("1607", "Palézieux-Gare", "1607", "Palézieux");
			this.Check ("1304", "Cossonay", "1304", "Cossonay-Ville");
			this.Check ("2610", "Saint-Imier", "2610", "St-Imier");
			this.Check ("1092", "Belmont", "1092", "Belmont-sur-Lausanne");
			this.Check ("1092", "Belmont-Lausanne", "1092", "Belmont-sur-Lausanne");
			this.Check ("1041", "Montaubion-Chardonnay", "1041", "Montaubion-Chardonney");
			this.Check ("1884", "Villars", "1884", "Villars-sur-Ollon");
			this.Check ("1084", "Carrouge", "1084", "Carrouge VD");
			this.Check ("1134", "Vufflens-Château", "1134", "Vufflens-le-Château");
			this.Check ("1801", "Le Mont Pélerin", "1801", "Le Mont-Pèlerin");
			this.Check ("1054", "Morrens", "1054", "Morrens VD");
			this.Check ("1072", "Forel", "1072", "Forel (Lavaux)");
			this.Check ("1892", "Lavey", "1892", "Lavey-Village");
			this.Check ("1188", "Saint-George", "1188", "St-George");
			this.Check ("1070", "Puidoux-Gare", "1070", "Puidoux");
			this.Check ("1562", "Corcelles-Payerne", "1562", "Corcelles-près-Payerne");
			this.Check ("1882", "Gron", "1882", "Gryon");
			this.Check ("3960", "Muraz", "3960", "Muraz (Sierre)");
			this.Check ("1414", "Rueyres", "1046", "Rueyres");
			this.Check ("1025", "St-Sulpice", "1025", "St-Sulpice VD");
			this.Check ("2035", "Corcelles", "2035", "Corcelles NE");
			this.Check ("1040", "Poliez-le-Grand", "1041", "Poliez-le-Grand");
			this.Check ("1261", "Le-Vaud", "1261", "Le Vaud");
			this.Check ("1020", "Renens 2", "1020", "Renens VD 2");
			this.Check ("1027", "Préverenges", "1028", "Préverenges");
			this.Check ("1441", "Valeyres-Montagny", "1441", "Valeyres-sous-Montagny");
			this.Check ("1032", "Romanel", "1032", "Romanel-sur-Lausanne");
			this.Check ("1071", "Saint-Saphorin", "1071", "St-Saphorin (Lavaux)");
			this.Check ("1417", "Essertines-Yverdon", "1417", "Essertines-sur-Yverdon");
			this.Check ("1188", "Saint-Georges", "1188", "St-George");
			this.Check ("1083", "Mézières", "1083", "Mézières VD");
			this.Check ("1880", "Aigle", "1860", "Aigle");
			this.Check ("1258", "Certoux", "1258", "Perly");
			this.Check ("1041", "Montaubion-Chardon", "1041", "Montaubion-Chardonney");
			this.Check ("1031", "Mex", "1031", "Mex VD");
			this.Check ("1053", "Bretigny", "1053", "Bretigny-sur-Morrens");
			this.Check ("1808", "Monts-de-Corsier", "1808", "Les Monts-de-Corsier");
			this.Check ("1413", "Oppens", "1047", "Oppens");
			this.Check ("1218", "Grand-Saconnex", "1218", "Le Grand-Saconnex");
			this.Check ("1200", "Genève 3", "1211", "Genève 3");
			this.Check ("3003", "Berne", "3003", "Bern");
			this.Check ("5405", "Baden-Dättwil", "5405", "Baden");
			this.Check ("1261", "Saint-George", "1188", "St-George");
			this.Check ("1682", "Dompierre", "1682", "Dompierre VD");
			this.Check ("3280", "Morat", "3280", "Murten");
			this.Check ("2000", "Neuchatel", "2000", "Neuchâtel");
			this.Check ("1219", "Aïre-Genève", "1219", "Aïre");
			this.Check ("1146", "Mollens", "1146", "Mollens VD");
			this.Check ("2603", "Pery", "2603", "Péry");
			this.Check ("3011", "Berne", "3011", "Bern");
			this.Check ("3036", "Detlingen", "3036", "Detligen");
			this.Check ("3007", "Berne", "3007", "Bern");
			this.Check ("1585", "Bellerive", "1585", "Bellerive VD");
			this.Check ("1091", "Aran-Villette", "1091", "Aran");
			this.Check ("1867", "Saint-Triphon", "1867", "St-Triphon");
			this.Check ("1535", "Combremont-Le-Grand", "1535", "Combremont-le-Grand");
			this.Check ("1867", "Saint-Triphon-Gare", "1867", "St-Triphon");
			this.Check ("1683", "Brenles-sur-Moudon", "1683", "Brenles");
			this.Check ("1569", "Atavaux", "1475", "Autavaux");
			this.Check ("1373", "Bavois", "1372", "Bavois");
			this.Check ("1187", "Saint-Oyens", "1187", "St-Oyens");
			this.Check ("1690", "Lussy", "1690", "Lussy FR");
			this.Check ("1308", "La Chaux-Cossonay", "1308", "La Chaux (Cossonay)");
			this.Check ("1145", "Vuiteboeuf", "1445", "Vuiteboeuf");
			this.Check ("1126", "Vaux-Sur-Morges", "1126", "Vaux-sur-Morges");
			this.Check ("1014", "Lausanne Adm cant", "1014", "Lausanne Adm cant VD");
			this.Check ("2503", "Bienne", "2503", "Biel/Bienne");
			this.Check ("1800", "Vevey 2", "1800", "Vevey");
			this.Check ("1470", "Estavayer", "1470", "Estavayer-le-Lac");
			this.Check ("1435", "Ependes", "1434", "Ependes VD");
			this.Check ("1374", "Corcelles-Chavornay", "1374", "Corcelles-sur-Chavornay");
			this.Check ("1143", "Ballens", "1144", "Ballens");
			this.Check ("1787", "Mur (Vully)", "1787", "Mur (Vully) FR");
			this.Check ("1053", "Montheron", "1053", "Cugy VD");
			this.Check ("1890", "Epinassey", "1890", "St-Maurice");
			this.Check ("1442", "Montagny", "1442", "Montagny-près-Yverdon");
			this.Check ("1787", "Mur", "1787", "Mur (Vully) FR");
			this.Check ("1096", "Villette", "1096", "Villette (Lavaux)");
			this.Check ("1582", "Donatyre", "1580", "Donatyre");
			this.Check ("1806", "Saint-Légier", "1806", "St-Légier-La Chiésaz");
			this.Check ("1377", "Oulens-Echallens", "1377", "Oulens-sous-Echallens");
			this.Check ("1040", "Saint-Barthélemy", "1040", "St-Barthélemy VD");
			this.Check ("1787", "Motier", "1787", "Môtier (Vully)");
			this.Check ("1148", "Chavannes-Veyron", "1148", "Chavannes-le-Veyron");
			this.Check ("1132", "Vufflens-le-Château", "1134", "Vufflens-le-Château");
			this.Check ("1965", "Mayens-De-La-Tour", "1965", "Mayens-de-la-Zour (Savièse)");
			this.Check ("1603", "Grandvaux", "1091", "Grandvaux");
			this.Check ("1855", "Saint-Triphon", "1867", "St-Triphon");
			this.Check ("1227", "Carouge", "1227", "Carouge GE");
			this.Check ("1400", "Yverdon", "1400", "Yverdon-les-Bains");
			this.Check ("2072", "Saint-Blaise", "2072", "St-Blaise");
			this.Check ("1432", "Belmont-Yverdon", "1432", "Belmont-sur-Yverdon");
			this.Check ("1680", "Romont", "1680", "Romont FR");
			this.Check ("1867", "Villy / Ollon VD", "1867", "Ollon VD");
			this.Check ("1176", "Saint-Livres", "1176", "St-Livres");
			this.Check ("1271", "Genolier", "1272", "Genolier");
			this.Check ("3000", "Berne 23", "3000", "Bern 23");
			this.Check ("1426", "Corcelles-Concise", "1426", "Corcelles-près-Concise");
			this.Check ("1359", "Rances", "1439", "Rances");
			this.Check ("1812", "Rivaz", "1071", "Rivaz");
			this.Check ("1832", "Villard /Chamby", "1832", "Villard-sur-Chamby");
			this.Check ("1228", "Genève", "1228", "Plan-les-Ouates");
			this.Check ("8021", "Zürich 1 Sihlpost", "8021", "Zürich 1");
			this.Check ("1180", "Gland", "1196", "Gland");
			this.Check ("1440", "Montagny", "1440", "Montagny-Chamard");
			this.Check ("1053", "Bretigny-Morrens", "1053", "Bretigny-sur-Morrens");
			this.Check ("1071", "St-Saphorin-Lavaux", "1071", "St-Saphorin (Lavaux)");
			this.Check ("3050", "Bern", "3050", "Bern Swisscom");
			this.Check ("1001", "Lausanne 22", "1000", "Lausanne 22");
			this.Check ("1224", "Genève", "1202", "Genève");
			this.Check ("1010", "Lausanne 10", "1000", "Lausanne 10");
			this.Check ("3001", "Berne", "3001", "Bern");
			this.Check ("2500", "Bienne 7", "2500", "Biel/Bienne 7");
			this.Check ("2504", "Bienne", "2504", "Biel/Bienne");
			this.Check ("2001", "Neuchatel", "2001", "Neuchâtel 1");
			this.Check ("2002", "Neuchâtel", "2002", "Neuchâtel 2");
			this.Check ("5001", "Aarau", "5001", "Aarau 1 Fächer");
			this.Check ("2206", "Geneveys-Coffrane", "2206", "Les Geneveys-sur-Coffrane");
			this.Check ("3000", "Berne 7", "3000", "Bern 7 Bärenplatz");
			this.Check ("8201", "Schaffhouse", "8201", "Schaffhausen");
			this.Check ("1000", "Lausanne 17", "1017", "Lausanne Charles Veillon SA");
			this.Check ("3000", "Berne 13", "3000", "Bern 13");
			this.Check ("8035", "Zürich", "8053", "Zürich");
			this.Check ("1200", "Genève 7", "1211", "Genève 7");
			this.Check ("1672", "Oron-la-Ville", "1610", "Oron-la-Ville");
			this.Check ("1000", "Lausanne 9", "1000", "Lausanne");
			this.Check ("1104", "Lausanne", "1004", "Lausanne");
			this.Check ("2501", "Bienne", "2501", "Biel/Bienne");
			this.Check ("1661", "Pâquier", "1661", "Le Pâquier-Montbarry");
			this.Check ("2003", "Neuchâtel", "2003", "Neuchâtel 3");
			this.Check ("3005", "Berne", "3005", "Bern");
			this.Check ("1000", "Lausanne 4", "1000", "Lausanne");
			this.Check ("1002", "Lausanne 2", "1002", "Lausanne");
			this.Check ("4089", "Basel", "4089", "Basel SPILOG");
			this.Check ("1133", "Lussy", "1167", "Lussy-sur-Morges");
			this.Check ("1132", "Lully", "1132", "Lully VD");
			this.Check ("1137", "Yens", "1169", "Yens");
			this.Check ("1092", "Belmont sur Lausanne", "1092", "Belmont-sur-Lausanne");
			this.Check ("1114", "Colombier", "1114", "Colombier VD");
			this.Check ("1987", "Mâche / VS", "1987", "Hérémence");
			this.Check ("1328", "Mont-la-Ville", "1148", "Mont-la-Ville");
			this.Check ("1024", "Ecublens", "1024", "Ecublens VD");
			this.Check ("1020", "Renens", "1020", "Renens VD");
			this.Check ("1804", "Corsier", "1804", "Corsier-sur-Vevey");
			this.Check ("", "Lausanne", "1000", "Lausanne");
			this.Check ("1176", "Saint-Livres", "1176", "St-Livres");
			this.Check ("1000", "Lausanne 9", "1000", "Lausanne");
			this.Check ("1227", "Carouge", "1227", "Carouge GE");
			this.Check ("1072", "Forel", "1072", "Forel (Lavaux)");
			this.Check ("1138", "Villars-sous-Yens", "1168", "Villars-sous-Yens");
			this.Check ("1417", "Essertines s/Yverdon", "1417", "Essertines-sur-Yverdon");
			this.Check ("1148", "L’Isle", "1148", "L'Isle");
			this.Check ("1030", "Bussigny", "1030", "Bussigny-près-Lausanne");
			this.Check ("3415", "Hasle-Ruegsau", "3415", "Hasle-Rüegsau");
			this.Check ("1113", "St-Saphorin-sur -Morges", "1113", "St-Saphorin-sur-Morges");
			this.Check ("1250", "Nyon", "1260", "Nyon");
			this.Check ("2532", "Macolin", "2532", "Magglingen/Macolin");
			this.Check ("1837", "Château d’Oex", "1660", "Château-d'Oex");
			this.Check ("1837", "Chateau-d'Oex", "1660", "Château-d'Oex");
			this.Check ("1080", "Cullayes", "1080", "Les Cullayes");
		}


		private void Check(string zip1, string name1, string zip2, string name2)
		{
			var result = UnitTestTownChecker.townChecker.Validate (zip1, name1);

			Assert.AreEqual (result.Item1, zip2);
			Assert.AreEqual (result.Item2, name2);
		}


		private static TownChecker townChecker;


	}


}
