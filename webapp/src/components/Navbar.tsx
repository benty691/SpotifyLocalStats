// import navbar buttons (tutorial, artists etc) 
import NavBarButton from './NavbarButton.tsx';
import Container from 'react-bootstrap/Container';
import NavDropdown from 'react-bootstrap/NavDropdown';
import Navbar from 'react-bootstrap/Navbar';
import Nav from 'react-bootstrap/Nav';


function NavBarMenu () 
{ 
    const buttonList = ['Tutorial', 'Artists', 'Albums', 'Tracks', 'Stats']
    
    // need to pass in base url (defnied as loacalhost essentially)
    // /probably set up a fetch class that we delegate every request tot he backend to, even a router? 
    
    return (
    <>
        <Navbar expand= "lg" className='bg-body-tertiary'>
            <Container>
                <Navbar.Brand href='#home'>SpotifyStatsLocal</Navbar.Brand>
                <Navbar.Toggle aria-controls="basic-navbar-nav" />
                <Navbar.Collapse id="basic-navbar-nav">
                <Nav className="me-auto">
                    <Nav.Link href="#home">Artists</Nav.Link>
                    <Nav.Link href="#Albums">Albums</Nav.Link>
                    <Nav.Link href="#Tracks">Tracks</Nav.Link>
                    <Nav.Link href="#Stats">Stats</Nav.Link>
                </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    </>
    );
}

export default NavBarMenu;