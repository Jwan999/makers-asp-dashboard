import Button from "./button";
import Backend from "../../utils/backend";
import AceEditor from "react-ace";
import { Dialog } from "primereact/dialog";
import { Slider } from "primereact/slider";
import { showToast } from "../../utils/toast";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { isNullOrWhiteSpace } from "../../utils/stringHelper";
import { faChevronLeft, faEdit, faEye } from "@fortawesome/free-solid-svg-icons";
import { ConfirmDialog, confirmDialog } from "primereact/confirmdialog";
import { useState, useEffect, useCallback } from "react";

import "ace-builds/src-noconflict/mode-sql";
import "ace-builds/src-noconflict/mode-html";
import "ace-builds/src-noconflict/theme-textmate";

const CodeEditor = () => {
  const [code, setCode] = useState("");
  const [fontSize, setFontSize] = useState(15);
  const [showHtmlViewer, setShowHtmlViewer] = useState(false);
  const { buttonText, language, headerTitle, codeId, dataUrl, backUrl, submitUrl } = useSelector((state) => state.codeEditor.data);

  const navigate = useNavigate();

  if (!codeId) {
    navigate("/dashboard/home", { replace: true });
  }

  const isLoading = useSelector((state) => state.dashboard.isLoading);

  const getCode = useCallback(async () => {
    const data = (await Backend.call(dataUrl, { CodeId: codeId })).ResponseData;
    setCode(data);
  }, [codeId, dataUrl]);

  useEffect(() => {
    getCode();

    document.title = headerTitle;
  }, [getCode, headerTitle]);

  const handleSubmit = () => {
    if (isNullOrWhiteSpace(code)) {
      showToast("Editor is empty, please write some code", "error");
    } else {
      confirmDialog({
        header: "Done Editing",
        dismissableMask: true,
        message: "Are you sure you that you finished editing?",
        accept: async () => {
          await Backend.call(submitUrl, {
            CodeId: codeId,
            Code: code,
          });
        },
      });
    }
  };

  return (
    <div className="content-component fade-in">
      <div className="content-component__header">
        <h4>{headerTitle}</h4>
        <div className="content-actions">
          {language === "html" ? <Button label="View" icon={faEye} disabled={isLoading} onClick={() => setShowHtmlViewer(true)} /> : null}
          <Button label={buttonText} icon={faEdit} disabled={isLoading} onClick={handleSubmit} />
          <Button
            icon={faChevronLeft}
            disabled={isLoading}
            onClick={() => {
              navigate(backUrl, { replace: true });
            }}
          />
        </div>
      </div>
      <Slider value={fontSize} onChange={(newSize) => setFontSize(newSize.value)} />
      <br />
      <AceEditor
        width="100%"
        height="80vh"
        mode={language || "text"}
        theme="textmate"
        fontSize={fontSize}
        showPrintMargin={false}
        showGutter={true}
        highlightActiveLine={true}
        value={code}
        onChange={(newCode) => {
          setCode(newCode);
        }}
        setOptions={{
          showLineNumbers: true,
          tabSize: 2,
          wrap: true,
          useWorker: false,
        }}
      />
      <ConfirmDialog />
      <Dialog
        header={<h5 className="modal-header">HTML Viewer</h5>}
        visible={showHtmlViewer}
        style={{ width: "50vw" }}
        onHide={() => setShowHtmlViewer(false)}
        closeOnEscape
        dismissableMask
      >
        <iframe title="htmlViewer" srcDoc={code} width="100%" height="400px" sandbox=""></iframe>
      </Dialog>
    </div>
  );
};

export default CodeEditor;
