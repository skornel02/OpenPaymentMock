import createClient from "openapi-fetch";
import { paths } from "./openpaymentmock-backend";
import {
  createQueryHook,
} from "swr-openapi";

export const client = createClient<paths>();

export const useBackendQuery = createQueryHook(client, 'openpaymentmock');