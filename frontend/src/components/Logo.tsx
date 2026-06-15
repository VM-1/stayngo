import { Link } from "react-router-dom";
import logoMark from "@/assets/logo.svg";
import { Text } from "@/components/ui/text";
import { cn } from "@/lib/utils";

type LogoProps = {
  /** Render only the mark (no wordmark) — useful for tight/mobile spots. */
  markOnly?: boolean;
  className?: string;
};

/** StayNGo brand lockup: exported SVG mark + wordmark, links home. */
export function Logo({ markOnly = false, className }: LogoProps) {
  return (
    <Link
      to="/"
      aria-label="StayNGo home"
      className={cn("flex items-center gap-2", className)}
    >
      <img src={logoMark} alt="" aria-hidden className="size-8" />
      {!markOnly && (
        <Text variant="h3" as="span" className="tracking-tight">
          StayNGo
        </Text>
      )}
    </Link>
  );
}

export default Logo;
