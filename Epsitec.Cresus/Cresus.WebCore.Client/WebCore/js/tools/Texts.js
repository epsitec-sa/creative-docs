// This class provides methods to get all texts that are displayed to the user
// and that don't come from the server. They are localized according to the
// files in the 'locale' folder.

Ext.require([
  'Epsitec.cresus.webcore.locale.Locale'
],
function() {
  Ext.define('Epsitec.cresus.webcore.tools.Texts', {
    alternateClassName: ['Epsitec.Texts'],

    /* Static methods */

    statics: {

      texts: Epsitec.Locale.getTexts(),

      getGlobalWarning: function() {
        return this.texts.globalWarning;
      },

      getSortLabel: function() {
        return this.texts.sortLabel;
      },

      getExportLabel: function() {
        return this.texts.exportLabel;
      },

      getExportCsvLabel: function() {
        return this.texts.exportCsvLabel;
      },

      getExportLabelLabel: function() {
        return this.texts.exportLabelLabel;
      },

      getExportLabelText: function() {
        return this.texts.exportLabelText;
      },

      getExportLabelLayout: function() {
        return this.texts.exportLabelLayout;
      },

      getSearchLabel: function() {
        return this.texts.searchLabel;
      },

      getRefreshLabel: function() {
        return this.texts.refreshLabel;
      },

      getRefreshTip: function() {
        return this.texts.refreshTip;
      },

      getAddLabel: function() {
        return this.texts.addLabel;
      },

      getAddTip: function() {
        return this.texts.addTip;
      },

      getRemoveLabel: function() {
        return this.texts.removeLabel;
      },

      getRemoveTip: function() {
        return this.texts.removeTip;
      },

      getCreateLabel: function() {
        return this.texts.createLabel;
      },

      getDeleteLabel: function() {
        return this.texts.deleteLabel;
      },

      getResetLabel: function() {
        return this.texts.resetLabel;
      },

      getSaveLabel: function() {
        return this.texts.saveLabel;
      },

      getOkLabel: function() {
        return this.texts.okLabel;
      },

      getCancelLabel: function() {
        return this.texts.cancelLabel;
      },

      getEmptySummaryText: function() {
        return this.texts.emptySummaryText;
      },

      getEmptyItemText: function() {

        return '<i>' + this.texts.emptyItemText + '</i>';
      },

      getNullItemText: function() {
        return this.texts.nullItemText;
      },

      getEmptyListText: function() {
        return '<i>' + this.texts.emptyListText + '</i>';
      },

      getLoadingText: function() {
        return this.texts.loadingText;
      },

      getSummaryHeader: function() {
        return this.texts.summaryHeader;
      },

      getEntityPickerTitle: function() {
        return this.texts.entityPickerTitle;
      },

      getErrorTitle: function() {
        return this.texts.errorTitle;
      },

      getErrorMessage: function() {
        return this.texts.errorMessage;
      },

      getServerErrorMessage: function() {
        return this.texts.serverErrorMessage;
      },

      getJsonErrorMessage: function() {
        return this.texts.jsonErrorMessage;
      },

      getErrorBusinessTitle: function() {
        return this.texts.errorBusinessTitle;
      },

      getLoginTitle: function() {
        return this.texts.loginTitle;
      },

      getLoginLabel: function() {
        return this.texts.loginLabel;
      },

      getUsernameLabel: function() {
        return this.texts.usernameLabel;
      },

      getPasswordLabel: function() {
        return this.texts.passwordLabel;
      },

      getDatabasesTitle: function() {
        return this.texts.databasesTitle;
      },

      getToolsTitle: function() {
        return this.texts.toolsTitle;
      },

      getAboutLabel: function() {
        return this.texts.aboutLabel;
      },

      getLogoutLabel: function() {
        return this.texts.logoutLabel;
      },

      getSortTitle: function() {
        return this.texts.sortTitle;
      },

      getScopeTitle: function() {
        return this.texts.scopeTitle;
      },

      getWindowTitle: function() {
        return this.texts.windowTitle;
      },

      getWarningTitle: function() {
        return this.texts.warningTitle;
      },

      getEntityCreationWarningMessage: function() {
        return this.texts.entityCreationWarningMessage;
      },

      getEntityRemoveWarningMessage: function() {
        return this.texts.entityRemoveWarningMessage;
      },

      getEntitySelectionErrorMessage: function() {
        return this.texts.entitySelectionErrorMessage;
      },

      getPickerFavouriteItems: function() {
        return this.texts.pickerFavouriteItems;
      },

      getPickerAllItems: function() {
        return this.texts.pickerAllItems;
      },

      getExportTitle: function() {
        return this.texts.exportTitle;
      },

      getExportExportedColumns: function() {
        return this.texts.exportExportedColumns;
      },

      getExportDiscardedColumns: function() {
        return this.texts.exportDiscardedColumns;
      },

      getExportImpossibleTitle: function() {
        return this.texts.exportImpossibleTitle;
      },

      getExportImpossibleEmpty: function() {
        return this.texts.exportImpossibleEmpty;
      },

      getExportImpossibleTooMany: function() {
        return this.texts.exportImpossibleTooMany;
      }
    }
  });
});
