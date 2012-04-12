//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	public static class Currencies
	{
		public static IEnumerable<FormattedText> CurrenciesForAutoCompleteMenu
		{
			get
			{
				yield return "AED;dirham (Émirats arabes unis)";
				yield return "AFN;afghani (Afghanistan)";
				yield return "ALL;lek (Albanie)";
				yield return "AMD;dram (Arménie)";
				yield return "ANG;florin (Antilles néerlandaises)";
				yield return "AOA;Kwanza (Angola)";
				yield return "ARS;peso (Argentine)";
				yield return "AUD;dollar (Australie)";
				yield return "AWG;florin (Aruba)";
				yield return "AZN;manat (Azerbaïdjan)";
				yield return "BAM;mark convertible (Bosnie-Herzégovine)";
				yield return "BBD;dollar (Barbade)";
				yield return "BDT;taka (Bangladesh)";
				yield return "BGN;lev (Bulgarie)";
				yield return "BHD;dinar (Bahreïn)";
				yield return "BIF;franc (Burundi)";
				yield return "BMD;dollar (Bermudes)";
				yield return "BND;dollar (Brunei)";
				yield return "BOB;boliviano (Bolivie)";
				yield return "BOV;mvdol (Bolivie)";
				yield return "BRL;réal (Brésil)";
				yield return "BSD;dollar (Bahamas)";
				yield return "BTN;ngultrum (Bhoutan)";
				yield return "BWP;pula (Botswana)";
				yield return "BYB;rouble (Biélorussie)";
				yield return "BZD;dollar (Belize)";
				yield return "CAD;dollar (Canada)";
				yield return "CDF;franc (République Démocratique du Congo)";
				yield return "CHF;franc (Suisse)";
				yield return "CLP;peso (Chili)";
				yield return "CNY;yuan renminbi (Chine, Macao)";
				yield return "COP;peso (Colombie)";
				yield return "CRC;colon (Costa Rica)";
				yield return "CUC;peso convertible (Cuba)";
				yield return "CUP;peso (Cuba)";
				yield return "CVE;escudo (Cap-Vert)";
				yield return "CYP;livre (Chypre)";
				yield return "CZK;couronne (République tchèque)";
				yield return "DJF;franc (Djibouti)";
				yield return "DKK;couronne (Danemark)";
				yield return "DOP;peso (République dominicaine)";
				yield return "DZD;dinar (Algérie)";
				yield return "EEK;couronne (Estonie)";
				yield return "EGP;livre (Égypte)";
				yield return "ERN;nafka (Érythrée)";
				yield return "ETB;birr (Éthiopie)";
				yield return "FJD;dollar (Fidji)";
				yield return "FKP;livre (Falkland)";
				yield return "GBP;livre sterling (Royaume-Uni)";
				yield return "GEL;lari (Géorgie)";
				yield return "GHC;cedi (Ghana)";
				yield return "GIP;livre (Gibraltar)";
				yield return "GMD;dalasi (Gambie)";
				yield return "GNF;franc (Guinée)";
				yield return "GTQ;quetzal (Guatemala)";
				yield return "GYD;dollar (Guyana)";
				yield return "HKD;dollar (Hong Kong)";
				yield return "HNL;lempira (Honduras)";
				yield return "HRK;kuna (Croatie)";
				yield return "HTG;gourde (Haïti)";
				yield return "HUF;forint (Hongrie)";
				yield return "IDR;roupie (Indonésie)";
				yield return "ILS;shekel (Israël)";
				yield return "INR;roupie (Inde)";
				yield return "IQD;dinar (Irak)";
				yield return "IRR;rial (Iran)";
				yield return "ISK;couronne (Islande)";
				yield return "JMD;dollar (Jamaïque)";
				yield return "JOD;dinar (Jordanie)";
				yield return "JPY;yen (Japon)";
				yield return "KES;schilling (Kenya)";
				yield return "KGS;som (Kirghizistan)";
				yield return "KHR;riel (Cambodge)";
				yield return "KMF;franc (Comores)";
				yield return "KPW;won (Corée du Nord)";
				yield return "KRW;won (Corée du Sud)";
				yield return "KWD;dinar (Koweït)";
				yield return "KYD;dollar (Îles Caïmanes)";
				yield return "KZT;tenge (Kazakhstan)";
				yield return "LAK;kip (Laos)";
				yield return "LBP;livre (Liban)";
				yield return "LKR;roupie (Sri Lanka)";
				yield return "LRD;dollar (Libéria)";
				yield return "LSL;loti (Lesotho)";
				yield return "LTL;litas (Lituanie)";
				yield return "LVL;Lats letton (Lettonie)";
				yield return "LYD;dinar (Libye)";
				yield return "MAD;dirham (Maroc)";
				yield return "MDL;leu (Moldavie)";
				yield return "MGA;ariary (Madagascar)";
				yield return "MKD;denar (Macédoine)";
				yield return "MMK;kyat (Birmanie)";
				yield return "MNT;tugrik (Mongolie)";
				yield return "MRO;ouguiya (Mauritanie)";
				yield return "MOP;Patacas (Macao)";
				yield return "MTL;lire (Malte)";
				yield return "MUR;roupie (Maurice)";
				yield return "MVR;rufiyaa (Maldives)";
				yield return "MWK;kwacha (Malawi)";
				yield return "MXN;peso (Mexique)";
				yield return "MYR;ringgit (Malaisie)";
				yield return "MZN;metical (Mozambique)";
				yield return "NAD;dollar (Namibie)";
				yield return "NGN;naira (Nigeria)";
				yield return "NIO;cordoba d’or (Nicaragua)";
				yield return "NOK;couronne (Norvège)";
				yield return "NPR;roupie (Népal)";
				yield return "NZD;dollar (Nouvelle-Zélande)";
				yield return "OMR;rial (Oman)";
				yield return "PAB;balboa (Panamá)";
				yield return "PEN;sol (Pérou)";
				yield return "PGK;kina (Papouasie-Nouvelle-Guinée)";
				yield return "PHP;peso (Philippines)";
				yield return "PKR;roupie (Pakistan)";
				yield return "PLN;zloty (Pologne)";
				yield return "PYG;guarani (Paraguay)";
				yield return "QAR;rial (Qatar)";
				yield return "RON;leu (Roumanie)";
				yield return "RSD;dinar (Serbie)";
				yield return "RUB;Rouble russe (Russie)";
				yield return "RWF;franc (Rwanda)";
				yield return "SAR;ryal (Arabie saoudite)";
				yield return "SBD;dollar (Îles Salomon)";
				yield return "SCR;roupie (Seychelles)";
				yield return "SDG;livre (Soudan)";
				yield return "SEK;couronne (Suède)";
				yield return "SGD;dollar (Singapour)";
				yield return "SHP;livre (Sainte-Hélène)";
				yield return "SKK;couronne (Slovaquie)";
				yield return "SLL;leone (Sierra Leone)";
				yield return "SOS;schilling (Somalie)";
				yield return "SRD;dollar (Suriname)";
				yield return "STD;dobra (São Tomé-et-Principe)";
				yield return "SVC;colon (Salvador)";
				yield return "SYP;livre (Syrie)";
				yield return "SZL;lilangeni (Swaziland)";
				yield return "THB;baht (Thaïlande)";
				yield return "TJS;somoni (Tadjikistan)";
				yield return "TMM;manat (Turkménistan)";
				yield return "TND;dinar (Tunisie)";
				yield return "TOP;pa’anga (Tonga)";
				yield return "TRY;livre (Turquie)";
				yield return "TTD;dollar (Trinité-et-Tobago)";
				yield return "TWD;dollar (Taïwan)";
				yield return "TZS;schilling (Tanzanie)";
				yield return "UAH;hryvnia (Ukraine)";
				yield return "UGX;schilling (Ouganda)";
				yield return "USD;dollar (États-Unis)";
				yield return "UYU;peso (Uruguay)";
				yield return "UZS;sum (Ouzbékistan)";
				yield return "VEB;bolivar (Venezuela)";
				yield return "VND;dong (Viêt Nam)";
				yield return "VUV;vatu (Vanuatu)";
				yield return "WST;tala (Samoa)";
				yield return "XAF;franc CFA (BEAC)";  // Cameroun, Centrafrique, Congo-Brazzaville, Gabon, Guinée-Equatoriale, Tchad
				yield return "XCD;dollar (Anguilla)";
				yield return "XOF;franc CFA (BCEAO)";  // Bénin, Burkina Faso, Côte d'Ivoire, Guinée-Bissau, Mali, Niger, Sénégal, Togo
				yield return "XPF;franc CFP (IEOM)";  // Nouvelle-Calédonie, Polynésie Française, Wallis-et-Futuna
				yield return "YER;riyal (Yémen)";
				yield return "ZAR;Rand (Afrique du Sud, Namibie)";
				yield return "ZMK;kwacha (Zambie)";
				yield return "ZWD;dollar (Zimbabwe)";

				yield return "XAG;Argent";
				yield return "XAU;Or";
				yield return "XPD;Palladium";
				yield return "XPT;Platine";
			}
		}
	}
}
