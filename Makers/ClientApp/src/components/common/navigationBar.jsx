import React, { useState, useEffect, useRef } from "react";
import { Dialog } from "primereact/dialog";
import { showToast } from "../../utils/toast";
import { useLocation } from "react-router-dom";
import { Link, useNavigate } from "react-router-dom";
import { saveLocalStorageItem } from "../../utils/localStorageHelper";
import { useDispatch, useSelector } from "react-redux";
import { BiCog } from "react-icons/bi";

import Jwt from "../../utils/jwt";
import logo from "../../images/logo-Makers2.png";
import Form, { ValidateForm } from "./form";
import Button from "./button";
import navData from "../../utils/navData";
import Constants from "../../utils/constants";
import { Menu, SidebarContent, ProSidebar, SidebarHeader } from "react-pro-sidebar";

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faAngleRight,
  faBuilding,
  faCalculator,
  faChartSimple,
  faClipboardList,
  faCode,
  faCogs,
  faComputer,
  faCreditCard,
  faDatabase,
  faDiagramProject,
  faEraser,
  faFax,
  faFingerprint,
  faIdCard,
  faKey,
  faLayerGroup,
  faListCheck,
  faMoneyBills,
  faPersonCircleExclamation,
  faPhoneFlip,
  faPowerOff,
  faRankingStar,
  faScaleBalanced,
  faServer,
  faShieldHalved,
  faShop,
  faTimes,
  faUserTie,
  faUsers,
  faCircleExclamation,
} from "@fortawesome/free-solid-svg-icons";
import { FaAnglesRight, FaAnglesLeft } from "react-icons/fa6";
const Navbar = ({ logout }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const dispatch = useDispatch();
  const isLoading = useSelector((state) => state.dashboard.isLoading);
  const passExpDate = Jwt.getPasswordExpirationDate();

  const [modal, setModal] = useState({
    title: null,
    visible: false,
    initialData: null,
    inputs: null,
    width: "35vw",
    buttonText: "Submit",
    buttonIcon: null,
    submit: null,
    hasSubmitButton: false,
    id: null,
    hasClearButton: false,
    body: null,
  });

  const handleModalSubmit = () => {
    var error = ValidateForm(modal.inputs, modal.data);

    if (!error) {
      if (modal.submit) {
        modal.submit();
        return;
      }
    } else {
      showToast(error.details[0].message, "error");
      return;
    }

    saveLocalStorageItem({ data: modal.data, itemId: modal.id, localStorageKey: "search-params" });

    const currentLocation = location.pathname.includes("dataViewer") ? "/dashboard/home" : location.pathname;

    dispatch({
      type: "dataViewer/setData",
      payload: { dataUrl: modal.dataUrl, dataParams: modal.data, headerTitle: modal.title, backUrl: currentLocation },
    });

    navigate("/dashboard/screens/dataViewer", { replace: true });
    setModal({ ...modal, visible: false });
  };

  const handleShowModal = async (newModal) => {
    var updatedFormInputs = Object.assign([], newModal.inputs);

    const optionsPromises = [];

    for (let i = 0; i < updatedFormInputs.length; i++) {
      if (["dropdown", "multiselect", "autocomplete"].includes(updatedFormInputs[i].type)) {
        optionsPromises.push(updatedFormInputs[i].optionsGetter(newModal.data));
      }
    }

    const resolvedPromises = await Promise.all(optionsPromises);

    let optionsIndex = 0;
    updatedFormInputs.forEach((formInput) => {
      if (["dropdown", "multiselect", "autocomplete"].includes(formInput.type)) {
        formInput.options = resolvedPromises[optionsIndex];
        optionsIndex++;
      }
    });

    setModal(newModal);
  };

  const handleFormUpdate = (id, value) => {
    let newData = { ...modal.data };
    newData[id] = value;
    setModal({ ...modal, data: newData });
  };

  const handleClearForm = () => {
    let modalData = { ...modal.data };
    Object.getOwnPropertyNames(modalData).forEach((p) => {
      modalData[p] = "";
    });
    setModal({ ...modal, data: modalData });
  };

  const handleCloseModal = () => {
    setModal({ ...modal, visible: false });
  };
  const [collapsed, setCollapsed] = useState(false);

  return (
    <div className="h-screen">
      <ProSidebar collapsed={collapsed} width={190}>
        <SidebarHeader className="text-center">
          {collapsed ? (
            <div className="cursor-pointer" onClick={() => setCollapsed(!collapsed)}>
              <p className="text-2xl">Makers</p>
            </div>
          ) : (
            <div className="cursor-pointer" onClick={() => setCollapsed(!collapsed)}>
              <p className="text-3xl">Makers</p>
            </div>
          )}
          {/* <img src={logo} width="135" height="32" alt="Logo" /> */}
        </SidebarHeader>
        {/* {useSelector((state) => state.dashboard.isLoading) ? <div className="loader"></div> : <div style={{ width: "24px", height: "24px" }}></div>} */}

        <SidebarContent className="space-y-5">
          <Menu iconShape="square" className="space-y-5">
            {navData.map(
              (menu, index) =>
                (!menu.position || menu.position === "left") && (
                  <MenuItem item={menu} key={index} depthLevel={0} modal={modal} showModal={handleShowModal} collapsed={collapsed} />
                )
            )}
          </Menu>
        </SidebarContent>
        {/* <span className="logged-in-as">
          Logged in as <span className="themed-element">{Jwt.getUsername()}</span> at{" "}
          <FontAwesomeIcon className="themed-element" icon={GetDepartmentIcon(Jwt.getDepartment())} />
          <br />
          Last login {Jwt.getLastLoginDate()}
        </span>
        {navData.map(
          (menu, index) => menu.position === "right" && <MenuItem item={menu} key={index} depthLevel={0} modal={modal} showModal={handleShowModal} />
        )}
        <div className={passExpDate ? "notification-container" : null}>
          <MenuItem item={{ icon: faKey, path: "/dashboard/changepassword", title: "Change Password" }} />
          {passExpDate ? <FontAwesomeIcon className="notification-bubble" size="sm" icon={faCircleExclamation} /> : null}
        </div>
        <MenuItem item={{ icon: faComputer, path: "/dashboard/screens/userdevices", title: "Trusted Devices" }} />
        <MenuItem item={{ icon: faPowerOff, path: "/home", title: "Log out", onClick: logout }} /> */}
        <MenuItem item={{ icon: faPowerOff, path: "/home", title: collapsed ? "" : "Log out", onClick: logout }} />
        <Dialog
          visible={modal.visible}
          style={{ width: modal.width }}
          header={<h5 className="modal-header">{modal.title}</h5>}
          onHide={() => setModal({ ...modal, visible: false })}
          footer={
            <>
              {modal.hasSubmitButton && <Button label={modal.buttonText} icon={modal.buttonIcon} disabled={isLoading} onClick={handleModalSubmit} />}
              {modal.hasClearButton && <Button label="Clear" icon={faEraser} onClick={handleClearForm} />}
              <Button label="Close" icon={faTimes} className="p-button-danger" onClick={handleCloseModal} />
            </>
          }
        >
          {modal.body ? modal.body() : <Form inputs={modal.inputs} data={modal.data} updateForm={handleFormUpdate} />}
        </Dialog>
      </ProSidebar>
    </div>
  );
};

const MenuItem = ({ item, depthLevel, showModal, collapsed }) => {
  const [isActive, setIsActive] = useState(false);

  let ref = useRef();

  useEffect(() => {
    const handler = (event) => {
      if (isActive && ref.current && !ref.current.contains(event.target)) {
        setIsActive(false);
      }
    };
    document.addEventListener("mousedown", handler);
    document.addEventListener("touchstart", handler);
    return () => {
      document.removeEventListener("mousedown", handler);
      document.removeEventListener("touchstart", handler);
    };
  }, [isActive]);

  const onMouseEnter = () => {
    setIsActive(true);
  };

  const onMouseLeave = () => {
    setIsActive(false);
  };

  let isAuthorized = false;

  if (item.submenu && !item.noAuth) {
    for (let i = 0; i < item.submenu.length; i++) {
      if (item.submenu[i].authList && Jwt.isAuthorized(item.submenu[i].authList)) {
        isAuthorized = true;
        break;
      }
      if (item.submenu[i].submenu) {
        for (let j = 0; j < item.submenu[i].submenu.length; j++) {
          if (item.submenu[i].submenu[j].authList && Jwt.isAuthorized(item.submenu[i].submenu[j].authList)) {
            isAuthorized = true;
            break;
          }
        }
      }
    }
  } else if (item.noAuth) {
    isAuthorized = true;
  } else {
    isAuthorized = Jwt.isAuthorized(item.authList);
  }

  return isAuthorized ? (
    <li className={`menu__item ${collapsed ? "py-2" : ""}`} ref={ref} onMouseEnter={onMouseEnter} onMouseLeave={onMouseLeave}>
      {item.submenu ? (
        <>
          <button className="menu__button" type="button" aria-haspopup="menu" aria-expanded={isActive ? "true" : "false"} title={item.title}>
            <div>
              {item.icon ? <FontAwesomeIcon size="2x" icon={item.icon} /> : item.title}
              {depthLevel > 0 && <FontAwesomeIcon icon={faAngleRight} />}
            </div>
          </button>
          <ul className={`dropdown-menu ${depthLevel + 1 > 1 ? "dropdown-menu--submenu" : ""} ${isActive ? "dropdown-menu--active" : ""}`}>
            {item.submenu.map((submenu, index) => (
              <MenuItem item={submenu} key={index} depthLevel={depthLevel + 1} showModal={showModal} />
            ))}
          </ul>
        </>
      ) : (
        <span
          className="row"
          onClick={() => {
            if (item.onClick) {
              item.onClick(showModal);
            }
          }}
        >
          {item.path ? (
            <Link to={item.path || ""} replace={true} className="menu__link" title={item.icon ? item.title : ""}>
              <div style={{ padding: "1%", display: "flex", alignItems: "center" }}>
                <FontAwesomeIcon size="2x" icon={item.icon} color="#cf612e" style={{ width: "25px" }} />
                <label style={{ marginLeft: "10%", color: "white", fontWeight: "bold", marginTop: "5px" }}>
                  <span style={{ fontSize: "15px" }}>{collapsed ? "" : item.title}</span>
                </label>
              </div>
            </Link>
          ) : (
            <div className="menu__link">
              <FontAwesomeIcon size="2x" icon={item.icon} />
              <span>{item.title}</span>
            </div>
          )}
        </span>
      )}
    </li>
  ) : null;
};

const GetDepartmentIcon = (department) => {
  if (department === Constants.DepartmentOperations || department === Constants.DepartmentServices) {
    return faLayerGroup;
  } else if (department === Constants.DepartmentITOperations) {
    return faCogs;
  } else if (department === Constants.DepartmentTerminals) {
    return faFax;
  } else if (department === Constants.DepartmentAccounting) {
    return faCalculator;
  } else if (department === Constants.DepartmentIT) {
    return faServer;
  } else if (department === Constants.DepartmentManagement) {
    return faUserTie;
  } else if (department === Constants.DepartmentSecretary) {
    return faClipboardList;
  } else if (department === Constants.DepartmentRiskManagement) {
    return faPersonCircleExclamation;
  } else if (department === Constants.DepartmentDBA) {
    return faDatabase;
  } else if (department === Constants.DepartmentDataManagement) {
    return faChartSimple;
  } else if (department === Constants.DepartmentPMO) {
    return faDiagramProject;
  } else if (department === Constants.DepartmentMarketing) {
    return faRankingStar;
  } else if (department === Constants.DepartmentCallCenter) {
    return faPhoneFlip;
  } else if (department === Constants.DepartmentFraud) {
    return faShieldHalved;
  } else if (department === Constants.DepartmentHR) {
    return faUsers;
  } else if (department === Constants.DepartmentLegal) {
    return faScaleBalanced;
  } else if (department === Constants.DepartmentSales) {
    return faMoneyBills;
  } else if (department === Constants.DepartmentMerchantManagement) {
    return faShop;
  } else if (department === Constants.DepartmentAML) {
    return faIdCard;
  } else if (department === Constants.DepartmentAdministration) {
    return faBuilding;
  } else if (department === Constants.DepartmentInformationSecurity) {
    return faFingerprint;
  } else if (department === Constants.DepartmentPersonalizationCenter) {
    return faCreditCard;
  } else if (department === Constants.DepartmentCompliance) {
    return faListCheck;
  } else if (department === Constants.DepartmentKeysManagement) {
    return faKey;
  } else {
    return faCode;
  }
};

export default Navbar;
