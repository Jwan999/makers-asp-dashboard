import React, { useEffect } from "react";
import { createPortal } from "react-dom";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useState, useRef } from "react";

const PopUp = ({ trigger, padding = "0 0.3rem", items, disabled, direction = "left", width = "220px", offsetX = 0, offsetY = 0 }) => {
  const [isActive, setIsActive] = useState(false);
  const triggerRef = useRef(null);
  const contentRef = useRef(null);

  const handleHidePopUp = () => {
    setIsActive(false);
  };

  const handleShowPopUp = () => {
    setIsActive(true);
  };

  useEffect(() => {
    if (isActive) {
      if (!triggerRef?.current || !contentRef?.current) return;
      const trigger = triggerRef.current.getBoundingClientRect();
      const content = contentRef.current.getBoundingClientRect();

      const cords = calculatePosition(trigger, content, direction);

      contentRef.current.style.top = `${cords.top + offsetY}px`;
      contentRef.current.style.left = `${cords.left + offsetX}px`;
    }
  }, [direction, isActive, offsetX, offsetY]);

  const filteredList = items.filter((item) => item);

  if (filteredList.length === 0) return null;

  return (
    <>
      <div className="popup" onMouseLeave={handleHidePopUp} onMouseEnter={handleShowPopUp} ref={triggerRef} style={{ padding: padding }}>
        {trigger}

        {isActive &&
          createPortal(
            <div className={`popup__children popup__children--${direction}`} style={{ width }} onClick={(e) => e.stopPropagation()} ref={contentRef}>
              {filteredList.map((item, index) => (
                <React.Fragment key={index}>
                  {(item.separator instanceof Function ? item.separator(items) : item.separator) && index > 0 && <span className="popup__separator" />}
                  <button className="popup__item" onClick={item.onClick} disabled={disabled}>
                    {item.icon && <FontAwesomeIcon icon={item.icon} className="popup__icon" />}
                    {item.title instanceof Function ? item.title() : item.title}
                  </button>
                </React.Fragment>
              ))}
            </div>,
            getRootPopup()
          )}
      </div>
    </>
  );
};

const getRootPopup = () => {
  let PopupRoot = document.getElementById("popup-root");

  if (PopupRoot === null) {
    PopupRoot = document.createElement("div");
    PopupRoot.setAttribute("id", "popup-root");
    document.body.appendChild(PopupRoot);
  }

  return PopupRoot;
};

const calculatePosition = (triggerBounding, ContentBounding, position) => {
  const CenterTop = triggerBounding.top + triggerBounding.height / 2;
  const CenterLeft = triggerBounding.left + triggerBounding.width / 2;
  const { height, width } = ContentBounding;
  let top = CenterTop - height / 2;
  let left = CenterLeft - width / 2;

  let transform = "";

  switch (position) {
    case "top":
      top -= height / 2 + triggerBounding.height / 2;
      transform = `rotate(180deg)  translateX(50%)`;
      break;
    case "bottom":
      top += height / 2 + triggerBounding.height / 2;
      transform = `rotate(0deg) translateY(-100%) translateX(-50%)`;
      break;
    case "left":
      left -= width / 2 + triggerBounding.width / 2;
      transform = ` rotate(90deg)  translateY(50%) translateX(-25%)`;
      break;
    case "right":
      left += width / 2 + triggerBounding.width / 2;
      transform = `rotate(-90deg)  translateY(-150%) translateX(25%)`;
      break;
    default:
  }

  return { top, left, transform };
};

export default PopUp;
