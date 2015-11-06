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

    width: 500,
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
    dataSetConfigs: {},
    configStore: null,
    currentConfig: {
      name : '',
      useFilters : false,
      url : null
    },
    comboField : null,
    configNameField : null,
    useFilterField : null,
    columnDefinitions: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions, dataSetConfig;



      this.leftGrid = this.createLeftGrid(options);
      this.rightGrid = this.createRightGrid(options);
      this.configStore = this.createConfigStore();

      newOptions = {
        items: [this.createConfigurationSavingTools(),this.leftGrid, this.rightGrid],
        dockedItems: [{
          xtype: 'toolbar',
          dock: 'top',
          items: this.createConfigurationLoadingTools()
        }],
        buttons: [
          this.createOkButton(),
          this.createCancelButton()
        ]
      };

      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      // we need to convert user's configurations
      this.convertLocalStorageOldKeys ();
      this.dataSetConfigs = JSON.parse(
                                  localStorage
                                  .getItem(this.getLocalConfigKey ()));

      if(this.dataSetConfigs !== null)
      {
        this.loadConfigStore();
      }

      return this;
    },

    /* Methods */
    getLocalConfigKey : function () {
      return this.exportUrl.split ('[')[1].split (']')[0];
    },

    convertLocalStorageOldKeys: function () {
      var addStars = function (n) {
          return new Array(1 + (n || 0)).join('*');
      };

      for (var i=0; i<localStorage.length;i++) {
        var oldKey    = localStorage.key (i);
        // if old key found -> convert it
        if (oldKey.startsWith ('proxy')) {
          var configurations = JSON.parse (localStorage.getItem (oldKey));
          var newKey         = oldKey.split ('[')[1].split (']')[0];
          var existingConfig = localStorage.getItem (newKey);
          if (existingConfig !== null) {
            // we have already converted this key
            // -> merge existingConfig with configurations
            var configToMerge = JSON.parse (existingConfig);
            for (var key in configurations) {
              var counter       = 0;
              if (configurations.hasOwnProperty(key)) {
                 var config = configurations[key];
                 // a config with this name already exist ?
                 if (configToMerge.hasOwnProperty (key)) {
                   // we rename the existing config by adding stars
                   var configToRename = configToMerge[key];
                   counter++;
                   configToRename.name += addStars (counter);
                   configToMerge[configToRename.name] = configToRename;
                 } else {
                   configToMerge[key] = config;
                 }
              }
            }
            // insert mergedConfig at new key
            localStorage.setItem (newKey, JSON.stringify (configToMerge));
          } else {
            localStorage.setItem (newKey, JSON.stringify (configurations));
          }
          // remove old key from localStorage
          localStorage.removeItem (oldKey);
        }
      }
    },

    loadConfig: function (config) {
      this.currentConfig.name = config.name;
      this.currentConfig.useFilters = config.useFilters;
      this.configNameField.setValue (this.currentConfig.name);
      this.useFilterField.setValue (this.currentConfig.useFilters);

      if(this.currentConfig.useFilters) {
        this.exportUrl = config.url;
      }
      this.leftGrid.store.loadData (config.data);
    },

    loadConfigStore: function () {
      //convert hashmap to array
      var output = [], item;
      for (var conf in this.dataSetConfigs) {
          item = {};
          item.name = conf;
          item.data = this.dataSetConfigs[conf].data;
          item.useFilters = this.dataSetConfigs[conf].useFilters;
          item.url = this.dataSetConfigs[conf].url;
          output.push(item);
      }
      //load available configurations
      this.configStore.loadData(output);
    },

    saveConfig : function () {
      //create key based on dataset url
      var key = this.getLocalConfigKey ();

      //create and push new config
      var configName = this.configNameField.getValue();
      if(configName.length < 1) {
        return;
      }

      var useFilters  = this.useFilterField.getValue();

      if(this.dataSetConfigs === null) {
        this.dataSetConfigs = {};
      }

      this.dataSetConfigs[configName] = {
        name    : configName,
        data    : this.getSelectedColumns(),
        useFilters : useFilters,
        url     : this.exportUrl
      };

      //save in local storage
      localStorage.setItem (key, JSON.stringify(this.dataSetConfigs));
      //relaod available configuration
      this.loadConfigStore();
      this.comboField.setValue(configName);
    },

    removeConfig : function () {
      //create key based on dataset url
      var key = this.getLocalConfigKey ();

      if(this.dataSetConfigs === null)
      {
        return;
      }

      var configName = this.configNameField.getValue();
      delete this.dataSetConfigs[configName];

      this.comboField.setValue(null);
      this.configNameField.setValue(null);
      this.useFilterField.setValue(null);

      //save in local storage
      localStorage.setItem (key, JSON.stringify(this.dataSetConfigs));
      //relaod available configuration
      this.loadConfigStore();
    },

    createConfigStore: function() {
      return Ext.create('Ext.data.ArrayStore', {
        fields: ['name','data','useFilters','url']
      });
    },

    createConfigurationLoadingTools : function () {
      this.comboField = Ext.create('Ext.form.field.ComboBox', {
        hideLabel: true,
        store: this.configStore,
        displayField: 'name',
        typeAhead: true,
        queryMode: 'local',
        triggerAction: 'all',
        emptyText: 'Charger',
        selectOnFocus: true,
        width: 130,
        indent: true,
        listeners:{
         scope: this,
         'select': function (c, r) {
             this.loadConfig(r[0].data);
           }
         }
       });

      return this.comboField;
    },

    createConfigurationSavingTools : function () {
      var buttons = [];

      buttons.push({
        xtype: 'label',
        text: 'Nom de la configuration'
      });

      this.configNameField = Ext.create('Ext.form.field.Text',{
        width: 120,
        emptyText: '',
        name: 'configName'
      });

      buttons.push(this.configNameField);

      this.useFilterField = Ext.create('Ext.form.field.Checkbox',{
        name: 'saveFilter',
        boxLabel: 'conserver les filtres'
      });

      buttons.push(this.useFilterField);

      buttons.push(Ext.create('Ext.Button', {
        text: 'Enregistrer',
        iconCls: 'icon-add',
        listeners: {
          click: this.saveConfig,
          scope: this
        }
      }));

      buttons.push(Ext.create('Ext.Button', {
        text: 'Supprimer',
        iconCls: 'icon-remove',
        listeners: {
          click: this.removeConfig,
          scope: this
        }
      }));

      return Ext.create('Ext.container.Container',{
        layout: 'auto',
        margin: '0 5 0 0',
        items: buttons
      });
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
    },

    getSelectedColumns: function() {
      return this.leftGrid.store
          .getRange()
          .map(function(r){
            return r.data;
          });
    }
  });
});
