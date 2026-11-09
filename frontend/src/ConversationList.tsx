import type { JobSummary, SessionUser } from "./api";
import { Avatar } from "./Avatar";
import { formatListTimestamp } from "./format";

type Props = {
  user: SessionUser;
  jobs: JobSummary[];
  selectedJobId: string | null;
  onSelect: (jobId: string) => void;
};

export function ConversationList({ user, jobs, selectedJobId, onSelect }: Props) {
  return (
    <aside className="sidebar">
      <header className="sidebar-header">Jobs</header>

      {jobs.length === 0 ? (
        <p className="empty-state">You are not assigned to any jobs yet.</p>
      ) : (
        <ul className="conversation-list">
          {jobs.map((job) => {
            const sender = job.participants.find((p) => p.id === job.lastMessage?.senderId);
            const senderLabel = job.lastMessage?.senderId === user.id ? "You" : sender?.name.split(" ")[0];
            return (
              <li key={job.id}>
                <button
                  className={`conversation${job.id === selectedJobId ? " selected" : ""}`}
                  onClick={() => onSelect(job.id)}
                >
                  <Avatar name={job.title} seed={job.id} size={48} />
                  <span className="conversation-text">
                    <span className="conversation-top">
                      <span className="conversation-title">{job.title}</span>
                      {job.lastMessage && (
                        <span className="conversation-time">{formatListTimestamp(job.lastMessage.createdAt)}</span>
                      )}
                    </span>
                    <span className="conversation-preview">
                      {job.lastMessage ? `${senderLabel}: ${job.lastMessage.body}` : "No messages yet"}
                    </span>
                  </span>
                </button>
              </li>
            );
          })}
        </ul>
      )}
    </aside>
  );
}
