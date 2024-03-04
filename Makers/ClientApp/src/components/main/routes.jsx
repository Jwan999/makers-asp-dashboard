import Jwt from "../../utils/jwt";
import Home from "./home";
import React from "react";
import Screen from "../screens/screen";
import Screens from "../../index";
import Notfound from "./notfound";
import DiffViewer from "../common/diffViewer";
import CodeEditor from "../common/codeEditor";
import ChangePassword from "./changePassword";
import { v4 as uuid } from "uuid";
import { Route, Routes as SwitchRoutes, Navigate, useParams } from "react-router-dom";
import Products from "../screens/data/system/products";
import Services from "../screens/data/system/services";

const Routes = () => {
  const params = useParams();

  const ScreenRoute = () => {
    const screen = Screens.find((s) => s.screenId === params["*"].split("/")[1]);
    if (screen) {
      return <Screen screen={screen.build ? screen.build() : screen} key={uuid()} />;
    } else {
      return <Navigate to="notfound" />;
    }
  };

  if (Jwt.getIsUsingDefaultPassword() === "true" || Jwt.getIsPasswordExpired() === "true") {
    return (
      <SwitchRoutes>
        <Route path="changepassword" element={<ChangePassword />} />
        <Route path="*" element={<Navigate to="changepassword" />} />
      </SwitchRoutes>
    );
  } else {
    return (
      <SwitchRoutes>
        <Route path="screens/:id" element={<ScreenRoute />} />
        <Route path="screens/products" element={<Products />} />
        <Route path="screens/services" element={<Services />} />
        <Route path="changepassword" element={<ChangePassword />} />
        <Route path="home" element={<Home />} />
        <Route path="editor" element={<CodeEditor />} />
        <Route path="diffviewer" element={<DiffViewer />} />
        <Route path="notfound" element={<Notfound />} />
        <Route path="*" element={<Navigate to="notfound" replace />} />
      </SwitchRoutes>
    );
  }
};

export default Routes;
