import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";

type ProfileCardProps = {
  name: string;
  email: string;
  onSignOut?: () => void;
};

function getInitials(name: string): string {
  const parts = name.trim().split(/\s+/).filter(Boolean);
  const first = parts[0];
  if (!first) return "";
  if (parts.length === 1) return first.slice(0, 2).toUpperCase();
  const last = parts[parts.length - 1] ?? first;
  return (first.charAt(0) + last.charAt(0)).toUpperCase();
}

export default function ProfileCard({ name, email, onSignOut }: ProfileCardProps) {
  return (
    <Card className="gap-5 rounded-2xl p-7">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Avatar className="size-16">
            <AvatarFallback className="bg-primary text-lg font-semibold text-primary-foreground">
              {getInitials(name)}
            </AvatarFallback>
          </Avatar>
          <div className="flex flex-col gap-0.5">
            <span className="text-lg font-semibold text-foreground">{name}</span>
            <span className="text-sm text-muted-foreground">{email}</span>
          </div>
        </div>
        <Badge
          variant="secondary"
          className="border-transparent bg-emerald-50 px-2.5 py-1 text-emerald-700"
        >
          Signed in
        </Badge>
      </div>
      <hr className="border-border" />
      <div className="flex items-center justify-end">
        <Button variant="outline" size="sm" onClick={onSignOut}>
          Sign out
        </Button>
      </div>
    </Card>
  );
}

export function ProfileSkeleton() {
  return (
    <div className="flex animate-pulse flex-col gap-5 rounded-2xl border border-slate-200 bg-white p-7 shadow-sm">
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <div className="size-16 rounded-full bg-slate-100" />
          <div className="flex flex-col gap-2">
            <div className="h-4 w-32 rounded bg-slate-100" />
            <div className="h-3 w-40 rounded bg-slate-100" />
          </div>
        </div>
        <div className="h-6 w-20 rounded-full bg-slate-100" />
      </div>
      <hr className="border-slate-200" />
      <div className="flex items-center justify-between">
        <div className="h-4 w-48 rounded bg-slate-100" />
        <div className="h-9 w-24 rounded-lg bg-slate-100" />
      </div>
    </div>
  );
}
