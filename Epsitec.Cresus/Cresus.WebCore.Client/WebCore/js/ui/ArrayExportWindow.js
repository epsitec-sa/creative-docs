// This class is a window that lets the used configure an exportation of
// entities to an array file (.csv or .pdf for instance). Mostly, it is used to
// select the columns that will be exported to the file. But in the future, it
// might include other options as well.

Ext.require([
  'Epsitec.cresus.webcore.tools.Texts',
  'Epsitec.cresus.webcore.tools.Tools',
  'Epsitec.cresus.webcore.ui.ArrayExportItem'
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.ArrayExportWindow', {
    extend: 'Ext.window.Window',
    alternateClassName: ['Epsitec.ArrayExportWindow'],

    /* Configuration */

    width: 400,
    height: 300,
    border: false,
    layout: {
      type: 'hbox',
      align: 'stretch'
    },
    modal: true,
    plain: true,
    title: Epsitec.Texts.getExportTitle(),

    /* Properties */

    leftGrid: null,
    rightGrid: null,
    exportUrl: null,
    dataSetConfigs: [],
    configStore: null,
    columnDefinitions: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions, dataSetConfig;

      this.leftGrid = this.createLeftGrid(options);
      this.rightGrid = this.createRightGrid(options);
      this.configStore = this.createConfigStore();


      newOptions = {
        items: [this.leftGrid, this.rightGrid],
        dockedItems: [{
          xtype: 'toolbar',
          dock: 'top',
          items: [this.createConfigCombo()]
        }],
        buttons: [
          this.createOkButton(),
          this.createCancelButton()
        ]
      };

      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);

      this.dataSetConfigs = JSON.parse(
                                  localStorage
                                  .getItem(this.getLocalConfigKey ()));

      console.log(this.dataSetConfigs);
      if(this.dataSetConfigs !== null)
      {
        this.configStore.data = this.dataSetConfigs;
      }

      return this;
    },

    /* Methods */
    getLocalConfigKey : function ()
    {
      return this.exportUrl.split('?')[0];
    },

    createConfigStore: function() {
      return Ext.create('Ext.data.ArrayStore', {
        fields: ['name'],
        data : this.dataSetConfigs
      });
    },

    createConfigCombo : function () {
      return Ext.create('Ext.form.field.ComboBox', {
        hideLabel: true,
        store: this.configStore,
        displayField: 'name',
        typeAhead: true,
        queryMode: 'local',
        triggerAction: 'all',
        emptyText: 'Exports précédents...',
        selectOnFocus: true,
        width: 135,
        indent: true,
        on: {
          select: function (c, r) {
              this.loadConfig(r[0]);
            }
        }
      });
    },

    loadConfig: function (config) {
      this.leftGrid.store.loadData(config.data);
    },

    saveConfig : function () {
      //create key based on dataset url
      var key = this.getLocalConfigKey ();

      //init configs for this key if empty
      if(this.dataSetConfigs===null)
      {
        this.dataSetConfigs = [];
      }

      //create and push new config
      var config = {
        name : new Date().toDateString(),
        data : this.leftGrid.store.snapshot,
        url : this.exportUrl
      };
      
      this.dataSetConfigs.push (config);

      //save in local storage
      localStorage.setItem (key, JSON.stringify(this.dataSetConfigs));
    },

    createLeftGrid: function(options) {
      var title = Epsitec.Texts.getExportExportedColumns();
      return this.createGrid(false, title, {
        margin: '0 5 0 0',
        store: this.createStore(options.columnDefinitions, false)
      });
    },

    createRightGrid: function(options) {
      var title = Epsitec.Texts.getExportDiscardedColumns();
      return this.createGrid(true, title, {
        store: this.createStore(options.columnDefinitions, true),
        listeners: {
          viewready: this.sortRightGrid,
          scope: this
        }
      });
    },

    createGrid: function(sortOnDrop, columnTitle, options) {
      var newOptions = {
        flex: 1,
        sortableColumns: false,
        viewConfig: {
          plugins: {
            ptype: 'gridviewdragdrop',
            ddGroup: 'sortPanelDDGroup'
          }
        },
        columns: [{
          text: columnTitle,
          dataIndex: 'title',
          flex: 1
        }],
        selModel: {
          selType: 'rowmodel',
          allowDeselect: true,
          mode: 'MULTI'
        }
      };

      if (sortOnDrop) {
        newOptions.viewConfig.listeners = {
          drop: this.sortRightGrid,
          scope: this
        };
      }

      Ext.applyIf(newOptions, options);

      return Ext.create('Ext.grid.Panel', newOptions);
    },

    createStore: function(columnDefinitions, hidden) {
      var data = columnDefinitions
          .filter(function(c) {
            return c.hidden === hidden;
          });

      return Ext.create('Ext.data.JsonStore', {
        model: 'Epsitec.cresus.webcore.ui.ArrayExportItem',
        proxy: {
          type: 'memory',
          reader: {
            type: 'json'
          }
        },
        data: data
      });
    },

    sortRightGrid: function() {
      this.rightGrid.store.sort({
        property: 'title',
        direction: 'ASC'
      });
    },

    createCancelButton: function() {
      return Ext.create('Ext.Button', {
        text: Epsitec.Texts.getCancelLabel(),
        handler: this.onCancelClick,
        scope: this
      });
    },

    createOkButton: function() {
      return Ext.create('Ext.Button', {
        text: Epsitec.Texts.getOkLabel(),
        handler: this.onOkClick,
        scope: this
      });
    },

    onCancelClick: function() {
      this.close();
    },

    onOkClick: function() {
      var url, columnIds;
      var scope = this;
      columnIds = this.getSelectedColumnIds().join(';');

      url = this.exportUrl;
      url = Epsitec.Tools.addParameterToUrl(url, 'type', 'array');
      url = Epsitec.Tools.addParameterToUrl(url, 'columns', columnIds);

      var ajaxTime= new Date().getTime();
      Ext.Ajax.request({
          url: url,
          success: function (response) {

          }
      });

      this.close();
    },

    getSelectedColumnIds: function() {
      return this.leftGrid.store
          .getRange()
          .map(function(i) {
            return i.get('name');
          });
    }
  });
});
