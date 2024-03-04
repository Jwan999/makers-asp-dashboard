import Jwt from "./jwt";
import store from "../store";
import Constants from "./constants";
import { showToast } from "./toast";
import * as signalR from "@microsoft/signalr";

class Backend {
  static async connect(url, operation, setProgressBar, data = {}, options = { isBlob: false }) {
    setProgressBar({ isShown: true, header: operation, status: "Connecting...", message: "", progrees: 0 });

    let connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.None)
      .withUrl(`/websockethub?access_token=${sessionStorage.getItem("748784")}`)
      .build();

    try {
      connection.on("UpdateProgress", (status, message, progress) => {
        setProgressBar({ isShown: true, header: operation, status, message, progress });
      });

      // Server sends heartbeat every 15 seconds. We close the connection if no message is received in 30 seconds.
      connection.serverTimeoutInMilliseconds = 30000;

      await connection.start();

      if (data instanceof FormData) {
        data.set("reqBody", JSON.stringify({ ConnectionId: connection.connection.connectionId, ...JSON.parse(data.get("reqBody")) }));
      } else {
        data["ConnectionId"] = connection.connection.connectionId;
      }

      if (operation === Constants.BackendConnectionOperationFileUpload) {
        setProgressBar({ isShown: true, header: operation, status: "Uploading...", message: "", progress: 0 });
      }

      var response = await Backend.call(url, data, options);

      await connection.stop();

      setTimeout(() => {
        setProgressBar({ isShown: false, header: operation, status: "", message: "", progrees: 0 });
      }, 1000);

      delete data["ConnectionId"];

      return response;
    } catch (e) {
      setProgressBar({ isShown: false, header: operation, status: "", message: "", progrees: 0 });
      console.log(e);
      delete data["ConnectionId"];
      await connection.stop();
    }
  }

  static async call(url, data = {}, options = { isBlob: false }) {
    if (!url) return;

    const headers = new Headers({
      Authorization: `Bearer ${sessionStorage.getItem("748784")}`,
      DFP: Jwt.getIsUsingDefaultPassword(),
      PWEXP: Jwt.getIsPasswordExpired(),
    });

    const isFormData = data instanceof FormData;

    const body = isFormData ? data : JSON.stringify(data);

    if (!isFormData) {
      headers.append("Content-Type", "application/json");
    }

    try {
      store.dispatch({ type: "dashboard/addUrl", payload: url });

      if (!store.getState().dashboard.isLoading) {
        store.dispatch({ type: "dashboard/setIsLoading", payload: true });
      }

      const response = await fetch(url, {
        method: "POST",
        headers,
        body,
      });

      store.dispatch({ type: "dashboard/removeUrl", payload: url });

      if (store.getState().dashboard.requestUrls.length === 0) {
        store.dispatch({ type: "dashboard/setIsLoading", payload: false });
      }

      const refreshedtoken = response.headers.get("refreshedtoken");

      if (refreshedtoken) {
        sessionStorage.setItem("748784", refreshedtoken);
      }

      if (response.status === 200) {
        if (options.isBlob) {
          let fileName = "";
          try {
            response.headers
              .get("content-disposition")
              .split(";")
              .forEach((e) => {
                if (e.trim().startsWith("filename=")) {
                  fileName = e.trim().split("=")[1].replaceAll('"', "");
                }
              });
          } catch (e) {
            console.log(e);
          }

          const blob = new Blob([await response.blob()]);

          const a = document.createElement("a");
          const url = URL.createObjectURL(blob);

          a.href = url;
          a.download = fileName || "INVALID_FILE";
          document.body.appendChild(a);
          a.click();

          setTimeout(() => {
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);
          }, 0);

          return;
        }

        const responseData = await response.json();

        if (responseData.StatusCode === 0) {
          if (responseData.StatusDescription) showToast(responseData.StatusDescription, "success");
          return responseData;
        } else {
          showToast(responseData.StatusDescription, "error");
          return responseData;
        }
      } else if (response.status === 401) {
        window.location.replace("/login");
      } else {
        showToast(response.statusText, "error");
      }
    } catch (error) {
      store.dispatch({ type: "dashboard/removeUrl", payload: url });
      if (store.getState().dashboard.requestUrls.length === 0) {
        store.dispatch({ type: "dashboard/setIsLoading", payload: false });
      }
      console.log(error);
    }
  }
}

export default Backend;
