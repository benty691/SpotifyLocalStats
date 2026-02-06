
import { Route, Routes } from 'react-router-dom';
import Home from './pages/Home.tsx';
import Stats from './pages/Stats.tsx';
import Artists from './pages/Artists.tsx';
import Tracks from './pages/Tracks.tsx';
import Albums from './pages/Albums.tsx';

function App() 
{
  return(
    <>
    <Routes>
      <Route path= '/' element={<Home/>}></Route>\
      <Route path= '/Stats' element={<Stats/>}></Route>
      <Route path= '/Artists' element={<Artists/>}></Route>
      <Route path= '/Tracks' element={<Tracks/>}></Route>
      <Route path= '/Albums' element={<Albums/>}></Route>
    </Routes>
    </>
  );
}

export default App;