import createClient from "openapi-fetch";
import { paths } from "./openpaymentmock-backend";

export const client = createClient<paths>();
