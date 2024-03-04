//import useState hook to create menu collapse state
import React, { useState } from "react";

//import react pro sidebar components
import { ProSidebar, Menu, MenuItem, SidebarHeader, SidebarFooter, SidebarContent } from "react-pro-sidebar";

//import icons from react icons
/*//import { FaList, } from "react-icons/fa";*/
import { FiHome, FiLogOut, FiArrowLeftCircle, FiArrowRightCircle } from "react-icons/fi";
// import { RiPencilLine } from "react-icons/ri";
import { BiCog } from "react-icons/bi";

//import sidebar css from react-pro-sidebar module and our custom css
import "react-pro-sidebar/dist/css/styles.css";
import { Link } from "react-router-dom";
// import "./header.css";

const Header = () => {
  //create initial menuCollapse state using useState hook
  const [menuCollapse, setMenuCollapse] = useState(false);

  //create a custom function that will change menucollapse state from false to true and true to false
  const menuIconClick = () => {
    //condition checking to change state from true to false and vice versa
    menuCollapse ? setMenuCollapse(false) : setMenuCollapse(true);
  };

  return (
    <>
      <div id="header">
        {/* collapsed props to change menu size using menucollapse state */}
        <ProSidebar collapsed={menuCollapse}>
          <SidebarHeader style={{ padding: "12%" }}>
            <div className="logotext" style={{ padding: "11%" }}>
              {/* small and big change using menucollapse state */}
              <p>{menuCollapse ? "Logo" : "Makers"}</p>
              {/* <ln></ln> */}
            </div>
            {/* <div className="closemenu" onClick={menuIconClick}> */}
            {/* changing menu collapse icon on click */}
            {/* {menuCollapse ? <FiArrowRightCircle /> : <FiArrowLeftCircle />}
            </div> */}
          </SidebarHeader>
          <SidebarContent>
            <Menu iconShape="square">
              {/* <MenuItem icon={<FaBuildingColumns />}>Institutions</MenuItem>
              <MenuItem icon={<FaBuildingColumns />}>Projects</MenuItem>
              <MenuItem icon={<RiPencilLine />}>Training</MenuItem> */}
              <Link to={"/dashboard/screens/usermanagement"}>
                <MenuItem icon={<BiCog />}>Users</MenuItem>{" "}
              </Link>

              <Link to={"/dashboard/screens/workshops"}>
                <MenuItem icon={<BiCog />}>workshops</MenuItem>{" "}
              </Link>
              <Link to={"/dashboard/screens/sessions"}>
                <MenuItem icon={<BiCog />}>Sessions</MenuItem>{" "}
              </Link>
              <MenuItem icon={<BiCog />}>Startups</MenuItem>
              <MenuItem icon={<BiCog />}>Interns</MenuItem>
              <MenuItem icon={<BiCog />}>Students</MenuItem>
              <MenuItem icon={<BiCog />}>Recommendations</MenuItem>

              <MenuItem icon={<BiCog />}>Success Stories</MenuItem>
              <MenuItem icon={<BiCog />}>Team Members</MenuItem>
            </Menu>
          </SidebarContent>
          <SidebarFooter>
            <Menu iconShape="square">
              <MenuItem icon={<FiLogOut />}>Logout</MenuItem>
            </Menu>
          </SidebarFooter>
        </ProSidebar>
      </div>
    </>
  );
};

export default Header;