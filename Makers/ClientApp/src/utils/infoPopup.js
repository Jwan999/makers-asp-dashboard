import { Button } from "primereact/button";
import { confirmDialog } from "primereact/confirmdialog";

const InfoPopUp = (title, width, info) => {
  const keysArray = [];
  const valuesArray = [];

  Object.keys(info).forEach(function (key) {
    keysArray.push(key);
    valuesArray.push(info[key]);
  });

  confirmDialog({
    header: title,
    style: { width },
    dismissableMask: true,
    message: () => (
      <>
        {keysArray.map((key, index) => {
          return (
            <div key={index} className="key-value-container">
              <span className="key-container">{key}:</span>
              <span className="value-container">{valuesArray[index]}</span>
            </div>
          );
        })}
      </>
    ),
    footer: ({ accept }) => <Button label="OK" onClick={accept} />,
  });
};

export default InfoPopUp;
