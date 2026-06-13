const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "";

export class ApiError extends Error {
  status: number;

  constructor(status: number, message: string) {
    super(message);
    this.status = status;
    this.name = "ApiError";
  }
}

type ApiOptions = RequestInit & { token?: string | null };

export async function apiFetch<T>(
  path: string,
  { token, headers, ...init }: ApiOptions = {},
): Promise<T> {
  const res = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...headers,
    },
  });

  if (!res.ok) {
    throw new ApiError(res.status, `${init.method ?? "GET"} ${path} → ${res.status}`);
  }
  return res.status === 204 ? (undefined as T) : ((await res.json()) as T);
}