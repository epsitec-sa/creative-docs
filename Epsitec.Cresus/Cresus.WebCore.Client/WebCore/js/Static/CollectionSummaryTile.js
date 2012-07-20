Ext.define('Epsitec.Cresus.Core.Static.CollectionSummaryTile', {
  extend: 'Epsitec.Cresus.Core.Static.SummaryTile',
  alias: 'widget.collectionsummarytile',

  /* Properties */

  propertyAccessorId: null,
  entityType: null,
  hideRemoveButton: false,
  hideAddButton: false,

  /* Constructor */

  constructor: function(o) {
    var options = o || {};
    this.addPlusMinusButtons(options);

    this.callParent([options]);
    return this;
  },

  /* Additional methods */

  addPlusMinusButtons: function(options) {
    options.tools = options.tools || [];

    if (!options.hideAddButton) {
      options.tools.push({
        type: 'plus',
        tooltip: 'Add a new item',
        handler: this.addEntity,
        scope: this
      });
    }

    if (!options.hideRemoveButton) {
      options.tools.push({
        type: 'minus',
        tooltip: 'Remove this item',
        handler: this.deleteEntity,
        scope: this
      });
    }
  },

  showEntityColumnRefreshAndSelect: function(subViewMode, subViewId, entityId) {
    var callbackQueue = Epsitec.Cresus.Core.Static.CallbackQueue.create(
        function() {
          this.entityPanel.columnManager.selectEntity(
              this.entityPanel.columnId, entityId
          );
        },
        this
        );

    this.showEntityColumnAndRefresh(
        subViewMode, subViewId, entityId, callbackQueue
    );
  },

  removePanel: function() {
    var columnManager = this.entityPanel.columnManager;

    // If this panel is currently selected, we must remove all the columns to
    // the right of this one.
    var columnId = this.entityPanel.columnId;
    var selectedEntityId = columnManager.getSelectedEntity(columnId);

    if (selectedEntityId === this.entityId) {
      columnManager.removeColumnsFromIndex(columnId + 1);
    }

    // Now we refresh the current column in order to update the UI with any
    // modification that the deletion might have done to summaries.
    this.refreshEntity(true);
  },

  addEntity: function() {
    this.setLoading();

    Ext.Ajax.request({
      url: 'proxy/collection/create',
      method: 'POST',
      params: {
        parentEntityId: this.entityPanel.entityId,
        entityType: this.entityType,
        propertyAccessorId: this.propertyAccessorId
      },
      success: function(response, options) {
        this.setLoading(false);

        var json;

        try {
          json = Ext.decode(response.responseText);
        }
        catch (err) {
          options.failure.apply(arguments);
          return;
        }

        var newEntityId = json.content;

        this.showEntityColumnRefreshAndSelect(
            this.subViewMode, this.subViewId, newEntityId
        );
      },
      failure: function(response, options) {
        this.setLoading(false);
        Epsitec.Cresus.Core.Static.ErrorHandler.handleError(response);
      },
      scope: this
    });
  },

  deleteEntity: function() {
    this.setLoading();

    Ext.Ajax.request({
      url: 'proxy/collection/delete',
      method: 'POST',
      params: {
        parentEntityId: this.entityPanel.entityId,
        deletedEntityId: this.entityId,
        propertyAccessorId: this.propertyAccessorId
      },
      success: function(response, options) {
        this.setLoading(false);

        this.removePanel();
      },
      failure: function(response, options) {
        this.setLoading(false);
        Epsitec.Cresus.Core.Static.ErrorHandler.handleError(response);
      },
      scope: this
    });
  }
});
