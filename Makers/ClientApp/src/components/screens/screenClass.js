/**
 * @namespace Screen
 **/

export default class Screen {
  /**
   * Create a new Screen
   * @param  {ScreenParams} params
   */
  constructor(params) {
    this.screenId = params.screenId;
    this.contentHeader = params.contentHeader;
    this.hasInitialFilter = params.hasInitialFilter;
    this.actions = params.actions;
    this.dataUrl = params.dataUrl;
    this.dataUrlDashboard = params.dataUrlDashboard;
    this.singleItemUrl = params.singleItemUrl;
    this.addUrl = params.addUrl;
    this.editUrl = params.editUrl;
    this.deleteUrl = params.deleteUrl;
    this.modalWidth = params.modalWidth;
    this.buttonClaimsAdd = params.buttonClaimsAdd;
    this.formInputs = params.formInputs;
    this.tableColumns = params.tableColumns;
    this.rowSubComponentData = params.rowSubComponentData;
    this.build = params.build;
    this.fetchByDate = params.fetchByDate;
    this.dateFilter = params.dateFilter;
    this.headerExtraInfo = params.headerExtraInfo;
    this.autoRefresh = params.autoRefresh;
    this.rowActions = params.rowActions;
    this.dashboard = params.dashboard;
    this.changeStatusUrl = params.changeStatusUrl;
  }
}
