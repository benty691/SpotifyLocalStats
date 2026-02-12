import { useState } from "react";
import { userApi } from "../services/api/userApi";
import ReactDOM from "react-dom";
import { useUserContext } from "../contexts/UserContexts";

function LoginPopup() {
  const [userName, setUserName] = useState<string>();
  const [userFirstName, setUserFirstName] = useState<string>();
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
    } finally {
      setLoading(false);
    }

    // post the users username and firstname to backend, create userand return the user to set user context.
  };

  return ReactDOM.createPortal(
    <div className='flex size-full'>
      <div className='login-grid'>
        <h3 className='login-header'>
          Please Create Account for your convenience:
        </h3>
        <form action='' className='login-form' onSubmit={handleSubmit}>
          <label htmlFor='username'>Username:</label>
          <input
            type='text'
            className='login-input'
            placeholder='Please choose your username'
            onChange={(e) => setUserName(e.target.value)}
          />
          <label htmlFor='firstName'>First Name:</label>
          <input
            type='text'
            className='login-input'
            placeholder='Please enter your first name'
            onChange={(e) => setUserFirstName(e.target.value)}
          />
          <button type='submit' className='submit-btn'>
            Submit Form
          </button>
        </form>
      </div>
    </div>,
    document.body,
  );
}

export default LoginPopup;
