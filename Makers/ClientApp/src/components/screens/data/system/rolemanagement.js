import Backend from "../../../../utils/backend";
import Screen from "../../screenClass";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import Constants from "../../../../utils/constants";

const roleManagement = new Screen({
  screenId: "rolemanagement",

  contentHeader: "Roles",

  dataUrl: "/api/dashboard/getroles",
  singleItemUrl: "/api/dashboard/getrole",
  addUrl: "/api/dashboard/addrole",
  editUrl: "/api/dashboard/editrole",
  deleteUrl: "/api/dashboard/deleterole",
  changeStatusUrl: "/api/dashboard/ChangeroleStatus",

  modalWidth: "25vw",

  buttonClaimsAdd: ["A007"],

  tableColumns: [
    { Header: "ID", accessor: "ID", maxWidth: 75, minWidth: 75 },
    { Header: "Name", accessor: "ROLE_NAME" },
    { Header: "Insert Date", accessor: "INSDATE" },
    { Header: "Last Update", accessor: "LUPDATE" },
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
    { title: "Edit", claims: ["A008"] },
    { title: "Clone", claims: ["A007"] },
    { title: row.original.IS_ACTIVE === Constants.Yes ? "Deactive" : "Active", claims: ["A011"] },
    { title: "Delete", claims: ["A009"] },
  ],

  formInputs: [
    { type: "text", id: "ROLE_NAME", label: "Role Name" },
    {
      type: "multiselect",
      id: "CLAIM_VALUE",
      label: "Role Claims",
      optionsGetter: async () => (await Backend.call("/api/dashboard/ClaimsDropdown")).ResponseData,
    },
  ],
});

export default roleManagement;
