import { Route, Routes } from "react-router";
import Navbar from "./components/Navbar.tsx";
import Home from "./pages/Home.tsx";
import Stats from "./pages/Stats.tsx";
import Artists from "./pages/Artists.tsx";
import Tracks from "./pages/Tracks.tsx";
import Albums from "./pages/Albums.tsx";
import Footer from "./components/Footer.tsx";
import { UserProvider } from "./contexts/UserContexts.tsx";

function App() {
  return (
    <main className="main-content">
      <UserProvider>
        <Navbar />
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/Stats" element={<Stats />} />
          <Route path="/Artists" element={<Artists />} />
          <Route path="/Tracks" element={<Tracks />} />
          <Route path="/Albums" element={<Albums />} />
        </Routes>
        <Footer />
      </UserProvider>
    </main>
  );
}

export default App;
