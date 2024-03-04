import React, { useState } from "react";
import Button from "./button";
import { useEffect } from "react";
import { useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { faChevronLeft } from "@fortawesome/free-solid-svg-icons";
import { parseDiff, Diff, Hunk } from "react-diff-view";
import { diffLines, formatLines } from "unidiff";

const DiffViewer = () => {
  const [splitView, setSplitView] = useState(true);
  const { newObj, oldObj, headerTitle, backUrl } = useSelector((state) => state.diffViewer.data);

  const isLoading = useSelector((state) => state.dashboard.isLoading);
  const navigate = useNavigate();

  const isUpdateQuery = newObj !== "" && oldObj !== "";
  const diffText = formatLines(diffLines(oldObj, newObj), { context: 10 });
  const [diff] = parseDiff(diffText, { nearbySequences: "zip" });

  useEffect(() => {
    document.title = headerTitle;
  }, [headerTitle]);

  return (
    <div className="content-component fade-in">
      <div className="content-component__header">
        <h4>{headerTitle}</h4>
        <div className="content-actions">
          {isUpdateQuery && <Button label="Split/Unified" disabled={isLoading} onClick={() => setSplitView(!splitView)} />}
          <Button
            icon={faChevronLeft}
            iconSize="lg"
            disabled={isLoading}
            onClick={() => {
              navigate(backUrl, { replace: true });
            }}
          />
        </div>
      </div>
      <div className="scrollable-container">
        <Diff viewType={splitView && isUpdateQuery ? "split" : "unified"} hunks={diff.hunks} diffType={diff.type}>
          {(hunks) => {
            return hunks.map((hunk) => <Hunk key={hunk.content} hunk={hunk} />);
          }}
        </Diff>
      </div>
    </div>
  );
};

export default DiffViewer;
