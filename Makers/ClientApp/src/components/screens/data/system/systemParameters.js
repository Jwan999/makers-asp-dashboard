import Backend from "../../../../utils/backend";
import { confirmDialog } from "primereact/confirmdialog";
import Screen from "../../screenClass";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import Constants from "../../../../utils/constants";

const systemParameters = new Screen({
  screenId: "systemparameters",

  contentHeader: "System Parameters",

  dataUrl: "/api/dashboard/getsystemparameters",
  singleItemUrl: "/api/dashboard/getsystemparameter",
  addUrl: "/api/dashboard/addsystemparameter",
  editUrl: "/api/dashboard/editsystemparameter",
  deleteUrl: "/api/dashboard/deletesystemparameter",

  modalWidth: "25vw",

  buttonClaimsAdd: ["A00B"],

  actions: [
    {
      title: "Reinitialize System",
      type: "button",
      claims: ["A001"],
      handler: () => {
        confirmDialog({
          header: "Reinitialize System",
          dismissableMask: true,
          message: "Are you sure you want to reinitialize the system?",
          accept: async () => {
            await Backend.call("/api/dashboard/reinit");
          },
        });
      },
    },
  ],

  tableColumns: [
    { Header: "Parameter Name", accessor: "PARAMETER_NAME" },
    { Header: "Parameter Value", accessor: "PARAMETER_VALUE" },
    { Header: "Parameter Description", accessor: "PARAMETER_DESCRIPTION" },
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
    { title: "Edit", claims: ["A00C"] },
    { title: "Clone", claims: ["A00B"] },
    { title: "Delete", claims: ["A00D"] },
  ],

  formInputs: [
    { type: "text", id: "PARAMETER_NAME", label: "Parameter Name" },
    { type: "text", id: "PARAMETER_VALUE", label: "Parameter Value" },
    { type: "text", id: "PARAMETER_DESCRIPTION", label: "Parameter Description" },
  ],
});

export default systemParameters;
