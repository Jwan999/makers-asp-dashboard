import { Dialog } from "primereact/dialog";
import { ProgressBar as PrimeReactProgressBar } from "primereact/progressbar";

const ProgressBar = ({ progressBar }) => {
  return (
    <Dialog header={<h5 className="modal-header">{progressBar.header}</h5>} visible={progressBar.isShown} style={{ width: "50vw" }} closable={false}>
      <div className="progress">
        <p>
          <b>{progressBar.status}</b>
        </p>
        <p>{progressBar.message}</p>
        <PrimeReactProgressBar value={progressBar.progress}></PrimeReactProgressBar>
      </div>
    </Dialog>
  );
};

export default ProgressBar;
