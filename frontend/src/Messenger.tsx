import wordmark from "./assets/tradify-wordmark.png";
import { useCallback, useEffect, useState } from "react";
import { getJobs, getMessages, postMessage, type JobSummary, type Message, type SessionUser } from "./api";
import { Avatar } from "./Avatar";
import { ConversationList } from "./ConversationList";
import { JobThread } from "./JobThread";

export function Messenger({ user, onLogout }: { user: SessionUser; onLogout: () => void }) {
  const [jobs, setJobs] = useState<JobSummary[]>([]);
  const [jobsError, setJobsError] = useState<string | null>(null);
  const [selectedJobId, setSelectedJobId] = useState<string | null>(null);
  const [messages, setMessages] = useState<Message[]>([]);
  const [messagesError, setMessagesError] = useState<string | null>(null);

  const refreshJobs = useCallback(() => {
    getJobs()
      .then((result) => {
        setJobs(result);
        setJobsError(null);
      })
      .catch((err: Error) => setJobsError(err.message));
  }, []);

  useEffect(refreshJobs, [refreshJobs]);

  useEffect(() => {
    if (!selectedJobId) {
      return;
    }
    let cancelled = false;
    setMessages([]);
    setMessagesError(null);
    getMessages(selectedJobId)
      .then((result) => !cancelled && setMessages(result))
      .catch((err: Error) => !cancelled && setMessagesError(err.message));
    return () => {
      cancelled = true;
    };
  }, [selectedJobId]);

  async function send(body: string) {
    if (!selectedJobId) {
      return;
    }
    const message = await postMessage(selectedJobId, body);
    setMessages((current) => [...current, message]);
    refreshJobs();
  }

  const selectedJob = jobs.find((job) => job.id === selectedJobId) ?? null;

  return (
    <div className="app-shell">
      <header className="app-header">
        <img className="app-brand" src={wordmark} alt="Tradify" />
        <span className="app-header-user">
          <Avatar name={user.name} seed={user.id} size={32} />
          <span>{user.name}</span>
          <button className="btn btn-outline btn-sm" onClick={onLogout}>
            Log out
          </button>
        </span>
      </header>
      <main className="page">
        <h1 className="page-title">Messages</h1>
        {jobsError && <p className="alert-error">Could not load jobs: {jobsError}</p>}
        <div className={`messenger${selectedJob ? " has-selection" : ""}`}>
          <ConversationList user={user} jobs={jobs} selectedJobId={selectedJobId} onSelect={setSelectedJobId} />
          {selectedJob ? (
            <JobThread
              user={user}
              job={selectedJob}
              messages={messages}
              error={messagesError}
              onSend={send}
              onBack={() => setSelectedJobId(null)}
            />
          ) : (
            <section className="thread thread-placeholder">
              <p>Select a job to view its conversation</p>
            </section>
          )}
        </div>
      </main>
    </div>
  );
}
