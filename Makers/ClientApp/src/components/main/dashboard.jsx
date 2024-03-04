import Routes from "./routes";
import Navbar from "../common/navigationBar";
import { useNavigate } from "react-router-dom";
import { useEffect } from "react";
import Header from "./header";

const Dashboard = () => {
  const navigate = useNavigate();

  const handleLogOut = () => {
    navigate("/login", { replace: true });
    sessionStorage.removeItem("748784");
  };

  useEffect(() => {
    if (!sessionStorage.getItem("748784")) {
      navigate("/login", { replace: true });
    }
  }, [navigate]);

  return (
    <div className="flex w-full h-full ">
      <Navbar logout={handleLogOut} />
      {/* <Header /> */}
      <Routes />
    </div>
  );
};

export default Dashboard;
