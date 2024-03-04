import Backend from "../../../../utils/backend";
import Screen from "../../screenClass";
import Constants from "../../../../utils/constants";
import store from "../../../../store";
import { showToast } from "../../../../utils/toast";
import { replaceAll } from "../../../../utils/stringHelper";
import { saveLocalStorageItem } from "../../../../utils/localStorageHelper";

const reports = new Screen({
  screenId: "reports",

  contentHeader: "Reports",

  dataUrl: "/api/dashboard/getreports",
  singleItemUrl: "/api/dashboard/getreport",
  addUrl: "/api/dashboard/addreport",
  editUrl: "/api/dashboard/editreport",
  deleteUrl: "/api/dashboard/deletereport",

  modalWidth: "35vw",

  buttonClaimsAdd: ["C02B"],

  tableColumns: [
    { Header: "Name", accessor: "REPORT_NAME", minWidth: 350, maxWidth: 350 },
    { Header: "Category", accessor: "REPORT_CAT", minWidth: 150, maxWidth: 150 },
    { Header: "Description", accessor: "REPORT_DESC" },
  ],

  rowActions: (row) => [
    {
      title: "Execute",
      claims: ["CO2F"],
      handler: async ({ navigate, changeModal }) => {
        const query = (await Backend.call("/api/dashboard/getreportquery", { CodeId: row.original.ID })).ResponseData;

        let isParameterized = false;
        let queryParams = "";

        if (query === null || query === undefined) {
          showToast("Query has not been defined yet for this report", "error");
          return;
        }

        replaceAll(
          replaceAll(
            replaceAll(
              replaceAll(
                replaceAll(replaceAll(replaceAll(query.replace(/'/g, " ").replace(/(\r\n\t|\n|\r\t)/gm, " "), "%", " "), "=", " "), "(", " "),
                ")",
                " "
              ),
              ",",
              " "
            ),
            ">",
            " "
          ),
          "<",
          " "
        )
          .split(" ")
          .forEach((queryWord) => {
            if (queryWord.includes("&") && !queryParams.split("&").includes(queryWord.replace("&", ""))) {
              queryParams += queryWord;
              isParameterized = true;
            }
          });

        if (isParameterized) {
          let splittedParams = queryParams.split("&").slice(1);
          let inputs = [];

          splittedParams.forEach((param) => {
            let inputType = "text";

            const isCalendar = param.toLowerCase().includes("yyyymmdd");

            if (isCalendar) {
              inputType = "calendar";
            }

            inputs.push({ type: inputType, id: param, label: param.toUpperCase(), dateFormat: "YYYYMMDD" });
          });

          let savedReportsParams = JSON.parse(localStorage.getItem("makers-reports-form-params")) || [];

          const currentSavedReport = savedReportsParams.find((report) => report.id === row.original.ID);

          if (currentSavedReport && currentSavedReport.data) {
            inputs.forEach((el, i) => {
              inputs[i].value = currentSavedReport.data[el.id];
            });
          }

          var currentDateTime = new Date().getTime();

          savedReportsParams = savedReportsParams.filter((report) => report.expirationDate > currentDateTime);

          localStorage.setItem("makers-reports-form-params", JSON.stringify(savedReportsParams));

          const handleSubmit = (formData) => {
            saveLocalStorageItem({ data: formData, itemId: row.original.ID, localStorageKey: "makers-reports-form-params" });

            store.dispatch({
              type: "dataViewer/setData",
              payload: {
                dataUrl: "/api/dashboard/executereport",
                dataParams: { ReportId: row.original.ID, IsParameterized: isParameterized, ReportParams: formData },
                backUrl: "/dashboard/screens/reports",
                headerTitle: row.original.REPORT_NAME,
                id: row.original.ID,
              },
            });

            navigate("/dashboard/screens/dataViewer", { replace: true });
          };

          changeModal({ title: "Parameters", inputs, buttonText: "Execute", width: "25vw", onSubmit: handleSubmit });
        } else {
          store.dispatch({
            type: "dataViewer/setData",
            payload: {
              dataUrl: "/api/dashboard/executereport",
              dataParams: { ReportId: row.original.ID, IsParameterized: false, ReportParams: "" },
              backUrl: "/dashboard/screens/reports",
              headerTitle: row.original.REPORT_NAME,
              id: row.original.ID,
            },
          });

          navigate("/dashboard/screens/dataViewer", { replace: true });
        }
      },
    },
    "-",
    {
      title: "Edit Query",
      claims: ["C02E"],
      handler: ({ navigate }) => {
        store.dispatch({
          type: "codeEditor/setEditor",
          payload: {
            buttonText: "Done",
            language: "sql",
            headerTitle: row.original.REPORT_NAME,
            codeId: row.original.ID,
            dataUrl: "/api/dashboard/getreportquery",
            backUrl: "/dashboard/screens/reports",
            submitUrl: "/api/dashboard/editreportquery",
          },
        });

        navigate("/dashboard/editor", { replace: true });
      },
    },
    { title: "Edit", claims: ["C02C"] },
    { title: "Clone", claims: ["C02B"] },
    { title: "Delete", claims: ["C02D"] },
  ],

  formInputs: [
    { type: "text", id: "REPORT_NAME", label: "Report Name" },
    { type: "text", id: "REPORT_DESC", label: "Report Description" },
    {
      type: "dropdown",
      id: "REPORT_CAT",
      label: "Report Category",
      optionsGetter: async () => (await Backend.call("/api/dashboard/Dictdropdown", { Key: Constants.DictKeyReportCategory, Lang: "en" })).ResponseData,
    },
  ],
});

export default reports;
