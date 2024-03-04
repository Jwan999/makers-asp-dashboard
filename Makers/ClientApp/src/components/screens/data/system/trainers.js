import Screen from "../../screenClass";
import Constants from "../../../../utils/constants";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import { faXmarkCircle } from "@fortawesome/free-regular-svg-icons";

const trainers = new Screen({
  screenId: "trainers",

  contentHeader: "Trainers",

  dataUrl: "/api/dashboard/gettrainers",
  singleItemUrl: "/api/dashboard/gettrainer",
  addUrl: "/api/dashboard/addtrainer",
  editUrl: "/api/dashboard/edittrainer",
  deleteUrl: "/api/dashboard/deletetrainer",
  changeStatusUrl: "/api/dashboard/ChangetrainerStatus",

  modalWidth: "25vw",

  buttonClaimsAdd: ["A024"],
  // fetchByDate: true,

  tableColumns: [
    { Header: "Name", accessor: "NAMEX" },
    { Header: "Phone Number", accessor: "PHONEX" },
    { Header: "Email", accessor: "EMAILX" },
    {
      Header: "External",
      accessor: "IS_EXTERNAL",
      maxWidth: 60,
      minWidth: 60,
      Cell: ({ cell }) => <FontAwesomeIcon color={cell.row.original.IS_EXTERNAL === "Y" ? "Red" : "Green"} icon={faCheckCircle} />,
    },
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
    { title: "Edit", claims: ["A049"] },
    { title: "Clone", claims: ["A024"] },
    { title: row.original.IS_ACTIVE === Constants.Yes ? "Deactive" : "Active", claims: ["A029"] },
    { title: "Delete", claims: ["A04A"] },
    "-",
    {
      title: "Open Resources",
      claims: ["A023"],
      handler: async () => {
        window.open(row.original.IMG, "_blank");
      },
    },
  ],

  formInputs: [
    { type: "text", id: "NAMEX", label: "Name" },
    { type: "text", id: "PHONEX", label: "Phone Number" },
    { type: "text", id: "EMAILX", label: "Email" },
    { type: "text", id: "IMG", label: "Resources Link" },
    { type: "dropdown", id: "IS_EXTERNAL", label: "Is External", optionsGetter: () => Constants.YesNoDropdown },
  ],
});

export default trainers;
