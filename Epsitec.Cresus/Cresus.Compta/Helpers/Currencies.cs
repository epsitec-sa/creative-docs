//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			//	Retourne l'esp�ce d'une monnaie. Par exemple, "CHF" retourne "Franc".
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
				yield return "AED;Dirham (�mirats arabes unis)";
				yield return "AFN;Afghani (Afghanistan)";
				yield return "ALL;Lek (Albanie)";
				yield return "AMD;Dram (Arm�nie)";
				yield return "ANG;Florin (Antilles n�erlandaises)";
				yield return "AOA;Kwanza (Angola)";
				yield return "ARS;Peso (Argentine)";
				yield return "AUD;Dollar (Australie)";
				yield return "AWG;Florin (Aruba)";
				yield return "AZN;Manat (Azerba�djan)";
				yield return "BAM;Mark convertible (Bosnie-Herz�govine)";
				yield return "BBD;Dollar (Barbade)";
				yield return "BDT;Taka (Bangladesh)";
				yield return "BGN;Lev (Bulgarie)";
				yield return "BHD;Dinar (Bahre�n)";
				yield return "BIF;Franc (Burundi)";
				yield return "BMD;Dollar (Bermudes)";
				yield return "BND;Dollar (Brunei)";
				yield return "BOB;Boliviano (Bolivie)";
				yield return "BOV;Mvdol (Bolivie)";
				yield return "BRL;R�al (Br�sil)";
				yield return "BSD;Dollar (Bahamas)";
				yield return "BTN;Ngultrum (Bhoutan)";
				yield return "BWP;Pula (Botswana)";
				yield return "BYB;Rouble (Bi�lorussie)";
				yield return "BZD;Dollar (Belize)";
				yield return "CAD;Dollar (Canada)";
				yield return "CDF;Franc (R�publique D�mocratique du Congo)";
				yield return "CHF;Franc (Suisse)";
				yield return "CLP;Peso (Chili)";
				yield return "CNY;Yuan renminbi (Chine, Macao)";
				yield return "COP;Peso (Colombie)";
				yield return "CRC;Colon (Costa Rica)";
				yield return "CUC;Peso convertible (Cuba)";
				yield return "CUP;Peso (Cuba)";
				yield return "CVE;Escudo (Cap-Vert)";
				yield return "CYP;Livre (Chypre)";
				yield return "CZK;Couronne (R�publique tch�que)";
				yield return "DJF;Franc (Djibouti)";
				yield return "DKK;Couronne (Danemark)";
				yield return "DOP;Peso (R�publique dominicaine)";
				yield return "DZD;Dinar (Alg�rie)";
				yield return "EEK;Couronne (Estonie)";
				yield return "EGP;Livre (�gypte)";
				yield return "ERN;Nafka (�rythr�e)";
				yield return "ETB;Birr (�thiopie)";
				yield return "EUR;Euro (Europe)";
				yield return "FJD;Dollar (Fidji)";
				yield return "FKP;Livre (Falkland)";
				yield return "GBP;Livre sterling (Royaume-Uni)";
				yield return "GEL;Lari (G�orgie)";
				yield return "GHC;Cedi (Ghana)";
				yield return "GIP;Livre (Gibraltar)";
				yield return "GMD;Dalasi (Gambie)";
				yield return "GNF;Franc (Guin�e)";
				yield return "GTQ;Quetzal (Guatemala)";
				yield return "GYD;Dollar (Guyana)";
				yield return "HKD;Dollar (Hong Kong)";
				yield return "HNL;Lempira (Honduras)";
				yield return "HRK;Kuna (Croatie)";
				yield return "HTG;Gourde (Ha�ti)";
				yield return "HUF;Forint (Hongrie)";
				yield return "IDR;Roupie (Indon�sie)";
				yield return "ILS;Shekel (Isra�l)";
				yield return "INR;Roupie (Inde)";
				yield return "IQD;Dinar (Irak)";
				yield return "IRR;Rial (Iran)";
				yield return "ISK;Couronne (Islande)";
				yield return "JMD;Dollar (Jama�que)";
				yield return "JOD;Dinar (Jordanie)";
				yield return "JPY;Yen (Japon)";
				yield return "KES;Schilling (Kenya)";
				yield return "KGS;Som (Kirghizistan)";
				yield return "KHR;Riel (Cambodge)";
				yield return "KMF;Franc (Comores)";
				yield return "KPW;Won (Cor�e du Nord)";
				yield return "KRW;Won (Cor�e du Sud)";
				yield return "KWD;Dinar (Kowe�t)";
				yield return "KYD;Dollar (�les Ca�manes)";
				yield return "KZT;Tenge (Kazakhstan)";
				yield return "LAK;Kip (Laos)";
				yield return "LBP;Livre (Liban)";
				yield return "LKR;Roupie (Sri Lanka)";
				yield return "LRD;Dollar (Lib�ria)";
				yield return "LSL;Loti (Lesotho)";
				yield return "LTL;Litas (Lituanie)";
				yield return "LVL;Lats letton (Lettonie)";
				yield return "LYD;Dinar (Libye)";
				yield return "MAD;Dirham (Maroc)";
				yield return "MDL;Leu (Moldavie)";
				yield return "MGA;Ariary (Madagascar)";
				yield return "MKD;Denar (Mac�doine)";
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
				yield return "NIO;Cordoba d�or (Nicaragua)";
				yield return "NOK;Couronne (Norv�ge)";
				yield return "NPR;Roupie (N�pal)";
				yield return "NZD;Dollar (Nouvelle-Z�lande)";
				yield return "OMR;Rial (Oman)";
				yield return "PAB;Balboa (Panam�)";
				yield return "PEN;Sol (P�rou)";
				yield return "PGK;Kina (Papouasie-Nouvelle-Guin�e)";
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
				yield return "SBD;Dollar (�les Salomon)";
				yield return "SCR;Roupie (Seychelles)";
				yield return "SDG;Livre (Soudan)";
				yield return "SEK;Couronne (Su�de)";
				yield return "SGD;Dollar (Singapour)";
				yield return "SHP;Livre (Sainte-H�l�ne)";
				yield return "SKK;Couronne (Slovaquie)";
				yield return "SLL;Leone (Sierra Leone)";
				yield return "SOS;Schilling (Somalie)";
				yield return "SRD;Dollar (Suriname)";
				yield return "STD;Dobra (S�o Tom�-et-Principe)";
				yield return "SVC;Colon (Salvador)";
				yield return "SYP;Livre (Syrie)";
				yield return "SZL;Lilangeni (Swaziland)";
				yield return "THB;Baht (Tha�lande)";
				yield return "TJS;Somoni (Tadjikistan)";
				yield return "TMM;Manat (Turkm�nistan)";
				yield return "TND;Dinar (Tunisie)";
				yield return "TOP;Pa�anga (Tonga)";
				yield return "TRY;Livre (Turquie)";
				yield return "TTD;Dollar (Trinit�-et-Tobago)";
				yield return "TWD;Dollar (Ta�wan)";
				yield return "TZS;Schilling (Tanzanie)";
				yield return "UAH;Hryvnia (Ukraine)";
				yield return "UGX;Schilling (Ouganda)";
				yield return "USD;Dollar (�tats-Unis)";  // Iles Marianne du Nord, Iles Vierges am�ricaines, Samoa Am�ricaines, Porto-Rico, Equateur, Guatemala, Haiti, Turks et Caicos, Iles Vierges Britanniques, Iles Marshall, Micron�sie, Panama, Salvador, Timor Oriental
				yield return "UYU;Peso (Uruguay)";
				yield return "UZS;Sum (Ouzb�kistan)";
				yield return "VEB;Bolivar (Venezuela)";
				yield return "VND;Dong (Vi�t Nam)";
				yield return "VUV;Vatu (Vanuatu)";
				yield return "WST;Tala (Samoa)";
				yield return "XAF;Franc CFA (BEAC)";  // Cameroun, Centrafrique, Congo-Brazzaville, Gabon, Guin�e-Equatoriale, Tchad
				yield return "XCD;Dollar (Anguilla)";
				yield return "XOF;Franc CFA (BCEAO)";  // B�nin, Burkina Faso, C�te d'Ivoire, Guin�e-Bissau, Mali, Niger, S�n�gal, Togo
				yield return "XPF;Franc CFP (IEOM)";  // Nouvelle-Cal�donie, Polyn�sie Fran�aise, Wallis-et-Futuna
				yield return "YER;Riyal (Y�men)";
				yield return "ZAR;Rand (Afrique du Sud, Namibie)";
				yield return "ZMK;Kwacha (Zambie)";
				yield return "ZWD;Dollar (Zimbabwe)";

				//	Mati�res premi�res:
				yield return "XAG;Argent";
				yield return "XAU;Or";
				yield return "XPD;Palladium";
				yield return "XPT;Platine";
			}
		}
	}
}
