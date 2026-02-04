// import navbar buttons (tutorial, artists etc) 
import NavBarButton from './NavbarButton.tsx';

function Navbar () 
{
    const buttons = [
        'Tutorial',
        'Artists',
        'Albums',
        'Tracks',
        'Stats'
    ]

    return (
        <nav>
            <ul className="list-group">
                {buttons.map((button) => (<li key={button}><button className='btn btn-primary'>{button}</button></li>))}    
            </ul>
        </nav>
    );
}

export default Navbar;