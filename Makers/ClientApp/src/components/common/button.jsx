import { Button as PrimeButton } from "primereact/button";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

const Button = ({ icon, iconSize = "1x", width, label, disabled, onClick, className, ...props }) => {
  return (
    <PrimeButton disabled={disabled} onClick={onClick} style={{ width }} className={className} {...props}>
      {icon && <FontAwesomeIcon icon={icon} size={iconSize} />}
      <span className={`${icon && label ? "button__text-padding" : ""}`}>{label}</span>
    </PrimeButton>
  );
};

export default Button;
