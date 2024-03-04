import Screen from "../../screenClass";
import dayjs from "dayjs";
import Backend from "../../../../utils/backend";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import Constants from "../../../../utils/constants";
import store from "../../../../store";

const events = new Screen({
  screenId: "events",

  contentHeader: "events",

  dataUrl: "/api/dashboard/getevents",
  singleItemUrl: "/api/dashboard/getevent",
  addUrl: "/api/dashboard/addevent",
  editUrl: "/api/dashboard/editevent",
  deleteUrl: "/api/dashboard/deleteevent",
  changeStatusUrl: "/api/dashboard/ChangeeventStatus",

  modalWidth: "25vw",

  buttonClaimsAdd: ["A040"],

  tableColumns: [
    { Header: "Name", accessor: "NAMEX" },
    { Header: "Project", accessor: "PROJ" },
    { Header: "Start Date", accessor: "DATEX" },
    { Header: "Duration in Days", accessor: "DURATION" },
    { Header: "Location", accessor: "LOCATIONX" },
    { Header: "Overview", accessor: "OVERVIEW" },
    { Header: "Number of Participants", accessor: "PARTICIPANTS_NUM" },
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
      title: "Open Link",
      claims: ["A03F"],
      handler: async () => {
        window.open(row.original.LINKX, "_blank");
      },
    },
    "-",
    { title: "Edit", claims: ["A041"] },
    { title: "Clone", claims: ["A040"] },
    { title: row.original.IS_ACTIVE === Constants.Yes ? "Deactive" : "Active", claims: ["A043"] },
    { title: "Delete", claims: ["A042"] },
  ],

  formInputs: [
    { type: "text", id: "NAMEX", label: "Name" },
    { type: "text", id: "OVERVIEW", label: "Overview" },
    { type: "calendar", id: "DATEX", label: "Start Date", value: dayjs().format("YYYY-MM-DD"), dateFormat: "YYYY-MM-DD" },
    { type: "text", id: "DURATION", label: "Duration in Days", dataType: "number" },
    { type: "text", id: "LINKX", label: "Link", required: false },
    { type: "textarea", id: "LOCATIONX", label: "Location", required: false },
    { type: "text", id: "PARTICIPANTS_NUM", label: "Number of Participants", dataType: "number", required: false },
  ],
});

export default events;
