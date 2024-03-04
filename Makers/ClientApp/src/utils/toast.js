import store from "../store";

export function showToast(message, type, style) {
  const ref = store.getState().toast.ref;

  store.dispatch({ type: "toast/setMessage", payload: { content: message, type, style } });

  const messages = store.getState().toast.messages;

  const handleClear = () => {
    ref.current.clear();

    store.dispatch({ type: "toast/clearMessages" });
  };

  const handleClipoard = async (message) => {
    await navigator.clipboard.writeText(message);
  };

  ref.current.show(
    messages.map((message) => ({
      severity: message.type,
      content: (
        <div className="toast">
          <p className="toast__content" style={message.style} dangerouslySetInnerHTML={{ __html: message.content }}></p>
          <div className="toast__actions">
            <div className="toast__action" onClick={handleClear}>
              Close All
            </div>
            <div className="toast__action" onClick={async () => await handleClipoard(message.content)}>
              Copy
            </div>
          </div>
        </div>
      ),
      sticky: true,
    }))
  );
}
