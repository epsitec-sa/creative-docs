//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				yield return "AED;dirham (�mirats arabes unis)";
				yield return "AFN;afghani (Afghanistan)";
				yield return "ALL;lek (Albanie)";
				yield return "AMD;dram (Arm�nie)";
				yield return "ANG;florin (Antilles n�erlandaises)";
				yield return "AOA;Kwanza (Angola)";
				yield return "ARS;peso (Argentine)";
				yield return "AUD;dollar (Australie)";
				yield return "AWG;florin (Aruba)";
				yield return "AZN;manat (Azerba�djan)";
				yield return "BAM;mark convertible (Bosnie-Herz�govine)";
				yield return "BBD;dollar (Barbade)";
				yield return "BDT;taka (Bangladesh)";
				yield return "BGN;lev (Bulgarie)";
				yield return "BHD;dinar (Bahre�n)";
				yield return "BIF;franc (Burundi)";
				yield return "BMD;dollar (Bermudes)";
				yield return "BND;dollar (Brunei)";
				yield return "BOB;boliviano (Bolivie)";
				yield return "BOV;mvdol (Bolivie)";
				yield return "BRL;r�al (Br�sil)";
				yield return "BSD;dollar (Bahamas)";
				yield return "BTN;ngultrum (Bhoutan)";
				yield return "BWP;pula (Botswana)";
				yield return "BYB;rouble (Bi�lorussie)";
				yield return "BZD;dollar (Belize)";
				yield return "CAD;dollar (Canada)";
				yield return "CDF;franc (R�publique D�mocratique du Congo)";
				yield return "CHF;franc (Suisse)";
				yield return "CLP;peso (Chili)";
				yield return "CNY;yuan renminbi (Chine, Macao)";
				yield return "COP;peso (Colombie)";
				yield return "CRC;colon (Costa Rica)";
				yield return "CUC;peso convertible (Cuba)";
				yield return "CUP;peso (Cuba)";
				yield return "CVE;escudo (Cap-Vert)";
				yield return "CYP;livre (Chypre)";
				yield return "CZK;couronne (R�publique tch�que)";
				yield return "DJF;franc (Djibouti)";
				yield return "DKK;couronne (Danemark)";
				yield return "DOP;peso (R�publique dominicaine)";
				yield return "DZD;dinar (Alg�rie)";
				yield return "EEK;couronne (Estonie)";
				yield return "EGP;livre (�gypte)";
				yield return "ERN;nafka (�rythr�e)";
				yield return "ETB;birr (�thiopie)";
				yield return "FJD;dollar (Fidji)";
				yield return "FKP;livre (Falkland)";
				yield return "GBP;livre sterling (Royaume-Uni)";
				yield return "GEL;lari (G�orgie)";
				yield return "GHC;cedi (Ghana)";
				yield return "GIP;livre (Gibraltar)";
				yield return "GMD;dalasi (Gambie)";
				yield return "GNF;franc (Guin�e)";
				yield return "GTQ;quetzal (Guatemala)";
				yield return "GYD;dollar (Guyana)";
				yield return "HKD;dollar (Hong Kong)";
				yield return "HNL;lempira (Honduras)";
				yield return "HRK;kuna (Croatie)";
				yield return "HTG;gourde (Ha�ti)";
				yield return "HUF;forint (Hongrie)";
				yield return "IDR;roupie (Indon�sie)";
				yield return "ILS;shekel (Isra�l)";
				yield return "INR;roupie (Inde)";
				yield return "IQD;dinar (Irak)";
				yield return "IRR;rial (Iran)";
				yield return "ISK;couronne (Islande)";
				yield return "JMD;dollar (Jama�que)";
				yield return "JOD;dinar (Jordanie)";
				yield return "JPY;yen (Japon)";
				yield return "KES;schilling (Kenya)";
				yield return "KGS;som (Kirghizistan)";
				yield return "KHR;riel (Cambodge)";
				yield return "KMF;franc (Comores)";
				yield return "KPW;won (Cor�e du Nord)";
				yield return "KRW;won (Cor�e du Sud)";
				yield return "KWD;dinar (Kowe�t)";
				yield return "KYD;dollar (�les Ca�manes)";
				yield return "KZT;tenge (Kazakhstan)";
				yield return "LAK;kip (Laos)";
				yield return "LBP;livre (Liban)";
				yield return "LKR;roupie (Sri Lanka)";
				yield return "LRD;dollar (Lib�ria)";
				yield return "LSL;loti (Lesotho)";
				yield return "LTL;litas (Lituanie)";
				yield return "LVL;Lats letton (Lettonie)";
				yield return "LYD;dinar (Libye)";
				yield return "MAD;dirham (Maroc)";
				yield return "MDL;leu (Moldavie)";
				yield return "MGA;ariary (Madagascar)";
				yield return "MKD;denar (Mac�doine)";
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
				yield return "NIO;cordoba d�or (Nicaragua)";
				yield return "NOK;couronne (Norv�ge)";
				yield return "NPR;roupie (N�pal)";
				yield return "NZD;dollar (Nouvelle-Z�lande)";
				yield return "OMR;rial (Oman)";
				yield return "PAB;balboa (Panam�)";
				yield return "PEN;sol (P�rou)";
				yield return "PGK;kina (Papouasie-Nouvelle-Guin�e)";
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
				yield return "SBD;dollar (�les Salomon)";
				yield return "SCR;roupie (Seychelles)";
				yield return "SDG;livre (Soudan)";
				yield return "SEK;couronne (Su�de)";
				yield return "SGD;dollar (Singapour)";
				yield return "SHP;livre (Sainte-H�l�ne)";
				yield return "SKK;couronne (Slovaquie)";
				yield return "SLL;leone (Sierra Leone)";
				yield return "SOS;schilling (Somalie)";
				yield return "SRD;dollar (Suriname)";
				yield return "STD;dobra (S�o Tom�-et-Principe)";
				yield return "SVC;colon (Salvador)";
				yield return "SYP;livre (Syrie)";
				yield return "SZL;lilangeni (Swaziland)";
				yield return "THB;baht (Tha�lande)";
				yield return "TJS;somoni (Tadjikistan)";
				yield return "TMM;manat (Turkm�nistan)";
				yield return "TND;dinar (Tunisie)";
				yield return "TOP;pa�anga (Tonga)";
				yield return "TRY;livre (Turquie)";
				yield return "TTD;dollar (Trinit�-et-Tobago)";
				yield return "TWD;dollar (Ta�wan)";
				yield return "TZS;schilling (Tanzanie)";
				yield return "UAH;hryvnia (Ukraine)";
				yield return "UGX;schilling (Ouganda)";
				yield return "USD;dollar (�tats-Unis)";
				yield return "UYU;peso (Uruguay)";
				yield return "UZS;sum (Ouzb�kistan)";
				yield return "VEB;bolivar (Venezuela)";
				yield return "VND;dong (Vi�t Nam)";
				yield return "VUV;vatu (Vanuatu)";
				yield return "WST;tala (Samoa)";
				yield return "XAF;franc CFA (BEAC)";  // Cameroun, Centrafrique, Congo-Brazzaville, Gabon, Guin�e-Equatoriale, Tchad
				yield return "XCD;dollar (Anguilla)";
				yield return "XOF;franc CFA (BCEAO)";  // B�nin, Burkina Faso, C�te d'Ivoire, Guin�e-Bissau, Mali, Niger, S�n�gal, Togo
				yield return "XPF;franc CFP (IEOM)";  // Nouvelle-Cal�donie, Polyn�sie Fran�aise, Wallis-et-Futuna
				yield return "YER;riyal (Y�men)";
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
