import Screen from "../../screenClass";
const logs = new Screen({
  screenId: "logs",

  contentHeader: "Logs",

  fetchByDate: true,
  dateFilter: "1",

  dataUrl: "/api/dashboard/getlogs",

  tableColumns: [
    { Header: "Date", accessor: "LOG_DATE", minWidth: 150, maxWidth: 150 },
    { Header: "Message", accessor: "MSG" },
  ],
});

export default logs;
