import Screen from "../../screenClass";
import dayjs from "dayjs";
import Backend from "../../../../utils/backend";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import Constants from "../../../../utils/constants";
import store from "../../../../store";

const interns = new Screen({
  screenId: "interns",

  contentHeader: "Interns",

  dataUrl: "/api/dashboard/getinterns",
  singleItemUrl: "/api/dashboard/getintern",
  addUrl: "/api/dashboard/addintern",
  editUrl: "/api/dashboard/editintern",
  deleteUrl: "/api/dashboard/deleteintern",
  changeStatusUrl: "/api/dashboard/ChangeinternStatus",

  modalWidth: "25vw",

  buttonClaimsAdd: ["A045"],

  tableColumns: [
    { Header: "Name", accessor: "NAMEX" },
    { Header: "Project", accessor: "PROJ" },
    { Header: "Gender", accessor: "GENDER" },
    { Header: "Age", accessor: "AGE" },
    { Header: "City", accessor: "CITY" },
    { Header: "Email", accessor: "EMAIL" },
    { Header: "Phone Number", accessor: "PHONE_NUMBER" },
    { Header: "Start Date", accessor: "START_DATE" },
    { Header: "Position", accessor: "POSITION" },
    { Header: "Salary", accessor: "SALARY" },
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
      claims: ["A044"],
      handler: async () => {
        window.open(row.original.LINKX, "_blank");
      },
    },
    "-",
    { title: "Edit", claims: ["A046"] },
    { title: "Clone", claims: ["A045"] },
    { title: row.original.IS_ACTIVE === Constants.Yes ? "Deactive" : "Active", claims: ["A048"] },
    { title: "Delete", claims: ["A047"] },
  ],

  formInputs: [
    { type: "text", id: "NAMEX", label: "Name" },
    { type: "dropdown", id: "GENDER", label: "Gender", optionsGetter: () => Constants.GenderDropdown },
    { type: "text", id: "AGE", label: "Age", dataType: "number" },
    {
      type: "dropdown",
      id: "PROJ",
      label: "Project",
      optionsGetter: async () => (await Backend.call("/api/dashboard/ProjectsDropdown")).ResponseData,
      dataType: "number",
    },
    {
      type: "dropdown",
      id: "CITY",
      label: "City",
      optionsGetter: async () => (await Backend.call("/api/dashboard/DictDropdown", { Key: Constants.DictKeyCity })).ResponseData,
    },
    { type: "calendar", id: "START_DATE", label: "Start Date", value: dayjs().format("YYYY-MM-DD"), dateFormat: "YYYY-MM-DD", required: false },
    { type: "text", id: "PHONE_NUMBER", label: "Phone Number" },
    { type: "text", id: "EMAIL", label: "Email" },
    {
      type: "dropdown",
      id: "POSITION",
      label: "Position",
      optionsGetter: async () => (await Backend.call("/api/dashboard/DictDropdown", { Key: Constants.DictKeyPosition })).ResponseData,
    },
    { type: "text", id: "SALARY", label: "Salary", dataType: "number", required: false },
    { type: "text", id: "LINKX", label: "Link", required: false },
  ],
});

export default interns;
