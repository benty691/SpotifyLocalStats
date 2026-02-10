import axios from "axios";
import React from "react";

export const apiClient = axios.create({
  baseURL: import.meta.env.REACT_APP_API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});
