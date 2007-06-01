using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.EntitiesEditor
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
			//	Compare si le facteur de zoom est (presque) �gal.
			return ( System.Math.Abs(this.zoom-zoom) < 0.0001 );
		}

		public int CompareTo(object obj)
		{
			//	D�fini par System.IComparable.
			ZoomMenu zm = obj as ZoomMenu;
			return this.zoom.CompareTo(zm.zoom);
		}


		protected static void Add(System.Collections.ArrayList list, int zoom)
		{
			//	Ajoute une entr�e dans une liste si elle n'existe pas encore.
			ZoomMenu.Add(list, zoom/100.0, "");
		}

		protected static void Add(System.Collections.ArrayList list, double zoom, string comment)
		{
			foreach ( ZoomMenu zm in list )
			{
				if ( zm.Equal(zoom) )  return;  // n'ajoute pas si existe d�j�
			}

			list.Add(new ZoomMenu(zoom, comment));
		}

		public static VMenu CreateZoomMenu(double currentZoom, double zoomPage, MessageEventHandler message)
		{
			//	Construit le menu pour changer de zoom.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			ZoomMenu.Add(list, 1, "zoom 1:1");
			ZoomMenu.Add(list, zoomPage, "pleine page");
			ZoomMenu.Add(list, currentZoom, "zoom courant");

			ZoomMenu.Add(list,   20);
			ZoomMenu.Add(list,   50);
			ZoomMenu.Add(list,   80);
			ZoomMenu.Add(list,  100);
			ZoomMenu.Add(list,  120);
			ZoomMenu.Add(list,  150);
			ZoomMenu.Add(list,  200);

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
				Misc.CreateStructuredCommandWithName(cmd);

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
