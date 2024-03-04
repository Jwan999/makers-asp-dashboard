import Screen from "../../screenClass";
import dayjs from "dayjs";
import Backend from "../../../../utils/backend";
import Constants from "../../../../utils/constants";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";

const training = new Screen({
  screenId: "training",

  contentHeader: "Training",

  dataUrl: "/api/dashboard/gettrainings",
  singleItemUrl: "/api/dashboard/gettraining",
  addUrl: "/api/dashboard/addtraining",
  editUrl: "/api/dashboard/edittraining",
  deleteUrl: "/api/dashboard/deletetraining",
  changeStatusUrl: "/api/dashboard/ChangetrainingStatus",

  modalWidth: "25vw",

  buttonClaimsAdd: ["A020"],

  tableColumns: [
    { Header: "Name", accessor: "NAMEX" },
    { Header: "Type", accessor: "TYPEX", maxWidth: 60, minWidth: 60 },
    { Header: "Start Date", accessor: "START_DATE" },
    { Header: "End Date", accessor: "END_DATE" },
    {
      Header: "Attendence Type",
      accessor: "ATTENDANCE_TYPE",
      optionsGetter: async () => (await Backend.call("/api/dashboard/DictDropdown", { Key: Constants.DictKeyAttendenceType })).ResponseData,
    },
    { Header: "Total Hours", accessor: "HOURSX", maxWidth: 60, minWidth: 60 },
    { Header: "Number of Lectures", accessor: "LEC_NUM", maxWidth: 100, minWidth: 100 },
    {
      Header: "Is Paid",
      accessor: "IS_PAID",
      maxWidth: 60,
      minWidth: 60,
      Cell: ({ cell }) => (
        <FontAwesomeIcon color={cell.row.original.IS_PAID === "Y" ? "Red" : "Green"} icon={cell.row.original.IS_PAID === "Y" ? faBan : faCheckCircle} />
      ),
    },
    { Header: "Price in IQD", accessor: "PRICEX", maxWidth: 60, minWidth: 60 },
    { Header: "Sponsoed By / Funded By", accessor: "INST", maxWidth: 120, minWidth: 120 },
    { Header: "Project", accessor: "PROJECT", maxWidth: 120, minWidth: 120 },
    { Header: "Traing Days", accessor: "TRAINING_DAYS" },
    { Header: "Trainers", accessor: "TRAINERS" },
    { Header: "Progress", accessor: "PROGRESS" },
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
    { title: "Edit", claims: ["A021"] },
    { title: "Clone", claims: ["A020"] },
    { title: row.original.IS_ACTIVE === Constants.Yes ? "Deactive" : "Active", claims: ["A028"] },
    { title: "Delete", claims: ["A022"] },
  ],

  formInputs: [
    { type: "text", id: "NAMEX", label: "Name" },
    {
      type: "dropdown",
      id: "TYPEX",
      label: "Type",
      optionsGetter: async () => (await Backend.call("/api/dashboard/DictDropdown", { Key: Constants.DictKeyTrainingType })).ResponseData,
    },
    {
      type: "dropdown",
      id: "PROJECT",
      label: "Project",
      optionsGetter: async () => (await Backend.call("/api/dashboard/projectDropdown")).ResponseData,
      dataType: "number",
    },
    {
      type: "dropdown",
      id: "ATTENDANCE_TYPE",
      label: "Attendence Type",
      optionsGetter: async () => (await Backend.call("/api/dashboard/DictDropdown", { Key: Constants.DictKeyAttendenceType })).ResponseData,
    },
    { type: "text", id: "HOURSX", label: "Total Hours", dataType: "number" },
    { type: "text", id: "LEC_NUM", label: "Number of Lectures", dataType: "number" },
    { type: "dropdown", id: "IS_PAID", label: "Is Paid", optionsGetter: () => Constants.YesNoDropdown },
    { type: "text", id: "PRICEX", label: "Price in IQD", dataType: "number", required: false, disabled: (formData) => formData.IS_PAID !== Constants.Yes },
    {
      type: "multiselect",
      id: "TRAINING_DAYS",
      label: "Training Days",
      optionsGetter: async () => (await Backend.call("/api/dashboard/DictDropdown", { Key: Constants.DictKeyTWeekDays })).ResponseData,
    },
    { type: "calendar", id: "START_DATE", label: "Start Date", value: dayjs().format("YYYY-MM-DD"), dateFormat: "YYYY-MM-DD" },
    { type: "calendar", id: "END_DATE", label: "End Date", value: dayjs().format("YYYY-MM-DD"), dateFormat: "YYYY-MM-DD" },
    { type: "multiselect", id: "TRAINERS", label: "Trainers", optionsGetter: async () => (await Backend.call("/api/dashboard/trainersDropdown")).ResponseData },
    {
      type: "dropdown",
      id: "PROGRESS",
      label: "Progress",
      optionsGetter: async () => (await Backend.call("/api/dashboard/dictdropdown", { Key: Constants.DictKeyTrainingProgress })).ResponseData,
    },
  ],
});

export default training;
