using NUnit.Framework;
using System.Collections.Generic;
using Epsitec.Common.Support;

namespace Epsitec.Common.Support
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
			//	Teste un mot. En cas d'erreur, �crit un texte explicite dans Console.Error.
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			List<int> hope = new List<int>();

			foreach (char c in word)
			{
				if (c == '/')  // s�parateur souhait� ?
				{
					hope.Add(buffer.Length);
				}
				else
				{
					buffer.Append(c);
				}
			}

			string brut = buffer.ToString();  // mot brut, sans s�parateurs

			List<int> result = new List<int>(Common.Text.BreakEngines.FrenchWordBreakEngine.Break(brut));

			if (!WordBreakTest.AreEqual(hope, result))  // r�sultat incorrect ?
			{
				string wrong = WordBreakTest.AddSeparators(brut, result);
				System.Console.Error.WriteLine(string.Format("Correct: {0}   Result: {1}", word, wrong));

				this.errorCounter++;
			}
		}

		static protected bool AreEqual(List<int> list1, List<int> list2)
		{
			//	V�rifie si deux listes sont identiques.
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

		static protected string AddSeparators(string word, List<int> list)
		{
			//	Remet les s�parateurs "/" dans un mot.
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();

			for (int i=0; i<word.Length; i++)
			{
				if (list.Contains(i))
				{
					buffer.Append("/");
				}

				buffer.Append(word[i]);
			}

			return buffer.ToString();
		}


		//	Liste des mots � tester. Les endroits o� WordBreak doit trouver
		//	une c�sure possible sont marqu� par un slash "/".
		static string[] list =
		{
			"a�ro/spa/tial",
			"an/ti/spas/mo/di/que",
			"an/ti/al/co/oli/que",
			"an/ti/th�/se",
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
			"ap/pr�/hen/der",
			"ap/par/te/ment",
			"ap/pi/toyer",
			"ap/pi/toye/ment",
			"as/siet/te",
			"an/nuel/le",
			"au/jour/d'hui",
			"ap/proxi/ma/tif",
			"asy/m�/trie",
			"ava/re",
			"at/mo/sph�/re",
			"ayions",

			"blas/ph�/me",
			"blond",
			"brouil/lard",
			"bon/hom/me",
			"bon/heur",
			"blas/ph�/me",
			"bio/lo/gi/que",
			"bil/bo/quet",
			"brah/ma/ni/que",
			"brouet/tier",
			"bu/ty/li/que",
			"bus",

			"ca/rac/t�/ris/ti/que",
			"chou/chou",
			"cons/puer",
			"cons/truit",
			"com/pen/sa/tion",
			"cr�/er",
			"cons/truc/tion",
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
			"co/op�/ra/tion",
			"cen/tra/li/sa/teur",
			"che/vreuil",
			"clouer",
			"cloa/que",
			"convain/cre",
			"cos/mo/go/nie",
			"cr�a/tion",
			"croyan/ce",
			"cul/tu/re",					//	et la r�gle des cul* ?

			"dac/ty/lo/gra/phie",
			"d�j�",
			"de",
			"des",
			"d�/doua/ner",
			"dis/cus/sion",
			"dro/gis/te",
			"du/pli/ca/ta",
			"d�/mons/tra/tion",
			"dia/go/na/le",
			"diag/nos/tic",
			"dia/mant",
			"dia/pa/son",
			"dio/ny/sia/que",
			"dex/te/ri/te",
			"deuxi�/me",
			"des/ha/bil/ler",
			"des/h�/ri/t�",
			"des/hon/neur",
			"des/obli/ger",
			"des/union",
			"des/ac/cor/der",
			"de/ser/ti/ques",
			"d�/si/gner",
			"d�/si/rer",
			"d�/sis/ter",
			"d�/so/la/tion",

			"es/prit",
			"�thnie",
			"exem/plai/re",
			"ex/ha/ler",
			"ex/tra/or/di/nai/re",
			"ex/tra/va/gent",
			"ex/trac/tion",
			"ex/trai/re",
			"ex/cel/lent",
			"�pis/to/lai/re",
			"epreu/ve",
			"Egip/tien",
			"�gyp/tien",
			"�bran/la/ble",
			"�brui/ter",
			"�co/le",
			"�gra/ti/gnu/re",
			"�lon/ga/tion",
			"el/les",
			"�meu/te",
			"en/ter/re/ment",
			"�ruc/ta/tion",
			"eu/cli/de",
			"ex/ploi/ta/tion",

			"fisc",
			"flam/boyer",
			"fos/set/te",
			"fu/sion",
			"fais/ceau",
			"fi�/vre",
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
			"h�/mo/phi/lie",
			"hen/de/ca/syl/la/de",
			"her/bier",
			"ho/mo/chro/mie",
			"hymne",
			"hy/da/tis/me",
			"hy/drau/li/ques",
			"h�mi/sph�/re",
			"hy/per/ten/sion",

			"im/pres/crip/ti/ble",
			"il/lo/gi/que",
			"idiot",
			"idio/me",
			"iden/ti/fier",
			"io/ni/ser",
			"iro/nie",
			"iso/mor/phes",
			"it�/ra/tion",
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
			"in/�gal",
			"in/�vi/ta/ble",
			"in/exis/tant",
			"in/ima/gi/na/ble",
			"in/in/t�/res/sant",
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
			"li�/vre",
			"lon/gueur",
			"long/temps",
			"lors/que",
			"loua/ble",
			"luxu/rieux",

			"ma/chia/ve/lis/me",
			"ma/gis/tral",
			"ma/ni/ch�/en",
			"ma/noeu/vre",
			"ma/nuel",
			"ma/nus/crit",
			"mar/bi�/re",
			"mar/tyr",
			"mas/cu/lin",
			"mes/da/me",
			"mes/de/moi/sel/les",
			"m�/t�o/ro/lo/gie",
			"m�/tho/de",
			"m�/ta/mor/pho/se",
			"miel/leux",
			"mi/lieu",
			"mn�/mo/ni/que",
			"moi/gnon",
			"moel/lon",
			"mo/sai/que",
			"moyen/ne",
			"mi/cro/sco/pe",
			"mi/nis/t�/re",
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

			"naif",
			"nean",
			"n�o/lo/gis/me",
			"n�/ces/sai/re",
			"n�an/moins",
			"n�/toyu/re",
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
			"obs/ti/n�",
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
			"pr�/sco/lai/re",
			"pros/ter/ner",
			"pros/ta/te",
			"pro/�mi/nen/ce",
			"po/ly/va/lent",
			"pro/phy/laxie",
			"pliu/re",
			"phos/pho/re",
			"proxi/mi/te",
			"pr�/his/toi/re",
			"pay/san",
			"pr�/oc/cu/per",
			"pres/by/te",
			"pres/crip/tion",
			"pres/sion",
			"pres/tan/ce",
			"pres/que",

			"quel/con/que",
			"quin/quen/nal",
			"qui/con/que",
			"ques/tion/nai/res",
			"quo/tient",

			"re/cons/ti/tu/tion",
			"r�/tro/spec/tif",
			"res/ti/tua/ble",
			"res/pec/tueux",
			"re/pro/duc/tion",
			"r�/flexion",
			"re/dis/tri/buer",
			"r�/abon/ner",
			"r�/ac/tion/ner",
			"r�/af/fiv/mer",
			"r�/agir",
			"r�/ar/mer",
			"r�/as/si/gner",
			"r�/at/te/ler",
			"r�/�lec/tion",
			"r�/in/te/grer",
			"r�/or/ga/ni/ser",
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
			"sys/t�/me",
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
			"sc�/ne",
			"sub/or/don/ner",
			"sub/or/ner",
			"sub/ur/bain",
			"sub/mer/ger",
			"su/b�/reux",
			"su/bi/te/ment",
			"su/bli/me",
			"sub/lu/nai/re",
			"sub/ro/ga/teur",
			"subs/tan/ce",
			"subs/ti/tu/tion",
			"sur/�le/ver",
			"sur/ali/men/ter",
			"sur/ac/ti/vi/t�",
			"sur/pro/duc/tion",
			"su/ran/ne",
			"su/reau",
			"su/re/ment",
			"su/re/te",
			"su/ri/ca/te",
			"su/ri/re",
			"su/roit",
			"su/ros",
			"su/p�/rio/ri/te",
			"su/p�/rieur",
			"su/per/struc/tu/res",
			"su/per/po/ser",

			"temps",
			"tem/po/rel",
			"tha/la/mus",
			"triom/pher",
			"tem/po/rai/re",
			"trip/ty/que",
			"th�a/tre",
			"trans/ac/tion",
			"trans/at/lan/ti/ques",
			"tran/sis/tor",
			"tran/si/toi/re",
			"trans/mu/ta/tion",
			"ta/chy/m�/tre",
			"tex/ti/les",
			"tex/tuel/le",
			"tech/ni/que",
			"te/le/sco/pe",
			"te/le/vi/sion",
			"te/le/ma/ti/que",

			"usi/ne",
			"ul/c�/re",
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

			"x�/no/pho/bie",
			"xy/lo/pho/nes",

			"yuan",

			"zi/za/nie",
			"zo/dia/que",

			"d'abord",
			"l'�toi/le",
			"l'ap/par/te/ment",
			"qu'il",
			"qu'el/les",
			"qu'avec",
			"qu'ain/si",
			"qu'alors",
			"jus/qu'ici",
			"s'ap/pi/toyer",
			"l'at/mo/sph�/re",
			"l'�thnie",
			"d'ex/tra/or/di/nai/re",
			"(d'ex/tra/or/di/nai/re)",
			"d'au/jour/d'hui",
			"(sub/ur/bains)",
			"(pr�/oc/cu/per)",
			"ma/chi/ne...",
			"ma/chi/nes...",
			"d'au/cuns",
			"com/men/c�e",
			"com/men/c�es",
			"es/pion",
			"oa/sis",
			"th��/tre",
			"taxer",
			"tuyau",
			"payer",
			"tex/tuel",
			"pay/san",
			"in/sta/ble",
			"(com/men/c�e)",
			"(com/men/c�es)",
			"(con/cier/ge)",

			//	Faux :

			//"ma/�s",
			"po/�te",
			
			"Alpes",
			"pr�/scien/ce",		//	OK
			"con/scien/ce",		//	pas OK? (r�gle des con* je veux bien, mais pas cons-cience)
			"ba/yer",
			"h�/mi/�dre",		//	� mon avis, la c�sure �tymologique prime ici !
			"stag/nant",
			"struc/tu/rer",		//	OK
			"re/struc/tu/rer",	//	pas OK?
			"sta/bi/li/ser",	//	OK
			"d�/sta/bi/li/ser",	//	pas OK?
		};


		protected int						errorCounter;
	}
}
