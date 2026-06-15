import { cva, type VariantProps } from "class-variance-authority"
import { Slot } from "radix-ui"
import * as React from "react"

import { cn } from "@/lib/utils"

const textVariants = cva("", {
  variants: {
    // Typographic scale (size / weight / leading / tracking). Mirrors the Figma type ramp.
    variant: {
      display: "text-5xl font-bold tracking-tight text-balance md:text-6xl",
      h1: "text-4xl font-bold tracking-tight text-balance",
      h2: "text-2xl font-semibold tracking-tight text-balance",
      h3: "text-xl font-semibold",
      h4: "text-base font-semibold",
      lead: "text-lg leading-relaxed",
      body: "text-base leading-relaxed",
      small: "text-sm",
      label: "text-sm font-medium",
      caption: "text-xs",
      mono: "font-mono text-xs",
    },
    // Color, decoupled from size so any variant can take any tone.
    tone: {
      default: "text-foreground",
      muted: "text-muted-foreground",
      primary: "text-primary",
      secondary: "text-secondary-foreground",
      destructive: "text-destructive",
      inverted: "text-primary-foreground", // for text on primary/dark surfaces
    },
  },
  defaultVariants: {
    variant: "body",
    tone: "default",
  },
})

// Semantic default element per variant (overridable via `as`).
const variantElement: Record<string, React.ElementType> = {
  display: "h1",
  h1: "h1",
  h2: "h2",
  h3: "h3",
  h4: "h4",
  lead: "p",
  body: "p",
  small: "p",
  label: "span",
  caption: "span",
  mono: "span",
}

type TextProps = React.HTMLAttributes<HTMLElement> &
  VariantProps<typeof textVariants> & {
    as?: React.ElementType
    asChild?: boolean
  }

function Text({ className, variant, tone, as, asChild = false, ...props }: TextProps) {
  const Comp = asChild ? Slot.Root : as ?? variantElement[variant ?? "body"] ?? "p"

  return (
    <Comp
      data-slot="text"
      data-variant={variant}
      className={cn(textVariants({ variant, tone, className }))}
      {...props}
    />
  )
}

export { Text, textVariants }
