export type Message = {
  id: string;
  jobId: string;
  senderId: string;
  body: string;
  createdAt: string;
};

export type Participant = {
  id: string;
  name: string;
};

export type JobSummary = {
  id: string;
  title: string;
  participants: Participant[];
  lastMessage: Message | null;
};

export type SessionUser = {
  id: string;
  username: string;
  name: string;
};

// Defaults to same-origin; the Vite dev server proxies /auth and /jobs to the API.
const BASE_URL = import.meta.env.VITE_API_BASE ?? "";

let currentUserId: string | null = null;

export function setCurrentUser(userId: string | null) {
  currentUserId = userId;
}

function authHeaders(): Record<string, string> {
  return currentUserId ? { "X-User-Id": currentUserId } : {};
}

async function ensureOk(response: Response): Promise<Response> {
  if (!response.ok) {
    throw new Error(`Request failed with status ${response.status}`);
  }
  return response;
}

export async function login(username: string, password: string): Promise<SessionUser> {
  const response = await fetch(`${BASE_URL}/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
  });
  if (response.status === 401) {
    throw new Error("Unknown username");
  }
  return (await ensureOk(response)).json();
}

export async function getJobs(): Promise<JobSummary[]> {
  const response = await fetch(`${BASE_URL}/jobs`, { headers: authHeaders() });
  return (await ensureOk(response)).json();
}

export async function getMessages(jobId: string): Promise<Message[]> {
  const response = await fetch(`${BASE_URL}/jobs/${jobId}/messages`, {
    headers: authHeaders(),
  });
  return (await ensureOk(response)).json();
}

export async function postMessage(jobId: string, body: string): Promise<Message> {
  const response = await fetch(`${BASE_URL}/jobs/${jobId}/messages`, {
    method: "POST",
    headers: { "Content-Type": "application/json", ...authHeaders() },
    body: JSON.stringify({ body }),
  });
  return (await ensureOk(response)).json();
}
