import { useEffect } from "react";

const Notfound = () => {
  useEffect(() => {
    document.title = "404 Not Found";
  }, []);
  return (
    <div className="error">
      <h2>404</h2>
      <h2>Not found</h2>
    </div>
  );
};

export default Notfound;
