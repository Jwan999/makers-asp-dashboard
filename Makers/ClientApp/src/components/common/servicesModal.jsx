import { useState } from "react";
import { Dialog, DialogPanel } from "@tremor/react";
import Button from "./button";
import Backend from "../../utils/backend";
import Form from "./form";
import { Button as ButtonX } from "@tremor/react";
import { faCheck, faTimes } from "@fortawesome/free-solid-svg-icons";
const ServicesModal = ({ id, name, overview, price, title, label, endpoint, formInputs }) => {
  const [loading, setLoading] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const [data, setData] = useState({
    NAMEX: name,
    OVERVIEW: overview,
    PRICEX: price,
    // IMG: img,
    // FILEX: file,
  });
  const handleSubmit = async () => {
    setLoading(true);
    const formData = new FormData();
    formData.append("EditEntityId", parseInt(id));
    formData.append("NAMEX", data.NAMEX);
    formData.append("OVERVIEW", data.OVERVIEW);
    formData.append("PRICEX", data.PRICEX);
    formData.append("IMG", data.IMG);
    formData.append("FILEX", data.FILEX);

    // Convert FormData to a regular JavaScript object
    const submitData = {};
    for (let [key, value] of formData.entries()) {
      submitData[key] = value;
    }
    await Backend.call("/api/dashboard/" + endpoint, submitData);
    setLoading(false);
    setIsOpen(false);
  };
  // const formInputs = [
  //   { type: "text", id: "NAMEX", label: "Name" },
  //   { type: "text", id: "OVERVIEW", label: "Overview" },
  //   { type: "text", id: "PRICEX", label: "Price", dataType: "number" },
  //   { type: "text", id: "COUNT", label: "Count", dataType: "number" },
  //   { type: "file", id: "IMG", label: "Image", required: false },
  //   { type: "file", id: "FILEX", label: "Files", required: false },
  // ];
  function updateForm(inputId, inputValue) {
    let newFormData = Object.assign({}, data);
    newFormData[inputId] = inputValue;
    setData(newFormData);
  }
  return (
    <div>
      <Button label={label} onClick={() => setIsOpen(true)} />
      <Dialog open={isOpen} onClose={(val) => setIsOpen(val)} static={true}>
        <DialogPanel>
          <div>
            <Form data={data} inputs={formInputs} updateForm={updateForm} isEditMode={true} />
          </div>
          <div className="flex justify-end items-center space-x-3 pt-3">
            <Button label="Close" icon={faTimes} className="p-button-primary" onClick={() => setIsOpen(false)} />
            <Button
              label={label}
              icon={faCheck}
              disabled={loading}
              onClick={() => {
                handleSubmit();
              }}
            />
          </div>
        </DialogPanel>
      </Dialog>
    </div>
  );
};

export default ServicesModal;

export const ServicesDeleteModal = ({ Id }) => {
  const [loading, setLoading] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const handleSubmit = async () => {
    setLoading(true);
    await Backend.call(`/api/dashboard/deleteservice`, { Id: parseInt(Id) }).then((res) => {
      console.log(res);
    });

    setLoading(false);
    setIsOpen(false);
  };
  return (
    <div>
      <Button label={"Delete"} onClick={() => setIsOpen(true)} />
      <Dialog open={isOpen} onClose={(val) => setIsOpen(val)} static={true}>
        <DialogPanel>
          <div>
            <h6>Are you sure you want to delete this service?</h6>
          </div>
          <div className="flex justify-end items-center space-x-3 pt-3">
            <Button label="Close" icon={faTimes} className="p-button-primary" onClick={() => setIsOpen(false)} />
            <Button
              label={"Delete"}
              icon={faCheck}
              disabled={loading}
              onClick={() => {
                handleSubmit();
              }}
            />
          </div>
        </DialogPanel>
      </Dialog>
    </div>
  );
};

export const ServicesDownloadModal = ({ Id }) => {
  const [loading, setLoading] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const handleSubmit = async () => {
    setLoading(true);
    await Backend.call("/api/dashboard/downloadservicesfile", { Id: parseInt(Id) }, { isBlob: true });
    setLoading(false);
    setIsOpen(false);
  };
  return (
    <div>
      <Button label={"Download"} onClick={() => handleSubmit()} />
    </div>
  );
};
