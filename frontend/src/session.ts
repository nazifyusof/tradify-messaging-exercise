import { setCurrentUser, type SessionUser } from "./api";

const STORAGE_KEY = "tradify.session";

export function loadSession(): SessionUser | null {
  const raw = localStorage.getItem(STORAGE_KEY);
  if (!raw) {
    return null;
  }
  try {
    const user = JSON.parse(raw) as Partial<SessionUser>;
    if (!user.role || !user.id) {
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
    setCurrentUser(user.id);
    return user as SessionUser;
  } catch {
    localStorage.removeItem(STORAGE_KEY);
    return null;
  }
}

export function saveSession(user: SessionUser) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
  setCurrentUser(user.id);
}

export function clearSession() {
  localStorage.removeItem(STORAGE_KEY);
  setCurrentUser(null);
}
