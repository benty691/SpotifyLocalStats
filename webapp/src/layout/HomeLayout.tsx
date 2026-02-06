import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

// so do we, in layot, build the layoput of a page, then import this layoput to the page? going to try... 
import Footer from '../components/Footer';
import Navbar from '../components/Navbar';

function HomeLayout()
{ return (
    <>
        <Navbar></Navbar> 
        <Container fluid= "md">
              <Row>
                <Col sm={8}>here we want HowTo</Col>
                <Col sm={4}>Here we want trackUploader</Col>
            </Row>

        </Container>
        <Footer></Footer>
    </>
);
}

export default HomeLayout;