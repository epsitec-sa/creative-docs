using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Menus
{
	public class ZoomMenu : System.IComparable
	{
		public ZoomMenu(double zoom, string comment)
		{
			//	Constructeur unique.
			this.zoom = zoom;

			if ( comment == "" )
			{
				this.text = string.Format("{0}%", (zoom*100.0).ToString("F0"));
			}
			else
			{
				this.text = string.Format("{0}% ({1})", (zoom*100.0).ToString("F0"), comment);
			}
		}

		public bool Equal(double zoom)
		{
			//	Compare si le facteur de zoom est (presque) égal.
			return ( System.Math.Abs(this.zoom-zoom) < 0.0001 );
		}

		public int CompareTo(object obj)
		{
			//	Défini par System.IComparable.
			ZoomMenu zm = obj as ZoomMenu;
			return this.zoom.CompareTo(zm.zoom);
		}


		protected static void Add(System.Collections.ArrayList list, int zoom)
		{
			//	Ajoute une entrée dans une liste si elle n'existe pas encore.
			ZoomMenu.Add(list, zoom/100.0, "");
		}

		protected static void Add(System.Collections.ArrayList list, double zoom, string comment)
		{
			foreach ( ZoomMenu zm in list )
			{
				if ( zm.Equal(zoom) )  return;  // n'ajoute pas si existe déjà
			}

			list.Add(new ZoomMenu(zoom, comment));
		}

		public static VMenu CreateZoomMenu(double currentZoom, double zoomPage, Support.EventHandler<MessageEventArgs> message)
		{
			//	Construit le menu pour changer de zoom.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			ZoomMenu.Add(list, zoomPage, Res.Strings.Menu.Zoom.Page);
			ZoomMenu.Add(list, currentZoom, Res.Strings.Menu.Zoom.Current);

			ZoomMenu.Add(list,   10);
			ZoomMenu.Add(list,   20);
			ZoomMenu.Add(list,   30);
			ZoomMenu.Add(list,   40);
			ZoomMenu.Add(list,   50);
			ZoomMenu.Add(list,   60);
			ZoomMenu.Add(list,   70);
			ZoomMenu.Add(list,   80);
			ZoomMenu.Add(list,   90);
			ZoomMenu.Add(list,  100);
			ZoomMenu.Add(list,  120);
			ZoomMenu.Add(list,  140);
			ZoomMenu.Add(list,  160);
			ZoomMenu.Add(list,  180);
			ZoomMenu.Add(list,  200);
			ZoomMenu.Add(list,  300);
			ZoomMenu.Add(list,  400);
			ZoomMenu.Add(list,  500);
			ZoomMenu.Add(list,  600);
			ZoomMenu.Add(list,  700);
			ZoomMenu.Add(list,  800);
			ZoomMenu.Add(list,  900);
			ZoomMenu.Add(list, 1000);

			list.Sort();

			VMenu menu = new VMenu();
			foreach ( ZoomMenu zm in list )
			{
				if ( zm.zoom == 1.0 )  // 100% ?
				{
					menu.Items.Add(new MenuSeparator());
				}

				string name = zm.text;

				string icon = Misc.Icon("RadioNo");
				if ( zm.Equal(currentZoom) )
				{
					icon = Misc.Icon("RadioYes");
					name = Misc.Bold(name);
				}

				string cmd = "ZoomChange";
				Misc.CreateStructuredCommandWithName (cmd);

				MenuItem item = new MenuItem(cmd, icon, name, "", zm.zoom.ToString());

				if ( message != null )
				{
					item.Pressed += message;
				}

				menu.Items.Add(item);

				if ( zm.zoom == 1.0 )  // 100% ?
				{
					menu.Items.Add(new MenuSeparator());
				}
			}
			menu.AdjustSize();
			return menu;
		}


		protected double		zoom;
		protected string		text;
	}
}
