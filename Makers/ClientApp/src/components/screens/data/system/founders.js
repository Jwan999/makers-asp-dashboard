import Screen from "../../screenClass";
import dayjs from "dayjs";
import Backend from "../../../../utils/backend";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import Constants from "../../../../utils/constants";

const founders = new Screen({
  screenId: "founders",

  contentHeader: "Founders",

  dataUrl: "api/dashboard/getfounders",
  singleItemUrl: "/api/dashboard/getfounder",
  addUrl: "/api/dashboard/addfounder",
  editUrl: "/api/dashboard/editfounder",
  deleteUrl: "/api/dashboard/deletefounder",

  modalWidth: "25vw",

  buttonClaimsAdd: ["A03B"],

  tableColumns: [
    { Header: "Name", accessor: "NAMEX" },
    { Header: "Email", accessor: "EMAILX" },
    { Header: "Phone", accessor: "PHONEX" },
    { Header: "Gender", accessor: "GENDER" },
  ],
  rowActions: (row) => [{ title: "Edit", claims: ["A03C"] }, { title: "Clone", claims: ["A03B"] }, "-", { title: "Delete", claims: ["A03D"] }],

  formInputs: [
    { type: "text", id: "NAMEX", label: "Name" },
    { type: "dropdown", id: "GENDER", label: "Gender", optionsGetter: () => Constants.GenderDropdown },
    { type: "text", id: "EMAILX", label: "Email" },
    { type: "text", id: "PHONEX", label: "Phone", required: false },
  ],
});

export default founders;
