import logo from "../../images/logo-makers.png";
import { useEffect } from "react";
import Header from "./header";

const Home = () => {
  useEffect(() => {
    document.title = "Home";
  }, []);

  return (
    <div className="">
      <div className="">{/* <Header /> */}</div>
    </div>
  );
};

export default Home;
