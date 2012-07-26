Ext.define('Epsitec.cresus.webcore.CollectionSummaryTile', {
  extend: 'Epsitec.cresus.webcore.SummaryTile',
  alternateClassName: ['Epsitec.CollectionSummaryTile'],
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

  addEntity: function() {
    this.setLoading();

    Ext.Ajax.request({
      url: 'proxy/collection/create',
      method: 'POST',
      params: {
        parentEntityId: this.column.entityId,
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
          options.failure.apply(this, arguments);
          return;
        }

        var newEntityId = json.content;
        this.column.addEntityColumn(
            this.subViewMode, this.subViewId, newEntityId, true
        );
      },
      failure: function(response, options) {
        this.setLoading(false);
        Epsitec.ErrorHandler.handleError(response);
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
        parentEntityId: this.column.entityId,
        deletedEntityId: this.entityId,
        propertyAccessorId: this.propertyAccessorId
      },
      success: function(response, options) {
        this.setLoading(false);
        if (this.isSelected()) {
          this.column.removeToRight();
        }
        this.column.refreshToLeft(true);
      },
      failure: function(response, options) {
        this.setLoading(false);
        Epsitec.ErrorHandler.handleError(response);
      },
      scope: this
    });
  }
});
