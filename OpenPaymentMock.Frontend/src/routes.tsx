import {RouteObject} from "react-router-dom";
import IndexPage from "@/pages";
import AdminLayout from "@/layouts/admin-layout";
import PaymentPage from "./pages/payment/payment";

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
                // element: <AdminIndexPage/>,
                lazy: async () => {
                    const AdminIndexPage = (await import('./pages/admin/admin-index')).default;
                    return {Component: AdminIndexPage}
                }
            },
            {
                path: "/admin/partners",
                lazy: async () => {
                    const PartnersPage = (await import('./pages/admin/partners/partners')).default;
                    return {Component: PartnersPage}
                }
            },
            {
                path: "/admin/payments",
                lazy: async () => {
                    const PaymentsPage = (await import('./pages/admin/payments/payments')).default;
                    return {Component: PaymentsPage}
                }
            }
        ]
    },
    {
        path: "/payments/:paymentId",
        element: <PaymentPage />
    }
]
