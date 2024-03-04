import Login from "./components/main/login";
import Dashboard from "./components/main/dashboard";
import { Toast } from "primereact/toast";
import { useDispatch } from "react-redux";
import { useEffect, useRef } from "react";
import { Route, Routes, Navigate } from "react-router-dom";

function App() {
  const toastRef = useRef(null);
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch({ type: "toast/setRef", payload: toastRef });
  }, [dispatch]);

  return (
    <>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/dashboard/*" element={<Dashboard />} />
        <Route path="/*" element={<Navigate to="/login" replace />} />
      </Routes>
      <Toast ref={toastRef} position="bottom-left" onRemove={() => dispatch({ type: "toast/clearMessages" })} />
    </>
  );
}

export default App;
