import { Route, Routes } from "react-router";
import Navbar from "./components/LayoutComponents/Navbar.tsx";
import Home from "./pages/Home.tsx";
import Stats from "./pages/Stats.tsx";
import Artists from "./pages/Artists.tsx";
import Tracks from "./pages/Tracks.tsx";
import Albums from "./pages/Albums.tsx";
import Footer from "./components/LayoutComponents/Footer.tsx";
import { UserProvider } from "./contexts/UserContexts.tsx";
import { ThemeProvider } from "./contexts/ThemeContext.tsx";
import Upload from "./pages/Upload.tsx";

function App() {
  return (
    <main className='min-h-screen bg-background text-text-primary'>
      <UserProvider>
        <ThemeProvider>
          <Navbar />
          <Routes>
            <Route path='/' element={<Home />} />
            <Route path='/Stats' element={<Stats />} />
            <Route path='/Artists' element={<Artists />} />
            <Route path='/Tracks' element={<Tracks />} />
            <Route path='/Albums' element={<Albums />} />
            <Route path='/Upload' element={<Upload />} />
          </Routes>
          <Footer />
        </ThemeProvider>
      </UserProvider>
    </main>
  );
}

export default App;
