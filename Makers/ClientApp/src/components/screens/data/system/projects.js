import Screen from "../../screenClass";
import dayjs from "dayjs";
import Backend from "../../../../utils/backend";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import Constants from "../../../../utils/constants";
import store from "../../../../store";

const projects = new Screen({
  screenId: "projects",

  contentHeader: "Projects",

  dataUrl: "/api/dashboard/getprojects",
  singleItemUrl: "/api/dashboard/getproject",
  addUrl: "/api/dashboard/addproject",
  editUrl: "/api/dashboard/editproject",
  deleteUrl: "/api/dashboard/deleteproject",
  changeStatusUrl: "/api/dashboard/ChangeprojectStatus",

  modalWidth: "25vw",

  buttonClaimsAdd: ["A01C"],

  tableColumns: [
    { Header: "Name", accessor: "NAMEX" },
    { Header: "Sponsoed By / Funded By", accessor: "INST" },
    { Header: "Start Date", accessor: "START_DATE" },
    { Header: "End Date", accessor: "END_DATE" },
    { Header: "Duration", accessor: "DURATION" },
    { Header: "Overview", accessor: "OVERVIEW" },
    {
      Header: "Status",
      accessor: "IS_ACTIVE",
      maxWidth: 60,
      minWidth: 10,
      Cell: ({ cell }) => (
        <FontAwesomeIcon color={cell.row.original.IS_ACTIVE === "N" ? "Red" : "Green"} icon={cell.row.original.IS_ACTIVE === "N" ? faBan : faCheckCircle} />
      ),
    },
  ],
  rowActions: (row) => [
    {
      title: "View Events",
      handler: ({ navigate }) => {
        store.dispatch({
          type: "screenQuery/setData",
          payload: {
            dataUrl: "/api/dashboard/getevents",
            dataParams: { PROJ_ID: row.original.ID },
            backUrl: "/dashboard/screens/projects",
            screenId: "events",
          },
        });
        navigate("/dashboard/screens/events", { replace: true });
      },
    },
    "-",
    {
      title: "View Image",
      claims: ["A001"],
      handler: async ({ setImageViewer }) => {
        var resp = (await Backend.call("/api/dashboard/getproject", { Id: row.original.ID })).ResponseData;
        setImageViewer({
          isShown: true,
          images: resp.map((img) => {
            return {
              src: img.ICON,
              title: <>{img.INSDATE}</>,
            };
          }),
        });
      },
    },
    {
      title: "Open Link",
      claims: ["A001"],
      handler: async () => {
        window.open(row.original.LINKX, "_blank");
      },
    },
    "-",
    { title: "Edit", claims: ["A01D"] },
    { title: "Clone", claims: ["A01C"] },
    { title: row.original.IS_ACTIVE === Constants.Yes ? "Deactive" : "Active", claims: ["A026"] },
    { title: "Delete", claims: ["A01E"] },
  ],

  formInputs: [
    { type: "text", id: "NAMEX", label: "Name" },
    {
      type: "multiselect",
      id: "INST",
      label: "Sponsoed By / Funded By",
      optionsGetter: async () => (await Backend.call("/api/dashboard/instDropdown")).ResponseData,
      dataType: "number",
    },
    { type: "text", id: "OVERVIEW", label: "Overview" },
    { type: "calendar", id: "START_DATE", label: "Contract Start Date", value: dayjs().format("YYYY-MM-DD"), dateFormat: "YYYY-MM-DD" },
    { type: "text", id: "DURATION", label: "Duration", dataType: "number" },
    { type: "text", id: "LINKX", label: "Link", required: false },
    { type: "file", id: "ICON", label: "Image", required: false },
  ],
});

export default projects;
