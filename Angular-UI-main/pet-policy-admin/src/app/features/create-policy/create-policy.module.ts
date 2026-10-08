import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CreatePolicyComponent } from './create-policy.component';

const routes: Routes = [{ path: '', component: CreatePolicyComponent }];

@NgModule({
  declarations: [CreatePolicyComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes)
  ]
})
export class CreatePolicyModule {}
