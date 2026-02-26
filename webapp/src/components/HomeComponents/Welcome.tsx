import { useUserContext } from "../../contexts/UserContexts";

function Welcome() {
  const { user } = useUserContext();

  return (
    <>
      <h2 className='text-5xl font-semibold tracking-tight pt-2 pb-10 text-center'>
        Welcome back,
        <strong className='text-accent-cyan'> {user?.userName} </strong>
      </h2>
    </>
  );
}

export default Welcome;
