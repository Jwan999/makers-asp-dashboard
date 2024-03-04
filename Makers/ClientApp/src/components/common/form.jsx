/* eslint-disable no-unused-expressions */

import React, { useRef, useEffect } from "react";
import dayjs from "dayjs";
import { showToast } from "../../utils/toast";
import { useDispatch } from "react-redux";
import { isArabic } from "../../utils/stringHelper";
import { formatFileSize } from "../../utils/stringHelper";
import { Dropdown } from "primereact/dropdown";
import { Calendar } from "primereact/calendar";
import { InputText } from "primereact/inputtext";
import { MultiSelect } from "primereact/multiselect";
import { AutoComplete } from "primereact/autocomplete";
import { InputTextarea } from "primereact/inputtextarea";

export function ValidateForm(formInputs, formData) {
  for (var i = 0; i < formInputs.length; i++) {
    if (formInputs[i].type === "section") {
      continue;
    }

    const value = formData[formInputs[i].id];

    const schema = {
      type: ["multiselect", "file"].includes(formInputs[i].type) ? "object" : formInputs[i].dataType === undefined ? "string" : formInputs[i].dataType,
      required: formInputs[i].required === undefined ? true : formInputs[i].required,
      min: formInputs[i].min,
      max: formInputs[i].max,
    };

    if (schema.required) {
      if (schema.type === "number" && isNaN(value)) {
        return `${formInputs[i].label} must be ${schema.type}`;
      }

      if (schema.type !== "number" && typeof value !== schema.type) {
        return `${formInputs[i].label} must be ${schema.type}`;
      }

      if (typeof value === "string") {
        if (!value) {
          return `${formInputs[i].label} is required`;
        }

        if (value.length < schema.min) {
          return `${formInputs[i].label} length must be greater than ${schema.min}`;
        }

        if (value.length > schema.max) {
          return `${formInputs[i].label} length must be less than ${schema.max}`;
        }
      }

      if (typeof value === "object") {
        if (!value || value.length === 0) {
          return `${formInputs[i].label} is required`;
        }
      }
    }
  }
}

function RenderTextBox({ input, props }) {
  const isRequired = input.required !== undefined ? input.required : true;
  const label = input.label instanceof Function ? input.label(props.data) : input.label;
  const inputRef = useRef(null);
  const dispatch = useDispatch();
  return (
    <React.Fragment key={input.id}>
      <div className="form__label">
        {label}
        <span className="form__required-mark">{isRequired ? " *" : ""}</span>
      </div>
      <InputText
        ref={inputRef}
        id={input.id}
        name={input.id}
        value={props.data[input.id]}
        className={`form__text ${isArabic(props.data[input.id]) ? "input-rtl" : ""}`}
        readOnly
        onFocus={() => inputRef.current.removeAttribute("readonly")}
        onBlur={() => inputRef.current.setAttribute("redonly", "")}
        type={input.type}
        disabled={props.isLoading || (input.disabled instanceof Function ? input.disabled(props.data) : input.disabled || false)}
        tooltip={input.tooltip}
        onKeyDown={(e) => {
          if (input.type === "password" && e.getModifierState("CapsLock")) {
            dispatch({ type: "toast/clearMessages" });
            showToast("Capslock is on!", "warn");
          }
        }}
        onChange={(e) => {
          props.updateForm(e.target.id, e.target.value);
        }}
      />
    </React.Fragment>
  );
}

function RenderDropdown({ input, props }) {
  const isRequired = input.required !== undefined ? input.required : true;
  const label = input.label instanceof Function ? input.label(props.data) : input.label;
  return (
    <React.Fragment key={input.id}>
      <div className="form__label">
        {label}
        <span className="form__required-mark">{isRequired ? " *" : ""}</span>
      </div>
      <Dropdown
        id={input.id}
        name={input.id}
        value={props.data[input.id]}
        className="form__dropdown"
        showClear={true}
        options={input.options}
        disabled={props.isLoading || (input.disabled instanceof Function ? input.disabled(props.data) : input.disabled || false)}
        filter={true}
        tooltip={input.tooltip}
        scrollHeight="200px"
        onChange={(e) => {
          props.updateForm(e.target.id, e.target.value);
          input.customOnChangeHandler ? input.customOnChangeHandler(e.target.value, props.handleDropdownChange, props.handleResetFormData, props.data) : null;
        }}
        onShow={() => (input.onShow ? input.onShow(props.data, props.handleDropdownChange) : null)}
      />
    </React.Fragment>
  );
}

function RenderMultiSelect({ input, props }) {
  const isRequired = input.required !== undefined ? input.required : true;
  const label = input.label instanceof Function ? input.label(props.data) : input.label;
  return (
    <React.Fragment key={input.id}>
      <div className="form__label">
        {label}
        <span className="form__required-mark">{isRequired ? " *" : ""}</span>
      </div>
      <MultiSelect
        id={input.id}
        name={input.id}
        value={props.data[input.id]}
        maxSelectedLabels={input.maxLabels ? input.maxLabels : 1}
        placeholder={`Select ${label}`}
        scrollHeight="200px"
        className="form__multiselect"
        filter={true}
        tooltip={input.tooltip}
        options={input.options}
        disabled={props.isLoading || (input.disabled instanceof Function ? input.disabled(props.data) : input.disabled || false)}
        onChange={(e) => {
          props.updateForm(e.target.id, e.target.value);
          input.customOnChangeHandler ? input.customOnChangeHandler(e.target.value, props.handleDropdownChange, props.handleResetFormData, props.data) : null;
        }}
      />
    </React.Fragment>
  );
}

function RenderCalendar({ input, props }) {
  const isRequired = input.required !== undefined ? input.required : true;
  const label = input.label instanceof Function ? input.label(props.data) : input.label;
  return (
    <React.Fragment key={input.id}>
      <div className="form__label">
        {label}
        <span className="form__required-mark">{isRequired ? " *" : ""}</span>
      </div>
      <Calendar
        id={input.id}
        name={input.id}
        value={dayjs(props.data[input.id])["$d"]}
        className="form__calendar"
        dateFormat="yy/mm/dd"
        disabled={props.isLoading || (input.disabled instanceof Function ? input.disabled(props.data) : input.disabled || false)}
        tooltip={input.tooltip}
        onChange={(e) => {
          const newValue = dayjs(e.target.value).format(input.dateFormat);
          props.updateForm(e.target.id, newValue);
        }}
      />
    </React.Fragment>
  );
}

function RenderAutoComplete({ input, props }) {
  const isRequired = input.required !== undefined ? input.required : true;
  const label = input.label instanceof Function ? input.label(props.data) : input.label;
  return (
    <React.Fragment key={input.id}>
      <div className="form__label">
        {label}
        <span className="form__required-mark">{isRequired ? " *" : ""}</span>
      </div>
      <AutoComplete
        id={input.id}
        name={input.id}
        value={props.data[input.id]}
        field="label"
        forceSelection
        multiple={input.multiselect}
        suggestions={input.suggestions}
        completeMethod={(e) => props.handleAutoComplete(e.query.toLowerCase(), input.id)}
        minLength={3}
        delay={2000}
        placeholder={input.placeholder}
        disabled={props.isLoading || (input.disabled instanceof Function ? input.disabled(props.data) : input.disabled || false)}
        tooltip={input.tooltip}
        className={input.multiselect ? "auto-complete-multiselect" : "dropdown"}
        onChange={(e) => {
          // Because Arrays are Objects, it must be checked first.
          if (e.target.value instanceof Array) {
            props.updateForm(e.target.id, e.target.value);
            input.customOnChangeHandler ? input.customOnChangeHandler(e.target.value, props.handleDropdownChange, props.handleResetFormData, props.data) : null;
          } else if (e.target.value instanceof Object) {
            props.updateForm(e.target.id, e.target.value.label);
            input.customOnChangeHandler
              ? input.customOnChangeHandler(e.target.value.label, props.handleDropdownChange, props.handleResetFormData, props.data)
              : null;
          } else if (e.target.value === "") {
            props.updateForm(e.target.id, "");
          }
        }}
      />
    </React.Fragment>
  );
}

function RenderTextArea({ input, props }) {
  const isRequired = input.required !== undefined ? input.required : true;
  const label = input.label instanceof Function ? input.label(props.data) : input.label;
  const footer = input.footer instanceof Function ? input.footer(props.data) : input.footer;

  return (
    <React.Fragment key={input.id}>
      <div className="form__label">
        {label}
        <span className="form__required-mark">{isRequired ? " *" : ""}</span>
      </div>
      <InputTextarea
        id={input.id}
        name={input.id}
        value={props.data[input.id]}
        className={`form__text ${isArabic(props.data[input.id]) ? "input-rtl" : ""}`}
        rows={5}
        type={input.type}
        disabled={props.isLoading || (input.disabled instanceof Function ? input.disabled(props.data) : input.disabled || false)}
        tooltip={input.tooltip}
        onChange={(e) => {
          props.updateForm(e.target.id, e.target.value);
        }}
      />
      {footer && <div className="form__input-footer">{footer}</div>}
    </React.Fragment>
  );
}0

function RenderFileInput({ input, props }) {
  const files = props.data[input.id] || [];

  return (
    <React.Fragment key={input.id}>
      <div className="file-dnd">
        <input
          id={input.id}
          name={input.id}
          multiple={input.isMultiple}
          type="file"
          className="file-input"
          readOnly
          disabled={props.isLoading || (input.disabled instanceof Function ? input.disabled(props.data) : input.disabled || false)}
          tooltip={input.tooltip}
          onChange={(e) => {
            const newFiles = [];
            for (let i = 0; i < e.target.files.length; i++) {
              let reader = new FileReader();
              reader.readAsDataURL(e.target.files[i]);
              reader.onload = function () {
                let base64data = reader.result;
                newFiles.push(base64data);
                console.log(newFiles);
              };

              reader.onerror = function (error) {
                console.log("Error: ", error);
              };
            }
            console.log(newFiles);

            props.updateForm(e.target.id, newFiles);
          }}
        />
        Select files or drag and drop your files here...
      </div>
    </React.Fragment>
  );
}

function RenderSection({ input }) {
  return <span key={input.id}>{input.label}</span>;
}

const Form = (props) => {
  const rendredInputs = [];
  for (let i = 0; i < props.inputs.length; i++) {
    switch (props.inputs[i].type) {
      case "text":
      case "password":
        rendredInputs.push(<RenderTextBox input={props.inputs[i]} props={props} />);
        break;
      case "dropdown":
        rendredInputs.push(<RenderDropdown input={props.inputs[i]} props={props} />);
        break;
      case "multiselect":
        rendredInputs.push(<RenderMultiSelect input={props.inputs[i]} props={props} />);
        break;
      case "calendar":
        rendredInputs.push(<RenderCalendar input={props.inputs[i]} props={props} />);
        break;
      case "autocomplete":
        rendredInputs.push(<RenderAutoComplete input={props.inputs[i]} props={props} />);
        break;
      case "textarea":
        rendredInputs.push(<RenderTextArea input={props.inputs[i]} props={props} />);
        break;
      case "file":
        rendredInputs.push(<RenderFileInput input={props.inputs[i]} props={props} />);
        break;
      case "section":
        rendredInputs.push(<RenderSection input={props.inputs[i]} props={props} />);
        break;
      default:
        break;
    }
  }

  const gridColumnClass = rendredInputs.length <= 8 ? "form--one-column" : rendredInputs.length >= 12 ? "form--three-columns" : "form--two-columns";
  const gridColumnEnd = rendredInputs.length <= 8 ? "1" : rendredInputs.length >= 12 ? "3" : "2";

  useEffect(() => {
    if (props.inputs?.length > 0 && props.inputs[0]?.type === "text") {
      var input = document.getElementById("modal-form").getElementsByTagName("input")[0];
      input.focus();
    }
  }, [props.inputs]);

  return (
    <div id="modal-form" className={`form ${gridColumnClass}`}>
      {rendredInputs.map((input, index) => (
        <div
          key={index}
          className={`form__grid-${input.props.input.type === "section" ? "section" : "item"}`}
          style={{ gridColumnEnd: input.props.input.type === "section" ? `span ${gridColumnEnd}` : "" }}
        >
          {input}
        </div>
      ))}
    </div>
  );
};

export default Form;
