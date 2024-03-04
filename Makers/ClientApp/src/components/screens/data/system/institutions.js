import Screen from "../../screenClass";
import dayjs from "dayjs";
import Backend from "../../../../utils/backend";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import Constants from "../../../../utils/constants";

const partners = new Screen({
  screenId: "partners",

  contentHeader: "Partners",

  dataUrl: "/api/dashboard/getinstitutions",
  singleItemUrl: "/api/dashboard/getinstitution",
  addUrl: "/api/dashboard/addinstitution",
  editUrl: "/api/dashboard/editinstitution",
  deleteUrl: "/api/dashboard/deleteinstitution",
  changeStatusUrl: "/api/dashboard/ChangeInstitutionStatus",

  modalWidth: "25vw",

  buttonClaimsAdd: ["A018"],

  // actions: [
  //   {
  //     title: "Add",
  //     type: "button",
  //     claims: ["A001"],
  //     handler: ({ changeModal }) => {
  //       changeModal({
  //         title: "Print Name on Card",
  //         width: "35vw",
  //         buttonText: "Add",
  //         inputs: [
  //           { type: "text", id: "NAMEX", label: "Full Name" },
  //           { type: "calendar", id: "START_DATE", label: "Contract Start Date", value: dayjs().format("YYYY-MM-DD"), dateFormat: "YYYY-MM-DD" },
  //           { type: "file", id: "ICON", label: "Image" },
  //         ],
  //         onSubmit: async (formData, getData) => {
  //           await Backend.call("/api/operations/AddData", { reqBody: formData });
  //           const submitData = new FormData();
  //           submitData.append("reqBody", JSON.stringify({ REF_ID: row.original.ID, REF_TYPE: "INST" }));
  //           if (formData.Image) {
  //             formData.Image.forEach((f) => {
  //               submitData.append("Image", f, f.name);
  //             });
  //           }
  //           await Backend.call("/api/dashboard/UploadImage", submitData);
  //           await getData(partners.dataUrl);
  //         },
  //       });
  //     },
  //   },
  // ],
  tableColumns: [
    { Header: "Full Name", accessor: "NAMEX" },
    { Header: "Contract Start Date", accessor: "START_DATE" },
    {
      Header: "Status",
      accessor: "IS_ACTIVE",
      maxWidth: 60,
      minWidth: 0,
      Cell: ({ cell }) => (
        <FontAwesomeIcon color={cell.row.original.IS_ACTIVE === "N" ? "Red" : "Green"} icon={cell.row.original.IS_ACTIVE === "N" ? faBan : faCheckCircle} />
      ),
    },
  ],

  rowActions: (row) => [
    { title: "Edit", claims: ["A019"] },
    { title: "Clone", claims: ["A018"] },
    "-",
    {
      title: "View Image",
      claims: ["A019"],
      handler: async ({ setImageViewer }) => {
        var resp = (await Backend.call("/api/dashboard/getinstitution", { Id: row.original.ID })).ResponseData;
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
    { title: row.original.IS_ACTIVE === Constants.Yes ? "Deactive" : "Active", claims: ["A025"] },
    { title: "Delete", claims: ["A01A"] },
  ],

  formInputs: [
    { type: "text", id: "NAMEX", label: "Full Name" },
    { type: "calendar", id: "START_DATE", label: "Contract Start Date", value: dayjs().format("YYYY-MM-DD"), dateFormat: "YYYY-MM-DD" },
    { type: "file", id: "ICON", label: "Image", required: false },
  ],
});

export default partners;
