/* eslint-disable react-hooks/exhaustive-deps */
import React, { useEffect, useRef, useState } from "react";
import Jwt from "../../utils/jwt";
import * as XLSX from "xlsx";
import store from "../../store";
import PopUp from "./popUp";
import Button from "./button";
import Backend from "../../utils/backend";
import OmsLogo from "../../images/logo-Makers2-login.png";
import TableHelpers from "../../utils/tableHelpers";
import { Chart } from "primereact/chart";
import ChartDataLabels from "chartjs-plugin-datalabels";
import { useNavigate } from "react-router-dom";
import { ContextMenu } from "primereact/contextmenu";
import { confirmDialog } from "primereact/confirmdialog";
import { useReactToPrint } from "react-to-print";
import { customFilter, getRandomColor } from "../../utils/stringHelper";
import { useDispatch, useSelector } from "react-redux";
import { useTable, useFilters, useExpanded, usePagination, useFlexLayout, useResizeColumns, useGlobalFilter } from "react-table";
import {
  faArrowsRotate,
  faChartSimple,
  faEraser,
  faFileCsv,
  faFileExcel,
  faTable,
  faTentArrowTurnLeft,
  faMagnifyingGlass,
  faDownload,
  faArrowsLeftRight,
  faArrowsTurnRight,
  faArrowRight,
  faArrowLeft,
  faAngleDoubleRight,
  faAngleDoubleLeft,
  faAngleLeft,
  faAngleRight,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { InputText } from "primereact/inputtext";

function Table({
  columns,
  data,
  rowSubComponentData,
  pageSize = 20,
  pageSizeOptions = [10, 20, 40],
  filterMethod = customFilter,
  getData,
  getSingleItem,
  deleteUrl,
  changeStatusUrl,
  dataUrl,
  screenId,
  changeModal,
  screenContentHeader,
  dateFilter,
  fetchByDate,
  setProgressBar,
  setImageViewer,
  rowActions,
  refreshData,
  search,
}) {
  const isLoading = useSelector((state) => state.dashboard.isLoading);
  const tableFilters = useSelector((state) => state.tableFilters.data);
  const [contextMenuItems, setContextMenuItems] = useState([]);

  const scrollRef = useRef(null);
  const ctxMenuRef = useRef(null);
  const visualizationChartRef = useRef();

  const dispatch = useDispatch();
  const navigate = useNavigate();

  const filterTypes = React.useMemo(
    () => ({
      text: filterMethod,
    }),
    [filterMethod]
  );

  const defaultColumn = {
    Filter: (column) => DefaultColumnFilter(column, screenId, gotoPage),
  };

  const prepareDataToExport = () => {
    const dataColumns = columns.filter((c) => c.Header);
    const dataColumnsAccessors = dataColumns.map((c) => c.accessor);

    const dataToExport = data.map((obj) => {
      var newObj = Object.assign({}, obj);

      Object.keys(newObj)
        .sort((a, b) => dataColumnsAccessors.indexOf(a) - dataColumnsAccessors.indexOf(b))
        .forEach((key) => {
          var col = dataColumns.find((h) => h.accessor === key);
          if (col) {
            newObj[col.Header] = newObj[key];
            var tempVal = col.Header === key ? newObj[key] : null;
          }
          delete newObj[key];
          if (col && col.Header === key) newObj[col.Header] = tempVal;
        });
      return newObj;
    });

    return dataToExport;
  };

  const printVisualizedChart = useReactToPrint({
    content: () => visualizationChartRef.current,
    documentTitle: screenContentHeader,
  });

  const {
    getTableProps,
    getTableBodyProps,
    headerGroups,
    prepareRow,
    state,
    visibleColumns,
    rows,

    page,
    pageCount,
    canPreviousPage,
    canNextPage,
    pageOptions,
    gotoPage,
    nextPage,
    previousPage,
    setPageSize,
    setGlobalFilter,
    setAllFilters,
  } = useTable(
    {
      columns,
      data,
      defaultColumn,
      filterTypes,
      initialState: {
        pageSize,
        pageSizeOptions,
        filters: tableFilters,
      },
      autoResetFilters: false,
      autoResetPage: false,
    },
    useFilters,
    useGlobalFilter,
    useExpanded,
    usePagination,
    useFlexLayout,
    useResizeColumns
  );

  const showContextMenu = (row, e) => {
    let ctxMenuItems = [
      {
        template: (
          <div className="ctx-menu-header">
            <h5 style={{ color: "white" }}>Makers</h5>
            {/* <img src={OmsLogo} alt="Logo" /> */}
          </div>
        ),
      },
    ];


    if (rowActions) {
      rowActions(row).forEach((action) => {
        if (action === "-") {
          ctxMenuItems.push({ separator: true });
          return;
        }

        if (action.title === "Deactive" || action.title === "Active") ctxMenuItems.push({ separator: true });

        if (Jwt.isAuthorized(action.claims) && (action.isHidden instanceof Function ? !action.isHidden() : !action.isHidden)) {
          ctxMenuItems.push({
            template: () => {
              return (
                <div
                  className="ctx-menu-item"
                  onClick={async () => {
                    ctxMenuRef.current.hide();
                    if (action.title === "Edit") {
                      await getSingleItem(row.original.ID);
                    } else if (action.title === "Deactive" || action.title === "Active") {
                      confirmDialog({
                        header: "Change Status for Item",
                        dismissableMask: true,
                        message: "Are you sure you want to change status for this item?",
                        accept: async () => {
                          console.log(changeStatusUrl);
                          const response = await Backend.call(changeStatusUrl, { Id: row.original.ID });
                          if (response.StatusCode === 0) {
                            var screenQueryData = {};
                            var screenQuery = store.getState().screenQuery;

                            if (screenId === screenQuery.data.screenId) {
                              screenQueryData = screenQuery.data.dataParams;
                            }
                            await getData(dataUrl, { Date: dateFilter, ...screenQueryData });
                          }
                        },
                      });
                    } else if (action.title === "Delete") {
                      confirmDialog({
                        header: "Delete Item",
                        dismissableMask: true,
                        message: "Are you sure you want to delete this item?",
                        accept: async () => {
                          const response = await Backend.call(deleteUrl, { Id: row.original.ID });
                          if (response.StatusCode === 0) {
                            var screenQueryData = {};
                            var screenQuery = store.getState().screenQuery;
                            if (screenId === screenQuery.data.screenId) {
                              screenQueryData = screenQuery.data.dataParams;
                            }
                            await getData(dataUrl, { Date: dateFilter, ...screenQueryData });
                          }
                        },
                      });
                    } else if (action.title === "Clone") {
                      await getSingleItem(row.original.ID, true);
                    } else {
                      await action.handler({ getData, navigate, changeModal, dateFilter, setProgressBar, setImageViewer });
                    }
                  }}
                >
                  {action.title}
                </div>
              );
            },
          });
        }
      });

      for (let index = 1; index < ctxMenuItems.length; index++) {
        if (ctxMenuItems[index].separator && (index === 1 || index === contextMenuItems.length - 1 || ctxMenuItems[index - 1].separator === true)) {
          ctxMenuItems.splice(index, 1);
          index--;
        }
      }
    }

    if (ctxMenuItems.filter((item) => !item.separator).length === 1) {
      ctxMenuItems.push({ template: <div className="ctx-menu-no-actions">No actions available</div> });
    }

    setContextMenuItems(ctxMenuItems);

    ctxMenuRef.current.show(e);
  };

  useEffect(() => {
    if (!screenId) return;
    const currentScreenFilters = tableFilters.filter((filter) => filter.screenId === screenId);
    if (currentScreenFilters.length > 0 && columns.length > 0) {
      setAllFilters(currentScreenFilters);
    } else {
      setAllFilters([]);
    }
  }, [setAllFilters, tableFilters, screenId, columns]);

  useEffect(() => {
    setGlobalFilter(search);
  }, [search]);
  return (
    <div className="mainWrapper p-1">
      <ContextMenu model={contextMenuItems} ref={ctxMenuRef} breakpoint="767px" disabled={isLoading} />
      <div className="tableWrapper" ref={scrollRef}>
        <div {...getTableProps()} className="rtable">
          <div className="rtable__thead">
            {headerGroups.map((headerGroup, index) => (
              <div {...headerGroup.getHeaderGroupProps()} key={index} className="rtable__rhead">
                {headerGroup.headers.map((column, index) => (
                  <div
                    key={index}
                    {...column.getHeaderProps({
                      style: {
                        minWidth: column.resizeable ? column.width : column.minWidth,
                        maxWidth: column.maxWidth,
                        width: column.width,
                      },
                    })}
                    className="rtable__th"
                  >
                    {column.render("Header")}
                    {/* {column.canFilter ? column.render("Filter") : null} */}
                    {column.resizeable && <div {...column.getResizerProps()} className={"rtable__resizer"}></div>}
                  </div>
                ))}
              </div>
            ))}
          </div>
          <div {...getTableBodyProps()} className="rtable__tbody">
            {page.map((row, i) => {
              prepareRow(row);
              const isEven = (i + 1) % 2;
              return (
                <React.Fragment key={i}>
                  <div
                    className={`rtable__tr ${isEven ? "table__tr--stripe" : ""}`}
                    style={{
                      cursor: rowSubComponentData ? "pointer" : "default",
                    }}
                    onClick={() => row.toggleRowExpanded(!row.isExpanded)}
                    onContextMenu={(e) => {
                      showContextMenu(row, e);
                    }}
                  >
                    {row.cells.map((cell) => {
                      return (
                        <React.Fragment key={cell.column.id}>
                          <div
                            {...row.getRowProps()}
                            {...cell.getCellProps([
                              {
                                style: {
                                  minWidth: cell.column.resizeable ? cell.column.width : cell.column.minWidth,
                                  maxWidth: cell.column.maxWidth,
                                  width: cell.column.width,
                                  wordBreak: cell.column.resizeable ? "break-all" : "",
                                },
                              },
                              cell.column.getTdProps ? cell.column.getTdProps({ cell }) : {},
                            ])}
                            className="rtable__td"
                          >
                            {cell.render("Cell")}
                          </div>
                        </React.Fragment>
                      );
                    })}
                  </div>
                  {/* {row.isExpanded && (
                    <div className="rtable__tr">
                      <div colSpan={visibleColumns.length} className="rtable__subtd">
                        {rowSubComponentData && (
                          <div className="subrow">
                            {rowSubComponentData({ row }).map((info, index) =>
                              info.type === "section" ? (
                                <h5 className="subrow__section__title" key={index}>
                                  {info.title}
                                </h5>
                              ) : (
                                <div className="subrow__item" key={index}>
                                  <span className="subrow__title">{info.title}: </span>
                                  {info.content}
                                </div>
                              )
                            )}
                          </div>
                        )}
                      </div>
                    </div>
                  )} */}
                </React.Fragment>
              );
            })}
          </div>
        </div>
      </div>
      <div className="flex justify-center items-center flex-col md:flex-row space-x-2">
        <div>
          <Button
            label={<FontAwesomeIcon icon={faAngleDoubleLeft} />}
            className="rtable__page-button"
            onClick={() => gotoPage(0)}
            disabled={!canPreviousPage}
          />{" "}
          <Button
            className="rtable__page-button"
            label={<FontAwesomeIcon icon={faAngleLeft} />}
            onClick={() => previousPage()}
            disabled={state.pageIndex + 1 === 1}
          />
        </div>
        <div className="flex justify-center items-center flex-col md:flex-row">
          <div>
            Page{" "}
            <select
              className="rtable__select"
              value={state.pageIndex + 1}
              onChange={(e) => {
                const page = e.target.value ? Number(e.target.value) - 1 : 0;
                gotoPage(page);
              }}
            >
              {Array(pageCount)
                .fill(0)
                .map((_, i) => (
                  <option key={i}>{i + 1}</option>
                ))}
            </select>{" "}
            of <strong> {pageOptions.length}</strong>{" "}
          </div>
          <div>
            | Rows:{" "}
            <select
              className="rtable__select"
              value={state.pageSize}
              onChange={(e) => {
                setPageSize(Number(e.target.value));
              }}
            >
              {pageSizeOptions.map((pageSize) => (
                <option key={pageSize} value={pageSize}>
                  {pageSize}
                </option>
              ))}
            </select>
          </div>

          <div className="rtable__actions">
            {/* <Button
              icon={faChartSimple}
              className="rtable__action"
              title="Visualize Data"
              onClick={() => {
                const colsDropdown = columns
                  .filter((c) => c.Header !== "")
                  .map((c) => {
                    return { label: c.Header, value: c.Header, accessor: c.accessor };
                  });
                changeModal({
                  title: "Visualize Data",
                  buttonText: "Visualize",
                  width: "25vw",
                  inputs: [
                    {
                      type: "dropdown",
                      id: "action",
                      label: "Action",
                      optionsGetter: () => [
                        { label: "Count", value: "Count" },
                        { label: "Sum", value: "Sum" },
                        { label: "Average", value: "Average" },
                        { label: "Min", value: "Min" },
                        { label: "Max", value: "Max" },
                      ],
                    },
                    {
                      type: "multiselect",
                      id: "columns",
                      label: "Columns",
                      optionsGetter: () => colsDropdown,
                      customOnChangeHandler: (value, handleDropdownChange) => {
                        handleDropdownChange(value, "actionColumn");
                      },
                    },
                    {
                      type: "dropdown",
                      id: "actionColumn",
                      required: false,
                      optionsGetter: () => [],
                      label: (formData) => (formData.action && formData.action !== "Count" ? `${formData.action} of` : "Action Column"),
                      disabled: (formData) => formData.action === "Count",
                    },
                    {
                      type: "dropdown",
                      id: "chartType",
                      label: "Chart Type",
                      optionsGetter: () => [
                        { label: "Pie", value: "pie" },
                        { label: "Doughunt", value: "doughnut" },
                        { label: "Bar", value: "bar" },
                        { label: "Line", value: "line" },
                        { label: "Polar Area", value: "polarArea" },
                        { label: "Radar", value: "radar" },
                      ],
                    },
                  ],
                  onSubmit: (formData) => {
                    const dataToVisualize = rows
                      .map((r) => r.original)
                      .map((backendDataFilteredData) => {
                        var objOfSelectedColumns = {};
                        formData.columns.forEach((header) => {
                          objOfSelectedColumns[header] = backendDataFilteredData[colsDropdown.find((c) => c.value === header).accessor];
                        });

                        return objOfSelectedColumns;
                      });

                    var groupByProperties = [];

                    if (formData.action === "Count") {
                      groupByProperties = Object.keys(dataToVisualize[0]);
                    } else {
                      groupByProperties = formData.columns.filter((c) => c !== formData.actionColumn);
                    }

                    var dataToVisualizeGrouped = TableHelpers.groupBy(dataToVisualize, ...groupByProperties);

                    var labels = [];
                    var datasets = [];

                    dataToVisualizeGrouped.forEach((g) => {
                      var groupLabel = "";
                      Object.keys(g[0]).forEach((p) => {
                        if (formData.action === "Count" || groupByProperties.includes(p)) {
                          groupLabel = `${groupLabel}${groupLabel ? " /" : ""} ${g[0][p]}`;
                        }
                      });

                      if (formData.action === "Count") {
                        labels.push(groupLabel);
                      } else {
                        labels.push(`${groupLabel} - ${formData.action} of ${formData.actionColumn}`);
                      }
                    });

                    var reducer;
                    var reducerInit = 0;
                    var datasetData;

                    if (formData.action === "Count") {
                      datasetData = dataToVisualizeGrouped.map((g) => g.length);
                    } else if (formData.action === "Sum" || formData.action === "Average") {
                      reducer = TableHelpers.sum;
                    } else if (formData.action === "Min") {
                      reducer = TableHelpers.min;
                      reducerInit = Number.MAX_VALUE;
                    } else if (formData.action === "Max") {
                      reducer = TableHelpers.max;
                    }

                    if (reducer) {
                      datasetData = dataToVisualizeGrouped.map(
                        (g) => g.reduce(reducer(formData.actionColumn), reducerInit) / (formData.action === "Average" ? g.length : 1)
                      );
                    }

                    datasets.push({
                      data: datasetData,
                      backgroundColor: labels.map(getRandomColor),
                    });

                    confirmDialog({
                      header: "Visualized Data",
                      dismissableMask: true,
                      contentClassName: "rtable__visualize",
                      message: (
                        <span ref={visualizationChartRef}>
                          <Chart
                            options={{
                              maintainAspectRatio: false,
                              plugins: {
                                legend: {
                                  display: ["pie", "doughnut"].includes(formData.chartType),
                                },
                                datalabels: {
                                  anchor: "center",
                                  color: "black",
                                  backgroundColor: "white",
                                  textAlign: "center",
                                  align: "center",
                                  padding: {
                                    left: 3,
                                    right: 3,
                                  },
                                  font: {
                                    weight: "bold",
                                  },
                                },
                              },
                            }}
                            plugins={[ChartDataLabels]}
                            height="100%"
                            width="100%"
                            type={formData.chartType}
                            data={{
                              labels,
                              datasets,
                            }}
                          />
                        </span>
                      ),

                      footer: () => <Button label="Print" onClick={printVisualizedChart} />,
                    });
                  },
                });
              }}
            /> */}

            {/* <Button icon={faArrowsRotate} className="rtable__action" title="Refresh Table" disabled={isLoading} onClick={() => refreshData()} /> */}

            {fetchByDate && (
              <PopUp
                trigger={<Button icon={faMagnifyingGlass} className="rtable__action" />}
                className="rtable__action--poup"
                width="180px"
                direction="top"
                padding="0"
                items={[
                  {
                    title: "1 Month",
                    onClick: async () => {
                      dispatch({ type: "dateFilters/addFilter", payload: { screenId, date: "1" } });
                      await getData(dataUrl, { Date: "1" });
                    },
                  },
                  {
                    title: "3 Months",
                    onClick: async () => {
                      dispatch({ type: "dateFilters/addFilter", payload: { screenId, date: "3" } });
                      await getData(dataUrl, { Date: "3" });
                    },
                  },
                  {
                    title: "6 Months",
                    onClick: async () => {
                      dispatch({ type: "dateFilters/addFilter", payload: { screenId, date: "6" } });
                      await getData(dataUrl, { Date: "6" });
                    },
                  },
                  {
                    title: "12 Months",
                    onClick: async () => {
                      dispatch({ type: "dateFilters/addFilter", payload: { screenId, date: "12" } });
                      await getData(dataUrl, { Date: "12" });
                    },
                  },
                  {
                    title: "All Data",
                    onClick: async () => {
                      dispatch({ type: "dateFilters/addFilter", payload: { screenId, date: "" } });
                      await getData(dataUrl);
                    },
                  },
                ]}
              />
            )}
            {/* 
            <Button
              icon={faEraser}
              className="rtable__action"
              title="Clear Filters"
              disabled={!tableFilters.some((f) => f.value && f.screenId === screenId)}
              onClick={() => {
                if (!screenId) {
                  setAllFilters([]);
                } else {
                  dispatch({ type: "tableFilters/clear", payload: { screenId } });
                }
              }}
            />

            <Button
              icon={faTentArrowTurnLeft}
              className="rtable__action"
              title="Reset Scroll"
              onClick={() => {
                scrollRef.current.scrollTo(0, 0);
              }}
            /> */}
          </div>
        </div>
        <div>
          <Button label={<FontAwesomeIcon icon={faAngleRight} />} className="rtable__page-button" onClick={() => nextPage()} disabled={!canNextPage} />
          <Button
            className="rtable__page-button"
            label={<FontAwesomeIcon icon={faAngleDoubleRight} />}
            onClick={() => gotoPage(pageOptions.length - 1)}
            disabled={state.pageIndex + 1 === pageOptions.length || !pageOptions.length}
          />
        </div>
      </div>
    </div>
  );
}

function DefaultColumnFilter({ column }, screenId, gotoPage) {
  const { filterValue, setFilter } = column;
  const tableFilters = useSelector((state) => state.tableFilters.data);
  const dispatch = useDispatch();

  let value = tableFilters.find((filter) => filter.id === column.id && filter.screenId === screenId)?.value;

  if (!value) {
    value = filterValue;
  }
  return (
    <input
      value={value || ""}
      className="rtable__filter"
      onChange={(e) => {
        gotoPage(0);
        const newValue = e.target.value || "";

        if (!screenId) {
          setFilter(newValue);
          return;
        }

        dispatch({
          type: "tableFilters/set",
          payload: {
            id: column.id,
            value: newValue,
            screenId,
          },
        });
      }}
    />
  );
}

export default Table;
