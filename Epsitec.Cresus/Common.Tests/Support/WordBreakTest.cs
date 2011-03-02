using NUnit.Framework;
using System.Collections.Generic;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture]
	public class WordBreakTest
	{
		[Test]
		public void CheckWordBreak()
		{
			this.errorCounter = 0;

			foreach (string word in WordBreakTest.list)
			{
				this.CheckWord(word);
			}

			if (this.errorCounter != 0)
			{
				Assert.Fail(string.Format("{0} errors listed in Console.Error !", this.errorCounter));
			}
			else
			{
				System.Console.Error.WriteLine("All word breaks are OK.");
			}
		}

		protected void CheckWord(string word)
		{
			//	Teste un mot. En cas d'erreur, écrit un texte explicite dans Console.Error.
			string brut;
			List<int> hope;
			WordBreakTest.RemoveSeparators(word, out brut, out hope);

			List<int> result = new List<int>(Common.Text.BreakEngines.FrenchWordBreakEngine.Break(brut));

			if (!WordBreakTest.AreEqual(hope, result))  // résultat incorrect ?
			{
				string wrong = WordBreakTest.AddSeparators(brut, result);
				System.Console.Error.WriteLine(string.Format("Correct: {0}   Result: {1}", word, wrong));

				this.errorCounter++;
			}
		}

		static protected bool AreEqual(List<int> list1, List<int> list2)
		{
			//	Vérifie si deux listes sont identiques.
			if (list1.Count != list2.Count)
			{
				return false;
			}

			for (int i=0; i<list1.Count; i++)
			{
				if (list1[i] != list2[i])
				{
					return false;
				}
			}

			return true;
		}

		static protected void RemoveSeparators(string wordWithSep, out string wordWithout, out List<int> list)
		{
			//	Enlève les séparateurs "/" dans un mot et retourne la liste des positions.
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			list = new List<int>();

			foreach (char c in wordWithSep)
			{
				if (c == '/')  // séparateur souhaité ?
				{
					list.Add(buffer.Length);
				}
				else
				{
					buffer.Append(c);
				}
			}

			wordWithout = buffer.ToString();  // mot brut, sans séparateurs
		}

		static protected string AddSeparators(string wordWithout, List<int> list)
		{
			//	Remet les séparateurs "/" dans un mot.
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();

			for (int i=0; i<wordWithout.Length; i++)
			{
				if (list.Contains(i))
				{
					buffer.Append("/");
				}

				buffer.Append(wordWithout[i]);
			}

			return buffer.ToString();
		}


		//	Liste des mots à tester. Les endroits où WordBreak doit trouver
		//	une césure possible sont marqués par un slash "/".
		static string[] list =
		{
			"aéro/spa/tial",
			"an/ti/spas/mo/di/que",
			"an/ti/al/co/oli/que",
			"an/ti/thè/se",
			"an/ti/cham/bre",
			"ab/sent",
			"abs/trait",
			"abra/ca/da/brant",
			"al/chi/mis/te",
			"al/pin",
			"amour",
			"ana/ly/se",
			"ano/ny/me",
			"ap/pros/ta/te",
			"apos/tro/phe",
			"asy/me/trie",
			"ai/san/ce",
			"aphteu/se",
			"aphro/di/sia/que",
			"ap/pré/hen/der",
			"ap/par/te/ment",
			"ap/pi/toyer",
			"ap/pi/toye/ment",
			"as/siet/te",
			"an/nuel/le",
			"au/jour/d'hui",
			"ap/proxi/ma/tif",
			"asy/mé/trie",
			"ava/re",
			"at/mo/sphè/re",
			"ayions",

			"ba/yer",
			"blas/phè/me",
			"blond",
			"brouil/lard",
			"bon/hom/me",
			"bon/heur",
			"blas/phè/me",
			"bio/lo/gi/que",
			"bil/bo/quet",
			"brah/ma/ni/que",
			"brouet/tier",
			"bu/ty/li/que",
			"bus",

			"ca/rac/té/ris/ti/que",
			"chou/chou",
			"com/pen/sa/tion",
			"cons/puer",
			"cons/truit",
			"cons/truc/tion",
			"con/scien/ce",
			"con/cier/ge",
			"ci/toyens",
			"ca/tas/tro/phe",
			"cir/cons/cri/re",
			"cir/cons/pec/tion",
			"cons/crip/tion",
			"cons/tan/ce",
			"cons/ti/tu/tion",
			"caou/tchouc",
			"cir/que",
			"co/opé/ra/tion",
			"cen/tra/li/sa/teur",
			"che/vreuil",
			"clouer",
			"cloa/que",
			"con/vain/cre",
			"cos/mo/go/nie",
			"cré/er",
			"créa/tion",
			"croyan/ce",
			"cul/tu/re",

			"dac/ty/lo/gra/phie",
			"déjà",
			"de",
			"des",
			"dé/doua/ner",
			"dis/cus/sion",
			"dro/gis/te",
			"du/pli/ca/ta",
			"dé/mons/tra/tion",
			"dia/go/na/le",
			"diag/nos/tic",
			"dia/mant",
			"dia/pa/son",
			"dio/ny/sia/que",
			"dex/te/ri/te",
			"deuxiè/me",
			"des/ha/bil/ler",
			"des/hé/ri/té",
			"des/hon/neur",
			"des/obli/ger",
			"des/union",
			"des/ac/cor/der",
			"de/ser/ti/ques",
			"dé/si/gner",
			"dé/si/rer",
			"dé/sis/ter",
			"dé/so/la/tion",
			"dé/sta/bi/li/ser",

			"es/prit",
			"éthnie",
			"exem/plai/re",
			"ex/ha/ler",
			"ex/tra/or/di/nai/re",
			"ex/tra/va/gent",
			"ex/trac/tion",
			"ex/trai/re",
			"ex/cel/lent",
			"épis/to/lai/re",
			"epreu/ve",
			"Egip/tien",
			"Égyp/tien",
			"ébran/la/ble",
			"ébrui/ter",
			"éco/le",
			"égra/ti/gnu/re",
			"élon/ga/tion",
			"el/les",
			"émeu/te",
			"en/ter/re/ment",
			"éruc/ta/tion",
			"eu/cli/de",
			"ex/ploi/ta/tion",

			"fisc",
			"flam/boyer",
			"fos/set/te",
			"fu/sion",
			"fais/ceau",
			"fiè/vre",
			"fan/geux",
			"fil/tra/tion",
			"fluet",
			"fos/si/li/sa/tion",
			"fuis/seur",
			"fu/tur",

			"gla/ciai/re",
			"gym/nas/ti/que",
			"gueu/se",
			"gui/don",
			"guer/re",
			"gal/va/ni/sa/tion",
			"gar/ga/ris/me",
			"gar/gouil/la/de",
			"gau/che",
			"ga/zel/le",
			"glou/ton/ne/rie",
			"glan/di/for/me",
			"gout/ter",
			"grand",
			"guin/da/ge",

			"har/mo/nies",
			"hé/mo/phi/lie",
			"hen/de/ca/syl/la/de",
			"her/bier",
			"ho/mo/chro/mie",
			"hymne",
			"hy/da/tis/me",
			"hy/drau/li/ques",
			"hé/mi/sphè/re",
			"hé/mi/èdre",
			"hy/per/ten/sion",

			"im/pres/crip/ti/ble",
			"il/lo/gi/que",
			"idiot",
			"idio/me",
			"iden/ti/fier",
			"io/ni/ser",
			"iro/nie",
			"iso/mor/phes",
			"ité/ra/tion",
			"in/com/pre/hen/si/ble",
			"ins/tal/la/tion",
			"ins/tau/ra/tion",
			"ins/truc/tion",
			"in/au/di/ble",
			"in/ex/pe/rien/ce",
			"in/ex/tin/gui/ble",
			"in/fluen/ce",
			"in/cli/nai/sons",
			"in/si/nua/tions",
			"in/ac/tif",
			"in/égal",
			"in/évi/ta/ble",
			"in/exis/tant",
			"in/ima/gi/na/ble",
			"in/in/té/res/sant",
			"in/ocu/ler",
			"in/of/fen/cif",
			"in/on/da/tions",
			"in/ou/blia/ble",
			"in/oxy/da/ble",
			"in/sta/ble",
			"in/uti/li/sa/ble",
			"ini/que",
			"ini/tial",
			"ini/tia/ti/ve",
			"ins/ta/ment",
			"ins/tan/ta/nes",

			"ja/bot",
			"jap/per",
			"jan/vier",
			"jar/di/na/ges",
			"jas/min",
			"jean/ne",
			"jon/gleur",
			"jon/quil/les",
			"jo/vial",
			"jour/nel/le/ment",
			"joux/te",
			"jus/te/ment",

			"ka/lei/dos/co/pe",
			"kle/phte",
			"ki/lo/oc/tets",

			"laid",
			"lam/pions",
			"liè/vre",
			"lon/gueur",
			"long/temps",
			"lors/que",
			"loua/ble",
			"luxu/rieux",

			"ma/chia/ve/lis/me",
			"ma/gis/tral",
			"ma/ni/ché/en",
			"ma/noeu/vre",
			"ma/nuel",
			"ma/nus/crit",
			"mar/biè/re",
			"mar/tyr",
			"mas/cu/lin",
			"mes/da/me",
			"mes/de/moi/sel/les",
			"mé/téo/ro/lo/gie",
			"mé/tho/de",
			"mé/ta/mor/pho/se",
			"miel/leux",
			"mi/lieu",
			"mné/mo/ni/que",
			"moi/gnon",
			"moel/lon",
			"mo/sai/que",
			"moyen/ne",
			"mi/cro/sco/pe",
			"mi/nis/tè/re",
			"mix/tu/re",
			"moyen/nant",
			"mal/adres/se",
			"mal/adroit",
			"mal/ap/pris",
			"mal/en/ten/du",
			"mal/in/ten/sion/ne",
			"mal/odo/rant",
			"ma/li/gin/te",
			"ma/la/die",
			"ma/lai/se",
			"ma/laxer",
			"mes/al/lian/ce",
			"mes/aven/tu/re",
			"mes/es/ti/mer",
			"me/su/res",
			"mi/cro/or/di/na/teur",
			"moyen/nant",
			"mais",
			"maïs",

			"naif",
			"nean",
			"néo/lo/gis/me",
			"né/ces/sai/re",
			"néan/moins",
			"né/toyu/re",
			"nim/be",
			"nos/tal/gi/que",
			"nuan/ces",
			"non/obs/tant",
			"no/nan/te",

			"obus",
			"ob/jec/tif",
			"ob/ses/sions",
			"obs/truc/tion/nis/tes",
			"oc/ca/sion",
			"of/fran/de",
			"oli/gar/chie",
			"om/ni/po/ten/ce",
			"or/tho/gra/phe",
			"orien/ter",
			"obs/cur/sis/ment",
			"obs/ta/cle",
			"obs/ti/né",
			"ou/vrir",
			"oto/rhi/no/la/ryn/go/lo/gis/tes",

			"par/ti/cu/liai/re/ment",
			"pas/til/le",
			"pa/tho/lo/gi/que",
			"pein/tu/re",
			"pen/ta/thlon",
			"phy/sio/lo/gie",
			"pis/ci/ne",
			"pyg/mee",
			"py/thon",
			"py/ra/mi/de",
			"pi/lu/le",
			"pha/rynx",
			"pers/pi/ca/ce",
			"pré/sco/lai/re",
			"pros/ter/ner",
			"pros/ta/te",
			"pro/émi/nen/ce",
			"po/êle",
			"po/ète",
			"po/èti/que/ment",
			"po/ly/va/lent",
			"pro/phy/laxie",
			"pliu/re",
			"phos/pho/re",
			"proxi/mi/te",
			"pré/his/toi/re",
			"pay/san",
			"pré/oc/cu/per",
			"pres/by/te",
			"pres/crip/tion",
			"pres/sion",
			"pres/tan/ce",
			"pres/que",
			"pré/scien/ce",

			"quel/con/que",
			"quin/quen/nal",
			"qui/con/que",
			"ques/tion/nai/res",
			"quo/tient",

			"re/cons/ti/tu/tion",
			"ré/tro/spec/tif",
			"res/ti/tua/ble",
			"res/pec/tueux",
			"re/struc/tu/rer",
			"re/pro/duc/tion",
			"ré/flexion",
			"re/dis/tri/buer",
			"ré/abon/ner",
			"ré/ac/tion/ner",
			"ré/af/fiv/mer",
			"ré/agir",
			"ré/ar/mer",
			"ré/as/si/gner",
			"ré/at/te/ler",
			"ré/élec/tion",
			"ré/in/te/grer",
			"ré/or/ga/ni/ser",
			"rei/nes",
			"ro/gnu/re",
			"rus/ti/que",
			"ry/thme",
			"ra/chat",
			"rua/de",
			"ruel/le",
			"rou/geo/le",
			"ro/man/tis/me",
			"rhu/mes",
			"rha/pso/die",

			"sans/crit",
			"san/glot",
			"san/gui/nel/le",
			"sanc/tions",
			"sain/doux",
			"sab/ba/tin",
			"site",
			"scru/pu/leu/se/ments",
			"soixan/te",
			"so/phis/ti/quer",
			"sor/cier",
			"spon/ta/ne/ment",
			"sus/pen/dre",
			"scra/be",
			"sys/tè/me",
			"syl/la/bes",
			"stric/te/ment",
			"sculp/tu/re",
			"sous/cri/re",
			"sous/trai/re",
			"sy/no/vie",
			"symp/to/mes",
			"si/tua/tion",
			"sour/cil/leux",
			"ser/vo/me/ca/nis/mes",
			"ser/vi/ce",
			"ser/pen/tai/re",
			"se/quoia",
			"scri/bouil/lard",
			"scin/til/ler",
			"scle/ro/se",
			"scar/la/ti/ne",
			"scè/ne",
			"stag/nant",
			"sta/bi/li/ser",
			"struc/tu/rer",
			"sub/or/don/ner",
			"sub/or/ner",
			"sub/ur/bain",
			"sub/mer/ger",
			"su/bé/reux",
			"su/bi/te/ment",
			"su/bli/me",
			"sub/lu/nai/re",
			"sub/ro/ga/teur",
			"subs/tan/ce",
			"subs/ti/tu/tion",
			"sur/éle/ver",
			"sur/ali/men/ter",
			"sur/ac/ti/vi/té",
			"sur/pro/duc/tion",
			"su/ran/ne",
			"su/reau",
			"su/re/ment",
			"su/re/te",
			"su/ri/ca/te",
			"su/ri/re",
			"su/roit",
			"su/ros",
			"su/pé/rio/ri/te",
			"su/pé/rieur",
			"su/per/struc/tu/res",
			"su/per/po/ser",

			"temps",
			"tem/po/rel",
			"tha/la/mus",
			"triom/pher",
			"tem/po/rai/re",
			"trip/ty/que",
			"théa/tre",
			"trans/ac/tion",
			"trans/at/lan/ti/ques",
			"tran/sis/tor",
			"tran/si/toi/re",
			"trans/mu/ta/tion",
			"ta/chy/mè/tre",
			"tex/ti/les",
			"tex/tuel/le",
			"tech/ni/que",
			"te/le/sco/pe",
			"te/le/vi/sion",
			"te/le/ma/ti/que",

			"usi/ne",
			"ul/cè/re",
			"uni/ta/ris/me",
			"uni/te",

			"vas/cu/lai/re",
			"vi/cis/si/tu/des",
			"vieil/lard",
			"vain/cre",
			"vac/cin",
			"ver/mi/ne",
			"vier/ges",
			"vir/gu/le",
			"voir",
			"vol/ti/ge",
			"voyant",

			"wa/gons",

			"xé/no/pho/bie",
			"xy/lo/pho/nes",

			"yuan",

			"zi/za/nie",
			"zo/dia/que",

			"d'abord",
			"l'étoi/le",
			"l'ap/par/te/ment",
			"qu'il",
			"qu'el/les",
			"qu'avec",
			"qu'ain/si",
			"qu'alors",
			"jus/qu'ici",
			"s'ap/pi/toyer",
			"l'at/mo/sphè/re",
			"l'éthnie",
			"d'ex/tra/or/di/nai/re",
			"(d'ex/tra/or/di/nai/re)",
			"d'au/jour/d'hui",
			"(sub/ur/bains)",
			"(pré/oc/cu/per)",
			"ma/chi/ne...",
			"ma/chi/nes...",
			"d'au/cuns",
			"com/men/cée",
			"com/men/cées",
			"es/pion",
			"oa/sis",
			"théâ/tre",
			"taxer",
			"tuyau",
			"payer",
			"tex/tuel",
			"pay/san",
			"in/sta/ble",
			"(com/men/cée)",
			"(com/men/cées)",
			"(con/cier/ge)",
		};


		protected int errorCounter;
	}
}
