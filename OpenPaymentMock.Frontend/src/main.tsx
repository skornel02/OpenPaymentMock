import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { routes } from '@/routes.tsx';
import { createTheme, MantineProvider } from '@mantine/core';
import { Toaster } from './components/ui/toaster';
// @ts-expect-error array.prototype.tosorted
import toSorted from 'array.prototype.tosorted';
import './index.css';
import '@mantine/core/styles.css';
import '@mantine/dates/styles.css';
import 'mantine-react-table/styles.css';

toSorted.shim();

const router = createBrowserRouter(routes);

const theme = createTheme({
  /** Put your mantine theme override here */
});

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <MantineProvider theme={theme}>
      <RouterProvider router={router} />
    </MantineProvider>
    <Toaster />
  </StrictMode>,
);
