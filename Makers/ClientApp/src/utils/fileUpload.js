import Backend from "./backend";
import { showToast } from "./toast";
import { confirmDialog } from "primereact/confirmdialog";

const fileUpload = ({ url, fileId, isMultipleUpload, additionalData, callback, showProgress = false, setProgressBar }) => {
  const fileSelector = document.createElement("input");
  fileSelector.setAttribute("type", "file");
  let files = [];

  if (isMultipleUpload) {
    fileSelector.setAttribute("multiple", "true");
  }

  fileSelector.onchange = (e) => {
    var fileObjects = [];

    for (var i = 0; i < e.target.files.length; i++) {
      fileObjects.push(e.target.files[i]);
    }

    if (fileObjects.length === 0) {
      showToast("Please choose a file", "error");
      return;
    }

    const data = new FormData();

    if (additionalData) {
      data.append("reqBody", JSON.stringify(additionalData));
    }

    fileObjects.forEach((file) => {
      data.append("files", file, file.name);
      files.push(file.name);
    });

    confirmDialog({
      header: "Upload " + fileId + " Files",
      dismissableMask: true,
      message: "Are you sure you want to upload these " + fileId + " files?",
      accept: async () => {
        if (showProgress) {
          await Backend.connect(url, "File Upload", setProgressBar, data);
        } else {
          await Backend.call(url, data);
        }
        callback();
      },
    });
  };

  return fileSelector.click();
};

export default fileUpload;
