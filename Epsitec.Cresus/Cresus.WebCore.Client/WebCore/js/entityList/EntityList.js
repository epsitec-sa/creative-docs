// This class is the base class of all entity lists. Entity lists are an array
// of entites that are loaded dynamicall from the server in a buffered store
// that can be sorted and filtered. The columns that are displayed are choosen
// by the user.

Ext.require([
  'Epsitec.cresus.webcore.tools.Callback',
  'Epsitec.cresus.webcore.tools.BooleanNullableColumn',
  'Epsitec.cresus.webcore.tools.Enumeration',
  'Epsitec.cresus.webcore.tools.ErrorHandler',
  'Epsitec.cresus.webcore.tools.ListColumn',
  'Epsitec.cresus.webcore.tools.Texts',
  'Epsitec.cresus.webcore.tools.TimeFilter',
  'Epsitec.cresus.webcore.tools.Tools',
  'Epsitec.cresus.webcore.ui.ArrayExportWindow',
  'Epsitec.cresus.webcore.ui.LabelExportWindow',
  'Epsitec.cresus.webcore.ui.SortWindow',
  'Epsitec.cresus.webcore.ui.SearchWindow',
  'Epsitec.cresus.webcore.ui.querybuilder.QueryWindow',
  'Ext.ux.grid.FiltersFeature',
  'Ext.ux.DataTip',
  'Ext.Action'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityList.EntityList', {
    extend: 'Ext.grid.Panel',
    alternateClassName: ['Epsitec.EntityList'],

    /* Configuration */

    border: false,
    viewConfig: {
      emptyText: Epsitec.Texts.getEmptyListText()
    },

    /* Properties */

    onSelectionChangeCallback: null,
    columnDefinitions: null,
    sorterDefinitions: null,
    labelExportDefinitions: null,
    getUrl: null,
    exportUrl: null,
    queryName: null,
    fullSearchWindow: null,
    queryBuilder: null,
    isSearching: false,
    isReloading: false,
    beforeReloadEntityId: null,
    selectedColumnId: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions, contextMenu;

      if(Ext.isDefined(options.sorterDefinitions[0]))
      {
        this.selectedColumnId = options.sorterDefinitions[0].name;
      }

      newOptions = {
        dockedItems: [
          this.createToolbar(options),
          this.createQueryToolbar(),
          this.createSecondaryToolbar()
        ],
        columns: this.createColumns(options),
        store: this.createStore(
            options.getUrl, true, options.columnDefinitions,
            options.sorterDefinitions
        ),
        /*plugins: {
          ptype: 'bufferedrenderer',
          trailingBufferZone: 20,  // Keep X rows rendered in the table behind scroll
          leadingBufferZone: 50   // Keep X rows rendered in the table ahead of scroll
        },*/
        selModel: this.createSelModel(options),
        onSelectionChangeCallback: options.onSelectionChange,
        listeners: {
          selectionchange: this.onSelectionChangeHandler,
          columnhide: this.setupColumnParameter,
          columnshow: this.setupColumnParameterAndRefresh,
          reconfigure: this.reconfigureFiltersFeature,
          //headerclick: function(ct, column) { this.selectedColumnId = column.dataIndex; },
          scope: this
        },
        features: [{
          ftype: 'filters',
          encode: true
        }]
      };

      if (epsitecConfig.featureContextualMenu) {
        if (!Ext.isEmpty(options.menuItems))
        {
          contextMenu = this.createContextMenu(options);

          newOptions.listeners.itemcontextmenu = function(v, r, n, i, e) {
              e.stopEvent();
              this.getSelectionModel().select(r);
              contextMenu.showAt(e.getXY());
              return false;
            };
        }
      }

      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Methods */
    createContextMenu: function(options) {
      return Ext.create('Ext.menu.Menu', {
        items: this.createContextMenuItems(options.menuItems)
      });
    },

    createContextMenuItems: function(menuItems) {
      return menuItems.map(this.createContextMenuItem, this);
    },

    createContextMenuItem: function(item) {
      switch (item.type) {
        case 'summarynavigation':
          return this.createContextMenuSummaryNavigationItem(item);

        default:
          throw 'invalid context menu item type: ' + item.type;
      }
    },

    createContextMenuSummaryNavigationItem: function(item) {
      return Ext.create('Ext.Action', {
        icon: '/images/Epsitec/Cresus/Core/Images/Base/' +
            'BusinessSettings/icon16.png',
        text: item.title,
        disabled: false,
        item: item,
        handler: this.summaryNavigationMenuHandler,
        scope: this
      });
    },

    summaryNavigationMenuHandler: function(widget, event) {
        var rec, path, app;
      rec = this.getSelectionModel().getSelection()[0];
      if (rec) {
        path = {
          databaseName: widget.item.databaseName,
          entityId: rec.raw[widget.item.columnName]
        };
        app = Epsitec.Cresus.Core.getApplication();
        app.showEntity(path, null);
      }
    },

    //AJOUTER AU PANIER
    onEntityBagAddHandler: function(widget, event) {
      var rec,entity,app;
      app = Epsitec.Cresus.Core.getApplication();
      rec = this.getSelectionModel().getSelection()[0];
      if (rec) {
        entity = {
          summary: rec.raw.summary,
          entityType: Epsitec.Cresus.Core.app.getCurrentDatabaseEntityType(),
          id: rec.raw.id
        };

        app.addEntityToBag(entity);
      }
    },

    createColumns: function(options) {
      var basicColumns = this.createBasicColumns(options.columnDefinitions),
          dynamicColumns = this.createDynamicColumns(options.columnDefinitions);

      return basicColumns.concat(dynamicColumns);
    },

    createBasicColumns: function(columnDefinitions) {
      var basicColumns = [{
        xtype: 'rownumberer',
        width: 35,
        sortable: false,
        resizable: true
      }];

      if (Ext.isEmpty(columnDefinitions)) {
        basicColumns.push({
          xtype: 'gridcolumn',
          text: Epsitec.Texts.getSummaryHeader(),
          flex: 1,
          dataIndex: 'summary',
          sortable: false
        });
      }

      return basicColumns;
    },

    createDynamicColumns: function(columnDefinitions) {
      return columnDefinitions.map(this.createDynamicColumn, this);
    },

    createDynamicColumn: function(columnDefinition) {
      var column = {
        text: columnDefinition.title,
        dataIndex: columnDefinition.name,
        sortable: columnDefinition.sortable,
        hidden: columnDefinition.hidden,
        filter: this.createFilter(columnDefinition)
      };

      if (columnDefinition.width === null) {
        column.flex = 1;
      }
      else {
        column.width = columnDefinition.width;
      }

      switch (columnDefinition.type.type) {
        case 'boolean':
          column.xtype = 'booleannullablecolumn';
          break;

        case 'date':
          column.xtype = 'datecolumn';
          column.format = 'd.m.Y';
          break;

        case 'dateTime':
          column.xtype = 'datecolumn';
          column.format = 'd.m.Y H:i:s';
          break;

        case 'int':
          column.xtype = 'numbercolumn';
          column.format = '0,000';
          break;

        case 'float':
          column.xtype = 'numbercolumn';
          break;

        case 'list':
          column.xtype = 'listcolumn';
          column.enumerationName = columnDefinition.type.enumerationName;
          break;

        case 'string':
          column.xtype = 'gridcolumn';
          break;

        case 'time':
          column.xtype = 'datecolumn';
          column.format = 'H:i:s';
          break;
      }

      return column;
    },

    createFilter: function(columnDefinition) {
      var typeDefinition = columnDefinition.type;

      if (!columnDefinition.filter.filterable) {
        return false;
      }

      switch (typeDefinition.type) {
        case 'boolean':
          return {
            type: 'boolean'
          };

        case 'date':
          return {
            type: 'date',
            dateFormat: 'd.m.Y'
          };

        case 'dateTime':
          return {
            type: 'datetime',
            date: {
              format: 'd.m.Y'
            },
            time: {
              format: 'H:i:s'
            },
            dock: {
              buttonText: Epsitec.Texts.getOkLabel(),
              dock: 'bottom'
            }
          };

        case 'int':
        case 'float':
          return {
            type: 'numeric'
          };

        case 'list':
          return {
            type: 'list',
            store: Epsitec.Enumeration.getStore(typeDefinition.enumerationName),
            labelField: 'text'
          };

        case 'string':
          return {
            type: 'string'
          };

        case 'time':
          return {
            type: 'time',
            timeFormant: 'H:i:s'
          };

        default:
          return false;
      }
    },

    createSelModel: function(options) {
      var config = {
        selType: 'rowmodel',
        pruneRemoved: false
      };

      if (options.multiSelect) {
        config.mode = 'MULTI';
      }
      else {
        config.mode = 'SINGLE';
        config.allowDeselect = options.allowDeselect;
      }

      return config;
    },

    createStore: function(url, autoLoad, columnDefinitions, sorterDefinitions) {
      return Ext.create('Ext.data.Store', {
        fields: this.createFields(columnDefinitions),
        sorters: this.createSorters(sorterDefinitions),
        autoLoad: autoLoad,
        pageSize: 100,
        buffered: true,
        remoteSort: true,
        remoteFilter: true,
        proxy: {
          type: 'ajax',
          url: url,
          extraParams : {
            query : this.queryName
          },
          reader: {
            type: 'json',
            root: 'content.entities',
            totalProperty: 'content.total'
          },
          encodeSorters: this.encodeSorters,
          listeners: {
            exception: function(proxy, response, operation, eOpts) {
              Epsitec.Tools.processProxyError(response);
            }
          }
        },
        listeners: {
          beforeLoad: this.setupColumnParameter,
          datachanged: this.onDataChange,
          scope: this
        }
      });
    },

    setupColumnParameterAndRefresh: function() {
      this.setupColumnParameter();
      this.resetStore(true);

      // This is a hack around a bug in the filters feature in Ext JS 4.2.1. The
      // feature is supposed to adapt to the columns of the grid panel, in such
      // a way that new columns also have a menu entry to filter it. It was its
      // behavior before in version 4.1.2, but it is not the same anymore. So
      // when we add a new column, we reset the internal state of the filters
      // feature to enable this menu item for the new column. It does not change
      // the filetrs criterions that might exist, it only enables the menu item
      // for the new column.
      if (this.filters) {
        // Note that this is a call to a private method. It might not be
        // supported in future versions of Ext JS.
        this.filters.createFilters();
      }
    },

    setupColumnParameter: function() {
      var key, value;

      key = 'columns';
      value = this.createColumnParameter();

      this.store.proxy.setExtraParam(key, value);
    },

    createColumnParameter: function() {
      return this.columns
          .filter(function(c) {
            return c.xtype !== 'rownumberer' && // remove the row numberer.
                c.dataIndex !== 'summary' &&    // remove the summary column.
                !c.hidden;                      // remove the hidden columns.
          })
          .map(function(c) {
            return c.dataIndex;
          })
          .join(';');
    },

    createFields: function(columnDefinitions) {
      var basicFields = this.createBasicFields(),
          dynamicFields = this.createDynamicFields(columnDefinitions);

      return basicFields.concat(dynamicFields);
    },

    reconfigureFiltersFeature: function() {
      // This method is a hack around a bug in Ext JS 4.2.1. The documentation
      // for the filters feature that we use explicitely states that the feature
      // listens on the reconfigure event of its associate grid panel in order
      // to rebind to the new store if it has changed. This is wrong, the
      // filters feature keeps its binding to the old store. So here we do this
      // work manually.

      if (this.filters && this.filters.store !== this.store) {
        this.filters.bindStore(this.store);
      }
    },

    createBasicFields: function() {
      return [{
        name: 'id',
        type: 'string'
      }, {
        name: 'summary',
        type: 'string'
      }];
    },

    createDynamicFields: function(columnDefinitions) {
      return columnDefinitions.map(function(c) {
        var field = {
          name: c.name,
          type: c.type.type
        };

        switch (c.type.type) {
          case 'int':
          case 'float':
          case 'boolean':
            field.useNull = true;
            break;

          case 'date':
            field.dateFormat = 'd.m.Y';
            break;

          case 'dateTime':
            field.type = 'date';
            field.dateFormat = 'd.m.Y H:i:s';
            break;

          case 'list':
            field.type = 'list'; //list?string
            break;

          case 'time':
            field.type = 'date';
            field.dateFormat = 'H:i:s';
            break;
        }

        return field;
      });
    },

    createSorters: function(sorterDefinitions) {
      return sorterDefinitions.map(function(s) {
        return {
          property: s.name,
          direction: s.sortDirection
        };
      });
    },

    encodeSorters: function(sorters) {
      var sorterStrings = sorters.map(function(s) {
        return s.property + ':' + s.direction;
      });

      return sorterStrings.join(';');
    },

    createToolbar: function(options) {
      return Ext.create('Ext.Toolbar', {
        dock: 'top',
        items: this.createButtons(options)
      });
    },

    createSecondaryToolbar: function() {
        return Ext.create('Ext.Toolbar', {
          dock: 'top',
          items: this.createSecondaryButtons()
        });
    },

    createQueryToolbar: function() {
      if (epsitecConfig.featureQueryBuilder) {
        var buttons = [];
        var druid = Epsitec.Cresus.Core.getApplication().tabManager.currentTab;

        var queryStore = Ext.create('Ext.data.Store', {
            fields: ['Id','Name','RawQuery'],
            proxy: {
                type: 'ajax',
                url: '/proxy/query/' + druid + '/load',
                reader: {
                  type : 'json'
                },
            },
            autoLoad:true
        });

        buttons.push(Ext.create('Ext.form.field.ComboBox', {
          hideLabel: true,
          store: queryStore,
          typeAhead: true,
          valueField: 'Name',
          displayField: 'Name',
          queryMode: 'local',
          triggerAction: 'all',
          emptyText: 'Choix de la présentation',
          selectOnFocus: true,
          width: 250,
          indent: true,
          listeners: {
           scope: this,
           'select': function (c, r) {
              this.queryName =  r[0].data.Name;
              this.store.proxy.setExtraParam("query", this.queryName);
              this.onRefreshHandler();
             }
           }
        }));

        buttons.push(Ext.create('Ext.Button', {
          text: '',
          iconCls: 'icon-filter',
          listeners: {
            click: this.onQueryBuildHandler,
            scope: this
          }
        }));

        return Ext.create('Ext.Toolbar', {
          dock: 'top',
          items: buttons
        });
      }
      else
        return null;
    },

    createButtons: function(options) {
      var buttons, exportMenuItems;

      buttons = Ext.Array.clone(options.toolbarButtons || []);

      buttons.push(Ext.create('Ext.Button', {
        text: Epsitec.Texts.getSortLabel(),
        iconCls: 'icon-sort',
        listeners: {
          click: this.onSortHandler,
          scope: this
        }
      }));

      buttons.push(Ext.create('Ext.Button', {
        text: Epsitec.Texts.getRefreshLabel(),
        iconCls: 'icon-refresh',
        listeners: {
          click: this.onRefreshHandler,
          scope: this
        }
      }));

      exportMenuItems = this.createExportMenuItems(options);
      if (!Ext.isEmpty(exportMenuItems))
      {
        buttons.push('->');
        buttons.push({
          text: Epsitec.Texts.getExportLabel(),
          iconCls: 'icon-export',
          menu: Ext.create('Ext.menu.Menu', {
                    items: exportMenuItems
          }),
          listeners: {
              click: function (comp) {
                  var item = comp.menu.items.items[1];
                  item.setText('Ajouter ' + this.store.getTotalCount() + ' lignes au panier');
              },
              scope: this
          }
        });
      }

      return buttons;
    },

    createExportMenuItems: function(options) {
      var items = [];

      items.push({
        text: Epsitec.Texts.getExportCsvLabel(),
        listeners: {
          click: function() { this.onExportHandler('csv'); },
          scope: this
        }
      });

      items.push({
        text: 'Ajouter au panier',
        listeners: {
        click: function() { this.onExportHandler('bag'); },
          scope: this
        }
      });

      items.push({
        text: 'Ajouter la séléction au panier',
        listeners: {
          click: function() { this.onExportSelectionToBagHandler(); },
          scope: this
        }
      });

      if (!Ext.isEmpty(options.labelExportDefinitions)) {
        items.push({
          text: Epsitec.Texts.getExportLabelLabel(),
          listeners: {
            click: function() { this.onExportHandler('label'); },
            scope: this
          }
        });
      }

      return items;
    },

    createSecondaryButtons: function() {
      var buttons = [];
      if (epsitecConfig.featureQuickSearch) {
        buttons.push({
          xtype: 'textfield',
          width: 250,
          emptyText: Epsitec.Texts.getSearchLabel(),
          name: 'quicksearch',
          listeners: {
            specialkey: this.onQuickSearchHandler,
            scope: this
          }
        });
      }
      if (epsitecConfig.featureSearch) {
        buttons.push(Ext.create('Ext.Button', {
          text: '',
          iconCls: 'icon-search',
          listeners: {
            click: this.onFullSearchHandler,
            scope: this
          }
        }));
      }

      return buttons;
    },

    onDataChange: function(store, e) {
      if (this.isSearching) {
        this.isSearching = false;
        this.fullSearchWindow.appliFilters();

      }
      if(this.isReloading) {
        this.isReloading = false;
      }
    },

    ///QUICK SEARCH
    onQuickSearchHandler: function(field, e) {
      var config, columnName = this.columnDefinitions[0].name;

      config = {
        type: 'string',
        dataIndex: columnName,
        value: field.value,
        active: true
      };
      if (e.getKey() === e.ENTER) {

        this.filters.clearFilters();

        if (this.fullSearchWindow) {
          this.fullSearchWindow.setQuickSearchValue(field.value);
        }
        if (this.filters.filters.items.length === 0) {
          this.filters.addFilter(config);

          this.filters.getFilter(columnName).fireEventArgs(
              'update', this.filters.getFilter(columnName)
          );
        }
        else {
          this.filters.getFilter(columnName).setValue(field.value);
          if (field.value !== '') {
            this.filters.getFilter(columnName).setActive(true);
          }
          else {
            this.filters.clearFilters();
          }
        }
      }
    },

    ///QUERY BUILDER
    onQueryBuildHandler: function(e) {
      if (!this.queryBuilder) {
        exportUrl = this.buildUrlWithSortersAndFilters(this.exportUrl);
        this.queryBuilder = Ext.create('Epsitec.QueryWindow', {
          columnDefinitions: this.columnDefinitions,
          exportUrl: exportUrl
        });

        this.queryBuilder.showAt(e.container.getXY());
      }
      else {
        if (this.queryBuilder.isVisible()) {
          this.queryBuilder.hide();
        }
        else {
          this.queryBuilder.show();
        }
      }
    },

    ///FULL SEARCH
    onFullSearchHandler: function(e) {
      if (!this.fullSearchWindow) {

        this.fullSearchWindow = Ext.create(
            'Epsitec.SearchWindow', this.columnDefinitions, this);
        this.fullSearchWindow.showAt(e.container.getXY());

      }
      else {
        if (this.fullSearchWindow.isVisible()) {
          this.fullSearchWindow.hide();
        }
        else {
          this.fullSearchWindow.show();
        }

      }
      this.fullSearchWindow.setQuickSearchValue(
          this.dockedItems.items[2].items.items[0].lastValue
      );
    },

    ///EXPORT
    onExportHandler: function(type) {
      var count = this.store.getTotalCount();

      if (count === 0) {
        Epsitec.ErrorHandler.showError(
            Epsitec.Texts.getExportImpossibleTitle(),
            Epsitec.Texts.getExportImpossibleEmpty()
        );
      }
      else {
        this.doExport(type);
      }
    },

    onExportToBagHandler: function() {
      var count = this.store.getTotalCount();

      if (count === 0) {
        Epsitec.ErrorHandler.showError(
            Epsitec.Texts.getExportImpossibleTitle(),
            Epsitec.Texts.getExportImpossibleEmpty()
        );
      }
      else {
        this.doExportToBag(count);
      }
    },

    onExportSelectionToBagHandler: function() {
      var count = this.getSelectionModel().getCount();

      if (count === 0) {
        Epsitec.ErrorHandler.showError(
            Epsitec.Texts.getExportImpossibleTitle(),
            Epsitec.Texts.getExportImpossibleEmpty()
        );
      }
      else {
        this.doExportSelectionToBag();
      }
    },

    doExport: function(type) {
      var exportUrl, exportWindow;

      exportUrl = this.buildUrlWithSortersAndFilters(this.exportUrl);

      switch (type) {
        case 'csv':
          exportWindow = Ext.create('Epsitec.ArrayExportWindow', {
            columnDefinitions: this.columnDefinitions,
            exportUrl: exportUrl
          });
          break;

        case 'label':
          exportWindow = Ext.create('Epsitec.LabelExportWindow', {
            labelExportDefinitions: this.labelExportDefinitions,
            exportUrl: exportUrl
          });
          break;
        case 'bag':

          exportUrl = Epsitec.Tools.addParameterToUrl(exportUrl, 'type', 'bag');

          Ext.Ajax.request({
              url: exportUrl,
              success: function (response) {
                  //TODO
              }
          });
          break;
        default:
          throw 'invalid export type: ' + type;
      }

      exportWindow.show();
    },

    doExportToBag: function(count) {
      //Iterate over the buffered store
      var useMenuItemId = Ext.isDefined(this.menuItems[0]);
      var menuItemId = null;
      if(useMenuItemId)
      {
        menuItemId = this.menuItems[0].columnName;
      }
      for (j=1; j<=this.store.data.length; j++) {
        var data = this.store.data.map[j].value;
        for (i=0; i<data.length; i++) {
           if(menuItemId !== null)
           {
              Epsitec.Cresus.Core.app.addCustomEntityToBag('Contact',data[i].data.summary,data[i].raw[menuItemId]);
           }
           else
           {
              Epsitec.Cresus.Core.app.addEntityToBag(data[i].data.summary,data[i].internalId);
           }

        }
      }
    },

    doExportSelectionToBag: function() {
      //Iterate over the buffered store
      var useMenuItemId = Ext.isDefined(this.menuItems[0]);
      var menuItemId = null;
      if(useMenuItemId)
      {
        menuItemId = this.menuItems[0].columnName;
      }

        var data = this.getSelectionModel().getSelection();
        for (i=0; i<data.length; i++) {
           if(menuItemId !== null)
           {
              Epsitec.Cresus.Core.app.addCustomEntityToBag('Contact',data[i].data.summary,data[i].raw[menuItemId]);
           }
           else
           {
              Epsitec.Cresus.Core.app.addEntityToBag(data[i].data.summary,data[i].internalId);
           }

        }
    },

    onRefreshHandler: function() {

      if(Ext.isDefined(this.entityListTypeName))
      {
        Epsitec.Cresus.Core.app.reloadCurrentList(this,false);
      }
      else
      {
        Epsitec.Cresus.Core.app.reloadCurrentDatabase(false);
      }

    },

    resetStore: function(autoLoad) {
      var oldStore, newStore, columnDefinitions, sorterDefinitions;
      oldStore = this.store;

      // Here we can simply reuse the initial column definitions, as we only
      // use their name and type properties. We use all definitions and don't
      // skip the hidden one for instance. If that where the case, we would
      // need to get the column definition out of the old store.
      columnDefinitions = this.columnDefinitions;

      // Here we extract the sorters used in the old store. We can't use the
      // initial sorter definitions because the sorters on the store might have
      // been changed (i.e. by the user) and we don't want to reset them.
      sorterDefinitions = oldStore.getSorters().map(function(s) {
        return {
          name: s.property,
          sortDirection: s.direction
        };
      });

      newStore = this.createStore(
          this.getUrl, autoLoad, columnDefinitions, sorterDefinitions);

      this.reconfigure(newStore, undefined);

      // Here we need to cleanup the old store. If the store is not performing
      // any load operation, we can do it right now. Otherwise, we have to wait
      // until the load is done. If we don't wait, there is a warning in the
      // console, because the load callback is called on a destroyed store,
      // which causes an exception.

      // Note that the method used to destroy the store is private and might
      // therefore not be available in future releases of ExtJs. See
      // http://www.sencha.com/forum/showthread.php?212134 for more details.

      if (!Epsitec.EntityList.isStoreLoading(oldStore)) {
        oldStore.destroyStore();
      }
      else {
        oldStore.on(
            'load',
            function() {
              if (!Epsitec.EntityList.isStoreLoading(oldStore)) {
                this.destroyStore();
              }
            },
            oldStore
        );
      }
    },

    onSortHandler: function() {
      var sortWindow = Ext.create('Epsitec.SortWindow', {
        callback: Epsitec.Callback.create(this.setSorters, this),
        sorters: this.getCurrentSorters(),
        initialSorters: this.getInitialSorters()
      });

      sortWindow.show();
    },

    getCurrentSorters: function() {
      var usedSorters, unusedSorters;

      usedSorters = this.getUsedSorters(this.store.getSorters());
      unusedSorters = this.getUnusedSorters(usedSorters);

      return usedSorters.concat(unusedSorters);
    },

    getInitialSorters: function() {
      var usedSorters, unusedSorters;

      usedSorters = this.getUsedSorters(
          this.createSorters(this.sorterDefinitions)
          );

      unusedSorters = this.getUnusedSorters(usedSorters);

      return usedSorters.concat(unusedSorters);
    },

    getUsedSorters: function(sorters) {
      return sorters.map(
          function(s) {
            return {
              title: this.columnDefinitions.filter(function(c) {
                return s.property === c.name;
              })[0].title,
              name: s.property,
              sortDirection: s.direction
            };
          },
          this
      );
    },

    getUnusedSorters: function(usedSorters) {
      return this.columnDefinitions
          .filter(function(c) {
            return c.sortable === true && !usedSorters.some(function(s) {
              return c.name === s.name;
            });
          })
          .map(function(c) {
            return {
              title: c.title,
              name: c.name,
              sortDirection: null
            };
          });
    },

    setSorters: function(sorters) {
      var newSorters = sorters.map(function(s) {
        return {
          property: s.name,
          direction: s.sortDirection
        };
      });

      // The store.sort(...) method requires at least one sorter to do its job.
      // So if the user has removed all the sort criteria, we must do the job by
      // ourselves.

      if (sorters.length === 0) {
        this.store.sorters.clear();
        this.resetStore(true);
      }
      else {
        this.store.sort(newSorters);
      }
    },

    onSelectionChangeHandler: function(view, selection, options) {
      var entityItems = this.getItems(selection);

      if (this.onSelectionChangeCallback !== null) {
        this.onSelectionChangeCallback.execute([entityItems]);
      }
    },

    getSelectedItems: function() {
      var selection = this.getSelectionModel().getSelection();
      return this.getItems(selection);
    },

    getItems: function(selection) {
      return selection.map(this.getItem);
    },

    getItem: function(row) {
      return {
        id: row.get('id'),
        summary: row.get('summary')
      };
    },

    buildUrlWithSortersAndFilters: function(base) {
      var sorters, filters, parameters, key, value;

      parameters = [];

      sorters = this.store.getSorters();
      if (sorters.length > 0) {
        key = 'sort';
        value = this.store.proxy.encodeSorters(sorters);
        parameters.push([key, value]);
      }

      filters = this.filters.getFilterData();
      if (filters.length > 0) {
        key = 'filter';
        value = this.filters.buildQuery(filters).filter;
        parameters.push([key, value]);
      }

      if (this.queryName !== undefined) {
        parameters.push(["query", this.queryName]);
      }

      return Epsitec.Tools.createUrl(base, parameters);
    },
    /* Static methods */

    statics: {
      isStoreLoading: function(store) {
        if (store.isLoading()) {
          return true;
        }

        // In ExtJs 4.2.1, if we have a store with the autoLoad option set, the
        // first load operation is deferred in the constructor of the store. So
        // if the store has been constructed, but the load operation has not
        // been started yet, the isLoading() method will return false, but there
        // is a load operation that has been scheduled. That's what we check
        // here.
        if (store.autoLoad) {
          // The totalCount property is supposed to hold the number of items in
          // in the store according to the proxy. If this property is undefined,
          // it means that the first request has not yet been finished (thus not
          // yet started either). Note that this is a private property from the
          // store and it might not work in future versions of ExtJs.
          if (store.getTotalCount() === 0 && !Ext.isDefined(store.totalCount)) {
            return true;
          }
        }

        return false;
      }
    }
  });
});
