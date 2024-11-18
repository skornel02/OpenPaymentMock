import {RouteObject} from "react-router-dom";
import IndexPage from "@/pages";
import Layout from "@/layout.tsx";

export const routes: RouteObject[] = [
    {
        path: "/",
        element: <Layout/>,
        id: '#Home',
        children: [
            {
                path: "/",
                element: <IndexPage/>,
            },
        ]
    }
]
