import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarProvider,
  SidebarTrigger,
} from '@/components/ui/sidebar';
import { Link, Outlet, useMatches } from 'react-router-dom';
import { useMemo, useState } from 'react';
import { ChevronsLeftRightEllipsis, CreditCard, Home, Users } from 'lucide-react';
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbList,
  BreadcrumbSeparator,
} from '@/components/ui/breadcrumb.tsx';
import { AuthenticationProvider } from '@/contexts/authentication-context';

export default function AdminLayout() {
  const [apiKey, setApiKey] = useState<string>("SecretAdminApiKey");
  const [authenticated,] = useState<boolean>(true);

  const [isSidebarOpen, setIsSidebarOpen] = useState(true);

  const menus = useMemo(
    () => [
      {
        title: 'Partners',
        url: 'partners',
        icon: Users,
      },
      {
        title: 'Payments',
        url: 'payments',
        icon: CreditCard,
      },
    ],
    [],
  );

  const path = useMatches();

  console.log(path);

  return (
    <AuthenticationProvider value={{apiKey}}>
      <SidebarProvider open={isSidebarOpen} onOpenChange={setIsSidebarOpen}>
        <Sidebar collapsible="icon">
          <SidebarHeader />
          <SidebarContent>
            <SidebarGroup>
              <SidebarGroupContent>
                <SidebarMenu>
                  <SidebarMenuItem>
                    <SidebarMenuButton asChild>
                      <Link to="/admin">
                        <Home />
                        Admin
                      </Link>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                  <SidebarMenuItem>
                    <SidebarMenuButton asChild>
                      <Link to="/swagger/index.html" reloadDocument>
                        <ChevronsLeftRightEllipsis />
                        Swagger
                      </Link>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                </SidebarMenu>
              </SidebarGroupContent>
            </SidebarGroup>
            <SidebarGroup>
              <SidebarGroupLabel>Resources</SidebarGroupLabel>
              <SidebarGroupContent>
                <SidebarMenu>
                  {menus.map((menu, index) => (
                    <SidebarMenuItem key={`simulation-${index}`}>
                      <SidebarMenuButton asChild>
                        <Link to={menu.url}>
                          <menu.icon />
                          <span>{menu.title}</span>
                        </Link>
                      </SidebarMenuButton>
                    </SidebarMenuItem>
                  ))}
                </SidebarMenu>
              </SidebarGroupContent>
            </SidebarGroup>
          </SidebarContent>
          <SidebarFooter>
            {isSidebarOpen && (
              <SidebarContent>
                <p className="text-center">OpenPaymentMock © 2024</p>
              </SidebarContent>
            )}
          </SidebarFooter>
        </Sidebar>
        <main className="min-h-[100vh] w-full">
          <div className="flex justify-start mt-2 mb-4">
            <SidebarTrigger className="mx-2" />
            <Breadcrumb className="flex align-center">
              <BreadcrumbList>
                {path
                  .filter((match) => match.id.startsWith('#'))
                  .map((match, index) => (
                    <>
                      {match.pathname !== '/' && (
                        <BreadcrumbSeparator>
                          <BreadcrumbSeparator />
                        </BreadcrumbSeparator>
                      )}
                      <BreadcrumbItem key={`breadcrumb-${index}`}>
                        <Link to={match.pathname}>{match.id.substring(1)}</Link>
                      </BreadcrumbItem>
                    </>
                  ))}
              </BreadcrumbList>
            </Breadcrumb>
          </div>

          {authenticated ? (
            <Outlet />
          ) : (
            <div className="flex h-screen items-center justify-center">
              <input
                type="text"
                value={apiKey}
                onChange={(e) => setApiKey(e.target.value)}
                placeholder="Enter your API key"
                className="p-2 border border-gray-300 rounded-lg"
              />
            </div>
          )}
        </main>
      </SidebarProvider>
    </AuthenticationProvider>
  );
}
