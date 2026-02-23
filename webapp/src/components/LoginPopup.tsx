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
    //e.preventDefault();

    if (!userName?.trim()) return;
    if (!userFirstName?.trim()) return;

    if (loading) return;

    try {
      setLoading(true);

      const res = await userApi.createNewUser(userName, userFirstName);
      console.log(import.meta.env.VITE_API_URL);

      updateUser(res.data);
    } catch (e) {
      setError("Error creating user");
    } finally {
      setLoading(false);
    }

    // post the users username and firstname to backend, create userand return the user to set user context.
  };

  return (
    <div className='flex size-full fixed z-1 left-0 top-0 overflow-auto'>
      <div className='absolute inset-0 backdrop-blur-sm ' />
      <div className='relative bg-surface border border-border m-auto p-6 rounded-lg w-2/5 max-w-lg shadow-xl'>
        <h3 className='mt-0 mb-2.5 text-3xl text-text-primary'>
          Please create an account
        </h3>
        <form action='' className='login-form' onSubmit={handleSubmit}>
          <label className='block mb-2.5 text-text-primary'>Username:</label>
          <input
            id='user_name'
            name='user_name'
            type='text'
            className='w-full p-2.5 rounded-md border border-border bg-background text-text-primary mb-2.5 focus:outline-none focus:border-primary'
            placeholder='Please choose your username'
            onChange={(e) => setUserName(e.target.value)}
          />
          <label className='block mb-2.5 text-text-primary'>First Name:</label>
          <input
            id='first_name'
            name='first_name'
            type='text'
            className='w-full p-2.5 rounded-md border border-border bg-background text-text-primary mb-2.5 focus:outline-none focus:border-primary'
            placeholder='Please enter your first name'
            onChange={(e) => setUserFirstName(e.target.value)}
          />
          <button
            type='submit'
            className='bg-primary hover:bg-primary-hover text-white border-0 px-6 py-2.5 inline-block text-base font-medium mt-2.5 cursor-pointer rounded-md transition-colors'
          >
            Submit Form
          </button>
        </form>
      </div>
    </div>
  );
}

export default LoginPopup;
