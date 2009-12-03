//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Graph
{
	/// <summary>
	/// The <c>GraphSerial</c> class implements licensing code.
	/// </summary>
	public static class GraphSerial
	{
		public static void CheckLicense(Window owner)
		{
			var info = GraphSerial.LicensingInfo;

			if ((info == LicensingInfo.Unknown) ||
				(info == LicensingInfo.Expired))
			{
				var compta = GraphSerial.GetComptactComptaSerial ();
				var graph  = GraphSerial.GetComptactGraphSerial ();

				if (GraphSerial.hasComptaLicense == false)
				{
					var dialog = new Dialogs.QuestionDialog (Res.Captions.Message.LicenseInvalid.QuestionStandalone,
						GraphSerial.hasGraphLicense ? Res.Captions.Message.LicenseInvalid.Option1UpdateGraph : Res.Captions.Message.LicenseInvalid.Option1BuyGraph,
						Res.Captions.Message.LicenseInvalid.Option3Quit)
					{
						OwnerWindow = owner
					};

					dialog.OpenDialog ();

					switch (dialog.Result)
					{
						case Epsitec.Common.Dialogs.DialogResult.Answer1:
							GraphSerial.OpenUrlBuyOrUpdateGraph (graph);
							break;
					}
				}
				else
				{
					var dialog = new Dialogs.QuestionDialog (Res.Captions.Message.LicenseInvalid.Question,
						GraphSerial.hasGraphLicense ? Res.Captions.Message.LicenseInvalid.Option1UpdateGraph : Res.Captions.Message.LicenseInvalid.Option1BuyGraph,
						Res.Captions.Message.LicenseInvalid.Option2UpdateCompta,
						Res.Captions.Message.LicenseInvalid.Option3Quit)
					{
						OwnerWindow = owner
					};

					dialog.OpenDialog ();

					switch (dialog.Result)
					{
						case Epsitec.Common.Dialogs.DialogResult.Answer1:
							GraphSerial.OpenUrlBuyOrUpdateGraph (graph);
							break;

						case Epsitec.Common.Dialogs.DialogResult.Answer2:
							UrlNavigator.OpenUrl (string.Format (GraphSerial.UrlBuyUpdate, compta));
							break;
					}
				}

				System.Environment.Exit (-1);
			}
		}


		public static LicensingInfo LicensingInfo
		{
			get
			{
				if (GraphSerial.licensingInfo == LicensingInfo.Undefined)
				{
					GraphSerial.licensingInfo = GraphSerial.GetLicensingInfo ();
				}

				return GraphSerial.licensingInfo;
			}
		}

		public static string LicensingFriendlyName
		{
			get
			{
				switch (GraphSerial.LicensingInfo)
				{
					case LicensingInfo.ValidPiccolo:
						return "PICCOLO";

					case LicensingInfo.ValidPro:
						return "PRO";

					case LicensingInfo.ValidLargo:
						return "LARGO";
					
					default:
						return "DEMO";
				}
			}
		}


		private static LicensingInfo GetLicensingInfo()
		{
			var graphId  = (string) Microsoft.Win32.Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Epsitec\Cresus Graphe\Setup", "InstallID", "");
			var comptaId = (string) Microsoft.Win32.Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Epsitec\Cresus\Setup", "InstallID", "");

			System.Diagnostics.Trace.WriteLine ("Crésus Graphe InstallID : " + graphId);
			System.Diagnostics.Trace.WriteLine ("Crésus Compta InstallID : " + comptaId);
			
			var now  = System.DateTime.Now;
			bool updatesAllowed;

			if (SerialAlgorithm.CheckSerial (graphId, 29, out updatesAllowed))
            {
				GraphSerial.hasGraphLicense = true;
				GraphSerial.hasValidGraphLicense = updatesAllowed;

				System.Diagnostics.Trace.WriteLine ("Graphe license validity : " + updatesAllowed.ToString ());
			}

			if (SerialAlgorithm.CheckSerial (comptaId, 20, out updatesAllowed))
			{
				GraphSerial.hasComptaLicense = true;
				GraphSerial.hasValidComptaLicense = updatesAllowed;

				System.Diagnostics.Trace.WriteLine ("Compta license validity : " + updatesAllowed.ToString ());
			}

			if (GraphSerial.hasGraphLicense)
            {
				int product = SerialAlgorithm.GetProduct (graphId);

				if (GraphSerial.hasValidGraphLicense)
				{
					switch (product)
					{
						case 029:
							return LicensingInfo.ValidPro;
						case 229:
							return LicensingInfo.ValidLargo;
						case 829:
							return LicensingInfo.ValidPiccolo;
					}
				}
            }

			if (GraphSerial.hasValidComptaLicense)
            {
				return LicensingInfo.ValidPiccolo;
            }
			if (GraphSerial.hasComptaLicense)
			{
				return LicensingInfo.Expired;
			}

			return LicensingInfo.Unknown;
		}

		
		private static string GetComptactGraphSerial()
		{
			var graphId = (string) Microsoft.Win32.Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Epsitec\Cresus Graphe\Setup", "InstallID", "");

			if (SerialAlgorithm.CheckSerial (graphId, 29))
			{
				return graphId.Replace ("-", "");
			}
			else
			{
				return null;
			}
		}

		private static string GetComptactComptaSerial()
		{
			var comptaId = (string) Microsoft.Win32.Registry.GetValue (@"HKEY_LOCAL_MACHINE\SOFTWARE\Epsitec\Cresus\Setup", "InstallID", "");

			if (SerialAlgorithm.CheckSerial (comptaId, 20))
			{
				return comptaId.Replace ("-", "");
			}
			else
			{
				return null;
			}
		}

		private static void OpenUrlBuyOrUpdateGraph(string graph)
		{
			if (GraphSerial.hasGraphLicense)
			{
				UrlNavigator.OpenUrl (string.Format (GraphSerial.UrlBuyUpdate, graph));
			}
			else
			{
				UrlNavigator.OpenUrl (GraphSerial.UrlBuyFull);
			}
		}

		
		private const string UrlBuyFull = "https://www.epsitec.ch/buy/full/";
		private const string UrlBuyUpdate = "https://www.epsitec.ch/buy/update/index.htm?url_key={0}";
		
		private static bool hasGraphLicense;
		private static bool hasValidGraphLicense;
		private static bool hasComptaLicense;
		private static bool hasValidComptaLicense;

		private static LicensingInfo licensingInfo; // = LicensingInfo.ValidPro;
	}
}