import { ClerkWithRouter } from "@/components/ClerkWithRouter";
import { createQueryClient } from "@/lib/createQueryClient.ts";
import { QueryClientProvider } from "@tanstack/react-query";
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import App from "./App.tsx";
import "./index.css";

const queryClient = createQueryClient();

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <BrowserRouter>
      <QueryClientProvider client={queryClient}>
        <ClerkWithRouter>
          <App />
        </ClerkWithRouter>
      </QueryClientProvider>
    </BrowserRouter>
  </StrictMode>,
);
