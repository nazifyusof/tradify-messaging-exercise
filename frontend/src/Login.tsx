import logoWhite from "./assets/tradify-logo-white.png";
import { useState, type FormEvent } from "react";
import { login, type SessionUser } from "./api";

const DEMO_USERS = [
  { username: "alex", label: "Alex Morgan" },
  { username: "sam", label: "Sam Lee" },
  { username: "priya", label: "Priya Shah" },
  { username: "chris", label: "Chris Walker" },
  { username: "dana", label: "Dana Price" },
];

export function Login({ onLogin }: { onLogin: (user: SessionUser) => void }) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      onLogin(await login(username, password));
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="login-page">
      <img className="login-logo" src={logoWhite} alt="Tradify" />
      <form className="login-card" onSubmit={handleSubmit}>
        <h1>Messages</h1>
        <p className="login-subtitle">Sign in to see your job conversations</p>

        <label>
          Username
          <input
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            autoFocus
            autoComplete="username"
            required
          />
        </label>

        <label>
          Password
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete="current-password"
            required
          />
        </label>

        {error && <p className="alert-error">{error}</p>}

        <button type="submit" className="btn btn-primary btn-block" disabled={submitting}>
          {submitting ? "Signing in..." : "Sign in"}
        </button>

        <div className="login-hint">
          <p>Demo users (any password works):</p>
          <ul>
            {DEMO_USERS.map((user) => (
              <li key={user.username}>
                <button type="button" className="link-button" onClick={() => setUsername(user.username)}>
                  {user.username}
                </button>{" "}
                {user.label}
              </li>
            ))}
          </ul>
        </div>
      </form>
    </div>
  );
}
