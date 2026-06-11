import Container from "@/components/Container";

const footerLinks = [
  { label: "Terms", href: "/terms" },
  { label: "Privacy", href: "/privacy" },
  { label: "Help", href: "/help" },
];

export const Footer = () => {
  return (
    <footer className="border-t border-slate-200 bg-white">
      <Container className="flex h-16 items-center justify-between text-xs text-slate-500">
        <span>© 2026 StayNGo</span>

        <nav aria-label="Footer navigation">
          <ul className="flex items-center gap-3">
            {footerLinks.map(({ label, href }, index) => (
              <li key={label} className="flex items-center gap-3">
                {index > 0 && (
                  <span
                    aria-hidden="true"
                    className="size-1 rounded-full bg-slate-300"
                  />
                )}

                <a
                  href={href}
                  className="transition-colors hover:text-slate-900"
                >
                  {label}
                </a>
              </li>
            ))}
          </ul>
        </nav>
      </Container>
    </footer>
  );
};