Ext.require([
  'Epsitec.cresus.webcore.Locale'
],
function() {
  Ext.define('Epsitec.cresus.webcore.Texts', {
    alternateClassName: ['Epsitec.Texts'],

    statics: {

      texts: Epsitec.Locale.getTexts(),

      getGlobalWarning: function() {
        return this.texts.globalWarning;
      },

      getSortLabel: function() {
        return this.texts.sortLabel;
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

        return this.texts.emptyItemText;
      },

      getNullItemText: function() {
        return this.texts.nullItemText;
      },

      getEmptyListText: function() {
        return this.texts.emptyListText;
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

      getEntitySelectionErrorMessage: function() {
        return this.texts.entitySelectionErrorMessage;
      }
    }
  });
});
