﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class DummyGroups
	{
		public static void AddGroups(DataMandat mandat)
		{
			var categories = mandat.GetData (BaseType.Groups);

			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);

			var oRoot = new DataObject ();
			categories.Add (oRoot);
			{
				var e = new DataEvent (start, EventType.Input);
				oRoot.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Groupes"));
			}

			///////////////

			{
				var oParent = new DataObject ();
				categories.Add (oParent);
				{
					var e = new DataEvent (start, EventType.Input);
					oParent.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
					e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oRoot.Guid));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Catégories MCH2"));
					e.AddProperty (new DataStringProperty (ObjectField.Number, "100"));
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Terrains"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "100.10"));
					}
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Routes"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "100.20"));
					}
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Traitement des eaux"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "100.30"));
					}
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Travaux de génie civil"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "100.40"));
					}
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Immeubles"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "100.50"));
					}

					{
						var oo = new DataObject ();
						categories.Add (oo);
						{
							var e = new DataEvent (start, EventType.Input);
							oo.AddEvent (e);
							e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
							e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o.Guid));
							e.AddProperty (new DataStringProperty (ObjectField.Name, "Bâtiments"));
							e.AddProperty (new DataStringProperty (ObjectField.Number, "10"));
						}
					}

					{
						var oo = new DataObject ();
						categories.Add (oo);
						{
							var e = new DataEvent (start, EventType.Input);
							oo.AddEvent (e);
							e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
							e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o.Guid));
							e.AddProperty (new DataStringProperty (ObjectField.Name, "Usines"));
							e.AddProperty (new DataStringProperty (ObjectField.Number, "20"));
						}
					}

					{
						var oo = new DataObject ();
						categories.Add (oo);
						{
							var e = new DataEvent (start, EventType.Input);
							oo.AddEvent (e);
							e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
							e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o.Guid));
							e.AddProperty (new DataStringProperty (ObjectField.Name, "Entrepôts"));
							e.AddProperty (new DataStringProperty (ObjectField.Number, "21"));
						}
					}

					{
						var oo = new DataObject ();
						categories.Add (oo);
						{
							var e = new DataEvent (start, EventType.Input);
							oo.AddEvent (e);
							e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
							e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o.Guid));
							e.AddProperty (new DataStringProperty (ObjectField.Name, "Ecoles"));
							e.AddProperty (new DataStringProperty (ObjectField.Number, "11"));
						}
					}
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Mobilier"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "100.60"));
					}
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Véhicules"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "100.70"));
					}

					{
						var oo = new DataObject ();
						categories.Add (oo);
						{
							var e = new DataEvent (start, EventType.Input);
							oo.AddEvent (e);
							e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
							e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o.Guid));
							e.AddProperty (new DataStringProperty (ObjectField.Name, "Camions"));
							e.AddProperty (new DataStringProperty (ObjectField.Number, "10"));
						}
					}

					{
						var oo = new DataObject ();
						categories.Add (oo);
						{
							var e = new DataEvent (start, EventType.Input);
							oo.AddEvent (e);
							e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
							e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o.Guid));
							e.AddProperty (new DataStringProperty (ObjectField.Name, "Camionnettes"));
							e.AddProperty (new DataStringProperty (ObjectField.Number, "20"));
						}
					}

					{
						var oo = new DataObject ();
						categories.Add (oo);
						{
							var e = new DataEvent (start, EventType.Input);
							oo.AddEvent (e);
							e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
							e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o.Guid));
							e.AddProperty (new DataStringProperty (ObjectField.Name, "Voitures"));
							e.AddProperty (new DataStringProperty (ObjectField.Number, "30"));
						}
					}
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Machines"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "100.80"));
					}
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "En construction"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "100.90"));
					}
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Autres immobilisations"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "100.95"));
					}
				}
			}

			///////////////

			{
				var oParent = new DataObject ();
				categories.Add (oParent);
				{
					var e = new DataEvent (start, EventType.Input);
					oParent.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
					e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oRoot.Guid));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Types MCH2"));
					e.AddProperty (new DataStringProperty (ObjectField.Number, "200"));
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Immobilisations corporelles"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "1"));
					}
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Immobilisations incorporelles"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "2"));
					}
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Immobilisations financières"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "3"));
					}
				}
			}

			///////////////

			{
				var oParent = new DataObject ();
				categories.Add (oParent);
				{
					var e = new DataEvent (start, EventType.Input);
					oParent.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
					e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oRoot.Guid));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Patrimoine MCH2"));
					e.AddProperty (new DataStringProperty (ObjectField.Number, "300"));
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Administratif"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "1"));
					}
				}

				{
					var o = new DataObject ();
					categories.Add (o);
					{
						var e = new DataEvent (start, EventType.Input);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
						e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oParent.Guid));
						e.AddProperty (new DataStringProperty (ObjectField.Name, "Financier"));
						e.AddProperty (new DataStringProperty (ObjectField.Number, "2"));
					}
				}
			}

			///////////////

#if false
			var oImmob = new DataObject ();
			categories.Add (oImmob);
			{
				var e = new DataEvent (start, EventType.Input);
				oImmob.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oRoot.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Immobilisations"));
			}

			var o1 = new DataObject ();
			categories.Add (o1);
			{
				var e = new DataEvent (start, EventType.Input);
				o1.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oImmob.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Bâtiments"));
			}

			var o11 = new DataObject ();
			categories.Add (o11);
			{
				var e = new DataEvent (start, EventType.Input);
				o11.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o1.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Edifices"));
			}

			var o12 = new DataObject ();
			categories.Add (o12);
			{
				var e = new DataEvent (start, EventType.Input);
				o12.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o1.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Usines"));
			}

			var o121 = new DataObject ();
			categories.Add (o121);
			{
				var e = new DataEvent (start, EventType.Input);
				o121.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o12.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Suisses"));
			}

			var o122 = new DataObject ();
			categories.Add (o122);
			{
				var e = new DataEvent (start, EventType.Input);
				o122.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o12.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Etrangères"));
			}

			var o13 = new DataObject ();
			categories.Add (o13);
			{
				var e = new DataEvent (start, EventType.Input);
				o13.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o1.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Entrepôts"));
			}

			///////////////

			var o2 = new DataObject ();
			categories.Add (o2);
			{
				var e = new DataEvent (start, EventType.Input);
				o2.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oImmob.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Véhicules"));
			}

			var o21 = new DataObject ();
			categories.Add (o21);
			{
				var e = new DataEvent (start, EventType.Input);
				o21.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Camions"));
			}

			var o22 = new DataObject ();
			categories.Add (o22);
			{
				var e = new DataEvent (start, EventType.Input);
				o22.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Camionnettes"));
			}

			var o23 = new DataObject ();
			categories.Add (o23);
			{
				var e = new DataEvent (start, EventType.Input);
				o23.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Voitures"));
			}
#endif

			///////////////

			var o3 = new DataObject ();
			categories.Add (o3);
			{
				var e = new DataEvent (start, EventType.Input);
				o3.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oRoot.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Secteurs"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "500"));
			}

			var o31 = new DataObject ();
			categories.Add (o31);
			{
				var e = new DataEvent (start, EventType.Input);
				o31.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Nord"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "1"));
			}

			var o32 = new DataObject ();
			categories.Add (o32);
			{
				var e = new DataEvent (start, EventType.Input);
				o32.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Sud"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "2"));
			}

			var o33 = new DataObject ();
			categories.Add (o33);
			{
				var e = new DataEvent (start, EventType.Input);
				o33.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Est"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "3"));
			}

			var o34 = new DataObject ();
			categories.Add (o34);
			{
				var e = new DataEvent (start, EventType.Input);
				o34.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Ouest"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "4"));
			}

			///////////////

			var o4 = new DataObject ();
			categories.Add (o4);
			{
				var e = new DataEvent (start, EventType.Input);
				o4.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oRoot.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Centres de frais"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "510"));
			}

			var o41 = new DataObject ();
			categories.Add (o41);
			{
				var e = new DataEvent (start, EventType.Input);
				o41.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Atelier"));
			}

			var o42 = new DataObject ();
			categories.Add (o42);
			{
				var e = new DataEvent (start, EventType.Input);
				o42.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Bureaux"));
			}

			var o43 = new DataObject ();
			categories.Add (o43);
			{
				var e = new DataEvent (start, EventType.Input);
				o43.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Distribution"));
			}

			var o44 = new DataObject ();
			categories.Add (o44);
			{
				var e = new DataEvent (start, EventType.Input);
				o44.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Stockage"));
			}

			var o45 = new DataObject ();
			categories.Add (o45);
			{
				var e = new DataEvent (start, EventType.Input);
				o45.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Transports"));
			}


			///////////////

			var o5 = new DataObject ();
			categories.Add (o5);
			{
				var e = new DataEvent (start, EventType.Input);
				o5.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oRoot.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Responsables"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "520"));
			}

			var o51 = new DataObject ();
			categories.Add (o51);
			{
				var e = new DataEvent (start, EventType.Input);
				o51.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Simone"));
			}

			var o52 = new DataObject ();
			categories.Add (o52);
			{
				var e = new DataEvent (start, EventType.Input);
				o52.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Georges-André"));
			}

			var o53 = new DataObject ();
			categories.Add (o53);
			{
				var e = new DataEvent (start, EventType.Input);
				o53.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Béatrice"));
			}

			var o54 = new DataObject ();
			categories.Add (o54);
			{
				var e = new DataEvent (start, EventType.Input);
				o54.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Dominique"));
			}

			var o55 = new DataObject ();
			categories.Add (o55);
			{
				var e = new DataEvent (start, EventType.Input);
				o55.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Joël"));
			}

			var o56 = new DataObject ();
			categories.Add (o56);
			{
				var e = new DataEvent (start, EventType.Input);
				o56.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Paul-Henry"));
			}

			var o57 = new DataObject ();
			categories.Add (o57);
			{
				var e = new DataEvent (start, EventType.Input);
				o57.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Jean-Daniel"));
			}

			var o58 = new DataObject ();
			categories.Add (o58);
			{
				var e = new DataEvent (start, EventType.Input);
				o58.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Sandra"));
			}
			///////////////

			var o6 = new DataObject ();
			categories.Add (o6);
			{
				var e = new DataEvent (start, EventType.Input);
				o6.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, oRoot.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Placements"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "530"));
			}

			var o61 = new DataObject ();
			categories.Add (o61);
			{
				var e = new DataEvent (start, EventType.Input);
				o61.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Portefeuille d’actions suisses"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "100"));
			}

			var o62 = new DataObject ();
			categories.Add (o62);
			{
				var e = new DataEvent (start, EventType.Input);
				o62.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Portefeuille d’actions européennes"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "200"));
			}

			var o63 = new DataObject ();
			categories.Add (o63);
			{
				var e = new DataEvent (start, EventType.Input);
				o63.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Portefeuille d’actions nord-américaines"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "410"));
			}

			var o64 = new DataObject ();
			categories.Add (o64);
			{
				var e = new DataEvent (start, EventType.Input);
				o64.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Portefeuille d’actions sub-américaines"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "420"));
			}

			var o65 = new DataObject ();
			categories.Add (o65);
			{
				var e = new DataEvent (start, EventType.Input);
				o65.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyGroups.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Portefeuille d’actions asiatiques"));
				e.AddProperty (new DataStringProperty (ObjectField.Number, "700"));
			}
		}

		public static GuidRatio GetGroup(DataMandat mandat, string text, decimal? ratio = null)
		{
			var list = mandat.GetData (BaseType.Groups);

			foreach (var group in list)
			{
				var nom = ObjectProperties.GetObjectPropertyString (group, null, ObjectField.Name);
				if (nom == text)
				{
					return new GuidRatio(group.Guid, ratio);
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("Le groupe {0} n'existe pas !", text));
			return GuidRatio.Empty;
		}


		private static int GroupNumber = 1;
	}
}