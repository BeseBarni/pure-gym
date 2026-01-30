import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import { RouterProvider, createRouter } from '@tanstack/react-router'
import { queryClient } from '../lib/query-client'
import { router } from '../lib/router'

export default function AppProvider() {
  return (
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
      
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  )
}