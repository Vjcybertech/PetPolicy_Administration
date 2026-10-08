import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    loadChildren: () => import('./features/dashboard/dashboard.module').then(m => m.DashboardModule)
  },
  {
    path: 'policies',
    loadChildren: () => import('./features/policies/policies.module').then(m => m.PoliciesModule)
  },
  {
    path: 'quotes',
    loadChildren: () => import('./features/quotes/quotes.module').then(m => m.QuotesModule)
  },
  {
    path: 'create-policy',
    loadChildren: () => import('./features/create-policy/create-policy.module').then(m => m.CreatePolicyModule)
  },
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
