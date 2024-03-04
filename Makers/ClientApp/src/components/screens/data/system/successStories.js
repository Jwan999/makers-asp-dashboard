import Screen from "../../screenClass";
import dayjs from "dayjs";
import Backend from "../../../../utils/backend";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import Constants from "../../../../utils/constants";
import store from "../../../../store";

const successstories = new Screen({
  screenId: "successstories",

  contentHeader: "Success Stories",

  dataUrl: "/api/dashboard/getsuccessstories",
  singleItemUrl: "/api/dashboard/getsuccessstory",
  addUrl: "/api/dashboard/addsuccessstory",
  editUrl: "/api/dashboard/editsuccessstory",
  deleteUrl: "/api/dashboard/deletesuccessstory",
  changeStatusUrl: "/api/dashboard/ChangesuccessstoryStatus",

  modalWidth: "25vw",

  buttonClaimsAdd: ["CO43"],

  tableColumns: [
    { Header: "Name", accessor: "NAMEX" },
    { Header: "Email", accessor: "EMAIL" },
    { Header: "Phone Number", accessor: "PHONE_NUMBER" },
    { Header: "Story", accessor: "STORY" },
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
      claims: ["CO42"],
      handler: async () => {
        window.open(row.original.LINKX, "_blank");
      },
    },
    "-",
    { title: "Edit", claims: ["CO44"] },
    { title: "Clone", claims: ["CO43"] },
    { title: row.original.IS_ACTIVE === Constants.Yes ? "Deactive" : "Active", claims: ["CO46"] },
    { title: "Delete", claims: ["CO45"] },
  ],

  formInputs: [
    { type: "text", id: "NAMEX", label: "Name" },
    { type: "text", id: "STORY", label: "Story" },
    { type: "text", id: "EMAIL", label: "Email", required: false },
    { type: "text", id: "LINKX", label: "Link", required: false },
    { type: "text", id: "PHONE_NUMBER", label: "Phone Number", required: false },
  ],
});

export default successstories;
