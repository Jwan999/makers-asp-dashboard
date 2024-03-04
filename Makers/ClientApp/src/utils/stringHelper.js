import Constants from "./constants";

const formatterUsd = Intl.NumberFormat("en-US", {
  style: "currency",
  currency: "USD",
});

const formatterIqd = Intl.NumberFormat("en-US", {
  style: "currency",
  currency: "IQD",
});

export function formatAmount(amount, currency) {
  if (amount === null || amount === undefined) {
    return "N/A";
  } else if (currency === "IQD" || currency === "368") {
    return formatterIqd.format(amount);
  } else {
    return formatterUsd.format(amount);
  }
}

export function escapeRegExp(str) {
  return str.replace(/([.*+?^=!${}()|[\]/\\])/g, "\\$1");
}

export function replaceAll(str, find, replace) {
  return str.replace(new RegExp(escapeRegExp(find), "g"), replace);
}

export function customFilter(filter, row) {
  const id = filter.pivotId || filter.id;
  if (row[id] !== null && row[id] !== undefined) {
    return row[id] !== undefined ? String(String(row[id]).toLowerCase()).includes(filter.value.toLowerCase()) : true;
  }
}

export function getDate(date) {
  return date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear() + " " + date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();
}

export function getTimeStamp(date) {
  var timestamp = "";

  timestamp += date.getFullYear();
  timestamp += date.getMonth() + 1;
  timestamp += date.getDate();
  timestamp += date.getHours();
  timestamp += date.getMinutes();
  timestamp += date.getSeconds();

  return timestamp;
}

export function isNullOrWhiteSpace(stringInput) {
  return !stringInput || !stringInput.trim();
}

export function getRandomColor() {
  return "#" + (((1 << 24) * Math.random()) | 0).toString(16);
}

export function formatFileSize(fileSize) {
  const kb = 1024;
  const mb = kb * 1024;
  const gb = mb * 1024;

  if (fileSize < kb) {
    return fileSize + " B";
  } else if (fileSize < mb) {
    return (fileSize / kb).toFixed(2) + " KB";
  } else if (fileSize < gb) {
    return (fileSize / mb).toFixed(2) + " MB";
  } else {
    return (fileSize / gb).toFixed(2) + " GB";
  }
}

export function singularize(word) {
  if (word.endsWith("ses") || word.endsWith("xes") || word.endsWith("zes")) {
    return word.slice(0, -2);
  }

  if (word.endsWith("shes") || word.endsWith("ches")) {
    return word.slice(0, -2);
  }

  if (word.endsWith("ies")) {
    return word.slice(0, -3) + "y";
  }

  return word.slice(0, -1);
}

export function isArabic(str) {
  let def = 0;
  let ar = 0;

  if (str && typeof str === "string") {
    str.split("").forEach((i) => (/[\u0600-\u06FF]/.test(i) ? ar++ : def++));
  }

  return ar > def;
}

export function verifyPhoneNumber(phone) {
  try {
    var rawPhone = phone.substring(phone.indexOf("7"), phone.length - phone.indexOf("7") + 4);

    if (!phone.startsWith(Constants.IraqiPhoneNumKey) || rawPhone.length !== 10 || rawPhone === null) {
      return false;
    } else {
      return true;
    }
  } catch {
    return false;
  }
}
