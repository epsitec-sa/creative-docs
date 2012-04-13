//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	public static class Currencies
	{
		public static FormattedText GetCurrencySpecies(FormattedText codeISO)
		{
			//	Retourne l'espèce d'une monnaie. Par exemple, "CHF" retourne "Franc".
			foreach (var monnaie in Currencies.CurrenciesForAutoCompleteMenu)
			{
				var m = monnaie.ToSimpleText ();

				var iso = m.Substring (0, 3);
				if (iso == codeISO)
				{
					int i = m.IndexOf ('(');
					if (i == -1)
					{
						return m.Substring (4);
					}
					else
					{
						return m.Substring (4, i-5);
					}
				}
			}

			return FormattedText.Null;
		}


		public static IEnumerable<FormattedText> CurrenciesForAutoCompleteMenu
		{
			//	Retourne la liste des monnaies pour le menu d'un AutoCompleteFieldController.
			//	Source 1: http://www.bureaux-de-change.com/codes-iso-devises.php
			//	Source 2: http://www.currency-iso.org/dl_iso_table_a1.xml
			get
			{
				yield return "AED;Dirham (Émirats arabes unis)";
				yield return "AFN;Afghani (Afghanistan)";
				yield return "ALL;Lek (Albanie)";
				yield return "AMD;Dram (Arménie)";
				yield return "ANG;Florin (Antilles néerlandaises)";
				yield return "AOA;Kwanza (Angola)";
				yield return "ARS;Peso (Argentine)";
				yield return "AUD;Dollar (Australie)";
				yield return "AWG;Florin (Aruba)";
				yield return "AZN;Manat (Azerbaïdjan)";
				yield return "BAM;Mark convertible (Bosnie-Herzégovine)";
				yield return "BBD;Dollar (Barbade)";
				yield return "BDT;Taka (Bangladesh)";
				yield return "BGN;Lev (Bulgarie)";
				yield return "BHD;Dinar (Bahreïn)";
				yield return "BIF;Franc (Burundi)";
				yield return "BMD;Dollar (Bermudes)";
				yield return "BND;Dollar (Brunei)";
				yield return "BOB;Boliviano (Bolivie)";
				yield return "BOV;Mvdol (Bolivie)";
				yield return "BRL;Réal (Brésil)";
				yield return "BSD;Dollar (Bahamas)";
				yield return "BTN;Ngultrum (Bhoutan)";
				yield return "BWP;Pula (Botswana)";
				yield return "BYB;Rouble (Biélorussie)";
				yield return "BZD;Dollar (Belize)";
				yield return "CAD;Dollar (Canada)";
				yield return "CDF;Franc (République Démocratique du Congo)";
				yield return "CHF;Franc (Suisse)";
				yield return "CLP;Peso (Chili)";
				yield return "CNY;Yuan renminbi (Chine, Macao)";
				yield return "COP;Peso (Colombie)";
				yield return "CRC;Colon (Costa Rica)";
				yield return "CUC;Peso convertible (Cuba)";
				yield return "CUP;Peso (Cuba)";
				yield return "CVE;Escudo (Cap-Vert)";
				yield return "CYP;Livre (Chypre)";
				yield return "CZK;Couronne (République tchèque)";
				yield return "DJF;Franc (Djibouti)";
				yield return "DKK;Couronne (Danemark)";
				yield return "DOP;Peso (République dominicaine)";
				yield return "DZD;Dinar (Algérie)";
				yield return "EEK;Couronne (Estonie)";
				yield return "EGP;Livre (Égypte)";
				yield return "ERN;Nafka (Érythrée)";
				yield return "ETB;Birr (Éthiopie)";
				yield return "EUR;Euro (Europe)";
				yield return "FJD;Dollar (Fidji)";
				yield return "FKP;Livre (Falkland)";
				yield return "GBP;Livre sterling (Royaume-Uni)";
				yield return "GEL;Lari (Géorgie)";
				yield return "GHC;Cedi (Ghana)";
				yield return "GIP;Livre (Gibraltar)";
				yield return "GMD;Dalasi (Gambie)";
				yield return "GNF;Franc (Guinée)";
				yield return "GTQ;Quetzal (Guatemala)";
				yield return "GYD;Dollar (Guyana)";
				yield return "HKD;Dollar (Hong Kong)";
				yield return "HNL;Lempira (Honduras)";
				yield return "HRK;Kuna (Croatie)";
				yield return "HTG;Gourde (Haïti)";
				yield return "HUF;Forint (Hongrie)";
				yield return "IDR;Roupie (Indonésie)";
				yield return "ILS;Shekel (Israël)";
				yield return "INR;Roupie (Inde)";
				yield return "IQD;Dinar (Irak)";
				yield return "IRR;Rial (Iran)";
				yield return "ISK;Couronne (Islande)";
				yield return "JMD;Dollar (Jamaïque)";
				yield return "JOD;Dinar (Jordanie)";
				yield return "JPY;Yen (Japon)";
				yield return "KES;Schilling (Kenya)";
				yield return "KGS;Som (Kirghizistan)";
				yield return "KHR;Riel (Cambodge)";
				yield return "KMF;Franc (Comores)";
				yield return "KPW;Won (Corée du Nord)";
				yield return "KRW;Won (Corée du Sud)";
				yield return "KWD;Dinar (Koweït)";
				yield return "KYD;Dollar (Îles Caïmanes)";
				yield return "KZT;Tenge (Kazakhstan)";
				yield return "LAK;Kip (Laos)";
				yield return "LBP;Livre (Liban)";
				yield return "LKR;Roupie (Sri Lanka)";
				yield return "LRD;Dollar (Libéria)";
				yield return "LSL;Loti (Lesotho)";
				yield return "LTL;Litas (Lituanie)";
				yield return "LVL;Lats letton (Lettonie)";
				yield return "LYD;Dinar (Libye)";
				yield return "MAD;Dirham (Maroc)";
				yield return "MDL;Leu (Moldavie)";
				yield return "MGA;Ariary (Madagascar)";
				yield return "MKD;Denar (Macédoine)";
				yield return "MMK;Kyat (Birmanie)";
				yield return "MNT;Tugrik (Mongolie)";
				yield return "MRO;Ouguiya (Mauritanie)";
				yield return "MOP;Patacas (Macao)";
				yield return "MTL;Lire (Malte)";
				yield return "MUR;Roupie (Maurice)";
				yield return "MVR;Rufiyaa (Maldives)";
				yield return "MWK;Kwacha (Malawi)";
				yield return "MXN;Peso (Mexique)";
				yield return "MYR;Ringgit (Malaisie)";
				yield return "MZN;Metical (Mozambique)";
				yield return "NAD;Dollar (Namibie)";
				yield return "NGN;Naira (Nigeria)";
				yield return "NIO;Cordoba d’or (Nicaragua)";
				yield return "NOK;Couronne (Norvège)";
				yield return "NPR;Roupie (Népal)";
				yield return "NZD;Dollar (Nouvelle-Zélande)";
				yield return "OMR;Rial (Oman)";
				yield return "PAB;Balboa (Panamá)";
				yield return "PEN;Sol (Pérou)";
				yield return "PGK;Kina (Papouasie-Nouvelle-Guinée)";
				yield return "PHP;Peso (Philippines)";
				yield return "PKR;Roupie (Pakistan)";
				yield return "PLN;Zloty (Pologne)";
				yield return "PYG;Guarani (Paraguay)";
				yield return "QAR;Rial (Qatar)";
				yield return "RON;Leu (Roumanie)";
				yield return "RSD;Dinar (Serbie)";
				yield return "RUB;Rouble russe (Russie)";
				yield return "RWF;Franc (Rwanda)";
				yield return "SAR;Ryal (Arabie saoudite)";
				yield return "SBD;Dollar (Îles Salomon)";
				yield return "SCR;Roupie (Seychelles)";
				yield return "SDG;Livre (Soudan)";
				yield return "SEK;Couronne (Suède)";
				yield return "SGD;Dollar (Singapour)";
				yield return "SHP;Livre (Sainte-Hélène)";
				yield return "SKK;Couronne (Slovaquie)";
				yield return "SLL;Leone (Sierra Leone)";
				yield return "SOS;Schilling (Somalie)";
				yield return "SRD;Dollar (Suriname)";
				yield return "STD;Dobra (São Tomé-et-Principe)";
				yield return "SVC;Colon (Salvador)";
				yield return "SYP;Livre (Syrie)";
				yield return "SZL;Lilangeni (Swaziland)";
				yield return "THB;Baht (Thaïlande)";
				yield return "TJS;Somoni (Tadjikistan)";
				yield return "TMM;Manat (Turkménistan)";
				yield return "TND;Dinar (Tunisie)";
				yield return "TOP;Pa’anga (Tonga)";
				yield return "TRY;Livre (Turquie)";
				yield return "TTD;Dollar (Trinité-et-Tobago)";
				yield return "TWD;Dollar (Taïwan)";
				yield return "TZS;Schilling (Tanzanie)";
				yield return "UAH;Hryvnia (Ukraine)";
				yield return "UGX;Schilling (Ouganda)";
				yield return "USD;Dollar (États-Unis)";  // Iles Marianne du Nord, Iles Vierges américaines, Samoa Américaines, Porto-Rico, Equateur, Guatemala, Haiti, Turks et Caicos, Iles Vierges Britanniques, Iles Marshall, Micronésie, Panama, Salvador, Timor Oriental
				yield return "UYU;Peso (Uruguay)";
				yield return "UZS;Sum (Ouzbékistan)";
				yield return "VEB;Bolivar (Venezuela)";
				yield return "VND;Dong (Viêt Nam)";
				yield return "VUV;Vatu (Vanuatu)";
				yield return "WST;Tala (Samoa)";
				yield return "XAF;Franc CFA (BEAC)";  // Cameroun, Centrafrique, Congo-Brazzaville, Gabon, Guinée-Equatoriale, Tchad
				yield return "XCD;Dollar (Anguilla)";
				yield return "XOF;Franc CFA (BCEAO)";  // Bénin, Burkina Faso, Côte d'Ivoire, Guinée-Bissau, Mali, Niger, Sénégal, Togo
				yield return "XPF;Franc CFP (IEOM)";  // Nouvelle-Calédonie, Polynésie Française, Wallis-et-Futuna
				yield return "YER;Riyal (Yémen)";
				yield return "ZAR;Rand (Afrique du Sud, Namibie)";
				yield return "ZMK;Kwacha (Zambie)";
				yield return "ZWD;Dollar (Zimbabwe)";

				//	Matières premières:
				yield return "XAG;Argent";
				yield return "XAU;Or";
				yield return "XPD;Palladium";
				yield return "XPT;Platine";
			}
		}
	}
}
