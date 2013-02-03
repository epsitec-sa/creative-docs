Ext.require([
  'Epsitec.cresus.webcore.tools.CallbackQueue',
  'Epsitec.cresus.webcore.tools.ViewMode'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityUi.EntityColumn', {
    extend: 'Ext.Panel',
    alternateClassName: ['Epsitec.EntityColumn'],

    /* Config */

    margin: '0 0 0 1',

    /* Properties */

    columnId: null,
    columnManager: null,
    entityId: null,
    viewMode: Epsitec.ViewMode.summary,
    viewId: 'null',

    /* Additional methods */

    // To be overriden in child classes.
    getState: function() {
      return { };
    },


    // To be overriden in child classes.
    setState: function(state) {
    },

    refresh: function() {
      this.columnManager.refreshColumn(this);
    },

    refreshToLeft: function(includeCurrent) {
      this.columnManager.refreshColumnsToLeft(this, includeCurrent);
    },

    refreshAll: function() {
      this.columnManager.refreshAllColumns();
    },

    addEntityColumn: function(viewMode, viewId, entityId, refreshToLeft) {
      var callbackQueue = null;
      if (refreshToLeft) {
        callbackQueue = Epsitec.CallbackQueue.create(
            function() { this.refreshToLeft(true); },
            this
            );
      }
      this.columnManager.addEntityColumn(
          viewMode, viewId, entityId, this, callbackQueue
      );
    },

    removeToRight: function() {
      this.columnManager.removeRightColumns(this);
    },

    showAction: function(viewId, entityId, callback) {
      this.columnManager.showAction(
          Epsitec.ViewMode.action, viewId, entityId, null, callback
      );
    },

    showTemplateAction: function(viewId, entityId, callback) {
      this.columnManager.showAction(
          Epsitec.ViewMode.action, viewId, this.entityId, entityId, callback
      );
    }
  });
});
