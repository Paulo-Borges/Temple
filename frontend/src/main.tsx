import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";

const elementoRaiz = document.getElementById("root");

// Sem o "!": se o index.html perder a <div id="root">, o erro diz exatamente o que faltou.
if (elementoRaiz === null) {
  throw new Error("Elemento #root não encontrado no index.html.");
}

createRoot(elementoRaiz).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
