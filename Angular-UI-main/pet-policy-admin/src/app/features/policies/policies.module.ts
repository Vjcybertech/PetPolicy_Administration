import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PoliciesListComponent } from './policies-list/policies-list.component';
import { PolicyDetailComponent } from './policy-detail/policy-detail.component';
import { RenewPolicyComponent } from './renew-policy/renew-policy.component';
import { CancelPolicyComponent } from './cancel-policy/cancel-policy.component';

const routes: Routes = [
  { path: '', component: PoliciesListComponent },
  { path: ':policyNumber', component: PolicyDetailComponent },
  { path: ':policyNumber/renew', component: RenewPolicyComponent },
  { path: ':policyNumber/cancel', component: CancelPolicyComponent },
];

@NgModule({
  declarations: [
    PoliciesListComponent,
    PolicyDetailComponent,
    RenewPolicyComponent,
    CancelPolicyComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes)
  ]
})
export class PoliciesModule {}
