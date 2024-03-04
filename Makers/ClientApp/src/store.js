import { configureStore, createSlice } from "@reduxjs/toolkit";

const store = configureStore({
  reducer: {
    dashboard: createSlice({
      name: "dashboard",
      initialState: {
        isLoading: false,
        requestUrls: [],
      },
      reducers: {
        setIsLoading: (state, action) => {
          state.isLoading = action.payload;
        },
        removeUrl: (state, action) => {
          state.requestUrls = state.requestUrls.filter((url) => url !== action.payload);
        },
        addUrl: (state, action) => {
          state.requestUrls.push(action.payload);
        },
      },
    }).reducer,

    diffViewer: createSlice({
      name: "diffViewer",
      initialState: {
        data: { oldObj: "", newObj: "", headerTitle: "", backUrl: "/dashboard/screens/audits" },
      },
      reducers: {
        setDiff: (state, action) => {
          state.data = action.payload;
        },
      },
    }).reducer,

    codeEditor: createSlice({
      name: "codeEditor",
      initialState: {
        data: { buttonText: "", language: "", headerTitle: "", codeId: "", dataUrl: "", backUrl: "", submitUrl: "" },
      },
      reducers: {
        setEditor: (state, action) => {
          state.data = action.payload;
        },
      },
    }).reducer,

    dataViewer: createSlice({
      name: "dataViewer",
      initialState: {
        data: { dataUrl: "", dataParams: {}, backUrl: "", headerTitle: "" },
      },
      reducers: {
        setData: (state, action) => {
          state.data = action.payload;
        },
      },
    }).reducer,

    formFilters: createSlice({
      name: "formFilters",
      initialState: {
        data: {},
        screenId: "",
      },
      reducers: {
        set: (state, action) => {
          state.data = action.payload.data;
          state.screenId = action.payload.screenId;
        },

        clear: (state) => {
          state.screenId = "";
          state.data = {};
        },
      },
    }).reducer,

    tableFilters: createSlice({
      name: "tableFilters",
      initialState: {
        data: [],
      },
      reducers: {
        set: (state, action) => {
          let filterIndex = state.data.findIndex((f) => f.id === action.payload.id && f.screenId === action.payload.screenId);

          if (filterIndex >= 0) {
            state.data[filterIndex] = action.payload;
          } else {
            state.data.push(action.payload);
          }
        },

        clear: (state, action) => {
          state.data = state.data.filter((f) => f.screenId !== action.payload.screenId);
        },
      },
    }).reducer,

    toast: createSlice({
      name: "toast",
      initialState: {
        ref: {},
        messages: [],
      },
      reducers: {
        setRef: (state, action) => {
          state.ref = { ...action.payload };
        },

        setMessage: (state, action) => {
          state.messages.push(action.payload);
        },

        clearMessages: (state) => {
          state.messages = [];
        },
      },
    }).reducer,

    dateFilters: createSlice({
      name: "dateFilters",
      initialState: {
        data: [],
      },
      reducers: {
        addFilter: (state, action) => {
          const filterIndex = state.data.findIndex((f) => f.screenId === action.payload.screenId);

          if (filterIndex >= 0) {
            state.data[filterIndex] = action.payload;
          } else {
            state.data.push(action.payload);
          }
        },
      },
    }).reducer,

    screenQuery: createSlice({
      name: "screenQuery",
      initialState: {
        data: { dataUrl: null, dataParams: null, backUrl: null, screenId: null },
      },
      reducers: {
        setData: (state, action) => {
          state.data = action.payload;
        },
        clearData: (state) => {
          state.data = { dataUrl: null, dataParams: null, backUrl: null, screenId: null };
        },
      },
    }).reducer,

    screenDashboardQuery: createSlice({
      name: "screenDashboardQuery",
      initialState: {
        data: { dataUrlDashboard: null, dataParams: null, screenId: null },
      },
      reducers: {
        setData: (state, action) => {
          state.data = action.payload;
        },
        clearData: (state) => {
          state.data = { dataUrlDashboard: null, dataParams: null, screenId: null };
        },
      },
    }).reducer,
  },

  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: ["toast/setRef"],
        ignoredPaths: ["toast.ref"],
      },
    }),
});

export default store;
