import Screen from "../../screenClass";
import dayjs from "dayjs";
import Backend from "../../../../utils/backend";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import Constants from "../../../../utils/constants";
import store from "../../../../store";

const startups = new Screen({
  screenId: "startups",

  contentHeader: "Startups",

  dataUrl: "api/dashboard/getstartups",
  singleItemUrl: "/api/dashboard/getstartup",
  addUrl: "/api/dashboard/addstartup",
  editUrl: "/api/dashboard/editstartup",
  deleteUrl: "/api/dashboard/deletestartup",
  changeStatusUrl: "/api/dashboard/ChangestartupStatus",

  modalWidth: "25vw",

  buttonClaimsAdd: ["A02C"],

  tableColumns: [
    { Header: "Name", accessor: "NAMEX" },

    { Header: "Start Date", accessor: "START_DATE" },
    { Header: "Location Type", accessor: "LOCATION_TYPE" },
    { Header: "Category", accessor: "CATEGORY" },
    { Header: "Description", accessor: "DESX" },
    { Header: "Has Female", accessor: "HAS_FEMALE_FOUNDER" },
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
      title: "Founders",
      handler: ({ navigate }) => {
        store.dispatch({
          type: "screenQuery/setData",
          payload: {
            dataUrl: "/api/dashboard/GetFounders",
            dataParams: { StartupId: row.original.ID },
            backUrl: "/dashboard/screens/startups",
            screenId: "founders",
          },
        });
        navigate("/dashboard/screens/founders", { replace: true });
      },
    },
    "-",
    { title: "Edit", claims: ["A02D"] },
    { title: "Clone", claims: ["A02C"] },
    "-",
    {
      title: "View Image",
      claims: ["A02B"],
      handler: async ({ setImageViewer }) => {
        var resp = (await Backend.call("/api/dashboard/getstartup", { Id: row.original.ID })).ResponseData;
        setImageViewer({
          isShown: true,
          images: resp.map((img) => {
            return {
              src: img.LOGO,
              title: <>{img.INSDATE}</>,
            };
          }),
        });
      },
    },
    {
      title: "Open Instagam Link",
      claims: ["A02B"],
      handler: async () => {
        window.open(row.original.INSTA_LINK, "_blank");
      },
    },
    {
      title: "Open Facbook Link",
      claims: ["A02B"],
      handler: async () => {
        window.open(row.original.FACEBOOK_LINK, "_blank");
      },
    },
    { title: row.original.IS_ACTIVE === Constants.Yes ? "Deactive" : "Active", claims: ["A02F"] },
    { title: "Delete", claims: ["A02E"] },
  ],

  formInputs: [
    { type: "text", id: "NAMEX", label: "Name" },
    { type: "text", id: "DESX", label: "Description" },
    { type: "calendar", id: "START_DATE", label: "Contract Start Date", value: dayjs().format("YYYY-MM-DD"), dateFormat: "YYYY-MM-DD" },
    { type: "dropdown", id: "LOCATION_TYPE", label: "Location Type", optionsGetter: () => Constants.LocationTypeDropdown },
    { type: "dropdown", id: "CATEGORY", label: "Category", optionsGetter: () => Constants.CategoryDropdown },
    { type: "dropdown", id: "HAS_FEMALE_FOUNDER", label: "Has Female Founder", optionsGetter: () => Constants.YesNoDropdown },
    { type: "text", id: "FACEBOOK_LINK", label: "Facebook Link", required: false },
    { type: "text", id: "INSTA_LINK", label: "Instagram Link", required: false },
    { type: "file", id: "LOGO", label: "Image", required: false },
  ],
});

export default startups;
