Ext.define('Epsitec.cresus.webcore.SummaryTile', {
  extend: 'Epsitec.cresus.webcore.Tile',
  alternateClassName: ['Epsitec.SummaryTile'],
  alias: 'widget.summarytile',

  /* Config */

  margin: '0 0 5 0',
  bodyCls: 'summary',
  overCls: 'over-summary',

  /* Properties */

  entityId: null,
  isRoot: false,
  subViewMode: '2',
  subViewId: 'null',
  autoCreatorId: null,
  selectedPanelCls: 'selected-entity',

  /* Constructor */

  constructor: function(o)  {
    var options = o || {};
    this.addRefreshButton(options);

    this.callParent([options]);
    return this;
  },

  /* Listeners */

  listeners: {
    render: function() {
      this.body.on('click', this.bodyClicked, this);
    }
  },

  /* Additional methods */

  addRefreshButton: function(options)  {
    options.tools = options.tools || [];

    if (options.isRoot) {
      options.tools.push({
        type: 'refresh',
        tooltip: 'Refresh entity',
        // We don't call the function directly because ExtJs calls the handler
        // with some arguments that are not compatible witht the refreshEntity
        // signature.
        handler: function() { this.refreshEntity(false); },
        scope: this
      });
    }
  },

  // Overriden by EmptySummaryTile
  bodyClicked: function() {
    if (this.autoCreatorId !== null) {
      this.autoCreateNullEntity();
    }
    else {
      this.showEntityColumn(this.subViewMode, this.subViewId, this.entityId);
    }
  },

  showEntityColumn: function(subViewMode, subViewId, entityId, callbackQueue)  {
    this.entityPanel.columnManager.addEntityColumn(
        subViewMode, subViewId, entityId, this, callbackQueue
    );
  },

  showEntityColumnAndRefresh: function(
      subViewMode,
      subViewId,
      entityId,
      callbackQueue) {
    var newCallbackQueue = Epsitec.CallbackQueue.create(
        function() {
          this.refreshEntity(true, callbackQueue);
        },
        this
        );

    this.showEntityColumn(subViewMode, subViewId, entityId, newCallbackQueue);
  },

  refreshEntity: function(refreshAll, callbackQueue) {
    var firstColumnId = refreshAll ? 0 : this.entityPanel.columnId;
    var lastColumnId = this.entityPanel.columnId;

    this.entityPanel.columnManager.refreshColumns(
        firstColumnId, lastColumnId, callbackQueue
    );
  },

  autoCreateNullEntity: function()  {
    this.setLoading();

    Ext.Ajax.request({
      url: 'proxy/entity/autoCreate',
      method: 'POST',
      params: {
        entityId: this.entityPanel.entityId,
        autoCreatorId: this.autoCreatorId
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

        // Here we first add the new column for the entity that we have created
        // with the ajax request. Then we refresh the current pannel where the
        // user has clicked, in case the new entity has some content that we
        // should display. We don't do it in the inverse order, because the
        // refresh replaces the current instance by a new one, and then this
        // messes up the things when we want to add a new column with the
        // current instance which will have been removed from the UI at the time
        // the callback will be called.

        if (this.entityId !== newEntityId) {
          this.showEntityColumnAndRefresh(
              this.subViewMode, this.subViewId, newEntityId
          );
        }
        else {
          this.showEntityColumn(this.subViewMode, this.subViewId, newEntityId);
        }
      },
      failure: function(response, options) {
        this.setLoading(false);
        Epsitec.ErrorHandler.handleError(response);
      },
      scope: this
    }
    );
  },

  select: function() {
    this.addCls(this.selectedPanelCls);
  },

  unselect: function() {
    this.removeCls(this.selectedPanelCls);
  }
});
