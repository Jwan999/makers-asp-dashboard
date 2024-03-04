import Jwt from "../../utils/jwt";
import Form from "../common/form";
import logo from "../../images/logo-makers.png";
import Button from "../common/button";
import Backend from "../../utils/backend";
import { Dialog } from "primereact/dialog";
import { showToast } from "../../utils/toast";
import { ValidateForm } from "../common/form";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { useEffect, useState, useCallback } from "react";
import { faTimes, faCheck } from "@fortawesome/free-solid-svg-icons";

const inputs = [
  { type: "password", id: "CurrentPassword", label: "Current Password" },
  { type: "password", id: "NewPassword", label: "New Password" },
  { type: "password", id: "PasswordConfirmation", label: "Confirm Password" },
];

const ChangePassword = () => {
  const [submitDisabled, setSubmitDisabled] = useState(false);
  const [formData, setFormData] = useState({ CurrentPassword: "", NewPassword: "", PasswordConfirmation: "" });
  const dispatch = useDispatch();

  const navigate = useNavigate();
  const isLoading = useSelector((state) => state.dashboard.isLoading);
  const toastRef = useSelector((state) => state.toast.ref);

  const getPasswordRequirements = useCallback(async () => {
    const response = await Backend.call("/api/dashboard/getpasswordrequirements");
    if (response.StatusCode !== 0) return;

    showToast(response.ResponseData, "info", { whiteSpace: "pre-line", fontSize: "0.8rem" });
  }, []);

  useEffect(() => {
    if (Jwt.getIsUsingDefaultPassword() === "true") {
      showToast("You are using the system default password", "error");
    }

    if (Jwt.getIsPasswordExpired() === "true") {
      showToast("Your password has been expired", "error");
    }

    getPasswordRequirements();
    document.title = "Change Password";
  }, [getPasswordRequirements]);

  const updateForm = (inputId, inputValue) => {
    let newData = { ...formData };
    newData[inputId] = inputValue;
    setFormData(newData);
  };

  const closeModal = () => {
    toastRef.current.clear();
    dispatch({ type: "toast/clearMessages" });
    navigate("/dashboard/home", { replace: true });
  };

  const sleep = async (time) => {
    return new Promise((resolve) => setTimeout(resolve, time));
  };

  const handleSubmit = async () => {
    var error = ValidateForm(inputs, formData);

    if (!error) {
      const responseStatus = (await Backend.call("/api/dashboard/changepassword", formData)).StatusCode;

      if (responseStatus !== 0) return;

      setSubmitDisabled(true);
      await sleep(3500);
      navigate("/login", { replace: true });
      toastRef.current.clear();
      dispatch({ type: "toast/clearMessages" });
      sessionStorage.removeItem("748784");
    } else {
      showToast(error, "error");
    }
  };

  return (
    <>
      <div className="makers-logo">
        <img src={logo} width={900} alt="Makers Logo" />
      </div>
      <Dialog
        visible={true}
        style={{ width: "25vw", height: "fit-content" }}
        onHide={() => (Jwt.getIsUsingDefaultPassword() === "true" || Jwt.getIsPasswordExpired() === "true" ? null : closeModal())}
        dismissableMask={false}
        header={<h5 className="modal-header">Change Password</h5>}
        footer={
          <div>
            <Button label="Change Password" icon={faCheck} disabled={isLoading || submitDisabled} onClick={handleSubmit} />
            {(Jwt.getIsUsingDefaultPassword() === "true" || Jwt.getIsPasswordExpired() === "true") === true ? null : (
              <Button label="Close" icon={faTimes} onClick={closeModal} />
            )}
          </div>
        }
      >
        <Form data={formData} updateForm={updateForm} inputs={inputs} />
      </Dialog>
    </>
  );
};

export default ChangePassword;
