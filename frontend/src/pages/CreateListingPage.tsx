import type { ReactNode } from "react";

import Container from "@/components/Container";
import { Text } from "@/components/ui/text";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import StatusBadge from "@/components/StatusBadge";
import { ChevronDown, Plus } from "lucide-react";

function Field({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div className="flex flex-col gap-1.5">
      <Text variant="label">{label}</Text>
      {children}
    </div>
  );
}

function SelectBox({ value }: { value: string }) {
  return (
    <div className="flex h-10 items-center justify-between rounded-md border border-input bg-background px-3">
      <Text variant="small">{value}</Text>
      <ChevronDown className="size-4 text-muted-foreground" />
    </div>
  );
}

export default function CreateListingPage() {
  return (
    <Container className="py-12">
      <div className="mx-auto flex max-w-xl flex-col gap-6">
        <div className="flex items-center gap-3">
          <Text variant="h2" as="h1">
            New listing
          </Text>
          <StatusBadge status="Draft" />
        </div>

        <Field label="Title">
          <Input placeholder="Sunlit loft in the old town" />
        </Field>

        <Field label="Description">
          <textarea
            className="min-h-24 rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            placeholder="A light-filled loft in the heart of the old town…"
          />
        </Field>

        <Field label="Photos">
          <div className="flex flex-col items-center justify-center gap-2 rounded-lg border border-dashed border-border bg-muted py-8 text-center">
            <Plus className="size-6 text-muted-foreground" />
            <Text variant="small" tone="muted">
              Drag photos here or click to upload
            </Text>
          </div>
        </Field>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <Field label="Address">
            <Input placeholder="Rua da Bica 12, Lisbon" />
          </Field>
          <Field label="Timezone">
            <SelectBox value="Europe/Lisbon" />
          </Field>
        </div>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <Field label="Capacity">
            <SelectBox value="4 guests" />
          </Field>
          <Field label="Price per night">
            <Input placeholder="$120" />
          </Field>
        </div>

        <div className="flex justify-end gap-3">
          <Button variant="outline">Save draft</Button>
          <Button>Publish listing</Button>
        </div>
      </div>
    </Container>
  );
}
