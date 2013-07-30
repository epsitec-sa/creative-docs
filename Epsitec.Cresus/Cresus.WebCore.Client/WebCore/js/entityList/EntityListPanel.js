// This class is a wrapper around an entity list. The idea with this class is
// that is it immediately constructed so it can be placed within the UI at the
// appropriate location, but the list is not yet loaded because the definition
// has to be obtained with an AJAX request to the server. Once the definition is
// loaded, the list can be instantiated and placed within the wrapper.

Ext.require([
  'Epsitec.cresus.webcore.entityList.DatabaseEntityList',
  'Epsitec.cresus.webcore.entityList.DatabaseEditableEntityList',
  'Epsitec.cresus.webcore.entityList.FavoritesEntityList',
  'Epsitec.cresus.webcore.entityList.SetEntityList',
  'Epsitec.cresus.webcore.entityList.SetEditableEntityList',
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityList.EntityListPanel', {
    extend: 'Ext.panel.Panel',
    alternateClassName: ['Epsitec.EntityListPanel'],

    /* Configuration */

    layout: 'fit',

    /* Properties */

    entityList: null,
    listCreationCallback: null,
    allowEntitySelection: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        allowEntitySelection: this.isEntitySelectionAllowed(options)
      };
      Ext.applyIf(newOptions, options.container);

      this.callParent([newOptions]);
      this.setupEntityList(options.list);
      return this;
    },

    /* Methods */

    isEntitySelectionAllowed: function(options) {
      var entityListTypeName = options.list.entityListTypeName;
      return entityListTypeName === 'Epsitec.DatabaseEditableEntityList';
    },

    setupEntityList: function(options) {
      if (this.isAsynchronousSetupRequired(options)) {
        this.setupEntityListAsynchronously(options);
      }
      else {
        this.setupEntityListSynchronously(options);
      }
    },

    isAsynchronousSetupRequired: function(options) {
      var asynchronous = [
        'Epsitec.DatabaseEntityList',
        'Epsitec.DatabaseEditableEntityList',
        'Epsitec.FavoritesEntityList'
      ];

      return Ext.Array.contains(asynchronous, options.entityListTypeName);
    },

    setupEntityListAsynchronously: function(options) {
      this.setLoading(true);
      Ext.Ajax.request({
        url: 'proxy/database/definition/' + options.databaseName,
        callback: function(requestOptions, success, response) {
          this.setupEntityListCallback(options, success, response);
        },
        scope: this
      });
    },

    setupEntityListCallback: function(options, success, response) {
      var json;

      this.setLoading(false);

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      this.createEntityList(options.entityListTypeName, {
        databaseName: options.databaseName,
        favoritesId: options.favoritesId,
        favoritesOnly: options.favoritesOnly,
        columnDefinitions: json.content.columns,
        sorterDefinitions: json.content.sorters,
        labelExportDefinitions: json.content.labelItems,
        enableCreate: json.content.enableCreate,
        enableDelete: json.content.enableDelete,
        creationViewId: json.content.creationViewId,
        deletionViewId: json.content.deletionViewId,
        entityTypeId: json.content.entityTypeId,
        multiSelect: options.multiSelect,
        menuItems: json.content.menuItems,
        onSelectionChange: options.onSelectionChange
      });
    },

    setupEntityListSynchronously: function(options) {
      this.createEntityList(options.entityListTypeName, options);
    },

    createEntityList: function(typeName, options) {
      this.entityList = Ext.create(typeName, options);
      this.add(this.entityList);

      if (this.listCreationCallback !== null) {
        this.listCreationCallback.apply(this, []);
        this.listCreationCallback = null;
      }
    },

    getEntityList: function() {
      return this.entityList;
    },

    selectEntity: function(entityId, suppressEvent) {
      if (!this.allowEntitySelection) {
        throw 'Entity selection is not supported by this list panel.';
      }

      // If the entity list is already created, we simply select the entity and
      // otherwise, we create a callback that will be called later on when the
      // list will be created.
      if (this.entityList !== null) {
        this.entityList.selectEntity(entityId, suppressEvent);
      }
      else {
        this.listCreationCallback = function() {
          this.entityList.selectEntity(entityId, suppressEvent);
        };
      }
    }
  });
});
