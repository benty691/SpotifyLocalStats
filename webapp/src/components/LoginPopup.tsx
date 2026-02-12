import { useState } from "react";
import { userApi } from "../services/api/userApi";
import ReactDOM from "react-dom";
import { useUserContext } from "../contexts/UserContexts";
import type { ApiError } from "../types/ApiError";

function LoginPopup() {
  const [userName, setUserName] = useState<string>();
  const [userFirstName, setUserFirstName] = useState<string>();
  const [error, setError] = useState<string>();
  const [loading, setLoading] = useState<boolean>();

  const { updateUser } = useUserContext();

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!userName?.trim()) return;
    if (!userFirstName?.trim()) return;

    setLoading(true);

    if (loading) return;

    try {
      const res = await userApi.createNewUser(userName, userFirstName);
      updateUser(res.data);
    } catch (e) {
      setError("Error creating user");
    } finally {
      setLoading(false);
    }

    // post the users username and firstname to backend, create userand return the user to set user context.
  };

  return (
    <div className='flex size-full fixed z-1 left-0 top-0 overflow-auto bg-blur '>
      <div className='bg-primary absolute translate-1/2 p-5 rounded-md w-2/5 blur-none'>
        <h3 className='mt-0 text-3xl'>Please create an account</h3>
        <form action='' className='login-form' onSubmit={handleSubmit}>
          <label className='block mb-2.5'>Username:</label>
          <input
            id='user_name'
            name='user_name'
            type='text'
            className='w-full p-1.25 rounded-md border border-primary mb-2.5'
            placeholder='Please choose your username'
            onChange={(e) => setUserName(e.target.value)}
          />
          <label>First Name:</label>
          <input
            id='first_name'
            name='first_name'
            type='text'
            className='w-full p-1.25 rounded-md border border-primary mb-2.5'
            placeholder='Please enter your first name'
            onChange={(e) => setUserFirstName(e.target.value)}
          />
          <button
            type='submit'
            className='bg-tertiary text-text-primary border-0 p-2 no-underline inline-block text-xl mr-2.5 mt-2.5 cursor-pointer rounded-md'
          >
            Submit Form
          </button>
        </form>
      </div>
    </div>
  );
}

export default LoginPopup;
