import Container from "react-bootstrap/Container";
import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col";
import StatsCard from "./StatsCard";

function HomeStats() {
  return (
    <Container fluid='md'>
      <Row>
        <Col sm={8}>
          <div className='header'>
            <h2>Welcome Back, user</h2>
          </div>
          <div className='home-content'>
            <StatsCard />
            <StatsCard />
            <StatsCard />
            <StatsCard />
          </div>
        </Col>
      </Row>
    </Container>
  );
}

export default HomeStats;
