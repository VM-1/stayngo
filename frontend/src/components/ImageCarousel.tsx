import { ChevronLeft, ChevronRight } from "lucide-react";
import { useRef, useState } from "react";

import { cn } from "@/lib/utils";

/** Dependency-free image slider (CSS scroll-snap) with prev/next controls and dots. */
export function ImageCarousel({
  images,
  alt,
  className,
}: {
  images: string[];
  alt: string;
  className?: string;
}) {
  const trackRef = useRef<HTMLDivElement>(null);
  const [index, setIndex] = useState(0);

  if (images.length === 0) {
    return <div className={cn("bg-gradient-to-br from-primary/70 to-primary", className)} />;
  }

  const scrollTo = (i: number) => {
    const clamped = Math.max(0, Math.min(images.length - 1, i));
    trackRef.current?.scrollTo({ left: clamped * trackRef.current.clientWidth, behavior: "smooth" });
    setIndex(clamped);
  };

  const onScroll = () => {
    const track = trackRef.current;
    if (track) setIndex(Math.round(track.scrollLeft / track.clientWidth));
  };

  return (
    <div className={cn("group relative overflow-hidden rounded-xl", className)}>
      <div
        ref={trackRef}
        onScroll={onScroll}
        className="flex size-full snap-x snap-mandatory overflow-x-auto [scrollbar-width:none] [&::-webkit-scrollbar]:hidden"
      >
        {images.map((src, i) => (
          <img
            key={i}
            src={src}
            alt={`${alt} — image ${i + 1}`}
            className="size-full shrink-0 snap-center object-cover"
          />
        ))}
      </div>

      {images.length > 1 && (
        <>
          <button
            type="button"
            aria-label="Previous image"
            onClick={() => scrollTo(index - 1)}
            disabled={index === 0}
            className="absolute left-3 top-1/2 flex size-9 -translate-y-1/2 items-center justify-center rounded-full bg-background/90 text-foreground shadow opacity-0 transition group-hover:opacity-100 disabled:cursor-default disabled:opacity-0"
          >
            <ChevronLeft className="size-5" />
          </button>
          <button
            type="button"
            aria-label="Next image"
            onClick={() => scrollTo(index + 1)}
            disabled={index === images.length - 1}
            className="absolute right-3 top-1/2 flex size-9 -translate-y-1/2 items-center justify-center rounded-full bg-background/90 text-foreground shadow opacity-0 transition group-hover:opacity-100 disabled:cursor-default disabled:opacity-0"
          >
            <ChevronRight className="size-5" />
          </button>
          <div className="absolute bottom-3 left-1/2 flex -translate-x-1/2 gap-1.5">
            {images.map((_, i) => (
              <span
                key={i}
                className={cn("size-1.5 rounded-full bg-background/60", i === index && "bg-background")}
              />
            ))}
          </div>
        </>
      )}
    </div>
  );
}

export default ImageCarousel;
