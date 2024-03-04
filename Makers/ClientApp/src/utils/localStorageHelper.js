export const getLocalStorageData = ({ data, localStorageKey, itemId }) => {
  let savedItemData = JSON.parse(localStorage.getItem(localStorageKey)) || [];

  const currentSavedItemData = savedItemData.find((item) => item.id === itemId);

  const currentDateTime = new Date().getTime();

  if (currentSavedItemData && currentSavedItemData.data && currentSavedItemData.expirationDate > currentDateTime) {
    Object.getOwnPropertyNames(data).forEach((p) => {
      data[p] = currentSavedItemData.data[p];
    });
  }

  return data;
};

export const saveLocalStorageItem = ({ data, localStorageKey, itemId }) => {
  let savedItemData = JSON.parse(localStorage.getItem(localStorageKey)) || [];

  savedItemData = savedItemData.filter((item) => item.id !== itemId);

  savedItemData.push({
    id: itemId,
    data,
    expirationDate: new Date().getTime() + 1209600 * 1000,
  });

  localStorage.setItem(localStorageKey, JSON.stringify(savedItemData));
};
