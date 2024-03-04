class TableHelpers {
  static groupBy(data, ...properties) {
    const newData = data.reduce((accumlator, object) => {
      const key = JSON.stringify(properties.map((x) => object[x] || null));

      if (!accumlator[key]) {
        accumlator[key] = [];
      }

      accumlator[key].push(object);
      return accumlator;
    }, {});

    return [...Object.values(newData)];
  }

  static sum(propName) {
    return function (total, obj) {
      const { [propName]: value } = obj;
      return total + TableHelpers.prepareValue(value);
    };
  }

  static min(propName) {
    return function (minimum, obj) {
      const { [propName]: value } = obj;
      var preparedValue = TableHelpers.prepareValue(value);
      return preparedValue < minimum ? preparedValue : minimum;
    };
  }

  static max(propName) {
    return function (maximum, obj) {
      const { [propName]: value } = obj;
      var preparedValue = TableHelpers.prepareValue(value);
      return preparedValue > maximum ? preparedValue : maximum;
    };
  }

  static prepareValue(value) {
    return Number.parseFloat(typeof value === "string" ? value.replace(/[A-Za-z,]/g, "").trim() : value);
  }
}

export default TableHelpers;
