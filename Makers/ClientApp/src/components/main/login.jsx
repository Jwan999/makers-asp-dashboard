import Jwt from "../../utils/jwt";
import Form from "../common/form";
import logo from "../../images/logo-Makers2-login.png";
import parser from "ua-parser-js";
import Button from "../common/button";
import Backend from "../../utils/backend";
import { Dialog } from "primereact/dialog";
import { useState, useEffect } from "react";
import { showToast } from "../../utils/toast";
import { ValidateForm } from "../common/form";
import { InputText } from "primereact/inputtext";
import { v4 as uuid } from "uuid";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { Card } from "@tremor/react";
const otpInputs = [{ type: "text", id: "OtpValue", label: "OTP" }];
const loginInputs = [
  { type: "text", id: "username", label: "Username" },
  { type: "text", id: "password", label: "Password" },
];

const ua = new parser();

const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [otpId, setOtpId] = useState("");
  const [otpFormData, setOtpFormData] = useState({ OtpValue: "" });
  const [isModalVisable, setIsModalVisable] = useState(false);
  const [isResendDisabled, setIsResendDisabled] = useState(false);
  const [resendTimer, setResendTimer] = useState(30);
  const [isOtpSubmitDisabled, setIsOtpSubmitDisabled] = useState(false);

  const dispatch = useDispatch();
  const navigate = useNavigate();
  const isLoading = useSelector((state) => state.dashboard.isLoading);
  const toastRef = useSelector((state) => state.toast.ref);

  const userAgentResults = ua.getResult();
  const browser = userAgentResults.browser.name + " " + userAgentResults.browser.version;
  const os = userAgentResults.os.name + " " + userAgentResults.os.version;

  var browserId = localStorage.getItem("BrowserId");

  if (!browserId) {
    browserId = uuid();
    localStorage.setItem("BrowserId", browserId);
  }

  useEffect(() => {
    let storedUsername = localStorage.getItem("Username");
    if (storedUsername) {
      setUsername(storedUsername);
    }

    document.title = "Makers";
  }, []);

  const updateOtpForm = (inputId, inputValue) => {
    let newData = { ...otpFormData };
    newData[inputId] = inputValue;
    setOtpFormData(newData);
  };

  const handleLogin = async (e) => {
    e.preventDefault();
    var error = ValidateForm(loginInputs, { username: username, password: password });
    if (error) {
      showToast(error, "error");
      return;
    }
    const resp = await Backend.call("/api/dashboard/login", {
      Username: username.trim(),
      Password: password,
      Browser: browser,
      Os: os,
      BrowserId: browserId,
    });

    if (resp.StatusCode === 0) {
      const respData = resp.ResponseData;

      if (respData.OtpId) {
        setIsModalVisable(true);
        setUsername(username);
        setOtpId(respData.OtpId);
        showToast(
          "To help keep your account safe, Makers wants to make sure that its really you trying to login. To continue, please enter the OTP sent to your email.",
          "info"
        );
        return;
      } else {
        localStorage.setItem("Username", username.trim());
        sessionStorage.setItem("748784", respData.JWT);
      }

      if (Jwt.getIsUsingDefaultPassword() === "true" || Jwt.getIsPasswordExpired() === "true") {
        navigate("/dashboard/changepassword", { replace: true });
      } else {
        let passExpDate = Jwt.getPasswordExpirationDate();

        if (passExpDate) {
          showToast(`Your password will be expired at: ${passExpDate}`, "warn");
        }

        navigate("/dashboard/screens/dashboard", { replace: true });
      }
    }
  };

  const handleOtpSubmit = async () => {
    var error = ValidateForm(otpInputs, { OtpValue: otpFormData.OtpValue });

    if (error) {
      showToast(error, "error");
      return;
    }
    const resp = await Backend.call("/api/dashboard/verifyotp", {
      Id: otpId,
      OtpValue: otpFormData.OtpValue,
      Os: os,
      Browser: browser,
      BrowserId: browserId,
    });

    if (resp.StatusCode === -1) {
      setIsOtpSubmitDisabled(true);
      window.location.replace("/login");
      return;
    } else if (resp.StatusCode === 0) {
      const respData = resp.ResponseData;

      localStorage.setItem("Username", username.trim());
      sessionStorage.setItem("748784", respData.JWT);

      if (Jwt.getIsUsingDefaultPassword() === "true" || Jwt.getIsPasswordExpired() === "true") {
        navigate("/dashboard/changepassword", { replace: true });
      } else {
        navigate("/dashboard/screens/dashboard", { replace: true });
      }

      toastRef.current.clear();
      dispatch({ type: "toast/clearMessages" });
    }
  };

  const handleResendOtp = async () => {
    setIsResendDisabled(true);
    setResendTimer(30);
    await Backend.call("/api/dashboard/resendotp", { Id: otpId });
    const interval = setInterval(() => {
      setResendTimer((prevTimer) => prevTimer - 1);
    }, 1000);
    setTimeout(() => {
      setIsResendDisabled(false);
      clearInterval(interval);
    }, 30000);
  };

  return (
    <div className="flex justify-center h-screen w-screen bg-gray-200">
      <div className="flex-col justify-center items-center lg:min-w-[300px] lg:max-w-[400px] h-fit w-[80vw] pt-20">
        <h2 className="text-center pb-5 text-gray-600 font-bold">Makers</h2>
        <Card className=" w-full h-full flex-col justify-center items-center pb-10 py-20">
          <h3 className="text-center text-gray-500">Welcome Back!</h3>
          <div className="border-b-2 border-gray-400 w-36 m-auto pt-5" />
          <form onSubmit={handleLogin} className="flex-col justify-center items-center ">
            <div className="flex-col ">
              <p className="font-bold text-lg">Username</p>
              <input
                className="w-full p-2 bg-gray-200  placeholder-gray-400 text-black rounded-xl  "
                id="username"
                value={username}
                placeholder="Username"
                type="text"
                onChange={(e) => setUsername(e.target.value)}
              />
            </div>
            <div className="flex-col  py-5">
              <p className="font-bold text-lg">Password</p>
              <input
                className="w-full p-2 bg-gray-200  placeholder-gray-400 text-black rounded-xl  "
                id="password"
                value={password}
                placeholder="Password"
                type="password"
                autoFocus={true}
                onChange={(e) => setPassword(e.target.value)}
                onKeyDown={(e) => {
                  if (e.getModifierState("CapsLock")) {
                    dispatch({ type: "toast/clearMessages" });
                    showToast("Capslock is on!", "warn");
                  }
                }}
              />
            </div>
            <div className=" text-center mt-3">
              <button disabled={isLoading} type="submit" className="w-full bg-[#cf612e] text-white rounded-xl py-2 text-lg flex justify-center items-center">
                {isLoading ? <div className="loader small"></div> : "Login"}
              </button>
            </div>
          </form>
        </Card>
      </div>

      <Dialog
        visible={isModalVisable}
        closable={false}
        dismissableMask={false}
        style={{ width: "35vw", height: "fit-content" }}
        header={<h5 className="modal-header">Login Authentication</h5>}
        footer={
          <div>
            <Button
              label={isResendDisabled ? `Resend again in ${resendTimer}` : "Resend OTP"}
              disabled={isLoading || isResendDisabled}
              onClick={handleResendOtp}
            />
            <Button label="Submit" disabled={isLoading || isOtpSubmitDisabled} onClick={handleOtpSubmit} />
          </div>
        }
      >
        <Form data={otpFormData} updateForm={updateOtpForm} inputs={otpInputs} />
      </Dialog>
    </div>
  );
};

export default Login;
