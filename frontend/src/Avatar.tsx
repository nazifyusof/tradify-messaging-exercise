import { avatarColour, initials } from "./format";

export function Avatar({ name, seed, size = 40 }: { name: string; seed: string; size?: number }) {
  return (
    <span
      className="avatar"
      style={{ background: avatarColour(seed), width: size, height: size, fontSize: size * 0.4 }}
      aria-hidden="true"
    >
      {initials(name)}
    </span>
  );
}
