class Jwt {
  static getUserId() {
    return this.getUserInfo()["117115101114105100"];
  }

  static getUsername() {
    return this.getUserInfo()["11711510111411097109101"];
  }

  static getDepartment() {
    return this.getUserInfo()["68698065828477697884"];
  }

  static getUserInsts() {
    return this.getUserInfo()["105110115116105116117116105111110115"].split(",");
  }

  static getUserInstsDropdown() {
    var userInstsDropdown = [];

    this.getUserInfo()
      ["105110115116105116117116105111110115"].split(",")
      .forEach((e) => userInstsDropdown.push({ label: e, value: e }));

    return userInstsDropdown;
  }

  static getRoleId() {
    return this.getUserInfo()["114111108101105100"];
  }

  static getLastLoginDate() {
    return this.getUserInfo()["10897115116108111103105110"];
  }

  static getIsUsingDefaultPassword() {
    return this.getUserInfo()["100102112"];
  }

  static getIsPasswordExpired() {
    return this.getUserInfo()["8087698880738269"];
  }

  static getPasswordExpirationDate() {
    return this.getUserInfo()["8097115115119111114100691201121051141016897116101"];
  }

  static getUserInfo = () => {
    const tokenFrMakersession = sessionStorage.getItem("748784");

    if (!tokenFrMakersession) {
      return "";
    }
    const jwt = JSON.parse(atob(tokenFrMakersession.split(".")[1]));

    if (!jwt) {
      sessionStorage.removeItem("748784");
      window.location.replace("/login");
      return;
    }

    return jwt;
  };

  static isAuthorized(toBeAuthorizedClaims) {
    var isAuthorized = true;

    if (!toBeAuthorizedClaims) {
      return isAuthorized;
    }

    if (this.getUserInfo() === "") {
      return false;
    }

    let claims = this.getUserInfo()["9910897105109115"].split(",");

    for (var i = 0; i < toBeAuthorizedClaims.length; i++) {
      if (claims.includes(toBeAuthorizedClaims[i])) {
        continue;
      } else {
        isAuthorized = false;
        break;
      }
    }

    return isAuthorized;
  }
}

export default Jwt;
