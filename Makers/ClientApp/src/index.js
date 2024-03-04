import React from "react";
import App from "./App";
import store from "./store";
import Chart from "chart.js/auto"; // eslint-disable-line no-unused-vars
import { Provider } from "react-redux";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";

import "primereact/resources/themes/nova/theme.css";
import "primereact/resources/primereact.min.css";
import "primeicons/primeicons.css";
import "react-diff-view/style/index.css";
import "yet-another-react-lightbox/styles.css";
import "yet-another-react-lightbox/plugins/captions.css";
import "yet-another-react-lightbox/plugins/thumbnails.css";
import "./styles/index.css";
import "./styles/variables.css";
import "./styles/global.css";
import "./styles/popUp.css";
import "./styles/dashboard.css";
import "./styles/login.css";
import "./styles/navbar.css";
import "./styles/table.css";
import "./styles/form.css";
import "./styles/confirmDialog.css";
import "./styles/button.css";
import "./styles/mediaQueries.css";
import "./styles/progress.css";
import "./styles/imageViewer.css";
import "./styles/paymentCard.css";
import "./styles/contextMenu.css";
import "./styles/screenDashboard.css";
import "./styles/iraqMap.css";
import "./styles/header.css";
import "./styles/dashStyle.css";
const baseUrl = document.getElementsByTagName("base")[0].getAttribute("href");
const rootElement = document.getElementById("root");

const Screens = [];
const context = require.context("./components/screens/data", true, /\.js$/);

context.keys().forEach(function (key) {
  Screens.push(context(key).default);
});

createRoot(rootElement).render(
  <Provider store={store}>
    <BrowserRouter basename={baseUrl}>
      <App />
    </BrowserRouter>
  </Provider>
);

export default Screens;
