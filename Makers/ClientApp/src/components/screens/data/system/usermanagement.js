import Backend from "../../../../utils/backend";
import Screen from "../../screenClass";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import Constants from "../../../../utils/constants";
import dayjs from "dayjs";

const userManagement = new Screen({
  screenId: "usermanagement",

  contentHeader: "Users",

  dataUrl: "/api/dashboard/getusers",
  singleItemUrl: "/api/dashboard/getuser",
  addUrl: "/api/dashboard/adduser",
  editUrl: "/api/dashboard/edituser",
  deleteUrl: "/api/dashboard/deleteuser",
  changeStatusUrl: "/api/dashboard/changeuserstatus",

  modalWidth: "25vw",

  buttonClaimsAdd: ["A002"],

  tableColumns: [
    { Header: "ID", accessor: "ID", minWidt: 40, maxWidth: 40 },
    { Header: "Username", accessor: "USERNAME" },
    { Header: "Full Name", accessor: "FULL_NAME" },
    { Header: "Register Date", accessor: "INSDATE" },
    { Header: "Last Login", accessor: "LUPDATE" },
    { Header: "Phone Number", accessor: "PHONE_NUMBER" },
    { Header: "Email", accessor: "EMAIL" },
    { Header: "Gender", accessor: "GENDER" },
    { Header: "Role", accessor: "ROLE_NAME" },
    { Header: "Birthdate", accessor: "BDATE" },
    {
      Header: "Status",
      accessor: "IS_BLOCKED",
      maxWidth: 60,
      minWidth: 60,
      Cell: ({ cell }) => (
        <FontAwesomeIcon color={cell.row.original.IS_BLOCKED === "Y" ? "Red" : "Green"} icon={cell.row.original.IS_BLOCKED === "Y" ? faBan : faCheckCircle} />
      ),
    },
    { Header: "Block Date", accessor: "BLOCK_DATE" },
  ],

  rowActions: (row) => [
    {
      title: "Reset Password",
      // claims: ["CAC4"],
      handler: async ({ getData }) => {
        await Backend.call("/api/dashboard/resetpassword", { Id: row.original.ID });
        await getData(userManagement.dataUrl);
      },
    },
    "-",
    { title: "Edit", claims: ["A003"] },
    { title: "Clone", claims: ["A002"] },
    { title: row.original.IS_BLOCKED === Constants.No ? "Deactive" : "Active", claims: ["A027"] },
    { title: "Delete", claims: ["A004"] },
  ],

  formInputs: [
    { type: "text", id: "USERNAME", label: "Username" },
    { type: "text", id: "EMAIL", label: "Email" },
    { type: "text", id: "FULL_NAME", label: "Full Name" },
    { type: "text", id: "PHONE_NUMBER", label: "Phone Number" },
    { type: "dropdown", id: "GENDER", label: "Gender", optionsGetter: () => Constants.GenderDropdown },
    { type: "calendar", id: "BDATE", label: "Birthdate", value: dayjs().format("YYYY-MM-DD"), dateFormat: "YYYY-MM-DD" },
    {
      type: "dropdown",
      id: "ROLE_ID_FK",
      label: "Role",
      dataType: "number",
      optionsGetter: async () => (await Backend.call("/api/dashboard/getrolesdropdown")).ResponseData,
    },
    // {
    //   type: "multiselect",
    //   id: "SCLAIM_VALUE",
    //   label: "Special Claims",
    //   required: false,
    //   optionsGetter: async () => (await Backend.call("/api/dashboard/getallclaims")).ResponseData,
    //   disabled: (formData) => formData.ROLE_ID_FK === 0,
    //   tooltip: "These claims will be added to the user in addition to the user's role claims",
    // },
  ],
});

export default userManagement;
