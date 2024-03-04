import mc from "../../images/logo-mc.png";
import visa from "../../images/logo-visa.png";
import chip from "../../images/chip.png";
import { useRef } from "react";
import { useReactToPrint } from "react-to-print";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPrint, faWifi } from "@fortawesome/free-solid-svg-icons";

const PaymentCard = ({ bank, cardNum, expiry, embossedName, width, printAlignFontSize, printAlignBottom, printAlignLeft, printAlignRotate, printCallback }) => {
  const printRef = useRef();

  const printHook = useReactToPrint({
    content: () => printRef.current,
    documentTitle: "Print Name on Card",
  });

  return (
    <>
      <div className="payment-card" style={{ width }}>
        <div className="payment-card-header">{bank.toUpperCase()}</div>
        <div className="payment-card-chip">
          <img src={chip} alt="chip" width={75} />
          <FontAwesomeIcon icon={faWifi} size="3x" rotation={90} />
        </div>
        <div className="payment-card-number">
          <span>{cardNum.substr(0, 4)}</span>
          <span>{cardNum.substr(4, 4)}</span>
          <span>{cardNum.substr(8, 4)}</span>
          <span>{cardNum.substr(12, 4)}</span>
        </div>
        <div className="payment-card-expiry">
          <div>VALID THRU {expiry.substr(3, 3) + expiry.substr(8, 2)}</div>
        </div>
        <div className="payment-card-embossed-name">
          <div>
            {embossedName}&nbsp;&nbsp;
            <FontAwesomeIcon
              className="payment-card-printer"
              icon={faPrint}
              onClick={async () => {
                await printCallback();
                printHook();
              }}
            />
          </div>
          <div>
            <img src={cardNum.startsWith("5") ? mc : visa} alt="network" height={30} />
          </div>
        </div>
      </div>

      {printCallback && (
        <div hidden>
          <style type="text/css" media="print">
            {"@page { size: landscape; }"}
            {`@body {  -webkit-transform: rotate(${printAlignRotate}deg); }`}
          </style>

          <div
            ref={printRef}
            style={{
              position: "absolute",
              color: "black",
              fontWeight: "bold",
              bottom: `${printAlignBottom}%`,
              left: `${printAlignLeft}%`,
              fontSize: `${printAlignFontSize}px`,
              transform: `rotate(${printAlignRotate}deg)`,
            }}
          >
            {embossedName}
          </div>
        </div>
      )}
    </>
  );
};

export default PaymentCard;
