import { createContext, useContext } from "react";

export type AuthenticationContextType = {
    apiKey: string;
};

export const AuthenticationContext = createContext<AuthenticationContextType>({
    apiKey: "",
});

export const AuthenticationProvider = AuthenticationContext.Provider;

export function useAuthentication() {
    return useContext(AuthenticationContext);
}