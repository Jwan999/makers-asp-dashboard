import Screen from "../../screenClass";
import Constants from "../../../../utils/constants";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import Backend from "../../../../utils/backend";

const students = new Screen({
  screenId: "students",

  contentHeader: "Students",

  dataUrl: "/api/dashboard/getstudents",
  singleItemUrl: "/api/dashboard/getstudent",
  addUrl: "/api/dashboard/addstudent",
  editUrl: "/api/dashboard/editstudent",
  deleteUrl: "/api/dashboard/deletestudent",
  changeStatusUrl: "/api/dashboard/ChangestudentStatus",

  modalWidth: "25vw",

  buttonClaimsAdd: ["CO31"],
  // fetchByDate: true,

  tableColumns: [
    { Header: "Name", accessor: "NAMEX" },
    { Header: "Training", accessor: "TRAINING" },
    { Header: "Field Of Study", accessor: "FIELD" },
    { Header: "University", accessor: "UNIVERSITY" },
    { Header: "Phone Number", accessor: "PHONE_NUMBER" },
    { Header: "Gender", accessor: "GENDER" },
    { Header: "Email", accessor: "EMAIL" },
    { Header: "Age", accessor: "AGE" },
    {
      Header: "Status",
      accessor: "IS_ACTIVE",
      maxWidth: 60,
      minWidth: 60,
      Cell: ({ cell }) => (
        <FontAwesomeIcon color={cell.row.original.IS_ACTIVE === "N" ? "Red" : "Green"} icon={cell.row.original.IS_ACTIVE === "N" ? faBan : faCheckCircle} />
      ),
    },
  ],

  rowActions: (row) => [
    { title: "Edit", claims: ["CO32"] },
    { title: "Clone", claims: ["CO31"] },
    { title: row.original.IS_ACTIVE === Constants.Yes ? "Deactive" : "Active", claims: ["CO34"] },
    { title: "Delete", claims: ["CO33"] },
    "-",
    {
      title: "Open Resources",
      claims: ["A001"],
      handler: async () => {
        window.open(row.original.IMG, "_blank");
      },
    },
  ],

  formInputs: [
    { type: "text", id: "NAMEX", label: "Name" },
    {
      type: "dropdown",
      id: "TRAINING",
      label: "Training",
      optionsGetter: async () => (await Backend.call("/api/dashboard/trainingdropdown")).ResponseData,
      dataType: "number",
    },
    { type: "text", id: "UNIVERSITY", label: "University" },
    { type: "text", id: "FIELD", label: "Feild of Study" },
    { type: "text", id: "PHONE_NUMBER", label: "Phone Number" },
    { type: "dropdown", id: "GENDER", label: "Gender", optionsGetter: () => Constants.GenderDropdown },
    { type: "text", id: "EMAIL", label: "Email", required: false },
    { type: "text", id: "AGE", label: "Age", dataType: "number", required: false },
  ],
});

export default students;
