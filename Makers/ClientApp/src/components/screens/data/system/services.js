import React, { useEffect, useState } from "react";
import Backend from "../../../../utils/backend";
import { Card, Grid } from "@tremor/react";
import logo from "../../../../images/logo-makers.png";
import Button from "../../../common/button";
import searchIcon from "../../../../images/searchIcon.png";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircleXmark } from "@fortawesome/free-solid-svg-icons";
import ServicesModal, { ServicesDeleteModal, ServicesDownloadModal } from "../../../common/servicesModal";
const Services = () => {
  const [displayCount, setDisplayCount] = useState(12);
  const [search, setSearch] = useState("");
  const inputRef = React.useRef();
  const handleLoadMore = () => {
    setDisplayCount((prevCount) => prevCount + 12); // Increase display count by 12
  };
  const [data, setData] = useState([]);
  const getData = async () => {
    await Backend.call("/api/dashboard/GetServices").then((res) => {
      console.log(res);
      setData(res.ResponseData);
    });
  };
  useEffect(() => {
    getData();
    const handleKeyPress = (event) => {
      if (event.key === "/") {
        inputRef.current.focus();
        event.preventDefault();
      }
    };
    document.addEventListener("keydown", handleKeyPress);
    return () => {
      document.removeEventListener("keydown", handleKeyPress);
    };
  }, []);

  return (
    <div className="fade-in bg-gray-200 h-screen w-full overflow-x-scroll lg:overflow-x-hidden">
      <div className="content-component__header">
        <div className="flex justify-center items-center">
          <h4 className="text-orange font-semibold" style={{ textAlign: "center" }}>
            Services
          </h4>
          <div className="relative flex items-center text-gray-400 focus-within:text-gray-600 pl-5">
            <img src={searchIcon} alt="" className="h-4 w-4 absolute ml-3 pointer-events-none" />
            <input
              className=" px-10 bg-gray-200  placeholder-gray-400 text-black rounded-xl  "
              value={search}
              placeholder="Press / to search"
              onChange={(e) => setSearch(e.target.value)}
              ref={inputRef}
            />
            <FontAwesomeIcon icon={faCircleXmark} className="absolute h-4 w-4 mr-3 right-0" onClick={() => setSearch("")} />
          </div>
        </div>
        <div className="content-actions">
          <div className="flex justify-end items-center space-x-2">
            <ServicesModal
              name={""}
              overview={""}
              price={""}
              id={""}
              title={"Add Service"}
              label="Add Service"
              endpoint={"addService"}
              formInputs={[
                { type: "text", id: "NAMEX", label: "Name" },
                { type: "text", id: "OVERVIEW", label: "Overview" },
                { type: "text", id: "PRICEX", label: "Price", dataType: "number", required: false },
                { type: "section", id: "Sub", label: "Images" },
                { type: "file", id: "IMG", label: "Image", required: false },
                // { type: "section", id: "Sub", label: "Files" },
                // { type: "file", id: "FILEX", label: "Files", required: false },
              ]}
            />
          </div>
        </div>
      </div>
      <Grid numItemsLg={4} numItemsSm={2} className="gap-4 p-5">
        {data
          .filter((item) => item.NAMEX.toLowerCase().includes(search.toLowerCase()))
          .slice(0, displayCount)
          .map((item, index) => {
            return (
              <Card key={index} className="" decoration="top" decorationColor="amber">
                <div className="flex justify-evenly w-full items-center">
                  <img src={item.IMG} alt="service" className="w-40 h-32 object-cover" />
                  <div>
                    <h1 className="text-2xl font-bold">{item.NAMEX}</h1>
                    <p className="text-gray-400">{item.OVERVIEW}</p>
                    <p className=" font-bold">Price: {item.PRICEX}</p>
                  </div>
                </div>
                <div className="w-full flex justify-end space-x-2">
                  <ServicesDeleteModal Id={item.ID} />
                  {/* <ServicesDownloadModal Id={item.ID} /> */}
                  <ServicesModal
                    name={item.NAMEX}
                    overview={item.OVERVIEW}
                    price={item.PRICEX}
                    id={item.ID}
                    img={item.IMG}
                    // file={item.FILEX}
                    title={"Edit Service"}
                    label={"Edit"}
                    endpoint={"EditService"}
                    formInputs={[
                      { type: "text", id: "NAMEX", label: "Name" },
                      { type: "text", id: "OVERVIEW", label: "Overview" },
                      { type: "text", id: "PRICEX", label: "Price", dataType: "number", required: false },
                      { type: "section", id: "Sub", label: "Images" },
                      { type: "file", id: "IMG", label: "Image", required: false },
                      // { type: "section", id: "Sub", label: "Files" },
                      // { type: "file", id: "FILEX", label: "Files", required: false },
                    ]}
                  />
                </div>
              </Card>
            );
          })}
      </Grid>
      {displayCount < data.length && <Button onClick={handleLoadMore}>Load More</Button>}
    </div>
  );
};

export default Services;
