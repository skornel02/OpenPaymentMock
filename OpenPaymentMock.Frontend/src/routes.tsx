import {RouteObject} from "react-router-dom";
import IndexPage from "@/pages";
import AdminIndexPage from "./pages/admin/admin-index";
import AdminLayout from "@/layouts/admin-layout";
import PartnersPage from "./pages/admin/partners/partners";
import PaymentsPage from "./pages/admin/payments/payments";

export const routes: RouteObject[] = [
    {
        path: "/",
        element: <IndexPage/>
    },
    {
        path: "/admin",
        element: <AdminLayout/>,
        id: '#Home',
        children: [
            {
                path: "/admin",
                element: <AdminIndexPage/>,
            },
            {
                path: "/admin/partners",
                element: <PartnersPage />
            },
            {
                path: "/admin/payments",
                element: <PaymentsPage />
            }
        ]
    },
    {
        path: "/payments/:paymentId",
    }
]
