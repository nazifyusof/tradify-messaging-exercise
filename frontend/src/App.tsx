import { useState } from "react";
import type { SessionUser } from "./api";
import { Login } from "./Login";
import { Messenger } from "./Messenger";
import { clearSession, loadSession, saveSession } from "./session";

export default function App() {
  const [user, setUser] = useState<SessionUser | null>(loadSession);

  if (!user) {
    return (
      <Login
        onLogin={(loggedIn) => {
          saveSession(loggedIn);
          setUser(loggedIn);
        }}
      />
    );
  }

  return (
    <Messenger
      key={user.id}
      user={user}
      onLogout={() => {
        clearSession();
        setUser(null);
      }}
    />
  );
}
