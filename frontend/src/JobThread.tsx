import { useEffect, useRef, useState, type FormEvent, type KeyboardEvent } from "react";
import type { JobSummary, Message, SessionUser } from "./api";
import { Avatar } from "./Avatar";
import { formatDayHeading, formatTime } from "./format";

type Props = {
  user: SessionUser;
  job: JobSummary;
  messages: Message[];
  error: string | null;
  onSend: (body: string) => Promise<void>;
  onBack: () => void;
};

export function JobThread({ user, job, messages, error, onSend, onBack }: Props) {
  const [draft, setDraft] = useState("");
  const [sending, setSending] = useState(false);
  const [sendError, setSendError] = useState<string | null>(null);
  const bottomRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ block: "end" });
  }, [messages]);

  useEffect(() => {
    setDraft("");
    setSendError(null);
  }, [job.id]);

  const nameFor = (senderId: string) => job.participants.find((p) => p.id === senderId)?.name ?? senderId;

  async function send(event?: FormEvent) {
    event?.preventDefault();
    const body = draft.trim();
    if (!body || sending) {
      return;
    }
    setSending(true);
    setSendError(null);
    try {
      await onSend(body);
      setDraft("");
    } catch (err) {
      setSendError((err as Error).message);
    } finally {
      setSending(false);
    }
  }

  function handleKeyDown(event: KeyboardEvent<HTMLTextAreaElement>) {
    if (event.key === "Enter" && !event.shiftKey) {
      event.preventDefault();
      void send();
    }
  }

  return (
    <section className="thread">
      <header className="thread-header">
        <button className="back-button" onClick={onBack} aria-label="Back to conversations">
          &larr;
        </button>
        <Avatar name={job.title} seed={job.id} size={40} />
        <div>
          <div className="thread-title">{job.title}</div>
          <div className="thread-participants">{job.participants.map((p) => p.name).join(", ")}</div>
        </div>
      </header>

      <div className="thread-body">
        {error && <p className="alert-error">Could not load messages: {error}</p>}
        {!error && messages.length === 0 && <p className="empty-state">No messages yet. Say hello!</p>}
        <ul className="message-list">
          {messages.map((message, index) => {
            const mine = message.senderId === user.id;
            const previous = messages[index - 1];
            const newDay =
              !previous || new Date(previous.createdAt).toDateString() !== new Date(message.createdAt).toDateString();
            const showSender = !mine && (newDay || previous?.senderId !== message.senderId);
            return (
              <li key={message.id}>
                {newDay && <div className="day-divider">{formatDayHeading(message.createdAt)}</div>}
                <div className={`bubble-row ${mine ? "mine" : "theirs"}`}>
                  <div className="bubble">
                    {showSender && <div className="bubble-sender">{nameFor(message.senderId)}</div>}
                    <span className="bubble-body">{message.body}</span>
                    <span className="bubble-time">{formatTime(message.createdAt)}</span>
                  </div>
                </div>
              </li>
            );
          })}
        </ul>
        <div ref={bottomRef} />
      </div>

      <form className="composer" onSubmit={send}>
        {sendError && <p className="composer-error">Could not send: {sendError}</p>}
        <textarea
          value={draft}
          onChange={(e) => setDraft(e.target.value)}
          onKeyDown={handleKeyDown}
          placeholder="Type a message"
          rows={2}
          aria-label="Message"
        />
        <button type="submit" className="btn btn-primary" disabled={!draft.trim() || sending}>
          Send
        </button>
      </form>
    </section>
  );
}
