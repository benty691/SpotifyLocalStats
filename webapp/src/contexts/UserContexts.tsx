import { createContext, useState, useContext, useEffect } from "react";
import type { User } from "../types/User";
import React from "react";
import { apiClient } from "../services/api/apiClient";
import { userApi } from "../services/api/userApi";

interface UserContextType {
  user: User | null;
  loading: boolean;
  storeUser: () => void;
  fetchUser: () => Promise<User>;
}

const UserContext = createContext<UserContextType | undefined>(undefined);

export const useUserContext = () => {
  const context = useContext(UserContext);
  if (!context) {
    throw new Error("useUserContext must be used within UserProvider");
  }
  return context;
};
export const UserProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    const storedUser = localStorage.getItem("currentUser");

    if (storedUser) {
      setUser(JSON.parse(storedUser));
    }
    setLoading(false);
  }, []);

  useEffect(() => {
    if (user) localStorage.setItem("currentUser", JSON.stringify(user));
  }, [user]);

  const fetchUser = async (): Promise<User> => {
    try {
      const res = await userApi.getCurrentUser();
      return res.data;
    } catch (e) {
      console.error("Failed to fetch User:", e);
      throw e;
    } finally {
      setLoading(false);
    }
  };

  const storeUser = async () => {
    setUser(await fetchUser());
  };

  const value = {
    user,
    loading,
    storeUser,
    fetchUser,
  };

  return <UserContext.Provider value={value}>{children}</UserContext.Provider>;
};
