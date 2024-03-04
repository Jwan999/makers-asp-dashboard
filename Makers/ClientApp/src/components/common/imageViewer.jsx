import Lightbox from "yet-another-react-lightbox";
import Captions from "yet-another-react-lightbox/plugins/captions";
import Thumbnails from "yet-another-react-lightbox/plugins/thumbnails";

const ImageViewer = ({ imageViewer, setImageViewer }) => {
  return (
    <>
      {imageViewer.isShown && (
        <Lightbox
          open={imageViewer.isShown}
          close={() => setImageViewer({ isShown: false, images: [] })}
          slides={imageViewer.images}
          plugins={[Captions, Thumbnails]}
          thumbnails={{
            position: "end",
            width: 200,
            height: 200,
            padding: 5,
            gap: 5,
            imageFit: "cover",
            showToggle: true,
            vignette: false,
          }}
        />
      )}
    </>
  );
};

export default ImageViewer;
