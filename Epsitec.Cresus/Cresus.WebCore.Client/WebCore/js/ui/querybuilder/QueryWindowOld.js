
Ext.require([
    'Epsitec.cresus.webcore.ui.querybuilder.QueryElement',
    'Epsitec.cresus.webcore.ui.querybuilder.QueryGroup',
    'Epsitec.cresus.webcore.ui.querybuilder.QueryOp',
    'Epsitec.cresus.webcore.ui.querybuilder.QueryBuilderPanel',
    'Epsitec.cresus.webcore.ui.querybuilder.QueryComposerPanel'
  ],
  function() {
    Ext.define('Epsitec.cresus.webcore.ui.querybuilder.QueryWindow', {
      extend: 'Ext.Window',
      alternateClassName: ['Epsitec.QueryWindow'],

      /* Properties */
      exportUrl: null,
      columnDefinitions: null,
      finalQuery: null,
      composer: null,
      builder: null,
      queryStore: null,
      queryNameField: null,
      dataSetQueries: {},
      currentQuery: {
        name: ''
      },
      comboField: null,

      /* Constructor */

      constructor: function(options) {
        var tabManager;
        tabManager = Epsitec.Cresus.Core.getApplication().tabManager;


        this.queryStore = this.createQueryStore();
        this.builder = Ext.create('Epsitec.QueryBuilderPanel', options.columnDefinitions);
        this.composer = Ext.create('Epsitec.QueryComposerPanel', this.builder);

        newOptions = {
          title: 'Editeur de requêtes',
          width: 800,
          height: 600,
          header: 'false',
          constrain: true,
          renderTo: Ext.get(tabManager.getLayout().getActiveItem().el),
          layout: {
            type: 'border',
            padding: 5
          },
          closable: true,
          closeAction: 'hide',
          dockedItems: [{
            xtype: 'toolbar',
            dock: 'top',
            items: this.createQueryLoadingTools()
          }, {
            xtype: 'toolbar',
            dock: 'left',
            items: this.createQuerySavingTools()
          }],
          items: [this.builder],
          buttons: [
            this.createFilterButton(),
            this.createCancelButton()
          ]
        };


        Ext.applyIf(newOptions, options);
        this.callParent([newOptions]);
        console.log(this.getLocalConfigKey());
        this.dataSetQueries = JSON.parse(
          localStorage
          .getItem(this.getLocalConfigKey()));

        if (this.dataSetQueries !== null) {
          this.loadQueryStore();
        }

        return this;
      },

      getLocalConfigKey: function() {
        return '[query:'+ this.exportUrl.split('?')[0].split('[')[1];
      },

      createQueryStore: function() {
        return Ext.create('Ext.data.ArrayStore', {
          fields: ['name', 'query']
        });
      },

      loadQueryStore: function() {
        //convert hashmap to array
        var output = [],
          item;
        for (var query in this.dataSetQueries) {
          item = {};
          item.name = query;
          item.query = this.dataSetQueries[query].query;
          output.push(item);
        }
        //load available configurations
        this.queryStore.loadData(output);
      },

      loadQuery: function(query) {
        this.currentQuery.name = query.name;
        this.queryNameField.setValue(this.currentQuery.name);

        this.builder.loadElements(query.query);
      },

      saveQuery: function() {
        //create key based on dataset url
        var key = this.getLocalConfigKey();

        //create and push new config
        var queryName = this.queryNameField.getValue();
        if (queryName.length < 1) {
          return;
        }

        if (this.dataSetQueries === null) {
          this.dataSetQueries = {};
        }

        this.dataSetQueries[queryName] = {
          name: queryName,
          query: this.builder.getElements()
        };

        //save in local storage
        localStorage.setItem(key, JSON.stringify(this.dataSetQueries));
        //reload available configuration
        this.loadQueryStore();
        this.comboField.setValue(queryName);
      },

      removeQuery: function() {
        //create key based on dataset url
        var key = this.getLocalConfigKey();

        if (this.dataSetQueries === null) {
          return;
        }

        var queryName = this.queryNameField.getValue();
        delete this.dataSetQueries[queryName];

        this.comboField.setValue(null);
        this.queryNameField.setValue(null);

        //save in local storage
        localStorage.setItem(key, JSON.stringify(this.dataSetQueries));
        //reload available configuration
        this.loadQueryStore();
        this.builder.resetElements();
      },

      createQueryLoadingTools: function() {
        this.comboField = Ext.create('Ext.form.field.ComboBox', {
          hideLabel: true,
          store: this.queryStore,
          displayField: 'name',
          typeAhead: true,
          queryMode: 'local',
          triggerAction: 'all',
          emptyText: 'Charger',
          selectOnFocus: true,
          width: 130,
          indent: true,
          listeners: {
            scope: this,
            'select': function(c, r) {
              this.loadQuery(r[0].data);
            }
          }
        });

        return this.comboField;
      },

      createQuerySavingTools: function() {
        var buttons = [];

        buttons.push({
          xtype: 'label',
          text: 'Nom de la requête :'
        });

        this.queryNameField = Ext.create('Ext.form.field.Text', {
          width: 80,
          emptyText: '',
          name: 'queryName'
        });

        buttons.push(this.queryNameField);

        buttons.push(Ext.create('Ext.Button', {
          text: 'Enregistrer',
          iconCls: 'icon-add',
          listeners: {
            click: this.saveQuery,
            scope: this
          }
        }));

        buttons.push(Ext.create('Ext.Button', {
          text: 'Supprimer',
          iconCls: 'icon-remove',
          listeners: {
            click: this.removeQuery,
            scope: this
          }
        }));

        return buttons;
      },

      createCancelButton: function() {
        return Ext.create('Ext.Button', {
          text: Epsitec.Texts.getCancelLabel(),
          handler: function() {},
          scope: this
        });
      },

      createFilterButton: function() {
        return Ext.create('Ext.Button', {
          text: 'Appliquer le filtre',
          handler: function() {},
          scope: this
        });
      },
    });
  });
