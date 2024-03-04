import store from "../../store";
import Jwt from "../../utils/jwt";
import Form from "../common/form";
import Table from "../common/table";
import PopUp from "../common/popUp";
import Button from "../common/button";
import Backend from "../../utils/backend.js";
import Constants from "../../utils/constants";
import ProgressBar from "../common/progressBar";
import ImageViewer from "../common/imageViewer";
import { Dialog } from "primereact/dialog";
import { showToast } from "../../utils/toast";
import { useNavigate } from "react-router-dom";
import { singularize } from "../../utils/stringHelper";
import React, { useState, useEffect, useRef } from "react";
import { ValidateForm } from "../common/form";
import { ConfirmDialog } from "primereact/confirmdialog";
import { useSelector, useDispatch } from "react-redux";
import { faPlus, faTimes, faPenToSquare, faCheck, faChevronLeft, faDashboard, faTable } from "@fortawesome/free-solid-svg-icons";
import { faCircleXmark } from "@fortawesome/free-solid-svg-icons";
import searchIcon from "../../images/searchIcon.png";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
function Screen({ screen }) {
  const [formData, setFormData] = useState(initializeFormData(screen.formInputs));
  const [formInputs, setFormInputs] = useState(screen.formInputs);
  const [modalData, setModalData] = useState();
  const [tableData, setTableData] = useState([]);
  const [dashboardData, setDashboardData] = useState([]);
  const [tableColumns, setTableColumns] = useState(screen.tableColumns || []);
  const [inEditMode, setInEditMode] = useState(false);
  const [inModalMode, setInModalMode] = useState(false);
  const [editEntityId, setEditEntityId] = useState(0);
  const [paramsList, setParamsList] = useState([]);
  const [progressBar, setProgressBar] = useState({ isShown: false, header: "", status: "", message: "", progress: 0 });
  const [imageViewer, setImageViewer] = useState({ isShown: false, images: [] });
  const [screenViewType, setScreenViewType] = useState(screen.dashboard ? Constants.ScreenViewTypeDashboard : Constants.ScreenViewTypeTable);

  const dispatch = useDispatch();
  const navigate = useNavigate();
  const isLoading = useSelector((state) => state.dashboard.isLoading);
  const formFilters = useSelector((state) => state.formFilters);
  const storedDateFilter = useSelector((state) => state.dateFilters.data).find((value) => value.screenId === screen.screenId)?.date;
  const screenQuery = useSelector((state) => state.screenQuery.data);
  const screenDashboardQuery = useSelector((state) => state.screenDashboardQuery.data);

  let autoRefreshInterval;

  useEffect(() => {
    if (screen.screenId !== formFilters.screenId && formFilters.screenId !== "") {
      dispatch({
        type: "formFilters/clear",
      });
    }

    if (screenQuery.screenId && screen.screenId !== screenQuery.screenId) {
      dispatch({ type: "screenQuery/clearData" });
    }

    if (screenDashboardQuery.screenId && screen.screenId !== screenDashboardQuery.screenId) {
      dispatch({ type: "screenDashboardQuery/clearData" });
    }

    if (store.getState().screenQuery.data.dataUrl) {
      getData(screenQuery.dataUrl, screenQuery.dataParams);
    }

    if (screen.hasInitialFilter && !store.getState().screenQuery.data.dataUrl) {
      var filter = document.getElementsByClassName("filter-button")[0];
      if (filter) {
        filter.click();
      }
    }

    if (!screen.hasInitialFilter && !store.getState().screenQuery.data.dataUrl) {
      if (screenViewType === Constants.ScreenViewTypeDashboard) {
        getData(screen.dataUrlDashboard);
      } else if (screen.dataUrl && screen.fetchByDate) {
        const defaultDate = screen.dateFilter || "3";
        if (storedDateFilter === undefined) {
          dispatch({ type: "dateFilters/addFilter", payload: { screenId: screen.screenId, date: defaultDate } });
        }
        getData(screen.dataUrl, { Date: storedDateFilter !== undefined ? storedDateFilter : defaultDate });
      } else if (screen.dataUrl && !screen.fetchByDate) {
        getData(screen.dataUrl);
      } else {
        navigate(screen.backUrl || "/dashboard/home", { replace: true });
      }
    }

    if (screen.autoRefresh && screen.autoRefresh.duration > 0) {
      // eslint-disable-next-line react-hooks/exhaustive-deps
      autoRefreshInterval = setInterval(() => refreshData(), screen.autoRefresh.duration);
    }

    document.title = screen.contentHeader;

    return () => {
      clearInterval(autoRefreshInterval);
    };

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function initializeFormData(formInputs) {
    var initialFormData = {};
    formInputs?.forEach((formInput) => {
      if (["multiselect", "file"].includes(formInput.type)) {
        initialFormData[formInput.id] = formInput.value || [];
      } else {
        initialFormData[formInput.id] = formInput.value || "";
      }
    });
    return initialFormData;
  }

  async function getData(url, dataParams = screen.dataParams || {}, screenViewTypeParam = screenViewType) {
    var response = await Backend.call(url, dataParams);

    if (response.StatusCode === 0) {
      if (screen.screenId === `dataViewer${screen.contentHeader}`) {
        let columns = [];

        if (!response.ResponseData || response.ResponseData.length === 0) {
          navigate(screen.backUrl || "/dashboard/home", { replace: true });
          return;
        }

        for (let property in response.ResponseData[0]) {
          let scaleFactor = 10;
          let width = 200;
          let dataWidth = 0;
          let headerWidth = property.length * (property.length > 5 ? scaleFactor : scaleFactor + 3);

          const firstColumnData = response.ResponseData[0][property];
          if (firstColumnData !== null && firstColumnData !== undefined) {
            dataWidth = firstColumnData.toString().length * (firstColumnData.toString().length > 5 ? scaleFactor : scaleFactor + 3);
          }

          if (headerWidth > dataWidth) {
            width = headerWidth;
          } else {
            width = dataWidth;
          }

          if (response.ResponseData[0].hasOwnProperty(property)) {
            columns.push({
              Header: property,
              accessor: property,
              width: width,
              resizeable: true,
            });
          }
        }

        // create a list of the parameters for the report query
        if (dataParams.ReportParams) {
          setParamsList(Object.keys(dataParams.ReportParams).map((param) => ({ title: param + " : " + dataParams.ReportParams[param] })));
        }

        setTableColumns(columns);
        setTableData(response.ResponseData);
      } else {
        if (screenViewTypeParam === Constants.ScreenViewTypeDashboard) {
          setDashboardData(response.ResponseData);
        } else {
          setTableColumns(screen.tableColumns);
          setTableData(response.ResponseData);
        }
      }
    } else {
      navigate(screen.backUrl || "/dashboard/home", { replace: true });
    }
  }

  function refreshData(newScreenViewType = Constants.ScreenViewTypeTable) {
    let data = {};
    let url = newScreenViewType === Constants.ScreenViewTypeDashboard ? screen.dataUrlDashboard : screen.dataUrl;
    const { formFilters, dataViewer, dateFilters, screenQuery, screenDashboardQuery } = store.getState();
    if (screen.screenId.startsWith("dataViewer")) {
      data = dataViewer.data.dataParams;
    } else if (formFilters.screenId === screen.screenId) {
      data = formFilters.data;
    } else if (screenQuery.data.dataUrl) {
      data = screenQuery.data.dataParams;
      url = screenQuery.data.dataUrl;
    } else if (newScreenViewType !== Constants.ScreenViewTypeTable && screenDashboardQuery.data.dataUrlDashboard) {
      data = screenDashboardQuery.data.dataParams;
      url = screenDashboardQuery.data.dataUrlDashboard;
    }
    if (screen.fetchByDate) {
      let storedDateFilter = dateFilters.data.find((v) => v.screenId === screen.screenId)?.date;
      data["Date"] = storedDateFilter;
    }
    getData(url, data, newScreenViewType);
  }

  async function getSingleItem(id, clone = false) {
    var singleItem = (await Backend.call(screen.singleItemUrl, { Id: id })).ResponseData[0];
    if (singleItem) {
      var newFormData = Object.assign({}, formData);
      Object.getOwnPropertyNames(formData).forEach((p) => {
        if (singleItem[p] === null || singleItem[p] === undefined) {
          if (typeof newFormData[p] === "object") {
            newFormData[p] = [];
          } else {
            newFormData[p] = "";
          }
        } else {
          newFormData[p] = singleItem[p];
        }
      });

      if (clone) {
        setInEditMode(false);
      } else {
        setInEditMode(true);
      }

      setFormData(newFormData);
      setEditEntityId(id);
      await showModal(newFormData);
    }
  }

  async function showModal(formData, inputs) {
    var updatedFormInputs = Object.assign([], inputs ? inputs : formInputs);

    const optionsPromises = [];

    for (let i = 0; i < updatedFormInputs.length; i++) {
      if (["dropdown", "multiselect", "autocomplete"].includes(updatedFormInputs[i].type)) {
        optionsPromises.push(updatedFormInputs[i].optionsGetter(formData));
      }
    }

    const resolvedPromises = await Promise.all(optionsPromises);

    let optionsIndex = 0;
    updatedFormInputs.forEach((formInput) => {
      if (["dropdown", "multiselect", "autocomplete"].includes(formInput.type)) {
        formInput.options = resolvedPromises[optionsIndex];
        optionsIndex++;
      }
    });

    setFormInputs(updatedFormInputs);
    setInModalMode(true);
  }

  function closeModal() {
    setInModalMode(false);

    // Allow modal to fully close and then reset data.
    setTimeout(() => {
      setEditEntityId(0);
      setInEditMode(false);
      setFormData(initializeFormData(screen.formInputs));
      setFormInputs(screen.formInputs);
      setModalData();
    }, 150);
  }

  function updateForm(inputId, inputValue) {
    var newFormData = Object.assign({}, formData);
    newFormData[inputId] = inputValue;
    setFormData(newFormData);
  }

  async function submit() {
    var error = ValidateForm(formInputs, formData);

    if (!error) {
      var submitData = Object.assign({}, formData);
      if (inEditMode) {
        submitData["EditEntityId"] = editEntityId;
      }
      if (screen.screenId === screenQuery.screenId) {
        submitData = { ...submitData, ...screenQuery.dataParams };
      }

      await Backend.call(inEditMode ? screen.editUrl : screen.addUrl, submitData);
      await getData(screen.dataUrl, { Date: storedDateFilter, ...screenQuery.dataParams });
      closeModal();
    } else {
      showToast(error, "error");
    }
  }

  async function handleDropdownChange(data, targetInputId, url) {
    if (!url) {
      setFormInputs(
        formInputs.map((input) => {
          if (input.id === targetInputId) {
            return {
              ...input,
              options: data,
            };
          }

          return input;
        })
      );
      return;
    }
    setFormData((prevState) => ({ ...prevState, [targetInputId]: typeof prevState[targetInputId] === "object" ? [] : "" }));

    if (Object.values(data).some((item) => !item)) {
      return;
    }

    var updatedFormInputs = Object.assign([], formInputs);

    for (var i = 0; i < updatedFormInputs.length; i++) {
      if (updatedFormInputs[i].id === targetInputId) {
        updatedFormInputs[i].options = (await Backend.call(url, data)).ResponseData || [];
      }
    }

    setFormInputs(updatedFormInputs);
  }

  function handleAutoComplete(query, inputId) {
    var updatedFormInputs = Object.assign([], formInputs);

    for (var i = 0; i < updatedFormInputs.length; i++) {
      if (updatedFormInputs[i].id === inputId) {
        updatedFormInputs[i].suggestions = updatedFormInputs[i].options.filter((value) => value.label.toLowerCase().includes(query));
      }
    }

    setFormInputs(updatedFormInputs);
  }

  const handleResetFormData = (data) => {
    setFormData((prevState) => ({ ...prevState, ...data }));
  };

  const changeScreenView = ({ newScreenViewType }) => {
    if (newScreenViewType === Constants.ScreenViewTypeTable && (!tableData || tableData.length === 0)) {
      refreshData(newScreenViewType);
    }
    setScreenViewType(newScreenViewType);
  };

  const changeModal = async ({ title, inputs, buttonText, width, onSubmit, footerActions }) => {
    setModalData({ title, buttonText, onSubmit, width, footerActions });

    var data = initializeFormData(inputs);

    if (formFilters.screenId === screen.screenId && JSON.stringify(Object.keys(data).sort()) === JSON.stringify(Object.keys(formFilters.data).sort())) {
      data = formFilters.data;
    }

    await showModal(data, inputs);

    setFormData(data);
  };

  const fieldName = singularize(screen.contentHeader);
  const [search, setSearch] = useState("");
  const inputRef = useRef(null);
  useEffect(() => {
    const handleKeyPress = (event) => {
      if (event.key === "/") {
        inputRef.current.focus();
        event.preventDefault();
      }
    };
    document.addEventListener("keydown", handleKeyPress);
    return () => {
      document.removeEventListener("keydown", handleKeyPress);
    };
  }, []);

  return (
    <div className="fade-in bg-gray-50 h-full w-full overflow-x-scroll lg:overflow-x-hidden ">
      <div className="content-component__header">
        <div className="flex justify-center items-center">
          <h4 className="text-orange font-semibold" style={{ textAlign: "center" }}>
            {screen.contentHeader === "Dashboard" ? screen.contentHeader : screen.contentHeader}
          </h4>
          {screen.contentHeader !== "Dashboard" && (
            <div className="relative flex items-center text-gray-400 focus-within:text-gray-600 pl-5">
              <img src={searchIcon} alt="" className="h-4 w-4 absolute ml-3 pointer-events-none" />
              <input
                className=" px-10 bg-gray-200  placeholder-gray-400 text-black rounded-xl  "
                value={search}
                placeholder="Press / to search"
                onChange={(e) => setSearch(e.target.value)}
                ref={inputRef}
              />
              <FontAwesomeIcon icon={faCircleXmark} className="absolute h-4 w-4 mr-3 right-0" onClick={() => setSearch("")} />
            </div>
          )}
        </div>
        <div className="content-actions">
          {screen.actions &&
            screen.actions.map((action, index) =>
              action.type === "button" || action.type === "filter" ? (
                Jwt.isAuthorized(action.claims) && (
                  <Button
                    key={index}
                    label={action.title}
                    icon={action.icon}
                    disabled={isLoading}
                    className={action.type === "filter" ? "filter-button" : ""}
                    onClick={() => action.handler({ changeModal, closeModal, getData, setProgressBar, screenViewType })}
                  />
                )
              ) : action.type === "popup" ? (
                <PopUp
                  key={index}
                  trigger={<Button label={action.title} icon={action.icon} />}
                  disabled={isLoading}
                  items={action.popUpActions().map(
                    (popupAction) =>
                      Jwt.isAuthorized(popupAction.claims) && {
                        title: popupAction.title,
                        onClick: () => popupAction.handler({ changeModal, closeModal, getData, setProgressBar }),
                      }
                  )}
                  width={action.width}
                  direction="bottom"
                />
              ) : null
            )}
          {paramsList.length > 0 && <PopUp trigger={<Button label="Parameters" />} items={paramsList} direction="bottom" width="250" />}
          {screen.dataUrlDashboard && screen.dataUrl && (
            <Button
              icon={screenViewType === Constants.ScreenViewTypeDashboard ? faTable : faDashboard}
              iconSize="lg"
              disabled={isLoading}
              onClick={() => {
                changeScreenView({
                  newScreenViewType: screenViewType === Constants.ScreenViewTypeDashboard ? Constants.ScreenViewTypeTable : Constants.ScreenViewTypeDashboard,
                });
              }}
            />
          )}
          {screen.buttonClaimsAdd && Jwt.isAuthorized(screen.buttonClaimsAdd) && (
            <div className="flex justify-end items-center space-x-2">
              <Button
                // icon={faPlus}
                label={"Add Item"}
                iconSize="lg"
                color="red"
                disabled={isLoading}
                onClick={async () => {
                  setInEditMode(false);
                  await showModal();
                }}
              />
            </div>
          )}
          {(screen.backUrl || screenQuery.backUrl) && (
            <Button
              icon={faChevronLeft}
              iconSize="lg"
              disabled={isLoading}
              onClick={() => {
                if (screenQuery.backUrl) {
                  navigate(screenQuery.backUrl, { replace: true });
                  dispatch({ type: "screenQuery/clearData" });
                } else {
                  navigate(screen.backUrl, { replace: true });
                }
              }}
            />
          )}
        </div>
      </div>
      <Dialog
        visible={inModalMode}
        style={{ width: !modalData ? screen.modalWidth : modalData.width }}
        onHide={closeModal}
        header={
          !modalData ? (
            <h5 className="modal-header">{inEditMode ? `Edit ${fieldName}` : `Add ${fieldName}`}</h5>
          ) : (
            <h5 className="modal-header">{modalData.title}</h5>
          )
        }
        footer={
          <>
            {modalData && modalData.footerActions
              ? modalData.footerActions.map((action) => {
                  return (
                    Jwt.isAuthorized(action.claims) && (
                      <Button
                        key={action.title}
                        label={action.title}
                        icon={action.icon}
                        className={action.className}
                        disabled={isLoading}
                        onClick={() => action.handler({ formData, setImageViewer })}
                      />
                    )
                  );
                })
              : null}
            {!modalData ? (
              <Button
                label={inEditMode ? "Edit" : "Add"}
                icon={inEditMode ? faPenToSquare : faPlus}
                disabled={isLoading}
                onClick={async () => await submit()}
              />
            ) : modalData.buttonText ? (
              <Button
                label={modalData.buttonText}
                icon={faCheck}
                disabled={isLoading}
                onClick={() => {
                  var error = ValidateForm(formInputs, formData);
                  if (!error) {
                    modalData.onSubmit(formData, getData);
                  } else {
                    showToast(error, "error");
                  }
                }}
              />
            ) : null}

            <Button label="Close" icon={faTimes} className="p-button-primary" onClick={closeModal} />
          </>
        }
      >
        <Form
          isLoading={isLoading}
          inEditMode={inEditMode}
          data={formData}
          updateForm={updateForm}
          inputs={formInputs || []}
          handleDropdownChange={handleDropdownChange}
          handleAutoComplete={handleAutoComplete}
          handleResetFormData={handleResetFormData}
        />
      </Dialog>
      {screenViewType === Constants.ScreenViewTypeTable ? (
        <Table
          data={tableData}
          columns={tableColumns}
          getData={getData}
          getSingleItem={getSingleItem}
          rowSubComponentData={screen.rowSubComponentData}
          deleteUrl={screen.deleteUrl}
          dataUrl={screen.dataUrl}
          screenId={screen.screenId}
          screenContentHeader={screen.contentHeader}
          changeModal={changeModal}
          fetchByDate={screen.fetchByDate}
          dateFilter={storedDateFilter !== undefined ? storedDateFilter : screen.dateFilter || "3"}
          setProgressBar={setProgressBar}
          setImageViewer={setImageViewer}
          rowActions={screen.rowActions}
          refreshData={refreshData}
          search={search}
          changeStatusUrl={screen.changeStatusUrl}
        />
      ) : screenViewType === Constants.ScreenViewTypeTable ? (
        screen.Cards(getData, getSingleItem, screen.dataUrl)
      ) : (
        screen.dashboard(dashboardData, getData, changeScreenView)
      )}
      <ProgressBar progressBar={progressBar} />
      <ImageViewer imageViewer={imageViewer} setImageViewer={setImageViewer} />
      <ConfirmDialog />
    </div>
  );
}

export default Screen;
