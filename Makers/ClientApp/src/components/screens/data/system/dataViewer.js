import store from "../../../../store";
import Screen from "../../screenClass";

const dataViewer = new Screen({
  screenId: "dataViewer",

  build() {
    const { dataUrl, dataParams, backUrl, headerTitle } = store.getState().dataViewer.data;

    return {
      screenId: `${dataViewer.screenId}${headerTitle}`,
      contentHeader: headerTitle,
      dataUrl,
      backUrl,
      dataParams,
    };
  },
});

export default dataViewer;
