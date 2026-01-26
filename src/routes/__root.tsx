import { createRootRoute, Outlet } from '@tanstack/react-router'
import { TanStackRouterDevtools } from '@tanstack/router-devtools'
import { SidebarProvider, SidebarTrigger } from "@/shadcn/sidebar"

export const Route = createRootRoute({
  component: () => (
    <SidebarProvider>
      <div className="flex min-h-screen w-full">
        
        
        <main className="flex-1 w-full">
          <div className="p-4 border-b">
             <SidebarTrigger />
          </div>
          <div className="p-4">
            <Outlet /> 
          </div>
        </main>
      </div>
      <TanStackRouterDevtools />
    </SidebarProvider>
  ),
})