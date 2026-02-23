import { useUserContext } from "../../contexts/UserContexts";

function Welcome() {
  const { user } = useUserContext();

  return (
    <>
      <h2 className='text-4xl p-4 text-center'>
        Welcome back,
        <strong className='text-accent-cyan'> {user?.userName} </strong>
      </h2>
    </>
  );
}

export default Welcome;
