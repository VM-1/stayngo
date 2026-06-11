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
  if (parts.length === 0) return "";
  if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase();
  return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
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
      <div className="flex items-center justify-between">
        <p className="text-sm text-muted-foreground">
          Fetched live from GET /identity/me
        </p>
        <Button variant="outline" size="sm" onClick={onSignOut}>
          Sign out
        </Button>
      </div>
    </Card>
  );
}
