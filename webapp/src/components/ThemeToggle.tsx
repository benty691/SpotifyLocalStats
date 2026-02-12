import { useState } from "react";
import { useThemeContext } from "../contexts/ThemeContext";

function ThemeToggle() {
  const { theme, toggleTheme } = useThemeContext();

  return (
    <div>
      <div>
        {theme === "dark" ? (
          <button onClick={toggleTheme}>LM</button>
        ) : (
          <button onClick={toggleTheme}>DM</button>
        )}
      </div>
    </div>
  );
}

export default ThemeToggle;
