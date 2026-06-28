import { useState, type ReactNode } from "react";
import { useNavigate } from "react-router-dom";

import Container from "@/components/Container";
import StatusBadge from "@/components/StatusBadge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Text } from "@/components/ui/text";
import type { UpsertListingRequest } from "@/features/listings/types";
import { useCreateListing, useListingActions } from "@/features/listings/useListings";

function Field({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div className="flex flex-col gap-1.5">
      <Text variant="label">{label}</Text>
      {children}
    </div>
  );
}

export default function CreateListingPage() {
  const navigate = useNavigate();
  const create = useCreateListing();
  const { publish } = useListingActions();

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [location, setLocation] = useState("");
  const [timeZone, setTimeZone] = useState("Europe/Lisbon");
  const [capacity, setCapacity] = useState("");
  const [price, setPrice] = useState("");
  const [currency, setCurrency] = useState("USD");
  const [mainImageUrl, setMainImageUrl] = useState("");
  const [error, setError] = useState<string | null>(null);

  const busy = create.isPending || publish.isPending;

  const buildRequest = (): UpsertListingRequest => ({
    title,
    description,
    location,
    timeZone,
    mainImageUrl,
    imageUrls: mainImageUrl ? [mainImageUrl] : [],
    price: { amountCents: Math.round(Number(price) * 100), currency: currency.toUpperCase() },
    capacity: Number(capacity),
  });

  const save = async (alsoPublish: boolean) => {
    setError(null);
    try {
      const created = await create.mutateAsync(buildRequest());
      if (alsoPublish) await publish.mutateAsync(created.id);
      navigate("/host/listings");
    } catch {
      setError(
        alsoPublish
          ? "Couldn’t publish — make sure every field is filled (title, description, location, price, capacity, photo)."
          : "Couldn’t save the listing. Please check the fields and try again.",
      );
    }
  };

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
          <Input value={title} onChange={(e) => setTitle(e.target.value)} placeholder="Sunlit loft in the old town" />
        </Field>

        <Field label="Description">
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            className="min-h-24 rounded-md border border-input bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            placeholder="A light-filled loft in the heart of the old town…"
          />
        </Field>

        <Field label="Main photo URL">
          <Input
            value={mainImageUrl}
            onChange={(e) => setMainImageUrl(e.target.value)}
            placeholder="https://…/photo.jpg"
          />
        </Field>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <Field label="Address">
            <Input value={location} onChange={(e) => setLocation(e.target.value)} placeholder="Rua da Bica 12, Lisbon" />
          </Field>
          <Field label="Timezone (IANA)">
            <Input value={timeZone} onChange={(e) => setTimeZone(e.target.value)} placeholder="Europe/Lisbon" />
          </Field>
        </div>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
          <Field label="Capacity">
            <Input
              type="number"
              min={1}
              value={capacity}
              onChange={(e) => setCapacity(e.target.value)}
              placeholder="4"
            />
          </Field>
          <Field label="Price / night">
            <Input
              type="number"
              min={0}
              value={price}
              onChange={(e) => setPrice(e.target.value)}
              placeholder="120"
            />
          </Field>
          <Field label="Currency">
            <Input value={currency} onChange={(e) => setCurrency(e.target.value)} maxLength={3} placeholder="USD" />
          </Field>
        </div>

        {error && (
          <Text variant="small" className="text-destructive">
            {error}
          </Text>
        )}

        <div className="flex justify-end gap-3">
          <Button variant="outline" disabled={busy} onClick={() => save(false)}>
            Save draft
          </Button>
          <Button disabled={busy} onClick={() => save(true)}>
            {busy ? "Saving…" : "Publish listing"}
          </Button>
        </div>
      </div>
    </Container>
  );
}
