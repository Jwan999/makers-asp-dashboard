import store from "../../../../store";
import Screen from "../../screenClass";
import Backend from "../../../../utils/backend";
import { showToast } from "../../../../utils/toast";

const audits = new Screen({
  screenId: "audits",

  contentHeader: "Audits",

  fetchByDate: true,
  dateFilter: "1",

  dataUrl: "/api/dashboard/getaudits",
  
  tableColumns: [
    { Header: "Username", accessor: "USERNAME" },
    { Header: "Audit Date", accessor: "AUDIT_DATE" },
    { Header: "Action Type", accessor: "ACTION_TYPE" },
    { Header: "Reference Type", accessor: "REF_TYPE" },
    { Header: "Reference Description", accessor: "REF_DESC", minWidth: 600, maxWidth: 600 },
  ],

  rowActions: (row) => [
    {
      title: "View Changes",
      handler: async ({ navigate }) => {
        const data = (await Backend.call("/api/dashboard/getauditdiffobject", { Id: row.original.ID })).ResponseData;
        let oldObj = data[0].OLD_OBJECT;
        let newObj = data[0].NEW_OBJECT;

        if (row.original.ACTION_TYPE === "INSERT") {
          oldObj = "";
        } else if (row.original.ACTION_TYPE === "DELETE") {
          oldObj = newObj;
          newObj = "";
        } else if (row.original.ACTION_TYPE !== "UPDATE") {
          showToast("No changes to show", "success");
          return;
        }

        store.dispatch({
          type: "diffViewer/setDiff",
          payload: { oldObj, newObj, headerTitle: row.original.REF_DESC, backUrl: "/dashboard/screens/audits" },
        });

        navigate("/dashboard/diffviewer", { replace: true });
      },
    },
  ],
});

export default audits;
